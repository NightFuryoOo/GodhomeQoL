using Modding;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class HiveKnightHelper : Module
{
    private static bool IsHiveKnight(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsHiveKnightObject(hm.gameObject);
    }

    private static bool IsHiveKnightObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, HiveKnightScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(HiveKnightName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsHiveKnightObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => hiveKnightUseMaxHp;
    private static bool ShouldUseCustomPhaseThresholds() => hiveKnightUseCustomPhase && !hiveKnightP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, HiveKnightScene, StringComparison.Ordinal))
        {
            if (BossManipulateEntryGuard.IsAllowedBossEntry(currentScene, nextScene))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, HiveKnightScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsHiveKnightPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsHiveKnightObject(fsm.gameObject))
        {
            return false;
        }

        return string.Equals(fsm.FsmName, "Control", StringComparison.Ordinal)
            || fsm.Fsm?.GetState("Phase Check") != null;
    }
}
