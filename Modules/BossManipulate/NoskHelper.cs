namespace GodhomeQoL.Modules.BossChallenge;

public sealed class NoskHelper : MimicSpiderBossHelper
{
    private const string NoskScene = "GG_Nosk_V";
    private const int DefaultNoskMaxHp = 980;

    [LocalSetting]
    [BoolOption]
    internal static bool noskUseMaxHp = false;

    [LocalSetting]
    internal static int noskMaxHp = DefaultNoskMaxHp;

    [LocalSetting]
    internal static bool noskUseCustomPhase = false;

    [LocalSetting]
    internal static int noskPhase2Hp = DefaultBossPhase2Hp;

    private static NoskHelper? instance;

    public NoskHelper() => instance = this;

    private protected override string SceneName => NoskScene;
    private protected override bool UseMaxHp { get => noskUseMaxHp; set => noskUseMaxHp = value; }
    private protected override int MaxHp { get => noskMaxHp; set => noskMaxHp = value; }
    private protected override bool UseCustomPhase { get => noskUseCustomPhase; set => noskUseCustomPhase = value; }
    private protected override int Phase2Hp { get => noskPhase2Hp; set => noskPhase2Hp = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void ApplyNoskHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyPhaseThresholdSettingsIfPresent() => instance?.ApplyPhaseThresholdSettingsIfPresentCore();

    internal static void RestoreVanillaPhaseThresholdsIfPresent() => instance?.RestoreVanillaPhaseThresholdsIfPresentCore();

    internal static int GetPhase2MaxHpForUi() => instance != null ? instance.GetPhase2MaxHpForUiCore() : default;
}
