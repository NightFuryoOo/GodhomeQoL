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

        ApplyPhaseThreshold(fsm);
        ApplyPhaseThresholdHook(fsm);
    }

    private static void ApplyPhaseThreshold(PlayMakerFSM fsm)
    {
        if (!UseCustomPhase2Threshold)
        {
            return;
        }

        try
        {
            foreach (FsmState state in fsm.FsmStates)
            {
                if (state?.Actions == null)
                {
                    continue;
                }

                foreach (IntCompare cmp in state.Actions.OfType<IntCompare>())
                {
                    if (cmp.integer2 != null)
                    {
                        cmp.integer2.Value = CustomPhase2Threshold;
                    }
                }
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "CollectorPhases.Phase.cs");
        }
    }

    private static void ApplyPhaseThresholdHook(PlayMakerFSM fsm)
    {
        if (!UseCustomPhase2Threshold)
        {
            return;
        }

        try
        {
            FsmState? check = fsm.Fsm.GetState("Check");
            if (check == null)
            {
                return;
            }

            check.InsertCustomAction(() =>
            {
                foreach (IntCompare cmp in check.Actions.OfType<IntCompare>())
                {
                    if (cmp.integer2 != null)
                    {
                        cmp.integer2.Value = CustomPhase2Threshold;
                    }
                }
            }, 0);
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
