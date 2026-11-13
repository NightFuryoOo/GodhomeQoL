using InControl;
using Satchel.BetterMenus;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class ShowHPOnDeath : Module
{
    internal static void SetFeatureEnabled(bool value)
    {
        Settings.EnabledMod = value;
        if (activeInstance == null || !activeInstance.Loaded)
        {
            return;
        }

        if (value)
        {
            activeInstance.InstallRuntimeHooks();
        }
        else
        {
            activeInstance.RemoveRuntimeHooks();
            activeInstance.ClearRuntimeState(clearLastDisplay: true);
            ShowHPOnDeathDisplay.Instance?.Destroy();
        }
    }

    internal static bool IsFeatureEnabled => Settings.EnabledMod;

    internal static void SyncFeatureState(bool previous)
    {
        bool target = Settings.EnabledMod;
        if (previous != target)
        {
            SetFeatureEnabled(target);
        }
    }

    internal static int OpacityPercent => Mathf.RoundToInt(ShowHPOnDeathDisplay.Opacity * 100f);

    internal static int ScalePercent => Mathf.RoundToInt(ShowHPOnDeathDisplay.Scale * 100f);

    internal static void SetOpacityPercent(float percent)
    {
        float clamped = Mathf.Clamp(Mathf.Round(percent), ShowHPOnDeathDisplay.MinOpacity * 100f, ShowHPOnDeathDisplay.MaxOpacity * 100f);
        Settings.HudOpacity = clamped / 100f;
        ShowHPOnDeathDisplay.Instance?.ApplyStyle();
    }

    internal static void SetScalePercent(float percent)
    {
        float clamped = Mathf.Clamp(Mathf.Round(percent), ShowHPOnDeathDisplay.MinScale * 100f, ShowHPOnDeathDisplay.MaxScale * 100f);
        Settings.HudScale = clamped / 100f;
        ShowHPOnDeathDisplay.Instance?.ApplyStyle();
    }

    internal static void ResetFeatureDefaults()
    {
        SetFeatureEnabled(false);
        Settings.ShowPB = true;
        Settings.HideAfter10Sec = true;
        Settings.HudFadeSeconds = 5f;
        Settings.HudOpacity = 1f;
        Settings.HudScale = 1f;
        ShowHPOnDeathDisplay.Instance?.ApplyStyle();
    }

    private protected override void Load()
    {
        activeInstance = this;
        runtimeHooksInstalled = false;
        InstallHistoryHooks();
        if (Settings.EnabledMod)
        {
            InstallRuntimeHooks();
        }
        else
        {
            ClearRuntimeState(clearLastDisplay: true);
        }
    }

    private protected override void Unload()
    {
        RemoveRuntimeHooks();
        RemoveHistoryHooks();
        ClearRuntimeState(clearLastDisplay: true);

        if (ShowHPOnDeathDisplay.Instance != null)
        {
            ShowHPOnDeathDisplay.Instance.Destroy();
        }

        activeInstance = null;
        runtimeHooksInstalled = false;
    }

    private void InstallRuntimeHooks()
    {
        if (runtimeHooksInstalled)
        {
            return;
        }

        On.HealthManager.OnEnable += OnHealthManagerEnable;
        On.HealthManager.Update += OnHealthManagerUpdate;
        ModHooks.BeforeSceneLoadHook += BeforeSceneLoad;
        ModHooks.HeroUpdateHook += OnHeroUpdate;
        ModHooks.AfterPlayerDeadHook += OnPlayerDead;
        On.HeroController.Die += OnHeroDie;
        On.HealthManager.Die += OnHealthManagerDie;
        On.BossSceneController.EndBossScene += OnEndBossScene;
        runtimeHooksInstalled = true;
    }

    private void RemoveRuntimeHooks()
    {
        if (!runtimeHooksInstalled)
        {
            return;
        }

        On.HealthManager.OnEnable -= OnHealthManagerEnable;
        On.HealthManager.Update -= OnHealthManagerUpdate;
        ModHooks.BeforeSceneLoadHook -= BeforeSceneLoad;
        ModHooks.HeroUpdateHook -= OnHeroUpdate;
        ModHooks.AfterPlayerDeadHook -= OnPlayerDead;
        On.HeroController.Die -= OnHeroDie;
        On.HealthManager.Die -= OnHealthManagerDie;
        On.BossSceneController.EndBossScene -= OnEndBossScene;
        runtimeHooksInstalled = false;
    }

    internal static MenuScreen GetMenu(MenuScreen parent)
    {
        List<Element> elements = new()
        {
            Blueprints.HorizontalBoolOption(
                "ShowHPOnDeath/GlobalSwitch".Localize(),
                "",
                SetFeatureEnabled,
                () => Settings.EnabledMod
            ),
            Blueprints.HorizontalBoolOption(
                "ShowHPOnDeath/ShowPB".Localize(),
                "",
                b => Settings.ShowPB = b,
                () => Settings.ShowPB
            ),
            Blueprints.HorizontalBoolOption(
                "ShowHPOnDeath/AutoHide".Localize(),
                "",
                b => Settings.HideAfter10Sec = b,
                () => Settings.HideAfter10Sec
            )
        };

        elements.Add(new KeyBind(
            "ShowHPOnDeath/HudToggleKey".Localize(),
            Settings.Keybinds.Hide,
            "show_hp_on_death_toggle_key"
        ));

        CustomSlider fadeSlider = new(
            "ShowHPOnDeath/HudFadeTime".Localize(),
            val =>
            {
                float clamped = Math.Max(1f, Math.Min(10f, val));
                Settings.HudFadeSeconds = (float)Math.Round(clamped, 1, MidpointRounding.AwayFromZero);
            },
            () => (float)Math.Round(Math.Max(1f, Math.Min(10f, Settings.HudFadeSeconds)), 1, MidpointRounding.AwayFromZero),
            1f,
            10f,
            false
        );
        elements.Add(fadeSlider);

        elements.Add(new CustomSlider(
            "ShowHPOnDeath/HudOpacity".Localize(),
            SetOpacityPercent,
            () => OpacityPercent,
            ShowHPOnDeathDisplay.MinOpacity * 100f,
            ShowHPOnDeathDisplay.MaxOpacity * 100f,
            true
        ));

        elements.Add(new CustomSlider(
            "ShowHPOnDeath/HudScale".Localize(),
            SetScalePercent,
            () => ScalePercent,
            ShowHPOnDeathDisplay.MinScale * 100f,
            ShowHPOnDeathDisplay.MaxScale * 100f,
            true
        ));

        Menu menu = new("ShowHPOnDeath".Localize(), elements.ToArray());
        return menu.GetMenuScreen(parent);
    }
}
