using Vasi;
using Random = UnityEngine.Random;

namespace GodhomeQoL.Modules.QoL
{

    public sealed partial class SkipCutscenes : Module
    {
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
            On.PlayMakerFSM.Start += OnPlayMakerFsmStart;
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
            On.PlayMakerFSM.Start -= OnPlayMakerFsmStart;
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

        private static void OnPlayMakerFsmStart(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
        {
            orig(self);

            _ = TryPatchPantheonVEndingFsm(self);
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
