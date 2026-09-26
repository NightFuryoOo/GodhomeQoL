namespace GodhomeQoL.Modules.BossChallenge;

public sealed class BroodingMawlekHelper : SingleBossHpHelper
{
    private const string MawlekScene = "GG_Brooding_Mawlek_V";
    private const string MawlekName = "Mawlek Body";
    private const int DefaultMawlekMaxHp = 1050;
    private const int DefaultMawlekVanillaHp = 1050;
    private const int P5MawlekHp = 750;

    [LocalSetting]
    [BoolOption]
    internal static bool mawlekUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool mawlekP5Hp = false;

    [LocalSetting]
    internal static int mawlekMaxHp = DefaultMawlekMaxHp;

    [LocalSetting]
    internal static int mawlekMaxHpBeforeP5 = DefaultMawlekMaxHp;

    [LocalSetting]
    internal static bool mawlekUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool mawlekHasStoredStateBeforeP5 = false;

    private static BroodingMawlekHelper? instance;

    public BroodingMawlekHelper() => instance = this;

    private protected override string SceneName => MawlekScene;
    private protected override string BossObjectName => MawlekName;
    private protected override int DefaultVanillaHp => DefaultMawlekVanillaHp;
    private protected override int P5Hp => P5MawlekHp;
    private protected override bool UseMaxHp { get => mawlekUseMaxHp; set => mawlekUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => mawlekP5Hp; set => mawlekP5Hp = value; }
    private protected override int MaxHp { get => mawlekMaxHp; set => mawlekMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => mawlekMaxHpBeforeP5; set => mawlekMaxHpBeforeP5 = value; }
    private protected override bool UseMaxHpBeforeP5 { get => mawlekUseMaxHpBeforeP5; set => mawlekUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => mawlekHasStoredStateBeforeP5; set => mawlekHasStoredStateBeforeP5 = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyMawlekHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();
}
