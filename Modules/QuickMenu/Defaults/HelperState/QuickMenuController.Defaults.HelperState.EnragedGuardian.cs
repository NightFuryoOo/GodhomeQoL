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
        private static void ResetEnragedGuardianHelperDefaults()
        {
            Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianUseMaxHp = false;
            Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianP5Hp = false;
            Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianMaxHp = 1250;
            Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianMaxHpBeforeP5 = 1250;
            Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianHasStoredStateBeforeP5 = false;
        }

        private bool GetEnragedGuardianHelperEnabled()
        {
            return GetEnragedGuardianHelperModule()?.Enabled ?? false;
        }

        private void SetEnragedGuardianHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetEnragedGuardianHelperModule(), value);
            UpdateEnragedGuardianHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetEnragedGuardianUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianP5Hp)
            {
                Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianUseMaxHp = true;
                RefreshEnragedGuardianHelperUi();
                return;
            }

            Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianUseMaxHp = value;
            Modules.BossChallenge.EnragedGuardianHelper.ReapplyLiveSettings();
            RefreshEnragedGuardianHelperUi();
        }

        private void SetEnragedGuardianP5HpEnabled(bool value)
        {
            Modules.BossChallenge.EnragedGuardianHelper.SetP5HpEnabled(value);
            RefreshEnragedGuardianHelperUi();
        }

        private void SetEnragedGuardianMaxHp(int value)
        {
            if (Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianP5Hp)
            {
                Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianMaxHp = 650;
                RefreshEnragedGuardianHelperUi();
                return;
            }

            Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianUseMaxHp)
            {
                Modules.BossChallenge.EnragedGuardianHelper.ApplyEnragedGuardianHealthIfPresent();
            }
        }
    }
}
