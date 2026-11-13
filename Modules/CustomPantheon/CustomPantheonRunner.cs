using Vasi;

namespace GodhomeQoL.Modules.BossChallenge;

internal static class CustomPantheonRunner
{
    internal const string DoorKey = "GodhomeQoLCustomPantheon";
    private const string SequenceName = "GodhomeQoL Custom Pantheon";
    private const string AtriumScene = "GG_Atrium";
    private const string WorkshopScene = CustomPantheonStatue.WorkshopScene;
    private const string WorkshopEntryGate = "left1";
    private const string EndScene = "GG_End_Sequence";
    private const string DefaultReturnGate = "door_dreamReturnGG_GG_Challenge_Door";
    private const string DreamReturnGate = "door_dreamReturn";
    private const string EntryGate = "door_dreamEnter";
    private const string AtriumDetourGate = "top1";
    private const string ChallengeClip = "Challenge Start";
    private const float ReadyTimeoutSeconds = 8f;
    private const float LoadTimeoutSeconds = 15f;
    private const float ReturnTimeoutSeconds = 10f;
    private const float NearStatueDistance = 1.2f;
    private const float BlackFadeSeconds = 0.35f;
    private const float WhiteFadeSeconds = 0.6f;
    private const int FallbackNailDamage = 13;
    private const float FallbackNailPercentage = 0.8f;
    private const int FallbackMaxHealth = 4;

    private static readonly Vector3 AtriumDetourPosition = new(97.15343f, 35.40812f, 0f);
    private static readonly string[] FallbackReturnGates = { "door_dreamEnter_ReturnGGMode", "door_dreamEnter" };

    private sealed class StepResult
    {
        internal bool Ok;
    }

