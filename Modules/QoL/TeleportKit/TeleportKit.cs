<<<<<<< HEAD
namespace GodhomeQoL.Modules.QoL;
=======
namespace SafeGodseekerQoL.Modules.QoL;
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265

public sealed class TeleportKit : Module
{
    internal static TeleportKit? Instance { get; private set; }

    [GlobalSetting]
    public static KeyCode MenuHotkey = KeyCode.F6;

    [GlobalSetting]
    public static KeyCode SaveTeleportHotkey = KeyCode.R;

    [GlobalSetting]
    public static KeyCode TeleportHotkey = KeyCode.T;

    internal TeleportKitLogger Log { get; private set; } = null!;
    internal TeleportData Data { get; private set; } = null!;
    internal TeleportInputHandler Input { get; private set; } = null!;
    internal TeleportManager Teleport { get; private set; } = null!;
    internal TeleportMenuGUI GUI { get; private set; } = null!;

    internal KeyCode MenuKey => MenuHotkey;
    internal KeyCode SaveTeleportKey => IsValidHotkey(SaveTeleportHotkey) ? SaveTeleportHotkey : KeyCode.R;
    internal KeyCode TeleportKey => IsValidHotkey(TeleportHotkey) ? TeleportHotkey : KeyCode.T;

    public override bool DefaultEnabled => false;
    public override ToggleableLevel ToggleableLevel => ToggleableLevel.AnyTime;

    private static bool IsValidHotkey(KeyCode key) => key != KeyCode.None;

    private protected override void Load()
    {
        Instance = this;

        Log = new TeleportKitLogger();
        Data = new TeleportData();
        Input = new TeleportInputHandler(this);
        Teleport = new TeleportManager(this);
        GUI = new TeleportMenuGUI(this);

<<<<<<< HEAD
        ModHooks.AfterSavegameLoadHook += OnAfterSavegameLoad;

=======
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265
        Log.Write("Teleport kit initialized");
    }

    private protected override void Unload()
    {
<<<<<<< HEAD
        ModHooks.AfterSavegameLoadHook -= OnAfterSavegameLoad;

=======
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265
        GUI?.Dispose();
        Input?.Dispose();
        Teleport?.Dispose();

        Instance = null;
    }
<<<<<<< HEAD

    private void OnAfterSavegameLoad(SaveGameData _)
    {
        Input?.ResetPauseFlag();
    }
=======
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265
}
