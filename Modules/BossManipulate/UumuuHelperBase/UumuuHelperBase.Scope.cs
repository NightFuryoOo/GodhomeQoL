using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class UumuuHelperBase : Module
{
    private protected bool IsBoss(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsBossObject(hm.gameObject);
    }

    private protected bool IsBossSummon(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsBossSummonObject(hm.gameObject);
    }

    private protected bool IsBossObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, SceneName, StringComparison.Ordinal)
            && gameObject.name.StartsWith(BossObjectName, StringComparison.Ordinal);
    }

    private protected abstract bool IsBossSummonObject(GameObject gameObject);

    private protected bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsBossObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private protected bool ShouldApplySummonSettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsBossSummonObject(gameObject))
        {
            return false;
        }

        if (!hoGEntryAllowed)
        {
            return false;
        }

        string activeScene = USceneManager.GetActiveScene().name;
        return string.Equals(activeScene, SceneName, StringComparison.Ordinal)
            || string.Equals(gameObject.scene.name, SceneName, StringComparison.Ordinal);
    }

    private protected bool ShouldUseCustomHp() => UseMaxHp;
    private protected abstract bool ShouldUseCustomSummonHp();

    private protected void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, SceneName, StringComparison.Ordinal))
        {
            if (BossManipulateEntryGuard.IsAllowedBossEntry(currentScene, nextScene))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, SceneName, StringComparison.Ordinal) && hoGEntryAllowed)
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
}
