using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class MarkothHelper : RageBossHelper
{
    private const string MarkothScene = "GG_Ghost_Markoth_V";
    private const string MarkothName = "Ghost Warrior Markoth";
    private const string MarkothPhaseFsmName = "Rage Check";
    private const string MarkothPhaseCheckStateName = "Check";
    private const string MarkothPhase2VariableName = "Rage HP";
    private const string MarkothAttackingFsmName = "Attacking";
    private const string MarkothShieldAttackFsmName = "Shield Attack";
    private const string MarkothRageVariableName = "Rage";
    private const string MarkothRageEventName = "RAGE";
    private const int DefaultMarkothMaxHp = 950;
    private const int DefaultMarkothVanillaHp = 950;
    private const int DefaultMarkothPhase2Hp = 475;
    private const int P5MarkothHp = 650;
    private const int MinMarkothHp = 1;
    private const int MaxMarkothHp = 999999;
    private const int MinMarkothPhase2Hp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool markothUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool markothP5Hp = false;

    [LocalSetting]
    internal static int markothMaxHp = DefaultMarkothMaxHp;

    [LocalSetting]
    internal static int markothMaxHpBeforeP5 = DefaultMarkothMaxHp;

    [LocalSetting]
    internal static bool markothUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool markothHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool markothUseCustomPhase = false;

    [LocalSetting]
    internal static int markothPhase2Hp = DefaultMarkothPhase2Hp;

    [LocalSetting]
    internal static bool markothUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int markothPhase2HpBeforeP5 = DefaultMarkothPhase2Hp;

    private static MarkothHelper? instance;

    public MarkothHelper() => instance = this;

    private protected override int P5Hp => P5MarkothHp;
    private protected override string SceneName => MarkothScene;
    private protected override string BossObjectName => MarkothName;
    private protected override string PhaseFsmName => MarkothPhaseFsmName;
    private protected override string PhaseCheckStateName => MarkothPhaseCheckStateName;
    private protected override string Phase2VariableName => MarkothPhase2VariableName;
    private protected override string AttackingFsmName => MarkothAttackingFsmName;
    private protected override string RageVariableName => MarkothRageVariableName;
    private protected override int DefaultVanillaHp => DefaultMarkothVanillaHp;
    private protected override int MinHp => MinMarkothHp;
    private protected override int MaxHpLimit => MaxMarkothHp;
    private protected override int MinPhase2Hp => MinMarkothPhase2Hp;
    private protected override bool UseMaxHp { get => markothUseMaxHp; set => markothUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => markothP5Hp; set => markothP5Hp = value; }
    private protected override int MaxHp { get => markothMaxHp; set => markothMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => markothMaxHpBeforeP5; set => markothMaxHpBeforeP5 = value; }
    private protected override bool UseMaxHpBeforeP5 { get => markothUseMaxHpBeforeP5; set => markothUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => markothHasStoredStateBeforeP5; set => markothHasStoredStateBeforeP5 = value; }
    private protected override bool UseCustomPhase { get => markothUseCustomPhase; set => markothUseCustomPhase = value; }
    private protected override int Phase2Hp { get => markothPhase2Hp; set => markothPhase2Hp = value; }
    private protected override bool UseCustomPhaseBeforeP5 { get => markothUseCustomPhaseBeforeP5; set => markothUseCustomPhaseBeforeP5 = value; }
    private protected override int Phase2HpBeforeP5 { get => markothPhase2HpBeforeP5; set => markothPhase2HpBeforeP5 = value; }

    private protected override bool ShouldApplyPhaseSettings(GameObject? gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, MarkothScene, StringComparison.Ordinal) && hoGEntryAllowed;
    }

    private protected override void TryForceBossRage(GameObject bossObject)
    {
        if (bossObject == null || !ShouldApplyPhaseSettings(bossObject))
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsBossObject(fsm.gameObject))
            {
                continue;
            }

            if (!string.Equals(fsm.FsmName, MarkothAttackingFsmName, StringComparison.Ordinal)
                && !string.Equals(fsm.FsmName, MarkothShieldAttackFsmName, StringComparison.Ordinal))
            {
                continue;
            }

            FsmBool? rageFlag = fsm.FsmVariables.GetFsmBool(MarkothRageVariableName);
            bool shouldSendEvent = true;
            if (rageFlag != null)
            {
                if (rageFlag.Value)
                {
                    shouldSendEvent = false;
                }
                else
                {
                    rageFlag.Value = true;
                }
            }

            if (shouldSendEvent)
            {
                fsm.SendEvent(MarkothRageEventName);
            }
        }
    }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyMarkothHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyPhaseThresholdSettingsIfPresent() => instance?.ApplyPhaseThresholdSettingsIfPresentCore();

    internal static void RestoreVanillaPhaseThresholdsIfPresent() => instance?.RestoreVanillaPhaseThresholdsIfPresentCore();
}
