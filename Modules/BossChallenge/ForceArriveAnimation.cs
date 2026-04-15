using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class ForceArriveAnimation : Module
{
    private sealed class DreamEntrySnapshot
    {
        public PlayMakerFSM? Fsm { get; init; }
        public int FsmId { get; init; }
        public string? StatueTransitionTarget { get; init; }
        public FsmEvent? FirstBossFalseEvent { get; init; }
        public bool Modified { get; set; }
    }

    private static readonly string[] scenes =
    [
        "GG_Vengefly",
        "GG_Vengefly_V",
        "GG_Ghost_Xero",
        "GG_Ghost_Xero_V",
        "GG_Hive_Knight",
        "GG_Crystal_Guardian_2"
    ];
    private static readonly Dictionary<int, DreamEntrySnapshot> snapshots = [];

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        snapshots.Clear();
        On.PlayMakerFSM.Start += ModifyDreamEntryFSM;
        TryPatchExistingDreamEntryFsm();
    }

    private protected override void Unload()
    {
        On.PlayMakerFSM.Start -= ModifyDreamEntryFSM;
        RestorePatchedDreamEntryFsms();
    }

    private static void ModifyDreamEntryFSM(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (ShouldPatch(self) && TryPatchDreamEntryFSM(self))
        {
            LogDebug("Dream Entry FSM modified");
        }
    }

    private static void TryPatchExistingDreamEntryFsm()
    {
        GameObject? dreamEntry = GameObject.Find("Dream Entry");
        PlayMakerFSM? fsm = dreamEntry?.LocateMyFSM("Control");
        if (fsm == null || !ShouldPatch(fsm))
        {
            return;
        }

        if (TryPatchDreamEntryFSM(fsm))
        {
            LogDebug("Dream Entry FSM modified");
        }
    }

    private static bool ShouldPatch(PlayMakerFSM fsm) =>
        fsm is { name: "Dream Entry", FsmName: "Control" }
        && scenes.Contains(fsm.gameObject.scene.name);

    private static bool TryPatchDreamEntryFSM(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (snapshots.TryGetValue(fsmId, out DreamEntrySnapshot? existing) && existing.Modified)
        {
            return false;
        }

        FsmState? firstBoss1 = fsm.Fsm.GetState("First Boss? 1");
        if (firstBoss1 == null)
        {
            return false;
        }

        string? statueTransitionTarget = GetTransitionTarget(firstBoss1, "STATUE");
        GGCheckIfBossSequence? firstBossCheck = fsm.GetAction<GGCheckIfBossSequence>("First Boss?", 0);
        GGCheckIfFirstBossScene? firstBossSceneCheck = fsm.GetAction<GGCheckIfFirstBossScene>("First Boss?", 1);
        if (string.IsNullOrEmpty(statueTransitionTarget) || firstBossCheck == null || firstBossSceneCheck == null)
        {
            return false;
        }

        DreamEntrySnapshot snapshot = new()
        {
            Fsm = fsm,
            FsmId = fsmId,
            StatueTransitionTarget = statueTransitionTarget,
            FirstBossFalseEvent = firstBossCheck.falseEvent,
            Modified = true
        };

        fsm.ChangeTransition("First Boss? 1", "STATUE", "Hide Player");
        firstBossCheck.falseEvent = firstBossSceneCheck.trueEvent;
        snapshots[fsmId] = snapshot;
        return true;
    }

    private static void RestorePatchedDreamEntryFsms()
    {
        foreach ((int id, DreamEntrySnapshot snapshot) in snapshots.ToArray())
        {
            PlayMakerFSM? fsm = snapshot.Fsm;
            if (fsm == null)
            {
                snapshots.Remove(id);
                continue;
            }

            if (!snapshot.Modified)
            {
                continue;
            }

            string? statueTarget = snapshot.StatueTransitionTarget;
            if (!string.IsNullOrEmpty(statueTarget))
            {
                TrySetTransition(fsm, "First Boss? 1", "STATUE", statueTarget!);
            }

            GGCheckIfBossSequence? firstBossCheck = fsm.GetAction<GGCheckIfBossSequence>("First Boss?", 0);
            if (firstBossCheck != null)
            {
                firstBossCheck.falseEvent = snapshot.FirstBossFalseEvent;
            }
        }

        snapshots.Clear();
    }

    private static string? GetTransitionTarget(FsmState state, string eventName)
    {
        foreach (FsmTransition transition in state.Transitions)
        {
            if (transition.EventName == eventName)
            {
                return transition.ToState;
            }
        }

        return null;
    }

    private static void TrySetTransition(PlayMakerFSM fsm, string stateName, string eventName, string targetState)
    {
        try
        {
            fsm.ChangeTransition(stateName, eventName, targetState);
        }
        catch
        {
            // Ignore missing states or transitions.
        }
    }
}
