using Vasi;
using Random = UnityEngine.Random;

namespace GodhomeQoL.Modules.QoL
{

    [UsedImplicitly]
    public sealed class SkipCutscenes : Module
    {
        #region Settings

        [GlobalSetting]
        public static bool AbsoluteRadiance = true;

        [GlobalSetting]
        public static bool HallOfGodsStatues = true;

        [GlobalSetting]
        public static bool PureVesselRoar = true;

        [GlobalSetting]
        public static bool GrimmNightmare = true;

        [GlobalSetting]
        public static bool GreyPrinceZote = true;

        [GlobalSetting]
        public static bool Collector = true;

        [GlobalSetting]
        public static bool AutoSkipCinematics = true;

        [GlobalSetting]
        public static bool AllowSkippingNonskippable = true;

        [GlobalSetting]
        public static bool SkipCutscenesWithoutPrompt = true;

        [GlobalSetting]
        public static bool SoulMasterPhaseTransitionSkip = true;

        #endregion
        public override bool DefaultEnabled => true;
        public override bool Hidden => true;
        public override bool AlwaysEnabled => true;

        public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

        private static readonly (Func<bool>, Func<Scene, IEnumerator>)[] FSM_SKIPS =
        {
            (() => AbsoluteRadiance, AbsRadSkip),
            (() => PureVesselRoar, HKPrimeSkip),
            (() => GrimmNightmare, GrimmNightmareSkip),
            (() => GreyPrinceZote, GreyPrinceZoteSkip),
            (() => Collector, CollectorSkip),
            (() => HallOfGodsStatues, StatueWait)
        };

        private static readonly HashSet<string> GodhomeHubScenes = new(StringComparer.Ordinal)
        {
            "GG_Workshop",
            "GG_Atrium",
            "GG_Atrium_Roof"
        };
        private static readonly HashSet<string> PantheonLikeScenes = new(StringComparer.OrdinalIgnoreCase)
        {
            "gg dryya",
            "gg hegemol",
            "gg zemer",
            "gg isma"
        };
        private static readonly HashSet<string> BossAnimationSceneNames = new(StringComparer.Ordinal)
        {
            "GG_Radiance",
            "GG_Hollow_Knight",
            "GG_Grimm_Nightmare",
            "GG_Grey_Prince_Zote",
            "GG_Collector",
            "GG_Collector_V",
            "GG_Mage_Knight",
            "GG_Mage_Knight_V"
        };

        private static bool suppressAutoSkipForTransition;
        private static bool timeScaleOverrideActive;
        private static int timeScaleOverrideHandle;
        private static int timeScaleOverrideGeneration;
        private const int StatuePatchMaxRetryFrames = 45;
        private const float StatueApproachMaxWait = 0.06f;
        private const float StatueDreamBoxDownMaxWait = 0.08f;

        private protected override void Load()
        {
            timeScaleOverrideGeneration++;
            suppressAutoSkipForTransition = false;
            timeScaleOverrideActive = false;
            timeScaleOverrideHandle = 0;
            On.CinematicSequence.Begin += CinematicBegin;
            On.FadeSequence.Begin += FadeBegin;
            On.AnimatorSequence.Begin += AnimatorBegin;
            On.InputHandler.SetSkipMode += OnSetSkip;
            On.GameManager.BeginSceneTransitionRoutine += OnBeginSceneTransition;
            On.GGCheckIfBossScene.OnEnter += MageLordPhaseTransitionSkip;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += FsmSkips;
        }

        private protected override void Unload()
        {
            On.CinematicSequence.Begin -= CinematicBegin;
            On.FadeSequence.Begin -= FadeBegin;
            On.AnimatorSequence.Begin -= AnimatorBegin;
            On.InputHandler.SetSkipMode -= OnSetSkip;
            On.GameManager.BeginSceneTransitionRoutine -= OnBeginSceneTransition;
            On.GGCheckIfBossScene.OnEnter -= MageLordPhaseTransitionSkip;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= FsmSkips;

            suppressAutoSkipForTransition = false;
            timeScaleOverrideGeneration++;
            if (timeScaleOverrideActive && timeScaleOverrideHandle != 0)
            {
                global::GodhomeQoL.Modules.Tools.SpeedChanger.EndTimeScaleOverride(timeScaleOverrideHandle);
            }

            timeScaleOverrideActive = false;
            timeScaleOverrideHandle = 0;
        }

