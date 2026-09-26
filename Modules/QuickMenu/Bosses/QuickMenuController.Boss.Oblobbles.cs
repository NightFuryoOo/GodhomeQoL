using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int OblobblesHelperCanvasSortOrder = 10060;
        private const float OblobblesHelperPanelHeight = PanelHeight;
        private const float OblobblesHelperRowSpacing = 44f;
        private RectTransform? oblobblesHelperContent;
        private GameObject? oblobblesHelperRoot;
        private bool oblobblesHelperVisible;
        private Text? oblobblesHelperToggleValue;
        private Image? oblobblesHelperToggleIcon;
        private Text? oblobblesP5HpValue;
        private Text? oblobblesUsePhase2HpValue;
        private InputField? oblobblesLeftPhase1HpField;
        private InputField? oblobblesRightPhase1HpField;
        private InputField? oblobblesPhase2HpField;
        private Module? oblobblesHelperModule;

        private static void ResetOblobblesHelperDefaults()
        {
            Modules.BossChallenge.OblobblesHelper.oblobblesP5Hp = false;
            Modules.BossChallenge.OblobblesHelper.oblobblesLeftPhase1Hp = 750;
            Modules.BossChallenge.OblobblesHelper.oblobblesRightPhase1Hp = 750;
            Modules.BossChallenge.OblobblesHelper.oblobblesUsePhase2Hp = false;
            Modules.BossChallenge.OblobblesHelper.oblobblesPhase2Hp = 750;
            Modules.BossChallenge.OblobblesHelper.oblobblesLeftPhase1HpBeforeP5 = 750;
            Modules.BossChallenge.OblobblesHelper.oblobblesRightPhase1HpBeforeP5 = 750;
            Modules.BossChallenge.OblobblesHelper.oblobblesUsePhase2HpBeforeP5 = false;
            Modules.BossChallenge.OblobblesHelper.oblobblesPhase2HpBeforeP5 = 750;
            Modules.BossChallenge.OblobblesHelper.oblobblesHasStoredStateBeforeP5 = false;
        }

        private Module? GetOblobblesHelperModule()
        {
            return GetCachedModule(ref oblobblesHelperModule, typeof(Modules.BossChallenge.OblobblesHelper));
        }

        private bool GetOblobblesHelperEnabled()
        {
            return GetOblobblesHelperModule()?.Enabled ?? false;
        }

        private void SetOblobblesHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetOblobblesHelperModule(), value);
            UpdateOblobblesHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetOblobblesP5HpEnabled(bool value)
        {
            Modules.BossChallenge.OblobblesHelper.SetP5HpEnabled(value);
            RefreshOblobblesHelperUi();
        }

        private void SetOblobblesLeftPhase1Hp(int value)
        {
            if (Modules.BossChallenge.OblobblesHelper.oblobblesP5Hp)
            {
                Modules.BossChallenge.OblobblesHelper.oblobblesLeftPhase1Hp = 450;
                RefreshOblobblesHelperUi();
                return;
            }

            Modules.BossChallenge.OblobblesHelper.oblobblesLeftPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.OblobblesHelper.ReapplyLiveSettings();
        }

        private void SetOblobblesRightPhase1Hp(int value)
        {
            if (Modules.BossChallenge.OblobblesHelper.oblobblesP5Hp)
            {
                Modules.BossChallenge.OblobblesHelper.oblobblesRightPhase1Hp = 450;
                RefreshOblobblesHelperUi();
                return;
            }

            Modules.BossChallenge.OblobblesHelper.oblobblesRightPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.OblobblesHelper.ReapplyLiveSettings();
        }

        private void SetOblobblesUsePhase2HpEnabled(bool value)
        {
            if (Modules.BossChallenge.OblobblesHelper.oblobblesP5Hp)
            {
                Modules.BossChallenge.OblobblesHelper.oblobblesUsePhase2Hp = false;
                RefreshOblobblesHelperUi();
                return;
            }

            Modules.BossChallenge.OblobblesHelper.oblobblesUsePhase2Hp = value;
            Modules.BossChallenge.OblobblesHelper.ReapplyLiveSettings();
            RefreshOblobblesHelperUi();
        }

        private void SetOblobblesPhase2Hp(int value)
        {
            if (Modules.BossChallenge.OblobblesHelper.oblobblesP5Hp)
            {
                RefreshOblobblesHelperUi();
                return;
            }

            Modules.BossChallenge.OblobblesHelper.oblobblesPhase2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.OblobblesHelper.ReapplyLiveSettings();
        }

        private void RefreshOblobblesHelperUi()
        {
            Module? module = GetOblobblesHelperModule();
            UpdateToggleValue(oblobblesHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(oblobblesHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(oblobblesP5HpValue, Modules.BossChallenge.OblobblesHelper.oblobblesP5Hp);
            UpdateToggleValue(oblobblesUsePhase2HpValue, Modules.BossChallenge.OblobblesHelper.oblobblesUsePhase2Hp);
            UpdateIntInputValue(oblobblesLeftPhase1HpField, Modules.BossChallenge.OblobblesHelper.oblobblesLeftPhase1Hp);
            UpdateIntInputValue(oblobblesRightPhase1HpField, Modules.BossChallenge.OblobblesHelper.oblobblesRightPhase1Hp);
            UpdateIntInputValue(oblobblesPhase2HpField, Modules.BossChallenge.OblobblesHelper.oblobblesPhase2Hp);

            UpdateOblobblesHelperInteractivity();
        }

        private void UpdateOblobblesHelperInteractivity()
        {
            SetContentInteractivity(oblobblesHelperContent, GetOblobblesHelperEnabled(), "OblobblesHelperEnableRow");
            if (!GetOblobblesHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.OblobblesHelper.oblobblesP5Hp;
            bool usePhase2Hp = Modules.BossChallenge.OblobblesHelper.oblobblesUsePhase2Hp;
            SetRowInteractivity(oblobblesHelperContent, "OblobblesLeftPhase1HpRow", !p5Hp);
            SetRowInteractivity(oblobblesHelperContent, "OblobblesRightPhase1HpRow", !p5Hp);
            SetRowInteractivity(oblobblesHelperContent, "OblobblesUsePhase2HpRow", !p5Hp);
            SetRowInteractivity(oblobblesHelperContent, "OblobblesPhase2HpRow", usePhase2Hp && !p5Hp);
        }

        private void SetOblobblesHelperVisible(bool value)
        {
            oblobblesHelperVisible = value;
            ApplyPanelVisible(oblobblesHelperRoot, value, RefreshOblobblesHelperUi);
        }

        private void OnBossManipulateOblobblesClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetOblobblesHelperVisible);
        }

        private void OnOblobblesHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetOblobblesHelperVisible);
        }

        private void OnOblobblesHelperResetDefaultsClicked()
        {
            ResetOblobblesHelperDefaults();
            SetOblobblesHelperEnabled(false);
            Modules.BossChallenge.OblobblesHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.OblobblesHelper.RestoreVanillaPhase2SettingsIfPresent();
            RefreshOblobblesHelperUi();
        }

        private void BuildOblobblesHelperOverlayUi()
        {
            oblobblesHelperRoot = CreateOverlayFrame("OblobblesHelperOverlayCanvas", OblobblesHelperCanvasSortOrder, "OblobblesHelperPanel", OblobblesHelperPanelHeight, "Modules/OblobblesHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            RectTransform content = CreateOverlayContent(panel, OblobblesHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            oblobblesHelperContent = content;

            float rowY = GetRowStartY(OblobblesHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "OblobblesHelperEnableRow",
                "Settings/OblobblesHelper/Enable".Localize(),
                rowY,
                GetOblobblesHelperEnabled,
                SetOblobblesHelperEnabled,
                out oblobblesHelperToggleValue,
                out oblobblesHelperToggleIcon
            );

            lastY = rowY;
            rowY += OblobblesHelperRowSpacing;
            CreateToggleRow(
                content,
                "OblobblesP5HpRow",
                "Settings/OblobblesHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.OblobblesHelper.oblobblesP5Hp,
                SetOblobblesP5HpEnabled,
                out oblobblesP5HpValue
            );

            lastY = rowY;
            rowY += OblobblesHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "OblobblesLeftPhase1HpRow",
                "Settings/OblobblesHelper/LeftPhase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.OblobblesHelper.oblobblesLeftPhase1Hp,
                SetOblobblesLeftPhase1Hp,
                1,
                999999,
                10,
                out oblobblesLeftPhase1HpField
            );

            lastY = rowY;
            rowY += OblobblesHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "OblobblesRightPhase1HpRow",
                "Settings/OblobblesHelper/RightPhase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.OblobblesHelper.oblobblesRightPhase1Hp,
                SetOblobblesRightPhase1Hp,
                1,
                999999,
                10,
                out oblobblesRightPhase1HpField
            );

            lastY = rowY;
            rowY += OblobblesHelperRowSpacing;
            CreateToggleRow(
                content,
                "OblobblesUsePhase2HpRow",
                "Settings/OblobblesHelper/UsePhase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.OblobblesHelper.oblobblesUsePhase2Hp,
                SetOblobblesUsePhase2HpEnabled,
                out oblobblesUsePhase2HpValue
            );

            lastY = rowY;
            rowY += OblobblesHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "OblobblesPhase2HpRow",
                "Settings/OblobblesHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.OblobblesHelper.oblobblesPhase2Hp,
                SetOblobblesPhase2Hp,
                1,
                999999,
                10,
                out oblobblesPhase2HpField
            );

            lastY = rowY;
            rowY += OblobblesHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "OblobblesHelperResetRow", "Settings/OblobblesHelper/Reset".Localize(), resetY, OnOblobblesHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "OblobblesHelperBackRow", "Back", backY, OnOblobblesHelperBackClicked);
        }
    }
}
