namespace GodhomeQoL.Modules.BossChallenge;

public sealed class PaintmasterSheoHelper : SingleBossHpHelper
{
    private const string PaintmasterSheoScene = "GG_Painter";
    private const string PaintmasterSheoName = "Sheo Boss";
    private const int DefaultPaintmasterSheoMaxHp = 1450;
    private const int DefaultPaintmasterSheoVanillaHp = 1450;
    private const int P5PaintmasterSheoHp = 950;

    [LocalSetting]
    [BoolOption]
    internal static bool paintmasterSheoUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool paintmasterSheoP5Hp = false;

    [LocalSetting]
    internal static int paintmasterSheoMaxHp = DefaultPaintmasterSheoMaxHp;

    [LocalSetting]
    internal static int paintmasterSheoMaxHpBeforeP5 = DefaultPaintmasterSheoMaxHp;

    [LocalSetting]
    internal static bool paintmasterSheoUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool paintmasterSheoHasStoredStateBeforeP5 = false;

    private static PaintmasterSheoHelper? instance;

    public PaintmasterSheoHelper() => instance = this;

    private protected override string SceneName => PaintmasterSheoScene;
    private protected override string BossObjectName => PaintmasterSheoName;
    private protected override int DefaultVanillaHp => DefaultPaintmasterSheoVanillaHp;
    private protected override int P5Hp => P5PaintmasterSheoHp;
    private protected override bool UseMaxHp { get => paintmasterSheoUseMaxHp; set => paintmasterSheoUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => paintmasterSheoP5Hp; set => paintmasterSheoP5Hp = value; }
    private protected override int MaxHp { get => paintmasterSheoMaxHp; set => paintmasterSheoMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => paintmasterSheoMaxHpBeforeP5; set => paintmasterSheoMaxHpBeforeP5 = value; }
    private protected override bool UseMaxHpBeforeP5 { get => paintmasterSheoUseMaxHpBeforeP5; set => paintmasterSheoUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => paintmasterSheoHasStoredStateBeforeP5; set => paintmasterSheoHasStoredStateBeforeP5 = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyPaintmasterSheoHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();
}
