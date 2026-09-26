using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildFreezeHitboxesOverlayUi()
        {
            freezeHitboxesRoot = CreateOverlayFrame("FreezeHitboxesOverlayCanvas", FreezeHitboxesCanvasSortOrder, "FreezeHitboxesPanel", PanelHeight, "Freeze Hitboxes", 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = PanelHeight;
            RectTransform content = CreateOverlayContent(panel, PanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            freezeHitboxesContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "FreezeHitboxesEnableRow",
                "Enable Freeze Hitboxes",
                rowY,
                GetFreezeHitboxesEnabled,
                SetFreezeHitboxesEnabled,
                out freezeHitboxesToggleValue,
                out freezeHitboxesToggleIcon
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateSelectRow(
                content,
                "FreezeHitboxesModeRow",
                "Freeze Mode",
                rowY,
                GetFreezeHitboxesModeLabel,
                ToggleFreezeHitboxesMode,
                out freezeHitboxesModeValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateKeybindRow(
                content,
                "FreezeHitboxesUnfreezeKeyRow",
                "Unfreeze Hotkey",
                rowY,
                GetFreezeHitboxesKeyLabel,
                StartFreezeHitboxesRebind,
                out freezeHitboxesUnfreezeKeyValue
            );

            lastY = rowY;
            rowY += RowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "FreezeHitboxesResetRow", "Reset Defaults", resetY, OnFreezeHitboxesResetDefaultsClicked);
            CreateButtonRow(panel.transform, "FreezeHitboxesBackRow", "Back", backY, OnFreezeHitboxesBackClicked);
        }
    }
}
