namespace GodhomeQoL.Modules.BossChallenge;

public sealed class ElderHuHelper : SingleBossHpHelper
{
    private const string ElderHuScene = "GG_Ghost_Hu";
    private const string ElderHuName = "Ghost Warrior Hu";
    private const int DefaultElderHuMaxHp = 800;
    private const int DefaultElderHuVanillaHp = 800;
    private const int P5ElderHuHp = 600;

    [LocalSetting]
    [BoolOption]
    internal static bool elderHuUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool elderHuP5Hp = false;

    [LocalSetting]
    internal static int elderHuMaxHp = DefaultElderHuMaxHp;

    [LocalSetting]
    internal static int elderHuMaxHpBeforeP5 = DefaultElderHuMaxHp;

    [LocalSetting]
    internal static bool elderHuUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool elderHuHasStoredStateBeforeP5 = false;

    private static ElderHuHelper? instance;

    public ElderHuHelper() => instance = this;

    private protected override string SceneName => ElderHuScene;
    private protected override string BossObjectName => ElderHuName;
    private protected override int DefaultVanillaHp => DefaultElderHuVanillaHp;
    private protected override int P5Hp => P5ElderHuHp;
    private protected override bool UseMaxHp { get => elderHuUseMaxHp; set => elderHuUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => elderHuP5Hp; set => elderHuP5Hp = value; }
    private protected override int MaxHp { get => elderHuMaxHp; set => elderHuMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => elderHuMaxHpBeforeP5; set => elderHuMaxHpBeforeP5 = value; }
    private protected override bool UseMaxHpBeforeP5 { get => elderHuUseMaxHpBeforeP5; set => elderHuUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => elderHuHasStoredStateBeforeP5; set => elderHuHasStoredStateBeforeP5 = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyElderHuHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();
}
