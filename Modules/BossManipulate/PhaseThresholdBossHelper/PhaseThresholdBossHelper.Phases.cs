using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class PhaseThresholdBossHelper : Module
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

    private protected void RestoreVanillaPhaseThresholdsIfPresentCore()
    {
        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsBossPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            RememberVanillaPhaseThresholds(fsm);
            (int phase2Hp, int phase3Hp) = GetVanillaPhaseThresholds(fsm);
            SetPhaseThresholdsOnFsm(fsm, phase2Hp, phase3Hp, useExactCustomThresholds: false);
        }
    }

    private protected void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsBossPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        RememberVanillaPhaseThresholds(fsm);
        int phase2MaxHp = ResolvePhase2MaxHp();
        (int phase2Hp, int phase3Hp) thresholds = ShouldUseCustomPhaseThresholds()
            ? (
                ClampBossPhase2Hp(Phase2Hp, phase2MaxHp),
                ClampBossPhase3Hp(Phase3Hp, Phase2Hp)
            )
            : GetVanillaPhaseThresholds(fsm);

        SetPhaseThresholdsOnFsm(fsm, thresholds.phase2Hp, thresholds.phase3Hp, ShouldUseCustomPhaseThresholds());
    }

    private protected virtual void SetPhaseThresholdsOnFsm(PlayMakerFSM fsm, int phase2Hp, int phase3Hp, bool useExactCustomThresholds)
    {
        int targetPhase2Hp = ClampBossPhase2Hp(phase2Hp, ResolvePhase2MaxHp());
        int targetPhase3Hp = ClampBossPhase3Hp(phase3Hp, targetPhase2Hp);

        FsmInt? phase2Variable = fsm.FsmVariables.GetFsmInt(Phase2VariableName);
        if (phase2Variable != null)
        {
            phase2Variable.Value = targetPhase2Hp;
        }

        FsmInt? phase3Variable = fsm.FsmVariables.GetFsmInt(Phase3VariableName);
        if (phase3Variable != null)
        {
            phase3Variable.Value = targetPhase3Hp;
        }

        SetThresholdOnCheckState(fsm, PhaseCheckState1Name, Phase2VariableName, targetPhase2Hp);
        SetThresholdOnCheckState(fsm, PhaseCheckState2Name, Phase3VariableName, targetPhase3Hp);
    }

    private protected virtual void SetThresholdOnCheckState(PlayMakerFSM fsm, string stateName, string variableName, int targetValue, bool useExactCustomThresholds)
    {
        FsmState? checkState = fsm.Fsm?.GetState(stateName);
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

            if (useExactCustomThresholds)
            {
                compare.integer2.UseVariable = false;
                compare.integer2.Name = string.Empty;
                compare.integer2.Value = SafeIncrementThreshold(targetValue);
            }
            else
            {
                compare.integer2.UseVariable = true;
                compare.integer2.Name = variableName;
                compare.integer2.Value = targetValue;
            }
        }
    }

    private protected void RememberVanillaPhaseThresholds(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaPhaseThresholdsByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int phase2Hp = DefaultPhase2Hp;
        int phase3Hp = DefaultPhase3Hp;

        bool phase2Captured = TryReadThresholdFromCheckState(fsm, PhaseCheckState1Name, out int capturedPhase2Hp);
        if (phase2Captured)
        {
            phase2Hp = capturedPhase2Hp;
        }

        bool phase3Captured = TryReadThresholdFromCheckState(fsm, PhaseCheckState2Name, out int capturedPhase3Hp);
        if (phase3Captured)
        {
            phase3Hp = capturedPhase3Hp;
        }

        if (!phase2Captured)
        {
            FsmInt? phase2Variable = fsm.FsmVariables.GetFsmInt(Phase2VariableName);
            if (phase2Variable != null && phase2Variable.Value > 0)
            {
                phase2Hp = phase2Variable.Value;
            }
        }

        if (!phase3Captured)
        {
            FsmInt? phase3Variable = fsm.FsmVariables.GetFsmInt(Phase3VariableName);
            if (phase3Variable != null && phase3Variable.Value > 0)
            {
                phase3Hp = phase3Variable.Value;
            }
        }

        phase2Hp = ClampBossPhase2Hp(phase2Hp, ResolvePhase2MaxHp());
        phase3Hp = ClampBossPhase3Hp(phase3Hp, phase2Hp);
        vanillaPhaseThresholdsByFsm[fsmId] = (phase2Hp, phase3Hp);
    }

    private protected bool TryReadThresholdFromCheckState(PlayMakerFSM fsm, string stateName, out int threshold)
    {
        threshold = 0;
        FsmState? state = fsm.Fsm?.GetState(stateName);
        if (state?.Actions == null)
        {
            return false;
        }

        foreach (FsmStateAction action in state.Actions)
        {
            if (action is not IntCompare compare || compare.integer2 == null)
            {
                continue;
            }

            if (compare.integer2.UseVariable && !string.IsNullOrEmpty(compare.integer2.Name))
            {
                FsmInt? variable = fsm.FsmVariables.GetFsmInt(compare.integer2.Name);
                if (variable != null)
                {
                    threshold = variable.Value;
                    return true;
                }
            }

            threshold = compare.integer2.Value;
            return true;
        }

        return false;
    }

    private protected virtual int GetPhase2MaxHpForUiCore() => ResolvePhase2MaxHp();

    private protected virtual int ClampBossPhase2Hp(int value, int maxHp)
    {
        int clampedMaxHp = ClampBossHp(maxHp);
        if (value < MinPhase2Hp)
        {
            return MinPhase2Hp;
        }

        return value > clampedMaxHp ? clampedMaxHp : value;
    }

    private protected virtual int ClampBossPhase3Hp(int value, int phase2Hp)
    {
        int clampedPhase2Hp = ClampBossPhase2Hp(phase2Hp, ResolvePhase2MaxHp());
        int maxPhase3Hp = Math.Max(MinPhase3Hp, clampedPhase2Hp - 1);

        if (value < MinPhase3Hp)
        {
            return MinPhase3Hp;
        }

        return value > maxPhase3Hp ? maxPhase3Hp : value;
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

    private protected virtual int SafeIncrementThreshold(int value)
    {
        return value >= int.MaxValue ? int.MaxValue : value + 1;
    }

    private protected virtual void SetThresholdOnCheckState(PlayMakerFSM fsm, string stateName, string variableName, int targetValue)
    {
        FsmState? checkState = fsm.Fsm?.GetState(stateName);
        if (checkState?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in checkState.Actions)
        {
            if (action is IntCompare compare && compare.integer2 != null)
            {
                bool assigned = false;

                if (compare.integer1 != null && string.Equals(compare.integer1.Name, variableName, StringComparison.Ordinal))
                {
                    compare.integer1.UseVariable = true;
                    compare.integer1.Name = variableName;
                    compare.integer1.Value = targetValue;
                    assigned = true;
                }

                if (compare.integer2 != null && string.Equals(compare.integer2.Name, variableName, StringComparison.Ordinal))
                {
                    compare.integer2.UseVariable = true;
                    compare.integer2.Name = variableName;
                    compare.integer2.Value = targetValue;
                    assigned = true;
                }

                if (!assigned && compare.integer2 != null)
                {
                    compare.integer2.UseVariable = true;
                    compare.integer2.Name = variableName;
                    compare.integer2.Value = targetValue;
                }
            }
        }
    }
}
