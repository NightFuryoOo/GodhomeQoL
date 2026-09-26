using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int WatcherKnightHelperCanvasSortOrder = 10057;
        private const float WatcherKnightHelperPanelHeight = PanelHeight;
        private const float WatcherKnightHelperRowSpacing = 44f;
        private RectTransform? watcherKnightHelperContent;
        private GameObject? watcherKnightHelperRoot;
        private bool watcherKnightHelperVisible;
        private Text? watcherKnightHelperToggleValue;
        private Image? watcherKnightHelperToggleIcon;
        private Text? watcherKnightP5HpValue;
        private InputField? watcherKnight1HpField;
        private InputField? watcherKnight2HpField;
        private InputField? watcherKnight3HpField;
        private InputField? watcherKnight4HpField;
        private InputField? watcherKnight5HpField;
        private InputField? watcherKnight6HpField;
        private Module? watcherKnightHelperModule;

        private static void ResetWatcherKnightHelperDefaults()
        {
            Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp = false;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight1Hp = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight2Hp = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight3Hp = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight4Hp = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight5Hp = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight6Hp = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight1HpBeforeP5 = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight2HpBeforeP5 = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight3HpBeforeP5 = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight4HpBeforeP5 = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight5HpBeforeP5 = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnight6HpBeforeP5 = 600;
            Modules.BossChallenge.WatcherKnightHelper.watcherKnightHasStoredStateBeforeP5 = false;
        }

        private Module? GetWatcherKnightHelperModule()
        {
            return GetCachedModule(ref watcherKnightHelperModule, typeof(Modules.BossChallenge.WatcherKnightHelper));
        }

        private bool GetWatcherKnightHelperEnabled()
        {
            return GetWatcherKnightHelperModule()?.Enabled ?? false;
        }

        private void SetWatcherKnightHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetWatcherKnightHelperModule(), value);
            UpdateWatcherKnightHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetWatcherKnightP5HpEnabled(bool value)
        {
            Modules.BossChallenge.WatcherKnightHelper.SetP5HpEnabled(value);
            RefreshWatcherKnightHelperUi();
        }

        private void SetWatcherKnight1Hp(int value)
        {
            if (Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp)
            {
                Modules.BossChallenge.WatcherKnightHelper.watcherKnight1Hp = 350;
                RefreshWatcherKnightHelperUi();
                return;
            }

            Modules.BossChallenge.WatcherKnightHelper.watcherKnight1Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.WatcherKnightHelper.ReapplyLiveSettings();
        }

        private void SetWatcherKnight2Hp(int value)
        {
            if (Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp)
            {
                Modules.BossChallenge.WatcherKnightHelper.watcherKnight2Hp = 350;
                RefreshWatcherKnightHelperUi();
                return;
            }

            Modules.BossChallenge.WatcherKnightHelper.watcherKnight2Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.WatcherKnightHelper.ReapplyLiveSettings();
        }

        private void SetWatcherKnight3Hp(int value)
        {
            if (Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp)
            {
                Modules.BossChallenge.WatcherKnightHelper.watcherKnight3Hp = 350;
                RefreshWatcherKnightHelperUi();
                return;
            }

            Modules.BossChallenge.WatcherKnightHelper.watcherKnight3Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.WatcherKnightHelper.ReapplyLiveSettings();
        }

        private void SetWatcherKnight4Hp(int value)
        {
            if (Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp)
            {
                Modules.BossChallenge.WatcherKnightHelper.watcherKnight4Hp = 350;
                RefreshWatcherKnightHelperUi();
                return;
            }

            Modules.BossChallenge.WatcherKnightHelper.watcherKnight4Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.WatcherKnightHelper.ReapplyLiveSettings();
        }

        private void SetWatcherKnight5Hp(int value)
        {
            if (Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp)
            {
                Modules.BossChallenge.WatcherKnightHelper.watcherKnight5Hp = 350;
                RefreshWatcherKnightHelperUi();
                return;
            }

            Modules.BossChallenge.WatcherKnightHelper.watcherKnight5Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.WatcherKnightHelper.ReapplyLiveSettings();
        }

        private void SetWatcherKnight6Hp(int value)
        {
            if (Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp)
            {
                Modules.BossChallenge.WatcherKnightHelper.watcherKnight6Hp = 350;
                RefreshWatcherKnightHelperUi();
                return;
            }

            Modules.BossChallenge.WatcherKnightHelper.watcherKnight6Hp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.WatcherKnightHelper.ReapplyLiveSettings();
        }

        private void RefreshWatcherKnightHelperUi()
        {
            Module? module = GetWatcherKnightHelperModule();
            UpdateToggleValue(watcherKnightHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(watcherKnightHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(watcherKnightP5HpValue, Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp);
            UpdateIntInputValue(watcherKnight1HpField, Modules.BossChallenge.WatcherKnightHelper.watcherKnight1Hp);
            UpdateIntInputValue(watcherKnight2HpField, Modules.BossChallenge.WatcherKnightHelper.watcherKnight2Hp);
            UpdateIntInputValue(watcherKnight3HpField, Modules.BossChallenge.WatcherKnightHelper.watcherKnight3Hp);
            UpdateIntInputValue(watcherKnight4HpField, Modules.BossChallenge.WatcherKnightHelper.watcherKnight4Hp);
            UpdateIntInputValue(watcherKnight5HpField, Modules.BossChallenge.WatcherKnightHelper.watcherKnight5Hp);
            UpdateIntInputValue(watcherKnight6HpField, Modules.BossChallenge.WatcherKnightHelper.watcherKnight6Hp);

            UpdateWatcherKnightHelperInteractivity();
        }

        private void UpdateWatcherKnightHelperInteractivity()
        {
            SetContentInteractivity(watcherKnightHelperContent, GetWatcherKnightHelperEnabled(), "WatcherKnightHelperEnableRow");
            if (!GetWatcherKnightHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp;
            SetRowInteractivity(watcherKnightHelperContent, "WatcherKnight1HpRow", !p5Hp);
            SetRowInteractivity(watcherKnightHelperContent, "WatcherKnight2HpRow", !p5Hp);
            SetRowInteractivity(watcherKnightHelperContent, "WatcherKnight3HpRow", !p5Hp);
            SetRowInteractivity(watcherKnightHelperContent, "WatcherKnight4HpRow", !p5Hp);
            SetRowInteractivity(watcherKnightHelperContent, "WatcherKnight5HpRow", !p5Hp);
            SetRowInteractivity(watcherKnightHelperContent, "WatcherKnight6HpRow", !p5Hp);
        }

        private void SetWatcherKnightHelperVisible(bool value)
        {
            watcherKnightHelperVisible = value;
            ApplyPanelVisible(watcherKnightHelperRoot, value, RefreshWatcherKnightHelperUi);
        }

        private void OnBossManipulateWatcherKnightClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetWatcherKnightHelperVisible);
        }

        private void OnWatcherKnightHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetWatcherKnightHelperVisible);
        }

        private void OnWatcherKnightHelperResetDefaultsClicked()
        {
            ResetWatcherKnightHelperDefaults();
            SetWatcherKnightHelperEnabled(false);
            Modules.BossChallenge.WatcherKnightHelper.RestoreVanillaHealthIfPresent();
            RefreshWatcherKnightHelperUi();
        }

        private void BuildWatcherKnightHelperOverlayUi()
        {
            watcherKnightHelperRoot = CreateOverlayFrame("WatcherKnightHelperOverlayCanvas", WatcherKnightHelperCanvasSortOrder, "WatcherKnightHelperPanel", WatcherKnightHelperPanelHeight, "Modules/WatcherKnightHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            RectTransform content = CreateOverlayContent(panel, WatcherKnightHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            watcherKnightHelperContent = content;

            float rowY = GetRowStartY(WatcherKnightHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "WatcherKnightHelperEnableRow",
                "Settings/WatcherKnightHelper/Enable".Localize(),
                rowY,
                GetWatcherKnightHelperEnabled,
                SetWatcherKnightHelperEnabled,
                out watcherKnightHelperToggleValue,
                out watcherKnightHelperToggleIcon
            );

            lastY = rowY;
            rowY += WatcherKnightHelperRowSpacing;
            CreateToggleRow(
                content,
                "WatcherKnightP5HpRow",
                "Settings/WatcherKnightHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WatcherKnightHelper.watcherKnightP5Hp,
                SetWatcherKnightP5HpEnabled,
                out watcherKnightP5HpValue
            );

            lastY = rowY;
            rowY += WatcherKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WatcherKnight1HpRow",
                "Settings/WatcherKnightHelper/Knight1HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WatcherKnightHelper.watcherKnight1Hp,
                SetWatcherKnight1Hp,
                1,
                999999,
                10,
                out watcherKnight1HpField
            );

            lastY = rowY;
            rowY += WatcherKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WatcherKnight2HpRow",
                "Settings/WatcherKnightHelper/Knight2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WatcherKnightHelper.watcherKnight2Hp,
                SetWatcherKnight2Hp,
                1,
                999999,
                10,
                out watcherKnight2HpField
            );

            lastY = rowY;
            rowY += WatcherKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WatcherKnight3HpRow",
                "Settings/WatcherKnightHelper/Knight3HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WatcherKnightHelper.watcherKnight3Hp,
                SetWatcherKnight3Hp,
                1,
                999999,
                10,
                out watcherKnight3HpField
            );

            lastY = rowY;
            rowY += WatcherKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WatcherKnight4HpRow",
                "Settings/WatcherKnightHelper/Knight4HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WatcherKnightHelper.watcherKnight4Hp,
                SetWatcherKnight4Hp,
                1,
                999999,
                10,
                out watcherKnight4HpField
            );

            lastY = rowY;
            rowY += WatcherKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WatcherKnight5HpRow",
                "Settings/WatcherKnightHelper/Knight5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WatcherKnightHelper.watcherKnight5Hp,
                SetWatcherKnight5Hp,
                1,
                999999,
                10,
                out watcherKnight5HpField
            );

            lastY = rowY;
            rowY += WatcherKnightHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "WatcherKnight6HpRow",
                "Settings/WatcherKnightHelper/Knight6HP".Localize(),
                rowY,
                () => Modules.BossChallenge.WatcherKnightHelper.watcherKnight6Hp,
                SetWatcherKnight6Hp,
                1,
                999999,
                10,
                out watcherKnight6HpField
            );

            lastY = rowY;
            rowY += WatcherKnightHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "WatcherKnightHelperResetRow", "Settings/WatcherKnightHelper/Reset".Localize(), resetY, OnWatcherKnightHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "WatcherKnightHelperBackRow", "Back", backY, OnWatcherKnightHelperBackClicked);
        }
    }
}
