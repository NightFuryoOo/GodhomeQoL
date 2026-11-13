namespace GodhomeQoL.Modules.Tools;

public sealed partial class HitCounter : Module
{
    internal const int MinVisibleSplits = 1;
    internal const int MaxVisibleSplits = 15;
    internal const int DefaultVisibleSplits = 7;
    internal const float MinOpacity = 0.2f;
    internal const float MinScale = 0.5f;
    internal const float MaxScale = 1.5f;

    [GlobalSetting]
    public static bool ShowInHallOfGods = true;

    [GlobalSetting]
    public static int VisibleSplits = DefaultVisibleSplits;

    [GlobalSetting]
    public static float HudOpacity = 1f;

    [GlobalSetting]
    public static float HudScale = 1f;

    [GlobalSetting]
    public static string ToggleKeybind = string.Empty;

    [GlobalSetting]
    public static float HudPositionX = DefaultPositionX;

    [GlobalSetting]
    public static float HudPositionY = DefaultPositionY;

    internal const float DefaultPositionX = 1f;
    internal const float DefaultPositionY = 0.9f;

    private static bool moduleActive;

    public override bool DefaultEnabled => false;

    internal static bool IsActive => moduleActive;

    internal static int VisibleSplitsClamped => Mathf.Clamp(VisibleSplits, MinVisibleSplits, MaxVisibleSplits);

    internal static float OpacityClamped => Mathf.Clamp(HudOpacity, MinOpacity, 1f);

    internal static float ScaleClamped => Mathf.Clamp(HudScale, MinScale, MaxScale);

    private protected override void Load()
    {
        moduleActive = true;
        On.BossSequenceController.SetupNewSequence += OnSetupNewSequence;
        On.BossSceneController.EndBossScene += OnEndBossScene;
        On.HeroController.TakeDamage += OnTakeDamage;
        USceneManager.activeSceneChanged += OnSceneChanged;
        ModHooks.HeroUpdateHook += OnHeroUpdate;
    }

    private protected override void Unload()
    {
        moduleActive = false;
        On.BossSequenceController.SetupNewSequence -= OnSetupNewSequence;
        On.BossSceneController.EndBossScene -= OnEndBossScene;
        On.HeroController.TakeDamage -= OnTakeDamage;
        USceneManager.activeSceneChanged -= OnSceneChanged;
        ModHooks.HeroUpdateHook -= OnHeroUpdate;
        ClearTracking();
        HitCounterDisplay.Destroy();
    }

    internal static void ResetSettings()
    {
        ShowInHallOfGods = true;
        VisibleSplits = DefaultVisibleSplits;
        HudOpacity = 1f;
        HudScale = 1f;
        ToggleKeybind = string.Empty;
        HudPositionX = DefaultPositionX;
        HudPositionY = DefaultPositionY;
        if (session != null && session.HallOfGods && !ShowInHallOfGods)
        {
            session = null;
        }

        Render();
    }

    internal static void SetShowInHallOfGods(bool value)
    {
        ShowInHallOfGods = value;
        if (!value && session != null && session.HallOfGods)
        {
            session = null;
            Render();
        }
    }

    internal static void SetVisibleSplits(float value)
    {
        VisibleSplits = Mathf.Clamp(Mathf.RoundToInt(value), MinVisibleSplits, MaxVisibleSplits);
        Render();
    }

    internal static void SetOpacityPercent(float percent)
    {
        HudOpacity = Mathf.Clamp(Mathf.Round(percent), MinOpacity * 100f, 100f) / 100f;
        HitCounterDisplay.ApplyStyle();
    }

    internal static void SetScalePercent(float percent)
    {
        HudScale = Mathf.Clamp(Mathf.Round(percent), MinScale * 100f, MaxScale * 100f) / 100f;
        HitCounterDisplay.ApplyStyle();
    }

    internal static void SetPositionXPercent(float percent)
    {
        HudPositionX = Mathf.Clamp(Mathf.Round(percent), 0f, 100f) / 100f;
        HitCounterDisplay.ApplyStyle();
    }

    internal static void SetPositionYPercent(float percent)
    {
        HudPositionY = Mathf.Clamp(Mathf.Round(percent), 0f, 100f) / 100f;
        HitCounterDisplay.ApplyStyle();
    }

    internal static float PositionXClamped => Mathf.Clamp01(HudPositionX);

    internal static float PositionYClamped => Mathf.Clamp01(HudPositionY);

    internal static int PositionXPercent => Mathf.RoundToInt(PositionXClamped * 100f);

    internal static int PositionYPercent => Mathf.RoundToInt(PositionYClamped * 100f);

    internal static int OpacityPercent => Mathf.RoundToInt(OpacityClamped * 100f);

    internal static int ScalePercent => Mathf.RoundToInt(ScaleClamped * 100f);

    internal static KeyCode GetToggleKey()
    {
        return !string.IsNullOrWhiteSpace(ToggleKeybind) && Enum.TryParse(ToggleKeybind, true, out KeyCode key)
            ? key
            : KeyCode.None;
    }

    internal static void SetToggleKeybind(string value)
    {
        ToggleKeybind = value ?? string.Empty;
        GodhomeQoL.SaveGlobalSettingsSafe();
    }

    internal static void ResetRecordsForCurrentSave()
    {
        HitCounterRecords.ClearCurrentSlot();
        if (session != null)
        {
            foreach (HitCounterSplit split in session.Splits)
            {
                split.Pb = -1;
                split.Best = -1;
                split.BestAtStart = -1;
            }

            session.PbTotal = -1;
        }

        Render();
    }
}
