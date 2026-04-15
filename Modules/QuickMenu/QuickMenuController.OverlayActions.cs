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
        private static readonly Type[] BossManipulateGlobalP5ModuleTypes = new[]
        {
            typeof(Modules.BossChallenge.GruzMotherHelper),
            typeof(Modules.BossChallenge.BroodingMawlekHelper),
            typeof(Modules.BossChallenge.HornetProtectorHelper),
            typeof(Modules.BossChallenge.HornetSentinelHelper),
            typeof(Modules.BossChallenge.MassiveMossChargerHelper),
            typeof(Modules.BossChallenge.CrystalGuardianHelper),
            typeof(Modules.BossChallenge.EnragedGuardianHelper),
            typeof(Modules.BossChallenge.MarmuHelper),
            typeof(Modules.BossChallenge.XeroHelper),
            typeof(Modules.BossChallenge.MarkothHelper),
            typeof(Modules.BossChallenge.GalienHelper),
            typeof(Modules.BossChallenge.GorbHelper),
            typeof(Modules.BossChallenge.ElderHuHelper),
            typeof(Modules.BossChallenge.NoEyesHelper),
            typeof(Modules.BossChallenge.DungDefenderHelper),
            typeof(Modules.BossChallenge.WhiteDefenderHelper),
            typeof(Modules.BossChallenge.HiveKnightHelper),
            typeof(Modules.BossChallenge.BrokenVesselHelper),
            typeof(Modules.BossChallenge.LostKinHelper),
            typeof(Modules.BossChallenge.WingedNoskHelper),
            typeof(Modules.BossChallenge.UumuuHelper),
            typeof(Modules.BossChallenge.TraitorLordHelper),
            typeof(Modules.BossChallenge.TroupeMasterGrimmHelper),
            typeof(Modules.BossChallenge.NightmareKingGrimmHelper),
            typeof(Modules.BossChallenge.PureVesselHelper),
            typeof(Modules.BossChallenge.AbsoluteRadianceHelper),
            typeof(Modules.BossChallenge.PaintmasterSheoHelper),
            typeof(Modules.BossChallenge.SoulWarriorHelper),
            typeof(Modules.BossChallenge.NailsageSlyHelper),
            typeof(Modules.BossChallenge.SoulMasterHelper),
            typeof(Modules.BossChallenge.SoulTyrantHelper),
            typeof(Modules.BossChallenge.WatcherKnightHelper),
            typeof(Modules.BossChallenge.OroMatoHelper),
            typeof(Modules.BossChallenge.GodTamerHelper),
            typeof(Modules.BossChallenge.OblobblesHelper),
            typeof(Modules.BossChallenge.FalseKnightHelper),
            typeof(Modules.BossChallenge.FailedChampionHelper),
            typeof(Modules.BossChallenge.SisterOfBattleHelper),
            typeof(Modules.BossChallenge.FlukemarmHelper),
            typeof(Modules.BossChallenge.VengeflyKing),
        };

        private void OnOverlayBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetOverlayVisible(false);
        }

        private void OnCollectorBackClicked()
        {
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetCollectorVisible(false);
        }

        private void OnBossManipulateBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;
            returnToBossManipulateOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetBossManipulateVisible(false);
        }

        private bool GetBossManipulateGlobalP5Enabled()
        {
            QuickMenuMasterSettings settings = GodhomeQoL.GlobalSettings.QuickMenuMasters ??= new QuickMenuMasterSettings();
            return settings.BossManipulateGlobalP5Enabled;
        }

        private void SetBossManipulateGlobalP5Enabled(bool value)
        {
            QuickMenuMasterSettings settings = GodhomeQoL.GlobalSettings.QuickMenuMasters ??= new QuickMenuMasterSettings();
            List<string> touchedP5Modules = settings.BossManipulateGlobalP5TouchedModules ??= new List<string>();
            List<string> enabledModules = settings.BossManipulateGlobalP5EnabledModules ??= new List<string>();

            if (value)
            {
                touchedP5Modules.Clear();
                enabledModules.Clear();
                foreach (Type moduleType in BossManipulateGlobalP5ModuleTypes)
                {
                    string key = GetBossManipulateModuleKey(moduleType);

                    if (TryGetModuleEnabled(moduleType, out bool isModuleEnabled) && !isModuleEnabled && TrySetModuleEnabled(moduleType, true))
                    {
                        if (!enabledModules.Contains(key))
                        {
                            enabledModules.Add(key);
                        }
                    }

                    if (!TryGetModuleP5HpEnabled(moduleType, out bool currentValue) || currentValue)
                    {
                        continue;
                    }

                    if (!TrySetModuleP5HpEnabled(moduleType, true))
                    {
                        continue;
                    }

                    if (!touchedP5Modules.Contains(key))
                    {
                        touchedP5Modules.Add(key);
                    }
                }

                settings.BossManipulateGlobalP5Enabled = true;
            }
            else
            {
                HashSet<string> touchedP5Set = new(touchedP5Modules);
                HashSet<string> enabledSet = new(enabledModules);
                foreach (Type moduleType in BossManipulateGlobalP5ModuleTypes)
                {
                    string key = GetBossManipulateModuleKey(moduleType);
                    if (touchedP5Set.Contains(key))
                    {
                        _ = TrySetModuleP5HpEnabled(moduleType, false);
                    }

                    if (enabledSet.Contains(key))
                    {
                        _ = TrySetModuleEnabled(moduleType, false);
                    }
                }

                touchedP5Modules.Clear();
                enabledModules.Clear();
                settings.BossManipulateGlobalP5Enabled = false;
            }

            GodhomeQoL.SaveGlobalSettingsSafe();
            RefreshBossManipulateGlobalUi();
            RefreshBossManipulateCardVisuals();
            UpdateQuickMenuEntryStateColors();
        }

        private void OnBossManipulateResetAllClicked()
        {
            SetBossManipulateResetConfirmVisible(true);
        }

        private void OnBossManipulateResetConfirmYes()
        {
            SetBossManipulateResetConfirmVisible(false);

            OnCollectorResetClicked();
            OnZoteHelperResetDefaultsClicked();
            OnGruzHelperResetDefaultsClicked();
            OnGruzMotherP1HelperResetDefaultsClicked();
            OnVengeflyKingP1HelperResetDefaultsClicked();
            OnBroodingMawlekP1HelperResetDefaultsClicked();
            OnNoskP2HelperResetDefaultsClicked();
            OnUumuuP3HelperResetDefaultsClicked();
            OnSoulWarriorP1HelperResetDefaultsClicked();
            OnNoEyesP4HelperResetDefaultsClicked();
            OnMarmuP2HelperResetDefaultsClicked();
            OnXeroP2HelperResetDefaultsClicked();
            OnMarkothP4HelperResetDefaultsClicked();
            OnGorbP1HelperResetDefaultsClicked();
            OnHornetHelperResetDefaultsClicked();
            OnMawlekHelperResetDefaultsClicked();
            OnMassiveMossHelperResetDefaultsClicked();
            OnCrystalGuardianHelperResetDefaultsClicked();
            OnEnragedGuardianHelperResetDefaultsClicked();
            OnHornetSentinelHelperResetDefaultsClicked();

            OnMarmuHelperResetDefaultsClicked();
            OnXeroHelperResetDefaultsClicked();
            OnMarkothHelperResetDefaultsClicked();
            OnGalienHelperResetDefaultsClicked();
            OnGorbHelperResetDefaultsClicked();
            OnElderHuHelperResetDefaultsClicked();
            OnNoEyesHelperResetDefaultsClicked();
            OnDungDefenderHelperResetDefaultsClicked();
            OnWhiteDefenderHelperResetDefaultsClicked();
            OnHiveKnightHelperResetDefaultsClicked();
            OnBrokenVesselHelperResetDefaultsClicked();
            OnLostKinHelperResetDefaultsClicked();
            OnNoskHelperResetDefaultsClicked();
            OnWingedNoskHelperResetDefaultsClicked();
            OnUumuuHelperResetDefaultsClicked();
            OnTraitorLordHelperResetDefaultsClicked();
            OnTroupeMasterGrimmHelperResetDefaultsClicked();
            OnNightmareKingGrimmHelperResetDefaultsClicked();
            OnPureVesselHelperResetDefaultsClicked();
            OnAbsoluteRadianceHelperResetDefaultsClicked();
            OnPaintmasterSheoHelperResetDefaultsClicked();
            OnSoulWarriorHelperResetDefaultsClicked();
            OnNailsageSlyHelperResetDefaultsClicked();
            OnSoulMasterHelperResetDefaultsClicked();
            OnSoulTyrantHelperResetDefaultsClicked();
            OnWatcherKnightHelperResetDefaultsClicked();
            OnOroMatoHelperResetDefaultsClicked();
            OnGodTamerHelperResetDefaultsClicked();
            OnOblobblesHelperResetDefaultsClicked();
            OnFalseKnightHelperResetDefaultsClicked();
            OnFailedChampionHelperResetDefaultsClicked();
            OnFlukemarmHelperResetDefaultsClicked();
            OnVengeflyKingResetDefaultsClicked();
            OnSisterOfBattleHelperResetDefaultsClicked();
            OnMantisLordHelperResetDefaultsClicked();

            QuickMenuMasterSettings settings = GodhomeQoL.GlobalSettings.QuickMenuMasters ??= new QuickMenuMasterSettings();
            settings.BossManipulateGlobalP5Enabled = false;
            settings.BossManipulateGlobalP5TouchedModules ??= new List<string>();
            settings.BossManipulateGlobalP5TouchedModules.Clear();
            settings.BossManipulateGlobalP5EnabledModules ??= new List<string>();
            settings.BossManipulateGlobalP5EnabledModules.Clear();

            GodhomeQoL.SaveGlobalSettingsSafe();
            RefreshBossManipulateGlobalUi();
            RefreshBossManipulateCardVisuals();
            UpdateQuickMenuEntryStateColors();
        }

        private void OnBossManipulateResetConfirmNo()
        {
            SetBossManipulateResetConfirmVisible(false);
        }

        private static string GetBossManipulateModuleKey(Type moduleType) =>
            moduleType.FullName ?? moduleType.Name;

        private static bool TryGetModuleEnabled(Type moduleType, out bool value)
        {
            value = false;
            if (!ModuleManager.TryGetModule(moduleType, out Module? module) || module == null)
            {
                return false;
            }

            value = module.Enabled;
            return true;
        }

        private static bool TrySetModuleEnabled(Type moduleType, bool value)
        {
            if (!ModuleManager.TryGetModule(moduleType, out Module? module) || module == null)
            {
                return false;
            }

            module.Enabled = value;
            return true;
        }

        private static bool TryGetModuleP5HpEnabled(Type moduleType, out bool value)
        {
            value = false;
            FieldInfo? field = moduleType
                .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(candidate =>
                    candidate.FieldType == typeof(bool)
                    && candidate.Name.EndsWith("P5Hp", StringComparison.Ordinal));
            if (field == null)
            {
                return false;
            }

            object? raw = field.GetValue(null);
            if (raw is bool typed)
            {
                value = typed;
                return true;
            }

            return false;
        }

        private static bool TrySetModuleP5HpEnabled(Type moduleType, bool value)
        {
            MethodInfo? method = moduleType.GetMethod(
                "SetP5HpEnabled",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == null)
            {
                return false;
            }

            try
            {
                method.Invoke(null, new object[] { value });
                return true;
            }
            catch (Exception exception)
            {
                LogDebug($"QuickMenu: failed to set global P5 for {moduleType.Name}: {exception.Message}");
                return false;
            }
        }

        private void OnBossManipulateCollectorClicked()
        {
            returnToBossManipulateOnClose = true;
            SetBossManipulateVisible(false);
            SetCollectorVisible(true);
        }

        private void OnBossManipulateZoteClicked()
        {
            returnToBossManipulateOnClose = true;
            SetBossManipulateVisible(false);
            SetZoteHelperVisible(true);
        }

        private void OnBossManipulateGruzClicked()
        {
            returnToBossManipulateOnClose = true;
            SetBossManipulateVisible(false);
            SetGruzHelperVisible(true);
        }

        private void OnBossManipulateHornetClicked()
        {
            returnToBossManipulateOnClose = true;
            SetBossManipulateVisible(false);
            SetHornetHelperVisible(true);
        }

        private void OnBossManipulateMawlekClicked()
        {
            returnToBossManipulateOnClose = true;
            SetBossManipulateVisible(false);
            SetMawlekHelperVisible(true);
        }

        private void OnBossManipulateMassiveMossClicked()
        {
            returnToBossManipulateOnClose = true;
            SetBossManipulateVisible(false);
            SetMassiveMossHelperVisible(true);
        }

        private void OnBossManipulateCrystalGuardianClicked()
        {
            returnToBossManipulateOnClose = true;
            SetBossManipulateVisible(false);
            SetCrystalGuardianHelperVisible(true);
        }

        private void OnBossManipulateEnragedGuardianClicked()
        {
            returnToBossManipulateOnClose = true;
            SetBossManipulateVisible(false);
            SetEnragedGuardianHelperVisible(true);
        }

        private void OnBossManipulateHornetSentinelClicked()
        {
            returnToBossManipulateOnClose = true;
            SetBossManipulateVisible(false);
            SetHornetSentinelHelperVisible(true);
        }

        private void OnFastReloadBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetFastReloadVisible(false);
        }

        private void OnFastReloadResetDefaultsClicked()
        {
            SetFastReloadEnabled(false);
            waitingForReloadRebind = false;
            Modules.FastReload.reloadKeyCode = (int)KeyCode.None;
            RefreshFastReloadUi();
        }

        private void OnDreamshieldBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetDreamshieldVisible(false);
        }

        private void OnDreamshieldResetDefaultsClicked()
        {
            Modules.QoL.DreamshieldStartAngle.ResetDefaults();
            RefreshDreamshieldUi();
            UpdateQuickMenuEntryStateColors();
        }

        private void OnShowHpOnDeathBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetShowHpOnDeathVisible(false);
        }

        private void OnMaskDamageBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
        }

        private void OnMaskDamageResetDefaultsClicked()
        {
            waitingForMaskDamageUiRebind = false;
            maskDamageUiPrevKey = string.Empty;

            SetMaskDamageEnabled(false);
            MaskDamage.SetMultiplier(1f);
            MaskDamage.SetUiVisible(true);
            MaskDamage.SetToggleUiKeybind(string.Empty);
            RefreshMaskDamageUi();
            UpdateQuickMenuEntryStateColors();
        }

        private void OnFreezeHitboxesBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetFreezeHitboxesVisible(false);
        }

        private void OnFreezeHitboxesResetDefaultsClicked()
        {
            waitingForFreezeHitboxesRebind = false;
            freezeHitboxesPrevKey = string.Empty;

            SetFreezeHitboxesEnabled(false);
            FreezeHitboxes.SetAnyHitsMode(false);
            FreezeHitboxes.SetUnfreezeKeybind(string.Empty);
            RefreshFreezeHitboxesUi();
            UpdateQuickMenuEntryStateColors();
        }

        private void OnShowHpOnDeathResetDefaultsClicked()
        {
            waitingForShowHpRebind = false;
            showHpPrevBindingRaw = string.Empty;
            ShowHPOnDeath.ResetFeatureDefaults();
            ApplyShowHpBinding(null);
            RefreshShowHpOnDeathUi();
            UpdateQuickMenuEntryStateColors();
        }

        private void OnSpeedChangerBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetSpeedChangerVisible(false);
        }

        private void OnSpeedChangerResetDefaultsClicked()
        {
            waitingForSpeedToggleRebind = false;
            waitingForSpeedInputRebind = false;
            speedTogglePrevKey = string.Empty;
            speedInputPrevKey = string.Empty;

            SetSpeedChangerGlobalSwitch(false);
            SpeedChanger.restrictToggleToRooms = false;
            SpeedChanger.unlimitedSpeed = false;
            SpeedChanger.displayStyle = 0;
            SpeedChanger.toggleKeybind = string.Empty;
            SpeedChanger.inputSpeedKeybind = string.Empty;
            SpeedChanger.speed = 1f;
            ApplySpeedChangerDisplayStyle(0);
            RefreshSpeedChangerKeybinds();
            RefreshSpeedChangerUi();
        }

        private void OnTeleportKitBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetTeleportKitVisible(false);
        }

        private void OnTeleportKitResetDefaultsClicked()
        {
            waitingForTeleportKitMenuRebind = false;
            waitingForTeleportKitSaveRebind = false;
            waitingForTeleportKitTeleportRebind = false;
            SetTeleportKitEnabled(false);
            Modules.QoL.TeleportKit.MenuHotkey = KeyCode.F6;
            Modules.QoL.TeleportKit.SaveTeleportHotkey = KeyCode.R;
            Modules.QoL.TeleportKit.TeleportHotkey = KeyCode.T;
            RefreshTeleportKitUi();
        }

        private void OnBossChallengeBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetBossChallengeVisible(false);
        }

        private void OnRandomPantheonsBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetRandomPantheonsVisible(false);
        }

        private void OnRandomPantheonsResetDefaultsClicked()
        {
            randomPantheonsMasterEnabled = false;
            randomPantheonsMasterHasSnapshot = false;
            randomPantheonsSavedP1 = false;
            randomPantheonsSavedP2 = false;
            randomPantheonsSavedP3 = false;
            randomPantheonsSavedP4 = false;
            randomPantheonsSavedP5 = false;

            Modules.BossChallenge.RandomPantheons.Pantheon1Enabled = false;
            Modules.BossChallenge.RandomPantheons.Pantheon2Enabled = false;
            Modules.BossChallenge.RandomPantheons.Pantheon3Enabled = false;
            Modules.BossChallenge.RandomPantheons.Pantheon4Enabled = false;
            Modules.BossChallenge.RandomPantheons.Pantheon5Enabled = false;
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(1);
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(2);
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(3);
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(4);
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(5);
            SetRandomPantheonsEnabled(false);
            RefreshRandomPantheonsUi();
            SaveMasterSettings();
        }

        private void OnTrueBossRushBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetTrueBossRushVisible(false);
        }

        private void OnTrueBossRushResetDefaultsClicked()
        {
            trueBossRushMasterEnabled = false;
            trueBossRushMasterHasSnapshot = false;
            trueBossRushSavedP1 = false;
            trueBossRushSavedP2 = false;
            trueBossRushSavedP3 = false;
            trueBossRushSavedP4 = false;
            trueBossRushSavedP5 = false;

            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon1Enabled = false;
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon2Enabled = false;
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon3Enabled = false;
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon4Enabled = false;
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon5Enabled = false;
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(1);
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(2);
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(3);
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(4);
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(5);
            SetTrueBossRushEnabled(false);
            RefreshTrueBossRushUi();
            SaveMasterSettings();
        }

        private void OnCheatsBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetCheatsVisible(false);
        }

        private void OnCheatsKillAllClicked()
        {
            _ = Modules.Cheats.Cheats.KillAll();
        }

        private void OnCheatsResetDefaultsClicked()
        {
            cheatsMasterEnabled = false;
            cheatsMasterHasSnapshot = false;
            cheatsSavedInfiniteSoul = false;
            cheatsSavedInfiniteHp = false;
            cheatsSavedInvincibility = false;
            cheatsSavedNoclip = false;
            waitingForCheatsKillAllRebind = false;
            cheatsKillAllPrevKey = string.Empty;

            Modules.Cheats.Cheats.SetInfiniteSoulEnabled(false);
            Modules.Cheats.Cheats.SetInfiniteHpEnabled(false);
            Modules.Cheats.Cheats.SetInvincibilityEnabled(false);
            Modules.Cheats.Cheats.SetNoclipEnabled(false);
            Modules.Cheats.Cheats.SetKillAllHotkeyRaw(string.Empty);
            SetCheatsEnabled(false);
            RefreshCheatsUi();
            SaveMasterSettings();
        }

        private void OnBossChallengeResetDefaultsClicked()
        {
            bossChallengeMasterEnabled = false;
            bossChallengeMasterHasSnapshot = false;
            SetInfiniteChallengeEnabled(false);
            SetCarefreeMelodyMode(Modules.QoL.CarefreeMelodyReset.ModeOff);
            SetForceArriveAnimationEnabled(false);
            SetInfiniteGrimmPufferfishEnabled(false);
            SetInfiniteRadianceClimbingEnabled(false);
            SetSegmentedP5Enabled(false);
            SetAddLifebloodEnabled(false);
            SetAddSoulEnabled(false);

            Modules.BossChallenge.InfiniteChallenge.restartFightOnSuccess = false;
            Modules.BossChallenge.InfiniteChallenge.restartFightAndMusic = false;
            Modules.BossChallenge.AddLifeblood.lifebloodAmount = 0;
            Modules.BossChallenge.AddSoul.soulAmount = 0;
            Modules.BossChallenge.SegmentedP5.selectedP5Segment = 0;
            RefreshBossChallengeUi();
            SaveMasterSettings();
        }

        private void OnAlwaysFuriousBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetAlwaysFuriousVisible(false);
        }

        private void OnAlwaysFuriousResetDefaultsClicked()
        {
            SetAlwaysFuriousEnabled(false);
            RefreshAlwaysFuriousUi();
        }

        private void OnQolBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetQolVisible(false);
        }

        private void OnQolResetDefaultsClicked()
        {
            qolMasterEnabled = true;
            qolMasterHasSnapshot = false;
            waitingForFastDreamWarpRebind = false;
            fastDreamWarpPrevKeyRaw = string.Empty;
            fastDreamWarpPrevButton = default;

            SetFastDreamWarpEnabled(false);
            FastDreamWarpSettings.Keybinds.Toggle.ClearBindings();
            SetShortDeathAnimationEnabled(true);
            SetUnlockAllModesEnabled(true);
            SetUnlockPantheonsEnabled(true);
            SetUnlockRadianceEnabled(true);
            SetUnlockRadiantEnabled(true);
            SetInvincibleIndicatorEnabled(true);
            SetScreenShakeEnabled(true);

            RefreshQolUi();
            SaveMasterSettings();
        }

        private void OnMenuAnimationBackClicked()
        {
            bool reopenQol = returnToQolOnClose;
            returnToQolOnClose = false;

            if (reopenQol)
            {
                SetQolVisible(true);
            }
            else
            {
                bool reopenQuick = returnToQuickOnClose;
                returnToQuickOnClose = false;
                if (reopenQuick)
                {
                    SetQuickVisible(true);
                }
            }

            SetMenuAnimationVisible(false);
        }

        private void OnMenuAnimationResetDefaultsClicked()
        {
            menuAnimMasterEnabled = true;
            menuAnimMasterHasSnapshot = false;
            SetDoorDefaultBeginEnabled(true);
            SetMemorizeBindingsEnabled(true);
            SetFasterLoadsEnabled(true);
            SetFastMenusEnabled(true);
            SetFastTextEnabled(true);
            Modules.QoL.SkipCutscenes.AutoSkipCinematics = true;
            Modules.QoL.SkipCutscenes.AllowSkippingNonskippable = true;
            Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt = true;
            RefreshMenuAnimationUi();
            SaveMasterSettings();
        }

        private void OnBossAnimationBackClicked()
        {
            bool reopenQol = returnToQolOnClose;
            returnToQolOnClose = false;

            if (reopenQol)
            {
                SetQolVisible(true);
            }
            else
            {
                bool reopenQuick = returnToQuickOnClose;
                returnToQuickOnClose = false;
                if (reopenQuick)
                {
                    SetQuickVisible(true);
                }
            }

            SetBossAnimationVisible(false);
        }

        private void OnBossAnimationResetDefaultsClicked()
        {
            bossAnimMasterEnabled = true;
            bossAnimMasterHasSnapshot = false;
            Modules.QoL.SkipCutscenes.HallOfGodsStatues = true;
            Modules.QoL.SkipCutscenes.AbsoluteRadiance = true;
            Modules.QoL.SkipCutscenes.PureVesselRoar = true;
            Modules.QoL.SkipCutscenes.GrimmNightmare = true;
            Modules.QoL.SkipCutscenes.GreyPrinceZote = true;
            Modules.QoL.SkipCutscenes.Collector = true;
            Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip = true;
            RefreshBossAnimationUi();
            SaveMasterSettings();
        }

        private void OnQuickSettingsResetDefaultsClicked()
        {
            SetQuickSettingsResetConfirmVisible(true);
        }

        private void OnQuickSettingsResetConfirmYes()
        {
            ResetAllModSettingsToDefaults();
            SetQuickSettingsResetConfirmVisible(false);
        }

        private void OnQuickSettingsResetConfirmNo()
        {
            SetQuickSettingsResetConfirmVisible(false);
        }

        private void ResetAllModSettingsToDefaults()
        {
            quickRenameMode = false;
            if (renameField != null)
            {
                UObject.Destroy(renameField.gameObject);
                renameField = null;
            }

            if (gearSwitcherPresetRenameField != null)
            {
                UObject.Destroy(gearSwitcherPresetRenameField.gameObject);
            }
            gearSwitcherPresetRenameField = null;
            gearSwitcherPresetRenameLabel = null;
            gearSwitcherPresetRenameOriginalLabel = null;
            gearSwitcherPresetRenameTargetName = null;

            SetGearSwitcherPresetDeleteVisible(false);
            SetGearSwitcherResetConfirmVisible(false);
            SetGearSwitcherPresetVisible(false);
            SetGearSwitcherCharmCostVisible(false);
            SetGearSwitcherVisible(false);
            SetBossManipulateVisible(false);
            SetBossManipulateOtherRoomsVisible(false);
            SetGruzMotherP1HelperVisible(false);
            SetVengeflyKingP1HelperVisible(false);
            SetBroodingMawlekP1HelperVisible(false);
            SetNoskP2HelperVisible(false);
            SetUumuuP3HelperVisible(false);
            SetSoulWarriorP1HelperVisible(false);
            SetNoEyesP4HelperVisible(false);
            SetMarmuP2HelperVisible(false);
            SetXeroP2HelperVisible(false);
            SetMarkothP4HelperVisible(false);
            SetGorbP1HelperVisible(false);
            SetGruzHelperVisible(false);
            SetHornetHelperVisible(false);
            SetMawlekHelperVisible(false);
            SetMassiveMossHelperVisible(false);
            SetCrystalGuardianHelperVisible(false);
            SetEnragedGuardianHelperVisible(false);
            SetHornetSentinelHelperVisible(false);
            SetAllAdditionalGhostHelpersVisible(false);
            SetCheatsVisible(false);
            SetTrueBossRushVisible(false);

            ApplyInitialDefaults();
            GearSwitcher.ResetDefaults();

            GodhomeQoL.GlobalSettings.QuickMenuOrder = new List<string>();
            GodhomeQoL.GlobalSettings.QuickMenuPositions = new Dictionary<string, QuickMenuEntryPosition>();
            GodhomeQoL.GlobalSettings.QuickMenuCustomLabels = new Dictionary<string, string>();
            GodhomeQoL.GlobalSettings.QuickMenuVisibility = new Dictionary<string, bool>();
            GodhomeQoL.GlobalSettings.QuickMenuFreeLayout = false;
            GodhomeQoL.GlobalSettings.QuickMenuHotkey = DefaultQuickMenuHotkey;
            GodhomeQoL.GlobalSettings.QuickMenuOverlayHotkeys = new Dictionary<string, string>();

            GodhomeQoL.SaveGlobalSettingsSafe();

            LoadMasterSettings();

            RebuildQuickMenuEntries();
            ApplyQuickMenuLayout();
            ApplyQuickMenuOpacity();
            UpdateFreeMenuLabel();
            UpdateRenameModeLabel();
            UpdateQuickMenuEntryStateColors();
            RefreshQuickMenuSettingsUi();

            RebuildGearSwitcherPresetOverlay();
            SetGearSwitcherPresetVisible(false);

            RefreshFastSuperDashUi();
            RefreshCollectorPhasesUi();
            RefreshFastReloadUi();
            RefreshDreamshieldUi();
            RefreshShowHpOnDeathUi();
            RefreshMaskDamageUi();
            RefreshFreezeHitboxesUi();
            RefreshSpeedChangerUi();
            RefreshTeleportKitUi();
            RefreshBossChallengeUi();
            RefreshRandomPantheonsUi();
            RefreshTrueBossRushUi();
            RefreshCheatsUi();
            RefreshAlwaysFuriousUi();
            RefreshGearSwitcherUi();
            RefreshQolUi();
            RefreshMenuAnimationUi();
            RefreshBossAnimationUi();
            RefreshZoteHelperUi();
            RefreshGruzHelperUi();
            RefreshHornetHelperUi();
            RefreshMawlekHelperUi();
            RefreshMassiveMossHelperUi();
            RefreshCrystalGuardianHelperUi();
            RefreshEnragedGuardianHelperUi();
            RefreshHornetSentinelHelperUi();
            RefreshAdditionalGhostHelpersUi();
        }

        private void OnQuickMenuSettingsBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetQuickSettingsVisible(false);
            UpdateQuickMenuEntryStateColors();
        }

        private void OnZoteHelperBackClicked()
        {
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetZoteHelperVisible(false);
        }

        private void OnZoteHelperResetDefaultsClicked()
        {
            bool wasEnabled = GetZoteHelperEnabled();
            ResetZoteHelperDefaults();
            Modules.BossChallenge.ForceGreyPrinceEnterType.gpzEnterType =
                Modules.BossChallenge.ForceGreyPrinceEnterType.EnterType.Off;
            ApplyBossGpzOption(0);

            if (wasEnabled)
            {
                Modules.BossChallenge.ZoteHelper.ApplyBossHealthIfPresent();
                Modules.BossChallenge.ZoteHelper.ApplyZotelingHealthIfPresent();
            }

            SetZoteHelperEnabled(false);
            RefreshZoteHelperUi();
        }

        private void OnGruzHelperBackClicked()
        {
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetGruzHelperVisible(false);
        }

        private void OnGruzHelperResetDefaultsClicked()
        {
            ResetGruzMotherHelperDefaults();
            SetGruzMotherHelperEnabled(false);
            Modules.BossChallenge.GruzMotherHelper.RestoreVanillaHealthIfPresent();
            RefreshGruzHelperUi();
        }

        private void OnHornetHelperBackClicked()
        {
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetHornetHelperVisible(false);
        }

        private void OnHornetHelperResetDefaultsClicked()
        {
            ResetHornetProtectorHelperDefaults();
            SetHornetProtectorHelperEnabled(false);
            Modules.BossChallenge.HornetProtectorHelper.RestoreVanillaHealthIfPresent();
            RefreshHornetHelperUi();
        }

        private void OnMawlekHelperBackClicked()
        {
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetMawlekHelperVisible(false);
        }

        private void OnMawlekHelperResetDefaultsClicked()
        {
            ResetBroodingMawlekHelperDefaults();
            SetBroodingMawlekHelperEnabled(false);
            Modules.BossChallenge.BroodingMawlekHelper.RestoreVanillaHealthIfPresent();
            RefreshMawlekHelperUi();
        }

        private void OnMassiveMossHelperBackClicked()
        {
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetMassiveMossHelperVisible(false);
        }

        private void OnMassiveMossHelperResetDefaultsClicked()
        {
            ResetMassiveMossChargerHelperDefaults();
            SetMassiveMossChargerHelperEnabled(false);
            Modules.BossChallenge.MassiveMossChargerHelper.RestoreVanillaHealthIfPresent();
            RefreshMassiveMossHelperUi();
        }

        private void OnCrystalGuardianHelperBackClicked()
        {
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetCrystalGuardianHelperVisible(false);
        }

        private void OnCrystalGuardianHelperResetDefaultsClicked()
        {
            ResetCrystalGuardianHelperDefaults();
            SetCrystalGuardianHelperEnabled(false);
            Modules.BossChallenge.CrystalGuardianHelper.RestoreVanillaHealthIfPresent();
            RefreshCrystalGuardianHelperUi();
        }

        private void OnEnragedGuardianHelperBackClicked()
        {
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetEnragedGuardianHelperVisible(false);
        }

        private void OnEnragedGuardianHelperResetDefaultsClicked()
        {
            ResetEnragedGuardianHelperDefaults();
            SetEnragedGuardianHelperEnabled(false);
            Modules.BossChallenge.EnragedGuardianHelper.RestoreVanillaHealthIfPresent();
            RefreshEnragedGuardianHelperUi();
        }

        private void OnHornetSentinelHelperBackClicked()
        {
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetHornetSentinelHelperVisible(false);
            SetAllAdditionalGhostHelpersVisible(false);
        }

        private void OnHornetSentinelHelperResetDefaultsClicked()
        {
            ResetHornetSentinelHelperDefaults();
            SetHornetSentinelHelperEnabled(false);
            Modules.BossChallenge.HornetSentinelHelper.RestoreVanillaHealthIfPresent();
            RefreshHornetSentinelHelperUi();
        }

        private void OnFastSuperDashResetDefaultsClicked()
        {
            SetModuleEnabled(false);
            Modules.QoL.FastSuperDash.instantSuperDash = false;
            Modules.QoL.FastSuperDash.fastSuperDashEverywhere = false;
            Modules.QoL.FastSuperDash.fastSuperDashSpeedMultiplier = 1f;
            fastSuperDashSpeedDirty = false;
            GodhomeQoL.SaveGlobalSettingsSafe();
            RefreshFastSuperDashUi();
        }

        private void OnCollectorResetClicked()
        {
            ResetCollectorPhasesDefaults();
            SetCollectorRoarEnabled(false);
            SetCollectorPhasesEnabled(false);
            RefreshCollectorPhasesUi();
        }
    }
}
