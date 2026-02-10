namespace GodhomeQoL.Modules.QoL;

public sealed class CarefreeMelodyReset : Module {
    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    internal const int ModeOff = 0;
    internal const int ModeHoG = 1;
    internal const int ModeHoGAndPantheons = 2;

    [GlobalSetting] public static int ResetMode = ModeOff;

    private const string HoGWorkshopScene = "GG_Workshop";
    private static readonly HashSet<string> NonBossGodhomeScenes = new()
    {
        "GG_Workshop",
        "GG_Atrium",
        "GG_Atrium_Roof",
        "GG_Spa"
    };

    private static bool enteredBossFromWorkshop;
    private static string? trackedBossScene;
    private static string? lastHubScene;

    private protected override void Load()
    {
        if (GetMode() == ModeOff)
        {
            ResetMode = ModeHoG;
        }

        OsmiHooks.SceneChangeHook += ResetCount;
    }

    private protected override void Unload() =>
        OsmiHooks.SceneChangeHook -= ResetCount;

    private static void ResetCount(Scene prev, Scene next)
    {
        string prevName = prev.name;
        string nextName = next.name;
        bool nextIsBoss = IsBossSceneName(nextName);

        if (IsHubSceneName(nextName))
        {
            lastHubScene = nextName;
        }

        if (nextIsBoss)
        {
            enteredBossFromWorkshop = string.Equals(lastHubScene, HoGWorkshopScene, StringComparison.OrdinalIgnoreCase);
            trackedBossScene = nextName;
        }
        else if (trackedBossScene != null && prevName == trackedBossScene && nextName == trackedBossScene)
        {
            // Boss reload; keep the flag.
        }
        else if (!nextIsBoss)
        {
            enteredBossFromWorkshop = false;
            trackedBossScene = null;
        }

        TryResetNow();
    }

    internal static void TryResetNow(bool ignoreBossScene = false) {
        if (Ref.HC == null) {
            return;
        }

        int mode = GetMode();
        if (mode == ModeOff)
        {
            return;
        }

        if (!Ref.HC.carefreeShieldEquipped) {
            return;
        }

        bool allow = mode switch
        {
            ModeHoG => enteredBossFromWorkshop,
            ModeHoGAndPantheons => enteredBossFromWorkshop || BossSequenceController.IsInSequence,
            _ => false
        };

        if (!allow)
        {
            return;
        }

        if (!ignoreBossScene && !BossSceneController.IsBossScene)
        {
            return;
        }

        HeroControllerR.hitsSinceShielded = 7;
        LogDebug("Carefree Melody hit count reset to max");
    }

    internal static int GetMode() =>
        Mathf.Clamp(ResetMode, ModeOff, ModeHoGAndPantheons);

    internal static void SetMode(int value)
    {
        int clamped = Mathf.Clamp(value, ModeOff, ModeHoGAndPantheons);
        ResetMode = clamped;

        if (ModuleManager.TryGetModule(typeof(CarefreeMelodyReset), out Module? module))
        {
            module.Enabled = clamped != ModeOff;
        }
    }

    internal static void MarkEnteredFromWorkshop(string bossScene)
    {
        if (string.IsNullOrEmpty(bossScene))
        {
            return;
        }

        if (!IsBossSceneName(bossScene))
        {
            return;
        }

        enteredBossFromWorkshop = true;
        trackedBossScene = bossScene;
    }

    private static bool IsBossSceneName(string sceneName)
    {
        if (!sceneName.StartsWith("GG_"))
        {
            return false;
        }

        return !NonBossGodhomeScenes.Contains(sceneName);
    }

    private static bool IsHubSceneName(string sceneName)
    {
        return NonBossGodhomeScenes.Contains(sceneName);
    }
}
