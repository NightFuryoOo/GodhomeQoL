using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class RageBossHelper : Module
{
    private protected bool IsBoss(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsBossObject(hm.gameObject);
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

    private protected bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsBossObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private protected bool ShouldUseCustomHp() => UseMaxHp;
    private protected virtual bool ShouldUseCustomPhaseThreshold() => UseCustomPhase && !P5HpEnabled;

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

    private protected virtual bool IsBossPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsBossObject(fsm.gameObject))
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, PhaseFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState(PhaseCheckStateName) != null
            && fsm.FsmVariables.GetFsmInt(Phase2VariableName) != null;
    }

    private protected virtual bool ShouldApplyPhaseSettings(GameObject? gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return IsBossObject(gameObject) && hoGEntryAllowed;
    }
}
