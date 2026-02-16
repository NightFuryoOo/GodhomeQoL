using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void UpdateFreeMenuLabel()
        {
            QuickMenuEntry? entry = quickEntries.FirstOrDefault(item => item.Id == "FreeMenu");
            if (entry == null)
            {
                return;
            }

            entry.Label.text = GetFreeMenuLabel();
        }

        private string GetRenameModeLabel()
        {
            string label = "QuickMenu/RenameMode".Localize();
            string state = quickRenameMode ? "ON" : "OFF";
            return $"{label}: {state}";
        }

        private void UpdateRenameModeLabel()
        {
            QuickMenuEntry? entry = quickEntries.FirstOrDefault(item => item.Id == "RenameMode");
            if (entry == null)
            {
                return;
            }

            entry.Label.text = GetRenameModeLabel();
        }

        private static bool IsQuickMenuRenameAllowed(string id)
        {
            return id != "FreeMenu" && id != "RenameMode" && id != "Settings";
        }

        private static bool IsOverlayHotkeySupported(string id)
        {
            return id != "FreeMenu" && id != "RenameMode" && id != "Settings";
        }

        private Dictionary<string, string> GetOverlayHotkeyMap()
        {
            return GodhomeQoL.GlobalSettings.QuickMenuOverlayHotkeys ??= new Dictionary<string, string>();
        }

        private string GetOverlayHotkeyRaw(string id)
        {
            Dictionary<string, string> map = GetOverlayHotkeyMap();
            return map.TryGetValue(id, out string? raw) ? raw : string.Empty;
        }

        private void SetOverlayHotkeyRaw(string id, string raw)
        {
            Dictionary<string, string> map = GetOverlayHotkeyMap();
            if (!string.IsNullOrWhiteSpace(raw))
            {
                if (TryParseOverlayControllerBinding(raw, out InputControlType button))
                {
                    if (IsOverlayControllerHotkeyInUse(id, button))
                    {
                        raw = string.Empty;
                    }
                }
                else if (Enum.TryParse(raw, true, out KeyCode key))
                {
                    if (IsOverlayKeyboardHotkeyInUse(id, key) || key == GetQuickMenuToggleKey())
                    {
                        raw = string.Empty;
                    }
                }
            }
            map[id] = raw;
        }

        private string GetOverlayHotkeyLabel(string id)
        {
            string raw = GetOverlayHotkeyRaw(id);
            if (string.IsNullOrWhiteSpace(raw))
            {
                return "Settings/FastReload/NotSet".Localize();
            }

            if (TryParseOverlayControllerBinding(raw, out InputControlType button))
            {
                return button.ToString();
            }

            if (Enum.TryParse(raw, true, out KeyCode key))
            {
                return FormatKeyLabel(key);
            }

            return raw;
        }

        private KeyCode GetOverlayHotkeyKey(string id)
        {
            string raw = GetOverlayHotkeyRaw(id);
            if (string.IsNullOrWhiteSpace(raw))
            {
                return KeyCode.None;
            }

            if (TryParseOverlayControllerBinding(raw, out _))
            {
                return KeyCode.None;
            }

            return Enum.TryParse(raw, true, out KeyCode key) ? key : KeyCode.None;
        }

        private InputControlType GetOverlayHotkeyControllerBinding(string id)
        {
            string raw = GetOverlayHotkeyRaw(id);
            return TryParseOverlayControllerBinding(raw, out InputControlType button) ? button : default;
        }

        private static bool TryParseOverlayControllerBinding(string raw, out InputControlType button)
        {
            button = default;
            if (string.IsNullOrWhiteSpace(raw) || !raw.StartsWith(OverlayControllerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string value = raw.Substring(OverlayControllerPrefix.Length);
            return Enum.TryParse(value, true, out button);
        }

        private bool IsOverlayKeyboardHotkeyInUse(string id, KeyCode key)
        {
            if (key == KeyCode.None)
            {
                return false;
            }

            foreach (KeyValuePair<string, string> entry in GetOverlayHotkeyMap())
            {
                if (string.Equals(entry.Key, id, StringComparison.Ordinal))
                {
                    continue;
                }

                string raw = entry.Value;
                if (string.IsNullOrWhiteSpace(raw) || TryParseOverlayControllerBinding(raw, out _))
                {
                    continue;
                }

                if (Enum.TryParse(raw, true, out KeyCode existing) && existing == key)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsOverlayControllerHotkeyInUse(string id, InputControlType button)
        {
            if (EqualityComparer<InputControlType>.Default.Equals(button, default))
            {
                return false;
            }

            foreach (KeyValuePair<string, string> entry in GetOverlayHotkeyMap())
            {
                if (string.Equals(entry.Key, id, StringComparison.Ordinal))
                {
                    continue;
                }

                string raw = entry.Value;
                if (TryParseOverlayControllerBinding(raw, out InputControlType existing)
                    && EqualityComparer<InputControlType>.Default.Equals(existing, button))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsControllerButtonPressed(InputControlType button)
        {
            foreach (InputDevice device in InputManager.Devices)
            {
                if (device == null)
                {
                    continue;
                }

                InputControl control = device.GetControl(button);
                if (control != null && control.IsButton && control.WasPressed)
                {
                    return true;
                }
            }

            return false;
        }

        private void SetOverlayHotkeySuppress(string raw, KeyCode key, InputControlType button)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                overlayHotkeySuppressActive = false;
                overlayHotkeySuppressKey = KeyCode.None;
                overlayHotkeySuppressButton = default;
                return;
            }

            if (key != KeyCode.None)
            {
                overlayHotkeySuppressActive = true;
                overlayHotkeySuppressKey = key;
                overlayHotkeySuppressButton = default;
                return;
            }

            if (!EqualityComparer<InputControlType>.Default.Equals(button, default))
            {
                overlayHotkeySuppressActive = true;
                overlayHotkeySuppressKey = KeyCode.None;
                overlayHotkeySuppressButton = button;
            }
        }

        private bool IsOverlayHotkeySuppressed()
        {
            if (!overlayHotkeySuppressActive)
            {
                return false;
            }

            if (overlayHotkeySuppressKey != KeyCode.None)
            {
                if (Input.GetKey(overlayHotkeySuppressKey))
                {
                    return true;
                }

                overlayHotkeySuppressActive = false;
                overlayHotkeySuppressKey = KeyCode.None;
                return false;
            }

            if (!EqualityComparer<InputControlType>.Default.Equals(overlayHotkeySuppressButton, default))
            {
                if (IsControllerButtonDown(overlayHotkeySuppressButton))
                {
                    return true;
                }

                overlayHotkeySuppressActive = false;
                overlayHotkeySuppressButton = default;
            }

            return false;
        }

        private static bool IsControllerButtonDown(InputControlType button)
        {
            foreach (InputDevice device in InputManager.Devices)
            {
                if (device == null)
                {
                    continue;
                }

                InputControl control = device.GetControl(button);
                if (control != null && control.IsPressed)
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetQuickMenuDisplayLabel(string id, string defaultLabel)
        {
            if (!IsQuickMenuRenameAllowed(id))
            {
                return defaultLabel;
            }

            Dictionary<string, string> labels = GodhomeQoL.GlobalSettings.QuickMenuCustomLabels ??= new Dictionary<string, string>();
            if (labels.TryGetValue(id, out string? custom) && !string.IsNullOrWhiteSpace(custom))
            {
                return custom;
            }

            return defaultLabel;
        }

        private static bool TryGetQuickMenuCustomLabel(string id, out string customLabel)
        {
            customLabel = string.Empty;
            if (!IsQuickMenuRenameAllowed(id))
            {
                return false;
            }

            Dictionary<string, string> labels = GodhomeQoL.GlobalSettings.QuickMenuCustomLabels ??= new Dictionary<string, string>();
            if (labels.TryGetValue(id, out string? custom) && !string.IsNullOrWhiteSpace(custom))
            {
                customLabel = custom;
                return true;
            }

            return false;
        }

        private float GetQuickRowY(int index)
        {
            return QuickButtonTop - index * (QuickButtonHeight + QuickButtonSpacing);
        }

        private QuickMenuEntry CreateQuickEntry(Transform parent, QuickMenuItemDefinition definition, float y)
        {
            GameObject rowObj = new GameObject($"{definition.Id}Row");
            rowObj.transform.SetParent(parent, false);

            RectTransform rowRect = rowObj.AddComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0f, 1f);
            rowRect.anchorMax = new Vector2(0f, 1f);
            rowRect.pivot = new Vector2(0f, 1f);
            rowRect.anchoredPosition = new Vector2(QuickButtonLeft, y);
            rowRect.sizeDelta = new Vector2(QuickRowWidth, QuickButtonHeight);

            GameObject handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(rowObj.transform, false);

            RectTransform handleRect = handleObj.AddComponent<RectTransform>();
            handleRect.anchorMin = new Vector2(0f, 0.5f);
            handleRect.anchorMax = new Vector2(0f, 0.5f);
            handleRect.pivot = new Vector2(0f, 0.5f);
            handleRect.anchoredPosition = Vector2.zero;
            handleRect.sizeDelta = new Vector2(QuickHandleWidth, QuickButtonHeight);

            Image handleImage = handleObj.AddComponent<Image>();
            handleImage.color = new Color(1f, 1f, 1f, 0f);

            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(handleObj.transform, false);

            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 0.5f);
            iconRect.anchorMax = new Vector2(0.5f, 0.5f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.sizeDelta = new Vector2(QuickHandleWidth, QuickButtonHeight);

            Sprite? handleSprite = quickHandleSprite ??= LoadQuickHandleSprite();
            if (handleSprite != null)
            {
                Image iconImage = iconObj.AddComponent<Image>();
                iconImage.sprite = handleSprite;
                iconImage.preserveAspect = true;
                iconImage.color = Color.white;
                iconImage.raycastTarget = false;
            }
            else
            {
                Text handleText = CreateText(iconObj.transform, "Label", "|||", 18, TextAnchor.MiddleCenter);
                handleText.raycastTarget = false;
                RectTransform handleTextRect = handleText.rectTransform;
                handleTextRect.anchorMin = Vector2.zero;
                handleTextRect.anchorMax = Vector2.one;
                handleTextRect.offsetMin = Vector2.zero;
                handleTextRect.offsetMax = Vector2.zero;
            }

            GameObject buttonObj = new GameObject("Button");
            buttonObj.transform.SetParent(rowObj.transform, false);

            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0f, 0.5f);
            buttonRect.anchorMax = new Vector2(0f, 0.5f);
            buttonRect.pivot = new Vector2(0f, 0.5f);
            buttonRect.anchoredPosition = new Vector2(QuickHandleWidth + QuickHandleGap, 0f);
            buttonRect.sizeDelta = new Vector2(QuickButtonWidth, QuickButtonHeight);

            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = QuickMenuBaseColor;

            GameObject fillObj = new GameObject("StateFill");
            fillObj.transform.SetParent(buttonObj.transform, false);
            RectTransform fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            Image fillImage = fillObj.AddComponent<Image>();
            fillImage.color = new Color(0f, 0f, 0f, 0f);
            fillImage.raycastTarget = false;

            Outline outline = buttonObj.AddComponent<Outline>();
            outline.effectColor = Color.white;
            outline.effectDistance = new Vector2(1f, -1f);

            Button button = buttonObj.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = buttonImage;

            string displayLabel = GetQuickMenuDisplayLabel(definition.Id, definition.Label);
            Text text = CreateText(buttonObj.transform, "Text", displayLabel, 24, TextAnchor.MiddleCenter);
            RectTransform textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            QuickMenuEntry entry = new(definition.Id, rowRect, button, text, definition.Label);
            QuickMenuDragHandle dragHandle = handleObj.AddComponent<QuickMenuDragHandle>();
            dragHandle.Owner = this;
            dragHandle.Entry = entry;
            button.onClick.AddListener(() => OnQuickEntryClicked(definition, entry));

            return entry;
        }

        private static void TryAddCollectorIcons(Transform parent, Text text, string id)
        {
            if (!string.Equals(id, "CollectorPhases", StringComparison.Ordinal))
            {
                return;
            }

            collectorIconLeftSprite ??= LoadCollectorIconSprite("25px-The_Collector_Icon RIGHT.png", "CollectorIconLeft");
            collectorIconRightSprite ??= LoadCollectorIconSprite("25px-The_Collector_Icon LEFT.png", "CollectorIconRight");

            if (collectorIconLeftSprite == null && collectorIconRightSprite == null)
            {
                return;
            }

            float offset = CalculateQuickMenuIconOffset(text, QuickCollectorIconSize, QuickButtonWidth);

            if (collectorIconLeftSprite != null)
            {
                CreateQuickMenuIcon(parent, "CollectorIconLeft", collectorIconLeftSprite, -offset, QuickCollectorIconSize);
            }

            if (collectorIconRightSprite != null)
            {
                CreateQuickMenuIcon(parent, "CollectorIconRight", collectorIconRightSprite, offset, QuickCollectorIconSize);
            }
        }

        private static void TryAddZoteIcons(Transform parent, Text text, string id)
        {
            if (!string.Equals(id, "ZoteHelper", StringComparison.Ordinal))
            {
                return;
            }

            zoteIconLeftSprite ??= LoadCollectorIconSprite("32px-Grey_Prince_Zote_Circle LEFT.png", "ZoteIconLeft");
            zoteIconRightSprite ??= LoadCollectorIconSprite("32px-Grey_Prince_Zote_Circle RIGHT.png", "ZoteIconRight");

            if (zoteIconLeftSprite == null && zoteIconRightSprite == null)
            {
                return;
            }

            float offset = CalculateQuickMenuIconOffset(text, QuickZoteIconSize, QuickButtonWidth);

            if (zoteIconLeftSprite != null)
            {
                CreateQuickMenuIcon(parent, "ZoteIconLeft", zoteIconLeftSprite, -offset, QuickZoteIconSize);
            }

            if (zoteIconRightSprite != null)
            {
                CreateQuickMenuIcon(parent, "ZoteIconRight", zoteIconRightSprite, offset, QuickZoteIconSize);
            }
        }

        private static void TryAddZoteTitleIcons(Text title)
        {
            if (title == null)
            {
                return;
            }

            zoteIconLeftSprite ??= LoadCollectorIconSprite("32px-Grey_Prince_Zote_Circle LEFT.png", "ZoteTitleIconLeft");
            zoteIconRightSprite ??= LoadCollectorIconSprite("32px-Grey_Prince_Zote_Circle RIGHT.png", "ZoteTitleIconRight");

            if (zoteIconLeftSprite == null && zoteIconRightSprite == null)
            {
                return;
            }

            float offset = CalculateQuickMenuIconOffset(title, QuickZoteIconSize, RowWidth);

            if (zoteIconLeftSprite != null)
            {
                CreateQuickMenuIcon(title.transform, "ZoteTitleIconLeft", zoteIconLeftSprite, -offset, QuickZoteIconSize);
            }

            if (zoteIconRightSprite != null)
            {
                CreateQuickMenuIcon(title.transform, "ZoteTitleIconRight", zoteIconRightSprite, offset, QuickZoteIconSize);
            }
        }

        private static float CalculateQuickMenuIconOffset(Text text, float iconSize, float rowWidth)
        {
            float textWidth = text.preferredWidth;
            if (textWidth <= 1f)
            {
                textWidth = text.text.Length * text.fontSize * 0.5f;
            }

            float halfTextWidth = textWidth * 0.5f;
            float maxOffset = rowWidth * 0.5f - iconSize * 0.5f - QuickCollectorIconPadding;
            float desiredOffset = halfTextWidth + iconSize * 0.5f + QuickCollectorIconPadding;
            float offset = Mathf.Min(desiredOffset, maxOffset);
            if (offset < iconSize * 0.5f)
            {
                offset = iconSize * 0.5f;
            }

            return offset;
        }

        private static void UpdateQuickMenuIconOffsets(Transform parent, Text text, float iconSize, string leftName, string rightName, float rowWidth)
        {
            float offset = CalculateQuickMenuIconOffset(text, iconSize, rowWidth);

            Transform? left = parent.Find(leftName);
            if (left != null && left.TryGetComponent(out RectTransform leftRect))
            {
                leftRect.anchoredPosition = new Vector2(-offset, 0f);
            }

            Transform? right = parent.Find(rightName);
            if (right != null && right.TryGetComponent(out RectTransform rightRect))
            {
                rightRect.anchoredPosition = new Vector2(offset, 0f);
            }
        }

        private void UpdateQuickMenuEntryIcons(QuickMenuEntry entry)
        {
            if (entry.Id == "CollectorPhases")
            {
                UpdateQuickMenuIconOffsets(entry.Button.transform, entry.Label, QuickCollectorIconSize, "CollectorIconLeft", "CollectorIconRight", QuickButtonWidth);
            }
            else if (entry.Id == "ZoteHelper")
            {
                UpdateQuickMenuIconOffsets(entry.Button.transform, entry.Label, QuickZoteIconSize, "ZoteIconLeft", "ZoteIconRight", QuickButtonWidth);
            }
        }

        private void UpdateQuickMenuEntryColor(string id, bool enabled)
        {
            QuickMenuEntry? entry = quickEntries.FirstOrDefault(item => item.Id == id);
            if (entry == null)
            {
                return;
            }

            Image? image = entry.Button.targetGraphic as Image;
            if (image == null)
            {
                return;
            }

            Color targetColor = enabled ? QuickMenuZoteEnabledColor : QuickMenuZoteDisabledColor;
            image.color = QuickMenuBaseColor;

            Transform? fill = entry.Button.transform.Find("StateFill");
            if (fill != null && fill.TryGetComponent(out Image fillImage))
            {
                fillImage.color = targetColor;
            }

            Outline? outline = entry.Button.GetComponent<Outline>();
            if (outline != null)
            {
                outline.effectColor = Color.white;
                outline.effectDistance = new Vector2(1f, -1f);
                outline.useGraphicAlpha = true;
                outline.enabled = true;
            }
        }

        private void UpdateQuickMenuEntryStateColors()
        {
            UpdateQuickMenuEntryColor("FastSuperDash", GetModuleEnabled());
            UpdateQuickMenuEntryColor("FastReload", GetFastReloadEnabled());
            UpdateQuickMenuEntryColor("DreamshieldSettings", GetDreamshieldEnabled());
            UpdateQuickMenuEntryColor("SpeedChanger", SpeedChanger.globalSwitch);
            UpdateQuickMenuEntryColor("TeleportKit", GetTeleportKitEnabled());
            UpdateQuickMenuEntryColor("ShowHPOnDeath", GetShowHpOnDeathEnabled());
            UpdateQuickMenuEntryColor("MaskDamage", GetMaskDamageEnabled());
            UpdateQuickMenuEntryColor("FreezeHitboxes", GetFreezeHitboxesEnabled());
            UpdateQuickMenuEntryColor("AlwaysFurious", GetAlwaysFuriousEnabled());
            UpdateQuickMenuEntryColor("GearSwitcher", GetGearSwitcherEnabled());
            UpdateQuickMenuEntryColor("ZoteHelper", GetZoteHelperEnabled());
            UpdateQuickMenuEntryColor("CollectorPhases", GetCollectorPhasesEnabled());
            UpdateQuickMenuEntryColor("BossChallenge", GetBossChallengeMasterEnabled());
            UpdateQuickMenuEntryColor("RandomPantheons", GetRandomPantheonsMasterEnabled());
            UpdateQuickMenuEntryColor("TrueBossRush", GetTrueBossRushMasterEnabled());
            UpdateQuickMenuEntryColor("Cheats", GetCheatsMasterEnabled());
            UpdateQuickMenuEntryColor("QualityOfLife", GetQolMasterEnabled());
            UpdateQuickMenuEntryColor("BossAnimationSkipping", GetBossAnimationMasterEnabled());
            UpdateQuickMenuEntryColor("MenuAnimationSkipping", GetMenuAnimationMasterEnabled());
        }

        internal void RefreshQuickMenuEntryColors()
        {
            UpdateQuickMenuEntryStateColors();
        }

    }
}
