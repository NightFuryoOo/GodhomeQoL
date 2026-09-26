using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private const int XeroP2HelperCanvasSortOrder = 10075;
        private const float XeroP2HelperPanelHeight = PanelHeight;
        private const float XeroP2HelperRowSpacing = 44f;
        private GameObject? xeroP2HelperRoot;
        private RectTransform? xeroP2HelperContent;
        private bool xeroP2HelperVisible;
        private Text? xeroP2HelperToggleValue;
        private Image? xeroP2HelperToggleIcon;
        private Text? xeroP2UseMaxHpValue;
        private InputField? xeroP2MaxHpField;
        private Text? xeroP2UseCustomPhaseValue;
        private InputField? xeroP2Phase2HpField;
        private Module? xeroP2HelperModule;

        private void BuildXeroP2HelperOverlayUi()
        {
            xeroP2HelperRoot = CreateOverlayFrame("XeroP2HelperOverlayCanvas", XeroP2HelperCanvasSortOrder, "XeroP2HelperPanel", XeroP2HelperPanelHeight, "Xero P2", 52, 420f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = XeroP2HelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, XeroP2HelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            xeroP2HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "XeroP2EnableRow",
                "Enable Xero P2",
                rowY,
                GetXeroP2HelperEnabled,
                SetXeroP2HelperEnabled,
                out xeroP2HelperToggleValue,
                out xeroP2HelperToggleIcon);

            rowY += XeroP2HelperRowSpacing;
            CreateToggleRow(
                content,
                "XeroP2UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.XeroP2Helper.xeroP2UseMaxHp,
                SetXeroP2UseMaxHpEnabled,
                out xeroP2UseMaxHpValue);
            lastY = rowY;

            rowY += XeroP2HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "XeroP2MaxHpRow",
                "Xero P2 Max HP",
                rowY,
                () => Modules.BossChallenge.XeroP2Helper.xeroP2MaxHp,
                SetXeroP2MaxHp,
                1,
                999999,
                1,
                out xeroP2MaxHpField);
            lastY = rowY;

            rowY += XeroP2HelperRowSpacing;
            CreateToggleRow(
                content,
                "XeroP2UseCustomPhaseRow",
                "Use Custom Phase",
                rowY,
                () => Modules.BossChallenge.XeroP2Helper.xeroP2UseCustomPhase,
                SetXeroP2UseCustomPhaseEnabled,
                out xeroP2UseCustomPhaseValue);
            lastY = rowY;

            rowY += XeroP2HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "XeroP2Phase2HpRow",
                "Phase 2 HP",
                rowY,
                () => Modules.BossChallenge.XeroP2Helper.xeroP2Phase2Hp,
                SetXeroP2Phase2Hp,
                1,
                999999,
                1,
                out xeroP2Phase2HpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "XeroP2ResetRow", "Reset Default", resetY, OnXeroP2HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "XeroP2BackRow", "Back", backY, OnXeroP2HelperBackClicked);
        }

        private Module? GetXeroP2HelperModule()
        {
            return GetCachedModule(ref xeroP2HelperModule, typeof(Modules.BossChallenge.XeroP2Helper));
        }

        private bool GetXeroP2HelperEnabled()
        {
            return GetXeroP2HelperModule()?.Enabled ?? false;
        }

        private void SetXeroP2HelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetXeroP2HelperModule(), value);
            UpdateXeroP2HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetXeroP2UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.XeroP2Helper.xeroP2UseMaxHp = value;
            Modules.BossChallenge.XeroP2Helper.ReapplyLiveSettings();
            RefreshXeroP2HelperUi();
        }

        private void SetXeroP2MaxHp(int value)
        {
            Modules.BossChallenge.XeroP2Helper.xeroP2MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.XeroP2Helper.xeroP2UseMaxHp)
            {
                Modules.BossChallenge.XeroP2Helper.ApplyXeroHealthIfPresent();
            }

            Modules.BossChallenge.XeroP2Helper.xeroP2Phase2Hp = Mathf.Clamp(
                Modules.BossChallenge.XeroP2Helper.xeroP2Phase2Hp,
                1,
                GetXeroP2Phase2MaxHp());
            if (Modules.BossChallenge.XeroP2Helper.xeroP2UseCustomPhase)
            {
                Modules.BossChallenge.XeroP2Helper.ReapplyLiveSettings();
            }

            RefreshXeroP2HelperUi();
        }

        private void SetXeroP2UseCustomPhaseEnabled(bool value)
        {
            Modules.BossChallenge.XeroP2Helper.xeroP2Phase2Hp = Mathf.Clamp(
                Modules.BossChallenge.XeroP2Helper.xeroP2Phase2Hp,
                1,
                GetXeroP2Phase2MaxHp());
            Modules.BossChallenge.XeroP2Helper.xeroP2UseCustomPhase = value;
            Modules.BossChallenge.XeroP2Helper.ReapplyLiveSettings();
            RefreshXeroP2HelperUi();
        }

        private void SetXeroP2Phase2Hp(int value)
        {
            Modules.BossChallenge.XeroP2Helper.xeroP2Phase2Hp = Mathf.Clamp(value, 1, GetXeroP2Phase2MaxHp());
            if (Modules.BossChallenge.XeroP2Helper.xeroP2UseCustomPhase)
            {
                Modules.BossChallenge.XeroP2Helper.ReapplyLiveSettings();
            }

            RefreshXeroP2HelperUi();
        }

        private int GetXeroP2Phase2MaxHp()
        {
            return Mathf.Max(1, Modules.BossChallenge.XeroP2Helper.GetPhase2MaxHpForUi());
        }

        private void RefreshXeroP2HelperUi()
        {
            Module? module = GetXeroP2HelperModule();
            UpdateToggleValue(xeroP2HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(xeroP2HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(xeroP2UseMaxHpValue, Modules.BossChallenge.XeroP2Helper.xeroP2UseMaxHp);
            UpdateToggleValue(xeroP2UseCustomPhaseValue, Modules.BossChallenge.XeroP2Helper.xeroP2UseCustomPhase);
            UpdateIntInputValue(xeroP2MaxHpField, Modules.BossChallenge.XeroP2Helper.xeroP2MaxHp);
            UpdateIntInputValue(xeroP2Phase2HpField, Modules.BossChallenge.XeroP2Helper.xeroP2Phase2Hp);

            UpdateXeroP2HelperInteractivity();
        }

        private void UpdateXeroP2HelperInteractivity()
        {
            SetContentInteractivity(xeroP2HelperContent, GetXeroP2HelperEnabled(), "XeroP2EnableRow");
            if (!GetXeroP2HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(xeroP2HelperContent, "XeroP2MaxHpRow", Modules.BossChallenge.XeroP2Helper.xeroP2UseMaxHp);
            SetRowInteractivity(xeroP2HelperContent, "XeroP2Phase2HpRow", Modules.BossChallenge.XeroP2Helper.xeroP2UseCustomPhase);
        }

        private void SetXeroP2HelperVisible(bool value)
        {
            xeroP2HelperVisible = value;
            ApplyPanelVisible(xeroP2HelperRoot, value, RefreshXeroP2HelperUi);
        }

        private static void ResetXeroP2HelperDefaults()
        {
            Modules.BossChallenge.XeroP2Helper.xeroP2UseMaxHp = false;
            Modules.BossChallenge.XeroP2Helper.xeroP2MaxHp = 650;
            Modules.BossChallenge.XeroP2Helper.xeroP2UseCustomPhase = false;
            Modules.BossChallenge.XeroP2Helper.xeroP2Phase2Hp = 325;
        }

        private void OnXeroP2HelperBackClicked()
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

            SetXeroP2HelperVisible(false);
        }

        private void OnXeroP2HelperResetDefaultsClicked()
        {
            ResetXeroP2HelperDefaults();
            SetXeroP2HelperEnabled(false);
            Modules.BossChallenge.XeroP2Helper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.XeroP2Helper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshXeroP2HelperUi();
        }
    }
}
