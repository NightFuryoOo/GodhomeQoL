using InControl;
using Satchel.BetterMenus;

namespace GodhomeQoL.Modules.Tools;

public sealed class ShowHPOnDeath : Module
{
    public override bool DefaultEnabled => false;
    public override bool Hidden => true;
    public override bool AlwaysEnabled => true;

    private sealed class BossState
    {
        public string DisplayName = "";
        public int CurrentHP;
    }

    private static readonly Dictionary<string, BossState> trackedBossesByKey = [];
    private static readonly List<string> trackedBossOrder = [];
    private static readonly Dictionary<int, string> bossKeyByInstanceId = [];
    private static readonly Dictionary<string, int> bossSignatureCounts = [];
    private static readonly Dictionary<string, int> initialHPs = [];
    private static readonly Dictionary<string, int> personalBestByBoss = [];
    private static ShowHPOnDeath? activeInstance;
    private static bool shouldTrackBosses = false;
    private static bool enteredFromWorkshop = false;
    private static string trackedScene = "";
    private static int trackedSceneHandle = -1;
    private static bool pendingTrackingStart = false;
    private static string pendingTrackingScene = "";
    private static bool pendingTrackingFromWorkshop = false;
    private static bool pendingTrackingClearDisplay = true;
    private static bool hasPendingDisplay = false;
    private static string pendingDisplayText = "";
    private static string lastDisplayText = "";
    private static bool isHudVisible = false;
    private static int trackingGeneration;
    private static int autoHideGeneration;
    private bool runtimeHooksInstalled;

    private static readonly HashSet<string> showAtScenes = new() { "GG_Workshop", "GG_Atrium", "GG_Atrium_Roof" };
    private static readonly HashSet<string> phaseBosses = new() { "False Knight", "Failed Champion" };
    private static readonly Dictionary<string, string> phaseBossScenes = new()
    {
        { "GG_False_Knight", "False Knight" },
        { "GG_Failed_Champion", "Failed Champion" }
    };
    private static readonly Dictionary<string, int> headHPs = new();
    private static readonly HashSet<string> headConfirmed = new();

    private sealed class PhaseTracker
    {
        public int CurrentPhase;
        public int[] MaxHP = new int[3];
        public int[] CurrentHP = new int[3];
        public bool[] PhaseDamaged = new bool[3];
        public int LastHP = -1;
    }

    private static readonly Dictionary<string, PhaseTracker> phaseTrackers = new();

    private static readonly HashSet<string> allowedBossScenes = new()
    {
        "GG_Gruz_Mother",
        "GG_Gruz_Mother_V",
        "GG_Vengefly",
        "GG_Vengefly_V",
        "GG_Brooding_Mawlek",
        "GG_Brooding_Mawlek_V",
        "GG_False_Knight",
        "GG_Failed_Champion",
        "GG_Hornet_1",
        "GG_Hornet_2",
        "GG_Mega_Moss_Charger",
        "GG_Flukemarm",
        "GG_Mantis_Lords",
        "GG_Mantis_Lords_V",
        "GG_Oblobbles",
        "GG_Hive_Knight",
        "GG_Broken_Vessel",
        "GG_Lost_Kin",
        "GG_Nosk",
        "GG_Nosk_V",
        "GG_Nosk_Hornet",
        "GG_Collector",
        "GG_Collector_V",
        "GG_God_Tamer",
        "GG_Crystal_Guardian",
        "GG_Crystal_Guardian_2",
        "GG_Uumuu",
        "GG_Uumuu_V",
        "GG_Traitor_Lord",
        "GG_Grey_Prince_Zote",
        "GG_Mage_Knight",
        "GG_Mage_Knight_V",
        "GG_Soul_Master",
        "GG_Soul_Tyrant",
        "GG_Dung_Defender",
        "GG_White_Defender",
        "GG_Watcher_Knights",
        "GG_Ghost_No_Eyes",
        "GG_Ghost_No_Eyes_V",
        "GG_Ghost_Marmu",
        "GG_Ghost_Marmu_V",
        "GG_Ghost_Xero",
        "GG_Ghost_Xero_V",
        "GG_Ghost_Markoth",
        "GG_Ghost_Markoth_V",
        "GG_Ghost_Galien",
        "GG_Ghost_Gorb",
        "GG_Ghost_Gorb_V",
        "GG_Ghost_Hu",
        "GG_Nailmasters",
        "GG_Painter",
        "GG_Sly",
        "GG_Hollow_Knight",
        "GG_Grimm",
        "GG_Grimm_Nightmare",
        "GG_Radiance"
    };