        private static void MageLordPhaseTransitionSkip(On.GGCheckIfBossScene.orig_OnEnter orig, GGCheckIfBossScene self)
        {
            Fsm? fsm = self.Fsm;
            GameObject? owner = self.Owner;
            FsmState? activeState = self.Fsm?.ActiveState;
            var activeActions = activeState?.Actions;
            string activeStateName = fsm?.ActiveStateName ?? string.Empty;
            if (
                !SoulMasterPhaseTransitionSkip
                || owner == null
                || fsm == null
                || owner.name.IndexOf("Corpse Mage", StringComparison.Ordinal) < 0
                || activeStateName.IndexOf("Quick Death?", StringComparison.Ordinal) < 0
                || activeActions == null
                || activeActions.Length <= 1
                || activeActions[1] is not PlayerDataBoolTest p
            )
            {
                orig(self);
                return;
            }

            fsm.Event(p.isTrue);
        }

        private static void FsmSkips(Scene arg0, Scene arg1)
        {
            if (!IsAnyBossSkipEnabled())
            {
                return;
            }

            var hc = HeroController.instance;

            if (hc == null) return;

            foreach (var (check, coro) in FSM_SKIPS)
            {
                if (!check())
                {
                    continue;
                }

                try
                {
                    hc.StartCoroutine(coro(arg1));
                }
                catch (Exception ex)
                {
                    LogDebug($"SkipCutscenes: failed to start skip coroutine for scene '{arg1.name}' - {ex.Message}");
                }
            }
        }

        private static IEnumerator StatueWait(Scene arg1)
        {
            if (arg1.name != "GG_Workshop") yield break;

            bool patchedAtLeastOne = false;
            int retries = 0;
            while (retries < StatuePatchMaxRetryFrames)
            {
                PlayMakerFSM[] statueUiFsms = UObject.FindObjectsOfType<PlayMakerFSM>()
                    .Where(x => x.FsmName == "GG Boss UI")
                    .ToArray();

                if (statueUiFsms.Length > 0)
                {
                    foreach (PlayMakerFSM fsm in statueUiFsms)
                    {
                        patchedAtLeastOne |= TryPatchStatueUiFsm(fsm);
                    }

                    if (patchedAtLeastOne)
                    {
                        yield break;
                    }
                }

                retries++;
                yield return null;
            }

            if (!patchedAtLeastOne)
            {
                LogDebug("SkipCutscenes: statue UI skip patch was not applied because GG Boss UI FSM was not ready in time");
            }
        }

        private static bool TryPatchStatueUiFsm(PlayMakerFSM fsm)
        {
            try
            {
                FsmState? onLeft = fsm.Fsm.GetState("On Left");
                FsmState? onRight = fsm.Fsm.GetState("On Right");
                FsmState? dreamBoxDown = fsm.Fsm.GetState("Dream Box Down");
                if (onLeft == null || onRight == null || dreamBoxDown == null)
                {
                    return false;
                }

                // Keep vanilla transition flow for statue UI to avoid first-open race,
                // and only clamp waits to make entry faster.
                _ = ClampStateWaits(onLeft, StatueApproachMaxWait);
                _ = ClampStateWaits(onRight, StatueApproachMaxWait);
                _ = ClampStateWaits(dreamBoxDown, StatueDreamBoxDownMaxWait);
                return true;
            }
            catch (Exception ex)
            {
                LogDebug($"SkipCutscenes: statue skip patch failed - {ex.Message}");
                return false;
            }
        }

        private static bool ClampStateWaits(FsmState state, float maxWait)
        {
            if (state.Actions == null || state.Actions.Length == 0)
            {
                return false;
            }

            bool changed = false;
            foreach (Wait wait in state.Actions.OfType<Wait>())
            {
                if (wait.time.Value > maxWait)
                {
                    wait.time.Value = maxWait;
                    changed = true;
                }
            }

            return changed;
        }

