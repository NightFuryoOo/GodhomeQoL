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
    private const string PatchFlag = "GodhomeQoL_InfiniteGrimmPufferfish_Patched";

    private static bool enteredFromWorkshop;

    private protected override void Load()
    {
        ModHooks.BeforeSceneLoadHook += BeforeSceneLoad;
        On.PlayMakerFSM.Start += ModifyFSM;
    }

    private protected override void Unload()
    {
        ModHooks.BeforeSceneLoadHook -= BeforeSceneLoad;
        On.PlayMakerFSM.Start -= ModifyFSM;
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

        if (BossSequenceController.IsInSequence)
        {
            return;
        }

        string sceneName = self.gameObject.scene.name;
        if (sceneName != GrimmScene && sceneName != NightmareScene)
        {
            return;
        }

        if (self.FsmName != ControlFsm)
        {
            return;
        }

        bool isNightmare = sceneName == NightmareScene;
        if (isNightmare)
        {
            if (!enteredFromWorkshop)
            {
                return;
            }

            if (self.gameObject.name != NightmareBossName && self.gameObject.name != GrimmBossName)
            {
                return;
            }
        }
        else if (self.gameObject.name != GrimmBossName)
        {
            return;
        }

        if (isNightmare)
        {
            if (!TryPatchMoveChoice(self))
            {
                TryPatchOutPause(self);
            }
        }
        else
        {
            TryPatchOutPause(self);
        }
    }

    private static bool TryPatchOutPause(PlayMakerFSM fsm)
    {
        FsmState? outPause = fsm.Fsm.GetState("Out Pause");
        if (outPause == null)
        {
            return false;
        }

        outPause.ChangeTransition(FsmEvent.Finished.Name, "Balloon Pos");
        LogDebug("Grimm FSM modified");
        return true;
    }

    private static bool TryPatchMoveChoice(PlayMakerFSM fsm)
    {
        FsmState? moveChoice = fsm.Fsm.GetState("Move Choice");
        if (moveChoice == null)
        {
            return false;
        }

        moveChoice.ChangeTransition("BALLOON", "Balloon Pos");

        if (!IsPatched(fsm))
        {
            moveChoice.InsertCustomAction(() => fsm.SendEvent("BALLOON"), 0);
            MarkPatched(fsm);
        }

        LogDebug("Grimm FSM modified (Move Choice)");
        return true;
    }

    private static bool IsPatched(PlayMakerFSM fsm)
    {
        FsmBool? flag = FindFsmBool(fsm, PatchFlag);
        return flag != null && flag.Value;
    }

    private static void MarkPatched(PlayMakerFSM fsm)
    {
        FsmBool? flag = FindFsmBool(fsm, PatchFlag);
        if (flag == null)
        {
            flag = new FsmBool(PatchFlag);
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
}
