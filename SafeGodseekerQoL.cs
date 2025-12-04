using MonoMod.ModInterop;

using SafeGodseekerQoL.ModInterop;
namespace SafeGodseekerQoL
{
    public sealed partial class SafeGodseekerQoL : Mod, ITogglableMod
    {
        public static SafeGodseekerQoL? Instance { get; private set; }
        public static SafeGodseekerQoL UnsafeInstance => Instance!;

        public static bool Active { get; private set; }

        public override string GetVersion() => ModInfo.Version;
        public override string GetMenuButtonText() =>
        "ModName".Localize() + ' ' + Lang.Get("MAIN_OPTIONS", "MainMenu");
        static SafeGodseekerQoL()
        {
            try
            {
                typeof(Exports).ModInterop();
            }
            catch (Exception e)
            {
                Logger.Log($"Exception during static initialization: {e}");
                //  Можно попытаться продолжить, но это может привести к дальнейшим проблемам.
            }
        }
        public SafeGodseekerQoL() : base(ModInfo.Name) => Instance = this;

        internal static void MarkMenuDirty() => ModMenu.MarkDirty();

        public override void Initialize()
        {
            if (Active)
            {
                LogWarn("Attempting to initialize multiple times, operation rejected");
                return;
            }

            Active = true;

            ModuleManager.Load();
        }
        public void Unload()
        {
            ModuleManager.Unload();

            Active = false;
        }

    }
}
