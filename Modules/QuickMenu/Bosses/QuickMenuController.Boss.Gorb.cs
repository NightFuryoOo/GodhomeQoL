using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int GorbHelperCanvasSortOrder = 10035;
        private const float GorbHelperPanelHeight = PanelHeight;
        private const float GorbHelperRowSpacing = 44f;
        private RectTransform? gorbHelperContent;
        private GameObject? gorbHelperRoot;
        private bool gorbHelperVisible;
        private Text? gorbHelperToggleValue;
        private Image? gorbHelperToggleIcon;
        private Text? gorbP5HpValue;
        private Text? gorbUseMaxHpValue;
        private Text? gorbUseCustomPhaseValue;
        private InputField? gorbMaxHpField;
        private InputField? gorbPhase2HpField;
        private InputField? gorbPhase3HpField;
        private Module? gorbHelperModule;

        private static void ResetGorbHelperDefaults()
        {
            Modules.BossChallenge.GorbHelper.gorbUseMaxHp = false;
            Modules.BossChallenge.GorbHelper.gorbP5Hp = false;
            Modules.BossChallenge.GorbHelper.gorbUseCustomPhase = false;
            Modules.BossChallenge.GorbHelper.gorbMaxHp = 1000;
            Modules.BossChallenge.GorbHelper.gorbPhase2Hp = 700;
            Modules.BossChallenge.GorbHelper.gorbPhase3Hp = 400;
            Modules.BossChallenge.GorbHelper.gorbMaxHpBeforeP5 = 1000;
            Modules.BossChallenge.GorbHelper.gorbUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.GorbHelper.gorbHasStoredStateBeforeP5 = false;
            Modules.BossChallenge.GorbHelper.gorbUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.GorbHelper.gorbPhase2HpBeforeP5 = 700;
            Modules.BossChallenge.GorbHelper.gorbPhase3HpBeforeP5 = 400;
        }

        private Module? GetGorbHelperModule()
        {
            return GetCachedModule(ref gorbHelperModule, typeof(Modules.BossChallenge.GorbHelper));
        }

        private bool GetGorbHelperEnabled()
        {
            return GetGorbHelperModule()?.Enabled ?? false;
        }

        private void SetGorbHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetGorbHelperModule(), value);
            UpdateGorbHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetGorbUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.GorbHelper.gorbP5Hp)
            {
                Modules.BossChallenge.GorbHelper.gorbUseMaxHp = true;
                RefreshGorbHelperUi();
                return;
            }

            Modules.BossChallenge.GorbHelper.gorbUseMaxHp = value;
            NormalizeGorbPhaseThresholdInputs();
            Modules.BossChallenge.GorbHelper.ReapplyLiveSettings();
            RefreshGorbHelperUi();
        }

        private void SetGorbUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.GorbHelper.gorbP5Hp)
            {
                Modules.BossChallenge.GorbHelper.gorbUseCustomPhase = false;
                RefreshGorbHelperUi();
                return;
            }

            int phase2MaxHp = GetGorbPhase2MaxHp();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.GorbHelper.gorbPhase2Hp, 2, phase2MaxHp);
            Modules.BossChallenge.GorbHelper.gorbPhase2Hp = phase2Hp;
            Modules.BossChallenge.GorbHelper.gorbPhase3Hp = Mathf.Clamp(Modules.BossChallenge.GorbHelper.gorbPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
            Modules.BossChallenge.GorbHelper.gorbUseCustomPhase = value;
            Modules.BossChallenge.GorbHelper.ReapplyLiveSettings();
            RefreshGorbHelperUi();
        }

        private void SetGorbP5HpEnabled(bool value)
        {
            Modules.BossChallenge.GorbHelper.SetP5HpEnabled(value);
            RefreshGorbHelperUi();
        }

        private void SetGorbMaxHp(int value)
        {
            if (Modules.BossChallenge.GorbHelper.gorbP5Hp)
            {
                Modules.BossChallenge.GorbHelper.gorbMaxHp = 650;
                RefreshGorbHelperUi();
                return;
            }

            Modules.BossChallenge.GorbHelper.gorbMaxHp = Mathf.Clamp(value, 1, 999999);
            NormalizeGorbPhaseThresholdInputs();
            if (Modules.BossChallenge.GorbHelper.gorbUseMaxHp)
            {
                Modules.BossChallenge.GorbHelper.ApplyGorbHealthIfPresent();
            }

            if (Modules.BossChallenge.GorbHelper.gorbUseCustomPhase)
            {
                Modules.BossChallenge.GorbHelper.ReapplyLiveSettings();
            }
        }

        private void SetGorbPhase2Hp(int value)
        {
            if (Modules.BossChallenge.GorbHelper.gorbP5Hp)
            {
                RefreshGorbHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(value, 2, GetGorbPhase2MaxHp());
            Modules.BossChallenge.GorbHelper.gorbPhase2Hp = phase2Hp;
            Modules.BossChallenge.GorbHelper.gorbPhase3Hp = Mathf.Clamp(Modules.BossChallenge.GorbHelper.gorbPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.GorbHelper.gorbUseCustomPhase)
            {
                Modules.BossChallenge.GorbHelper.ReapplyLiveSettings();
            }

            RefreshGorbHelperUi();
        }

        private void SetGorbPhase3Hp(int value)
        {
            if (Modules.BossChallenge.GorbHelper.gorbP5Hp)
            {
                RefreshGorbHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.GorbHelper.gorbPhase2Hp, 2, GetGorbPhase2MaxHp());
            Modules.BossChallenge.GorbHelper.gorbPhase2Hp = phase2Hp;
            Modules.BossChallenge.GorbHelper.gorbPhase3Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.GorbHelper.gorbUseCustomPhase)
            {
                Modules.BossChallenge.GorbHelper.ReapplyLiveSettings();
            }

            RefreshGorbHelperUi();
        }

        private int GetGorbPhase2MaxHp()
        {
            return Mathf.Clamp(Modules.BossChallenge.GorbHelper.GetPhase2MaxHpForUi(), 2, 999999);
        }

        private void NormalizeGorbPhaseThresholdInputs()
        {
            int phase2MaxHp = GetGorbPhase2MaxHp();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.GorbHelper.gorbPhase2Hp, 2, phase2MaxHp);
            Modules.BossChallenge.GorbHelper.gorbPhase2Hp = phase2Hp;
            Modules.BossChallenge.GorbHelper.gorbPhase3Hp = Mathf.Clamp(Modules.BossChallenge.GorbHelper.gorbPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
        }

        private void RefreshGorbHelperUi()
        {
            Module? module = GetGorbHelperModule();
            UpdateToggleValue(gorbHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(gorbHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(gorbP5HpValue, Modules.BossChallenge.GorbHelper.gorbP5Hp);
            UpdateToggleValue(gorbUseMaxHpValue, Modules.BossChallenge.GorbHelper.gorbUseMaxHp);
            UpdateToggleValue(gorbUseCustomPhaseValue, Modules.BossChallenge.GorbHelper.gorbUseCustomPhase);
            UpdateIntInputValue(gorbMaxHpField, Modules.BossChallenge.GorbHelper.gorbMaxHp);
            UpdateIntInputValue(gorbPhase2HpField, Modules.BossChallenge.GorbHelper.gorbPhase2Hp);
            UpdateIntInputValue(gorbPhase3HpField, Modules.BossChallenge.GorbHelper.gorbPhase3Hp);

            UpdateGorbHelperInteractivity();
        }

        private void UpdateGorbHelperInteractivity()
        {
            SetContentInteractivity(gorbHelperContent, GetGorbHelperEnabled(), "GorbHelperEnableRow");
            if (!GetGorbHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.GorbHelper.gorbUseMaxHp;
            bool p5Hp = Modules.BossChallenge.GorbHelper.gorbP5Hp;
            bool useCustomPhase = Modules.BossChallenge.GorbHelper.gorbUseCustomPhase;
            SetRowInteractivity(gorbHelperContent, "GorbUseMaxHpRow", !p5Hp);
            SetRowInteractivity(gorbHelperContent, "GorbMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(gorbHelperContent, "GorbUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(gorbHelperContent, "GorbPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(gorbHelperContent, "GorbPhase3HpRow", useCustomPhase && !p5Hp);
        }

        private void SetGorbHelperVisible(bool value)
        {
            gorbHelperVisible = value;
            ApplyPanelVisible(gorbHelperRoot, value, RefreshGorbHelperUi);
        }

        private void OnBossManipulateGorbClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetGorbHelperVisible);
        }

        private void OnGorbHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetGorbHelperVisible);
        }

        private void OnGorbHelperResetDefaultsClicked()
        {
            ResetGorbHelperDefaults();
            SetGorbHelperEnabled(false);
            Modules.BossChallenge.GorbHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.GorbHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshGorbHelperUi();
        }

        private void BuildGorbHelperOverlayUi()
        {
            gorbHelperRoot = CreateOverlayFrame("GorbHelperOverlayCanvas", GorbHelperCanvasSortOrder, "GorbHelperPanel", GorbHelperPanelHeight, "Modules/GorbHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            RectTransform content = CreateOverlayContent(panel, GorbHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            gorbHelperContent = content;

            float rowY = GetRowStartY(GorbHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "GorbHelperEnableRow",
                "Settings/GorbHelper/Enable".Localize(),
                rowY,
                GetGorbHelperEnabled,
                SetGorbHelperEnabled,
                out gorbHelperToggleValue,
                out gorbHelperToggleIcon
            );

            lastY = rowY;
            rowY += GorbHelperRowSpacing;
            CreateToggleRow(
                content,
                "GorbP5HpRow",
                "Settings/GorbHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.GorbHelper.gorbP5Hp,
                SetGorbP5HpEnabled,
                out gorbP5HpValue
            );

            lastY = rowY;
            rowY += GorbHelperRowSpacing;
            CreateToggleRow(
                content,
                "GorbUseMaxHpRow",
                "Settings/GorbHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.GorbHelper.gorbUseMaxHp,
                SetGorbUseMaxHpEnabled,
                out gorbUseMaxHpValue
            );

            lastY = rowY;
            rowY += GorbHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GorbMaxHpRow",
                "Settings/GorbHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.GorbHelper.gorbMaxHp,
                SetGorbMaxHp,
                1,
                999999,
                10,
                out gorbMaxHpField
            );

            lastY = rowY;
            rowY += GorbHelperRowSpacing;
            CreateToggleRow(
                content,
                "GorbUseCustomPhaseRow",
                "Settings/GorbHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.GorbHelper.gorbUseCustomPhase,
                SetGorbUseCustomPhaseEnabled,
                out gorbUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += GorbHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GorbPhase2HpRow",
                "Settings/GorbHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.GorbHelper.gorbPhase2Hp,
                SetGorbPhase2Hp,
                2,
                999999,
                10,
                out gorbPhase2HpField
            );

            lastY = rowY;
            rowY += GorbHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GorbPhase3HpRow",
                "Settings/GorbHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.GorbHelper.gorbPhase3Hp,
                SetGorbPhase3Hp,
                1,
                999999,
                10,
                out gorbPhase3HpField
            );

            lastY = rowY;
            rowY += GorbHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "GorbHelperResetRow", "Settings/GorbHelper/Reset".Localize(), resetY, OnGorbHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "GorbHelperBackRow", "Back", backY, OnGorbHelperBackClicked);
        }
    }
}
