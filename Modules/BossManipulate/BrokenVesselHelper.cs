namespace GodhomeQoL.Modules.BossChallenge;

public sealed class BrokenVesselHelper : BalloonBossHelper
{
    private const string BrokenVesselScene = "GG_Broken_Vessel";
    private const string BrokenVesselName = "Infected Knight";
    private const int DefaultBrokenVesselMaxHp = 1000;
    private const int DefaultBrokenVesselVanillaHp = 1000;
    private const int DefaultBrokenVesselSummonLimit = 3;
    private const int DefaultBrokenVesselVanillaSummonLimit = 3;
    private const int DefaultBrokenVesselPhase2Hp = 420;
    private const int DefaultBrokenVesselPhase3Hp = 370;
    private const int DefaultBrokenVesselPhase4Hp = 220;
    private const int DefaultBrokenVesselPhase5Hp = 110;
    private const int P5BrokenVesselHp = 700;

    [LocalSetting]
    [BoolOption]
    internal static bool brokenVesselUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool brokenVesselP5Hp = false;

    [LocalSetting]
    internal static int brokenVesselMaxHp = DefaultBrokenVesselMaxHp;

    [LocalSetting]
    internal static int brokenVesselMaxHpBeforeP5 = DefaultBrokenVesselMaxHp;

    [LocalSetting]
    internal static bool brokenVesselUseCustomSummonHp = false;

    [LocalSetting]
    internal static int brokenVesselSummonHp = DefaultBossSummonHp;

    [LocalSetting]
    internal static bool brokenVesselUseCustomSummonLimit = false;

    [LocalSetting]
    internal static int brokenVesselSummonLimit = DefaultBrokenVesselSummonLimit;

    [LocalSetting]
    internal static bool brokenVesselUseCustomPhase = false;

    [LocalSetting]
    internal static int brokenVesselPhase2Hp = DefaultBrokenVesselPhase2Hp;

    [LocalSetting]
    internal static int brokenVesselPhase3Hp = DefaultBrokenVesselPhase3Hp;

    [LocalSetting]
    internal static int brokenVesselPhase4Hp = DefaultBrokenVesselPhase4Hp;

    [LocalSetting]
    internal static int brokenVesselPhase5Hp = DefaultBrokenVesselPhase5Hp;

    [LocalSetting]
    internal static bool brokenVesselUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool brokenVesselHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool brokenVesselUseCustomSummonHpBeforeP5 = false;

    [LocalSetting]
    internal static int brokenVesselSummonHpBeforeP5 = DefaultBossSummonHp;

    [LocalSetting]
    internal static bool brokenVesselUseCustomSummonLimitBeforeP5 = false;

    [LocalSetting]
    internal static int brokenVesselSummonLimitBeforeP5 = DefaultBrokenVesselSummonLimit;

    [LocalSetting]
    internal static bool brokenVesselUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int brokenVesselPhase2HpBeforeP5 = DefaultBrokenVesselPhase2Hp;

    [LocalSetting]
    internal static int brokenVesselPhase3HpBeforeP5 = DefaultBrokenVesselPhase3Hp;

    [LocalSetting]
    internal static int brokenVesselPhase4HpBeforeP5 = DefaultBrokenVesselPhase4Hp;

    [LocalSetting]
    internal static int brokenVesselPhase5HpBeforeP5 = DefaultBrokenVesselPhase5Hp;

    private static BrokenVesselHelper? instance;

    public BrokenVesselHelper() => instance = this;

