namespace GodhomeQoL.Modules.Tools;

internal sealed class HitCounterSplit
{
    internal string Key = string.Empty;
    internal string Scene = string.Empty;
    internal string Image = string.Empty;
    internal int Hits;
    internal bool Done;
    internal int Pb = -1;
    internal int Best = -1;
    internal int BestAtStart = -1;
}

internal sealed class HitCounterSession
{
    internal bool HallOfGods;
    internal bool Preview;
    internal int Tier;
    internal bool LevelResolved;
    internal bool Segmented;
    internal bool Custom;
    internal string Title = string.Empty;
    internal readonly List<HitCounterSplit> Splits = new();
    internal readonly HashSet<string> Scenes = new(StringComparer.Ordinal);
    internal int Current = -1;
    internal bool InSplit;
    internal bool Ended;
    internal bool Completed;
    internal bool NewPb;
    internal int PbTotal = -1;

    internal int Total => Splits.Sum(split => split.Hits);
}

public sealed partial class HitCounter : Module
{
    private const string DoorPrefix = "bossDoorStateTier";
    private const string WorkshopScene = "GG_Workshop";
    private const float PreviewSeconds = 3f;

    private static readonly string[] PantheonNames =
    {
        "Pantheon",
        "Pantheon of the Master",
        "Pantheon of the Artist",
        "Pantheon of the Sage",
        "Pantheon of the Knight",
        "Pantheon of Hallownest"
    };

    private static readonly string[] LevelNames = { "Attuned", "Ascended", "Radiant" };

    private static readonly HashSet<string> HubScenes = new(StringComparer.Ordinal)
    {
        "GG_Atrium",
        "GG_Atrium_Roof",
        "GG_Workshop",
        "GG_Boss_Door_Entrance",
        "GG_End_Sequence"
    };

    private static string? pendingDoor;
    private static HitCounterSession? session;
    private static HitCounterSession? previewSample;
    private static bool previewActive;
    private static int previewGeneration;
    private static bool hiddenByUser;
    private static bool moveMode;
    private static bool heroInputTaken;
    private static bool browsing;

    internal static void SetBrowsing(bool value)
    {
        if (browsing == value)
        {
            return;
        }

        browsing = value;
        if (!value)
        {
            HitCounterDisplay.ResetScroll();
        }

        if (moduleActive)
        {
            Render();
        }
    }

    internal static HitCounterSession? VisibleSession => session ?? (previewActive || moveMode ? previewSample ??= BuildPreview() : null);

    internal static bool IsMoving => moveMode;

    internal static void SetMoveMode(bool value)
    {
        if (moveMode == value)
        {
            return;
        }

        moveMode = value;
        if (value)
        {
            previewActive = false;
            previewGeneration++;
        }
        else
        {
            ReleaseHeroInput();
        }

        Render();
    }

    internal static void SetPositionRaw(float x, float y)
    {
        HudPositionX = Mathf.Clamp01(x);
        HudPositionY = Mathf.Clamp01(y);
    }

    private static void HoldHeroInput()
    {
        HeroController? hero = HeroController.instance;
        if (hero == null || !hero.acceptingInput)
        {
            return;
        }

        hero.IgnoreInput();
        heroInputTaken = true;
    }

    private static void ReleaseHeroInput()
    {
        if (!heroInputTaken)
        {
            return;
        }

        heroInputTaken = false;
        HeroController? hero = HeroController.instance;
        GameManager? manager = GameManager.instance;
        if (hero != null
            && !hero.acceptingInput
            && !hero.controlReqlinquished
            && !hero.cState.dead
            && manager != null
            && !manager.IsInSceneTransition)
        {
            hero.AcceptInput();
        }
    }

    internal static bool IsHiddenByUser => hiddenByUser;

    internal static string LevelName(int level) => level >= 0 && level < LevelNames.Length ? LevelNames[level] : string.Empty;

    private static void ClearTracking()
    {
        pendingDoor = null;
        session = null;
        previewActive = false;
        previewGeneration++;
        moveMode = false;
        ReleaseHeroInput();
    }

    private static void OnSetupNewSequence(
        On.BossSequenceController.orig_SetupNewSequence orig,
        BossSequence sequence,
        BossSequenceController.ChallengeBindings bindings,
        string playerData)
    {
        orig(sequence, bindings, playerData);
        pendingDoor = playerData ?? string.Empty;
    }

    private static void OnSceneChanged(Scene from, Scene to)
    {
        try
        {
            HandleSceneChange(from.name ?? string.Empty, to.name ?? string.Empty);
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "HitCounter.Tracking.cs/Scene");
        }

