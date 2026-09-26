namespace GodhomeQoL.Modules.BossChallenge;

public sealed class SoulMasterHelper : MageLordBossHelper
{
    private const string SoulMasterScene = "GG_Soul_Master";
    private const string SoulMasterName = "Mage Lord";
    private const int DefaultSoulMasterPhase1Hp = 900;
    private const int DefaultSoulMasterPhase2Hp = 600;
    private const int P5SoulMasterPhase1Hp = 600;

    [LocalSetting]
    [BoolOption]
    internal static bool soulMasterP5Hp = false;

    [LocalSetting]
    internal static int soulMasterPhase1Hp = DefaultSoulMasterPhase1Hp;

    [LocalSetting]
    internal static int soulMasterPhase2Hp = DefaultSoulMasterPhase2Hp;

    [LocalSetting]
    internal static int soulMasterPhase1HpBeforeP5 = DefaultSoulMasterPhase1Hp;

    [LocalSetting]
    internal static int soulMasterPhase2HpBeforeP5 = DefaultSoulMasterPhase2Hp;

    [LocalSetting]
    internal static bool soulMasterHasStoredStateBeforeP5 = false;

    private static SoulMasterHelper? instance;

    public SoulMasterHelper() => instance = this;

    private protected override string SceneName => SoulMasterScene;
    private protected override string BossObjectName => SoulMasterName;
    private protected override int P5Phase1Hp => P5SoulMasterPhase1Hp;
    private protected override bool P5HpEnabled { get => soulMasterP5Hp; set => soulMasterP5Hp = value; }
    private protected override int Phase1Hp { get => soulMasterPhase1Hp; set => soulMasterPhase1Hp = value; }
    private protected override int Phase2Hp { get => soulMasterPhase2Hp; set => soulMasterPhase2Hp = value; }
    private protected override int Phase1HpBeforeP5 { get => soulMasterPhase1HpBeforeP5; set => soulMasterPhase1HpBeforeP5 = value; }
    private protected override int Phase2HpBeforeP5 { get => soulMasterPhase2HpBeforeP5; set => soulMasterPhase2HpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => soulMasterHasStoredStateBeforeP5; set => soulMasterHasStoredStateBeforeP5 = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplySoulMasterHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();
}
