using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildRandomPantheonsOverlayUi()
        {
            randomPantheonsRoot = CreateOverlayFrame("RandomPantheonsOverlayCanvas", RandomPantheonsCanvasSortOrder, "RandomPantheonsPanel", PanelHeight, "Modules/RandomPantheons".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = PanelHeight;
            RectTransform content = CreateOverlayContent(panel, PanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            randomPantheonsContent = content;

            float rowY = GetRowStartY(panelHeight, GearSwitcherRowStartY, topOffset) + GearSwitcherTopPadding;
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "RandomPantheonsToggleRow",
                "Modules/RandomPantheons".Localize(),
                rowY,
                GetRandomPantheonsMasterEnabled,
                SetRandomPantheonsMasterEnabled,
                out randomPantheonsToggleValue,
                out randomPantheonsToggleIcon
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "RandomPantheonsP1Row",
                "Pantheon 1",
                rowY,
                GetRandomPantheonsP1Enabled,
                SetRandomPantheonsP1Enabled,
                out randomPantheonsP1Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "RandomPantheonsP2Row",
                "Pantheon 2",
                rowY,
                GetRandomPantheonsP2Enabled,
                SetRandomPantheonsP2Enabled,
                out randomPantheonsP2Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "RandomPantheonsP3Row",
                "Pantheon 3",
                rowY,
                GetRandomPantheonsP3Enabled,
                SetRandomPantheonsP3Enabled,
                out randomPantheonsP3Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "RandomPantheonsP4Row",
                "Pantheon 4",
                rowY,
                GetRandomPantheonsP4Enabled,
                SetRandomPantheonsP4Enabled,
                out randomPantheonsP4Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "RandomPantheonsP5Row",
                "Pantheon 5",
                rowY,
                GetRandomPantheonsP5Enabled,
                SetRandomPantheonsP5Enabled,
                out randomPantheonsP5Value
            );

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "RandomPantheonsResetRow", "Settings/RandomPantheons/Reset".Localize(), resetY, OnRandomPantheonsResetDefaultsClicked);
            CreateButtonRow(panel.transform, "RandomPantheonsBackRow", "Back", backY, OnRandomPantheonsBackClicked);
        }
    }
}
