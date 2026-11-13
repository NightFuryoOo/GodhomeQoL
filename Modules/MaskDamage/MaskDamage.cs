namespace GodhomeQoL.Modules.Tools;

public sealed class MaskDamage : Module
{
    [GlobalSetting]
    public static float MaskDamageTextScale = 1f;

    private const float HalfMultiplier = 0.5f;
    private const int MinMultiplier = 1;
    private const int MaxMultiplier = 999;

    public override bool DefaultEnabled => false;
    public override bool Hidden => true;
    public override bool AlwaysEnabled => true;

    private static MaskDamageSettings Settings => GodhomeQoL.GlobalSettings.MaskDamage ??= new MaskDamageSettings();

    private static MaskDamageDisplay? display;

    private protected override void Load()
    {
        ModHooks.AfterTakeDamageHook += OnAfterTakeDamage;
        ModHooks.HeroUpdateHook += OnHeroUpdate;

        if (Settings.Enabled && Settings.ShowUI)
        {
            EnsureDisplay();
            UpdateDisplay();
        }
    }

    private protected override void Unload()
    {
        ModHooks.AfterTakeDamageHook -= OnAfterTakeDamage;
        ModHooks.HeroUpdateHook -= OnHeroUpdate;
        DestroyDisplay();
    }

    private static bool chainActive;
    private static bool chainSavedEnabled;
    private static float chainSavedMultiplier = 1f;

    internal static void ApplyChainDamage(float multiplier)
    {
        if (!chainActive)
        {
            chainActive = true;
            chainSavedEnabled = Settings.Enabled;
            chainSavedMultiplier = Settings.DamageMultiplier;
        }

        float normalized = NormalizeMultiplier(multiplier);
        Settings.DamageMultiplier = normalized;
        Settings.Enabled = !Mathf.Approximately(normalized, 1f);
        if (Settings.Enabled && Settings.ShowUI)
        {
            EnsureDisplay();
            UpdateDisplay();
        }
        else
        {
            HideDisplay();
        }
    }

    internal static void EndChainDamage()
    {
        if (!chainActive)
        {
            return;
        }

        chainActive = false;
        Settings.Enabled = chainSavedEnabled;
        Settings.DamageMultiplier = chainSavedMultiplier;
        if (Settings.Enabled && Settings.ShowUI)
        {
            EnsureDisplay();
            UpdateDisplay();
        }
        else
        {
            HideDisplay();
        }
    }

    internal static float GetMultiplier() =>
        NormalizeMultiplier(Settings.DamageMultiplier);

    internal static bool GetEnabled() => Settings.Enabled;

    internal static void SetEnabled(bool value)
    {
        Settings.Enabled = value;
        GodhomeQoL.SaveGlobalSettingsSafe();

        if (!value)
        {
            HideDisplay();
            return;
        }

        if (Settings.ShowUI)
        {
            EnsureDisplay();
            UpdateDisplay();
        }
    }

    internal static void SetMultiplier(float value)
    {
        Settings.DamageMultiplier = NormalizeMultiplier(value);
        GodhomeQoL.SaveGlobalSettingsSafe();
        UpdateDisplay();
    }

    internal static bool GetUiVisible() => Settings.ShowUI;

    internal static void SetUiVisible(bool value)
    {
        Settings.ShowUI = value;
        GodhomeQoL.SaveGlobalSettingsSafe();

        if (value)
        {
            if (Settings.Enabled)
            {
                EnsureDisplay();
                UpdateDisplay();
            }
        }
        else
        {
            HideDisplay();
        }
    }

    internal static string GetToggleUiKeybind() => Settings.ToggleUiKeybind ?? string.Empty;

    internal static void SetToggleUiKeybind(string value)
    {
        Settings.ToggleUiKeybind = value;
        GodhomeQoL.SaveGlobalSettingsSafe();
    }

    internal static KeyCode GetToggleUiKey()
    {
        string raw = Settings.ToggleUiKeybind ?? string.Empty;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return KeyCode.None;
        }

