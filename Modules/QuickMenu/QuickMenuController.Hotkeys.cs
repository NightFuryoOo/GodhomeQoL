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
    {        private bool IsFastReloadRebinding()
        {
            return waitingForReloadRebind;
        }

        private bool IsAnyRebinding()
        {
            return IsFastReloadRebinding()
                || IsShowHpOnDeathRebinding()
                || IsMaskDamageRebinding()
                || IsFreezeHitboxesRebinding()
                || IsSpeedChangerRebinding()
                || IsTeleportKitRebinding()
                || IsFastDreamWarpRebinding()
                || renameField != null
                || gearSwitcherPresetRenameField != null
                || waitingForQuickMenuHotkeyRebind
                || waitingForOverlayHotkeyRebind;
        }

        internal bool IsHotkeyInputBlocked()
        {
            return quickRenameMode
                || IsAnyRebinding()
                || gearSwitcherPresetDeleteVisible
                || gearSwitcherResetConfirmVisible
                || quickSettingsResetConfirmVisible;
        }

        internal void StartQuickMenuHotkeyRebind()
        {
            waitingForQuickMenuHotkeyRebind = true;
            quickMenuPrevKey = GetQuickMenuToggleKey();
            UpdateQuickMenuHotkeyButton("Settings/FastReload/SetKey".Localize());
        }

        private void StartOverlayHotkeyRebind(string id)
        {
            waitingForOverlayHotkeyRebind = true;
            overlayHotkeyRebindId = id;
            overlayHotkeyPrevKey = GetOverlayHotkeyKey(id);
            overlayHotkeyPrevButton = GetOverlayHotkeyControllerBinding(id);
            if (quickSettingsHotkeyValues.TryGetValue(id, out Text? text))
            {
                UpdateKeybindValue(text, "Settings/FastReload/SetKey".Localize());
            }
        }

        private void CancelOverlayHotkeyRebind()
        {
            if (!waitingForOverlayHotkeyRebind)
            {
                return;
            }

            waitingForOverlayHotkeyRebind = false;
            if (quickSettingsHotkeyValues.TryGetValue(overlayHotkeyRebindId, out Text? text))
            {
                UpdateKeybindValue(text, GetOverlayHotkeyLabel(overlayHotkeyRebindId));
            }

            overlayHotkeyRebindId = string.Empty;
            overlayHotkeyPrevKey = KeyCode.None;
            overlayHotkeyPrevButton = default;
        }

        private void StartReloadRebind()
        {
            waitingForReloadRebind = true;
            reloadPrevKey = GetReloadKey();
            UpdateKeybindValue(reloadKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void CancelFastReloadRebind()
        {
            if (waitingForReloadRebind)
            {
                waitingForReloadRebind = false;
                UpdateKeybindValue(reloadKeyValue, FormatKeyLabel(GetReloadKey()));
            }
        }

        private void HandleQuickMenuHotkeyRebind()
        {
            if (!waitingForQuickMenuHotkeyRebind)
            {
                return;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key) || key == KeyCode.None)
                {
                    continue;
                }

                if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
                {
                    continue;
                }

                ApplyQuickMenuHotkeyRebind(key);
                return;
            }
        }

        private void HandleOverlayHotkeyRebind()
        {
            if (!waitingForOverlayHotkeyRebind)
            {
                return;
            }

            if (TryGetPressedControllerButton(out InputControlType button))
            {
                ApplyOverlayHotkeyControllerRebind(button);
                return;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key) || key == KeyCode.None)
                {
                    continue;
                }

                if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
                {
                    continue;
                }

                ApplyOverlayHotkeyRebind(key);
                return;
            }
        }

        private void ApplyOverlayHotkeyRebind(KeyCode key)
        {
            if (string.IsNullOrEmpty(overlayHotkeyRebindId))
            {
                waitingForOverlayHotkeyRebind = false;
                return;
            }

            if (key == KeyCode.Escape)
            {
                waitingForOverlayHotkeyRebind = false;
                if (quickSettingsHotkeyValues.TryGetValue(overlayHotkeyRebindId, out Text? text))
                {
                    UpdateKeybindValue(text, GetOverlayHotkeyLabel(overlayHotkeyRebindId));
                }
                overlayHotkeyRebindId = string.Empty;
                overlayHotkeyPrevKey = KeyCode.None;
                return;
            }

            bool isQuickMenuKey = key == GetQuickMenuToggleKey();
            string raw = key == overlayHotkeyPrevKey
                || IsOverlayKeyboardHotkeyInUse(overlayHotkeyRebindId, key)
                || isQuickMenuKey
                ? string.Empty
                : key.ToString();
            SetOverlayHotkeyRaw(overlayHotkeyRebindId, raw);
            GodhomeQoL.SaveGlobalSettingsSafe();
            GodhomeQoL.MarkMenuDirty();
            SetOverlayHotkeySuppress(raw, key, default);
            overlayHotkeySuppressFrames = 2;

            waitingForOverlayHotkeyRebind = false;
            if (quickSettingsHotkeyValues.TryGetValue(overlayHotkeyRebindId, out Text? valueText))
            {
                UpdateKeybindValue(valueText, GetOverlayHotkeyLabel(overlayHotkeyRebindId));
            }

            overlayHotkeyRebindId = string.Empty;
            overlayHotkeyPrevKey = KeyCode.None;
            overlayHotkeyPrevButton = default;
        }

        private void ApplyOverlayHotkeyControllerRebind(InputControlType button)
        {
            if (string.IsNullOrEmpty(overlayHotkeyRebindId))
            {
                waitingForOverlayHotkeyRebind = false;
                return;
            }

            bool clear = !EqualityComparer<InputControlType>.Default.Equals(overlayHotkeyPrevButton, default)
                && EqualityComparer<InputControlType>.Default.Equals(overlayHotkeyPrevButton, button);

            string raw = clear || IsOverlayControllerHotkeyInUse(overlayHotkeyRebindId, button)
                ? string.Empty
                : $"{OverlayControllerPrefix}{button}";

            SetOverlayHotkeyRaw(overlayHotkeyRebindId, raw);
            GodhomeQoL.SaveGlobalSettingsSafe();
            GodhomeQoL.MarkMenuDirty();
            SetOverlayHotkeySuppress(raw, KeyCode.None, button);
            overlayHotkeySuppressFrames = 2;

            waitingForOverlayHotkeyRebind = false;
            if (quickSettingsHotkeyValues.TryGetValue(overlayHotkeyRebindId, out Text? valueText))
            {
                UpdateKeybindValue(valueText, GetOverlayHotkeyLabel(overlayHotkeyRebindId));
            }

            overlayHotkeyRebindId = string.Empty;
            overlayHotkeyPrevKey = KeyCode.None;
            overlayHotkeyPrevButton = default;
        }

        private void ApplyQuickMenuHotkeyRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForQuickMenuHotkeyRebind = false;
                UpdateQuickMenuHotkeyButton(GetQuickMenuHotkeyLabel());
                return;
            }

            bool hasOverlayConflict = IsOverlayKeyboardHotkeyInUse(string.Empty, key);
            GodhomeQoL.GlobalSettings.QuickMenuHotkey = key == quickMenuPrevKey || hasOverlayConflict
                ? string.Empty
                : key.ToString();
            GodhomeQoL.SaveGlobalSettingsSafe();
            GodhomeQoL.MarkMenuDirty();
            overlayHotkeySuppressFrames = 2;

            waitingForQuickMenuHotkeyRebind = false;
            UpdateQuickMenuHotkeyButton(GetQuickMenuHotkeyLabel());
        }

        private void HandleFastReloadRebind()
        {
            if (!IsFastReloadRebinding())
            {
                return;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key) || key == KeyCode.None)
                {
                    continue;
                }

                if (key == KeyCode.Mouse0)
                {
                    continue;
                }

                if (waitingForReloadRebind)
                {
                    ApplyReloadRebind(key);
                    return;
                }
            }
        }

        private void ApplyReloadRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForReloadRebind = false;
                UpdateKeybindValue(reloadKeyValue, FormatKeyLabel(GetReloadKey()));
                return;
            }

            Modules.FastReload.reloadKeyCode = key == reloadPrevKey || IsFastReloadKeyInUse(key, forReload: true)
                ? (int)KeyCode.None
                : (int)key;

            waitingForReloadRebind = false;
            UpdateKeybindValue(reloadKeyValue, FormatKeyLabel(GetReloadKey()));
        }

        private static bool IsFastReloadKeyInUse(KeyCode key, bool forReload)
        {
            if (key == KeyCode.None)
            {
                return false;
            }

            KeyCode other = forReload ? GetTeleportKey() : GetReloadKey();
            return key == other;
        }

        private static KeyCode GetReloadKey() => (KeyCode)Modules.FastReload.reloadKeyCode;

        private static KeyCode GetTeleportKey() => (KeyCode)Modules.FastReload.teleportKeyCode;

        private static string FormatKeyLabel(KeyCode key)
        {
            return key == KeyCode.None
                ? "Settings/FastReload/NotSet".Localize()
                : key.ToString();
        }

        private string GetMaskDamageToggleUiKeyLabel()
        {
            return FormatMaskDamageKeyLabel(MaskDamage.GetToggleUiKeybind());
        }

        private string GetFreezeHitboxesKeyLabel()
        {
            return FormatMaskDamageKeyLabel(FreezeHitboxes.GetUnfreezeKeybind());
        }

        private static string FormatMaskDamageKeyLabel(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return "Settings/FastReload/NotSet".Localize();
            }

            if (Enum.TryParse(raw, true, out KeyCode key))
            {
                return FormatKeyLabel(key);
            }

            return raw;
        }

        private void UpdateKeybindValue(Text? text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        private bool IsShowHpOnDeathRebinding()
        {
            return waitingForShowHpRebind;
        }

        private void StartShowHpOnDeathRebind()
        {
            waitingForShowHpRebind = true;
            showHpPrevBindingRaw = GetShowHpBindingRaw();
            UpdateKeybindValue(showHpHudToggleKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void CancelShowHpOnDeathRebind()
        {
            if (!waitingForShowHpRebind)
            {
                return;
            }

            waitingForShowHpRebind = false;
            UpdateKeybindValue(showHpHudToggleKeyValue, GetShowHpBindingLabel());
        }

        private void HandleShowHpOnDeathRebind()
        {
            if (!waitingForShowHpRebind)
            {
                return;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key) || key == KeyCode.None)
                {
                    continue;
                }

                if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
                {
                    continue;
                }

                ApplyShowHpOnDeathRebind(key);
                return;
            }
        }

        private void ApplyShowHpOnDeathRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForShowHpRebind = false;
                UpdateKeybindValue(showHpHudToggleKeyValue, GetShowHpBindingLabel());
                return;
            }

            if (!TryMapKeyCodeToInControlKey(key, out Key mapped))
            {
                return;
            }

            string newLabel = mapped.ToString();
            bool clear = !string.IsNullOrEmpty(showHpPrevBindingRaw)
                && string.Equals(showHpPrevBindingRaw, newLabel, StringComparison.Ordinal);

            ApplyShowHpBinding(clear ? (Key?)null : mapped);
            waitingForShowHpRebind = false;
            UpdateKeybindValue(showHpHudToggleKeyValue, GetShowHpBindingLabel());
        }

        private static void ApplyShowHpBinding(Key? key)
        {
            PlayerAction action = ShowHpSettings.Keybinds.Hide;
            InputControlType controllerBinding = KeybindUtil.GetControllerButtonBinding(action);

            action.ClearBindings();

            if (!EqualityComparer<InputControlType>.Default.Equals(controllerBinding, default))
            {
                KeybindUtil.AddInputControlType(action, controllerBinding);
            }

            if (key.HasValue && !EqualityComparer<Key>.Default.Equals(key.Value, default))
            {
                KeybindUtil.AddKeyOrMouseBinding(action, new InputHandler.KeyOrMouseBinding(key.Value));
            }
        }

        private string GetShowHpBindingLabel()
        {
            string raw = GetShowHpBindingRaw();
            return string.IsNullOrEmpty(raw) ? "Settings/FastReload/NotSet".Localize() : raw;
        }

        private static string GetShowHpBindingRaw()
        {
            InputHandler.KeyOrMouseBinding binding = KeybindUtil.GetKeyOrMouseBinding(ShowHpSettings.Keybinds.Hide);
            return FormatKeyOrMouseBinding(binding);
        }

        private bool IsMaskDamageRebinding()
        {
            return waitingForMaskDamageUiRebind;
        }

        private void StartMaskDamageUiRebind()
        {
            waitingForMaskDamageUiRebind = true;
            maskDamageUiPrevKey = MaskDamage.GetToggleUiKeybind();
            UpdateKeybindValue(maskDamageToggleUiKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void CancelMaskDamageRebind()
        {
            if (!waitingForMaskDamageUiRebind)
            {
                return;
            }

            waitingForMaskDamageUiRebind = false;
            UpdateKeybindValue(maskDamageToggleUiKeyValue, GetMaskDamageToggleUiKeyLabel());
        }

        private void HandleMaskDamageUiRebind()
        {
            if (!waitingForMaskDamageUiRebind)
            {
                return;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key) || key == KeyCode.None)
                {
                    continue;
                }

                if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
                {
                    continue;
                }

                ApplyMaskDamageUiRebind(key);
                return;
            }
        }

        private void ApplyMaskDamageUiRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForMaskDamageUiRebind = false;
                UpdateKeybindValue(maskDamageToggleUiKeyValue, GetMaskDamageToggleUiKeyLabel());
                return;
            }

            string keyName = key.ToString();
            bool clear = !string.IsNullOrEmpty(maskDamageUiPrevKey)
                && string.Equals(maskDamageUiPrevKey, keyName, StringComparison.OrdinalIgnoreCase);

            MaskDamage.SetToggleUiKeybind(clear ? string.Empty : keyName);
            waitingForMaskDamageUiRebind = false;
            UpdateKeybindValue(maskDamageToggleUiKeyValue, GetMaskDamageToggleUiKeyLabel());
        }

        private bool IsFreezeHitboxesRebinding()
        {
            return waitingForFreezeHitboxesRebind;
        }

        private void StartFreezeHitboxesRebind()
        {
            waitingForFreezeHitboxesRebind = true;
            freezeHitboxesPrevKey = FreezeHitboxes.GetUnfreezeKeybind();
            UpdateKeybindValue(freezeHitboxesUnfreezeKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void CancelFreezeHitboxesRebind()
        {
            if (!waitingForFreezeHitboxesRebind)
            {
                return;
            }

            waitingForFreezeHitboxesRebind = false;
            UpdateKeybindValue(freezeHitboxesUnfreezeKeyValue, GetFreezeHitboxesKeyLabel());
        }

        private void HandleFreezeHitboxesRebind()
        {
            if (!waitingForFreezeHitboxesRebind)
            {
                return;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key) || key == KeyCode.None)
                {
                    continue;
                }

                if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
                {
                    continue;
                }

                ApplyFreezeHitboxesRebind(key);
                return;
            }
        }

        private void ApplyFreezeHitboxesRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForFreezeHitboxesRebind = false;
                UpdateKeybindValue(freezeHitboxesUnfreezeKeyValue, GetFreezeHitboxesKeyLabel());
                return;
            }

            string keyName = key.ToString();
            bool clear = !string.IsNullOrEmpty(freezeHitboxesPrevKey)
                && string.Equals(freezeHitboxesPrevKey, keyName, StringComparison.OrdinalIgnoreCase);

            FreezeHitboxes.SetUnfreezeKeybind(clear ? string.Empty : keyName);
            waitingForFreezeHitboxesRebind = false;
            UpdateKeybindValue(freezeHitboxesUnfreezeKeyValue, GetFreezeHitboxesKeyLabel());
        }

        private static string FormatKeyOrMouseBinding(InputHandler.KeyOrMouseBinding binding)
        {
            if (TryGetBindingKey(binding, out Key key)
                && !EqualityComparer<Key>.Default.Equals(key, default))
            {
                return key.ToString();
            }

            if (TryGetBindingMouse(binding, out Mouse mouse)
                && !EqualityComparer<Mouse>.Default.Equals(mouse, default))
            {
                return mouse.ToString();
            }

            return string.Empty;
        }

        private static bool TryGetBindingKey(InputHandler.KeyOrMouseBinding binding, out Key key)
        {
            object boxed = binding;
            Type type = boxed.GetType();

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.PropertyType == typeof(Key))
                {
                    key = (Key)property.GetValue(boxed);
                    return true;
                }
            }

            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (field.FieldType == typeof(Key))
                {
                    key = (Key)field.GetValue(boxed);
                    return true;
                }
            }

            key = default;
            return false;
        }

        private static bool TryGetBindingMouse(InputHandler.KeyOrMouseBinding binding, out Mouse mouse)
        {
            object boxed = binding;
            Type type = boxed.GetType();

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.PropertyType == typeof(Mouse))
                {
                    mouse = (Mouse)property.GetValue(boxed);
                    return true;
                }
            }

            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (field.FieldType == typeof(Mouse))
                {
                    mouse = (Mouse)field.GetValue(boxed);
                    return true;
                }
            }

            mouse = default;
            return false;
        }

        private static bool TryMapKeyCodeToInControlKey(KeyCode keyCode, out Key key)
        {
            key = default;
            if (keyCode == KeyCode.None)
            {
                return false;
            }

            if (keyCode >= KeyCode.Alpha0 && keyCode <= KeyCode.Alpha9)
            {
                int digit = (int)keyCode - (int)KeyCode.Alpha0;
                return Enum.TryParse($"Key{digit}", out key);
            }

            if (keyCode >= KeyCode.Keypad0 && keyCode <= KeyCode.Keypad9)
            {
                int digit = (int)keyCode - (int)KeyCode.Keypad0;
                return Enum.TryParse($"Pad{digit}", out key);
            }

            if (keyCode == KeyCode.KeypadEnter)
            {
                if (Enum.TryParse("PadEnter", out key))
                {
                    return true;
                }

                return Enum.TryParse("KeypadEnter", out key);
            }

            return Enum.TryParse(keyCode.ToString(), out key);
        }

        private bool IsSpeedChangerRebinding()
        {
            return waitingForSpeedToggleRebind || waitingForSpeedInputRebind;
        }

        private void StartSpeedChangerToggleRebind()
        {
            waitingForSpeedToggleRebind = true;
            speedTogglePrevKey = SpeedChanger.toggleKeybind;
            UpdateKeybindValue(speedChangerToggleKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void StartSpeedChangerInputRebind()
        {
            waitingForSpeedInputRebind = true;
            speedInputPrevKey = SpeedChanger.inputSpeedKeybind;
            UpdateKeybindValue(speedChangerInputKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void CancelSpeedChangerRebind()
        {
            if (waitingForSpeedToggleRebind)
            {
                waitingForSpeedToggleRebind = false;
                UpdateKeybindValue(speedChangerToggleKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.toggleKeybind));
            }

            if (waitingForSpeedInputRebind)
            {
                waitingForSpeedInputRebind = false;
                UpdateKeybindValue(speedChangerInputKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.inputSpeedKeybind));
            }
        }

        private void HandleSpeedChangerRebind()
        {
            if (!IsSpeedChangerRebinding())
            {
                return;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key) || key == KeyCode.None)
                {
                    continue;
                }

                if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
                {
                    continue;
                }

                if (waitingForSpeedToggleRebind)
                {
                    ApplySpeedChangerToggleRebind(key);
                    return;
                }

                if (waitingForSpeedInputRebind)
                {
                    ApplySpeedChangerInputRebind(key);
                    return;
                }

            }
        }

        private void ApplySpeedChangerToggleRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForSpeedToggleRebind = false;
                UpdateKeybindValue(speedChangerToggleKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.toggleKeybind));
                return;
            }

            SetSpeedChangerKeybind(key, speedTogglePrevKey, "toggle", value => SpeedChanger.toggleKeybind = value);
            SpeedChanger.SuppressToggleUntilRelease(key);
            waitingForSpeedToggleRebind = false;
            UpdateKeybindValue(speedChangerToggleKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.toggleKeybind));
        }

        private void ApplySpeedChangerInputRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForSpeedInputRebind = false;
                UpdateKeybindValue(speedChangerInputKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.inputSpeedKeybind));
                return;
            }

            SetSpeedChangerKeybind(key, speedInputPrevKey, "input", value => SpeedChanger.inputSpeedKeybind = value);
            SpeedChanger.SuppressInputUntilRelease(key);
            waitingForSpeedInputRebind = false;
            UpdateKeybindValue(speedChangerInputKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.inputSpeedKeybind));
        }

        private void SetSpeedChangerKeybind(KeyCode key, string previous, string slot, Action<string> setter)
        {
            string keyName = key.ToString();
            bool clear = string.Equals(previous, keyName, StringComparison.OrdinalIgnoreCase)
                || IsSpeedChangerKeyInUse(keyName, slot);

            setter(clear ? string.Empty : keyName);
            RefreshSpeedChangerKeybinds();
            GodhomeQoL.SaveGlobalSettingsSafe();
        }

        private static bool IsSpeedChangerKeyInUse(string keyName, string except)
        {
            if (string.IsNullOrWhiteSpace(keyName))
            {
                return false;
            }

            bool inToggle = !string.Equals(except, "toggle", StringComparison.OrdinalIgnoreCase)
                && string.Equals(SpeedChanger.toggleKeybind, keyName, StringComparison.OrdinalIgnoreCase);
            bool inInput = !string.Equals(except, "input", StringComparison.OrdinalIgnoreCase)
                && string.Equals(SpeedChanger.inputSpeedKeybind, keyName, StringComparison.OrdinalIgnoreCase);
            bool inUp = !string.Equals(except, "up", StringComparison.OrdinalIgnoreCase)
                && string.Equals(SpeedChanger.speedUpKeybind, keyName, StringComparison.OrdinalIgnoreCase);
            bool inDown = !string.Equals(except, "down", StringComparison.OrdinalIgnoreCase)
                && string.Equals(SpeedChanger.slowDownKeybind, keyName, StringComparison.OrdinalIgnoreCase);

            return inToggle || inInput || inUp || inDown;
        }

        private void RefreshSpeedChangerKeybinds()
        {
            SpeedChanger? module = GetSpeedChangerModule();
            if (module == null)
            {
                return;
            }

            MethodInfo? method = typeof(SpeedChanger).GetMethod("RefreshKeybinds", BindingFlags.Instance | BindingFlags.NonPublic);
            method?.Invoke(module, null);
        }

        private static string FormatSpeedChangerKeyLabel(string storedKey)
        {
            if (string.IsNullOrWhiteSpace(storedKey) || storedKey.Equals("Not Set", StringComparison.OrdinalIgnoreCase))
            {
                return "SpeedChanger/NotSet".Localize();
            }

            return storedKey;
        }

        private static string GetSpeedChangerDisplayValue()
        {
            int index = ClampOptionIndex(SpeedChanger.displayStyle, SpeedChangerDisplayOptions.Length);
            return SpeedChangerDisplayOptions[index];
        }

        private bool IsTeleportKitRebinding()
        {
            return waitingForTeleportKitMenuRebind || waitingForTeleportKitSaveRebind || waitingForTeleportKitTeleportRebind;
        }

        private void StartTeleportKitMenuRebind()
        {
            waitingForTeleportKitMenuRebind = true;
            UpdateKeybindValue(teleportKitMenuKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void StartTeleportKitSaveRebind()
        {
            waitingForTeleportKitSaveRebind = true;
            UpdateKeybindValue(teleportKitSaveKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void StartTeleportKitTeleportRebind()
        {
            waitingForTeleportKitTeleportRebind = true;
            UpdateKeybindValue(teleportKitTeleportKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void CancelTeleportKitRebind()
        {
            if (waitingForTeleportKitMenuRebind)
            {
                waitingForTeleportKitMenuRebind = false;
                UpdateKeybindValue(teleportKitMenuKeyValue, GetTeleportKitMenuKeyLabel());
            }

            if (waitingForTeleportKitSaveRebind)
            {
                waitingForTeleportKitSaveRebind = false;
                UpdateKeybindValue(teleportKitSaveKeyValue, GetTeleportKitSaveKeyLabel());
            }

            if (waitingForTeleportKitTeleportRebind)
            {
                waitingForTeleportKitTeleportRebind = false;
                UpdateKeybindValue(teleportKitTeleportKeyValue, GetTeleportKitTeleportKeyLabel());
            }
        }

        private void HandleTeleportKitRebind()
        {
            if (!IsTeleportKitRebinding())
            {
                return;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key) || key == KeyCode.None)
                {
                    continue;
                }

                if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
                {
                    continue;
                }

                if (waitingForTeleportKitMenuRebind)
                {
                    ApplyTeleportKitMenuRebind(key);
                    return;
                }

                if (waitingForTeleportKitSaveRebind)
                {
                    ApplyTeleportKitSaveRebind(key);
                    return;
                }

                if (waitingForTeleportKitTeleportRebind)
                {
                    ApplyTeleportKitTeleportRebind(key);
                    return;
                }
            }
        }

        private void ApplyTeleportKitMenuRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForTeleportKitMenuRebind = false;
                UpdateKeybindValue(teleportKitMenuKeyValue, GetTeleportKitMenuKeyLabel());
                return;
            }

            Modules.QoL.TeleportKit.MenuHotkey = key;
            waitingForTeleportKitMenuRebind = false;
            UpdateKeybindValue(teleportKitMenuKeyValue, GetTeleportKitMenuKeyLabel());
        }

        private void ApplyTeleportKitSaveRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForTeleportKitSaveRebind = false;
                UpdateKeybindValue(teleportKitSaveKeyValue, GetTeleportKitSaveKeyLabel());
                return;
            }

            Modules.QoL.TeleportKit.SaveTeleportHotkey = key;
            waitingForTeleportKitSaveRebind = false;
            UpdateKeybindValue(teleportKitSaveKeyValue, GetTeleportKitSaveKeyLabel());
        }

        private void ApplyTeleportKitTeleportRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForTeleportKitTeleportRebind = false;
                UpdateKeybindValue(teleportKitTeleportKeyValue, GetTeleportKitTeleportKeyLabel());
                return;
            }

            Modules.QoL.TeleportKit.TeleportHotkey = key;
            waitingForTeleportKitTeleportRebind = false;
            UpdateKeybindValue(teleportKitTeleportKeyValue, GetTeleportKitTeleportKeyLabel());
        }

        private bool IsFastDreamWarpRebinding()
        {
            return waitingForFastDreamWarpRebind;
        }

        private void StartFastDreamWarpRebind()
        {
            waitingForFastDreamWarpRebind = true;
            fastDreamWarpPrevKeyRaw = GetFastDreamWarpKeyBindingRaw();
            fastDreamWarpPrevButton = GetFastDreamWarpControllerBinding();
            UpdateKeybindValue(fastDreamWarpKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void CancelFastDreamWarpRebind()
        {
            if (!waitingForFastDreamWarpRebind)
            {
                return;
            }

            waitingForFastDreamWarpRebind = false;
            UpdateKeybindValue(fastDreamWarpKeyValue, GetFastDreamWarpBindingLabel());
        }

        private void HandleFastDreamWarpRebind()
        {
            if (!waitingForFastDreamWarpRebind)
            {
                return;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key) || key == KeyCode.None)
                {
                    continue;
                }

                if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
                {
                    continue;
                }

                ApplyFastDreamWarpKeyRebind(key);
                return;
            }

            if (TryGetPressedControllerButton(out InputControlType button))
            {
                ApplyFastDreamWarpControllerRebind(button);
            }
        }

        private void ApplyFastDreamWarpKeyRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForFastDreamWarpRebind = false;
                UpdateKeybindValue(fastDreamWarpKeyValue, GetFastDreamWarpBindingLabel());
                return;
            }

            if (!TryMapKeyCodeToInControlKey(key, out Key mapped))
            {
                return;
            }

            string newLabel = mapped.ToString();
            bool clear = !string.IsNullOrEmpty(fastDreamWarpPrevKeyRaw)
                && string.Equals(fastDreamWarpPrevKeyRaw, newLabel, StringComparison.Ordinal);

            ApplyFastDreamWarpKeyBinding(clear ? (Key?)null : mapped);
            waitingForFastDreamWarpRebind = false;
            UpdateKeybindValue(fastDreamWarpKeyValue, GetFastDreamWarpBindingLabel());
        }

        private void ApplyFastDreamWarpControllerRebind(InputControlType button)
        {
            bool clear = !EqualityComparer<InputControlType>.Default.Equals(fastDreamWarpPrevButton, default)
                && EqualityComparer<InputControlType>.Default.Equals(fastDreamWarpPrevButton, button);

            ApplyFastDreamWarpControllerBinding(clear ? (InputControlType?)null : button);
            waitingForFastDreamWarpRebind = false;
            UpdateKeybindValue(fastDreamWarpKeyValue, GetFastDreamWarpBindingLabel());
        }

        private void HandleFastDreamWarpHotkey()
        {
            PlayerAction action = FastDreamWarpSettings.Keybinds.Toggle;
            if (!action.WasPressed)
            {
                return;
            }

            bool newValue = !GetFastDreamWarpEnabled();
            SetFastDreamWarpEnabled(newValue);
            UpdateToggleValue(bossAnimFastDreamWarpValue, newValue);
            string state = newValue ? "ON" : "OFF";
            string label = "Modules/FastDreamWarp".Localize();
            ShowStatusMessage($"{label}: {state}");
        }

        private void HandleOverlayHotkeys()
        {
            if (IsOverlayHotkeySuppressed())
            {
                return;
            }

            foreach (QuickMenuItemDefinition def in GetOrderedQuickMenuDefinitions())
            {
                if (!IsOverlayHotkeySupported(def.Id))
                {
                    continue;
                }

                InputControlType button = GetOverlayHotkeyControllerBinding(def.Id);
                if (!EqualityComparer<InputControlType>.Default.Equals(button, default) && IsControllerButtonPressed(button))
                {
                    ToggleOverlayFromHotkey(def.Id);
                    break;
                }

                KeyCode key = GetOverlayHotkeyKey(def.Id);
                if (key == KeyCode.None)
                {
                    continue;
                }

                if (Input.GetKeyDown(key))
                {
                    ToggleOverlayFromHotkey(def.Id);
                    break;
                }
            }
        }

        private bool IsOverlayHotkeyEnabled(string id)
        {
            return id switch
            {
                "FastSuperDash" => GetModuleEnabled(),
                "CollectorPhases" => GetCollectorPhasesEnabled(),
                "FastReload" => GetFastReloadEnabled(),
                "DreamshieldSettings" => GetDreamshieldEnabled(),
                "ShowHPOnDeath" => GetShowHpOnDeathEnabled(),
                "MaskDamage" => GetMaskDamageEnabled(),
                "FreezeHitboxes" => GetFreezeHitboxesEnabled(),
                "SpeedChanger" => GetSpeedChangerEnabled(),
                "TeleportKit" => GetTeleportKitEnabled(),
                "BossChallenge" => GetBossChallengeMasterEnabled(),
                "RandomPantheons" => GetRandomPantheonsMasterEnabled(),
                "AlwaysFurious" => GetAlwaysFuriousEnabled(),
                "GearSwitcher" => GetGearSwitcherEnabled(),
                "ZoteHelper" => GetZoteHelperEnabled(),
                "QualityOfLife" => GetQolMasterEnabled(),
                "BossAnimationSkipping" => GetBossAnimationMasterEnabled(),
                "MenuAnimationSkipping" => GetMenuAnimationMasterEnabled(),
                _ => false
            };
        }

        private bool IsOverlayVisible(string id)
        {
            return id switch
            {
                "FastSuperDash" => overlayVisible,
                "CollectorPhases" => collectorVisible,
                "FastReload" => fastReloadVisible,
                "DreamshieldSettings" => dreamshieldVisible,
                "ShowHPOnDeath" => showHpOnDeathVisible,
                "MaskDamage" => maskDamageVisible,
                "FreezeHitboxes" => freezeHitboxesVisible,
                "SpeedChanger" => speedChangerVisible,
                "TeleportKit" => teleportKitVisible,
                "BossChallenge" => bossChallengeVisible,
                "RandomPantheons" => randomPantheonsVisible,
                "AlwaysFurious" => alwaysFuriousVisible,
                "GearSwitcher" => gearSwitcherVisible || gearSwitcherCharmCostVisible || gearSwitcherPresetVisible,
                "ZoteHelper" => zoteHelperVisible,
                "QualityOfLife" => qolVisible,
                "BossAnimationSkipping" => bossAnimationVisible,
                "MenuAnimationSkipping" => menuAnimationVisible,
                _ => false
            };
        }

        private void ToggleOverlayFromHotkey(string id)
        {
            if (IsOverlayVisible(id))
            {
                CloseAllOverlaysForHotkey();
                return;
            }

            if (!IsOverlayHotkeyEnabled(id))
            {
                return;
            }

            returnToQuickOnClose = false;
            returnToQolOnClose = false;

            SetQuickVisible(false);
            SetQuickSettingsVisible(false);
            CloseAllOverlaysForHotkey();

            switch (id)
            {
                case "FastSuperDash":
                    SetOverlayVisible(true);
                    break;
                case "CollectorPhases":
                    SetCollectorVisible(true);
                    break;
                case "FastReload":
                    SetFastReloadVisible(true);
                    break;
                case "DreamshieldSettings":
                    SetDreamshieldVisible(true);
                    break;
                case "ShowHPOnDeath":
                    SetShowHpOnDeathVisible(true);
                    break;
                case "MaskDamage":
                    SetMaskDamageVisible(true);
                    break;
                case "FreezeHitboxes":
                    SetFreezeHitboxesVisible(true);
                    break;
                case "SpeedChanger":
                    SetSpeedChangerVisible(true);
                    break;
                case "TeleportKit":
                    SetTeleportKitVisible(true);
                    break;
                case "BossChallenge":
                    SetBossChallengeVisible(true);
                    break;
                case "RandomPantheons":
                    SetRandomPantheonsVisible(true);
                    break;
                case "AlwaysFurious":
                    SetAlwaysFuriousVisible(true);
                    break;
                case "GearSwitcher":
                    SetGearSwitcherVisible(true);
                    break;
                case "ZoteHelper":
                    SetZoteHelperVisible(true);
                    break;
                case "QualityOfLife":
                    SetQolVisible(true);
                    break;
                case "BossAnimationSkipping":
                    SetBossAnimationVisible(true);
                    break;
                case "MenuAnimationSkipping":
                    SetMenuAnimationVisible(true);
                    break;
            }
        }

        private void CloseAllOverlaysForHotkey()
        {
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetRandomPantheonsVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetGearSwitcherCharmCostVisible(false);
            SetGearSwitcherPresetVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
        }

        private static void ApplyFastDreamWarpKeyBinding(Key? key)
        {
            PlayerAction action = FastDreamWarpSettings.Keybinds.Toggle;
            InputControlType controllerBinding = KeybindUtil.GetControllerButtonBinding(action);

            action.ClearBindings();

            if (!EqualityComparer<InputControlType>.Default.Equals(controllerBinding, default))
            {
                KeybindUtil.AddInputControlType(action, controllerBinding);
            }

            if (key.HasValue && !EqualityComparer<Key>.Default.Equals(key.Value, default))
            {
                KeybindUtil.AddKeyOrMouseBinding(action, new InputHandler.KeyOrMouseBinding(key.Value));
            }
        }

        private static void ApplyFastDreamWarpControllerBinding(InputControlType? button)
        {
            PlayerAction action = FastDreamWarpSettings.Keybinds.Toggle;
            Key? key = GetFastDreamWarpKeyBinding();

            action.ClearBindings();

            if (button.HasValue && !EqualityComparer<InputControlType>.Default.Equals(button.Value, default))
            {
                KeybindUtil.AddInputControlType(action, button.Value);
            }

            if (key.HasValue && !EqualityComparer<Key>.Default.Equals(key.Value, default))
            {
                KeybindUtil.AddKeyOrMouseBinding(action, new InputHandler.KeyOrMouseBinding(key.Value));
            }
        }

        private string GetFastDreamWarpBindingLabel()
        {
            string keyLabel = GetFastDreamWarpKeyBindingRaw();
            string controllerLabel = GetFastDreamWarpControllerBindingLabel();

            if (string.IsNullOrEmpty(keyLabel) && string.IsNullOrEmpty(controllerLabel))
            {
                return "Settings/FastReload/NotSet".Localize();
            }

            if (string.IsNullOrEmpty(keyLabel))
            {
                return controllerLabel;
            }

            if (string.IsNullOrEmpty(controllerLabel))
            {
                return keyLabel;
            }

            return $"{keyLabel} / {controllerLabel}";
        }

        private static string GetFastDreamWarpKeyBindingRaw()
        {
            InputHandler.KeyOrMouseBinding binding = KeybindUtil.GetKeyOrMouseBinding(FastDreamWarpSettings.Keybinds.Toggle);
            return FormatKeyOrMouseBinding(binding);
        }

        private static InputControlType GetFastDreamWarpControllerBinding()
        {
            return KeybindUtil.GetControllerButtonBinding(FastDreamWarpSettings.Keybinds.Toggle);
        }

        private static string GetFastDreamWarpControllerBindingLabel()
        {
            InputControlType binding = GetFastDreamWarpControllerBinding();
            return EqualityComparer<InputControlType>.Default.Equals(binding, default)
                ? string.Empty
                : binding.ToString();
        }

        private static bool TryGetPressedControllerButton(out InputControlType button)
        {
            button = default;

            foreach (InputDevice device in InputManager.Devices)
            {
                if (device == null || !device.AnyButtonWasPressed)
                {
                    continue;
                }

                foreach (InputControl control in device.Controls)
                {
                    if (control != null && control.IsButton && control.WasPressed)
                    {
                        button = control.Target;
                        return true;
                    }
                }
            }

            return false;
        }

        private static string GetTeleportKitMenuKeyLabel()
        {
            return FormatTeleportKitKeyLabel(Modules.QoL.TeleportKit.MenuHotkey);
        }

        private static string GetTeleportKitSaveKeyLabel()
        {
            return FormatTeleportKitKeyLabel(GetTeleportKitSaveKeyDisplay());
        }

        private static string GetTeleportKitTeleportKeyLabel()
        {
            return FormatTeleportKitKeyLabel(GetTeleportKitTeleportKeyDisplay());
        }

        private static KeyCode GetTeleportKitSaveKeyDisplay()
        {
            return Modules.QoL.TeleportKit.SaveTeleportHotkey == KeyCode.None
                ? KeyCode.R
                : Modules.QoL.TeleportKit.SaveTeleportHotkey;
        }

        private static KeyCode GetTeleportKitTeleportKeyDisplay()
        {
            return Modules.QoL.TeleportKit.TeleportHotkey == KeyCode.None
                ? KeyCode.T
                : Modules.QoL.TeleportKit.TeleportHotkey;
        }

        private static string FormatTeleportKitKeyLabel(KeyCode key)
        {
            return key == KeyCode.None
                ? "Settings/FastReload/NotSet".Localize()
                : key.ToString();
        }
    }
}
