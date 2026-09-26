using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int WhiteDefenderHelperCanvasSortOrder = 10039;
        private const float WhiteDefenderHelperPanelHeight = PanelHeight;
        private const float WhiteDefenderHelperRowSpacing = 44f;
        private RectTransform? whiteDefenderHelperContent;
        private GameObject? whiteDefenderHelperRoot;
        private bool whiteDefenderHelperVisible;
        private Text? whiteDefenderHelperToggleValue;
        private Image? whiteDefenderHelperToggleIcon;
        private Text? whiteDefenderP5HpValue;
        private Text? whiteDefenderUseMaxHpValue;
        private Text? whiteDefenderUseCustomPhaseValue;
        private InputField? whiteDefenderMaxHpField;
        private InputField? whiteDefenderPhase2HpField;
        private Module? whiteDefenderHelperModule;

        private static void ResetWhiteDefenderHelperDefaults()
        {
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseMaxHp = false;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderP5Hp = false;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderMaxHp = 1600;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderMaxHpBeforeP5 = 1600;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseCustomPhase = false;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderPhase2Hp = 600;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderPhase2HpBeforeP5 = 600;
        }

        private Module? GetWhiteDefenderHelperModule()
        {
            return GetCachedModule(ref whiteDefenderHelperModule, typeof(Modules.BossChallenge.WhiteDefenderHelper));
        }

        private bool GetWhiteDefenderHelperEnabled()
        {
            return GetWhiteDefenderHelperModule()?.Enabled ?? false;
        }

        private void SetWhiteDefenderHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetWhiteDefenderHelperModule(), value);
            UpdateWhiteDefenderHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetWhiteDefenderUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderP5Hp)
            {
                Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseMaxHp = true;
                RefreshWhiteDefenderHelperUi();
                return;
            }

            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseMaxHp = value;
            Modules.BossChallenge.WhiteDefenderHelper.ReapplyLiveSettings();
            RefreshWhiteDefenderHelperUi();
        }

        private void SetWhiteDefenderUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderP5Hp)
            {
                Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseCustomPhase = false;
                RefreshWhiteDefenderHelperUi();
                return;
            }

            int maxPhase2Hp = Mathf.Max(1, Modules.BossChallenge.WhiteDefenderHelper.GetPhase2MaxHpForUi());
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderPhase2Hp = Mathf.Clamp(Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderPhase2Hp, 1, maxPhase2Hp);
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseCustomPhase = value;
            Modules.BossChallenge.WhiteDefenderHelper.ReapplyLiveSettings();
            RefreshWhiteDefenderHelperUi();
        }

        private void SetWhiteDefenderP5HpEnabled(bool value)
        {
            Modules.BossChallenge.WhiteDefenderHelper.SetP5HpEnabled(value);
            RefreshWhiteDefenderHelperUi();
        }

        private void SetWhiteDefenderMaxHp(int value)
        {
            if (Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderP5Hp)
            {
                Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderMaxHp = 1600;
                RefreshWhiteDefenderHelperUi();
                return;
            }

            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseMaxHp)
            {
                Modules.BossChallenge.WhiteDefenderHelper.ApplyWhiteDefenderHealthIfPresent();
            }
        }

        private void SetWhiteDefenderPhase2Hp(int value)
        {
            if (Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderP5Hp)
            {
                Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderPhase2Hp = 600;
                RefreshWhiteDefenderHelperUi();
                return;
            }

            int maxPhase2Hp = Mathf.Max(1, Modules.BossChallenge.WhiteDefenderHelper.GetPhase2MaxHpForUi());
            Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderPhase2Hp = Mathf.Clamp(value, 1, maxPhase2Hp);
            if (Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseCustomPhase)
            {
                Modules.BossChallenge.WhiteDefenderHelper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshWhiteDefenderHelperUi();
        }

        private void RefreshWhiteDefenderHelperUi()
        {
            Module? module = GetWhiteDefenderHelperModule();
            UpdateToggleValue(whiteDefenderHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(whiteDefenderHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(whiteDefenderP5HpValue, Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderP5Hp);
            UpdateToggleValue(whiteDefenderUseMaxHpValue, Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseMaxHp);
            UpdateToggleValue(whiteDefenderUseCustomPhaseValue, Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseCustomPhase);
            UpdateIntInputValue(whiteDefenderMaxHpField, Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderMaxHp);
            UpdateIntInputValue(whiteDefenderPhase2HpField, Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderPhase2Hp);

            UpdateWhiteDefenderHelperInteractivity();
        }

        private void UpdateWhiteDefenderHelperInteractivity()
        {
            SetContentInteractivity(whiteDefenderHelperContent, GetWhiteDefenderHelperEnabled(), "WhiteDefenderHelperEnableRow");
            if (!GetWhiteDefenderHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseMaxHp;
            bool p5Hp = Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderP5Hp;
            bool useCustomPhase = Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseCustomPhase;
            SetRowInteractivity(whiteDefenderHelperContent, "WhiteDefenderUseMaxHpRow", !p5Hp);
            SetRowInteractivity(whiteDefenderHelperContent, "WhiteDefenderMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(whiteDefenderHelperContent, "WhiteDefenderUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(whiteDefenderHelperContent, "WhiteDefenderPhase2HpRow", useCustomPhase && !p5Hp);
        }

        private void SetWhiteDefenderHelperVisible(bool value)
        {
            whiteDefenderHelperVisible = value;
            ApplyPanelVisible(whiteDefenderHelperRoot, value, RefreshWhiteDefenderHelperUi);
        }

        private void OnBossManipulateWhiteDefenderClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetWhiteDefenderHelperVisible);
        }

        private void OnWhiteDefenderHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetWhiteDefenderHelperVisible);
        }

        private void OnWhiteDefenderHelperResetDefaultsClicked()
        {
            ResetWhiteDefenderHelperDefaults();
            SetWhiteDefenderHelperEnabled(false);
            Modules.BossChallenge.WhiteDefenderHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.WhiteDefenderHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshWhiteDefenderHelperUi();
        }

        private void BuildWhiteDefenderHelperOverlayUi()
        {
            whiteDefenderHelperRoot = CreateOverlayFrame("WhiteDefenderHelperOverlayCanvas", WhiteDefenderHelperCanvasSortOrder, "WhiteDefenderHelperPanel", WhiteDefenderHelperPanelHeight, "Modules/WhiteDefenderHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = WhiteDefenderHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, WhiteDefenderHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            whiteDefenderHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "WhiteDefenderHelperEnableRow",
                "Settings/WhiteDefenderHelper/Enable".Localize(),
                rowY,
                GetWhiteDefenderHelperEnabled,
                SetWhiteDefenderHelperEnabled,
                out whiteDefenderHelperToggleValue,
                out whiteDefenderHelperToggleIcon
            );

            lastY = rowY;
            rowY += WhiteDefenderHelperRowSpacing;
            CreateToggleRow(
                content,
                "WhiteDefenderP5HpRow",
                "Settings/WhiteDefenderHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderP5Hp,
                SetWhiteDefenderP5HpEnabled,
                out whiteDefenderP5HpValue
            );

            lastY = rowY;
            rowY += WhiteDefenderHelperRowSpacing;
            CreateToggleRow(
                content,
                "WhiteDefenderUseMaxHpRow",
                "Settings/WhiteDefenderHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseMaxHp,
                SetWhiteDefenderUseMaxHpEnabled,
                out whiteDefenderUseMaxHpValue
            );

            lastY = rowY;
            rowY += WhiteDefenderHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WhiteDefenderMaxHpRow",
                "Settings/WhiteDefenderHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderMaxHp,
                SetWhiteDefenderMaxHp,
                1,
                999999,
                10,
                out whiteDefenderMaxHpField
            );

            lastY = rowY;
            rowY += WhiteDefenderHelperRowSpacing;
            CreateToggleRow(
                content,
                "WhiteDefenderUseCustomPhaseRow",
                "Settings/WhiteDefenderHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderUseCustomPhase,
                SetWhiteDefenderUseCustomPhaseEnabled,
                out whiteDefenderUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += WhiteDefenderHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WhiteDefenderPhase2HpRow",
                "Settings/WhiteDefenderHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WhiteDefenderHelper.whiteDefenderPhase2Hp,
                SetWhiteDefenderPhase2Hp,
                1,
                999999,
                10,
                out whiteDefenderPhase2HpField
            );

            lastY = rowY;
            rowY += WhiteDefenderHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "WhiteDefenderHelperResetRow", "Settings/WhiteDefenderHelper/Reset".Localize(), resetY, OnWhiteDefenderHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "WhiteDefenderHelperBackRow", "Back", backY, OnWhiteDefenderHelperBackClicked);
        }
    }
}
