using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class AbsoluteRadianceHelper : Module
{
    private static bool IsAbsoluteRadiance(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsAbsoluteRadianceObject(hm.gameObject);
    }

    private static bool IsAbsoluteRadianceObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, AbsoluteRadianceScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(AbsoluteRadianceName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsAbsoluteRadianceObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => absoluteRadianceUseMaxHp;
    private static bool ShouldUseCustomPhaseThresholds() => absoluteRadianceUseCustomPhase && !absoluteRadianceP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, AbsoluteRadianceScene, StringComparison.Ordinal))
        {
            if (BossManipulateEntryGuard.IsAllowedBossEntry(currentScene, nextScene))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, AbsoluteRadianceScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsAbsoluteRadiancePhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsAbsoluteRadianceObject(fsm.gameObject))
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, AbsoluteRadiancePhaseControlFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        if (string.Equals(fsm.Fsm?.Name, AbsoluteRadiancePhaseControlFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState("Check 1") != null
            && fsm.Fsm?.GetState("Check 4") != null
            && fsm.FsmVariables.GetFsmInt(AbsoluteRadiancePhase2VariableName) != null;
    }

    private static bool IsAbsoluteRadianceControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsAbsoluteRadianceObject(fsm.gameObject))
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, AbsoluteRadianceControlFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        if (string.Equals(fsm.Fsm?.Name, AbsoluteRadianceControlFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return TryGetFinalPhaseSetHpAction(fsm, out _);
    }
}
