using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int UumuuHelperCanvasSortOrder = 10044;
        private const float UumuuHelperPanelHeight = PanelHeight;
        private const float UumuuHelperRowSpacing = 44f;
        private RectTransform? uumuuHelperContent;
        private GameObject? uumuuHelperRoot;
        private bool uumuuHelperVisible;
        private Text? uumuuHelperToggleValue;
        private Image? uumuuHelperToggleIcon;
        private Text? uumuuP5HpValue;
        private Text? uumuuUseMaxHpValue;
        private Text? uumuuUseCustomSummonHpValue;
        private InputField? uumuuMaxHpField;
        private InputField? uumuuSummonHpField;
        private Module? uumuuHelperModule;

        private static void ResetUumuuHelperDefaults()
        {
            Modules.BossChallenge.UumuuHelper.uumuuUseMaxHp = false;
            Modules.BossChallenge.UumuuHelper.uumuuP5Hp = false;
            Modules.BossChallenge.UumuuHelper.uumuuUseCustomSummonHp = false;
            Modules.BossChallenge.UumuuHelper.uumuuMaxHp = 700;
            Modules.BossChallenge.UumuuHelper.uumuuSummonHp = 1;
            Modules.BossChallenge.UumuuHelper.uumuuMaxHpBeforeP5 = 700;
            Modules.BossChallenge.UumuuHelper.uumuuUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.UumuuHelper.uumuuUseCustomSummonHpBeforeP5 = false;
            Modules.BossChallenge.UumuuHelper.uumuuSummonHpBeforeP5 = 1;
            Modules.BossChallenge.UumuuHelper.uumuuHasStoredStateBeforeP5 = false;
        }

        private Module? GetUumuuHelperModule()
        {
            return GetCachedModule(ref uumuuHelperModule, typeof(Modules.BossChallenge.UumuuHelper));
        }

        private bool GetUumuuHelperEnabled()
        {
            return GetUumuuHelperModule()?.Enabled ?? false;
        }

        private void SetUumuuHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetUumuuHelperModule(), value);
            UpdateUumuuHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetUumuuUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.UumuuHelper.uumuuP5Hp)
            {
                Modules.BossChallenge.UumuuHelper.uumuuUseMaxHp = true;
                RefreshUumuuHelperUi();
                return;
            }

            Modules.BossChallenge.UumuuHelper.uumuuUseMaxHp = value;
            Modules.BossChallenge.UumuuHelper.ReapplyLiveSettings();
            RefreshUumuuHelperUi();
        }

        private void SetUumuuUseCustomSummonHpEnabled(bool value)
        {
            if (Modules.BossChallenge.UumuuHelper.uumuuP5Hp)
            {
                Modules.BossChallenge.UumuuHelper.uumuuUseCustomSummonHp = false;
                RefreshUumuuHelperUi();
                return;
            }

            Modules.BossChallenge.UumuuHelper.uumuuUseCustomSummonHp = value;
            Modules.BossChallenge.UumuuHelper.ReapplyLiveSettings();
            RefreshUumuuHelperUi();
        }

        private void SetUumuuP5HpEnabled(bool value)
        {
            Modules.BossChallenge.UumuuHelper.SetP5HpEnabled(value);
            RefreshUumuuHelperUi();
        }

        private void SetUumuuMaxHp(int value)
        {
            if (Modules.BossChallenge.UumuuHelper.uumuuP5Hp)
            {
                Modules.BossChallenge.UumuuHelper.uumuuMaxHp = 350;
                RefreshUumuuHelperUi();
                return;
            }

            Modules.BossChallenge.UumuuHelper.uumuuMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.UumuuHelper.uumuuUseMaxHp)
            {
                Modules.BossChallenge.UumuuHelper.ApplyUumuuHealthIfPresent();
            }

            RefreshUumuuHelperUi();
        }

        private void SetUumuuSummonHp(int value)
        {
            if (Modules.BossChallenge.UumuuHelper.uumuuP5Hp)
            {
                Modules.BossChallenge.UumuuHelper.uumuuSummonHp = 1;
                RefreshUumuuHelperUi();
                return;
            }

            Modules.BossChallenge.UumuuHelper.uumuuSummonHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.UumuuHelper.uumuuUseCustomSummonHp)
            {
                Modules.BossChallenge.UumuuHelper.ApplyUumuuSummonHealthIfPresent();
            }

            RefreshUumuuHelperUi();
        }

        private void RefreshUumuuHelperUi()
        {
            Module? module = GetUumuuHelperModule();
            UpdateToggleValue(uumuuHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(uumuuHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(uumuuP5HpValue, Modules.BossChallenge.UumuuHelper.uumuuP5Hp);
            UpdateToggleValue(uumuuUseMaxHpValue, Modules.BossChallenge.UumuuHelper.uumuuUseMaxHp);
            UpdateToggleValue(uumuuUseCustomSummonHpValue, Modules.BossChallenge.UumuuHelper.uumuuUseCustomSummonHp);
            UpdateIntInputValue(uumuuMaxHpField, Modules.BossChallenge.UumuuHelper.uumuuMaxHp);
            UpdateIntInputValue(uumuuSummonHpField, Modules.BossChallenge.UumuuHelper.uumuuSummonHp);

            UpdateUumuuHelperInteractivity();
        }

        private void UpdateUumuuHelperInteractivity()
        {
            SetContentInteractivity(uumuuHelperContent, GetUumuuHelperEnabled(), "UumuuHelperEnableRow");
            if (!GetUumuuHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.UumuuHelper.uumuuUseMaxHp;
            bool p5Hp = Modules.BossChallenge.UumuuHelper.uumuuP5Hp;
            bool useCustomSummonHp = Modules.BossChallenge.UumuuHelper.uumuuUseCustomSummonHp;
            SetRowInteractivity(uumuuHelperContent, "UumuuUseMaxHpRow", !p5Hp);
            SetRowInteractivity(uumuuHelperContent, "UumuuMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(uumuuHelperContent, "UumuuUseCustomSummonHpRow", !p5Hp);
            SetRowInteractivity(uumuuHelperContent, "UumuuSummonHpRow", useCustomSummonHp && !p5Hp);
        }

        private void SetUumuuHelperVisible(bool value)
        {
            uumuuHelperVisible = value;
            ApplyPanelVisible(uumuuHelperRoot, value, RefreshUumuuHelperUi);
        }

        private void OnBossManipulateUumuuClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetUumuuHelperVisible);
        }

        private void OnUumuuHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetUumuuHelperVisible);
        }

        private void OnUumuuHelperResetDefaultsClicked()
        {
            ResetUumuuHelperDefaults();
            SetUumuuHelperEnabled(false);
            Modules.BossChallenge.UumuuHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.UumuuHelper.RestoreVanillaSummonHealthIfPresent();
            RefreshUumuuHelperUi();
        }

        private void BuildUumuuHelperOverlayUi()
        {
            uumuuHelperRoot = CreateOverlayFrame("UumuuHelperOverlayCanvas", UumuuHelperCanvasSortOrder, "UumuuHelperPanel", UumuuHelperPanelHeight, "Modules/UumuuHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = UumuuHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, UumuuHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            uumuuHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "UumuuHelperEnableRow",
                "Settings/UumuuHelper/Enable".Localize(),
                rowY,
                GetUumuuHelperEnabled,
                SetUumuuHelperEnabled,
                out uumuuHelperToggleValue,
                out uumuuHelperToggleIcon
            );

            lastY = rowY;
            rowY += UumuuHelperRowSpacing;
            CreateToggleRow(
                content,
                "UumuuP5HpRow",
                "Settings/UumuuHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.UumuuHelper.uumuuP5Hp,
                SetUumuuP5HpEnabled,
                out uumuuP5HpValue
            );

            lastY = rowY;
            rowY += UumuuHelperRowSpacing;
            CreateToggleRow(
                content,
                "UumuuUseMaxHpRow",
                "Settings/UumuuHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.UumuuHelper.uumuuUseMaxHp,
                SetUumuuUseMaxHpEnabled,
                out uumuuUseMaxHpValue
            );

            lastY = rowY;
            rowY += UumuuHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "UumuuMaxHpRow",
                "Settings/UumuuHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.UumuuHelper.uumuuMaxHp,
                SetUumuuMaxHp,
                1,
                999999,
                10,
                out uumuuMaxHpField
            );

            lastY = rowY;
            rowY += UumuuHelperRowSpacing;
            CreateToggleRow(
                content,
                "UumuuUseCustomSummonHpRow",
                "Settings/UumuuHelper/UseCustomSummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.UumuuHelper.uumuuUseCustomSummonHp,
                SetUumuuUseCustomSummonHpEnabled,
                out uumuuUseCustomSummonHpValue
            );

            lastY = rowY;
            rowY += UumuuHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "UumuuSummonHpRow",
                "Settings/UumuuHelper/SummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.UumuuHelper.uumuuSummonHp,
                SetUumuuSummonHp,
                1,
                999999,
                1,
                out uumuuSummonHpField
            );

            lastY = rowY;
            rowY += UumuuHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "UumuuHelperResetRow", "Settings/UumuuHelper/Reset".Localize(), resetY, OnUumuuHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "UumuuHelperBackRow", "Back", backY, OnUumuuHelperBackClicked);
        }
    }
}
