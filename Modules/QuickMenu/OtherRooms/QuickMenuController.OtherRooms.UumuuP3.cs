using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private const int UumuuP3HelperCanvasSortOrder = 10071;
        private const float UumuuP3HelperPanelHeight = PanelHeight;
        private const float UumuuP3HelperRowSpacing = 44f;
        private GameObject? uumuuP3HelperRoot;
        private RectTransform? uumuuP3HelperContent;
        private bool uumuuP3HelperVisible;
        private Text? uumuuP3HelperToggleValue;
        private Image? uumuuP3HelperToggleIcon;
        private Text? uumuuP3UseMaxHpValue;
        private InputField? uumuuP3MaxHpField;
        private Text? uumuuP3UseCustomSummonHpValue;
        private InputField? uumuuP3SummonHpField;
        private Module? uumuuP3HelperModule;

        private void BuildUumuuP3HelperOverlayUi()
        {
            uumuuP3HelperRoot = CreateOverlayFrame("UumuuP3HelperOverlayCanvas", UumuuP3HelperCanvasSortOrder, "UumuuP3HelperPanel", UumuuP3HelperPanelHeight, "Uumuu P3", 52, 420f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = UumuuP3HelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, UumuuP3HelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            uumuuP3HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "UumuuP3EnableRow",
                "Enable Uumuu P3",
                rowY,
                GetUumuuP3HelperEnabled,
                SetUumuuP3HelperEnabled,
                out uumuuP3HelperToggleValue,
                out uumuuP3HelperToggleIcon);

            rowY += UumuuP3HelperRowSpacing;
            CreateToggleRow(
                content,
                "UumuuP3UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.UumuuP3Helper.uumuuP3UseMaxHp,
                SetUumuuP3UseMaxHpEnabled,
                out uumuuP3UseMaxHpValue);
            lastY = rowY;

            rowY += UumuuP3HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "UumuuP3MaxHpRow",
                "Uumuu P3 Max HP",
                rowY,
                () => Modules.BossChallenge.UumuuP3Helper.uumuuP3MaxHp,
                SetUumuuP3MaxHp,
                1,
                999999,
                1,
                out uumuuP3MaxHpField);
            lastY = rowY;

            rowY += UumuuP3HelperRowSpacing;
            CreateToggleRow(
                content,
                "UumuuP3UseCustomSummonHpRow",
                "Use Custom Summon HP",
                rowY,
                () => Modules.BossChallenge.UumuuP3Helper.uumuuP3UseCustomSummonHp,
                SetUumuuP3UseCustomSummonHpEnabled,
                out uumuuP3UseCustomSummonHpValue);
            lastY = rowY;

            rowY += UumuuP3HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "UumuuP3SummonHpRow",
                "Summon HP",
                rowY,
                () => Modules.BossChallenge.UumuuP3Helper.uumuuP3SummonHp,
                SetUumuuP3SummonHp,
                1,
                999999,
                1,
                out uumuuP3SummonHpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "UumuuP3ResetRow", "Reset Default", resetY, OnUumuuP3HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "UumuuP3BackRow", "Back", backY, OnUumuuP3HelperBackClicked);
        }

        private Module? GetUumuuP3HelperModule()
        {
            return GetCachedModule(ref uumuuP3HelperModule, typeof(Modules.BossChallenge.UumuuP3Helper));
        }

        private bool GetUumuuP3HelperEnabled()
        {
            return GetUumuuP3HelperModule()?.Enabled ?? false;
        }

        private void SetUumuuP3HelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetUumuuP3HelperModule(), value);
            UpdateUumuuP3HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetUumuuP3UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.UumuuP3Helper.uumuuP3UseMaxHp = value;
            Modules.BossChallenge.UumuuP3Helper.ReapplyLiveSettings();
            RefreshUumuuP3HelperUi();
        }

        private void SetUumuuP3MaxHp(int value)
        {
            Modules.BossChallenge.UumuuP3Helper.uumuuP3MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.UumuuP3Helper.uumuuP3UseMaxHp)
            {
                Modules.BossChallenge.UumuuP3Helper.ApplyUumuuHealthIfPresent();
            }
        }

        private void SetUumuuP3UseCustomSummonHpEnabled(bool value)
        {
            Modules.BossChallenge.UumuuP3Helper.uumuuP3UseCustomSummonHp = value;
            Modules.BossChallenge.UumuuP3Helper.ReapplyLiveSettings();
            RefreshUumuuP3HelperUi();
        }

        private void SetUumuuP3SummonHp(int value)
        {
            Modules.BossChallenge.UumuuP3Helper.uumuuP3SummonHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.UumuuP3Helper.uumuuP3UseCustomSummonHp)
            {
                Modules.BossChallenge.UumuuP3Helper.ApplyUumuuSummonHealthIfPresent();
            }
        }

        private void RefreshUumuuP3HelperUi()
        {
            Module? module = GetUumuuP3HelperModule();
            UpdateToggleValue(uumuuP3HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(uumuuP3HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(uumuuP3UseMaxHpValue, Modules.BossChallenge.UumuuP3Helper.uumuuP3UseMaxHp);
            UpdateIntInputValue(uumuuP3MaxHpField, Modules.BossChallenge.UumuuP3Helper.uumuuP3MaxHp);
            UpdateToggleValue(uumuuP3UseCustomSummonHpValue, Modules.BossChallenge.UumuuP3Helper.uumuuP3UseCustomSummonHp);
            UpdateIntInputValue(uumuuP3SummonHpField, Modules.BossChallenge.UumuuP3Helper.uumuuP3SummonHp);

            UpdateUumuuP3HelperInteractivity();
        }

        private void UpdateUumuuP3HelperInteractivity()
        {
            SetContentInteractivity(uumuuP3HelperContent, GetUumuuP3HelperEnabled(), "UumuuP3EnableRow");
            if (!GetUumuuP3HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(uumuuP3HelperContent, "UumuuP3MaxHpRow", Modules.BossChallenge.UumuuP3Helper.uumuuP3UseMaxHp);
            SetRowInteractivity(uumuuP3HelperContent, "UumuuP3SummonHpRow", Modules.BossChallenge.UumuuP3Helper.uumuuP3UseCustomSummonHp);
        }

        private void SetUumuuP3HelperVisible(bool value)
        {
            uumuuP3HelperVisible = value;
            ApplyPanelVisible(uumuuP3HelperRoot, value, RefreshUumuuP3HelperUi);
        }

        private static void ResetUumuuP3HelperDefaults()
        {
            Modules.BossChallenge.UumuuP3Helper.uumuuP3UseMaxHp = false;
            Modules.BossChallenge.UumuuP3Helper.uumuuP3MaxHp = 350;
            Modules.BossChallenge.UumuuP3Helper.uumuuP3UseCustomSummonHp = false;
            Modules.BossChallenge.UumuuP3Helper.uumuuP3SummonHp = 1;
        }

        private void OnUumuuP3HelperBackClicked()
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

            SetUumuuP3HelperVisible(false);
        }

        private void OnUumuuP3HelperResetDefaultsClicked()
        {
            ResetUumuuP3HelperDefaults();
            SetUumuuP3HelperEnabled(false);
            Modules.BossChallenge.UumuuP3Helper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.UumuuP3Helper.RestoreVanillaSummonHealthIfPresent();
            RefreshUumuuP3HelperUi();
        }
    }
}
