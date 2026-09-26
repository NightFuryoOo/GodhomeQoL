using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class NoEyesHelper : PhaseThresholdBossHelper
{
    private const string NoEyesScene = "GG_Ghost_No_Eyes_V";
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
    private const int DefaultNoEyesMaxHp = 800;
    private const int DefaultNoEyesVanillaHp = 800;
    private const int DefaultNoEyesPhase2Hp = 150;
    private const int DefaultNoEyesPhase3Hp = 90;
    private const int P5NoEyesHp = 570;
    private const int MinNoEyesHp = 1;
    private const int MaxNoEyesHp = 999999;
    private const int MinNoEyesPhase2Hp = 2;
    private const int MinNoEyesPhase3Hp = 1;
    private const int NoEyesPhaseThresholdReapplyAttempts = 8;
    private const float NoEyesPhaseThresholdReapplyInterval = 0.15f;

    [LocalSetting]
    [BoolOption]
    internal static bool noEyesUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool noEyesP5Hp = false;

    [LocalSetting]
    internal static int noEyesMaxHp = DefaultNoEyesMaxHp;

    [LocalSetting]
    internal static int noEyesMaxHpBeforeP5 = DefaultNoEyesMaxHp;

    [LocalSetting]
    internal static bool noEyesUseCustomPhase = false;

    [LocalSetting]
    internal static int noEyesPhase2Hp = DefaultNoEyesPhase2Hp;

    [LocalSetting]
    internal static int noEyesPhase3Hp = DefaultNoEyesPhase3Hp;

    [LocalSetting]
    internal static bool noEyesUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool noEyesHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool noEyesUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int noEyesPhase2HpBeforeP5 = DefaultNoEyesPhase2Hp;

    [LocalSetting]
    internal static int noEyesPhase3HpBeforeP5 = DefaultNoEyesPhase3Hp;

    private static NoEyesHelper? instance;

    public NoEyesHelper() => instance = this;

    private protected override int P5Hp => P5NoEyesHp;
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
    private protected override bool UseMaxHp { get => noEyesUseMaxHp; set => noEyesUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => noEyesP5Hp; set => noEyesP5Hp = value; }
    private protected override int MaxHp { get => noEyesMaxHp; set => noEyesMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => noEyesMaxHpBeforeP5; set => noEyesMaxHpBeforeP5 = value; }
    private protected override bool UseCustomPhase { get => noEyesUseCustomPhase; set => noEyesUseCustomPhase = value; }
    private protected override int Phase2Hp { get => noEyesPhase2Hp; set => noEyesPhase2Hp = value; }
    private protected override int Phase3Hp { get => noEyesPhase3Hp; set => noEyesPhase3Hp = value; }
    private protected override bool UseMaxHpBeforeP5 { get => noEyesUseMaxHpBeforeP5; set => noEyesUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => noEyesHasStoredStateBeforeP5; set => noEyesHasStoredStateBeforeP5 = value; }
    private protected override bool UseCustomPhaseBeforeP5 { get => noEyesUseCustomPhaseBeforeP5; set => noEyesUseCustomPhaseBeforeP5 = value; }
    private protected override int Phase2HpBeforeP5 { get => noEyesPhase2HpBeforeP5; set => noEyesPhase2HpBeforeP5 = value; }
    private protected override int Phase3HpBeforeP5 { get => noEyesPhase3HpBeforeP5; set => noEyesPhase3HpBeforeP5 = value; }

    private protected override void NormalizeP5State()
    {
        if (!noEyesP5Hp)
        {
            return;
        }

        if (!noEyesHasStoredStateBeforeP5)
        {
            noEyesMaxHpBeforeP5 = ClampBossHp(noEyesMaxHp);
            noEyesUseMaxHpBeforeP5 = noEyesUseMaxHp;
            noEyesUseCustomPhaseBeforeP5 = noEyesUseCustomPhase;
            noEyesPhase2HpBeforeP5 = ClampBossPhase2Hp(noEyesPhase2Hp, ResolvePhase2MaxHp());
            noEyesPhase3HpBeforeP5 = ClampBossPhase3Hp(noEyesPhase3Hp, noEyesPhase2HpBeforeP5);
            noEyesHasStoredStateBeforeP5 = true;
        }

        noEyesUseMaxHp = true;
        noEyesUseCustomPhase = false;
        noEyesMaxHp = P5NoEyesHp;
        NormalizePhaseThresholdState();
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

    private protected override void SetP5HpEnabledCore(bool value)
    {
        if (value)
        {
            if (!noEyesP5Hp)
            {
                noEyesMaxHpBeforeP5 = ClampBossHp(noEyesMaxHp);
                noEyesUseMaxHpBeforeP5 = noEyesUseMaxHp;
                noEyesUseCustomPhaseBeforeP5 = noEyesUseCustomPhase;
                noEyesPhase2HpBeforeP5 = ClampBossPhase2Hp(noEyesPhase2Hp, ResolvePhase2MaxHp());
                noEyesPhase3HpBeforeP5 = ClampBossPhase3Hp(noEyesPhase3Hp, noEyesPhase2HpBeforeP5);
                noEyesHasStoredStateBeforeP5 = true;
            }

            noEyesP5Hp = true;
            noEyesUseMaxHp = true;
            noEyesUseCustomPhase = false;
            noEyesMaxHp = P5NoEyesHp;
        }
        else
        {
            if (noEyesP5Hp && noEyesHasStoredStateBeforeP5)
            {
                noEyesMaxHp = ClampBossHp(noEyesMaxHpBeforeP5);
                noEyesUseMaxHp = noEyesUseMaxHpBeforeP5;
                noEyesUseCustomPhase = noEyesUseCustomPhaseBeforeP5;
                noEyesPhase2Hp = ClampBossPhase2Hp(noEyesPhase2HpBeforeP5, ResolvePhase2MaxHp());
                noEyesPhase3Hp = ClampBossPhase3Hp(noEyesPhase3HpBeforeP5, noEyesPhase2Hp);
            }

            noEyesP5Hp = false;
            noEyesHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettingsCore();
    }

    private protected override int GetPhase2MaxHpForUiCore()
    {
        return ResolvePhase2MaxHp();
    }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyNoEyesHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyPhaseThresholdSettingsIfPresent() => instance?.ApplyPhaseThresholdSettingsIfPresentCore();

    internal static void RestoreVanillaPhaseThresholdsIfPresent() => instance?.RestoreVanillaPhaseThresholdsIfPresentCore();

    internal static int GetPhase2MaxHpForUi() => instance?.GetPhase2MaxHpForUiCore() ?? default;
}
