#nullable enable

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using GodhomeQoL.Utils;
using ToggleableBindings.HKQuickSettings;
using ToggleableBindings.UI;
using UnityEngine;

namespace ToggleableBindings
{
    /// <summary>
    /// ToggleableBindings host integrated into GodhomeQoL.
    /// </summary>
    public sealed partial class ToggleableBindings
    {
        /// <summary>
        /// Gets the instance of this host.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.NotNull]
        public static ToggleableBindings? Instance { get; private set; }

        [QuickSetting, DefaultValue(true)]
        internal static bool EnforceBindingRestrictions { get; private set; } = true;

        [System.Diagnostics.CodeAnalysis.NotNull]
        internal QuickSettings? Settings { get; private set; }

        private static bool initialized;

        private ToggleableBindings()
        {
        }

        internal static void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            if (initialized)
            {
                Instance?.LogWarn("ToggleableBindings already initialized.");
                return;
            }

            Instance = new ToggleableBindings();
            initialized = true;

            BaseGamePrefabs.Initialize(preloadedObjects);

            Instance.AddHooks();
            Instance.Settings = new QuickSettings("ToggleableBindings");
            BindingManager.Initialize();
            BindingsUIController.Initialize();

            Instance.LogDebug("Initialized.");
        }

        internal static void Unload()
        {
            if (!initialized)
            {
                return;
            }

            initialized = false;

            Instance?.RemoveHooks();
            if (Instance?.Settings?.CurrentSaveSlot != null)
            {
                Instance.Settings.SaveSaveSettings();
            }

            Instance?.Settings?.Unload();
            BindingManager.Unload();
            BindingsUIController.Unload();

            Instance = null;
        }

        internal void Log(string message) => Logger.Log(message);

        internal void LogDebug(string message) => Logger.LogDebug(message);

        internal void LogWarn(string message) => Logger.LogWarn(message);

        internal void LogError(string message) => Logger.LogError(message);
    }
}

