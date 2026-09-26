using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int SisterOfBattleHelperCanvasSortOrder = 10063;
        private const float SisterOfBattleHelperPanelHeight = PanelHeight;
        private const float SisterOfBattleHelperRowSpacing = 44f;
        private RectTransform? sisterOfBattleHelperContent;
        private GameObject? sisterOfBattleHelperRoot;
        private bool sisterOfBattleHelperVisible;
        private Text? sisterOfBattleHelperToggleValue;
        private Image? sisterOfBattleHelperToggleIcon;
        private Text? sisterOfBattleP5HpValue;
        private InputField? sisterOfBattlePhase1HpField;
        private InputField? sisterOfBattlePhase2S1HpField;
        private InputField? sisterOfBattlePhase2S2HpField;
        private InputField? sisterOfBattlePhase2S3HpField;
        private Module? sisterOfBattleHelperModule;

        private static void ResetSisterOfBattleHelperDefaults()
        {
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleP5Hp = false;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase1Hp = 600;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S1Hp = 950;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S2Hp = 950;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S3Hp = 950;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase1HpBeforeP5 = 600;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S1HpBeforeP5 = 950;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S2HpBeforeP5 = 950;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S3HpBeforeP5 = 950;
            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleHasStoredStateBeforeP5 = false;
        }

        private Module? GetSisterOfBattleHelperModule()
        {
            return GetCachedModule(ref sisterOfBattleHelperModule, typeof(Modules.BossChallenge.SisterOfBattleHelper));
        }

        private bool GetSisterOfBattleHelperEnabled()
        {
            return GetSisterOfBattleHelperModule()?.Enabled ?? false;
        }

        private void SetSisterOfBattleHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetSisterOfBattleHelperModule(), value);
            UpdateSisterOfBattleHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetSisterOfBattleP5HpEnabled(bool value)
        {
            Modules.BossChallenge.SisterOfBattleHelper.SetP5HpEnabled(value);
            RefreshSisterOfBattleHelperUi();
        }

        private void SetSisterOfBattlePhase1Hp(int value)
        {
            if (Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleP5Hp)
            {
                Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase1Hp = 500;
                RefreshSisterOfBattleHelperUi();
                return;
            }

            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.SisterOfBattleHelper.ReapplyLiveSettings();
        }

        private void SetSisterOfBattlePhase2S1Hp(int value)
        {
            if (Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleP5Hp)
            {
                Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S1Hp = 750;
                RefreshSisterOfBattleHelperUi();
                return;
            }

            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.SisterOfBattleHelper.ReapplyLiveSettings();
        }

        private void SetSisterOfBattlePhase2S2Hp(int value)
        {
            if (Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleP5Hp)
            {
                Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S2Hp = 750;
                RefreshSisterOfBattleHelperUi();
                return;
            }

            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.SisterOfBattleHelper.ReapplyLiveSettings();
        }

        private void SetSisterOfBattlePhase2S3Hp(int value)
        {
            if (Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleP5Hp)
            {
                Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S3Hp = 750;
                RefreshSisterOfBattleHelperUi();
                return;
            }

            Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S3Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.SisterOfBattleHelper.ReapplyLiveSettings();
        }

        private void RefreshSisterOfBattleHelperUi()
        {
            Module? module = GetSisterOfBattleHelperModule();
            UpdateToggleValue(sisterOfBattleHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(sisterOfBattleHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(sisterOfBattleP5HpValue, Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleP5Hp);
            UpdateIntInputValue(sisterOfBattlePhase1HpField, Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase1Hp);
            UpdateIntInputValue(sisterOfBattlePhase2S1HpField, Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S1Hp);
            UpdateIntInputValue(sisterOfBattlePhase2S2HpField, Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S2Hp);
            UpdateIntInputValue(sisterOfBattlePhase2S3HpField, Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S3Hp);

            UpdateSisterOfBattleHelperInteractivity();
        }

        private void UpdateSisterOfBattleHelperInteractivity()
        {
            SetContentInteractivity(sisterOfBattleHelperContent, GetSisterOfBattleHelperEnabled(), "SisterOfBattleHelperEnableRow");
            if (!GetSisterOfBattleHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleP5Hp;
            SetRowInteractivity(sisterOfBattleHelperContent, "SisterOfBattlePhase1HpRow", !p5Hp);
            SetRowInteractivity(sisterOfBattleHelperContent, "SisterOfBattlePhase2S1HpRow", !p5Hp);
            SetRowInteractivity(sisterOfBattleHelperContent, "SisterOfBattlePhase2S2HpRow", !p5Hp);
            SetRowInteractivity(sisterOfBattleHelperContent, "SisterOfBattlePhase2S3HpRow", !p5Hp);
        }

        private void SetSisterOfBattleHelperVisible(bool value)
        {
            sisterOfBattleHelperVisible = value;
            ApplyPanelVisible(sisterOfBattleHelperRoot, value, RefreshSisterOfBattleHelperUi);
        }

        private void OnBossManipulateSisterOfBattleClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetSisterOfBattleHelperVisible);
        }

        private void OnSisterOfBattleHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetSisterOfBattleHelperVisible);
        }

        private void OnSisterOfBattleHelperResetDefaultsClicked()
        {
            ResetSisterOfBattleHelperDefaults();
            SetSisterOfBattleHelperEnabled(false);
            Modules.BossChallenge.SisterOfBattleHelper.RestoreVanillaHealthIfPresent();
            RefreshSisterOfBattleHelperUi();
        }

        private void BuildSisterOfBattleHelperOverlayUi()
        {
            sisterOfBattleHelperRoot = CreateOverlayFrame("SisterOfBattleHelperOverlayCanvas", SisterOfBattleHelperCanvasSortOrder, "SisterOfBattleHelperPanel", SisterOfBattleHelperPanelHeight, "Modules/SisterOfBattleHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            RectTransform content = CreateOverlayContent(panel, SisterOfBattleHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            sisterOfBattleHelperContent = content;

            float rowY = GetRowStartY(SisterOfBattleHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "SisterOfBattleHelperEnableRow",
                "Settings/SisterOfBattleHelper/Enable".Localize(),
                rowY,
                GetSisterOfBattleHelperEnabled,
                SetSisterOfBattleHelperEnabled,
                out sisterOfBattleHelperToggleValue,
                out sisterOfBattleHelperToggleIcon
            );

            lastY = rowY;
            rowY += SisterOfBattleHelperRowSpacing;
            CreateToggleRow(
                content,
                "SisterOfBattleP5HpRow",
                "Settings/SisterOfBattleHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattleP5Hp,
                SetSisterOfBattleP5HpEnabled,
                out sisterOfBattleP5HpValue
            );

            lastY = rowY;
            rowY += SisterOfBattleHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SisterOfBattlePhase1HpRow",
                "Settings/SisterOfBattleHelper/Phase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase1Hp,
                SetSisterOfBattlePhase1Hp,
                1,
                999999,
                10,
                out sisterOfBattlePhase1HpField
            );

            lastY = rowY;
            rowY += SisterOfBattleHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SisterOfBattlePhase2S1HpRow",
                "Settings/SisterOfBattleHelper/Phase2S1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S1Hp,
                SetSisterOfBattlePhase2S1Hp,
                1,
                999999,
                10,
                out sisterOfBattlePhase2S1HpField
            );

            lastY = rowY;
            rowY += SisterOfBattleHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SisterOfBattlePhase2S2HpRow",
                "Settings/SisterOfBattleHelper/Phase2S2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S2Hp,
                SetSisterOfBattlePhase2S2Hp,
                1,
                999999,
                10,
                out sisterOfBattlePhase2S2HpField
            );

            lastY = rowY;
            rowY += SisterOfBattleHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SisterOfBattlePhase2S3HpRow",
                "Settings/SisterOfBattleHelper/Phase2S3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SisterOfBattleHelper.sisterOfBattlePhase2S3Hp,
                SetSisterOfBattlePhase2S3Hp,
                1,
                999999,
                10,
                out sisterOfBattlePhase2S3HpField
            );

            lastY = rowY;
            rowY += SisterOfBattleHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "SisterOfBattleHelperResetRow", "Settings/SisterOfBattleHelper/Reset".Localize(), resetY, OnSisterOfBattleHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "SisterOfBattleHelperBackRow", "Back", backY, OnSisterOfBattleHelperBackClicked);
        }
    }
}
