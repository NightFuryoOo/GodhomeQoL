namespace GodhomeQoL.Modules.BossChallenge;

public sealed class BroodingMawlekP1Helper : RegularBossHpHelper
{
    private const string MawlekScene = "GG_Brooding_Mawlek";
    private const string MawlekName = "Mawlek Body";
    private const int DefaultMawlekMaxHp = 1050;
    private const int DefaultMawlekVanillaHp = 1050;

    [LocalSetting]
    [BoolOption]
    internal static bool broodingMawlekP1UseMaxHp = false;

    [LocalSetting]
    internal static int broodingMawlekP1MaxHp = DefaultMawlekMaxHp;

    private static BroodingMawlekP1Helper? instance;

    public BroodingMawlekP1Helper() => instance = this;

    private protected override string SceneName => MawlekScene;
    private protected override string BossObjectName => MawlekName;
    private protected override int DefaultVanillaHp => DefaultMawlekVanillaHp;
    private protected override bool UseMaxHp { get => broodingMawlekP1UseMaxHp; set => broodingMawlekP1UseMaxHp = value; }
    private protected override int MaxHp { get => broodingMawlekP1MaxHp; set => broodingMawlekP1MaxHp = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void ApplyMawlekHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();
}
