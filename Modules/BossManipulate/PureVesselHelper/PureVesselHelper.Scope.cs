using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class PureVesselHelper : Module
{
    private static bool IsPureVessel(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsPureVesselObject(hm.gameObject);
    }

    private static bool IsPureVesselObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, PureVesselScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(PureVesselName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsPureVesselObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => pureVesselUseMaxHp;
    private static bool ShouldUseCustomPhaseThresholds() => pureVesselUseCustomPhase && !pureVesselP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, PureVesselScene, StringComparison.Ordinal))
        {
            if (BossManipulateEntryGuard.IsAllowedBossEntry(currentScene, nextScene))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, PureVesselScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsPureVesselPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsPureVesselObject(fsm.gameObject))
        {
            return false;
        }

        return string.Equals(fsm.FsmName, "Control", StringComparison.Ordinal)
            || fsm.Fsm?.GetState("Phase?") != null;
    }
}
