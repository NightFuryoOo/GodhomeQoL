using Modding;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class FlukemarmHelper : Module
{
    private static bool IsFlukemarm(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsFlukemarmObject(hm.gameObject);
    }

    private static bool IsFlukemarmObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        if (!string.Equals(gameObject.scene.name, FlukemarmScene, StringComparison.Ordinal))
        {
            return false;
        }

        return GetFlukemarmTarget(gameObject) != FlukemarmTarget.Unknown;
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsFlukemarmObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp(FlukemarmTarget target)
    {
        return target switch
        {
            FlukemarmTarget.Boss => flukemarmUseMaxHp,
            FlukemarmTarget.Fly => flukemarmUseMaxHp && !flukemarmP5Hp,
            _ => false,
        };
    }

    private static bool ShouldUseCustomSummonLimit() => flukemarmUseCustomSummonLimit && !flukemarmP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, FlukemarmScene, StringComparison.Ordinal))
        {
            if (BossManipulateEntryGuard.IsAllowedBossEntry(currentScene, nextScene))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, FlukemarmScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsFlukemarmSummonControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null)
        {
            return false;
        }

        if (GetFlukemarmTarget(fsm.gameObject) != FlukemarmTarget.Boss)
        {
            return false;
        }

        return fsm.FsmVariables?.FindFsmInt("Spawned Max") != null
            || fsm.Fsm?.GetState("Check Spawn") != null;
    }
}
