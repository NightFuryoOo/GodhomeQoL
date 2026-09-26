using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private const int BroodingMawlekP1HelperCanvasSortOrder = 10069;
        private const float BroodingMawlekP1HelperPanelHeight = PanelHeight;
        private const float BroodingMawlekP1HelperRowSpacing = 44f;
        private GameObject? broodingMawlekP1HelperRoot;
        private RectTransform? broodingMawlekP1HelperContent;
        private bool broodingMawlekP1HelperVisible;
        private Text? broodingMawlekP1HelperToggleValue;
        private Image? broodingMawlekP1HelperToggleIcon;
        private Text? broodingMawlekP1UseMaxHpValue;
        private InputField? broodingMawlekP1MaxHpField;
        private Module? broodingMawlekP1HelperModule;

        private void BuildBroodingMawlekP1HelperOverlayUi()
        {
            broodingMawlekP1HelperRoot = CreateOverlayFrame("BroodingMawlekP1HelperOverlayCanvas", BroodingMawlekP1HelperCanvasSortOrder, "BroodingMawlekP1HelperPanel", BroodingMawlekP1HelperPanelHeight, "Brooding Mawlek P1", 52, 420f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = BroodingMawlekP1HelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, BroodingMawlekP1HelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            broodingMawlekP1HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "BroodingMawlekP1EnableRow",
                "Enable Brooding Mawlek P1",
                rowY,
                GetBroodingMawlekP1HelperEnabled,
                SetBroodingMawlekP1HelperEnabled,
                out broodingMawlekP1HelperToggleValue,
                out broodingMawlekP1HelperToggleIcon);

            rowY += BroodingMawlekP1HelperRowSpacing;
            CreateToggleRow(
                content,
                "BroodingMawlekP1UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1UseMaxHp,
                SetBroodingMawlekP1UseMaxHpEnabled,
                out broodingMawlekP1UseMaxHpValue);
            lastY = rowY;

            rowY += BroodingMawlekP1HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "BroodingMawlekP1MaxHpRow",
                "Brooding Mawlek P1 Max HP",
                rowY,
                () => Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1MaxHp,
                SetBroodingMawlekP1MaxHp,
                1,
                999999,
                1,
                out broodingMawlekP1MaxHpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "BroodingMawlekP1ResetRow", "Reset Default", resetY, OnBroodingMawlekP1HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "BroodingMawlekP1BackRow", "Back", backY, OnBroodingMawlekP1HelperBackClicked);
        }

        private Module? GetBroodingMawlekP1HelperModule()
        {
            return GetCachedModule(ref broodingMawlekP1HelperModule, typeof(Modules.BossChallenge.BroodingMawlekP1Helper));
        }

        private bool GetBroodingMawlekP1HelperEnabled()
        {
            return GetBroodingMawlekP1HelperModule()?.Enabled ?? false;
        }

        private void SetBroodingMawlekP1HelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetBroodingMawlekP1HelperModule(), value);
            UpdateBroodingMawlekP1HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetBroodingMawlekP1UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1UseMaxHp = value;
            Modules.BossChallenge.BroodingMawlekP1Helper.ReapplyLiveSettings();
            RefreshBroodingMawlekP1HelperUi();
        }

        private void SetBroodingMawlekP1MaxHp(int value)
        {
            Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1UseMaxHp)
            {
                Modules.BossChallenge.BroodingMawlekP1Helper.ApplyMawlekHealthIfPresent();
            }
        }

        private void RefreshBroodingMawlekP1HelperUi()
        {
            Module? module = GetBroodingMawlekP1HelperModule();
            UpdateToggleValue(broodingMawlekP1HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(broodingMawlekP1HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(broodingMawlekP1UseMaxHpValue, Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1UseMaxHp);
            UpdateIntInputValue(broodingMawlekP1MaxHpField, Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1MaxHp);

            UpdateBroodingMawlekP1HelperInteractivity();
        }

        private void UpdateBroodingMawlekP1HelperInteractivity()
        {
            SetContentInteractivity(broodingMawlekP1HelperContent, GetBroodingMawlekP1HelperEnabled(), "BroodingMawlekP1EnableRow");
            if (!GetBroodingMawlekP1HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(broodingMawlekP1HelperContent, "BroodingMawlekP1MaxHpRow", Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1UseMaxHp);
        }

        private void SetBroodingMawlekP1HelperVisible(bool value)
        {
            broodingMawlekP1HelperVisible = value;
            ApplyPanelVisible(broodingMawlekP1HelperRoot, value, RefreshBroodingMawlekP1HelperUi);
        }

        private static void ResetBroodingMawlekP1HelperDefaults()
        {
            Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1UseMaxHp = false;
            Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1MaxHp = 1050;
        }

        private void OnBroodingMawlekP1HelperBackClicked()
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

            SetBroodingMawlekP1HelperVisible(false);
        }

        private void OnBroodingMawlekP1HelperResetDefaultsClicked()
        {
            ResetBroodingMawlekP1HelperDefaults();
            SetBroodingMawlekP1HelperEnabled(false);
            Modules.BossChallenge.BroodingMawlekP1Helper.RestoreVanillaHealthIfPresent();
            RefreshBroodingMawlekP1HelperUi();
        }
    }
}
