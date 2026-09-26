using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class SoulWarriorHelper : Module
{
    private const int DefaultSoulWarriorSummonVanillaHp = 13;
    private const int DefaultSoulWarriorSummonLimit = 36;
    private const int DefaultSoulWarriorVanillaSummonLimit = 36;
    private const int MinSoulWarriorSummonLimit = DefaultSoulWarriorSummonLimit;
    private const int MaxSoulWarriorSummonLimit = 999;

    internal static void ApplySummonLimitSettingsIfPresent()
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

            ApplySummonLimitSettings(fsm);
        }
    }

    internal static void RestoreVanillaSummonLimitsIfPresent()
    {
        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsSoulWarriorSummonControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySummonLimitSettings(fsm))
            {
                continue;
            }

            RememberVanillaSummonLimit(fsm);
            RememberVanillaSummonCount(fsm);
            SetSummonLimitOnFsm(fsm, GetVanillaSummonLimit(fsm));
            SetSummonCountOnFsm(fsm, GetVanillaSummonCount(fsm), custom: false);
            RestoreVanillaSummonPool(fsm);
        }
    }

    private static void ApplySummonLimitSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsSoulWarriorSummonControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySummonLimitSettings(fsm))
        {
            return;
        }

        RememberVanillaSummonLimit(fsm);
        RememberVanillaSummonCount(fsm);
        RememberVanillaSummonPools(fsm);
        int targetLimit = ShouldUseCustomSummonLimit()
            ? ClampSoulWarriorSummonLimit(soulWarriorSummonLimit)
            : GetVanillaSummonLimit(fsm);
        SetSummonLimitOnFsm(fsm, targetLimit);
        SetSummonCountOnFsm(fsm, targetLimit, ShouldUseCustomSummonLimit());
        ApplySummonCountSetterOverrides(fsm, targetLimit);
        SetSummonPoolLimitOnFsm(fsm, targetLimit);
    }

    private static void SetSummonLimitOnFsm(PlayMakerFSM fsm, int value)
    {
        List<IntCompare> compares = FindSummonEnemyCountCompareActions(fsm);
        if (compares.Count == 0)
        {
            return;
        }

        int clampedLimit = ClampSoulWarriorSummonLimit(value);
        foreach (IntCompare compare in compares)
        {
            if (compare == null)
            {
                continue;
            }

            if (IsLikelySummonCountVariable(compare.integer1) && compare.integer2 != null)
            {
                compare.integer2.UseVariable = false;
                compare.integer2.Name = string.Empty;
                compare.integer2.Value = clampedLimit;
                continue;
            }

            if (IsLikelySummonCountVariable(compare.integer2) && compare.integer1 != null)
            {
                compare.integer1.UseVariable = false;
                compare.integer1.Name = string.Empty;
                compare.integer1.Value = clampedLimit;
                continue;
            }

            if (compare.integer2 != null)
            {
                compare.integer2.UseVariable = false;
                compare.integer2.Name = string.Empty;
                compare.integer2.Value = clampedLimit;
                continue;
            }

            if (compare.integer1 != null)
            {
                compare.integer1.UseVariable = false;
                compare.integer1.Name = string.Empty;
                compare.integer1.Value = clampedLimit;
            }
        }
    }

    private static void RememberVanillaSummonLimit(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSummonLimitByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int limit = DefaultSoulWarriorVanillaSummonLimit;
        IntCompare? compare = FindSummonEnemyCountCompareAction(fsm);
        if (compare != null)
        {
            int readLimit = ReadSummonLimitThreshold(compare, DefaultSoulWarriorVanillaSummonLimit);
            if (readLimit > 0)
            {
                limit = readLimit;
            }
        }
        else
        {
            int poolCount = ResolveVanillaSummonPoolSize(fsm);
            if (poolCount > 0)
            {
                limit = poolCount;
            }
        }

        vanillaSummonLimitByFsm[fsmId] = ClampSoulWarriorSummonLimit(limit);
    }

    private static int GetVanillaSummonLimit(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSummonLimitByFsm.TryGetValue(fsmId, out int limit))
        {
            return ClampSoulWarriorSummonLimit(limit);
        }

        RememberVanillaSummonLimit(fsm);
        if (vanillaSummonLimitByFsm.TryGetValue(fsmId, out limit))
        {
            return ClampSoulWarriorSummonLimit(limit);
        }

        return DefaultSoulWarriorVanillaSummonLimit;
    }

    private static int ClampSoulWarriorSummonLimit(int value)
    {
        if (value < MinSoulWarriorSummonLimit)
        {
            return MinSoulWarriorSummonLimit;
        }

        return value > MaxSoulWarriorSummonLimit ? MaxSoulWarriorSummonLimit : value;
    }
}
