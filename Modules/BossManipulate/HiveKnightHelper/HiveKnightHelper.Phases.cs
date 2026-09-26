using Modding;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class HiveKnightHelper : Module
{
    private const int DefaultHiveKnightPhase2Hp = 580;
    private const int DefaultHiveKnightPhase3Hp = 350;
    private const int PhaseCompareCount = 2;
    private const int MinHiveKnightPhase2Hp = 2;
    private const int MinHiveKnightPhase3Hp = 1;

    internal static void ApplyPhaseThresholdSettingsIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsHiveKnightPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsHiveKnightPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            RememberVanillaPhaseThresholds(fsm);
            (int phase2Hp, int phase3Hp) = GetVanillaPhaseThresholds(fsm);
            SetPhaseThresholdsOnFsm(fsm, phase2Hp, phase3Hp, triggerOnEqual: false);
        }
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsHiveKnightPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        RememberVanillaPhaseThresholds(fsm);
        (int phase2Hp, int phase3Hp) thresholds = ShouldUseCustomPhaseThresholds()
            ? (
                ClampHiveKnightPhase2Hp(hiveKnightPhase2Hp),
                ClampHiveKnightPhase3Hp(hiveKnightPhase3Hp, hiveKnightPhase2Hp)
            )
            : GetVanillaPhaseThresholds(fsm);

        SetPhaseThresholdsOnFsm(fsm, thresholds.phase2Hp, thresholds.phase3Hp, ShouldUseCustomPhaseThresholds());
    }

    private static void SetPhaseThresholdsOnFsm(PlayMakerFSM fsm, int phase2Hp, int phase3Hp, bool triggerOnEqual)
    {
        int targetPhase2Hp = ClampHiveKnightPhase2Hp(phase2Hp);
        int targetPhase3Hp = ClampHiveKnightPhase3Hp(phase3Hp, targetPhase2Hp);

        FsmInt? phase2Variable = fsm.FsmVariables.GetFsmInt("P2 HP");
        if (phase2Variable != null)
        {
            phase2Variable.Value = targetPhase2Hp;
        }

        FsmInt? phase3Variable = fsm.FsmVariables.GetFsmInt("P3 HP");
        if (phase3Variable != null)
        {
            phase3Variable.Value = targetPhase3Hp;
        }

        FsmState? phaseCheck = fsm.Fsm?.GetState("Phase Check");
        if (phaseCheck?.Actions != null)
        {
            int compareIndex = 0;
            foreach (FsmStateAction action in phaseCheck.Actions)
            {
                if (action is not IntCompare compare || compare.integer2 == null)
                {
                    continue;
                }

                compare.integer2.Value = compareIndex == 0 ? targetPhase2Hp : targetPhase3Hp;
                SetPhaseCompareEqualEvent(compare, triggerOnEqual);
                compareIndex++;
                if (compareIndex >= PhaseCompareCount)
                {
                    break;
                }
            }
        }

        FsmState? variant = fsm.Fsm?.GetState("Variant");
        if (variant?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in variant.Actions)
        {
            if (action is not SetIntValue setIntValue || setIntValue.intVariable == null || setIntValue.intValue == null)
            {
                continue;
            }

            if (string.Equals(setIntValue.intVariable.Name, "P2 HP", StringComparison.Ordinal))
            {
                setIntValue.intValue.Value = targetPhase2Hp;
                setIntValue.intVariable.Value = targetPhase2Hp;
            }
            else if (string.Equals(setIntValue.intVariable.Name, "P3 HP", StringComparison.Ordinal))
            {
                setIntValue.intValue.Value = targetPhase3Hp;
                setIntValue.intVariable.Value = targetPhase3Hp;
            }
        }
    }

    private static void SetPhaseCompareEqualEvent(IntCompare compare, bool triggerOnEqual)
    {
        compare.equal = triggerOnEqual && compare.lessThan != null
            ? compare.lessThan
            : null;
    }

    private static void RememberVanillaPhaseThresholds(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaPhaseThresholdsByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int phase2Hp = DefaultHiveKnightPhase2Hp;
        int phase3Hp = DefaultHiveKnightPhase3Hp;
        bool capturedFromPhaseCheck = false;

        FsmState? phaseCheck = fsm.Fsm?.GetState("Phase Check");
        if (phaseCheck?.Actions != null)
        {
            int compareIndex = 0;
            foreach (FsmStateAction action in phaseCheck.Actions)
            {
                if (action is not IntCompare compare || compare.integer2 == null)
                {
                    continue;
                }

                if (compareIndex == 0)
                {
                    phase2Hp = compare.integer2.Value;
                    capturedFromPhaseCheck = true;
                }
                else if (compareIndex == 1)
                {
                    phase3Hp = compare.integer2.Value;
                    capturedFromPhaseCheck = true;
                    break;
                }

                compareIndex++;
            }
        }

        if (!capturedFromPhaseCheck)
        {
            FsmInt? phase2Variable = fsm.FsmVariables.GetFsmInt("P2 HP");
            if (phase2Variable != null && phase2Variable.Value > 0)
            {
                phase2Hp = phase2Variable.Value;
            }

            FsmInt? phase3Variable = fsm.FsmVariables.GetFsmInt("P3 HP");
            if (phase3Variable != null && phase3Variable.Value > 0)
            {
                phase3Hp = phase3Variable.Value;
            }
        }

        phase2Hp = ClampHiveKnightPhase2Hp(phase2Hp);
        phase3Hp = ClampHiveKnightPhase3Hp(phase3Hp, phase2Hp);
        vanillaPhaseThresholdsByFsm[fsmId] = (phase2Hp, phase3Hp);
    }

    private static (int phase2Hp, int phase3Hp) GetVanillaPhaseThresholds(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaPhaseThresholdsByFsm.TryGetValue(fsmId, out (int phase2Hp, int phase3Hp) thresholds))
        {
            return (
                ClampHiveKnightPhase2Hp(thresholds.phase2Hp),
                ClampHiveKnightPhase3Hp(thresholds.phase3Hp, thresholds.phase2Hp)
            );
        }

        RememberVanillaPhaseThresholds(fsm);
        if (vanillaPhaseThresholdsByFsm.TryGetValue(fsmId, out thresholds))
        {
            return (
                ClampHiveKnightPhase2Hp(thresholds.phase2Hp),
                ClampHiveKnightPhase3Hp(thresholds.phase3Hp, thresholds.phase2Hp)
            );
        }

        return (DefaultHiveKnightPhase2Hp, DefaultHiveKnightPhase3Hp);
    }

    private static int ClampHiveKnightPhase2Hp(int value)
    {
        int maxPhase2Hp = ResolvePhase2MaxHp();

        if (value < MinHiveKnightPhase2Hp)
        {
            return MinHiveKnightPhase2Hp;
        }

        return value > maxPhase2Hp ? maxPhase2Hp : value;
    }

    private static int ResolvePhase2MaxHp()
    {
        return ShouldUseCustomHp()
            ? ClampHiveKnightHp(hiveKnightMaxHp)
            : DefaultHiveKnightVanillaHp;
    }

    internal static int GetPhase2MaxHpForUi()
    {
        return ResolvePhase2MaxHp();
    }

    internal static int GetPhase3MaxHpForUi()
    {
        int phase2Hp = ClampHiveKnightPhase2Hp(hiveKnightPhase2Hp);
        return Math.Max(MinHiveKnightPhase3Hp, phase2Hp - 1);
    }

    private static int ClampHiveKnightPhase3Hp(int value, int phase2Hp)
    {
        int clampedPhase2Hp = ClampHiveKnightPhase2Hp(phase2Hp);
        int maxPhase3Hp = Math.Max(MinHiveKnightPhase3Hp, clampedPhase2Hp - 1);

        if (value < MinHiveKnightPhase3Hp)
        {
            return MinHiveKnightPhase3Hp;
        }

        return value > maxPhase3Hp ? maxPhase3Hp : value;
    }
}
