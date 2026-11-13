namespace GodhomeQoL.Modules.Tools;

public sealed class NailDamageCheck : Module
{
    [GlobalSetting]
    public static float NailDamageTextScale = 1f;

    private const float DisplaySeconds = 3f;

    public override bool DefaultEnabled => true;
    public override bool Hidden => true;
    public override bool AlwaysEnabled => true;

    private static NailDamageCheckDisplay? display;
    private static float hideAtTime = -1f;

    private protected override void Load()
    {
        ModHooks.HeroUpdateHook += OnHeroUpdate;
    }

    private protected override void Unload()
    {
        ModHooks.HeroUpdateHook -= OnHeroUpdate;
        DestroyDisplay();
    }

    internal static string GetKeybind() => GodhomeQoL.GlobalSettings.NailDamageCheckKeybind ?? string.Empty;

    internal static void SetKeybind(string value)
    {
        GodhomeQoL.GlobalSettings.NailDamageCheckKeybind = value ?? string.Empty;
        GodhomeQoL.SaveGlobalSettingsSafe();
    }

    internal static KeyCode GetKey()
    {
        string raw = GetKeybind();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return KeyCode.None;
        }

        return Enum.TryParse(raw, true, out KeyCode key) ? key : KeyCode.None;
    }

    private static void OnHeroUpdate()
    {
        if (hideAtTime >= 0f && Time.unscaledTime >= hideAtTime)
        {
            HideDisplay();
        }

        if (!IsQolMasterEnabled() || QuickMenu.IsAnyUiVisible() || QuickMenu.IsHotkeyInputBlocked())
        {
            return;
        }

        KeyCode key = GetKey();
        if (key != KeyCode.None && Input.GetKeyDown(key))
        {
            ShowNailDamage();
        }
    }

    private static bool IsQolMasterEnabled() => GodhomeQoL.GlobalSettings.QuickMenuMasters?.QolEnabled ?? true;

    private static void ShowNailDamage()
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        display ??= new NailDamageCheckDisplay();
        display.Display($"Nail Damage: {pd.nailDamage}");
        hideAtTime = Time.unscaledTime + DisplaySeconds;
    }

    internal static void PreviewText()
    {
        display ??= new NailDamageCheckDisplay();
        display.ApplyScale();
        display.Preview($"Nail Damage: {PlayerData.instance?.nailDamage ?? 5}");
    }

    private static void HideDisplay()
    {
        display?.Hide();
        hideAtTime = -1f;
    }

    private static void DestroyDisplay()
    {
        display?.Destroy();
        display = null;
        hideAtTime = -1f;
    }
}

internal sealed class NailDamageCheckDisplay
{
    private readonly HudLabel label = new("NailDamageCheckCanvas", Vector2.one, new Vector2(-29f, -38f), 26, () => HudTextScales.NailDamageText);

    public void Destroy() => label.Destroy();

    public void Hide() => label.Hide();

    public void Display(string value) => label.Show(value);

    internal void ApplyScale() => label.ApplyScale();

    internal void Preview(string value) => label.Preview(value);
}
