using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ToggleableBindings;
using ToggleableBindings.VanillaBindings;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
		private void CreateGearSwitcherPresetRow(Transform parent, int index, string presetName, float y)
		{
			GameObject gameObject = CreateRow(parent, $"GearSwitcherPresetRow{index}", y, new Vector2(820f, 44f));
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			Button button = gameObject.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.targetGraphic = image;
			Text label = CreateRowLabel(gameObject.transform, GetGearSwitcherPresetDisplayName(presetName));
			button.onClick.AddListener(delegate
			{
				OnGearSwitcherPresetSelected(index);
			});
			RowHighlight highlight = CreateRowHighlight(gameObject, image);
			AttachRowHighlight(gameObject, highlight);
			gearSwitcherPresetEntries.Add(new GearSwitcherPresetEntry(index, label, highlight));
			if (!IsBuiltinPresetName(presetName))
			{
				Button? button2 = CreateGearSwitcherPresetEditButton(gameObject.transform, delegate
				{
					StartGearSwitcherPresetRename(label, presetName);
				});
				if (button2 != null)
				{
					AttachRowHighlight(button2.gameObject, highlight);
				}
			}
			if (!IsBuiltinPresetName(presetName))
			{
				Button? button3 = CreateGearSwitcherPresetDeleteButton(gameObject.transform, delegate
				{
					StartGearSwitcherPresetDeleteByIndex(index);
				});
				if (button3 != null)
				{
					AttachRowHighlight(button3.gameObject, highlight);
				}
			}
		}

		private Button? CreateGearSwitcherPresetEditButton(Transform parent, Action onClick)
		{
			gearSwitcherPresetEditSprite ??= LoadCollectorIconSprite("Edit.png", "GearPresetEdit");
			if (gearSwitcherPresetEditSprite == null)
			{
				return null;
			}

			return CreateGearSwitcherPresetIconButton(parent, "PresetEdit", gearSwitcherPresetEditSprite, new Vector2(340f, 0f), onClick);
		}

		private Button? CreateGearSwitcherPresetDeleteButton(Transform parent, Action onClick)
		{
			gearSwitcherPresetDeleteSprite ??= LoadCollectorIconSprite("Del.png", "GearPresetDelete");
			if (gearSwitcherPresetDeleteSprite == null)
			{
				return null;
			}

			return CreateGearSwitcherPresetIconButton(parent, "PresetDelete", gearSwitcherPresetDeleteSprite, new Vector2(380f, 0f), onClick);
		}

		private Button CreateGearSwitcherPresetIconButton(Transform parent, string name, Sprite sprite, Vector2 position, Action onClick)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = position;
			rectTransform.sizeDelta = new Vector2(28f, 28f);
			Image image = gameObject.AddComponent<Image>();
			image.sprite = sprite;
			image.preserveAspect = true;
			Button button = gameObject.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.targetGraphic = image;
			button.onClick.AddListener(delegate
			{
				onClick();
			});
			return button;
		}
    }
}
