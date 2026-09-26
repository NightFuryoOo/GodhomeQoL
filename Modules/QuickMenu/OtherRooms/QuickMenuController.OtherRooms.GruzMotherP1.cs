using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private const int GruzMotherP1HelperCanvasSortOrder = 10067;
        private const float GruzMotherP1HelperPanelHeight = PanelHeight;
        private const float GruzMotherP1HelperRowSpacing = 44f;
        private GameObject? gruzMotherP1HelperRoot;
        private RectTransform? gruzMotherP1HelperContent;
        private bool gruzMotherP1HelperVisible;
        private Text? gruzMotherP1HelperToggleValue;
        private Image? gruzMotherP1HelperToggleIcon;
        private Text? gruzMotherP1UseMaxHpValue;
        private InputField? gruzMotherP1MaxHpField;
        private Module? gruzMotherP1HelperModule;

        private void BuildGruzMotherP1HelperOverlayUi()
        {
            gruzMotherP1HelperRoot = CreateOverlayFrame("GruzMotherP1HelperOverlayCanvas", GruzMotherP1HelperCanvasSortOrder, "GruzMotherP1HelperPanel", GruzMotherP1HelperPanelHeight, "Gruz Mother P1", 52, 420f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = GruzMotherP1HelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, GruzMotherP1HelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            gruzMotherP1HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "GruzMotherP1EnableRow",
                "Enable Gruz Mother P1",
                rowY,
                GetGruzMotherP1HelperEnabled,
                SetGruzMotherP1HelperEnabled,
                out gruzMotherP1HelperToggleValue,
                out gruzMotherP1HelperToggleIcon);

            rowY += GruzMotherP1HelperRowSpacing;
            CreateToggleRow(
                content,
                "GruzMotherP1UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1UseMaxHp,
                SetGruzMotherP1UseMaxHpEnabled,
                out gruzMotherP1UseMaxHpValue);
            lastY = rowY;

            rowY += GruzMotherP1HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GruzMotherP1MaxHpRow",
                "Gruz Mother P1 Max HP",
                rowY,
                () => Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1MaxHp,
                SetGruzMotherP1MaxHp,
                1,
                999999,
                1,
                out gruzMotherP1MaxHpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "GruzMotherP1ResetRow", "Reset Default", resetY, OnGruzMotherP1HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "GruzMotherP1BackRow", "Back", backY, OnGruzMotherP1HelperBackClicked);
        }

        private Module? GetGruzMotherP1HelperModule()
        {
            return GetCachedModule(ref gruzMotherP1HelperModule, typeof(Modules.BossChallenge.GruzMotherP1Helper));
        }

        private bool GetGruzMotherP1HelperEnabled()
        {
            return GetGruzMotherP1HelperModule()?.Enabled ?? false;
        }

        private void SetGruzMotherP1HelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetGruzMotherP1HelperModule(), value);
            UpdateGruzMotherP1HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetGruzMotherP1UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1UseMaxHp = value;
            Modules.BossChallenge.GruzMotherP1Helper.ReapplyLiveSettings();
            RefreshGruzMotherP1HelperUi();
        }

        private void SetGruzMotherP1MaxHp(int value)
        {
            Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1UseMaxHp)
            {
                Modules.BossChallenge.GruzMotherP1Helper.ApplyGruzHealthIfPresent();
            }
        }

        private void RefreshGruzMotherP1HelperUi()
        {
            Module? module = GetGruzMotherP1HelperModule();
            UpdateToggleValue(gruzMotherP1HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(gruzMotherP1HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(gruzMotherP1UseMaxHpValue, Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1UseMaxHp);
            UpdateIntInputValue(gruzMotherP1MaxHpField, Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1MaxHp);

            UpdateGruzMotherP1HelperInteractivity();
        }

        private void UpdateGruzMotherP1HelperInteractivity()
        {
            SetContentInteractivity(gruzMotherP1HelperContent, GetGruzMotherP1HelperEnabled(), "GruzMotherP1EnableRow");
            if (!GetGruzMotherP1HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(gruzMotherP1HelperContent, "GruzMotherP1MaxHpRow", Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1UseMaxHp);
        }

        private void SetGruzMotherP1HelperVisible(bool value)
        {
            gruzMotherP1HelperVisible = value;
            ApplyPanelVisible(gruzMotherP1HelperRoot, value, RefreshGruzMotherP1HelperUi);
        }

        private static void ResetGruzMotherP1HelperDefaults()
        {
            Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1UseMaxHp = false;
            Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1MaxHp = 650;
        }

        private void OnGruzMotherP1HelperBackClicked()
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

            SetGruzMotherP1HelperVisible(false);
        }

        private void OnGruzMotherP1HelperResetDefaultsClicked()
        {
            ResetGruzMotherP1HelperDefaults();
            SetGruzMotherP1HelperEnabled(false);
            Modules.BossChallenge.GruzMotherP1Helper.RestoreVanillaHealthIfPresent();
            RefreshGruzMotherP1HelperUi();
        }
    }
}
