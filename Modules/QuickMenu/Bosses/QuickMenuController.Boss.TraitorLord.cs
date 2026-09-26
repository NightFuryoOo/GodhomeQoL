using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int TraitorLordHelperCanvasSortOrder = 10045;
        private const float TraitorLordHelperPanelHeight = PanelHeight;
        private const float TraitorLordHelperRowSpacing = 44f;
        private RectTransform? traitorLordHelperContent;
        private GameObject? traitorLordHelperRoot;
        private bool traitorLordHelperVisible;
        private Text? traitorLordHelperToggleValue;
        private Image? traitorLordHelperToggleIcon;
        private Text? traitorLordP5HpValue;
        private Text? traitorLordUseMaxHpValue;
        private Text? traitorLordUseCustomPhaseValue;
        private InputField? traitorLordMaxHpField;
        private InputField? traitorLordPhase2HpField;
        private Module? traitorLordHelperModule;

        private static void ResetTraitorLordHelperDefaults()
        {
            Modules.BossChallenge.TraitorLordHelper.traitorLordUseMaxHp = false;
            Modules.BossChallenge.TraitorLordHelper.traitorLordP5Hp = false;
            Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhase = false;
            Modules.BossChallenge.TraitorLordHelper.traitorLordMaxHp = 1300;
            Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp = 500;
            Modules.BossChallenge.TraitorLordHelper.traitorLordMaxHpBeforeP5 = 1300;
            Modules.BossChallenge.TraitorLordHelper.traitorLordUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2HpBeforeP5 = 500;
            Modules.BossChallenge.TraitorLordHelper.traitorLordHasStoredStateBeforeP5 = false;
        }

        private Module? GetTraitorLordHelperModule()
        {
            return GetCachedModule(ref traitorLordHelperModule, typeof(Modules.BossChallenge.TraitorLordHelper));
        }

        private bool GetTraitorLordHelperEnabled()
        {
            return GetTraitorLordHelperModule()?.Enabled ?? false;
        }

        private void SetTraitorLordHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetTraitorLordHelperModule(), value);
            UpdateTraitorLordHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetTraitorLordUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.TraitorLordHelper.traitorLordP5Hp)
            {
                Modules.BossChallenge.TraitorLordHelper.traitorLordUseMaxHp = true;
                RefreshTraitorLordHelperUi();
                return;
            }

            Modules.BossChallenge.TraitorLordHelper.traitorLordUseMaxHp = value;
            Modules.BossChallenge.TraitorLordHelper.ReapplyLiveSettings();
            RefreshTraitorLordHelperUi();
        }

        private void SetTraitorLordUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.TraitorLordHelper.traitorLordP5Hp)
            {
                Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhase = false;
                RefreshTraitorLordHelperUi();
                return;
            }

            int maxPhase2Hp = GetTraitorLordPhase2MaxHp();
            Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp =
                Mathf.Clamp(Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp, 1, maxPhase2Hp);
            Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhase = value;
            Modules.BossChallenge.TraitorLordHelper.ReapplyLiveSettings();
            RefreshTraitorLordHelperUi();
        }

        private void SetTraitorLordP5HpEnabled(bool value)
        {
            Modules.BossChallenge.TraitorLordHelper.SetP5HpEnabled(value);
            RefreshTraitorLordHelperUi();
        }

        private void SetTraitorLordMaxHp(int value)
        {
            if (Modules.BossChallenge.TraitorLordHelper.traitorLordP5Hp)
            {
                Modules.BossChallenge.TraitorLordHelper.traitorLordMaxHp = 800;
                RefreshTraitorLordHelperUi();
                return;
            }

            Modules.BossChallenge.TraitorLordHelper.traitorLordMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.TraitorLordHelper.traitorLordUseMaxHp)
            {
                Modules.BossChallenge.TraitorLordHelper.ApplyTraitorLordHealthIfPresent();
            }

            int maxPhase2Hp = GetTraitorLordPhase2MaxHp();
            Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp =
                Mathf.Clamp(Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp, 1, maxPhase2Hp);
            if (Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhase)
            {
                Modules.BossChallenge.TraitorLordHelper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshTraitorLordHelperUi();
        }

        private void SetTraitorLordPhase2Hp(int value)
        {
            if (Modules.BossChallenge.TraitorLordHelper.traitorLordP5Hp)
            {
                Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp = 500;
                RefreshTraitorLordHelperUi();
                return;
            }

            int maxPhase2Hp = GetTraitorLordPhase2MaxHp();
            Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp = Mathf.Clamp(value, 1, maxPhase2Hp);
            if (Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhase)
            {
                Modules.BossChallenge.TraitorLordHelper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshTraitorLordHelperUi();
        }

        private int GetTraitorLordPhase2MaxHp()
        {
            return Mathf.Max(1, Modules.BossChallenge.TraitorLordHelper.GetPhase2MaxHpForUi());
        }

        private void RefreshTraitorLordHelperUi()
        {
            Module? module = GetTraitorLordHelperModule();
            UpdateToggleValue(traitorLordHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(traitorLordHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(traitorLordP5HpValue, Modules.BossChallenge.TraitorLordHelper.traitorLordP5Hp);
            UpdateToggleValue(traitorLordUseMaxHpValue, Modules.BossChallenge.TraitorLordHelper.traitorLordUseMaxHp);
            UpdateToggleValue(traitorLordUseCustomPhaseValue, Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhase);
            UpdateIntInputValue(traitorLordMaxHpField, Modules.BossChallenge.TraitorLordHelper.traitorLordMaxHp);
            UpdateIntInputValue(traitorLordPhase2HpField, Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp);

            UpdateTraitorLordHelperInteractivity();
        }

        private void UpdateTraitorLordHelperInteractivity()
        {
            SetContentInteractivity(traitorLordHelperContent, GetTraitorLordHelperEnabled(), "TraitorLordHelperEnableRow");
            if (!GetTraitorLordHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.TraitorLordHelper.traitorLordUseMaxHp;
            bool p5Hp = Modules.BossChallenge.TraitorLordHelper.traitorLordP5Hp;
            bool useCustomPhase = Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhase;
            SetRowInteractivity(traitorLordHelperContent, "TraitorLordUseMaxHpRow", !p5Hp);
            SetRowInteractivity(traitorLordHelperContent, "TraitorLordMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(traitorLordHelperContent, "TraitorLordUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(traitorLordHelperContent, "TraitorLordPhase2HpRow", useCustomPhase && !p5Hp);
        }

        private void SetTraitorLordHelperVisible(bool value)
        {
            traitorLordHelperVisible = value;
            ApplyPanelVisible(traitorLordHelperRoot, value, RefreshTraitorLordHelperUi);
        }

        private void OnBossManipulateTraitorLordClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetTraitorLordHelperVisible);
        }

        private void OnTraitorLordHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetTraitorLordHelperVisible);
        }

        private void OnTraitorLordHelperResetDefaultsClicked()
        {
            ResetTraitorLordHelperDefaults();
            SetTraitorLordHelperEnabled(false);
            Modules.BossChallenge.TraitorLordHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.TraitorLordHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshTraitorLordHelperUi();
        }

        private void BuildTraitorLordHelperOverlayUi()
        {
            traitorLordHelperRoot = CreateOverlayFrame("TraitorLordHelperOverlayCanvas", TraitorLordHelperCanvasSortOrder, "TraitorLordHelperPanel", TraitorLordHelperPanelHeight, "Modules/TraitorLordHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = TraitorLordHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, TraitorLordHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            traitorLordHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "TraitorLordHelperEnableRow",
                "Settings/TraitorLordHelper/Enable".Localize(),
                rowY,
                GetTraitorLordHelperEnabled,
                SetTraitorLordHelperEnabled,
                out traitorLordHelperToggleValue,
                out traitorLordHelperToggleIcon
            );

            lastY = rowY;
            rowY += TraitorLordHelperRowSpacing;
            CreateToggleRow(
                content,
                "TraitorLordP5HpRow",
                "Settings/TraitorLordHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.TraitorLordHelper.traitorLordP5Hp,
                SetTraitorLordP5HpEnabled,
                out traitorLordP5HpValue
            );

            lastY = rowY;
            rowY += TraitorLordHelperRowSpacing;
            CreateToggleRow(
                content,
                "TraitorLordUseMaxHpRow",
                "Settings/TraitorLordHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.TraitorLordHelper.traitorLordUseMaxHp,
                SetTraitorLordUseMaxHpEnabled,
                out traitorLordUseMaxHpValue
            );

            lastY = rowY;
            rowY += TraitorLordHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "TraitorLordMaxHpRow",
                "Settings/TraitorLordHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.TraitorLordHelper.traitorLordMaxHp,
                SetTraitorLordMaxHp,
                1,
                999999,
                10,
                out traitorLordMaxHpField
            );

            lastY = rowY;
            rowY += TraitorLordHelperRowSpacing;
            CreateToggleRow(
                content,
                "TraitorLordUseCustomPhaseRow",
                "Settings/TraitorLordHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.TraitorLordHelper.traitorLordUseCustomPhase,
                SetTraitorLordUseCustomPhaseEnabled,
                out traitorLordUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += TraitorLordHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "TraitorLordPhase2HpRow",
                "Settings/TraitorLordHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.TraitorLordHelper.traitorLordPhase2Hp,
                SetTraitorLordPhase2Hp,
                1,
                999999,
                10,
                out traitorLordPhase2HpField
            );

            lastY = rowY;
            rowY += TraitorLordHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "TraitorLordHelperResetRow", "Settings/TraitorLordHelper/Reset".Localize(), resetY, OnTraitorLordHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "TraitorLordHelperBackRow", "Back", backY, OnTraitorLordHelperBackClicked);
        }
    }
}
