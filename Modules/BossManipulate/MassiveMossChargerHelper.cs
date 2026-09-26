namespace GodhomeQoL.Modules.BossChallenge;

public sealed class MassiveMossChargerHelper : SingleBossHpHelper
{
    private const string MassiveMossScene = "GG_Mega_Moss_Charger";
    private const string MassiveMossName = "Mega Moss Charger";
    private const int DefaultMassiveMossMaxHp = 850;
    private const int DefaultMassiveMossVanillaHp = 850;
    private const int P5MassiveMossHp = 480;

    [LocalSetting]
    [BoolOption]
    internal static bool massiveMossUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool massiveMossP5Hp = false;

    [LocalSetting]
    internal static int massiveMossMaxHp = DefaultMassiveMossMaxHp;

    [LocalSetting]
    internal static int massiveMossMaxHpBeforeP5 = DefaultMassiveMossMaxHp;

    [LocalSetting]
    internal static bool massiveMossUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool massiveMossHasStoredStateBeforeP5 = false;

    private static MassiveMossChargerHelper? instance;

    public MassiveMossChargerHelper() => instance = this;

    private protected override string SceneName => MassiveMossScene;
    private protected override string BossObjectName => MassiveMossName;
    private protected override int DefaultVanillaHp => DefaultMassiveMossVanillaHp;
    private protected override int P5Hp => P5MassiveMossHp;
    private protected override bool UseMaxHp { get => massiveMossUseMaxHp; set => massiveMossUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => massiveMossP5Hp; set => massiveMossP5Hp = value; }
    private protected override int MaxHp { get => massiveMossMaxHp; set => massiveMossMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => massiveMossMaxHpBeforeP5; set => massiveMossMaxHpBeforeP5 = value; }
    private protected override bool UseMaxHpBeforeP5 { get => massiveMossUseMaxHpBeforeP5; set => massiveMossUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => massiveMossHasStoredStateBeforeP5; set => massiveMossHasStoredStateBeforeP5 = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyMassiveMossHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();
}