    private static ShowHPOnDeathSettings Settings => GodhomeQoL.GlobalSettings.ShowHPOnDeath ??= new ShowHPOnDeathSettings();

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

    internal static void ResetFeatureDefaults()
    {
        SetFeatureEnabled(false);
        Settings.ShowPB = true;
        Settings.HideAfter10Sec = true;
        Settings.HudFadeSeconds = 5f;
    }

    private protected override void Load()
    {
        activeInstance = this;
        runtimeHooksInstalled = false;
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

        Menu menu = new("ShowHPOnDeath".Localize(), elements.ToArray());
        return menu.GetMenuScreen(parent);
    }

    private void OnHeroUpdate()
    {
        if (!Settings.EnabledMod)
        {
            return;
        }

        TryBeginPendingTrackingAfterTransition();

        if (QuickMenu.IsHotkeyInputBlocked())
        {
            return;
        }

        if (Settings.Keybinds.Hide.WasPressed)
        {
            ToggleHud();
        }
    }

    private static void StartTracking(string sceneName, bool fromWorkshop, bool clearDisplay = true)
    {
        trackingGeneration++;
        shouldTrackBosses = true;
        enteredFromWorkshop = fromWorkshop;
        trackedScene = sceneName;
        Scene activeScene = USceneManager.GetActiveScene();
        trackedSceneHandle = activeScene.IsValid()
            && activeScene.isLoaded
            && string.Equals(activeScene.name, sceneName, StringComparison.Ordinal)
            ? activeScene.handle
            : -1;
        pendingTrackingStart = false;
        pendingTrackingScene = "";
        pendingTrackingFromWorkshop = false;
        pendingTrackingClearDisplay = true;
        lastDisplayText = string.Empty;
        ClearPendingDisplay();
        ClearTrackedBossCollection();
        phaseTrackers.Clear();
        headHPs.Clear();
        headConfirmed.Clear();
        if (clearDisplay)
        {
            UpdateDisplay("");
        }
    }

    private static void ResetTracking()
    {
        trackingGeneration++;
        shouldTrackBosses = false;
        enteredFromWorkshop = false;
        trackedScene = "";
        trackedSceneHandle = -1;
        pendingTrackingStart = false;
        pendingTrackingScene = "";
        pendingTrackingFromWorkshop = false;
        pendingTrackingClearDisplay = true;
        ClearTrackedBossCollection();
        phaseTrackers.Clear();
        headHPs.Clear();
        headConfirmed.Clear();
    }

    private static void ScheduleTrackingStart(string sceneName, bool fromWorkshop, bool clearDisplay)
    {
        trackingGeneration++;
        shouldTrackBosses = false;
        enteredFromWorkshop = false;
        trackedScene = "";
        trackedSceneHandle = -1;
        pendingTrackingStart = true;
        pendingTrackingScene = sceneName;
        pendingTrackingFromWorkshop = fromWorkshop;
        pendingTrackingClearDisplay = clearDisplay;
        lastDisplayText = string.Empty;
        ClearPendingDisplay();
        ClearTrackedBossCollection();
        phaseTrackers.Clear();
        headHPs.Clear();
        headConfirmed.Clear();
        if (clearDisplay)
        {
            UpdateDisplay("");
        }
    }

    private static void ClearTrackedBossCollection()
    {
        trackedBossesByKey.Clear();
        trackedBossOrder.Clear();
        bossKeyByInstanceId.Clear();
        bossSignatureCounts.Clear();
        initialHPs.Clear();
    }

