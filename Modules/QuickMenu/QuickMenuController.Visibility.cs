using UnityEngine;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void ToggleMenu()
        {
            if (quickVisible || quickSettingsVisible || overlayVisible || collectorVisible || fastReloadVisible || dreamshieldVisible || showHpOnDeathVisible || maskDamageVisible || speedChangerVisible || teleportKitVisible || bossChallengeVisible || randomPantheonsVisible || alwaysFuriousVisible || gearSwitcherVisible || gearSwitcherCharmCostVisible || gearSwitcherPresetVisible || qolVisible || menuAnimationVisible || bossAnimationVisible || zoteHelperVisible)
            {
                returnToQuickOnClose = false;
                returnToQolOnClose = false;
                SetQuickVisible(false);
                SetOverlayVisible(false);
                SetCollectorVisible(false);
                SetFastReloadVisible(false);
                SetDreamshieldVisible(false);
                SetShowHpOnDeathVisible(false);
                SetMaskDamageVisible(false);
                SetFreezeHitboxesVisible(false);
                SetSpeedChangerVisible(false);
                SetTeleportKitVisible(false);
                SetBossChallengeVisible(false);
                SetRandomPantheonsVisible(false);
                SetAlwaysFuriousVisible(false);
                SetGearSwitcherVisible(false);
                SetGearSwitcherCharmCostVisible(false);
                SetGearSwitcherPresetVisible(false);
                SetQolVisible(false);
                SetMenuAnimationVisible(false);
                SetBossAnimationVisible(false);
                SetZoteHelperVisible(false);
                SetQuickSettingsVisible(false);
                return;
            }

            SetQuickVisible(true);
        }

        private void SetQuickVisible(bool value, bool instant = false)
        {
            quickVisible = value;
            if (quickRoot == null)
            {
                UpdateUiState();
                return;
            }

            if (!value)
            {
                CancelQuickMenuDrag();
                CancelQuickMenuRename();

                if (quickPanelGroup != null)
                {
                    quickPanelGroup.interactable = false;
                    quickPanelGroup.blocksRaycasts = false;
                }

                StopQuickMenuFade();
                quickMenuFadeAlpha = 0f;
                UpdateQuickMenuAlpha();
                quickRoot.SetActive(false);
            }
            else
            {
                quickRoot.SetActive(true);
                ApplyQuickMenuLayout();
                UpdateFreeMenuLabel();
                UpdateRenameModeLabel();

                if (quickPanelGroup != null)
                {
                    quickPanelGroup.interactable = true;
                    quickPanelGroup.blocksRaycasts = true;
                }

                StopQuickMenuFade();
                quickMenuFadeAlpha = 1f;
                UpdateQuickMenuAlpha();
            }

            UpdateUiState();
        }

        private void SetQuickSettingsVisible(bool value)
        {
            quickSettingsVisible = value;
            if (quickSettingsRoot != null)
            {
                quickSettingsRoot.SetActive(value);
            }

            if (value)
            {
                RefreshQuickMenuSettingsUi();
                SetQuickSettingsResetConfirmVisible(false);
            }
            else
            {
                SetQuickSettingsResetConfirmVisible(false);
                CancelOverlayHotkeyRebind();
            }

            UpdateUiState();
        }

        private void SetOverlayVisible(bool value)
        {
            overlayVisible = value;
            if (overlayRoot != null)
            {
                overlayRoot.SetActive(value);
            }

            if (value)
            {
                RefreshFastSuperDashUi();
            }

            UpdateUiState();
        }

        private void SetCollectorVisible(bool value)
        {
            collectorVisible = value;
            if (collectorRoot != null)
            {
                collectorRoot.SetActive(value);
            }

            if (value)
            {
                RefreshCollectorPhasesUi();
            }

            UpdateUiState();
        }

        private void SetFastReloadVisible(bool value)
        {
            fastReloadVisible = value;
            if (fastReloadRoot != null)
            {
                fastReloadRoot.SetActive(value);
            }

            if (value)
            {
                RefreshFastReloadUi();
            }
            else
            {
                CancelFastReloadRebind();
            }

            UpdateUiState();
        }

        private void SetDreamshieldVisible(bool value)
        {
            dreamshieldVisible = value;
            if (dreamshieldRoot != null)
            {
                dreamshieldRoot.SetActive(value);
            }

            if (value)
            {
                RefreshDreamshieldUi();
            }

            UpdateUiState();
        }

        private void SetShowHpOnDeathVisible(bool value)
        {
            showHpOnDeathVisible = value;
            if (showHpOnDeathRoot != null)
            {
                showHpOnDeathRoot.SetActive(value);
            }

            if (value)
            {
                RefreshShowHpOnDeathUi();
            }
            else
            {
                CancelShowHpOnDeathRebind();
            }

            UpdateUiState();
        }

        private void SetMaskDamageVisible(bool value)
        {
            maskDamageVisible = value;
            if (maskDamageRoot != null)
            {
                maskDamageRoot.SetActive(value);
            }

            if (value)
            {
                RefreshMaskDamageUi();
            }
            else
            {
                CancelMaskDamageRebind();
            }

            UpdateUiState();
        }

        private void SetFreezeHitboxesVisible(bool value)
        {
            freezeHitboxesVisible = value;
            if (freezeHitboxesRoot != null)
            {
                freezeHitboxesRoot.SetActive(value);
            }

            if (value)
            {
                RefreshFreezeHitboxesUi();
            }
            else
            {
                CancelFreezeHitboxesRebind();
            }

            UpdateUiState();
        }

        private void SetSpeedChangerVisible(bool value)
        {
            speedChangerVisible = value;
            if (speedChangerRoot != null)
            {
                speedChangerRoot.SetActive(value);
            }

            if (value)
            {
                RefreshSpeedChangerUi();
            }
            else
            {
                CancelSpeedChangerRebind();
            }

            UpdateUiState();
        }

        internal bool IsSpeedChangerVisible()
        {
            return speedChangerVisible;
        }

        internal void SetSpeedChangerVisibleFromExternal(bool value)
        {
            SetSpeedChangerVisible(value);
        }

        private void SetTeleportKitVisible(bool value)
        {
            teleportKitVisible = value;
            if (teleportKitRoot != null)
            {
                teleportKitRoot.SetActive(value);
            }

            if (value)
            {
                RefreshTeleportKitUi();
            }
            else
            {
                CancelTeleportKitRebind();
            }

            UpdateUiState();
        }

        private void SetBossChallengeVisible(bool value)
        {
            bossChallengeVisible = value;
            if (bossChallengeRoot != null)
            {
                bossChallengeRoot.SetActive(value);
            }

            if (value)
            {
                RefreshBossChallengeUi();
            }

            UpdateUiState();
        }

        private void SetRandomPantheonsVisible(bool value)
        {
            randomPantheonsVisible = value;
            if (randomPantheonsRoot != null)
            {
                randomPantheonsRoot.SetActive(value);
            }

            if (value)
            {
                RefreshRandomPantheonsUi();
            }

            UpdateUiState();
        }

        private void SetAlwaysFuriousVisible(bool value)
        {
            alwaysFuriousVisible = value;
            if (alwaysFuriousRoot != null)
            {
                alwaysFuriousRoot.SetActive(value);
            }

            if (value)
            {
                RefreshAlwaysFuriousUi();
            }

            UpdateUiState();
        }

        private void SetGearSwitcherVisible(bool value)
        {
            gearSwitcherVisible = value;
            if (gearSwitcherRoot != null)
            {
                gearSwitcherRoot.SetActive(value);
            }

            if (value)
            {
                UpdateGearSwitcherHeaderLayout();
                RefreshGearSwitcherUi();
            }
            else
            {
                SetGearSwitcherResetConfirmVisible(false);
            }

            UpdateUiState();
        }

        private void SetGearSwitcherCharmCostVisible(bool value)
        {
            gearSwitcherCharmCostVisible = value;
            if (gearSwitcherCharmCostRoot != null)
            {
                gearSwitcherCharmCostRoot.SetActive(value);
            }

            if (value)
            {
                RefreshGearSwitcherCharmCostUi();
            }

            UpdateUiState();
        }

        private void SetGearSwitcherPresetVisible(bool value)
        {
            gearSwitcherPresetVisible = value;
            if (gearSwitcherPresetRoot != null)
            {
                gearSwitcherPresetRoot.SetActive(value);
            }

            if (value)
            {
                RefreshGearSwitcherPresetSelectUi();
            }
            else if (gearSwitcherPresetRenameField != null)
            {
                CommitGearSwitcherPresetRename(gearSwitcherPresetRenameField.text);
            }

            if (!value)
            {
                SetGearSwitcherPresetDeleteVisible(false);
            }

            UpdateUiState();
        }

        private void SetQolVisible(bool value)
        {
            qolVisible = value;
            if (qolRoot != null)
            {
                qolRoot.SetActive(value);
            }

            if (value)
            {
                RefreshQolUi();
                ScrollToTop(qolScrollRect);
            }

            UpdateUiState();
        }

        private void SetMenuAnimationVisible(bool value)
        {
            menuAnimationVisible = value;
            if (menuAnimationRoot != null)
            {
                menuAnimationRoot.SetActive(value);
            }

            if (value)
            {
                RefreshMenuAnimationUi();
            }

            UpdateUiState();
        }

        private void SetBossAnimationVisible(bool value)
        {
            bossAnimationVisible = value;
            if (bossAnimationRoot != null)
            {
                bossAnimationRoot.SetActive(value);
            }

            if (value)
            {
                RefreshBossAnimationUi();
            }
            else
            {
                CancelFastDreamWarpRebind();
            }

            UpdateUiState();
        }

        private void SetZoteHelperVisible(bool value)
        {
            zoteHelperVisible = value;
            if (zoteHelperRoot != null)
            {
                zoteHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshZoteHelperUi();
            }

            UpdateUiState();
        }

        private void UpdateUiState()
        {
            bool anyVisible = quickVisible || quickSettingsVisible || overlayVisible || collectorVisible || fastReloadVisible || dreamshieldVisible || showHpOnDeathVisible || maskDamageVisible || freezeHitboxesVisible || speedChangerVisible || teleportKitVisible || bossChallengeVisible || randomPantheonsVisible || alwaysFuriousVisible || gearSwitcherVisible || gearSwitcherCharmCostVisible || gearSwitcherPresetVisible || qolVisible || menuAnimationVisible || bossAnimationVisible || zoteHelperVisible;

            if (anyVisible && !uiActive)
            {
                EnableUiInteraction();
                EnableCursorHook();
                uiActive = true;
                return;
            }

            if (!anyVisible && uiActive)
            {
                DisableCursorHook();
                RestoreUiInteraction();
                uiActive = false;
            }
        }

        private void EnableCursorHook()
        {
            if (cursorHookActive)
            {
                return;
            }

            On.InputHandler.OnGUI += ForceCursorVisible;
            ModHooks.CursorHook += ForceCursorVisible;
            cursorHookActive = true;
        }

        private void DisableCursorHook()
        {
            if (!cursorHookActive)
            {
                return;
            }

            On.InputHandler.OnGUI -= ForceCursorVisible;
            ModHooks.CursorHook -= ForceCursorVisible;
            cursorHookActive = false;
        }

        private static void ForceCursorVisible(On.InputHandler.orig_OnGUI orig, InputHandler self)
        {
            orig(self);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private static void ForceCursorVisible()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
