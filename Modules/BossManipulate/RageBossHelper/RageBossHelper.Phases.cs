using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class RageBossHelper : Module
{
    private protected void ApplyPhaseThresholdSettingsIfPresentCore()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsBossPhaseControlFsm(fsm))
            {
                continue;
            }

            ApplyPhaseThresholdSettings(fsm);
        }
    }

    private protected virtual void RestoreVanillaPhaseThresholdsIfPresentCore()
    {
        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsBossPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplyPhaseSettings(fsm.gameObject))
            {
                continue;
            }

            SetPhase2ThresholdOnFsm(fsm, GetVanillaPhase2Hp(), useVanillaVariable: true);
        }
    }

    private protected virtual void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsBossPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplyPhaseSettings(fsm.gameObject))
        {
            return;
        }

        bool useVanillaVariable = !ShouldUseCustomPhaseThreshold();
        int targetThreshold = useVanillaVariable
            ? GetVanillaPhase2Hp()
            : ClampBossPhase2Hp(Phase2Hp, ResolvePhase2MaxHp());

        SetPhase2ThresholdOnFsm(fsm, targetThreshold, useVanillaVariable);
    }

    private protected virtual void SetPhase2ThresholdOnFsm(PlayMakerFSM fsm, int value, bool useVanillaVariable)
    {
        if (fsm == null)
        {
            return;
        }

        int threshold = ClampBossPhase2Hp(value, ResolvePhase2MaxHp());
        FsmInt? rageHpVariable = fsm.FsmVariables.GetFsmInt(Phase2VariableName);
        if (rageHpVariable != null)
        {
            rageHpVariable.Value = threshold;
        }

        FsmState? checkState = fsm.Fsm?.GetState(PhaseCheckStateName);
        if (checkState?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in checkState.Actions)
        {
            if (action is not IntCompare compare || compare.integer2 == null)
            {
                continue;
            }

            if (useVanillaVariable)
            {
                compare.integer2.UseVariable = true;
                compare.integer2.Name = Phase2VariableName;
            }
            else
            {
                compare.integer2.UseVariable = false;
                compare.integer2.Name = string.Empty;
                compare.integer2.Value = threshold;
            }
        }
    }

    private protected virtual int GetVanillaPhase2Hp()
    {
        int maxHp = ResolvePhase2MaxHp();
        int threshold = maxHp / 2;
        return ClampBossPhase2Hp(threshold, maxHp);
    }

    private protected int ClampBossPhase2Hp(int value, int maxHp)
    {
        int clampedMaxHp = ClampBossHp(maxHp);
        if (value < MinPhase2Hp)
        {
            return MinPhase2Hp;
        }

        return value > clampedMaxHp ? clampedMaxHp : value;
    }

    private protected int ResolvePhase2MaxHp()
    {
        if (ShouldUseCustomHp())
        {
            return ClampBossHp(MaxHp);
        }

        if (TryFindBossHealthManager(out HealthManager? hm) && hm != null && TryGetVanillaHp(hm, out int vanillaHp))
        {
            return ClampBossHp(vanillaHp);
        }

        return DefaultVanillaHp;
    }

    private protected virtual int GetPhase2MaxHpForUiCore()
    {
        return ResolvePhase2MaxHp();
    }

    private protected virtual FsmInt? ResolveThresholdOperand(IntCompare compare, string thresholdVariableName)
    {
        if (compare == null)
        {
            return null;
        }

        FsmInt? integer1 = compare.integer1;
        FsmInt? integer2 = compare.integer2;
        if (integer1 == null && integer2 == null)
        {
            return null;
        }

        if (integer1 != null && string.Equals(integer1.Name, thresholdVariableName, StringComparison.Ordinal))
        {
            return integer1;
        }

        if (integer2 != null && string.Equals(integer2.Name, thresholdVariableName, StringComparison.Ordinal))
        {
            return integer2;
        }

        if (integer1 != null && string.Equals(integer1.Name, "HP", StringComparison.Ordinal))
        {
            return integer2 ?? integer1;
        }

        if (integer2 != null && string.Equals(integer2.Name, "HP", StringComparison.Ordinal))
        {
            return integer1 ?? integer2;
        }

        return integer2 ?? integer1;
    }
}
