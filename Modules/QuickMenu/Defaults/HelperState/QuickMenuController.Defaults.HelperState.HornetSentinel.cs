using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private static void ResetHornetSentinelHelperDefaults()
        {
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseMaxHp = false;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelP5Hp = false;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelMaxHp = 1200;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseCustomPhase = false;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelPhase2Hp = 480;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelMaxHpBeforeP5 = 1200;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelPhase2HpBeforeP5 = 480;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelHasStoredStateBeforeP5 = false;
        }

        private bool GetHornetSentinelHelperEnabled()
        {
            return GetHornetSentinelHelperModule()?.Enabled ?? false;
        }

        private void SetHornetSentinelHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetHornetSentinelHelperModule(), value);
            UpdateHornetSentinelHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetHornetSentinelUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.HornetSentinelHelper.hornetSentinelP5Hp)
            {
                Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseMaxHp = true;
                RefreshHornetSentinelHelperUi();
                return;
            }

            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseMaxHp = value;
            Modules.BossChallenge.HornetSentinelHelper.ReapplyLiveSettings();
            RefreshHornetSentinelHelperUi();
        }

        private void SetHornetSentinelUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.HornetSentinelHelper.hornetSentinelP5Hp)
            {
                Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseCustomPhase = false;
                RefreshHornetSentinelHelperUi();
                return;
            }

            ClampHornetSentinelPhaseThresholdsForUi();
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseCustomPhase = value;
            Modules.BossChallenge.HornetSentinelHelper.ReapplyLiveSettings();
            RefreshHornetSentinelHelperUi();
        }

        private void SetHornetSentinelP5HpEnabled(bool value)
        {
            Modules.BossChallenge.HornetSentinelHelper.SetP5HpEnabled(value);
            RefreshHornetSentinelHelperUi();
        }

        private void SetHornetSentinelMaxHp(int value)
        {
            if (Modules.BossChallenge.HornetSentinelHelper.hornetSentinelP5Hp)
            {
                Modules.BossChallenge.HornetSentinelHelper.hornetSentinelMaxHp = 850;
                RefreshHornetSentinelHelperUi();
                return;
            }

            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelMaxHp = Mathf.Clamp(value, 1, 999999);
            ClampHornetSentinelPhaseThresholdsForUi();
            if (Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseMaxHp
                || Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseCustomPhase)
            {
                Modules.BossChallenge.HornetSentinelHelper.ReapplyLiveSettings();
            }

            RefreshHornetSentinelHelperUi();
        }

        private void SetHornetSentinelPhase2Hp(int value)
        {
            if (Modules.BossChallenge.HornetSentinelHelper.hornetSentinelP5Hp)
            {
                RefreshHornetSentinelHelperUi();
                return;
            }

            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelPhase2Hp = value;
            ClampHornetSentinelPhaseThresholdsForUi();
            if (Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseCustomPhase)
            {
                Modules.BossChallenge.HornetSentinelHelper.ReapplyLiveSettings();
            }

            RefreshHornetSentinelHelperUi();
        }

        private static void ClampHornetSentinelPhaseThresholdsForUi()
        {
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelPhase2Hp = Mathf.Clamp(
                Modules.BossChallenge.HornetSentinelHelper.hornetSentinelPhase2Hp,
                1,
                Modules.BossChallenge.HornetSentinelHelper.GetPhase2MaxHpForUi()
            );
        }
    }
}
