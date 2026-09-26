using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int MantisLordHelperCanvasSortOrder = 10064;
        private const float MantisLordHelperPanelHeight = PanelHeight;
        private const float MantisLordHelperRowSpacing = 44f;
        private RectTransform? mantisLordHelperContent;
        private GameObject? mantisLordHelperRoot;
        private bool mantisLordHelperVisible;
        private Text? mantisLordHelperToggleValue;
        private Image? mantisLordHelperToggleIcon;
        private InputField? mantisLordPhase1HpField;
        private InputField? mantisLordPhase2S1HpField;
        private InputField? mantisLordPhase2S2HpField;
        private Module? mantisLordHelperModule;

        private static void ResetMantisLordHelperDefaults()
        {
            Modules.BossChallenge.MantisLordHelper.mantisLordPhase1Hp = 500;
            Modules.BossChallenge.MantisLordHelper.mantisLordPhase2S1Hp = 600;
            Modules.BossChallenge.MantisLordHelper.mantisLordPhase2S2Hp = 600;
        }

        private Module? GetMantisLordHelperModule()
        {
            return GetCachedModule(ref mantisLordHelperModule, typeof(Modules.BossChallenge.MantisLordHelper));
        }

        private bool GetMantisLordHelperEnabled()
        {
            return GetMantisLordHelperModule()?.Enabled ?? false;
        }

        private void SetMantisLordHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetMantisLordHelperModule(), value);
            UpdateMantisLordHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetMantisLordPhase1Hp(int value)
        {
            Modules.BossChallenge.MantisLordHelper.mantisLordPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.MantisLordHelper.ReapplyLiveSettings();
        }

        private void SetMantisLordPhase2S1Hp(int value)
        {
            Modules.BossChallenge.MantisLordHelper.mantisLordPhase2S1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.MantisLordHelper.ReapplyLiveSettings();
        }

        private void SetMantisLordPhase2S2Hp(int value)
        {
            Modules.BossChallenge.MantisLordHelper.mantisLordPhase2S2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.MantisLordHelper.ReapplyLiveSettings();
        }

        private void RefreshMantisLordHelperUi()
        {
            Module? module = GetMantisLordHelperModule();
            UpdateToggleValue(mantisLordHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(mantisLordHelperToggleIcon, module?.Enabled ?? false);
            UpdateIntInputValue(mantisLordPhase1HpField, Modules.BossChallenge.MantisLordHelper.mantisLordPhase1Hp);
            UpdateIntInputValue(mantisLordPhase2S1HpField, Modules.BossChallenge.MantisLordHelper.mantisLordPhase2S1Hp);
            UpdateIntInputValue(mantisLordPhase2S2HpField, Modules.BossChallenge.MantisLordHelper.mantisLordPhase2S2Hp);

            UpdateMantisLordHelperInteractivity();
        }

        private void UpdateMantisLordHelperInteractivity()
        {
            SetContentInteractivity(mantisLordHelperContent, GetMantisLordHelperEnabled(), "MantisLordHelperEnableRow");
            if (!GetMantisLordHelperEnabled())
            {
                return;
            }

            SetRowInteractivity(mantisLordHelperContent, "MantisLordPhase1HpRow", true);
            SetRowInteractivity(mantisLordHelperContent, "MantisLordPhase2S1HpRow", true);
            SetRowInteractivity(mantisLordHelperContent, "MantisLordPhase2S2HpRow", true);
        }

        private void SetMantisLordHelperVisible(bool value)
        {
            mantisLordHelperVisible = value;
            ApplyPanelVisible(mantisLordHelperRoot, value, RefreshMantisLordHelperUi);
        }

        private void OnBossManipulateMantisLordClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetMantisLordHelperVisible);
        }

        private void OnMantisLordHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetMantisLordHelperVisible);
        }

        private void OnMantisLordHelperResetDefaultsClicked()
        {
            ResetMantisLordHelperDefaults();
            SetMantisLordHelperEnabled(false);
            Modules.BossChallenge.MantisLordHelper.RestoreVanillaHealthIfPresent();
            RefreshMantisLordHelperUi();
        }

        private void BuildMantisLordHelperOverlayUi()
        {
            mantisLordHelperRoot = CreateOverlayFrame("MantisLordHelperOverlayCanvas", MantisLordHelperCanvasSortOrder, "MantisLordHelperPanel", MantisLordHelperPanelHeight, "Modules/MantisLordHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            RectTransform content = CreateOverlayContent(panel, MantisLordHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            mantisLordHelperContent = content;

            float rowY = GetRowStartY(MantisLordHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "MantisLordHelperEnableRow",
                "Settings/MantisLordHelper/Enable".Localize(),
                rowY,
                GetMantisLordHelperEnabled,
                SetMantisLordHelperEnabled,
                out mantisLordHelperToggleValue,
                out mantisLordHelperToggleIcon
            );

            lastY = rowY;
            rowY += MantisLordHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MantisLordPhase1HpRow",
                "Settings/MantisLordHelper/Phase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.MantisLordHelper.mantisLordPhase1Hp,
                SetMantisLordPhase1Hp,
                1,
                999999,
                10,
                out mantisLordPhase1HpField
            );

            lastY = rowY;
            rowY += MantisLordHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MantisLordPhase2S1HpRow",
                "Settings/MantisLordHelper/Phase2S1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.MantisLordHelper.mantisLordPhase2S1Hp,
                SetMantisLordPhase2S1Hp,
                1,
                999999,
                10,
                out mantisLordPhase2S1HpField
            );

            lastY = rowY;
            rowY += MantisLordHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MantisLordPhase2S2HpRow",
                "Settings/MantisLordHelper/Phase2S2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.MantisLordHelper.mantisLordPhase2S2Hp,
                SetMantisLordPhase2S2Hp,
                1,
                999999,
                10,
                out mantisLordPhase2S2HpField
            );

            lastY = rowY;
            rowY += MantisLordHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "MantisLordHelperResetRow", "Settings/MantisLordHelper/Reset".Localize(), resetY, OnMantisLordHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "MantisLordHelperBackRow", "Back", backY, OnMantisLordHelperBackClicked);
        }
    }
}
