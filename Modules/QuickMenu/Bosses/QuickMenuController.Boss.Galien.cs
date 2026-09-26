using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int GalienHelperCanvasSortOrder = 10034;
        private const float GalienHelperPanelHeight = PanelHeight;
        private const float GalienHelperRowSpacing = 44f;
        private RectTransform? galienHelperContent;
        private GameObject? galienHelperRoot;
        private bool galienHelperVisible;
        private Text? galienHelperToggleValue;
        private Image? galienHelperToggleIcon;
        private Text? galienP5HpValue;
        private Text? galienUseMaxHpValue;
        private Text? galienUseCustomPhaseValue;
        private InputField? galienMaxHpField;
        private InputField? galienPhase2HpField;
        private InputField? galienPhase3HpField;
        private Module? galienHelperModule;

        private static void ResetGalienHelperDefaults()
        {
            Modules.BossChallenge.GalienHelper.galienUseMaxHp = false;
            Modules.BossChallenge.GalienHelper.galienP5Hp = false;
            Modules.BossChallenge.GalienHelper.galienUseCustomPhase = false;
            Modules.BossChallenge.GalienHelper.galienMaxHp = 1000;
            Modules.BossChallenge.GalienHelper.galienPhase2Hp = 700;
            Modules.BossChallenge.GalienHelper.galienPhase3Hp = 400;
            Modules.BossChallenge.GalienHelper.galienMaxHpBeforeP5 = 1000;
            Modules.BossChallenge.GalienHelper.galienUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.GalienHelper.galienHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.GalienHelper.galienUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.GalienHelper.galienPhase2HpBeforeP5 = 700;
            Modules.BossChallenge.GalienHelper.galienPhase3HpBeforeP5 = 400;
        }

        private Module? GetGalienHelperModule()
        {
            return GetCachedModule(ref galienHelperModule, typeof(Modules.BossChallenge.GalienHelper));
        }

        private bool GetGalienHelperEnabled()
        {
            return GetGalienHelperModule()?.Enabled ?? false;
        }

        private void SetGalienHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetGalienHelperModule(), value);
            UpdateGalienHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetGalienUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.GalienHelper.galienP5Hp)
            {
                Modules.BossChallenge.GalienHelper.galienUseMaxHp = true;
                RefreshGalienHelperUi();
                return;
            }

            Modules.BossChallenge.GalienHelper.galienUseMaxHp = value;
            NormalizeGalienPhaseThresholdInputs();
            Modules.BossChallenge.GalienHelper.ReapplyLiveSettings();
            RefreshGalienHelperUi();
        }

        private void SetGalienUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.GalienHelper.galienP5Hp)
            {
                Modules.BossChallenge.GalienHelper.galienUseCustomPhase = false;
                RefreshGalienHelperUi();
                return;
            }

            int phase2MaxHp = GetGalienPhase2MaxHp();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.GalienHelper.galienPhase2Hp, 2, phase2MaxHp);
            Modules.BossChallenge.GalienHelper.galienPhase2Hp = phase2Hp;
            Modules.BossChallenge.GalienHelper.galienPhase3Hp = Mathf.Clamp(Modules.BossChallenge.GalienHelper.galienPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
            Modules.BossChallenge.GalienHelper.galienUseCustomPhase = value;
            Modules.BossChallenge.GalienHelper.ReapplyLiveSettings();
            RefreshGalienHelperUi();
        }

        private void SetGalienP5HpEnabled(bool value)
        {
            Modules.BossChallenge.GalienHelper.SetP5HpEnabled(value);
            RefreshGalienHelperUi();
        }

        private void SetGalienMaxHp(int value)
        {
            if (Modules.BossChallenge.GalienHelper.galienP5Hp)
            {
                Modules.BossChallenge.GalienHelper.galienMaxHp = 650;
                RefreshGalienHelperUi();
                return;
            }

            Modules.BossChallenge.GalienHelper.galienMaxHp = Mathf.Clamp(value, 1, 999999);
            NormalizeGalienPhaseThresholdInputs();
            if (Modules.BossChallenge.GalienHelper.galienUseMaxHp)
            {
                Modules.BossChallenge.GalienHelper.ApplyGalienHealthIfPresent();
            }

            if (Modules.BossChallenge.GalienHelper.galienUseCustomPhase)
            {
                Modules.BossChallenge.GalienHelper.ReapplyLiveSettings();
            }
        }

        private void SetGalienPhase2Hp(int value)
        {
            if (Modules.BossChallenge.GalienHelper.galienP5Hp)
            {
                RefreshGalienHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(value, 2, GetGalienPhase2MaxHp());
            Modules.BossChallenge.GalienHelper.galienPhase2Hp = phase2Hp;
            Modules.BossChallenge.GalienHelper.galienPhase3Hp = Mathf.Clamp(Modules.BossChallenge.GalienHelper.galienPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.GalienHelper.galienUseCustomPhase)
            {
                Modules.BossChallenge.GalienHelper.ReapplyLiveSettings();
            }

            RefreshGalienHelperUi();
        }

        private void SetGalienPhase3Hp(int value)
        {
            if (Modules.BossChallenge.GalienHelper.galienP5Hp)
            {
                RefreshGalienHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.GalienHelper.galienPhase2Hp, 2, GetGalienPhase2MaxHp());
            Modules.BossChallenge.GalienHelper.galienPhase2Hp = phase2Hp;
            Modules.BossChallenge.GalienHelper.galienPhase3Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.GalienHelper.galienUseCustomPhase)
            {
                Modules.BossChallenge.GalienHelper.ReapplyLiveSettings();
            }

            RefreshGalienHelperUi();
        }

        private int GetGalienPhase2MaxHp()
        {
            return Mathf.Clamp(Modules.BossChallenge.GalienHelper.GetPhase2MaxHpForUi(), 2, 999999);
        }

        private void NormalizeGalienPhaseThresholdInputs()
        {
            int phase2MaxHp = GetGalienPhase2MaxHp();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.GalienHelper.galienPhase2Hp, 2, phase2MaxHp);
            Modules.BossChallenge.GalienHelper.galienPhase2Hp = phase2Hp;
            Modules.BossChallenge.GalienHelper.galienPhase3Hp = Mathf.Clamp(Modules.BossChallenge.GalienHelper.galienPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
        }

        private void RefreshGalienHelperUi()
        {
            Module? module = GetGalienHelperModule();
            UpdateToggleValue(galienHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(galienHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(galienP5HpValue, Modules.BossChallenge.GalienHelper.galienP5Hp);
            UpdateToggleValue(galienUseMaxHpValue, Modules.BossChallenge.GalienHelper.galienUseMaxHp);
            UpdateToggleValue(galienUseCustomPhaseValue, Modules.BossChallenge.GalienHelper.galienUseCustomPhase);
            UpdateIntInputValue(galienMaxHpField, Modules.BossChallenge.GalienHelper.galienMaxHp);
            UpdateIntInputValue(galienPhase2HpField, Modules.BossChallenge.GalienHelper.galienPhase2Hp);
            UpdateIntInputValue(galienPhase3HpField, Modules.BossChallenge.GalienHelper.galienPhase3Hp);

            UpdateGalienHelperInteractivity();
        }

        private void UpdateGalienHelperInteractivity()
        {
            SetContentInteractivity(galienHelperContent, GetGalienHelperEnabled(), "GalienHelperEnableRow");
            if (!GetGalienHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.GalienHelper.galienUseMaxHp;
            bool p5Hp = Modules.BossChallenge.GalienHelper.galienP5Hp;
            bool useCustomPhase = Modules.BossChallenge.GalienHelper.galienUseCustomPhase;
            SetRowInteractivity(galienHelperContent, "GalienUseMaxHpRow", !p5Hp);
            SetRowInteractivity(galienHelperContent, "GalienMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(galienHelperContent, "GalienUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(galienHelperContent, "GalienPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(galienHelperContent, "GalienPhase3HpRow", useCustomPhase && !p5Hp);
        }

        private void SetGalienHelperVisible(bool value)
        {
            galienHelperVisible = value;
            ApplyPanelVisible(galienHelperRoot, value, RefreshGalienHelperUi);
        }

        private void OnBossManipulateGalienClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetGalienHelperVisible);
        }

        private void OnGalienHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetGalienHelperVisible);
        }

        private void OnGalienHelperResetDefaultsClicked()
        {
            ResetGalienHelperDefaults();
            SetGalienHelperEnabled(false);
            Modules.BossChallenge.GalienHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.GalienHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshGalienHelperUi();
        }

        private void BuildGalienHelperOverlayUi()
        {
            galienHelperRoot = CreateOverlayFrame("GalienHelperOverlayCanvas", GalienHelperCanvasSortOrder, "GalienHelperPanel", GalienHelperPanelHeight, "Modules/GalienHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = GalienHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, GalienHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            galienHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "GalienHelperEnableRow",
                "Settings/GalienHelper/Enable".Localize(),
                rowY,
                GetGalienHelperEnabled,
                SetGalienHelperEnabled,
                out galienHelperToggleValue,
                out galienHelperToggleIcon
            );

            lastY = rowY;
            rowY += GalienHelperRowSpacing;
            CreateToggleRow(
                content,
                "GalienP5HpRow",
                "Settings/GalienHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.GalienHelper.galienP5Hp,
                SetGalienP5HpEnabled,
                out galienP5HpValue
            );

            lastY = rowY;
            rowY += GalienHelperRowSpacing;
            CreateToggleRow(
                content,
                "GalienUseMaxHpRow",
                "Settings/GalienHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.GalienHelper.galienUseMaxHp,
                SetGalienUseMaxHpEnabled,
                out galienUseMaxHpValue
            );

            lastY = rowY;
            rowY += GalienHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GalienMaxHpRow",
                "Settings/GalienHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.GalienHelper.galienMaxHp,
                SetGalienMaxHp,
                1,
                999999,
                10,
                out galienMaxHpField
            );

            lastY = rowY;
            rowY += GalienHelperRowSpacing;
            CreateToggleRow(
                content,
                "GalienUseCustomPhaseRow",
                "Settings/GalienHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.GalienHelper.galienUseCustomPhase,
                SetGalienUseCustomPhaseEnabled,
                out galienUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += GalienHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GalienPhase2HpRow",
                "Settings/GalienHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.GalienHelper.galienPhase2Hp,
                SetGalienPhase2Hp,
                2,
                999999,
                10,
                out galienPhase2HpField
            );

            lastY = rowY;
            rowY += GalienHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GalienPhase3HpRow",
                "Settings/GalienHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.GalienHelper.galienPhase3Hp,
                SetGalienPhase3Hp,
                1,
                999999,
                10,
                out galienPhase3HpField
            );

            lastY = rowY;
            rowY += GalienHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "GalienHelperResetRow", "Settings/GalienHelper/Reset".Localize(), resetY, OnGalienHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "GalienHelperBackRow", "Back", backY, OnGalienHelperBackClicked);
        }
    }
}