    private static void ClearPendingDisplay()
    {
        hasPendingDisplay = false;
        pendingDisplayText = "";
    }

    private static void CancelAutoHide()
    {
        autoHideGeneration++;
    }

    private void ClearRuntimeState(bool clearLastDisplay)
    {
        CancelAutoHide();
        ClearPendingDisplay();
        ResetTracking();
        personalBestByBoss.Clear();
        if (clearLastDisplay)
        {
            lastDisplayText = "";
        }

        UpdateDisplay("");
    }

    private string BeforeSceneLoad(string newSceneName)
    {
        string currentScene = USceneManager.GetActiveScene().name;
        bool enteringFromWorkshop = currentScene == "GG_Workshop" && allowedBossScenes.Contains(newSceneName);
        bool enteringBossFromOther = !enteringFromWorkshop && allowedBossScenes.Contains(newSceneName) && currentScene != newSceneName;
        bool reloadingTrackedBoss = shouldTrackBosses && enteredFromWorkshop && allowedBossScenes.Contains(currentScene) && currentScene == trackedScene && currentScene == newSceneName;
        bool leavingTrackedBoss = shouldTrackBosses && allowedBossScenes.Contains(currentScene) && currentScene == trackedScene && currentScene != newSceneName;
        bool showingAtReturn = hasPendingDisplay && showAtScenes.Contains(newSceneName);

        if (!Settings.EnabledMod)
        {
            ClearRuntimeState(clearLastDisplay: true);
            return newSceneName;
        }
        if (enteringFromWorkshop)
        {
            StartTracking(newSceneName, fromWorkshop: true);
            return newSceneName;
        }

        if (reloadingTrackedBoss)
        {
            string text = BuildDisplayText();
            DisplayWithAutoHide(text);
            ScheduleTrackingStart(newSceneName, fromWorkshop: true, clearDisplay: false);
            return newSceneName;
        }

        if (enteringBossFromOther)
        {
            StartTracking(newSceneName, fromWorkshop: false);
            return newSceneName;
        }

        if (leavingTrackedBoss)
        {
            string text = BuildDisplayText();
            if (enteredFromWorkshop)
            {
                DisplayWithAutoHide(text);
            }
            else if (showAtScenes.Contains(newSceneName))
            {
                if (!BossSequenceController.WasCompleted)
                {
                    DisplayWithAutoHide(text);
                }
                ClearPendingDisplay();
            }
            else
            {
                hasPendingDisplay = true;
                pendingDisplayText = text;
            }

            ResetTracking();
            return newSceneName;
        }
        if (showingAtReturn)
        {
            if (!BossSequenceController.WasCompleted)
            {
                DisplayWithAutoHide(pendingDisplayText);
            }
            ClearPendingDisplay();
            ResetTracking();
            return newSceneName;
        }

        if (!shouldTrackBosses && !hasPendingDisplay)
        {
            ClearTrackedBossCollection();
            UpdateDisplay("");
        }

        return newSceneName;
    }

    private static void TryBeginPendingTrackingAfterTransition()
    {
        if (!pendingTrackingStart || string.IsNullOrEmpty(pendingTrackingScene))
        {
            return;
        }

        GameManager? manager = GameManager.instance;
        if (manager == null || manager.IsInSceneTransition)
        {
            return;
        }

        Scene activeScene = USceneManager.GetActiveScene();
        if (!activeScene.IsValid() || !activeScene.isLoaded)
        {
            return;
        }

        if (!string.Equals(activeScene.name, pendingTrackingScene, StringComparison.Ordinal))
        {
            return;
        }

        StartTracking(pendingTrackingScene, pendingTrackingFromWorkshop, pendingTrackingClearDisplay);
    }

