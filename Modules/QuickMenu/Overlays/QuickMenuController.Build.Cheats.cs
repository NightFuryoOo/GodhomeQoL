using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildCheatsOverlayUi()
        {
            cheatsRoot = CreateOverlayFrame("CheatsOverlayCanvas", CheatsCanvasSortOrder, "CheatsPanel", PanelHeight, "Modules/Cheats".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = PanelHeight;
            RectTransform content = CreateOverlayContent(panel, PanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            cheatsContent = content;

            float rowY = GetRowStartY(panelHeight, GearSwitcherRowStartY, topOffset) + GearSwitcherTopPadding;
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "CheatsEnableRow",
                "Settings/Cheats/Enable".Localize(),
                rowY,
                GetCheatsMasterEnabled,
                SetCheatsMasterEnabled,
                out cheatsEnableValue,
                out cheatsEnableIcon
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "CheatsInfiniteSoulRow",
                "Settings/Cheats/InfiniteSoul".Localize(),
                rowY,
                GetCheatsInfiniteSoulEnabled,
                SetCheatsInfiniteSoulEnabled,
                out cheatsInfiniteSoulValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "CheatsInfiniteHpRow",
                "Settings/Cheats/InfiniteHp".Localize(),
                rowY,
                GetCheatsInfiniteHpEnabled,
                SetCheatsInfiniteHpEnabled,
                out cheatsInfiniteHpValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "CheatsInvincibilityRow",
                "Settings/Cheats/Invincibility".Localize(),
                rowY,
                GetCheatsInvincibilityEnabled,
                SetCheatsInvincibilityEnabled,
                out cheatsInvincibilityValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "CheatsNoclipRow",
                "Settings/Cheats/Noclip".Localize(),
                rowY,
                GetCheatsNoclipEnabled,
                SetCheatsNoclipEnabled,
                out cheatsNoclipValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateButtonRow(
                content,
                "CheatsKillAllRow",
                "Settings/Cheats/KillAll".Localize(),
                rowY,
                OnCheatsKillAllClicked
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateKeybindRow(
                content,
                "CheatsKillAllHotkeyRow",
                "Settings/Cheats/KillAllHotkey".Localize(),
                rowY,
                GetCheatsKillAllHotkeyLabel,
                StartCheatsKillAllRebind,
                out cheatsKillAllHotkeyValue
            );

            lastY = rowY;
            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "CheatsResetRow", "Settings/Cheats/Reset".Localize(), resetY, OnCheatsResetDefaultsClicked);
            CreateButtonRow(panel.transform, "CheatsBackRow", "Back", backY, OnCheatsBackClicked);
        }
    }
}
