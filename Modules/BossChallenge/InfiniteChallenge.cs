using UnityEngine.Audio;
using Satchel.Futils;
using GodhomeQoL.Modules.QoL;
namespace GodhomeQoL.Modules.BossChallenge;

public sealed class InfiniteChallenge : Module
{
    [GlobalSetting]
    [BoolOption]
    public static bool restartFightOnSuccess = false;

    [GlobalSetting]
    [BoolOption]
    public static bool restartFightAndMusic = false;

    private static readonly Func<GameManager.SceneLoadInfo, bool>[] defaultReturnScenePredicates = [
        (info) => info.SceneName is "GG_Workshop",
        (info) => info.SceneName is "White_Palace_09"
    ];
    private static readonly Dictionary<int, Func<GameManager.SceneLoadInfo, bool>> externalReturnScenePredicates = [];
    private static int nextExternalReturnScenePredicateHandle;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    internal static bool OwnsFinishedBossReturningFlag() =>
        ModuleManager.TryGetLoadedModule(typeof(InfiniteChallenge), out Module? module)
        && module.Enabled;

    internal static int AddReturnScenePredicate(Func<GameManager.SceneLoadInfo, bool>? predicate)
    {
        if (predicate == null)
        {
            return 0;
        }

        foreach ((int handle, Func<GameManager.SceneLoadInfo, bool> existing) in externalReturnScenePredicates)
        {
            if (existing == predicate)
            {
                return handle;
            }
        }

        int handleId = ++nextExternalReturnScenePredicateHandle;
        externalReturnScenePredicates[handleId] = predicate;
        return handleId;
    }

    internal static void RemoveReturnScenePredicate(int handle)
    {
        if (handle == 0)
        {
            return;
        }

        _ = externalReturnScenePredicates.Remove(handle);
    }

    internal static int RemoveReturnScenePredicate(Func<GameManager.SceneLoadInfo, bool>? predicate)
    {
        if (predicate == null || externalReturnScenePredicates.Count == 0)
        {
            return 0;
        }

        int[] handles = externalReturnScenePredicates
            .Where(pair => pair.Value == predicate)
            .Select(pair => pair.Key)
            .ToArray();

        foreach (int handle in handles)
        {
            _ = externalReturnScenePredicates.Remove(handle);
        }

        return handles.Length;
    }

    private static bool MatchesReturnScenePredicate(GameManager.SceneLoadInfo info)
    {
        foreach (Func<GameManager.SceneLoadInfo, bool> predicate in defaultReturnScenePredicates)
        {
            if (predicate.Invoke(info))
            {
                return true;
            }
        }

        if (externalReturnScenePredicates.Count == 0)
        {
            return false;
        }

        List<int>? brokenHandles = null;
        foreach ((int handle, Func<GameManager.SceneLoadInfo, bool> predicate) in externalReturnScenePredicates.ToArray())
        {
            try
            {
                if (predicate.Invoke(info))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                brokenHandles ??= [];
                brokenHandles.Add(handle);
                LogWarn($"InfiniteChallenge return-scene predicate handle {handle} failed and will be removed: {ex}");
            }
        }

        if (brokenHandles != null)
        {
            foreach (int handle in brokenHandles)
            {
                _ = externalReturnScenePredicates.Remove(handle);
            }
        }

        return false;
    }

    private protected override void Load()
    {
        On.BossSceneController.Awake += RecordSetupEvent;
        On.GameManager.BeginSceneTransition += RestartFight;
    }

    private protected override void Unload()
    {
        On.BossSceneController.Awake -= RecordSetupEvent;
        On.GameManager.BeginSceneTransition -= RestartFight;
    }

    private static void RecordSetupEvent(On.BossSceneController.orig_Awake orig, BossSceneController self)
    {
        BossFightRestartCompatibility.RecordCurrentSetupEvent();
        orig(self);
        BossFightRestartCompatibility.RecordCurrentSetupEvent();
    }

    private static void RestartFight(On.GameManager.orig_BeginSceneTransition orig, GameManager self, GameManager.SceneLoadInfo info)
    {
        if (BossFightRestartCompatibility.IsFastReloadTransitionInProgress)
        {
            orig(self, info);
            return;
        }

        try
        {
            string currentSceneName = self.sceneName;
            BossFightRestartCompatibility.RecordCurrentSetupEvent();

            if (
                BossSceneController.IsBossScene
                && MatchesReturnScenePredicate(info)
                && (
                    Ref.HC.heroDeathPrefab.activeSelf
                    ||
                        restartFightOnSuccess
                        && StaticVariableList.GetValue<bool>("finishedBossReturning")

                )
            )
            {
                if (!BossFightRestartCompatibility.TryApplySetupEventForScene(currentSceneName))
                {
                    LogError($"InfiniteChallenge: setup event is not available for scene {currentSceneName}");
                    orig(self, info);
                    return;
                }

                StaticVariableList.SetValue("finishedBossReturning", false);

                _ = GlobalCoroutineExecutor.Start(DelayedEnableRenderer());
                Ref.HC.EnterWithoutInput(true);
                Ref.HC.AcceptInput();
                Ref.HC.StartCoroutine(UpdateHUD());
                Ref.HC.gameObject.LocateMyFSM("Dream Return").GetVariable<FsmBool>("Dream Returning").Value = false;

                if (ModuleManager.TryGetLoadedModule<CarefreeMelodyReset>(out _))
                {
                    CarefreeMelodyReset.MarkEnteredFromWorkshop(currentSceneName);
                    CarefreeMelodyReset.TryResetNow(ignoreBossScene: true);
                }

                info.SceneName = currentSceneName;
                info.EntryGateName = "door_dreamEnter";

                _ = GlobalCoroutineExecutor.Start(SetAudio(restartFightAndMusic));
            }
        }
        catch (Exception e) { LogError(e.Message); }

        orig(self, info);
    }

    private static IEnumerator DelayedEnableRenderer()
    {
        yield return new WaitUntil(() => Ref.GM.IsInSceneTransition);
        yield return new WaitWhile(() => Ref.GM.IsInSceneTransition);

        Ref.HC.EnableRenderer();
    }

    private static IEnumerator SetAudio(bool reset)
    {
        AudioMixer mixer = Ref.GM.AudioManager.Reflect().musicSources[0].outputAudioMixerGroup.audioMixer;

        if (reset)
        {
            Ref.GM.AudioManager.Reflect().musicSources.ForEach(
                i => i.Stop()
            );
            Ref.GM.AudioManager.Reflect().currentMusicCue = null;
        }
        else
        {
            mixer.FindSnapshot("Silent").TransitionTo(0.5f);
        }

        yield return new WaitUntil(() => Ref.GM.IsInSceneTransition);
        yield return new WaitWhile(() => Ref.GM.IsInSceneTransition);
        mixer.FindSnapshot("Normal").TransitionTo(0f);
    }


    private static IEnumerator UpdateHUD()
    {
        yield return new WaitUntil(() => Ref.GM.gameState == GameState.PLAYING);

        Ref.GC.hudCanvas?.LocateMyFSM("Slide Out")?.SendEvent("IN");
        Ref.GC.hudCanvas?.gameObject?.SetActive(true);

        if (UIManager.instance != null)
        {
            try
            {
                ReflectionHelper.SetField(UIManager.instance, "uiState", UIState.PLAYING);
            }
            catch
            {
            }
        }

        Ref.HC.ClearMP();
        Ref.HC.ClearMPSendEvents();
    }
}