    private readonly struct Vitals
    {
        internal readonly int Health;
        internal readonly int Lifeblood;
        internal readonly int Soul;

        internal Vitals(int health, int lifeblood, int soul)
        {
            Health = health;
            Lifeblood = lifeblood;
            Soul = soul;
        }
    }
    private static readonly FieldInfo BossScenesField = typeof(BossSequence).GetField("bossScenes", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
    private static readonly FieldInfo? ForceAssetUnloadField = typeof(BossScene).GetField("forceAssetUnload", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    private enum Phase
    {
        Idle,
        Preparing,
        Running,
        Returning
    }

    private static Phase phase;
    private static int generation;
    private static bool hooked;
    private static BossSequence? sequence;
    private static BossSequenceController.ChallengeBindings bindings;
    private static string chainName = string.Empty;
    private static string? previousDreamReturnScene;
    private static readonly HashSet<string> runScenes = new(StringComparer.Ordinal);
    private static readonly List<UObject> createdAssets = new();
    private static readonly List<CustomPantheonFight> runFights = new();
    private static GearPreset? runFullGear;
    private static int currentFightIndex = -1;
    private static bool effectsApplied;
    private static bool firstSceneFadePending;
    private static PlayMakerFSM? statueConversation;
    private static bool victoryReturn;
    private static bool firstFightEntered;
    private static bool controlTaken;
    private static bool blackShown;
    private static bool whiteShown;

    private static GearPreset GearFor(CustomPantheonFight? fight) =>
        fight?.Gear ?? (runFullGear ??= CreateFullGear());

    private static GearPreset CreateFullGear() =>
        GearPresetDefaults.CreateDefaults().TryGetValue("FullGear", out GearPreset? full) && full != null ? full : new GearPreset();

    internal static bool IsActive => phase != Phase.Idle;

    internal static string ActiveChainName => chainName;

    internal static bool IsCustomDoor(string? door) => string.Equals(door, DoorKey, StringComparison.Ordinal);

    internal static string[] DescribeActiveRun()
    {
        System.Globalization.CultureInfo invariant = System.Globalization.CultureInfo.InvariantCulture;
        string[] lines = new string[runFights.Count];
        for (int i = 0; i < runFights.Count; i++)
        {
            CustomPantheonFight fight = runFights[i];
            if (CustomPantheonCatalog.IsBench(fight.Scene))
            {
                lines[i] = $"{i + 1}. {fight.Scene} | bench";
                continue;
            }

            string caption = string.IsNullOrWhiteSpace(fight.Caption) ? "-" : fight.Caption.Trim();
            float speed = fight.GameSpeed > 0f ? fight.GameSpeed : 1f;
            lines[i] = string.Join(" | ", new[]
            {
                $"{i + 1}. {fight.Scene}",
                $"caption: {caption}",
                $"speed: {speed.ToString("0.##", invariant)}",
                $"mask damage: {Math.Max(1, fight.MaskDamage)}",
                $"restore health: {(fight.RestoreHealth ? "on" : "off")}",
                $"infinite hp: {(fight.InfiniteHp ? "on" : "off")}",
                $"soul: {(fight.Soul.HasValue ? fight.Soul.Value.ToString(invariant) : "-")}",
                $"lifeblood: {(fight.Lifeblood.HasValue ? fight.Lifeblood.Value.ToString(invariant) : "-")}",
                $"gear: {(fight.Gear == null ? "FullGear" : "Custom")}",
                $"boss settings: {(fight.HasBossSettings ? "on" : "off")}"
            });
        }

        return lines;
    }

    internal static bool IsCustomSequence(BossSequence? candidate) =>
        candidate != null && (ReferenceEquals(candidate, sequence) || string.Equals(candidate.name, SequenceName, StringComparison.Ordinal));

    internal static bool CanOfferStart(CustomPantheonChain? chain) =>
        phase == Phase.Idle
        && chain != null
        && CustomPantheonCatalog.CountBosses(CustomPantheonCatalog.SanitizeFights(chain.Fights).Select(fight => fight.Scene).ToList()) > 0;

    internal static bool TryStart(CustomPantheonChain? chain, out string messageKey) =>
        TryStartInternal(chain, null, out messageKey);

    internal static bool TryStartAtStatue(CustomPantheonChain? chain, PlayMakerFSM conversation, out string messageKey) =>
        TryStartInternal(chain, conversation, out messageKey);

    private static bool TryStartInternal(CustomPantheonChain? chain, PlayMakerFSM? conversation, out string messageKey)
    {
        messageKey = string.Empty;
        if (chain == null)
        {
            messageKey = "CustomPantheon/NoSelection";
            return false;
        }

        if (phase != Phase.Idle)
        {
            messageKey = "CustomPantheon/Busy";
            return false;
        }

        List<CustomPantheonFight> fights = CustomPantheonCatalog.SanitizeFights(chain.Fights);
        List<string> scenes = fights.Select(fight => fight.Scene).ToList();
        if (CustomPantheonCatalog.CountBosses(scenes) == 0)
        {
            messageKey = "CustomPantheon/Empty";
            return false;
        }

        if (!CanStartHere())
        {
            messageKey = "CustomPantheon/NotNow";
            return false;
        }

        try
        {
            DestroyCreatedAssets();
            sequence = BuildSequence(scenes);
        }
        catch (Exception ex)
        {
            LogError($"[CustomPantheon] Failed to build the chain: {ex}");
            messageKey = "CustomPantheon/Failed";
            return false;
        }

        runScenes.Clear();
        foreach (string scene in scenes)
        {
            runScenes.Add(scene);
        }

        runFights.Clear();
        runFights.AddRange(fights);
        runFullGear = null;
        currentFightIndex = -1;
        effectsApplied = false;
        bindings = BossSequenceController.ChallengeBindings.None;
        chainName = string.IsNullOrWhiteSpace(chain.Name) ? "CustomPantheon/Title".Localize() : chain.Name.Trim();
        statueConversation = conversation;
        victoryReturn = false;
        firstFightEntered = false;
        Tools.GearSwitcher.PreserveHealthOnApply = false;
        controlTaken = false;
        blackShown = false;
        whiteShown = false;
        phase = Phase.Preparing;
        int token = ++generation;
        InstallHooks();
        _ = GlobalCoroutineExecutor.Start(StartRoutine(token, scenes[0], conversation != null));
        return true;
    }

    private static bool CanStartHere()
    {
        GameManager? manager = GameManager.instance;
        HeroController? hero = HeroController.instance;
        return manager != null
            && hero != null
            && manager.gameState == GameState.PLAYING
            && manager.IsGameplayScene()
            && !manager.IsInSceneTransition
            && !manager.IsGamePaused()
            && !hero.cState.dead
            && !hero.cState.transitioning
            && !BossSceneController.IsBossScene
            && PlayerData.instance != null
            && !PlayerData.instance.GetBool("atBench")
            && !string.Equals(manager.sceneName, EndScene, StringComparison.Ordinal)
            && !Tools.SpeedChanger.IsTimeScaleFrozen
            && !Tools.FreezeHitboxes.IsFreezeActive();
    }

    private static bool IsHeroReady()
    {
        GameManager? manager = GameManager.instance;
        HeroController? hero = HeroController.instance;
        return manager != null
            && hero != null
            && manager.gameState == GameState.PLAYING
            && !manager.IsInSceneTransition
            && !manager.IsGamePaused()
            && !hero.cState.dead
            && !hero.cState.transitioning;
    }

    private static bool IsPreparing(int token) => token == generation && phase == Phase.Preparing;

    private static IEnumerator StartRoutine(int token, string firstScene, bool atStatue)
    {
        StepResult result = new();
        if (!atStatue)
        {
            float waitUntil = Time.realtimeSinceStartup + 1f;
            yield return null;
            while (Tools.QuickMenu.IsAnyUiVisible() && Time.realtimeSinceStartup < waitUntil)
            {
                yield return null;
            }

            yield return null;
            if (!IsPreparing(token))
            {
                yield break;
            }

            yield return TravelToStatue(token, result);
            if (!IsPreparing(token))
            {
                yield break;
            }

            if (!result.Ok)
            {
                Abort(token);
                yield break;
            }
        }

        result.Ok = false;
        yield return PlayChallenge(token, result, !atStatue);
        if (!IsPreparing(token))
        {
            yield break;
        }

        if (!result.Ok)
        {
            Abort(token);
            yield break;
        }

        result.Ok = false;
        yield return HopThroughAtrium(token, result);
        if (!IsPreparing(token) || sequence == null)
        {
            yield break;
        }

        if (!result.Ok || BossSceneController.IsBossScene)
        {
            Abort(token);
            yield break;
        }

        Begin(firstScene);
    }

    private static IEnumerator WaitForScene(int token, string scene, StepResult result)
    {
        result.Ok = false;
        float deadline = Time.realtimeSinceStartup + LoadTimeoutSeconds;
        yield return null;
        while (GameManager.instance == null
            || GameManager.instance.IsInSceneTransition
            || !string.Equals(GameManager.instance.sceneName, scene, StringComparison.Ordinal))
        {
            if (!IsPreparing(token) || Time.realtimeSinceStartup > deadline)
            {
                yield break;
            }

            yield return null;
        }

        while (!IsHeroReady())
        {
            if (!IsPreparing(token) || Time.realtimeSinceStartup > deadline)
            {
                yield break;
            }

            yield return null;
        }

        result.Ok = true;
    }

    private static void PlaceHero(HeroController hero, Vector3 position)
    {
        hero.transform.position = position;
        Rigidbody2D? body = hero.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.velocity = Vector2.zero;
        }
    }

    private static IEnumerator TravelToStatue(int token, StepResult result)
    {
        result.Ok = false;
        GameManager? manager = GameManager.instance;
        HeroController? hero = HeroController.instance;
        if (manager == null || hero == null)
        {
            yield break;
        }

        bool inWorkshop = string.Equals(manager.sceneName, WorkshopScene, StringComparison.Ordinal);
        if (inWorkshop
            && CustomPantheonStatue.TryGetStandPoint(out Vector3 near, out _)
            && Vector2.Distance(hero.transform.position, near) <= NearStatueDistance)
        {
            result.Ok = true;
            yield break;
        }

        blackShown = true;
        CustomPantheonStatue.FadeBlanker(false, true, BlackFadeSeconds);
        yield return new WaitForSecondsRealtime(BlackFadeSeconds + 0.05f);
        if (!IsPreparing(token))
        {
            yield break;
        }

        if (!inWorkshop)
        {
            Rigidbody2D? body = hero.GetComponent<Rigidbody2D>();
            if (body != null)
            {
                body.velocity = Vector2.zero;
            }

            manager.BeginSceneTransition(new GameManager.SceneLoadInfo
            {
                SceneName = WorkshopScene,
                EntryGateName = WorkshopEntryGate,
                EntryDelay = 0f,
                Visualization = GameManager.SceneLoadVisualizations.Default,
                PreventCameraFadeOut = false,
                WaitForSceneTransitionCameraFade = false
            });

            StepResult loaded = new();
            yield return WaitForScene(token, WorkshopScene, loaded);
            if (!loaded.Ok)
            {
                yield break;
            }
        }

        hero = HeroController.instance;
        if (hero == null || !CustomPantheonStatue.TryGetStandPoint(out Vector3 stand, out _))
        {
            yield break;
        }

        hero.RelinquishControl();
        controlTaken = true;
        PlaceHero(hero, stand);
        hero.FaceRight();
        GameManager.instance.cameraCtrl?.PositionToHero(false);
        yield return null;
        PlaceHero(hero, stand);
        yield return new WaitForSecondsRealtime(0.35f);
        if (!IsPreparing(token))
        {
            yield break;
        }

        CustomPantheonStatue.FadeBlanker(false, false, 0.5f);
        blackShown = false;
        yield return new WaitForSecondsRealtime(0.45f);
        result.Ok = HeroController.instance != null;
    }

    private static void SetDreamConvo(HeroController hero, bool value)
    {
        PlayMakerFSM? dreamNail = hero.gameObject.LocateMyFSM("Dream Nail");
        FsmBool? flag = dreamNail != null ? dreamNail.FsmVariables.FindFsmBool("Dream Convo") : null;
        if (flag != null)
        {
            flag.Value = value;
        }
    }

    private static IEnumerator PlayChallenge(int token, StepResult result, bool silenceStatue)
    {
        result.Ok = false;
        HeroController? hero = HeroController.instance;
        if (hero == null)
        {
            yield break;
        }

        if (silenceStatue)
        {
            CustomPantheonStatue.DisableStatuePrompt();
        }

        PlayerData.instance.SetBool("disablePause", true);
        hero.RelinquishControl();
        hero.StopAnimationControl();
        controlTaken = true;
        SetDreamConvo(hero, true);
        Rigidbody2D? body = hero.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.velocity = Vector2.zero;
        }

        if (CustomPantheonStatue.TryGetStandPoint(out _, out float statueX))
        {
            if (hero.transform.position.x <= statueX)
            {
                hero.FaceRight();
            }
            else
            {
                hero.FaceLeft();
            }
        }

        CustomPantheonStatue.ChallengeAssets assets = CustomPantheonStatue.FindChallengeAssets();
        AudioSource? heroAudio = hero.GetComponent<AudioSource>();
        tk2dSpriteAnimator? animator = hero.GetComponent<tk2dSpriteAnimator>();
        bool triggered = false;
        bool completed = false;
        Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int> onTrigger = (anim, clip, frame) => triggered = true;
        Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip> onComplete = (anim, clip) => completed = true;
        if (animator != null)
        {
            animator.AnimationEventTriggered += onTrigger;
            animator.AnimationCompleted += onComplete;
            animator.Play(ChallengeClip);
        }

        try
        {
            float deadline = Time.realtimeSinceStartup + 1.5f;
            while (!triggered && !completed && Time.realtimeSinceStartup < deadline && IsPreparing(token))
            {
                yield return null;
            }

            if (!IsPreparing(token))
            {
                yield break;
            }

            if (heroAudio != null && assets.Unsheath != null)
            {
                heroAudio.PlayOneShot(assets.Unsheath);
            }

            deadline = Time.realtimeSinceStartup + 2f;
            while (!completed && animator != null && animator.IsPlaying(ChallengeClip) && Time.realtimeSinceStartup < deadline && IsPreparing(token))
            {
                yield return null;
            }
        }
        finally
        {
            if (animator != null)
            {
                animator.AnimationEventTriggered -= onTrigger;
                animator.AnimationCompleted -= onComplete;
            }
        }

        if (!IsPreparing(token))
        {
            yield break;
        }

        if (heroAudio != null && assets.Impact != null)
        {
            heroAudio.PlayOneShot(assets.Impact);
        }

        if (assets.DreamEffect != null)
        {
            UObject.Instantiate(assets.DreamEffect, Vector3.zero, Quaternion.identity);
        }

        whiteShown = true;
        CustomPantheonStatue.FadeBlanker(true, true, WhiteFadeSeconds);
        yield return new WaitForSecondsRealtime(WhiteFadeSeconds + 0.15f);
        result.Ok = IsPreparing(token);
    }

