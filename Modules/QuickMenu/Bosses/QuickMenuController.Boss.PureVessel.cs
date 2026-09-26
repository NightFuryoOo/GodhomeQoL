using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int PureVesselHelperCanvasSortOrder = 10048;
        private const float PureVesselHelperPanelHeight = PanelHeight;
        private const float PureVesselHelperRowSpacing = 44f;
        private RectTransform? pureVesselHelperContent;
        private GameObject? pureVesselHelperRoot;
        private bool pureVesselHelperVisible;
        private Text? pureVesselHelperToggleValue;
        private Image? pureVesselHelperToggleIcon;
        private Text? pureVesselP5HpValue;
        private Text? pureVesselUseMaxHpValue;
        private Text? pureVesselUseCustomPhaseValue;
        private InputField? pureVesselMaxHpField;
        private InputField? pureVesselPhase2HpField;
        private InputField? pureVesselPhase3HpField;
        private Module? pureVesselHelperModule;

        private static void ResetPureVesselHelperDefaults()
        {
            Modules.BossChallenge.PureVesselHelper.pureVesselUseMaxHp = false;
            Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp = false;
            Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhase = false;
            Modules.BossChallenge.PureVesselHelper.pureVesselMaxHp = 1850;
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase2Hp = 1232;
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase3Hp = 616;
            Modules.BossChallenge.PureVesselHelper.pureVesselMaxHpBeforeP5 = 1850;
            Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase2HpBeforeP5 = 1232;
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase3HpBeforeP5 = 616;
            Modules.BossChallenge.PureVesselHelper.pureVesselUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.PureVesselHelper.pureVesselHasStoredStateBeforeP5 = false;
        }

        private Module? GetPureVesselHelperModule()
        {
            return GetCachedModule(ref pureVesselHelperModule, typeof(Modules.BossChallenge.PureVesselHelper));
        }

        private bool GetPureVesselHelperEnabled()
        {
            return GetPureVesselHelperModule()?.Enabled ?? false;
        }

        private void SetPureVesselHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetPureVesselHelperModule(), value);
            UpdatePureVesselHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetPureVesselUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp)
            {
                Modules.BossChallenge.PureVesselHelper.pureVesselUseMaxHp = true;
                RefreshPureVesselHelperUi();
                return;
            }

            Modules.BossChallenge.PureVesselHelper.pureVesselUseMaxHp = value;
            Modules.BossChallenge.PureVesselHelper.ReapplyLiveSettings();
            RefreshPureVesselHelperUi();
        }

        private void SetPureVesselUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp)
            {
                Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhase = false;
                RefreshPureVesselHelperUi();
                return;
            }

            ClampPureVesselPhaseThresholdsForUi();
            Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhase = value;
            Modules.BossChallenge.PureVesselHelper.ReapplyLiveSettings();
            RefreshPureVesselHelperUi();
        }

        private void SetPureVesselP5HpEnabled(bool value)
        {
            Modules.BossChallenge.PureVesselHelper.SetP5HpEnabled(value);
            RefreshPureVesselHelperUi();
        }

        private void SetPureVesselMaxHp(int value)
        {
            if (Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp)
            {
                Modules.BossChallenge.PureVesselHelper.pureVesselMaxHp = 1600;
                RefreshPureVesselHelperUi();
                return;
            }

            Modules.BossChallenge.PureVesselHelper.pureVesselMaxHp = Mathf.Clamp(value, 1, 999999);
            ClampPureVesselPhaseThresholdsForUi();
            if (Modules.BossChallenge.PureVesselHelper.pureVesselUseMaxHp || Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhase)
            {
                Modules.BossChallenge.PureVesselHelper.ReapplyLiveSettings();
            }

            RefreshPureVesselHelperUi();
        }

        private void SetPureVesselPhase2Hp(int value)
        {
            if (Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp)
            {
                RefreshPureVesselHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(value, 2, Modules.BossChallenge.PureVesselHelper.GetPhase2MaxHpForUi());
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase2Hp = phase2Hp;
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase3Hp = Mathf.Clamp(Modules.BossChallenge.PureVesselHelper.pureVesselPhase3Hp, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhase)
            {
                Modules.BossChallenge.PureVesselHelper.ReapplyLiveSettings();
            }

            RefreshPureVesselHelperUi();
        }

        private void SetPureVesselPhase3Hp(int value)
        {
            if (Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp)
            {
                RefreshPureVesselHelperUi();
                return;
            }

            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.PureVesselHelper.pureVesselPhase2Hp, 2, Modules.BossChallenge.PureVesselHelper.GetPhase2MaxHpForUi());
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase2Hp = phase2Hp;
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase3Hp = Mathf.Clamp(value, 1, Modules.BossChallenge.PureVesselHelper.GetPhase3MaxHpForUi());

            if (Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhase)
            {
                Modules.BossChallenge.PureVesselHelper.ReapplyLiveSettings();
            }

            RefreshPureVesselHelperUi();
        }

        private static void ClampPureVesselPhaseThresholdsForUi()
        {
            int phase2Hp = Mathf.Clamp(
                Modules.BossChallenge.PureVesselHelper.pureVesselPhase2Hp,
                2,
                Modules.BossChallenge.PureVesselHelper.GetPhase2MaxHpForUi()
            );
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase2Hp = phase2Hp;
            Modules.BossChallenge.PureVesselHelper.pureVesselPhase3Hp = Mathf.Clamp(
                Modules.BossChallenge.PureVesselHelper.pureVesselPhase3Hp,
                1,
                Mathf.Max(1, phase2Hp - 1)
            );
        }

        private void RefreshPureVesselHelperUi()
        {
            Module? module = GetPureVesselHelperModule();
            UpdateToggleValue(pureVesselHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(pureVesselHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(pureVesselP5HpValue, Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp);
            UpdateToggleValue(pureVesselUseMaxHpValue, Modules.BossChallenge.PureVesselHelper.pureVesselUseMaxHp);
            UpdateToggleValue(pureVesselUseCustomPhaseValue, Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhase);
            UpdateIntInputValue(pureVesselMaxHpField, Modules.BossChallenge.PureVesselHelper.pureVesselMaxHp);
            UpdateIntInputValue(pureVesselPhase2HpField, Modules.BossChallenge.PureVesselHelper.pureVesselPhase2Hp);
            UpdateIntInputValue(pureVesselPhase3HpField, Modules.BossChallenge.PureVesselHelper.pureVesselPhase3Hp);

            UpdatePureVesselHelperInteractivity();
        }

        private void UpdatePureVesselHelperInteractivity()
        {
            SetContentInteractivity(pureVesselHelperContent, GetPureVesselHelperEnabled(), "PureVesselHelperEnableRow");
            if (!GetPureVesselHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.PureVesselHelper.pureVesselUseMaxHp;
            bool p5Hp = Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp;
            bool useCustomPhase = Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhase;
            SetRowInteractivity(pureVesselHelperContent, "PureVesselUseMaxHpRow", !p5Hp);
            SetRowInteractivity(pureVesselHelperContent, "PureVesselMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(pureVesselHelperContent, "PureVesselUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(pureVesselHelperContent, "PureVesselPhase2HpRow", useCustomPhase && !p5Hp);
            SetRowInteractivity(pureVesselHelperContent, "PureVesselPhase3HpRow", useCustomPhase && !p5Hp);
        }

        private void SetPureVesselHelperVisible(bool value)
        {
            pureVesselHelperVisible = value;
            ApplyPanelVisible(pureVesselHelperRoot, value, RefreshPureVesselHelperUi);
        }

        private void OnBossManipulatePureVesselClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetPureVesselHelperVisible);
        }

        private void OnPureVesselHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetPureVesselHelperVisible);
        }

        private void OnPureVesselHelperResetDefaultsClicked()
        {
            ResetPureVesselHelperDefaults();
            SetPureVesselHelperEnabled(false);
            Modules.BossChallenge.PureVesselHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.PureVesselHelper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshPureVesselHelperUi();
        }

        private void BuildPureVesselHelperOverlayUi()
        {
            pureVesselHelperRoot = CreateOverlayFrame("PureVesselHelperOverlayCanvas", PureVesselHelperCanvasSortOrder, "PureVesselHelperPanel", PureVesselHelperPanelHeight, "Modules/PureVesselHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = PureVesselHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, PureVesselHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            pureVesselHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "PureVesselHelperEnableRow",
                "Settings/PureVesselHelper/Enable".Localize(),
                rowY,
                GetPureVesselHelperEnabled,
                SetPureVesselHelperEnabled,
                out pureVesselHelperToggleValue,
                out pureVesselHelperToggleIcon
            );

            lastY = rowY;
            rowY += PureVesselHelperRowSpacing;
            CreateToggleRow(
                content,
                "PureVesselP5HpRow",
                "Settings/PureVesselHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.PureVesselHelper.pureVesselP5Hp,
                SetPureVesselP5HpEnabled,
                out pureVesselP5HpValue
            );

            lastY = rowY;
            rowY += PureVesselHelperRowSpacing;
            CreateToggleRow(
                content,
                "PureVesselUseMaxHpRow",
                "Settings/PureVesselHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.PureVesselHelper.pureVesselUseMaxHp,
                SetPureVesselUseMaxHpEnabled,
                out pureVesselUseMaxHpValue
            );

            lastY = rowY;
            rowY += PureVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "PureVesselMaxHpRow",
                "Settings/PureVesselHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.PureVesselHelper.pureVesselMaxHp,
                SetPureVesselMaxHp,
                1,
                999999,
                10,
                out pureVesselMaxHpField
            );

            lastY = rowY;
            rowY += PureVesselHelperRowSpacing;
            CreateToggleRow(
                content,
                "PureVesselUseCustomPhaseRow",
                "Settings/PureVesselHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.PureVesselHelper.pureVesselUseCustomPhase,
                SetPureVesselUseCustomPhaseEnabled,
                out pureVesselUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += PureVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "PureVesselPhase2HpRow",
                "Settings/PureVesselHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.PureVesselHelper.pureVesselPhase2Hp,
                SetPureVesselPhase2Hp,
                2,
                999999,
                10,
                out pureVesselPhase2HpField
            );

            lastY = rowY;
            rowY += PureVesselHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "PureVesselPhase3HpRow",
                "Settings/PureVesselHelper/Phase3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.PureVesselHelper.pureVesselPhase3Hp,
                SetPureVesselPhase3Hp,
                1,
                999998,
                10,
                out pureVesselPhase3HpField
            );

            lastY = rowY;
            rowY += PureVesselHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "PureVesselHelperResetRow", "Settings/PureVesselHelper/Reset".Localize(), resetY, OnPureVesselHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "PureVesselHelperBackRow", "Back", backY, OnPureVesselHelperBackClicked);
        }
    }
}
