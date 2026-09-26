using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class NoEyesP4Helper : PhaseThresholdBossHelper
{
    private const string NoEyesScene = "GG_Ghost_No_Eyes";
    private const string NoEyesName = "Ghost Warrior No Eyes";
    private const string NoEyesPhaseFsmName = "Escalation";
    private const string NoEyesPhaseCheckState1Name = "Check";
    private const string NoEyesPhaseCheckState2Name = "Check 2";
    private const string NoEyesPhaseIdleStateName = "Idle";
    private const string NoEyesPhaseIdle2StateName = "Idle 2";
    private const string NoEyesPhaseEscalateState1Name = "Escalate";
    private const string NoEyesPhaseEscalateState2Name = "Escalate 2";
    private const string NoEyesPhase2VariableName = "Esc 1";
    private const string NoEyesPhase3VariableName = "Esc 2";
    private const int DefaultNoEyesMaxHp = 570;
    private const int DefaultNoEyesVanillaHp = 570;
    private const int DefaultNoEyesPhase2Hp = 150;
    private const int DefaultNoEyesPhase3Hp = 90;
    private const int MinNoEyesHp = 1;
    private const int MaxNoEyesHp = 999999;
    private const int MinNoEyesPhase2Hp = 2;
    private const int MinNoEyesPhase3Hp = 1;
    private const int NoEyesPhaseThresholdReapplyAttempts = 8;
    private const float NoEyesPhaseThresholdReapplyInterval = 0.15f;

    [LocalSetting]
    [BoolOption]
    internal static bool noEyesP4UseMaxHp = false;

    [LocalSetting]
    internal static int noEyesP4MaxHp = DefaultNoEyesMaxHp;

    [LocalSetting]
    internal static bool noEyesP4UseCustomPhase = false;

    [LocalSetting]
    internal static int noEyesP4Phase2Hp = DefaultNoEyesPhase2Hp;

    [LocalSetting]
    internal static int noEyesP4Phase3Hp = DefaultNoEyesPhase3Hp;

    private static NoEyesP4Helper? instance;

    public NoEyesP4Helper() => instance = this;

    private protected override int PhaseThresholdReapplyAttempts => NoEyesPhaseThresholdReapplyAttempts;
    private protected override float PhaseThresholdReapplyInterval => NoEyesPhaseThresholdReapplyInterval;
    private protected override string SceneName => NoEyesScene;
    private protected override string BossObjectName => NoEyesName;
    private protected override string PhaseFsmName => NoEyesPhaseFsmName;
    private protected override string PhaseCheckState1Name => NoEyesPhaseCheckState1Name;
    private protected override string PhaseCheckState2Name => NoEyesPhaseCheckState2Name;
    private protected override string Phase2VariableName => NoEyesPhase2VariableName;
    private protected override string Phase3VariableName => NoEyesPhase3VariableName;
    private protected override int DefaultPhase2Hp => DefaultNoEyesPhase2Hp;
    private protected override int DefaultPhase3Hp => DefaultNoEyesPhase3Hp;
    private protected override int DefaultVanillaHp => DefaultNoEyesVanillaHp;
    private protected override int MinHp => MinNoEyesHp;
    private protected override int MaxHpLimit => MaxNoEyesHp;
    private protected override int MinPhase2Hp => MinNoEyesPhase2Hp;
    private protected override int MinPhase3Hp => MinNoEyesPhase3Hp;
    private protected override string PhaseIdleStateName => NoEyesPhaseIdleStateName;
    private protected override string PhaseIdle2StateName => NoEyesPhaseIdle2StateName;
    private protected override string PhaseEscalateState1Name => NoEyesPhaseEscalateState1Name;
    private protected override string PhaseEscalateState2Name => NoEyesPhaseEscalateState2Name;
    private protected override bool UseMaxHp { get => noEyesP4UseMaxHp; set => noEyesP4UseMaxHp = value; }
    private protected override int MaxHp { get => noEyesP4MaxHp; set => noEyesP4MaxHp = value; }
    private protected override bool UseCustomPhase { get => noEyesP4UseCustomPhase; set => noEyesP4UseCustomPhase = value; }
    private protected override int Phase2Hp { get => noEyesP4Phase2Hp; set => noEyesP4Phase2Hp = value; }
    private protected override int Phase3Hp { get => noEyesP4Phase3Hp; set => noEyesP4Phase3Hp = value; }

    private protected override void Load()
    {
        moduleActive = true;
        BossManipulateEntryGuard.EnsureHooks();
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

    private protected override void OnHealthManagerUpdate(On.HealthManager.orig_Update orig, HealthManager self)
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

        TryForceBossEscalationTransitions(self.gameObject, self.hp);
    }

    private protected override bool ShouldUseCustomPhaseThresholds() => noEyesP4UseCustomPhase;

    private protected override void SetPhaseThresholdsOnFsm(PlayMakerFSM fsm, int phase2Hp, int phase3Hp, bool useExactCustomThresholds)
    {
        int targetPhase2Hp = ClampBossPhase2Hp(phase2Hp, ResolvePhase2MaxHp());
        int targetPhase3Hp = ClampBossPhase3Hp(phase3Hp, targetPhase2Hp);

        FsmInt? phase2Variable = fsm.FsmVariables.GetFsmInt(NoEyesPhase2VariableName);
        if (phase2Variable != null)
        {
            phase2Variable.Value = targetPhase2Hp;
        }

        FsmInt? phase3Variable = fsm.FsmVariables.GetFsmInt(NoEyesPhase3VariableName);
        if (phase3Variable != null)
        {
            phase3Variable.Value = targetPhase3Hp;
        }

        SetThresholdOnCheckState(fsm, NoEyesPhaseCheckState1Name, NoEyesPhase2VariableName, targetPhase2Hp, useExactCustomThresholds);
        SetThresholdOnCheckState(fsm, NoEyesPhaseCheckState2Name, NoEyesPhase3VariableName, targetPhase3Hp, useExactCustomThresholds);
    }

    private protected override int ClampBossPhase2Hp(int value, int maxHp)
    {
        int maxPhase2Hp = Math.Max(MinNoEyesPhase2Hp, ClampBossHp(maxHp));
        if (value < MinNoEyesPhase2Hp)
        {
            return MinNoEyesPhase2Hp;
        }

        return value > maxPhase2Hp ? maxPhase2Hp : value;
    }

    private protected override int ClampBossPhase3Hp(int value, int phase2Hp)
    {
        int clampedPhase2Hp = phase2Hp < MinNoEyesPhase2Hp
            ? MinNoEyesPhase2Hp
            : phase2Hp;
        int maxPhase3Hp = Math.Max(MinNoEyesPhase3Hp, clampedPhase2Hp - 1);
        if (value < MinNoEyesPhase3Hp)
        {
            return MinNoEyesPhase3Hp;
        }

        return value > maxPhase3Hp ? maxPhase3Hp : value;
    }

    private protected override int GetPhase2MaxHpForUiCore()
    {
        return ResolvePhase2MaxHp();
    }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void ApplyNoEyesHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyPhaseThresholdSettingsIfPresent() => instance?.ApplyPhaseThresholdSettingsIfPresentCore();

    internal static void RestoreVanillaPhaseThresholdsIfPresent() => instance?.RestoreVanillaPhaseThresholdsIfPresentCore();

    internal static int GetPhase2MaxHpForUi() => instance?.GetPhase2MaxHpForUiCore() ?? default;
}
