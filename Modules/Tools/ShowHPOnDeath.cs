using InControl;
using Satchel.BetterMenus;
using System.Threading;
using System.Threading.Tasks;

namespace GodhomeQoL.Modules.Tools;

public sealed class ShowHPOnDeath : Module
{
    public override bool DefaultEnabled => true;
    public override bool Hidden => true;

    private static List<(string Name, int HP)> currentBosses = [];
    private static readonly Dictionary<string, int> initialHPs = [];
    private static int personalBest = int.MaxValue;
    private static string lastBossName = "";
    private static CancellationTokenSource? currentDisplayToken;
    private static bool shouldTrackBosses = false;
    private static bool enteredFromWorkshop = false;
    private static string trackedScene = "";
    private static bool hasPendingDisplay = false;
    private static string pendingDisplayText = "";
    private static string lastDisplayText = "";
    private static bool isHudVisible = false;

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

    private protected override void Load()
    {
        On.HealthManager.OnEnable += OnHealthManagerEnable;
        On.HealthManager.Update += OnHealthManagerUpdate;
        ModHooks.BeforeSceneLoadHook += BeforeSceneLoad;
        ModHooks.HeroUpdateHook += OnHeroUpdate;
        ModHooks.AfterPlayerDeadHook += OnPlayerDead;
        CreateUI();
    }

    private protected override void Unload()
    {
        On.HealthManager.OnEnable -= OnHealthManagerEnable;
        On.HealthManager.Update -= OnHealthManagerUpdate;
        ModHooks.BeforeSceneLoadHook -= BeforeSceneLoad;
        ModHooks.HeroUpdateHook -= OnHeroUpdate;
        ModHooks.AfterPlayerDeadHook -= OnPlayerDead;

        currentDisplayToken?.Cancel();
        currentDisplayToken?.Dispose();
        currentDisplayToken = null;

        ResetTracking();
        UpdateDisplay("");

        if (ShowHPOnDeathDisplay.Instance != null)
        {
            ShowHPOnDeathDisplay.Instance.Destroy();
        }
    }