        private static IEnumerator HKPrimeSkip(Scene arg1)
        {
            if (arg1.name != "GG_Hollow_Knight") yield break;

            yield return null;

            GameObject? hkPrime = GameObject.Find("HK Prime");
            PlayMakerFSM? control = hkPrime?.LocateMyFSM("Control");
            if (control == null)
            {
                yield break;
            }

            try
            {
                control.Fsm.GetState("Init")?.ChangeTransition("FINISHED", "Intro Roar");
                TrySetWaitTime(control, "Intro 2", 0.01f);
                TrySetWaitTime(control, "Intro 1", 0.01f);
                TrySetWaitTime(control, "Intro Roar", 1f);
            }
            catch (Exception ex)
            {
                LogDebug($"SkipCutscenes: HK Prime skip patch failed - {ex.Message}");
            }
        }
        private static IEnumerator GrimmNightmareSkip(Scene arg1)
        {
            if (arg1.name != "GG_Grimm_Nightmare") yield break;

            yield return null;

            GameObject? grimmControl = GameObject.Find("Grimm Control");
            PlayMakerFSM? control = grimmControl?.LocateMyFSM("Control");

            if (control != null)
            {
                TrySetStateWaitValue(control, "Pause", 0.5f);
                TrySetStateWaitValue(control, "Pan Over", 0.5f);
                TrySetStateWaitValue(control, "Eye 1", 0.1f);
                TrySetStateWaitValue(control, "Eye 2", 0.1f);
                TrySetStateWaitValue(control, "Pan Over 2", 0.1f);
                TrySetStateWaitValue(control, "Eye 3", 0.1f);
                TrySetStateWaitValue(control, "Eye 4", 0.1f);
                TrySetStateWaitValue(control, "Silhouette", 0.1f);
                TrySetStateWaitValue(control, "Silhouette 2", 0.1f);
                TrySetStateWaitValue(control, "Title Up", 0.1f);
                TrySetStateWaitValue(control, "Title Up 2", 0.1f);
                TrySetStateWaitValue(control, "Defeated Pause", 0.1f);
                TrySetStateWaitValue(control, "Defeated Start", 0.1f);
                TrySetStateWaitValue(control, "Explode Start", 0.1f);
                TrySetStateWaitValue(control, "Silhouette Up", 0.1f);
                TrySetStateWaitValue(control, "Ash Away", 0.1f);
                TrySetStateWaitValue(control, "Fade", 0.1f);

            }

        }

        private static IEnumerator CollectorSkip(Scene scene)
        {
            if (!scene.name.Contains("GG_Collector")) yield break;

            yield return null;

            GameObject? jarCollector = GameObject.Find("Jar Collector");
            PlayMakerFSM? control = jarCollector?.LocateMyFSM("Control");
            bool collectorPhasesPatchingActive = IsCollectorPhasesPatchingActive(jarCollector);

            if (control != null)
            {
                FsmState? roar = control.Fsm.GetState("Roar");
                if (roar == null)
                {
                    yield break;
                }

                Wait? roarWait = roar.Actions?.OfType<Wait>().FirstOrDefault();
                if (roarWait != null)
                {
                    roarWait.time.Value = 0.5f;
                }

                if (!collectorPhasesPatchingActive)
                {
                    TrimCollectorRoarActions(roar);
                }
            }
        }

        private static bool IsCollectorPhasesPatchingActive(GameObject? collector) =>
            global::GodhomeQoL.Modules.CollectorPhases.CollectorPhases.IsCollectorPatchingActiveFor(collector);

        private static void TrimCollectorRoarActions(FsmState roar)
        {
            if (roar.Actions == null)
            {
                return;
            }

            // Keep old aggressive roar-trim behavior only when CollectorHelper is not actively patching this run.
            for (int i = 7; i >= 6; i--)
            {
                if (i >= 0 && i < roar.Actions.Length)
                {
                    roar.RemoveAction(i);
                }
            }
        }

