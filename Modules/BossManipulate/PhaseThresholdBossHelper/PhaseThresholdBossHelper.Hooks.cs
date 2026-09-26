using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class PhaseThresholdBossHelper : Module
{
    private protected override void Load()
    {
        moduleActive = true;
        BossManipulateEntryGuard.EnsureHooks();
        NormalizeP5State();
        NormalizePhaseThresholdState();
        vanillaHpByInstance.Clear();
        vanillaPhaseThresholdsByFsm.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.HealthManager.Update += OnHealthManagerUpdate;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_Boss;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_Boss;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresentCore();
        RestoreVanillaPhaseThresholdsIfPresentCore();
        moduleActive = false;
        On.HealthManager.Awake -= OnHealthManagerAwake;
        On.HealthManager.Start -= OnHealthManagerStart;
        On.HealthManager.Update -= OnHealthManagerUpdate;
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_Boss;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_Boss;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
        vanillaPhaseThresholdsByFsm.Clear();
        hoGEntryAllowed = false;
    }

    private protected void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsBoss(self))
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyBossHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private protected void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsBoss(self))
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyBossHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }

        _ = self.StartCoroutine(DeferredApplyPhaseThresholds(self));
    }

    private protected void OnPlayMakerFsmOnEnable_Boss(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsBossPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private protected void OnPlayMakerFsmStart_Boss(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsBossPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private protected virtual void OnHealthManagerUpdate(On.HealthManager.orig_Update orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsBoss(self))
        {
            return;
        }

        if (!ShouldApplySettings(self.gameObject) || !ShouldUseCustomPhaseThresholds())
        {
            return;
        }

        TryForceBossPhaseTransitions(self.gameObject, self.hp);
    }

    private protected IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsBoss(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyBossHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsBoss(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyBossHealth(hm.gameObject, hm);
            }
        }

        ApplyPhaseThresholdSettingsIfPresentCore();
    }

    private protected IEnumerator DeferredApplyPhaseThresholds(HealthManager hm)
    {
        for (int attempt = 0; attempt < PhaseThresholdReapplyAttempts; attempt++)
        {
            if (!moduleActive || hm == null || hm.gameObject == null || !IsBoss(hm) || !ShouldApplySettings(hm.gameObject))
            {
                yield break;
            }

            if (!ShouldUseCustomPhaseThresholds())
            {
                yield break;
            }

            ApplyPhaseThresholdSettingsIfPresentCore();
            yield return new WaitForSeconds(PhaseThresholdReapplyInterval);
        }
    }

    private protected void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, SceneName, StringComparison.Ordinal))
        {
            vanillaHpByInstance.Clear();
            vanillaPhaseThresholdsByFsm.Clear();
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyHealthIfPresentCore();
        }
        else
        {
            RestoreVanillaHealthIfPresentCore();
        }

        ApplyPhaseThresholdSettingsIfPresentCore();
    }

    private protected string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);
        return newSceneName;
    }

    private protected virtual void TryForceBossPhaseTransitions(GameObject bossObject, int currentHp)
    {
        if (bossObject == null || !ShouldApplySettings(bossObject) || !ShouldUseCustomPhaseThresholds())
        {
            return;
        }

        int phase2Threshold = ClampBossPhase2Hp(Phase2Hp, ResolvePhase2MaxHp());
        int phase3Threshold = ClampBossPhase3Hp(Phase3Hp, phase2Threshold);

        foreach (PlayMakerFSM fsm in bossObject.GetComponents<PlayMakerFSM>())
        {
            if (fsm == null || !IsBossPhaseControlFsm(fsm))
            {
                continue;
            }

            string activeState = fsm.ActiveStateName ?? string.Empty;
            if (string.Equals(activeState, PhaseCheckState1Name, StringComparison.Ordinal) && currentHp <= phase2Threshold)
            {
                fsm.Fsm.SetState(Phase2TransitionStateName);
                continue;
            }

            if (string.Equals(activeState, PhaseCheckState2Name, StringComparison.Ordinal) && currentHp <= phase3Threshold)
            {
                fsm.Fsm.SetState(Phase3TransitionStateName);
            }
        }
    }

    private protected virtual void TryForceBossEscalationTransitions(GameObject bossObject, int currentHp)
    {
        if (bossObject == null || !ShouldApplySettings(bossObject) || !ShouldUseCustomPhaseThresholds())
        {
            return;
        }

        int phase2Threshold = ClampBossPhase2Hp(Phase2Hp, ResolvePhase2MaxHp());
        int phase3Threshold = ClampBossPhase3Hp(Phase3Hp, phase2Threshold);

        foreach (PlayMakerFSM fsm in bossObject.GetComponents<PlayMakerFSM>())
        {
            if (fsm == null || !IsBossPhaseControlFsm(fsm))
            {
                continue;
            }

            string activeState = fsm.ActiveStateName ?? string.Empty;
            if ((string.Equals(activeState, PhaseIdleStateName, StringComparison.Ordinal)
                || string.Equals(activeState, PhaseCheckState1Name, StringComparison.Ordinal))
                && currentHp <= phase2Threshold)
            {
                fsm.Fsm.SetState(PhaseEscalateState1Name);
                continue;
            }

            if ((string.Equals(activeState, PhaseIdle2StateName, StringComparison.Ordinal)
                || string.Equals(activeState, PhaseCheckState2Name, StringComparison.Ordinal))
                && currentHp <= phase3Threshold)
            {
                fsm.Fsm.SetState(PhaseEscalateState2Name);
            }
        }
    }
}
