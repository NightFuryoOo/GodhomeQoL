using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Satchel;
using Satchel.BetterMenus;
using Satchel.Futils;
using GodhomeQoL.Utils;

namespace GodhomeQoL.Modules.CollectorPhases;

public sealed partial class CollectorPhases : Module
{
    private static TAction? GetFirstActionOfType<TAction>(PlayMakerFSM fsm, string state) where TAction : FsmStateAction =>
        fsm.Fsm.GetState(state)
            ?.Actions
            ?.OfType<TAction>()
            .FirstOrDefault();

    private static void ApplySummonCounts(PlayMakerFSM fsm)
    {
        if (!DisableSummonLimit)
        {
            return;
        }

        try
        {
            FsmState? summon = fsm.Fsm.GetState("Summon?");
            FsmState? enemyCount = fsm.Fsm.GetState("Enemy Count");

            if (summon != null)
            {
                IntCompare? cmp = FindLikelySummonLimitCompare(summon);
                if (cmp?.integer2 != null)
                {
                    cmp.integer2.Value = CustomSummonLimit;
                }
            }

            if (enemyCount != null)
            {
                IntCompare? cmp = FindLikelySummonLimitCompare(enemyCount);
                if (cmp?.integer2 != null)
                {
                    cmp.integer2.Value = CustomSummonLimit + 3;
                }
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "CollectorPhases.Summons.cs");
        }
    }

    private static IntCompare? FindLikelySummonLimitCompare(FsmState state)
    {
        if (state?.Actions == null)
        {
            return null;
        }

        IntCompare? best = null;
        int bestValue = int.MinValue;

        foreach (IntCompare cmp in state.Actions.OfType<IntCompare>())
        {
            if (cmp?.integer2 == null)
            {
                continue;
            }

            int value = cmp.integer2.Value;
            if (value > bestValue)
            {
                best = cmp;
                bestValue = value;
            }
        }

        return bestValue >= 4 ? best : null;
    }

    private const string SpawnGateFlag = "GodhomeQoL_CollectorPhases_SpawnGated";

    private static void GateSpawnStates(PlayMakerFSM fsm)
    {
        if (IsSpawnGateInstalled(fsm))
        {
            return;
        }

        MarkSpawnGateInstalled(fsm);
        TryGateStateLive(fsm, "Spawn Buzzer", () => spawnBuzzer);
        TryGateStateLive(fsm, "Spawn Roller", () => spawnRoller);
        TryGateStateLive(fsm, "Spawn Spitter", () => spawnSpitter);
    }

