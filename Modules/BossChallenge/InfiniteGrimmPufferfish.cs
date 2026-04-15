using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class InfiniteGrimmPufferfish : Module
{
    private const string GrimmScene = "GG_Grimm";
    private const string NightmareScene = "GG_Grimm_Nightmare";
    private const string WorkshopScene = "GG_Workshop";
    private const string GrimmBossName = "Grimm Boss";
    private const string NightmareBossName = "Nightmare Grimm Boss";
    private const string ControlFsm = "Control";

    private sealed class GrimmFsmSnapshot
    {
        public PlayMakerFSM? Fsm { get; init; }
        public int FsmId { get; init; }
        public bool ModifiedOutPause { get; set; }
        public bool ModifiedMoveChoice { get; set; }
        public string? OutPauseFinishedTarget { get; set; }
        public string? MoveChoiceBalloonTarget { get; set; }
        public FsmStateAction[] MoveChoiceActions { get; set; } = Array.Empty<FsmStateAction>();
    }

    private sealed class GrimmPatchMarker : MonoBehaviour
    {
        public int patchedFsmId;
    }

    private static bool enteredFromWorkshop;
    private static readonly Dictionary<int, GrimmFsmSnapshot> patchedFsms = [];

    private protected override void Load()
    {
        patchedFsms.Clear();
        ModHooks.BeforeSceneLoadHook += BeforeSceneLoad;
        On.PlayMakerFSM.Start += ModifyFSM;
        TryPatchExistingFsms();
    }

    private protected override void Unload()
    {
        ModHooks.BeforeSceneLoadHook -= BeforeSceneLoad;
        On.PlayMakerFSM.Start -= ModifyFSM;
        RestorePatchedFsms();
        enteredFromWorkshop = false;
    }

    private static string BeforeSceneLoad(string newSceneName)
    {
        string currentScene = USceneManager.GetActiveScene().name;

        if (newSceneName == NightmareScene)
        {
            if (currentScene != NightmareScene)
            {
                enteredFromWorkshop = currentScene == WorkshopScene;
            }

            return newSceneName;
        }

        enteredFromWorkshop = false;
        return newSceneName;
    }

    private static void ModifyFSM(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!ShouldPatchFsm(self, out bool isNightmare))
        {
            return;
        }

        if (TryPatch(self, isNightmare))
        {
            LogDebug(isNightmare ? "Grimm FSM modified (Move Choice)" : "Grimm FSM modified");
        }
    }

    private static void TryPatchExistingFsms()
    {
        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null)
            {
                continue;
            }

            if (!ShouldPatchFsm(fsm, out bool isNightmare))
            {
                continue;
            }

            if (TryPatch(fsm, isNightmare))
            {
                LogDebug(isNightmare ? "Grimm FSM modified (Move Choice)" : "Grimm FSM modified");
            }
        }
    }

    private static bool ShouldPatchFsm(PlayMakerFSM fsm, out bool isNightmare)
    {
        isNightmare = false;

        if (BossSequenceController.IsInSequence)
        {
            return false;
        }

        string sceneName = fsm.gameObject.scene.name;
        if (sceneName != GrimmScene && sceneName != NightmareScene)
        {
            return false;
        }

        if (fsm.FsmName != ControlFsm)
        {
            return false;
        }

        isNightmare = sceneName == NightmareScene;
        if (isNightmare)
        {
            if (!enteredFromWorkshop)
            {
                return false;
            }

            return fsm.gameObject.name == NightmareBossName || fsm.gameObject.name == GrimmBossName;
        }

        return fsm.gameObject.name == GrimmBossName;
    }

    private static bool TryPatch(PlayMakerFSM fsm, bool isNightmare)
    {
        int fsmId = fsm.GetInstanceID();
        if (patchedFsms.ContainsKey(fsmId))
        {
            return false;
        }

        GrimmPatchMarker? marker = fsm.gameObject.GetComponent<GrimmPatchMarker>();
        if (marker != null && marker.patchedFsmId == fsmId)
        {
            return false;
        }

        GrimmFsmSnapshot snapshot = new()
        {
            Fsm = fsm,
            FsmId = fsmId
        };

        bool patched = isNightmare
            ? TryPatchMoveChoice(fsm, snapshot) || TryPatchOutPause(fsm, snapshot)
            : TryPatchOutPause(fsm, snapshot);
        if (!patched)
        {
            return false;
        }

        patchedFsms[fsmId] = snapshot;

        if (marker == null)
        {
            marker = fsm.gameObject.AddComponent<GrimmPatchMarker>();
        }

        marker.patchedFsmId = fsmId;
        return true;
    }

    private static void RestorePatchedFsms()
    {
        foreach ((_, GrimmFsmSnapshot snapshot) in patchedFsms)
        {
            PlayMakerFSM? fsm = snapshot.Fsm;
            if (fsm == null)
            {
                continue;
            }

            string? outPauseTarget = snapshot.OutPauseFinishedTarget;
            if (snapshot.ModifiedOutPause && !string.IsNullOrEmpty(outPauseTarget))
            {
                TrySetTransition(fsm, "Out Pause", FsmEvent.Finished.Name, outPauseTarget!);
            }

            if (snapshot.ModifiedMoveChoice)
            {
                FsmState? moveChoice = fsm.Fsm.GetState("Move Choice");
                if (moveChoice != null)
                {
                    moveChoice.Actions = snapshot.MoveChoiceActions.ToArray();
                }

                string? moveChoiceTarget = snapshot.MoveChoiceBalloonTarget;
                if (!string.IsNullOrEmpty(moveChoiceTarget))
                {
                    TrySetTransition(fsm, "Move Choice", "BALLOON", moveChoiceTarget!);
                }
            }

            GrimmPatchMarker? marker = fsm.gameObject.GetComponent<GrimmPatchMarker>();
            if (marker != null && marker.patchedFsmId == snapshot.FsmId)
            {
                UObject.Destroy(marker);
            }
        }

        patchedFsms.Clear();
    }

    private static bool TryPatchOutPause(PlayMakerFSM fsm, GrimmFsmSnapshot snapshot)
    {
        FsmState? outPause = fsm.Fsm.GetState("Out Pause");
        if (outPause == null)
        {
            return false;
        }

        string? originalTarget = GetTransitionTarget(outPause, FsmEvent.Finished.Name);
        if (string.IsNullOrEmpty(originalTarget))
        {
            return false;
        }

        snapshot.OutPauseFinishedTarget = originalTarget;
        outPause.ChangeTransition(FsmEvent.Finished.Name, "Balloon Pos");
        snapshot.ModifiedOutPause = true;
        return true;
    }

    private static bool TryPatchMoveChoice(PlayMakerFSM fsm, GrimmFsmSnapshot snapshot)
    {
        FsmState? moveChoice = fsm.Fsm.GetState("Move Choice");
        if (moveChoice == null)
        {
            return false;
        }

        string? originalTarget = GetTransitionTarget(moveChoice, "BALLOON");
        if (string.IsNullOrEmpty(originalTarget))
        {
            return false;
        }

        snapshot.MoveChoiceBalloonTarget = originalTarget;
        snapshot.MoveChoiceActions = moveChoice.Actions?.ToArray() ?? Array.Empty<FsmStateAction>();
        moveChoice.ChangeTransition("BALLOON", "Balloon Pos");
        moveChoice.InsertCustomAction(() => fsm.SendEvent("BALLOON"), 0);
        snapshot.ModifiedMoveChoice = true;
        return true;
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
            // ignore missing states or transitions
        }
    }
}
