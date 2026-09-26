using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int FlukemarmHelperCanvasSortOrder = 10053;
        private const float FlukemarmHelperPanelHeight = PanelHeight;
        private const float FlukemarmHelperRowSpacing = 44f;
        private RectTransform? flukemarmHelperContent;
        private GameObject? flukemarmHelperRoot;
        private bool flukemarmHelperVisible;
        private Text? flukemarmHelperToggleValue;
        private Image? flukemarmHelperToggleIcon;
        private Text? flukemarmP5HpValue;
        private Text? flukemarmUseMaxHpValue;
        private Text? flukemarmUseCustomSummonLimitValue;
        private InputField? flukemarmMaxHpField;
        private InputField? flukemarmFlyHpField;
        private InputField? flukemarmSummonLimitField;
        private Module? flukemarmHelperModule;

        private static void ResetFlukemarmHelperDefaults()
        {
            Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHp = false;
            Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp = false;
            Modules.BossChallenge.FlukemarmHelper.flukemarmUseCustomSummonLimit = false;
            Modules.BossChallenge.FlukemarmHelper.flukemarmMaxHp = 900;
            Modules.BossChallenge.FlukemarmHelper.flukemarmFlyHp = 35;
            Modules.BossChallenge.FlukemarmHelper.flukemarmSummonLimit = 6;
            Modules.BossChallenge.FlukemarmHelper.flukemarmMaxHpBeforeP5 = 900;
            Modules.BossChallenge.FlukemarmHelper.flukemarmFlyHpBeforeP5 = 35;
            Modules.BossChallenge.FlukemarmHelper.flukemarmUseCustomSummonLimitBeforeP5 = false;
            Modules.BossChallenge.FlukemarmHelper.flukemarmSummonLimitBeforeP5 = 6;
            Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.FlukemarmHelper.flukemarmHasStoredStateBeforeP5 = false;
        }

        private Module? GetFlukemarmHelperModule()
        {
            return GetCachedModule(ref flukemarmHelperModule, typeof(Modules.BossChallenge.FlukemarmHelper));
        }

        private bool GetFlukemarmHelperEnabled()
        {
            return GetFlukemarmHelperModule()?.Enabled ?? false;
        }

        private void SetFlukemarmHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetFlukemarmHelperModule(), value);
            UpdateFlukemarmHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetFlukemarmUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp)
            {
                Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHp = true;
                RefreshFlukemarmHelperUi();
                return;
            }

            Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHp = value;
            Modules.BossChallenge.FlukemarmHelper.ReapplyLiveSettings();
            RefreshFlukemarmHelperUi();
        }

        private void SetFlukemarmP5HpEnabled(bool value)
        {
            Modules.BossChallenge.FlukemarmHelper.SetP5HpEnabled(value);
            RefreshFlukemarmHelperUi();
        }

        private void SetFlukemarmUseCustomSummonLimitEnabled(bool value)
        {
            if (Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp)
            {
                Modules.BossChallenge.FlukemarmHelper.flukemarmUseCustomSummonLimit = false;
                RefreshFlukemarmHelperUi();
                return;
            }

            Modules.BossChallenge.FlukemarmHelper.flukemarmUseCustomSummonLimit = value;
            Modules.BossChallenge.FlukemarmHelper.ReapplyLiveSettings();
            RefreshFlukemarmHelperUi();
        }

        private void SetFlukemarmMaxHp(int value)
        {
            if (Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp)
            {
                Modules.BossChallenge.FlukemarmHelper.flukemarmMaxHp = 500;
                RefreshFlukemarmHelperUi();
                return;
            }

            Modules.BossChallenge.FlukemarmHelper.flukemarmMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHp)
            {
                Modules.BossChallenge.FlukemarmHelper.ApplyFlukemarmHealthIfPresent();
            }
        }

        private void SetFlukemarmFlyHp(int value)
        {
            if (Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp)
            {
                Modules.BossChallenge.FlukemarmHelper.flukemarmFlyHp = 35;
                RefreshFlukemarmHelperUi();
                return;
            }

            Modules.BossChallenge.FlukemarmHelper.flukemarmFlyHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHp)
            {
                Modules.BossChallenge.FlukemarmHelper.ApplyFlukemarmHealthIfPresent();
            }
        }

        private void SetFlukemarmSummonLimit(int value)
        {
            if (Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp)
            {
                Modules.BossChallenge.FlukemarmHelper.flukemarmSummonLimit = 6;
                RefreshFlukemarmHelperUi();
                return;
            }

            Modules.BossChallenge.FlukemarmHelper.flukemarmSummonLimit = Mathf.Clamp(value, 6, 999);
            if (Modules.BossChallenge.FlukemarmHelper.flukemarmUseCustomSummonLimit)
            {
                Modules.BossChallenge.FlukemarmHelper.ApplySummonLimitSettingsIfPresent();
            }
        }

        private void RefreshFlukemarmHelperUi()
        {
            Module? module = GetFlukemarmHelperModule();
            UpdateToggleValue(flukemarmHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(flukemarmHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(flukemarmP5HpValue, Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp);
            UpdateToggleValue(flukemarmUseMaxHpValue, Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHp);
            UpdateToggleValue(flukemarmUseCustomSummonLimitValue, Modules.BossChallenge.FlukemarmHelper.flukemarmUseCustomSummonLimit);
            UpdateIntInputValue(flukemarmMaxHpField, Modules.BossChallenge.FlukemarmHelper.flukemarmMaxHp);
            UpdateIntInputValue(flukemarmFlyHpField, Modules.BossChallenge.FlukemarmHelper.flukemarmFlyHp);
            UpdateIntInputValue(flukemarmSummonLimitField, Modules.BossChallenge.FlukemarmHelper.flukemarmSummonLimit);

            UpdateFlukemarmHelperInteractivity();
        }

        private void UpdateFlukemarmHelperInteractivity()
        {
            SetContentInteractivity(flukemarmHelperContent, GetFlukemarmHelperEnabled(), "FlukemarmHelperEnableRow");
            if (!GetFlukemarmHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHp;
            bool p5Hp = Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp;
            bool useCustomSummonLimit = Modules.BossChallenge.FlukemarmHelper.flukemarmUseCustomSummonLimit;
            SetRowInteractivity(flukemarmHelperContent, "FlukemarmUseMaxHpRow", !p5Hp);
            SetRowInteractivity(flukemarmHelperContent, "FlukemarmMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(flukemarmHelperContent, "FlukemarmFlyHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(flukemarmHelperContent, "FlukemarmUseCustomSummonLimitRow", !p5Hp);
            SetRowInteractivity(flukemarmHelperContent, "FlukemarmSummonLimitRow", useCustomSummonLimit && !p5Hp);
        }

        private void SetFlukemarmHelperVisible(bool value)
        {
            flukemarmHelperVisible = value;
            ApplyPanelVisible(flukemarmHelperRoot, value, RefreshFlukemarmHelperUi);
        }

        private void OnBossManipulateFlukemarmClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetFlukemarmHelperVisible);
        }

        private void OnFlukemarmHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetFlukemarmHelperVisible);
        }

        private void OnFlukemarmHelperResetDefaultsClicked()
        {
            ResetFlukemarmHelperDefaults();
            SetFlukemarmHelperEnabled(false);
            Modules.BossChallenge.FlukemarmHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.FlukemarmHelper.RestoreVanillaSummonLimitsIfPresent();
            RefreshFlukemarmHelperUi();
        }

        private void BuildFlukemarmHelperOverlayUi()
        {
            flukemarmHelperRoot = CreateOverlayFrame("FlukemarmHelperOverlayCanvas", FlukemarmHelperCanvasSortOrder, "FlukemarmHelperPanel", FlukemarmHelperPanelHeight, "Modules/FlukemarmHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            RectTransform content = CreateOverlayContent(panel, FlukemarmHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            flukemarmHelperContent = content;

            float rowY = GetRowStartY(FlukemarmHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "FlukemarmHelperEnableRow",
                "Settings/FlukemarmHelper/Enable".Localize(),
                rowY,
                GetFlukemarmHelperEnabled,
                SetFlukemarmHelperEnabled,
                out flukemarmHelperToggleValue,
                out flukemarmHelperToggleIcon
            );

            lastY = rowY;
            rowY += FlukemarmHelperRowSpacing;
            CreateToggleRow(
                content,
                "FlukemarmP5HpRow",
                "Settings/FlukemarmHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.FlukemarmHelper.flukemarmP5Hp,
                SetFlukemarmP5HpEnabled,
                out flukemarmP5HpValue
            );

            lastY = rowY;
            rowY += FlukemarmHelperRowSpacing;
            CreateToggleRow(
                content,
                "FlukemarmUseMaxHpRow",
                "Settings/FlukemarmHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.FlukemarmHelper.flukemarmUseMaxHp,
                SetFlukemarmUseMaxHpEnabled,
                out flukemarmUseMaxHpValue
            );

            lastY = rowY;
            rowY += FlukemarmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FlukemarmMaxHpRow",
                "Settings/FlukemarmHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.FlukemarmHelper.flukemarmMaxHp,
                SetFlukemarmMaxHp,
                1,
                999999,
                10,
                out flukemarmMaxHpField
            );

            lastY = rowY;
            rowY += FlukemarmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FlukemarmFlyHpRow",
                "Settings/FlukemarmHelper/FlukeFlyHP".Localize(),
                rowY,
                () => Modules.BossChallenge.FlukemarmHelper.flukemarmFlyHp,
                SetFlukemarmFlyHp,
                1,
                999999,
                1,
                out flukemarmFlyHpField
            );

            lastY = rowY;
            rowY += FlukemarmHelperRowSpacing;
            CreateToggleRow(
                content,
                "FlukemarmUseCustomSummonLimitRow",
                "Settings/FlukemarmHelper/UseCustomSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.FlukemarmHelper.flukemarmUseCustomSummonLimit,
                SetFlukemarmUseCustomSummonLimitEnabled,
                out flukemarmUseCustomSummonLimitValue
            );

            lastY = rowY;
            rowY += FlukemarmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "FlukemarmSummonLimitRow",
                "Settings/FlukemarmHelper/SummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.FlukemarmHelper.flukemarmSummonLimit,
                SetFlukemarmSummonLimit,
                6,
                999,
                1,
                out flukemarmSummonLimitField
            );

            lastY = rowY;
            rowY += FlukemarmHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "FlukemarmHelperResetRow", "Settings/FlukemarmHelper/Reset".Localize(), resetY, OnFlukemarmHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "FlukemarmHelperBackRow", "Back", backY, OnFlukemarmHelperBackClicked);
        }
    }
}
