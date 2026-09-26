using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class AbsoluteRadianceHelper : Module
{
    private const string AbsoluteRadiancePhaseControlFsmName = "Phase Control";
    private const string AbsoluteRadianceFinalPhaseStateName = "Scream";
    private const string AbsoluteRadiancePhase2VariableName = "P2 Spike Waves";
    private const string AbsoluteRadiancePhase3VariableName = "P3 A1 Rage";
    private const string AbsoluteRadiancePhase4VariableName = "P4 Stun1";
    private const string AbsoluteRadiancePhase5VariableName = "P5 Acend";
    private const int DefaultAbsoluteRadiancePhase2Hp = 2600;
    private const int DefaultAbsoluteRadiancePhase3Hp = 2150;
    private const int DefaultAbsoluteRadiancePhase4Hp = 1850;
    private const int DefaultAbsoluteRadiancePhase5Hp = 1100;
    private const int DefaultAbsoluteRadianceFinalPhaseHp = 1000;
    private const int MinAbsoluteRadiancePhaseHp = 1;

    internal static void ApplyPhaseThresholdSettingsIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null)
            {
                continue;
            }

            if (IsAbsoluteRadiancePhaseControlFsm(fsm))
            {
                ApplyPhaseThresholdSettings(fsm);
            }

            if (IsAbsoluteRadianceControlFsm(fsm))
            {
                ApplyFinalPhaseHpSetting(fsm);
            }
        }
    }

    internal static void RestoreVanillaPhaseThresholdsIfPresent()
    {
        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null)
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            if (IsAbsoluteRadiancePhaseControlFsm(fsm))
            {
                RememberVanillaPhaseThresholds(fsm);
                (int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp) = GetVanillaPhaseThresholds(fsm);
                SetPhaseThresholdsOnFsm(fsm, phase2Hp, phase3Hp, phase4Hp, phase5Hp);
            }

            if (IsAbsoluteRadianceControlFsm(fsm))
            {
                RememberVanillaFinalPhaseHp(fsm);
                SetFinalPhaseHpOnFsm(fsm, GetVanillaFinalPhaseHp(fsm));
            }
        }
    }

    private static bool TryGetFinalPhaseSetHpAction(PlayMakerFSM fsm, out SetHP? setHp)
    {
        setHp = null;
        FsmState? scream = fsm.Fsm?.GetState(AbsoluteRadianceFinalPhaseStateName);
        if (scream?.Actions == null)
        {
            return false;
        }

        foreach (FsmStateAction action in scream.Actions)
        {
            if (action is SetHP setHpAction)
            {
                setHp = setHpAction;
                return true;
            }
        }

        return false;
    }

    private static void ApplyFinalPhaseHpSetting(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsAbsoluteRadianceControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        RememberVanillaFinalPhaseHp(fsm);
        int targetFinalHp = ShouldUseCustomPhaseThresholds()
            ? ClampAbsoluteRadianceFinalPhaseHp(absoluteRadianceFinalPhaseHp)
            : GetVanillaFinalPhaseHp(fsm);

        SetFinalPhaseHpOnFsm(fsm, targetFinalHp);
    }

    private static void RememberVanillaFinalPhaseHp(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaFinalPhaseHpByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int vanillaHp = DefaultAbsoluteRadianceFinalPhaseHp;
        if (TryGetFinalPhaseSetHpAction(fsm, out SetHP? setHp) && setHp?.hp != null && !setHp.hp.IsNone)
        {
            int candidate = setHp.hp.Value;
            if (candidate > 0)
            {
                vanillaHp = candidate;
            }
        }

        vanillaFinalPhaseHpByFsm[fsmId] = ClampAbsoluteRadianceFinalPhaseHp(vanillaHp);
    }

    private static int GetVanillaFinalPhaseHp(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaFinalPhaseHpByFsm.TryGetValue(fsmId, out int hp) && hp > 0)
        {
            return ClampAbsoluteRadianceFinalPhaseHp(hp);
        }

        RememberVanillaFinalPhaseHp(fsm);
        return vanillaFinalPhaseHpByFsm.TryGetValue(fsmId, out hp)
            ? ClampAbsoluteRadianceFinalPhaseHp(hp)
            : DefaultAbsoluteRadianceFinalPhaseHp;
    }

    private static void SetFinalPhaseHpOnFsm(PlayMakerFSM fsm, int hp)
    {
        if (!TryGetFinalPhaseSetHpAction(fsm, out SetHP? setHp) || setHp?.hp == null)
        {
            return;
        }

        setHp.hp.UseVariable = false;
        setHp.hp.Value = ClampAbsoluteRadianceFinalPhaseHp(hp);
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsAbsoluteRadiancePhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        RememberVanillaPhaseThresholds(fsm);
        (int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp) thresholds = ShouldUseCustomPhaseThresholds()
            ? GetCustomPhaseThresholds()
            : GetVanillaPhaseThresholds(fsm);

        SetPhaseThresholdsOnFsm(
            fsm,
            thresholds.phase2Hp,
            thresholds.phase3Hp,
            thresholds.phase4Hp,
            thresholds.phase5Hp
        );
    }

    private static (int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp) GetCustomPhaseThresholds()
    {
        NormalizeCustomPhaseThresholdState();
        return (absoluteRadiancePhase2Hp, absoluteRadiancePhase3Hp, absoluteRadiancePhase4Hp, absoluteRadiancePhase5Hp);
    }

    private static void SetPhaseThresholdsOnFsm(PlayMakerFSM fsm, int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp)
    {
        (int p2, int p3, int p4, int p5) = NormalizePhaseThresholdChain(phase2Hp, phase3Hp, phase4Hp, phase5Hp);

        SetFsmIntVariableValue(fsm, AbsoluteRadiancePhase2VariableName, p2);
        SetFsmIntVariableValue(fsm, AbsoluteRadiancePhase3VariableName, p3);
        SetFsmIntVariableValue(fsm, AbsoluteRadiancePhase4VariableName, p4);
        SetFsmIntVariableValue(fsm, AbsoluteRadiancePhase5VariableName, p5);

        SetCheckStateThreshold(fsm, "Check 1", AbsoluteRadiancePhase2VariableName, p2);
        SetCheckStateThreshold(fsm, "Check 2", AbsoluteRadiancePhase3VariableName, p3);
        SetCheckStateThreshold(fsm, "Check 3", AbsoluteRadiancePhase4VariableName, p4);
        SetCheckStateThreshold(fsm, "Check 4", AbsoluteRadiancePhase5VariableName, p5);
    }

    private static void SetFsmIntVariableValue(PlayMakerFSM fsm, string variableName, int value)
    {
        FsmInt? fsmInt = fsm.FsmVariables.GetFsmInt(variableName);
        if (fsmInt != null)
        {
            fsmInt.Value = Math.Max(MinAbsoluteRadiancePhaseHp, value);
        }
    }

    private static void SetCheckStateThreshold(PlayMakerFSM fsm, string stateName, string variableName, int value)
    {
        FsmState? state = fsm.Fsm?.GetState(stateName);
        if (state?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in state.Actions)
        {
            if (action is not IntCompare compare || compare.integer2 == null)
            {
                continue;
            }

            compare.integer2.UseVariable = true;
            compare.integer2.Name = variableName;
            compare.integer2.Value = Math.Max(MinAbsoluteRadiancePhaseHp, value);
            break;
        }
    }

    private static void RememberVanillaPhaseThresholds(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaPhaseThresholdsByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int phase2Hp = ReadFsmIntVariableValue(fsm, AbsoluteRadiancePhase2VariableName, DefaultAbsoluteRadiancePhase2Hp);
        int phase3Hp = ReadFsmIntVariableValue(fsm, AbsoluteRadiancePhase3VariableName, DefaultAbsoluteRadiancePhase3Hp);
        int phase4Hp = ReadFsmIntVariableValue(fsm, AbsoluteRadiancePhase4VariableName, DefaultAbsoluteRadiancePhase4Hp);
        int phase5Hp = ReadFsmIntVariableValue(fsm, AbsoluteRadiancePhase5VariableName, DefaultAbsoluteRadiancePhase5Hp);

        phase2Hp = ReadCheckStateThreshold(fsm, "Check 1", phase2Hp);
        phase3Hp = ReadCheckStateThreshold(fsm, "Check 2", phase3Hp);
        phase4Hp = ReadCheckStateThreshold(fsm, "Check 3", phase4Hp);
        phase5Hp = ReadCheckStateThreshold(fsm, "Check 4", phase5Hp);

        vanillaPhaseThresholdsByFsm[fsmId] = NormalizePhaseThresholdChain(phase2Hp, phase3Hp, phase4Hp, phase5Hp);
    }

    private static int ReadFsmIntVariableValue(PlayMakerFSM fsm, string variableName, int fallback)
    {
        FsmInt? fsmInt = fsm.FsmVariables.GetFsmInt(variableName);
        if (fsmInt != null && fsmInt.Value > 0)
        {
            return fsmInt.Value;
        }

        return fallback;
    }

    private static int ReadCheckStateThreshold(PlayMakerFSM fsm, string stateName, int fallback)
    {
        FsmState? state = fsm.Fsm?.GetState(stateName);
        if (state?.Actions == null)
        {
            return fallback;
        }

        foreach (FsmStateAction action in state.Actions)
        {
            if (action is IntCompare compare && compare.integer2 != null && compare.integer2.Value > 0)
            {
                return compare.integer2.Value;
            }
        }

        return fallback;
    }

    private static (int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp) GetVanillaPhaseThresholds(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaPhaseThresholdsByFsm.TryGetValue(fsmId, out (int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp) thresholds))
        {
            return thresholds;
        }

        RememberVanillaPhaseThresholds(fsm);
        if (vanillaPhaseThresholdsByFsm.TryGetValue(fsmId, out thresholds))
        {
            return thresholds;
        }

        return (
            DefaultAbsoluteRadiancePhase2Hp,
            DefaultAbsoluteRadiancePhase3Hp,
            DefaultAbsoluteRadiancePhase4Hp,
            DefaultAbsoluteRadiancePhase5Hp
        );
    }

    private static (int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp) NormalizePhaseThresholdChain(int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp)
    {
        int p2 = Math.Max(MinAbsoluteRadiancePhaseHp, phase2Hp);
        int p3 = ClampAbsoluteRadiancePhaseHpBelowPrevious(phase3Hp, p2);
        int p4 = ClampAbsoluteRadiancePhaseHpBelowPrevious(phase4Hp, p3);
        int p5 = ClampAbsoluteRadiancePhaseHpBelowPrevious(phase5Hp, p4);
        return (p2, p3, p4, p5);
    }

    private static int ClampAbsoluteRadianceFinalPhaseHp(int value) => ClampAbsoluteRadianceHp(value);

    private static int ClampAbsoluteRadiancePhase2Hp(int value, int maxHp)
    {
        int clampedMaxHp = Math.Max(MinAbsoluteRadiancePhaseHp, ClampAbsoluteRadianceHp(maxHp));
        if (value < MinAbsoluteRadiancePhaseHp)
        {
            return MinAbsoluteRadiancePhaseHp;
        }

        return value > clampedMaxHp ? clampedMaxHp : value;
    }

    private static int ClampAbsoluteRadiancePhaseHpBelowPrevious(int value, int previousPhaseHp)
    {
        int maxValue = Math.Max(MinAbsoluteRadiancePhaseHp, previousPhaseHp - 1);
        if (value < MinAbsoluteRadiancePhaseHp)
        {
            return MinAbsoluteRadiancePhaseHp;
        }

        return value > maxValue ? maxValue : value;
    }
}
