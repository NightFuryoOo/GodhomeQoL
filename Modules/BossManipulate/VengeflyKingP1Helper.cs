namespace GodhomeQoL.Modules.BossChallenge;

public sealed class VengeflyKingP1Helper : RegularBossHpHelper
{
    private const string VengeflyScene = "GG_Vengefly";
    private const string VengeflyName = "Giant Buzzer Col";
    private const int DefaultVengeflyMaxHp = 450;
    private const int DefaultVengeflyVanillaHp = 450;

    [LocalSetting]
    [BoolOption]
    internal static bool vengeflyKingP1UseMaxHp = false;

    [LocalSetting]
    internal static int vengeflyKingP1MaxHp = DefaultVengeflyMaxHp;

    private static VengeflyKingP1Helper? instance;

    public VengeflyKingP1Helper() => instance = this;

    private protected override string SceneName => VengeflyScene;
    private protected override string BossObjectName => VengeflyName;
    private protected override int DefaultVanillaHp => DefaultVengeflyVanillaHp;
    private protected override bool UseMaxHp { get => vengeflyKingP1UseMaxHp; set => vengeflyKingP1UseMaxHp = value; }
    private protected override int MaxHp { get => vengeflyKingP1MaxHp; set => vengeflyKingP1MaxHp = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void ApplyVengeflyHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();
}
