using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int VengeflyKingCanvasSortOrder = 10054;
        private const float VengeflyKingPanelHeight = PanelHeight;
        private const float VengeflyKingRowSpacing = 44f;
        private RectTransform? vengeflyKingContent;
        private GameObject? vengeflyKingRoot;
        private bool vengeflyKingVisible;
        private Text? vengeflyKingToggleValue;
        private Image? vengeflyKingToggleIcon;
        private Text? vengeflyKingP5HpValue;
        private Text? vengeflyKingUseMaxHpValue;
        private Text? vengeflyKingUseCustomSummonLimitValue;
        private InputField? vengeflyKingLeftMaxHpField;
        private InputField? vengeflyKingRightMaxHpField;
        private InputField? vengeflyKingSummonMaxHpField;
        private InputField? vengeflyKingLeftSummonLimitField;
        private InputField? vengeflyKingRightSummonLimitField;
        private InputField? vengeflyKingLeftSummonAttackLimitField;
        private InputField? vengeflyKingRightSummonAttackLimitField;
        private Module? vengeflyKingModule;

        private static void ResetVengeflyKingDefaults()
        {
            Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp = false;
            Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp = false;
            Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit = false;
            Modules.BossChallenge.VengeflyKing.vengeflyKingLeftMaxHp = 750;
            Modules.BossChallenge.VengeflyKing.vengeflyKingRightMaxHp = 430;
            Modules.BossChallenge.VengeflyKing.vengeflyKingSummonMaxHp = 8;
            Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonLimit = 4;
            Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonLimit = 4;
            Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonAttackLimit = 15;
            Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonAttackLimit = 15;
            Modules.BossChallenge.VengeflyKing.vengeflyKingLeftMaxHpBeforeP5 = 750;
            Modules.BossChallenge.VengeflyKing.vengeflyKingRightMaxHpBeforeP5 = 430;
            Modules.BossChallenge.VengeflyKing.vengeflyKingSummonMaxHpBeforeP5 = 8;
            Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimitBeforeP5 = false;
            Modules.BossChallenge.VengeflyKing.vengeflyKingHasStoredStateBeforeP5 = false;
        }

        private Module? GetVengeflyKingModule()
        {
            return GetCachedModule(ref vengeflyKingModule, typeof(Modules.BossChallenge.VengeflyKing));
        }

        private bool GetVengeflyKingEnabled()
        {
            return GetVengeflyKingModule()?.Enabled ?? false;
        }

        private void SetVengeflyKingEnabled(bool value)
        {
            SetModuleEnabledFlag(GetVengeflyKingModule(), value);
            UpdateVengeflyKingInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetVengeflyKingUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp = true;
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp = value;
            Modules.BossChallenge.VengeflyKing.ReapplyLiveSettings();
            RefreshVengeflyKingUi();
        }

        private void SetVengeflyKingP5HpEnabled(bool value)
        {
            Modules.BossChallenge.VengeflyKing.SetP5HpEnabled(value);
            RefreshVengeflyKingUi();
        }

        private void SetVengeflyKingUseCustomSummonLimitEnabled(bool value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit = false;
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit = value;
            Modules.BossChallenge.VengeflyKing.ReapplyLiveSettings();
            RefreshVengeflyKingUi();
        }

        private void SetVengeflyKingLeftMaxHp(int value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                Modules.BossChallenge.VengeflyKing.vengeflyKingLeftMaxHp = 450;
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingLeftMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp)
            {
                Modules.BossChallenge.VengeflyKing.ApplyVengeflyHealthIfPresent();
            }
        }

        private void SetVengeflyKingRightMaxHp(int value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                Modules.BossChallenge.VengeflyKing.vengeflyKingRightMaxHp = 190;
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingRightMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp)
            {
                Modules.BossChallenge.VengeflyKing.ApplyVengeflyHealthIfPresent();
            }
        }

        private void SetVengeflyKingSummonMaxHp(int value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                Modules.BossChallenge.VengeflyKing.vengeflyKingSummonMaxHp = 8;
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingSummonMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp)
            {
                Modules.BossChallenge.VengeflyKing.ApplyVengeflyHealthIfPresent();
            }
        }

        private void SetVengeflyKingLeftSummonLimit(int value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonLimit = 4;
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonLimit = Mathf.Clamp(value, 0, 999);
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit)
            {
                Modules.BossChallenge.VengeflyKing.ApplySummonLimitSettingsIfPresent();
            }
        }

        private void SetVengeflyKingRightSummonLimit(int value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonLimit = 4;
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonLimit = Mathf.Clamp(value, 0, 999);
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit)
            {
                Modules.BossChallenge.VengeflyKing.ApplySummonLimitSettingsIfPresent();
            }
        }

        private void SetVengeflyKingLeftSummonAttackLimit(int value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonAttackLimit = Mathf.Clamp(value, 0, 999);
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit)
            {
                Modules.BossChallenge.VengeflyKing.ApplySummonLimitSettingsIfPresent();
            }
        }

        private void SetVengeflyKingRightSummonAttackLimit(int value)
        {
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp)
            {
                RefreshVengeflyKingUi();
                return;
            }

            Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonAttackLimit = Mathf.Clamp(value, 0, 999);
            if (Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit)
            {
                Modules.BossChallenge.VengeflyKing.ApplySummonLimitSettingsIfPresent();
            }
        }

        private void RefreshVengeflyKingUi()
        {
            Module? module = GetVengeflyKingModule();
            UpdateToggleValue(vengeflyKingToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(vengeflyKingToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(vengeflyKingP5HpValue, Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp);
            UpdateToggleValue(vengeflyKingUseMaxHpValue, Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp);
            UpdateToggleValue(vengeflyKingUseCustomSummonLimitValue, Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit);
            UpdateIntInputValue(vengeflyKingLeftMaxHpField, Modules.BossChallenge.VengeflyKing.vengeflyKingLeftMaxHp);
            UpdateIntInputValue(vengeflyKingRightMaxHpField, Modules.BossChallenge.VengeflyKing.vengeflyKingRightMaxHp);
            UpdateIntInputValue(vengeflyKingSummonMaxHpField, Modules.BossChallenge.VengeflyKing.vengeflyKingSummonMaxHp);
            UpdateIntInputValue(vengeflyKingLeftSummonLimitField, Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonLimit);
            UpdateIntInputValue(vengeflyKingRightSummonLimitField, Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonLimit);
            UpdateIntInputValue(vengeflyKingLeftSummonAttackLimitField, Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonAttackLimit);
            UpdateIntInputValue(vengeflyKingRightSummonAttackLimitField, Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonAttackLimit);

            UpdateVengeflyKingInteractivity();
        }

        private void UpdateVengeflyKingInteractivity()
        {
            SetContentInteractivity(vengeflyKingContent, GetVengeflyKingEnabled(), "VengeflyKingEnableRow");
            if (!GetVengeflyKingEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp;
            bool p5Hp = Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp;
            bool useCustomSummonLimit = Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit;
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingUseMaxHpRow", !p5Hp);
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingLeftMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingRightMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingSummonMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingUseCustomSummonLimitRow", !p5Hp);
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingLeftSummonLimitRow", useCustomSummonLimit && !p5Hp);
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingRightSummonLimitRow", useCustomSummonLimit && !p5Hp);
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingLeftSummonAttackLimitRow", useCustomSummonLimit && !p5Hp);
            SetRowInteractivity(vengeflyKingContent, "VengeflyKingRightSummonAttackLimitRow", useCustomSummonLimit && !p5Hp);
        }

        private void SetVengeflyKingVisible(bool value)
        {
            vengeflyKingVisible = value;
            ApplyPanelVisible(vengeflyKingRoot, value, RefreshVengeflyKingUi);
        }

        private void OnBossManipulateVengeflyKingClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetVengeflyKingVisible);
        }

        private void OnVengeflyKingBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetVengeflyKingVisible);
        }

        private void OnVengeflyKingResetDefaultsClicked()
        {
            ResetVengeflyKingDefaults();
            SetVengeflyKingEnabled(false);
            Modules.BossChallenge.VengeflyKing.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.VengeflyKing.RestoreVanillaSummonLimitsIfPresent();
            RefreshVengeflyKingUi();
        }

        private void BuildVengeflyKingOverlayUi()
        {
            vengeflyKingRoot = CreateOverlayFrame("VengeflyKingOverlayCanvas", VengeflyKingCanvasSortOrder, "VengeflyKingPanel", VengeflyKingPanelHeight, "Modules/VengeflyKing".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            RectTransform content = CreateOverlayContent(panel, VengeflyKingPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            vengeflyKingContent = content;

            float rowY = GetRowStartY(VengeflyKingPanelHeight, RowStartY, topOffset);
            float lastY = rowY;

            CreateToggleRowWithIcon(
                content,
                "VengeflyKingEnableRow",
                "Settings/VengeflyKing/Enable".Localize(),
                rowY,
                GetVengeflyKingEnabled,
                SetVengeflyKingEnabled,
                out vengeflyKingToggleValue,
                out vengeflyKingToggleIcon
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateToggleRow(
                content,
                "VengeflyKingP5HpRow",
                "Settings/VengeflyKing/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingP5Hp,
                SetVengeflyKingP5HpEnabled,
                out vengeflyKingP5HpValue
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateToggleRow(
                content,
                "VengeflyKingUseMaxHpRow",
                "Settings/VengeflyKing/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingUseMaxHp,
                SetVengeflyKingUseMaxHpEnabled,
                out vengeflyKingUseMaxHpValue
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateAdjustInputRow(
                content,
                "VengeflyKingLeftMaxHpRow",
                "Settings/VengeflyKing/LeftMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingLeftMaxHp,
                SetVengeflyKingLeftMaxHp,
                1,
                999999,
                10,
                out vengeflyKingLeftMaxHpField
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateAdjustInputRow(
                content,
                "VengeflyKingRightMaxHpRow",
                "Settings/VengeflyKing/RightMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingRightMaxHp,
                SetVengeflyKingRightMaxHp,
                1,
                999999,
                10,
                out vengeflyKingRightMaxHpField
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateAdjustInputRow(
                content,
                "VengeflyKingSummonMaxHpRow",
                "Settings/VengeflyKing/VengeflyHP".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingSummonMaxHp,
                SetVengeflyKingSummonMaxHp,
                1,
                999999,
                1,
                out vengeflyKingSummonMaxHpField
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateToggleRow(
                content,
                "VengeflyKingUseCustomSummonLimitRow",
                "Settings/VengeflyKing/UseCustomSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingUseCustomSummonLimit,
                SetVengeflyKingUseCustomSummonLimitEnabled,
                out vengeflyKingUseCustomSummonLimitValue
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateAdjustInputRow(
                content,
                "VengeflyKingLeftSummonLimitRow",
                "Settings/VengeflyKing/LeftSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonLimit,
                SetVengeflyKingLeftSummonLimit,
                0,
                999,
                1,
                out vengeflyKingLeftSummonLimitField
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateAdjustInputRow(
                content,
                "VengeflyKingRightSummonLimitRow",
                "Settings/VengeflyKing/RightSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonLimit,
                SetVengeflyKingRightSummonLimit,
                0,
                999,
                1,
                out vengeflyKingRightSummonLimitField
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateAdjustInputRow(
                content,
                "VengeflyKingLeftSummonAttackLimitRow",
                "Settings/VengeflyKing/LeftSummonAttackLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingLeftSummonAttackLimit,
                SetVengeflyKingLeftSummonAttackLimit,
                0,
                999,
                1,
                out vengeflyKingLeftSummonAttackLimitField
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;
            CreateAdjustInputRow(
                content,
                "VengeflyKingRightSummonAttackLimitRow",
                "Settings/VengeflyKing/RightSummonAttackLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.VengeflyKing.vengeflyKingRightSummonAttackLimit,
                SetVengeflyKingRightSummonAttackLimit,
                0,
                999,
                1,
                out vengeflyKingRightSummonAttackLimitField
            );

            lastY = rowY;
            rowY += VengeflyKingRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "VengeflyKingResetRow", "Settings/VengeflyKing/Reset".Localize(), resetY, OnVengeflyKingResetDefaultsClicked);
            CreateButtonRow(panel.transform, "VengeflyKingBackRow", "Back", backY, OnVengeflyKingBackClicked);
        }
    }
}