    private static IEnumerator HopThroughAtrium(int token, StepResult result)
    {
        result.Ok = false;
        GameManager? manager = GameManager.instance;
        HeroController? hero = HeroController.instance;
        if (manager == null || hero == null)
        {
            yield break;
        }

        hero.ClearMPSendEvents();
        manager.BeginSceneTransition(new GameManager.SceneLoadInfo
        {
            SceneName = AtriumScene,
            EntryGateName = AtriumDetourGate,
            EntryDelay = 0f,
            Visualization = GameManager.SceneLoadVisualizations.Default,
            PreventCameraFadeOut = true,
            WaitForSceneTransitionCameraFade = false
        });

        StepResult loaded = new();
        float deadline = Time.realtimeSinceStartup + LoadTimeoutSeconds;
        yield return null;
        while (GameManager.instance == null
            || GameManager.instance.IsInSceneTransition
            || !string.Equals(GameManager.instance.sceneName, AtriumScene, StringComparison.Ordinal))
        {
            if (!IsPreparing(token) || Time.realtimeSinceStartup > deadline)
            {
                yield break;
            }

            yield return null;
        }

        hero = HeroController.instance;
        if (hero != null)
        {
            PlaceHero(hero, AtriumDetourPosition);
        }

        yield return WaitForScene(token, AtriumScene, loaded);
        if (!loaded.Ok)
        {
            yield break;
        }

        yield return new WaitForSecondsRealtime(0.2f);
        result.Ok = IsPreparing(token) && IsHeroReady();
    }

    private static void Begin(string firstScene)
    {
        GameManager manager = GameManager.instance;
        HeroController hero = HeroController.instance;
        PlayerData data = PlayerData.instance;

        if (BossSequenceController.IsInSequence)
        {
            BossSequenceController.Reset();
        }

        previousDreamReturnScene = data.GetString("dreamReturnScene");
        data.SetString("dreamReturnScene", WorkshopScene);
        data.SetString("bossReturnEntryGate", CustomPantheonStatue.ReturnGateName);
        data.SetBool("disablePause", true);
        StaticVariableList.SetValue("ggCinematicEnding", false);

        effectsApplied = true;
        firstSceneFadePending = true;
        try
        {
            CustomPantheonIsolation.Begin();
            CustomPantheonIsolation.PrepareFight(runFights.Count > 0 ? runFights[0] : null);
            Tools.GearSwitcher.ApplyTemporaryGear(GearFor(runFights.Count > 0 ? runFights[0] : null));
        }
        catch (Exception ex)
        {
            LogError($"[CustomPantheon] Preparing the first fight failed: {ex}");
        }

        hero.RelinquishControl();
        Rigidbody2D? body = hero.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.velocity = Vector2.zero;
        }

        hero.ClearMPSendEvents();
        phase = Phase.Running;
        BossSequenceController.SetupNewSequence(sequence, bindings, DoorKey);
        if (!whiteShown)
        {
            whiteShown = true;
            CustomPantheonStatue.FadeBlanker(true, true, 0f);
        }

