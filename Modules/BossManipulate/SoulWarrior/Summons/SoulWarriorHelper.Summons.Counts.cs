using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class SoulWarriorHelper : Module
{
    private static void RememberVanillaSummonCount(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSummonCountByFsm.ContainsKey(fsmId))
        {
            return;
        }

        Dictionary<string, int> stored = new(StringComparer.OrdinalIgnoreCase);
        foreach (FsmInt countVar in FindSummonCountVariables(fsm))
        {
            if (countVar == null)
            {
                continue;
            }

            int value = countVar.Value;
            if (value <= 0)
            {
                value = ResolveVanillaSummonPoolSize(fsm);
            }

            if (value <= 0)
            {
                value = DefaultSoulWarriorVanillaSummonLimit;
            }

            stored[countVar.Name] = ClampSoulWarriorSummonLimit(value);
        }

        if (stored.Count == 0)
        {
            stored["__fallback__"] = DefaultSoulWarriorVanillaSummonLimit;
        }

        vanillaSummonCountByFsm[fsmId] = stored;
    }

    private static int GetVanillaSummonCount(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSummonCountByFsm.TryGetValue(fsmId, out Dictionary<string, int>? stored) && stored != null)
        {
            foreach (int count in stored.Values)
            {
                return ClampSoulWarriorSummonLimit(count);
            }
        }

        RememberVanillaSummonCount(fsm);
        if (vanillaSummonCountByFsm.TryGetValue(fsmId, out stored) && stored != null)
        {
            foreach (int count in stored.Values)
            {
                return ClampSoulWarriorSummonLimit(count);
            }
        }

        return DefaultSoulWarriorVanillaSummonLimit;
    }

    private static void SetSummonCountOnFsm(PlayMakerFSM fsm, int value, bool custom)
    {
        int clamped = ClampSoulWarriorSummonLimit(value);
        List<FsmInt> countVars = FindSummonCountVariables(fsm);
        if (custom)
        {
            foreach (FsmInt count in countVars)
            {
                if (count != null)
                {
                    count.Value = clamped;
                }
            }

            return;
        }

        RememberVanillaSummonCount(fsm);
        int fsmId = fsm.GetInstanceID();
        if (!vanillaSummonCountByFsm.TryGetValue(fsmId, out Dictionary<string, int>? stored) || stored == null)
        {
            return;
        }

        foreach (FsmInt count in countVars)
        {
            if (count == null)
            {
                continue;
            }

            if (stored.TryGetValue(count.Name, out int vanilla))
            {
                count.Value = ClampSoulWarriorSummonLimit(vanilla);
            }
            else
            {
                count.Value = GetVanillaSummonCount(fsm);
            }
        }
    }

    private static void ApplySummonCountSetterOverrides(PlayMakerFSM fsm, int value)
    {
        if (fsm?.Fsm?.States == null)
        {
            return;
        }

        int clamped = ClampSoulWarriorSummonLimit(value);
        List<FsmState> candidateStates = GetLikelySummonStates(fsm);
        if (candidateStates.Count == 0)
        {
            return;
        }

        foreach (FsmState state in candidateStates)
        {
            if (state?.Actions == null)
            {
                continue;
            }

            foreach (FsmStateAction action in state.Actions)
            {
                if (action is SetIntValue setInt)
                {
                    if (!IsLikelySummonCountVariable(setInt.intVariable) || setInt.intValue == null)
                    {
                        continue;
                    }

                    setInt.intValue.UseVariable = false;
                    setInt.intValue.Name = string.Empty;
                    setInt.intValue.Value = clamped;
                    continue;
                }

                if (action is SetFsmInt setFsm)
                {
                    string variableName = setFsm.variableName?.Value ?? string.Empty;
                    if (!IsKnownSummonCountVariableName(variableName))
                    {
                        continue;
                    }

                    if (setFsm.setValue == null)
                    {
                        continue;
                    }

                    setFsm.setValue.UseVariable = false;
                    setFsm.setValue.Name = string.Empty;
                    setFsm.setValue.Value = clamped;
                }
            }
        }
    }

    private static List<FsmInt> FindSummonCountVariables(PlayMakerFSM fsm)
    {
        List<FsmInt> result = [];
        if (fsm?.FsmVariables?.IntVariables == null)
        {
            return result;
        }

        foreach (string varName in SoulWarriorSummonCountVariableNames)
        {
            FsmInt? exact = fsm.FsmVariables.GetFsmInt(varName);
            if (exact != null && !result.Contains(exact))
            {
                result.Add(exact);
            }
        }

        if (result.Count > 0)
        {
            return result;
        }

        foreach (FsmInt intVar in fsm.FsmVariables.IntVariables)
        {
            if (intVar == null || !IsKnownSummonCountVariableName(intVar.Name))
            {
                continue;
            }

            if (!result.Contains(intVar))
            {
                result.Add(intVar);
            }
        }

        return result;
    }
}
