using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {        private void OnSpeedChanged(float value)
        {
            float rounded = RoundToTenth(value);
            Modules.QoL.FastSuperDash.fastSuperDashSpeedMultiplier = rounded;
            UpdateSpeedValueText(rounded);
        }

        private void OnDreamshieldDelayChanged(float value)
        {
            float rounded = (float)Math.Round(value, 2);
            Modules.QoL.DreamshieldStartAngle.rotationDelay = rounded;
            UpdateFloatValueText(dreamshieldDelayValue, rounded, 2);
            if (dreamshieldDelaySlider != null && Math.Abs(dreamshieldDelaySlider.value - rounded) > 0.0001f)
            {
                dreamshieldDelaySlider.value = rounded;
            }
        }

        private void OnDreamshieldSpeedChanged(float value)
        {
            float rounded = (float)Math.Round(value, 2);
            Modules.QoL.DreamshieldStartAngle.rotationSpeed = rounded;
            UpdateFloatValueText(dreamshieldSpeedValue, rounded, 2);
            if (dreamshieldSpeedSlider != null && Math.Abs(dreamshieldSpeedSlider.value - rounded) > 0.0001f)
            {
                dreamshieldSpeedSlider.value = rounded;
            }
        }

        private void OnShowHpFadeChanged(float value)
        {
            float rounded = (float)Math.Round(ClampShowHpFade(value), 1, MidpointRounding.AwayFromZero);
            ShowHpSettings.HudFadeSeconds = rounded;
            UpdateFloatValueText(showHpFadeValue, rounded, 1);
            if (showHpFadeSlider != null && Math.Abs(showHpFadeSlider.value - rounded) > 0.0001f)
            {
                showHpFadeSlider.value = rounded;
            }
        }

        private static float ClampShowHpFade(float value)
        {
            return Mathf.Clamp(value, 1f, 10f);
        }

        private static float RoundToTenth(float value)
        {
            return Mathf.Round(value * 10f) / 10f;
        }

        private static string FormatToggleValue(float value)
        {
            return value.ToString("0.0");
        }

        private static string FormatFloatValue(float value, int decimals)
        {
            string format = decimals <= 0 ? "0" : $"0.{new string('0', decimals)}";
            return value.ToString(format);
        }

        private void UpdateSpeedValueText(float value)
        {
            if (speedValueText != null)
            {
                speedValueText.text = FormatToggleValue(value);
            }
        }

        private void UpdateFloatValueText(Text? text, float value, int decimals)
        {
            if (text != null)
            {
                text.text = FormatFloatValue(value, decimals);
            }
        }

        private static int ClampOptionIndex(int value, int length)
        {
            if (length <= 0)
            {
                return 0;
            }

            return value < 0 || value >= length ? 0 : value;
        }

        private void UpdateIntValueText(Text? text, int value)
        {
            if (text != null)
            {
                text.text = value.ToString();
            }
        }

        private void UpdateIntInputValue(InputField? field, int value)
        {
            if (field != null)
            {
                field.text = value.ToString();
            }
        }

        private void AdjustIntValue(
            Func<int> getter,
            Action<int> setter,
            Text? valueText,
            int direction,
            int minValue,
            int maxValue,
            int baseStep)
        {
            int current = getter();
            int step = baseStep * GetStepMultiplier();
            long next = current + (long)direction * step;
            if (next < minValue)
            {
                next = minValue;
            }
            else if (next > maxValue)
            {
                next = maxValue;
            }

            setter((int)next);
            UpdateIntValueText(valueText, getter());
        }

        private void AdjustIntInputValue(
            Func<int> getter,
            Action<int> setter,
            InputField? valueField,
            int direction,
            int minValue,
            int maxValue,
            int baseStep)
        {
            int current = getter();
            int step = baseStep * GetStepMultiplier();
            long next = current + (long)direction * step;
            if (next < minValue)
            {
                next = minValue;
            }
            else if (next > maxValue)
            {
                next = maxValue;
            }

            setter((int)next);
            UpdateIntInputValue(valueField, getter());
        }

        private void ApplyInputValue(
            string rawValue,
            Func<int> getter,
            Action<int> setter,
            InputField? field,
            int minValue,
            int maxValue)
        {
            if (!int.TryParse(rawValue, out int parsed))
            {
                UpdateIntInputValue(field, getter());
                return;
            }

            parsed = Mathf.Clamp(parsed, minValue, maxValue);
            setter(parsed);
            UpdateIntInputValue(field, getter());
        }

        private static int GetStepMultiplier()
        {
            int mult = 1;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                mult *= 10;
            }

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                mult *= 100;
            }

            return mult;
        }

        private void UpdateToggleValue(Text? text, bool value)
        {
            if (text != null)
            {
                text.text = value ? "ON" : "OFF";
            }
        }
    }
}
