namespace GodhomeQoL.Modules.BossChallenge;

public sealed class FalseKnightHelper : ArmoredBossHelper
{
    private const string FalseKnightScene = "GG_False_Knight";
    private const int DefaultArmorPhaseHpValue = 560;
    private const int P5ArmorPhaseHpValue = 260;

    [LocalSetting]
    [BoolOption]
    internal static bool falseKnightP5Hp = false;

    [LocalSetting]
    internal static int falseKnightArmorPhase1Hp = DefaultArmorPhaseHpValue;

    [LocalSetting]
    internal static int falseKnightArmorPhase2Hp = DefaultArmorPhaseHpValue;

    [LocalSetting]
    internal static int falseKnightArmorPhase3Hp = DefaultArmorPhaseHpValue;

    [LocalSetting]
    internal static int falseKnightArmorPhase1HpBeforeP5 = DefaultArmorPhaseHpValue;

    [LocalSetting]
    internal static int falseKnightArmorPhase2HpBeforeP5 = DefaultArmorPhaseHpValue;

    [LocalSetting]
    internal static int falseKnightArmorPhase3HpBeforeP5 = DefaultArmorPhaseHpValue;

    [LocalSetting]
    internal static bool falseKnightHasStoredStateBeforeP5 = false;

    private static FalseKnightHelper? instance;

    public FalseKnightHelper() => instance = this;

    private protected override string SceneName => FalseKnightScene;
    private protected override int DefaultArmorPhaseHp => DefaultArmorPhaseHpValue;
    private protected override int P5ArmorPhaseHp => P5ArmorPhaseHpValue;
    private protected override bool P5HpEnabled { get => falseKnightP5Hp; set => falseKnightP5Hp = value; }
    private protected override int ArmorPhase1Hp { get => falseKnightArmorPhase1Hp; set => falseKnightArmorPhase1Hp = value; }
    private protected override int ArmorPhase2Hp { get => falseKnightArmorPhase2Hp; set => falseKnightArmorPhase2Hp = value; }
    private protected override int ArmorPhase3Hp { get => falseKnightArmorPhase3Hp; set => falseKnightArmorPhase3Hp = value; }
    private protected override int ArmorPhase1HpBeforeP5 { get => falseKnightArmorPhase1HpBeforeP5; set => falseKnightArmorPhase1HpBeforeP5 = value; }
    private protected override int ArmorPhase2HpBeforeP5 { get => falseKnightArmorPhase2HpBeforeP5; set => falseKnightArmorPhase2HpBeforeP5 = value; }
    private protected override int ArmorPhase3HpBeforeP5 { get => falseKnightArmorPhase3HpBeforeP5; set => falseKnightArmorPhase3HpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => falseKnightHasStoredStateBeforeP5; set => falseKnightHasStoredStateBeforeP5 = value; }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyFalseKnightSettingsIfPresent(bool forceReapply = false) => instance?.ApplySettingsIfPresentCore(forceReapply);

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();
}
