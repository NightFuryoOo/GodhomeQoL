using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private const int VengeflyKingP1HelperCanvasSortOrder = 10068;
        private const float VengeflyKingP1HelperPanelHeight = PanelHeight;
        private const float VengeflyKingP1HelperRowSpacing = 44f;
        private GameObject? vengeflyKingP1HelperRoot;
        private RectTransform? vengeflyKingP1HelperContent;
        private bool vengeflyKingP1HelperVisible;
        private Text? vengeflyKingP1HelperToggleValue;
        private Image? vengeflyKingP1HelperToggleIcon;
        private Text? vengeflyKingP1UseMaxHpValue;
        private InputField? vengeflyKingP1MaxHpField;
        private Module? vengeflyKingP1HelperModule;

        private void BuildVengeflyKingP1HelperOverlayUi()
        {
            vengeflyKingP1HelperRoot = CreateOverlayFrame("VengeflyKingP1HelperOverlayCanvas", VengeflyKingP1HelperCanvasSortOrder, "VengeflyKingP1HelperPanel", VengeflyKingP1HelperPanelHeight, "Vengefly King P1", 52, 420f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = VengeflyKingP1HelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, VengeflyKingP1HelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            vengeflyKingP1HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "VengeflyKingP1EnableRow",
                "Enable Vengefly King P1",
                rowY,
                GetVengeflyKingP1HelperEnabled,
                SetVengeflyKingP1HelperEnabled,
                out vengeflyKingP1HelperToggleValue,
                out vengeflyKingP1HelperToggleIcon);

            rowY += VengeflyKingP1HelperRowSpacing;
            CreateToggleRow(
                content,
                "VengeflyKingP1UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1UseMaxHp,
                SetVengeflyKingP1UseMaxHpEnabled,
                out vengeflyKingP1UseMaxHpValue);
            lastY = rowY;

            rowY += VengeflyKingP1HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "VengeflyKingP1MaxHpRow",
                "Vengefly King P1 Max HP",
                rowY,
                () => Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1MaxHp,
                SetVengeflyKingP1MaxHp,
                1,
                999999,
                1,
                out vengeflyKingP1MaxHpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "VengeflyKingP1ResetRow", "Reset Default", resetY, OnVengeflyKingP1HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "VengeflyKingP1BackRow", "Back", backY, OnVengeflyKingP1HelperBackClicked);
        }

        private Module? GetVengeflyKingP1HelperModule()
        {
            return GetCachedModule(ref vengeflyKingP1HelperModule, typeof(Modules.BossChallenge.VengeflyKingP1Helper));
        }

        private bool GetVengeflyKingP1HelperEnabled()
        {
            return GetVengeflyKingP1HelperModule()?.Enabled ?? false;
        }

        private void SetVengeflyKingP1HelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetVengeflyKingP1HelperModule(), value);
            UpdateVengeflyKingP1HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetVengeflyKingP1UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1UseMaxHp = value;
            Modules.BossChallenge.VengeflyKingP1Helper.ReapplyLiveSettings();
            RefreshVengeflyKingP1HelperUi();
        }

        private void SetVengeflyKingP1MaxHp(int value)
        {
            Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1UseMaxHp)
            {
                Modules.BossChallenge.VengeflyKingP1Helper.ApplyVengeflyHealthIfPresent();
            }
        }

        private void RefreshVengeflyKingP1HelperUi()
        {
            Module? module = GetVengeflyKingP1HelperModule();
            UpdateToggleValue(vengeflyKingP1HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(vengeflyKingP1HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(vengeflyKingP1UseMaxHpValue, Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1UseMaxHp);
            UpdateIntInputValue(vengeflyKingP1MaxHpField, Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1MaxHp);

            UpdateVengeflyKingP1HelperInteractivity();
        }

        private void UpdateVengeflyKingP1HelperInteractivity()
        {
            SetContentInteractivity(vengeflyKingP1HelperContent, GetVengeflyKingP1HelperEnabled(), "VengeflyKingP1EnableRow");
            if (!GetVengeflyKingP1HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(vengeflyKingP1HelperContent, "VengeflyKingP1MaxHpRow", Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1UseMaxHp);
        }

        private void SetVengeflyKingP1HelperVisible(bool value)
        {
            vengeflyKingP1HelperVisible = value;
            ApplyPanelVisible(vengeflyKingP1HelperRoot, value, RefreshVengeflyKingP1HelperUi);
        }

        private static void ResetVengeflyKingP1HelperDefaults()
        {
            Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1UseMaxHp = false;
            Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1MaxHp = 450;
        }

        private void OnVengeflyKingP1HelperBackClicked()
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

            SetVengeflyKingP1HelperVisible(false);
        }

        private void OnVengeflyKingP1HelperResetDefaultsClicked()
        {
            ResetVengeflyKingP1HelperDefaults();
            SetVengeflyKingP1HelperEnabled(false);
            Modules.BossChallenge.VengeflyKingP1Helper.RestoreVanillaHealthIfPresent();
            RefreshVengeflyKingP1HelperUi();
        }
    }
}
