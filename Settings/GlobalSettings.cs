namespace GodhomeQoL.Settings;

public sealed class GlobalSettings : SettingBase<GlobalSettingAttribute>
{
    private readonly Dictionary<string, bool> modules = ModuleManager
        .Modules
        .Filter(pair => !pair.Value.Hidden)
        .ToDictionary(
            pair => pair.Key,
            pair => pair.Value.DefaultEnabled
        );

    [JsonProperty(PropertyName = nameof(modules))]
    public Dictionary<string, bool> Modules
    {
        get => modules;
        set
        {
            foreach (KeyValuePair<string, bool> pair in value)
            {
                if (modules.ContainsKey(pair.Key))
                {
                    modules[pair.Key] = pair.Value;

                    _ = ModuleManager.TryGetModule(pair.Key, out Module? module);
                    module!.Enabled = pair.Value;
                }
            }
        }
    }

    public ShowHPOnDeathSettings ShowHPOnDeath { get; set; } = new();
<<<<<<< HEAD
=======

    public FastDreamWarpSettings FastDreamWarp { get; set; } = new();

    public List<string> QuickMenuOrder { get; set; } = new();

    public bool QuickMenuFreeLayout { get; set; } = false;

    public Dictionary<string, QuickMenuEntryPosition> QuickMenuPositions { get; set; } = new();

    public Dictionary<string, string> QuickMenuCustomLabels { get; set; } = new();

    public Dictionary<string, bool> QuickMenuVisibility { get; set; } = new();

    public string QuickMenuHotkey { get; set; } = "F3";

    public int QuickMenuOpacity { get; set; } = 100;

    public string BindingsMenuHotkey { get; set; } = string.Empty;

    public bool BindingsMenuEverywhere { get; set; } = false;
>>>>>>> fcd9e8b (Update 1.0.0.7)
}
