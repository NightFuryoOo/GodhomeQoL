using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int MarkothHelperCanvasSortOrder = 10033;
        private const float MarkothHelperPanelHeight = PanelHeight;
        private const float MarkothHelperRowSpacing = 44f;
        private RectTransform? markothHelperContent;
        private GameObject? markothHelperRoot;
        private bool markothHelperVisible;
        private Text? markothHelperToggleValue;
        private Image? markothHelperToggleIcon;
        private Text? markothP5HpValue;
        private Text? markothUseMaxHpValue;
        private Text? markothUseCustomPhaseValue;
        private InputField? markothMaxHpField;
        private InputField? markothPhase2HpField;
        private Module? markothHelperModule;

        private static void ResetMarkothHelperDefaults()
        {
            Modules.BossChallenge.MarkothHelper.markothUseMaxHp = false;
            Modules.BossChallenge.MarkothHelper.markothP5Hp = false;
            Modules.BossChallenge.MarkothHelper.markothUseCustomPhase = false;
            Modules.BossChallenge.MarkothHelper.markothMaxHp = 950;
            Modules.BossChallenge.MarkothHelper.markothPhase2Hp = 475;
            Modules.BossChallenge.MarkothHelper.markothMaxHpBeforeP5 = 950;
            Modules.BossChallenge.MarkothHelper.markothUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.MarkothHelper.markothUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.MarkothHelper.markothPhase2HpBeforeP5 = 475;
            Modules.BossChallenge.MarkothHelper.markothHasStoredStateBeforeP5 = false;
        }

        private Module? GetMarkothHelperModule()
        {
            return GetCachedModule(ref markothHelperModule, typeof(Modules.BossChallenge.MarkothHelper));
        }

        private bool GetMarkothHelperEnabled()
        {
            return GetMarkothHelperModule()?.Enabled ?? false;
        }

        private void SetMarkothHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetMarkothHelperModule(), value);
            UpdateMarkothHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetMarkothUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.MarkothHelper.markothP5Hp)
            {
                Modules.BossChallenge.MarkothHelper.markothUseMaxHp = true;
                RefreshMarkothHelperUi();
                return;
            }

            Modules.BossChallenge.MarkothHelper.markothUseMaxHp = value;
            Modules.BossChallenge.MarkothHelper.markothPhase2Hp = Mathf.Clamp(Modules.BossChallenge.MarkothHelper.markothPhase2Hp, 1, GetMarkothPhase2MaxHp());
            Modules.BossChallenge.MarkothHelper.ReapplyLiveSettings();
            RefreshMarkothHelperUi();
        }

        private void SetMarkothUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.MarkothHelper.markothP5Hp)
            {
                Modules.BossChallenge.MarkothHelper.markothUseCustomPhase = false;
                RefreshMarkothHelperUi();
                return;
            }

            Modules.BossChallenge.MarkothHelper.markothPhase2Hp = Mathf.Clamp(Modules.BossChallenge.MarkothHelper.markothPhase2Hp, 1, GetMarkothPhase2MaxHp());
            Modules.BossChallenge.MarkothHelper.markothUseCustomPhase = value;
            Modules.BossChallenge.MarkothHelper.ReapplyLiveSettings();
            RefreshMarkothHelperUi();
        }

        private void SetMarkothP5HpEnabled(bool value)
        {
            Modules.BossChallenge.MarkothHelper.SetP5HpEnabled(value);
            RefreshMarkothHelperUi();
        }

        private void SetMarkothMaxHp(int value)
        {
            if (Modules.BossChallenge.MarkothHelper.markothP5Hp)
            {
                Modules.BossChallenge.MarkothHelper.markothMaxHp = 650;
                RefreshMarkothHelperUi();
                return;
            }

            Modules.BossChallenge.MarkothHelper.markothMaxHp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.MarkothHelper.markothPhase2Hp = Mathf.Clamp(Modules.BossChallenge.MarkothHelper.markothPhase2Hp, 1, GetMarkothPhase2MaxHp());
            if (Modules.BossChallenge.MarkothHelper.markothUseMaxHp)
            {
                Modules.BossChallenge.MarkothHelper.ApplyMarkothHealthIfPresent();
            }

            if (Modules.BossChallenge.MarkothHelper.markothUseCustomPhase)
            {
                Modules.BossChallenge.MarkothHelper.ReapplyLiveSettings();
            }

            RefreshMarkothHelperUi();
        }

        private void SetMarkothPhase2Hp(int value)
        {
            if (Modules.BossChallenge.MarkothHelper.markothP5Hp)
            {
                RefreshMarkothHelperUi();
                return;
            }

            Modules.BossChallenge.MarkothHelper.markothPhase2Hp = Mathf.Clamp(value, 1, GetMarkothPhase2MaxHp());

            if (Modules.BossChallenge.MarkothHelper.markothUseCustomPhase)
            {
                Modules.BossChallenge.MarkothHelper.ReapplyLiveSettings();
            }

            RefreshMarkothHelperUi();
        }

        private static int GetMarkothPhase2MaxHp()
        {
            if (Modules.BossChallenge.MarkothHelper.markothP5Hp)
            {
                return 650;
            }

            if (Modules.BossChallenge.MarkothHelper.markothUseMaxHp)
            {
                return Mathf.Clamp(Modules.BossChallenge.MarkothHelper.markothMaxHp, 1, 999999);
            }

            return 950;
        }

        private void RefreshMarkothHelperUi()
        {
            Module? module = GetMarkothHelperModule();
            UpdateToggleValue(markothHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(markothHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(markothP5HpValue, Modules.BossChallenge.MarkothHelper.markothP5Hp);
            UpdateToggleValue(markothUseMaxHpValue, Modules.BossChallenge.MarkothHelper.markothUseMaxHp);
            UpdateToggleValue(markothUseCustomPhaseValue, Modules.BossChallenge.MarkothHelper.markothUseCustomPhase);
            UpdateIntInputValue(markothMaxHpField, Modules.BossChallenge.MarkothHelper.markothMaxHp);
            UpdateIntInputValue(markothPhase2HpField, Modules.BossChallenge.MarkothHelper.markothPhase2Hp);

            UpdateMarkothHelperInteractivity();
        }

        private void UpdateMarkothHelperInteractivity()
        {
            SetContentInteractivity(markothHelperContent, GetMarkothHelperEnabled(), "MarkothHelperEnableRow");
            if (!GetMarkothHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.MarkothHelper.markothUseMaxHp;
            bool p5Hp = Modules.BossChallenge.MarkothHelper.markothP5Hp;
            bool useCustomPhase = Modules.BossChallenge.MarkothHelper.markothUseCustomPhase;
            SetRowInteractivity(markothHelperContent, "MarkothUseMaxHpRow", !p5Hp);
            SetRowInteractivity(markothHelperContent, "MarkothMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(markothHelperContent, "MarkothUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(markothHelperContent, "MarkothPhase2HpRow", useCustomPhase && !p5Hp);
        }

        private void SetMarkothHelperVisible(bool value)
        {
            markothHelperVisible = value;
            ApplyPanelVisible(markothHelperRoot, value, RefreshMarkothHelperUi);
        }

        private void OnBossManipulateMarkothClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetMarkothHelperVisible);
        }

        private void OnMarkothHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetMarkothHelperVisible);
        }

        private void OnMarkothHelperResetDefaultsClicked()
        {
            ResetMarkothHelperDefaults();
            SetMarkothHelperEnabled(false);
            Modules.BossChallenge.MarkothHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.MarkothHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshMarkothHelperUi();
        }

        private void BuildMarkothHelperOverlayUi()
        {
            markothHelperRoot = CreateOverlayFrame("MarkothHelperOverlayCanvas", MarkothHelperCanvasSortOrder, "MarkothHelperPanel", MarkothHelperPanelHeight, "Modules/MarkothHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            RectTransform content = CreateOverlayContent(panel, MarkothHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            markothHelperContent = content;

            float rowY = GetRowStartY(MarkothHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "MarkothHelperEnableRow",
                "Settings/MarkothHelper/Enable".Localize(),
                rowY,
                GetMarkothHelperEnabled,
                SetMarkothHelperEnabled,
                out markothHelperToggleValue,
                out markothHelperToggleIcon
            );

            lastY = rowY;
            rowY += MarkothHelperRowSpacing;
            CreateToggleRow(
                content,
                "MarkothP5HpRow",
                "Settings/MarkothHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.MarkothHelper.markothP5Hp,
                SetMarkothP5HpEnabled,
                out markothP5HpValue
            );

            lastY = rowY;
            rowY += MarkothHelperRowSpacing;
            CreateToggleRow(
                content,
                "MarkothUseMaxHpRow",
                "Settings/MarkothHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.MarkothHelper.markothUseMaxHp,
                SetMarkothUseMaxHpEnabled,
                out markothUseMaxHpValue
            );

            lastY = rowY;
            rowY += MarkothHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MarkothMaxHpRow",
                "Settings/MarkothHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.MarkothHelper.markothMaxHp,
                SetMarkothMaxHp,
                1,
                999999,
                10,
                out markothMaxHpField
            );

            lastY = rowY;
            rowY += MarkothHelperRowSpacing;
            CreateToggleRow(
                content,
                "MarkothUseCustomPhaseRow",
                "Settings/MarkothHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.MarkothHelper.markothUseCustomPhase,
                SetMarkothUseCustomPhaseEnabled,
                out markothUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += MarkothHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MarkothPhase2HpRow",
                "Settings/MarkothHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.MarkothHelper.markothPhase2Hp,
                SetMarkothPhase2Hp,
                1,
                999999,
                10,
                out markothPhase2HpField
            );

            lastY = rowY;
            rowY += MarkothHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "MarkothHelperResetRow", "Settings/MarkothHelper/Reset".Localize(), resetY, OnMarkothHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "MarkothHelperBackRow", "Back", backY, OnMarkothHelperBackClicked);
        }
    }
}
