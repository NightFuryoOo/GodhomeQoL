namespace GodhomeQoL.Modules.BossChallenge;

public sealed class SoulTyrantHelper : MageLordBossHelper
{
    private const string SoulTyrantScene = "GG_Soul_Tyrant";
    private const string SoulTyrantName = "Dream Mage Lord";
    private const int DefaultSoulTyrantPhase1Hp = 1200;
    private const int DefaultSoulTyrantPhase2Hp = 650;
    private const int P5SoulTyrantPhase1Hp = 900;

    [LocalSetting]
    [BoolOption]
    internal static bool soulTyrantP5Hp = false;

    [LocalSetting]
    internal static int soulTyrantPhase1Hp = DefaultSoulTyrantPhase1Hp;

    [LocalSetting]
    internal static int soulTyrantPhase2Hp = DefaultSoulTyrantPhase2Hp;

    [LocalSetting]
    internal static int soulTyrantPhase1HpBeforeP5 = DefaultSoulTyrantPhase1Hp;

    [LocalSetting]
    internal static int soulTyrantPhase2HpBeforeP5 = DefaultSoulTyrantPhase2Hp;

    [LocalSetting]
    internal static bool soulTyrantHasStoredStateBeforeP5 = false;

    private static SoulTyrantHelper? instance;

    public SoulTyrantHelper() => instance = this;

    private protected override string SceneName => SoulTyrantScene;
    private protected override string BossObjectName => SoulTyrantName;
    private protected override int P5Phase1Hp => P5SoulTyrantPhase1Hp;
    private protected override bool P5HpEnabled { get => soulTyrantP5Hp; set => soulTyrantP5Hp = value; }
    private protected override int Phase1Hp { get => soulTyrantPhase1Hp; set => soulTyrantPhase1Hp = value; }
    private protected override int Phase2Hp { get => soulTyrantPhase2Hp; set => soulTyrantPhase2Hp = value; }
    private protected override int Phase1HpBeforeP5 { get => soulTyrantPhase1HpBeforeP5; set => soulTyrantPhase1HpBeforeP5 = value; }
    private protected override int Phase2HpBeforeP5 { get => soulTyrantPhase2HpBeforeP5; set => soulTyrantPhase2HpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => soulTyrantHasStoredStateBeforeP5; set => soulTyrantHasStoredStateBeforeP5 = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplySoulTyrantHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();
}
