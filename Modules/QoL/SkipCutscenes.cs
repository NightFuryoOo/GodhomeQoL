using Vasi;
using static UnityEngine.UI.GridLayoutGroup;
using Random = UnityEngine.Random;

namespace SafeGodseekerQoL.Modules.QoL
{

    [UsedImplicitly]
    public class SkipCutscenes : Module
    {
        #region Settings

        [GlobalSetting]
        [BoolOption]
        public static bool DreamersGet = true;

        [GlobalSetting]
        [BoolOption]
        public static bool AbsoluteRadiance = true;

        [GlobalSetting]
        [BoolOption]
        public static bool AbyssShriekGet = true;

        [GlobalSetting]
        [BoolOption]
        public static bool AfterKingsBrandGet = true;

        [GlobalSetting]
        [BoolOption]
        public static bool BlackEggOpen = true;

        [GlobalSetting]
        [BoolOption]
        public static bool StagArrive = true;

        [GlobalSetting]
        [BoolOption]
        public static bool HallOfGodsStatues = true;

        [GlobalSetting]
        [BoolOption]
        public static bool GodhomeEntry = true;

        [GlobalSetting]
        [BoolOption]
        public static bool PureVesselRoar = true;

        [GlobalSetting]
        [BoolOption]
        public static bool GrimmNightmare = true;

        [GlobalSetting]
        [BoolOption]
        public static bool GreyPrinceZote = true;

        [GlobalSetting]
        [BoolOption]
        public static bool Collector = true;

        [GlobalSetting]
        [BoolOption]
        public static bool FirstTimeBosses = true;

        [GlobalSetting]
        [BoolOption]
        public static bool FirstCharm = true;

        [GlobalSetting]
        [BoolOption]
        public static bool AutoSkipCinematics = true;

        [GlobalSetting]
        [BoolOption]
        public static bool AllowSkippingNonskippable = true;

        [GlobalSetting]
        [BoolOption]
        public static bool SkipCutscenesWithoutPrompt = true;

        [GlobalSetting]
        [BoolOption]
        public static bool InstantSceneFadeIns = true;

        [GlobalSetting]
        [BoolOption]
        public static bool SoulMasterPhaseTransitionSkip = true;

        #endregion
        public override bool DefaultEnabled => true;

        public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

        private const string GUARDIAN = "Dream_Guardian_";

        private static readonly string[] DREAMERS = { "Deepnest_Spider_Town", "Fungus3_archive_02", "Ruins2_Watcher_Room" };

        private static readonly string[] ALL_DREAMER_BOOLS =
        {
            nameof(PlayerData.corniferAtHome),
            nameof(PlayerData.iseldaConvo1),
            nameof(PlayerData.dungDefenderSleeping),
            nameof(PlayerData.corn_crossroadsLeft),
            nameof(PlayerData.corn_greenpathLeft),
            nameof(PlayerData.corn_fogCanyonLeft),
            nameof(PlayerData.corn_fungalWastesLeft),
            nameof(PlayerData.corn_cityLeft),
            nameof(PlayerData.corn_waterwaysLeft),
            nameof(PlayerData.corn_minesLeft),
            nameof(PlayerData.corn_cliffsLeft),
            nameof(PlayerData.corn_deepnestLeft),
            nameof(PlayerData.corn_outskirtsLeft),
            nameof(PlayerData.corn_royalGardensLeft),
            nameof(PlayerData.corn_abyssLeft),
            nameof(PlayerData.metIselda),
        };

        private static readonly (Func<bool>, Func<Scene, IEnumerator>)[] FSM_SKIPS =
        {
            (() => DreamersGet, DreamerFsm),
            (() => AbsoluteRadiance, AbsRadSkip),
            (() => PureVesselRoar, HKPrimeSkip),
            (() => GrimmNightmare, GrimmNightmareSkip),
            (() => GreyPrinceZote, GreyPrinceZoteSkip),
            (() => Collector, CollectorSkip),
            (() => HallOfGodsStatues, StatueWait),
            (() => StagArrive, StagCutscene),
            (() => AbyssShriekGet, AbyssShriekPickup),
            (() => AfterKingsBrandGet, KingsBrand),
            (() => BlackEggOpen, BlackEgg)
        };



