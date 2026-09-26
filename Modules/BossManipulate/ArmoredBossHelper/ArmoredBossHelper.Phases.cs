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
    private protected const string MainPhase2StateName = "To Phase 2";
    private protected const string MainPhase3StateName = "To Phase 3";

    private int ResolvePhase(HealthManager armorHm)
    {
        PlayMakerFSM? mainFsm = FindMainFsm(armorHm.gameObject);
        mainFsm ??= FindMainFsmInScene(armorHm.gameObject.scene.name);
        if (mainFsm == null)
        {
            return 1;
        }

        return ResolvePhaseFromMainFsm(mainFsm);
    }

    private int ResolvePhaseFromMainFsm(PlayMakerFSM mainFsm)
    {
        int phase = 1;
        int rages = mainFsm.FsmVariables.GetFsmInt(RageCountVariableName)?.Value ?? 0;
        if (rages >= 2)
        {
            phase = 3;
        }
        else if (rages >= 1)
        {
            phase = 2;
        }

        int stunnedAmount = mainFsm.FsmVariables.GetFsmInt(StunnedAmountVariableName)?.Value ?? 0;
        if (stunnedAmount >= 2)
        {
            phase = Math.Max(phase, 3);
        }
        else if (stunnedAmount >= 1)
        {
            phase = Math.Max(phase, 2);
        }

        string activeState = mainFsm.ActiveStateName ?? string.Empty;
        if (string.Equals(activeState, MainPhase3StateName, StringComparison.Ordinal)
            || string.Equals(activeState, MainOpened2StateName, StringComparison.Ordinal)
            || string.Equals(activeState, MainHit2StateName, StringComparison.Ordinal))
        {
            phase = 3;
        }
        else if (string.Equals(activeState, MainPhase2StateName, StringComparison.Ordinal))
        {
            phase = Math.Max(phase, 2);
        }

        return Mathf.Clamp(phase, 1, 3);
    }

    private void ApplyPhaseSettings(HealthManager armorHm, bool forceReapply)
    {
        int armorInstanceId = armorHm.GetInstanceID();
        int phase = Math.Max(ResolvePhase(armorHm), confirmedHeadKillCount + 1);
        bool firstApply = !trackedPhaseByArmorInstance.TryGetValue(armorInstanceId, out int trackedPhase);
        bool phaseChanged = firstApply || trackedPhase != phase;
        int appliedHeadKills = appliedHeadKillsByArmorInstance.TryGetValue(armorInstanceId, out int appliedKills)
            ? appliedKills
            : 0;
        bool confirmedHeadPhaseAdvance = confirmedHeadKillCount > appliedHeadKills;
        if (!phaseChanged && !forceReapply)
        {
            return;
        }

        trackedPhaseByArmorInstance[armorInstanceId] = phase;
        if (confirmedHeadPhaseAdvance)
        {
            appliedHeadKillsByArmorInstance[armorInstanceId] = confirmedHeadKillCount;
        }

        int armorHp = GetArmorHpForPhase(phase);
        ApplyArmorHealth(armorHm, armorHp, resetCurrentHp: forceReapply || firstApply || confirmedHeadPhaseAdvance);
        ApplyRecoverHp(armorHm, armorHp);
    }

    private int GetArmorHpForPhase(int phase)
    {
        return phase switch
        {
            2 => ClampHp(ArmorPhase2Hp),
            3 => ClampHp(ArmorPhase3Hp),
            _ => ClampHp(ArmorPhase1Hp),
        };
    }
}
