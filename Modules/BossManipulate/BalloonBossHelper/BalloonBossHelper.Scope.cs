using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class BalloonBossHelper : Module
{
    private bool IsBoss(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsBossObject(hm.gameObject);
    }

    private bool IsBossObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, SceneName, StringComparison.Ordinal)
            && gameObject.name.StartsWith(BossObjectName, StringComparison.Ordinal);
    }

    private bool IsBossSummon(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsBossSummonObject(hm.gameObject);
    }

    private bool IsBossSummonObject(GameObject gameObject)
    {
        if (gameObject == null || IsBossObject(gameObject))
        {
            return false;
        }

        bool isBrokenSceneObject = string.Equals(gameObject.scene.name, SceneName, StringComparison.Ordinal);
        bool isBrokenSceneActive = string.Equals(USceneManager.GetActiveScene().name, SceneName, StringComparison.Ordinal);
        if (!isBrokenSceneObject && !isBrokenSceneActive)
        {
            return false;
        }

        string name = gameObject.name ?? string.Empty;
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        foreach (string hint in BossSummonNameHints)
        {
            if (name.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return gameObject.GetComponent<HealthManager>() != null;
            }
        }

        return false;
    }

    private bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsBossObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private bool ShouldApplySummonSettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsBossSummonObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private bool ShouldUseCustomHp() => UseMaxHp;
    private bool ShouldUseCustomSummonHp() => UseCustomSummonHp && !P5HpEnabled;
    private bool ShouldUseCustomSummonLimit() => UseCustomSummonLimit && !P5HpEnabled;
    private bool ShouldUseCustomPhaseThresholds() => UseCustomPhase && !P5HpEnabled;

    private void UpdateHoGEntryAllowed(string currentScene, string nextScene)
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

    private bool IsBossPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsBossObject(fsm.gameObject))
        {
            return false;
        }

        return IsBossShakeTokenFsm(fsm) || IsBossSpawnBalloonFsm(fsm);
    }

    private bool IsBossShakeTokenFsm(PlayMakerFSM fsm)
    {
        if (fsm == null)
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, BossShakeTokenControlFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState(BossCheckState1Name) != null
            && fsm.Fsm?.GetState(BossCheckState2Name) != null
            && fsm.Fsm?.GetState(BossCheckState3Name) != null
            && fsm.FsmVariables.GetFsmInt(BossToken1VariableName) != null
            && fsm.FsmVariables.GetFsmInt(BossToken2VariableName) != null
            && fsm.FsmVariables.GetFsmInt(BossToken3VariableName) != null;
    }

    private bool IsBossSpawnBalloonFsm(PlayMakerFSM fsm)
    {
        if (fsm == null)
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, BossSpawnBalloonFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState(BossSpawnStateName) != null
            && FindSpawnHpCompareAction(fsm) != null;
    }

    private bool IsHpCompare(IntCompare compare)
    {
        return (compare.integer1 != null
            && string.Equals(compare.integer1.Name, BossHpVariableName, StringComparison.Ordinal))
            || (compare.integer2 != null
                && string.Equals(compare.integer2.Name, BossHpVariableName, StringComparison.Ordinal));
    }

    private bool IsEnemyCountCompare(IntCompare compare)
    {
        return (compare.integer1 != null
            && string.Equals(compare.integer1.Name, "Enemy Count", StringComparison.Ordinal))
            || (compare.integer2 != null
                && string.Equals(compare.integer2.Name, "Enemy Count", StringComparison.Ordinal));
    }
}
