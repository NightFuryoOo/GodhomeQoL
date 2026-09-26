namespace GodhomeQoL.Modules.BossChallenge;

public sealed class LostKinHelper : BalloonBossHelper
{
    private const string LostKinScene = "GG_Lost_Kin";
    private const string LostKinName = "Lost Kin";
    private const int DefaultLostKinMaxHp = 1650;
    private const int DefaultLostKinVanillaHp = 1650;
    private const int DefaultLostKinSummonLimit = 5;
    private const int DefaultLostKinVanillaSummonLimit = 5;
    private const int DefaultLostKinPhase2Hp = 1150;
    private const int DefaultLostKinPhase3Hp = 550;
    private const int DefaultLostKinPhase4Hp = 350;
    private const int DefaultLostKinPhase5Hp = 175;
    private const int P5LostKinHp = 1200;

    [LocalSetting]
    [BoolOption]
    internal static bool lostKinUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool lostKinP5Hp = false;

    [LocalSetting]
    internal static int lostKinMaxHp = DefaultLostKinMaxHp;

    [LocalSetting]
    internal static int lostKinMaxHpBeforeP5 = DefaultLostKinMaxHp;

    [LocalSetting]
    internal static bool lostKinUseCustomSummonHp = false;

    [LocalSetting]
    internal static int lostKinSummonHp = DefaultBossSummonHp;

    [LocalSetting]
    internal static bool lostKinUseCustomSummonLimit = false;

    [LocalSetting]
    internal static int lostKinSummonLimit = DefaultLostKinSummonLimit;

    [LocalSetting]
    internal static bool lostKinUseCustomPhase = false;

    [LocalSetting]
    internal static int lostKinPhase2Hp = DefaultLostKinPhase2Hp;

    [LocalSetting]
    internal static int lostKinPhase3Hp = DefaultLostKinPhase3Hp;

    [LocalSetting]
    internal static int lostKinPhase4Hp = DefaultLostKinPhase4Hp;

    [LocalSetting]
    internal static int lostKinPhase5Hp = DefaultLostKinPhase5Hp;

    [LocalSetting]
    internal static bool lostKinUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool lostKinHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool lostKinUseCustomSummonHpBeforeP5 = false;

    [LocalSetting]
    internal static int lostKinSummonHpBeforeP5 = DefaultBossSummonHp;

    [LocalSetting]
    internal static bool lostKinUseCustomSummonLimitBeforeP5 = false;

    [LocalSetting]
    internal static int lostKinSummonLimitBeforeP5 = DefaultLostKinSummonLimit;

    [LocalSetting]
    internal static bool lostKinUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int lostKinPhase2HpBeforeP5 = DefaultLostKinPhase2Hp;

    [LocalSetting]
    internal static int lostKinPhase3HpBeforeP5 = DefaultLostKinPhase3Hp;

    [LocalSetting]
    internal static int lostKinPhase4HpBeforeP5 = DefaultLostKinPhase4Hp;

    [LocalSetting]
    internal static int lostKinPhase5HpBeforeP5 = DefaultLostKinPhase5Hp;

    private static LostKinHelper? instance;

    public LostKinHelper() => instance = this;

