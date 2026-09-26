using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class WingedNoskHelper : Module
{
    private const string WingedNoskPhaseCheckStateName = "Choose Attack";
    private const int DefaultWingedNoskPhase2Hp = 525;
    private const int MinWingedNoskPhase2Hp = 1;

    private static void NormalizePhaseThresholdState()
    {
        wingedNoskPhase2Hp = ClampWingedNoskPhase2Hp(wingedNoskPhase2Hp, ResolvePhase2MaxHp());
    }

    internal static void ApplyPhaseThresholdSettingsIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsWingedNoskPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsWingedNoskPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            SetPhase2ThresholdOnFsm(fsm, GetVanillaPhase2Hp(), useVanillaVariable: true);
        }
    }

    private static IntCompare? FindEnemyCountCompareInState(PlayMakerFSM fsm, string stateName)
    {
        FsmState? state = fsm.Fsm?.GetState(stateName);
        if (state?.Actions == null)
        {
            return null;
        }

        foreach (FsmStateAction action in state.Actions)
        {
            if (action is IntCompare compare && IsEnemyCountCompare(compare))
            {
                return compare;
            }
        }

        return null;
    }

    private static int ReadEnemyCountThreshold(IntCompare compare, int fallback)
    {
        if (compare.integer1 != null
            && string.Equals(compare.integer1.Name, WingedNoskEnemyCountVariableName, StringComparison.Ordinal)
            && compare.integer2 != null)
        {
            return compare.integer2.Value;
        }

        if (compare.integer2 != null
            && string.Equals(compare.integer2.Name, WingedNoskEnemyCountVariableName, StringComparison.Ordinal)
            && compare.integer1 != null)
        {
            return compare.integer1.Value;
        }

        if (compare.integer2 != null)
        {
            return compare.integer2.Value;
        }

        return fallback;
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsWingedNoskPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        bool useVanillaVariable = !ShouldUseCustomPhaseThreshold();
        int targetThreshold = useVanillaVariable
            ? GetVanillaPhase2Hp()
            : ClampWingedNoskPhase2Hp(wingedNoskPhase2Hp, ResolvePhase2MaxHp());
        SetPhase2ThresholdOnFsm(fsm, targetThreshold, useVanillaVariable);
    }

    private static void SetPhase2ThresholdOnFsm(PlayMakerFSM fsm, int value, bool useVanillaVariable)
    {
        int threshold = ClampWingedNoskPhase2Hp(value, ResolvePhase2MaxHp());
        FsmInt? halfHpVariable = fsm.FsmVariables.GetFsmInt(WingedNoskHalfHpVariableName);
        if (halfHpVariable != null)
        {
            halfHpVariable.Value = threshold;
        }

        IntCompare? compare = FindPhase2HpCompareAction(fsm);
        if (compare == null)
        {
            return;
        }

        FsmInt? thresholdOperand = null;
        if (compare.integer1 != null && string.Equals(compare.integer1.Name, WingedNoskHalfHpVariableName, StringComparison.Ordinal))
        {
            thresholdOperand = compare.integer1;
        }
        else if (compare.integer2 != null && string.Equals(compare.integer2.Name, WingedNoskHalfHpVariableName, StringComparison.Ordinal))
        {
            thresholdOperand = compare.integer2;
        }
        else if (compare.integer1 != null && string.Equals(compare.integer1.Name, WingedNoskHpVariableName, StringComparison.Ordinal))
        {
            thresholdOperand = compare.integer2;
        }
        else if (compare.integer2 != null && string.Equals(compare.integer2.Name, WingedNoskHpVariableName, StringComparison.Ordinal))
        {
            thresholdOperand = compare.integer1;
        }
        else
        {
            thresholdOperand = compare.integer2 ?? compare.integer1;
        }

        if (thresholdOperand == null)
        {
            return;
        }

        if (useVanillaVariable)
        {
            thresholdOperand.UseVariable = true;
            thresholdOperand.Name = WingedNoskHalfHpVariableName;
            thresholdOperand.Value = threshold;
        }
        else
        {
            thresholdOperand.UseVariable = false;
            thresholdOperand.Name = string.Empty;
            thresholdOperand.Value = threshold;
        }
    }

    private static IntCompare? FindPhase2HpCompareAction(PlayMakerFSM fsm)
    {
        FsmState? state = fsm.Fsm?.GetState(WingedNoskPhaseCheckStateName);
        if (state?.Actions == null)
        {
            return null;
        }

        foreach (FsmStateAction action in state.Actions)
        {
            if (action is IntCompare compare && IsHpCompare(compare))
            {
                return compare;
            }
        }

        return null;
    }

    private static int GetVanillaPhase2Hp()
    {
        int maxHp = ResolvePhase2MaxHp();
        int threshold = maxHp / 2;
        return ClampWingedNoskPhase2Hp(threshold, maxHp);
    }

    private static int ResolvePhase2MaxHp()
    {
        if (ShouldUseCustomHp())
        {
            return ClampWingedNoskHp(wingedNoskMaxHp);
        }

        if (TryFindWingedNoskHealthManager(out HealthManager? hm) && hm != null && TryGetVanillaHp(hm, out int vanillaHp))
        {
            return ClampWingedNoskHp(vanillaHp);
        }

        return DefaultWingedNoskVanillaHp;
    }

    private static int ClampWingedNoskPhase2Hp(int value, int maxHp)
    {
        int clampedMaxHp = ClampWingedNoskHp(maxHp);
        if (value < MinWingedNoskPhase2Hp)
        {
            return MinWingedNoskPhase2Hp;
        }

        return value > clampedMaxHp ? clampedMaxHp : value;
    }
}
