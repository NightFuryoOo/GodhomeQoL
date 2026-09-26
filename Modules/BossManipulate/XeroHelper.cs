using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class XeroHelper : RageBossHelper
{
    private const string XeroScene = "GG_Ghost_Xero_V";
    private const string XeroName = "Ghost Warrior Xero";
    private const string XeroPhaseFsmName = "Sword Summon";
    private const string XeroPhaseCheckStateName = "Check";
    private const string XeroPhase2VariableName = "Half HP";
    private const string XeroAttackingFsmName = "Attacking";
    private const string XeroRageVariableName = "Rage";
    private const string XeroSummonEventName = "SUMMON";
    private const int DefaultXeroMaxHp = 900;
    private const int DefaultXeroVanillaHp = 900;
    private const int DefaultXeroPhase2Hp = 450;
    private const int P5XeroHp = 650;
    private const int MinXeroHp = 1;
    private const int MaxXeroHp = 999999;
    private const int MinXeroPhase2Hp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool xeroUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool xeroP5Hp = false;

    [LocalSetting]
    internal static int xeroMaxHp = DefaultXeroMaxHp;

    [LocalSetting]
    internal static int xeroMaxHpBeforeP5 = DefaultXeroMaxHp;

    [LocalSetting]
    internal static bool xeroUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool xeroHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool xeroUseCustomPhase = false;

    [LocalSetting]
    internal static int xeroPhase2Hp = DefaultXeroPhase2Hp;

    [LocalSetting]
    internal static bool xeroUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int xeroPhase2HpBeforeP5 = DefaultXeroPhase2Hp;

    private static XeroHelper? instance;

    public XeroHelper() => instance = this;

    private protected override int P5Hp => P5XeroHp;
    private protected override string SceneName => XeroScene;
    private protected override string BossObjectName => XeroName;
    private protected override string PhaseFsmName => XeroPhaseFsmName;
    private protected override string PhaseCheckStateName => XeroPhaseCheckStateName;
    private protected override string Phase2VariableName => XeroPhase2VariableName;
    private protected override string AttackingFsmName => XeroAttackingFsmName;
    private protected override string RageVariableName => XeroRageVariableName;
    private protected override string SummonEventName => XeroSummonEventName;
    private protected override int DefaultVanillaHp => DefaultXeroVanillaHp;
    private protected override int MinHp => MinXeroHp;
    private protected override int MaxHpLimit => MaxXeroHp;
    private protected override int MinPhase2Hp => MinXeroPhase2Hp;
    private protected override bool UseMaxHp { get => xeroUseMaxHp; set => xeroUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => xeroP5Hp; set => xeroP5Hp = value; }
    private protected override int MaxHp { get => xeroMaxHp; set => xeroMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => xeroMaxHpBeforeP5; set => xeroMaxHpBeforeP5 = value; }
    private protected override bool UseMaxHpBeforeP5 { get => xeroUseMaxHpBeforeP5; set => xeroUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => xeroHasStoredStateBeforeP5; set => xeroHasStoredStateBeforeP5 = value; }
    private protected override bool UseCustomPhase { get => xeroUseCustomPhase; set => xeroUseCustomPhase = value; }
    private protected override int Phase2Hp { get => xeroPhase2Hp; set => xeroPhase2Hp = value; }
    private protected override bool UseCustomPhaseBeforeP5 { get => xeroUseCustomPhaseBeforeP5; set => xeroUseCustomPhaseBeforeP5 = value; }
    private protected override int Phase2HpBeforeP5 { get => xeroPhase2HpBeforeP5; set => xeroPhase2HpBeforeP5 = value; }

    private protected override bool IsBossPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null)
        {
            return false;
        }

        if (!string.Equals(fsm.gameObject.scene.name, XeroScene, StringComparison.Ordinal))
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, XeroPhaseFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState(XeroPhaseCheckStateName) != null
            && fsm.FsmVariables.GetFsmInt(XeroPhase2VariableName) != null;
    }

    private protected override bool ShouldApplyPhaseSettings(GameObject? gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, XeroScene, StringComparison.Ordinal) && hoGEntryAllowed;
    }

    private protected override void SetPhase2ThresholdOnFsm(PlayMakerFSM fsm, int value, bool useVanillaVariable)
    {
        if (fsm == null)
        {
            return;
        }

        int threshold = ClampBossPhase2Hp(value, ResolvePhase2MaxHp());
        FsmInt? halfHpVariable = fsm.FsmVariables.GetFsmInt(XeroPhase2VariableName);
        if (halfHpVariable != null)
        {
            halfHpVariable.Value = threshold;
        }

        FsmState? checkState = fsm.Fsm?.GetState(XeroPhaseCheckStateName);
        if (checkState?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in checkState.Actions)
        {
            if (action is IntCompare compare && compare.integer2 != null)
            {
                if (useVanillaVariable)
                {
                    compare.integer2.UseVariable = true;
                    compare.integer2.Name = XeroPhase2VariableName;
                }
                else
                {
                    compare.integer2.UseVariable = false;
                    compare.integer2.Name = string.Empty;
                    compare.integer2.Value = threshold;
                }
            }
        }
    }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyXeroHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyPhaseThresholdSettingsIfPresent() => instance?.ApplyPhaseThresholdSettingsIfPresentCore();

    internal static void RestoreVanillaPhaseThresholdsIfPresent() => instance?.RestoreVanillaPhaseThresholdsIfPresentCore();
}
