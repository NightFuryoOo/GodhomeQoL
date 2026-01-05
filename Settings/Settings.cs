
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
            GlobalSettings.QuickMenuOrder ??= new List<string>();
            GlobalSettings.QuickMenuPositions ??= new Dictionary<string, QuickMenuEntryPosition>();
            GlobalSettings.QuickMenuCustomLabels ??= new Dictionary<string, string>();
            GlobalSettings.QuickMenuVisibility ??= new Dictionary<string, bool>();
            GlobalSettings.QuickMenuHotkey ??= "F3";
            GlobalSettings.BindingsMenuHotkey ??= string.Empty;

            if (isFirstRun)
            {
                Modules.Tools.QuickMenu.ApplyInitialDefaults();
                SaveGlobalSettingsSafe();
            }
        }
        public GlobalSettings OnSaveGlobal() => GlobalSettings;

        public static LocalSettings LocalSettings { get; private set; } = new();
        public void OnLoadLocal(LocalSettings s) => LocalSettings = s;
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
    }
}
