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
        private static void ResetZoteHelperDefaults()
        {
            Modules.BossChallenge.ZoteHelper.zoteBossHp = 1400;
            Modules.BossChallenge.ZoteHelper.zoteUseCustomBossHp = true;
            Modules.BossChallenge.ZoteHelper.zoteImmortal = false;
            Modules.BossChallenge.ZoteHelper.zoteSpawnFlying = true;
            Modules.BossChallenge.ZoteHelper.zoteSpawnHopping = true;
            Modules.BossChallenge.ZoteHelper.zoteSummonFlyingHp = 20;
            Modules.BossChallenge.ZoteHelper.zoteUseCustomFlyingHp = true;
            Modules.BossChallenge.ZoteHelper.zoteSummonHoppingHp = 20;
            Modules.BossChallenge.ZoteHelper.zoteUseCustomHoppingHp = true;
            Modules.BossChallenge.ZoteHelper.zoteSummonLimit = 3;
            Modules.BossChallenge.ZoteHelper.zoteUseCustomSummonLimit = false;
            Modules.BossChallenge.ZoteHelper.zoteDoubleSummons = false;
            Modules.BossChallenge.ZoteHelper.zoteAttackSummonsOnlyOnStart = false;
        }

        private bool GetZoteHelperEnabled()
        {
            return GetZoteHelperModule()?.Enabled ?? false;
        }

        private void SetZoteHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetZoteHelperModule(), value);
            UpdateZoteHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetZoteBossHp(int value)
        {
            Modules.BossChallenge.ZoteHelper.zoteBossHp = Mathf.Clamp(value, 100, 999999);
            if (Modules.BossChallenge.ZoteHelper.zoteUseCustomBossHp)
            {
                Modules.BossChallenge.ZoteHelper.ApplyBossHealthIfPresent();
            }
        }

        private void SetZoteUseCustomBossHpEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteUseCustomBossHp = value;
            Modules.BossChallenge.ZoteHelper.ReapplyLiveSettings();
            RefreshZoteHelperUi();
        }

        private void SetZoteImmortalEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteImmortal = value;
            Modules.BossChallenge.ZoteHelper.ApplyBossHealthIfPresent();
        }

        private void SetZoteSpawnFlyingEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteSpawnFlying = value;
        }

        private void SetZoteSpawnHoppingEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteSpawnHopping = value;
        }

        private void SetZoteFlyingHp(int value)
        {
            Modules.BossChallenge.ZoteHelper.zoteSummonFlyingHp = Mathf.Clamp(value, 0, 99);
            if (Modules.BossChallenge.ZoteHelper.zoteUseCustomFlyingHp)
            {
                Modules.BossChallenge.ZoteHelper.ApplyZotelingHealthIfPresent();
            }
        }

        private void SetZoteUseCustomFlyingHpEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteUseCustomFlyingHp = value;
            Modules.BossChallenge.ZoteHelper.ApplyZotelingHealthIfPresent();
            RefreshZoteHelperUi();
        }

        private void SetZoteHoppingHp(int value)
        {
            Modules.BossChallenge.ZoteHelper.zoteSummonHoppingHp = Mathf.Clamp(value, 0, 99);
            if (Modules.BossChallenge.ZoteHelper.zoteUseCustomHoppingHp)
            {
                Modules.BossChallenge.ZoteHelper.ApplyZotelingHealthIfPresent();
            }
        }

        private void SetZoteUseCustomHoppingHpEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteUseCustomHoppingHp = value;
            Modules.BossChallenge.ZoteHelper.ApplyZotelingHealthIfPresent();
            RefreshZoteHelperUi();
        }

        private void SetZoteSummonLimit(int value)
        {
            Modules.BossChallenge.ZoteHelper.zoteSummonLimit = Mathf.Clamp(value, 0, 99);
            if (Modules.BossChallenge.ZoteHelper.zoteUseCustomSummonLimit)
            {
                Modules.BossChallenge.ZoteHelper.ReapplyLiveSettings();
            }
        }

        private void SetZoteUseCustomSummonLimitEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteUseCustomSummonLimit = value;
            Modules.BossChallenge.ZoteHelper.ReapplyLiveSettings();
            RefreshZoteHelperUi();
        }

        private void SetZoteDoubleSummonsEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteDoubleSummons = value;
            Modules.BossChallenge.ZoteHelper.ReapplyLiveSettings();
            RefreshZoteHelperUi();
        }

        private void SetZoteAttackSummonsOnlyOnStartEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteAttackSummonsOnlyOnStart = value;
            Modules.BossChallenge.ZoteHelper.ReapplyLiveSettings();
            RefreshZoteHelperUi();
        }
    }
}
