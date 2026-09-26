using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class SoulWarriorHelper : Module
{
    private static IntCompare? FindSummonEnemyCountCompareAction(PlayMakerFSM fsm)
    {
        List<IntCompare> compares = FindSummonEnemyCountCompareActions(fsm);
        return compares.Count > 0 ? compares[0] : null;
    }

    private static List<FsmState> GetLikelySummonStates(PlayMakerFSM fsm)
    {
        List<FsmState> result = [];
        if (fsm?.Fsm?.States == null)
        {
            return result;
        }

        foreach (FsmState state in fsm.Fsm.States)
        {
            if (state != null && IsLikelySummonStateName(state.Name))
            {
                result.Add(state);
            }
        }

        if (result.Count > 0)
        {
            return result;
        }

        foreach (string hint in SoulWarriorSummonStateHints)
        {
            FsmState? state = fsm.Fsm.GetState(hint);
            if (state != null && !result.Contains(state))
            {
                result.Add(state);
            }
        }

        return result;
    }

    private static bool HasLikelySummonState(PlayMakerFSM fsm)
    {
        return GetLikelySummonStates(fsm).Count > 0;
    }

    private static List<IntCompare> FindSummonEnemyCountCompareActions(PlayMakerFSM fsm)
    {
        if (fsm?.Fsm == null)
        {
            return [];
        }

        IntCompare? best = null;
        int bestScore = int.MinValue;
        HashSet<IntCompare> matches = [];

        List<FsmState> candidateStates = GetLikelySummonStates(fsm);
        if (candidateStates.Count == 0 && fsm.Fsm.States != null)
        {
            candidateStates.AddRange(fsm.Fsm.States.Where(state => state != null));
        }

        foreach (FsmState state in candidateStates)
        {
            if (state?.Actions == null)
            {
                continue;
            }

            foreach (FsmStateAction action in state.Actions)
            {
                if (action is not IntCompare compare)
                {
                    continue;
                }

                int score = GetSummonLimitCompareScore(compare);
                if (score > bestScore)
                {
                    best = compare;
                    bestScore = score;
                }

                if (score > 0)
                {
                    matches.Add(compare);
                }
            }
        }

        if (bestScore > 0 && best != null)
        {
            matches.Add(best);
        }

        return matches.ToList();
    }

    private static int GetSummonLimitCompareScore(IntCompare compare)
    {
        if (compare == null)
        {
            return int.MinValue;
        }

        int score = 0;
        if (IsLikelySummonCountVariable(compare.integer1))
        {
            score += 100;
        }

        if (IsLikelySummonCountVariable(compare.integer2))
        {
            score += 100;
        }

        int candidateValue = ReadSummonLimitThreshold(compare, fallback: -1);
        if (candidateValue >= 0)
        {
            if (candidateValue >= 2 && candidateValue <= 200)
            {
                score += 20;
            }

            if (candidateValue == DefaultSoulWarriorVanillaSummonLimit)
            {
                score += 40;
            }
        }

        return score;
    }

    private static int ReadSummonLimitThreshold(IntCompare compare, int fallback)
    {
        if (compare == null)
        {
            return fallback;
        }

        if (IsLikelySummonCountVariable(compare.integer1) && compare.integer2 != null)
        {
            return compare.integer2.Value;
        }

        if (IsLikelySummonCountVariable(compare.integer2) && compare.integer1 != null)
        {
            return compare.integer1.Value;
        }

        if (compare.integer2 != null)
        {
            return compare.integer2.Value;
        }

        if (compare.integer1 != null)
        {
            return compare.integer1.Value;
        }

        return fallback;
    }
}
