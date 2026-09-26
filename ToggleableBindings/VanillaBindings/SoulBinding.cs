#nullable enable

using HutongGames.PlayMaker;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ToggleableBindings.Utility;
using UnityEngine;
using Vasi;

namespace ToggleableBindings.VanillaBindings
{
    [VanillaBinding]
    internal sealed class SoulBinding : Binding
    {
        private const string BindVesselOrbEvent = "BIND VESSEL ORB";
        private const string UnbindVesselOrbEvent = "UNBIND VESSEL ORB";
        private const string MPLoseEvent = "MP LOSE";
        private const string MPReserveDownEvent = "MP RESERVE DOWN";
        private const int HudEventWaitFrames = 600;
        private const string SoulOrbObjectName = "Soul Orb";
        private const string SoulOrbFsmName = "Soul Orb Control";
        private const string SoulOrbIdleState = "Idle";
        private const string BindingCapName = "Binding Cap";
        private const string BindingCapFullName = "Binding Cap Full";
        private static readonly HashSet<int> _patchedSoulOrbFsms = new();
        private readonly List<IDetour> _detours;
        private bool _pendingHudBindDispatch;

        private Sprite? _defaultSprite;
        private Sprite? _selectedSprite;

        public override Sprite DefaultSprite => _defaultSprite = _defaultSprite != null ? _defaultSprite : _defaultSprite = BaseGamePrefabs.SoulButton.UnsafeGameObject.GetComponent<BossDoorChallengeUIBindingButton>().iconImage.sprite;

        public override Sprite SelectedSprite => _selectedSprite = _selectedSprite != null ? _selectedSprite : _selectedSprite = BaseGamePrefabs.SoulButton.UnsafeGameObject.GetComponent<BossDoorChallengeUIBindingButton>().selectedSprite;

        public SoulBinding() : base("Soul")
        {
            var boundSoulGetter = typeof(BossSequenceController).GetMethod("get_BoundSoul", BindingFlags.Public | BindingFlags.Static);

            _detours = new(1)
            {
                new Hook(boundSoulGetter, new Func<bool>(() => true), TBConstants.HookManualApply)
            };
        }

        protected override void OnApplied()
        {
            _pendingHudBindDispatch = false;
            HudEvents.In += HudEvents_In;
            On.GGCheckBoundSoul.OnEnter += GGCheckBoundSoul_OnEnter;
            IL.BossSequenceController.RestoreBindings += BossSequenceController_RestoreBindings;
            foreach (var detour in _detours)
                detour.Apply();

            CoroutineController.Start(OnAppliedCoroutine());
        }

        private IEnumerator OnAppliedCoroutine()
        {
            yield return new WaitWhile(() => !HeroController.instance);
            yield return null;

            if (!IsApplied)
            {
                yield break;
            }

            int mpLeft = Math.Min(PlayerData.instance.MPCharge, 33);
            PlayerData.instance.ClearMP();
            PlayerData.instance.AddMPCharge(mpLeft);

            var gm = GameManager.instance;
            yield return new WaitWhile(() => !gm.soulOrb_fsm || !gm.soulVessel_fsm);
            PatchSoulOrbFsm(gm.soulOrb_fsm);
            SyncBindingCap();
            if (!IsApplied)
            {
                yield break;
            }

            gm.soulOrb_fsm.SendEvent(MPLoseEvent);
            gm.soulVessel_fsm.SendEvent(MPReserveDownEvent);
            yield return WaitForHudEventReady(BindVesselOrbEvent, HudEventWaitFrames);
            if (IsApplied && IsHudEventReady(BindVesselOrbEvent))
            {
                EventRegister.SendEvent(BindVesselOrbEvent);
            }
        }

        protected override void OnRestored()
        {
            _pendingHudBindDispatch = false;
            HudEvents.In -= HudEvents_In;
            On.GGCheckBoundSoul.OnEnter -= GGCheckBoundSoul_OnEnter;
            IL.BossSequenceController.RestoreBindings -= BossSequenceController_RestoreBindings;
            foreach (var detour in _detours)
                detour.Undo();

            CoroutineController.Start(OnRestoredCoroutine());
        }

