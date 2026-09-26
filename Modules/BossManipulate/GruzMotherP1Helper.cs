namespace GodhomeQoL.Modules.BossChallenge;

public sealed class GruzMotherP1Helper : RegularBossHpHelper
{
    private const string GruzScene = "GG_Gruz_Mother";
    private const string GruzName = "Giant Fly";
    private const int DefaultGruzMaxHp = 650;
    private const int DefaultGruzVanillaHp = 945;

    [LocalSetting]
    [BoolOption]
    internal static bool gruzMotherP1UseMaxHp = false;

    [LocalSetting]
    internal static int gruzMotherP1MaxHp = DefaultGruzMaxHp;

    private static GruzMotherP1Helper? instance;

    public GruzMotherP1Helper() => instance = this;

    private protected override string SceneName => GruzScene;
    private protected override string BossObjectName => GruzName;
    private protected override int DefaultVanillaHp => DefaultGruzVanillaHp;
    private protected override bool UseMaxHp { get => gruzMotherP1UseMaxHp; set => gruzMotherP1UseMaxHp = value; }
    private protected override int MaxHp { get => gruzMotherP1MaxHp; set => gruzMotherP1MaxHp = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void ApplyGruzHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();
}
