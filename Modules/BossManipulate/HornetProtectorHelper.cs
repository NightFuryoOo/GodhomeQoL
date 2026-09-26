namespace GodhomeQoL.Modules.BossChallenge;

public sealed class HornetProtectorHelper : SingleBossHpHelper
{
    private const string HornetScene = "GG_Hornet_1";
    private const string HornetName = "Hornet Boss 1";
    private const int DefaultHornetMaxHp = 1250;
    private const int DefaultHornetVanillaHp = 1250;
    private const int P5HornetHp = 900;

    [LocalSetting]
    [BoolOption]
    internal static bool hornetUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool hornetP5Hp = false;

    [LocalSetting]
    internal static int hornetMaxHp = DefaultHornetMaxHp;

    [LocalSetting]
    internal static int hornetMaxHpBeforeP5 = DefaultHornetMaxHp;

    [LocalSetting]
    internal static bool hornetUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool hornetHasStoredStateBeforeP5 = false;

    private static HornetProtectorHelper? instance;

    public HornetProtectorHelper() => instance = this;

    private protected override string SceneName => HornetScene;
    private protected override string BossObjectName => HornetName;
    private protected override int DefaultVanillaHp => DefaultHornetVanillaHp;
    private protected override int P5Hp => P5HornetHp;
    private protected override bool UseMaxHp { get => hornetUseMaxHp; set => hornetUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => hornetP5Hp; set => hornetP5Hp = value; }
    private protected override int MaxHp { get => hornetMaxHp; set => hornetMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => hornetMaxHpBeforeP5; set => hornetMaxHpBeforeP5 = value; }
    private protected override bool UseMaxHpBeforeP5 { get => hornetUseMaxHpBeforeP5; set => hornetUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => hornetHasStoredStateBeforeP5; set => hornetHasStoredStateBeforeP5 = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyHornetHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();
}
