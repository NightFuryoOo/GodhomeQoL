using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private readonly List<(Slider Slider, Text Value, Func<float> Get)> textScaleSliders = new();
        private bool textScaleSync;

        private float AddTextScaleRow(
            RectTransform content,
            string name,
            string label,
            float rowY,
            float spacing,
            Func<float> get,
            Action<float> set,
            Action preview,
            out float lastY)
        {
            CreateFloatSliderRow(
                content,
                name,
                label,
                rowY,
                rowY + 40f,
                HudTextScales.Min * 100f,
                HudTextScales.Max * 100f,
                HudTextScales.ToPercent(get()),
                0,
                out Text valueText,
                out Slider slider
            );

            textScaleSliders.Add((slider, valueText, get));
            slider.onValueChanged.AddListener(value =>
            {
                if (textScaleSync)
                {
                    return;
                }

                set(HudTextScales.FromPercent(value));
                SyncTextScaleSlider(slider, valueText, get);
                preview();
            });

            lastY = rowY + 40f;
            return rowY + spacing + 40f;
        }

        private void SyncTextScaleSlider(Slider slider, Text valueText, Func<float> get)
        {
            int percent = HudTextScales.ToPercent(get());
            UpdateFloatValueText(valueText, percent, 0);
            if (Math.Abs(slider.value - percent) <= 0.0001f)
            {
                return;
            }

            bool previous = textScaleSync;
            textScaleSync = true;
            slider.value = percent;
            textScaleSync = previous;
        }

        private void RefreshTextScaleSliders()
        {
            foreach ((Slider slider, Text value, Func<float> get) in textScaleSliders)
            {
                if (slider != null)
                {
                    SyncTextScaleSlider(slider, value, get);
                }
            }
        }

        private static void PreviewSpeedChangerText()
        {
            ModDisplay.Instance?.ApplyScale();
            ModDisplay.Instance?.Preview("Game Speed: 1.00");
        }
    }
}
