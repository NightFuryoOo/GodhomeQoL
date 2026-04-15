namespace GodhomeQoL.Modules.BossChallenge;

internal static class PantheonSequenceCompatibility
{
    internal static bool IsSegmentedP5Active() =>
        ModuleManager.TryGetLoadedModule(typeof(SegmentedP5), out Module? module)
        && module.Enabled;

    internal static bool IsRandomPantheonsActive() =>
        RandomPantheons.AnyPantheonEnabled
        && ModuleManager.IsModuleLoaded(typeof(RandomPantheons));

    internal static bool ShouldSkipRandomPantheons() => IsSegmentedP5Active();

    internal static bool ShouldApplyRandomPantheonsRadianceEndingSceneOverride() =>
        IsRandomPantheonsActive() && !IsSegmentedP5Active();

    internal static bool ShouldSkipTrueBossRush() =>
        IsSegmentedP5Active() || IsRandomPantheonsActive();

    internal static bool DisableRandomPantheons()
    {
        bool changed = false;
        bool loaded = ModuleManager.TryGetLoadedModule(typeof(RandomPantheons), out _);
        bool quickMenuChanged = SyncRandomPantheonsQuickMenuState();

        if (RandomPantheons.AnyPantheonEnabled)
        {
            RandomPantheons.Pantheon1Enabled = false;
            RandomPantheons.Pantheon2Enabled = false;
            RandomPantheons.Pantheon3Enabled = false;
            RandomPantheons.Pantheon4Enabled = false;
            RandomPantheons.Pantheon5Enabled = false;
            changed = true;
        }

        if (loaded)
        {
            RefreshRandomPantheons();
        }

        if (ModuleManager.TryGetModule(typeof(RandomPantheons), out Module? module) && module.Enabled)
        {
            module.Enabled = false;
            changed = true;
        }

        if (quickMenuChanged)
        {
            changed = true;
        }

        if (changed)
        {
            GodhomeQoL.SaveGlobalSettingsSafe();
            Modules.Tools.QuickMenu.RefreshPantheonCompatibilityUi();
        }

        return changed;
    }

    internal static bool DisableTrueBossRush()
    {
        bool changed = false;
        bool loaded = ModuleManager.TryGetLoadedModule(typeof(TrueBossRush), out _);
        bool quickMenuChanged = SyncTrueBossRushQuickMenuState();

        if (TrueBossRush.AnyPantheonEnabled)
        {
            TrueBossRush.TrueBossRushPantheon1Enabled = false;
            TrueBossRush.TrueBossRushPantheon2Enabled = false;
            TrueBossRush.TrueBossRushPantheon3Enabled = false;
            TrueBossRush.TrueBossRushPantheon4Enabled = false;
            TrueBossRush.TrueBossRushPantheon5Enabled = false;
            changed = true;
        }

        if (loaded)
        {
            RefreshTrueBossRush();
        }

        if (ModuleManager.TryGetModule(typeof(TrueBossRush), out Module? module) && module.Enabled)
        {
            module.Enabled = false;
            changed = true;
        }

        if (quickMenuChanged)
        {
            changed = true;
        }

        if (changed)
        {
            GodhomeQoL.SaveGlobalSettingsSafe();
            Modules.Tools.QuickMenu.RefreshPantheonCompatibilityUi();
        }

        return changed;
    }

    internal static bool DisableSegmentedP5()
    {
        if (!ModuleManager.TryGetModule(typeof(SegmentedP5), out Module? module) || !module.Enabled)
        {
            return false;
        }

        module.Enabled = false;
        GodhomeQoL.SaveGlobalSettingsSafe();
        Modules.Tools.QuickMenu.RefreshPantheonCompatibilityUi();
        return true;
    }

    private static void RefreshRandomPantheons()
    {
        for (int i = 1; i <= 5; i++)
        {
            RandomPantheons.RefreshPantheon(i);
        }
    }

    private static void RefreshTrueBossRush()
    {
        for (int i = 1; i <= 5; i++)
        {
            TrueBossRush.RefreshPantheon(i);
        }
    }

    private static bool SyncRandomPantheonsQuickMenuState()
    {
        QuickMenuMasterSettings settings = GodhomeQoL.GlobalSettings.QuickMenuMasters ??= new QuickMenuMasterSettings();
        bool changed = false;

        if (settings.RandomPantheonsEnabled)
        {
            settings.RandomPantheonsEnabled = false;
            changed = true;
        }

        bool p1 = RandomPantheons.Pantheon1Enabled;
        bool p2 = RandomPantheons.Pantheon2Enabled;
        bool p3 = RandomPantheons.Pantheon3Enabled;
        bool p4 = RandomPantheons.Pantheon4Enabled;
        bool p5 = RandomPantheons.Pantheon5Enabled;
        bool anyPantheonEnabled = p1 || p2 || p3 || p4 || p5;

        if (!anyPantheonEnabled)
        {
            return changed;
        }

        if (!settings.RandomPantheonsHasSnapshot
            || settings.RandomPantheonsSavedP1 != p1
            || settings.RandomPantheonsSavedP2 != p2
            || settings.RandomPantheonsSavedP3 != p3
            || settings.RandomPantheonsSavedP4 != p4
            || settings.RandomPantheonsSavedP5 != p5)
        {
            settings.RandomPantheonsHasSnapshot = true;
            settings.RandomPantheonsSavedP1 = p1;
            settings.RandomPantheonsSavedP2 = p2;
            settings.RandomPantheonsSavedP3 = p3;
            settings.RandomPantheonsSavedP4 = p4;
            settings.RandomPantheonsSavedP5 = p5;
            changed = true;
        }

        return changed;
    }

    private static bool SyncTrueBossRushQuickMenuState()
    {
        QuickMenuMasterSettings settings = GodhomeQoL.GlobalSettings.QuickMenuMasters ??= new QuickMenuMasterSettings();
        bool changed = false;

        if (settings.TrueBossRushEnabled)
        {
            settings.TrueBossRushEnabled = false;
            changed = true;
        }

        bool p1 = TrueBossRush.TrueBossRushPantheon1Enabled;
        bool p2 = TrueBossRush.TrueBossRushPantheon2Enabled;
        bool p3 = TrueBossRush.TrueBossRushPantheon3Enabled;
        bool p4 = TrueBossRush.TrueBossRushPantheon4Enabled;
        bool p5 = TrueBossRush.TrueBossRushPantheon5Enabled;
        bool anyPantheonEnabled = p1 || p2 || p3 || p4 || p5;

        if (!anyPantheonEnabled)
        {
            return changed;
        }

        if (!settings.TrueBossRushHasSnapshot
            || settings.TrueBossRushSavedP1 != p1
            || settings.TrueBossRushSavedP2 != p2
            || settings.TrueBossRushSavedP3 != p3
            || settings.TrueBossRushSavedP4 != p4
            || settings.TrueBossRushSavedP5 != p5)
        {
            settings.TrueBossRushHasSnapshot = true;
            settings.TrueBossRushSavedP1 = p1;
            settings.TrueBossRushSavedP2 = p2;
            settings.TrueBossRushSavedP3 = p3;
            settings.TrueBossRushSavedP4 = p4;
            settings.TrueBossRushSavedP5 = p5;
            changed = true;
        }

        return changed;
    }
}