        private IEnumerator OnRestoredCoroutine()
        {
            yield return new WaitWhile(() => !HeroController.instance);
            yield return null;

            var gm = GameManager.instance;
            yield return new WaitWhile(() => !gm.soulOrb_fsm);

            if (IsApplied)
            {
                yield break;
            }

            gm.soulOrb_fsm.SendEvent(MPLoseEvent);
            SyncBindingCap();
            yield return WaitForHudEventReady(UnbindVesselOrbEvent, HudEventWaitFrames);
            if (!IsApplied && IsHudEventReady(UnbindVesselOrbEvent))
            {
                EventRegister.SendEvent(UnbindVesselOrbEvent);
            }
        }

        private void HudEvents_In()
        {
            GameManager? gm = GameManager.instance;
            PatchSoulOrbFsm(gm != null ? gm.soulOrb_fsm : null);
            SyncBindingCap();

            if (IsHudEventReady(BindVesselOrbEvent))
            {
                EventRegister.SendEvent(BindVesselOrbEvent);
                return;
            }

            if (_pendingHudBindDispatch)
            {
                return;
            }

            _pendingHudBindDispatch = true;
            CoroutineController.Start(SendBindWhenHudEventReady());
        }

        private IEnumerator SendBindWhenHudEventReady()
        {
            yield return WaitForHudEventReady(BindVesselOrbEvent, HudEventWaitFrames);
            _pendingHudBindDispatch = false;
            if (IsApplied && IsHudEventReady(BindVesselOrbEvent))
            {
                EventRegister.SendEvent(BindVesselOrbEvent);
            }
        }

        private static IEnumerator WaitForHudEventReady(string eventName, int maxFrames)
        {
            int frames = Math.Max(1, maxFrames);
            for (int i = 0; i < frames; i++)
            {
                if (IsHudEventReady(eventName))
                {
                    yield break;
                }

                yield return null;
            }
        }

        private static void PatchSoulOrbFsm(PlayMakerFSM? fsm)
        {
            if (fsm == null || fsm.gameObject.name != SoulOrbObjectName || fsm.FsmName != SoulOrbFsmName)
            {
                return;
            }

            int id = fsm.GetInstanceID();
            if (_patchedSoulOrbFsms.Contains(id))
            {
                return;
            }

            FsmState? idle = fsm.Fsm.GetState(SoulOrbIdleState);
            if (idle == null)
            {
                return;
            }

            idle.AddMethod(SyncBindingCap);
            _patchedSoulOrbFsms.Add(id);
        }

        private static bool IsSoulBound()
        {
            if (BindingManager.TryGetBinding(out SoulBinding? binding) && binding.IsApplied)
            {
                return true;
            }

            return BossSequenceController.IsInSequence && BossSequenceController.BoundSoul;
        }

        internal static void SyncBindingCap()
        {
            try
            {
                GameManager? gm = GameManager.instance;
                PlayMakerFSM? orb = gm != null ? gm.soulOrb_fsm : null;
                if (orb == null)
                {
                    return;
                }

                if (IsSoulBound())
                {
                    SetHudChildActive(orb.transform, BindingCapName, true);
                    return;
                }

                SetHudChildActive(orb.transform, BindingCapName, false);
                SetHudChildActive(orb.transform, BindingCapFullName, false);
            }
            catch
            {
            }
        }

        private static void SetHudChildActive(Transform parent, string childName, bool active)
        {
            Transform? child = parent.Find(childName);
            if (child != null && child.gameObject.activeSelf != active)
            {
                child.gameObject.SetActive(active);
            }
        }

        private static bool IsHudEventReady(string eventName)
        {
            try
            {
                return !string.IsNullOrEmpty(eventName)
                    && EventRegister.eventRegister != null
                    && EventRegister.eventRegister.ContainsKey(eventName);
            }
            catch
            {
                return false;
            }
        }

        private void BossSequenceController_RestoreBindings(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext
            (
                i => i.MatchLdstr(UnbindVesselOrbEvent),
                i => i.MatchCall(typeof(EventRegister), nameof(EventRegister.SendEvent))
            );
            c.RemoveRange(2);
        }

        private void GGCheckBoundSoul_OnEnter(On.GGCheckBoundSoul.orig_OnEnter orig, GGCheckBoundSoul self)
        {
            self.Fsm.Event(self.boundEvent);
            self.Finish();
        }
    }
}
