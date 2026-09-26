using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int MarmuHelperCanvasSortOrder = 10031;
        private const float MarmuHelperPanelHeight = PanelHeight;
        private const float MarmuHelperRowSpacing = 44f;
        private RectTransform? marmuHelperContent;
        private GameObject? marmuHelperRoot;
        private bool marmuHelperVisible;
        private Text? marmuHelperToggleValue;
        private Image? marmuHelperToggleIcon;
        private Text? marmuP5HpValue;
        private Text? marmuUseMaxHpValue;
        private InputField? marmuMaxHpField;
        private Module? marmuHelperModule;

        private static void ResetMarmuHelperDefaults()
        {
            Modules.BossChallenge.MarmuHelper.marmuUseMaxHp = false;
            Modules.BossChallenge.MarmuHelper.marmuP5Hp = false;
            Modules.BossChallenge.MarmuHelper.marmuMaxHp = 600;
            Modules.BossChallenge.MarmuHelper.marmuMaxHpBeforeP5 = 600;
            Modules.BossChallenge.MarmuHelper.marmuUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.MarmuHelper.marmuHasStoredStateBeforeP5 = false;
        }

        private Module? GetMarmuHelperModule()
        {
            return GetCachedModule(ref marmuHelperModule, typeof(Modules.BossChallenge.MarmuHelper));
        }

        private bool GetMarmuHelperEnabled()
        {
            return GetMarmuHelperModule()?.Enabled ?? false;
        }

        private void SetMarmuHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetMarmuHelperModule(), value);
            UpdateMarmuHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetMarmuUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.MarmuHelper.marmuP5Hp)
            {
                Modules.BossChallenge.MarmuHelper.marmuUseMaxHp = true;
                RefreshMarmuHelperUi();
                return;
            }

            Modules.BossChallenge.MarmuHelper.marmuUseMaxHp = value;
            Modules.BossChallenge.MarmuHelper.ReapplyLiveSettings();
            RefreshMarmuHelperUi();
        }

        private void SetMarmuP5HpEnabled(bool value)
        {
            Modules.BossChallenge.MarmuHelper.SetP5HpEnabled(value);
            RefreshMarmuHelperUi();
        }

        private void SetMarmuMaxHp(int value)
        {
            if (Modules.BossChallenge.MarmuHelper.marmuP5Hp)
            {
                Modules.BossChallenge.MarmuHelper.marmuMaxHp = 416;
                RefreshMarmuHelperUi();
                return;
            }

            Modules.BossChallenge.MarmuHelper.marmuMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.MarmuHelper.marmuUseMaxHp)
            {
                Modules.BossChallenge.MarmuHelper.ApplyMarmuHealthIfPresent();
            }
        }

        private void RefreshMarmuHelperUi()
        {
            Module? module = GetMarmuHelperModule();
            UpdateToggleValue(marmuHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(marmuHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(marmuP5HpValue, Modules.BossChallenge.MarmuHelper.marmuP5Hp);
            UpdateToggleValue(marmuUseMaxHpValue, Modules.BossChallenge.MarmuHelper.marmuUseMaxHp);
            UpdateIntInputValue(marmuMaxHpField, Modules.BossChallenge.MarmuHelper.marmuMaxHp);

            UpdateMarmuHelperInteractivity();
        }

        private void UpdateMarmuHelperInteractivity()
        {
            SetContentInteractivity(marmuHelperContent, GetMarmuHelperEnabled(), "MarmuHelperEnableRow");
            if (!GetMarmuHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.MarmuHelper.marmuUseMaxHp;
            bool p5Hp = Modules.BossChallenge.MarmuHelper.marmuP5Hp;
            SetRowInteractivity(marmuHelperContent, "MarmuUseMaxHpRow", !p5Hp);
            SetRowInteractivity(marmuHelperContent, "MarmuMaxHpRow", useMaxHp && !p5Hp);
        }

        private void SetMarmuHelperVisible(bool value)
        {
            marmuHelperVisible = value;
            ApplyPanelVisible(marmuHelperRoot, value, RefreshMarmuHelperUi);
        }

        private void OnBossManipulateMarmuClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetMarmuHelperVisible);
        }

        private void OnMarmuHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetMarmuHelperVisible);
        }

        private void OnMarmuHelperResetDefaultsClicked()
        {
            ResetMarmuHelperDefaults();
            SetMarmuHelperEnabled(false);
            Modules.BossChallenge.MarmuHelper.RestoreVanillaHealthIfPresent();
            RefreshMarmuHelperUi();
        }

        private void BuildMarmuHelperOverlayUi()
        {
            BuildStandardGhostHelperOverlayUi(
                ref marmuHelperRoot,
                ref marmuHelperContent,
                "MarmuHelper",
                MarmuHelperCanvasSortOrder,
                MarmuHelperPanelHeight,
                MarmuHelperRowSpacing,
                GetMarmuHelperEnabled,
                SetMarmuHelperEnabled,
                () => Modules.BossChallenge.MarmuHelper.marmuP5Hp,
                SetMarmuP5HpEnabled,
                () => Modules.BossChallenge.MarmuHelper.marmuUseMaxHp,
                SetMarmuUseMaxHpEnabled,
                () => Modules.BossChallenge.MarmuHelper.marmuMaxHp,
                SetMarmuMaxHp,
                OnMarmuHelperResetDefaultsClicked,
                OnMarmuHelperBackClicked,
                out marmuHelperToggleValue,
                out marmuHelperToggleIcon,
                out marmuP5HpValue,
                out marmuUseMaxHpValue,
                out marmuMaxHpField);
        }
    }
}
