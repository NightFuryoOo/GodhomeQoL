using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int PaintmasterSheoHelperCanvasSortOrder = 10050;
        private const float PaintmasterSheoHelperPanelHeight = PanelHeight;
        private const float PaintmasterSheoHelperRowSpacing = 44f;
        private RectTransform? paintmasterSheoHelperContent;
        private GameObject? paintmasterSheoHelperRoot;
        private bool paintmasterSheoHelperVisible;
        private Text? paintmasterSheoHelperToggleValue;
        private Image? paintmasterSheoHelperToggleIcon;
        private Text? paintmasterSheoP5HpValue;
        private Text? paintmasterSheoUseMaxHpValue;
        private InputField? paintmasterSheoMaxHpField;
        private Module? paintmasterSheoHelperModule;

        private static void ResetPaintmasterSheoHelperDefaults()
        {
            Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoUseMaxHp = false;
            Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoP5Hp = false;
            Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoMaxHp = 1450;
            Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoMaxHpBeforeP5 = 1450;
            Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoHasStoredStateBeforeP5 = false;
        }

        private Module? GetPaintmasterSheoHelperModule()
        {
            return GetCachedModule(ref paintmasterSheoHelperModule, typeof(Modules.BossChallenge.PaintmasterSheoHelper));
        }

        private bool GetPaintmasterSheoHelperEnabled()
        {
            return GetPaintmasterSheoHelperModule()?.Enabled ?? false;
        }

        private void SetPaintmasterSheoHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetPaintmasterSheoHelperModule(), value);
            UpdatePaintmasterSheoHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetPaintmasterSheoUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoP5Hp)
            {
                Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoUseMaxHp = true;
                RefreshPaintmasterSheoHelperUi();
                return;
            }

            Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoUseMaxHp = value;
            Modules.BossChallenge.PaintmasterSheoHelper.ReapplyLiveSettings();
            RefreshPaintmasterSheoHelperUi();
        }

        private void SetPaintmasterSheoP5HpEnabled(bool value)
        {
            Modules.BossChallenge.PaintmasterSheoHelper.SetP5HpEnabled(value);
            RefreshPaintmasterSheoHelperUi();
        }

        private void SetPaintmasterSheoMaxHp(int value)
        {
            if (Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoP5Hp)
            {
                Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoMaxHp = 950;
                RefreshPaintmasterSheoHelperUi();
                return;
            }

            Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoUseMaxHp)
            {
                Modules.BossChallenge.PaintmasterSheoHelper.ApplyPaintmasterSheoHealthIfPresent();
            }
        }

        private void RefreshPaintmasterSheoHelperUi()
        {
            Module? module = GetPaintmasterSheoHelperModule();
            UpdateToggleValue(paintmasterSheoHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(paintmasterSheoHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(paintmasterSheoP5HpValue, Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoP5Hp);
            UpdateToggleValue(paintmasterSheoUseMaxHpValue, Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoUseMaxHp);
            UpdateIntInputValue(paintmasterSheoMaxHpField, Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoMaxHp);

            UpdatePaintmasterSheoHelperInteractivity();
        }

        private void UpdatePaintmasterSheoHelperInteractivity()
        {
            SetContentInteractivity(paintmasterSheoHelperContent, GetPaintmasterSheoHelperEnabled(), "PaintmasterSheoHelperEnableRow");
            if (!GetPaintmasterSheoHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoUseMaxHp;
            bool p5Hp = Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoP5Hp;
            SetRowInteractivity(paintmasterSheoHelperContent, "PaintmasterSheoUseMaxHpRow", !p5Hp);
            SetRowInteractivity(paintmasterSheoHelperContent, "PaintmasterSheoMaxHpRow", useMaxHp && !p5Hp);
        }

        private void SetPaintmasterSheoHelperVisible(bool value)
        {
            paintmasterSheoHelperVisible = value;
            ApplyPanelVisible(paintmasterSheoHelperRoot, value, RefreshPaintmasterSheoHelperUi);
        }

        private void OnBossManipulatePaintmasterSheoClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetPaintmasterSheoHelperVisible);
        }

        private void OnPaintmasterSheoHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetPaintmasterSheoHelperVisible);
        }

        private void OnPaintmasterSheoHelperResetDefaultsClicked()
        {
            ResetPaintmasterSheoHelperDefaults();
            SetPaintmasterSheoHelperEnabled(false);
            Modules.BossChallenge.PaintmasterSheoHelper.RestoreVanillaHealthIfPresent();
            RefreshPaintmasterSheoHelperUi();
        }

        private void BuildPaintmasterSheoHelperOverlayUi()
        {
            BuildStandardGhostHelperOverlayUi(
                ref paintmasterSheoHelperRoot,
                ref paintmasterSheoHelperContent,
                "PaintmasterSheoHelper",
                PaintmasterSheoHelperCanvasSortOrder,
                PaintmasterSheoHelperPanelHeight,
                PaintmasterSheoHelperRowSpacing,
                GetPaintmasterSheoHelperEnabled,
                SetPaintmasterSheoHelperEnabled,
                () => Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoP5Hp,
                SetPaintmasterSheoP5HpEnabled,
                () => Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoUseMaxHp,
                SetPaintmasterSheoUseMaxHpEnabled,
                () => Modules.BossChallenge.PaintmasterSheoHelper.paintmasterSheoMaxHp,
                SetPaintmasterSheoMaxHp,
                OnPaintmasterSheoHelperResetDefaultsClicked,
                OnPaintmasterSheoHelperBackClicked,
                out paintmasterSheoHelperToggleValue,
                out paintmasterSheoHelperToggleIcon,
                out paintmasterSheoP5HpValue,
                out paintmasterSheoUseMaxHpValue,
                out paintmasterSheoMaxHpField);
        }
    }
}
