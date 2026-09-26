namespace GodhomeQoL.Modules.BossChallenge;

public sealed class MarmuHelper : SingleBossHpHelper
{
    private const string MarmuScene = "GG_Ghost_Marmu_V";
    private const string MarmuName = "Ghost Warrior Marmu";
    private const int DefaultMarmuMaxHp = 600;
    private const int DefaultMarmuVanillaHp = 600;
    private const int P5MarmuHp = 416;

    [LocalSetting]
    [BoolOption]
    internal static bool marmuUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool marmuP5Hp = false;

    [LocalSetting]
    internal static int marmuMaxHp = DefaultMarmuMaxHp;

    [LocalSetting]
    internal static int marmuMaxHpBeforeP5 = DefaultMarmuMaxHp;

    [LocalSetting]
    internal static bool marmuUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool marmuHasStoredStateBeforeP5 = false;

    private static MarmuHelper? instance;

    public MarmuHelper() => instance = this;

    private protected override string SceneName => MarmuScene;
    private protected override string BossObjectName => MarmuName;
    private protected override int DefaultVanillaHp => DefaultMarmuVanillaHp;
    private protected override int P5Hp => P5MarmuHp;
    private protected override bool UseMaxHp { get => marmuUseMaxHp; set => marmuUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => marmuP5Hp; set => marmuP5Hp = value; }
    private protected override int MaxHp { get => marmuMaxHp; set => marmuMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => marmuMaxHpBeforeP5; set => marmuMaxHpBeforeP5 = value; }
    private protected override bool UseMaxHpBeforeP5 { get => marmuUseMaxHpBeforeP5; set => marmuUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => marmuHasStoredStateBeforeP5; set => marmuHasStoredStateBeforeP5 = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyMarmuHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();
}
