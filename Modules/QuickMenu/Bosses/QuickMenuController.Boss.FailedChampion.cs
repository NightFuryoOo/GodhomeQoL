using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int FailedChampionHelperCanvasSortOrder = 10062;
        private const float FailedChampionHelperPanelHeight = PanelHeight;
        private const float FailedChampionHelperRowSpacing = 44f;
        private RectTransform? failedChampionHelperContent;
        private GameObject? failedChampionHelperRoot;
        private bool failedChampionHelperVisible;
        private Text? failedChampionHelperToggleValue;
        private Image? failedChampionHelperToggleIcon;
        private Text? failedChampionP5HpValue;
        private InputField? failedChampionArmorPhase1HpField;
        private InputField? failedChampionArmorPhase2HpField;
        private InputField? failedChampionArmorPhase3HpField;
        private Module? failedChampionHelperModule;

        private static void ResetFailedChampionHelperDefaults()
        {
            Modules.BossChallenge.FailedChampionHelper.failedChampionP5Hp = false;
            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase1Hp = 600;
            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase2Hp = 600;
            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase3Hp = 600;
            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase1HpBeforeP5 = 600;
            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase2HpBeforeP5 = 600;
            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase3HpBeforeP5 = 600;
            Modules.BossChallenge.FailedChampionHelper.failedChampionHasStoredStateBeforeP5 = false;
        }

        private Module? GetFailedChampionHelperModule()
        {
            return GetCachedModule(ref failedChampionHelperModule, typeof(Modules.BossChallenge.FailedChampionHelper));
        }

        private bool GetFailedChampionHelperEnabled()
        {
            return GetFailedChampionHelperModule()?.Enabled ?? false;
        }

        private void SetFailedChampionHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetFailedChampionHelperModule(), value);
            UpdateFailedChampionHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetFailedChampionP5HpEnabled(bool value)
        {
            Modules.BossChallenge.FailedChampionHelper.SetP5HpEnabled(value);
            RefreshFailedChampionHelperUi();
        }

        private void SetFailedChampionArmorPhase1Hp(int value)
        {
            if (Modules.BossChallenge.FailedChampionHelper.failedChampionP5Hp)
            {
                Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase1Hp = 360;
                RefreshFailedChampionHelperUi();
                return;
            }

            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.FailedChampionHelper.ReapplyLiveSettings();
        }

        private void SetFailedChampionArmorPhase2Hp(int value)
        {
            if (Modules.BossChallenge.FailedChampionHelper.failedChampionP5Hp)
            {
                Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase2Hp = 360;
                RefreshFailedChampionHelperUi();
                return;
            }

            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.FailedChampionHelper.ReapplyLiveSettings();
        }

        private void SetFailedChampionArmorPhase3Hp(int value)
        {
            if (Modules.BossChallenge.FailedChampionHelper.failedChampionP5Hp)
            {
                Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase3Hp = 360;
                RefreshFailedChampionHelperUi();
                return;
            }

            Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase3Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.FailedChampionHelper.ReapplyLiveSettings();
        }

        private void RefreshFailedChampionHelperUi()
        {
            Module? module = GetFailedChampionHelperModule();
            UpdateToggleValue(failedChampionHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(failedChampionHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(failedChampionP5HpValue, Modules.BossChallenge.FailedChampionHelper.failedChampionP5Hp);
            UpdateIntInputValue(failedChampionArmorPhase1HpField, Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase1Hp);
            UpdateIntInputValue(failedChampionArmorPhase2HpField, Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase2Hp);
            UpdateIntInputValue(failedChampionArmorPhase3HpField, Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase3Hp);

            UpdateFailedChampionHelperInteractivity();
        }

        private void UpdateFailedChampionHelperInteractivity()
        {
            SetContentInteractivity(failedChampionHelperContent, GetFailedChampionHelperEnabled(), "FailedChampionHelperEnableRow");
            if (!GetFailedChampionHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.FailedChampionHelper.failedChampionP5Hp;
            SetRowInteractivity(failedChampionHelperContent, "FailedChampionArmorPhase1HpRow", !p5Hp);
            SetRowInteractivity(failedChampionHelperContent, "FailedChampionArmorPhase2HpRow", !p5Hp);
            SetRowInteractivity(failedChampionHelperContent, "FailedChampionArmorPhase3HpRow", !p5Hp);
        }

        private void SetFailedChampionHelperVisible(bool value)
        {
            failedChampionHelperVisible = value;
            ApplyPanelVisible(failedChampionHelperRoot, value, RefreshFailedChampionHelperUi);
        }

        private void OnBossManipulateFailedChampionClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetFailedChampionHelperVisible);
        }

        private void OnFailedChampionHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetFailedChampionHelperVisible);
        }

        private void OnFailedChampionHelperResetDefaultsClicked()
        {
            ResetFailedChampionHelperDefaults();
            SetFailedChampionHelperEnabled(false);
            Modules.BossChallenge.FailedChampionHelper.RestoreVanillaHealthIfPresent();
            RefreshFailedChampionHelperUi();
        }

        private void BuildFailedChampionHelperOverlayUi()
        {
            failedChampionHelperRoot = CreateOverlayFrame("FailedChampionHelperOverlayCanvas", FailedChampionHelperCanvasSortOrder, "FailedChampionHelperPanel", FailedChampionHelperPanelHeight, "Modules/FailedChampionHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            RectTransform content = CreateOverlayContent(panel, FailedChampionHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            failedChampionHelperContent = content;

            float rowY = GetRowStartY(FailedChampionHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "FailedChampionHelperEnableRow",
                "Settings/FailedChampionHelper/Enable".Localize(),
                rowY,
                GetFailedChampionHelperEnabled,
                SetFailedChampionHelperEnabled,
                out failedChampionHelperToggleValue,
                out failedChampionHelperToggleIcon
            );

            lastY = rowY;
            rowY += FailedChampionHelperRowSpacing;
            CreateToggleRow(
                content,
                "FailedChampionP5HpRow",
                "Settings/FailedChampionHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FailedChampionHelper.failedChampionP5Hp,
                SetFailedChampionP5HpEnabled,
                out failedChampionP5HpValue
            );

            lastY = rowY;
            rowY += FailedChampionHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FailedChampionArmorPhase1HpRow",
                "Settings/FailedChampionHelper/ArmorPhase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase1Hp,
                SetFailedChampionArmorPhase1Hp,
                1,
                999999,
                10,
                out failedChampionArmorPhase1HpField
            );

            lastY = rowY;
            rowY += FailedChampionHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FailedChampionArmorPhase2HpRow",
                "Settings/FailedChampionHelper/ArmorPhase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase2Hp,
                SetFailedChampionArmorPhase2Hp,
                1,
                999999,
                10,
                out failedChampionArmorPhase2HpField
            );

            lastY = rowY;
            rowY += FailedChampionHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FailedChampionArmorPhase3HpRow",
                "Settings/FailedChampionHelper/ArmorPhase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FailedChampionHelper.failedChampionArmorPhase3Hp,
                SetFailedChampionArmorPhase3Hp,
                1,
                999999,
                10,
                out failedChampionArmorPhase3HpField
            );

            lastY = rowY;
            rowY += FailedChampionHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "FailedChampionHelperResetRow", "Settings/FailedChampionHelper/Reset".Localize(), resetY, OnFailedChampionHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "FailedChampionHelperBackRow", "Back", backY, OnFailedChampionHelperBackClicked);
        }
    }
}