    private void OnHealthManagerEnable(On.HealthManager.orig_OnEnable orig, HealthManager self)
    {
        orig(self);

        if (!Settings.EnabledMod || !shouldTrackBosses)
        {
            return;
        }

        if (BossNames.Map.TryGetValue(self.gameObject.name, out string displayName)
            && TryEnsureTrackedBossKey(self, displayName, out string bossKey))
        {
            UpdateTrackedBossHp(bossKey, self.hp);
            if (!initialHPs.ContainsKey(bossKey))
            {
                int generation = trackingGeneration;
                string sceneName = trackedScene;
                _ = GlobalCoroutineExecutor.Start(CaptureBossStateAfterDelay(self, bossKey, sceneName, generation));
            }
        }
    }

    private static IEnumerator CaptureBossStateAfterDelay(
        HealthManager source,
        string bossKey,
        string sceneName,
        int generation)
    {
        yield return new WaitForSecondsRealtime(1f);

        if (generation != trackingGeneration || !Settings.EnabledMod || !shouldTrackBosses)
        {
            yield break;
        }

        if (source == null || source.gameObject == null)
        {
            yield break;
        }

        if (!string.Equals(source.gameObject.scene.name, sceneName, StringComparison.Ordinal))
        {
            yield break;
        }

        HealthManager? hm = source.gameObject.GetComponent<HealthManager>();
        if (hm == null)
        {
            yield break;
        }

        if (!trackedBossesByKey.TryGetValue(bossKey, out BossState? state))
        {
            yield break;
        }

        int hp = Math.Max(hm.hp, 0);
        if (!initialHPs.ContainsKey(bossKey) && hp > 0)
        {
            initialHPs[bossKey] = hp;
        }

        state.CurrentHP = hp;
    }

    private static bool TryEnsureTrackedBossKey(HealthManager source, string displayName, out string bossKey)
    {
        bossKey = "";
        if (source == null || source.gameObject == null)
        {
            return false;
        }

        string sourceScene = source.gameObject.scene.name;
        if (!string.Equals(sourceScene, trackedScene, StringComparison.Ordinal))
        {
            return false;
        }

        int sourceSceneHandle = source.gameObject.scene.handle;
        if (trackedSceneHandle < 0)
        {
            Scene activeScene = USceneManager.GetActiveScene();
            if (activeScene.IsValid()
                && activeScene.isLoaded
                && string.Equals(activeScene.name, trackedScene, StringComparison.Ordinal))
            {
                trackedSceneHandle = activeScene.handle;
            }
            else
            {
                trackedSceneHandle = sourceSceneHandle;
            }
        }

        if (sourceSceneHandle != trackedSceneHandle)
        {
            return false;
        }

        int instanceId = source.GetInstanceID();
        if (bossKeyByInstanceId.TryGetValue(instanceId, out bossKey))
        {
            if (trackedBossesByKey.TryGetValue(bossKey, out BossState? existingState))
            {
                existingState.DisplayName = displayName;
            }
            return true;
        }

        string signature = $"{sourceScene}|{displayName}|{source.gameObject.name}";
        int slot = bossSignatureCounts.TryGetValue(signature, out int count) ? count + 1 : 1;
        bossSignatureCounts[signature] = slot;

        bossKey = $"{signature}#{slot}";
        bossKeyByInstanceId[instanceId] = bossKey;

        if (!trackedBossesByKey.ContainsKey(bossKey))
        {
            trackedBossesByKey[bossKey] = new BossState
            {
                DisplayName = displayName,
                CurrentHP = Math.Max(source.hp, 0)
            };
            trackedBossOrder.Add(bossKey);
        }

        return true;
    }

    private static void UpdateTrackedBossHp(string bossKey, int hp)
    {
        if (trackedBossesByKey.TryGetValue(bossKey, out BossState? state))
        {
            state.CurrentHP = Math.Max(hp, 0);
        }
    }

    private static bool IsCollectorImmortalTrackingSuppressed(GameObject? target = null)
    {
        return global::GodhomeQoL.Modules.CollectorPhases.CollectorPhases.IsCollectorImmortalityActiveFor(target);
    }

