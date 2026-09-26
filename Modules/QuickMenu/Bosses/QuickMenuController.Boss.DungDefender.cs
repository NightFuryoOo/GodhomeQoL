using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int DungDefenderHelperCanvasSortOrder = 10038;
        private const float DungDefenderHelperPanelHeight = PanelHeight;
        private const float DungDefenderHelperRowSpacing = 44f;
        private RectTransform? dungDefenderHelperContent;
        private GameObject? dungDefenderHelperRoot;
        private bool dungDefenderHelperVisible;
        private Text? dungDefenderHelperToggleValue;
        private Image? dungDefenderHelperToggleIcon;
        private Text? dungDefenderP5HpValue;
        private Text? dungDefenderUseMaxHpValue;
        private Text? dungDefenderUseCustomPhaseValue;
        private InputField? dungDefenderMaxHpField;
        private InputField? dungDefenderPhase2HpField;
        private Module? dungDefenderHelperModule;

        private static void ResetDungDefenderHelperDefaults()
        {
            Modules.BossChallenge.DungDefenderHelper.dungDefenderUseMaxHp = false;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderP5Hp = false;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderMaxHp = 1100;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderMaxHpBeforeP5 = 1100;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderUseCustomPhase = false;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2Hp = 350;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2HpBeforeP5 = 350;
        }

        private Module? GetDungDefenderHelperModule()
        {
            return GetCachedModule(ref dungDefenderHelperModule, typeof(Modules.BossChallenge.DungDefenderHelper));
        }

        private bool GetDungDefenderHelperEnabled()
        {
            return GetDungDefenderHelperModule()?.Enabled ?? false;
        }

        private void SetDungDefenderHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetDungDefenderHelperModule(), value);
            UpdateDungDefenderHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetDungDefenderUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.DungDefenderHelper.dungDefenderP5Hp)
            {
                Modules.BossChallenge.DungDefenderHelper.dungDefenderUseMaxHp = true;
                RefreshDungDefenderHelperUi();
                return;
            }

            Modules.BossChallenge.DungDefenderHelper.dungDefenderUseMaxHp = value;
            Modules.BossChallenge.DungDefenderHelper.ReapplyLiveSettings();
            RefreshDungDefenderHelperUi();
        }

        private void SetDungDefenderUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.DungDefenderHelper.dungDefenderP5Hp)
            {
                Modules.BossChallenge.DungDefenderHelper.dungDefenderUseCustomPhase = false;
                RefreshDungDefenderHelperUi();
                return;
            }

            int maxPhase2Hp = Mathf.Max(1, Modules.BossChallenge.DungDefenderHelper.GetPhase2MaxHpForUi());
            Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2Hp = Mathf.Clamp(Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2Hp, 1, maxPhase2Hp);
            Modules.BossChallenge.DungDefenderHelper.dungDefenderUseCustomPhase = value;
            Modules.BossChallenge.DungDefenderHelper.ReapplyLiveSettings();
            RefreshDungDefenderHelperUi();
        }

        private void SetDungDefenderP5HpEnabled(bool value)
        {
            Modules.BossChallenge.DungDefenderHelper.SetP5HpEnabled(value);
            RefreshDungDefenderHelperUi();
        }

        private void SetDungDefenderMaxHp(int value)
        {
            if (Modules.BossChallenge.DungDefenderHelper.dungDefenderP5Hp)
            {
                Modules.BossChallenge.DungDefenderHelper.dungDefenderMaxHp = 800;
                RefreshDungDefenderHelperUi();
                return;
            }

            Modules.BossChallenge.DungDefenderHelper.dungDefenderMaxHp = Mathf.Clamp(value, 1, 999999);
            int maxPhase2Hp = Mathf.Max(1, Modules.BossChallenge.DungDefenderHelper.GetPhase2MaxHpForUi());
            Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2Hp = Mathf.Clamp(Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2Hp, 1, maxPhase2Hp);
            if (Modules.BossChallenge.DungDefenderHelper.dungDefenderUseMaxHp
                || Modules.BossChallenge.DungDefenderHelper.dungDefenderUseCustomPhase)
            {
                Modules.BossChallenge.DungDefenderHelper.ReapplyLiveSettings();
            }

            RefreshDungDefenderHelperUi();
        }

        private void SetDungDefenderPhase2Hp(int value)
        {
            if (Modules.BossChallenge.DungDefenderHelper.dungDefenderP5Hp)
            {
                Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2Hp = 350;
                RefreshDungDefenderHelperUi();
                return;
            }

            int maxPhase2Hp = Mathf.Max(1, Modules.BossChallenge.DungDefenderHelper.GetPhase2MaxHpForUi());
            Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2Hp = Mathf.Clamp(value, 1, maxPhase2Hp);
            if (Modules.BossChallenge.DungDefenderHelper.dungDefenderUseCustomPhase)
            {
                Modules.BossChallenge.DungDefenderHelper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshDungDefenderHelperUi();
        }

        private void RefreshDungDefenderHelperUi()
        {
            Module? module = GetDungDefenderHelperModule();
            UpdateToggleValue(dungDefenderHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(dungDefenderHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(dungDefenderP5HpValue, Modules.BossChallenge.DungDefenderHelper.dungDefenderP5Hp);
            UpdateToggleValue(dungDefenderUseMaxHpValue, Modules.BossChallenge.DungDefenderHelper.dungDefenderUseMaxHp);
            UpdateToggleValue(dungDefenderUseCustomPhaseValue, Modules.BossChallenge.DungDefenderHelper.dungDefenderUseCustomPhase);
            UpdateIntInputValue(dungDefenderMaxHpField, Modules.BossChallenge.DungDefenderHelper.dungDefenderMaxHp);
            UpdateIntInputValue(dungDefenderPhase2HpField, Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2Hp);

            UpdateDungDefenderHelperInteractivity();
        }

        private void UpdateDungDefenderHelperInteractivity()
        {
            SetContentInteractivity(dungDefenderHelperContent, GetDungDefenderHelperEnabled(), "DungDefenderHelperEnableRow");
            if (!GetDungDefenderHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.DungDefenderHelper.dungDefenderUseMaxHp;
            bool p5Hp = Modules.BossChallenge.DungDefenderHelper.dungDefenderP5Hp;
            bool useCustomPhase = Modules.BossChallenge.DungDefenderHelper.dungDefenderUseCustomPhase;
            SetRowInteractivity(dungDefenderHelperContent, "DungDefenderUseMaxHpRow", !p5Hp);
            SetRowInteractivity(dungDefenderHelperContent, "DungDefenderMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(dungDefenderHelperContent, "DungDefenderUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(dungDefenderHelperContent, "DungDefenderPhase2HpRow", useCustomPhase && !p5Hp);
        }

        private void SetDungDefenderHelperVisible(bool value)
        {
            dungDefenderHelperVisible = value;
            ApplyPanelVisible(dungDefenderHelperRoot, value, RefreshDungDefenderHelperUi);
        }

        private void OnBossManipulateDungDefenderClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetDungDefenderHelperVisible);
        }

        private void OnDungDefenderHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetDungDefenderHelperVisible);
        }

        private void OnDungDefenderHelperResetDefaultsClicked()
        {
            ResetDungDefenderHelperDefaults();
            SetDungDefenderHelperEnabled(false);
            Modules.BossChallenge.DungDefenderHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.DungDefenderHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshDungDefenderHelperUi();
        }

        private void BuildDungDefenderHelperOverlayUi()
        {
            dungDefenderHelperRoot = CreateOverlayFrame("DungDefenderHelperOverlayCanvas", DungDefenderHelperCanvasSortOrder, "DungDefenderHelperPanel", DungDefenderHelperPanelHeight, "Modules/DungDefenderHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = DungDefenderHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, DungDefenderHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            dungDefenderHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "DungDefenderHelperEnableRow",
                "Settings/DungDefenderHelper/Enable".Localize(),
                rowY,
                GetDungDefenderHelperEnabled,
                SetDungDefenderHelperEnabled,
                out dungDefenderHelperToggleValue,
                out dungDefenderHelperToggleIcon
            );

            lastY = rowY;
            rowY += DungDefenderHelperRowSpacing;
            CreateToggleRow(
                content,
                "DungDefenderP5HpRow",
                "Settings/DungDefenderHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.DungDefenderHelper.dungDefenderP5Hp,
                SetDungDefenderP5HpEnabled,
                out dungDefenderP5HpValue
            );

            lastY = rowY;
            rowY += DungDefenderHelperRowSpacing;
            CreateToggleRow(
                content,
                "DungDefenderUseMaxHpRow",
                "Settings/DungDefenderHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.DungDefenderHelper.dungDefenderUseMaxHp,
                SetDungDefenderUseMaxHpEnabled,
                out dungDefenderUseMaxHpValue
            );

            lastY = rowY;
            rowY += DungDefenderHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "DungDefenderMaxHpRow",
                "Settings/DungDefenderHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.DungDefenderHelper.dungDefenderMaxHp,
                SetDungDefenderMaxHp,
                1,
                999999,
                10,
                out dungDefenderMaxHpField
            );

            lastY = rowY;
            rowY += DungDefenderHelperRowSpacing;
            CreateToggleRow(
                content,
                "DungDefenderUseCustomPhaseRow",
                "Settings/DungDefenderHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.DungDefenderHelper.dungDefenderUseCustomPhase,
                SetDungDefenderUseCustomPhaseEnabled,
                out dungDefenderUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += DungDefenderHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "DungDefenderPhase2HpRow",
                "Settings/DungDefenderHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.DungDefenderHelper.dungDefenderPhase2Hp,
                SetDungDefenderPhase2Hp,
                1,
                999999,
                10,
                out dungDefenderPhase2HpField
            );

            lastY = rowY;
            rowY += DungDefenderHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "DungDefenderHelperResetRow", "Settings/DungDefenderHelper/Reset".Localize(), resetY, OnDungDefenderHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "DungDefenderHelperBackRow", "Back", backY, OnDungDefenderHelperBackClicked);
        }
    }
}
