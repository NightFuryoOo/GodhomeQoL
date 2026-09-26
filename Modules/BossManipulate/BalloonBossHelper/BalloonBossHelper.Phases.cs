using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class BalloonBossHelper : Module
{
    private protected const string BossSpawnBalloonFsmName = "Spawn Balloon";
    private protected const string BossShakeTokenControlFsmName = "Shake Token Control";
    private protected const int MinBossPhase2Hp = 4;
    private protected const int MinBossPhase3Hp = 3;
    private protected const int MinBossPhase4Hp = 2;
    private protected const int MinBossPhase5Hp = 1;

    private void NormalizePhaseThresholdState()
    {
        Phase2Hp = ClampBossPhase2Hp(Phase2Hp, ResolvePhase2MaxHp());
        Phase3Hp = ClampBossPhase3Hp(Phase3Hp, Phase2Hp);
        Phase4Hp = ClampBossPhase4Hp(Phase4Hp, Phase3Hp);
        Phase5Hp = ClampBossPhase5Hp(Phase5Hp, Phase4Hp);
    }

    internal void ApplyPhaseThresholdSettingsIfPresentCore()
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

    internal void RestoreVanillaPhaseThresholdsIfPresentCore()
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

            if (IsBossShakeTokenFsm(fsm))
            {
                RememberVanillaShakeThresholds(fsm);
                (int phase3Hp, int phase4Hp, int phase5Hp) thresholds = GetVanillaShakeThresholds(fsm);
                SetShakeThresholdsOnFsm(fsm, thresholds.phase3Hp, thresholds.phase4Hp, thresholds.phase5Hp, useExactCustomThresholds: false);
                continue;
            }

            if (IsBossSpawnBalloonFsm(fsm))
            {
                RememberVanillaSpawnThreshold(fsm);
                int vanillaPhase2Hp = GetVanillaSpawnThreshold(fsm);
                SetSpawnThresholdOnFsm(fsm, vanillaPhase2Hp, useExactCustomThresholds: false);
            }
        }
    }

    private void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsBossPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        bool useExactCustomThresholds = ShouldUseCustomPhaseThresholds();
        int customPhase2Hp = ClampBossPhase2Hp(Phase2Hp, ResolvePhase2MaxHp());
        int customPhase3Hp = ClampBossPhase3Hp(Phase3Hp, customPhase2Hp);
        int customPhase4Hp = ClampBossPhase4Hp(Phase4Hp, customPhase3Hp);
        int customPhase5Hp = ClampBossPhase5Hp(Phase5Hp, customPhase4Hp);

        if (IsBossShakeTokenFsm(fsm))
        {
            RememberVanillaShakeThresholds(fsm);
            (int phase3Hp, int phase4Hp, int phase5Hp) thresholds = useExactCustomThresholds
                ? (customPhase3Hp, customPhase4Hp, customPhase5Hp)
                : GetVanillaShakeThresholds(fsm);

            SetShakeThresholdsOnFsm(fsm, thresholds.phase3Hp, thresholds.phase4Hp, thresholds.phase5Hp, useExactCustomThresholds);
            return;
        }

        if (IsBossSpawnBalloonFsm(fsm))
        {
            RememberVanillaSpawnThreshold(fsm);
            int phase2Hp = useExactCustomThresholds ? customPhase2Hp : GetVanillaSpawnThreshold(fsm);
            SetSpawnThresholdOnFsm(fsm, phase2Hp, useExactCustomThresholds);
        }
    }

    private void SetShakeThresholdsOnFsm(PlayMakerFSM fsm, int phase3Hp, int phase4Hp, int phase5Hp, bool useExactCustomThresholds)
    {
        int targetPhase3Hp = Math.Max(MinBossPhase3Hp, phase3Hp);
        int targetPhase4Hp = Math.Max(MinBossPhase4Hp, phase4Hp);
        int targetPhase5Hp = Math.Max(MinBossPhase5Hp, phase5Hp);

        FsmInt? token1 = fsm.FsmVariables.GetFsmInt(BossToken1VariableName);
        if (token1 != null)
        {
            token1.Value = targetPhase3Hp;
        }

        FsmInt? token2 = fsm.FsmVariables.GetFsmInt(BossToken2VariableName);
        if (token2 != null)
        {
            token2.Value = targetPhase4Hp;
        }

        FsmInt? token3 = fsm.FsmVariables.GetFsmInt(BossToken3VariableName);
        if (token3 != null)
        {
            token3.Value = targetPhase5Hp;
        }

        SetThresholdOnCheckState(
            fsm,
            BossCheckState1Name,
            BossToken1VariableName,
            targetPhase3Hp,
            useExactCustomThresholds);
        SetThresholdOnCheckState(
            fsm,
            BossCheckState2Name,
            BossToken2VariableName,
            targetPhase4Hp,
            useExactCustomThresholds);
        SetThresholdOnCheckState(
            fsm,
            BossCheckState3Name,
            BossToken3VariableName,
            targetPhase5Hp,
            useExactCustomThresholds);
    }

    private void SetThresholdOnCheckState(
        PlayMakerFSM fsm,
        string stateName,
        string variableName,
        int targetValue,
        bool useExactCustomThresholds)
    {
        FsmState? checkState = fsm.Fsm?.GetState(stateName);
        if (checkState?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in checkState.Actions)
        {
            if (action is not IntCompare compare || compare.integer2 == null || !IsHpCompare(compare))
            {
                continue;
            }

            if (useExactCustomThresholds)
            {
                compare.integer2.UseVariable = false;
                compare.integer2.Name = string.Empty;
                compare.integer2.Value = targetValue;
            }
            else
            {
                compare.integer2.UseVariable = true;
                compare.integer2.Name = variableName;
                compare.integer2.Value = targetValue;
            }

            break;
        }
    }

    private void SetSpawnThresholdOnFsm(PlayMakerFSM fsm, int phase2Hp, bool useExactCustomThresholds)
    {
        IntCompare? hpCompare = FindSpawnHpCompareAction(fsm);
        if (hpCompare?.integer2 == null)
        {
            return;
        }

        int targetPhase2Hp = Math.Max(MinBossPhase2Hp, phase2Hp);
        hpCompare.integer2.UseVariable = false;
        hpCompare.integer2.Name = string.Empty;
        hpCompare.integer2.Value = useExactCustomThresholds
            ? targetPhase2Hp
            : targetPhase2Hp;
    }

    private IntCompare? FindSpawnHpCompareAction(PlayMakerFSM fsm)
    {
        FsmState? spawnState = fsm.Fsm?.GetState(BossSpawnStateName);
        if (spawnState?.Actions == null)
        {
            return null;
        }

        foreach (FsmStateAction action in spawnState.Actions)
        {
            if (action is IntCompare compare && IsHpCompare(compare))
            {
                return compare;
            }
        }

        return null;
    }

    private IntCompare? FindSpawnEnemyCountCompareAction(PlayMakerFSM fsm)
    {
        FsmState? spawnState = fsm.Fsm?.GetState(BossSpawnStateName);
        if (spawnState?.Actions == null)
        {
            return null;
        }

        foreach (FsmStateAction action in spawnState.Actions)
        {
            if (action is IntCompare compare && IsEnemyCountCompare(compare))
            {
                return compare;
            }
        }

        return null;
    }

    private void RememberVanillaShakeThresholds(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaShakeThresholdsByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int phase3Hp = DefaultPhase3Hp;
        int phase4Hp = DefaultPhase4Hp;
        int phase5Hp = DefaultPhase5Hp;

        FsmInt? token1 = fsm.FsmVariables.GetFsmInt(BossToken1VariableName);
        if (token1 != null && token1.Value > 0)
        {
            phase3Hp = token1.Value;
        }

        FsmInt? token2 = fsm.FsmVariables.GetFsmInt(BossToken2VariableName);
        if (token2 != null && token2.Value > 0)
        {
            phase4Hp = token2.Value;
        }

        FsmInt? token3 = fsm.FsmVariables.GetFsmInt(BossToken3VariableName);
        if (token3 != null && token3.Value > 0)
        {
            phase5Hp = token3.Value;
        }

        vanillaShakeThresholdsByFsm[fsmId] = (phase3Hp, phase4Hp, phase5Hp);
    }

    private void RememberVanillaSpawnThreshold(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSpawnThresholdByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int phase2Hp = DefaultPhase2Hp;
        IntCompare? hpCompare = FindSpawnHpCompareAction(fsm);
        if (hpCompare?.integer2 != null && hpCompare.integer2.Value > 0)
        {
            phase2Hp = hpCompare.integer2.Value;
        }

        vanillaSpawnThresholdByFsm[fsmId] = phase2Hp;
    }

    private int GetVanillaSpawnThreshold(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSpawnThresholdByFsm.TryGetValue(fsmId, out int threshold) && threshold > 0)
        {
            return threshold;
        }

        RememberVanillaSpawnThreshold(fsm);
        if (vanillaSpawnThresholdByFsm.TryGetValue(fsmId, out threshold) && threshold > 0)
        {
            return threshold;
        }

        return DefaultPhase2Hp;
    }

    private int ResolvePhase2MaxHp()
    {
        int configuredMaxHp = ClampBossHp(MaxHp);
        return Math.Max(MinBossPhase2Hp, configuredMaxHp);
    }

    private int ClampBossPhase2Hp(int value, int phase2MaxHp)
    {
        int maxPhase2Hp = Math.Max(MinBossPhase2Hp, phase2MaxHp);
        if (value < MinBossPhase2Hp)
        {
            return MinBossPhase2Hp;
        }

        return value > maxPhase2Hp ? maxPhase2Hp : value;
    }

    private int ClampBossPhase3Hp(int value, int phase2Hp)
    {
        int clampedPhase2Hp = phase2Hp < MinBossPhase2Hp
            ? MinBossPhase2Hp
            : phase2Hp;
        int maxPhase3Hp = Math.Max(MinBossPhase3Hp, clampedPhase2Hp - 1);
        if (value < MinBossPhase3Hp)
        {
            return MinBossPhase3Hp;
        }

        return value > maxPhase3Hp ? maxPhase3Hp : value;
    }

    private int ClampBossPhase4Hp(int value, int phase3Hp)
    {
        int clampedPhase3Hp = phase3Hp < MinBossPhase3Hp
            ? MinBossPhase3Hp
            : phase3Hp;
        int maxPhase4Hp = Math.Max(MinBossPhase4Hp, clampedPhase3Hp - 1);
        if (value < MinBossPhase4Hp)
        {
            return MinBossPhase4Hp;
        }

        return value > maxPhase4Hp ? maxPhase4Hp : value;
    }

    private int ClampBossPhase5Hp(int value, int phase4Hp)
    {
        int clampedPhase4Hp = phase4Hp < MinBossPhase4Hp
            ? MinBossPhase4Hp
            : phase4Hp;
        int maxPhase5Hp = Math.Max(MinBossPhase5Hp, clampedPhase4Hp - 1);
        if (value < MinBossPhase5Hp)
        {
            return MinBossPhase5Hp;
        }

        return value > maxPhase5Hp ? maxPhase5Hp : value;
    }
}