        private static IEnumerator GreyPrinceZoteSkip(Scene scene)
        {
            if (scene.name != "GG_Grey_Prince_Zote") yield break;

            yield return null;

            GameObject? title = GameObject.Find("Grey Prince Title");
            PlayMakerFSM? control = title?.LocateMyFSM("Control");

            if (control != null)
            {
                TrySetStateWaitValue(control, "Get Level", 0.1f);
                TrySetStateWaitValue(control, "Main Title Pause", 0.1f);
                TrySetStateWaitValue(control, "Main Title", 0.5f);
                TrySetStateWaitValue(control, "Extra 1", 0.01f);
                TrySetStateWaitValue(control, "Extra 2", 0.01f);
                TrySetStateWaitValue(control, "Extra 3", 0.01f);
                TrySetStateWaitValue(control, "Extra 4", 0.01f);
                TrySetStateWaitValue(control, "Extra 5", 0.01f);
                TrySetStateWaitValue(control, "Extra 6", 0.01f);
                TrySetStateWaitValue(control, "Extra 7", 0.01f);
                TrySetStateWaitValue(control, "Extra 8", 0.01f);
                TrySetStateWaitValue(control, "Extra 9", 0.01f);
                TrySetStateWaitValue(control, "Extra 10", 0.01f);
                TrySetStateWaitValue(control, "Extra 11", 0.01f);
                TrySetStateWaitValue(control, "Extra 12", 0.01f);
                TrySetStateWaitValue(control, "Extra 13", 0.01f);
            }
        }

