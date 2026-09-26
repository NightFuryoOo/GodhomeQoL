using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int OroMatoHelperCanvasSortOrder = 10058;
        private const float OroMatoHelperPanelHeight = PanelHeight;
        private const float OroMatoHelperRowSpacing = 44f;
        private RectTransform? oroMatoHelperContent;
        private GameObject? oroMatoHelperRoot;
        private bool oroMatoHelperVisible;
        private Text? oroMatoHelperToggleValue;
        private Image? oroMatoHelperToggleIcon;
        private Text? oroMatoP5HpValue;
        private InputField? oroMatoOroPhase1HpField;
        private InputField? oroMatoOroPhase2HpField;
        private InputField? oroMatoMatoHpField;
        private Module? oroMatoHelperModule;

        private static void ResetOroMatoHelperDefaults()
        {
            Modules.BossChallenge.OroMatoHelper.oroMatoP5Hp = false;
            Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase1Hp = 800;
            Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase2Hp = 1000;
            Modules.BossChallenge.OroMatoHelper.oroMatoMatoHp = 1000;
            Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase1HpBeforeP5 = 800;
            Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase2HpBeforeP5 = 1000;
            Modules.BossChallenge.OroMatoHelper.oroMatoMatoHpBeforeP5 = 1000;
            Modules.BossChallenge.OroMatoHelper.oroMatoHasStoredStateBeforeP5 = false;
        }

        private Module? GetOroMatoHelperModule()
        {
            return GetCachedModule(ref oroMatoHelperModule, typeof(Modules.BossChallenge.OroMatoHelper));
        }

        private bool GetOroMatoHelperEnabled()
        {
            return GetOroMatoHelperModule()?.Enabled ?? false;
        }

        private void SetOroMatoHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetOroMatoHelperModule(), value);
            UpdateOroMatoHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetOroMatoP5HpEnabled(bool value)
        {
            Modules.BossChallenge.OroMatoHelper.SetP5HpEnabled(value);
            RefreshOroMatoHelperUi();
        }

        private void SetOroMatoOroPhase1Hp(int value)
        {
            if (Modules.BossChallenge.OroMatoHelper.oroMatoP5Hp)
            {
                Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase1Hp = 500;
                RefreshOroMatoHelperUi();
                return;
            }

            Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.OroMatoHelper.ReapplyLiveSettings();
        }

        private void SetOroMatoOroPhase2Hp(int value)
        {
            if (Modules.BossChallenge.OroMatoHelper.oroMatoP5Hp)
            {
                Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase2Hp = 600;
                RefreshOroMatoHelperUi();
                return;
            }

            Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.OroMatoHelper.ReapplyLiveSettings();
        }

        private void SetOroMatoMatoHp(int value)
        {
            if (Modules.BossChallenge.OroMatoHelper.oroMatoP5Hp)
            {
                Modules.BossChallenge.OroMatoHelper.oroMatoMatoHp = 1000;
                RefreshOroMatoHelperUi();
                return;
            }

            Modules.BossChallenge.OroMatoHelper.oroMatoMatoHp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.OroMatoHelper.ReapplyLiveSettings();
        }

        private void RefreshOroMatoHelperUi()
        {
            Module? module = GetOroMatoHelperModule();
            UpdateToggleValue(oroMatoHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(oroMatoHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(oroMatoP5HpValue, Modules.BossChallenge.OroMatoHelper.oroMatoP5Hp);
            UpdateIntInputValue(oroMatoOroPhase1HpField, Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase1Hp);
            UpdateIntInputValue(oroMatoOroPhase2HpField, Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase2Hp);
            UpdateIntInputValue(oroMatoMatoHpField, Modules.BossChallenge.OroMatoHelper.oroMatoMatoHp);

            UpdateOroMatoHelperInteractivity();
        }

        private void UpdateOroMatoHelperInteractivity()
        {
            SetContentInteractivity(oroMatoHelperContent, GetOroMatoHelperEnabled(), "OroMatoHelperEnableRow");
            if (!GetOroMatoHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.OroMatoHelper.oroMatoP5Hp;
            SetRowInteractivity(oroMatoHelperContent, "OroMatoOroPhase1HpRow", !p5Hp);
            SetRowInteractivity(oroMatoHelperContent, "OroMatoOroPhase2HpRow", !p5Hp);
            SetRowInteractivity(oroMatoHelperContent, "OroMatoMatoHpRow", !p5Hp);
        }

        private void SetOroMatoHelperVisible(bool value)
        {
            oroMatoHelperVisible = value;
            ApplyPanelVisible(oroMatoHelperRoot, value, RefreshOroMatoHelperUi);
        }

        private void OnBossManipulateOroMatoClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetOroMatoHelperVisible);
        }

        private void OnOroMatoHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetOroMatoHelperVisible);
        }

        private void OnOroMatoHelperResetDefaultsClicked()
        {
            ResetOroMatoHelperDefaults();
            SetOroMatoHelperEnabled(false);
            Modules.BossChallenge.OroMatoHelper.RestoreVanillaHealthIfPresent();
            RefreshOroMatoHelperUi();
        }

        private void BuildOroMatoHelperOverlayUi()
        {
            oroMatoHelperRoot = CreateOverlayFrame("OroMatoHelperOverlayCanvas", OroMatoHelperCanvasSortOrder, "OroMatoHelperPanel", OroMatoHelperPanelHeight, "Modules/OroMatoHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            RectTransform content = CreateOverlayContent(panel, OroMatoHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            oroMatoHelperContent = content;

            float rowY = GetRowStartY(OroMatoHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "OroMatoHelperEnableRow",
                "Settings/OroMatoHelper/Enable".Localize(),
                rowY,
                GetOroMatoHelperEnabled,
                SetOroMatoHelperEnabled,
                out oroMatoHelperToggleValue,
                out oroMatoHelperToggleIcon
            );

            lastY = rowY;
            rowY += OroMatoHelperRowSpacing;
            CreateToggleRow(
                content,
                "OroMatoP5HpRow",
                "Settings/OroMatoHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.OroMatoHelper.oroMatoP5Hp,
                SetOroMatoP5HpEnabled,
                out oroMatoP5HpValue
            );

            lastY = rowY;
            rowY += OroMatoHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "OroMatoOroPhase1HpRow",
                "Settings/OroMatoHelper/OroPhase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase1Hp,
                SetOroMatoOroPhase1Hp,
                1,
                999999,
                10,
                out oroMatoOroPhase1HpField
            );

            lastY = rowY;
            rowY += OroMatoHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "OroMatoOroPhase2HpRow",
                "Settings/OroMatoHelper/OroPhase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.OroMatoHelper.oroMatoOroPhase2Hp,
                SetOroMatoOroPhase2Hp,
                1,
                999999,
                10,
                out oroMatoOroPhase2HpField
            );

            lastY = rowY;
            rowY += OroMatoHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "OroMatoMatoHpRow",
                "Settings/OroMatoHelper/MatoHP".Localize(),
                rowY,
                () => Modules.BossChallenge.OroMatoHelper.oroMatoMatoHp,
                SetOroMatoMatoHp,
                1,
                999999,
                10,
                out oroMatoMatoHpField
            );

            lastY = rowY;
            rowY += OroMatoHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "OroMatoHelperResetRow", "Settings/OroMatoHelper/Reset".Localize(), resetY, OnOroMatoHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "OroMatoHelperBackRow", "Back", backY, OnOroMatoHelperBackClicked);
        }
    }
}
