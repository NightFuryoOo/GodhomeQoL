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
    {        private void OnOverlayBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetOverlayVisible(false);
        }

        private void OnCollectorBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetCollectorVisible(false);
        }

        private void OnFastReloadBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetFastReloadVisible(false);
        }

        private void OnFastReloadResetDefaultsClicked()
        {
            SetFastReloadEnabled(false);
            waitingForReloadRebind = false;
            Modules.FastReload.reloadKeyCode = (int)KeyCode.None;
            Modules.FastReload.teleportKeyCode = (int)KeyCode.None;
            RefreshFastReloadUi();
        }

        private void OnDreamshieldBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetDreamshieldVisible(false);
        }

        private void OnDreamshieldResetDefaultsClicked()
        {
            Modules.QoL.DreamshieldStartAngle.startAngleEnabled = false;
            Modules.QoL.DreamshieldStartAngle.rotationDelay = 0f;
            Modules.QoL.DreamshieldStartAngle.rotationSpeed = 1f;
            RefreshDreamshieldUi();
        }

        private void OnShowHpOnDeathBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetShowHpOnDeathVisible(false);
        }

        private void OnMaskDamageBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
        }

        private void OnMaskDamageResetDefaultsClicked()
        {
            waitingForMaskDamageUiRebind = false;
            maskDamageUiPrevKey = string.Empty;

            SetMaskDamageEnabled(false);
            MaskDamage.SetMultiplier(1);
            MaskDamage.SetToggleUiKeybind(string.Empty);
            RefreshMaskDamageUi();
            UpdateQuickMenuEntryStateColors();
        }

        private void OnFreezeHitboxesBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetFreezeHitboxesVisible(false);
        }

        private void OnFreezeHitboxesResetDefaultsClicked()
        {
            waitingForFreezeHitboxesRebind = false;
            freezeHitboxesPrevKey = string.Empty;

            SetFreezeHitboxesEnabled(false);
            FreezeHitboxes.SetAnyHitsMode(false);
            FreezeHitboxes.SetUnfreezeKeybind(string.Empty);
            RefreshFreezeHitboxesUi();
            UpdateQuickMenuEntryStateColors();
        }

        private void OnShowHpOnDeathResetDefaultsClicked()
        {
            waitingForShowHpRebind = false;
            showHpPrevBindingRaw = string.Empty;
            ShowHpSettings.EnabledMod = false;
            ShowHpSettings.ShowPB = true;
            ShowHpSettings.HideAfter10Sec = true;
            ShowHpSettings.HudFadeSeconds = 5f;
            ApplyShowHpBinding(null);
            RefreshShowHpOnDeathUi();
            UpdateQuickMenuEntryStateColors();
        }

        private void OnSpeedChangerBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetSpeedChangerVisible(false);
        }

        private void OnSpeedChangerResetDefaultsClicked()
        {
            waitingForSpeedToggleRebind = false;
            waitingForSpeedInputRebind = false;
            speedTogglePrevKey = string.Empty;
            speedInputPrevKey = string.Empty;

            SetSpeedChangerGlobalSwitch(false);
            SpeedChanger.restrictToggleToRooms = false;
            SpeedChanger.unlimitedSpeed = false;
            SpeedChanger.displayStyle = 0;
            SpeedChanger.toggleKeybind = string.Empty;
            SpeedChanger.inputSpeedKeybind = string.Empty;
            SpeedChanger.slowDownKeybind = string.Empty;
            SpeedChanger.speedUpKeybind = string.Empty;
            SpeedChanger.speed = 1f;
            ApplySpeedChangerDisplayStyle(0);
            RefreshSpeedChangerKeybinds();
            RefreshSpeedChangerUi();
        }

        private void OnTeleportKitBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetTeleportKitVisible(false);
        }

        private void OnTeleportKitResetDefaultsClicked()
        {
            waitingForTeleportKitMenuRebind = false;
            waitingForTeleportKitSaveRebind = false;
            waitingForTeleportKitTeleportRebind = false;
            SetTeleportKitEnabled(false);
            Modules.QoL.TeleportKit.MenuHotkey = KeyCode.F6;
            Modules.QoL.TeleportKit.SaveTeleportHotkey = KeyCode.R;
            Modules.QoL.TeleportKit.TeleportHotkey = KeyCode.T;
            RefreshTeleportKitUi();
        }

        private void OnBossChallengeBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetBossChallengeVisible(false);
        }

        private void OnRandomPantheonsBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetRandomPantheonsVisible(false);
        }

        private void OnBossChallengeResetDefaultsClicked()
        {
            bossChallengeMasterEnabled = false;
            bossChallengeMasterHasSnapshot = false;
            SetInfiniteChallengeEnabled(false);
            SetCarefreeMelodyMode(Modules.QoL.CarefreeMelodyReset.ModeOff);
            SetForceArriveAnimationEnabled(false);
            SetInfiniteGrimmPufferfishEnabled(false);
            SetInfiniteRadianceClimbingEnabled(false);
            SetP5HealthEnabled(false);
            SetSegmentedP5Enabled(false);
            SetHalveAscendedEnabled(false);
            SetHalveAttunedEnabled(false);
            SetAddLifebloodEnabled(false);
            SetAddSoulEnabled(false);

            Modules.BossChallenge.InfiniteChallenge.restartFightOnSuccess = false;
            Modules.BossChallenge.InfiniteChallenge.restartFightAndMusic = false;
            Modules.BossChallenge.AddLifeblood.lifebloodAmount = 0;
            Modules.BossChallenge.AddSoul.soulAmount = 0;
            Modules.BossChallenge.SegmentedP5.selectedP5Segment = 0;
            RefreshBossChallengeUi();
            SaveMasterSettings();
        }

        private void OnAlwaysFuriousBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetAlwaysFuriousVisible(false);
        }

        private void OnAlwaysFuriousResetDefaultsClicked()
        {
            SetAlwaysFuriousEnabled(false);
            RefreshAlwaysFuriousUi();
        }

        private void OnQolBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetQolVisible(false);
        }

        private void OnQolResetDefaultsClicked()
        {
            qolMasterEnabled = true;
            qolMasterHasSnapshot = false;
            waitingForFastDreamWarpRebind = false;
            fastDreamWarpPrevKeyRaw = string.Empty;
            fastDreamWarpPrevButton = default;

            SetFastDreamWarpEnabled(false);
            FastDreamWarpSettings.Keybinds.Toggle.ClearBindings();
            SetShortDeathAnimationEnabled(true);
            Modules.QoL.SkipCutscenes.HallOfGodsStatues = true;
            SetUnlockAllModesEnabled(true);
            SetUnlockPantheonsEnabled(true);
            SetUnlockRadianceEnabled(true);
            SetUnlockRadiantEnabled(true);
            SetInvincibleIndicatorEnabled(true);
            SetScreenShakeEnabled(true);

            RefreshQolUi();
            SaveMasterSettings();
        }

        private void OnMenuAnimationBackClicked()
        {
            bool reopenQol = returnToQolOnClose;
            returnToQolOnClose = false;

            if (reopenQol)
            {
                SetQolVisible(true);
            }
            else
            {
                bool reopenQuick = returnToQuickOnClose;
                returnToQuickOnClose = false;
                if (reopenQuick)
                {
                    SetQuickVisible(true);
                }
            }

            SetMenuAnimationVisible(false);
        }

        private void OnMenuAnimationResetDefaultsClicked()
        {
            menuAnimMasterEnabled = true;
            menuAnimMasterHasSnapshot = false;
            SetDoorDefaultBeginEnabled(true);
            SetMemorizeBindingsEnabled(true);
            SetFasterLoadsEnabled(true);
            SetFastMenusEnabled(true);
            SetFastTextEnabled(true);
            Modules.QoL.SkipCutscenes.AutoSkipCinematics = true;
            Modules.QoL.SkipCutscenes.AllowSkippingNonskippable = true;
            Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt = true;
            RefreshMenuAnimationUi();
            SaveMasterSettings();
        }

        private void OnBossAnimationBackClicked()
        {
            bool reopenQol = returnToQolOnClose;
            returnToQolOnClose = false;

            if (reopenQol)
            {
                SetQolVisible(true);
            }
            else
            {
                bool reopenQuick = returnToQuickOnClose;
                returnToQuickOnClose = false;
                if (reopenQuick)
                {
                    SetQuickVisible(true);
                }
            }

            SetBossAnimationVisible(false);
        }

        private void OnBossAnimationResetDefaultsClicked()
        {
            bossAnimMasterEnabled = true;
            bossAnimMasterHasSnapshot = false;
            Modules.QoL.SkipCutscenes.AbsoluteRadiance = true;
            Modules.QoL.SkipCutscenes.PureVesselRoar = true;
            Modules.QoL.SkipCutscenes.GrimmNightmare = true;
            Modules.QoL.SkipCutscenes.GreyPrinceZote = true;
            Modules.QoL.SkipCutscenes.Collector = true;
            Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip = true;
            RefreshBossAnimationUi();
            SaveMasterSettings();
        }

        private void OnQuickSettingsResetDefaultsClicked()
        {
            SetQuickSettingsResetConfirmVisible(true);
        }

        private void OnQuickSettingsResetConfirmYes()
        {
            ResetAllModSettingsToDefaults();
            SetQuickSettingsResetConfirmVisible(false);
        }

        private void OnQuickSettingsResetConfirmNo()
        {
            SetQuickSettingsResetConfirmVisible(false);
        }

        private void ResetAllModSettingsToDefaults()
        {
            quickRenameMode = false;
            if (renameField != null)
            {
                UObject.Destroy(renameField.gameObject);
                renameField = null;
            }

            if (gearSwitcherPresetRenameField != null)
            {
                UObject.Destroy(gearSwitcherPresetRenameField.gameObject);
            }
            gearSwitcherPresetRenameField = null;
            gearSwitcherPresetRenameLabel = null;
            gearSwitcherPresetRenameOriginalLabel = null;
            gearSwitcherPresetRenameTargetName = null;

            SetGearSwitcherPresetDeleteVisible(false);
            SetGearSwitcherResetConfirmVisible(false);
            SetGearSwitcherPresetVisible(false);
            SetGearSwitcherCharmCostVisible(false);
            SetGearSwitcherVisible(false);

            ApplyInitialDefaults();
            GearSwitcher.ResetDefaults();

            GodhomeQoL.GlobalSettings.QuickMenuOrder = new List<string>();
            GodhomeQoL.GlobalSettings.QuickMenuPositions = new Dictionary<string, QuickMenuEntryPosition>();
            GodhomeQoL.GlobalSettings.QuickMenuCustomLabels = new Dictionary<string, string>();
            GodhomeQoL.GlobalSettings.QuickMenuVisibility = new Dictionary<string, bool>();
            GodhomeQoL.GlobalSettings.QuickMenuFreeLayout = false;
            GodhomeQoL.GlobalSettings.QuickMenuHotkey = DefaultQuickMenuHotkey;
            GodhomeQoL.GlobalSettings.QuickMenuOverlayHotkeys = new Dictionary<string, string>();

            GodhomeQoL.SaveGlobalSettingsSafe();

            LoadMasterSettings();

            RebuildQuickMenuEntries();
            ApplyQuickMenuLayout();
            ApplyQuickMenuOpacity();
            UpdateFreeMenuLabel();
            UpdateRenameModeLabel();
            UpdateQuickMenuEntryStateColors();
            RefreshQuickMenuSettingsUi();

            RebuildGearSwitcherPresetOverlay();
            SetGearSwitcherPresetVisible(false);

            RefreshFastSuperDashUi();
            RefreshCollectorPhasesUi();
            RefreshFastReloadUi();
            RefreshDreamshieldUi();
            RefreshShowHpOnDeathUi();
            RefreshMaskDamageUi();
            RefreshFreezeHitboxesUi();
            RefreshSpeedChangerUi();
            RefreshTeleportKitUi();
            RefreshBossChallengeUi();
            RefreshRandomPantheonsUi();
            RefreshAlwaysFuriousUi();
            RefreshGearSwitcherUi();
            RefreshQolUi();
            RefreshMenuAnimationUi();
            RefreshBossAnimationUi();
            RefreshZoteHelperUi();
        }

        private void OnQuickMenuSettingsBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetQuickSettingsVisible(false);
            UpdateQuickMenuEntryStateColors();
        }

        private void OnZoteHelperBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetZoteHelperVisible(false);
        }

        private void OnZoteHelperResetDefaultsClicked()
        {
            bool wasEnabled = GetZoteHelperEnabled();
            ResetZoteHelperDefaults();
            Modules.BossChallenge.ForceGreyPrinceEnterType.gpzEnterType =
                Modules.BossChallenge.ForceGreyPrinceEnterType.EnterType.Off;
            ApplyBossGpzOption(0);

            if (wasEnabled)
            {
                Modules.BossChallenge.ZoteHelper.ApplyBossHealthIfPresent();
                Modules.BossChallenge.ZoteHelper.ApplyZotelingHealthIfPresent();
            }

            SetZoteHelperEnabled(false);
            RefreshZoteHelperUi();
        }

        private void OnFastSuperDashResetDefaultsClicked()
        {
            SetModuleEnabled(false);
            Modules.QoL.FastSuperDash.instantSuperDash = false;
            Modules.QoL.FastSuperDash.fastSuperDashEverywhere = false;
            Modules.QoL.FastSuperDash.fastSuperDashSpeedMultiplier = 1f;
            RefreshFastSuperDashUi();
        }

        private void OnCollectorResetClicked()
        {
            ResetCollectorPhasesDefaults();
            Modules.CollectorPhases.CollectorPhases.HoGOnly = true;
            SetCollectorRoarEnabled(false);
            SetCollectorPhasesEnabled(false);
            RefreshCollectorPhasesUi();
        }
    }
}
