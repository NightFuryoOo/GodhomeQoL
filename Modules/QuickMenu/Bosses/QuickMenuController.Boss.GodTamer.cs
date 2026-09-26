using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int GodTamerHelperCanvasSortOrder = 10059;
        private const float GodTamerHelperPanelHeight = PanelHeight;
        private const float GodTamerHelperRowSpacing = 44f;
        private RectTransform? godTamerHelperContent;
        private GameObject? godTamerHelperRoot;
        private bool godTamerHelperVisible;
        private Text? godTamerHelperToggleValue;
        private Image? godTamerHelperToggleIcon;
        private Text? godTamerP5HpValue;
        private InputField? godTamerLobsterHpField;
        private InputField? godTamerLancerHpField;
        private Module? godTamerHelperModule;

        private static void ResetGodTamerHelperDefaults()
        {
            Modules.BossChallenge.GodTamerHelper.godTamerP5Hp = false;
            Modules.BossChallenge.GodTamerHelper.godTamerLobsterHp = 1000;
            Modules.BossChallenge.GodTamerHelper.godTamerLancerHp = 1000;
            Modules.BossChallenge.GodTamerHelper.godTamerLobsterHpBeforeP5 = 1000;
            Modules.BossChallenge.GodTamerHelper.godTamerLancerHpBeforeP5 = 1000;
            Modules.BossChallenge.GodTamerHelper.godTamerHasStoredStateBeforeP5 = false;
        }

        private Module? GetGodTamerHelperModule()
        {
            return GetCachedModule(ref godTamerHelperModule, typeof(Modules.BossChallenge.GodTamerHelper));
        }

        private bool GetGodTamerHelperEnabled()
        {
            return GetGodTamerHelperModule()?.Enabled ?? false;
        }

        private void SetGodTamerHelperEnabled(bool value)
        {
            SetModuleEnabledFlag(GetGodTamerHelperModule(), value);
            UpdateGodTamerHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetGodTamerP5HpEnabled(bool value)
        {
            Modules.BossChallenge.GodTamerHelper.SetP5HpEnabled(value);
            RefreshGodTamerHelperUi();
        }

        private void SetGodTamerLobsterHp(int value)
        {
            if (Modules.BossChallenge.GodTamerHelper.godTamerP5Hp)
            {
                Modules.BossChallenge.GodTamerHelper.godTamerLobsterHp = 750;
                RefreshGodTamerHelperUi();
                return;
            }

            Modules.BossChallenge.GodTamerHelper.godTamerLobsterHp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.GodTamerHelper.ReapplyLiveSettings();
        }

        private void SetGodTamerLancerHp(int value)
        {
            if (Modules.BossChallenge.GodTamerHelper.godTamerP5Hp)
            {
                Modules.BossChallenge.GodTamerHelper.godTamerLancerHp = 750;
                RefreshGodTamerHelperUi();
                return;
            }

            Modules.BossChallenge.GodTamerHelper.godTamerLancerHp = Mathf.Clamp(value, 1, 999999);
            Modules.BossChallenge.GodTamerHelper.ReapplyLiveSettings();
        }

        private void RefreshGodTamerHelperUi()
        {
            Module? module = GetGodTamerHelperModule();
            UpdateToggleValue(godTamerHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(godTamerHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(godTamerP5HpValue, Modules.BossChallenge.GodTamerHelper.godTamerP5Hp);
            UpdateIntInputValue(godTamerLobsterHpField, Modules.BossChallenge.GodTamerHelper.godTamerLobsterHp);
            UpdateIntInputValue(godTamerLancerHpField, Modules.BossChallenge.GodTamerHelper.godTamerLancerHp);

            UpdateGodTamerHelperInteractivity();
        }

        private void UpdateGodTamerHelperInteractivity()
        {
            SetContentInteractivity(godTamerHelperContent, GetGodTamerHelperEnabled(), "GodTamerHelperEnableRow");
            if (!GetGodTamerHelperEnabled())
            {
                return;
            }

            bool p5Hp = Modules.BossChallenge.GodTamerHelper.godTamerP5Hp;
            SetRowInteractivity(godTamerHelperContent, "GodTamerLobsterHpRow", !p5Hp);
            SetRowInteractivity(godTamerHelperContent, "GodTamerLancerHpRow", !p5Hp);
        }

        private void SetGodTamerHelperVisible(bool value)
        {
            godTamerHelperVisible = value;
            ApplyPanelVisible(godTamerHelperRoot, value, RefreshGodTamerHelperUi);
        }

        private void OnBossManipulateGodTamerClicked()
        {
            OpenAdditionalGhostHelperOverlay(SetGodTamerHelperVisible);
        }

        private void OnGodTamerHelperBackClicked()
        {
            CloseAdditionalGhostHelperOverlay(SetGodTamerHelperVisible);
        }

        private void OnGodTamerHelperResetDefaultsClicked()
        {
            ResetGodTamerHelperDefaults();
            SetGodTamerHelperEnabled(false);
            Modules.BossChallenge.GodTamerHelper.RestoreVanillaHealthIfPresent();
            RefreshGodTamerHelperUi();
        }

        private void BuildGodTamerHelperOverlayUi()
        {
            godTamerHelperRoot = CreateOverlayFrame("GodTamerHelperOverlayCanvas", GodTamerHelperCanvasSortOrder, "GodTamerHelperPanel", GodTamerHelperPanelHeight, "Modules/GodTamerHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            RectTransform content = CreateOverlayContent(panel, GodTamerHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            godTamerHelperContent = content;

            float rowY = GetRowStartY(GodTamerHelperPanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "GodTamerHelperEnableRow",
                "Settings/GodTamerHelper/Enable".Localize(),
                rowY,
                GetGodTamerHelperEnabled,
                SetGodTamerHelperEnabled,
                out godTamerHelperToggleValue,
                out godTamerHelperToggleIcon
            );

            lastY = rowY;
            rowY += GodTamerHelperRowSpacing;
            CreateToggleRow(
                content,
                "GodTamerP5HpRow",
                "Settings/GodTamerHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.GodTamerHelper.godTamerP5Hp,
                SetGodTamerP5HpEnabled,
                out godTamerP5HpValue
            );

            lastY = rowY;
            rowY += GodTamerHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GodTamerLobsterHpRow",
                "Settings/GodTamerHelper/LobsterHP".Localize(),
                rowY,
                () => Modules.BossChallenge.GodTamerHelper.godTamerLobsterHp,
                SetGodTamerLobsterHp,
                1,
                999999,
                10,
                out godTamerLobsterHpField
            );

            lastY = rowY;
            rowY += GodTamerHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GodTamerLancerHpRow",
                "Settings/GodTamerHelper/LancerHP".Localize(),
                rowY,
                () => Modules.BossChallenge.GodTamerHelper.godTamerLancerHp,
                SetGodTamerLancerHp,
                1,
                999999,
                10,
                out godTamerLancerHpField
            );

            lastY = rowY;
            rowY += GodTamerHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "GodTamerHelperResetRow", "Settings/GodTamerHelper/Reset".Localize(), resetY, OnGodTamerHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "GodTamerHelperBackRow", "Back", backY, OnGodTamerHelperBackClicked);
        }
    }
}
