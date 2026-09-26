using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildMaskDamageOverlayUi()
        {
            maskDamageRoot = CreateOverlayFrame("MaskDamageOverlayCanvas", MaskDamageCanvasSortOrder, "MaskDamagePanel", PanelHeight, "Modules/MaskDamage".Localize(), 52, 420f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = PanelHeight;
            RectTransform content = CreateOverlayContent(panel, PanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            maskDamageContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "MaskDamageEnableRow",
                "Settings/MaskDamage/Enable".Localize(),
                rowY,
                GetMaskDamageEnabled,
                SetMaskDamageEnabled,
                out maskDamageToggleValue,
                out maskDamageToggleIcon
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "MaskDamageShowUiRow",
                "Settings/MaskDamage/ShowUI".Localize(),
                rowY,
                GetMaskDamageUiVisible,
                SetMaskDamageUiVisible,
                out maskDamageShowUiValue
            );
            maskDamageShowUiIcon = null;

            lastY = rowY;
            rowY += RowSpacing;
            CreateAdjustFloatInputRow(
                content,
                "MaskDamageMultiplierRow",
                "Settings/MaskDamage/Multiplier".Localize(),
                rowY,
                MaskDamage.GetMultiplier,
                MaskDamage.SetMultiplier,
                0.5f,
                999f,
                1f,
                out maskDamageMultiplierField
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateKeybindRow(
                content,
                "MaskDamageToggleUiRow",
                "Settings/MaskDamage/ToggleUI".Localize(),
                rowY,
                GetMaskDamageToggleUiKeyLabel,
                StartMaskDamageUiRebind,
                out maskDamageToggleUiKeyValue
            );

            lastY = rowY;
            rowY += RowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "MaskDamageResetRow", "Reset Defaults", resetY, OnMaskDamageResetDefaultsClicked);
            CreateButtonRow(panel.transform, "MaskDamageBackRow", "Back", backY, OnMaskDamageBackClicked);
        }
    }
}
