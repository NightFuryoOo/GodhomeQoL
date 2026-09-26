using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class TroupeMasterGrimmHelper : Module
{
    private const string RagePhase1VariableName = "Rage HP 1";
    private const string RagePhase2VariableName = "Rage HP 2";
    private const string RagePhase3VariableName = "Rage HP 3";
    private const int DefaultTroupeMasterGrimmPhase2Hp = 750;
    private const int DefaultTroupeMasterGrimmPhase3Hp = 500;
    private const int DefaultTroupeMasterGrimmPhase4Hp = 250;
    private const int MinTroupeMasterGrimmPhase2Hp = 1;
    private const int MinTroupeMasterGrimmPhase3Hp = 1;
    private const int MinTroupeMasterGrimmPhase4Hp = 1;

    internal static void ApplyPhaseThresholdSettingsIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsTroupeMasterGrimmPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsTroupeMasterGrimmPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            (int phase2Hp, int phase3Hp, int phase4Hp) = GetVanillaPhaseThresholds(fsm.gameObject);
            SetPhaseThresholdsOnFsm(fsm, phase2Hp, phase3Hp, phase4Hp, useExactCustomThresholds: false);
        }
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsTroupeMasterGrimmPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        int referenceHp = GetPhaseReferenceHp(fsm.gameObject);
        (int phase2Hp, int phase3Hp, int phase4Hp) thresholds = ShouldUseCustomPhaseThresholds()
            ? (
                ClampTroupeMasterGrimmPhase2Hp(troupeMasterGrimmPhase2Hp, referenceHp),
                ClampTroupeMasterGrimmPhase3Hp(troupeMasterGrimmPhase3Hp, troupeMasterGrimmPhase2Hp),
                ClampTroupeMasterGrimmPhase4Hp(troupeMasterGrimmPhase4Hp, troupeMasterGrimmPhase3Hp)
            )
            : GetVanillaPhaseThresholds(fsm.gameObject);

        SetPhaseThresholdsOnFsm(fsm, thresholds.phase2Hp, thresholds.phase3Hp, thresholds.phase4Hp, ShouldUseCustomPhaseThresholds());
    }

    private static void SetPhaseThresholdsOnFsm(PlayMakerFSM fsm, int phase2Hp, int phase3Hp, int phase4Hp, bool useExactCustomThresholds)
    {
        if (fsm == null)
        {
            return;
        }

        var fsmVariables = fsm.FsmVariables;
        if (fsmVariables == null)
        {
            return;
        }

        int referenceHp = GetPhaseReferenceHp(fsm?.gameObject);
        int targetPhase2Hp = ClampTroupeMasterGrimmPhase2Hp(phase2Hp, referenceHp);
        int targetPhase3Hp = ClampTroupeMasterGrimmPhase3Hp(phase3Hp, targetPhase2Hp);
        int targetPhase4Hp = ClampTroupeMasterGrimmPhase4Hp(phase4Hp, targetPhase3Hp);

        FsmInt? phase2Variable = fsmVariables.GetFsmInt(RagePhase1VariableName);
        if (phase2Variable != null)
        {
            phase2Variable.Value = targetPhase2Hp;
        }

        FsmInt? phase3Variable = fsmVariables.GetFsmInt(RagePhase2VariableName);
        if (phase3Variable != null)
        {
            phase3Variable.Value = targetPhase3Hp;
        }

        FsmInt? phase4Variable = fsmVariables.GetFsmInt(RagePhase3VariableName);
        if (phase4Variable != null)
        {
            phase4Variable.Value = targetPhase4Hp;
        }

        SetThresholdOnBalloonCheckState(fsm!, targetPhase2Hp, targetPhase3Hp, targetPhase4Hp, useExactCustomThresholds);
    }

    private static void SetThresholdOnBalloonCheckState(PlayMakerFSM fsm, int phase2Hp, int phase3Hp, int phase4Hp, bool useExactCustomThresholds)
    {
        FsmState? balloonCheckState = fsm.Fsm?.GetState(BalloonCheckStateName);
        if (balloonCheckState?.Actions == null)
        {
            return;
        }

        int thresholdActionIndex = 0;
        foreach (FsmStateAction action in balloonCheckState.Actions)
        {
            if (action is not IntTestToBool compare || compare.int2 == null)
            {
                continue;
            }

            string variableName = thresholdActionIndex switch
            {
                0 => RagePhase1VariableName,
                1 => RagePhase2VariableName,
                2 => RagePhase3VariableName,
                _ => string.Empty
            };
            int targetThreshold = thresholdActionIndex switch
            {
                0 => phase2Hp,
                1 => phase3Hp,
                2 => phase4Hp,
                _ => 0
            };
            thresholdActionIndex++;

            if (string.IsNullOrEmpty(variableName))
            {
                continue;
            }

            if (useExactCustomThresholds)
            {
                compare.int2.UseVariable = false;
                compare.int2.Name = string.Empty;
                compare.int2.Value = SafeIncrementThreshold(targetThreshold);
            }
            else
            {
                compare.int2.UseVariable = true;
                compare.int2.Name = variableName;
                compare.int2.Value = targetThreshold;
            }
        }
    }

    private static (int phase2Hp, int phase3Hp, int phase4Hp) GetVanillaPhaseThresholds(GameObject bossObject)
    {
        int referenceHp = GetPhaseReferenceHp(bossObject);
        int quarterHp = referenceHp / 4;
        int phase2Hp = referenceHp - quarterHp;
        int phase3Hp = phase2Hp - quarterHp;
        int phase4Hp = phase3Hp - quarterHp;

        int clampedPhase2Hp = ClampTroupeMasterGrimmPhase2Hp(phase2Hp, referenceHp);
        int clampedPhase3Hp = ClampTroupeMasterGrimmPhase3Hp(phase3Hp, clampedPhase2Hp);
        int clampedPhase4Hp = ClampTroupeMasterGrimmPhase4Hp(phase4Hp, clampedPhase3Hp);
        return (clampedPhase2Hp, clampedPhase3Hp, clampedPhase4Hp);
    }

    private static int GetPhaseReferenceHp(GameObject? bossObject)
    {
        if (bossObject != null)
        {
            HealthManager? hm = bossObject.GetComponent<HealthManager>();
            if (hm != null)
            {
                int maxHp = hm.hp;
                if (maxHp > 0)
                {
                    return ClampTroupeMasterGrimmHp(maxHp);
                }

                if (hm.hp > 0)
                {
                    return ClampTroupeMasterGrimmHp(hm.hp);
                }
            }
        }

        int configuredHp = ClampTroupeMasterGrimmHp(troupeMasterGrimmMaxHp);
        return configuredHp > 0 ? configuredHp : DefaultTroupeMasterGrimmVanillaHp;
    }

    private static int ClampTroupeMasterGrimmPhase2Hp(int value, int referenceHp)
    {
        int maxPhase2Hp = Math.Max(MinTroupeMasterGrimmPhase2Hp, ClampTroupeMasterGrimmHp(referenceHp));
        if (value < MinTroupeMasterGrimmPhase2Hp)
        {
            return MinTroupeMasterGrimmPhase2Hp;
        }

        return value > maxPhase2Hp ? maxPhase2Hp : value;
    }

    private static int ClampTroupeMasterGrimmPhase3Hp(int value, int phase2Hp)
    {
        int clampedPhase2Hp = phase2Hp < MinTroupeMasterGrimmPhase2Hp
            ? MinTroupeMasterGrimmPhase2Hp
            : phase2Hp;
        int maxPhase3Hp = Math.Max(MinTroupeMasterGrimmPhase3Hp, clampedPhase2Hp - 1);
        if (value < MinTroupeMasterGrimmPhase3Hp)
        {
            return MinTroupeMasterGrimmPhase3Hp;
        }

        return value > maxPhase3Hp ? maxPhase3Hp : value;
    }

    private static int ClampTroupeMasterGrimmPhase4Hp(int value, int phase3Hp)
    {
        int clampedPhase3Hp = phase3Hp < MinTroupeMasterGrimmPhase3Hp
            ? MinTroupeMasterGrimmPhase3Hp
            : phase3Hp;
        int maxPhase4Hp = Math.Max(MinTroupeMasterGrimmPhase4Hp, clampedPhase3Hp - 1);
        if (value < MinTroupeMasterGrimmPhase4Hp)
        {
            return MinTroupeMasterGrimmPhase4Hp;
        }

        return value > maxPhase4Hp ? maxPhase4Hp : value;
    }

    private static int SafeIncrementThreshold(int value)
    {
        return value >= int.MaxValue ? int.MaxValue : value + 1;
    }
}