    private protected override string SceneName => LostKinScene;
    private protected override string BossObjectName => LostKinName;
    private protected override int DefaultVanillaHp => DefaultLostKinVanillaHp;
    private protected override int DefaultVanillaSummonLimit => DefaultLostKinVanillaSummonLimit;
    private protected override int DefaultPhase2Hp => DefaultLostKinPhase2Hp;
    private protected override int DefaultPhase3Hp => DefaultLostKinPhase3Hp;
    private protected override int DefaultPhase4Hp => DefaultLostKinPhase4Hp;
    private protected override int DefaultPhase5Hp => DefaultLostKinPhase5Hp;
    private protected override int P5Hp => P5LostKinHp;
    private protected override bool UseMaxHp { get => lostKinUseMaxHp; set => lostKinUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => lostKinP5Hp; set => lostKinP5Hp = value; }
    private protected override int MaxHp { get => lostKinMaxHp; set => lostKinMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => lostKinMaxHpBeforeP5; set => lostKinMaxHpBeforeP5 = value; }
    private protected override bool UseCustomSummonHp { get => lostKinUseCustomSummonHp; set => lostKinUseCustomSummonHp = value; }
    private protected override int SummonHp { get => lostKinSummonHp; set => lostKinSummonHp = value; }
    private protected override bool UseCustomSummonLimit { get => lostKinUseCustomSummonLimit; set => lostKinUseCustomSummonLimit = value; }
    private protected override int SummonLimit { get => lostKinSummonLimit; set => lostKinSummonLimit = value; }
    private protected override bool UseCustomPhase { get => lostKinUseCustomPhase; set => lostKinUseCustomPhase = value; }
    private protected override int Phase2Hp { get => lostKinPhase2Hp; set => lostKinPhase2Hp = value; }
    private protected override int Phase3Hp { get => lostKinPhase3Hp; set => lostKinPhase3Hp = value; }
    private protected override int Phase4Hp { get => lostKinPhase4Hp; set => lostKinPhase4Hp = value; }
    private protected override int Phase5Hp { get => lostKinPhase5Hp; set => lostKinPhase5Hp = value; }
    private protected override bool UseMaxHpBeforeP5 { get => lostKinUseMaxHpBeforeP5; set => lostKinUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => lostKinHasStoredStateBeforeP5; set => lostKinHasStoredStateBeforeP5 = value; }
    private protected override bool UseCustomSummonHpBeforeP5 { get => lostKinUseCustomSummonHpBeforeP5; set => lostKinUseCustomSummonHpBeforeP5 = value; }
    private protected override int SummonHpBeforeP5 { get => lostKinSummonHpBeforeP5; set => lostKinSummonHpBeforeP5 = value; }
    private protected override bool UseCustomSummonLimitBeforeP5 { get => lostKinUseCustomSummonLimitBeforeP5; set => lostKinUseCustomSummonLimitBeforeP5 = value; }
    private protected override int SummonLimitBeforeP5 { get => lostKinSummonLimitBeforeP5; set => lostKinSummonLimitBeforeP5 = value; }
    private protected override bool UseCustomPhaseBeforeP5 { get => lostKinUseCustomPhaseBeforeP5; set => lostKinUseCustomPhaseBeforeP5 = value; }
    private protected override int Phase2HpBeforeP5 { get => lostKinPhase2HpBeforeP5; set => lostKinPhase2HpBeforeP5 = value; }
    private protected override int Phase3HpBeforeP5 { get => lostKinPhase3HpBeforeP5; set => lostKinPhase3HpBeforeP5 = value; }
    private protected override int Phase4HpBeforeP5 { get => lostKinPhase4HpBeforeP5; set => lostKinPhase4HpBeforeP5 = value; }
    private protected override int Phase5HpBeforeP5 { get => lostKinPhase5HpBeforeP5; set => lostKinPhase5HpBeforeP5 = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyLostKinHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyLostKinSummonHealthIfPresent() => instance?.ApplySummonHealthIfPresentCore();

    internal static void RestoreVanillaSummonHealthIfPresent() => instance?.RestoreVanillaSummonHealthIfPresentCore();

    internal static void ApplySummonLimitSettingsIfPresent() => instance?.ApplySummonLimitSettingsIfPresentCore();

    internal static void RestoreVanillaSummonLimitsIfPresent() => instance?.RestoreVanillaSummonLimitsIfPresentCore();

    internal static void EnforceCustomSummonLimitIfPresent() => instance?.EnforceCustomSummonLimitIfPresentCore();

    internal static void ApplyPhaseThresholdSettingsIfPresent() => instance?.ApplyPhaseThresholdSettingsIfPresentCore();

    internal static void RestoreVanillaPhaseThresholdsIfPresent() => instance?.RestoreVanillaPhaseThresholdsIfPresentCore();
}
