namespace GodhomeQoL.Modules.BossChallenge;

public sealed class SoulWarriorP1Helper : RegularBossHpHelper
{
    private const string SoulWarriorScene = "GG_Mage_Knight";
    private const string SoulWarriorName = "Mage Knight";
    private const int DefaultSoulWarriorMaxHp = 750;
    private const int DefaultSoulWarriorVanillaHp = 750;

    [LocalSetting]
    [BoolOption]
    internal static bool soulWarriorP1UseMaxHp = false;

    [LocalSetting]
    internal static int soulWarriorP1MaxHp = DefaultSoulWarriorMaxHp;

    private static SoulWarriorP1Helper? instance;

    public SoulWarriorP1Helper() => instance = this;

    private protected override string SceneName => SoulWarriorScene;
    private protected override string BossObjectName => SoulWarriorName;
    private protected override int DefaultVanillaHp => DefaultSoulWarriorVanillaHp;
    private protected override bool UseMaxHp { get => soulWarriorP1UseMaxHp; set => soulWarriorP1UseMaxHp = value; }
    private protected override int MaxHp { get => soulWarriorP1MaxHp; set => soulWarriorP1MaxHp = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void ApplySoulWarriorHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();
}
