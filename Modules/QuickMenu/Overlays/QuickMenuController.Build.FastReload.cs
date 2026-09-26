using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildFastReloadOverlayUi()
        {
            fastReloadRoot = CreateOverlayFrame("FastReloadOverlayCanvas", FastReloadCanvasSortOrder, "FastReloadPanel", PanelHeight, "Modules/FastReload".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = PanelHeight;
            RectTransform content = CreateOverlayContent(panel, PanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            fastReloadContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "FastReloadToggleRow",
                "Settings/FastReload/Enable".Localize(),
                rowY,
                GetFastReloadEnabled,
                SetFastReloadEnabled,
                out fastReloadToggleValue,
                out fastReloadToggleIcon
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateKeybindRow(
                content,
                "ReloadKeyRow",
                "Settings/reloadBossKey".Localize(),
                rowY,
                () => FormatKeyLabel(GetReloadKey()),
                StartReloadRebind,
                out reloadKeyValue
            );

            lastY = rowY;
            rowY += RowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "FastReloadResetRow", "Settings/FastReload/Reset".Localize(), resetY, OnFastReloadResetDefaultsClicked);
            CreateButtonRow(panel.transform, "FastReloadBackRow", "Back", backY, OnFastReloadBackClicked);
        }
    }
}
