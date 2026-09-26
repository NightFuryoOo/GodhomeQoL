using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private const int SoulWarriorP1HelperCanvasSortOrder = 10072;
        private const float SoulWarriorP1HelperPanelHeight = PanelHeight;
        private const float SoulWarriorP1HelperRowSpacing = 44f;
        private GameObject? soulWarriorP1HelperRoot;
        private RectTransform? soulWarriorP1HelperContent;
        private bool soulWarriorP1HelperVisible;
        private Text? soulWarriorP1HelperToggleValue;
        private Image? soulWarriorP1HelperToggleIcon;
        private Text? soulWarriorP1UseMaxHpValue;
        private InputField? soulWarriorP1MaxHpField;
        private Module? soulWarriorP1HelperModule;

        private void BuildSoulWarriorP1HelperOverlayUi()
        {
            soulWarriorP1HelperRoot = CreateOverlayFrame("SoulWarriorP1HelperOverlayCanvas", SoulWarriorP1HelperCanvasSortOrder, "SoulWarriorP1HelperPanel", SoulWarriorP1HelperPanelHeight, "Soul Warrior P1", 52, 420f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = SoulWarriorP1HelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, SoulWarriorP1HelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            soulWarriorP1HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "SoulWarriorP1EnableRow",
                "Enable Soul Warrior P1",
                rowY,
                GetSoulWarriorP1HelperEnabled,
                SetSoulWarriorP1HelperEnabled,
                out soulWarriorP1HelperToggleValue,
                out soulWarriorP1HelperToggleIcon);

            rowY += SoulWarriorP1HelperRowSpacing;
            CreateToggleRow(
                content,
                "SoulWarriorP1UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1UseMaxHp,
                SetSoulWarriorP1UseMaxHpEnabled,
                out soulWarriorP1UseMaxHpValue);
            lastY = rowY;

            rowY += SoulWarriorP1HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SoulWarriorP1MaxHpRow",
                "Soul Warrior P1 Max HP",
                rowY,
                () => Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1MaxHp,
                SetSoulWarriorP1MaxHp,
                1,
                999999,
                1,
                out soulWarriorP1MaxHpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "SoulWarriorP1ResetRow", "Reset Default", resetY, OnSoulWarriorP1HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "SoulWarriorP1BackRow", "Back", backY, OnSoulWarriorP1HelperBackClicked);
        }

        private Module? GetSoulWarriorP1HelperModule()
        {
            return GetCachedModule(ref soulWarriorP1HelperModule, typeof(Modules.BossChallenge.SoulWarriorP1Helper));
        }

        private bool GetSoulWarriorP1HelperEnabled()
        {
            return GetSoulWarriorP1HelperModule()?.Enabled ?? false;
        }

        private void SetSoulWarriorP1HelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetSoulWarriorP1HelperModule(), value);
            UpdateSoulWarriorP1HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetSoulWarriorP1UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1UseMaxHp = value;
            Modules.BossChallenge.SoulWarriorP1Helper.ReapplyLiveSettings();
            RefreshSoulWarriorP1HelperUi();
        }

        private void SetSoulWarriorP1MaxHp(int value)
        {
            Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1UseMaxHp)
            {
                Modules.BossChallenge.SoulWarriorP1Helper.ApplySoulWarriorHealthIfPresent();
            }
        }

        private void RefreshSoulWarriorP1HelperUi()
        {
            Module? module = GetSoulWarriorP1HelperModule();
            UpdateToggleValue(soulWarriorP1HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(soulWarriorP1HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(soulWarriorP1UseMaxHpValue, Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1UseMaxHp);
            UpdateIntInputValue(soulWarriorP1MaxHpField, Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1MaxHp);

            UpdateSoulWarriorP1HelperInteractivity();
        }

        private void UpdateSoulWarriorP1HelperInteractivity()
        {
            SetContentInteractivity(soulWarriorP1HelperContent, GetSoulWarriorP1HelperEnabled(), "SoulWarriorP1EnableRow");
            if (!GetSoulWarriorP1HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(soulWarriorP1HelperContent, "SoulWarriorP1MaxHpRow", Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1UseMaxHp);
        }

        private void SetSoulWarriorP1HelperVisible(bool value)
        {
            soulWarriorP1HelperVisible = value;
            ApplyPanelVisible(soulWarriorP1HelperRoot, value, RefreshSoulWarriorP1HelperUi);
        }

        private static void ResetSoulWarriorP1HelperDefaults()
        {
            Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1UseMaxHp = false;
            Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1MaxHp = 750;
        }

        private void OnSoulWarriorP1HelperBackClicked()
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

            SetSoulWarriorP1HelperVisible(false);
        }

        private void OnSoulWarriorP1HelperResetDefaultsClicked()
        {
            ResetSoulWarriorP1HelperDefaults();
            SetSoulWarriorP1HelperEnabled(false);
            Modules.BossChallenge.SoulWarriorP1Helper.RestoreVanillaHealthIfPresent();
            RefreshSoulWarriorP1HelperUi();
        }
    }
}
