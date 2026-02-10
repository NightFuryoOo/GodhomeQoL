using System;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void CreateCollectorToggleRow(
            Transform parent,
            string name,
            string label,
            float y,
            Func<bool> getter,
            Action<bool> setter,
            out Text valueText)
        {
            GameObject row = CreateRow(parent, name, y, new Vector2(CollectorRowWidth, CollectorRowHeight));
            Image image = row.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);

            Button button = row.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = image;

            _ = CreateCollectorRowLabel(row.transform, label);
            Text valueLabel = CreateCollectorRowValue(row.transform, getter() ? "ON" : "OFF");
            valueText = valueLabel;

            button.onClick.AddListener(() =>
            {
                bool newValue = !getter();
                setter(newValue);
                UpdateToggleValue(valueLabel, newValue);
            });

            RowHighlight highlight = CreateRowHighlight(row, image);
            AttachRowHighlight(row, highlight);
        }

        private void CreateCollectorToggleRowWithIcon(
            Transform parent,
            string name,
            string label,
            float y,
            Func<bool> getter,
            Action<bool> setter,
            out Text valueText,
            out Image? iconImage)
        {
            GameObject row = CreateRow(parent, name, y, new Vector2(CollectorRowWidth, CollectorRowHeight));
            Image image = row.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);

            Button button = row.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = image;

            bool initialValue = getter();
            _ = CreateCollectorRowLabelWithIcon(row.transform, label, initialValue, out iconImage);
            Text valueLabel = CreateCollectorRowValue(row.transform, initialValue ? "ON" : "OFF");
            valueText = valueLabel;
            Image? toggleIcon = iconImage;

            button.onClick.AddListener(() =>
            {
                bool newValue = !getter();
                setter(newValue);
                bool current = getter();
                UpdateToggleValue(valueLabel, current);
                UpdateToggleIcon(toggleIcon, current);
            });

            RowHighlight highlight = CreateRowHighlight(row, image);
            AttachRowHighlight(row, highlight);
        }

        private void CreateCollectorAdjustRow(
            Transform parent,
            string name,
            string label,
            float y,
            Func<int> getter,
            Action<int> setter,
            int minValue,
            int maxValue,
            int baseStep,
            out Text valueText)
        {
            GameObject row = CreateRow(parent, name, y, new Vector2(CollectorRowWidth, CollectorRowHeight));
            Image image = row.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);

            _ = CreateCollectorRowLabel(row.transform, label);

            Text valueLabel = CreateText(row.transform, "Value", getter().ToString(), CollectorValueFontSize, TextAnchor.MiddleCenter);
            RectTransform valueRect = valueLabel.rectTransform;
            valueRect.anchorMin = new Vector2(1f, 0.5f);
            valueRect.anchorMax = new Vector2(1f, 0.5f);
            valueRect.pivot = new Vector2(1f, 0.5f);
            valueRect.anchoredPosition = new Vector2(CollectorControlValueRight, 0f);
            valueRect.sizeDelta = new Vector2(CollectorControlValueWidth, CollectorRowHeight);
            valueText = valueLabel;

            Button minus = CreateCollectorMiniButton(row.transform, "Minus", "-", new Vector2(CollectorControlMinusRight, 0f));
            minus.onClick.AddListener(() => AdjustIntValue(getter, setter, valueLabel, -1, minValue, maxValue, baseStep));

            Button plus = CreateCollectorMiniButton(row.transform, "Plus", "+", new Vector2(CollectorControlPlusRight, 0f));
            plus.onClick.AddListener(() => AdjustIntValue(getter, setter, valueLabel, 1, minValue, maxValue, baseStep));

            RowHighlight highlight = CreateRowHighlight(row, image);
            AttachRowHighlight(row, highlight);
            AttachRowHighlight(minus.gameObject, highlight);
            AttachRowHighlight(plus.gameObject, highlight);
        }

        private void CreateCollectorAdjustInputRow(
            Transform parent,
            string name,
            string label,
            float y,
            Func<int> getter,
            Action<int> setter,
            int minValue,
            int maxValue,
            int baseStep,
            out InputField valueField)
        {
            GameObject row = CreateRow(parent, name, y, new Vector2(CollectorRowWidth, CollectorRowHeight));
            Image image = row.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);

            _ = CreateCollectorRowLabel(row.transform, label);

            InputField input = CreateCollectorInputField(
                row.transform,
                getter().ToString(),
                new Vector2(CollectorControlValueRight, 0f),
                new Vector2(CollectorControlValueWidth, CollectorRowHeight));
            valueField = input;

            input.onEndEdit.AddListener(value =>
                ApplyInputValue(value, getter, setter, input, minValue, maxValue));

            Button minus = CreateCollectorMiniButton(row.transform, "Minus", "-", new Vector2(CollectorControlMinusRight, 0f));
            minus.onClick.AddListener(() => AdjustIntInputValue(getter, setter, input, -1, minValue, maxValue, baseStep));

            Button plus = CreateCollectorMiniButton(row.transform, "Plus", "+", new Vector2(CollectorControlPlusRight, 0f));
            plus.onClick.AddListener(() => AdjustIntInputValue(getter, setter, input, 1, minValue, maxValue, baseStep));

            RowHighlight highlight = CreateRowHighlight(row, image);
            AttachRowHighlight(row, highlight);
            AttachRowHighlight(input.gameObject, highlight);
            AttachRowHighlight(minus.gameObject, highlight);
            AttachRowHighlight(plus.gameObject, highlight);
        }

        private InputField CreateCollectorInputField(Transform parent, string text, Vector2 anchoredPosition, Vector2 size)
        {
            GameObject fieldObj = new GameObject("InputField");
            fieldObj.transform.SetParent(parent, false);

            RectTransform rect = fieldObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0.5f);
            rect.anchorMax = new Vector2(1f, 0.5f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            Image image = fieldObj.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.08f);

            InputField input = fieldObj.AddComponent<InputField>();
            input.contentType = InputField.ContentType.IntegerNumber;
            input.lineType = InputField.LineType.SingleLine;
            input.caretColor = Color.white;
            input.selectionColor = new Color(1f, 1f, 1f, 0.25f);
            input.targetGraphic = image;

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(fieldObj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(6f, 0f);
            textRect.offsetMax = new Vector2(-6f, 0f);

            Text valueText = textObj.AddComponent<Text>();
            valueText.text = text;
            valueText.font = GetMenuFont();
            valueText.fontSize = CollectorValueFontSize;
            valueText.alignment = TextAnchor.MiddleCenter;
            valueText.color = Color.white;
            valueText.raycastTarget = false;

            input.textComponent = valueText;
            input.text = text;

            return input;
        }

        private Text CreateCollectorRowLabel(Transform parent, string text)
        {
            Text label = CreateText(parent, "Label", text, CollectorLabelFontSize, TextAnchor.MiddleLeft);
            RectTransform rect = label.rectTransform;
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(20f, 0f);
            rect.sizeDelta = new Vector2(CollectorLabelWidth, CollectorRowHeight);
            return label;
        }

        private Text CreateCollectorRowLabelWithIcon(Transform parent, string text, bool isOn, out Image? icon)
        {
            Sprite? sprite = GetToggleIconSprite(isOn);
            if (sprite == null)
            {
                icon = null;
                return CreateCollectorRowLabel(parent, text);
            }

            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(parent, false);

            icon = iconObj.AddComponent<Image>();
            icon.sprite = sprite;
            icon.preserveAspect = true;

            RectTransform iconRect = icon.rectTransform;
            iconRect.anchorMin = new Vector2(0f, 0f);
            iconRect.anchorMax = new Vector2(0f, 1f);
            iconRect.pivot = new Vector2(0f, 0.5f);
            iconRect.anchoredPosition = new Vector2(20f, 0f);
            iconRect.sizeDelta = new Vector2(ToggleRowIconSize, ToggleRowIconSize);

            Text label = CreateCollectorRowLabel(parent, text);
            RectTransform rect = label.rectTransform;
            float labelOffset = 20f + ToggleRowIconSize + ToggleRowIconSpacing;
            rect.anchoredPosition = new Vector2(labelOffset, 0f);
            rect.sizeDelta = new Vector2(CollectorLabelWidth - (labelOffset - 20f), CollectorRowHeight);
            return label;
        }

        private Text CreateCollectorRowValue(Transform parent, string text)
        {
            Text value = CreateText(parent, "Value", text, CollectorValueFontSize, TextAnchor.MiddleRight);
            RectTransform rect = value.rectTransform;
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = new Vector2(-20f, 0f);
            rect.sizeDelta = new Vector2(160f, CollectorRowHeight);
            return value;
        }

        private Button CreateCollectorMiniButton(Transform parent, string name, string label, Vector2 anchoredPosition)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0.5f);
            rect.anchorMax = new Vector2(1f, 0.5f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(CollectorControlButtonWidth, CollectorControlButtonHeight);

            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.08f);

            Outline outline = buttonObj.AddComponent<Outline>();
            outline.effectColor = Color.white;
            outline.effectDistance = new Vector2(1f, -1f);

            Button button = buttonObj.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = image;

            Text text = CreateText(buttonObj.transform, "Label", label, 22, TextAnchor.MiddleCenter);
            RectTransform textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            return button;
        }
    }
}
