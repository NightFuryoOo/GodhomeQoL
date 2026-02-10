
using System.IO;

namespace GodhomeQoL
{
    public sealed partial class GodhomeQoL
    : IGlobalSettings<GlobalSettings>, ILocalSettings<LocalSettings>
    {
        public static GlobalSettings GlobalSettings { get; private set; } = new();
        public void OnLoadGlobal(GlobalSettings s)
        {
            bool isFirstRun = IsFirstRun();
            GlobalSettings = s;
            GlobalSettings.ShowHPOnDeath ??= new ShowHPOnDeathSettings();
            GlobalSettings.FastDreamWarp ??= new FastDreamWarpSettings();
            GlobalSettings.MaskDamage ??= new MaskDamageSettings();
            GlobalSettings.GearSwitcher ??= new GearSwitcherSettings();
            GlobalSettings.QuickMenuMasters ??= new QuickMenuMasterSettings();
            GlobalSettings.QuickMenuOrder ??= new List<string>();
            GlobalSettings.QuickMenuPositions ??= new Dictionary<string, QuickMenuEntryPosition>();
            GlobalSettings.QuickMenuCustomLabels ??= new Dictionary<string, string>();
            GlobalSettings.QuickMenuVisibility ??= new Dictionary<string, bool>();
            GlobalSettings.QuickMenuHotkey ??= "F3";

            EnsureGearSwitcherDefaults(GlobalSettings.GearSwitcher);

            if (isFirstRun)
            {
                Modules.Tools.QuickMenu.ApplyInitialDefaults();
                SaveGlobalSettingsSafe();
            }
        }
        public GlobalSettings OnSaveGlobal() => GlobalSettings;

        public static LocalSettings LocalSettings { get; private set; } = new();
        public void OnLoadLocal(LocalSettings s)
        {
            LocalSettings = s;
            if (GlobalSettings?.GearSwitcher != null && !string.IsNullOrWhiteSpace(s.GearSwitcherLastPreset))
            {
                string globalPreset = GlobalSettings.GearSwitcher.LastPreset ?? string.Empty;
                if (string.IsNullOrWhiteSpace(globalPreset)
                    || (string.Equals(globalPreset, "FullGear", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(s.GearSwitcherLastPreset, "FullGear", StringComparison.OrdinalIgnoreCase)))
                {
                    GlobalSettings.GearSwitcher.LastPreset = s.GearSwitcherLastPreset;
                    SaveGlobalSettingsSafe();
                }
            }
        }
        public LocalSettings OnSaveLocal() => LocalSettings;

        private static bool IsFirstRun()
        {
            string settingsPath = Path.Combine(Application.persistentDataPath, $"{ModInfo.Name}.GlobalSettings.json");
            if (File.Exists(settingsPath) || File.Exists(settingsPath + ".bak"))
            {
                return false;
            }

            return true;
        }

        private static void EnsureGearSwitcherDefaults(GearSwitcherSettings settings)
        {
            if (settings.Presets == null || settings.Presets.Count == 0)
            {
                settings.Presets = GearPresetDefaults.CreateDefaults();
            }
            else
            {
                Dictionary<string, GearPreset> defaults = GearPresetDefaults.CreateDefaults();
                foreach ((string name, GearPreset preset) in defaults)
                {
                    if (!settings.Presets.ContainsKey(name))
                    {
                        settings.Presets[name] = preset;
                    }
                }
            }

            if (settings.PresetOrder == null || settings.PresetOrder.Count == 0)
            {
                settings.PresetOrder = GearPresetDefaults.DefaultOrder();
            }
            else
            {
                foreach (string name in GearPresetDefaults.DefaultOrder())
                {
                    if (!settings.PresetOrder.Contains(name))
                    {
                        settings.PresetOrder.Add(name);
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(settings.FullGearDisplayName))
            {
                settings.FullGearDisplayName = "FullGear";
            }

            if (string.IsNullOrWhiteSpace(settings.LastPreset))
            {
                settings.LastPreset = "FullGear";
            }

            MigrateGearSwitcherMoves(settings);
        }

        private static void MigrateGearSwitcherMoves(GearSwitcherSettings settings)
        {
            if (settings.Presets == null)
            {
                return;
            }

            foreach (GearPreset preset in settings.Presets.Values)
            {
                if (!preset.HasAllMoveAbilities)
                {
                    continue;
                }

                preset.HasMoveAbilities ??= new Dictionary<string, bool>();
                preset.HasMoveAbilities["AcidArmour"] = true;
                preset.HasMoveAbilities["Dash"] = true;
                preset.HasMoveAbilities["Walljump"] = true;
                preset.HasMoveAbilities["SuperDash"] = true;
                preset.HasMoveAbilities["ShadowDash"] = true;
                preset.HasMoveAbilities["DoubleJump"] = true;
                preset.HasAllMoveAbilities = false;
            }
        }
    }
}
