namespace GodhomeQoL.Modules.BossChallenge;

public sealed class MarmuP2Helper : RegularBossHpHelper
{
    private const string MarmuScene = "GG_Ghost_Marmu";
    private const string MarmuName = "Ghost Warrior Marmu";
    private const int DefaultMarmuMaxHp = 416;
    private const int DefaultMarmuVanillaHp = 416;

    [LocalSetting]
    [BoolOption]
    internal static bool marmuP2UseMaxHp = false;

    [LocalSetting]
    internal static int marmuP2MaxHp = DefaultMarmuMaxHp;

    private static MarmuP2Helper? instance;

    public MarmuP2Helper() => instance = this;

    private protected override string SceneName => MarmuScene;
    private protected override string BossObjectName => MarmuName;
    private protected override int DefaultVanillaHp => DefaultMarmuVanillaHp;
    private protected override bool UseMaxHp { get => marmuP2UseMaxHp; set => marmuP2UseMaxHp = value; }
    private protected override int MaxHp { get => marmuP2MaxHp; set => marmuP2MaxHp = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void ApplyMarmuHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();
}
