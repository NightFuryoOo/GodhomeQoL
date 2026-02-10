namespace GodhomeQoL.Modules.Tools;

public sealed class MaskDamage : Module
{
    private const int MinMultiplier = 1;
    private const int MaxMultiplier = 999;

    public override bool DefaultEnabled => false;
    public override bool Hidden => true;

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

    internal static int GetMultiplier() =>
        Mathf.Clamp(Settings.DamageMultiplier, MinMultiplier, MaxMultiplier);

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

    internal static void SetMultiplier(int value)
    {
        Settings.DamageMultiplier = Mathf.Clamp(value, MinMultiplier, MaxMultiplier);
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
            EnsureDisplay();
            UpdateDisplay();
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
        display?.Display($"Damage Multiplier: {GetMultiplier()}");
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

        int multiplier = GetMultiplier();
        return damageAmount * multiplier;
    }

    private static void OnHeroUpdate()
    {
        if (!Settings.Enabled)
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
    private string displayText = "";
    private readonly Vector2 textSize = new(800, 500);
    private readonly Vector2 textPosition = new(0.310f, 0.243f);

    private GameObject? canvas;
    private UnityEngine.UI.Text? text;

    public MaskDamageDisplay() => Create();

    private void Create()
    {
        if (canvas != null)
        {
            return;
        }

        canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080), "MaskDamageCanvas");

        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        UObject.DontDestroyOnLoad(canvas);

        text = CanvasUtil.CreateTextPanel(
            canvas,
            "",
            24,
            TextAnchor.LowerLeft,
            new CanvasUtil.RectData(textSize, Vector2.zero, textPosition, textPosition),
            CanvasUtil.GetFont("Perpetua")
        ).GetComponent<UnityEngine.UI.Text>();
    }

    public void Destroy()
    {
        if (canvas != null)
        {
            UObject.Destroy(canvas);
        }

        canvas = null;
        text = null;
    }

    public void Hide()
    {
        if (canvas != null)
        {
            canvas.SetActive(false);
        }
    }

    private void Update()
    {
        if (text == null || canvas == null)
        {
            return;
        }

        text.text = displayText;
        canvas.SetActive(true);
    }

    public void Display(string value)
    {
        displayText = value.Trim();
        Update();
    }
}
