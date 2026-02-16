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
    {        internal static void ApplyInitialDefaults()
        {
            GodhomeQoL.GlobalSettings.QuickMenuMasters ??= new QuickMenuMasterSettings();

            SetModuleEnabled<Modules.QoL.FastSuperDash>(false);
            Modules.QoL.FastSuperDash.instantSuperDash = false;
            Modules.QoL.FastSuperDash.fastSuperDashEverywhere = false;
            Modules.QoL.FastSuperDash.fastSuperDashSpeedMultiplier = 1f;

            ResetCollectorPhasesDefaults();
            Modules.CollectorPhases.CollectorPhases.HoGOnly = true;
            SetModuleEnabled<Modules.QoL.CollectorRoarMute>(false);
            SetModuleEnabled<Modules.CollectorPhases.CollectorPhases>(false);

            SetModuleEnabled<Modules.FastReload>(false);
            Modules.FastReload.reloadKeyCode = (int)KeyCode.None;
            Modules.FastReload.teleportKeyCode = (int)KeyCode.None;

            Modules.QoL.DreamshieldStartAngle.startAngleEnabled = false;
            Modules.QoL.DreamshieldStartAngle.rotationDelay = 0f;
            Modules.QoL.DreamshieldStartAngle.rotationSpeed = 1f;

            ShowHpSettings.EnabledMod = false;
            ShowHpSettings.ShowPB = true;
            ShowHpSettings.HideAfter10Sec = true;
            ShowHpSettings.HudFadeSeconds = 5f;
            ApplyShowHpBinding(null);

            GodhomeQoL.GlobalSettings.MaskDamage ??= new MaskDamageSettings();
            GodhomeQoL.GlobalSettings.MaskDamage.Enabled = false;
            GodhomeQoL.GlobalSettings.MaskDamage.DamageMultiplier = 1;
            GodhomeQoL.GlobalSettings.MaskDamage.ShowUI = true;
            GodhomeQoL.GlobalSettings.MaskDamage.ToggleUiKeybind = string.Empty;

            GodhomeQoL.GlobalSettings.FreezeHitboxes ??= new FreezeHitboxesSettings();
            GodhomeQoL.GlobalSettings.FreezeHitboxes.Enabled = false;
            GodhomeQoL.GlobalSettings.FreezeHitboxes.AnyHits = false;
            GodhomeQoL.GlobalSettings.FreezeHitboxes.UnfreezeKeybind = string.Empty;

            SpeedChanger.globalSwitch = false;
            SpeedChanger.restrictToggleToRooms = false;
            SpeedChanger.unlimitedSpeed = false;
            SpeedChanger.displayStyle = 0;
            SpeedChanger.toggleKeybind = string.Empty;
            SpeedChanger.inputSpeedKeybind = string.Empty;
            SpeedChanger.slowDownKeybind = string.Empty;
            SpeedChanger.speedUpKeybind = string.Empty;
            SpeedChanger.speed = 1f;

            SetModuleEnabled<Modules.QoL.TeleportKit>(false);
            Modules.QoL.TeleportKit.MenuHotkey = KeyCode.F6;
            Modules.QoL.TeleportKit.SaveTeleportHotkey = KeyCode.R;
            Modules.QoL.TeleportKit.TeleportHotkey = KeyCode.T;

            SetModuleEnabled<Modules.BossChallenge.InfiniteChallenge>(false);
            SetModuleEnabled<Modules.BossChallenge.AlwaysFurious>(false);
            SetModuleEnabled<Modules.BossChallenge.RandomPantheons>(false);
            SetModuleEnabled<Modules.BossChallenge.TrueBossRush>(false);
            SetModuleEnabled<Modules.Cheats.Cheats>(false);
            SetModuleEnabled<Modules.BossChallenge.InfiniteGrimmPufferfish>(false);
            SetModuleEnabled<Modules.BossChallenge.InfiniteRadianceClimbing>(false);
            Modules.QoL.CarefreeMelodyReset.SetMode(Modules.QoL.CarefreeMelodyReset.ModeOff);
            SetModuleEnabled<Modules.BossChallenge.P5Health>(false);
            SetModuleEnabled<Modules.BossChallenge.SegmentedP5>(false);
            SetModuleEnabled<Modules.BossChallenge.HalveDamageHoGAscendedOrAbove>(false);
            SetModuleEnabled<Modules.BossChallenge.HalveDamageHoGAttuned>(false);
            SetModuleEnabled<Modules.BossChallenge.AddLifeblood>(false);
            SetModuleEnabled<Modules.BossChallenge.AddSoul>(false);
            SetModuleEnabled<Modules.BossChallenge.ForceArriveAnimation>(false);

            Modules.BossChallenge.InfiniteChallenge.restartFightOnSuccess = false;
            Modules.BossChallenge.InfiniteChallenge.restartFightAndMusic = false;
            Modules.BossChallenge.AddLifeblood.lifebloodAmount = 0;
            Modules.BossChallenge.AddSoul.soulAmount = 0;
            Modules.BossChallenge.SegmentedP5.selectedP5Segment = 0;

            ResetZoteHelperDefaults();
            Modules.BossChallenge.ForceGreyPrinceEnterType.gpzEnterType =
                Modules.BossChallenge.ForceGreyPrinceEnterType.EnterType.Off;
            SetModuleEnabled<Modules.BossChallenge.ZoteHelper>(false);

            SetModuleEnabled<Modules.QoL.FastDreamWarp>(false);
            FastDreamWarpSettings.Keybinds.Toggle.ClearBindings();
            SetModuleEnabled<Modules.QoL.ShortDeathAnimation>(true);
            SetModuleEnabled<Modules.QoL.InvincibleIndicator>(true);
            SetModuleEnabled<Modules.QoL.ScreenShake>(true);
            Modules.QoL.SkipCutscenes.HallOfGodsStatues = true;
            SetModuleEnabled<Modules.QoL.UnlockAllModes>(true);
            SetModuleEnabled<Modules.QoL.UnlockPantheons>(true);
            SetModuleEnabled<Modules.QoL.UnlockRadiance>(true);
            SetModuleEnabled<Modules.QoL.UnlockRadiant>(true);

            GodhomeQoL.GlobalSettings.QuickMenuOpacity = 100;
            GodhomeQoL.GlobalSettings.GearSwitcher ??= new GearSwitcherSettings();
            GodhomeQoL.GlobalSettings.GearSwitcher.Enabled = false;

            SetModuleEnabled<Modules.QoL.DoorDefaultBegin>(true);
            SetModuleEnabled<Modules.QoL.MemorizeBindings>(true);
            SetModuleEnabled<Modules.QoL.FasterLoads>(true);
            SetModuleEnabled<Modules.QoL.FastMenus>(true);
            SetModuleEnabled<Modules.QoL.FastText>(true);
            Modules.QoL.SkipCutscenes.AutoSkipCinematics = true;
            Modules.QoL.SkipCutscenes.AllowSkippingNonskippable = true;
            Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt = true;

            Modules.QoL.SkipCutscenes.AbsoluteRadiance = true;
            Modules.QoL.SkipCutscenes.PureVesselRoar = true;
            Modules.QoL.SkipCutscenes.GrimmNightmare = true;
            Modules.QoL.SkipCutscenes.GreyPrinceZote = true;
            Modules.QoL.SkipCutscenes.Collector = true;
            Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip = true;

            Modules.BossChallenge.RandomPantheons.Pantheon1Enabled = false;
            Modules.BossChallenge.RandomPantheons.Pantheon2Enabled = false;
            Modules.BossChallenge.RandomPantheons.Pantheon3Enabled = false;
            Modules.BossChallenge.RandomPantheons.Pantheon4Enabled = false;
            Modules.BossChallenge.RandomPantheons.Pantheon5Enabled = false;
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon1Enabled = false;
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon2Enabled = false;
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon3Enabled = false;
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon4Enabled = false;
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon5Enabled = false;
            Modules.Cheats.Cheats.InfiniteSoulEnabled = false;
            Modules.Cheats.Cheats.InfiniteHpEnabled = false;
            Modules.Cheats.Cheats.InvincibilityEnabled = false;
            Modules.Cheats.Cheats.NoclipEnabled = false;
            Modules.Cheats.Cheats.KillAllHotkeyRaw = string.Empty;

            GodhomeQoL.GlobalSettings.QuickMenuMasters.BossChallengeEnabled = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.BossChallengeHasSnapshot = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.QolEnabled = true;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.QolHasSnapshot = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.MenuAnimEnabled = true;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.MenuAnimHasSnapshot = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.BossAnimEnabled = true;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.BossAnimHasSnapshot = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.RandomPantheonsEnabled = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.RandomPantheonsHasSnapshot = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.RandomPantheonsSavedP1 = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.RandomPantheonsSavedP2 = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.RandomPantheonsSavedP3 = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.RandomPantheonsSavedP4 = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.RandomPantheonsSavedP5 = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.TrueBossRushEnabled = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.TrueBossRushHasSnapshot = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.TrueBossRushSavedP1 = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.TrueBossRushSavedP2 = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.TrueBossRushSavedP3 = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.TrueBossRushSavedP4 = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.TrueBossRushSavedP5 = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.CheatsEnabled = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.CheatsHasSnapshot = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.CheatsSavedInfiniteSoul = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.CheatsSavedInfiniteHp = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.CheatsSavedInvincibility = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.CheatsSavedNoclip = false;
        }

        private static void ResetCollectorPhasesDefaults()
        {
            try
            {
                MethodInfo? method = typeof(Modules.CollectorPhases.CollectorPhases)
                    .GetMethod("ResetDefaults", BindingFlags.NonPublic | BindingFlags.Static);
                if (method != null)
                {
                    method.Invoke(null, null);
                    return;
                }
            }
            catch
            {
                // Ignore reflection failures and fall back to hard-coded defaults.
            }

            Modules.CollectorPhases.CollectorPhases.collectorPhase = 3;
            Modules.CollectorPhases.CollectorPhases.collectorMaxHP = 1200;
            Modules.CollectorPhases.CollectorPhases.UseMaxHP = true;
            Modules.CollectorPhases.CollectorPhases.UseCustomPhase2Threshold = false;
            Modules.CollectorPhases.CollectorPhases.CustomPhase2Threshold = 850;
            Modules.CollectorPhases.CollectorPhases.buzzerHP = 26;
            Modules.CollectorPhases.CollectorPhases.rollerHP = 26;
            Modules.CollectorPhases.CollectorPhases.spitterHP = 26;
            Modules.CollectorPhases.CollectorPhases.spawnBuzzer = false;
            Modules.CollectorPhases.CollectorPhases.spawnRoller = false;
            Modules.CollectorPhases.CollectorPhases.spawnSpitter = true;
            Modules.CollectorPhases.CollectorPhases.CollectorImmortal = false;
            Modules.CollectorPhases.CollectorPhases.IgnoreInitialJarLimit = true;
            Modules.CollectorPhases.CollectorPhases.DisableSummonLimit = false;
            Modules.CollectorPhases.CollectorPhases.CustomSummonLimit = 20;
            Modules.CollectorPhases.CollectorPhases.HoGOnly = true;
        }

        private static void ResetZoteHelperDefaults()
        {
            Modules.BossChallenge.ZoteHelper.zoteBossHp = 1400;
            Modules.BossChallenge.ZoteHelper.zoteImmortal = false;
            Modules.BossChallenge.ZoteHelper.zoteSpawnFlying = true;
            Modules.BossChallenge.ZoteHelper.zoteSpawnHopping = true;
            Modules.BossChallenge.ZoteHelper.zoteSummonFlyingHp = 20;
            Modules.BossChallenge.ZoteHelper.zoteSummonHoppingHp = 20;
            Modules.BossChallenge.ZoteHelper.zoteSummonLimit = 3;
            Modules.BossChallenge.ZoteHelper.ZoteHoGOnly = true;
        }

        private bool GetModuleEnabled()
        {
            Module? module = GetFastSuperDashModule();
            return module?.Enabled ?? false;
        }

        private void SetModuleEnabled(bool value)
        {
            Module? module = GetFastSuperDashModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateFastSuperDashInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private bool GetCollectorPhasesEnabled()
        {
            Module? module = GetCollectorPhasesModule();
            return module?.Enabled ?? false;
        }

        private void SetCollectorPhasesEnabled(bool value)
        {
            Module? module = GetCollectorPhasesModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateCollectorInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private bool GetFastReloadEnabled()
        {
            Module? module = GetFastReloadModule();
            return module?.Enabled ?? false;
        }

        private void SetFastReloadEnabled(bool value)
        {
            Module? module = GetFastReloadModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateFastReloadInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private bool GetDreamshieldEnabled()
        {
            return Modules.QoL.DreamshieldStartAngle.startAngleEnabled;
        }

        private void SetDreamshieldEnabled(bool value)
        {
            Modules.QoL.DreamshieldStartAngle.startAngleEnabled = value;
            UpdateDreamshieldSliderState();
            UpdateDreamshieldInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private bool GetTeleportKitEnabled()
        {
            Module? module = GetTeleportKitModule();
            return module?.Enabled ?? false;
        }

        private void SetTeleportKitEnabled(bool value)
        {
            Module? module = GetTeleportKitModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateTeleportKitInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private bool GetShowHpOnDeathEnabled() => ShowHpSettings.EnabledMod;

        private void SetShowHpOnDeathEnabled(bool value)
        {
            ShowHpSettings.EnabledMod = value;
            UpdateShowHpOnDeathInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private bool GetMaskDamageEnabled()
        {
            return MaskDamage.GetEnabled();
        }

        private void SetMaskDamageEnabled(bool value)
        {
            MaskDamage.SetEnabled(value);

            UpdateMaskDamageInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private bool GetFreezeHitboxesEnabled()
        {
            return FreezeHitboxes.GetEnabled();
        }

        private void SetFreezeHitboxesEnabled(bool value)
        {
            FreezeHitboxes.SetEnabled(value);

            UpdateFreezeHitboxesInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private static string GetFreezeHitboxesModeLabel()
        {
            return FreezeHitboxes.GetAnyHitsMode() ? "Any Hits" : "Death";
        }

        private void ToggleFreezeHitboxesMode()
        {
            FreezeHitboxes.SetAnyHitsMode(!FreezeHitboxes.GetAnyHitsMode());
            if (freezeHitboxesModeValue != null)
            {
                freezeHitboxesModeValue.text = GetFreezeHitboxesModeLabel();
            }
        }

        private bool GetZoteHelperEnabled()
        {
            Module? module = GetZoteHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetZoteHelperEnabled(bool value)
        {
            Module? module = GetZoteHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateZoteHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetZoteBossHp(int value)
        {
            Modules.BossChallenge.ZoteHelper.zoteBossHp = Mathf.Clamp(value, 100, 999999);
            Modules.BossChallenge.ZoteHelper.ApplyBossHealthIfPresent();
        }

        private void SetZoteImmortalEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteImmortal = value;
            Modules.BossChallenge.ZoteHelper.ApplyBossHealthIfPresent();
        }

        private void SetZoteSpawnFlyingEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteSpawnFlying = value;
        }

        private void SetZoteSpawnHoppingEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteSpawnHopping = value;
        }

        private void SetZoteFlyingHp(int value)
        {
            Modules.BossChallenge.ZoteHelper.zoteSummonFlyingHp = Mathf.Clamp(value, 0, 99);
            Modules.BossChallenge.ZoteHelper.ApplyZotelingHealthIfPresent();
        }

        private void SetZoteHoppingHp(int value)
        {
            Modules.BossChallenge.ZoteHelper.zoteSummonHoppingHp = Mathf.Clamp(value, 0, 99);
            Modules.BossChallenge.ZoteHelper.ApplyZotelingHealthIfPresent();
        }

        private void SetCollectorPhase(int value)
        {
            int phase = Mathf.Clamp(value, 1, 3);
            Modules.CollectorPhases.CollectorPhases.collectorPhase = phase;

            if (phase == 2 && !Modules.CollectorPhases.CollectorPhases.IgnoreInitialJarLimit)
            {
                Modules.CollectorPhases.CollectorPhases.IgnoreInitialJarLimit = true;
                UpdateToggleValue(ignoreInitialJarLimitValue, true);
            }
        }

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
            bossChallengeSavedP5Health = GetP5HealthEnabled();
            bossChallengeSavedSegmentedP5 = GetSegmentedP5Enabled();
            bossChallengeSavedHalveAscended = GetHalveAscendedEnabled();
            bossChallengeSavedHalveAttuned = GetHalveAttunedEnabled();
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
            SetP5HealthEnabled(bossChallengeSavedP5Health);
            SetSegmentedP5Enabled(bossChallengeSavedSegmentedP5);
            SetHalveAscendedEnabled(bossChallengeSavedHalveAscended);
            SetHalveAttunedEnabled(bossChallengeSavedHalveAttuned);
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
            SetP5HealthEnabled(value);
            SetSegmentedP5Enabled(value);
            SetHalveAscendedEnabled(value);
            SetHalveAttunedEnabled(value);
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
            bossAnimSavedAbsoluteRadiance = Modules.QoL.SkipCutscenes.AbsoluteRadiance;
            bossAnimSavedPureVesselRoar = Modules.QoL.SkipCutscenes.PureVesselRoar;
            bossAnimSavedGrimmNightmare = Modules.QoL.SkipCutscenes.GrimmNightmare;
            bossAnimSavedGreyPrinceZote = Modules.QoL.SkipCutscenes.GreyPrinceZote;
            bossAnimSavedCollector = Modules.QoL.SkipCutscenes.Collector;
            bossAnimSavedSoulMaster = Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip;
        }

        private void RestoreBossAnimationSnapshot()
        {
            if (!bossAnimMasterHasSnapshot)
            {
                return;
            }

            Modules.QoL.SkipCutscenes.AbsoluteRadiance = bossAnimSavedAbsoluteRadiance;
            Modules.QoL.SkipCutscenes.PureVesselRoar = bossAnimSavedPureVesselRoar;
            Modules.QoL.SkipCutscenes.GrimmNightmare = bossAnimSavedGrimmNightmare;
            Modules.QoL.SkipCutscenes.GreyPrinceZote = bossAnimSavedGreyPrinceZote;
            Modules.QoL.SkipCutscenes.Collector = bossAnimSavedCollector;
            Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip = bossAnimSavedSoulMaster;
        }

        private void SetBossAnimationAll(bool value)
        {
            Modules.QoL.SkipCutscenes.AbsoluteRadiance = value;
            Modules.QoL.SkipCutscenes.PureVesselRoar = value;
            Modules.QoL.SkipCutscenes.GrimmNightmare = value;
            Modules.QoL.SkipCutscenes.GreyPrinceZote = value;
            Modules.QoL.SkipCutscenes.Collector = value;
            Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip = value;
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
            settings.BossChallengeSavedP5Health = bossChallengeSavedP5Health;
            settings.BossChallengeSavedSegmentedP5 = bossChallengeSavedSegmentedP5;
            settings.BossChallengeSavedHalveAscended = bossChallengeSavedHalveAscended;
            settings.BossChallengeSavedHalveAttuned = bossChallengeSavedHalveAttuned;
            settings.BossChallengeSavedAddLifeblood = bossChallengeSavedAddLifeblood;
            settings.BossChallengeSavedAddSoul = bossChallengeSavedAddSoul;
            settings.BossChallengeSavedForceArriveAnimation = bossChallengeSavedForceArrive;

            settings.QolEnabled = qolMasterEnabled;
            settings.QolHasSnapshot = qolMasterHasSnapshot;
            settings.QolSavedFastDreamWarp = qolSavedFastDreamWarp;
            settings.QolSavedShortDeath = qolSavedShortDeath;
            settings.QolSavedHallOfGods = qolSavedHallOfGods;
            settings.QolSavedUnlockAllModes = qolSavedUnlockAllModes;
            settings.QolSavedUnlockPantheons = qolSavedUnlockPantheons;
            settings.QolSavedUnlockRadiance = qolSavedUnlockRadiance;
            settings.QolSavedUnlockRadiant = qolSavedUnlockRadiant;
            settings.QolSavedInvincibleIndicator = qolSavedInvincibleIndicator;
            settings.QolSavedScreenShake = qolSavedScreenShake;

            settings.MenuAnimEnabled = menuAnimMasterEnabled;
            settings.MenuAnimHasSnapshot = menuAnimMasterHasSnapshot;
            settings.MenuAnimSavedDoorDefaultBegin = menuAnimSavedDoorDefaultBegin;
            settings.MenuAnimSavedMemorizeBindings = menuAnimSavedMemorizeBindings;
            settings.MenuAnimSavedFasterLoads = menuAnimSavedFasterLoads;
            settings.MenuAnimSavedFastMenus = menuAnimSavedFastMenus;
            settings.MenuAnimSavedFastText = menuAnimSavedFastText;
            settings.MenuAnimSavedAutoSkip = menuAnimSavedAutoSkip;
            settings.MenuAnimSavedAllowSkipping = menuAnimSavedAllowSkipping;
            settings.MenuAnimSavedSkipWithoutPrompt = menuAnimSavedSkipWithoutPrompt;

            settings.BossAnimEnabled = bossAnimMasterEnabled;
            settings.BossAnimHasSnapshot = bossAnimMasterHasSnapshot;
            settings.BossAnimSavedAbsoluteRadiance = bossAnimSavedAbsoluteRadiance;
            settings.BossAnimSavedPureVesselRoar = bossAnimSavedPureVesselRoar;
            settings.BossAnimSavedGrimmNightmare = bossAnimSavedGrimmNightmare;
            settings.BossAnimSavedGreyPrinceZote = bossAnimSavedGreyPrinceZote;
            settings.BossAnimSavedCollector = bossAnimSavedCollector;
            settings.BossAnimSavedSoulMaster = bossAnimSavedSoulMaster;

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

        private bool GetInfiniteChallengeEnabled()
        {
            Module? module = GetInfiniteChallengeModule();
            return module?.Enabled ?? false;
        }

        private void SetInfiniteChallengeEnabled(bool value)
        {
            Module? module = GetInfiniteChallengeModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetRandomPantheonsEnabled()
        {
            Module? module = GetRandomPantheonsModule();
            return module?.Enabled ?? false;
        }

        private void SetRandomPantheonsEnabled(bool value)
        {
            Module? module = GetRandomPantheonsModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateQuickMenuEntryStateColors();
        }

        private bool GetRandomPantheonsMasterEnabled() => randomPantheonsMasterEnabled;

        private void SetRandomPantheonsMasterEnabled(bool value)
        {
            if (randomPantheonsMasterEnabled == value)
            {
                return;
            }

            randomPantheonsMasterEnabled = value;
            if (!value)
            {
                CaptureRandomPantheonsSnapshot();
                SetRandomPantheonsAll(false);
            }
            else
            {
                if (!GetRandomPantheonsEnabled())
                {
                    SetRandomPantheonsEnabled(true);
                }
                RestoreRandomPantheonsSnapshot();
            }

            RefreshRandomPantheonsUi();
            UpdateQuickMenuEntryStateColors();
            SaveMasterSettings();
        }

        private void CaptureRandomPantheonsSnapshot()
        {
            randomPantheonsMasterHasSnapshot = true;
            randomPantheonsSavedP1 = GetRandomPantheonsP1Enabled();
            randomPantheonsSavedP2 = GetRandomPantheonsP2Enabled();
            randomPantheonsSavedP3 = GetRandomPantheonsP3Enabled();
            randomPantheonsSavedP4 = GetRandomPantheonsP4Enabled();
            randomPantheonsSavedP5 = GetRandomPantheonsP5Enabled();
        }

        private void RestoreRandomPantheonsSnapshot()
        {
            if (!randomPantheonsMasterHasSnapshot)
            {
                return;
            }

            SetRandomPantheonsP1Enabled(randomPantheonsSavedP1);
            SetRandomPantheonsP2Enabled(randomPantheonsSavedP2);
            SetRandomPantheonsP3Enabled(randomPantheonsSavedP3);
            SetRandomPantheonsP4Enabled(randomPantheonsSavedP4);
            SetRandomPantheonsP5Enabled(randomPantheonsSavedP5);
        }

        private void SetRandomPantheonsAll(bool value)
        {
            SetRandomPantheonsP1Enabled(value);
            SetRandomPantheonsP2Enabled(value);
            SetRandomPantheonsP3Enabled(value);
            SetRandomPantheonsP4Enabled(value);
            SetRandomPantheonsP5Enabled(value);
        }

        private bool GetRandomPantheonsP1Enabled() => Modules.BossChallenge.RandomPantheons.Pantheon1Enabled;

        private void SetRandomPantheonsP1Enabled(bool value)
        {
            Modules.BossChallenge.RandomPantheons.Pantheon1Enabled = value;
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(1);
            SaveMasterSettings();
        }

        private bool GetRandomPantheonsP2Enabled() => Modules.BossChallenge.RandomPantheons.Pantheon2Enabled;

        private void SetRandomPantheonsP2Enabled(bool value)
        {
            Modules.BossChallenge.RandomPantheons.Pantheon2Enabled = value;
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(2);
            SaveMasterSettings();
        }

        private bool GetRandomPantheonsP3Enabled() => Modules.BossChallenge.RandomPantheons.Pantheon3Enabled;

        private void SetRandomPantheonsP3Enabled(bool value)
        {
            Modules.BossChallenge.RandomPantheons.Pantheon3Enabled = value;
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(3);
            SaveMasterSettings();
        }

        private bool GetRandomPantheonsP4Enabled() => Modules.BossChallenge.RandomPantheons.Pantheon4Enabled;

        private void SetRandomPantheonsP4Enabled(bool value)
        {
            Modules.BossChallenge.RandomPantheons.Pantheon4Enabled = value;
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(4);
            SaveMasterSettings();
        }

        private bool GetRandomPantheonsP5Enabled() => Modules.BossChallenge.RandomPantheons.Pantheon5Enabled;

        private void SetRandomPantheonsP5Enabled(bool value)
        {
            Modules.BossChallenge.RandomPantheons.Pantheon5Enabled = value;
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(5);
            SaveMasterSettings();
        }

        private bool GetTrueBossRushEnabled()
        {
            Module? module = GetTrueBossRushModule();
            return module?.Enabled ?? false;
        }

        private void SetTrueBossRushEnabled(bool value)
        {
            Module? module = GetTrueBossRushModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateQuickMenuEntryStateColors();
        }

        private bool GetTrueBossRushMasterEnabled() => trueBossRushMasterEnabled;

        private void SetTrueBossRushMasterEnabled(bool value)
        {
            if (trueBossRushMasterEnabled == value)
            {
                return;
            }

            trueBossRushMasterEnabled = value;
            if (!value)
            {
                CaptureTrueBossRushSnapshot();
                SetTrueBossRushAll(false);
                SetTrueBossRushEnabled(false);
            }
            else
            {
                if (!GetTrueBossRushEnabled())
                {
                    SetTrueBossRushEnabled(true);
                }

                RestoreTrueBossRushSnapshot();
            }

            RefreshTrueBossRushUi();
            UpdateQuickMenuEntryStateColors();
            SaveMasterSettings();
        }

        private void CaptureTrueBossRushSnapshot()
        {
            trueBossRushMasterHasSnapshot = true;
            trueBossRushSavedP1 = GetTrueBossRushP1Enabled();
            trueBossRushSavedP2 = GetTrueBossRushP2Enabled();
            trueBossRushSavedP3 = GetTrueBossRushP3Enabled();
            trueBossRushSavedP4 = GetTrueBossRushP4Enabled();
            trueBossRushSavedP5 = GetTrueBossRushP5Enabled();
        }

        private void RestoreTrueBossRushSnapshot()
        {
            if (!trueBossRushMasterHasSnapshot)
            {
                return;
            }

            SetTrueBossRushP1Enabled(trueBossRushSavedP1);
            SetTrueBossRushP2Enabled(trueBossRushSavedP2);
            SetTrueBossRushP3Enabled(trueBossRushSavedP3);
            SetTrueBossRushP4Enabled(trueBossRushSavedP4);
            SetTrueBossRushP5Enabled(trueBossRushSavedP5);
        }

        private void SetTrueBossRushAll(bool value)
        {
            SetTrueBossRushP1Enabled(value);
            SetTrueBossRushP2Enabled(value);
            SetTrueBossRushP3Enabled(value);
            SetTrueBossRushP4Enabled(value);
            SetTrueBossRushP5Enabled(value);
        }

        private bool GetTrueBossRushP1Enabled() => Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon1Enabled;

        private void SetTrueBossRushP1Enabled(bool value)
        {
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon1Enabled = value;
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(1);
            SaveMasterSettings();
        }

        private bool GetTrueBossRushP2Enabled() => Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon2Enabled;

        private void SetTrueBossRushP2Enabled(bool value)
        {
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon2Enabled = value;
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(2);
            SaveMasterSettings();
        }

        private bool GetTrueBossRushP3Enabled() => Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon3Enabled;

        private void SetTrueBossRushP3Enabled(bool value)
        {
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon3Enabled = value;
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(3);
            SaveMasterSettings();
        }

        private bool GetTrueBossRushP4Enabled() => Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon4Enabled;

        private void SetTrueBossRushP4Enabled(bool value)
        {
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon4Enabled = value;
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(4);
            SaveMasterSettings();
        }

        private bool GetTrueBossRushP5Enabled() => Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon5Enabled;

        private void SetTrueBossRushP5Enabled(bool value)
        {
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon5Enabled = value;
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(5);
            SaveMasterSettings();
        }

        private bool GetCheatsEnabled()
        {
            Module? module = GetCheatsModule();
            return module?.Enabled ?? false;
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
            SaveMasterSettings();
        }

        private bool GetCheatsInfiniteHpEnabled() => Modules.Cheats.Cheats.GetInfiniteHpEnabled();

        private void SetCheatsInfiniteHpEnabled(bool value)
        {
            Modules.Cheats.Cheats.SetInfiniteHpEnabled(value);
            SaveMasterSettings();
        }

        private bool GetCheatsInvincibilityEnabled() => Modules.Cheats.Cheats.GetInvincibilityEnabled();

        private void SetCheatsInvincibilityEnabled(bool value)
        {
            Modules.Cheats.Cheats.SetInvincibilityEnabled(value);
            SaveMasterSettings();
        }

        private bool GetCheatsNoclipEnabled() => Modules.Cheats.Cheats.GetNoclipEnabled();

        private void SetCheatsNoclipEnabled(bool value)
        {
            Modules.Cheats.Cheats.SetNoclipEnabled(value);
            SaveMasterSettings();
        }

        private bool GetAlwaysFuriousEnabled()
        {
            Module? module = GetAlwaysFuriousModule();
            return module?.Enabled ?? false;
        }

        private void SetAlwaysFuriousEnabled(bool value)
        {
            Module? module = GetAlwaysFuriousModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateQuickMenuEntryStateColors();
        }

        private bool GetInfiniteGrimmPufferfishEnabled()
        {
            Module? module = GetInfiniteGrimmPufferfishModule();
            return module?.Enabled ?? false;
        }
    }
}