    private static void TryGateStateLive(PlayMakerFSM fsm, string stateName, Func<bool> isEnabled)
    {
        try
        {
            fsm.InsertCustomAction(stateName, () =>
            {
                if (!isEnabled())
                {
                    fsm.SendEvent(FsmEvent.Finished.Name);
                }
            }, 0);
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "CollectorPhases.Summons.cs");
        }
    }

    private static bool IsSpawnGateInstalled(PlayMakerFSM fsm)
    {
        FsmBool? flag = FindFsmBool(fsm, SpawnGateFlag);
        return flag != null && flag.Value;
    }

    private static void MarkSpawnGateInstalled(PlayMakerFSM fsm)
    {
        FsmBool? flag = FindFsmBool(fsm, SpawnGateFlag);
        if (flag == null)
        {
            flag = new FsmBool(SpawnGateFlag);
            fsm.FsmVariables.BoolVariables = fsm.FsmVariables.BoolVariables.Append(flag).ToArray();
        }

        flag.Value = true;
    }

    private static FsmBool? FindFsmBool(PlayMakerFSM fsm, string name)
    {
        foreach (FsmBool variable in fsm.FsmVariables.BoolVariables)
        {
            if (variable.Name == name)
            {
                return variable;
            }
        }

        return null;
    }

    private static void FilterCollectorSummonPool(PlayMakerFSM fsm)
    {
        try
        {
            FsmInt? count = fsm.FsmVariables.FindFsmInt("Selection Count");
            FsmArray? pool = fsm.FsmVariables.FindFsmArray("Selection Pool");
            if (count == null || pool == null)
            {
                return;
            }

            object[] original = pool.Values ?? Array.Empty<object>();

            List<int> enabledTypes = [];
            if (spawnBuzzer) enabledTypes.Add(0);
            if (spawnRoller) enabledTypes.Add(1);
            if (spawnSpitter) enabledTypes.Add(2);

            if (enabledTypes.Count == 1)
            {
                int only = enabledTypes[0];
                object? candidate = original.FirstOrDefault(o => MatchesType(o, only));
                pool.Values = new object[] { candidate ?? only };
                count.Value = 1;
                return;
            }
            bool Keep(object obj)
            {
                switch (obj)
                {
                    case int i:
                        return i switch
                        {
                            0 => spawnBuzzer,
                            1 => spawnRoller,
                            2 => spawnSpitter,
                            _ => true
                        };
                    case float f:
                        return ((int)f) switch
                        {
                            0 => spawnBuzzer,
                            1 => spawnRoller,
                            2 => spawnSpitter,
                            _ => true
                        };
                    case GameObject go:
                        string n = go.name;
                        if (!spawnBuzzer && IsBuzzerName(n)) return false;
                        if (!spawnRoller && IsRollerName(n)) return false;
                        if (!spawnSpitter && IsSpitterName(n)) return false;
                        return true;
                    case string s when !spawnBuzzer && IsBuzzerName(s):
                        return false;
                    case string s when !spawnRoller && IsRollerName(s):
                        return false;
                    case string s when !spawnSpitter && IsSpitterName(s):
                        return false;
                    default:
                        return true;
                }
            }

            List<object> filtered = original.Where(Keep).ToList();

            if (filtered.Count == 0)
            {
                List<object> fallback = [];
                if (spawnBuzzer) fallback.Add(0);
                if (spawnRoller) fallback.Add(1);
                if (spawnSpitter) fallback.Add(2);

                if (fallback.Count == 0)
                {
                    pool.Values = Array.Empty<object>();
                    count.Value = 0;
                    return;
                }

                filtered.AddRange(fallback);
            }

            pool.Values = filtered.ToArray();
            count.Value = filtered.Count;
        }
        catch (Exception e)
        {
            LogWarn($"Failed to filter collector summon pool: {e.Message}");
        }
    }

    private static bool MatchesType(object obj, int typeIndex)
    {
        return obj switch
        {
            int i => i == typeIndex,
            float f => (int)f == typeIndex,
            GameObject go => typeIndex switch
            {
                0 => IsBuzzerName(go.name),
                1 => IsRollerName(go.name),
                2 => IsSpitterName(go.name),
                _ => false
            },
            string s => typeIndex switch
            {
                0 => IsBuzzerName(s),
                1 => IsRollerName(s),
                2 => IsSpitterName(s),
                _ => false
            },
            _ => false
        };
    }

    private static string? GetFirstEnabledState()
    {
        if (spawnBuzzer)
        {
            return "Buzzer";
        }

        if (spawnRoller)
        {
            return "Roller";
        }

        if (spawnSpitter)
        {
            return "Spitter";
        }

        return null;
    }

    private static void TryGateSpawnerFSM(GameObject go)
    {
        if (go == null || !IsCollectorScene(go.scene.name))
        {
            return;
        }

        PlayMakerFSM[] fsms = go.GetComponents<PlayMakerFSM>();
        if (fsms == null || fsms.Length == 0)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in fsms)
        {
            if (fsm == null)
            {
                continue;
            }

            bool hasAny =
                HasState(fsm, "Buzzer") ||
                HasState(fsm, "Roller") ||
                HasState(fsm, "Spitter") ||
                HasState(fsm, "Spawn Buzzer") ||
                HasState(fsm, "Spawn Roller") ||
                HasState(fsm, "Spawn Spitter");

            if (!hasAny || IsSpawnGateInstalled(fsm))
            {
                continue;
            }

            MarkSpawnGateInstalled(fsm);
            GateSpawnerStateLive(fsm, "Buzzer", () => spawnBuzzer);
            GateSpawnerStateLive(fsm, "Roller", () => spawnRoller);
            GateSpawnerStateLive(fsm, "Spitter", () => spawnSpitter);

            GateSpawnerStateLive(fsm, "Spawn Buzzer", () => spawnBuzzer);
            GateSpawnerStateLive(fsm, "Spawn Roller", () => spawnRoller);
            GateSpawnerStateLive(fsm, "Spawn Spitter", () => spawnSpitter);
        }
    }

    private static bool HasState(PlayMakerFSM fsm, string stateName) =>
        fsm.FsmStates.Any(s => s?.Name == stateName);

    private static void GateSpawnerStateLive(PlayMakerFSM fsm, string stateName, Func<bool> isEnabled)
    {
        if (!HasState(fsm, stateName))
        {
            return;
        }

        try
        {
            fsm.InsertCustomAction(stateName, () =>
            {
                if (isEnabled())
                {
                    return;
                }

                string? fallback = GetFirstEnabledState();
                if (fallback == null)
                {
                    UObject.Destroy(fsm.gameObject);
                    return;
                }

                fsm.SetState(fallback);
            }, 0);
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "CollectorPhases.Summons.cs");
        }
    }

    private static void SetIntVariable(PlayMakerFSM fsm, string name, int value)
    {
        FsmInt? v = fsm.FsmVariables.FindFsmInt(name);
        if (v != null)
        {
            v.Value = value;
        }
        else
        {
            LogWarn($"FSM int variable '{name}' not found on {fsm.gameObject.name}");
        }
    }
}