        private static readonly string[] PD_BOOLS =
        {
            nameof(PlayerData.unchainedHollowKnight),
            nameof(PlayerData.encounteredMimicSpider),
            nameof(PlayerData.infectedKnightEncountered),
            nameof(PlayerData.mageLordEncountered),
            nameof(PlayerData.mageLordEncountered_2),
        };

        private protected override void Load()
        {
            On.CinematicSequence.Begin += CinematicBegin;
            On.FadeSequence.Begin += FadeBegin;
            On.AnimatorSequence.Begin += AnimatorBegin;
            On.InputHandler.SetSkipMode += OnSetSkip;
            On.GameManager.BeginSceneTransitionRoutine += OnBeginSceneTransition;
            On.HutongGames.PlayMaker.Actions.EaseColor.OnEnter += FastEaseColor;
            On.GameManager.FadeSceneInWithDelay += NoFade;
            On.GGCheckIfBossScene.OnEnter += MageLordPhaseTransitionSkip;
            ModHooks.NewGameHook += OnNewGame;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += FsmSkips;
        }

        private protected override void Unload()
        {
            On.CinematicSequence.Begin -= CinematicBegin;
            On.FadeSequence.Begin -= FadeBegin;
            On.AnimatorSequence.Begin -= AnimatorBegin;
            On.InputHandler.SetSkipMode -= OnSetSkip;
            On.GameManager.BeginSceneTransitionRoutine -= OnBeginSceneTransition;
            On.HutongGames.PlayMaker.Actions.EaseColor.OnEnter -= FastEaseColor;
            On.GameManager.FadeSceneInWithDelay -= NoFade;
            On.GGCheckIfBossScene.OnEnter -= MageLordPhaseTransitionSkip;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= FsmSkips;
            ModHooks.NewGameHook -= OnNewGame;
        }

        private static void OnNewGame()
        {
            if (FirstCharm)
                PlayerData.instance.SetBool(nameof(PlayerData.hasCharm), true);

            if (GodhomeEntry)
                PlayerData.instance.SetBool(nameof(PlayerData.enteredGGAtrium), true);

            if (!FirstTimeBosses)
                return;

            foreach (string @bool in PD_BOOLS)
            {
                PlayerData.instance.SetBool(@bool, true);
            }
        }

        private static IEnumerator NoFade(On.GameManager.orig_FadeSceneInWithDelay orig, GameManager self, float delay)
        {
            yield return orig(self, InstantSceneFadeIns ? 0 : delay);
        }

        private static void FastEaseColor(On.HutongGames.PlayMaker.Actions.EaseColor.orig_OnEnter orig, EaseColor self)
        {
            if (InstantSceneFadeIns && self.Owner.name == "Blanker White" && Math.Abs(self.time.Value - 0.3) < .05)
            {
                self.time.Value = 0.066f;
            }

            orig(self);
        }

        private static void MageLordPhaseTransitionSkip(On.GGCheckIfBossScene.orig_OnEnter orig, GGCheckIfBossScene self)
        {
            if (
                !SoulMasterPhaseTransitionSkip
                || !self.Owner.transform.name.Contains("Corpse Mage")
                || !self.Fsm.ActiveStateName.Contains("Quick Death?")
                || self.Fsm.ActiveState.Actions[1] is not PlayerDataBoolTest p
            )
            {
                orig(self);
                return;
            }

            self.Fsm.Event(p.isTrue);
        }

        private static void FsmSkips(Scene arg0, Scene arg1)
        {
            var hc = HeroController.instance;

            if (hc == null) return;

            foreach (var (check, coro) in FSM_SKIPS)
            {
                if (check())
                    hc.StartCoroutine(coro(arg1));
            }
        }

