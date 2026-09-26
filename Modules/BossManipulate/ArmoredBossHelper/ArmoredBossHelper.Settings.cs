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
    internal void ReapplyLiveSettingsCore()
    {
        if (!moduleActive)
        {
            return;
        }

        NormalizeConfiguredHpState();
        ApplySettingsIfPresentCore(forceReapply: true);
    }

    internal void SetP5HpEnabledCore(bool value)
    {
        if (value)
        {
            if (!P5HpEnabled)
            {
                ArmorPhase1HpBeforeP5 = ClampHp(ArmorPhase1Hp);
                ArmorPhase2HpBeforeP5 = ClampHp(ArmorPhase2Hp);
                ArmorPhase3HpBeforeP5 = ClampHp(ArmorPhase3Hp);
                HasStoredStateBeforeP5 = true;
            }

            P5HpEnabled = true;
            ArmorPhase1Hp = P5ArmorPhaseHp;
            ArmorPhase2Hp = P5ArmorPhaseHp;
            ArmorPhase3Hp = P5ArmorPhaseHp;
        }
        else
        {
            if (P5HpEnabled && HasStoredStateBeforeP5)
            {
                ArmorPhase1Hp = ClampHp(ArmorPhase1HpBeforeP5);
                ArmorPhase2Hp = ClampHp(ArmorPhase2HpBeforeP5);
                ArmorPhase3Hp = ClampHp(ArmorPhase3HpBeforeP5);
            }

            P5HpEnabled = false;
            HasStoredStateBeforeP5 = false;
        }

        NormalizeConfiguredHpState();
        ApplySettingsIfPresentCore(forceReapply: true);
    }

    internal void ApplySettingsIfPresentCore(bool forceReapply = false)
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsArmor(hm) || !ShouldApplySettings(hm.gameObject))
            {
                continue;
            }

            ApplyPhaseSettings(hm, forceReapply);
        }
    }

    private void NormalizeP5State()
    {
        NormalizeConfiguredHpState();

        if (!P5HpEnabled)
        {
            return;
        }

        if (!HasStoredStateBeforeP5)
        {
            ArmorPhase1HpBeforeP5 = ClampHp(ArmorPhase1Hp);
            ArmorPhase2HpBeforeP5 = ClampHp(ArmorPhase2Hp);
            ArmorPhase3HpBeforeP5 = ClampHp(ArmorPhase3Hp);
            HasStoredStateBeforeP5 = true;
        }

        ArmorPhase1Hp = P5ArmorPhaseHp;
        ArmorPhase2Hp = P5ArmorPhaseHp;
        ArmorPhase3Hp = P5ArmorPhaseHp;
    }

    private void NormalizeConfiguredHpState()
    {
        ArmorPhase1Hp = ClampHp(ArmorPhase1Hp);
        ArmorPhase2Hp = ClampHp(ArmorPhase2Hp);
        ArmorPhase3Hp = ClampHp(ArmorPhase3Hp);
        ArmorPhase1HpBeforeP5 = ClampHp(ArmorPhase1HpBeforeP5);
        ArmorPhase2HpBeforeP5 = ClampHp(ArmorPhase2HpBeforeP5);
        ArmorPhase3HpBeforeP5 = ClampHp(ArmorPhase3HpBeforeP5);
    }
}
