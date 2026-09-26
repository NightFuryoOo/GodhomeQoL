using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using Modding;
using Satchel;
using Satchel.Futils;
using UnityEngine;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class ArmoredBossHelper : Module
{
    private bool IsBossScene(string? sceneName)
    {
        return string.Equals(sceneName, SceneName, StringComparison.Ordinal);
    }

    private bool IsArmor(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        GameObject gameObject = hm.gameObject;
        return IsBossScene(gameObject.scene.name)
            && gameObject.name.StartsWith(ArmorNamePrefix, StringComparison.Ordinal);
    }

    private bool IsHead(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        GameObject gameObject = hm.gameObject;
        if (!IsBossScene(gameObject.scene.name))
        {
            return false;
        }

        string objectName = gameObject.name;
        return objectName.IndexOf("Head", StringComparison.OrdinalIgnoreCase) >= 0
            || objectName.IndexOf("Hornhead", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsBossScene(gameObject.scene.name))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private PlayMakerFSM? FindMainFsm(GameObject armorObject)
    {
        if (armorObject == null)
        {
            return null;
        }

        foreach (PlayMakerFSM fsm in armorObject.GetComponents<PlayMakerFSM>())
        {
            if (fsm != null && IsMainFsm(fsm))
            {
                return fsm;
            }
        }

        return null;
    }

    private PlayMakerFSM? FindMainFsmInScene(string sceneName)
    {
        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !string.Equals(fsm.gameObject.scene.name, sceneName, StringComparison.Ordinal))
            {
                continue;
            }

            if (IsMainFsm(fsm))
            {
                return fsm;
            }
        }

        return null;
    }

    private bool IsMainFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsBossScene(fsm.gameObject.scene.name))
        {
            return false;
        }

        return fsm.Fsm?.GetState(MainPhase2StateName) != null
            && fsm.Fsm?.GetState(MainPhase3StateName) != null;
    }

    private bool IsRecoverFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsBossScene(fsm.gameObject.scene.name))
        {
            return false;
        }

        return fsm.Fsm?.GetState("Check") != null
            && fsm.Fsm?.GetState("Stun") != null
            && fsm.FsmVariables.GetFsmInt(RecoverVariableName) != null;
    }
}
