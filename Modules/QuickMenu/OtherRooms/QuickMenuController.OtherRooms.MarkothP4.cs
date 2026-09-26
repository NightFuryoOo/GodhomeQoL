using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private const int MarkothP4HelperCanvasSortOrder = 10076;
        private const float MarkothP4HelperPanelHeight = PanelHeight;
        private const float MarkothP4HelperRowSpacing = 44f;
        private GameObject? markothP4HelperRoot;
        private RectTransform? markothP4HelperContent;
        private bool markothP4HelperVisible;
        private Text? markothP4HelperToggleValue;
        private Image? markothP4HelperToggleIcon;
        private Text? markothP4UseMaxHpValue;
        private InputField? markothP4MaxHpField;
        private Text? markothP4UseCustomPhaseValue;
        private InputField? markothP4Phase2HpField;
        private Module? markothP4HelperModule;

        private void BuildMarkothP4HelperOverlayUi()
        {
            markothP4HelperRoot = CreateOverlayFrame("MarkothP4HelperOverlayCanvas", MarkothP4HelperCanvasSortOrder, "MarkothP4HelperPanel", MarkothP4HelperPanelHeight, "Markoth P4", 52, 420f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = MarkothP4HelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, MarkothP4HelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            markothP4HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "MarkothP4EnableRow",
                "Enable Markoth P4",
                rowY,
                GetMarkothP4HelperEnabled,
                SetMarkothP4HelperEnabled,
                out markothP4HelperToggleValue,
                out markothP4HelperToggleIcon);

            rowY += MarkothP4HelperRowSpacing;
            CreateToggleRow(
                content,
                "MarkothP4UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.MarkothP4Helper.markothP4UseMaxHp,
                SetMarkothP4UseMaxHpEnabled,
                out markothP4UseMaxHpValue);
            lastY = rowY;

            rowY += MarkothP4HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MarkothP4MaxHpRow",
                "Markoth P4 Max HP",
                rowY,
                () => Modules.BossChallenge.MarkothP4Helper.markothP4MaxHp,
                SetMarkothP4MaxHp,
                1,
                999999,
                1,
                out markothP4MaxHpField);
            lastY = rowY;

            rowY += MarkothP4HelperRowSpacing;
            CreateToggleRow(
                content,
                "MarkothP4UseCustomPhaseRow",
                "Use Custom Phase",
                rowY,
                () => Modules.BossChallenge.MarkothP4Helper.markothP4UseCustomPhase,
                SetMarkothP4UseCustomPhaseEnabled,
                out markothP4UseCustomPhaseValue);
            lastY = rowY;

            rowY += MarkothP4HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MarkothP4Phase2HpRow",
                "Phase 2 HP",
                rowY,
                () => Modules.BossChallenge.MarkothP4Helper.markothP4Phase2Hp,
                SetMarkothP4Phase2Hp,
                1,
                999999,
                1,
                out markothP4Phase2HpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "MarkothP4ResetRow", "Reset Default", resetY, OnMarkothP4HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "MarkothP4BackRow", "Back", backY, OnMarkothP4HelperBackClicked);
        }

        private Module? GetMarkothP4HelperModule()
        {
            return GetCachedModule(ref markothP4HelperModule, typeof(Modules.BossChallenge.MarkothP4Helper));
        }

        private bool GetMarkothP4HelperEnabled()
        {
            return GetMarkothP4HelperModule()?.Enabled ?? false;
        }

        private void SetMarkothP4HelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetMarkothP4HelperModule(), value);
            UpdateMarkothP4HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetMarkothP4UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.MarkothP4Helper.markothP4UseMaxHp = value;
            Modules.BossChallenge.MarkothP4Helper.ReapplyLiveSettings();
            RefreshMarkothP4HelperUi();
        }

        private void SetMarkothP4MaxHp(int value)
        {
            Modules.BossChallenge.MarkothP4Helper.markothP4MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.MarkothP4Helper.markothP4UseMaxHp)
            {
                Modules.BossChallenge.MarkothP4Helper.ApplyMarkothHealthIfPresent();
            }

            Modules.BossChallenge.MarkothP4Helper.markothP4Phase2Hp = Mathf.Clamp(
                Modules.BossChallenge.MarkothP4Helper.markothP4Phase2Hp,
                1,
                GetMarkothP4Phase2MaxHp());
            if (Modules.BossChallenge.MarkothP4Helper.markothP4UseCustomPhase)
            {
                Modules.BossChallenge.MarkothP4Helper.ReapplyLiveSettings();
            }

            RefreshMarkothP4HelperUi();
        }

        private void SetMarkothP4UseCustomPhaseEnabled(bool value)
        {
            Modules.BossChallenge.MarkothP4Helper.markothP4Phase2Hp = Mathf.Clamp(
                Modules.BossChallenge.MarkothP4Helper.markothP4Phase2Hp,
                1,
                GetMarkothP4Phase2MaxHp());
            Modules.BossChallenge.MarkothP4Helper.markothP4UseCustomPhase = value;
            Modules.BossChallenge.MarkothP4Helper.ReapplyLiveSettings();
            RefreshMarkothP4HelperUi();
        }

        private void SetMarkothP4Phase2Hp(int value)
        {
            Modules.BossChallenge.MarkothP4Helper.markothP4Phase2Hp = Mathf.Clamp(value, 1, GetMarkothP4Phase2MaxHp());
            if (Modules.BossChallenge.MarkothP4Helper.markothP4UseCustomPhase)
            {
                Modules.BossChallenge.MarkothP4Helper.ReapplyLiveSettings();
            }

            RefreshMarkothP4HelperUi();
        }

        private int GetMarkothP4Phase2MaxHp()
        {
            return Mathf.Max(1, Modules.BossChallenge.MarkothP4Helper.GetPhase2MaxHpForUi());
        }

        private void RefreshMarkothP4HelperUi()
        {
            Module? module = GetMarkothP4HelperModule();
            UpdateToggleValue(markothP4HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(markothP4HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(markothP4UseMaxHpValue, Modules.BossChallenge.MarkothP4Helper.markothP4UseMaxHp);
            UpdateToggleValue(markothP4UseCustomPhaseValue, Modules.BossChallenge.MarkothP4Helper.markothP4UseCustomPhase);
            UpdateIntInputValue(markothP4MaxHpField, Modules.BossChallenge.MarkothP4Helper.markothP4MaxHp);
            UpdateIntInputValue(markothP4Phase2HpField, Modules.BossChallenge.MarkothP4Helper.markothP4Phase2Hp);

            UpdateMarkothP4HelperInteractivity();
        }

        private void UpdateMarkothP4HelperInteractivity()
        {
            SetContentInteractivity(markothP4HelperContent, GetMarkothP4HelperEnabled(), "MarkothP4EnableRow");
            if (!GetMarkothP4HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(markothP4HelperContent, "MarkothP4MaxHpRow", Modules.BossChallenge.MarkothP4Helper.markothP4UseMaxHp);
            SetRowInteractivity(markothP4HelperContent, "MarkothP4Phase2HpRow", Modules.BossChallenge.MarkothP4Helper.markothP4UseCustomPhase);
        }

        private void SetMarkothP4HelperVisible(bool value)
        {
            markothP4HelperVisible = value;
            ApplyPanelVisible(markothP4HelperRoot, value, RefreshMarkothP4HelperUi);
        }

        private static void ResetMarkothP4HelperDefaults()
        {
            Modules.BossChallenge.MarkothP4Helper.markothP4UseMaxHp = false;
            Modules.BossChallenge.MarkothP4Helper.markothP4MaxHp = 650;
            Modules.BossChallenge.MarkothP4Helper.markothP4UseCustomPhase = false;
            Modules.BossChallenge.MarkothP4Helper.markothP4Phase2Hp = 325;
        }

        private void OnMarkothP4HelperBackClicked()
        {
            bool reopenOtherRooms = returnToBossManipulateOtherRoomsOnClose;
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOtherRoomsOnClose = false;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenOtherRooms)
            {
                returnToBossManipulateOnClose = true;
                SetBossManipulateOtherRoomsVisible(true);
            }
            else if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetMarkothP4HelperVisible(false);
        }

        private void OnMarkothP4HelperResetDefaultsClicked()
        {
            ResetMarkothP4HelperDefaults();
            SetMarkothP4HelperEnabled(false);
            Modules.BossChallenge.MarkothP4Helper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.MarkothP4Helper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshMarkothP4HelperUi();
        }
    }
}
