using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int ElderHuHelperCanvasSortOrder = 10036;
        private const float ElderHuHelperPanelHeight = PanelHeight;
        private const float ElderHuHelperRowSpacing = 44f;
        private RectTransform? elderHuHelperContent;
        private GameObject? elderHuHelperRoot;
        private bool elderHuHelperVisible;
        private Text? elderHuHelperToggleValue;
        private Image? elderHuHelperToggleIcon;
        private Text? elderHuP5HpValue;
        private Text? elderHuUseMaxHpValue;
        private InputField? elderHuMaxHpField;
        private Module? elderHuHelperModule;

        private static void ResetElderHuHelperDefaults()
        {
            Modules.BossChallenge.ElderHuHelper.elderHuUseMaxHp = false;
            Modules.BossChallenge.ElderHuHelper.elderHuP5Hp = false;
            Modules.BossChallenge.ElderHuHelper.elderHuMaxHp = 800;
            Modules.BossChallenge.ElderHuHelper.elderHuMaxHpBeforeP5 = 800;
            Modules.BossChallenge.ElderHuHelper.elderHuUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.ElderHuHelper.elderHuHasStoredStateBeforeP5 = false;
        }

        private Module? GetElderHuHelperModule()
        {
            return GetCachedModule(ref elderHuHelperModule, typeof(Modules.BossChallenge.ElderHuHelper));
        }

        private bool GetElderHuHelperEnabled()
        {
            return GetElderHuHelperModule()?.Enabled ?? false;
        }

        private void SetElderHuHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetElderHuHelperModule(), value);
            UpdateElderHuHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetElderHuUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.ElderHuHelper.elderHuP5Hp)
            {
                Modules.BossChallenge.ElderHuHelper.elderHuUseMaxHp = true;
                RefreshElderHuHelperUi();
                return;
            }

            Modules.BossChallenge.ElderHuHelper.elderHuUseMaxHp = value;
            Modules.BossChallenge.ElderHuHelper.ReapplyLiveSettings();
            RefreshElderHuHelperUi();
        }

        private void SetElderHuP5HpEnabled(bool value)
        {
            Modules.BossChallenge.ElderHuHelper.SetP5HpEnabled(value);
            RefreshElderHuHelperUi();
        }

        private void SetElderHuMaxHp(int value)
        {
            if (Modules.BossChallenge.ElderHuHelper.elderHuP5Hp)
            {
                Modules.BossChallenge.ElderHuHelper.elderHuMaxHp = 600;
                RefreshElderHuHelperUi();
                return;
            }

            Modules.BossChallenge.ElderHuHelper.elderHuMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.ElderHuHelper.elderHuUseMaxHp)
            {
                Modules.BossChallenge.ElderHuHelper.ApplyElderHuHealthIfPresent();
            }
        }

        private void RefreshElderHuHelperUi()
        {
            Module? module = GetElderHuHelperModule();
            UpdateToggleValue(elderHuHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(elderHuHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(elderHuP5HpValue, Modules.BossChallenge.ElderHuHelper.elderHuP5Hp);
            UpdateToggleValue(elderHuUseMaxHpValue, Modules.BossChallenge.ElderHuHelper.elderHuUseMaxHp);
            UpdateIntInputValue(elderHuMaxHpField, Modules.BossChallenge.ElderHuHelper.elderHuMaxHp);

            UpdateElderHuHelperInteractivity();
        }

        private void UpdateElderHuHelperInteractivity()
        {
            SetContentInteractivity(elderHuHelperContent, GetElderHuHelperEnabled(), "ElderHuHelperEnableRow");
            if (!GetElderHuHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.ElderHuHelper.elderHuUseMaxHp;
            bool p5Hp = Modules.BossChallenge.ElderHuHelper.elderHuP5Hp;
            SetRowInteractivity(elderHuHelperContent, "ElderHuUseMaxHpRow", !p5Hp);
            SetRowInteractivity(elderHuHelperContent, "ElderHuMaxHpRow", useMaxHp && !p5Hp);
        }

        private void SetElderHuHelperVisible(bool value)
        {
            elderHuHelperVisible = value;
            ApplyPanelVisible(elderHuHelperRoot, value, RefreshElderHuHelperUi);
        }

        private void OnBossManipulateElderHuClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetElderHuHelperVisible);
        }

        private void OnElderHuHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetElderHuHelperVisible);
        }

        private void OnElderHuHelperResetDefaultsClicked()
        {
            ResetElderHuHelperDefaults();
            SetElderHuHelperEnabled(false);
            Modules.BossChallenge.ElderHuHelper.RestoreVanillaHealthIfPresent();
            RefreshElderHuHelperUi();
        }

        private void BuildElderHuHelperOverlayUi()
        {
            BuildStandardGhostHelperOverlayUi(
                ref elderHuHelperRoot,
                ref elderHuHelperContent,
                "ElderHuHelper",
                ElderHuHelperCanvasSortOrder,
                ElderHuHelperPanelHeight,
                ElderHuHelperRowSpacing,
                GetElderHuHelperEnabled,
                SetElderHuHelperEnabled,
                () => Modules.BossChallenge.ElderHuHelper.elderHuP5Hp,
                SetElderHuP5HpEnabled,
                () => Modules.BossChallenge.ElderHuHelper.elderHuUseMaxHp,
                SetElderHuUseMaxHpEnabled,
                () => Modules.BossChallenge.ElderHuHelper.elderHuMaxHp,
                SetElderHuMaxHp,
                OnElderHuHelperResetDefaultsClicked,
                OnElderHuHelperBackClicked,
                out elderHuHelperToggleValue,
                out elderHuHelperToggleIcon,
                out elderHuP5HpValue,
                out elderHuUseMaxHpValue,
                out elderHuMaxHpField);
        }
    }
}
