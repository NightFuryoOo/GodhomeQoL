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
        private static void ResetHornetProtectorHelperDefaults()
        {
            Modules.BossChallenge.HornetProtectorHelper.hornetUseMaxHp = false;
            Modules.BossChallenge.HornetProtectorHelper.hornetP5Hp = false;
            Modules.BossChallenge.HornetProtectorHelper.hornetMaxHp = 1250;
            Modules.BossChallenge.HornetProtectorHelper.hornetMaxHpBeforeP5 = 1250;
            Modules.BossChallenge.HornetProtectorHelper.hornetUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.HornetProtectorHelper.hornetHasStoredStateBeforeP5 = false;
        }

        private bool GetHornetProtectorHelperEnabled()
        {
            return GetHornetProtectorHelperModule()?.Enabled ?? false;
        }

        private void SetHornetProtectorHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetHornetProtectorHelperModule(), value);
            UpdateHornetHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetHornetUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.HornetProtectorHelper.hornetP5Hp)
            {
                Modules.BossChallenge.HornetProtectorHelper.hornetUseMaxHp = true;
                RefreshHornetHelperUi();
                return;
            }

            Modules.BossChallenge.HornetProtectorHelper.hornetUseMaxHp = value;
            Modules.BossChallenge.HornetProtectorHelper.ReapplyLiveSettings();
            RefreshHornetHelperUi();
        }

        private void SetHornetP5HpEnabled(bool value)
        {
            Modules.BossChallenge.HornetProtectorHelper.SetP5HpEnabled(value);
            RefreshHornetHelperUi();
        }

        private void SetHornetMaxHp(int value)
        {
            if (Modules.BossChallenge.HornetProtectorHelper.hornetP5Hp)
            {
                Modules.BossChallenge.HornetProtectorHelper.hornetMaxHp = 900;
                RefreshHornetHelperUi();
                return;
            }

            Modules.BossChallenge.HornetProtectorHelper.hornetMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.HornetProtectorHelper.hornetUseMaxHp)
            {
                Modules.BossChallenge.HornetProtectorHelper.ApplyHornetHealthIfPresent();
            }
        }
    }
}