        Render();
    }

    private static void HandleSceneChange(string from, string scene)
    {
        if (session != null && session.Ended && !HubScenes.Contains(scene))
        {
            session = null;
        }

        if (session != null && !session.Ended)
        {
            if (session.HallOfGods)
            {
                if (scene != session.Splits[0].Scene)
                {
                    EndSession();
                }
                else if (from == scene)
                {
                    session = null;
                }
            }
            else if (session.Scenes.Contains(scene))
            {
                MoveTo(session, scene);
            }
            else
            {
                EndSession();
            }
        }

        if (session != null && !session.Ended)
        {
            return;
        }

        if (!BossHistory.TryGetBossForScene(scene, out string bossKey))
        {
            return;
        }

        if (pendingDoor != null)
        {
            string door = pendingDoor;
            pendingDoor = null;
            if (BossSequenceController.IsInSequence)
            {
                StartPantheon(door, scene);
                return;
            }
        }

        if (ShowInHallOfGods && (from == WorkshopScene || from == scene))
        {
            StartHallOfGods(bossKey, scene);
        }
    }

    private static void StartPantheon(string door, string scene)
    {
        BossSequence? sequence = BossSequenceControllerR.currentSequence;
        if (sequence == null)
        {
            return;
        }

        bool customChain = BossChallenge.CustomPantheonRunner.IsCustomDoor(door);
        bool customDoor = true;
        int tier = 0;
        if (!customChain)
        {
            tier = ParseTier(door, out customDoor);
        }

        HitCounterSession created = new()
        {
            Tier = tier,
            Custom = customChain,
            Title = customChain
                ? BossChallenge.CustomPantheonRunner.ActiveChainName
                : tier > 0 && tier < PantheonNames.Length ? PantheonNames[tier] : PantheonNames[0]
        };

        for (int i = 0; i < sequence.Count; i++)
        {
            if (!sequence.CanLoad(i))
            {
                continue;
            }

            string sceneName = sequence.GetSceneAt(i);
            created.Scenes.Add(sceneName);
            if (BossHistory.TryGetBossForScene(sceneName, out string key))
            {
                created.Splits.Add(new HitCounterSplit
                {
                    Key = key,
                    Scene = sceneName,
                    Image = BossHistory.Find(key)?.Image ?? string.Empty
                });
            }
        }

        if (created.Splits.Count == 0)
        {
            return;
        }

        HitCounterPantheonRecord? record = customChain ? null : HitCounterRecords.GetPantheon(tier);
        if (record != null)
        {
            created.PbTotal = record.PbTotal;
            foreach (HitCounterSplit split in created.Splits)
            {
                split.Pb = record.PbSplits.TryGetValue(split.Scene, out int pb) ? pb : -1;
                split.Best = record.BestSplits.TryGetValue(split.Scene, out int best) ? best : -1;
                split.BestAtStart = split.Best;
            }
        }

        MoveTo(created, scene);
        created.Segmented = customDoor || created.Current > 0;
        session = created;
    }

    private static void StartHallOfGods(string bossKey, string scene)
    {
        HitCounterSession created = new()
        {
            HallOfGods = true,
            Title = bossKey,
            Current = 0,
            InSplit = true
        };
        created.Splits.Add(new HitCounterSplit
        {
            Key = bossKey,
            Scene = scene,
            Image = BossHistory.Find(bossKey)?.Image ?? string.Empty
        });
        created.Scenes.Add(scene);
        session = created;
        ResolveHallOfGodsLevel();
    }

    private static void ResolveHallOfGodsLevel()
    {
        if (session == null || !session.HallOfGods || session.LevelResolved || !BossSceneController.IsBossScene)
        {
            return;
        }

        session.Tier = BossSceneController.Instance.BossLevel;
        session.LevelResolved = true;
        HitCounterSplit split = session.Splits[0];
        split.Pb = HitCounterRecords.GetHallOfGodsBest(split.Scene, session.Tier);
        split.Best = split.Pb;
        split.BestAtStart = split.Pb;
        Render();
    }

    private static void MoveTo(HitCounterSession target, string scene)
    {
        int start = Math.Max(target.Current, 0);
        for (int i = start; i < target.Splits.Count; i++)
        {
            if (string.Equals(target.Splits[i].Scene, scene, StringComparison.Ordinal) && !target.Splits[i].Done)
            {
                target.Current = i;
                target.InSplit = true;
                return;
            }
        }

        target.InSplit = false;
    }

    private static int ParseTier(string door, out bool customDoor)
    {
        customDoor = false;
        if (string.IsNullOrEmpty(door) || !door.StartsWith(DoorPrefix, StringComparison.Ordinal))
        {
            customDoor = true;
            return 0;
        }

        string rest = door.Substring(DoorPrefix.Length);
        int digits = 0;
        while (digits < rest.Length && char.IsDigit(rest[digits]))
        {
            digits++;
        }

        customDoor = digits < rest.Length;
        return digits > 0 && int.TryParse(rest.Substring(0, digits), out int tier) ? tier : 0;
    }

    private static void EndSession()
    {
        if (session == null || session.Ended)
        {
            return;
        }

        session.Ended = true;
        session.InSplit = false;
        if (session.HallOfGods)
        {
            return;
        }

        session.Completed = BossSequenceController.WasCompleted && session.Splits.All(split => split.Done);
        if (!session.Completed || session.Segmented)
        {
            return;
        }

        int total = session.Total;
        if (session.PbTotal >= 0 && total >= session.PbTotal)
        {
            return;
        }

        Dictionary<string, int> splits = new(StringComparer.Ordinal);
        foreach (HitCounterSplit split in session.Splits)
        {
            splits[split.Scene] = split.Hits;
        }

        HitCounterRecords.SavePantheonPb(session.Tier, total, splits);
        session.NewPb = true;
    }

    private static void OnTakeDamage(
        On.HeroController.orig_TakeDamage orig,
        HeroController self,
        GameObject go,
        GlobalEnums.CollisionSide damageSide,
        int damageAmount,
        int hazardType)
    {
        int before = CurrentHealth();
        orig(self, go, damageSide, damageAmount, hazardType);
        if (CurrentHealth() < before)
        {
            RegisterHit();
        }
    }

    private static int CurrentHealth()
    {
        PlayerData? data = PlayerData.instance;
        return data == null ? 0 : data.health + data.healthBlue;
    }

    private static void RegisterHit()
    {
        if (session == null || session.Ended || !session.InSplit || session.Current < 0 || session.Current >= session.Splits.Count)
        {
            return;
        }

        HitCounterSplit split = session.Splits[session.Current];
        if (split.Done)
        {
            return;
        }

        split.Hits++;
        Render();
    }

    private static void OnEndBossScene(On.BossSceneController.orig_EndBossScene orig, BossSceneController self)
    {
        orig(self);
        try
        {
            MarkCurrentDone();
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "HitCounter.Tracking.cs/EndBossScene");
        }
    }

    private static void MarkCurrentDone()
    {
        if (session == null || session.Ended || !session.InSplit || session.Current < 0 || session.Current >= session.Splits.Count)
        {
            return;
        }

        HitCounterSplit split = session.Splits[session.Current];
        if (split.Done)
        {
            return;
        }

        split.Done = true;
        if (session.HallOfGods)
        {
            ResolveHallOfGodsLevel();
            if (session.LevelResolved && (split.Best < 0 || split.Hits < split.Best))
            {
                split.Best = split.Hits;
                split.Pb = split.Hits;
                HitCounterRecords.SaveHallOfGodsBest(split.Scene, session.Tier, split.Hits);
            }
        }
        else if (!session.Custom && (split.Best < 0 || split.Hits < split.Best))
        {
            split.Best = split.Hits;
            HitCounterRecords.SavePantheonBest(session.Tier, split.Scene, split.Hits);
        }

        Render();
    }

    private static void OnHeroUpdate()
    {
        if (moveMode)
        {
            HoldHeroInput();
        }

        if (session != null && session.HallOfGods && !session.LevelResolved)
        {
            ResolveHallOfGodsLevel();
        }

        KeyCode key = GetToggleKey();
        if (key == KeyCode.None || QuickMenu.IsAnyUiVisible() || QuickMenu.IsHotkeyInputBlocked() || !Input.GetKeyDown(key))
        {
            return;
        }

        hiddenByUser = !hiddenByUser;
        Render();
    }

    internal static void ShowPreview()
    {
        if (!moduleActive)
        {
            return;
        }

        previewActive = true;
        int generation = ++previewGeneration;
        _ = GlobalCoroutineExecutor.Start(HidePreviewAfterDelay(generation));
        Render();
    }

    private static IEnumerator HidePreviewAfterDelay(int generation)
    {
        yield return new WaitForSecondsRealtime(PreviewSeconds);
        if (generation == previewGeneration)
        {
            previewActive = false;
            Render();
        }
    }

    private static HitCounterSession BuildPreview()
    {
        HitCounterSession sample = new()
        {
            Preview = true,
            Tier = 1,
            Title = PantheonNames[1],
            Current = 2,
            InSplit = true,
            PbTotal = 6
        };

        (string key, int hits, int pb, bool done)[] rows =
        {
            ("Vengefly King", 0, 1, true),
            ("Gruz Mother", 2, 1, true),
            ("False Knight", 1, 0, false),
            ("Massive Moss Charger", 0, 1, false),
            ("Hornet Protector", 0, 2, false),
            ("Gorb", 0, 0, false),
            ("Dung Defender", 0, 1, false),
            ("Soul Warrior", 0, 0, false),
            ("Brooding Mawlek", 0, 0, false),
            ("Oro & Mato", 0, 0, false)
        };

        foreach ((string key, int hits, int pb, bool done) in rows)
        {
            BossHistoryBoss? boss = BossHistory.Find(key);
            sample.Splits.Add(new HitCounterSplit
            {
                Key = key,
                Scene = boss?.Scenes.FirstOrDefault() ?? key,
                Image = boss?.Image ?? string.Empty,
                Hits = hits,
                Pb = pb,
                Best = pb,
                BestAtStart = pb,
                Done = done
            });
        }

        return sample;
    }

    internal static void Render()
    {
        try
        {
            HitCounterSession? visible = moduleActive && (!hiddenByUser || previewActive || moveMode) ? VisibleSession : null;
            if (visible != null && visible.HallOfGods && !ShowInHallOfGods && !moveMode)
            {
                visible = null;
            }

            HitCounterDisplay.Render(visible, previewActive && !moveMode, moveMode, browsing && !moveMode);
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "HitCounter.Tracking.cs/Render");
        }
    }
}
