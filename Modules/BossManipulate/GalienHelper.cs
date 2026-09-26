using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class GalienHelper : PhaseThresholdBossHelper
{
    private const string GalienScene = "GG_Ghost_Galien";
    private const string GalienName = "Ghost Warrior Galien";
    private const string GalienPhaseFsmName = "Summon Minis";
    private const string GalienPhaseCheckState1Name = "Check";
    private const string GalienPhaseCheckState2Name = "Check 2";
    private const string GalienPhaseIdleStateName = "Idle";
    private const string GalienPhaseIdle2StateName = "Idle 2";
    private const string GalienPhaseSummonAnticStateName = "Summon Antic";
    private const string GalienPhaseSummonAntic2StateName = "Summon Antic 2";
    private const string GalienPhase2VariableName = "Summon HP1";
    private const string GalienPhase3VariableName = "Summon HP2";
    private const int DefaultGalienMaxHp = 1000;
    private const int DefaultGalienVanillaHp = 1000;
    private const int DefaultGalienPhase2Hp = 700;
    private const int DefaultGalienPhase3Hp = 400;
    private const int P5GalienHp = 650;
    private const int MinGalienHp = 1;
    private const int MaxGalienHp = 999999;
    private const int MinGalienPhase2Hp = 2;
    private const int MinGalienPhase3Hp = 1;
    private const int GalienPhaseThresholdReapplyAttempts = 8;
    private const float GalienPhaseThresholdReapplyInterval = 0.15f;

    [LocalSetting]
    [BoolOption]
    internal static bool galienUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool galienP5Hp = false;

    [LocalSetting]
    internal static int galienMaxHp = DefaultGalienMaxHp;

    [LocalSetting]
    internal static int galienMaxHpBeforeP5 = DefaultGalienMaxHp;

    [LocalSetting]
    internal static bool galienUseCustomPhase = false;

    [LocalSetting]
    internal static int galienPhase2Hp = DefaultGalienPhase2Hp;

    [LocalSetting]
    internal static int galienPhase3Hp = DefaultGalienPhase3Hp;

    [LocalSetting]
    internal static bool galienUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool galienHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool galienUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int galienPhase2HpBeforeP5 = DefaultGalienPhase2Hp;

    [LocalSetting]
    internal static int galienPhase3HpBeforeP5 = DefaultGalienPhase3Hp;

    private static GalienHelper? instance;

    public GalienHelper() => instance = this;

    private protected override int P5Hp => P5GalienHp;
    private protected override int PhaseThresholdReapplyAttempts => GalienPhaseThresholdReapplyAttempts;
    private protected override float PhaseThresholdReapplyInterval => GalienPhaseThresholdReapplyInterval;
    private protected override string SceneName => GalienScene;
    private protected override string BossObjectName => GalienName;
    private protected override string PhaseFsmName => GalienPhaseFsmName;
    private protected override string PhaseCheckState1Name => GalienPhaseCheckState1Name;
    private protected override string PhaseCheckState2Name => GalienPhaseCheckState2Name;
    private protected override string Phase2VariableName => GalienPhase2VariableName;
    private protected override string Phase3VariableName => GalienPhase3VariableName;
    private protected override int DefaultPhase2Hp => DefaultGalienPhase2Hp;
    private protected override int DefaultPhase3Hp => DefaultGalienPhase3Hp;
    private protected override int DefaultVanillaHp => DefaultGalienVanillaHp;
    private protected override int MinHp => MinGalienHp;
    private protected override int MaxHpLimit => MaxGalienHp;
    private protected override int MinPhase2Hp => MinGalienPhase2Hp;
    private protected override int MinPhase3Hp => MinGalienPhase3Hp;
    private protected override string PhaseIdleStateName => GalienPhaseIdleStateName;
    private protected override string PhaseIdle2StateName => GalienPhaseIdle2StateName;
    private protected override bool UseMaxHp { get => galienUseMaxHp; set => galienUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => galienP5Hp; set => galienP5Hp = value; }
    private protected override int MaxHp { get => galienMaxHp; set => galienMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => galienMaxHpBeforeP5; set => galienMaxHpBeforeP5 = value; }
    private protected override bool UseCustomPhase { get => galienUseCustomPhase; set => galienUseCustomPhase = value; }
    private protected override int Phase2Hp { get => galienPhase2Hp; set => galienPhase2Hp = value; }
    private protected override int Phase3Hp { get => galienPhase3Hp; set => galienPhase3Hp = value; }
    private protected override bool UseMaxHpBeforeP5 { get => galienUseMaxHpBeforeP5; set => galienUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => galienHasStoredStateBeforeP5; set => galienHasStoredStateBeforeP5 = value; }
    private protected override bool UseCustomPhaseBeforeP5 { get => galienUseCustomPhaseBeforeP5; set => galienUseCustomPhaseBeforeP5 = value; }
    private protected override int Phase2HpBeforeP5 { get => galienPhase2HpBeforeP5; set => galienPhase2HpBeforeP5 = value; }
    private protected override int Phase3HpBeforeP5 { get => galienPhase3HpBeforeP5; set => galienPhase3HpBeforeP5 = value; }

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

        TryForceGalienSummonTransitions(self.gameObject, self.hp);
    }

    private void TryForceGalienSummonTransitions(GameObject bossObject, int currentHp)
    {
        if (bossObject == null || !ShouldApplySettings(bossObject) || !ShouldUseCustomPhaseThresholds())
        {
            return;
        }

        int phase2Threshold = ClampBossPhase2Hp(galienPhase2Hp, ResolvePhase2MaxHp());
        int phase3Threshold = ClampBossPhase3Hp(galienPhase3Hp, phase2Threshold);

        foreach (PlayMakerFSM fsm in bossObject.GetComponents<PlayMakerFSM>())
        {
            if (fsm == null || !IsBossPhaseControlFsm(fsm))
            {
                continue;
            }

            string activeState = fsm.ActiveStateName ?? string.Empty;
            if ((string.Equals(activeState, GalienPhaseIdleStateName, StringComparison.Ordinal)
                || string.Equals(activeState, GalienPhaseCheckState1Name, StringComparison.Ordinal))
                && currentHp <= phase2Threshold)
            {
                fsm.Fsm.SetState(GalienPhaseSummonAnticStateName);
                continue;
            }

            if ((string.Equals(activeState, GalienPhaseIdle2StateName, StringComparison.Ordinal)
                || string.Equals(activeState, GalienPhaseCheckState2Name, StringComparison.Ordinal))
                && currentHp <= phase3Threshold)
            {
                fsm.Fsm.SetState(GalienPhaseSummonAntic2StateName);
            }
        }
    }

    private protected override void SetPhaseThresholdsOnFsm(PlayMakerFSM fsm, int phase2Hp, int phase3Hp, bool useExactCustomThresholds)
    {
        int targetPhase2Hp = ClampBossPhase2Hp(phase2Hp, ResolvePhase2MaxHp());
        int targetPhase3Hp = ClampBossPhase3Hp(phase3Hp, targetPhase2Hp);

        FsmInt? phase2Variable = fsm.FsmVariables.GetFsmInt(GalienPhase2VariableName);
        if (phase2Variable != null)
        {
            phase2Variable.Value = targetPhase2Hp;
        }

        FsmInt? phase3Variable = fsm.FsmVariables.GetFsmInt(GalienPhase3VariableName);
        if (phase3Variable != null)
        {
            phase3Variable.Value = targetPhase3Hp;
        }

        SetThresholdOnCheckState(fsm, GalienPhaseCheckState1Name, GalienPhase2VariableName, targetPhase2Hp, useExactCustomThresholds);
        SetThresholdOnCheckState(fsm, GalienPhaseCheckState2Name, GalienPhase3VariableName, targetPhase3Hp, useExactCustomThresholds);

        FsmState? initState = fsm.Fsm?.GetState("Init");
        if (initState?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in initState.Actions)
        {
            if (action is not SetIntValue setIntValue || setIntValue.intVariable == null || setIntValue.intValue == null)
            {
                continue;
            }

            if (string.Equals(setIntValue.intVariable.Name, GalienPhase2VariableName, StringComparison.Ordinal))
            {
                setIntValue.intValue.Value = targetPhase2Hp;
                setIntValue.intVariable.Value = targetPhase2Hp;
            }
            else if (string.Equals(setIntValue.intVariable.Name, GalienPhase3VariableName, StringComparison.Ordinal))
            {
                setIntValue.intValue.Value = targetPhase3Hp;
                setIntValue.intVariable.Value = targetPhase3Hp;
            }
        }
    }

    private protected override void SetThresholdOnCheckState(PlayMakerFSM fsm, string stateName, string variableName, int targetValue, bool useExactCustomThresholds)
    {
        FsmState? checkState = fsm.Fsm?.GetState(stateName);
        if (checkState?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in checkState.Actions)
        {
            if (action is IntCompare compare && compare.integer2 != null)
            {
                if (useExactCustomThresholds)
                {
                    compare.integer2.UseVariable = false;
                    compare.integer2.Name = string.Empty;
                    compare.integer2.Value = SafeIncrementThreshold(targetValue);
                }
                else
                {
                    compare.integer2.UseVariable = true;
                    compare.integer2.Name = variableName;
                    compare.integer2.Value = targetValue;
                }
            }
        }
    }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyGalienHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyPhaseThresholdSettingsIfPresent() => instance?.ApplyPhaseThresholdSettingsIfPresentCore();

    internal static void RestoreVanillaPhaseThresholdsIfPresent() => instance?.RestoreVanillaPhaseThresholdsIfPresentCore();

    internal static int GetPhase2MaxHpForUi() => instance?.GetPhase2MaxHpForUiCore() ?? default;
}
