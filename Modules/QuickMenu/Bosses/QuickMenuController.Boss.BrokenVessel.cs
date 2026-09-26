using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int BrokenVesselHelperCanvasSortOrder = 10041;
        private const float BrokenVesselHelperPanelHeight = PanelHeight;
        private const float BrokenVesselHelperRowSpacing = 44f;
        private RectTransform? brokenVesselHelperContent;
        private GameObject? brokenVesselHelperRoot;
        private bool brokenVesselHelperVisible;
        private Text? brokenVesselHelperToggleValue;
        private Image? brokenVesselHelperToggleIcon;
        private Text? brokenVesselP5HpValue;
        private Text? brokenVesselUseMaxHpValue;
        private Text? brokenVesselUseCustomPhaseValue;
        private Text? brokenVesselUseCustomSummonHpValue;
        private Text? brokenVesselUseCustomSummonLimitValue;
        private InputField? brokenVesselMaxHpField;
        private InputField? brokenVesselPhase2HpField;
        private InputField? brokenVesselPhase3HpField;
        private InputField? brokenVesselPhase4HpField;
        private InputField? brokenVesselPhase5HpField;
        private InputField? brokenVesselSummonHpField;
        private InputField? brokenVesselSummonLimitField;
        private Module? brokenVesselHelperModule;

        private static void ResetBrokenVesselHelperDefaults()
        {
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseMaxHp = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselMaxHp = 1000;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonHp = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonHp = 1;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonLimit = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonLimit = 3;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp = 420;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp = 370;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp = 220;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp = 110;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselMaxHpBeforeP5 = 1000;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonHpBeforeP5 = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonHpBeforeP5 = 1;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonLimitBeforeP5 = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonLimitBeforeP5 = 3;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2HpBeforeP5 = 420;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3HpBeforeP5 = 370;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4HpBeforeP5 = 220;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5HpBeforeP5 = 110;
        }

        private Module? GetBrokenVesselHelperModule()
        {
            return GetCachedModule(ref brokenVesselHelperModule, typeof(Modules.BossChallenge.BrokenVesselHelper));
        }

        private bool GetBrokenVesselHelperEnabled()
        {
            return GetBrokenVesselHelperModule()?.Enabled ?? false;
        }

        private void SetBrokenVesselHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetBrokenVesselHelperModule(), value);
            UpdateBrokenVesselHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetBrokenVesselUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseMaxHp = true;
                RefreshBrokenVesselHelperUi();
                return;
            }

            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseMaxHp = value;
            Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase = false;
                RefreshBrokenVesselHelperUi();
                return;
            }

            NormalizeBrokenVesselPhaseThresholds();
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase = value;
            Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselUseCustomSummonHpEnabled(bool value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonHp = false;
                RefreshBrokenVesselHelperUi();
                return;
            }

            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonHp = value;
            Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselUseCustomSummonLimitEnabled(bool value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonLimit = false;
                RefreshBrokenVesselHelperUi();
                return;
            }

            Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonLimit = value;
            Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselP5HpEnabled(bool value)
        {
            Modules.BossChallenge.BrokenVesselHelper.SetP5HpEnabled(value);
            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselMaxHp(int value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselMaxHp = 700;
                RefreshBrokenVesselHelperUi();
                return;
            }

            Modules.BossChallenge.BrokenVesselHelper.brokenVesselMaxHp = Mathf.Clamp(value, 1, 999999);
            NormalizeBrokenVesselPhaseThresholds();
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseMaxHp
                || Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase)
            {
                Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            }

            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselPhase2Hp(int value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                RefreshBrokenVesselHelperUi();
                return;
            }

            int phase2Max = GetBrokenVesselPhase2Max();
            int phase2Hp = Mathf.Clamp(value, 4, phase2Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp = Mathf.Clamp(
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp,
                3,
                phase3Max);

            int phase4Max = Mathf.Max(2, Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp - 1);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp = Mathf.Clamp(
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp,
                2,
                phase4Max);

            int phase5Max = Mathf.Max(1, Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp - 1);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp = Mathf.Clamp(
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp,
                1,
                phase5Max);

            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase)
            {
                Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            }

            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselPhase3Hp(int value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                RefreshBrokenVesselHelperUi();
                return;
            }

            int phase2Max = GetBrokenVesselPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp, 4, phase2Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(value, 3, phase3Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp = phase3Hp;

            int phase4Max = Mathf.Max(2, phase3Hp - 1);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp = Mathf.Clamp(
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp,
                2,
                phase4Max);

            int phase5Max = Mathf.Max(1, Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp - 1);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp = Mathf.Clamp(
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp,
                1,
                phase5Max);

            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase)
            {
                Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            }

            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselPhase4Hp(int value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                RefreshBrokenVesselHelperUi();
                return;
            }

            int phase2Max = GetBrokenVesselPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp, 4, phase2Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp, 3, phase3Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp = phase3Hp;

            int phase4Max = Mathf.Max(2, phase3Hp - 1);
            int phase4Hp = Mathf.Clamp(value, 2, phase4Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp = phase4Hp;

            int phase5Max = Mathf.Max(1, phase4Hp - 1);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp = Mathf.Clamp(
                Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp,
                1,
                phase5Max);

            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase)
            {
                Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            }

            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselPhase5Hp(int value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                RefreshBrokenVesselHelperUi();
                return;
            }

            int phase2Max = GetBrokenVesselPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp, 4, phase2Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp, 3, phase3Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp = phase3Hp;

            int phase4Max = Mathf.Max(2, phase3Hp - 1);
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp, 2, phase4Max);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp = phase4Hp;

            int phase5Max = Mathf.Max(1, phase4Hp - 1);
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp = Mathf.Clamp(value, 1, phase5Max);

            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase)
            {
                Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            }

            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselSummonHp(int value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                RefreshBrokenVesselHelperUi();
                return;
            }

            Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonHp)
            {
                Modules.BossChallenge.BrokenVesselHelper.ApplyBrokenVesselSummonHealthIfPresent();
            }

            RefreshBrokenVesselHelperUi();
        }

        private void SetBrokenVesselSummonLimit(int value)
        {
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp)
            {
                RefreshBrokenVesselHelperUi();
                return;
            }

            Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonLimit = Mathf.Clamp(value, 0, 999);
            if (Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonLimit)
            {
                Modules.BossChallenge.BrokenVesselHelper.ReapplyLiveSettings();
            }

            RefreshBrokenVesselHelperUi();
        }

        private int GetBrokenVesselPhase2Max()
        {
            return Mathf.Max(4, Modules.BossChallenge.BrokenVesselHelper.brokenVesselMaxHp);
        }

        private void NormalizeBrokenVesselPhaseThresholds()
        {
            int phase2Max = GetBrokenVesselPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp, 4, phase2Max);
            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp, 3, phase3Max);
            int phase4Max = Mathf.Max(2, phase3Hp - 1);
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp, 2, phase4Max);
            int phase5Max = Mathf.Max(1, phase4Hp - 1);
            int phase5Hp = Mathf.Clamp(Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp, 1, phase5Max);

            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp = phase2Hp;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp = phase3Hp;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp = phase4Hp;
            Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp = phase5Hp;
        }

        private void RefreshBrokenVesselHelperUi()
        {
            Module? module = GetBrokenVesselHelperModule();
            UpdateToggleValue(brokenVesselHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(brokenVesselHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(brokenVesselP5HpValue, Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp);
            UpdateToggleValue(brokenVesselUseMaxHpValue, Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseMaxHp);
            UpdateToggleValue(brokenVesselUseCustomPhaseValue, Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase);
            UpdateToggleValue(brokenVesselUseCustomSummonHpValue, Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonHp);
            UpdateToggleValue(brokenVesselUseCustomSummonLimitValue, Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonLimit);
            UpdateIntInputValue(brokenVesselMaxHpField, Modules.BossChallenge.BrokenVesselHelper.brokenVesselMaxHp);
            UpdateIntInputValue(brokenVesselPhase2HpField, Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp);
            UpdateIntInputValue(brokenVesselPhase3HpField, Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp);
            UpdateIntInputValue(brokenVesselPhase4HpField, Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp);
            UpdateIntInputValue(brokenVesselPhase5HpField, Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp);
            UpdateIntInputValue(brokenVesselSummonHpField, Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonHp);
            UpdateIntInputValue(brokenVesselSummonLimitField, Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonLimit);

            UpdateBrokenVesselHelperInteractivity();
        }

        private void UpdateBrokenVesselHelperInteractivity()
        {
            SetContentInteractivity(brokenVesselHelperContent, GetBrokenVesselHelperEnabled(), "BrokenVesselHelperEnableRow");
            if (!GetBrokenVesselHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseMaxHp;
            bool p5Hp = Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp;
            bool useCustomPhase = Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase;
            bool useCustomSummonHp = Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonHp;
            bool useCustomSummonLimit = Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonLimit;
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselUseMaxHpRow", !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselPhase3HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselPhase4HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselPhase5HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselUseCustomSummonHpRow", !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselSummonHpRow", useCustomSummonHp && !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselUseCustomSummonLimitRow", !p5Hp);
            SetRowInteractivity(brokenVesselHelperContent, "BrokenVesselSummonLimitRow", useCustomSummonLimit && !p5Hp);
        }

        private void SetBrokenVesselHelperVisible(bool value)
        {
            brokenVesselHelperVisible = value;
            ApplyPanelVisible(brokenVesselHelperRoot, value, RefreshBrokenVesselHelperUi);
        }

        private void OnBossManipulateBrokenVesselClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetBrokenVesselHelperVisible);
        }

        private void OnBrokenVesselHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetBrokenVesselHelperVisible);
        }

        private void OnBrokenVesselHelperResetDefaultsClicked()
        {
            ResetBrokenVesselHelperDefaults();
            SetBrokenVesselHelperEnabled(false);
            Modules.BossChallenge.BrokenVesselHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.BrokenVesselHelper.RestoreVanillaSummonHealthIfPresent();
            Modules.BossChallenge.BrokenVesselHelper.RestoreVanillaSummonLimitsIfPresent();
            Modules.BossChallenge.BrokenVesselHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshBrokenVesselHelperUi();
        }

        private void BuildBrokenVesselHelperOverlayUi()
        {
            brokenVesselHelperRoot = CreateOverlayFrame("BrokenVesselHelperOverlayCanvas", BrokenVesselHelperCanvasSortOrder, "BrokenVesselHelperPanel", BrokenVesselHelperPanelHeight, "Modules/BrokenVesselHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = BrokenVesselHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, BrokenVesselHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            brokenVesselHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "BrokenVesselHelperEnableRow",
                "Settings/BrokenVesselHelper/Enable".Localize(),
                rowY,
                GetBrokenVesselHelperEnabled,
                SetBrokenVesselHelperEnabled,
                out brokenVesselHelperToggleValue,
                out brokenVesselHelperToggleIcon
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateToggleRow(
                content,
                "BrokenVesselP5HpRow",
                "Settings/BrokenVesselHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselP5Hp,
                SetBrokenVesselP5HpEnabled,
                out brokenVesselP5HpValue
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateToggleRow(
                content,
                "BrokenVesselUseMaxHpRow",
                "Settings/BrokenVesselHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseMaxHp,
                SetBrokenVesselUseMaxHpEnabled,
                out brokenVesselUseMaxHpValue
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "BrokenVesselMaxHpRow",
                "Settings/BrokenVesselHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselMaxHp,
                SetBrokenVesselMaxHp,
                1,
                999999,
                10,
                out brokenVesselMaxHpField
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateToggleRow(
                content,
                "BrokenVesselUseCustomPhaseRow",
                "Settings/BrokenVesselHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomPhase,
                SetBrokenVesselUseCustomPhaseEnabled,
                out brokenVesselUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "BrokenVesselPhase2HpRow",
                "Settings/BrokenVesselHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase2Hp,
                SetBrokenVesselPhase2Hp,
                4,
                999999,
                10,
                out brokenVesselPhase2HpField
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "BrokenVesselPhase3HpRow",
                "Settings/BrokenVesselHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase3Hp,
                SetBrokenVesselPhase3Hp,
                3,
                999998,
                10,
                out brokenVesselPhase3HpField
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "BrokenVesselPhase4HpRow",
                "Settings/BrokenVesselHelper/Phase4HP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase4Hp,
                SetBrokenVesselPhase4Hp,
                2,
                999997,
                10,
                out brokenVesselPhase4HpField
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "BrokenVesselPhase5HpRow",
                "Settings/BrokenVesselHelper/Phase5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselPhase5Hp,
                SetBrokenVesselPhase5Hp,
                1,
                999996,
                10,
                out brokenVesselPhase5HpField
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateToggleRow(
                content,
                "BrokenVesselUseCustomSummonHpRow",
                "Settings/BrokenVesselHelper/UseCustomSummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonHp,
                SetBrokenVesselUseCustomSummonHpEnabled,
                out brokenVesselUseCustomSummonHpValue
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "BrokenVesselSummonHpRow",
                "Settings/BrokenVesselHelper/SummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonHp,
                SetBrokenVesselSummonHp,
                1,
                999999,
                10,
                out brokenVesselSummonHpField
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateToggleRow(
                content,
                "BrokenVesselUseCustomSummonLimitRow",
                "Settings/BrokenVesselHelper/UseCustomSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselUseCustomSummonLimit,
                SetBrokenVesselUseCustomSummonLimitEnabled,
                out brokenVesselUseCustomSummonLimitValue
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "BrokenVesselSummonLimitRow",
                "Settings/BrokenVesselHelper/SummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.BrokenVesselHelper.brokenVesselSummonLimit,
                SetBrokenVesselSummonLimit,
                0,
                999,
                1,
                out brokenVesselSummonLimitField
            );

            lastY = rowY;
            rowY += BrokenVesselHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "BrokenVesselHelperResetRow", "Settings/BrokenVesselHelper/Reset".Localize(), resetY, OnBrokenVesselHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "BrokenVesselHelperBackRow", "Back", backY, OnBrokenVesselHelperBackClicked);
        }
    }
}
