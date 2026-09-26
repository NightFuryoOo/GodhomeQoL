using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int XeroHelperCanvasSortOrder = 10032;
        private const float XeroHelperPanelHeight = PanelHeight;
        private const float XeroHelperRowSpacing = 44f;
        private RectTransform? xeroHelperContent;
        private GameObject? xeroHelperRoot;
        private bool xeroHelperVisible;
        private Text? xeroHelperToggleValue;
        private Image? xeroHelperToggleIcon;
        private Text? xeroP5HpValue;
        private Text? xeroUseMaxHpValue;
        private Text? xeroUseCustomPhaseValue;
        private InputField? xeroMaxHpField;
        private InputField? xeroPhase2HpField;
        private Module? xeroHelperModule;

        private static void ResetXeroHelperDefaults()
        {
            Modules.BossChallenge.XeroHelper.xeroUseMaxHp = false;
            Modules.BossChallenge.XeroHelper.xeroP5Hp = false;
            Modules.BossChallenge.XeroHelper.xeroUseCustomPhase = false;
            Modules.BossChallenge.XeroHelper.xeroMaxHp = 900;
            Modules.BossChallenge.XeroHelper.xeroPhase2Hp = 450;
            Modules.BossChallenge.XeroHelper.xeroMaxHpBeforeP5 = 900;
            Modules.BossChallenge.XeroHelper.xeroUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.XeroHelper.xeroUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.XeroHelper.xeroPhase2HpBeforeP5 = 450;
            Modules.BossChallenge.XeroHelper.xeroHasStoredStateBeforeP5 = false;
        }

        private Module? GetXeroHelperModule()
        {
            return GetCachedModule(ref xeroHelperModule, typeof(Modules.BossChallenge.XeroHelper));
        }

        private bool GetXeroHelperEnabled()
        {
            return GetXeroHelperModule()?.Enabled ?? false;
        }

        private void SetXeroHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetXeroHelperModule(), value);
            UpdateXeroHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetXeroUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.XeroHelper.xeroP5Hp)
            {
                Modules.BossChallenge.XeroHelper.xeroUseMaxHp = true;
                RefreshXeroHelperUi();
                return;
            }

            Modules.BossChallenge.XeroHelper.xeroUseMaxHp = value;
            Modules.BossChallenge.XeroHelper.xeroPhase2Hp = Mathf.Clamp(Modules.BossChallenge.XeroHelper.xeroPhase2Hp, 1, GetXeroPhase2MaxHp());
            Modules.BossChallenge.XeroHelper.ReapplyLiveSettings();
            RefreshXeroHelperUi();
        }

        private void SetXeroUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.XeroHelper.xeroP5Hp)
            {
                Modules.BossChallenge.XeroHelper.xeroUseCustomPhase = false;
                RefreshXeroHelperUi();
                return;
            }

            Modules.BossChallenge.XeroHelper.xeroPhase2Hp = Mathf.Clamp(Modules.BossChallenge.XeroHelper.xeroPhase2Hp, 1, GetXeroPhase2MaxHp());
            Modules.BossChallenge.XeroHelper.xeroUseCustomPhase = value;
            Modules.BossChallenge.XeroHelper.ReapplyLiveSettings();
            RefreshXeroHelperUi();
        }

        private void SetXeroP5HpEnabled(bool value)
        {
            Modules.BossChallenge.XeroHelper.SetP5HpEnabled(value);
            RefreshXeroHelperUi();
        }

        private void SetXeroMaxHp(int value)
        {
            if (Modules.BossChallenge.XeroHelper.xeroP5Hp)
            {
                Modules.BossChallenge.XeroHelper.xeroMaxHp = 650;
                RefreshXeroHelperUi();
                return;
            }

            Modules.BossChallenge.XeroHelper.xeroMaxHp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.XeroHelper.xeroPhase2Hp = Mathf.Clamp(Modules.BossChallenge.XeroHelper.xeroPhase2Hp, 1, GetXeroPhase2MaxHp());
            if (Modules.BossChallenge.XeroHelper.xeroUseMaxHp)
            {
                Modules.BossChallenge.XeroHelper.ApplyXeroHealthIfPresent();
            }

            if (Modules.BossChallenge.XeroHelper.xeroUseCustomPhase)
            {
                Modules.BossChallenge.XeroHelper.ReapplyLiveSettings();
            }

            RefreshXeroHelperUi();
        }

        private void SetXeroPhase2Hp(int value)
        {
            if (Modules.BossChallenge.XeroHelper.xeroP5Hp)
            {
                RefreshXeroHelperUi();
                return;
            }

            Modules.BossChallenge.XeroHelper.xeroPhase2Hp = Mathf.Clamp(value, 1, GetXeroPhase2MaxHp());

            if (Modules.BossChallenge.XeroHelper.xeroUseCustomPhase)
            {
                Modules.BossChallenge.XeroHelper.ReapplyLiveSettings();
            }

            RefreshXeroHelperUi();
        }

        private static int GetXeroPhase2MaxHp()
        {
            if (Modules.BossChallenge.XeroHelper.xeroP5Hp)
            {
                return 650;
            }

            if (Modules.BossChallenge.XeroHelper.xeroUseMaxHp)
            {
                return Mathf.Clamp(Modules.BossChallenge.XeroHelper.xeroMaxHp, 1, 999999);
            }

            return 900;
        }

        private void RefreshXeroHelperUi()
        {
            Module? module = GetXeroHelperModule();
            UpdateToggleValue(xeroHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(xeroHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(xeroP5HpValue, Modules.BossChallenge.XeroHelper.xeroP5Hp);
            UpdateToggleValue(xeroUseMaxHpValue, Modules.BossChallenge.XeroHelper.xeroUseMaxHp);
            UpdateToggleValue(xeroUseCustomPhaseValue, Modules.BossChallenge.XeroHelper.xeroUseCustomPhase);
            UpdateIntInputValue(xeroMaxHpField, Modules.BossChallenge.XeroHelper.xeroMaxHp);
            UpdateIntInputValue(xeroPhase2HpField, Modules.BossChallenge.XeroHelper.xeroPhase2Hp);

            UpdateXeroHelperInteractivity();
        }

        private void UpdateXeroHelperInteractivity()
        {
            SetContentInteractivity(xeroHelperContent, GetXeroHelperEnabled(), "XeroHelperEnableRow");
            if (!GetXeroHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.XeroHelper.xeroUseMaxHp;
            bool p5Hp = Modules.BossChallenge.XeroHelper.xeroP5Hp;
            bool useCustomPhase = Modules.BossChallenge.XeroHelper.xeroUseCustomPhase;
            SetRowInteractivity(xeroHelperContent, "XeroUseMaxHpRow", !p5Hp);
            SetRowInteractivity(xeroHelperContent, "XeroMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(xeroHelperContent, "XeroUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(xeroHelperContent, "XeroPhase2HpRow", useCustomPhase && !p5Hp);
        }

        private void SetXeroHelperVisible(bool value)
        {
            xeroHelperVisible = value;
            ApplyPanelVisible(xeroHelperRoot, value, RefreshXeroHelperUi);
        }

        private void OnBossManipulateXeroClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetXeroHelperVisible);
        }

        private void OnXeroHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetXeroHelperVisible);
        }

        private void OnXeroHelperResetDefaultsClicked()
        {
            ResetXeroHelperDefaults();
            SetXeroHelperEnabled(false);
            Modules.BossChallenge.XeroHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.XeroHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshXeroHelperUi();
        }

        private void BuildXeroHelperOverlayUi()
        {
            xeroHelperRoot = CreateOverlayFrame("XeroHelperOverlayCanvas", XeroHelperCanvasSortOrder, "XeroHelperPanel", XeroHelperPanelHeight, "Modules/XeroHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = XeroHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, XeroHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            xeroHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "XeroHelperEnableRow",
                "Settings/XeroHelper/Enable".Localize(),
                rowY,
                GetXeroHelperEnabled,
                SetXeroHelperEnabled,
                out xeroHelperToggleValue,
                out xeroHelperToggleIcon
            );

            lastY = rowY;
            rowY += XeroHelperRowSpacing;
            CreateToggleRow(
                content,
                "XeroP5HpRow",
                "Settings/XeroHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.XeroHelper.xeroP5Hp,
                SetXeroP5HpEnabled,
                out xeroP5HpValue
            );

            lastY = rowY;
            rowY += XeroHelperRowSpacing;
            CreateToggleRow(
                content,
                "XeroUseMaxHpRow",
                "Settings/XeroHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.XeroHelper.xeroUseMaxHp,
                SetXeroUseMaxHpEnabled,
                out xeroUseMaxHpValue
            );

            lastY = rowY;
            rowY += XeroHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "XeroMaxHpRow",
                "Settings/XeroHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.XeroHelper.xeroMaxHp,
                SetXeroMaxHp,
                1,
                999999,
                10,
                out xeroMaxHpField
            );

            lastY = rowY;
            rowY += XeroHelperRowSpacing;
            CreateToggleRow(
                content,
                "XeroUseCustomPhaseRow",
                "Settings/XeroHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.XeroHelper.xeroUseCustomPhase,
                SetXeroUseCustomPhaseEnabled,
                out xeroUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += XeroHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "XeroPhase2HpRow",
                "Settings/XeroHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.XeroHelper.xeroPhase2Hp,
                SetXeroPhase2Hp,
                1,
                999999,
                10,
                out xeroPhase2HpField
            );

            lastY = rowY;
            rowY += XeroHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "XeroHelperResetRow", "Settings/XeroHelper/Reset".Localize(), resetY, OnXeroHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "XeroHelperBackRow", "Back", backY, OnXeroHelperBackClicked);
        }
    }
}
