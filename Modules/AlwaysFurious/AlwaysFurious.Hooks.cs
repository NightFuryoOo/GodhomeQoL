using GodhomeQoL.Modules.Tools;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class AlwaysFurious : Module
{
    private protected override void Load()
    {
        On.PlayMakerFSM.OnEnable += OnFsmEnable;
        On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.OnEnter += OnPlayerDataBoolTestAction;
        On.HeroController.CharmUpdate += OnCharmUpdate;
        USceneManager.activeSceneChanged += OnSceneChanged;
        wasFuryEquippedOnLastCharmUpdate = IsFuryEquipped();
        TryModifyExistingFsm();
        if (IsFuryEquipped())
        {
            TryApplyForcedHealth();
        }
        else
        {
            TryRestoreForcedHealth(allowDeferredWhenUnsafe: true, ignoreSafetyChecks: true);
            TryForceDisableFuryEffectIfUnequipped();
        }

        QuickMenu.RefreshQuickMenuEntryColors();
    }

    private protected override void Unload()
    {
        On.PlayMakerFSM.OnEnable -= OnFsmEnable;
        On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.OnEnter -= OnPlayerDataBoolTestAction;
        On.HeroController.CharmUpdate -= OnCharmUpdate;
        USceneManager.activeSceneChanged -= OnSceneChanged;
        RestoreModifiedFsms();
        bool restoredImmediately = TryRestoreForcedHealth(allowDeferredWhenUnsafe: true, ignoreSafetyChecks: true);
        TryForceDisableFuryEffectIfUnequipped();
        if (restoredImmediately)
        {
            TryRefreshHeroHealth(ignoreSafetyChecks: true);
            TryRefreshHudMasks(ignoreSafetyChecks: true);
        }

        wasFuryEquippedOnLastCharmUpdate = false;
        pendingApply = false;
        CancelPendingRefresh();
        GearSwitcher.ReapplyLastPresetStats();
        QuickMenu.RefreshQuickMenuEntryColors();
    }

    private void OnSceneChanged(Scene from, Scene to)
    {
        if (!Loaded || disablingFromMainMenuTransition)
        {
            return;
        }

        if (!string.Equals(to.name, MainMenuSceneName, StringComparison.Ordinal))
        {
            return;
        }

        try
        {
            disablingFromMainMenuTransition = true;
            Enabled = false;
            GodhomeQoL.SaveGlobalSettingsSafe();
        }
        finally
        {
            disablingFromMainMenuTransition = false;
        }
    }

    private void OnPlayerDataBoolTestAction(
        On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.orig_OnEnter orig,
        HutongGames.PlayMaker.Actions.PlayerDataBoolTest self)
    {
        if (self.Fsm.Name == "Fury"
            && self.Fsm.GameObject.name == "Charm Effects"
            && self.State.Name == "Check HP")
        {
            if (!IsFuryEquipped())
            {
                orig(self);
                return;
            }

            FsmEvent? originalIsTrue = self.isTrue;
            try
            {
                self.isTrue = FsmEvent.GetFsmEvent("FURY");
                orig(self);
            }
            finally
            {
                self.isTrue = originalIsTrue;
            }

            return;
        }

        orig(self);
    }

    private void OnFsmEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (self.gameObject.name == "Charm Effects" && self.FsmName == "Fury")
        {
            TryModifyFuryFsm(self);
            if (IsFuryEquipped())
            {
                TryApplyForcedHealth();
            }
            else
            {
                TryRestoreForcedHealth();
                TryForceDisableFuryEffectIfUnequipped();
            }
        }
    }

    private void OnCharmUpdate(On.HeroController.orig_CharmUpdate orig, HeroController self)
    {
        if (charmUpdateInProgress)
        {
            orig(self);
            return;
        }

        charmUpdateInProgress = true;
        try
        {
            orig(self);

            if (!Loaded)
            {
                return;
            }

            bool furyEquipped = IsFuryEquipped();
            if (furyEquipped)
            {
                TryModifyExistingFsm(forceEnable: !wasFuryEquippedOnLastCharmUpdate);
                TryApplyForcedHealth();
            }
            else
            {
                TryRestoreForcedHealth();
                TryForceDisableFuryEffectIfUnequipped();
            }

            wasFuryEquippedOnLastCharmUpdate = furyEquipped;
        }
        finally
        {
            charmUpdateInProgress = false;
        }
    }
}