        private static IEnumerator StagCutscene(Scene _)
        {
            yield return null;

            GameObject stag = GameObject.Find("Stag");

            if (stag == null)
                yield break;

            PlayMakerFSM ctrl = stag.LocateMyFSM("Stag Control");

            ctrl.GetState("Arrive Pause").RemoveAction<Wait>();
            ctrl.GetState("Activate").RemoveAction<Wait>();
            var anim = stag.GetComponent<tk2dSpriteAnimator>();

            anim.GetClipByName("Arrive").fps = 600;

            GameObject grate = ctrl.GetState("Open Grate").GetAction<Tk2dPlayAnimationWithEvents>().gameObject.GameObject.Value;

            if (grate == null)
                yield break;

            var grate_anim = grate.GetComponent<tk2dSpriteAnimator>();

            grate_anim.GetClipByName("Grate_disappear").fps = 600;
        }

        private static IEnumerator StatueWait(Scene arg1)
        {
            if (arg1.name != "GG_Workshop") yield break;

            foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>().Where(x => x.FsmName == "GG Boss UI"))
            {
                fsm.GetState("On Left").ChangeTransition("FINISHED", "Dream Box Down");
                fsm.GetState("On Right").ChangeTransition("FINISHED", "Dream Box Down");
                fsm.GetState("Dream Box Down").InsertAction(0, fsm.GetAction<SetPlayerDataString>("Impact"));
            }
        }

        private static IEnumerator HKPrimeSkip(Scene arg1)
        {
            if (arg1.name != "GG_Hollow_Knight") yield break;

            yield return null;

            PlayMakerFSM control = GameObject.Find("HK Prime").LocateMyFSM("Control");

            control.GetState("Init").ChangeTransition("FINISHED", "Intro Roar");

            control.GetAction<Wait>("Intro 2").time = 0.01f;
            control.GetAction<Wait>("Intro 1").time = 0.01f;
            control.GetAction<Wait>("Intro Roar").time = 1f;
        }
        private static IEnumerator GrimmNightmareSkip(Scene arg1)
        {
            if (arg1.name != "GG_Grimm_Nightmare") yield break;

            yield return null;

            PlayMakerFSM control = GameObject.Find("Grimm Control").LocateMyFSM("Control");

            if (control != null)
            {
                control.GetState("Pause").GetAction<Wait>().time.Value = 0.5f;
                control.GetState("Pan Over").GetAction<Wait>().time.Value = 0.5f;
                control.GetState("Eye 1").GetAction<Wait>().time.Value = 0.1f;
                control.GetState("Eye 2").GetAction<Wait>().time.Value = 0.1f;
                control.GetState("Pan Over 2").GetAction<Wait>().time.Value = 0.1f;
                control.GetState("Eye 3").GetAction<Wait>().time.Value = 0.1f;
                control.GetState("Eye 4").GetAction<Wait>().time.Value = 0.1f;
                control.GetState("Silhouette").GetAction<Wait>().time.Value = 0.1f;
                control.GetState("Silhouette 2").GetAction<Wait>().time.Value = 0.1f;
                control.GetState("Title Up").GetAction<Wait>().time.Value = 0.1f;
                control.GetState("Title Up 2").GetAction<Wait>().time.Value = 0.1f;
                control.GetState("Defeated Pause").GetAction<Wait>().time.Value = 0.1f;
                control.GetState("Defeated Start").GetAction<Wait>().time.Value = 0.1f;
                control.GetState("Explode Start").GetAction<Wait>().time.Value = 0.1f;
                control.GetState("Silhouette Up").GetAction<Wait>().time.Value = 0.1f;
                control.GetState("Ash Away").GetAction<Wait>().time.Value = 0.1f;
                control.GetState("Fade").GetAction<Wait>().time.Value = 0.1f;

            }

        }

