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
    {        public void ResetFreeMenuPositions()
        {
            EnsureQuickMenuPositions();
            if (quickVisible && IsQuickMenuFreeLayoutEnabled())
            {
                ApplyQuickMenuFreePositions();
            }
        }

        private bool IsQuickMenuFreeLayoutEnabled()
        {
            return GodhomeQoL.GlobalSettings.QuickMenuFreeLayout;
        }

        private int GetQuickMenuOpacity()
        {
            return Mathf.Clamp(GodhomeQoL.GlobalSettings.QuickMenuOpacity, 1, 100);
        }

        private float GetQuickMenuOpacityAlpha()
        {
            return GetQuickMenuOpacity() / 100f;
        }

        private void ApplyQuickMenuOpacity()
        {
            quickMenuOpacityAlpha = GetQuickMenuOpacityAlpha();
            UpdateQuickMenuAlpha();
        }

        private void UpdateQuickMenuAlpha()
        {
            if (quickPanelGroup != null)
            {
                quickPanelGroup.alpha = quickMenuOpacityAlpha * quickMenuFadeAlpha;
            }
        }

        private void StartQuickMenuFade(bool show)
        {
            if (quickPanelGroup == null || quickRoot == null)
            {
                return;
            }

            StopQuickMenuFade();
            float target = show ? 1f : 0f;
            quickMenuFadeCoroutine = StartCoroutine(FadeQuickMenu(target, show));
        }

        private void StopQuickMenuFade()
        {
            if (quickMenuFadeCoroutine != null)
            {
                StopCoroutine(quickMenuFadeCoroutine);
                quickMenuFadeCoroutine = null;
            }
        }

        private IEnumerator FadeQuickMenu(float target, bool show)
        {
            float start = quickMenuFadeAlpha;
            float duration = QuickMenuFadeSeconds;
            if (Mathf.Abs(target - start) < 0.001f || duration <= 0f)
            {
                quickMenuFadeAlpha = target;
                UpdateQuickMenuAlpha();
                if (!show && quickRoot != null)
                {
                    quickRoot.SetActive(false);
                }

                quickMenuFadeCoroutine = null;
                yield break;
            }

            float time = 0f;
            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(time / duration);
                quickMenuFadeAlpha = Mathf.Lerp(start, target, t);
                UpdateQuickMenuAlpha();
                yield return null;
            }

            quickMenuFadeAlpha = target;
            UpdateQuickMenuAlpha();
            if (!show && quickRoot != null)
            {
                quickRoot.SetActive(false);
            }

            quickMenuFadeCoroutine = null;
        }

        private void SetQuickMenuFreeLayoutEnabled(bool value)
        {
            GodhomeQoL.GlobalSettings.QuickMenuFreeLayout = value;
            GodhomeQoL.SaveGlobalSettingsSafe();
        }

        private void OnQuickMenuOpacityChanged(float value)
        {
            int rounded = Mathf.Clamp(Mathf.RoundToInt(value), 1, 100);
            GodhomeQoL.GlobalSettings.QuickMenuOpacity = rounded;
            GodhomeQoL.SaveGlobalSettingsSafe();
            UpdateFloatValueText(quickMenuOpacityValue, rounded, 0);
            if (quickMenuOpacitySlider != null && Math.Abs(quickMenuOpacitySlider.value - rounded) > 0.0001f)
            {
                quickMenuOpacitySlider.value = rounded;
            }

            ApplyQuickMenuOpacity();
        }

        private void OnQuickFreeMenuClicked()
        {
            bool newValue = !IsQuickMenuFreeLayoutEnabled();
            SetQuickMenuFreeLayoutEnabled(newValue);
            ApplyQuickMenuLayout();
            UpdateFreeMenuLabel();
        }

        private void OnQuickRenameModeClicked()
        {
            quickRenameMode = !quickRenameMode;
            if (!quickRenameMode && renameField != null)
            {
                CommitQuickMenuRename(renameField.text);
            }

            UpdateRenameModeLabel();
        }

        private void OnQuickSettingsClicked()
        {
            returnToQuickOnClose = true;
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
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetGearSwitcherCharmCostVisible(false);
            SetGearSwitcherPresetVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetQuickSettingsVisible(true);
            SetQuickVisible(false);
        }
    }
}
