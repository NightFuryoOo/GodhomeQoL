using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private bool GetBossChallengeMasterEnabled() => bossChallengeMasterEnabled;

        private void SetBossChallengeMasterEnabled(bool value)
        {
            if (bossChallengeMasterEnabled == value)
            {
                return;
            }

            bossChallengeMasterEnabled = value;
            if (!value)
            {
                CaptureBossChallengeSnapshot();
                SetBossChallengeAll(false);
            }
            else
            {
                RestoreBossChallengeSnapshot();
            }

            RefreshBossChallengeUi();
            UpdateQuickMenuEntryStateColors();
            SaveMasterSettings();
        }

        private void CaptureBossChallengeSnapshot()
        {
            bossChallengeMasterHasSnapshot = true;
            bossChallengeSavedInfiniteChallenge = GetInfiniteChallengeEnabled();
            bossChallengeSavedRestartOnSuccess = Modules.BossChallenge.InfiniteChallenge.restartFightOnSuccess;
            bossChallengeSavedRestartAndMusic = Modules.BossChallenge.InfiniteChallenge.restartFightAndMusic;
            bossChallengeSavedCarefreeMelodyMode = GetCarefreeMelodyMode();
            bossChallengeSavedForceArrive = GetForceArriveAnimationEnabled();
            bossChallengeSavedInfiniteGrimm = GetInfiniteGrimmPufferfishEnabled();
            bossChallengeSavedInfiniteRadiance = GetInfiniteRadianceClimbingEnabled();
            bossChallengeSavedSegmentedP5 = GetSegmentedP5Enabled();
            bossChallengeSavedAddLifeblood = GetAddLifebloodEnabled();
            bossChallengeSavedAddSoul = GetAddSoulEnabled();
        }

        private void RestoreBossChallengeSnapshot()
        {
            if (!bossChallengeMasterHasSnapshot)
            {
                return;
            }

            SetInfiniteChallengeEnabled(bossChallengeSavedInfiniteChallenge);
            Modules.BossChallenge.InfiniteChallenge.restartFightOnSuccess = bossChallengeSavedRestartOnSuccess;
            Modules.BossChallenge.InfiniteChallenge.restartFightAndMusic = bossChallengeSavedRestartAndMusic;
            SetCarefreeMelodyMode(bossChallengeSavedCarefreeMelodyMode);
            SetForceArriveAnimationEnabled(bossChallengeSavedForceArrive);
            SetInfiniteGrimmPufferfishEnabled(bossChallengeSavedInfiniteGrimm);
            SetInfiniteRadianceClimbingEnabled(bossChallengeSavedInfiniteRadiance);
            SetSegmentedP5Enabled(bossChallengeSavedSegmentedP5);
            SetAddLifebloodEnabled(bossChallengeSavedAddLifeblood);
            SetAddSoulEnabled(bossChallengeSavedAddSoul);
        }

        private void SetBossChallengeAll(bool value)
        {
            SetInfiniteChallengeEnabled(value);
            Modules.BossChallenge.InfiniteChallenge.restartFightOnSuccess = value;
            Modules.BossChallenge.InfiniteChallenge.restartFightAndMusic = value;
            SetCarefreeMelodyMode(value ? Modules.QoL.CarefreeMelodyReset.ModeHoG : Modules.QoL.CarefreeMelodyReset.ModeOff);
            SetForceArriveAnimationEnabled(value);
            SetInfiniteGrimmPufferfishEnabled(value);
            SetInfiniteRadianceClimbingEnabled(value);
            SetSegmentedP5Enabled(value);
            SetAddLifebloodEnabled(value);
            SetAddSoulEnabled(value);
        }

        private bool GetBossAnimationMasterEnabled() => bossAnimMasterEnabled;

        private void SetBossAnimationMasterEnabled(bool value)
        {
            if (bossAnimMasterEnabled == value)
            {
                return;
            }

            bossAnimMasterEnabled = value;
            if (!value)
            {
                CaptureBossAnimationSnapshot();
                SetBossAnimationAll(false);
            }
            else
            {
                RestoreBossAnimationSnapshot();
            }

            RefreshBossAnimationUi();
            UpdateQuickMenuEntryStateColors();
            SaveMasterSettings();
        }

        private void CaptureBossAnimationSnapshot()
        {
            bossAnimMasterHasSnapshot = true;
            bossAnimSavedHallOfGods = Modules.QoL.SkipCutscenes.HallOfGodsStatues;
            bossAnimSavedAbsoluteRadiance = Modules.QoL.SkipCutscenes.AbsoluteRadiance;
            bossAnimSavedPantheonVEnding = Modules.QoL.SkipCutscenes.PantheonVEnding;
            bossAnimSavedPureVesselRoar = Modules.QoL.SkipCutscenes.PureVesselRoar;
            bossAnimSavedGrimmNightmare = Modules.QoL.SkipCutscenes.GrimmNightmare;
            bossAnimSavedGreyPrinceZote = Modules.QoL.SkipCutscenes.GreyPrinceZote;
            bossAnimSavedCollector = Modules.QoL.SkipCutscenes.Collector;
            bossAnimSavedSoulMaster = Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip;
            bossAnimSavedCollectorRoar = GetCollectorRoarEnabled();
        }

        private void RestoreBossAnimationSnapshot()
        {
            if (!bossAnimMasterHasSnapshot)
            {
                return;
            }

            Modules.QoL.SkipCutscenes.HallOfGodsStatues = bossAnimSavedHallOfGods;
            Modules.QoL.SkipCutscenes.AbsoluteRadiance = bossAnimSavedAbsoluteRadiance;
            Modules.QoL.SkipCutscenes.PantheonVEnding = bossAnimSavedPantheonVEnding;
            Modules.QoL.SkipCutscenes.PureVesselRoar = bossAnimSavedPureVesselRoar;
            Modules.QoL.SkipCutscenes.GrimmNightmare = bossAnimSavedGrimmNightmare;
            Modules.QoL.SkipCutscenes.GreyPrinceZote = bossAnimSavedGreyPrinceZote;
            Modules.QoL.SkipCutscenes.Collector = bossAnimSavedCollector;
            Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip = bossAnimSavedSoulMaster;
            SetCollectorRoarEnabled(bossAnimSavedCollectorRoar);
        }

        private void SetBossAnimationAll(bool value)
        {
            Modules.QoL.SkipCutscenes.HallOfGodsStatues = value;
            Modules.QoL.SkipCutscenes.AbsoluteRadiance = value;
            Modules.QoL.SkipCutscenes.PantheonVEnding = value;
            Modules.QoL.SkipCutscenes.PureVesselRoar = value;
            Modules.QoL.SkipCutscenes.GrimmNightmare = value;
            Modules.QoL.SkipCutscenes.GreyPrinceZote = value;
            Modules.QoL.SkipCutscenes.Collector = value;
            Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip = value;
            SetCollectorRoarEnabled(value);
        }

        private void SaveMasterSettings()
        {
            QuickMenuMasterSettings settings = GodhomeQoL.GlobalSettings.QuickMenuMasters ??= new QuickMenuMasterSettings();

            settings.BossChallengeEnabled = bossChallengeMasterEnabled;
            settings.BossChallengeHasSnapshot = bossChallengeMasterHasSnapshot;
            settings.BossChallengeSavedInfiniteChallenge = bossChallengeSavedInfiniteChallenge;
            settings.BossChallengeSavedRestartOnSuccess = bossChallengeSavedRestartOnSuccess;
            settings.BossChallengeSavedRestartAndMusic = bossChallengeSavedRestartAndMusic;
            settings.BossChallengeSavedCarefreeMelody = bossChallengeSavedCarefreeMelodyMode != Modules.QoL.CarefreeMelodyReset.ModeOff;
            settings.BossChallengeSavedCarefreeMelodyMode = bossChallengeSavedCarefreeMelodyMode;
            settings.BossChallengeSavedInfiniteGrimm = bossChallengeSavedInfiniteGrimm;
            settings.BossChallengeSavedInfiniteRadiance = bossChallengeSavedInfiniteRadiance;
            settings.BossChallengeSavedSegmentedP5 = bossChallengeSavedSegmentedP5;
            settings.BossChallengeSavedAddLifeblood = bossChallengeSavedAddLifeblood;
            settings.BossChallengeSavedAddSoul = bossChallengeSavedAddSoul;
            settings.BossChallengeSavedForceArriveAnimation = bossChallengeSavedForceArrive;

            settings.QolEnabled = qolMasterEnabled;
            settings.QolHasSnapshot = qolMasterHasSnapshot;
            settings.QolSavedFastDreamWarp = qolSavedFastDreamWarp;
            settings.QolSavedShortDeath = qolSavedShortDeath;
            settings.QolSavedHallOfGods = false;
            settings.QolSavedUnlockAllModes = qolSavedUnlockAllModes;
            settings.QolSavedUnlockPantheons = qolSavedUnlockPantheons;
            settings.QolSavedUnlockRadiance = qolSavedUnlockRadiance;
            settings.QolSavedUnlockRadiant = qolSavedUnlockRadiant;
            settings.QolSavedInvincibleIndicator = qolSavedInvincibleIndicator;
            settings.QolSavedScreenShake = qolSavedScreenShake;

            settings.MenuAnimEnabled = menuAnimMasterEnabled;
            settings.MenuAnimHasSnapshot = menuAnimMasterHasSnapshot;
            settings.MenuAnimSavedDoorDefaultBegin = menuAnimSavedDoorDefaultBegin;
            settings.MenuAnimSavedFasterLoads = menuAnimSavedFasterLoads;
            settings.MenuAnimSavedFastMenus = menuAnimSavedFastMenus;
            settings.MenuAnimSavedFastText = menuAnimSavedFastText;
            settings.MenuAnimSavedAutoSkip = menuAnimSavedAutoSkip;
            settings.MenuAnimSavedAllowSkipping = menuAnimSavedAllowSkipping;
            settings.MenuAnimSavedSkipWithoutPrompt = menuAnimSavedSkipWithoutPrompt;

            settings.BossAnimEnabled = bossAnimMasterEnabled;
            settings.BossAnimHasSnapshot = bossAnimMasterHasSnapshot;
            settings.BossAnimSavedHallOfGods = bossAnimSavedHallOfGods;
            settings.BossAnimSavedAbsoluteRadiance = bossAnimSavedAbsoluteRadiance;
            settings.BossAnimSavedPantheonVEnding = bossAnimSavedPantheonVEnding;
            settings.BossAnimSavedPureVesselRoar = bossAnimSavedPureVesselRoar;
            settings.BossAnimSavedGrimmNightmare = bossAnimSavedGrimmNightmare;
            settings.BossAnimSavedGreyPrinceZote = bossAnimSavedGreyPrinceZote;
            settings.BossAnimSavedCollector = bossAnimSavedCollector;
            settings.BossAnimSavedSoulMaster = bossAnimSavedSoulMaster;
            settings.BossAnimSavedCollectorRoar = bossAnimSavedCollectorRoar;

            settings.RandomPantheonsEnabled = randomPantheonsMasterEnabled;
            settings.RandomPantheonsHasSnapshot = randomPantheonsMasterHasSnapshot;
            settings.RandomPantheonsSavedP1 = randomPantheonsSavedP1;
            settings.RandomPantheonsSavedP2 = randomPantheonsSavedP2;
            settings.RandomPantheonsSavedP3 = randomPantheonsSavedP3;
            settings.RandomPantheonsSavedP4 = randomPantheonsSavedP4;
            settings.RandomPantheonsSavedP5 = randomPantheonsSavedP5;

            settings.TrueBossRushEnabled = trueBossRushMasterEnabled;
            settings.TrueBossRushHasSnapshot = trueBossRushMasterHasSnapshot;
            settings.TrueBossRushSavedP1 = trueBossRushSavedP1;
            settings.TrueBossRushSavedP2 = trueBossRushSavedP2;
            settings.TrueBossRushSavedP3 = trueBossRushSavedP3;
            settings.TrueBossRushSavedP4 = trueBossRushSavedP4;
            settings.TrueBossRushSavedP5 = trueBossRushSavedP5;

            settings.CheatsEnabled = cheatsMasterEnabled;
            settings.CheatsHasSnapshot = cheatsMasterHasSnapshot;
            settings.CheatsSavedInfiniteSoul = cheatsSavedInfiniteSoul;
            settings.CheatsSavedInfiniteHp = cheatsSavedInfiniteHp;
            settings.CheatsSavedInvincibility = cheatsSavedInvincibility;
            settings.CheatsSavedNoclip = cheatsSavedNoclip;

            GodhomeQoL.SaveGlobalSettingsSafe();
        }

        private bool GetCheatsEnabled()
        {
            return GetCheatsModule()?.Enabled ?? false;
        }

        private void SetCheatsEnabled(bool value)
        {
            Module? module = GetCheatsModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateQuickMenuEntryStateColors();
        }

        private bool GetCheatsMasterEnabled() => cheatsMasterEnabled;

        private void SetCheatsMasterEnabled(bool value)
        {
            if (cheatsMasterEnabled == value)
            {
                return;
            }

            cheatsMasterEnabled = value;
            if (!value)
            {
                CaptureCheatsSnapshot();
                SetCheatsAll(false);
                SetCheatsEnabled(false);
            }
            else
            {
                if (!GetCheatsEnabled())
                {
                    SetCheatsEnabled(true);
                }

                RestoreCheatsSnapshot();
            }

            RefreshCheatsUi();
            UpdateQuickMenuEntryStateColors();
            SaveMasterSettings();
        }

        private void CaptureCheatsSnapshot()
        {
            cheatsMasterHasSnapshot = true;
            cheatsSavedInfiniteSoul = GetCheatsInfiniteSoulEnabled();
            cheatsSavedInfiniteHp = GetCheatsInfiniteHpEnabled();
            cheatsSavedInvincibility = GetCheatsInvincibilityEnabled();
            cheatsSavedNoclip = GetCheatsNoclipEnabled();
        }

        private void RestoreCheatsSnapshot()
        {
            if (!cheatsMasterHasSnapshot)
            {
                return;
            }

            SetCheatsInfiniteSoulEnabled(cheatsSavedInfiniteSoul);
            SetCheatsInfiniteHpEnabled(cheatsSavedInfiniteHp);
            SetCheatsInvincibilityEnabled(cheatsSavedInvincibility);
            SetCheatsNoclipEnabled(cheatsSavedNoclip);
        }

        private void SetCheatsAll(bool value)
        {
            SetCheatsInfiniteSoulEnabled(value);
            SetCheatsInfiniteHpEnabled(value);
            SetCheatsInvincibilityEnabled(value);
            SetCheatsNoclipEnabled(value);
        }

        private bool GetCheatsInfiniteSoulEnabled() => Modules.Cheats.Cheats.GetInfiniteSoulEnabled();

        private void SetCheatsInfiniteSoulEnabled(bool value)
        {
            Modules.Cheats.Cheats.SetInfiniteSoulEnabled(value);
            if (cheatsMasterEnabled)
            {
                cheatsMasterHasSnapshot = true;
                cheatsSavedInfiniteSoul = value;
            }

            SaveMasterSettings();
        }

        private bool GetCheatsInfiniteHpEnabled() => Modules.Cheats.Cheats.GetInfiniteHpEnabled();

        private void SetCheatsInfiniteHpEnabled(bool value)
        {
            Modules.Cheats.Cheats.SetInfiniteHpEnabled(value);
            if (cheatsMasterEnabled)
            {
                cheatsMasterHasSnapshot = true;
                cheatsSavedInfiniteHp = value;
            }

            SaveMasterSettings();
        }

        private bool GetCheatsInvincibilityEnabled() => Modules.Cheats.Cheats.GetInvincibilityEnabled();

        private void SetCheatsInvincibilityEnabled(bool value)
        {
            Modules.Cheats.Cheats.SetInvincibilityEnabled(value);
            if (cheatsMasterEnabled)
            {
                cheatsMasterHasSnapshot = true;
                cheatsSavedInvincibility = value;
            }

            SaveMasterSettings();
        }

        private bool GetCheatsNoclipEnabled() => Modules.Cheats.Cheats.GetNoclipEnabled();

        private void SetCheatsNoclipEnabled(bool value)
        {
            Modules.Cheats.Cheats.SetNoclipEnabled(value);
            if (cheatsMasterEnabled)
            {
                cheatsMasterHasSnapshot = true;
                cheatsSavedNoclip = value;
            }

            SaveMasterSettings();
        }
    }
}
