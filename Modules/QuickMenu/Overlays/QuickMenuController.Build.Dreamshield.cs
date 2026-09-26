using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildDreamshieldOverlayUi()
        {
            dreamshieldRoot = CreateOverlayFrame("DreamshieldOverlayCanvas", DreamshieldCanvasSortOrder, "DreamshieldPanel", PanelHeight, "DreamshieldSettings".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = PanelHeight;
            RectTransform content = CreateOverlayContent(panel, PanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            dreamshieldContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "DreamshieldToggleRow",
                "Settings/DreamshieldStartAngle/Enable".Localize(),
                rowY,
                GetDreamshieldEnabled,
                SetDreamshieldEnabled,
                out dreamshieldToggleValue,
                out dreamshieldToggleIcon
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateFloatSliderRow(
                content,
                "DreamshieldDelayRow",
                "Settings/DreamshieldStartAngle/rotationDelay".Localize(),
                rowY,
                rowY + 40f,
                0f,
                10f,
                Modules.QoL.DreamshieldStartAngle.rotationDelay,
                2,
                out dreamshieldDelayValue,
                out dreamshieldDelaySlider
            );
            if (dreamshieldDelaySlider != null)
            {
                dreamshieldDelaySlider.onValueChanged.AddListener(OnDreamshieldDelayChanged);
            }

            lastY = rowY + 40f;
            rowY += RowSpacing + 40f;
            CreateFloatSliderRow(
                content,
                "DreamshieldSpeedRow",
                "Settings/DreamshieldStartAngle/rotationSpeed".Localize(),
                rowY,
                rowY + 40f,
                0f,
                10f,
                Modules.QoL.DreamshieldStartAngle.rotationSpeed,
                2,
                out dreamshieldSpeedValue,
                out dreamshieldSpeedSlider
            );
            if (dreamshieldSpeedSlider != null)
            {
                dreamshieldSpeedSlider.onValueChanged.AddListener(OnDreamshieldSpeedChanged);
            }

            lastY = rowY + 40f;
            rowY += RowSpacing + 40f;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "DreamshieldResetRow", "Settings/DreamshieldStartAngle/Reset".Localize(), resetY, OnDreamshieldResetDefaultsClicked);
            CreateButtonRow(panel.transform, "DreamshieldBackRow", "Back", backY, OnDreamshieldBackClicked);
        }
    }
}
