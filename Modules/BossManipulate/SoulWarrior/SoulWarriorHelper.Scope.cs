using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class SoulWarriorHelper : Module
{
    private static bool IsSoulWarrior(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsSoulWarriorObject(hm.gameObject);
    }

    private static bool IsSoulWarriorSummon(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsSoulWarriorSummonObject(hm.gameObject);
    }

    private static bool IsSoulWarriorObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, SoulWarriorScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(SoulWarriorName, StringComparison.Ordinal);
    }

    private static bool IsSoulWarriorSummonObject(GameObject gameObject)
    {
        if (gameObject == null || IsSoulWarriorObject(gameObject))
        {
            return false;
        }

        string name = gameObject.name;
        bool matchesSummonName = false;
        foreach (string hint in SoulWarriorSummonNameHints)
        {
            if (name.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                matchesSummonName = true;
                break;
            }
        }

        if (!matchesSummonName)
        {
            return false;
        }

        if (string.Equals(gameObject.scene.name, SoulWarriorScene, StringComparison.Ordinal))
        {
            return true;
        }

        return hoGEntryAllowed && string.Equals(USceneManager.GetActiveScene().name, SoulWarriorScene, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsSoulWarriorObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldApplySummonLimitSettings(PlayMakerFSM? fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsSoulWarriorSummonControlFsm(fsm))
        {
            return false;
        }

        if (!hoGEntryAllowed)
        {
            return false;
        }

        string activeScene = USceneManager.GetActiveScene().name;
        if (!string.Equals(activeScene, SoulWarriorScene, StringComparison.Ordinal))
        {
            return false;
        }

        return string.Equals(fsm.gameObject.scene.name, SoulWarriorScene, StringComparison.Ordinal)
            || IsSoulWarriorObject(fsm.gameObject);
    }

    private static bool ShouldApplySummonSettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsSoulWarriorSummonObject(gameObject))
        {
            return false;
        }

        if (!hoGEntryAllowed)
        {
            return false;
        }

        string activeScene = USceneManager.GetActiveScene().name;
        return string.Equals(activeScene, SoulWarriorScene, StringComparison.Ordinal)
            || string.Equals(gameObject.scene.name, SoulWarriorScene, StringComparison.Ordinal);
    }

    private static bool IsSoulWarriorArenaContext(GameObject gameObject)
    {
        if (gameObject == null || !hoGEntryAllowed)
        {
            return false;
        }

        if (string.Equals(USceneManager.GetActiveScene().name, SoulWarriorScene, StringComparison.Ordinal))
        {
            return true;
        }

        return string.Equals(gameObject.scene.name, SoulWarriorScene, StringComparison.Ordinal);
    }

    private static bool ShouldUseCustomHp() => soulWarriorUseMaxHp;
    private static bool ShouldUseCustomSummonHp() => soulWarriorUseCustomSummonHp && !soulWarriorP5Hp;
    private static bool ShouldUseCustomSummonLimit() => soulWarriorUseCustomSummonLimit && !soulWarriorP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, SoulWarriorScene, StringComparison.Ordinal))
        {
            if (BossManipulateEntryGuard.IsAllowedBossEntry(currentScene, nextScene))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, SoulWarriorScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsSoulWarriorSummonControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsSoulWarriorArenaContext(fsm.gameObject))
        {
            return false;
        }

        if (FindSummonCountVariables(fsm).Count > 0)
        {
            return true;
        }

        if (HasLikelySummonState(fsm))
        {
            return true;
        }

        return FindSummonPoolVariables(fsm).Count > 0;
    }

    private static bool IsLikelySummonStateName(string? stateName)
    {
        if (string.IsNullOrWhiteSpace(stateName))
        {
            return false;
        }

        foreach (string hint in SoulWarriorSummonStateHints)
        {
            if (string.Equals(stateName, hint, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        string normalizedStateName = stateName!;
        return normalizedStateName.IndexOf("summon", StringComparison.OrdinalIgnoreCase) >= 0
            || normalizedStateName.IndexOf("balloon", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static bool IsLikelySummonCountVariable(FsmInt? operand)
    {
        if (operand == null)
        {
            return false;
        }

        return IsKnownSummonCountVariableName(operand.Name);
    }

    private static bool IsKnownSummonCountVariableName(string? variableName)
    {
        if (string.IsNullOrWhiteSpace(variableName))
        {
            return false;
        }

        foreach (string knownName in SoulWarriorSummonCountVariableNames)
        {
            if (string.Equals(variableName, knownName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        string normalizedVariableName = variableName!;
        bool hasSummonToken = normalizedVariableName.IndexOf("ball", StringComparison.OrdinalIgnoreCase) >= 0
            || normalizedVariableName.IndexOf("summon", StringComparison.OrdinalIgnoreCase) >= 0;
        if (!hasSummonToken)
        {
            return false;
        }

        return normalizedVariableName.IndexOf("hp", StringComparison.OrdinalIgnoreCase) < 0;
    }
}
