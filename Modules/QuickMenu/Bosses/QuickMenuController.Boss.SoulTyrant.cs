using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int SoulTyrantHelperCanvasSortOrder = 10056;
        private const float SoulTyrantHelperPanelHeight = PanelHeight;
        private const float SoulTyrantHelperRowSpacing = 44f;
        private RectTransform? soulTyrantHelperContent;
        private GameObject? soulTyrantHelperRoot;
        private bool soulTyrantHelperVisible;
        private Text? soulTyrantHelperToggleValue;
        private Image? soulTyrantHelperToggleIcon;
        private Text? soulTyrantP5HpValue;
        private InputField? soulTyrantPhase1HpField;
        private InputField? soulTyrantPhase2HpField;
        private Module? soulTyrantHelperModule;

        private static void ResetSoulTyrantHelperDefaults()
        {
            Modules.BossChallenge.SoulTyrantHelper.soulTyrantP5Hp = false;
            Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase1Hp = 1200;
            Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase2Hp = 650;
            Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase1HpBeforeP5 = 1200;
            Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase2HpBeforeP5 = 650;
            Modules.BossChallenge.SoulTyrantHelper.soulTyrantHasStoredStateBeforeP5 = false;
        }

        private Module? GetSoulTyrantHelperModule()
        {
            return GetCachedModule(ref soulTyrantHelperModule, typeof(Modules.BossChallenge.SoulTyrantHelper));
        }

        private bool GetSoulTyrantHelperEnabled()
        {
            return GetSoulTyrantHelperModule()?.Enabled ?? false;
        }

        private void SetSoulTyrantHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetSoulTyrantHelperModule(), value);
            UpdateSoulTyrantHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetSoulTyrantP5HpEnabled(bool value)
        {
            Modules.BossChallenge.SoulTyrantHelper.SetP5HpEnabled(value);
            RefreshSoulTyrantHelperUi();
        }

        private void SetSoulTyrantPhase1Hp(int value)
        {
            if (Modules.BossChallenge.SoulTyrantHelper.soulTyrantP5Hp)
            {
                Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase1Hp = 900;
                RefreshSoulTyrantHelperUi();
                return;
            }

            Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.SoulTyrantHelper.ReapplyLiveSettings();
        }

        private void SetSoulTyrantPhase2Hp(int value)
        {
            if (Modules.BossChallenge.SoulTyrantHelper.soulTyrantP5Hp)
            {
                Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase2Hp = 350;
                RefreshSoulTyrantHelperUi();
                return;
            }

            Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.SoulTyrantHelper.ReapplyLiveSettings();
        }

        private void RefreshSoulTyrantHelperUi()
        {
            Module? module = GetSoulTyrantHelperModule();
            UpdateToggleValue(soulTyrantHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(soulTyrantHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(soulTyrantP5HpValue, Modules.BossChallenge.SoulTyrantHelper.soulTyrantP5Hp);
            UpdateIntInputValue(soulTyrantPhase1HpField, Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase1Hp);
            UpdateIntInputValue(soulTyrantPhase2HpField, Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase2Hp);

            UpdateSoulTyrantHelperInteractivity();
        }

        private void UpdateSoulTyrantHelperInteractivity()
        {
            SetContentInteractivity(soulTyrantHelperContent, GetSoulTyrantHelperEnabled(), "SoulTyrantHelperEnableRow");
            if (!GetSoulTyrantHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.SoulTyrantHelper.soulTyrantP5Hp;
            SetRowInteractivity(soulTyrantHelperContent, "SoulTyrantPhase1HpRow", !p5Hp);
            SetRowInteractivity(soulTyrantHelperContent, "SoulTyrantPhase2HpRow", !p5Hp);
        }

        private void SetSoulTyrantHelperVisible(bool value)
        {
            soulTyrantHelperVisible = value;
            ApplyPanelVisible(soulTyrantHelperRoot, value, RefreshSoulTyrantHelperUi);
        }

        private void OnBossManipulateSoulTyrantClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetSoulTyrantHelperVisible);
        }

        private void OnSoulTyrantHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetSoulTyrantHelperVisible);
        }

        private void OnSoulTyrantHelperResetDefaultsClicked()
        {
            ResetSoulTyrantHelperDefaults();
            SetSoulTyrantHelperEnabled(false);
            Modules.BossChallenge.SoulTyrantHelper.RestoreVanillaHealthIfPresent();
            RefreshSoulTyrantHelperUi();
        }

        private void BuildSoulTyrantHelperOverlayUi()
        {
            soulTyrantHelperRoot = CreateOverlayFrame("SoulTyrantHelperOverlayCanvas", SoulTyrantHelperCanvasSortOrder, "SoulTyrantHelperPanel", SoulTyrantHelperPanelHeight, "Modules/SoulTyrantHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            RectTransform content = CreateOverlayContent(panel, SoulTyrantHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            soulTyrantHelperContent = content;

            float rowY = GetRowStartY(SoulTyrantHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "SoulTyrantHelperEnableRow",
                "Settings/SoulTyrantHelper/Enable".Localize(),
                rowY,
                GetSoulTyrantHelperEnabled,
                SetSoulTyrantHelperEnabled,
                out soulTyrantHelperToggleValue,
                out soulTyrantHelperToggleIcon
            );

            lastY = rowY;
            rowY += SoulTyrantHelperRowSpacing;
            CreateToggleRow(
                content,
                "SoulTyrantP5HpRow",
                "Settings/SoulTyrantHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulTyrantHelper.soulTyrantP5Hp,
                SetSoulTyrantP5HpEnabled,
                out soulTyrantP5HpValue
            );

            lastY = rowY;
            rowY += SoulTyrantHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SoulTyrantPhase1HpRow",
                "Settings/SoulTyrantHelper/Phase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase1Hp,
                SetSoulTyrantPhase1Hp,
                1,
                999999,
                10,
                out soulTyrantPhase1HpField
            );

            lastY = rowY;
            rowY += SoulTyrantHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SoulTyrantPhase2HpRow",
                "Settings/SoulTyrantHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulTyrantHelper.soulTyrantPhase2Hp,
                SetSoulTyrantPhase2Hp,
                1,
                999999,
                10,
                out soulTyrantPhase2HpField
            );

            lastY = rowY;
            rowY += SoulTyrantHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "SoulTyrantHelperResetRow", "Settings/SoulTyrantHelper/Reset".Localize(), resetY, OnSoulTyrantHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "SoulTyrantHelperBackRow", "Back", backY, OnSoulTyrantHelperBackClicked);
        }
    }
}