        private static IEnumerator AbsRadSkip(Scene arg1)
        {
            if (arg1.name != "GG_Radiance") yield break;

            yield return null;
            try
            {
                GameObject? bossControl = GameObject.Find("Boss Control");
                PlayMakerFSM? control = bossControl?.LocateMyFSM("Control");
                if (control == null)
                {
                    yield break;
                }

                UObject.Destroy(GameObject.Find("Sun"));
                UObject.Destroy(GameObject.Find("feather_particles"));

                FsmState? setup = control.Fsm.GetState("Setup");
                if (setup == null)
                {
                    yield break;
                }

                Wait? setupWait = setup.Actions?.OfType<Wait>().FirstOrDefault();
                if (setupWait != null)
                {
                    setupWait.time = 1.5f;
                }

                setup.RemoveAction<SetPlayerDataBool>();
                setup.ChangeTransition("FINISHED", "Appear Boom");

                TrySetWaitTime(control, "Title Up", 1f);

            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        private static IEnumerator OnBeginSceneTransition(On.GameManager.orig_BeginSceneTransitionRoutine orig, GameManager self, GameManager.SceneLoadInfo info) =>
            RunSceneTransition(orig(self, info), info);

        private static IEnumerator RunSceneTransition(IEnumerator routine, GameManager.SceneLoadInfo info)
        {
            bool suppress = ShouldSuppressAutoSkip(info);
            if (suppress)
            {
                suppressAutoSkipForTransition = true;
                int generation = timeScaleOverrideGeneration;
                _ = GlobalCoroutineExecutor.Start(SuppressTimeScaleDuringReturn(info, generation));
            }

            try
            {
                while (routine.MoveNext())
                {
                    yield return routine.Current;
                }
            }
            finally
            {
                if (suppress)
                {
                    suppressAutoSkipForTransition = false;
                }
            }
        }

        private static bool ShouldSuppressAutoSkip(GameManager.SceneLoadInfo info) =>
            AutoSkipCinematics
            && BossSequenceController.IsInSequence
            && !string.IsNullOrEmpty(info.SceneName)
            && GodhomeHubScenes.Contains(info.SceneName);

        private static bool IsMenuSkipSettingsEnabled() =>
            AutoSkipCinematics || AllowSkippingNonskippable || SkipCutscenesWithoutPrompt;

        private static bool IsAnyBossSkipEnabled() =>
            AbsoluteRadiance
            || HallOfGodsStatues
            || PureVesselRoar
            || GrimmNightmare
            || GreyPrinceZote
            || Collector
            || SoulMasterPhaseTransitionSkip;

        private static bool IsBossOrPantheonScene()
        {
            if (BossSequenceController.IsInSequence)
            {
                return true;
            }

            string sceneName = GameManager.instance?.GetSceneNameString() ?? string.Empty;
            if (sceneName.Length == 0)
            {
                return false;
            }

            if (sceneName.StartsWith("GG_Collector", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return PantheonLikeScenes.Contains(sceneName)
                || BossAnimationSceneNames.Contains(sceneName);
        }

        private static bool ShouldAutoSkipCinematicsNow() =>
            AutoSkipCinematics
            && !suppressAutoSkipForTransition
            && !IsBossOrPantheonScene();

        private static IEnumerator SuppressTimeScaleDuringReturn(GameManager.SceneLoadInfo info, int generation)
        {
            if (generation != timeScaleOverrideGeneration)
            {
                yield break;
            }

            if (timeScaleOverrideActive)
            {
                yield break;
            }

            if (!ShouldSuppressAutoSkip(info))
            {
                yield break;
            }

            if (global::GodhomeQoL.Modules.Tools.SpeedChanger.HasManagedTimeScaleControl)
            {
                yield break;
            }

            if (Math.Abs(Time.timeScale - 1f) < 0.001f)
            {
                yield break;
            }

            if (!global::GodhomeQoL.Modules.Tools.SpeedChanger.TryBeginTimeScaleOverride(1f, out int handle))
            {
                yield break;
            }

            timeScaleOverrideActive = true;
            timeScaleOverrideHandle = handle;
            try
            {
                yield return new UnityEngine.WaitForSecondsRealtime(3f);
            }
            finally
            {
                if (generation == timeScaleOverrideGeneration
                    && timeScaleOverrideActive
                    && timeScaleOverrideHandle == handle)
                {
                    global::GodhomeQoL.Modules.Tools.SpeedChanger.EndTimeScaleOverride(handle);
                    timeScaleOverrideHandle = 0;
                    timeScaleOverrideActive = false;
                }
            }
        }

        private static void TrySetWaitTime(PlayMakerFSM fsm, string stateName, float value)
        {
            try
            {
                Wait? wait = fsm.GetAction<Wait>(stateName);
                if (wait != null)
                {
                    wait.time = value;
                }
            }
            catch (Exception ex)
            {
                LogDebug($"SkipCutscenes: failed to set Wait time in state '{stateName}' - {ex.Message}");
            }
        }

        private static void TrySetStateWaitValue(PlayMakerFSM fsm, string stateName, float value)
        {
            try
            {
                Wait? wait = fsm.Fsm.GetState(stateName)?.Actions?.OfType<Wait>().FirstOrDefault();
                if (wait != null)
                {
                    wait.time.Value = value;
                }
            }
            catch (Exception ex)
            {
                LogDebug($"SkipCutscenes: failed to set Wait.Value in state '{stateName}' - {ex.Message}");
            }
        }

        private static void OnSetSkip(On.InputHandler.orig_SetSkipMode orig, InputHandler self, SkipPromptMode newmode)
        {
            if (suppressAutoSkipForTransition || !IsMenuSkipSettingsEnabled() || IsBossOrPantheonScene())
            {
                orig(self, newmode);
                return;
            }

            if (AllowSkippingNonskippable && newmode is not (SkipPromptMode.SKIP_INSTANT or SkipPromptMode.SKIP_PROMPT))
            {
                newmode = SkipCutscenesWithoutPrompt ? SkipPromptMode.SKIP_INSTANT : SkipPromptMode.SKIP_PROMPT;
            }
            else if (SkipCutscenesWithoutPrompt && newmode == SkipPromptMode.SKIP_PROMPT)
            {
                newmode = SkipPromptMode.SKIP_INSTANT;
            }

            orig(self, newmode);
        }

        private static void AnimatorBegin(On.AnimatorSequence.orig_Begin orig, AnimatorSequence self)
        {
            if (ShouldAutoSkipCinematicsNow())
                self.Skip();
            else
                orig(self);
        }

        private static void FadeBegin(On.FadeSequence.orig_Begin orig, FadeSequence self)
        {
            if (ShouldAutoSkipCinematicsNow())
                self.Skip();
            else
                orig(self);
        }

        private static void CinematicBegin(On.CinematicSequence.orig_Begin orig, CinematicSequence self)
        {
            if (ShouldAutoSkipCinematicsNow())
                self.Skip();
            else
                orig(self);
        }
    }
}