    private static bool IsZoteImmortalTrackingSuppressed(GameObject? target = null)
    {
        return global::GodhomeQoL.Modules.BossChallenge.ZoteHelper.IsGreyPrinceImmortalityActiveFor(target);
    }

    private static void RemoveTrackedBossEntriesByName(string displayName)
    {
        List<string> keysToRemove = trackedBossesByKey
            .Where(kvp => string.Equals(kvp.Value.DisplayName, displayName, StringComparison.Ordinal))
            .Select(kvp => kvp.Key)
            .ToList();

        if (keysToRemove.Count == 0)
        {
            return;
        }

        HashSet<string> keySet = [.. keysToRemove];
        for (int i = trackedBossOrder.Count - 1; i >= 0; i--)
        {
            if (keySet.Contains(trackedBossOrder[i]))
            {
                trackedBossOrder.RemoveAt(i);
            }
        }

        foreach (string key in keysToRemove)
        {
            trackedBossesByKey.Remove(key);
            initialHPs.Remove(key);
            personalBestByBoss.Remove(key);

            int hashIndex = key.LastIndexOf('#');
            if (hashIndex > 0)
            {
                bossSignatureCounts.Remove(key[..hashIndex]);
            }
        }

        foreach ((int instanceId, string key) in bossKeyByInstanceId.ToArray())
        {
            if (keySet.Contains(key))
            {
                bossKeyByInstanceId.Remove(instanceId);
            }
        }
    }

    private void OnHealthManagerUpdate(On.HealthManager.orig_Update orig, HealthManager self)
    {
        orig(self);
        if (!Settings.EnabledMod || !shouldTrackBosses)
        {
            return;
        }

        UpdateHeadCandidate(self);

        if (self.gameObject.name == "Jar Collector" && IsCollectorImmortalTrackingSuppressed(self.gameObject))
        {
            if (BossNames.Map.TryGetValue(self.gameObject.name, out string collectorName))
            {
                RemoveTrackedBossEntriesByName(collectorName);
            }

            return;
        }

        if (self.gameObject.name == "Grey Prince" && IsZoteImmortalTrackingSuppressed(self.gameObject))
        {
            if (BossNames.Map.TryGetValue(self.gameObject.name, out string zoteName))
            {
                RemoveTrackedBossEntriesByName(zoteName);
            }

            return;
        }

        if (!BossNames.Map.TryGetValue(self.gameObject.name, out string displayName))
        {
            return;
        }

        if (TryEnsureTrackedBossKey(self, displayName, out string bossKey))
        {
            int hp = Math.Max(self.hp, 0);
            UpdateTrackedBossHp(bossKey, hp);
            if (!initialHPs.ContainsKey(bossKey) && hp > 0)
            {
                initialHPs[bossKey] = hp;
            }
        }

        if (phaseBosses.Contains(displayName))
        {
            UpdatePhaseTracker(displayName, self.hp);
        }
    }

    private static PhaseTracker GetPhaseTracker(string displayName)
    {
        if (!phaseTrackers.TryGetValue(displayName, out PhaseTracker? tracker))
        {
            tracker = new PhaseTracker();
            phaseTrackers[displayName] = tracker;
        }
        return tracker;
    }

