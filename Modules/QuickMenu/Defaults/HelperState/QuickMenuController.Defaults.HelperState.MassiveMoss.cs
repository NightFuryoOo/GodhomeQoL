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
        private static void ResetMassiveMossChargerHelperDefaults()
        {
            Modules.BossChallenge.MassiveMossChargerHelper.massiveMossUseMaxHp = false;
            Modules.BossChallenge.MassiveMossChargerHelper.massiveMossP5Hp = false;
            Modules.BossChallenge.MassiveMossChargerHelper.massiveMossMaxHp = 850;
            Modules.BossChallenge.MassiveMossChargerHelper.massiveMossMaxHpBeforeP5 = 850;
            Modules.BossChallenge.MassiveMossChargerHelper.massiveMossUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.MassiveMossChargerHelper.massiveMossHasStoredStateBeforeP5 = false;
        }

        private bool GetMassiveMossChargerHelperEnabled()
        {
            return GetMassiveMossChargerHelperModule()?.Enabled ?? false;
        }

        private void SetMassiveMossChargerHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetMassiveMossChargerHelperModule(), value);
            UpdateMassiveMossHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetMassiveMossUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.MassiveMossChargerHelper.massiveMossP5Hp)
            {
                Modules.BossChallenge.MassiveMossChargerHelper.massiveMossUseMaxHp = true;
                RefreshMassiveMossHelperUi();
                return;
            }

            Modules.BossChallenge.MassiveMossChargerHelper.massiveMossUseMaxHp = value;
            Modules.BossChallenge.MassiveMossChargerHelper.ReapplyLiveSettings();
            RefreshMassiveMossHelperUi();
        }

        private void SetMassiveMossP5HpEnabled(bool value)
        {
            Modules.BossChallenge.MassiveMossChargerHelper.SetP5HpEnabled(value);
            RefreshMassiveMossHelperUi();
        }

        private void SetMassiveMossMaxHp(int value)
        {
            if (Modules.BossChallenge.MassiveMossChargerHelper.massiveMossP5Hp)
            {
                Modules.BossChallenge.MassiveMossChargerHelper.massiveMossMaxHp = 480;
                RefreshMassiveMossHelperUi();
                return;
            }

            Modules.BossChallenge.MassiveMossChargerHelper.massiveMossMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.MassiveMossChargerHelper.massiveMossUseMaxHp)
            {
                Modules.BossChallenge.MassiveMossChargerHelper.ApplyMassiveMossHealthIfPresent();
            }
        }
    }
}