        return Enum.TryParse(raw, true, out KeyCode key) ? key : KeyCode.None;
    }

    internal static void UpdateDisplay()
    {
        if (!Settings.Enabled || !Settings.ShowUI)
        {
            return;
        }

        EnsureDisplay();
        display?.Display($"Damage Multiplier: {FormatMultiplierLabel(GetMultiplier())}");
    }

    internal static void PreviewText()
    {
        EnsureDisplay();
        display?.ApplyScale();
        display?.Preview($"Damage Multiplier: {FormatMultiplierLabel(GetMultiplier())}");
    }

    private static float NormalizeMultiplier(float value)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
        {
            return 1f;
        }

        if (value < 1f)
        {
            return HalfMultiplier;
        }

        int integerMultiplier = Mathf.Clamp(Mathf.FloorToInt(value), MinMultiplier, MaxMultiplier);
        return integerMultiplier;
    }

    private static bool IsHalfMultiplier(float value) =>
        Mathf.Abs(value - HalfMultiplier) < 0.0001f;

    private static string FormatMultiplierLabel(float value)
    {
        return IsHalfMultiplier(value)
            ? "0.5"
            : Mathf.RoundToInt(value).ToString();
    }

    private static void EnsureDisplay()
    {
        display ??= new MaskDamageDisplay();
    }

    private static void HideDisplay()
    {
        display?.Hide();
    }

    private static void DestroyDisplay()
    {
        display?.Destroy();
        display = null;
    }

    private static int OnAfterTakeDamage(int hazardType, int damageAmount)
    {
        if (!Settings.Enabled)
        {
            return damageAmount;
        }

        if (damageAmount <= 0)
        {
            return damageAmount;
        }

        float multiplier = GetMultiplier();
        if (IsHalfMultiplier(multiplier))
        {
            if (damageAmount <= 1)
            {
                return 1;
            }

            return damageAmount / 2;
        }

        int integerMultiplier = Mathf.RoundToInt(multiplier);
        long scaled = (long)damageAmount * integerMultiplier;
        if (scaled > int.MaxValue)
        {
            return int.MaxValue;
        }

        if (scaled < int.MinValue)
        {
            return int.MinValue;
        }

        return (int)scaled;
    }

    private static void OnHeroUpdate()
    {
        if (!Settings.Enabled)
        {
            return;
        }

        if (QuickMenu.IsAnyUiVisible())
        {
            return;
        }

        if (QuickMenu.IsHotkeyInputBlocked())
        {
            return;
        }

        KeyCode key = GetToggleUiKey();
        if (key != KeyCode.None && Input.GetKeyDown(key))
        {
            SetUiVisible(!Settings.ShowUI);
        }
    }
}

internal sealed class MaskDamageDisplay
{
    private const float Left = 16f;
    private const float Bottom = 12f;
    private const float Gap = 10f;

    private HudLabel? label;

    public MaskDamageDisplay() => Create();

    private void Create()
    {
        if (label != null)
        {
            return;
        }

        label = new HudLabel("MaskDamageCanvas", Vector2.zero, new Vector2(Left, Bottom), 24, () => HudTextScales.MaskDamageText);
        label.AddUpdater(Place);
    }

    private void Place()
    {
        if (label == null)
        {
            return;
        }

        HudLabel? speed = ModDisplay.Instance?.MainLabel;
        float x = speed != null && speed.IsShown ? speed.Offset.x + speed.ScaledWidth + Gap : Left;
        label.Offset = new Vector2(x, Bottom);
    }

    public void Destroy()
    {
        label?.Destroy();
        label = null;
    }

    public void Hide()
    {
        label?.Hide();
    }

    public void Display(string value)
    {
        label?.Show(value);
        Place();
    }

    internal void ApplyScale()
    {
        label?.ApplyScale();
    }

    internal void Preview(string value)
    {
        label?.Preview(value);
        Place();
    }
}
