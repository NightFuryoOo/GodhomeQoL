using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int SoulWarriorHelperCanvasSortOrder = 10051;
        private const float SoulWarriorHelperPanelHeight = PanelHeight;
        private const float SoulWarriorHelperRowSpacing = 44f;
        private RectTransform? soulWarriorHelperContent;
        private GameObject? soulWarriorHelperRoot;
        private bool soulWarriorHelperVisible;
        private Text? soulWarriorHelperToggleValue;
        private Image? soulWarriorHelperToggleIcon;
        private Text? soulWarriorP5HpValue;
        private Text? soulWarriorUseMaxHpValue;
        private Text? soulWarriorUseCustomSummonHpValue;
        private Text? soulWarriorUseCustomSummonLimitValue;
        private InputField? soulWarriorMaxHpField;
        private InputField? soulWarriorSummonHpField;
        private InputField? soulWarriorSummonLimitField;
        private Module? soulWarriorHelperModule;

        private static void ResetSoulWarriorHelperDefaults()
        {
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseMaxHp = false;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp = false;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonHp = false;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonLimit = false;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorMaxHp = 1000;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonHp = 13;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonLimit = 36;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorMaxHpBeforeP5 = 1000;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonHpBeforeP5 = false;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonHpBeforeP5 = 13;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonLimitBeforeP5 = false;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonLimitBeforeP5 = 36;
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorHasStoredStateBeforeP5 = false;
        }

        private Module? GetSoulWarriorHelperModule()
        {
            return GetCachedModule(ref soulWarriorHelperModule, typeof(Modules.BossChallenge.SoulWarriorHelper));
        }

        private bool GetSoulWarriorHelperEnabled()
        {
            return GetSoulWarriorHelperModule()?.Enabled ?? false;
        }

        private void SetSoulWarriorHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetSoulWarriorHelperModule(), value);
            UpdateSoulWarriorHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetSoulWarriorUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp)
            {
                Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseMaxHp = true;
                RefreshSoulWarriorHelperUi();
                return;
            }

            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseMaxHp = value;
            Modules.BossChallenge.SoulWarriorHelper.ReapplyLiveSettings();
            RefreshSoulWarriorHelperUi();
        }

        private void SetSoulWarriorP5HpEnabled(bool value)
        {
            Modules.BossChallenge.SoulWarriorHelper.SetP5HpEnabled(value);
            RefreshSoulWarriorHelperUi();
        }

        private void SetSoulWarriorMaxHp(int value)
        {
            if (Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp)
            {
                Modules.BossChallenge.SoulWarriorHelper.soulWarriorMaxHp = 750;
                RefreshSoulWarriorHelperUi();
                return;
            }

            Modules.BossChallenge.SoulWarriorHelper.soulWarriorMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseMaxHp)
            {
                Modules.BossChallenge.SoulWarriorHelper.ApplySoulWarriorHealthIfPresent();
            }
        }

        private void SetSoulWarriorUseCustomSummonHpEnabled(bool value)
        {
            if (Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp)
            {
                Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonHp = false;
                RefreshSoulWarriorHelperUi();
                return;
            }

            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonHp = value;
            Modules.BossChallenge.SoulWarriorHelper.ReapplyLiveSettings();
            RefreshSoulWarriorHelperUi();
        }

        private void SetSoulWarriorSummonHp(int value)
        {
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonHp
                && !Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp)
            {
                Modules.BossChallenge.SoulWarriorHelper.ApplySoulWarriorSummonHealthIfPresent();
            }
        }

        private void SetSoulWarriorUseCustomSummonLimitEnabled(bool value)
        {
            if (Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp)
            {
                Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonLimit = false;
                RefreshSoulWarriorHelperUi();
                return;
            }

            Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonLimit = value;
            Modules.BossChallenge.SoulWarriorHelper.ReapplyLiveSettings();
            RefreshSoulWarriorHelperUi();
        }

        private void SetSoulWarriorSummonLimit(int value)
        {
            Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonLimit = Mathf.Clamp(value, 36, 999);
            if (Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonLimit
                && !Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp)
            {
                Modules.BossChallenge.SoulWarriorHelper.ApplySummonLimitSettingsIfPresent();
            }
        }

        private void RefreshSoulWarriorHelperUi()
        {
            Module? module = GetSoulWarriorHelperModule();
            UpdateToggleValue(soulWarriorHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(soulWarriorHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(soulWarriorP5HpValue, Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp);
            UpdateToggleValue(soulWarriorUseMaxHpValue, Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseMaxHp);
            UpdateToggleValue(soulWarriorUseCustomSummonHpValue, Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonHp);
            UpdateToggleValue(soulWarriorUseCustomSummonLimitValue, Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonLimit);
            UpdateIntInputValue(soulWarriorMaxHpField, Modules.BossChallenge.SoulWarriorHelper.soulWarriorMaxHp);
            UpdateIntInputValue(soulWarriorSummonHpField, Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonHp);
            UpdateIntInputValue(soulWarriorSummonLimitField, Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonLimit);

            UpdateSoulWarriorHelperInteractivity();
        }

        private void UpdateSoulWarriorHelperInteractivity()
        {
            SetContentInteractivity(soulWarriorHelperContent, GetSoulWarriorHelperEnabled(), "SoulWarriorHelperEnableRow");
            if (!GetSoulWarriorHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseMaxHp;
            bool p5Hp = Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp;
            bool useCustomSummonHp = Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonHp;
            bool useCustomSummonLimit = Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonLimit;
            SetRowInteractivity(soulWarriorHelperContent, "SoulWarriorUseMaxHpRow", !p5Hp);
            SetRowInteractivity(soulWarriorHelperContent, "SoulWarriorMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(soulWarriorHelperContent, "SoulWarriorUseCustomSummonHpRow", !p5Hp);
            SetRowInteractivity(soulWarriorHelperContent, "SoulWarriorSummonHpRow", useCustomSummonHp && !p5Hp);
            SetRowInteractivity(soulWarriorHelperContent, "SoulWarriorUseCustomSummonLimitRow", !p5Hp);
            SetRowInteractivity(soulWarriorHelperContent, "SoulWarriorSummonLimitRow", useCustomSummonLimit && !p5Hp);
        }

        private void SetSoulWarriorHelperVisible(bool value)
        {
            soulWarriorHelperVisible = value;
            ApplyPanelVisible(soulWarriorHelperRoot, value, RefreshSoulWarriorHelperUi);
        }

        private void OnBossManipulateSoulWarriorClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetSoulWarriorHelperVisible);
        }

        private void OnSoulWarriorHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetSoulWarriorHelperVisible);
        }

        private void OnSoulWarriorHelperResetDefaultsClicked()
        {
            ResetSoulWarriorHelperDefaults();
            SetSoulWarriorHelperEnabled(false);
            Modules.BossChallenge.SoulWarriorHelper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.SoulWarriorHelper.RestoreVanillaSummonHealthIfPresent();
            Modules.BossChallenge.SoulWarriorHelper.RestoreVanillaSummonLimitsIfPresent();
            RefreshSoulWarriorHelperUi();
        }

        private void BuildSoulWarriorHelperOverlayUi()
        {
            soulWarriorHelperRoot = CreateOverlayFrame("SoulWarriorHelperOverlayCanvas", SoulWarriorHelperCanvasSortOrder, "SoulWarriorHelperPanel", SoulWarriorHelperPanelHeight, "Modules/SoulWarriorHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            RectTransform content = CreateOverlayContent(panel, SoulWarriorHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            soulWarriorHelperContent = content;

            float rowY = GetRowStartY(SoulWarriorHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "SoulWarriorHelperEnableRow",
                "Settings/SoulWarriorHelper/Enable".Localize(),
                rowY,
                GetSoulWarriorHelperEnabled,
                SetSoulWarriorHelperEnabled,
                out soulWarriorHelperToggleValue,
                out soulWarriorHelperToggleIcon
            );

            lastY = rowY;
            rowY += SoulWarriorHelperRowSpacing;
            CreateToggleRow(
                content,
                "SoulWarriorP5HpRow",
                "Settings/SoulWarriorHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulWarriorHelper.soulWarriorP5Hp,
                SetSoulWarriorP5HpEnabled,
                out soulWarriorP5HpValue
            );

            lastY = rowY;
            rowY += SoulWarriorHelperRowSpacing;
            CreateToggleRow(
                content,
                "SoulWarriorUseMaxHpRow",
                "Settings/SoulWarriorHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseMaxHp,
                SetSoulWarriorUseMaxHpEnabled,
                out soulWarriorUseMaxHpValue
            );

            lastY = rowY;
            rowY += SoulWarriorHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SoulWarriorMaxHpRow",
                "Settings/SoulWarriorHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulWarriorHelper.soulWarriorMaxHp,
                SetSoulWarriorMaxHp,
                1,
                999999,
                10,
                out soulWarriorMaxHpField
            );

            lastY = rowY;
            rowY += SoulWarriorHelperRowSpacing;
            CreateToggleRow(
                content,
                "SoulWarriorUseCustomSummonHpRow",
                "Settings/SoulWarriorHelper/UseCustomSummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonHp,
                SetSoulWarriorUseCustomSummonHpEnabled,
                out soulWarriorUseCustomSummonHpValue
            );

            lastY = rowY;
            rowY += SoulWarriorHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SoulWarriorSummonHpRow",
                "Settings/SoulWarriorHelper/SummonHP".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonHp,
                SetSoulWarriorSummonHp,
                1,
                999999,
                10,
                out soulWarriorSummonHpField
            );

            lastY = rowY;
            rowY += SoulWarriorHelperRowSpacing;
            CreateToggleRow(
                content,
                "SoulWarriorUseCustomSummonLimitRow",
                "Settings/SoulWarriorHelper/UseCustomSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulWarriorHelper.soulWarriorUseCustomSummonLimit,
                SetSoulWarriorUseCustomSummonLimitEnabled,
                out soulWarriorUseCustomSummonLimitValue
            );

            lastY = rowY;
            rowY += SoulWarriorHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SoulWarriorSummonLimitRow",
                "Settings/SoulWarriorHelper/SummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.SoulWarriorHelper.soulWarriorSummonLimit,
                SetSoulWarriorSummonLimit,
                36,
                999,
                1,
                out soulWarriorSummonLimitField
            );

            lastY = rowY;
            rowY += SoulWarriorHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "SoulWarriorHelperResetRow", "Settings/SoulWarriorHelper/Reset".Localize(), resetY, OnSoulWarriorHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "SoulWarriorHelperBackRow", "Back", backY, OnSoulWarriorHelperBackClicked);
        }
    }
}
