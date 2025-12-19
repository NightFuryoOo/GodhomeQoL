<<<<<<< HEAD

namespace GodhomeQoL
{
    public sealed partial class GodhomeQoL
=======
﻿
namespace SafeGodseekerQoL
{
    public sealed partial class SafeGodseekerQoL
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265
    : IGlobalSettings<GlobalSettings>, ILocalSettings<LocalSettings>
    {
        public static GlobalSettings GlobalSettings { get; private set; } = new();
        public void OnLoadGlobal(GlobalSettings s) => GlobalSettings = s;
        public GlobalSettings OnSaveGlobal() => GlobalSettings;

        public static LocalSettings LocalSettings { get; private set; } = new();
        public void OnLoadLocal(LocalSettings s) => LocalSettings = s;
        public LocalSettings OnSaveLocal() => LocalSettings;
    }
}
