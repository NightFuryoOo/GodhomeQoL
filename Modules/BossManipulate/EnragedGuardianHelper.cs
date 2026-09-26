namespace GodhomeQoL.Modules.BossChallenge;

public sealed class EnragedGuardianHelper : SingleBossHpHelper
{
    private const string EnragedGuardianScene = "GG_Crystal_Guardian_2";
    private const string EnragedGuardianName = "Zombie Beam Miner Rematch";
    private const int DefaultEnragedGuardianMaxHp = 1250;
    private const int DefaultEnragedGuardianVanillaHp = 1250;
    private const int P5EnragedGuardianHp = 650;

    [LocalSetting]
    [BoolOption]
    internal static bool enragedGuardianUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool enragedGuardianP5Hp = false;

    [LocalSetting]
    internal static int enragedGuardianMaxHp = DefaultEnragedGuardianMaxHp;

    [LocalSetting]
    internal static int enragedGuardianMaxHpBeforeP5 = DefaultEnragedGuardianMaxHp;

    [LocalSetting]
    internal static bool enragedGuardianUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool enragedGuardianHasStoredStateBeforeP5 = false;

    private static EnragedGuardianHelper? instance;

    public EnragedGuardianHelper() => instance = this;

    private protected override string SceneName => EnragedGuardianScene;
    private protected override string BossObjectName => EnragedGuardianName;
    private protected override int DefaultVanillaHp => DefaultEnragedGuardianVanillaHp;
    private protected override int P5Hp => P5EnragedGuardianHp;
    private protected override bool UseMaxHp { get => enragedGuardianUseMaxHp; set => enragedGuardianUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => enragedGuardianP5Hp; set => enragedGuardianP5Hp = value; }
    private protected override int MaxHp { get => enragedGuardianMaxHp; set => enragedGuardianMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => enragedGuardianMaxHpBeforeP5; set => enragedGuardianMaxHpBeforeP5 = value; }
    private protected override bool UseMaxHpBeforeP5 { get => enragedGuardianUseMaxHpBeforeP5; set => enragedGuardianUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => enragedGuardianHasStoredStateBeforeP5; set => enragedGuardianHasStoredStateBeforeP5 = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyEnragedGuardianHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();
}
