namespace GodhomeQoL.Modules.BossChallenge;

public sealed class FailedChampionHelper : ArmoredBossHelper
{
    private const string FailedChampionScene = "GG_Failed_Champion";
    private const int DefaultArmorPhaseHpValue = 600;
    private const int P5ArmorPhaseHpValue = 360;

    [LocalSetting]
    [BoolOption]
    internal static bool failedChampionP5Hp = false;

    [LocalSetting]
    internal static int failedChampionArmorPhase1Hp = DefaultArmorPhaseHpValue;

    [LocalSetting]
    internal static int failedChampionArmorPhase2Hp = DefaultArmorPhaseHpValue;

    [LocalSetting]
    internal static int failedChampionArmorPhase3Hp = DefaultArmorPhaseHpValue;

    [LocalSetting]
    internal static int failedChampionArmorPhase1HpBeforeP5 = DefaultArmorPhaseHpValue;

    [LocalSetting]
    internal static int failedChampionArmorPhase2HpBeforeP5 = DefaultArmorPhaseHpValue;

    [LocalSetting]
    internal static int failedChampionArmorPhase3HpBeforeP5 = DefaultArmorPhaseHpValue;

    [LocalSetting]
    internal static bool failedChampionHasStoredStateBeforeP5 = false;

    private static FailedChampionHelper? instance;

    public FailedChampionHelper() => instance = this;

    private protected override string SceneName => FailedChampionScene;
    private protected override int DefaultArmorPhaseHp => DefaultArmorPhaseHpValue;
    private protected override int P5ArmorPhaseHp => P5ArmorPhaseHpValue;
    private protected override bool P5HpEnabled { get => failedChampionP5Hp; set => failedChampionP5Hp = value; }
    private protected override int ArmorPhase1Hp { get => failedChampionArmorPhase1Hp; set => failedChampionArmorPhase1Hp = value; }
    private protected override int ArmorPhase2Hp { get => failedChampionArmorPhase2Hp; set => failedChampionArmorPhase2Hp = value; }
    private protected override int ArmorPhase3Hp { get => failedChampionArmorPhase3Hp; set => failedChampionArmorPhase3Hp = value; }
    private protected override int ArmorPhase1HpBeforeP5 { get => failedChampionArmorPhase1HpBeforeP5; set => failedChampionArmorPhase1HpBeforeP5 = value; }
    private protected override int ArmorPhase2HpBeforeP5 { get => failedChampionArmorPhase2HpBeforeP5; set => failedChampionArmorPhase2HpBeforeP5 = value; }
    private protected override int ArmorPhase3HpBeforeP5 { get => failedChampionArmorPhase3HpBeforeP5; set => failedChampionArmorPhase3HpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => failedChampionHasStoredStateBeforeP5; set => failedChampionHasStoredStateBeforeP5 = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyFailedChampionSettingsIfPresent(bool forceReapply = false) => instance?.ApplySettingsIfPresentCore(forceReapply);

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();
}
