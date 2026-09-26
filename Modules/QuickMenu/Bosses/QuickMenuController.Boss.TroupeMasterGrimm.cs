using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int TroupeMasterGrimmHelperCanvasSortOrder = 10046;
        private const float TroupeMasterGrimmHelperPanelHeight = PanelHeight;
        private const float TroupeMasterGrimmHelperRowSpacing = 44f;
        private RectTransform? troupeMasterGrimmHelperContent;
        private GameObject? troupeMasterGrimmHelperRoot;
        private bool troupeMasterGrimmHelperVisible;
        private Text? troupeMasterGrimmHelperToggleValue;
        private Image? troupeMasterGrimmHelperToggleIcon;
        private Text? troupeMasterGrimmP5HpValue;
        private Text? troupeMasterGrimmUseMaxHpValue;
        private Text? troupeMasterGrimmUseCustomPhaseValue;
        private InputField? troupeMasterGrimmMaxHpField;
        private InputField? troupeMasterGrimmPhase2HpField;
        private InputField? troupeMasterGrimmPhase3HpField;
        private InputField? troupeMasterGrimmPhase4HpField;
        private Module? troupeMasterGrimmHelperModule;

        private static void ResetTroupeMasterGrimmHelperDefaults()
        {
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseMaxHp = false;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp = false;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase = false;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmMaxHp = 1000;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp = 750;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp = 500;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp = 250;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmMaxHpBeforeP5 = 1000;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2HpBeforeP5 = 750;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3HpBeforeP5 = 500;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4HpBeforeP5 = 250;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmHasStoredStateBeforeP5 = false;
        }

        private Module? GetTroupeMasterGrimmHelperModule()
        {
            return GetCachedModule(ref troupeMasterGrimmHelperModule, typeof(Modules.BossChallenge.TroupeMasterGrimmHelper));
        }

        private bool GetTroupeMasterGrimmHelperEnabled()
        {
            return GetTroupeMasterGrimmHelperModule()?.Enabled ?? false;
        }

        private void SetTroupeMasterGrimmHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetTroupeMasterGrimmHelperModule(), value);
            UpdateTroupeMasterGrimmHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetTroupeMasterGrimmUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp)
            {
                Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseMaxHp = true;
                RefreshTroupeMasterGrimmHelperUi();
                return;
            }

            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseMaxHp = value;
            Modules.BossChallenge.TroupeMasterGrimmHelper.ReapplyLiveSettings();
            RefreshTroupeMasterGrimmHelperUi();
        }

        private void SetTroupeMasterGrimmUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp)
            {
                Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase = false;
                RefreshTroupeMasterGrimmHelperUi();
                return;
            }

            NormalizeTroupeMasterGrimmPhaseThresholds();
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase = value;
            Modules.BossChallenge.TroupeMasterGrimmHelper.ReapplyLiveSettings();
            RefreshTroupeMasterGrimmHelperUi();
        }

        private void SetTroupeMasterGrimmP5HpEnabled(bool value)
        {
            Modules.BossChallenge.TroupeMasterGrimmHelper.SetP5HpEnabled(value);
            RefreshTroupeMasterGrimmHelperUi();
        }

        private void SetTroupeMasterGrimmMaxHp(int value)
        {
            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp)
            {
                Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmMaxHp = 1000;
                RefreshTroupeMasterGrimmHelperUi();
                return;
            }

            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmMaxHp = Mathf.Clamp(value, 1, 999999);
            NormalizeTroupeMasterGrimmPhaseThresholds();
            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseMaxHp
                || Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase)
            {
                Modules.BossChallenge.TroupeMasterGrimmHelper.ReapplyLiveSettings();
            }

            RefreshTroupeMasterGrimmHelperUi();
        }

        private void SetTroupeMasterGrimmPhase2Hp(int value)
        {
            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp)
            {
                RefreshTroupeMasterGrimmHelperUi();
                return;
            }

            int phase2Max = GetTroupeMasterGrimmPhase2Max();
            int phase2Hp = Mathf.Clamp(value, 1, phase2Max);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(1, phase2Hp - 1);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp = Mathf.Clamp(
                Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp,
                1,
                phase3Max);

            int phase4Max = Mathf.Max(1, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp - 1);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp = Mathf.Clamp(
                Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp,
                1,
                phase4Max);

            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase)
            {
                Modules.BossChallenge.TroupeMasterGrimmHelper.ReapplyLiveSettings();
            }

            RefreshTroupeMasterGrimmHelperUi();
        }

        private void SetTroupeMasterGrimmPhase3Hp(int value)
        {
            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp)
            {
                RefreshTroupeMasterGrimmHelperUi();
                return;
            }

            int phase2Max = GetTroupeMasterGrimmPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp, 1, phase2Max);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(1, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(value, 1, phase3Max);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp = phase3Hp;

            int phase4Max = Mathf.Max(1, phase3Hp - 1);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp = Mathf.Clamp(
                Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp,
                1,
                phase4Max);

            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase)
            {
                Modules.BossChallenge.TroupeMasterGrimmHelper.ReapplyLiveSettings();
            }

            RefreshTroupeMasterGrimmHelperUi();
        }

        private void SetTroupeMasterGrimmPhase4Hp(int value)
        {
            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp)
            {
                RefreshTroupeMasterGrimmHelperUi();
                return;
            }

            int phase2Max = GetTroupeMasterGrimmPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp, 1, phase2Max);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(1, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp, 1, phase3Max);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp = phase3Hp;

            int phase4Max = Mathf.Max(1, phase3Hp - 1);
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp = Mathf.Clamp(value, 1, phase4Max);

            if (Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase)
            {
                Modules.BossChallenge.TroupeMasterGrimmHelper.ReapplyLiveSettings();
            }

            RefreshTroupeMasterGrimmHelperUi();
        }

        private int GetTroupeMasterGrimmPhase2Max()
        {
            return Mathf.Max(1, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmMaxHp);
        }

        private void NormalizeTroupeMasterGrimmPhaseThresholds()
        {
            int phase2Max = GetTroupeMasterGrimmPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp, 1, phase2Max);
            int phase3Max = Mathf.Max(1, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp, 1, phase3Max);
            int phase4Max = Mathf.Max(1, phase3Hp - 1);
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp, 1, phase4Max);

            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp = phase2Hp;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp = phase3Hp;
            Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp = phase4Hp;
        }

        private void RefreshTroupeMasterGrimmHelperUi()
        {
            Module? module = GetTroupeMasterGrimmHelperModule();
            UpdateToggleValue(troupeMasterGrimmHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(troupeMasterGrimmHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(troupeMasterGrimmP5HpValue, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp);
            UpdateToggleValue(troupeMasterGrimmUseMaxHpValue, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseMaxHp);
            UpdateToggleValue(troupeMasterGrimmUseCustomPhaseValue, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase);
            UpdateIntInputValue(troupeMasterGrimmMaxHpField, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmMaxHp);
            UpdateIntInputValue(troupeMasterGrimmPhase2HpField, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp);
            UpdateIntInputValue(troupeMasterGrimmPhase3HpField, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp);
            UpdateIntInputValue(troupeMasterGrimmPhase4HpField, Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp);

            UpdateTroupeMasterGrimmHelperInteractivity();
        }

        private void UpdateTroupeMasterGrimmHelperInteractivity()
        {
            SetContentInteractivity(troupeMasterGrimmHelperContent, GetTroupeMasterGrimmHelperEnabled(), "TroupeMasterGrimmHelperEnableRow");
            if (!GetTroupeMasterGrimmHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseMaxHp;
            bool p5Hp = Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp;
            bool useCustomPhase = Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase;
            SetRowInteractivity(troupeMasterGrimmHelperContent, "TroupeMasterGrimmUseMaxHpRow", !p5Hp);
            SetRowInteractivity(troupeMasterGrimmHelperContent, "TroupeMasterGrimmMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(troupeMasterGrimmHelperContent, "TroupeMasterGrimmUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(troupeMasterGrimmHelperContent, "TroupeMasterGrimmPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(troupeMasterGrimmHelperContent, "TroupeMasterGrimmPhase3HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(troupeMasterGrimmHelperContent, "TroupeMasterGrimmPhase4HpRow", useCustomPhase && !p5Hp);
        }

        private void SetTroupeMasterGrimmHelperVisible(bool value)
        {
            troupeMasterGrimmHelperVisible = value;
            ApplyPanelVisible(troupeMasterGrimmHelperRoot, value, RefreshTroupeMasterGrimmHelperUi);
        }

        private void OnBossManipulateTroupeMasterGrimmClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetTroupeMasterGrimmHelperVisible);
        }

        private void OnTroupeMasterGrimmHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetTroupeMasterGrimmHelperVisible);
        }

        private void OnTroupeMasterGrimmHelperResetDefaultsClicked()
        {
            ResetTroupeMasterGrimmHelperDefaults();
            SetTroupeMasterGrimmHelperEnabled(false);
            Modules.BossChallenge.TroupeMasterGrimmHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.TroupeMasterGrimmHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshTroupeMasterGrimmHelperUi();
        }

        private void BuildTroupeMasterGrimmHelperOverlayUi()
        {
            troupeMasterGrimmHelperRoot = CreateOverlayFrame("TroupeMasterGrimmHelperOverlayCanvas", TroupeMasterGrimmHelperCanvasSortOrder, "TroupeMasterGrimmHelperPanel", TroupeMasterGrimmHelperPanelHeight, "Modules/TroupeMasterGrimmHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = TroupeMasterGrimmHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, TroupeMasterGrimmHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            troupeMasterGrimmHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "TroupeMasterGrimmHelperEnableRow",
                "Settings/TroupeMasterGrimmHelper/Enable".Localize(),
                rowY,
                GetTroupeMasterGrimmHelperEnabled,
                SetTroupeMasterGrimmHelperEnabled,
                out troupeMasterGrimmHelperToggleValue,
                out troupeMasterGrimmHelperToggleIcon
            );

            lastY = rowY;
            rowY += TroupeMasterGrimmHelperRowSpacing;
            CreateToggleRow(
                content,
                "TroupeMasterGrimmP5HpRow",
                "Settings/TroupeMasterGrimmHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmP5Hp,
                SetTroupeMasterGrimmP5HpEnabled,
                out troupeMasterGrimmP5HpValue
            );

            lastY = rowY;
            rowY += TroupeMasterGrimmHelperRowSpacing;
            CreateToggleRow(
                content,
                "TroupeMasterGrimmUseMaxHpRow",
                "Settings/TroupeMasterGrimmHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseMaxHp,
                SetTroupeMasterGrimmUseMaxHpEnabled,
                out troupeMasterGrimmUseMaxHpValue
            );

            lastY = rowY;
            rowY += TroupeMasterGrimmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "TroupeMasterGrimmMaxHpRow",
                "Settings/TroupeMasterGrimmHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmMaxHp,
                SetTroupeMasterGrimmMaxHp,
                1,
                999999,
                10,
                out troupeMasterGrimmMaxHpField
            );

            lastY = rowY;
            rowY += TroupeMasterGrimmHelperRowSpacing;
            CreateToggleRow(
                content,
                "TroupeMasterGrimmUseCustomPhaseRow",
                "Settings/TroupeMasterGrimmHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmUseCustomPhase,
                SetTroupeMasterGrimmUseCustomPhaseEnabled,
                out troupeMasterGrimmUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += TroupeMasterGrimmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "TroupeMasterGrimmPhase2HpRow",
                "Settings/TroupeMasterGrimmHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase2Hp,
                SetTroupeMasterGrimmPhase2Hp,
                1,
                999999,
                10,
                out troupeMasterGrimmPhase2HpField
            );

            lastY = rowY;
            rowY += TroupeMasterGrimmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "TroupeMasterGrimmPhase3HpRow",
                "Settings/TroupeMasterGrimmHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase3Hp,
                SetTroupeMasterGrimmPhase3Hp,
                1,
                999998,
                10,
                out troupeMasterGrimmPhase3HpField
            );

            lastY = rowY;
            rowY += TroupeMasterGrimmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "TroupeMasterGrimmPhase4HpRow",
                "Settings/TroupeMasterGrimmHelper/Phase4HP".Localize(),
                rowY,
                () => Modules.BossChallenge.TroupeMasterGrimmHelper.troupeMasterGrimmPhase4Hp,
                SetTroupeMasterGrimmPhase4Hp,
                1,
                999997,
                10,
                out troupeMasterGrimmPhase4HpField
            );

            lastY = rowY;
            rowY += TroupeMasterGrimmHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "TroupeMasterGrimmHelperResetRow", "Settings/TroupeMasterGrimmHelper/Reset".Localize(), resetY, OnTroupeMasterGrimmHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "TroupeMasterGrimmHelperBackRow", "Back", backY, OnTroupeMasterGrimmHelperBackClicked);
        }
    }
}
