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
            SetModuleEnabled<Modules.QoL.CollectorRoarMute>(false);
            SetModuleEnabled<Modules.CollectorPhases.CollectorPhases>(false);

            SetModuleEnabled<Modules.FastReload>(false);
            Modules.FastReload.reloadKeyCode = (int)KeyCode.None;

            Modules.QoL.DreamshieldStartAngle.ResetDefaults();

            ShowHPOnDeath.ResetFeatureDefaults();
            ApplyShowHpBinding(null);

            GodhomeQoL.GlobalSettings.MaskDamage ??= new MaskDamageSettings();
            GodhomeQoL.GlobalSettings.MaskDamage.Enabled = false;
            GodhomeQoL.GlobalSettings.MaskDamage.DamageMultiplier = 1f;
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
            SetModuleEnabled<Modules.BossChallenge.SegmentedP5>(false);
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
            ResetGruzMotherHelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.GruzMotherHelper>(false);
            ResetGruzMotherP1HelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.GruzMotherP1Helper>(false);
            ResetVengeflyKingP1HelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.VengeflyKingP1Helper>(false);
            ResetBroodingMawlekP1HelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.BroodingMawlekP1Helper>(false);
            ResetNoskP2HelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.NoskP2Helper>(false);
            ResetUumuuP3HelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.UumuuP3Helper>(false);
            ResetSoulWarriorP1HelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.SoulWarriorP1Helper>(false);
            ResetNoEyesP4HelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.NoEyesP4Helper>(false);
            ResetMarmuP2HelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.MarmuP2Helper>(false);
            ResetXeroP2HelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.XeroP2Helper>(false);
            ResetMarkothP4HelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.MarkothP4Helper>(false);
            ResetGorbP1HelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.GorbP1Helper>(false);
            ResetHornetProtectorHelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.HornetProtectorHelper>(false);
            ResetBroodingMawlekHelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.BroodingMawlekHelper>(false);
            ResetMassiveMossChargerHelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.MassiveMossChargerHelper>(false);
            ResetCrystalGuardianHelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.CrystalGuardianHelper>(false);
            ResetEnragedGuardianHelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.EnragedGuardianHelper>(false);
            ResetHornetSentinelHelperDefaults();
            SetModuleEnabled<Modules.BossChallenge.HornetSentinelHelper>(false);
            ResetAdditionalGhostHelperDefaultsGlobal();
            SetAdditionalGhostHelpersModulesEnabled(false);

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
            Modules.Cheats.Cheats.SetInfiniteSoulEnabled(false);
            Modules.Cheats.Cheats.SetInfiniteHpEnabled(false);
            Modules.Cheats.Cheats.SetInvincibilityEnabled(false);
            Modules.Cheats.Cheats.SetNoclipEnabled(false);
            Modules.Cheats.Cheats.SetKillAllHotkeyRaw(string.Empty);

            GodhomeQoL.GlobalSettings.QuickMenuMasters.BossChallengeEnabled = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.BossChallengeHasSnapshot = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.QolEnabled = true;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.QolHasSnapshot = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.MenuAnimEnabled = true;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.MenuAnimHasSnapshot = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.BossAnimEnabled = true;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.BossAnimHasSnapshot = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.BossAnimSavedHallOfGods = false;
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
            GodhomeQoL.GlobalSettings.QuickMenuMasters.BossManipulateGlobalP5Enabled = false;
            GodhomeQoL.GlobalSettings.QuickMenuMasters.BossManipulateGlobalP5TouchedModules ??= new List<string>();
            GodhomeQoL.GlobalSettings.QuickMenuMasters.BossManipulateGlobalP5TouchedModules.Clear();
            GodhomeQoL.GlobalSettings.QuickMenuMasters.BossManipulateGlobalP5EnabledModules ??= new List<string>();
            GodhomeQoL.GlobalSettings.QuickMenuMasters.BossManipulateGlobalP5EnabledModules.Clear();
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
            Modules.CollectorPhases.CollectorPhases.spawnBuzzer = true;
            Modules.CollectorPhases.CollectorPhases.spawnRoller = true;
            Modules.CollectorPhases.CollectorPhases.spawnSpitter = true;
            Modules.CollectorPhases.CollectorPhases.CollectorImmortal = false;
            Modules.CollectorPhases.CollectorPhases.IgnoreInitialJarLimit = false;
            Modules.CollectorPhases.CollectorPhases.DisableSummonLimit = false;
            Modules.CollectorPhases.CollectorPhases.CustomSummonLimit = 20;
        }

        private static void ResetZoteHelperDefaults()
        {
            Modules.BossChallenge.ZoteHelper.zoteBossHp = 1400;
            Modules.BossChallenge.ZoteHelper.zoteUseCustomBossHp = true;
            Modules.BossChallenge.ZoteHelper.zoteImmortal = false;
            Modules.BossChallenge.ZoteHelper.zoteSpawnFlying = true;
            Modules.BossChallenge.ZoteHelper.zoteSpawnHopping = true;
            Modules.BossChallenge.ZoteHelper.zoteSummonFlyingHp = 20;
            Modules.BossChallenge.ZoteHelper.zoteUseCustomFlyingHp = true;
            Modules.BossChallenge.ZoteHelper.zoteSummonHoppingHp = 20;
            Modules.BossChallenge.ZoteHelper.zoteUseCustomHoppingHp = true;
            Modules.BossChallenge.ZoteHelper.zoteSummonLimit = 3;
            Modules.BossChallenge.ZoteHelper.zoteUseCustomSummonLimit = false;
        }

        private static void ResetGruzMotherHelperDefaults()
        {
            Modules.BossChallenge.GruzMotherHelper.gruzUseMaxHp = false;
            Modules.BossChallenge.GruzMotherHelper.gruzP5Hp = false;
            Modules.BossChallenge.GruzMotherHelper.gruzMaxHp = 945;
            Modules.BossChallenge.GruzMotherHelper.gruzMaxHpBeforeP5 = 945;
            Modules.BossChallenge.GruzMotherHelper.gruzHasStoredMaxHpBeforeP5 = false;
        }

        private static void ResetHornetProtectorHelperDefaults()
        {
            Modules.BossChallenge.HornetProtectorHelper.hornetUseMaxHp = false;
            Modules.BossChallenge.HornetProtectorHelper.hornetP5Hp = false;
            Modules.BossChallenge.HornetProtectorHelper.hornetMaxHp = 1250;
            Modules.BossChallenge.HornetProtectorHelper.hornetMaxHpBeforeP5 = 1250;
            Modules.BossChallenge.HornetProtectorHelper.hornetUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.HornetProtectorHelper.hornetHasStoredStateBeforeP5 = false;
        }

        private static void ResetBroodingMawlekHelperDefaults()
        {
            Modules.BossChallenge.BroodingMawlekHelper.mawlekUseMaxHp = false;
            Modules.BossChallenge.BroodingMawlekHelper.mawlekP5Hp = false;
            Modules.BossChallenge.BroodingMawlekHelper.mawlekMaxHp = 1050;
            Modules.BossChallenge.BroodingMawlekHelper.mawlekMaxHpBeforeP5 = 1050;
            Modules.BossChallenge.BroodingMawlekHelper.mawlekUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.BroodingMawlekHelper.mawlekHasStoredStateBeforeP5 = false;
        }

        private static void ResetMassiveMossChargerHelperDefaults()
        {
            Modules.BossChallenge.MassiveMossChargerHelper.massiveMossUseMaxHp = false;
            Modules.BossChallenge.MassiveMossChargerHelper.massiveMossP5Hp = false;
            Modules.BossChallenge.MassiveMossChargerHelper.massiveMossMaxHp = 850;
            Modules.BossChallenge.MassiveMossChargerHelper.massiveMossMaxHpBeforeP5 = 850;
            Modules.BossChallenge.MassiveMossChargerHelper.massiveMossUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.MassiveMossChargerHelper.massiveMossHasStoredStateBeforeP5 = false;
        }

        private static void ResetCrystalGuardianHelperDefaults()
        {
            Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianUseMaxHp = false;
            Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianP5Hp = false;
            Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianMaxHp = 900;
            Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianMaxHpBeforeP5 = 900;
            Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianHasStoredStateBeforeP5 = false;
        }

        private static void ResetEnragedGuardianHelperDefaults()
        {
            Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianUseMaxHp = false;
            Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianP5Hp = false;
            Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianMaxHp = 1250;
            Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianMaxHpBeforeP5 = 1250;
            Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianHasStoredStateBeforeP5 = false;
        }

        private static void ResetHornetSentinelHelperDefaults()
        {
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseMaxHp = false;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelP5Hp = false;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelMaxHp = 1200;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseCustomPhase = false;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelPhase2Hp = 480;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelMaxHpBeforeP5 = 1200;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseCustomPhaseBeforeP5 = false;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelPhase2HpBeforeP5 = 480;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseMaxHpBeforeP5 = false;
            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelHasStoredStateBeforeP5 = false;
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

            GodhomeQoL.SaveGlobalSettingsSafe();
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
            Modules.QoL.DreamshieldStartAngle.SetEnabled(value);
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
            ShowHPOnDeath.SetFeatureEnabled(value);
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

        private static bool GetMaskDamageUiVisible()
        {
            return MaskDamage.GetUiVisible();
        }

        private void SetMaskDamageUiVisible(bool value)
        {
            MaskDamage.SetUiVisible(value);
            RefreshMaskDamageUi();
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
            if (Modules.BossChallenge.ZoteHelper.zoteUseCustomBossHp)
            {
                Modules.BossChallenge.ZoteHelper.ApplyBossHealthIfPresent();
            }
        }

        private void SetZoteUseCustomBossHpEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteUseCustomBossHp = value;
            Modules.BossChallenge.ZoteHelper.ReapplyLiveSettings();
            RefreshZoteHelperUi();
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
            if (Modules.BossChallenge.ZoteHelper.zoteUseCustomFlyingHp)
            {
                Modules.BossChallenge.ZoteHelper.ApplyZotelingHealthIfPresent();
            }
        }

        private void SetZoteUseCustomFlyingHpEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteUseCustomFlyingHp = value;
            Modules.BossChallenge.ZoteHelper.ApplyZotelingHealthIfPresent();
            RefreshZoteHelperUi();
        }

        private void SetZoteHoppingHp(int value)
        {
            Modules.BossChallenge.ZoteHelper.zoteSummonHoppingHp = Mathf.Clamp(value, 0, 99);
            if (Modules.BossChallenge.ZoteHelper.zoteUseCustomHoppingHp)
            {
                Modules.BossChallenge.ZoteHelper.ApplyZotelingHealthIfPresent();
            }
        }

        private void SetZoteUseCustomHoppingHpEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteUseCustomHoppingHp = value;
            Modules.BossChallenge.ZoteHelper.ApplyZotelingHealthIfPresent();
            RefreshZoteHelperUi();
        }

        private void SetZoteSummonLimit(int value)
        {
            Modules.BossChallenge.ZoteHelper.zoteSummonLimit = Mathf.Clamp(value, 0, 99);
            if (Modules.BossChallenge.ZoteHelper.zoteUseCustomSummonLimit)
            {
                Modules.BossChallenge.ZoteHelper.ReapplyLiveSettings();
            }
        }

        private void SetZoteUseCustomSummonLimitEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteUseCustomSummonLimit = value;
            Modules.BossChallenge.ZoteHelper.ReapplyLiveSettings();
            RefreshZoteHelperUi();
        }

        private bool GetGruzMotherHelperEnabled()
        {
            Module? module = GetGruzMotherHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetGruzMotherHelperEnabled(bool value)
        {
            Module? module = GetGruzMotherHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateGruzHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetGruzUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.GruzMotherHelper.gruzP5Hp)
            {
                Modules.BossChallenge.GruzMotherHelper.gruzUseMaxHp = true;
                RefreshGruzHelperUi();
                return;
            }

            Modules.BossChallenge.GruzMotherHelper.gruzUseMaxHp = value;
            Modules.BossChallenge.GruzMotherHelper.ReapplyLiveSettings();
            RefreshGruzHelperUi();
        }

        private void SetGruzP5HpEnabled(bool value)
        {
            Modules.BossChallenge.GruzMotherHelper.SetP5HpEnabled(value);
            RefreshGruzHelperUi();
        }

        private void SetGruzMaxHp(int value)
        {
            if (Modules.BossChallenge.GruzMotherHelper.gruzP5Hp)
            {
                Modules.BossChallenge.GruzMotherHelper.gruzMaxHp = 650;
                RefreshGruzHelperUi();
                return;
            }

            Modules.BossChallenge.GruzMotherHelper.gruzMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.GruzMotherHelper.gruzUseMaxHp)
            {
                Modules.BossChallenge.GruzMotherHelper.ApplyGruzHealthIfPresent();
            }
        }

        private bool GetHornetProtectorHelperEnabled()
        {
            Module? module = GetHornetProtectorHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetHornetProtectorHelperEnabled(bool value)
        {
            Module? module = GetHornetProtectorHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateHornetHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetHornetUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.HornetProtectorHelper.hornetP5Hp)
            {
                Modules.BossChallenge.HornetProtectorHelper.hornetUseMaxHp = true;
                RefreshHornetHelperUi();
                return;
            }

            Modules.BossChallenge.HornetProtectorHelper.hornetUseMaxHp = value;
            Modules.BossChallenge.HornetProtectorHelper.ReapplyLiveSettings();
            RefreshHornetHelperUi();
        }

        private void SetHornetP5HpEnabled(bool value)
        {
            Modules.BossChallenge.HornetProtectorHelper.SetP5HpEnabled(value);
            RefreshHornetHelperUi();
        }

        private void SetHornetMaxHp(int value)
        {
            if (Modules.BossChallenge.HornetProtectorHelper.hornetP5Hp)
            {
                Modules.BossChallenge.HornetProtectorHelper.hornetMaxHp = 900;
                RefreshHornetHelperUi();
                return;
            }

            Modules.BossChallenge.HornetProtectorHelper.hornetMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.HornetProtectorHelper.hornetUseMaxHp)
            {
                Modules.BossChallenge.HornetProtectorHelper.ApplyHornetHealthIfPresent();
            }
        }

        private bool GetBroodingMawlekHelperEnabled()
        {
            Module? module = GetBroodingMawlekHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetBroodingMawlekHelperEnabled(bool value)
        {
            Module? module = GetBroodingMawlekHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateMawlekHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetMawlekUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.BroodingMawlekHelper.mawlekP5Hp)
            {
                Modules.BossChallenge.BroodingMawlekHelper.mawlekUseMaxHp = true;
                RefreshMawlekHelperUi();
                return;
            }

            Modules.BossChallenge.BroodingMawlekHelper.mawlekUseMaxHp = value;
            Modules.BossChallenge.BroodingMawlekHelper.ReapplyLiveSettings();
            RefreshMawlekHelperUi();
        }

        private void SetMawlekP5HpEnabled(bool value)
        {
            Modules.BossChallenge.BroodingMawlekHelper.SetP5HpEnabled(value);
            RefreshMawlekHelperUi();
        }

        private void SetMawlekMaxHp(int value)
        {
            if (Modules.BossChallenge.BroodingMawlekHelper.mawlekP5Hp)
            {
                Modules.BossChallenge.BroodingMawlekHelper.mawlekMaxHp = 750;
                RefreshMawlekHelperUi();
                return;
            }

            Modules.BossChallenge.BroodingMawlekHelper.mawlekMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.BroodingMawlekHelper.mawlekUseMaxHp)
            {
                Modules.BossChallenge.BroodingMawlekHelper.ApplyMawlekHealthIfPresent();
            }
        }

        private bool GetMassiveMossChargerHelperEnabled()
        {
            Module? module = GetMassiveMossChargerHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetMassiveMossChargerHelperEnabled(bool value)
        {
            Module? module = GetMassiveMossChargerHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateMassiveMossHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetMassiveMossUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.MassiveMossChargerHelper.massiveMossP5Hp)
            {
                Modules.BossChallenge.MassiveMossChargerHelper.massiveMossUseMaxHp = true;
                RefreshMassiveMossHelperUi();
                return;
            }

            Modules.BossChallenge.MassiveMossChargerHelper.massiveMossUseMaxHp = value;
            Modules.BossChallenge.MassiveMossChargerHelper.ReapplyLiveSettings();
            RefreshMassiveMossHelperUi();
        }

        private void SetMassiveMossP5HpEnabled(bool value)
        {
            Modules.BossChallenge.MassiveMossChargerHelper.SetP5HpEnabled(value);
            RefreshMassiveMossHelperUi();
        }

        private void SetMassiveMossMaxHp(int value)
        {
            if (Modules.BossChallenge.MassiveMossChargerHelper.massiveMossP5Hp)
            {
                Modules.BossChallenge.MassiveMossChargerHelper.massiveMossMaxHp = 480;
                RefreshMassiveMossHelperUi();
                return;
            }

            Modules.BossChallenge.MassiveMossChargerHelper.massiveMossMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.MassiveMossChargerHelper.massiveMossUseMaxHp)
            {
                Modules.BossChallenge.MassiveMossChargerHelper.ApplyMassiveMossHealthIfPresent();
            }
        }

        private bool GetCrystalGuardianHelperEnabled()
        {
            Module? module = GetCrystalGuardianHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetCrystalGuardianHelperEnabled(bool value)
        {
            Module? module = GetCrystalGuardianHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateCrystalGuardianHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetCrystalGuardianUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianP5Hp)
            {
                Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianUseMaxHp = true;
                RefreshCrystalGuardianHelperUi();
                return;
            }

            Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianUseMaxHp = value;
            Modules.BossChallenge.CrystalGuardianHelper.ReapplyLiveSettings();
            RefreshCrystalGuardianHelperUi();
        }

        private void SetCrystalGuardianP5HpEnabled(bool value)
        {
            Modules.BossChallenge.CrystalGuardianHelper.SetP5HpEnabled(value);
            RefreshCrystalGuardianHelperUi();
        }

        private void SetCrystalGuardianMaxHp(int value)
        {
            if (Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianP5Hp)
            {
                Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianMaxHp = 650;
                RefreshCrystalGuardianHelperUi();
                return;
            }

            Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianUseMaxHp)
            {
                Modules.BossChallenge.CrystalGuardianHelper.ApplyCrystalGuardianHealthIfPresent();
            }
        }

        private bool GetEnragedGuardianHelperEnabled()
        {
            Module? module = GetEnragedGuardianHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetEnragedGuardianHelperEnabled(bool value)
        {
            Module? module = GetEnragedGuardianHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateEnragedGuardianHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetEnragedGuardianUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianP5Hp)
            {
                Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianUseMaxHp = true;
                RefreshEnragedGuardianHelperUi();
                return;
            }

            Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianUseMaxHp = value;
            Modules.BossChallenge.EnragedGuardianHelper.ReapplyLiveSettings();
            RefreshEnragedGuardianHelperUi();
        }

        private void SetEnragedGuardianP5HpEnabled(bool value)
        {
            Modules.BossChallenge.EnragedGuardianHelper.SetP5HpEnabled(value);
            RefreshEnragedGuardianHelperUi();
        }

        private void SetEnragedGuardianMaxHp(int value)
        {
            if (Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianP5Hp)
            {
                Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianMaxHp = 650;
                RefreshEnragedGuardianHelperUi();
                return;
            }

            Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianUseMaxHp)
            {
                Modules.BossChallenge.EnragedGuardianHelper.ApplyEnragedGuardianHealthIfPresent();
            }
        }

        private bool GetHornetSentinelHelperEnabled()
        {
            Module? module = GetHornetSentinelHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetHornetSentinelHelperEnabled(bool value)
        {
            Module? module = GetHornetSentinelHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateHornetSentinelHelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetHornetSentinelUseMaxHpEnabled(bool value)
        {
            if (Modules.BossChallenge.HornetSentinelHelper.hornetSentinelP5Hp)
            {
                Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseMaxHp = true;
                RefreshHornetSentinelHelperUi();
                return;
            }

            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseMaxHp = value;
            Modules.BossChallenge.HornetSentinelHelper.ReapplyLiveSettings();
            RefreshHornetSentinelHelperUi();
        }

        private void SetHornetSentinelUseCustomPhaseEnabled(bool value)
        {
            if (Modules.BossChallenge.HornetSentinelHelper.hornetSentinelP5Hp)
            {
                Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseCustomPhase = false;
                RefreshHornetSentinelHelperUi();
                return;
            }

            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseCustomPhase = value;
            Modules.BossChallenge.HornetSentinelHelper.ReapplyLiveSettings();
            RefreshHornetSentinelHelperUi();
        }

        private void SetHornetSentinelP5HpEnabled(bool value)
        {
            Modules.BossChallenge.HornetSentinelHelper.SetP5HpEnabled(value);
            RefreshHornetSentinelHelperUi();
        }

        private void SetHornetSentinelMaxHp(int value)
        {
            if (Modules.BossChallenge.HornetSentinelHelper.hornetSentinelP5Hp)
            {
                Modules.BossChallenge.HornetSentinelHelper.hornetSentinelMaxHp = 850;
                RefreshHornetSentinelHelperUi();
                return;
            }

            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelMaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseMaxHp)
            {
                Modules.BossChallenge.HornetSentinelHelper.ApplyHornetSentinelHealthIfPresent();
            }
        }

        private void SetHornetSentinelPhase2Hp(int value)
        {
            if (Modules.BossChallenge.HornetSentinelHelper.hornetSentinelP5Hp)
            {
                RefreshHornetSentinelHelperUi();
                return;
            }

            Modules.BossChallenge.HornetSentinelHelper.hornetSentinelPhase2Hp = Mathf.Clamp(value, 1, 1200);
            if (Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseCustomPhase)
            {
                Modules.BossChallenge.HornetSentinelHelper.ApplyPhaseThresholdSettingsIfPresent();
            }
        }

        private static void ReapplyCollectorPhasesLiveSettings()
        {
            Modules.CollectorPhases.CollectorPhases.ReapplyLiveSettings();
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

            ReapplyCollectorPhasesLiveSettings();
        }

        private void SetCollectorImmortalEnabled(bool value)
        {
            Modules.CollectorPhases.CollectorPhases.CollectorImmortal = value;
            ReapplyCollectorPhasesLiveSettings();
        }

        private void SetCollectorIgnoreInitialJarLimitEnabled(bool value)
        {
            Modules.CollectorPhases.CollectorPhases.IgnoreInitialJarLimit = value;
            ReapplyCollectorPhasesLiveSettings();
        }

        private void SetCollectorUseCustomPhase2ThresholdEnabled(bool value)
        {
            Modules.CollectorPhases.CollectorPhases.UseCustomPhase2Threshold = value;
            ReapplyCollectorPhasesLiveSettings();
        }

        private void SetCollectorCustomPhase2Threshold(int value)
        {
            Modules.CollectorPhases.CollectorPhases.CustomPhase2Threshold = Mathf.Clamp(value, 1, 99999);
            ReapplyCollectorPhasesLiveSettings();
        }

        private void SetCollectorUseMaxHpEnabled(bool value)
        {
            Modules.CollectorPhases.CollectorPhases.UseMaxHP = value;
            ReapplyCollectorPhasesLiveSettings();
        }

        private void SetCollectorMaxHp(int value)
        {
            Modules.CollectorPhases.CollectorPhases.collectorMaxHP = Math.Max(value, 100);
            ReapplyCollectorPhasesLiveSettings();
        }

        private void SetCollectorBuzzerHp(int value)
        {
            Modules.CollectorPhases.CollectorPhases.buzzerHP = Math.Max(value, 1);
            ReapplyCollectorPhasesLiveSettings();
        }

        private void SetCollectorSpawnBuzzerEnabled(bool value)
        {
            Modules.CollectorPhases.CollectorPhases.spawnBuzzer = value;
            ReapplyCollectorPhasesLiveSettings();
        }

        private void SetCollectorRollerHp(int value)
        {
            Modules.CollectorPhases.CollectorPhases.rollerHP = Math.Max(value, 1);
            ReapplyCollectorPhasesLiveSettings();
        }

        private void SetCollectorSpawnRollerEnabled(bool value)
        {
            Modules.CollectorPhases.CollectorPhases.spawnRoller = value;
            ReapplyCollectorPhasesLiveSettings();
        }

        private void SetCollectorSpitterHp(int value)
        {
            Modules.CollectorPhases.CollectorPhases.spitterHP = Math.Max(value, 1);
            ReapplyCollectorPhasesLiveSettings();
        }

        private void SetCollectorSpawnSpitterEnabled(bool value)
        {
            Modules.CollectorPhases.CollectorPhases.spawnSpitter = value;
            ReapplyCollectorPhasesLiveSettings();
        }

        private void SetCollectorDisableSummonLimitEnabled(bool value)
        {
            Modules.CollectorPhases.CollectorPhases.DisableSummonLimit = value;
            ReapplyCollectorPhasesLiveSettings();
        }

        private void SetCollectorCustomSummonLimit(int value)
        {
            Modules.CollectorPhases.CollectorPhases.CustomSummonLimit = Mathf.Clamp(value, 2, 999);
            ReapplyCollectorPhasesLiveSettings();
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

            Modules.QoL.SkipCutscenes.HallOfGodsStatues = bossAnimSavedHallOfGods;
            Modules.QoL.SkipCutscenes.AbsoluteRadiance = bossAnimSavedAbsoluteRadiance;
            Modules.QoL.SkipCutscenes.PureVesselRoar = bossAnimSavedPureVesselRoar;
            Modules.QoL.SkipCutscenes.GrimmNightmare = bossAnimSavedGrimmNightmare;
            Modules.QoL.SkipCutscenes.GreyPrinceZote = bossAnimSavedGreyPrinceZote;
            Modules.QoL.SkipCutscenes.Collector = bossAnimSavedCollector;
            Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip = bossAnimSavedSoulMaster;
        }

        private void SetBossAnimationAll(bool value)
        {
            Modules.QoL.SkipCutscenes.HallOfGodsStatues = value;
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
            settings.MenuAnimSavedMemorizeBindings = menuAnimSavedMemorizeBindings;
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

            if (value)
            {
                bool hadTrueBossRush =
                    GetTrueBossRushMasterEnabled()
                    || Modules.BossChallenge.TrueBossRush.AnyPantheonEnabled;
                bool hadSegmentedP5 = GetSegmentedP5Enabled();

                if (hadTrueBossRush)
                {
                    SetTrueBossRushMasterEnabled(false);
                    _ = Modules.BossChallenge.PantheonSequenceCompatibility.DisableTrueBossRush();
                }

                if (hadSegmentedP5)
                {
                    SetSegmentedP5Enabled(false);
                }

                if (hadTrueBossRush || hadSegmentedP5)
                {
                    ShowStatusMessage("Random Pantheons disabled True Boss Rush / Segmented P5.");
                }
            }

            randomPantheonsMasterEnabled = value;
            if (!value)
            {
                CaptureRandomPantheonsSnapshot();
                SetRandomPantheonsAll(false);
                SetRandomPantheonsEnabled(false);
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

            if (value)
            {
                bool hadRandomPantheons =
                    GetRandomPantheonsMasterEnabled()
                    || GetRandomPantheonsEnabled()
                    || Modules.BossChallenge.RandomPantheons.AnyPantheonEnabled;
                bool hadSegmentedP5 = GetSegmentedP5Enabled();

                if (hadRandomPantheons)
                {
                    SetRandomPantheonsMasterEnabled(false);
                    SetRandomPantheonsEnabled(false);
                    _ = Modules.BossChallenge.PantheonSequenceCompatibility.DisableRandomPantheons();
                }

                if (hadSegmentedP5)
                {
                    SetSegmentedP5Enabled(false);
                }

                if (hadRandomPantheons || hadSegmentedP5)
                {
                    ShowStatusMessage("True Boss Rush disabled Random Pantheons / Segmented P5.");
                }
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
            RefreshAlwaysFuriousUi();
            RefreshGearSwitcherUi();
        }

        private bool GetInfiniteGrimmPufferfishEnabled()
        {
            Module? module = GetInfiniteGrimmPufferfishModule();
            return module?.Enabled ?? false;
        }
    }
}
