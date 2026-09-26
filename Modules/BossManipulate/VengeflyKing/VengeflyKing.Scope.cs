using Modding;
using Satchel;
using Satchel.Futils;
using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class VengeflyKing : Module
{
    private static bool IsVengefly(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsVengeflyObject(hm.gameObject);
    }

    private static bool IsVengeflyObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        if (!string.Equals(gameObject.scene.name, VengeflyScene, StringComparison.Ordinal))
        {
            return false;
        }

        return GetVengeflySide(gameObject) != VengeflySide.Unknown;
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsVengeflyObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => vengeflyKingUseMaxHp;
    private static bool ShouldUseCustomSummonLimit() => vengeflyKingUseCustomSummonLimit && !vengeflyKingP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, VengeflyScene, StringComparison.Ordinal))
        {
            if (BossManipulateEntryGuard.IsAllowedBossEntry(currentScene, nextScene))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, VengeflyScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsBigBuzzerFsm(PlayMakerFSM fsm) =>
        fsm != null && string.Equals(fsm.FsmName, "Big Buzzer", StringComparison.Ordinal);
}
