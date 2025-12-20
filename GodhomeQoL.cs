namespace GodhomeQoL;

public sealed partial class GodhomeQoL : Mod, ITogglableMod, ICustomMenuMod
{
    public static GodhomeQoL? Instance { get; private set; }
    public static GodhomeQoL UnsafeInstance => Instance!;

    public static bool Active { get; private set; }

    public GodhomeQoL() : base(ModInfo.Name) => Instance = this;

    public override string GetVersion() => ModInfo.Version;

    public override List<(string, string)> GetPreloadNames() => new();

    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        ModuleManager.Load();
        Active = true;
    }

    public void Unload()
    {
        Active = false;
        ModuleManager.Unload();
    }

    internal static void MarkMenuDirty() => ModMenu.MarkDirty();
}
