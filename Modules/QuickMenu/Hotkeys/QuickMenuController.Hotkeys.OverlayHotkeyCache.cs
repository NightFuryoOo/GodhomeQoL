using System;
using System.Collections.Generic;
using InControl;
using UnityEngine;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private const float OverlayHotkeyCacheMaxAge = 2f;

        private readonly struct OverlayHotkeyCacheEntry
        {
            public OverlayHotkeyCacheEntry(string id, KeyCode key, InputControlType button)
            {
                Id = id;
                Key = key;
                Button = button;
            }

            public string Id { get; }

            public KeyCode Key { get; }

            public InputControlType Button { get; }
        }

        private readonly List<OverlayHotkeyCacheEntry> overlayHotkeyCache = new();
        private bool overlayHotkeyCacheDirty = true;
        private float overlayHotkeyCacheBuiltAt;
        private KeyCode cachedQuickMenuToggleKey = KeyCode.None;

        private bool TryUseOverlayHotkeyCache()
        {
            if (!Modules.Performance.FpsBoost.IsMenuHotkeyCacheActive() || IsAnyUiVisible())
            {
                overlayHotkeyCacheDirty = true;
                return false;
            }

            float now = Time.unscaledTime;
            if (overlayHotkeyCacheDirty || now - overlayHotkeyCacheBuiltAt >= OverlayHotkeyCacheMaxAge)
            {
                RebuildOverlayHotkeyCache();
                overlayHotkeyCacheBuiltAt = now;
                overlayHotkeyCacheDirty = false;
            }

            return true;
        }

        private void RebuildOverlayHotkeyCache()
        {
            overlayHotkeyCache.Clear();
            foreach (QuickMenuItemDefinition def in GetOrderedQuickMenuDefinitions())
            {
                if (!IsOverlayHotkeySupported(def.Id))
                {
                    continue;
                }

                InputControlType button = GetOverlayHotkeyControllerBinding(def.Id);
                KeyCode key = GetOverlayHotkeyKey(def.Id);
                if (EqualityComparer<InputControlType>.Default.Equals(button, default) && key == KeyCode.None)
                {
                    continue;
                }

                overlayHotkeyCache.Add(new OverlayHotkeyCacheEntry(def.Id, key, button));
            }

            cachedQuickMenuToggleKey = GetQuickMenuToggleKey();
        }

        private void HandleOverlayHotkeysCached()
        {
            for (int i = 0; i < overlayHotkeyCache.Count; i++)
            {
                OverlayHotkeyCacheEntry entry = overlayHotkeyCache[i];
                if (!EqualityComparer<InputControlType>.Default.Equals(entry.Button, default) && IsControllerButtonPressed(entry.Button))
                {
                    ToggleOverlayFromHotkey(entry.Id);
                    break;
                }

                if (entry.Key != KeyCode.None && Input.GetKeyDown(entry.Key))
                {
                    ToggleOverlayFromHotkey(entry.Id);
                    break;
                }
            }
        }

        private KeyCode GetQuickMenuToggleKeyForFrame()
        {
            return TryUseOverlayHotkeyCache() ? cachedQuickMenuToggleKey : GetQuickMenuToggleKey();
        }
    }
}
