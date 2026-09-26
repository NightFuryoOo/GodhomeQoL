using Vasi;
using Random = UnityEngine.Random;

namespace GodhomeQoL.Modules.QoL
{

    public sealed partial class SkipCutscenes : Module
    {
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
            || SoulMasterPhaseTransitionSkip
            || PantheonVEnding;

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
    }
}
