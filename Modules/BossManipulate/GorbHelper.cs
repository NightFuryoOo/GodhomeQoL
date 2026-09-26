using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class GorbHelper : PhaseThresholdBossHelper
{
    private const string GorbScene = "GG_Ghost_Gorb_V";
    private const string GorbName = "Ghost Warrior Slug";
    private const string GorbPhaseFsmName = "Attacking";
    private const string GorbPhaseCheckState1Name = "Double?";
    private const string GorbPhaseCheckState2Name = "Triple?";
    private const string GorbPhase2TransitionStateName = "Double Pause";
    private const string GorbPhase3TransitionStateName = "Anim";
    private const string GorbPhase2VariableName = "Double HP";
    private const string GorbPhase3VariableName = "Triple HP";
    private const int DefaultGorbMaxHp = 1000;
    private const int DefaultGorbVanillaHp = 1000;
    private const int DefaultGorbPhase2Hp = 700;
    private const int DefaultGorbPhase3Hp = 400;
    private const int P5GorbHp = 650;
    private const int MinGorbHp = 1;
    private const int MaxGorbHp = 999999;
    private const int MinGorbPhase2Hp = 2;
    private const int MinGorbPhase3Hp = 1;
    private const int GorbPhaseThresholdReapplyAttempts = 8;
    private const float GorbPhaseThresholdReapplyInterval = 0.15f;

    [LocalSetting]
    [BoolOption]
    internal static bool gorbUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool gorbP5Hp = false;

    [LocalSetting]
    internal static int gorbMaxHp = DefaultGorbMaxHp;

    [LocalSetting]
    internal static int gorbMaxHpBeforeP5 = DefaultGorbMaxHp;

    [LocalSetting]
    internal static bool gorbUseCustomPhase = false;

    [LocalSetting]
    internal static int gorbPhase2Hp = DefaultGorbPhase2Hp;

    [LocalSetting]
    internal static int gorbPhase3Hp = DefaultGorbPhase3Hp;

    [LocalSetting]
    internal static bool gorbUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool gorbHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool gorbUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int gorbPhase2HpBeforeP5 = DefaultGorbPhase2Hp;

    [LocalSetting]
    internal static int gorbPhase3HpBeforeP5 = DefaultGorbPhase3Hp;

    private static GorbHelper? instance;

    public GorbHelper() => instance = this;

    private protected override int P5Hp => P5GorbHp;
    private protected override int PhaseThresholdReapplyAttempts => GorbPhaseThresholdReapplyAttempts;
    private protected override float PhaseThresholdReapplyInterval => GorbPhaseThresholdReapplyInterval;
    private protected override string SceneName => GorbScene;
    private protected override string BossObjectName => GorbName;
    private protected override string PhaseFsmName => GorbPhaseFsmName;
    private protected override string PhaseCheckState1Name => GorbPhaseCheckState1Name;
    private protected override string PhaseCheckState2Name => GorbPhaseCheckState2Name;
    private protected override string Phase2VariableName => GorbPhase2VariableName;
    private protected override string Phase3VariableName => GorbPhase3VariableName;
    private protected override int DefaultPhase2Hp => DefaultGorbPhase2Hp;
    private protected override int DefaultPhase3Hp => DefaultGorbPhase3Hp;
    private protected override int DefaultVanillaHp => DefaultGorbVanillaHp;
    private protected override int MinHp => MinGorbHp;
    private protected override int MaxHpLimit => MaxGorbHp;
    private protected override int MinPhase2Hp => MinGorbPhase2Hp;
    private protected override int MinPhase3Hp => MinGorbPhase3Hp;
    private protected override string Phase2TransitionStateName => GorbPhase2TransitionStateName;
    private protected override string Phase3TransitionStateName => GorbPhase3TransitionStateName;
    private protected override bool UseMaxHp { get => gorbUseMaxHp; set => gorbUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => gorbP5Hp; set => gorbP5Hp = value; }
    private protected override int MaxHp { get => gorbMaxHp; set => gorbMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => gorbMaxHpBeforeP5; set => gorbMaxHpBeforeP5 = value; }
    private protected override bool UseCustomPhase { get => gorbUseCustomPhase; set => gorbUseCustomPhase = value; }
    private protected override int Phase2Hp { get => gorbPhase2Hp; set => gorbPhase2Hp = value; }
    private protected override int Phase3Hp { get => gorbPhase3Hp; set => gorbPhase3Hp = value; }
    private protected override bool UseMaxHpBeforeP5 { get => gorbUseMaxHpBeforeP5; set => gorbUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => gorbHasStoredStateBeforeP5; set => gorbHasStoredStateBeforeP5 = value; }
    private protected override bool UseCustomPhaseBeforeP5 { get => gorbUseCustomPhaseBeforeP5; set => gorbUseCustomPhaseBeforeP5 = value; }
    private protected override int Phase2HpBeforeP5 { get => gorbPhase2HpBeforeP5; set => gorbPhase2HpBeforeP5 = value; }
    private protected override int Phase3HpBeforeP5 { get => gorbPhase3HpBeforeP5; set => gorbPhase3HpBeforeP5 = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyGorbHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyPhaseThresholdSettingsIfPresent() => instance?.ApplyPhaseThresholdSettingsIfPresentCore();

    internal static void RestoreVanillaPhaseThresholdsIfPresent() => instance?.RestoreVanillaPhaseThresholdsIfPresentCore();

    internal static int GetPhase2MaxHpForUi() => instance?.GetPhase2MaxHpForUiCore() ?? default;
}
