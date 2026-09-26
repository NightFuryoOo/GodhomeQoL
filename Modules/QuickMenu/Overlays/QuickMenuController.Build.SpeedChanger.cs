using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildSpeedChangerOverlayUi()
        {
            speedChangerRoot = CreateOverlayFrame("SpeedChangerOverlayCanvas", SpeedChangerCanvasSortOrder, "SpeedChangerPanel", SpeedChangerPanelHeight, "SpeedChanger".Localize(), 52, 300f, out GameObject panel, out RectTransform titleRect);


            float panelHeight = SpeedChangerPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            speedChangerContent = content;

            float rowY = GetRowStartY(panelHeight, SpeedChangerRowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "SpeedChangerGlobalRow",
                "SpeedChanger/Enable".Localize(),
                rowY,
                () => SpeedChanger.globalSwitch,
                SetSpeedChangerGlobalSwitch,
                out speedChangerGlobalValue,
                out speedChangerGlobalIcon
            );

            lastY = rowY;
            rowY += SpeedChangerRowSpacing;
            CreateToggleRow(
                content,
                "SpeedChangerRestrictRow",
                "SpeedChanger/RestrictRooms".Localize(),
                rowY,
                () => SpeedChanger.restrictToggleToRooms,
                value => SpeedChanger.restrictToggleToRooms = value,
                out speedChangerRestrictValue
            );

            lastY = rowY;
            rowY += SpeedChangerRowSpacing;
            CreateKeybindRow(
                content,
                "SpeedChangerToggleKeyRow",
                "SpeedChanger/ToggleKey".Localize(),
                rowY,
                () => FormatSpeedChangerKeyLabel(SpeedChanger.toggleKeybind),
                StartSpeedChangerToggleRebind,
                out speedChangerToggleKeyValue
            );

            lastY = rowY;
            rowY += SpeedChangerRowSpacing;
            CreateToggleRow(
                content,
                "SpeedChangerUnlimitedRow",
                "SpeedChanger/UnlimitedSpeed".Localize(),
                rowY,
                () => SpeedChanger.unlimitedSpeed,
                value => SpeedChanger.unlimitedSpeed = value,
                out speedChangerUnlimitedValue
            );

            lastY = rowY;
            rowY += SpeedChangerRowSpacing;
            CreateKeybindRow(
                content,
                "SpeedChangerInputKeyRow",
                "SpeedChanger/InputKey".Localize(),
                rowY,
                () => FormatSpeedChangerKeyLabel(SpeedChanger.inputSpeedKeybind),
                StartSpeedChangerInputRebind,
                out speedChangerInputKeyValue
            );

            lastY = rowY;
            rowY += SpeedChangerRowSpacing;
            CreateCycleRow(
                content,
                "SpeedChangerDisplayRow",
                "SpeedChanger/DisplayStyle".Localize(),
                rowY,
                SpeedChangerDisplayOptions,
                () => SpeedChanger.displayStyle,
                ApplySpeedChangerDisplayStyle,
                out speedChangerDisplayValue
            );

            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "SpeedChangerResetRow", "SpeedChanger/Reset".Localize(), resetY, OnSpeedChangerResetDefaultsClicked);
            CreateButtonRow(panel.transform, "SpeedChangerBackRow", "Back", backY, OnSpeedChangerBackClicked);
        }
    }
}
