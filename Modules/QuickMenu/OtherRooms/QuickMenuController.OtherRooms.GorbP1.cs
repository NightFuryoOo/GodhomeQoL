using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private const int GorbP1HelperCanvasSortOrder = 10077;
        private const float GorbP1HelperPanelHeight = PanelHeight;
        private const float GorbP1HelperRowSpacing = 44f;
        private GameObject? gorbP1HelperRoot;
        private RectTransform? gorbP1HelperContent;
        private bool gorbP1HelperVisible;
        private Text? gorbP1HelperToggleValue;
        private Image? gorbP1HelperToggleIcon;
        private Text? gorbP1UseMaxHpValue;
        private InputField? gorbP1MaxHpField;
        private Text? gorbP1UseCustomPhaseValue;
        private InputField? gorbP1Phase2HpField;
        private InputField? gorbP1Phase3HpField;
        private Module? gorbP1HelperModule;

        private void BuildGorbP1HelperOverlayUi()
        {
            gorbP1HelperRoot = CreateOverlayFrame("GorbP1HelperOverlayCanvas", GorbP1HelperCanvasSortOrder, "GorbP1HelperPanel", GorbP1HelperPanelHeight, "Gorb P1", 52, 420f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = GorbP1HelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, GorbP1HelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            gorbP1HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "GorbP1EnableRow",
                "Enable Gorb P1",
                rowY,
                GetGorbP1HelperEnabled,
                SetGorbP1HelperEnabled,
                out gorbP1HelperToggleValue,
                out gorbP1HelperToggleIcon);

            rowY += GorbP1HelperRowSpacing;
            CreateToggleRow(
                content,
                "GorbP1UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.GorbP1Helper.gorbP1UseMaxHp,
                SetGorbP1UseMaxHpEnabled,
                out gorbP1UseMaxHpValue);
            lastY = rowY;

            rowY += GorbP1HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GorbP1MaxHpRow",
                "Gorb P1 Max HP",
                rowY,
                () => Modules.BossChallenge.GorbP1Helper.gorbP1MaxHp,
                SetGorbP1MaxHp,
                1,
                999999,
                1,
                out gorbP1MaxHpField);
            lastY = rowY;

            rowY += GorbP1HelperRowSpacing;
            CreateToggleRow(
                content,
                "GorbP1UseCustomPhaseRow",
                "Use Custom Phase",
                rowY,
                () => Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase,
                SetGorbP1UseCustomPhaseEnabled,
                out gorbP1UseCustomPhaseValue);
            lastY = rowY;

            rowY += GorbP1HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GorbP1Phase2HpRow",
                "Phase 2 HP",
                rowY,
                () => Modules.BossChallenge.GorbP1Helper.gorbP1Phase2Hp,
                SetGorbP1Phase2Hp,
                1,
                999999,
                1,
                out gorbP1Phase2HpField);
            lastY = rowY;

            rowY += GorbP1HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GorbP1Phase3HpRow",
                "Phase 3 HP",
                rowY,
                () => Modules.BossChallenge.GorbP1Helper.gorbP1Phase3Hp,
                SetGorbP1Phase3Hp,
                1,
                999999,
                1,
                out gorbP1Phase3HpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "GorbP1ResetRow", "Reset Default", resetY, OnGorbP1HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "GorbP1BackRow", "Back", backY, OnGorbP1HelperBackClicked);
        }

        private Module? GetGorbP1HelperModule()
        {
            return GetCachedModule(ref gorbP1HelperModule, typeof(Modules.BossChallenge.GorbP1Helper));
        }

        private bool GetGorbP1HelperEnabled()
        {
            return GetGorbP1HelperModule()?.Enabled ?? false;
        }

        private void SetGorbP1HelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetGorbP1HelperModule(), value);
            UpdateGorbP1HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetGorbP1UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.GorbP1Helper.gorbP1UseMaxHp = value;
            Modules.BossChallenge.GorbP1Helper.ReapplyLiveSettings();
            RefreshGorbP1HelperUi();
        }

        private void SetGorbP1MaxHp(int value)
        {
            Modules.BossChallenge.GorbP1Helper.gorbP1MaxHp = Mathf.Clamp(value, 1, 999999);
            NormalizeGorbP1PhaseThresholdInputs();
            if (Modules.BossChallenge.GorbP1Helper.gorbP1UseMaxHp)
            {
                Modules.BossChallenge.GorbP1Helper.ApplyGorbHealthIfPresent();
            }

            if (Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase)
            {
                Modules.BossChallenge.GorbP1Helper.ReapplyLiveSettings();
            }

            RefreshGorbP1HelperUi();
        }

        private void SetGorbP1UseCustomPhaseEnabled(bool value)
        {
            NormalizeGorbP1PhaseThresholdInputs();
            Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase = value;
            Modules.BossChallenge.GorbP1Helper.ReapplyLiveSettings();
            RefreshGorbP1HelperUi();
        }

        private void SetGorbP1Phase2Hp(int value)
        {
            int phase2Hp = Mathf.Clamp(value, 2, GetGorbP1Phase2MaxHp());
            Modules.BossChallenge.GorbP1Helper.gorbP1Phase2Hp = phase2Hp;
            Modules.BossChallenge.GorbP1Helper.gorbP1Phase3Hp =
                Mathf.Clamp(Modules.BossChallenge.GorbP1Helper.gorbP1Phase3Hp, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase)
            {
                Modules.BossChallenge.GorbP1Helper.ReapplyLiveSettings();
            }

            RefreshGorbP1HelperUi();
        }

        private void SetGorbP1Phase3Hp(int value)
        {
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.GorbP1Helper.gorbP1Phase2Hp, 2, GetGorbP1Phase2MaxHp());
            Modules.BossChallenge.GorbP1Helper.gorbP1Phase2Hp = phase2Hp;
            Modules.BossChallenge.GorbP1Helper.gorbP1Phase3Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase)
            {
                Modules.BossChallenge.GorbP1Helper.ReapplyLiveSettings();
            }

            RefreshGorbP1HelperUi();
        }

        private int GetGorbP1Phase2MaxHp()
        {
            return Mathf.Clamp(Modules.BossChallenge.GorbP1Helper.GetPhase2MaxHpForUi(), 2, 999999);
        }

        private void NormalizeGorbP1PhaseThresholdInputs()
        {
            int phase2MaxHp = GetGorbP1Phase2MaxHp();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.GorbP1Helper.gorbP1Phase2Hp, 2, phase2MaxHp);
            Modules.BossChallenge.GorbP1Helper.gorbP1Phase2Hp = phase2Hp;
            Modules.BossChallenge.GorbP1Helper.gorbP1Phase3Hp =
                Mathf.Clamp(Modules.BossChallenge.GorbP1Helper.gorbP1Phase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
        }

        private void RefreshGorbP1HelperUi()
        {
            Module? module = GetGorbP1HelperModule();
            UpdateToggleValue(gorbP1HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(gorbP1HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(gorbP1UseMaxHpValue, Modules.BossChallenge.GorbP1Helper.gorbP1UseMaxHp);
            UpdateToggleValue(gorbP1UseCustomPhaseValue, Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase);
            UpdateIntInputValue(gorbP1MaxHpField, Modules.BossChallenge.GorbP1Helper.gorbP1MaxHp);
            UpdateIntInputValue(gorbP1Phase2HpField, Modules.BossChallenge.GorbP1Helper.gorbP1Phase2Hp);
            UpdateIntInputValue(gorbP1Phase3HpField, Modules.BossChallenge.GorbP1Helper.gorbP1Phase3Hp);

            UpdateGorbP1HelperInteractivity();
        }

        private void UpdateGorbP1HelperInteractivity()
        {
            SetContentInteractivity(gorbP1HelperContent, GetGorbP1HelperEnabled(), "GorbP1EnableRow");
            if (!GetGorbP1HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(gorbP1HelperContent, "GorbP1MaxHpRow", Modules.BossChallenge.GorbP1Helper.gorbP1UseMaxHp);
            SetRowInteractivity(gorbP1HelperContent, "GorbP1Phase2HpRow", Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase);
            SetRowInteractivity(gorbP1HelperContent, "GorbP1Phase3HpRow", Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase);
        }

        private void SetGorbP1HelperVisible(bool value)
        {
            gorbP1HelperVisible = value;
            ApplyPanelVisible(gorbP1HelperRoot, value, RefreshGorbP1HelperUi);
        }

        private static void ResetGorbP1HelperDefaults()
        {
            Modules.BossChallenge.GorbP1Helper.gorbP1UseMaxHp = false;
            Modules.BossChallenge.GorbP1Helper.gorbP1MaxHp = 650;
            Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase = false;
            Modules.BossChallenge.GorbP1Helper.gorbP1Phase2Hp = 455;
            Modules.BossChallenge.GorbP1Helper.gorbP1Phase3Hp = 260;
        }

        private void OnGorbP1HelperBackClicked()
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

            SetGorbP1HelperVisible(false);
        }

        private void OnGorbP1HelperResetDefaultsClicked()
        {
            ResetGorbP1HelperDefaults();
            SetGorbP1HelperEnabled(false);
            Modules.BossChallenge.GorbP1Helper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.GorbP1Helper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshGorbP1HelperUi();
        }
    }
}
