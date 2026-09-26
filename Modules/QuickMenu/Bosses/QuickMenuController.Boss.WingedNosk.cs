using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int WingedNoskHelperCanvasSortOrder = 10043;
        private const float WingedNoskHelperPanelHeight = PanelHeight;
        private const float WingedNoskHelperRowSpacing = 44f;
        private RectTransform? wingedNoskHelperContent;
        private GameObject? wingedNoskHelperRoot;
        private bool wingedNoskHelperVisible;
        private Text? wingedNoskHelperToggleValue;
        private Image? wingedNoskHelperToggleIcon;
        private Text? wingedNoskP5HpValue;
        private Text? wingedNoskUseMaxHpValue;
        private Text? wingedNoskUseCustomPhaseValue;
        private Text? wingedNoskUseCustomSummonHpValue;
        private Text? wingedNoskUseCustomSummonLimitValue;
        private InputField? wingedNoskMaxHpField;
        private InputField? wingedNoskPhase2HpField;
        private InputField? wingedNoskSummonHpField;
        private InputField? wingedNoskSummonLimitField;
        private Module? wingedNoskHelperModule;

        private static void ResetWingedNoskHelperDefaults()
        {
            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseMaxHp = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomPhase = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonHp = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonLimit = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskMaxHp = 1050;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2Hp = 525;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonHp = 1;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonLimit = 5;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskMaxHpBeforeP5 = 1050;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2HpBeforeP5 = 525;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonHpBeforeP5 = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonHpBeforeP5 = 1;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonLimitBeforeP5 = false;
            Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonLimitBeforeP5 = 5;
        }

        private Module? GetWingedNoskHelperModule()
        {
            return GetCachedModule(ref wingedNoskHelperModule, typeof(Modules.BossChallenge.WingedNoskHelper));
        }

        private bool GetWingedNoskHelperEnabled()
        {
            return GetWingedNoskHelperModule()?.Enabled ?? false;
        }

        private void SetWingedNoskHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetWingedNoskHelperModule(), value);
            UpdateWingedNoskHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetWingedNoskUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp)
            {
                Modules.BossChallenge.WingedNoskHelper.wingedNoskUseMaxHp = true;
                RefreshWingedNoskHelperUi();
                return;
            }

            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseMaxHp = value;
            Modules.BossChallenge.WingedNoskHelper.ReapplyLiveSettings();
            RefreshWingedNoskHelperUi();
        }

        private void SetWingedNoskP5HpEnabled(bool value)
        {
            Modules.BossChallenge.WingedNoskHelper.SetP5HpEnabled(value);
            RefreshWingedNoskHelperUi();
        }

        private void SetWingedNoskMaxHp(int value)
        {
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp)
            {
                Modules.BossChallenge.WingedNoskHelper.wingedNoskMaxHp = 750;
                Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2Hp = 375;
                RefreshWingedNoskHelperUi();
                return;
            }

            Modules.BossChallenge.WingedNoskHelper.wingedNoskMaxHp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2Hp = ClampWingedNoskPhase2HpForUi(Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2Hp);
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskUseMaxHp)
            {
                Modules.BossChallenge.WingedNoskHelper.ApplyWingedNoskHealthIfPresent();
            }

            Modules.BossChallenge.WingedNoskHelper.ApplyPhaseThresholdSettingsIfPresent();
            RefreshWingedNoskHelperUi();
        }

        private void SetWingedNoskUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp)
            {
                Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomPhase = false;
                RefreshWingedNoskHelperUi();
                return;
            }

            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomPhase = value;
            Modules.BossChallenge.WingedNoskHelper.ApplyPhaseThresholdSettingsIfPresent();
            RefreshWingedNoskHelperUi();
        }

        private void SetWingedNoskPhase2Hp(int value)
        {
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp)
            {
                Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2Hp = 375;
                RefreshWingedNoskHelperUi();
                return;
            }

            int clamped = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2Hp = ClampWingedNoskPhase2HpForUi(clamped);
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomPhase)
            {
                Modules.BossChallenge.WingedNoskHelper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshWingedNoskHelperUi();
        }

        private void SetWingedNoskUseCustomSummonHpEnabled(bool value)
        {
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp)
            {
                Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonHp = false;
                RefreshWingedNoskHelperUi();
                return;
            }

            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonHp = value;
            Modules.BossChallenge.WingedNoskHelper.ReapplyLiveSettings();
            RefreshWingedNoskHelperUi();
        }

        private void SetWingedNoskSummonHp(int value)
        {
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp)
            {
                Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonHp = 1;
                RefreshWingedNoskHelperUi();
                return;
            }

            Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonHp)
            {
                Modules.BossChallenge.WingedNoskHelper.ApplyWingedNoskSummonHealthIfPresent();
            }

            RefreshWingedNoskHelperUi();
        }

        private void SetWingedNoskUseCustomSummonLimitEnabled(bool value)
        {
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp)
            {
                Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonLimit = false;
                RefreshWingedNoskHelperUi();
                return;
            }

            Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonLimit = value;
            Modules.BossChallenge.WingedNoskHelper.ReapplyLiveSettings();
            RefreshWingedNoskHelperUi();
        }

        private void SetWingedNoskSummonLimit(int value)
        {
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp)
            {
                Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonLimit = 5;
                RefreshWingedNoskHelperUi();
                return;
            }

            Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonLimit = Mathf.Clamp(value, 0, 999);
            if (Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonLimit)
            {
                Modules.BossChallenge.WingedNoskHelper.ApplySummonLimitSettingsIfPresent();
            }

            RefreshWingedNoskHelperUi();
        }

        private static int ClampWingedNoskPhase2HpForUi(int value)
        {
            int maxHp = Mathf.Clamp(Modules.BossChallenge.WingedNoskHelper.wingedNoskMaxHp, 1, 999999);
            return Mathf.Clamp(value, 1, maxHp);
        }

        private void RefreshWingedNoskHelperUi()
        {
            Module? module = GetWingedNoskHelperModule();
            UpdateToggleValue(wingedNoskHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(wingedNoskHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(wingedNoskP5HpValue, Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp);
            UpdateToggleValue(wingedNoskUseMaxHpValue, Modules.BossChallenge.WingedNoskHelper.wingedNoskUseMaxHp);
            UpdateToggleValue(wingedNoskUseCustomPhaseValue, Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomPhase);
            UpdateToggleValue(wingedNoskUseCustomSummonHpValue, Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonHp);
            UpdateToggleValue(wingedNoskUseCustomSummonLimitValue, Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonLimit);
            UpdateIntInputValue(wingedNoskMaxHpField, Modules.BossChallenge.WingedNoskHelper.wingedNoskMaxHp);
            UpdateIntInputValue(wingedNoskPhase2HpField, Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2Hp);
            UpdateIntInputValue(wingedNoskSummonHpField, Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonHp);
            UpdateIntInputValue(wingedNoskSummonLimitField, Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonLimit);

            UpdateWingedNoskHelperInteractivity();
        }

        private void UpdateWingedNoskHelperInteractivity()
        {
            SetContentInteractivity(wingedNoskHelperContent, GetWingedNoskHelperEnabled(), "WingedNoskHelperEnableRow");
            if (!GetWingedNoskHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.WingedNoskHelper.wingedNoskUseMaxHp;
            bool p5Hp = Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp;
            bool useCustomPhase = Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomPhase;
            bool useCustomSummonHp = Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonHp;
            bool useCustomSummonLimit = Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonLimit;
            SetRowInteractivity(wingedNoskHelperContent, "WingedNoskUseMaxHpRow", !p5Hp);
            SetRowInteractivity(wingedNoskHelperContent, "WingedNoskMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(wingedNoskHelperContent, "WingedNoskUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(wingedNoskHelperContent, "WingedNoskPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(wingedNoskHelperContent, "WingedNoskUseCustomSummonHpRow", !p5Hp);
            SetRowInteractivity(wingedNoskHelperContent, "WingedNoskSummonHpRow", useCustomSummonHp && !p5Hp);
            SetRowInteractivity(wingedNoskHelperContent, "WingedNoskUseCustomSummonLimitRow", !p5Hp);
            SetRowInteractivity(wingedNoskHelperContent, "WingedNoskSummonLimitRow", useCustomSummonLimit && !p5Hp);
        }

        private void SetWingedNoskHelperVisible(bool value)
        {
            wingedNoskHelperVisible = value;
            ApplyPanelVisible(wingedNoskHelperRoot, value, RefreshWingedNoskHelperUi);
        }

        private void OnBossManipulateWingedNoskClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetWingedNoskHelperVisible);
        }

        private void OnWingedNoskHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetWingedNoskHelperVisible);
        }

        private void OnWingedNoskHelperResetDefaultsClicked()
        {
            ResetWingedNoskHelperDefaults();
            SetWingedNoskHelperEnabled(false);
            Modules.BossChallenge.WingedNoskHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.WingedNoskHelper.RestoreVanillaSummonHealthIfPresent();
            Modules.BossChallenge.WingedNoskHelper.RestoreVanillaSummonLimitsIfPresent();
            Modules.BossChallenge.WingedNoskHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshWingedNoskHelperUi();
        }

        private void BuildWingedNoskHelperOverlayUi()
        {
            wingedNoskHelperRoot = CreateOverlayFrame("WingedNoskHelperOverlayCanvas", WingedNoskHelperCanvasSortOrder, "WingedNoskHelperPanel", WingedNoskHelperPanelHeight, "Modules/WingedNoskHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = WingedNoskHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, WingedNoskHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            wingedNoskHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "WingedNoskHelperEnableRow",
                "Settings/WingedNoskHelper/Enable".Localize(),
                rowY,
                GetWingedNoskHelperEnabled,
                SetWingedNoskHelperEnabled,
                out wingedNoskHelperToggleValue,
                out wingedNoskHelperToggleIcon
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateToggleRow(
                content,
                "WingedNoskP5HpRow",
                "Settings/WingedNoskHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskP5Hp,
                SetWingedNoskP5HpEnabled,
                out wingedNoskP5HpValue
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateToggleRow(
                content,
                "WingedNoskUseMaxHpRow",
                "Settings/WingedNoskHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskUseMaxHp,
                SetWingedNoskUseMaxHpEnabled,
                out wingedNoskUseMaxHpValue
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WingedNoskMaxHpRow",
                "Settings/WingedNoskHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskMaxHp,
                SetWingedNoskMaxHp,
                1,
                999999,
                10,
                out wingedNoskMaxHpField
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateToggleRow(
                content,
                "WingedNoskUseCustomPhaseRow",
                "Settings/WingedNoskHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomPhase,
                SetWingedNoskUseCustomPhaseEnabled,
                out wingedNoskUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WingedNoskPhase2HpRow",
                "Settings/WingedNoskHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskPhase2Hp,
                SetWingedNoskPhase2Hp,
                1,
                999999,
                10,
                out wingedNoskPhase2HpField
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateToggleRow(
                content,
                "WingedNoskUseCustomSummonHpRow",
                "Settings/WingedNoskHelper/UseCustomSummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonHp,
                SetWingedNoskUseCustomSummonHpEnabled,
                out wingedNoskUseCustomSummonHpValue
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WingedNoskSummonHpRow",
                "Settings/WingedNoskHelper/SummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonHp,
                SetWingedNoskSummonHp,
                1,
                999999,
                1,
                out wingedNoskSummonHpField
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateToggleRow(
                content,
                "WingedNoskUseCustomSummonLimitRow",
                "Settings/WingedNoskHelper/UseCustomSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskUseCustomSummonLimit,
                SetWingedNoskUseCustomSummonLimitEnabled,
                out wingedNoskUseCustomSummonLimitValue
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WingedNoskSummonLimitRow",
                "Settings/WingedNoskHelper/SummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.WingedNoskHelper.wingedNoskSummonLimit,
                SetWingedNoskSummonLimit,
                0,
                999,
                1,
                out wingedNoskSummonLimitField
            );

            lastY = rowY;
            rowY += WingedNoskHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "WingedNoskHelperResetRow", "Settings/WingedNoskHelper/Reset".Localize(), resetY, OnWingedNoskHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "WingedNoskHelperBackRow", "Back", backY, OnWingedNoskHelperBackClicked);
        }
    }
}
