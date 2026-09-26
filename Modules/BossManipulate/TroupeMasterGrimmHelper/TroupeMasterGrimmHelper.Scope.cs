using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class TroupeMasterGrimmHelper : Module
{
    private static bool IsTroupeMasterGrimm(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsTroupeMasterGrimmObject(hm.gameObject);
    }

    private static bool IsTroupeMasterGrimmObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, TroupeMasterGrimmScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(TroupeMasterGrimmName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsTroupeMasterGrimmObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => troupeMasterGrimmUseMaxHp;
    private static bool ShouldUseCustomPhaseThresholds() => troupeMasterGrimmUseCustomPhase && !troupeMasterGrimmP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, TroupeMasterGrimmScene, StringComparison.Ordinal))
        {
            if (BossManipulateEntryGuard.IsAllowedBossEntry(currentScene, nextScene))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, TroupeMasterGrimmScene, StringComparison.Ordinal) && hoGEntryAllowed)
            {
                hoGEntryAllowed = true;
            }
            else
            {
                hoGEntryAllowed = false;
            }

            return;
        }

        hoGEntryAllowed = false;
    }

    private static bool IsTroupeMasterGrimmPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsTroupeMasterGrimmObject(fsm.gameObject))
        {
            return false;
        }

        return string.Equals(fsm.FsmName, "Control", StringComparison.Ordinal)
            || fsm.Fsm?.GetState("Balloon?") != null;
    }
}
