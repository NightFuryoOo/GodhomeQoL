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
    {
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
    }
}