    private static void UpdatePhaseTracker(string displayName, int hp)
    {
        PhaseTracker tracker = GetPhaseTracker(displayName);
        int phaseIndex = Math.Min(tracker.CurrentPhase, 2);
        int clampedHp = Math.Max(hp, 0);

        if (tracker.MaxHP[phaseIndex] == 0)
        {
            tracker.MaxHP[phaseIndex] = clampedHp;
            tracker.CurrentHP[phaseIndex] = clampedHp;
            tracker.LastHP = clampedHp;
            return;
        }

        if (clampedHp > tracker.MaxHP[phaseIndex] && !tracker.PhaseDamaged[phaseIndex])
        {
            tracker.MaxHP[phaseIndex] = clampedHp;
        }

        if (tracker.LastHP >= 0 && clampedHp > tracker.LastHP && tracker.PhaseDamaged[phaseIndex] && phaseIndex < 2)
        {
            tracker.CurrentHP[phaseIndex] = 0;
            tracker.PhaseDamaged[phaseIndex] = true;
            tracker.CurrentPhase++;
            phaseIndex = tracker.CurrentPhase;
            tracker.MaxHP[phaseIndex] = clampedHp;
            tracker.CurrentHP[phaseIndex] = clampedHp;
            tracker.LastHP = clampedHp;
            return;
        }

        tracker.CurrentHP[phaseIndex] = Math.Min(clampedHp, tracker.MaxHP[phaseIndex]);
        if (tracker.CurrentHP[phaseIndex] < tracker.MaxHP[phaseIndex])
        {
            tracker.PhaseDamaged[phaseIndex] = true;
        }
        tracker.LastHP = clampedHp;
    }

    private static bool TryGetPhaseBossForScene(out string bossName)
    {
        string scene = USceneManager.GetActiveScene().name;
        return phaseBossScenes.TryGetValue(scene, out bossName);
    }

    private static void UpdateHeadCandidate(HealthManager self)
    {
        if (!TryGetPhaseBossForScene(out string bossName))
        {
            return;
        }

        if (BossNames.Map.TryGetValue(self.gameObject.name, out string mappedName) && mappedName == bossName)
        {
            return;
        }

        int hp = Math.Max(self.hp, 0);
        string objName = self.gameObject.name;
        bool nameLooksHead = objName.IndexOf("Head", StringComparison.OrdinalIgnoreCase) >= 0
            || objName.IndexOf("Hornhead", StringComparison.OrdinalIgnoreCase) >= 0;

        if (nameLooksHead)
        {
            headHPs[bossName] = hp;
            headConfirmed.Add(bossName);
            return;
        }

        if (hp == 0 || headConfirmed.Contains(bossName))
        {
            return;
        }

        if (!headHPs.TryGetValue(bossName, out int existing) || hp < existing)
        {
            headHPs[bossName] = hp;
        }
    }

    private static string BuildPhaseLines(string bossName, PhaseTracker tracker, int fallbackInitialHp)
    {
        int fallbackMax = tracker.MaxHP[Math.Min(tracker.CurrentPhase, 2)];
        if (fallbackMax == 0 && fallbackInitialHp > 0)
        {
            fallbackMax = fallbackInitialHp;
        }

        string phaseText = "";
        for (int i = 0; i < 3; i++)
        {
            int max = tracker.MaxHP[i] > 0 ? tracker.MaxHP[i] : fallbackMax;
            if (max <= 0)
            {
                continue;
            }

            int current;
            if (i < tracker.CurrentPhase)
            {
                current = 0;
            }
            else if (i == tracker.CurrentPhase)
            {
                current = Math.Min(tracker.CurrentHP[i], max);
            }
            else
            {
                current = max;
            }

            phaseText += $"HP: {current} / {max}\n";
        }

        int headHp = headHPs.TryGetValue(bossName, out int head) ? head : 0;
        phaseText += $"HP: {headHp} / 0\n";

        return phaseText;
    }

    private void OnPlayerDead()
    {
        if (!shouldTrackBosses)
        {
            return;
        }

        ShowBossDisplayForScene();
    }

    private void ShowBossDisplayForScene()
    {
        string displayText = BuildDisplayText();
        DisplayWithAutoHide(displayText);
    }

