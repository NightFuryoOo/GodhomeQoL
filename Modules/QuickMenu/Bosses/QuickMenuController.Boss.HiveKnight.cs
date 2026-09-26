using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int HiveKnightHelperCanvasSortOrder = 10040;
        private const float HiveKnightHelperPanelHeight = PanelHeight;
        private const float HiveKnightHelperRowSpacing = 44f;
        private RectTransform? hiveKnightHelperContent;
        private GameObject? hiveKnightHelperRoot;
        private bool hiveKnightHelperVisible;
        private Text? hiveKnightHelperToggleValue;
        private Image? hiveKnightHelperToggleIcon;
        private Text? hiveKnightP5HpValue;
        private Text? hiveKnightUseMaxHpValue;
        private Text? hiveKnightUseCustomPhaseValue;
        private InputField? hiveKnightMaxHpField;
        private InputField? hiveKnightPhase2HpField;
        private InputField? hiveKnightPhase3HpField;
        private Module? hiveKnightHelperModule;

        private static void ResetHiveKnightHelperDefaults()
        {
            Modules.BossChallenge.HiveKnightHelper.hiveKnightUseMaxHp = false;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp = false;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightMaxHp = 1300;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhase = false;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase2Hp = 580;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase3Hp = 350;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightMaxHpBeforeP5 = 1300;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase2HpBeforeP5 = 580;
            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase3HpBeforeP5 = 350;
        }

        private Module? GetHiveKnightHelperModule()
        {
            return GetCachedModule(ref hiveKnightHelperModule, typeof(Modules.BossChallenge.HiveKnightHelper));
        }

        private bool GetHiveKnightHelperEnabled()
        {
            return GetHiveKnightHelperModule()?.Enabled ?? false;
        }

        private void SetHiveKnightHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetHiveKnightHelperModule(), value);
            UpdateHiveKnightHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetHiveKnightUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp)
            {
                Modules.BossChallenge.HiveKnightHelper.hiveKnightUseMaxHp = true;
                RefreshHiveKnightHelperUi();
                return;
            }

            Modules.BossChallenge.HiveKnightHelper.hiveKnightUseMaxHp = value;
            Modules.BossChallenge.HiveKnightHelper.ReapplyLiveSettings();
            RefreshHiveKnightHelperUi();
        }

        private void SetHiveKnightUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp)
            {
                Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhase = false;
                RefreshHiveKnightHelperUi();
                return;
            }

            ClampHiveKnightPhaseThresholdsForUi();
            Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhase = value;
            Modules.BossChallenge.HiveKnightHelper.ReapplyLiveSettings();
            RefreshHiveKnightHelperUi();
        }

        private void SetHiveKnightP5HpEnabled(bool value)
        {
            Modules.BossChallenge.HiveKnightHelper.SetP5HpEnabled(value);
            RefreshHiveKnightHelperUi();
        }

        private void SetHiveKnightMaxHp(int value)
        {
            if (Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp)
            {
                Modules.BossChallenge.HiveKnightHelper.hiveKnightMaxHp = 850;
                RefreshHiveKnightHelperUi();
                return;
            }

            Modules.BossChallenge.HiveKnightHelper.hiveKnightMaxHp = Mathf.Clamp(value, 1, 999999);
            ClampHiveKnightPhaseThresholdsForUi();
            if (Modules.BossChallenge.HiveKnightHelper.hiveKnightUseMaxHp
                || Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhase)
            {
                Modules.BossChallenge.HiveKnightHelper.ReapplyLiveSettings();
            }

            RefreshHiveKnightHelperUi();
        }

        private void SetHiveKnightPhase2Hp(int value)
        {
            if (Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp)
            {
                RefreshHiveKnightHelperUi();
                return;
            }

            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase2Hp = value;
            ClampHiveKnightPhaseThresholdsForUi();

            if (Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhase)
            {
                Modules.BossChallenge.HiveKnightHelper.ReapplyLiveSettings();
            }

            RefreshHiveKnightHelperUi();
        }

        private void SetHiveKnightPhase3Hp(int value)
        {
            if (Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp)
            {
                RefreshHiveKnightHelperUi();
                return;
            }

            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase3Hp = value;
            ClampHiveKnightPhaseThresholdsForUi();

            if (Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhase)
            {
                Modules.BossChallenge.HiveKnightHelper.ReapplyLiveSettings();
            }

            RefreshHiveKnightHelperUi();
        }

        private static void ClampHiveKnightPhaseThresholdsForUi()
        {
            int phase2Hp = Mathf.Clamp(
                Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase2Hp,
                2,
                Modules.BossChallenge.HiveKnightHelper.GetPhase2MaxHpForUi()
            );
            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase2Hp = phase2Hp;

            Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase3Hp = Mathf.Clamp(
                Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase3Hp,
                1,
                Modules.BossChallenge.HiveKnightHelper.GetPhase3MaxHpForUi()
            );
        }

        private void RefreshHiveKnightHelperUi()
        {
            Module? module = GetHiveKnightHelperModule();
            UpdateToggleValue(hiveKnightHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(hiveKnightHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(hiveKnightP5HpValue, Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp);
            UpdateToggleValue(hiveKnightUseMaxHpValue, Modules.BossChallenge.HiveKnightHelper.hiveKnightUseMaxHp);
            UpdateToggleValue(hiveKnightUseCustomPhaseValue, Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhase);
            UpdateIntInputValue(hiveKnightMaxHpField, Modules.BossChallenge.HiveKnightHelper.hiveKnightMaxHp);
            UpdateIntInputValue(hiveKnightPhase2HpField, Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase2Hp);
            UpdateIntInputValue(hiveKnightPhase3HpField, Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase3Hp);

            UpdateHiveKnightHelperInteractivity();
        }

        private void UpdateHiveKnightHelperInteractivity()
        {
            SetContentInteractivity(hiveKnightHelperContent, GetHiveKnightHelperEnabled(), "HiveKnightHelperEnableRow");
            if (!GetHiveKnightHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.HiveKnightHelper.hiveKnightUseMaxHp;
            bool p5Hp = Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp;
            bool useCustomPhase = Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhase;
            SetRowInteractivity(hiveKnightHelperContent, "HiveKnightUseMaxHpRow", !p5Hp);
            SetRowInteractivity(hiveKnightHelperContent, "HiveKnightMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(hiveKnightHelperContent, "HiveKnightUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(hiveKnightHelperContent, "HiveKnightPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(hiveKnightHelperContent, "HiveKnightPhase3HpRow", useCustomPhase && !p5Hp);
        }

        private void SetHiveKnightHelperVisible(bool value)
        {
            hiveKnightHelperVisible = value;
            ApplyPanelVisible(hiveKnightHelperRoot, value, RefreshHiveKnightHelperUi);
        }

        private void OnBossManipulateHiveKnightClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetHiveKnightHelperVisible);
        }

        private void OnHiveKnightHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetHiveKnightHelperVisible);
        }

        private void OnHiveKnightHelperResetDefaultsClicked()
        {
            ResetHiveKnightHelperDefaults();
            SetHiveKnightHelperEnabled(false);
            Modules.BossChallenge.HiveKnightHelper.RestoreVanillaHealthIfPresent();
            RefreshHiveKnightHelperUi();
        }

        private void BuildHiveKnightHelperOverlayUi()
        {
            hiveKnightHelperRoot = CreateOverlayFrame("HiveKnightHelperOverlayCanvas", HiveKnightHelperCanvasSortOrder, "HiveKnightHelperPanel", HiveKnightHelperPanelHeight, "Modules/HiveKnightHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = HiveKnightHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, HiveKnightHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            hiveKnightHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "HiveKnightHelperEnableRow",
                "Settings/HiveKnightHelper/Enable".Localize(),
                rowY,
                GetHiveKnightHelperEnabled,
                SetHiveKnightHelperEnabled,
                out hiveKnightHelperToggleValue,
                out hiveKnightHelperToggleIcon
            );

            lastY = rowY;
            rowY += HiveKnightHelperRowSpacing;
            CreateToggleRow(
                content,
                "HiveKnightP5HpRow",
                "Settings/HiveKnightHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.HiveKnightHelper.hiveKnightP5Hp,
                SetHiveKnightP5HpEnabled,
                out hiveKnightP5HpValue
            );

            lastY = rowY;
            rowY += HiveKnightHelperRowSpacing;
            CreateToggleRow(
                content,
                "HiveKnightUseMaxHpRow",
                "Settings/HiveKnightHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.HiveKnightHelper.hiveKnightUseMaxHp,
                SetHiveKnightUseMaxHpEnabled,
                out hiveKnightUseMaxHpValue
            );

            lastY = rowY;
            rowY += HiveKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "HiveKnightMaxHpRow",
                "Settings/HiveKnightHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.HiveKnightHelper.hiveKnightMaxHp,
                SetHiveKnightMaxHp,
                1,
                999999,
                10,
                out hiveKnightMaxHpField
            );

            lastY = rowY;
            rowY += HiveKnightHelperRowSpacing;
            CreateToggleRow(
                content,
                "HiveKnightUseCustomPhaseRow",
                "Settings/HiveKnightHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.HiveKnightHelper.hiveKnightUseCustomPhase,
                SetHiveKnightUseCustomPhaseEnabled,
                out hiveKnightUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += HiveKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "HiveKnightPhase2HpRow",
                "Settings/HiveKnightHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase2Hp,
                SetHiveKnightPhase2Hp,
                2,
                999999,
                10,
                out hiveKnightPhase2HpField
            );

            lastY = rowY;
            rowY += HiveKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "HiveKnightPhase3HpRow",
                "Settings/HiveKnightHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.HiveKnightHelper.hiveKnightPhase3Hp,
                SetHiveKnightPhase3Hp,
                1,
                999998,
                10,
                out hiveKnightPhase3HpField
            );

            lastY = rowY;
            rowY += HiveKnightHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "HiveKnightHelperResetRow", "Settings/HiveKnightHelper/Reset".Localize(), resetY, OnHiveKnightHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "HiveKnightHelperBackRow", "Back", backY, OnHiveKnightHelperBackClicked);
        }
    }
}