    private protected override string SceneName => BrokenVesselScene;
    private protected override string BossObjectName => BrokenVesselName;
    private protected override int DefaultVanillaHp => DefaultBrokenVesselVanillaHp;
    private protected override int DefaultVanillaSummonLimit => DefaultBrokenVesselVanillaSummonLimit;
    private protected override int DefaultPhase2Hp => DefaultBrokenVesselPhase2Hp;
    private protected override int DefaultPhase3Hp => DefaultBrokenVesselPhase3Hp;
    private protected override int DefaultPhase4Hp => DefaultBrokenVesselPhase4Hp;
    private protected override int DefaultPhase5Hp => DefaultBrokenVesselPhase5Hp;
    private protected override int P5Hp => P5BrokenVesselHp;
    private protected override bool UseMaxHp { get => brokenVesselUseMaxHp; set => brokenVesselUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => brokenVesselP5Hp; set => brokenVesselP5Hp = value; }
    private protected override int MaxHp { get => brokenVesselMaxHp; set => brokenVesselMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => brokenVesselMaxHpBeforeP5; set => brokenVesselMaxHpBeforeP5 = value; }
    private protected override bool UseCustomSummonHp { get => brokenVesselUseCustomSummonHp; set => brokenVesselUseCustomSummonHp = value; }
    private protected override int SummonHp { get => brokenVesselSummonHp; set => brokenVesselSummonHp = value; }
    private protected override bool UseCustomSummonLimit { get => brokenVesselUseCustomSummonLimit; set => brokenVesselUseCustomSummonLimit = value; }
    private protected override int SummonLimit { get => brokenVesselSummonLimit; set => brokenVesselSummonLimit = value; }
    private protected override bool UseCustomPhase { get => brokenVesselUseCustomPhase; set => brokenVesselUseCustomPhase = value; }
    private protected override int Phase2Hp { get => brokenVesselPhase2Hp; set => brokenVesselPhase2Hp = value; }
    private protected override int Phase3Hp { get => brokenVesselPhase3Hp; set => brokenVesselPhase3Hp = value; }
    private protected override int Phase4Hp { get => brokenVesselPhase4Hp; set => brokenVesselPhase4Hp = value; }
    private protected override int Phase5Hp { get => brokenVesselPhase5Hp; set => brokenVesselPhase5Hp = value; }
    private protected override bool UseMaxHpBeforeP5 { get => brokenVesselUseMaxHpBeforeP5; set => brokenVesselUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => brokenVesselHasStoredStateBeforeP5; set => brokenVesselHasStoredStateBeforeP5 = value; }
    private protected override bool UseCustomSummonHpBeforeP5 { get => brokenVesselUseCustomSummonHpBeforeP5; set => brokenVesselUseCustomSummonHpBeforeP5 = value; }
    private protected override int SummonHpBeforeP5 { get => brokenVesselSummonHpBeforeP5; set => brokenVesselSummonHpBeforeP5 = value; }
    private protected override bool UseCustomSummonLimitBeforeP5 { get => brokenVesselUseCustomSummonLimitBeforeP5; set => brokenVesselUseCustomSummonLimitBeforeP5 = value; }
    private protected override int SummonLimitBeforeP5 { get => brokenVesselSummonLimitBeforeP5; set => brokenVesselSummonLimitBeforeP5 = value; }
    private protected override bool UseCustomPhaseBeforeP5 { get => brokenVesselUseCustomPhaseBeforeP5; set => brokenVesselUseCustomPhaseBeforeP5 = value; }
    private protected override int Phase2HpBeforeP5 { get => brokenVesselPhase2HpBeforeP5; set => brokenVesselPhase2HpBeforeP5 = value; }
    private protected override int Phase3HpBeforeP5 { get => brokenVesselPhase3HpBeforeP5; set => brokenVesselPhase3HpBeforeP5 = value; }
    private protected override int Phase4HpBeforeP5 { get => brokenVesselPhase4HpBeforeP5; set => brokenVesselPhase4HpBeforeP5 = value; }
    private protected override int Phase5HpBeforeP5 { get => brokenVesselPhase5HpBeforeP5; set => brokenVesselPhase5HpBeforeP5 = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyBrokenVesselHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyBrokenVesselSummonHealthIfPresent() => instance?.ApplySummonHealthIfPresentCore();

    internal static void RestoreVanillaSummonHealthIfPresent() => instance?.RestoreVanillaSummonHealthIfPresentCore();

    internal static void ApplySummonLimitSettingsIfPresent() => instance?.ApplySummonLimitSettingsIfPresentCore();

    internal static void RestoreVanillaSummonLimitsIfPresent() => instance?.RestoreVanillaSummonLimitsIfPresentCore();

    internal static void EnforceCustomSummonLimitIfPresent() => instance?.EnforceCustomSummonLimitIfPresentCore();

    internal static void ApplyPhaseThresholdSettingsIfPresent() => instance?.ApplyPhaseThresholdSettingsIfPresentCore();

    internal static void RestoreVanillaPhaseThresholdsIfPresent() => instance?.RestoreVanillaPhaseThresholdsIfPresentCore();
}
