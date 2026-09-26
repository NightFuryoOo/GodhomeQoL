using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private const int NoEyesP4HelperCanvasSortOrder = 10073;
        private const float NoEyesP4HelperPanelHeight = PanelHeight;
        private const float NoEyesP4HelperRowSpacing = 44f;
        private GameObject? noEyesP4HelperRoot;
        private RectTransform? noEyesP4HelperContent;
        private bool noEyesP4HelperVisible;
        private Text? noEyesP4HelperToggleValue;
        private Image? noEyesP4HelperToggleIcon;
        private Text? noEyesP4UseMaxHpValue;
        private InputField? noEyesP4MaxHpField;
        private Text? noEyesP4UseCustomPhaseValue;
        private InputField? noEyesP4Phase2HpField;
        private InputField? noEyesP4Phase3HpField;
        private Module? noEyesP4HelperModule;

        private void BuildNoEyesP4HelperOverlayUi()
        {
            noEyesP4HelperRoot = CreateOverlayFrame("NoEyesP4HelperOverlayCanvas", NoEyesP4HelperCanvasSortOrder, "NoEyesP4HelperPanel", NoEyesP4HelperPanelHeight, "No Eyes P4", 52, 420f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = NoEyesP4HelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, NoEyesP4HelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            noEyesP4HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "NoEyesP4EnableRow",
                "Enable No Eyes P4",
                rowY,
                GetNoEyesP4HelperEnabled,
                SetNoEyesP4HelperEnabled,
                out noEyesP4HelperToggleValue,
                out noEyesP4HelperToggleIcon);

            rowY += NoEyesP4HelperRowSpacing;
            CreateToggleRow(
                content,
                "NoEyesP4UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseMaxHp,
                SetNoEyesP4UseMaxHpEnabled,
                out noEyesP4UseMaxHpValue);
            lastY = rowY;

            rowY += NoEyesP4HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoEyesP4MaxHpRow",
                "No Eyes P4 Max HP",
                rowY,
                () => Modules.BossChallenge.NoEyesP4Helper.noEyesP4MaxHp,
                SetNoEyesP4MaxHp,
                1,
                999999,
                1,
                out noEyesP4MaxHpField);
            lastY = rowY;

            rowY += NoEyesP4HelperRowSpacing;
            CreateToggleRow(
                content,
                "NoEyesP4UseCustomPhaseRow",
                "Use Custom Phase",
                rowY,
                () => Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase,
                SetNoEyesP4UseCustomPhaseEnabled,
                out noEyesP4UseCustomPhaseValue);
            lastY = rowY;

            rowY += NoEyesP4HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoEyesP4Phase2HpRow",
                "Phase 2 HP",
                rowY,
                () => Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase2Hp,
                SetNoEyesP4Phase2Hp,
                1,
                999999,
                1,
                out noEyesP4Phase2HpField);
            lastY = rowY;

            rowY += NoEyesP4HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoEyesP4Phase3HpRow",
                "Phase 3 HP",
                rowY,
                () => Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase3Hp,
                SetNoEyesP4Phase3Hp,
                1,
                999999,
                1,
                out noEyesP4Phase3HpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "NoEyesP4ResetRow", "Reset Default", resetY, OnNoEyesP4HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "NoEyesP4BackRow", "Back", backY, OnNoEyesP4HelperBackClicked);
        }

        private Module? GetNoEyesP4HelperModule()
        {
            return GetCachedModule(ref noEyesP4HelperModule, typeof(Modules.BossChallenge.NoEyesP4Helper));
        }

        private bool GetNoEyesP4HelperEnabled()
        {
            return GetNoEyesP4HelperModule()?.Enabled ?? false;
        }

        private void SetNoEyesP4HelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetNoEyesP4HelperModule(), value);
            UpdateNoEyesP4HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetNoEyesP4UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseMaxHp = value;
            Modules.BossChallenge.NoEyesP4Helper.ReapplyLiveSettings();
            RefreshNoEyesP4HelperUi();
        }

        private void SetNoEyesP4MaxHp(int value)
        {
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4MaxHp = Mathf.Clamp(value, 1, 999999);
            NormalizeNoEyesP4PhaseThresholdInputs();
            if (Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseMaxHp || Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase)
            {
                Modules.BossChallenge.NoEyesP4Helper.ReapplyLiveSettings();
            }

            RefreshNoEyesP4HelperUi();
        }

        private void SetNoEyesP4UseCustomPhaseEnabled(bool value)
        {
            NormalizeNoEyesP4PhaseThresholdInputs();
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase = value;
            Modules.BossChallenge.NoEyesP4Helper.ReapplyLiveSettings();
            RefreshNoEyesP4HelperUi();
        }

        private void SetNoEyesP4Phase2Hp(int value)
        {
            int phase2Hp = Mathf.Clamp(value, 2, GetNoEyesP4Phase2MaxHp());
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase2Hp = phase2Hp;
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase3Hp =
                Mathf.Clamp(Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase3Hp, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase)
            {
                Modules.BossChallenge.NoEyesP4Helper.ReapplyLiveSettings();
            }

            RefreshNoEyesP4HelperUi();
        }

        private void SetNoEyesP4Phase3Hp(int value)
        {
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase2Hp, 2, GetNoEyesP4Phase2MaxHp());
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase2Hp = phase2Hp;
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase3Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase)
            {
                Modules.BossChallenge.NoEyesP4Helper.ReapplyLiveSettings();
            }

            RefreshNoEyesP4HelperUi();
        }

        private int GetNoEyesP4Phase2MaxHp()
        {
            return Mathf.Clamp(Modules.BossChallenge.NoEyesP4Helper.GetPhase2MaxHpForUi(), 2, 999999);
        }

        private void NormalizeNoEyesP4PhaseThresholdInputs()
        {
            int phase2MaxHp = GetNoEyesP4Phase2MaxHp();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase2Hp, 2, phase2MaxHp);
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase2Hp = phase2Hp;
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase3Hp =
                Mathf.Clamp(Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
        }

        private void RefreshNoEyesP4HelperUi()
        {
            Module? module = GetNoEyesP4HelperModule();
            UpdateToggleValue(noEyesP4HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(noEyesP4HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(noEyesP4UseMaxHpValue, Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseMaxHp);
            UpdateToggleValue(noEyesP4UseCustomPhaseValue, Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase);
            UpdateIntInputValue(noEyesP4MaxHpField, Modules.BossChallenge.NoEyesP4Helper.noEyesP4MaxHp);
            UpdateIntInputValue(noEyesP4Phase2HpField, Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase2Hp);
            UpdateIntInputValue(noEyesP4Phase3HpField, Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase3Hp);

            UpdateNoEyesP4HelperInteractivity();
        }

        private void UpdateNoEyesP4HelperInteractivity()
        {
            SetContentInteractivity(noEyesP4HelperContent, GetNoEyesP4HelperEnabled(), "NoEyesP4EnableRow");
            if (!GetNoEyesP4HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(noEyesP4HelperContent, "NoEyesP4MaxHpRow", Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseMaxHp);
            SetRowInteractivity(noEyesP4HelperContent, "NoEyesP4Phase2HpRow", Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase);
            SetRowInteractivity(noEyesP4HelperContent, "NoEyesP4Phase3HpRow", Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase);
        }

        private void SetNoEyesP4HelperVisible(bool value)
        {
            noEyesP4HelperVisible = value;
            ApplyPanelVisible(noEyesP4HelperRoot, value, RefreshNoEyesP4HelperUi);
        }

        private static void ResetNoEyesP4HelperDefaults()
        {
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseMaxHp = false;
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4MaxHp = 570;
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase = false;
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase2Hp = 150;
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase3Hp = 90;
        }

        private void OnNoEyesP4HelperBackClicked()
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

            SetNoEyesP4HelperVisible(false);
        }

        private void OnNoEyesP4HelperResetDefaultsClicked()
        {
            ResetNoEyesP4HelperDefaults();
            SetNoEyesP4HelperEnabled(false);
            Modules.BossChallenge.NoEyesP4Helper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.NoEyesP4Helper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshNoEyesP4HelperUi();
        }
    }
}
