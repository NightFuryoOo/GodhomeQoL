using Vasi;
using Random = UnityEngine.Random;

namespace GodhomeQoL.Modules.QoL
{

    public sealed partial class SkipCutscenes : Module
    {
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

        private static IEnumerator PantheonVEndingSkip(Scene arg1)
        {
            if (arg1.name != AbsoluteRadianceScene)
            {
                yield break;
            }

            for (int i = 0; i < 60; i++)
            {
                PlayMakerFSM? control = FindAbsoluteRadianceControlFsm();
                if (TryPatchPantheonVEndingFsm(control))
                {
                    yield break;
                }

                yield return null;
            }

            LogDebug("SkipCutscenes: Pantheon V ending skip patch was not applied because Absolute Radiance Control FSM was not ready");
        }

        private static PlayMakerFSM? FindAbsoluteRadianceControlFsm()
        {
            GameObject? absoluteRadiance = GameObject.Find("Absolute Radiance");
            PlayMakerFSM? control = absoluteRadiance?.LocateMyFSM("Control");
            if (IsAbsoluteRadianceEndingControlFsm(control))
            {
                return control;
            }

            return UObject.FindObjectsOfType<PlayMakerFSM>()
                .FirstOrDefault(fsm => fsm != null
                    && fsm.FsmName == "Control"
                    && IsAbsoluteRadianceEndingControlFsm(fsm));
        }

        private static bool TryPatchPantheonVEndingFsm(PlayMakerFSM? control)
        {
            if (!PantheonVEnding || !BossSequenceController.IsInSequence || !IsAbsoluteRadianceEndingControlFsm(control))
            {
                return false;
            }

            try
            {
                SetStaticVariable? endingSceneAction = control!.GetAction<SetStaticVariable>("Ending Scene", 1);
                if (endingSceneAction?.setValue == null)
                {
                    return false;
                }

                endingSceneAction.setValue.boolValue = false;
                return true;
            }
            catch (Exception ex)
            {
                LogDebug($"SkipCutscenes: Pantheon V ending skip patch failed - {ex.Message}");
                return false;
            }
        }

        private static bool IsAbsoluteRadianceEndingControlFsm(PlayMakerFSM? fsm) =>
            fsm != null
            && fsm.FsmName == "Control"
            && fsm.gameObject != null
            && fsm.gameObject.name == "Absolute Radiance"
            && fsm.Fsm.GetState("Ending Scene") != null;
    }
}
