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
    private static void HandlePhaseControl(PlayMakerFSM fsm)
    {
        if (collectorPhase == 1)
        {
            RemoveStateTransitionsByEvent(fsm, "Init", "FINISHED");
            return;
        }

        if (collectorPhase == 2)
        {
            FsmCompat.ChangeTransition(fsm, "Init", "FINISHED", "Phase 2");
            return;
        }

        ApplyPhaseThresholdHook(fsm);
    }

    private static void ApplyPhaseThresholdHook(PlayMakerFSM fsm)
    {
        try
        {
            FsmState? check = fsm.Fsm.GetState("Check");
            if (check == null || !patchedPhaseControls.Add(fsm.GetInstanceID()))
            {
                return;
            }

            FsmInt[] thresholds = check.Actions
                .OfType<IntCompare>()
                .Select(cmp => cmp.integer2)
                .Where(value => value != null)
                .ToArray();
            if (thresholds.Length == 0)
            {
                return;
            }

            int[] vanillaThresholds = new int[thresholds.Length];
            bool overridden = false;

            void SyncThreshold()
            {
                if (moduleActive && UseCustomPhase2Threshold)
                {
                    if (!overridden)
                    {
                        for (int i = 0; i < thresholds.Length; i++)
                        {
                            vanillaThresholds[i] = thresholds[i].Value;
                        }

                        overridden = true;
                    }

                    foreach (FsmInt threshold in thresholds)
                    {
                        threshold.Value = CustomPhase2Threshold;
                    }
                }
                else if (overridden)
                {
                    for (int i = 0; i < thresholds.Length; i++)
                    {
                        thresholds[i].Value = vanillaThresholds[i];
                    }

                    overridden = false;
                }
            }

            check.InsertCustomAction(SyncThreshold, 0);
            fsm.Fsm.GetState("Set")?.AddCustomAction(() =>
            {
                overridden = false;
                SyncThreshold();
            });
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "CollectorPhases.Phase.cs");
        }
    }

    private static void RemoveStateTransitionsByEvent(PlayMakerFSM fsm, string stateName, string eventName)
    {
        FsmState? state = fsm.Fsm.GetState(stateName);
        if (state?.Transitions == null || state.Transitions.Length == 0)
        {
            return;
        }

        state.Transitions = state.Transitions
            .Where(transition => transition.EventName != eventName)
            .ToArray();
    }
}
