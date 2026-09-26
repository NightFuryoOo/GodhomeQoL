using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int NightmareKingGrimmHelperCanvasSortOrder = 10047;
        private const float NightmareKingGrimmHelperPanelHeight = PanelHeight;
        private const float NightmareKingGrimmHelperRowSpacing = 44f;
        private RectTransform? nightmareKingGrimmHelperContent;
        private GameObject? nightmareKingGrimmHelperRoot;
        private bool nightmareKingGrimmHelperVisible;
        private Text? nightmareKingGrimmHelperToggleValue;
        private Image? nightmareKingGrimmHelperToggleIcon;
        private Text? nightmareKingGrimmP5HpValue;
        private Text? nightmareKingGrimmUseMaxHpValue;
        private Text? nightmareKingGrimmUseCustomPhaseValue;
        private InputField? nightmareKingGrimmMaxHpField;
        private InputField? nightmareKingGrimmRagePhase1HpField;
        private InputField? nightmareKingGrimmRagePhase2HpField;
        private InputField? nightmareKingGrimmRagePhase3HpField;
        private Module? nightmareKingGrimmHelperModule;

        private static void ResetNightmareKingGrimmHelperDefaults()
        {
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseMaxHp = false;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp = false;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase = false;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmMaxHp = 1650;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1Hp = 1238;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2Hp = 826;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3Hp = 414;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmMaxHpBeforeP5 = 1650;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1HpBeforeP5 = 1238;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2HpBeforeP5 = 826;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3HpBeforeP5 = 414;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmHasStoredStateBeforeP5 = false;
        }

        private Module? GetNightmareKingGrimmHelperModule()
        {
            return GetCachedModule(ref nightmareKingGrimmHelperModule, typeof(Modules.BossChallenge.NightmareKingGrimmHelper));
        }

        private bool GetNightmareKingGrimmHelperEnabled()
        {
            return GetNightmareKingGrimmHelperModule()?.Enabled ?? false;
        }

        private void SetNightmareKingGrimmHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetNightmareKingGrimmHelperModule(), value);
            UpdateNightmareKingGrimmHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetNightmareKingGrimmUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp)
            {
                Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseMaxHp = true;
                RefreshNightmareKingGrimmHelperUi();
                return;
            }

            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseMaxHp = value;
            Modules.BossChallenge.NightmareKingGrimmHelper.ReapplyLiveSettings();
            RefreshNightmareKingGrimmHelperUi();
        }

        private void SetNightmareKingGrimmUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp)
            {
                Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase = false;
                RefreshNightmareKingGrimmHelperUi();
                return;
            }

            ClampNightmareKingGrimmPhaseThresholdsForUi();
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase = value;
            Modules.BossChallenge.NightmareKingGrimmHelper.ReapplyLiveSettings();
            RefreshNightmareKingGrimmHelperUi();
        }

        private void SetNightmareKingGrimmP5HpEnabled(bool value)
        {
            Modules.BossChallenge.NightmareKingGrimmHelper.SetP5HpEnabled(value);
            RefreshNightmareKingGrimmHelperUi();
        }

        private void ClampNightmareKingGrimmPhaseThresholdsForUi()
        {
            int ragePhase1Hp = Mathf.Clamp(
                Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1Hp,
                3,
                Modules.BossChallenge.NightmareKingGrimmHelper.GetRagePhase1MaxHpForUi()
            );
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1Hp = ragePhase1Hp;

            int ragePhase2Hp = Mathf.Clamp(
                Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2Hp,
                2,
                Modules.BossChallenge.NightmareKingGrimmHelper.GetRagePhase2MaxHpForUi()
            );
            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2Hp = ragePhase2Hp;

            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3Hp = Mathf.Clamp(
                Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3Hp,
                1,
                Modules.BossChallenge.NightmareKingGrimmHelper.GetRagePhase3MaxHpForUi()
            );
        }

        private void SetNightmareKingGrimmMaxHp(int value)
        {
            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp)
            {
                Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmMaxHp = 1250;
                RefreshNightmareKingGrimmHelperUi();
                return;
            }

            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmMaxHp = Mathf.Clamp(value, 1, 999999);
            ClampNightmareKingGrimmPhaseThresholdsForUi();
            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseMaxHp
                || Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase)
            {
                Modules.BossChallenge.NightmareKingGrimmHelper.ReapplyLiveSettings();
            }

            RefreshNightmareKingGrimmHelperUi();
        }

        private void SetNightmareKingGrimmRagePhase1Hp(int value)
        {
            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp)
            {
                RefreshNightmareKingGrimmHelperUi();
                return;
            }

            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1Hp = value;
            ClampNightmareKingGrimmPhaseThresholdsForUi();

            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase)
            {
                Modules.BossChallenge.NightmareKingGrimmHelper.ReapplyLiveSettings();
            }

            RefreshNightmareKingGrimmHelperUi();
        }

        private void SetNightmareKingGrimmRagePhase2Hp(int value)
        {
            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp)
            {
                RefreshNightmareKingGrimmHelperUi();
                return;
            }

            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2Hp = value;
            ClampNightmareKingGrimmPhaseThresholdsForUi();

            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase)
            {
                Modules.BossChallenge.NightmareKingGrimmHelper.ReapplyLiveSettings();
            }

            RefreshNightmareKingGrimmHelperUi();
        }

        private void SetNightmareKingGrimmRagePhase3Hp(int value)
        {
            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp)
            {
                RefreshNightmareKingGrimmHelperUi();
                return;
            }

            Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3Hp = value;
            ClampNightmareKingGrimmPhaseThresholdsForUi();

            if (Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase)
            {
                Modules.BossChallenge.NightmareKingGrimmHelper.ReapplyLiveSettings();
            }

            RefreshNightmareKingGrimmHelperUi();
        }

        private void RefreshNightmareKingGrimmHelperUi()
        {
            Module? module = GetNightmareKingGrimmHelperModule();
            UpdateToggleValue(nightmareKingGrimmHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(nightmareKingGrimmHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(nightmareKingGrimmP5HpValue, Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp);
            UpdateToggleValue(nightmareKingGrimmUseMaxHpValue, Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseMaxHp);
            UpdateToggleValue(nightmareKingGrimmUseCustomPhaseValue, Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase);
            UpdateIntInputValue(nightmareKingGrimmMaxHpField, Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmMaxHp);
            UpdateIntInputValue(nightmareKingGrimmRagePhase1HpField, Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1Hp);
            UpdateIntInputValue(nightmareKingGrimmRagePhase2HpField, Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2Hp);
            UpdateIntInputValue(nightmareKingGrimmRagePhase3HpField, Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3Hp);

            UpdateNightmareKingGrimmHelperInteractivity();
        }

        private void UpdateNightmareKingGrimmHelperInteractivity()
        {
            SetContentInteractivity(nightmareKingGrimmHelperContent, GetNightmareKingGrimmHelperEnabled(), "NightmareKingGrimmHelperEnableRow");
            if (!GetNightmareKingGrimmHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseMaxHp;
            bool p5Hp = Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp;
            bool useCustomPhase = Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase;
            SetRowInteractivity(nightmareKingGrimmHelperContent, "NightmareKingGrimmUseMaxHpRow", !p5Hp);
            SetRowInteractivity(nightmareKingGrimmHelperContent, "NightmareKingGrimmMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(nightmareKingGrimmHelperContent, "NightmareKingGrimmUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(nightmareKingGrimmHelperContent, "NightmareKingGrimmRagePhase1HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(nightmareKingGrimmHelperContent, "NightmareKingGrimmRagePhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(nightmareKingGrimmHelperContent, "NightmareKingGrimmRagePhase3HpRow", useCustomPhase && !p5Hp);
        }

        private void SetNightmareKingGrimmHelperVisible(bool value)
        {
            nightmareKingGrimmHelperVisible = value;
            ApplyPanelVisible(nightmareKingGrimmHelperRoot, value, RefreshNightmareKingGrimmHelperUi);
        }

        private void OnBossManipulateNightmareKingGrimmClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetNightmareKingGrimmHelperVisible);
        }

        private void OnNightmareKingGrimmHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetNightmareKingGrimmHelperVisible);
        }

        private void OnNightmareKingGrimmHelperResetDefaultsClicked()
        {
            ResetNightmareKingGrimmHelperDefaults();
            SetNightmareKingGrimmHelperEnabled(false);
            Modules.BossChallenge.NightmareKingGrimmHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.NightmareKingGrimmHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshNightmareKingGrimmHelperUi();
        }

        private void BuildNightmareKingGrimmHelperOverlayUi()
        {
            nightmareKingGrimmHelperRoot = CreateOverlayFrame("NightmareKingGrimmHelperOverlayCanvas", NightmareKingGrimmHelperCanvasSortOrder, "NightmareKingGrimmHelperPanel", NightmareKingGrimmHelperPanelHeight, "Modules/NightmareKingGrimmHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = NightmareKingGrimmHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, NightmareKingGrimmHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            nightmareKingGrimmHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "NightmareKingGrimmHelperEnableRow",
                "Settings/NightmareKingGrimmHelper/Enable".Localize(),
                rowY,
                GetNightmareKingGrimmHelperEnabled,
                SetNightmareKingGrimmHelperEnabled,
                out nightmareKingGrimmHelperToggleValue,
                out nightmareKingGrimmHelperToggleIcon
            );

            lastY = rowY;
            rowY += NightmareKingGrimmHelperRowSpacing;
            CreateToggleRow(
                content,
                "NightmareKingGrimmP5HpRow",
                "Settings/NightmareKingGrimmHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmP5Hp,
                SetNightmareKingGrimmP5HpEnabled,
                out nightmareKingGrimmP5HpValue
            );

            lastY = rowY;
            rowY += NightmareKingGrimmHelperRowSpacing;
            CreateToggleRow(
                content,
                "NightmareKingGrimmUseMaxHpRow",
                "Settings/NightmareKingGrimmHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseMaxHp,
                SetNightmareKingGrimmUseMaxHpEnabled,
                out nightmareKingGrimmUseMaxHpValue
            );

            lastY = rowY;
            rowY += NightmareKingGrimmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NightmareKingGrimmMaxHpRow",
                "Settings/NightmareKingGrimmHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmMaxHp,
                SetNightmareKingGrimmMaxHp,
                1,
                999999,
                10,
                out nightmareKingGrimmMaxHpField
            );

            lastY = rowY;
            rowY += NightmareKingGrimmHelperRowSpacing;
            CreateToggleRow(
                content,
                "NightmareKingGrimmUseCustomPhaseRow",
                "Settings/NightmareKingGrimmHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmUseCustomPhase,
                SetNightmareKingGrimmUseCustomPhaseEnabled,
                out nightmareKingGrimmUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += NightmareKingGrimmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NightmareKingGrimmRagePhase1HpRow",
                "Settings/NightmareKingGrimmHelper/RagePhase1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase1Hp,
                SetNightmareKingGrimmRagePhase1Hp,
                3,
                999999,
                10,
                out nightmareKingGrimmRagePhase1HpField
            );

            lastY = rowY;
            rowY += NightmareKingGrimmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NightmareKingGrimmRagePhase2HpRow",
                "Settings/NightmareKingGrimmHelper/RagePhase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase2Hp,
                SetNightmareKingGrimmRagePhase2Hp,
                2,
                999998,
                10,
                out nightmareKingGrimmRagePhase2HpField
            );

            lastY = rowY;
            rowY += NightmareKingGrimmHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NightmareKingGrimmRagePhase3HpRow",
                "Settings/NightmareKingGrimmHelper/RagePhase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.NightmareKingGrimmHelper.nightmareKingGrimmRagePhase3Hp,
                SetNightmareKingGrimmRagePhase3Hp,
                1,
                999997,
                10,
                out nightmareKingGrimmRagePhase3HpField
            );

            lastY = rowY;
            rowY += NightmareKingGrimmHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "NightmareKingGrimmHelperResetRow", "Settings/NightmareKingGrimmHelper/Reset".Localize(), resetY, OnNightmareKingGrimmHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "NightmareKingGrimmHelperBackRow", "Back", backY, OnNightmareKingGrimmHelperBackClicked);
        }
    }
}
