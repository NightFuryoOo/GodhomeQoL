using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int NoskHelperCanvasSortOrder = 10065;
        private const float NoskHelperPanelHeight = PanelHeight;
        private const float NoskHelperRowSpacing = 44f;
        private RectTransform? noskHelperContent;
        private GameObject? noskHelperRoot;
        private bool noskHelperVisible;
        private Text? noskHelperToggleValue;
        private Image? noskHelperToggleIcon;
        private Text? noskUseMaxHpValue;
        private Text? noskUseCustomPhaseValue;
        private InputField? noskMaxHpField;
        private InputField? noskPhase2HpField;
        private Module? noskHelperModule;

        private static void ResetNoskHelperDefaults()
        {
            Modules.BossChallenge.NoskHelper.noskUseMaxHp = false;
            Modules.BossChallenge.NoskHelper.noskUseCustomPhase = false;
            Modules.BossChallenge.NoskHelper.noskMaxHp = 980;
            Modules.BossChallenge.NoskHelper.noskPhase2Hp = 560;
        }

        private Module? GetNoskHelperModule()
        {
            return GetCachedModule(ref noskHelperModule, typeof(Modules.BossChallenge.NoskHelper));
        }

        private bool GetNoskHelperEnabled()
        {
            return GetNoskHelperModule()?.Enabled ?? false;
        }

        private void SetNoskHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetNoskHelperModule(), value);
            UpdateNoskHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetNoskUseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.NoskHelper.noskUseMaxHp = value;
            Modules.BossChallenge.NoskHelper.ReapplyLiveSettings();
            RefreshNoskHelperUi();
        }

        private void SetNoskUseCustomPhaseEnabled(bool value)
        {
            int maxPhase2Hp = GetNoskPhase2MaxHp();
            Modules.BossChallenge.NoskHelper.noskPhase2Hp =
                Mathf.Clamp(Modules.BossChallenge.NoskHelper.noskPhase2Hp, 1, maxPhase2Hp);
            Modules.BossChallenge.NoskHelper.noskUseCustomPhase = value;
            Modules.BossChallenge.NoskHelper.ReapplyLiveSettings();
            RefreshNoskHelperUi();
        }

        private void SetNoskMaxHp(int value)
        {
            Modules.BossChallenge.NoskHelper.noskMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.NoskHelper.noskUseMaxHp)
            {
                Modules.BossChallenge.NoskHelper.ApplyNoskHealthIfPresent();
            }

            int maxPhase2Hp = GetNoskPhase2MaxHp();
            Modules.BossChallenge.NoskHelper.noskPhase2Hp =
                Mathf.Clamp(Modules.BossChallenge.NoskHelper.noskPhase2Hp, 1, maxPhase2Hp);
            if (Modules.BossChallenge.NoskHelper.noskUseCustomPhase)
            {
                Modules.BossChallenge.NoskHelper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshNoskHelperUi();
        }

        private void SetNoskPhase2Hp(int value)
        {
            int maxPhase2Hp = GetNoskPhase2MaxHp();
            Modules.BossChallenge.NoskHelper.noskPhase2Hp = Mathf.Clamp(value, 1, maxPhase2Hp);
            if (Modules.BossChallenge.NoskHelper.noskUseCustomPhase)
            {
                Modules.BossChallenge.NoskHelper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshNoskHelperUi();
        }

        private int GetNoskPhase2MaxHp()
        {
            return Mathf.Max(1, Modules.BossChallenge.NoskHelper.GetPhase2MaxHpForUi());
        }

        private void RefreshNoskHelperUi()
        {
            Module? module = GetNoskHelperModule();
            UpdateToggleValue(noskHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(noskHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(noskUseMaxHpValue, Modules.BossChallenge.NoskHelper.noskUseMaxHp);
            UpdateToggleValue(noskUseCustomPhaseValue, Modules.BossChallenge.NoskHelper.noskUseCustomPhase);
            UpdateIntInputValue(noskMaxHpField, Modules.BossChallenge.NoskHelper.noskMaxHp);
            UpdateIntInputValue(noskPhase2HpField, Modules.BossChallenge.NoskHelper.noskPhase2Hp);

            UpdateNoskHelperInteractivity();
        }

        private void UpdateNoskHelperInteractivity()
        {
            SetContentInteractivity(noskHelperContent, GetNoskHelperEnabled(), "NoskHelperEnableRow");
            if (!GetNoskHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.NoskHelper.noskUseMaxHp;
            bool useCustomPhase = Modules.BossChallenge.NoskHelper.noskUseCustomPhase;
            SetRowInteractivity(noskHelperContent, "NoskMaxHpRow", useMaxHp);
            SetRowInteractivity(noskHelperContent, "NoskPhase2HpRow", useCustomPhase);
        }

        private void SetNoskHelperVisible(bool value)
        {
            noskHelperVisible = value;
            ApplyPanelVisible(noskHelperRoot, value, RefreshNoskHelperUi);
        }

        private void OnBossManipulateNoskClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetNoskHelperVisible);
        }

        private void OnNoskHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetNoskHelperVisible);
        }

        private void OnNoskHelperResetDefaultsClicked()
        {
            ResetNoskHelperDefaults();
            SetNoskHelperEnabled(false);
            Modules.BossChallenge.NoskHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.NoskHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshNoskHelperUi();
        }

        private void BuildNoskHelperOverlayUi()
        {
            noskHelperRoot = CreateOverlayFrame("NoskHelperOverlayCanvas", NoskHelperCanvasSortOrder, "NoskHelperPanel", NoskHelperPanelHeight, "Modules/NoskHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = NoskHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, NoskHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            noskHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "NoskHelperEnableRow",
                "Settings/NoskHelper/Enable".Localize(),
                rowY,
                GetNoskHelperEnabled,
                SetNoskHelperEnabled,
                out noskHelperToggleValue,
                out noskHelperToggleIcon
            );

            lastY = rowY;
            rowY += NoskHelperRowSpacing;
            CreateToggleRow(
                content,
                "NoskUseMaxHpRow",
                "Settings/NoskHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.NoskHelper.noskUseMaxHp,
                SetNoskUseMaxHpEnabled,
                out noskUseMaxHpValue
            );

            lastY = rowY;
            rowY += NoskHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoskMaxHpRow",
                "Settings/NoskHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.NoskHelper.noskMaxHp,
                SetNoskMaxHp,
                1,
                999999,
                10,
                out noskMaxHpField
            );

            lastY = rowY;
            rowY += NoskHelperRowSpacing;
            CreateToggleRow(
                content,
                "NoskUseCustomPhaseRow",
                "Settings/NoskHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.NoskHelper.noskUseCustomPhase,
                SetNoskUseCustomPhaseEnabled,
                out noskUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += NoskHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoskPhase2HpRow",
                "Settings/NoskHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NoskHelper.noskPhase2Hp,
                SetNoskPhase2Hp,
                1,
                999999,
                10,
                out noskPhase2HpField
            );

            lastY = rowY;
            rowY += NoskHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "NoskHelperResetRow", "Settings/NoskHelper/Reset".Localize(), resetY, OnNoskHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "NoskHelperBackRow", "Back", backY, OnNoskHelperBackClicked);
        }
    }
}
