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
        private static void ResetBroodingMawlekHelperDefaults()
        {
            Modules.BossChallenge.BroodingMawlekHelper.mawlekUseMaxHp = false;
            Modules.BossChallenge.BroodingMawlekHelper.mawlekP5Hp = false;
            Modules.BossChallenge.BroodingMawlekHelper.mawlekMaxHp = 1050;
            Modules.BossChallenge.BroodingMawlekHelper.mawlekMaxHpBeforeP5 = 1050;
            Modules.BossChallenge.BroodingMawlekHelper.mawlekUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.BroodingMawlekHelper.mawlekHasStoredStateBeforeP5 = false;
        }

        private bool GetBroodingMawlekHelperEnabled()
        {
            return GetBroodingMawlekHelperModule()?.Enabled ?? false;
        }

        private void SetBroodingMawlekHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetBroodingMawlekHelperModule(), value);
            UpdateMawlekHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetMawlekUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.BroodingMawlekHelper.mawlekP5Hp)
            {
                Modules.BossChallenge.BroodingMawlekHelper.mawlekUseMaxHp = true;
                RefreshMawlekHelperUi();
                return;
            }

            Modules.BossChallenge.BroodingMawlekHelper.mawlekUseMaxHp = value;
            Modules.BossChallenge.BroodingMawlekHelper.ReapplyLiveSettings();
            RefreshMawlekHelperUi();
        }

        private void SetMawlekP5HpEnabled(bool value)
        {
            Modules.BossChallenge.BroodingMawlekHelper.SetP5HpEnabled(value);
            RefreshMawlekHelperUi();
        }

        private void SetMawlekMaxHp(int value)
        {
            if (Modules.BossChallenge.BroodingMawlekHelper.mawlekP5Hp)
            {
                Modules.BossChallenge.BroodingMawlekHelper.mawlekMaxHp = 750;
                RefreshMawlekHelperUi();
                return;
            }

            Modules.BossChallenge.BroodingMawlekHelper.mawlekMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.BroodingMawlekHelper.mawlekUseMaxHp)
            {
                Modules.BossChallenge.BroodingMawlekHelper.ApplyMawlekHealthIfPresent();
            }
        }
    }
}
