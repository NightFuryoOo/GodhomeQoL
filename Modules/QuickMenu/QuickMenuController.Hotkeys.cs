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
                || IsCheatsKillAllRebinding()
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
            return (quickVisible && quickRenameMode)
                || IsAnyRebinding()
                || gearSwitcherPresetDeleteVisible
                || gearSwitcherResetConfirmVisible
                || quickSettingsResetConfirmVisible
                || IsTeleportKitGameplayMenuVisible();
        }

        private static bool IsTeleportKitGameplayMenuVisible()
        {
            return Modules.QoL.TeleportKit.Instance?.Input.ShowMenu ?? false;
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

            bool clear = key == overlayHotkeyPrevKey;
            if (!clear && TryGetHotkeyConflictOwnersExceptSelf(key, GetOverlayHotkeyOwnerLabel(overlayHotkeyRebindId), out string conflictOwners))
            {
                ShowStatusMessage($"HOTKEY {FormatKeyLabel(key)} занят: {conflictOwners}");
                if (quickSettingsHotkeyValues.TryGetValue(overlayHotkeyRebindId, out Text? text))
                {
                    UpdateKeybindValue(text, "Settings/FastReload/SetKey".Localize());
                }
                return;
            }

            string raw = clear ? string.Empty : key.ToString();
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

            bool clear = key == quickMenuPrevKey;
            if (!clear && TryGetHotkeyConflictOwnersExceptSelf(key, "Settings/QuickMenu/Hotkey".Localize(), out string conflictOwners))
            {
                ShowStatusMessage($"HOTKEY {FormatKeyLabel(key)} занят: {conflictOwners}");
                UpdateQuickMenuHotkeyButton("Settings/FastReload/SetKey".Localize());
                return;
            }

            GodhomeQoL.GlobalSettings.QuickMenuHotkey = clear ? string.Empty : key.ToString();
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

            if (key == reloadPrevKey)
            {
                Modules.FastReload.reloadKeyCode = (int)KeyCode.None;
                waitingForReloadRebind = false;
                UpdateKeybindValue(reloadKeyValue, FormatKeyLabel(GetReloadKey()));
                return;
            }

            if (TryGetFastReloadHotkeyConflictOwners(key, out string conflictOwners))
            {
                ShowStatusMessage($"HOTKEY {FormatKeyLabel(key)} занят: {conflictOwners}");
                UpdateKeybindValue(reloadKeyValue, "Settings/FastReload/SetKey".Localize());
                return;
            }

            Modules.FastReload.reloadKeyCode = (int)key;

            waitingForReloadRebind = false;
            UpdateKeybindValue(reloadKeyValue, FormatKeyLabel(GetReloadKey()));
        }

        private bool TryGetFastReloadHotkeyConflictOwners(KeyCode key, out string ownersText)
        {
            List<string> owners = new();
            HashSet<string> unique = new(StringComparer.OrdinalIgnoreCase);

            void AddOwner(string owner)
            {
                if (string.IsNullOrWhiteSpace(owner))
                {
                    return;
                }

                if (unique.Add(owner))
                {
                    owners.Add(owner);
                }
            }

            if (GetQuickMenuToggleKey() == key)
            {
                AddOwner("Settings/QuickMenu/Hotkey".Localize());
            }

            foreach (QuickMenuItemDefinition def in GetOrderedQuickMenuDefinitions())
            {
                if (!IsOverlayHotkeySupported(def.Id))
                {
                    continue;
                }

                if (GetOverlayHotkeyKey(def.Id) == key)
                {
                    AddOwner($"{def.Label} Overlay Hotkey");
                }
            }

            if (KeyCodeMatchesRawKeybind(MaskDamage.GetToggleUiKeybind(), key))
            {
                AddOwner("Settings/MaskDamage/ToggleUI".Localize());
            }

            if (KeyCodeMatchesRawKeybind(FreezeHitboxes.GetUnfreezeKeybind(), key))
            {
                AddOwner("Freeze Hitboxes Unfreeze Hotkey");
            }

            if (KeyCodeMatchesRawKeybind(Modules.Cheats.Cheats.GetKillAllHotkeyRaw(), key))
            {
                AddOwner("Settings/Cheats/KillAllHotkey".Localize());
            }

            if (KeyCodeMatchesRawKeybind(SpeedChanger.toggleKeybind, key))
            {
                AddOwner("SpeedChanger/ToggleKey".Localize());
            }

            if (KeyCodeMatchesRawKeybind(SpeedChanger.inputSpeedKeybind, key))
            {
                AddOwner("SpeedChanger/InputKey".Localize());
            }

            if (Modules.QoL.TeleportKit.MenuHotkey == key)
            {
                AddOwner("TeleportKit/MenuHotkey".Localize());
            }

            if (GetTeleportKitSaveKeyDisplay() == key)
            {
                AddOwner("TeleportKit/SaveHotkey".Localize());
            }

            if (GetTeleportKitTeleportKeyDisplay() == key)
            {
                AddOwner("TeleportKit/TeleportHotkey".Localize());
            }

            if (TryMapKeyCodeToInControlKey(key, out Key mapped))
            {
                string mappedRaw = mapped.ToString();
                if (string.Equals(GetShowHpBindingRaw(), mappedRaw, StringComparison.OrdinalIgnoreCase))
                {
                    AddOwner("ShowHPOnDeath/HudToggleKey".Localize());
                }

                if (string.Equals(GetFastDreamWarpKeyBindingRaw(), mappedRaw, StringComparison.OrdinalIgnoreCase))
                {
                    AddOwner("Settings/FastDreamWarp/Hotkey".Localize());
                }
            }

            ownersText = string.Join(", ", owners);
            return owners.Count > 0;
        }

        private bool TryGetHotkeyConflictOwnersExceptSelf(KeyCode key, string selfOwner, out string ownersText)
        {
            if (!TryGetFastReloadHotkeyConflictOwners(key, out string allOwners))
            {
                ownersText = string.Empty;
                return false;
            }

            List<string> filtered = allOwners
                .Split(',')
                .Select(o => o.Trim())
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .Where(o => !string.Equals(o, selfOwner, StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            ownersText = string.Join(", ", filtered);
            return filtered.Count > 0;
        }

        internal bool TryGetHotkeyConflictOwnersExceptSelfExternal(KeyCode key, string selfOwner, out string ownersText)
        {
            return TryGetHotkeyConflictOwnersExceptSelf(key, selfOwner, out ownersText);
        }

        private string GetOverlayHotkeyOwnerLabel(string id)
        {
            QuickMenuItemDefinition? definition = GetOrderedQuickMenuDefinitions()
                .FirstOrDefault(def => string.Equals(def.Id, id, StringComparison.Ordinal));

            if (definition == null)
            {
                return "Overlay Hotkey";
            }

            return $"{definition.Label} Overlay Hotkey";
        }

        private static bool KeyCodeMatchesRawKeybind(string raw, KeyCode key)
        {
            return !string.IsNullOrWhiteSpace(raw)
                && Enum.TryParse(raw, true, out KeyCode parsed)
                && parsed == key;
        }

        private static KeyCode GetReloadKey() => (KeyCode)Modules.FastReload.reloadKeyCode;

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

            if (!clear && TryGetShowHpOnDeathHotkeyConflictOwners(key, out string conflictOwners))
            {
                ShowStatusMessage($"HOTKEY {FormatKeyLabel(key)} занят: {conflictOwners}");
                UpdateKeybindValue(showHpHudToggleKeyValue, "Settings/FastReload/SetKey".Localize());
                return;
            }

            ApplyShowHpBinding(clear ? (Key?)null : mapped);
            waitingForShowHpRebind = false;
            UpdateKeybindValue(showHpHudToggleKeyValue, GetShowHpBindingLabel());
        }

        private bool TryGetShowHpOnDeathHotkeyConflictOwners(KeyCode key, out string ownersText)
        {
            return TryGetHotkeyConflictOwnersExceptSelf(key, "ShowHPOnDeath/HudToggleKey".Localize(), out ownersText);
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

            if (!clear && TryGetMaskDamageHotkeyConflictOwners(key, out string conflictOwners))
            {
                ShowStatusMessage($"HOTKEY {FormatKeyLabel(key)} занят: {conflictOwners}");
                UpdateKeybindValue(maskDamageToggleUiKeyValue, "Settings/FastReload/SetKey".Localize());
                return;
            }

            MaskDamage.SetToggleUiKeybind(clear ? string.Empty : keyName);
            waitingForMaskDamageUiRebind = false;
            UpdateKeybindValue(maskDamageToggleUiKeyValue, GetMaskDamageToggleUiKeyLabel());
        }

        private bool TryGetMaskDamageHotkeyConflictOwners(KeyCode key, out string ownersText)
        {
            return TryGetHotkeyConflictOwnersExceptSelf(key, "Settings/MaskDamage/ToggleUI".Localize(), out ownersText);
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

            if (!clear && TryGetHotkeyConflictOwnersExceptSelf(key, "Freeze Hitboxes Unfreeze Hotkey", out string conflictOwners))
            {
                ShowStatusMessage($"HOTKEY {FormatKeyLabel(key)} занят: {conflictOwners}");
                UpdateKeybindValue(freezeHitboxesUnfreezeKeyValue, "Settings/FastReload/SetKey".Localize());
                return;
            }

            FreezeHitboxes.SetUnfreezeKeybind(clear ? string.Empty : keyName);
            waitingForFreezeHitboxesRebind = false;
            UpdateKeybindValue(freezeHitboxesUnfreezeKeyValue, GetFreezeHitboxesKeyLabel());
        }

        private bool IsCheatsKillAllRebinding()
        {
            return waitingForCheatsKillAllRebind;
        }

        private string GetCheatsKillAllHotkeyLabel()
        {
            return FormatMaskDamageKeyLabel(Modules.Cheats.Cheats.GetKillAllHotkeyRaw());
        }

        private void StartCheatsKillAllRebind()
        {
            waitingForCheatsKillAllRebind = true;
            cheatsKillAllPrevKey = Modules.Cheats.Cheats.GetKillAllHotkeyRaw();
            UpdateKeybindValue(cheatsKillAllHotkeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void CancelCheatsKillAllRebind()
        {
            if (!waitingForCheatsKillAllRebind)
            {
                return;
            }

            waitingForCheatsKillAllRebind = false;
            UpdateKeybindValue(cheatsKillAllHotkeyValue, GetCheatsKillAllHotkeyLabel());
        }

        private void HandleCheatsKillAllRebind()
        {
            if (!waitingForCheatsKillAllRebind)
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

                ApplyCheatsKillAllRebind(key);
                return;
            }
        }

        private void ApplyCheatsKillAllRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForCheatsKillAllRebind = false;
                UpdateKeybindValue(cheatsKillAllHotkeyValue, GetCheatsKillAllHotkeyLabel());
                return;
            }

            string keyName = key.ToString();
            bool clear = !string.IsNullOrEmpty(cheatsKillAllPrevKey)
                && string.Equals(cheatsKillAllPrevKey, keyName, StringComparison.OrdinalIgnoreCase);

            if (!clear && TryGetHotkeyConflictOwnersExceptSelf(key, "Settings/Cheats/KillAllHotkey".Localize(), out string conflictOwners))
            {
                ShowStatusMessage($"HOTKEY {FormatKeyLabel(key)} занят: {conflictOwners}");
                UpdateKeybindValue(cheatsKillAllHotkeyValue, "Settings/FastReload/SetKey".Localize());
                return;
            }

            Modules.Cheats.Cheats.SetKillAllHotkeyRaw(clear ? string.Empty : keyName);
            waitingForCheatsKillAllRebind = false;
            UpdateKeybindValue(cheatsKillAllHotkeyValue, GetCheatsKillAllHotkeyLabel());
        }

        private void HandleCheatsKillAllHotkey()
        {
            if (!GetCheatsMasterEnabled() || !GetCheatsEnabled())
            {
                return;
            }

            KeyCode key = Modules.Cheats.Cheats.GetKillAllHotkey();
            if (key == KeyCode.None || !Input.GetKeyDown(key))
            {
                return;
            }

            _ = Modules.Cheats.Cheats.KillAll();
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
            return waitingForSpeedToggleRebind
                || waitingForSpeedInputRebind;
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

            bool clear = string.Equals(speedTogglePrevKey, key.ToString(), StringComparison.OrdinalIgnoreCase);
            if (!clear && TryGetHotkeyConflictOwnersExceptSelf(key, "SpeedChanger/ToggleKey".Localize(), out string conflictOwners))
            {
                ShowStatusMessage($"HOTKEY {FormatKeyLabel(key)} занят: {conflictOwners}");
                UpdateKeybindValue(speedChangerToggleKeyValue, "Settings/FastReload/SetKey".Localize());
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

            bool clear = string.Equals(speedInputPrevKey, key.ToString(), StringComparison.OrdinalIgnoreCase);
            if (!clear && TryGetHotkeyConflictOwnersExceptSelf(key, "SpeedChanger/InputKey".Localize(), out string conflictOwners))
            {
                ShowStatusMessage($"HOTKEY {FormatKeyLabel(key)} занят: {conflictOwners}");
                UpdateKeybindValue(speedChangerInputKeyValue, "Settings/FastReload/SetKey".Localize());
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

            return inToggle || inInput;
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

            if (!Modules.QoL.TeleportKit.TryAssignHotkey(Modules.QoL.TeleportKit.HotkeySlot.Menu, key, out string failureReason))
            {
                ShowStatusMessage($"HOTKEY {FormatKeyLabel(key)} {failureReason}");
                UpdateKeybindValue(teleportKitMenuKeyValue, "Settings/FastReload/SetKey".Localize());
                return;
            }

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

            if (!Modules.QoL.TeleportKit.TryAssignHotkey(Modules.QoL.TeleportKit.HotkeySlot.SaveTeleport, key, out string failureReason))
            {
                ShowStatusMessage($"HOTKEY {FormatKeyLabel(key)} {failureReason}");
                UpdateKeybindValue(teleportKitSaveKeyValue, "Settings/FastReload/SetKey".Localize());
                return;
            }

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

            if (!Modules.QoL.TeleportKit.TryAssignHotkey(Modules.QoL.TeleportKit.HotkeySlot.Teleport, key, out string failureReason))
            {
                ShowStatusMessage($"HOTKEY {FormatKeyLabel(key)} {failureReason}");
                UpdateKeybindValue(teleportKitTeleportKeyValue, "Settings/FastReload/SetKey".Localize());
                return;
            }

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

            if (!clear && TryGetHotkeyConflictOwnersExceptSelf(key, "Settings/FastDreamWarp/Hotkey".Localize(), out string conflictOwners))
            {
                ShowStatusMessage($"HOTKEY {FormatKeyLabel(key)} занят: {conflictOwners}");
                UpdateKeybindValue(fastDreamWarpKeyValue, "Settings/FastReload/SetKey".Localize());
                return;
            }

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
            if (IsAnyUiVisible())
            {
                return;
            }

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
                "FastReload" => GetFastReloadEnabled(),
                "DreamshieldSettings" => GetDreamshieldEnabled(),
                "ShowHPOnDeath" => GetShowHpOnDeathEnabled(),
                "MaskDamage" => GetMaskDamageEnabled(),
                "FreezeHitboxes" => GetFreezeHitboxesEnabled(),
                "SpeedChanger" => GetSpeedChangerEnabled(),
                "TeleportKit" => GetTeleportKitEnabled(),
                "BossChallenge" => GetBossChallengeMasterEnabled(),
                "BossManipulate" => GetCollectorPhasesEnabled() || GetZoteHelperEnabled() || GetGruzMotherHelperEnabled() || GetHornetProtectorHelperEnabled() || GetBroodingMawlekHelperEnabled() || GetMassiveMossChargerHelperEnabled() || GetCrystalGuardianHelperEnabled() || GetEnragedGuardianHelperEnabled() || GetHornetSentinelHelperEnabled() || GetAdditionalGhostHelpersEnabled() || GetGruzMotherP1HelperEnabled() || GetVengeflyKingP1HelperEnabled() || GetBroodingMawlekP1HelperEnabled() || GetNoskP2HelperEnabled() || GetUumuuP3HelperEnabled() || GetSoulWarriorP1HelperEnabled() || GetNoEyesP4HelperEnabled() || GetMarmuP2HelperEnabled() || GetXeroP2HelperEnabled() || GetMarkothP4HelperEnabled() || GetGorbP1HelperEnabled(),
                "RandomPantheons" => GetRandomPantheonsMasterEnabled(),
                "TrueBossRush" => GetTrueBossRushMasterEnabled() && GetTrueBossRushEnabled(),
                "Cheats" => GetCheatsMasterEnabled(),
                "AlwaysFurious" => GetAlwaysFuriousEnabled(),
                "GearSwitcher" => GetGearSwitcherEnabled(),
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
                "FastReload" => fastReloadVisible,
                "DreamshieldSettings" => dreamshieldVisible,
                "ShowHPOnDeath" => showHpOnDeathVisible,
                "MaskDamage" => maskDamageVisible,
                "FreezeHitboxes" => freezeHitboxesVisible,
                "SpeedChanger" => speedChangerVisible,
                "TeleportKit" => teleportKitVisible,
                "BossChallenge" => bossChallengeVisible,
                "BossManipulate" => bossManipulateVisible || bossManipulateOtherRoomsVisible || gruzMotherP1HelperVisible || vengeflyKingP1HelperVisible || broodingMawlekP1HelperVisible || noskP2HelperVisible || uumuuP3HelperVisible || soulWarriorP1HelperVisible || noEyesP4HelperVisible || marmuP2HelperVisible || xeroP2HelperVisible || markothP4HelperVisible || gorbP1HelperVisible || collectorVisible || zoteHelperVisible || gruzHelperVisible || hornetHelperVisible || mawlekHelperVisible || massiveMossHelperVisible || crystalGuardianHelperVisible || enragedGuardianHelperVisible || hornetSentinelHelperVisible || IsAnyAdditionalGhostHelperVisible(),
                "RandomPantheons" => randomPantheonsVisible,
                "TrueBossRush" => trueBossRushVisible,
                "Cheats" => cheatsVisible,
                "AlwaysFurious" => alwaysFuriousVisible,
                "GearSwitcher" => gearSwitcherVisible || gearSwitcherCharmCostVisible || gearSwitcherPresetVisible,
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
            returnToBossManipulateOnClose = false;
            returnToQolOnClose = false;

            SetQuickVisible(false);
            SetQuickSettingsVisible(false);
            CloseAllOverlaysForHotkey();

            switch (id)
            {
                case "FastSuperDash":
                    SetOverlayVisible(true);
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
                case "BossManipulate":
                    SetBossManipulateVisible(true);
                    break;
                case "RandomPantheons":
                    SetRandomPantheonsVisible(true);
                    break;
                case "TrueBossRush":
                    SetTrueBossRushVisible(true);
                    break;
                case "Cheats":
                    SetCheatsVisible(true);
                    break;
                case "AlwaysFurious":
                    SetAlwaysFuriousVisible(true);
                    break;
                case "GearSwitcher":
                    SetGearSwitcherVisible(true);
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
            returnToBossManipulateOnClose = false;
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
            SetTrueBossRushVisible(false);
            SetCheatsVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetGearSwitcherCharmCostVisible(false);
            SetGearSwitcherPresetVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetGruzHelperVisible(false);
            SetHornetHelperVisible(false);
            SetMawlekHelperVisible(false);
            SetMassiveMossHelperVisible(false);
            SetCrystalGuardianHelperVisible(false);
            SetEnragedGuardianHelperVisible(false);
            SetHornetSentinelHelperVisible(false);
            SetAllAdditionalGhostHelpersVisible(false);
            SetBossManipulateVisible(false);
            SetBossManipulateOtherRoomsVisible(false);
            SetGruzMotherP1HelperVisible(false);
            SetVengeflyKingP1HelperVisible(false);
            SetBroodingMawlekP1HelperVisible(false);
            SetNoskP2HelperVisible(false);
            SetUumuuP3HelperVisible(false);
            SetSoulWarriorP1HelperVisible(false);
            SetNoEyesP4HelperVisible(false);
            SetMarmuP2HelperVisible(false);
            SetXeroP2HelperVisible(false);
            SetMarkothP4HelperVisible(false);
            SetGorbP1HelperVisible(false);
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