    internal static MenuScreen GetMenu(MenuScreen parent)
    {
        List<Element> elements = new()
        {
            Blueprints.HorizontalBoolOption(
                "ShowHPOnDeath/GlobalSwitch".Localize(),
                "",
                b => Settings.EnabledMod = b,
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
        if (Settings.Keybinds.Hide.WasPressed)
        {
            ToggleHud();
        }
    }

    private void StartTracking(string sceneName, bool fromWorkshop, bool clearDisplay = true)
    {
        shouldTrackBosses = true;
        enteredFromWorkshop = fromWorkshop;
        trackedScene = sceneName;
        hasPendingDisplay = false;
        pendingDisplayText = "";
        currentBosses.Clear();
        initialHPs.Clear();
        phaseTrackers.Clear();
        headHPs.Clear();
        headConfirmed.Clear();
        if (clearDisplay)
        {
            UpdateDisplay("");
        }
    }

    private void ResetTracking()
    {
        shouldTrackBosses = false;
        enteredFromWorkshop = false;
        trackedScene = "";
        currentBosses.Clear();
        initialHPs.Clear();
        phaseTrackers.Clear();
        headHPs.Clear();
        headConfirmed.Clear();
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
            ResetTracking();
            UpdateDisplay("");
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
            ResetTracking();
            StartTracking(newSceneName, fromWorkshop: true, clearDisplay: false);
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
                hasPendingDisplay = false;
                pendingDisplayText = "";
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
            hasPendingDisplay = false;
            pendingDisplayText = "";
            ResetTracking();
            return newSceneName;
        }

        if (!shouldTrackBosses && !hasPendingDisplay)
        {
            currentBosses.Clear();
            initialHPs.Clear();
            UpdateDisplay("");
        }

        return newSceneName;
    }

    private void OnHealthManagerEnable(On.HealthManager.orig_OnEnable orig, HealthManager self)
    {
        if (!Settings.EnabledMod || !shouldTrackBosses)
        {
            orig(self);
            return;
        }
        if (BossNames.Map.TryGetValue(self.gameObject.name, out string displayName))
        {
            if (!initialHPs.ContainsKey(displayName))
            {
                _ = Task.Delay(1000).ContinueWith(_ =>
                {
                    HealthManager hm = self.gameObject.GetComponent<HealthManager>();
                    if (hm != null)
                    {
                        initialHPs[displayName] = hm.hp;
                    }
                });
            }

            if (!currentBosses.Any(b => b.Name == displayName))
            {
                _ = Task.Delay(1000).ContinueWith(_ =>
                {
                    HealthManager hm = self.gameObject.GetComponent<HealthManager>();
                    if (hm != null)
                    {
                        currentBosses.Add((displayName, hm.hp));
                    }
                });
            }
        }
        orig(self);
    }

    private void OnHealthManagerUpdate(On.HealthManager.orig_Update orig, HealthManager self)
    {
        orig(self);
        if (!Settings.EnabledMod || !shouldTrackBosses)
        {
            return;
        }

        UpdateHeadCandidate(self);

        if (BossNames.Map.TryGetValue(self.gameObject.name, out string displayName) && phaseBosses.Contains(displayName))
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

    private static string BuildPhaseLines(string displayName, PhaseTracker tracker)
    {
        int fallbackMax = tracker.MaxHP[Math.Min(tracker.CurrentPhase, 2)];
        if (fallbackMax == 0 && initialHPs.TryGetValue(displayName, out int initial))
        {
            fallbackMax = initial;
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

        int headHp = headHPs.TryGetValue(displayName, out int head) ? head : 0;
        phaseText += $"HP: {headHp} / 0\n";

        return phaseText;
    }

    private void OnPlayerDead()
    {
        if (!shouldTrackBosses)
        {
            return;
        }

        string sceneName = USceneManager.GetActiveScene().name;
        ShowBossDisplayForScene(sceneName);
    }

    private void ShowBossDisplayForScene(string sceneName)
    {
        string displayText = BuildDisplayText();
        DisplayWithAutoHide(displayText);
    }

    private string BuildDisplayText()
    {
        var snapshot = new List<(string Name, int HP)>();

        foreach (var boss in UObject.FindObjectsOfType<HealthManager>())
        {
            if (boss != null && boss.gameObject.activeInHierarchy && boss.hp > 0)
            {
                if (BossNames.Map.TryGetValue(boss.gameObject.name, out string displayName))
                {
                    snapshot.Add((displayName, boss.hp));

                    if (displayName == lastBossName)
                    {
                        if (boss.hp < personalBest)
                        {
                            personalBest = boss.hp;
                        }
                    }
                    else
                    {
                        personalBest = boss.hp;
                        lastBossName = displayName;
                    }
                }
            }
        }

        if (snapshot.Count > 0)
        {
            currentBosses = snapshot;
        }

        string hideKey = Settings.Keybinds.Hide.UnfilteredBindings.Count > 0
            ? Settings.Keybinds.Hide.UnfilteredBindings[0].Name
            : "Unbound";
        string displayText = $"Press [{hideKey}] to hide\n";

        for (int i = 0; i < currentBosses.Count; i++)
        {
            var (name, currentHP) = currentBosses[i];
            int count = currentBosses.Take(i).Count(b => b.Name == name);
            string finalName = count > 0 ? $"{name} ({count + 1})" : name;

            if (phaseBosses.Contains(name) && phaseTrackers.TryGetValue(name, out PhaseTracker? tracker))
            {
                string pbText = personalBest == int.MaxValue ? "-" : personalBest.ToString();
                displayText += $"[{finalName}]\n";
                displayText += BuildPhaseLines(name, tracker);
                if (Settings.ShowPB)
                {
                    displayText += $"PB: {pbText}\n";
                }
                continue;
            }

            if (initialHPs.TryGetValue(name, out int initialHP))
            {
                string pbText = personalBest == int.MaxValue ? "-" : personalBest.ToString();
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
        currentDisplayToken?.Cancel();
        currentDisplayToken?.Dispose();
        currentDisplayToken = new CancellationTokenSource();

        lastDisplayText = text;
        UpdateDisplay(text);

        if (Settings.HideAfter10Sec)
        {
            float seconds = Math.Max(1f, Math.Min(10f, Settings.HudFadeSeconds));
            int delayMs = (int)Math.Round(seconds * 1000f, MidpointRounding.AwayFromZero);
            _ = Task.Delay(delayMs, currentDisplayToken.Token)
                .ContinueWith(_ =>
                {
                    if (!_.IsCanceled)
                    {
                        UpdateDisplay("");
                    }
                }, TaskScheduler.Default);
        }
    }

    private void CreateUI()
    {
        if (ShowHPOnDeathDisplay.Instance == null)
        {
            _ = new ShowHPOnDeathDisplay();
        }
        else
        {
            ShowHPOnDeathDisplay.Instance.Destroy();
            _ = new ShowHPOnDeathDisplay();
        }
    }

    private void UpdateDisplay(string text)
    {
        ShowHPOnDeathDisplay.Instance?.Display(text);
        isHudVisible = !string.IsNullOrWhiteSpace(text);
    }

    private void ToggleHud()
    {
        if (isHudVisible)
        {
            currentDisplayToken?.Cancel();
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
