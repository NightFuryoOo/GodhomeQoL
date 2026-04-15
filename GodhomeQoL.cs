namespace GodhomeQoL;

public sealed partial class GodhomeQoL : Mod, ITogglableMod, ICustomMenuMod
{
    public static GodhomeQoL? Instance { get; private set; }
    public static GodhomeQoL UnsafeInstance => Instance!;

    public static bool Active { get; private set; }

    public GodhomeQoL() : base(ModInfo.Name) => Instance = this;

    public override string GetVersion() => ModInfo.Version;

    public override List<(string, string)> GetPreloadNames() => new()
    {
        ("GG_Atrium", "GG_Challenge_Door"),
        ("Room_mapper", "Shop Menu")
    };

    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        On.GameManager.OnApplicationQuit += OnApplicationQuit;
        ModMenu.InstallHooks();
        ToggleableBindings.ToggleableBindings.Initialize(preloadedObjects);
        ModuleManager.Load();
        Active = true;
    }

    public void Unload()
    {
        Active = false;
        On.GameManager.OnApplicationQuit -= OnApplicationQuit;
        ModMenu.UninstallHooks();
        ModuleManager.Unload();
        ToggleableBindings.ToggleableBindings.Unload();
    }

    private void OnApplicationQuit(On.GameManager.orig_OnApplicationQuit orig, GameManager self)
    {
        try
        {
            ForceDisableAlwaysFuriousForNextSession();
        }
        finally
        {
            orig(self);
        }
    }

    private static void ForceDisableAlwaysFuriousForNextSession()
    {
        string moduleName = typeof(global::GodhomeQoL.Modules.BossChallenge.AlwaysFurious).Name;
        if (!Setting.Global.Modules.TryGetValue(moduleName, out bool isEnabledInSettings) || !isEnabledInSettings)
        {
            return;
        }

        if (ModuleManager.TryGetModule(moduleName, out Module? module))
        {
            module.Enabled = false;
        }
        else
        {
            Setting.Global.Modules[moduleName] = false;
        }

        SaveGlobalSettingsSafe();
    }

    internal static void SaveGlobalSettingsSafe() => Instance?.SaveGlobalSettings();

    internal static void MarkMenuDirty() => ModMenu.MarkDirty();
}
