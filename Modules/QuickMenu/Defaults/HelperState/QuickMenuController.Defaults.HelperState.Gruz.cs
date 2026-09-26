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
        private static void ResetGruzMotherHelperDefaults()
        {
            Modules.BossChallenge.GruzMotherHelper.gruzUseMaxHp = false;
            Modules.BossChallenge.GruzMotherHelper.gruzP5Hp = false;
            Modules.BossChallenge.GruzMotherHelper.gruzMaxHp = 945;
            Modules.BossChallenge.GruzMotherHelper.gruzMaxHpBeforeP5 = 945;
            Modules.BossChallenge.GruzMotherHelper.gruzHasStoredMaxHpBeforeP5 = false;
        }

        private bool GetGruzMotherHelperEnabled()
        {
            return GetGruzMotherHelperModule()?.Enabled ?? false;
        }

        private void SetGruzMotherHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetGruzMotherHelperModule(), value);
            UpdateGruzHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetGruzUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.GruzMotherHelper.gruzP5Hp)
            {
                Modules.BossChallenge.GruzMotherHelper.gruzUseMaxHp = true;
                RefreshGruzHelperUi();
                return;
            }

            Modules.BossChallenge.GruzMotherHelper.gruzUseMaxHp = value;
            Modules.BossChallenge.GruzMotherHelper.ReapplyLiveSettings();
            RefreshGruzHelperUi();
        }

        private void SetGruzP5HpEnabled(bool value)
        {
            Modules.BossChallenge.GruzMotherHelper.SetP5HpEnabled(value);
            RefreshGruzHelperUi();
        }

        private void SetGruzMaxHp(int value)
        {
            if (Modules.BossChallenge.GruzMotherHelper.gruzP5Hp)
            {
                Modules.BossChallenge.GruzMotherHelper.gruzMaxHp = 650;
                RefreshGruzHelperUi();
                return;
            }

            Modules.BossChallenge.GruzMotherHelper.gruzMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.GruzMotherHelper.gruzUseMaxHp)
            {
                Modules.BossChallenge.GruzMotherHelper.ApplyGruzHealthIfPresent();
            }
        }
    }
}
