using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private const int MarmuP2HelperCanvasSortOrder = 10074;
        private const float MarmuP2HelperPanelHeight = PanelHeight;
        private const float MarmuP2HelperRowSpacing = 44f;
        private GameObject? marmuP2HelperRoot;
        private RectTransform? marmuP2HelperContent;
        private bool marmuP2HelperVisible;
        private Text? marmuP2HelperToggleValue;
        private Image? marmuP2HelperToggleIcon;
        private Text? marmuP2UseMaxHpValue;
        private InputField? marmuP2MaxHpField;
        private Module? marmuP2HelperModule;

        private void BuildMarmuP2HelperOverlayUi()
        {
            marmuP2HelperRoot = CreateOverlayFrame("MarmuP2HelperOverlayCanvas", MarmuP2HelperCanvasSortOrder, "MarmuP2HelperPanel", MarmuP2HelperPanelHeight, "Marmu P2", 52, 420f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = MarmuP2HelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, MarmuP2HelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            marmuP2HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "MarmuP2EnableRow",
                "Enable Marmu P2",
                rowY,
                GetMarmuP2HelperEnabled,
                SetMarmuP2HelperEnabled,
                out marmuP2HelperToggleValue,
                out marmuP2HelperToggleIcon);

            rowY += MarmuP2HelperRowSpacing;
            CreateToggleRow(
                content,
                "MarmuP2UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.MarmuP2Helper.marmuP2UseMaxHp,
                SetMarmuP2UseMaxHpEnabled,
                out marmuP2UseMaxHpValue);
            lastY = rowY;

            rowY += MarmuP2HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MarmuP2MaxHpRow",
                "Marmu P2 Max HP",
                rowY,
                () => Modules.BossChallenge.MarmuP2Helper.marmuP2MaxHp,
                SetMarmuP2MaxHp,
                1,
                999999,
                1,
                out marmuP2MaxHpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "MarmuP2ResetRow", "Reset Default", resetY, OnMarmuP2HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "MarmuP2BackRow", "Back", backY, OnMarmuP2HelperBackClicked);
        }

        private Module? GetMarmuP2HelperModule()
        {
            return GetCachedModule(ref marmuP2HelperModule, typeof(Modules.BossChallenge.MarmuP2Helper));
        }

        private bool GetMarmuP2HelperEnabled()
        {
            return GetMarmuP2HelperModule()?.Enabled ?? false;
        }

        private void SetMarmuP2HelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetMarmuP2HelperModule(), value);
            UpdateMarmuP2HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetMarmuP2UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.MarmuP2Helper.marmuP2UseMaxHp = value;
            Modules.BossChallenge.MarmuP2Helper.ReapplyLiveSettings();
            RefreshMarmuP2HelperUi();
        }

        private void SetMarmuP2MaxHp(int value)
        {
            Modules.BossChallenge.MarmuP2Helper.marmuP2MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.MarmuP2Helper.marmuP2UseMaxHp)
            {
                Modules.BossChallenge.MarmuP2Helper.ApplyMarmuHealthIfPresent();
            }
        }

        private void RefreshMarmuP2HelperUi()
        {
            Module? module = GetMarmuP2HelperModule();
            UpdateToggleValue(marmuP2HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(marmuP2HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(marmuP2UseMaxHpValue, Modules.BossChallenge.MarmuP2Helper.marmuP2UseMaxHp);
            UpdateIntInputValue(marmuP2MaxHpField, Modules.BossChallenge.MarmuP2Helper.marmuP2MaxHp);

            UpdateMarmuP2HelperInteractivity();
        }

        private void UpdateMarmuP2HelperInteractivity()
        {
            SetContentInteractivity(marmuP2HelperContent, GetMarmuP2HelperEnabled(), "MarmuP2EnableRow");
            if (!GetMarmuP2HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(marmuP2HelperContent, "MarmuP2MaxHpRow", Modules.BossChallenge.MarmuP2Helper.marmuP2UseMaxHp);
        }

        private void SetMarmuP2HelperVisible(bool value)
        {
            marmuP2HelperVisible = value;
            ApplyPanelVisible(marmuP2HelperRoot, value, RefreshMarmuP2HelperUi);
        }

        private static void ResetMarmuP2HelperDefaults()
        {
            Modules.BossChallenge.MarmuP2Helper.marmuP2UseMaxHp = false;
            Modules.BossChallenge.MarmuP2Helper.marmuP2MaxHp = 416;
        }

        private void OnMarmuP2HelperBackClicked()
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

            SetMarmuP2HelperVisible(false);
        }

        private void OnMarmuP2HelperResetDefaultsClicked()
        {
            ResetMarmuP2HelperDefaults();
            SetMarmuP2HelperEnabled(false);
            Modules.BossChallenge.MarmuP2Helper.RestoreVanillaHealthIfPresent();
            RefreshMarmuP2HelperUi();
        }
    }
}
