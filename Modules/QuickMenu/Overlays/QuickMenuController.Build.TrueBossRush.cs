using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildTrueBossRushOverlayUi()
        {
            trueBossRushRoot = CreateOverlayFrame("TrueBossRushOverlayCanvas", TrueBossRushCanvasSortOrder, "TrueBossRushPanel", PanelHeight, "Modules/TrueBossRush".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = PanelHeight;
            RectTransform content = CreateOverlayContent(panel, PanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            trueBossRushContent = content;

            float rowY = GetRowStartY(panelHeight, GearSwitcherRowStartY, topOffset) + GearSwitcherTopPadding;
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "TrueBossRushToggleRow",
                "Settings/TrueBossRush/Enable".Localize(),
                rowY,
                GetTrueBossRushMasterEnabled,
                SetTrueBossRushMasterEnabled,
                out trueBossRushToggleValue,
                out trueBossRushToggleIcon
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "TrueBossRushP1Row",
                "Pantheon 1",
                rowY,
                GetTrueBossRushP1Enabled,
                SetTrueBossRushP1Enabled,
                out trueBossRushP1Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "TrueBossRushP2Row",
                "Pantheon 2",
                rowY,
                GetTrueBossRushP2Enabled,
                SetTrueBossRushP2Enabled,
                out trueBossRushP2Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "TrueBossRushP3Row",
                "Pantheon 3",
                rowY,
                GetTrueBossRushP3Enabled,
                SetTrueBossRushP3Enabled,
                out trueBossRushP3Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "TrueBossRushP4Row",
                "Pantheon 4",
                rowY,
                GetTrueBossRushP4Enabled,
                SetTrueBossRushP4Enabled,
                out trueBossRushP4Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "TrueBossRushP5Row",
                "Pantheon 5",
                rowY,
                GetTrueBossRushP5Enabled,
                SetTrueBossRushP5Enabled,
                out trueBossRushP5Value
            );

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "TrueBossRushResetRow", "Settings/TrueBossRush/Reset".Localize(), resetY, OnTrueBossRushResetDefaultsClicked);
            CreateButtonRow(panel.transform, "TrueBossRushBackRow", "Back", backY, OnTrueBossRushBackClicked);
        }
    }
}
