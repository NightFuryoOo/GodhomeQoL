using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private void OnQuickEntryClicked(QuickMenuItemDefinition definition, QuickMenuEntry entry)
        {
            if (renameField != null && (!quickRenameMode || !IsQuickMenuRenameAllowed(entry.Id)))
            {
                CommitQuickMenuRename(renameField.text);
            }

            if (quickRenameMode && IsQuickMenuRenameAllowed(entry.Id))
            {
                StartQuickMenuRename(entry);
                return;
            }

            definition.OnClick();
        }

        private void HandleQuickMenuRename()
        {
            if (renameField == null)
            {
                return;
            }

            if (IsRenameConfirmPressed())
            {
                quickRenameSubmitPending = true;
                renameField.DeactivateInputField();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                renameCancelled = true;
                CancelQuickMenuRename();
            }
        }

        private void StartQuickMenuRename(QuickMenuEntry entry)
        {
            if (!IsQuickMenuRenameAllowed(entry.Id))
            {
                return;
            }

            if (renameField != null && renameEntry != null)
            {
                CommitQuickMenuRename(renameField.text);
            }

            renameCancelled = false;
            quickRenameSubmitPending = false;
            renameEntry = entry;
            renameOriginalLabel = entry.Label.text;

            InputField field = CreateQuickMenuRenameField(entry.Button.transform, entry.Label.text, entry.Label.fontSize);
            field.onEndEdit.AddListener(OnQuickMenuRenameEndEdit);
            renameField = field;

            entry.Label.gameObject.SetActive(false);
            field.ActivateInputField();
            field.Select();
            field.MoveTextEnd(false);
        }

        private void OnQuickMenuRenameEndEdit(string value)
        {
            if (renameCancelled)
            {
                renameCancelled = false;
                return;
            }

            if (!quickRenameSubmitPending)
            {
                if (renameField != null)
                {
                    renameField.ActivateInputField();
                    renameField.Select();
                }
                return;
            }

            quickRenameSubmitPending = false;
            CommitQuickMenuRename(value);
        }

        private void CommitQuickMenuRename(string value)
        {
            if (renameEntry == null)
            {
                return;
            }

            string trimmed = value.Trim();
            if (string.IsNullOrEmpty(trimmed) || string.Equals(trimmed, renameEntry.DefaultLabel, StringComparison.Ordinal))
            {
                SetQuickMenuCustomLabel(renameEntry.Id, null);
                renameEntry.Label.text = renameEntry.DefaultLabel;
            }
            else
            {
                SetQuickMenuCustomLabel(renameEntry.Id, trimmed);
                renameEntry.Label.text = trimmed;
            }

            renameEntry.Label.gameObject.SetActive(true);
            UpdateQuickMenuEntryIcons(renameEntry);

            if (renameField != null)
            {
                UObject.Destroy(renameField.gameObject);
            }

            renameField = null;
            renameEntry = null;
            renameOriginalLabel = null;
        }

        private void CancelQuickMenuRename()
        {
            if (renameEntry == null)
            {
                return;
            }

            if (renameOriginalLabel != null)
            {
                renameEntry.Label.text = renameOriginalLabel;
                UpdateQuickMenuEntryIcons(renameEntry);
            }

            renameEntry.Label.gameObject.SetActive(true);

            if (renameField != null)
            {
                UObject.Destroy(renameField.gameObject);
            }

            renameField = null;
            renameEntry = null;
            renameOriginalLabel = null;
        }

        private InputField CreateQuickMenuRenameField(Transform parent, string text, int fontSize)
        {
            GameObject fieldObj = new GameObject("RenameField");
            fieldObj.transform.SetParent(parent, false);

            RectTransform rect = fieldObj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = fieldObj.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.08f);

            InputField input = fieldObj.AddComponent<InputField>();
            input.lineType = InputField.LineType.SingleLine;
            input.contentType = InputField.ContentType.Standard;
            input.caretColor = Color.white;
            input.selectionColor = new Color(1f, 1f, 1f, 0.25f);
            input.targetGraphic = image;
            input.text = text;

            Text inputText = CreateText(fieldObj.transform, "Text", text, fontSize, TextAnchor.MiddleCenter);
            RectTransform textRect = inputText.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(6f, 0f);
            textRect.offsetMax = new Vector2(-6f, 0f);
            input.textComponent = inputText;

            return input;
        }

        private void SetQuickMenuCustomLabel(string id, string? value)
        {
            Dictionary<string, string> labels = GodhomeQoL.GlobalSettings.QuickMenuCustomLabels ??= new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(value))
            {
                labels.Remove(id);
            }
            else
            {
                labels[id] = value!;
            }

            GodhomeQoL.SaveGlobalSettingsSafe();
        }
    }
}
