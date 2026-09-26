using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int NailsageSlyHelperCanvasSortOrder = 10052;
        private const float NailsageSlyHelperPanelHeight = PanelHeight;
        private const float NailsageSlyHelperRowSpacing = 44f;
        private RectTransform? nailsageSlyHelperContent;
        private GameObject? nailsageSlyHelperRoot;
        private bool nailsageSlyHelperVisible;
        private Text? nailsageSlyHelperToggleValue;
        private Image? nailsageSlyHelperToggleIcon;
        private Text? nailsageSlyP5HpValue;
        private InputField? nailsageSlyPhase1HpField;
        private InputField? nailsageSlyPhase2HpField;
        private Module? nailsageSlyHelperModule;

        private static void ResetNailsageSlyHelperDefaults()
        {
            Modules.BossChallenge.NailsageSlyHelper.nailsageSlyP5Hp = false;
            Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase1Hp = 1200;
            Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase2Hp = 600;
            Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase1HpBeforeP5 = 1200;
            Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase2HpBeforeP5 = 600;
            Modules.BossChallenge.NailsageSlyHelper.nailsageSlyHasStoredStateBeforeP5 = false;
        }

        private Module? GetNailsageSlyHelperModule()
        {
            return GetCachedModule(ref nailsageSlyHelperModule, typeof(Modules.BossChallenge.NailsageSlyHelper));
        }

        private bool GetNailsageSlyHelperEnabled()
        {
            return GetNailsageSlyHelperModule()?.Enabled ?? false;
        }

        private void SetNailsageSlyHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetNailsageSlyHelperModule(), value);
            UpdateNailsageSlyHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetNailsageSlyP5HpEnabled(bool value)
        {
            Modules.BossChallenge.NailsageSlyHelper.SetP5HpEnabled(value);
            RefreshNailsageSlyHelperUi();
        }

        private void SetNailsageSlyPhase1Hp(int value)
        {
            if (Modules.BossChallenge.NailsageSlyHelper.nailsageSlyP5Hp)
            {
                Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase1Hp = 800;
                RefreshNailsageSlyHelperUi();
                return;
            }

            Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.NailsageSlyHelper.ReapplyLiveSettings();
        }

        private void SetNailsageSlyPhase2Hp(int value)
        {
            if (Modules.BossChallenge.NailsageSlyHelper.nailsageSlyP5Hp)
            {
                Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase2Hp = 250;
                RefreshNailsageSlyHelperUi();
                return;
            }

            Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.NailsageSlyHelper.ReapplyLiveSettings();
        }

        private void RefreshNailsageSlyHelperUi()
        {
            Module? module = GetNailsageSlyHelperModule();
            UpdateToggleValue(nailsageSlyHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(nailsageSlyHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(nailsageSlyP5HpValue, Modules.BossChallenge.NailsageSlyHelper.nailsageSlyP5Hp);
            UpdateIntInputValue(nailsageSlyPhase1HpField, Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase1Hp);
            UpdateIntInputValue(nailsageSlyPhase2HpField, Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase2Hp);

            UpdateNailsageSlyHelperInteractivity();
        }

        private void UpdateNailsageSlyHelperInteractivity()
        {
            SetContentInteractivity(nailsageSlyHelperContent, GetNailsageSlyHelperEnabled(), "NailsageSlyHelperEnableRow");
            if (!GetNailsageSlyHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.NailsageSlyHelper.nailsageSlyP5Hp;
            SetRowInteractivity(nailsageSlyHelperContent, "NailsageSlyPhase1HpRow", !p5Hp);
            SetRowInteractivity(nailsageSlyHelperContent, "NailsageSlyPhase2HpRow", !p5Hp);
        }

        private void SetNailsageSlyHelperVisible(bool value)
        {
            nailsageSlyHelperVisible = value;
            ApplyPanelVisible(nailsageSlyHelperRoot, value, RefreshNailsageSlyHelperUi);
        }

        private void OnBossManipulateNailsageSlyClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetNailsageSlyHelperVisible);
        }

        private void OnNailsageSlyHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetNailsageSlyHelperVisible);
        }

        private void OnNailsageSlyHelperResetDefaultsClicked()
        {
            ResetNailsageSlyHelperDefaults();
            SetNailsageSlyHelperEnabled(false);
            Modules.BossChallenge.NailsageSlyHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.NailsageSlyHelper.RestoreVanillaPhase2HpIfPresent();
            RefreshNailsageSlyHelperUi();
        }

        private void BuildNailsageSlyHelperOverlayUi()
        {
            nailsageSlyHelperRoot = CreateOverlayFrame("NailsageSlyHelperOverlayCanvas", NailsageSlyHelperCanvasSortOrder, "NailsageSlyHelperPanel", NailsageSlyHelperPanelHeight, "Modules/NailsageSlyHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            RectTransform content = CreateOverlayContent(panel, NailsageSlyHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            nailsageSlyHelperContent = content;

            float rowY = GetRowStartY(NailsageSlyHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "NailsageSlyHelperEnableRow",
                "Settings/NailsageSlyHelper/Enable".Localize(),
                rowY,
                GetNailsageSlyHelperEnabled,
                SetNailsageSlyHelperEnabled,
                out nailsageSlyHelperToggleValue,
                out nailsageSlyHelperToggleIcon
            );

            lastY = rowY;
            rowY += NailsageSlyHelperRowSpacing;
            CreateToggleRow(
                content,
                "NailsageSlyP5HpRow",
                "Settings/NailsageSlyHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NailsageSlyHelper.nailsageSlyP5Hp,
                SetNailsageSlyP5HpEnabled,
                out nailsageSlyP5HpValue
            );

            lastY = rowY;
            rowY += NailsageSlyHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NailsageSlyPhase1HpRow",
                "Settings/NailsageSlyHelper/Phase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase1Hp,
                SetNailsageSlyPhase1Hp,
                1,
                999999,
                10,
                out nailsageSlyPhase1HpField
            );

            lastY = rowY;
            rowY += NailsageSlyHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NailsageSlyPhase2HpRow",
                "Settings/NailsageSlyHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NailsageSlyHelper.nailsageSlyPhase2Hp,
                SetNailsageSlyPhase2Hp,
                1,
                999999,
                10,
                out nailsageSlyPhase2HpField
            );

            lastY = rowY;
            rowY += NailsageSlyHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "NailsageSlyHelperResetRow", "Settings/NailsageSlyHelper/Reset".Localize(), resetY, OnNailsageSlyHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "NailsageSlyHelperBackRow", "Back", backY, OnNailsageSlyHelperBackClicked);
        }
    }
}
