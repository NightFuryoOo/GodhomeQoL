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
        private static void ResetCrystalGuardianHelperDefaults()
        {
            Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianUseMaxHp = false;
            Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianP5Hp = false;
            Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianMaxHp = 900;
            Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianMaxHpBeforeP5 = 900;
            Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianHasStoredStateBeforeP5 = false;
        }

        private bool GetCrystalGuardianHelperEnabled()
        {
            return GetCrystalGuardianHelperModule()?.Enabled ?? false;
        }

        private void SetCrystalGuardianHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetCrystalGuardianHelperModule(), value);
            UpdateCrystalGuardianHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetCrystalGuardianUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianP5Hp)
            {
                Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianUseMaxHp = true;
                RefreshCrystalGuardianHelperUi();
                return;
            }

            Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianUseMaxHp = value;
            Modules.BossChallenge.CrystalGuardianHelper.ReapplyLiveSettings();
            RefreshCrystalGuardianHelperUi();
        }

        private void SetCrystalGuardianP5HpEnabled(bool value)
        {
            Modules.BossChallenge.CrystalGuardianHelper.SetP5HpEnabled(value);
            RefreshCrystalGuardianHelperUi();
        }

        private void SetCrystalGuardianMaxHp(int value)
        {
            if (Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianP5Hp)
            {
                Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianMaxHp = 650;
                RefreshCrystalGuardianHelperUi();
                return;
            }

            Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianUseMaxHp)
            {
                Modules.BossChallenge.CrystalGuardianHelper.ApplyCrystalGuardianHealthIfPresent();
            }
        }
    }
}