    private string BuildDisplayText()
    {
        var snapshot = new List<(string Key, string Name, int HP)>();
        bool suppressCollector = IsCollectorImmortalTrackingSuppressed();
        bool suppressZote = IsZoteImmortalTrackingSuppressed();
        foreach (string bossKey in trackedBossOrder)
        {
            if (!trackedBossesByKey.TryGetValue(bossKey, out BossState? state))
            {
                continue;
            }

            if (suppressCollector && string.Equals(state.DisplayName, "The Collector", StringComparison.Ordinal))
            {
                continue;
            }

            if (suppressZote && string.Equals(state.DisplayName, "Grey Prince Zote", StringComparison.Ordinal))
            {
                continue;
            }

            int hp = Math.Max(state.CurrentHP, 0);
            if (hp <= 0)
            {
                continue;
            }

            snapshot.Add((bossKey, state.DisplayName, hp));
            if (!personalBestByBoss.TryGetValue(bossKey, out int best) || hp < best)
            {
                personalBestByBoss[bossKey] = hp;
            }
        }

        string hideKey = Settings.Keybinds.Hide.UnfilteredBindings.Count > 0
            ? Settings.Keybinds.Hide.UnfilteredBindings[0].Name
            : "Unbound";
        string displayText = $"Press [{hideKey}] to hide\n";

        Dictionary<string, int> nameInstances = [];
        foreach (var (bossKey, name, currentHP) in snapshot)
        {
            int count = nameInstances.TryGetValue(name, out int existingCount) ? existingCount + 1 : 1;
            nameInstances[name] = count;
            string finalName = count > 1 ? $"{name} ({count})" : name;

            if (phaseBosses.Contains(name) && phaseTrackers.TryGetValue(name, out PhaseTracker? tracker))
            {
                int fallbackInitialHp = initialHPs.TryGetValue(bossKey, out int initialForPhase) ? initialForPhase : 0;
                string pbText = personalBestByBoss.TryGetValue(bossKey, out int pb) ? pb.ToString() : "-";
                displayText += $"[{finalName}]\n";
                displayText += BuildPhaseLines(name, tracker, fallbackInitialHp);
                if (Settings.ShowPB)
                {
                    displayText += $"PB: {pbText}\n";
                }
                continue;
            }

            if (initialHPs.TryGetValue(bossKey, out int initialHP))
            {
                string pbText = personalBestByBoss.TryGetValue(bossKey, out int pb) ? pb.ToString() : "-";
                displayText += $"[{finalName}]\nHP: {currentHP} / {initialHP}\n";
                if (Settings.ShowPB)
                {
                    displayText += $"PB: {pbText}\n";
                }
            }
            else
            {
                displayText += $"[{finalName}]\nHP: {currentHP}\n";
            }
        }

        return displayText;
    }

    private void DisplayWithAutoHide(string text)
    {
        int displayGeneration = ++autoHideGeneration;

        lastDisplayText = text;
        UpdateDisplay(text);

        if (Settings.HideAfter10Sec && !string.IsNullOrWhiteSpace(text))
        {
            float seconds = Math.Max(1f, Math.Min(10f, Settings.HudFadeSeconds));
            _ = GlobalCoroutineExecutor.Start(HideDisplayAfterDelay(displayGeneration, seconds));
        }
    }

    private static IEnumerator HideDisplayAfterDelay(int displayGeneration, float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        if (displayGeneration != autoHideGeneration)
        {
            yield break;
        }

        UpdateDisplay("");
    }

    private void CreateUI()
    {
        if (ShowHPOnDeathDisplay.Instance == null)
        {
            _ = new ShowHPOnDeathDisplay();
        }
    }

    private static void EnsureDisplayUi()
    {
        if (ShowHPOnDeathDisplay.Instance != null || activeInstance == null || !activeInstance.Loaded)
        {
            return;
        }

        activeInstance.CreateUI();
    }

    private static void UpdateDisplay(string text)
    {
        if (ShowHPOnDeathDisplay.Instance == null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                isHudVisible = false;
                return;
            }

            EnsureDisplayUi();
        }

        ShowHPOnDeathDisplay.Instance?.Display(text);
        isHudVisible = !string.IsNullOrWhiteSpace(text);
    }

    private void ToggleHud()
    {
        if (isHudVisible)
        {
            CancelAutoHide();
            UpdateDisplay("");
            return;
        }

        if (string.IsNullOrWhiteSpace(lastDisplayText))
        {
            return;
        }

        DisplayWithAutoHide(lastDisplayText);
    }
}
