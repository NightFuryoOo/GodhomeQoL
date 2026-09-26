using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int LostKinHelperCanvasSortOrder = 10042;
        private const float LostKinHelperPanelHeight = PanelHeight;
        private const float LostKinHelperRowSpacing = 44f;
        private RectTransform? lostKinHelperContent;
        private GameObject? lostKinHelperRoot;
        private bool lostKinHelperVisible;
        private Text? lostKinHelperToggleValue;
        private Image? lostKinHelperToggleIcon;
        private Text? lostKinP5HpValue;
        private Text? lostKinUseMaxHpValue;
        private Text? lostKinUseCustomPhaseValue;
        private Text? lostKinUseCustomSummonHpValue;
        private Text? lostKinUseCustomSummonLimitValue;
        private InputField? lostKinMaxHpField;
        private InputField? lostKinPhase2HpField;
        private InputField? lostKinPhase3HpField;
        private InputField? lostKinPhase4HpField;
        private InputField? lostKinPhase5HpField;
        private InputField? lostKinSummonHpField;
        private InputField? lostKinSummonLimitField;
        private Module? lostKinHelperModule;

        private static void ResetLostKinHelperDefaults()
        {
            Modules.BossChallenge.LostKinHelper.lostKinUseMaxHp = false;
            Modules.BossChallenge.LostKinHelper.lostKinP5Hp = false;
            Modules.BossChallenge.LostKinHelper.lostKinMaxHp = 1650;
            Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonHp = false;
            Modules.BossChallenge.LostKinHelper.lostKinSummonHp = 1;
            Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonLimit = false;
            Modules.BossChallenge.LostKinHelper.lostKinSummonLimit = 5;
            Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase = false;
            Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp = 1150;
            Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp = 550;
            Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp = 350;
            Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp = 175;
            Modules.BossChallenge.LostKinHelper.lostKinMaxHpBeforeP5 = 1650;
            Modules.BossChallenge.LostKinHelper.lostKinUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.LostKinHelper.lostKinHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonHpBeforeP5 = false;
            Modules.BossChallenge.LostKinHelper.lostKinSummonHpBeforeP5 = 1;
            Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonLimitBeforeP5 = false;
            Modules.BossChallenge.LostKinHelper.lostKinSummonLimitBeforeP5 = 5;
            Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.LostKinHelper.lostKinPhase2HpBeforeP5 = 1150;
            Modules.BossChallenge.LostKinHelper.lostKinPhase3HpBeforeP5 = 550;
            Modules.BossChallenge.LostKinHelper.lostKinPhase4HpBeforeP5 = 350;
            Modules.BossChallenge.LostKinHelper.lostKinPhase5HpBeforeP5 = 175;
        }

        private Module? GetLostKinHelperModule()
        {
            return GetCachedModule(ref lostKinHelperModule, typeof(Modules.BossChallenge.LostKinHelper));
        }

        private bool GetLostKinHelperEnabled()
        {
            return GetLostKinHelperModule()?.Enabled ?? false;
        }

        private void SetLostKinHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetLostKinHelperModule(), value);
            UpdateLostKinHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetLostKinUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                Modules.BossChallenge.LostKinHelper.lostKinUseMaxHp = true;
                RefreshLostKinHelperUi();
                return;
            }

            Modules.BossChallenge.LostKinHelper.lostKinUseMaxHp = value;
            Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            RefreshLostKinHelperUi();
        }

        private void SetLostKinUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase = false;
                RefreshLostKinHelperUi();
                return;
            }

            NormalizeLostKinPhaseThresholds();
            Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase = value;
            Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            RefreshLostKinHelperUi();
        }

        private void SetLostKinUseCustomSummonHpEnabled(bool value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonHp = false;
                RefreshLostKinHelperUi();
                return;
            }

            Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonHp = value;
            Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            RefreshLostKinHelperUi();
        }

        private void SetLostKinUseCustomSummonLimitEnabled(bool value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonLimit = false;
                RefreshLostKinHelperUi();
                return;
            }

            Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonLimit = value;
            Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            RefreshLostKinHelperUi();
        }

        private void SetLostKinP5HpEnabled(bool value)
        {
            Modules.BossChallenge.LostKinHelper.SetP5HpEnabled(value);
            RefreshLostKinHelperUi();
        }

        private void SetLostKinMaxHp(int value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                Modules.BossChallenge.LostKinHelper.lostKinMaxHp = 1200;
                RefreshLostKinHelperUi();
                return;
            }

            Modules.BossChallenge.LostKinHelper.lostKinMaxHp = Mathf.Clamp(value, 1, 999999);
            NormalizeLostKinPhaseThresholds();
            if (Modules.BossChallenge.LostKinHelper.lostKinUseMaxHp
                || Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase)
            {
                Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            }

            RefreshLostKinHelperUi();
        }

        private void SetLostKinPhase2Hp(int value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                RefreshLostKinHelperUi();
                return;
            }

            int phase2Max = GetLostKinPhase2Max();
            int phase2Hp = Mathf.Clamp(value, 4, phase2Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp = Mathf.Clamp(
                Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp,
                3,
                phase3Max);

            int phase4Max = Mathf.Max(2, Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp - 1);
            Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp = Mathf.Clamp(
                Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp,
                2,
                phase4Max);

            int phase5Max = Mathf.Max(1, Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp - 1);
            Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp = Mathf.Clamp(
                Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp,
                1,
                phase5Max);

            if (Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase)
            {
                Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            }

            RefreshLostKinHelperUi();
        }

        private void SetLostKinPhase3Hp(int value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                RefreshLostKinHelperUi();
                return;
            }

            int phase2Max = GetLostKinPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp, 4, phase2Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(value, 3, phase3Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp = phase3Hp;

            int phase4Max = Mathf.Max(2, phase3Hp - 1);
            Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp = Mathf.Clamp(
                Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp,
                2,
                phase4Max);

            int phase5Max = Mathf.Max(1, Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp - 1);
            Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp = Mathf.Clamp(
                Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp,
                1,
                phase5Max);

            if (Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase)
            {
                Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            }

            RefreshLostKinHelperUi();
        }

        private void SetLostKinPhase4Hp(int value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                RefreshLostKinHelperUi();
                return;
            }

            int phase2Max = GetLostKinPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp, 4, phase2Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp, 3, phase3Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp = phase3Hp;

            int phase4Max = Mathf.Max(2, phase3Hp - 1);
            int phase4Hp = Mathf.Clamp(value, 2, phase4Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp = phase4Hp;

            int phase5Max = Mathf.Max(1, phase4Hp - 1);
            Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp = Mathf.Clamp(
                Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp,
                1,
                phase5Max);

            if (Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase)
            {
                Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            }

            RefreshLostKinHelperUi();
        }

        private void SetLostKinPhase5Hp(int value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                RefreshLostKinHelperUi();
                return;
            }

            int phase2Max = GetLostKinPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp, 4, phase2Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp = phase2Hp;

            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp, 3, phase3Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp = phase3Hp;

            int phase4Max = Mathf.Max(2, phase3Hp - 1);
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp, 2, phase4Max);
            Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp = phase4Hp;

            int phase5Max = Mathf.Max(1, phase4Hp - 1);
            Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp = Mathf.Clamp(value, 1, phase5Max);

            if (Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase)
            {
                Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            }

            RefreshLostKinHelperUi();
        }

        private void SetLostKinSummonHp(int value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                RefreshLostKinHelperUi();
                return;
            }

            Modules.BossChallenge.LostKinHelper.lostKinSummonHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonHp)
            {
                Modules.BossChallenge.LostKinHelper.ApplyLostKinSummonHealthIfPresent();
            }

            RefreshLostKinHelperUi();
        }

        private void SetLostKinSummonLimit(int value)
        {
            if (Modules.BossChallenge.LostKinHelper.lostKinP5Hp)
            {
                RefreshLostKinHelperUi();
                return;
            }

            Modules.BossChallenge.LostKinHelper.lostKinSummonLimit = Mathf.Clamp(value, 0, 999);
            if (Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonLimit)
            {
                Modules.BossChallenge.LostKinHelper.ReapplyLiveSettings();
            }

            RefreshLostKinHelperUi();
        }

        private int GetLostKinPhase2Max()
        {
            return Mathf.Max(4, Modules.BossChallenge.LostKinHelper.lostKinMaxHp);
        }

        private void NormalizeLostKinPhaseThresholds()
        {
            int phase2Max = GetLostKinPhase2Max();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp, 4, phase2Max);
            int phase3Max = Mathf.Max(3, phase2Hp - 1);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp, 3, phase3Max);
            int phase4Max = Mathf.Max(2, phase3Hp - 1);
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp, 2, phase4Max);
            int phase5Max = Mathf.Max(1, phase4Hp - 1);
            int phase5Hp = Mathf.Clamp(Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp, 1, phase5Max);

            Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp = phase2Hp;
            Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp = phase3Hp;
            Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp = phase4Hp;
            Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp = phase5Hp;
        }

        private void RefreshLostKinHelperUi()
        {
            Module? module = GetLostKinHelperModule();
            UpdateToggleValue(lostKinHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(lostKinHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(lostKinP5HpValue, Modules.BossChallenge.LostKinHelper.lostKinP5Hp);
            UpdateToggleValue(lostKinUseMaxHpValue, Modules.BossChallenge.LostKinHelper.lostKinUseMaxHp);
            UpdateToggleValue(lostKinUseCustomPhaseValue, Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase);
            UpdateToggleValue(lostKinUseCustomSummonHpValue, Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonHp);
            UpdateToggleValue(lostKinUseCustomSummonLimitValue, Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonLimit);
            UpdateIntInputValue(lostKinMaxHpField, Modules.BossChallenge.LostKinHelper.lostKinMaxHp);
            UpdateIntInputValue(lostKinPhase2HpField, Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp);
            UpdateIntInputValue(lostKinPhase3HpField, Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp);
            UpdateIntInputValue(lostKinPhase4HpField, Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp);
            UpdateIntInputValue(lostKinPhase5HpField, Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp);
            UpdateIntInputValue(lostKinSummonHpField, Modules.BossChallenge.LostKinHelper.lostKinSummonHp);
            UpdateIntInputValue(lostKinSummonLimitField, Modules.BossChallenge.LostKinHelper.lostKinSummonLimit);

            UpdateLostKinHelperInteractivity();
        }

        private void UpdateLostKinHelperInteractivity()
        {
            SetContentInteractivity(lostKinHelperContent, GetLostKinHelperEnabled(), "LostKinHelperEnableRow");
            if (!GetLostKinHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.LostKinHelper.lostKinUseMaxHp;
            bool p5Hp = Modules.BossChallenge.LostKinHelper.lostKinP5Hp;
            bool useCustomPhase = Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase;
            bool useCustomSummonHp = Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonHp;
            bool useCustomSummonLimit = Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonLimit;
            SetRowInteractivity(lostKinHelperContent, "LostKinUseMaxHpRow", !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinPhase3HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinPhase4HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinPhase5HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinUseCustomSummonHpRow", !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinSummonHpRow", useCustomSummonHp && !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinUseCustomSummonLimitRow", !p5Hp);
            SetRowInteractivity(lostKinHelperContent, "LostKinSummonLimitRow", useCustomSummonLimit && !p5Hp);
        }

        private void SetLostKinHelperVisible(bool value)
        {
            lostKinHelperVisible = value;
            ApplyPanelVisible(lostKinHelperRoot, value, RefreshLostKinHelperUi);
        }

        private void OnBossManipulateLostKinClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetLostKinHelperVisible);
        }

        private void OnLostKinHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetLostKinHelperVisible);
        }

        private void OnLostKinHelperResetDefaultsClicked()
        {
            ResetLostKinHelperDefaults();
            SetLostKinHelperEnabled(false);
            Modules.BossChallenge.LostKinHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.LostKinHelper.RestoreVanillaSummonHealthIfPresent();
            Modules.BossChallenge.LostKinHelper.RestoreVanillaSummonLimitsIfPresent();
            Modules.BossChallenge.LostKinHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshLostKinHelperUi();
        }

        private void BuildLostKinHelperOverlayUi()
        {
            lostKinHelperRoot = CreateOverlayFrame("LostKinHelperOverlayCanvas", LostKinHelperCanvasSortOrder, "LostKinHelperPanel", LostKinHelperPanelHeight, "Modules/LostKinHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = LostKinHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, LostKinHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            lostKinHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "LostKinHelperEnableRow",
                "Settings/LostKinHelper/Enable".Localize(),
                rowY,
                GetLostKinHelperEnabled,
                SetLostKinHelperEnabled,
                out lostKinHelperToggleValue,
                out lostKinHelperToggleIcon
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateToggleRow(
                content,
                "LostKinP5HpRow",
                "Settings/LostKinHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinP5Hp,
                SetLostKinP5HpEnabled,
                out lostKinP5HpValue
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateToggleRow(
                content,
                "LostKinUseMaxHpRow",
                "Settings/LostKinHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinUseMaxHp,
                SetLostKinUseMaxHpEnabled,
                out lostKinUseMaxHpValue
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "LostKinMaxHpRow",
                "Settings/LostKinHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinMaxHp,
                SetLostKinMaxHp,
                1,
                999999,
                10,
                out lostKinMaxHpField
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateToggleRow(
                content,
                "LostKinUseCustomPhaseRow",
                "Settings/LostKinHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinUseCustomPhase,
                SetLostKinUseCustomPhaseEnabled,
                out lostKinUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "LostKinPhase2HpRow",
                "Settings/LostKinHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinPhase2Hp,
                SetLostKinPhase2Hp,
                4,
                999999,
                10,
                out lostKinPhase2HpField
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "LostKinPhase3HpRow",
                "Settings/LostKinHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinPhase3Hp,
                SetLostKinPhase3Hp,
                3,
                999998,
                10,
                out lostKinPhase3HpField
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "LostKinPhase4HpRow",
                "Settings/LostKinHelper/Phase4HP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinPhase4Hp,
                SetLostKinPhase4Hp,
                2,
                999997,
                10,
                out lostKinPhase4HpField
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "LostKinPhase5HpRow",
                "Settings/LostKinHelper/Phase5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinPhase5Hp,
                SetLostKinPhase5Hp,
                1,
                999996,
                10,
                out lostKinPhase5HpField
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateToggleRow(
                content,
                "LostKinUseCustomSummonHpRow",
                "Settings/LostKinHelper/UseCustomSummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonHp,
                SetLostKinUseCustomSummonHpEnabled,
                out lostKinUseCustomSummonHpValue
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "LostKinSummonHpRow",
                "Settings/LostKinHelper/SummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinSummonHp,
                SetLostKinSummonHp,
                1,
                999999,
                10,
                out lostKinSummonHpField
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateToggleRow(
                content,
                "LostKinUseCustomSummonLimitRow",
                "Settings/LostKinHelper/UseCustomSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinUseCustomSummonLimit,
                SetLostKinUseCustomSummonLimitEnabled,
                out lostKinUseCustomSummonLimitValue
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "LostKinSummonLimitRow",
                "Settings/LostKinHelper/SummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.LostKinHelper.lostKinSummonLimit,
                SetLostKinSummonLimit,
                0,
                999,
                1,
                out lostKinSummonLimitField
            );

            lastY = rowY;
            rowY += LostKinHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "LostKinHelperResetRow", "Settings/LostKinHelper/Reset".Localize(), resetY, OnLostKinHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "LostKinHelperBackRow", "Back", backY, OnLostKinHelperBackClicked);
        }
    }
}