        private static IEnumerator CollectorSkip(Scene scene)
        {
            if (!scene.name.Contains("GG_Collector")) yield break;

            yield return null;

            PlayMakerFSM control = GameObject.Find("Jar Collector").LocateMyFSM("Control");

            if (control != null)
            {
                control.GetState("Roar").GetAction<Wait>().time.Value = 0.5f;
                control.GetState("Roar").RemoveAction(6);
                control.GetState("Roar").RemoveAction(7);
            }
        }

        private static IEnumerator GreyPrinceZoteSkip(Scene scene)
        {
            if (scene.name != "GG_Grey_Prince_Zote") yield break;

            yield return null;

            PlayMakerFSM control = GameObject.Find("Grey Prince Title").LocateMyFSM("Control");

            if (control != null)
            {
                control.GetState("Get Level").GetAction<Wait>().time.Value = 0.1f;
                control.GetState("Main Title Pause").GetAction<Wait>().time.Value = 0.1f;
                control.GetState("Main Title").GetAction<Wait>().time.Value = 0.5f;
                control.GetState("Extra 1").GetAction<Wait>().time.Value = 0.01f;
                control.GetState("Extra 2").GetAction<Wait>().time.Value = 0.01f;
                control.GetState("Extra 3").GetAction<Wait>().time.Value = 0.01f;
                control.GetState("Extra 4").GetAction<Wait>().time.Value = 0.01f;
                control.GetState("Extra 5").GetAction<Wait>().time.Value = 0.01f;
                control.GetState("Extra 6").GetAction<Wait>().time.Value = 0.01f;
                control.GetState("Extra 7").GetAction<Wait>().time.Value = 0.01f;
                control.GetState("Extra 8").GetAction<Wait>().time.Value = 0.01f;
                control.GetState("Extra 9").GetAction<Wait>().time.Value = 0.01f;
                control.GetState("Extra 10").GetAction<Wait>().time.Value = 0.01f;
                control.GetState("Extra 11").GetAction<Wait>().time.Value = 0.01f;
                control.GetState("Extra 12").GetAction<Wait>().time.Value = 0.01f;
                control.GetState("Extra 13").GetAction<Wait>().time.Value = 0.01f;
            }
        }

