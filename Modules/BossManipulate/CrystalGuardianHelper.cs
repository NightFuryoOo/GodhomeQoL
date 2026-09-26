namespace GodhomeQoL.Modules.BossChallenge;

public sealed class CrystalGuardianHelper : SingleBossHpHelper
{
    private const string CrystalGuardianScene = "GG_Crystal_Guardian";
    private const string CrystalGuardianName = "Mega Zombie Beam Miner (1)";
    private const int DefaultCrystalGuardianMaxHp = 900;
    private const int DefaultCrystalGuardianVanillaHp = 900;
    private const int P5CrystalGuardianHp = 650;

    [LocalSetting]
    [BoolOption]
    internal static bool crystalGuardianUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool crystalGuardianP5Hp = false;

    [LocalSetting]
    internal static int crystalGuardianMaxHp = DefaultCrystalGuardianMaxHp;

    [LocalSetting]
    internal static int crystalGuardianMaxHpBeforeP5 = DefaultCrystalGuardianMaxHp;

    [LocalSetting]
    internal static bool crystalGuardianUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool crystalGuardianHasStoredStateBeforeP5 = false;

    private static CrystalGuardianHelper? instance;

    public CrystalGuardianHelper() => instance = this;

    private protected override string SceneName => CrystalGuardianScene;
    private protected override string BossObjectName => CrystalGuardianName;
    private protected override int DefaultVanillaHp => DefaultCrystalGuardianVanillaHp;
    private protected override int P5Hp => P5CrystalGuardianHp;
    private protected override bool UseMaxHp { get => crystalGuardianUseMaxHp; set => crystalGuardianUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => crystalGuardianP5Hp; set => crystalGuardianP5Hp = value; }
    private protected override int MaxHp { get => crystalGuardianMaxHp; set => crystalGuardianMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => crystalGuardianMaxHpBeforeP5; set => crystalGuardianMaxHpBeforeP5 = value; }
    private protected override bool UseMaxHpBeforeP5 { get => crystalGuardianUseMaxHpBeforeP5; set => crystalGuardianUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => crystalGuardianHasStoredStateBeforeP5; set => crystalGuardianHasStoredStateBeforeP5 = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyCrystalGuardianHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();
}
