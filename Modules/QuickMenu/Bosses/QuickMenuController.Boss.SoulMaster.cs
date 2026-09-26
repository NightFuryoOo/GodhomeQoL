using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int SoulMasterHelperCanvasSortOrder = 10055;
        private const float SoulMasterHelperPanelHeight = PanelHeight;
        private const float SoulMasterHelperRowSpacing = 44f;
        private RectTransform? soulMasterHelperContent;
        private GameObject? soulMasterHelperRoot;
        private bool soulMasterHelperVisible;
        private Text? soulMasterHelperToggleValue;
        private Image? soulMasterHelperToggleIcon;
        private Text? soulMasterP5HpValue;
        private InputField? soulMasterPhase1HpField;
        private InputField? soulMasterPhase2HpField;
        private Module? soulMasterHelperModule;

        private static void ResetSoulMasterHelperDefaults()
        {
            Modules.BossChallenge.SoulMasterHelper.soulMasterP5Hp = false;
            Modules.BossChallenge.SoulMasterHelper.soulMasterPhase1Hp = 900;
            Modules.BossChallenge.SoulMasterHelper.soulMasterPhase2Hp = 600;
            Modules.BossChallenge.SoulMasterHelper.soulMasterPhase1HpBeforeP5 = 900;
            Modules.BossChallenge.SoulMasterHelper.soulMasterPhase2HpBeforeP5 = 600;
            Modules.BossChallenge.SoulMasterHelper.soulMasterHasStoredStateBeforeP5 = false;
        }

        private Module? GetSoulMasterHelperModule()
        {
            return GetCachedModule(ref soulMasterHelperModule, typeof(Modules.BossChallenge.SoulMasterHelper));
        }

        private bool GetSoulMasterHelperEnabled()
        {
            return GetSoulMasterHelperModule()?.Enabled ?? false;
        }

        private void SetSoulMasterHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetSoulMasterHelperModule(), value);
            UpdateSoulMasterHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetSoulMasterP5HpEnabled(bool value)
        {
            Modules.BossChallenge.SoulMasterHelper.SetP5HpEnabled(value);
            RefreshSoulMasterHelperUi();
        }

        private void SetSoulMasterPhase1Hp(int value)
        {
            if (Modules.BossChallenge.SoulMasterHelper.soulMasterP5Hp)
            {
                Modules.BossChallenge.SoulMasterHelper.soulMasterPhase1Hp = 600;
                RefreshSoulMasterHelperUi();
                return;
            }

            Modules.BossChallenge.SoulMasterHelper.soulMasterPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.SoulMasterHelper.ReapplyLiveSettings();
        }

        private void SetSoulMasterPhase2Hp(int value)
        {
            if (Modules.BossChallenge.SoulMasterHelper.soulMasterP5Hp)
            {
                Modules.BossChallenge.SoulMasterHelper.soulMasterPhase2Hp = 350;
                RefreshSoulMasterHelperUi();
                return;
            }

            Modules.BossChallenge.SoulMasterHelper.soulMasterPhase2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.SoulMasterHelper.ReapplyLiveSettings();
        }

        private void RefreshSoulMasterHelperUi()
        {
            Module? module = GetSoulMasterHelperModule();
            UpdateToggleValue(soulMasterHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(soulMasterHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(soulMasterP5HpValue, Modules.BossChallenge.SoulMasterHelper.soulMasterP5Hp);
            UpdateIntInputValue(soulMasterPhase1HpField, Modules.BossChallenge.SoulMasterHelper.soulMasterPhase1Hp);
            UpdateIntInputValue(soulMasterPhase2HpField, Modules.BossChallenge.SoulMasterHelper.soulMasterPhase2Hp);

            UpdateSoulMasterHelperInteractivity();
        }

        private void UpdateSoulMasterHelperInteractivity()
        {
            SetContentInteractivity(soulMasterHelperContent, GetSoulMasterHelperEnabled(), "SoulMasterHelperEnableRow");
            if (!GetSoulMasterHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.SoulMasterHelper.soulMasterP5Hp;
            SetRowInteractivity(soulMasterHelperContent, "SoulMasterPhase1HpRow", !p5Hp);
            SetRowInteractivity(soulMasterHelperContent, "SoulMasterPhase2HpRow", !p5Hp);
        }

        private void SetSoulMasterHelperVisible(bool value)
        {
            soulMasterHelperVisible = value;
            ApplyPanelVisible(soulMasterHelperRoot, value, RefreshSoulMasterHelperUi);
        }

        private void OnBossManipulateSoulMasterClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetSoulMasterHelperVisible);
        }

        private void OnSoulMasterHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetSoulMasterHelperVisible);
        }

        private void OnSoulMasterHelperResetDefaultsClicked()
        {
            ResetSoulMasterHelperDefaults();
            SetSoulMasterHelperEnabled(false);
            Modules.BossChallenge.SoulMasterHelper.RestoreVanillaHealthIfPresent();
            RefreshSoulMasterHelperUi();
        }

        private void BuildSoulMasterHelperOverlayUi()
        {
            soulMasterHelperRoot = CreateOverlayFrame("SoulMasterHelperOverlayCanvas", SoulMasterHelperCanvasSortOrder, "SoulMasterHelperPanel", SoulMasterHelperPanelHeight, "Modules/SoulMasterHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            RectTransform content = CreateOverlayContent(panel, SoulMasterHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            soulMasterHelperContent = content;

            float rowY = GetRowStartY(SoulMasterHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "SoulMasterHelperEnableRow",
                "Settings/SoulMasterHelper/Enable".Localize(),
                rowY,
                GetSoulMasterHelperEnabled,
                SetSoulMasterHelperEnabled,
                out soulMasterHelperToggleValue,
                out soulMasterHelperToggleIcon
            );

            lastY = rowY;
            rowY += SoulMasterHelperRowSpacing;
            CreateToggleRow(
                content,
                "SoulMasterP5HpRow",
                "Settings/SoulMasterHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulMasterHelper.soulMasterP5Hp,
                SetSoulMasterP5HpEnabled,
                out soulMasterP5HpValue
            );

            lastY = rowY;
            rowY += SoulMasterHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SoulMasterPhase1HpRow",
                "Settings/SoulMasterHelper/Phase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulMasterHelper.soulMasterPhase1Hp,
                SetSoulMasterPhase1Hp,
                1,
                999999,
                10,
                out soulMasterPhase1HpField
            );

            lastY = rowY;
            rowY += SoulMasterHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SoulMasterPhase2HpRow",
                "Settings/SoulMasterHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulMasterHelper.soulMasterPhase2Hp,
                SetSoulMasterPhase2Hp,
                1,
                999999,
                10,
                out soulMasterPhase2HpField
            );

            lastY = rowY;
            rowY += SoulMasterHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "SoulMasterHelperResetRow", "Settings/SoulMasterHelper/Reset".Localize(), resetY, OnSoulMasterHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "SoulMasterHelperBackRow", "Back", backY, OnSoulMasterHelperBackClicked);
        }
    }
}
