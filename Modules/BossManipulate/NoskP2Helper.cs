namespace GodhomeQoL.Modules.BossChallenge;

public sealed class NoskP2Helper : MimicSpiderBossHelper
{
    private const string NoskScene = "GG_Nosk";
    private const int DefaultNoskMaxHp = 680;

    [LocalSetting]
    [BoolOption]
    internal static bool noskP2UseMaxHp = false;

    [LocalSetting]
    internal static int noskP2MaxHp = DefaultNoskMaxHp;

    [LocalSetting]
    internal static bool noskP2UseCustomPhase = false;

    [LocalSetting]
    internal static int noskP2Phase2Hp = DefaultBossPhase2Hp;

    private static NoskP2Helper? instance;

    public NoskP2Helper() => instance = this;

    private protected override string SceneName => NoskScene;
    private protected override bool UseMaxHp { get => noskP2UseMaxHp; set => noskP2UseMaxHp = value; }
    private protected override int MaxHp { get => noskP2MaxHp; set => noskP2MaxHp = value; }
    private protected override bool UseCustomPhase { get => noskP2UseCustomPhase; set => noskP2UseCustomPhase = value; }
    private protected override int Phase2Hp { get => noskP2Phase2Hp; set => noskP2Phase2Hp = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void ApplyNoskHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyPhaseThresholdSettingsIfPresent() => instance?.ApplyPhaseThresholdSettingsIfPresentCore();

    internal static void RestoreVanillaPhaseThresholdsIfPresent() => instance?.RestoreVanillaPhaseThresholdsIfPresentCore();

    internal static int GetPhase2MaxHpForUi() => instance != null ? instance.GetPhase2MaxHpForUiCore() : default;
}
