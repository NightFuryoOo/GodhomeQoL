using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int AbsoluteRadianceHelperCanvasSortOrder = 10049;
        private const float AbsoluteRadianceHelperPanelHeight = PanelHeight;
        private const float AbsoluteRadianceHelperRowSpacing = 44f;
        private RectTransform? absoluteRadianceHelperContent;
        private GameObject? absoluteRadianceHelperRoot;
        private bool absoluteRadianceHelperVisible;
        private Text? absoluteRadianceHelperToggleValue;
        private Image? absoluteRadianceHelperToggleIcon;
        private Text? absoluteRadianceP5HpValue;
        private Text? absoluteRadianceUseMaxHpValue;
        private Text? absoluteRadianceUseCustomPhaseValue;
        private InputField? absoluteRadianceMaxHpField;
        private InputField? absoluteRadiancePhase2HpField;
        private InputField? absoluteRadiancePhase3HpField;
        private InputField? absoluteRadiancePhase4HpField;
        private InputField? absoluteRadiancePhase5HpField;
        private InputField? absoluteRadianceFinalPhaseHpField;
        private Module? absoluteRadianceHelperModule;

        private static void ResetAbsoluteRadianceHelperDefaults()
        {
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseMaxHp = false;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp = false;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase = false;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp = 3000;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp = 2600;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp = 2150;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp = 1850;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp = 1100;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHp = 1000;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHpBeforeP5 = 3000;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2HpBeforeP5 = 2600;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3HpBeforeP5 = 2150;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4HpBeforeP5 = 1850;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5HpBeforeP5 = 1100;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHpBeforeP5 = 1000;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceHasStoredStateBeforeP5 = false;
        }

        private Module? GetAbsoluteRadianceHelperModule()
        {
            return GetCachedModule(ref absoluteRadianceHelperModule, typeof(Modules.BossChallenge.AbsoluteRadianceHelper));
        }

        private bool GetAbsoluteRadianceHelperEnabled()
        {
            return GetAbsoluteRadianceHelperModule()?.Enabled ?? false;
        }

        private void SetAbsoluteRadianceHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetAbsoluteRadianceHelperModule(), value);
            UpdateAbsoluteRadianceHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetAbsoluteRadianceUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseMaxHp = true;
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseMaxHp = value;
            Modules.BossChallenge.AbsoluteRadianceHelper.ReapplyLiveSettings();
            RefreshAbsoluteRadianceHelperUi();
        }

        private void SetAbsoluteRadianceUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase = false;
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            int maxHp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp, 1, 999999);
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp, 1, maxHp);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp, 1, Mathf.Max(1, phase3Hp - 1));
            int phase5Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp, 1, Mathf.Max(1, phase4Hp - 1));
            int finalPhaseHp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHp, 1, 999999);

            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp = phase2Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp = phase3Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp = phase4Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp = phase5Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHp = finalPhaseHp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase = value;
            Modules.BossChallenge.AbsoluteRadianceHelper.ReapplyLiveSettings();
            RefreshAbsoluteRadianceHelperUi();
        }

        private void SetAbsoluteRadianceP5HpEnabled(bool value)
        {
            Modules.BossChallenge.AbsoluteRadianceHelper.SetP5HpEnabled(value);
            RefreshAbsoluteRadianceHelperUi();
        }

        private void SetAbsoluteRadianceMaxHp(int value)
        {
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp = 3000;
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase)
            {
                int maxHp = Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp;
                int phase2Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp, 1, maxHp);
                int phase3Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
                int phase4Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp, 1, Mathf.Max(1, phase3Hp - 1));
                int phase5Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp, 1, Mathf.Max(1, phase4Hp - 1));
                int finalPhaseHp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHp, 1, 999999);
                Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp = phase2Hp;
                Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp = phase3Hp;
                Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp = phase4Hp;
                Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp = phase5Hp;
                Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHp = finalPhaseHp;
                Modules.BossChallenge.AbsoluteRadianceHelper.ReapplyLiveSettings();
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseMaxHp)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.ApplyAbsoluteRadianceHealthIfPresent();
            }
        }

        private void SetAbsoluteRadiancePhase2Hp(int value)
        {
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp)
            {
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            int maxHp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp, 1, 999999);
            int phase2Hp = Mathf.Clamp(value, 1, maxHp);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp, 1, Mathf.Max(1, phase3Hp - 1));
            int phase5Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp, 1, Mathf.Max(1, phase4Hp - 1));

            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp = phase2Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp = phase3Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp = phase4Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp = phase5Hp;

            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.ReapplyLiveSettings();
            }

            RefreshAbsoluteRadianceHelperUi();
        }

        private void SetAbsoluteRadianceFinalPhaseHp(int value)
        {
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp)
            {
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHp = Mathf.Clamp(value, 1, 999999);

            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.ReapplyLiveSettings();
            }

            RefreshAbsoluteRadianceHelperUi();
        }

        private void SetAbsoluteRadiancePhase3Hp(int value)
        {
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp)
            {
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            int maxHp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp, 1, 999999);
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp, 1, maxHp);
            int phase3Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase2Hp - 1));
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp, 1, Mathf.Max(1, phase3Hp - 1));
            int phase5Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp, 1, Mathf.Max(1, phase4Hp - 1));

            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp = phase2Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp = phase3Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp = phase4Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp = phase5Hp;

            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.ReapplyLiveSettings();
            }

            RefreshAbsoluteRadianceHelperUi();
        }

        private void SetAbsoluteRadiancePhase4Hp(int value)
        {
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp)
            {
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            int maxHp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp, 1, 999999);
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp, 1, maxHp);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
            int phase4Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase3Hp - 1));
            int phase5Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp, 1, Mathf.Max(1, phase4Hp - 1));

            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp = phase2Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp = phase3Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp = phase4Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp = phase5Hp;

            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.ReapplyLiveSettings();
            }

            RefreshAbsoluteRadianceHelperUi();
        }

        private void SetAbsoluteRadiancePhase5Hp(int value)
        {
            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp)
            {
                RefreshAbsoluteRadianceHelperUi();
                return;
            }

            int maxHp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp, 1, 999999);
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp, 1, maxHp);
            int phase3Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
            int phase4Hp = Mathf.Clamp(Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp, 1, Mathf.Max(1, phase3Hp - 1));
            int phase5Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase4Hp - 1));

            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp = phase2Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp = phase3Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp = phase4Hp;
            Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp = phase5Hp;

            if (Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase)
            {
                Modules.BossChallenge.AbsoluteRadianceHelper.ReapplyLiveSettings();
            }

            RefreshAbsoluteRadianceHelperUi();
        }

        private void RefreshAbsoluteRadianceHelperUi()
        {
            Module? module = GetAbsoluteRadianceHelperModule();
            UpdateToggleValue(absoluteRadianceHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(absoluteRadianceHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(absoluteRadianceP5HpValue, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp);
            UpdateToggleValue(absoluteRadianceUseMaxHpValue, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseMaxHp);
            UpdateToggleValue(absoluteRadianceUseCustomPhaseValue, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase);
            UpdateIntInputValue(absoluteRadianceMaxHpField, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp);
            UpdateIntInputValue(absoluteRadiancePhase2HpField, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp);
            UpdateIntInputValue(absoluteRadiancePhase3HpField, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp);
            UpdateIntInputValue(absoluteRadiancePhase4HpField, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp);
            UpdateIntInputValue(absoluteRadiancePhase5HpField, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp);
            UpdateIntInputValue(absoluteRadianceFinalPhaseHpField, Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHp);

            UpdateAbsoluteRadianceHelperInteractivity();
        }

        private void UpdateAbsoluteRadianceHelperInteractivity()
        {
            SetContentInteractivity(absoluteRadianceHelperContent, GetAbsoluteRadianceHelperEnabled(), "AbsoluteRadianceHelperEnableRow");
            if (!GetAbsoluteRadianceHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseMaxHp;
            bool p5Hp = Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp;
            bool useCustomPhase = Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase;
            SetRowInteractivity(absoluteRadianceHelperContent, "AbsoluteRadianceUseMaxHpRow", !p5Hp);
            SetRowInteractivity(absoluteRadianceHelperContent, "AbsoluteRadianceMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(absoluteRadianceHelperContent, "AbsoluteRadianceUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(absoluteRadianceHelperContent, "AbsoluteRadiancePhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(absoluteRadianceHelperContent, "AbsoluteRadiancePhase3HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(absoluteRadianceHelperContent, "AbsoluteRadiancePhase4HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(absoluteRadianceHelperContent, "AbsoluteRadiancePhase5HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(absoluteRadianceHelperContent, "AbsoluteRadianceFinalPhaseHpRow", useCustomPhase && !p5Hp);
        }

        private void SetAbsoluteRadianceHelperVisible(bool value)
        {
            absoluteRadianceHelperVisible = value;
            ApplyPanelVisible(absoluteRadianceHelperRoot, value, RefreshAbsoluteRadianceHelperUi);
        }

        private void OnBossManipulateAbsoluteRadianceClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetAbsoluteRadianceHelperVisible);
        }

        private void OnAbsoluteRadianceHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetAbsoluteRadianceHelperVisible);
        }

        private void OnAbsoluteRadianceHelperResetDefaultsClicked()
        {
            ResetAbsoluteRadianceHelperDefaults();
            SetAbsoluteRadianceHelperEnabled(false);
            Modules.BossChallenge.AbsoluteRadianceHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.AbsoluteRadianceHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshAbsoluteRadianceHelperUi();
        }

        private void BuildAbsoluteRadianceHelperOverlayUi()
        {
            absoluteRadianceHelperRoot = CreateOverlayFrame("AbsoluteRadianceHelperOverlayCanvas", AbsoluteRadianceHelperCanvasSortOrder, "AbsoluteRadianceHelperPanel", AbsoluteRadianceHelperPanelHeight, "Modules/AbsoluteRadianceHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = AbsoluteRadianceHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, AbsoluteRadianceHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            absoluteRadianceHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "AbsoluteRadianceHelperEnableRow",
                "Settings/AbsoluteRadianceHelper/Enable".Localize(),
                rowY,
                GetAbsoluteRadianceHelperEnabled,
                SetAbsoluteRadianceHelperEnabled,
                out absoluteRadianceHelperToggleValue,
                out absoluteRadianceHelperToggleIcon
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateToggleRow(
                content,
                "AbsoluteRadianceP5HpRow",
                "Settings/AbsoluteRadianceHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceP5Hp,
                SetAbsoluteRadianceP5HpEnabled,
                out absoluteRadianceP5HpValue
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateToggleRow(
                content,
                "AbsoluteRadianceUseMaxHpRow",
                "Settings/AbsoluteRadianceHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseMaxHp,
                SetAbsoluteRadianceUseMaxHpEnabled,
                out absoluteRadianceUseMaxHpValue
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "AbsoluteRadianceMaxHpRow",
                "Settings/AbsoluteRadianceHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceMaxHp,
                SetAbsoluteRadianceMaxHp,
                1,
                999999,
                10,
                out absoluteRadianceMaxHpField
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateToggleRow(
                content,
                "AbsoluteRadianceUseCustomPhaseRow",
                "Settings/AbsoluteRadianceHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceUseCustomPhase,
                SetAbsoluteRadianceUseCustomPhaseEnabled,
                out absoluteRadianceUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "AbsoluteRadiancePhase2HpRow",
                "Settings/AbsoluteRadianceHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase2Hp,
                SetAbsoluteRadiancePhase2Hp,
                1,
                999999,
                10,
                out absoluteRadiancePhase2HpField
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "AbsoluteRadiancePhase3HpRow",
                "Settings/AbsoluteRadianceHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase3Hp,
                SetAbsoluteRadiancePhase3Hp,
                1,
                999999,
                10,
                out absoluteRadiancePhase3HpField
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "AbsoluteRadiancePhase4HpRow",
                "Settings/AbsoluteRadianceHelper/Phase4HP".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase4Hp,
                SetAbsoluteRadiancePhase4Hp,
                1,
                999999,
                10,
                out absoluteRadiancePhase4HpField
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "AbsoluteRadiancePhase5HpRow",
                "Settings/AbsoluteRadianceHelper/Phase5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadiancePhase5Hp,
                SetAbsoluteRadiancePhase5Hp,
                1,
                999999,
                10,
                out absoluteRadiancePhase5HpField
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "AbsoluteRadianceFinalPhaseHpRow",
                "Settings/AbsoluteRadianceHelper/FinalPhaseHP".Localize(),
                rowY,
                () => Modules.BossChallenge.AbsoluteRadianceHelper.absoluteRadianceFinalPhaseHp,
                SetAbsoluteRadianceFinalPhaseHp,
                1,
                999999,
                10,
                out absoluteRadianceFinalPhaseHpField
            );

            lastY = rowY;
            rowY += AbsoluteRadianceHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "AbsoluteRadianceHelperResetRow", "Settings/AbsoluteRadianceHelper/Reset".Localize(), resetY, OnAbsoluteRadianceHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "AbsoluteRadianceHelperBackRow", "Back", backY, OnAbsoluteRadianceHelperBackClicked);
        }
    }
}
