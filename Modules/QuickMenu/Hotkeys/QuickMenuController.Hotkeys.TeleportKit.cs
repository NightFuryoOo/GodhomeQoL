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
        private static bool IsTeleportKitGameplayMenuVisible()
        {
            return Modules.QoL.TeleportKit.Instance?.Input.ShowMenu ?? false;
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