        FsmBool? noFade = GameCameras.instance != null && GameCameras.instance.cameraFadeFSM != null
            ? GameCameras.instance.cameraFadeFSM.FsmVariables.FindFsmBool("No Fade")
            : null;
        if (noFade != null)
        {
            noFade.Value = true;
        }

        hero.AffectedByGravity(false);
        manager.BeginSceneTransition(new GameManager.SceneLoadInfo
        {
            SceneName = firstScene,
            EntryGateName = EntryGate,
            EntryDelay = 0f,
            Visualization = GameManager.SceneLoadVisualizations.GodsAndGlory,
            PreventCameraFadeOut = true,
            WaitForSceneTransitionCameraFade = true,
            AlwaysUnloadUnusedAssets = false
        });

        LogDebug($"[CustomPantheon] Started '{chainName}' with {sequence!.Count} scenes");
    }

    private static void Abort(int token)
    {
        if (token != generation)
        {
            return;
        }

        LogDebug("[CustomPantheon] Start aborted");
        RecoverAfterAbort();
        Finish();
        Tools.QuickMenu.ShowStatusMessageFromExternal("CustomPantheon/NotNow".Localize());
    }

    private static void RecoverAfterAbort()
    {
        try
        {
            if (blackShown)
            {
                blackShown = false;
                CustomPantheonStatue.FadeBlanker(false, false, 0.3f);
            }

            if (whiteShown)
            {
                whiteShown = false;
                CustomPantheonStatue.FadeBlanker(true, false, 0.5f);
            }

            HeroController? hero = HeroController.instance;
            if (hero != null && !hero.cState.transitioning)
            {
                hero.AffectedByGravity(true);
            }

            if (controlTaken && hero != null)
            {
                controlTaken = false;
                SetDreamConvo(hero, false);
                PlayerData.instance.SetBool("disablePause", false);
                hero.StartAnimationControl();
                hero.RegainControl();
            }

            PlayMakerFSM? conversation = statueConversation;
            statueConversation = null;
            if (conversation != null)
            {
                conversation.SendEvent("TALK FINISH");
            }
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "CustomPantheonRunner.cs/Recover");
        }
    }

    private static void Finish()
    {
        if (phase == Phase.Idle)
        {
            return;
        }

        bool started = phase != Phase.Preparing;
        phase = Phase.Idle;
        generation++;

        try
        {
            if (started && sequence != null && BossSequenceController.CheckIfSequence(sequence))
            {
                BossSequenceController.Reset();
            }

            PlayerData? data = PlayerData.instance;
            if (started && data != null)
            {
                if (string.Equals(data.GetString("dreamReturnScene"), WorkshopScene, StringComparison.Ordinal)
                    && !string.IsNullOrEmpty(previousDreamReturnScene))
                {
                    data.SetString("dreamReturnScene", previousDreamReturnScene);
                }

                string returnGate = data.GetString("bossReturnEntryGate");
                if (string.Equals(returnGate, DefaultReturnGate, StringComparison.Ordinal)
                    || string.Equals(returnGate, CustomPantheonStatue.ReturnGateName, StringComparison.Ordinal))
                {
                    data.SetString("bossReturnEntryGate", string.Empty);
                }
            }
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "CustomPantheonRunner.cs/Finish");
        }

        if (effectsApplied)
        {
            effectsApplied = false;
            RestoreEffects();
        }

        _ = GlobalCoroutineExecutor.Start(RemoveHooksWhenIdle());
        runScenes.Clear();
        runFights.Clear();
        runFullGear = null;
        currentFightIndex = -1;
        previousDreamReturnScene = null;
        chainName = string.Empty;
        statueConversation = null;
        victoryReturn = false;
        controlTaken = false;
        blackShown = false;
        whiteShown = false;
        LogDebug("[CustomPantheon] Run finished");
    }

    private static void RestoreEffectsNow()
    {
        if (!effectsApplied)
        {
            return;
        }

        effectsApplied = false;
        RestoreEffects();
        LogDebug("[CustomPantheon] Player settings restored");
    }

    internal static void StopForQuit()
    {
        if (phase == Phase.Idle)
        {
            return;
        }

        try
        {
            RestoreEffectsNow();
            Finish();
        }
        catch (Exception ex)
        {
            LogError($"[CustomPantheon] Restoring settings on quit failed: {ex}");
        }
    }

    private static void RestoreEffects()
    {
        infiniteHpActive = false;
        Tools.GearSwitcher.PreserveHealthOnApply = false;
        TryRestore(Tools.SpeedChanger.EndChainSpeed, "Speed");
        TryRestore(Tools.MaskDamage.EndChainDamage, "MaskDamage");
        TryRestore(CustomPantheonIsolation.End, "Isolation");
        TryRestore(Tools.GearSwitcher.EndTemporaryGear, "Gear");
    }

    private static void TryRestore(Action action, string what)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            LogError($"[CustomPantheon] Restoring {what} failed: {ex}");
        }
    }

    private static bool infiniteHpActive;

    private static int OnTakeHealth(int damage) =>
        phase == Phase.Running && infiniteHpActive ? 0 : damage;

    private static void ApplyFightEffects(CustomPantheonFight fight)
    {
        infiniteHpActive = fight.InfiniteHp;
        try
        {
            Tools.GearSwitcher.ApplyTemporaryGear(GearFor(fight));
            Tools.SpeedChanger.ApplyChainSpeed(fight.GameSpeed > 0f ? fight.GameSpeed : 1f);
            Tools.MaskDamage.ApplyChainDamage(Math.Max(1, fight.MaskDamage));
        }
        catch (Exception ex)
        {
            LogError($"[CustomPantheon] Applying fight settings failed: {ex}");
        }
    }

    private static Vitals CaptureVitals()
    {
        PlayerData? data = PlayerData.instance;
        return data == null ? new Vitals(1, 0, 0) : new Vitals(data.health, data.healthBlue, data.MPCharge + data.MPReserve);
    }

    private static IEnumerator ApplyVitalsWhenReady(int token, int index, bool first, Vitals carried)
    {
        float deadline = Time.realtimeSinceStartup + 10f;
        yield return null;
        while (token == generation
            && phase == Phase.Running
            && currentFightIndex == index
            && Time.realtimeSinceStartup < deadline
            && (GameManager.instance == null || GameManager.instance.IsInSceneTransition || Tools.GearSwitcher.HasPendingApply))
        {
            yield return null;
        }

        yield return null;
        if (token != generation || phase != Phase.Running || currentFightIndex != index || index >= runFights.Count)
        {
            yield break;
        }

        CustomPantheonFight fight = runFights[index];
        try
        {
            if (!first)
            {
                if (fight.RestoreHealth)
                {
                    Tools.GearSwitcher.FillChainHealth();
                }
                else
                {
                    Tools.GearSwitcher.SetChainHealth(carried.Health);
                }
            }

            int? lifeblood = fight.Lifeblood ?? (first ? (int?)null : carried.Lifeblood);
            if (lifeblood.HasValue)
            {
                Tools.GearSwitcher.SetChainLifeblood(lifeblood.Value);
            }

            int? soul = fight.Soul ?? (first ? (int?)null : carried.Soul);
            LogDebug($"[CustomPantheon] Fight {index + 1} {fight.Scene}: soul target {(soul.HasValue ? soul.Value.ToString() : "-")}, carried {carried.Soul}, now {Tools.GearSwitcher.DescribeSoul()}");
            if (soul.HasValue)
            {
                Tools.GearSwitcher.SetChainSoul(soul.Value);
            }
        }
        catch (Exception ex)
        {
            LogError($"[CustomPantheon] Setting health, soul or lifeblood failed: {ex}");
        }

        foreach (float delay in SoulHudRefreshDelays)
        {
            yield return new WaitForSecondsRealtime(delay);
            if (token != generation || phase != Phase.Running || currentFightIndex != index)
            {
                yield break;
            }

            Tools.GearSwitcher.RefreshSoulVesselsHud();
        }
    }

    private static readonly float[] SoulHudRefreshDelays = { 0.9f, 1.2f };

    private static void ApplyBenchEffects()
    {
        infiniteHpActive = false;
        try
        {
            Tools.SpeedChanger.ApplyChainSpeed(1f);
            Tools.MaskDamage.ApplyChainDamage(1f);
        }
        catch (Exception ex)
        {
            LogError($"[CustomPantheon] Applying bench settings failed: {ex}");
        }
    }

    private static int FindFightIndex(string scene)
    {
        int byController = BossSequenceController.BossIndex;
        if (byController >= 0 && byController < runFights.Count
            && string.Equals(runFights[byController].Scene, scene, StringComparison.Ordinal)
            && byController > currentFightIndex)
        {
            return byController;
        }

        for (int i = Math.Max(currentFightIndex + 1, 0); i < runFights.Count; i++)
        {
            if (string.Equals(runFights[i].Scene, scene, StringComparison.Ordinal))
            {
                return i;
            }
        }

        return -1;
    }

    private static CustomPantheonFight? NextBossFight(int afterIndex)
    {
        for (int i = afterIndex + 1; i < runFights.Count; i++)
        {
            if (!CustomPantheonCatalog.IsBench(runFights[i].Scene))
            {
                return runFights[i];
            }
        }

        return null;
    }

    private static void OnEndBossScene(On.BossSceneController.orig_EndBossScene orig, BossSceneController self)
    {
        orig(self);
        if (phase != Phase.Running || currentFightIndex < 0)
        {
            return;
        }

        try
        {
            CustomPantheonIsolation.PrepareFight(NextBossFight(currentFightIndex));
        }
        catch (Exception ex)
        {
            LogError($"[CustomPantheon] Preparing the next fight failed: {ex}");
        }
    }

    private static void InstallHooks()
    {
        if (hooked)
        {
            return;
        }

        hooked = true;
        USceneManager.activeSceneChanged += OnActiveSceneChanged;
        On.GameManager.EnterHero += OnEnterHero;
        On.PlayMakerFSM.Start += OnFsmStart;
        On.BossSceneController.EndBossScene += OnEndBossScene;
        On.tk2dSpriteAnimator.Start += KeepClipStartedBeforeAutoplay;
        On.GameManager.BeginSceneTransition += OnBeginSceneTransition;
        ModHooks.TakeHealthHook += OnTakeHealth;
    }

    private static void OnBeginSceneTransition(On.GameManager.orig_BeginSceneTransition orig, GameManager self, GameManager.SceneLoadInfo info)
    {
        try
        {
            if (phase == Phase.Running
                && CustomPantheonCatalog.IsBench(self.sceneName)
                && info != null
                && runScenes.Contains(info.SceneName ?? string.Empty)
                && HeroController.instance != null)
            {
                HeroController.instance.AffectedByGravity(false);
            }
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "CustomPantheonRunner.cs/BenchExit");
        }

        orig(self, info);
    }

    private static IEnumerator RemoveHooksWhenIdle()
    {
        yield return null;
        if (phase == Phase.Idle)
        {
            RemoveHooks();
        }
    }

    private static void RemoveHooks()
    {
        if (!hooked)
        {
            return;
        }

        hooked = false;
        infiniteHpActive = false;
        USceneManager.activeSceneChanged -= OnActiveSceneChanged;
        On.GameManager.EnterHero -= OnEnterHero;
        On.PlayMakerFSM.Start -= OnFsmStart;
        On.BossSceneController.EndBossScene -= OnEndBossScene;
        On.tk2dSpriteAnimator.Start -= KeepClipStartedBeforeAutoplay;
        On.GameManager.BeginSceneTransition -= OnBeginSceneTransition;
        ModHooks.TakeHealthHook -= OnTakeHealth;
    }

    private static void OnActiveSceneChanged(Scene from, Scene to)
    {
        if (phase != Phase.Running)
        {
            return;
        }

        string scene = to.name ?? string.Empty;
        if (runScenes.Contains(scene))
        {
            if (firstSceneFadePending)
            {
                firstSceneFadePending = false;
                _ = GlobalCoroutineExecutor.Start(RevealFirstScene(generation));
            }

            if (RandomPantheons.HidesHudOnLeave(from.name))
            {
                _ = GlobalCoroutineExecutor.Start(ShowHudNextFrame());
            }

            if (CustomPantheonCatalog.IsBench(scene))
            {
                Tools.GearSwitcher.PreserveHealthOnApply = false;
                ApplyBenchEffects();
                return;
            }

            int index = FindFightIndex(scene);
            if (index >= 0)
            {
                currentFightIndex = index;
                CustomPantheonFight fight = runFights[index];
                bool first = !firstFightEntered;
                firstFightEntered = true;
                Vitals carried = CaptureVitals();
                Tools.GearSwitcher.PreserveHealthOnApply = !first && !fight.RestoreHealth;
                ApplyFightEffects(fight);
                _ = GlobalCoroutineExecutor.Start(ApplyVitalsWhenReady(generation, index, first, carried));
            }

            return;
        }

        if (string.Equals(scene, EndScene, StringComparison.Ordinal))
        {
            victoryReturn = true;
            RestoreEffectsNow();
            return;
        }

        if (string.Equals(scene, WorkshopScene, StringComparison.Ordinal)
            || string.Equals(scene, AtriumScene, StringComparison.Ordinal))
        {
            phase = Phase.Returning;
            _ = GlobalCoroutineExecutor.Start(FinishIfStuck(generation));
            return;
        }

        Finish();
    }

    private static IEnumerator RevealFirstScene(int token)
    {
        float deadline = Time.realtimeSinceStartup + 8f;
        yield return null;
        while (token == generation && Time.realtimeSinceStartup < deadline && GameManager.instance != null && GameManager.instance.IsInSceneTransition)
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(1.5f);
        if (token != generation)
        {
            yield break;
        }

        PlayMakerFSM? cameraFade = GameCameras.instance != null ? GameCameras.instance.cameraFadeFSM : null;
        string fadeState = cameraFade != null ? cameraFade.ActiveStateName : string.Empty;
        if (cameraFade != null && fadeState != "Normal" && fadeState != "FadingOut" && GameManager.instance != null)
        {
            LogDebug($"[CustomPantheon] Camera stayed in '{fadeState}', fading the first scene in");
            GameManager.instance.FadeSceneIn();
        }

        yield return new WaitForSecondsRealtime(1.5f);
        if (token != generation || !whiteShown)
        {
            yield break;
        }

        whiteShown = false;
        if (CustomPantheonStatue.IsBlankerVisible(true))
        {
            LogDebug("[CustomPantheon] White screen still up in the first scene, fading it out");
            CustomPantheonStatue.FadeBlanker(true, false, 0.5f);
        }
    }

    private static IEnumerator ShowHudNextFrame()
    {
        yield return null;
        if (GameCameras.instance != null && GameCameras.instance.hudCanvas != null)
        {
            GameCameras.instance.hudCanvas.LocateMyFSM("Slide Out")?.SendEvent("IN");
        }
    }

    private static IEnumerator FinishIfStuck(int token)
    {
        yield return new WaitForSecondsRealtime(ReturnTimeoutSeconds);
        if (token == generation && phase == Phase.Returning)
        {
            Finish();
        }
    }

    private static void OnEnterHero(On.GameManager.orig_EnterHero orig, GameManager self, bool additiveGateSearch)
    {
        bool toWorkshop = string.Equals(self.sceneName, WorkshopScene, StringComparison.Ordinal);
        bool returning = (phase == Phase.Running || phase == Phase.Returning)
            && (toWorkshop || string.Equals(self.sceneName, AtriumScene, StringComparison.Ordinal));
        bool victory = returning && victoryReturn;
        if (returning)
        {
            try
            {
                if (toWorkshop)
                {
                    FixWorkshopReturnGate(self, victory);
                }
                else
                {
                    FixReturnGate(self);
                }
            }
            catch (Exception ex)
            {
                LogSuppressed(ex, "CustomPantheonRunner.cs/ReturnGate");
            }
        }

        orig(self, additiveGateSearch);

        if (returning)
        {
            if (victory)
            {
                _ = GlobalCoroutineExecutor.Start(FinishVictoryReturn());
            }

            Finish();
        }
    }

    private static IEnumerator FinishVictoryReturn()
    {
        float deadline = Time.realtimeSinceStartup + 10f;
        yield return null;
        while (Time.realtimeSinceStartup < deadline
            && (GameManager.instance == null || GameManager.instance.IsInSceneTransition || HeroController.instance == null))
        {
            yield return null;
        }

        GameManager? manager = GameManager.instance;
        HeroController? hero = HeroController.instance;
        if (manager == null || hero == null)
        {
            yield break;
        }

        try
        {
            manager.cameraCtrl?.PositionToHero(false);
            manager.FadeSceneIn();
            if (GameCameras.instance != null && GameCameras.instance.hudCanvas != null)
            {
                GameCameras.instance.hudCanvas.LocateMyFSM("Slide Out")?.SendEvent("IN");
            }
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "CustomPantheonRunner.cs/VictoryFade");
        }

        yield return new WaitForSecondsRealtime(0.5f);
        hero = HeroController.instance;
        if (hero == null)
        {
            yield break;
        }

        try
        {
            StaticVariableList.SetValue("finishedBossReturning", false);
            PlayMakerFSM? dreamReturn = hero.gameObject.LocateMyFSM("Dream Return");
            FsmBool? returningFlag = dreamReturn != null ? dreamReturn.FsmVariables.FindFsmBool("Dream Returning") : null;
            if (returningFlag != null)
            {
                returningFlag.Value = false;
            }

            SetDreamConvo(hero, false);
            PlayerData.instance.SetBool("disablePause", false);
            hero.StartAnimationControl();
            hero.RegainControl();
            hero.AcceptInput();
            LogDebug("[CustomPantheon] Victory return: control given back");
        }
        catch (Exception ex)
        {
            LogError($"[CustomPantheon] Giving control back after the chain failed: {ex}");
        }
    }

    private static void FixWorkshopReturnGate(GameManager manager, bool victory)
    {
        string? chosen = CustomPantheonStatue.PrepareReturnGate(victory) ?? CustomPantheonStatue.FindNearestReturnGate();
        if (chosen == null)
        {
            return;
        }

        string current = ReflectionHelper.GetField<GameManager, string>(manager, "entryGateName") ?? string.Empty;
        if (string.Equals(current, DreamReturnGate, StringComparison.Ordinal))
        {
            PlayerData.instance.SetString("bossReturnEntryGate", chosen);
            return;
        }

        if (current.StartsWith("door_dreamReturnGG", StringComparison.Ordinal))
        {
            ReflectionHelper.SetField(manager, "entryGateName", chosen);
        }
    }

    private static void FixReturnGate(GameManager manager)
    {
        string? chosen = ChooseAtriumGate();
        if (chosen == null)
        {
            return;
        }

        string current = ReflectionHelper.GetField<GameManager, string>(manager, "entryGateName") ?? string.Empty;
        if (string.Equals(current, DreamReturnGate, StringComparison.Ordinal))
        {
            PlayerData.instance.SetString("bossReturnEntryGate", chosen);
            return;
        }

        if (string.Equals(current, DefaultReturnGate, StringComparison.Ordinal)
            || current.StartsWith("door_dreamReturnGG", StringComparison.Ordinal))
        {
            ReflectionHelper.SetField(manager, "entryGateName", chosen);
        }
    }

    private static string? ChooseAtriumGate()
    {
        string? best = null;
        int bestTier = int.MaxValue;
        foreach (BossSequenceDoor door in UObject.FindObjectsOfType<BossSequenceDoor>())
        {
            GameObject? gate = door.dreamReturnGate;
            if (gate == null || !gate.activeInHierarchy || gate.GetComponent<TransitionPoint>() == null)
            {
                continue;
            }

            int tier = ParseDoorTier(door.playerDataString);
            if (tier < bestTier)
            {
                bestTier = tier;
                best = gate.name;
            }
        }

        if (best != null)
        {
            return best;
        }

        foreach (string name in FallbackReturnGates)
        {
            GameObject? gate = GameObject.Find(name);
            if (gate != null && gate.GetComponent<TransitionPoint>() != null)
            {
                return name;
            }
        }

        return null;
    }

    internal static int ParseDoorTier(string? playerData)
    {
        const string prefix = "bossDoorStateTier";
        if (string.IsNullOrEmpty(playerData) || !playerData!.StartsWith(prefix, StringComparison.Ordinal))
        {
            return 99;
        }

        string rest = playerData.Substring(prefix.Length);
        return rest.Length > 0 && rest.All(char.IsDigit) && int.TryParse(rest, out int tier) ? tier : 99;
    }

    private static void OnFsmStart(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        if (phase == Phase.Running && self is { name: "_SceneManager", FsmName: "gg_music_control" })
        {
            try
            {
                ConfigureMusic(self);
            }
            catch (Exception ex)
            {
                LogSuppressed(ex, "CustomPantheonRunner.cs/Music");
            }
        }

        orig(self);
        if (phase == Phase.Running && self is { name: "Dream Entry", FsmName: "Control" })
        {
            try
            {
                ConfigureArrival(self);
            }
            catch (Exception ex)
            {
                LogSuppressed(ex, "CustomPantheonRunner.cs/DreamEntry");
            }

            return;
        }

        if (phase != Phase.Running || self is not { name: "Absolute Radiance", FsmName: "Control" })
        {
            return;
        }

        try
        {
            self.GetAction<SetStaticVariable>("Ending Scene", 1).setValue.boolValue = false;
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "CustomPantheonRunner.cs/Radiance");
        }
    }

    private static readonly HashSet<string> ArrivalStates = new(StringComparer.Ordinal)
    {
        "Take Control", "First Boss? 1", "Statue", "Start Kneeling", "Pause Kneeling", "Wait for Transition",
        "Hide Player", "Start Fade", "First Boss?", "Arrive Anim", "Wait for Anim", "Return from Kneeling"
    };

    private static FsmState? FindFsmState(PlayMakerFSM fsm, string name) =>
        fsm.FsmStates.FirstOrDefault(state => state.Name == name);

    private static void ConfigureArrival(PlayMakerFSM fsm)
    {
        FsmState? statue = FindFsmState(fsm, "Statue");
        FsmState? rise = FindFsmState(fsm, "Return from Kneeling");
        FsmState? hide = FindFsmState(fsm, "Hide Player");
        FsmState? arrive = FindFsmState(fsm, "Arrive Anim");
        FsmState? takeControl = FindFsmState(fsm, "Take Control");
        if (statue == null || rise == null || hide == null || arrive == null || takeControl == null)
        {
            return;
        }

        int index = BossSequenceController.BossIndex;
        bool arrival = index >= 0 && index < runFights.Count && runFights[index].ArrivalAnimation;
        if (arrival)
        {
            RedirectTransition(fsm, "First Boss? 1", "FINISHED", hide);
            RedirectTransition(fsm, "First Boss?", "FALSE", arrive);
        }
        else
        {
            RedirectTransition(fsm, "First Boss? 1", "FIRST", statue);
            RedirectTransition(fsm, "First Boss?", "TRUE", rise);
        }

        int token = generation;
        takeControl.InsertMethod(0, () => _ = GlobalCoroutineExecutor.Start(HoldHeroDuringArrival(fsm, token)));
    }

    private static IEnumerator HoldHeroDuringArrival(PlayMakerFSM fsm, int token)
    {
        HeroController? hero = HeroController.instance;
        if (hero == null)
        {
            yield break;
        }

        float? heldX = null;
        float deadline = Time.realtimeSinceStartup + 15f;
        while (token == generation
            && fsm != null
            && ArrivalStates.Contains(fsm.ActiveStateName)
            && Time.realtimeSinceStartup < deadline)
        {
            hero = HeroController.instance;
            if (hero == null)
            {
                yield break;
            }

            if (!hero.controlReqlinquished)
            {
                hero.RelinquishControl();
            }

            if (heldX == null && IsHeroAtEntryOf(hero, fsm))
            {
                heldX = hero.transform.position.x;
                LogDebug($"[CustomPantheon] Arrival hold in {fsm.gameObject.scene.name} at x {heldX.Value:0.00}");
            }

            if (heldX != null)
            {
                hero.transform.position = new Vector3(heldX.Value, hero.transform.position.y, hero.transform.position.z);
            }

            Rigidbody2D? body = hero.GetComponent<Rigidbody2D>();
            if (body != null)
            {
                body.velocity = new Vector2(0f, Math.Min(0f, body.velocity.y));
            }

            yield return null;
        }
    }

    private static bool IsHeroAtEntryOf(HeroController hero, PlayMakerFSM fsm)
    {
        TransitionPoint? gate = hero.sceneEntryGate;
        return gate != null && gate.gameObject.scene == fsm.gameObject.scene;
    }

    private static readonly FieldInfo? CurrentMusicCueField = typeof(AudioManager).GetField("currentMusicCue", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly FieldInfo? MusicSourcesField = typeof(AudioManager).GetField("musicSources", BindingFlags.Instance | BindingFlags.NonPublic);

    private static void ConfigureMusic(PlayMakerFSM fsm)
    {
        FsmState? inert = FindFsmState(fsm, "Inert");
        if (inert == null)
        {
            return;
        }

        bool remix = fsm.FsmVariables.FindFsmBool("Has Remix")?.Value == true
            && BossSceneController.Instance != null
            && BossSceneController.Instance.BossLevel > 0;
        MusicCue? remixCue = remix ? fsm.FsmVariables.FindFsmObject("StatueRemixMusicCue")?.Value as MusicCue : null;
        MusicCue? cue = remixCue ?? fsm.FsmVariables.FindFsmObject("StatueRegularMusicCue")?.Value as MusicCue;
        if (cue == null)
        {
            return;
        }

        UnityEngine.Audio.AudioMixerSnapshot? snapshot = fsm.FsmVariables.FindFsmObject(remixCue != null ? "StatueRemixSnapshot" : "StatueRegularSnapshot")?.Value as UnityEngine.Audio.AudioMixerSnapshot;
        RedirectTransition(fsm, "Init", "FINISHED", inert);
        _ = GlobalCoroutineExecutor.Start(PlayStatueMusic(generation, BossSequenceController.BossIndex, fsm.gameObject.scene.name, cue, snapshot));
    }

    private static IEnumerator PlayStatueMusic(int token, int index, string scene, MusicCue cue, UnityEngine.Audio.AudioMixerSnapshot? snapshot)
    {
        float deadline = Time.realtimeSinceStartup + 8f;
        yield return null;
        while (token == generation
            && Time.realtimeSinceStartup < deadline
            && (GameManager.instance == null || GameManager.instance.IsInSceneTransition))
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.2f);
        AudioManager? audio = GameManager.instance != null ? GameManager.instance.AudioManager : null;
        if (token != generation || phase != Phase.Running || audio == null)
        {
            yield break;
        }

        try
        {
            CurrentMusicCueField?.SetValue(audio, null);
            audio.ApplyMusicCue(cue, 0f, 0f, false);
            if (snapshot != null)
            {
                audio.ApplyMusicSnapshot(snapshot, 0f, 0f);
            }
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "CustomPantheonRunner.cs/Music");
            yield break;
        }

        yield return new WaitForSecondsRealtime(2f);
        if (token == generation)
        {
            LogDebug($"[CustomPantheon] Fight {index + 1} {scene}: statue music '{cue.name}', now {DescribeMusic(audio)}");
        }
    }

    private static string DescribeMusic(AudioManager audio)
    {
        try
        {
            MusicCue? current = CurrentMusicCueField?.GetValue(audio) as MusicCue;
            string playing = MusicSourcesField?.GetValue(audio) is AudioSource[] sources
                ? string.Join(", ", sources.Where(source => source != null && source.isPlaying && source.clip != null).Select(source => $"{source.clip.name} vol {source.volume:0.##}"))
                : "?";
            return $"cue '{(current != null ? current.name : "none")}', playing [{playing}]";
        }
        catch (Exception ex)
        {
            return $"unknown ({ex.GetType().Name})";
        }
    }

    private static void KeepClipStartedBeforeAutoplay(On.tk2dSpriteAnimator.orig_Start orig, tk2dSpriteAnimator self)
    {
        try
        {
            if (phase == Phase.Running
                && BossSceneController.IsBossScene
                && self.GetComponent<HealthManager>() != null
                && self.playAutomatically
                && self.Playing
                && self.CurrentClip != null
                && self.CurrentClip != self.DefaultClip)
            {
                Log($"[CustomPantheon] Kept clip '{self.CurrentClip.name}' on '{self.gameObject.name}' instead of letting the animator autoplay '{self.DefaultClip?.name}' over it");
                return;
            }
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "CustomPantheonRunner.cs/Autoplay");
        }

        orig(self);
    }

    private static void RedirectTransition(PlayMakerFSM fsm, string stateName, string eventName, FsmState target)
    {
        FsmState? state = fsm.FsmStates.FirstOrDefault(candidate => candidate.Name == stateName);
        if (state == null)
        {
            return;
        }

        foreach (FsmTransition transition in state.Transitions)
        {
            if (transition.EventName == eventName)
            {
                transition.ToState = target.Name;
                transition.ToFsmState = target;
            }
        }
    }

    private static BossSequence BuildSequence(List<string> scenes)
    {
        Dictionary<string, BossScene> originals = new(StringComparer.Ordinal);
        BossSequence? template = null;
        for (int tier = 1; tier <= CustomPantheonCatalog.PantheonCount; tier++)
        {
            BossSequence? loaded = LoadTierSequence(tier);
            if (loaded == null)
            {
                continue;
            }

            if (tier == CustomPantheonCatalog.PantheonCount || template == null)
            {
                template = loaded;
            }

            if (BossScenesField.GetValue(loaded) is BossScene[] tierScenes)
            {
                foreach (BossScene scene in tierScenes)
                {
                    if (scene != null && !string.IsNullOrEmpty(scene.sceneName) && !originals.ContainsKey(scene.sceneName))
                    {
                        originals[scene.sceneName] = scene;
                    }
                }
            }
        }

        BossSequence built = ScriptableObject.CreateInstance<BossSequence>();
        built.name = SequenceName;
        built.useSceneUnlocks = false;
        built.tests = Array.Empty<BossScene.BossTest>();
        built.achievementKey = string.Empty;
        built.customEndScene = string.Empty;
        built.customEndScenePlayerData = string.Empty;
        built.nailDamage = template != null ? template.nailDamage : FallbackNailDamage;
        built.lowerNailDamagePercentage = template != null ? template.lowerNailDamagePercentage : FallbackNailPercentage;
        built.maxHealth = template != null ? template.maxHealth : FallbackMaxHealth;
        createdAssets.Add(built);

        BossScene[] bossScenes = new BossScene[scenes.Count];
        for (int i = 0; i < scenes.Count; i++)
        {
            bossScenes[i] = CreateScene(scenes[i], originals);
        }

        BossScenesField.SetValue(built, bossScenes);
        return built;
    }

    private static BossSequence? LoadTierSequence(int tier)
    {
        string name = $"Boss Sequence Tier {tier}";
        try
        {
            BossSequence? loaded = Resources.Load<BossSequence>($"GG/{name}");
            if (loaded != null)
            {
                return loaded;
            }

            return Resources.FindObjectsOfTypeAll<BossSequence>().FirstOrDefault(candidate => string.Equals(candidate.name, name, StringComparison.Ordinal));
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "CustomPantheonRunner.cs/LoadTier");
            return null;
        }
    }

    private static BossScene CreateScene(string sceneName, Dictionary<string, BossScene> originals)
    {
        BossScene scene = ScriptableObject.CreateInstance<BossScene>();
        scene.name = sceneName;
        scene.sceneName = sceneName;
        scene.bossTests = Array.Empty<BossScene.BossTest>();
        scene.baseBoss = null;
        scene.substituteBoss = false;
        scene.isHidden = false;
        scene.requireUnlock = false;
        if (ForceAssetUnloadField != null && originals.TryGetValue(sceneName, out BossScene original))
        {
            ForceAssetUnloadField.SetValue(scene, original.ForceAssetUnload);
        }

        createdAssets.Add(scene);
        return scene;
    }

    private static void DestroyCreatedAssets()
    {
        foreach (UObject asset in createdAssets)
        {
            if (asset != null && !(asset is BossSequence old && BossSequenceController.CheckIfSequence(old)))
            {
                UObject.Destroy(asset);
            }
        }

        createdAssets.Clear();
        sequence = null;
    }
}