        private static IEnumerator AbsRadSkip(Scene arg1)
        {
            if (arg1.name != "GG_Radiance") yield break;

            yield return null;
            try
            {
                PlayMakerFSM control = GameObject.Find("Boss Control").LocateMyFSM("Control");

                UObject.Destroy(GameObject.Find("Sun"));
                UObject.Destroy(GameObject.Find("feather_particles"));

                FsmState setup = Vasi.FsmUtil.GetState(control, "Setup");

                setup.GetAction<Wait>().time = 1.5f;
                setup.RemoveAction<SetPlayerDataBool>();
                //Vasi.FsmUtil.RemoveAction(setup, 3);
                setup.ChangeTransition("FINISHED", "Appear Boom");

                control.GetAction<Wait>("Title Up").time = 1f;

            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        private static IEnumerator DreamerFsm(Scene arg1)
        {
            if (!DREAMERS.Contains(arg1.name)) yield break;

            yield return null;

            GameObject dreamEnter = GameObject.Find("Dream Enter");

            if (dreamEnter == null)
                yield break;

            dreamEnter.LocateMyFSM("Control").GetState("Idle").ChangeTransition("DREAM HIT", "Change Scene");
        }

        private static IEnumerator AbyssShriekPickup(Scene arg1)
        {
            if (arg1.name != "Abyss_12") yield break;

            yield return null;

            PlayMakerFSM shriek = GameObject.Find("Scream 2 Get").LocateMyFSM("Scream Get");

            shriek.GetState("Move To").ChangeTransition("FINISHED", "Get");
            shriek.GetState("Get Pause").RemoveAllOfType<Wait>();
            shriek.GetState("Land").RemoveAllOfType<Wait>();

        }

        private static IEnumerator KingsBrand(Scene arg1)
        {
            yield return KingsBrandHornet(arg1);
            yield return KingsBrandAvalanche(arg1);
        }

        private static IEnumerator KingsBrandHornet(Scene arg1)
        {
            if (arg1.name != "Deepnest_East_12") yield break;

            yield return null;

            PlayMakerFSM hornet = GameObject.Find("Hornet Blizzard Return Scene").LocateMyFSM("Control");



            hornet.GetState("Fade Pause").RemoveAllOfType<Wait>();
            hornet.GetState("Fade In").RemoveAllOfType<Wait>();
            hornet.GetState("Land").RemoveAllOfType<Wait>();
        }

        private static IEnumerator KingsBrandAvalanche(Scene arg1)
        {
            if (arg1.name != "Room_Wyrm") yield break;

            yield return null;

            PlayMakerFSM avalanche = GameObject.Find("Avalanche End").LocateMyFSM("Control");


            avalanche.GetState("Fade").GetAction<Wait>().time = 1;
        }

        private static IEnumerator BlackEgg(Scene arg1)
        {
            if (arg1.name != "Room_temple") yield break;

            yield return null;

            PlayMakerFSM door = GameObject.Find("Final Boss Door").LocateMyFSM("Control");


            door.GetState("Take Control").RemoveAllOfType<Wait>();
            door.GetState("Shake").GetAction<Wait>().time = 1;
            door.GetState("Barrier Flash").RemoveAllOfType<Wait>();
            door.GetState("Blow").RemoveAllOfType<Wait>();
            door.GetState("Door Off").RemoveAllOfType<Wait>();
            door.GetState("Roar").RemoveAllOfType<Wait>();
            door.GetState("Roar End").GetAction<Wait>().time = 1;
        }

        private static IEnumerator OnBeginSceneTransition(On.GameManager.orig_BeginSceneTransitionRoutine orig, GameManager self, GameManager.SceneLoadInfo info)
        {
            if (InstantSceneFadeIns)
            {
                info.EntryDelay = 0f;
            }

            if (!DreamersGet || info.SceneName.Length <= 15 || info.SceneName.Substring(0, 15) != GUARDIAN)
            {
                yield return orig(self, info);

                yield break;
            }

            string @bool = info.SceneName.Substring(15);

            PlayerData pd = PlayerData.instance;

            pd.SetBool($"{@bool.ToLower()}Defeated", true);
            pd.SetBool($"maskBroken{@bool}", true);
            pd.guardiansDefeated++;
            pd.crossroadsInfected = true;

            if (pd.guardiansDefeated == 3)
            {
                pd.mrMushroomState = 1;
                pd.brettaState++;

                foreach (string pdBool in ALL_DREAMER_BOOLS)
                    pd.SetBool(pdBool, true);
            }

            info.SceneName = GameManager.instance.sceneName;
            info.EntryGateName = "door_dreamReturn";

            GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE INSTANT");

            yield return orig(self, info);

            GameCameras.instance.cameraFadeFSM.Fsm.SetState("FadeIn");

            yield return null;

            HeroController.instance.MaxHealth();

            while (GameManager.instance.gameState != GameState.PLAYING)
            {
                yield return null;
            }

            yield return null;

            PlayMakerFSM.BroadcastEvent("BOX DOWN");
            PlayMakerFSM.BroadcastEvent("BOX DOWN DREAM");

            pd.SetBenchRespawn(UObject.FindObjectOfType<RespawnMarker>(), GameManager.instance.sceneName, 2);
            GameManager.instance.SaveGame();

            HeroController.instance.AcceptInput();
        }

        private static void OnSetSkip(On.InputHandler.orig_SetSkipMode orig, InputHandler self, SkipPromptMode newmode)
        {
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
            if (AutoSkipCinematics)
                self.Skip();
            else
                orig(self);
        }

        private static void FadeBegin(On.FadeSequence.orig_Begin orig, FadeSequence self)
        {
            if (AutoSkipCinematics)
                self.Skip();
            else
                orig(self);
        }

        private static void CinematicBegin(On.CinematicSequence.orig_Begin orig, CinematicSequence self)
        {
            if (AutoSkipCinematics)
                self.Skip();
            else
                orig(self);
        }
    }
}

