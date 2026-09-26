using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class PureVesselHelper : Module
{
    private const string PureVesselPhase2VariableName = "Half HP";
    private const string PureVesselPhase3VariableName = "Quarter HP";
    private const int DefaultPureVesselPhase2Hp = 1232;
    private const int DefaultPureVesselPhase3Hp = 616;
    private const int MinPureVesselPhase2Hp = 2;
    private const int MinPureVesselPhase3Hp = 1;

    internal static void ApplyPhaseThresholdSettingsIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsPureVesselPhaseControlFsm(fsm))
            {
                continue;
            }

            ApplyPhaseThresholdSettings(fsm);
        }
    }

    internal static void RestoreVanillaPhaseThresholdsIfPresent()
    {
        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsPureVesselPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            SetPhaseThresholdsOnFsm(
                fsm,
                DefaultPureVesselPhase2Hp,
                DefaultPureVesselPhase3Hp,
                useVanillaVariables: true
            );
        }
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsPureVesselPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        if (ShouldUseCustomPhaseThresholds())
        {
            SetPhaseThresholdsOnFsm(
                fsm,
                ClampPureVesselPhase2Hp(pureVesselPhase2Hp),
                ClampPureVesselPhase3Hp(pureVesselPhase3Hp, pureVesselPhase2Hp),
                useVanillaVariables: false
            );
        }
        else
        {
            SetPhaseThresholdsOnFsm(
                fsm,
                DefaultPureVesselPhase2Hp,
                DefaultPureVesselPhase3Hp,
                useVanillaVariables: true
            );
        }
    }

    private static void SetPhaseThresholdsOnFsm(PlayMakerFSM fsm, int phase2Hp, int phase3Hp, bool useVanillaVariables)
    {
        FsmState? phaseState = fsm.Fsm?.GetState("Phase?");
        if (phaseState?.Actions == null)
        {
            return;
        }

        int targetPhase2Hp = ClampPureVesselPhase2Hp(phase2Hp);
        int targetPhase3Hp = ClampPureVesselPhase3Hp(phase3Hp, targetPhase2Hp);

        if (!useVanillaVariables)
        {
            FsmInt? phase2Variable = fsm.FsmVariables.GetFsmInt(PureVesselPhase2VariableName);
            if (phase2Variable != null)
            {
                phase2Variable.Value = targetPhase2Hp;
            }

            FsmInt? phase3Variable = fsm.FsmVariables.GetFsmInt(PureVesselPhase3VariableName);
            if (phase3Variable != null)
            {
                phase3Variable.Value = targetPhase3Hp;
            }
        }

        int compareIndex = 0;
        foreach (FsmStateAction action in phaseState.Actions)
        {
            if (action is not IntCompare compare || compare.integer2 == null)
            {
                continue;
            }

            if (useVanillaVariables)
            {
                compare.integer2.UseVariable = true;
                compare.integer2.Name = compareIndex == 0 ? PureVesselPhase2VariableName : PureVesselPhase3VariableName;
            }
            else
            {
                compare.integer2.UseVariable = false;
                compare.integer2.Name = string.Empty;
                compare.integer2.Value = compareIndex == 0 ? targetPhase2Hp : targetPhase3Hp;
            }

            compareIndex++;
            if (compareIndex >= 2)
            {
                break;
            }
        }
    }

    internal static int GetPhase2MaxHpForUi() => ResolvePhase2MaxHp();

    internal static int GetPhase3MaxHpForUi()
    {
        int clampedPhase2Hp = ClampPureVesselPhase2Hp(pureVesselPhase2Hp);
        return Math.Max(MinPureVesselPhase3Hp, clampedPhase2Hp - 1);
    }

    private static int ClampPureVesselPhase2Hp(int value)
    {
        int maxPhase2Hp = ResolvePhase2MaxHp();
        if (value < MinPureVesselPhase2Hp)
        {
            return MinPureVesselPhase2Hp;
        }

        return value > maxPhase2Hp ? maxPhase2Hp : value;
    }

    private static int ResolvePhase2MaxHp()
    {
        int referenceHp = pureVesselUseMaxHp || pureVesselP5Hp
            ? pureVesselMaxHp
            : DefaultPureVesselVanillaHp;

        return Math.Max(MinPureVesselPhase2Hp, ClampPureVesselHp(referenceHp));
    }

    private static int ClampPureVesselPhase3Hp(int value, int phase2Hp)
    {
        int clampedPhase2Hp = ClampPureVesselPhase2Hp(phase2Hp);
        int maxPhase3Hp = Math.Max(MinPureVesselPhase3Hp, clampedPhase2Hp - 1);

        if (value < MinPureVesselPhase3Hp)
        {
            return MinPureVesselPhase3Hp;
        }

        return value > maxPhase3Hp ? maxPhase3Hp : value;
    }
}
