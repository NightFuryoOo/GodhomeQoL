using HutongGames.PlayMaker;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class NightmareKingGrimmHelper : Module
{
    private static bool IsNightmareKingGrimm(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsNightmareKingGrimmObject(hm.gameObject);
    }

    private static bool IsNightmareKingGrimmObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, NightmareKingGrimmScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(NightmareKingGrimmName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsNightmareKingGrimmObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => nightmareKingGrimmUseMaxHp;
    private static bool ShouldUseCustomPhaseThresholds() => nightmareKingGrimmUseCustomPhase && !nightmareKingGrimmP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, NightmareKingGrimmScene, StringComparison.Ordinal))
        {
            if (BossManipulateEntryGuard.IsAllowedBossEntry(currentScene, nextScene))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, NightmareKingGrimmScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsNightmareKingGrimmPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsNightmareKingGrimmObject(fsm.gameObject))
        {
            return false;
        }

        return string.Equals(fsm.FsmName, "Control", StringComparison.Ordinal)
            || fsm.Fsm?.GetState("Balloon?") != null;
    }
}
