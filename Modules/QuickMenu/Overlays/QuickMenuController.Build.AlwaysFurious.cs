using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildAlwaysFuriousOverlayUi()
        {
            alwaysFuriousRoot = CreateOverlayFrame("AlwaysFuriousOverlayCanvas", AlwaysFuriousCanvasSortOrder, "AlwaysFuriousPanel", PanelHeight, "Modules/AlwaysFurious".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = PanelHeight;
            RectTransform content = CreateOverlayContent(panel, PanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);

            float rowY = GetRowStartY(panelHeight, GearSwitcherRowStartY, topOffset) + GearSwitcherTopPadding;
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "AlwaysFuriousToggleRow",
                "Settings/AlwaysFurious/Enable".Localize(),
                rowY,
                GetAlwaysFuriousEnabled,
                SetAlwaysFuriousEnabled,
                out alwaysFuriousToggleValue,
                out alwaysFuriousToggleIcon
            );

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "AlwaysFuriousResetRow", "Settings/AlwaysFurious/Reset".Localize(), resetY, OnAlwaysFuriousResetDefaultsClicked);
            CreateButtonRow(panel.transform, "AlwaysFuriousBackRow", "Back", backY, OnAlwaysFuriousBackClicked);
        }
    }
}
