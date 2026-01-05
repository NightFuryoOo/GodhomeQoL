
<<<<<<< HEAD
=======
using System.IO;

>>>>>>> fcd9e8b (Update 1.0.0.7)
namespace GodhomeQoL
{
    public sealed partial class GodhomeQoL
    : IGlobalSettings<GlobalSettings>, ILocalSettings<LocalSettings>
    {
        public static GlobalSettings GlobalSettings { get; private set; } = new();
        public void OnLoadGlobal(GlobalSettings s)
        {
<<<<<<< HEAD
            GlobalSettings = s;
            GlobalSettings.ShowHPOnDeath ??= new ShowHPOnDeathSettings();
=======
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
>>>>>>> fcd9e8b (Update 1.0.0.7)
        }
        public GlobalSettings OnSaveGlobal() => GlobalSettings;

        public static LocalSettings LocalSettings { get; private set; } = new();
        public void OnLoadLocal(LocalSettings s) => LocalSettings = s;
        public LocalSettings OnSaveLocal() => LocalSettings;
<<<<<<< HEAD
=======

        private static bool IsFirstRun()
        {
            string settingsPath = Path.Combine(Application.persistentDataPath, $"{ModInfo.Name}.GlobalSettings.json");
            if (File.Exists(settingsPath) || File.Exists(settingsPath + ".bak"))
            {
                return false;
            }

            return true;
        }
>>>>>>> fcd9e8b (Update 1.0.0.7)
    }
}
