using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class WingedNoskHelper : Module
{
    private static bool IsWingedNosk(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsWingedNoskObject(hm.gameObject);
    }

    private static bool IsWingedNoskObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, WingedNoskScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(WingedNoskName, StringComparison.Ordinal);
    }

    private static bool IsWingedNoskSummon(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsWingedNoskSummonObject(hm.gameObject);
    }

    private static bool IsWingedNoskSummonObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        if (gameObject.name.StartsWith(WingedNoskName, StringComparison.Ordinal))
        {
            return false;
        }

        bool matchesSummonName = false;
        foreach (string hint in WingedNoskSummonNameHints)
        {
            if (gameObject.name.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                matchesSummonName = true;
                break;
            }
        }

        if (!matchesSummonName)
        {
            return false;
        }

        if (string.Equals(gameObject.scene.name, WingedNoskScene, StringComparison.Ordinal))
        {
            return true;
        }

        return hoGEntryAllowed && string.Equals(USceneManager.GetActiveScene().name, WingedNoskScene, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsWingedNoskObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldApplySummonSettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsWingedNoskSummonObject(gameObject))
        {
            return false;
        }

        if (!hoGEntryAllowed)
        {
            return false;
        }

        string activeScene = USceneManager.GetActiveScene().name;
        return string.Equals(activeScene, WingedNoskScene, StringComparison.Ordinal)
            || string.Equals(gameObject.scene.name, WingedNoskScene, StringComparison.Ordinal);
    }

    private static bool ShouldUseCustomHp() => wingedNoskUseMaxHp;
    private static bool ShouldUseCustomPhaseThreshold() => wingedNoskUseCustomPhase && !wingedNoskP5Hp;
    private static bool ShouldUseCustomSummonHp() => wingedNoskUseCustomSummonHp && !wingedNoskP5Hp;
    private static bool ShouldUseCustomSummonLimit() => wingedNoskUseCustomSummonLimit && !wingedNoskP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, WingedNoskScene, StringComparison.Ordinal))
        {
            if (BossManipulateEntryGuard.IsAllowedBossEntry(currentScene, nextScene))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, WingedNoskScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsWingedNoskSummonControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsWingedNoskObject(fsm.gameObject))
        {
            return false;
        }

        return FindSummonEnemyCountCompareAction(fsm) != null;
    }

    private static bool IsEnemyCountCompare(IntCompare compare)
    {
        return (compare.integer1 != null
            && string.Equals(compare.integer1.Name, WingedNoskEnemyCountVariableName, StringComparison.Ordinal))
            || (compare.integer2 != null
                && string.Equals(compare.integer2.Name, WingedNoskEnemyCountVariableName, StringComparison.Ordinal));
    }

    private static bool IsWingedNoskPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsWingedNoskObject(fsm.gameObject))
        {
            return false;
        }

        return fsm.Fsm?.GetState(WingedNoskPhaseCheckStateName) != null
            && fsm.FsmVariables.GetFsmInt(WingedNoskHalfHpVariableName) != null;
    }

    private static bool IsHpCompare(IntCompare compare)
    {
        return (compare.integer1 != null
            && string.Equals(compare.integer1.Name, WingedNoskHpVariableName, StringComparison.Ordinal))
            || (compare.integer2 != null
                && string.Equals(compare.integer2.Name, WingedNoskHpVariableName, StringComparison.Ordinal));
    }
}
