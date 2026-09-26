using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private const int NoskP2HelperCanvasSortOrder = 10070;
        private const float NoskP2HelperPanelHeight = PanelHeight;
        private const float NoskP2HelperRowSpacing = 44f;
        private GameObject? noskP2HelperRoot;
        private RectTransform? noskP2HelperContent;
        private bool noskP2HelperVisible;
        private Text? noskP2HelperToggleValue;
        private Image? noskP2HelperToggleIcon;
        private Text? noskP2UseMaxHpValue;
        private InputField? noskP2MaxHpField;
        private Text? noskP2UseCustomPhaseValue;
        private InputField? noskP2Phase2HpField;
        private Module? noskP2HelperModule;

        private void BuildNoskP2HelperOverlayUi()
        {
            noskP2HelperRoot = CreateOverlayFrame("NoskP2HelperOverlayCanvas", NoskP2HelperCanvasSortOrder, "NoskP2HelperPanel", NoskP2HelperPanelHeight, "Nosk P2", 52, 420f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = NoskP2HelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, NoskP2HelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            noskP2HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "NoskP2EnableRow",
                "Enable Nosk P2",
                rowY,
                GetNoskP2HelperEnabled,
                SetNoskP2HelperEnabled,
                out noskP2HelperToggleValue,
                out noskP2HelperToggleIcon);

            rowY += NoskP2HelperRowSpacing;
            CreateToggleRow(
                content,
                "NoskP2UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.NoskP2Helper.noskP2UseMaxHp,
                SetNoskP2UseMaxHpEnabled,
                out noskP2UseMaxHpValue);
            lastY = rowY;

            rowY += NoskP2HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoskP2MaxHpRow",
                "Nosk P2 Max HP",
                rowY,
                () => Modules.BossChallenge.NoskP2Helper.noskP2MaxHp,
                SetNoskP2MaxHp,
                1,
                999999,
                1,
                out noskP2MaxHpField);
            lastY = rowY;

            rowY += NoskP2HelperRowSpacing;
            CreateToggleRow(
                content,
                "NoskP2UseCustomPhaseRow",
                "Use Custom Phase",
                rowY,
                () => Modules.BossChallenge.NoskP2Helper.noskP2UseCustomPhase,
                SetNoskP2UseCustomPhaseEnabled,
                out noskP2UseCustomPhaseValue);
            lastY = rowY;

            rowY += NoskP2HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoskP2Phase2HpRow",
                "Phase 2 HP",
                rowY,
                () => Modules.BossChallenge.NoskP2Helper.noskP2Phase2Hp,
                SetNoskP2Phase2Hp,
                1,
                999999,
                1,
                out noskP2Phase2HpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "NoskP2ResetRow", "Reset Default", resetY, OnNoskP2HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "NoskP2BackRow", "Back", backY, OnNoskP2HelperBackClicked);
        }

        private Module? GetNoskP2HelperModule()
        {
            return GetCachedModule(ref noskP2HelperModule, typeof(Modules.BossChallenge.NoskP2Helper));
        }

        private bool GetNoskP2HelperEnabled()
        {
            return GetNoskP2HelperModule()?.Enabled ?? false;
        }

        private void SetNoskP2HelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetNoskP2HelperModule(), value);
            UpdateNoskP2HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetNoskP2UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.NoskP2Helper.noskP2UseMaxHp = value;
            Modules.BossChallenge.NoskP2Helper.ReapplyLiveSettings();
            RefreshNoskP2HelperUi();
        }

        private void SetNoskP2MaxHp(int value)
        {
            Modules.BossChallenge.NoskP2Helper.noskP2MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.NoskP2Helper.noskP2UseMaxHp)
            {
                Modules.BossChallenge.NoskP2Helper.ApplyNoskHealthIfPresent();
            }

            Modules.BossChallenge.NoskP2Helper.noskP2Phase2Hp = Mathf.Clamp(
                Modules.BossChallenge.NoskP2Helper.noskP2Phase2Hp,
                1,
                GetNoskP2Phase2MaxHp());
            if (Modules.BossChallenge.NoskP2Helper.noskP2UseCustomPhase)
            {
                Modules.BossChallenge.NoskP2Helper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshNoskP2HelperUi();
        }

        private void SetNoskP2UseCustomPhaseEnabled(bool value)
        {
            Modules.BossChallenge.NoskP2Helper.noskP2Phase2Hp = Mathf.Clamp(
                Modules.BossChallenge.NoskP2Helper.noskP2Phase2Hp,
                1,
                GetNoskP2Phase2MaxHp());
            Modules.BossChallenge.NoskP2Helper.noskP2UseCustomPhase = value;
            Modules.BossChallenge.NoskP2Helper.ReapplyLiveSettings();
            RefreshNoskP2HelperUi();
        }

        private void SetNoskP2Phase2Hp(int value)
        {
            Modules.BossChallenge.NoskP2Helper.noskP2Phase2Hp = Mathf.Clamp(value, 1, GetNoskP2Phase2MaxHp());
            if (Modules.BossChallenge.NoskP2Helper.noskP2UseCustomPhase)
            {
                Modules.BossChallenge.NoskP2Helper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshNoskP2HelperUi();
        }

        private int GetNoskP2Phase2MaxHp()
        {
            return Mathf.Max(1, Modules.BossChallenge.NoskP2Helper.GetPhase2MaxHpForUi());
        }

        private void RefreshNoskP2HelperUi()
        {
            Module? module = GetNoskP2HelperModule();
            UpdateToggleValue(noskP2HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(noskP2HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(noskP2UseMaxHpValue, Modules.BossChallenge.NoskP2Helper.noskP2UseMaxHp);
            UpdateToggleValue(noskP2UseCustomPhaseValue, Modules.BossChallenge.NoskP2Helper.noskP2UseCustomPhase);
            UpdateIntInputValue(noskP2MaxHpField, Modules.BossChallenge.NoskP2Helper.noskP2MaxHp);
            UpdateIntInputValue(noskP2Phase2HpField, Modules.BossChallenge.NoskP2Helper.noskP2Phase2Hp);

            UpdateNoskP2HelperInteractivity();
        }

        private void UpdateNoskP2HelperInteractivity()
        {
            SetContentInteractivity(noskP2HelperContent, GetNoskP2HelperEnabled(), "NoskP2EnableRow");
            if (!GetNoskP2HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(noskP2HelperContent, "NoskP2MaxHpRow", Modules.BossChallenge.NoskP2Helper.noskP2UseMaxHp);
            SetRowInteractivity(noskP2HelperContent, "NoskP2Phase2HpRow", Modules.BossChallenge.NoskP2Helper.noskP2UseCustomPhase);
        }

        private void SetNoskP2HelperVisible(bool value)
        {
            noskP2HelperVisible = value;
            ApplyPanelVisible(noskP2HelperRoot, value, RefreshNoskP2HelperUi);
        }

        private static void ResetNoskP2HelperDefaults()
        {
            Modules.BossChallenge.NoskP2Helper.noskP2UseMaxHp = false;
            Modules.BossChallenge.NoskP2Helper.noskP2MaxHp = 680;
            Modules.BossChallenge.NoskP2Helper.noskP2UseCustomPhase = false;
            Modules.BossChallenge.NoskP2Helper.noskP2Phase2Hp = 560;
        }

        private void OnNoskP2HelperBackClicked()
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

            SetNoskP2HelperVisible(false);
        }

        private void OnNoskP2HelperResetDefaultsClicked()
        {
            ResetNoskP2HelperDefaults();
            SetNoskP2HelperEnabled(false);
            Modules.BossChallenge.NoskP2Helper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.NoskP2Helper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshNoskP2HelperUi();
        }
    }
}
