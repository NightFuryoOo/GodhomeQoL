using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int FalseKnightHelperCanvasSortOrder = 10061;
        private const float FalseKnightHelperPanelHeight = PanelHeight;
        private const float FalseKnightHelperRowSpacing = 44f;
        private RectTransform? falseKnightHelperContent;
        private GameObject? falseKnightHelperRoot;
        private bool falseKnightHelperVisible;
        private Text? falseKnightHelperToggleValue;
        private Image? falseKnightHelperToggleIcon;
        private Text? falseKnightP5HpValue;
        private InputField? falseKnightArmorPhase1HpField;
        private InputField? falseKnightArmorPhase2HpField;
        private InputField? falseKnightArmorPhase3HpField;
        private Module? falseKnightHelperModule;

        private static void ResetFalseKnightHelperDefaults()
        {
            Modules.BossChallenge.FalseKnightHelper.falseKnightP5Hp = false;
            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase1Hp = 560;
            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase2Hp = 560;
            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase3Hp = 560;
            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase1HpBeforeP5 = 560;
            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase2HpBeforeP5 = 560;
            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase3HpBeforeP5 = 560;
            Modules.BossChallenge.FalseKnightHelper.falseKnightHasStoredStateBeforeP5 = false;
        }

        private Module? GetFalseKnightHelperModule()
        {
            return GetCachedModule(ref falseKnightHelperModule, typeof(Modules.BossChallenge.FalseKnightHelper));
        }

        private bool GetFalseKnightHelperEnabled()
        {
            return GetFalseKnightHelperModule()?.Enabled ?? false;
        }

        private void SetFalseKnightHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetFalseKnightHelperModule(), value);
            UpdateFalseKnightHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetFalseKnightP5HpEnabled(bool value)
        {
            Modules.BossChallenge.FalseKnightHelper.SetP5HpEnabled(value);
            RefreshFalseKnightHelperUi();
        }

        private void SetFalseKnightArmorPhase1Hp(int value)
        {
            if (Modules.BossChallenge.FalseKnightHelper.falseKnightP5Hp)
            {
                Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase1Hp = 260;
                RefreshFalseKnightHelperUi();
                return;
            }

            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.FalseKnightHelper.ReapplyLiveSettings();
        }

        private void SetFalseKnightArmorPhase2Hp(int value)
        {
            if (Modules.BossChallenge.FalseKnightHelper.falseKnightP5Hp)
            {
                Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase2Hp = 260;
                RefreshFalseKnightHelperUi();
                return;
            }

            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.FalseKnightHelper.ReapplyLiveSettings();
        }

        private void SetFalseKnightArmorPhase3Hp(int value)
        {
            if (Modules.BossChallenge.FalseKnightHelper.falseKnightP5Hp)
            {
                Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase3Hp = 260;
                RefreshFalseKnightHelperUi();
                return;
            }

            Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase3Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.FalseKnightHelper.ReapplyLiveSettings();
        }

        private void RefreshFalseKnightHelperUi()
        {
            Module? module = GetFalseKnightHelperModule();
            UpdateToggleValue(falseKnightHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(falseKnightHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(falseKnightP5HpValue, Modules.BossChallenge.FalseKnightHelper.falseKnightP5Hp);
            UpdateIntInputValue(falseKnightArmorPhase1HpField, Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase1Hp);
            UpdateIntInputValue(falseKnightArmorPhase2HpField, Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase2Hp);
            UpdateIntInputValue(falseKnightArmorPhase3HpField, Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase3Hp);

            UpdateFalseKnightHelperInteractivity();
        }

        private void UpdateFalseKnightHelperInteractivity()
        {
            SetContentInteractivity(falseKnightHelperContent, GetFalseKnightHelperEnabled(), "FalseKnightHelperEnableRow");
            if (!GetFalseKnightHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.FalseKnightHelper.falseKnightP5Hp;
            SetRowInteractivity(falseKnightHelperContent, "FalseKnightArmorPhase1HpRow", !p5Hp);
            SetRowInteractivity(falseKnightHelperContent, "FalseKnightArmorPhase2HpRow", !p5Hp);
            SetRowInteractivity(falseKnightHelperContent, "FalseKnightArmorPhase3HpRow", !p5Hp);
        }

        private void SetFalseKnightHelperVisible(bool value)
        {
            falseKnightHelperVisible = value;
            ApplyPanelVisible(falseKnightHelperRoot, value, RefreshFalseKnightHelperUi);
        }

        private void OnBossManipulateFalseKnightClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetFalseKnightHelperVisible);
        }

        private void OnFalseKnightHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetFalseKnightHelperVisible);
        }

        private void OnFalseKnightHelperResetDefaultsClicked()
        {
            ResetFalseKnightHelperDefaults();
            SetFalseKnightHelperEnabled(false);
            Modules.BossChallenge.FalseKnightHelper.RestoreVanillaHealthIfPresent();
            RefreshFalseKnightHelperUi();
        }

        private void BuildFalseKnightHelperOverlayUi()
        {
            falseKnightHelperRoot = CreateOverlayFrame("FalseKnightHelperOverlayCanvas", FalseKnightHelperCanvasSortOrder, "FalseKnightHelperPanel", FalseKnightHelperPanelHeight, "Modules/FalseKnightHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            RectTransform content = CreateOverlayContent(panel, FalseKnightHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            falseKnightHelperContent = content;

            float rowY = GetRowStartY(FalseKnightHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "FalseKnightHelperEnableRow",
                "Settings/FalseKnightHelper/Enable".Localize(),
                rowY,
                GetFalseKnightHelperEnabled,
                SetFalseKnightHelperEnabled,
                out falseKnightHelperToggleValue,
                out falseKnightHelperToggleIcon
            );

            lastY = rowY;
            rowY += FalseKnightHelperRowSpacing;
            CreateToggleRow(
                content,
                "FalseKnightP5HpRow",
                "Settings/FalseKnightHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FalseKnightHelper.falseKnightP5Hp,
                SetFalseKnightP5HpEnabled,
                out falseKnightP5HpValue
            );

            lastY = rowY;
            rowY += FalseKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FalseKnightArmorPhase1HpRow",
                "Settings/FalseKnightHelper/ArmorPhase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase1Hp,
                SetFalseKnightArmorPhase1Hp,
                1,
                999999,
                10,
                out falseKnightArmorPhase1HpField
            );

            lastY = rowY;
            rowY += FalseKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FalseKnightArmorPhase2HpRow",
                "Settings/FalseKnightHelper/ArmorPhase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase2Hp,
                SetFalseKnightArmorPhase2Hp,
                1,
                999999,
                10,
                out falseKnightArmorPhase2HpField
            );

            lastY = rowY;
            rowY += FalseKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FalseKnightArmorPhase3HpRow",
                "Settings/FalseKnightHelper/ArmorPhase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FalseKnightHelper.falseKnightArmorPhase3Hp,
                SetFalseKnightArmorPhase3Hp,
                1,
                999999,
                10,
                out falseKnightArmorPhase3HpField
            );

            lastY = rowY;
            rowY += FalseKnightHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "FalseKnightHelperResetRow", "Settings/FalseKnightHelper/Reset".Localize(), resetY, OnFalseKnightHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "FalseKnightHelperBackRow", "Back", backY, OnFalseKnightHelperBackClicked);
        }
    }
}
