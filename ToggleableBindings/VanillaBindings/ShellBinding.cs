#nullable enable

using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ToggleableBindings.HKQuickSettings;
using ToggleableBindings.Utility;
using UnityEngine;

namespace ToggleableBindings.VanillaBindings
{
    [VanillaBinding]
    internal sealed class ShellBinding : Binding
    {
        [QuickSetting]
        public static int MaxBoundShellHealth { get; private set; } = 4;

        private const string UpdateBlueHealthEvent = "UPDATE BLUE HEALTH";
        private const string AddBlueHealthEvent = "ADD BLUE HEALTH";
        private const string BlueHealthFsmName = "Blue Health Control";
        private const string BlueHealthFsmWaitState = "Wait";
        private const string BlueHealthFsmLastHpEvent = "LAST HP ADDED";
        private const string CharmIndicatorCheckEvent = "CHARM INDICATOR CHECK";
        private readonly List<IDetour> _detours;
        private static int pendingBlueHealth;
        private static int pendingBlueHealthStart;
        private static bool pendingBlueHealthCoroutine;

        private Sprite? _defaultSprite;
        private Sprite? _selectedSprite;

        public override Sprite DefaultSprite => _defaultSprite = _defaultSprite != null ? _defaultSprite : _defaultSprite = BaseGamePrefabs.ShellButton.UnsafeGameObject.GetComponent<BossDoorChallengeUIBindingButton>().iconImage.sprite;

        public override Sprite SelectedSprite => _selectedSprite = _selectedSprite != null ? _selectedSprite : _selectedSprite = BaseGamePrefabs.ShellButton.UnsafeGameObject.GetComponent<BossDoorChallengeUIBindingButton>().selectedSprite;

        public ShellBinding() : base("Shell")
        {
            var boundShellGetter = typeof(BossSequenceController).GetMethod($"get_BoundShell", BindingFlags.Public | BindingFlags.Static);
            var boundMaxHealthGetter = typeof(BossSequenceController).GetMethod($"get_BoundMaxHealth", BindingFlags.Public | BindingFlags.Static);

            _detours = new(2)
            {
                new Hook(boundShellGetter, new Func<bool>(() => true), TBConstants.HookManualApply),
                new Hook(boundMaxHealthGetter, new Func<int>(BoundMaxHealthOverride), TBConstants.HookManualApply)
            };
        }

        protected override void OnApplied()
        {
            HudEvents.In += HudEvents_In;
            On.PlayerData.SetInt += PlayerData_SetInt;
            On.EventRegister.SendEvent += EventRegister_SendEvent;
            foreach (var detour in _detours)
                detour.Apply();

            CoroutineController.Start(OnToggledCoroutine());
        }

        protected override void OnRestored()
        {
            HudEvents.In -= HudEvents_In;
            On.PlayerData.SetInt -= PlayerData_SetInt;
            On.EventRegister.SendEvent -= EventRegister_SendEvent;
            foreach (var detour in _detours)
                detour.Undo();

            CoroutineController.Start(OnToggledCoroutine());
        }

        private IEnumerator OnToggledCoroutine()
        {
            yield return new WaitWhile(() => HeroController.instance == null);
            yield return null;

            PlayMakerFSM.BroadcastEvent(CharmIndicatorCheckEvent);
            EventRegister.SendEvent(UpdateBlueHealthEvent);

            PlayerData.instance.MaxHealth();
        }

        private static int BoundMaxHealthOverride()
        {
            int maxBoundHealth = MaxBoundShellHealth;

            bool fragileHealthEquipped = PlayerData.instance.equippedCharm_23 && !PlayerData.instance.brokenCharm_23;
            if (fragileHealthEquipped)
                maxBoundHealth += 2;

            return maxBoundHealth;
        }

        private void HudEvents_In()
        {
            PlayMakerFSM.BroadcastEvent(CharmIndicatorCheckEvent);
        }

        private void PlayerData_SetInt(On.PlayerData.orig_SetInt orig, PlayerData self, string intName, int value)
        {
            orig(self, intName, value);

            if (intName == "maxHealth")
                PlayMakerFSM.BroadcastEvent(CharmIndicatorCheckEvent);
        }

        private void EventRegister_SendEvent(On.EventRegister.orig_SendEvent orig, string eventName)
        {
            if (!string.IsNullOrEmpty(eventName) && eventName.StartsWith(AddBlueHealthEvent, StringComparison.Ordinal))
            {
                PlayerData? pd = PlayerData.instance;
                int before = pd?.healthBlue ?? 0;

                FixBlueHealthFsm();
                orig(eventName);

                if (!IsApplied || pd == null)
                {
                    return;
                }

                if (!pendingBlueHealthCoroutine)
                {
                    pendingBlueHealthStart = before;
                    pendingBlueHealthCoroutine = true;
                    CoroutineController.Start(EnsureBlueHealthApplied());
                }

                pendingBlueHealth++;

                return;
            }

            orig(eventName);
        }

        private IEnumerator EnsureBlueHealthApplied()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return null;
                if (!IsApplied)
                {
                    pendingBlueHealth = 0;
                    pendingBlueHealthCoroutine = false;
                    yield break;
                }
            }

            PlayerData? pd = PlayerData.instance;
            if (pd != null)
            {
                int target = pendingBlueHealthStart + pendingBlueHealth;
                int maxBlue = GetHealthBlueMax(pd);
                if (maxBlue < target)
                {
                    SetHealthBlueMax(pd, target);
                    maxBlue = GetHealthBlueMax(pd);
                }

                if (pendingBlueHealth > 0 && pd.healthBlue < target)
                {
                    pd.healthBlue = Math.Min(target, maxBlue);
                    EventRegister.SendEvent(UpdateBlueHealthEvent);
                }
            }

            pendingBlueHealth = 0;
            pendingBlueHealthCoroutine = false;
        }

        private static int GetHealthBlueMax(PlayerData pd)
        {
            try
            {
                return ReflectionHelper.GetField<PlayerData, int>(pd, "healthBlueMax");
            }
            catch
            {
                return int.MaxValue;
            }
        }

        private static void SetHealthBlueMax(PlayerData pd, int value)
        {
            try
            {
                ReflectionHelper.SetField(pd, "healthBlueMax", value);
            }
            catch
            {
                // ignore missing field
            }
        }

        private static void FixBlueHealthFsm()
        {
            try
            {
                GameObject? hud = Ref.GC?.hudCanvas?.gameObject;
                if (hud == null)
                {
                    return;
                }

                PlayMakerFSM? fsm = hud.Child("Health")?.LocateMyFSM(BlueHealthFsmName);
                if (fsm != null && fsm.ActiveStateName == BlueHealthFsmWaitState)
                {
                    fsm.SendEvent(BlueHealthFsmLastHpEvent);
                }
            }
            catch
            {
                // ignore FSM lookup errors
            }
        }
    }
}
