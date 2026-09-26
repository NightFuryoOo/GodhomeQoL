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
		private void HandleGearSwitcherPresetRename()
		{
			if (!(gearSwitcherPresetRenameField == null))
			{
				if (IsRenameConfirmPressed())
				{
					gearSwitcherPresetRenameSubmitPending = true;
					gearSwitcherPresetRenameField.DeactivateInputField();
				}
				else if (Input.GetKeyDown(KeyCode.Escape))
				{
					gearSwitcherPresetRenameCancelled = true;
					CancelGearSwitcherPresetRename();
				}
			}
		}

		private void StartGearSwitcherPresetRename(Text label, string presetName)
		{
			if (gearSwitcherPresetRenameField != null)
			{
				CommitGearSwitcherPresetRename(gearSwitcherPresetRenameField.text);
			}
			SetGearSwitcherPresetDeleteVisible(value: false);
			gearSwitcherPresetRenameCancelled = false;
			gearSwitcherPresetRenameSubmitPending = false;
			gearSwitcherPresetRenameLabel = label;
			gearSwitcherPresetRenameOriginalLabel = label.text;
			gearSwitcherPresetRenameTargetName = presetName;
			UnityEngine.UI.InputField inputField = CreateGearSwitcherPresetRenameField(label.transform.parent, label.text, label.fontSize);
			inputField.onEndEdit.AddListener(OnGearSwitcherPresetRenameEndEdit);
			gearSwitcherPresetRenameField = inputField;
			label.gameObject.SetActive(value: false);
			inputField.ActivateInputField();
			inputField.Select();
			inputField.MoveTextEnd(shift: false);
		}

		private void OnGearSwitcherPresetRenameEndEdit(string value)
		{
			if (gearSwitcherPresetRenameCancelled)
			{
				gearSwitcherPresetRenameCancelled = false;
			}
			else if (!gearSwitcherPresetRenameSubmitPending)
			{
				if (gearSwitcherPresetRenameField != null)
				{
					gearSwitcherPresetRenameField.ActivateInputField();
					gearSwitcherPresetRenameField.Select();
				}
			}
			else
			{
				gearSwitcherPresetRenameSubmitPending = false;
				CommitGearSwitcherPresetRename(value);
			}
		}

		private static bool IsRenameConfirmPressed()
		{
			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				return true;
			}
			try
			{
				HeroActions? heroActions = InputHandler.Instance?.inputActions;
				if (heroActions != null && heroActions.menuSubmit.WasPressed)
				{
					return true;
				}
			}
			catch (Exception swallowed)
			{
				LogSuppressed(swallowed, "QuickMenuController.GearSwitcher.Presets.Rename.cs");
			}
			return false;
		}

		private void CommitGearSwitcherPresetRename(string value)
		{
			if (gearSwitcherPresetRenameLabel == null)
			{
				return;
			}
			string text = value.Trim();
			if (text.Length > 40)
			{
				text = text.Substring(0, 40);
			}
			string text2 = gearSwitcherPresetRenameTargetName ?? string.Empty;
			string text3;
			if (IsFullGearPreset(text2))
			{
				text3 = NormalizeFullGearPresetName(text);
				SetFullGearDisplayName(text3);
			}
			else
			{
				text3 = NormalizeCustomPresetName(text2, text);
				if (!string.Equals(text3, text2, StringComparison.OrdinalIgnoreCase))
				{
					if (GearSwitcher.RenameCustomPreset(text2, text3))
					{
						if (string.Equals(gearSwitcherSelectedPreset, text2, StringComparison.OrdinalIgnoreCase))
						{
							gearSwitcherSelectedPreset = text3;
						}
					}
					else
					{
						text3 = text2;
					}
				}
			}
			gearSwitcherPresetRenameLabel.text = GetGearSwitcherPresetDisplayName(text3);
			gearSwitcherPresetRenameLabel.gameObject.SetActive(value: true);
			if (gearSwitcherPresetRenameField != null)
			{
				UnityEngine.Object.Destroy(gearSwitcherPresetRenameField.gameObject);
			}
			gearSwitcherPresetRenameField = null;
			gearSwitcherPresetRenameLabel = null;
			gearSwitcherPresetRenameOriginalLabel = null;
			gearSwitcherPresetRenameTargetName = null;
			RebuildGearSwitcherPresetOverlay();
			SetGearSwitcherPresetVisible(value: true);
			RefreshGearSwitcherUi();
		}

		private void CancelGearSwitcherPresetRename()
		{
			if (!(gearSwitcherPresetRenameLabel == null))
			{
				if (gearSwitcherPresetRenameOriginalLabel != null)
				{
					gearSwitcherPresetRenameLabel.text = gearSwitcherPresetRenameOriginalLabel;
				}
				gearSwitcherPresetRenameLabel.gameObject.SetActive(value: true);
				if (gearSwitcherPresetRenameField != null)
				{
					UnityEngine.Object.Destroy(gearSwitcherPresetRenameField.gameObject);
				}
				gearSwitcherPresetRenameField = null;
				gearSwitcherPresetRenameLabel = null;
				gearSwitcherPresetRenameOriginalLabel = null;
				gearSwitcherPresetRenameTargetName = null;
			}
		}

		private string NormalizeFullGearPresetName(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return "FullGear";
			}
			if (string.Equals(value, "FullGear", StringComparison.OrdinalIgnoreCase))
			{
				return "FullGear";
			}
			string fullGearDisplayName = GetFullGearDisplayName();
			if (string.Equals(value, fullGearDisplayName, StringComparison.OrdinalIgnoreCase))
			{
				return "FullGear";
			}
			if (IsGearSwitcherPresetNameTaken(value))
			{
				return "FullGear";
			}
			return value;
		}

		private UnityEngine.UI.InputField CreateGearSwitcherPresetRenameField(Transform parent, string text, int fontSize)
		{
			GameObject gameObject = new GameObject("PresetRenameField");
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(1f, 1f, 1f, 0.08f);
			UnityEngine.UI.InputField inputField = gameObject.AddComponent<UnityEngine.UI.InputField>();
			inputField.lineType = UnityEngine.UI.InputField.LineType.SingleLine;
			inputField.contentType = UnityEngine.UI.InputField.ContentType.Standard;
			inputField.caretColor = Color.white;
			inputField.selectionColor = new Color(1f, 1f, 1f, 0.25f);
			inputField.targetGraphic = image;
			inputField.text = text;
			inputField.characterLimit = 40;
			Text text2 = CreateText(gameObject.transform, "Text", text, fontSize, TextAnchor.MiddleLeft);
			RectTransform rectTransform2 = text2.rectTransform;
			rectTransform2.anchorMin = new Vector2(0f, 0f);
			rectTransform2.anchorMax = new Vector2(1f, 1f);
			rectTransform2.offsetMin = new Vector2(20f, 0f);
			rectTransform2.offsetMax = new Vector2(-40f, 0f);
			inputField.textComponent = text2;
			return inputField;
		}
    }
}
