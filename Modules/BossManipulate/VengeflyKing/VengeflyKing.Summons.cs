using Modding;
using Satchel;
using Satchel.Futils;
using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class VengeflyKing : Module
{
    private const int DefaultSummonLimit = 4;
    private const int DefaultSummonAttackLimit = 15;
    private const int MinSummonLimit = 0;
    private const int MaxSummonLimit = 999;
    private const int MinSummonAttackLimit = 0;
    private const int MaxSummonAttackLimit = 999;

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
            if (fsm == null || fsm.gameObject == null || !IsBigBuzzerFsm(fsm))
            {
                continue;
            }

            if (!IsVengeflyObject(fsm.gameObject))
            {
                continue;
            }

            RememberVanillaSummonAttackLimit(fsm);
            SetSummonLimitOnBigBuzzerFsm(fsm, DefaultSummonLimit);
            SetSummonAttackLimitOnBigBuzzerFsm(fsm, GetVanillaSummonAttackLimit(fsm));
        }
    }

    private static void ApplySummonLimitSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsBigBuzzerFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        VengeflySide side = GetVengeflySide(fsm.gameObject);
        if (side != VengeflySide.Left && side != VengeflySide.Right)
        {
            return;
        }

        RememberVanillaSummonAttackLimit(fsm);

        int targetLimit = DefaultSummonLimit;
        int targetSummonAttackLimit = GetVanillaSummonAttackLimit(fsm);
        if (ShouldUseCustomSummonLimit())
        {
            targetLimit = GetConfiguredSummonLimit(side);
            targetSummonAttackLimit = GetConfiguredSummonAttackLimit(side);
        }

        SetSummonLimitOnBigBuzzerFsm(fsm, targetLimit);
        SetSummonAttackLimitOnBigBuzzerFsm(fsm, targetSummonAttackLimit);
    }

    private static void SetSummonLimitOnBigBuzzerFsm(PlayMakerFSM fsm, int value)
    {
        int clampedLimit = ClampSummonLimit(value);
        int patched = SetStateIntCompareThreshold(fsm, "Check Summon GG", clampedLimit);

        if (patched == 0)
        {
            _ = SetStateIntCompareThreshold(fsm, "Wait Frame", clampedLimit);
        }
    }

    private static int SetStateIntCompareThreshold(PlayMakerFSM fsm, string stateName, int threshold)
    {
        FsmState? state = fsm.Fsm?.GetState(stateName);
        if (state?.Actions == null)
        {
            return 0;
        }

        int patched = 0;
        foreach (IntCompare compare in state.Actions.OfType<IntCompare>())
        {
            if (compare.integer2 == null)
            {
                continue;
            }

            compare.integer2.Value = threshold;
            patched++;
        }

        return patched;
    }

    private static int GetConfiguredSummonLimit(VengeflySide side)
    {
        return side switch
        {
            VengeflySide.Left => ClampSummonLimit(vengeflyKingLeftSummonLimit),
            VengeflySide.Right => ClampSummonLimit(vengeflyKingRightSummonLimit),
            _ => DefaultSummonLimit,
        };
    }

    private static int GetConfiguredSummonAttackLimit(VengeflySide side)
    {
        return side switch
        {
            VengeflySide.Left => ClampSummonAttackLimit(vengeflyKingLeftSummonAttackLimit),
            VengeflySide.Right => ClampSummonAttackLimit(vengeflyKingRightSummonAttackLimit),
            _ => DefaultSummonAttackLimit,
        };
    }

    private static void SetSummonAttackLimitOnBigBuzzerFsm(PlayMakerFSM fsm, int value)
    {
        FsmInt? summons = fsm.FsmVariables?.FindFsmInt("Summons");
        if (summons == null)
        {
            return;
        }

        summons.Value = ClampSummonAttackLimit(value);
    }

    private static void RememberVanillaSummonAttackLimit(PlayMakerFSM fsm)
    {
        int id = fsm.GetInstanceID();
        if (vanillaSummonAttackLimitByFsm.ContainsKey(id))
        {
            return;
        }

        FsmInt? summons = fsm.FsmVariables?.FindFsmInt("Summons");
        int value = summons?.Value ?? DefaultSummonAttackLimit;
        vanillaSummonAttackLimitByFsm[id] = ClampSummonAttackLimit(value);
    }

    private static int GetVanillaSummonAttackLimit(PlayMakerFSM fsm)
    {
        int id = fsm.GetInstanceID();
        if (vanillaSummonAttackLimitByFsm.TryGetValue(id, out int stored))
        {
            return ClampSummonAttackLimit(stored);
        }

        FsmInt? summons = fsm.FsmVariables?.FindFsmInt("Summons");
        int value = ClampSummonAttackLimit(summons?.Value ?? DefaultSummonAttackLimit);
        vanillaSummonAttackLimitByFsm[id] = value;
        return value;
    }

    private static int ClampSummonLimit(int value)
    {
        if (value < MinSummonLimit)
        {
            return MinSummonLimit;
        }

        return value > MaxSummonLimit ? MaxSummonLimit : value;
    }

    private static int ClampSummonAttackLimit(int value)
    {
        if (value < MinSummonAttackLimit)
        {
            return MinSummonAttackLimit;
        }

        return value > MaxSummonAttackLimit ? MaxSummonAttackLimit : value;
    }
}
