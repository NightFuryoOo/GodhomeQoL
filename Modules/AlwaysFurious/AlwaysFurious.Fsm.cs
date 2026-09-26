using GodhomeQoL.Modules.Tools;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class AlwaysFurious : Module
{
    private void TryModifyExistingFsm(bool forceEnable = false)
    {
        CleanupSnapshotCache();

        GameObject? charmEffects = GameObject.Find("Charm Effects");
        if (charmEffects == null)
        {
            return;
        }

        PlayMakerFSM? fsm = charmEffects.LocateMyFSM("Fury");
        if (fsm == null)
        {
            return;
        }

        TryModifyFuryFsm(fsm, forceEnable);
    }

    private static void TryModifyFuryFsm(PlayMakerFSM fsm, bool forceEnable = false)
    {
        FuryFsmSnapshot snapshot = GetOrCreateSnapshot(fsm);
        if (snapshot.Modified)
        {
            if (forceEnable)
            {
                TryForceEnable(fsm);
            }

            return;
        }

        if (!snapshot.HadEquipGlobalTransition)
        {
            FsmCompat.AddGlobalTransition(fsm, "CHARM EQUIP CHECK", "Check HP");
        }

        TrySetTransition(fsm, "Init", "FINISHED", "Check HP");
        TrySetTransition(fsm, "Check HP", "CANCEL", "Deactivate");
        TrySetTransition(fsm, "Activate", "HERO HEALED FULL", "Recheck");
        TrySetTransition(fsm, "Stay Furied", "HERO HEALED FULL", "Recheck");
        TrySetTransition(fsm, "Recheck", "FINISHED", "Stay Furied");
        snapshot.Modified = true;
        TryForceEnable(fsm);
    }

    private static FuryFsmSnapshot GetOrCreateSnapshot(PlayMakerFSM fsm)
    {
        CleanupSnapshotCache();

        int id = fsm.GetInstanceID();
        if (FurySnapshots.TryGetValue(id, out FuryFsmSnapshot? snapshot))
        {
            return snapshot;
        }

        snapshot = new FuryFsmSnapshot
        {
            Fsm = fsm,
            HadEquipGlobalTransition = HasGlobalTransition(fsm, "CHARM EQUIP CHECK"),
            InitFinished = GetTransitionTarget(fsm, "Init", "FINISHED"),
            CheckHpCancel = GetTransitionTarget(fsm, "Check HP", "CANCEL"),
            ActivateHealedFull = GetTransitionTarget(fsm, "Activate", "HERO HEALED FULL"),
            StayFuriedHealedFull = GetTransitionTarget(fsm, "Stay Furied", "HERO HEALED FULL"),
            RecheckFinished = GetTransitionTarget(fsm, "Recheck", "FINISHED"),
            Modified = false
        };

        FurySnapshots[id] = snapshot;
        return snapshot;
    }

    private static void CleanupSnapshotCache()
    {
        foreach ((int id, FuryFsmSnapshot snapshot) in FurySnapshots.ToArray())
        {
            if (snapshot.Fsm == null)
            {
                FurySnapshots.Remove(id);
            }
        }
    }

    private static void RestoreModifiedFsms()
    {
        foreach ((int id, FuryFsmSnapshot snapshot) in FurySnapshots.ToArray())
        {
            PlayMakerFSM? fsm = snapshot.Fsm;
            if (fsm == null)
            {
                FurySnapshots.Remove(id);
                continue;
            }

            if (!snapshot.Modified)
            {
                continue;
            }

            if (!snapshot.HadEquipGlobalTransition)
            {
                TryRemoveGlobalTransition(fsm, "CHARM EQUIP CHECK");
            }

            TrySetTransition(fsm, "Init", "FINISHED", snapshot.InitFinished);
            TrySetTransition(fsm, "Check HP", "CANCEL", snapshot.CheckHpCancel);
            TrySetTransition(fsm, "Activate", "HERO HEALED FULL", snapshot.ActivateHealedFull);
            TrySetTransition(fsm, "Stay Furied", "HERO HEALED FULL", snapshot.StayFuriedHealedFull);
            TrySetTransition(fsm, "Recheck", "FINISHED", snapshot.RecheckFinished);
            TryForceRecheck(fsm);
            snapshot.Modified = false;
        }

        CleanupSnapshotCache();
    }

    private static bool HasGlobalTransition(PlayMakerFSM fsm, string eventName)
    {
        foreach (FsmTransition transition in fsm.Fsm.GlobalTransitions)
        {
            if (transition.EventName == eventName)
            {
                return true;
            }
        }

        return false;
    }

    private static string? GetTransitionTarget(PlayMakerFSM fsm, string stateName, string eventName)
    {
        FsmState? state = fsm.Fsm.GetState(stateName);
        if (state == null)
        {
            return null;
        }

        foreach (FsmTransition transition in state.Transitions)
        {
            if (transition.EventName == eventName)
            {
                return transition.ToState;
            }
        }

        return null;
    }

    private static void TrySetTransition(PlayMakerFSM fsm, string stateName, string eventName, string? targetState)
    {
        if (string.IsNullOrEmpty(targetState))
        {
            return;
        }

        try
        {
            FsmCompat.ChangeTransition(fsm, stateName, eventName, targetState);
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "AlwaysFurious.Fsm.cs");
        }
    }

    private static void TryRemoveGlobalTransition(PlayMakerFSM fsm, string eventName)
    {
        try
        {
            FsmCompat.RemoveGlobalTransition(fsm, eventName);
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "AlwaysFurious.Fsm.cs");
        }
    }
}
