#nullable enable

using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ToggleableBindings.Utility;
using UnityEngine;

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

            int mpLeft = Math.Min(PlayerData.instance.MPCharge, 33);
            PlayerData.instance.ClearMP();
            PlayerData.instance.AddMPCharge(mpLeft);

            var gm = GameManager.instance;
            yield return new WaitWhile(() => !gm.soulOrb_fsm || !gm.soulVessel_fsm);
            gm.soulOrb_fsm.SendEvent(MPLoseEvent);
            gm.soulVessel_fsm.SendEvent(MPReserveDownEvent);
            yield return WaitForHudEventReady(BindVesselOrbEvent, HudEventWaitFrames);
            if (IsHudEventReady(BindVesselOrbEvent))
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

            gm.soulOrb_fsm.SendEvent(MPLoseEvent);
            yield return WaitForHudEventReady(UnbindVesselOrbEvent, HudEventWaitFrames);
            if (IsHudEventReady(UnbindVesselOrbEvent))
            {
                EventRegister.SendEvent(UnbindVesselOrbEvent);
            }
        }

        private void HudEvents_In()
        {
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
