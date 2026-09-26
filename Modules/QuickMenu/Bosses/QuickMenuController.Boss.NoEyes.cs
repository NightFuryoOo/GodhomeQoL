using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int NoEyesHelperCanvasSortOrder = 10037;
        private const float NoEyesHelperPanelHeight = PanelHeight;
        private const float NoEyesHelperRowSpacing = 44f;
        private RectTransform? noEyesHelperContent;
        private GameObject? noEyesHelperRoot;
        private bool noEyesHelperVisible;
        private Text? noEyesHelperToggleValue;
        private Image? noEyesHelperToggleIcon;
        private Text? noEyesP5HpValue;
        private Text? noEyesUseMaxHpValue;
        private Text? noEyesUseCustomPhaseValue;
        private InputField? noEyesMaxHpField;
        private InputField? noEyesPhase2HpField;
        private InputField? noEyesPhase3HpField;
        private Module? noEyesHelperModule;

        private static void ResetNoEyesHelperDefaults()
        {
            Modules.BossChallenge.NoEyesHelper.noEyesUseMaxHp = false;
            Modules.BossChallenge.NoEyesHelper.noEyesP5Hp = false;
            Modules.BossChallenge.NoEyesHelper.noEyesMaxHp = 800;
            Modules.BossChallenge.NoEyesHelper.noEyesMaxHpBeforeP5 = 800;
            Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase = false;
            Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp = 150;
            Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp = 90;
            Modules.BossChallenge.NoEyesHelper.noEyesUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.NoEyesHelper.noEyesHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.NoEyesHelper.noEyesPhase2HpBeforeP5 = 150;
            Modules.BossChallenge.NoEyesHelper.noEyesPhase3HpBeforeP5 = 90;
        }

        private Module? GetNoEyesHelperModule()
        {
            return GetCachedModule(ref noEyesHelperModule, typeof(Modules.BossChallenge.NoEyesHelper));
        }

        private bool GetNoEyesHelperEnabled()
        {
            return GetNoEyesHelperModule()?.Enabled ?? false;
        }

        private void SetNoEyesHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetNoEyesHelperModule(), value);
            UpdateNoEyesHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetNoEyesUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.NoEyesHelper.noEyesP5Hp)
            {
                Modules.BossChallenge.NoEyesHelper.noEyesUseMaxHp = true;
                RefreshNoEyesHelperUi();
                return;
            }

            Modules.BossChallenge.NoEyesHelper.noEyesUseMaxHp = value;
            NormalizeNoEyesPhaseThresholdInputs();
            Modules.BossChallenge.NoEyesHelper.ReapplyLiveSettings();
            RefreshNoEyesHelperUi();
        }

        private void SetNoEyesUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.NoEyesHelper.noEyesP5Hp)
            {
                Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase = false;
                RefreshNoEyesHelperUi();
                return;
            }

            int phase2MaxHp = GetNoEyesPhase2MaxHp();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp, 2, phase2MaxHp);
            Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp = phase2Hp;
            Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp = Mathf.Clamp(Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
            Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase = value;
            Modules.BossChallenge.NoEyesHelper.ReapplyLiveSettings();
            RefreshNoEyesHelperUi();
        }

        private void SetNoEyesP5HpEnabled(bool value)
        {
            Modules.BossChallenge.NoEyesHelper.SetP5HpEnabled(value);
            RefreshNoEyesHelperUi();
        }

        private void SetNoEyesMaxHp(int value)
        {
            if (Modules.BossChallenge.NoEyesHelper.noEyesP5Hp)
            {
                Modules.BossChallenge.NoEyesHelper.noEyesMaxHp = 570;
                RefreshNoEyesHelperUi();
                return;
            }

            Modules.BossChallenge.NoEyesHelper.noEyesMaxHp = Mathf.Clamp(value, 1, 999999);
            NormalizeNoEyesPhaseThresholdInputs();
            if (Modules.BossChallenge.NoEyesHelper.noEyesUseMaxHp || Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase)
            {
                Modules.BossChallenge.NoEyesHelper.ReapplyLiveSettings();
            }

            RefreshNoEyesHelperUi();
        }

        private void SetNoEyesPhase2Hp(int value)
        {
            if (Modules.BossChallenge.NoEyesHelper.noEyesP5Hp)
            {
                RefreshNoEyesHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(value, 2, GetNoEyesPhase2MaxHp());
            Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp = phase2Hp;
            Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp = Mathf.Clamp(Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase)
            {
                Modules.BossChallenge.NoEyesHelper.ReapplyLiveSettings();
            }

            RefreshNoEyesHelperUi();
        }

        private void SetNoEyesPhase3Hp(int value)
        {
            if (Modules.BossChallenge.NoEyesHelper.noEyesP5Hp)
            {
                RefreshNoEyesHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp, 2, GetNoEyesPhase2MaxHp());
            Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp = phase2Hp;
            Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase)
            {
                Modules.BossChallenge.NoEyesHelper.ReapplyLiveSettings();
            }

            RefreshNoEyesHelperUi();
        }

        private int GetNoEyesPhase2MaxHp()
        {
            return Mathf.Clamp(Modules.BossChallenge.NoEyesHelper.GetPhase2MaxHpForUi(), 2, 999999);
        }

        private void NormalizeNoEyesPhaseThresholdInputs()
        {
            int phase2MaxHp = GetNoEyesPhase2MaxHp();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp, 2, phase2MaxHp);
            Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp = phase2Hp;
            Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp = Mathf.Clamp(Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
        }

        private void RefreshNoEyesHelperUi()
        {
            Module? module = GetNoEyesHelperModule();
            UpdateToggleValue(noEyesHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(noEyesHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(noEyesP5HpValue, Modules.BossChallenge.NoEyesHelper.noEyesP5Hp);
            UpdateToggleValue(noEyesUseMaxHpValue, Modules.BossChallenge.NoEyesHelper.noEyesUseMaxHp);
            UpdateToggleValue(noEyesUseCustomPhaseValue, Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase);
            UpdateIntInputValue(noEyesMaxHpField, Modules.BossChallenge.NoEyesHelper.noEyesMaxHp);
            UpdateIntInputValue(noEyesPhase2HpField, Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp);
            UpdateIntInputValue(noEyesPhase3HpField, Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp);

            UpdateNoEyesHelperInteractivity();
        }

        private void UpdateNoEyesHelperInteractivity()
        {
            SetContentInteractivity(noEyesHelperContent, GetNoEyesHelperEnabled(), "NoEyesHelperEnableRow");
            if (!GetNoEyesHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.NoEyesHelper.noEyesUseMaxHp;
            bool p5Hp = Modules.BossChallenge.NoEyesHelper.noEyesP5Hp;
            bool useCustomPhase = Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase;
            SetRowInteractivity(noEyesHelperContent, "NoEyesUseMaxHpRow", !p5Hp);
            SetRowInteractivity(noEyesHelperContent, "NoEyesMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(noEyesHelperContent, "NoEyesUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(noEyesHelperContent, "NoEyesPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(noEyesHelperContent, "NoEyesPhase3HpRow", useCustomPhase && !p5Hp);
        }

        private void SetNoEyesHelperVisible(bool value)
        {
            noEyesHelperVisible = value;
            ApplyPanelVisible(noEyesHelperRoot, value, RefreshNoEyesHelperUi);
        }

        private void OnBossManipulateNoEyesClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetNoEyesHelperVisible);
        }

        private void OnNoEyesHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetNoEyesHelperVisible);
        }

        private void OnNoEyesHelperResetDefaultsClicked()
        {
            ResetNoEyesHelperDefaults();
            SetNoEyesHelperEnabled(false);
            Modules.BossChallenge.NoEyesHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.NoEyesHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshNoEyesHelperUi();
        }

        private void BuildNoEyesHelperOverlayUi()
        {
            noEyesHelperRoot = CreateOverlayFrame("NoEyesHelperOverlayCanvas", NoEyesHelperCanvasSortOrder, "NoEyesHelperPanel", NoEyesHelperPanelHeight, "Modules/NoEyesHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = NoEyesHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, NoEyesHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            noEyesHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "NoEyesHelperEnableRow",
                "Settings/NoEyesHelper/Enable".Localize(),
                rowY,
                GetNoEyesHelperEnabled,
                SetNoEyesHelperEnabled,
                out noEyesHelperToggleValue,
                out noEyesHelperToggleIcon
            );

            lastY = rowY;
            rowY += NoEyesHelperRowSpacing;
            CreateToggleRow(
                content,
                "NoEyesP5HpRow",
                "Settings/NoEyesHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NoEyesHelper.noEyesP5Hp,
                SetNoEyesP5HpEnabled,
                out noEyesP5HpValue
            );

            lastY = rowY;
            rowY += NoEyesHelperRowSpacing;
            CreateToggleRow(
                content,
                "NoEyesUseMaxHpRow",
                "Settings/NoEyesHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.NoEyesHelper.noEyesUseMaxHp,
                SetNoEyesUseMaxHpEnabled,
                out noEyesUseMaxHpValue
            );

            lastY = rowY;
            rowY += NoEyesHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoEyesMaxHpRow",
                "Settings/NoEyesHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.NoEyesHelper.noEyesMaxHp,
                SetNoEyesMaxHp,
                1,
                999999,
                10,
                out noEyesMaxHpField
            );

            lastY = rowY;
            rowY += NoEyesHelperRowSpacing;
            CreateToggleRow(
                content,
                "NoEyesUseCustomPhaseRow",
                "Settings/NoEyesHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.NoEyesHelper.noEyesUseCustomPhase,
                SetNoEyesUseCustomPhaseEnabled,
                out noEyesUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += NoEyesHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoEyesPhase2HpRow",
                "Settings/NoEyesHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NoEyesHelper.noEyesPhase2Hp,
                SetNoEyesPhase2Hp,
                2,
                999999,
                10,
                out noEyesPhase2HpField
            );

            lastY = rowY;
            rowY += NoEyesHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoEyesPhase3HpRow",
                "Settings/NoEyesHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NoEyesHelper.noEyesPhase3Hp,
                SetNoEyesPhase3Hp,
                1,
                999999,
                10,
                out noEyesPhase3HpField
            );

            lastY = rowY;
            rowY += NoEyesHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "NoEyesHelperResetRow", "Settings/NoEyesHelper/Reset".Localize(), resetY, OnNoEyesHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "NoEyesHelperBackRow", "Back", backY, OnNoEyesHelperBackClicked);
        }
    }
}
