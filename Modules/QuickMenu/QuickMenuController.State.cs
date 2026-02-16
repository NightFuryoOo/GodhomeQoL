using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void SetInfiniteGrimmPufferfishEnabled(bool value)
        {
            Module? module = GetInfiniteGrimmPufferfishModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetInfiniteRadianceClimbingEnabled()
        {
            Module? module = GetInfiniteRadianceClimbingModule();
            return module?.Enabled ?? false;
        }

        private void SetInfiniteRadianceClimbingEnabled(bool value)
        {
            Module? module = GetInfiniteRadianceClimbingModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetP5HealthEnabled()
        {
            Module? module = GetP5HealthModule();
            return module?.Enabled ?? false;
        }

        private void SetP5HealthEnabled(bool value)
        {
            Module? module = GetP5HealthModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetSegmentedP5Enabled()
        {
            Module? module = GetSegmentedP5Module();
            return module?.Enabled ?? false;
        }

        private void SetSegmentedP5Enabled(bool value)
        {
            Module? module = GetSegmentedP5Module();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetHalveAscendedEnabled()
        {
            Module? module = GetHalveAscendedModule();
            return module?.Enabled ?? false;
        }

        private void SetHalveAscendedEnabled(bool value)
        {
            Module? module = GetHalveAscendedModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetHalveAttunedEnabled()
        {
            Module? module = GetHalveAttunedModule();
            return module?.Enabled ?? false;
        }

        private void SetHalveAttunedEnabled(bool value)
        {
            Module? module = GetHalveAttunedModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetAddLifebloodEnabled()
        {
            Module? module = GetAddLifebloodModule();
            return module?.Enabled ?? false;
        }

        private void SetAddLifebloodEnabled(bool value)
        {
            Module? module = GetAddLifebloodModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetAddSoulEnabled()
        {
            Module? module = GetAddSoulModule();
            return module?.Enabled ?? false;
        }

        private void SetAddSoulEnabled(bool value)
        {
            Module? module = GetAddSoulModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private int GetCarefreeMelodyMode() =>
            Modules.QoL.CarefreeMelodyReset.GetMode();

        private string GetCarefreeMelodyModeLabel() =>
            GetCarefreeMelodyMode() switch
            {
                Modules.QoL.CarefreeMelodyReset.ModeHoG => "HoG",
                Modules.QoL.CarefreeMelodyReset.ModeHoGAndPantheons => "HoG & Pantheons",
                _ => "Off"
            };

        private void ToggleCarefreeMelodyMode()
        {
            int next = GetCarefreeMelodyMode() + 1;
            if (next > Modules.QoL.CarefreeMelodyReset.ModeHoGAndPantheons)
            {
                next = Modules.QoL.CarefreeMelodyReset.ModeOff;
            }

            SetCarefreeMelodyMode(next);
            if (qolCarefreeMelodyValue != null)
            {
                qolCarefreeMelodyValue.text = GetCarefreeMelodyModeLabel();
            }
        }

        private void SetCarefreeMelodyMode(int value)
        {
            Modules.QoL.CarefreeMelodyReset.SetMode(value);
        }

        private bool GetForceArriveAnimationEnabled()
        {
            Module? module = GetForceArriveAnimationModule();
            return module?.Enabled ?? false;
        }

        private void SetForceArriveAnimationEnabled(bool value)
        {
            Module? module = GetForceArriveAnimationModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetCollectorRoarEnabled()
        {
            Module? module = GetCollectorRoarModule();
            return module?.Enabled ?? false;
        }

        private void SetCollectorRoarEnabled(bool value)
        {
            Module? module = GetCollectorRoarModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetUnlockAllModesEnabled()
        {
            Module? module = GetUnlockAllModesModule();
            return module?.Enabled ?? false;
        }

        private void SetUnlockAllModesEnabled(bool value)
        {
            Module? module = GetUnlockAllModesModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetUnlockPantheonsEnabled()
        {
            Module? module = GetUnlockPantheonsModule();
            return module?.Enabled ?? false;
        }

        private void SetUnlockPantheonsEnabled(bool value)
        {
            Module? module = GetUnlockPantheonsModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetUnlockRadianceEnabled()
        {
            Module? module = GetUnlockRadianceModule();
            return module?.Enabled ?? false;
        }

        private void SetUnlockRadianceEnabled(bool value)
        {
            Module? module = GetUnlockRadianceModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetUnlockRadiantEnabled()
        {
            Module? module = GetUnlockRadiantModule();
            return module?.Enabled ?? false;
        }

        private void SetUnlockRadiantEnabled(bool value)
        {
            Module? module = GetUnlockRadiantModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetDoorDefaultBeginEnabled()
        {
            Module? module = GetDoorDefaultBeginModule();
            return module?.Enabled ?? false;
        }

        private bool GetMenuAnimationMasterEnabled() => menuAnimMasterEnabled;

        private void SetMenuAnimationMasterEnabled(bool value)
        {
            if (menuAnimMasterEnabled == value)
            {
                return;
            }

            menuAnimMasterEnabled = value;
            if (!value)
            {
                CaptureMenuAnimationSnapshot();
                SetMenuAnimationAll(false);
            }
            else
            {
                RestoreMenuAnimationSnapshot();
            }

            RefreshMenuAnimationUi();
            UpdateQuickMenuEntryStateColors();
            SaveMasterSettings();
        }

        private void SetDoorDefaultBeginEnabled(bool value)
        {
            Module? module = GetDoorDefaultBeginModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetMemorizeBindingsEnabled()
        {
            Module? module = GetMemorizeBindingsModule();
            return module?.Enabled ?? false;
        }

        private void SetMemorizeBindingsEnabled(bool value)
        {
            Module? module = GetMemorizeBindingsModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private void CaptureMenuAnimationSnapshot()
        {
            menuAnimMasterHasSnapshot = true;
            menuAnimSavedDoorDefaultBegin = GetDoorDefaultBeginEnabled();
            menuAnimSavedMemorizeBindings = GetMemorizeBindingsEnabled();
            menuAnimSavedFasterLoads = GetFasterLoadsEnabled();
            menuAnimSavedFastMenus = GetFastMenusEnabled();
            menuAnimSavedFastText = GetFastTextEnabled();
            menuAnimSavedAutoSkip = Modules.QoL.SkipCutscenes.AutoSkipCinematics;
            menuAnimSavedAllowSkipping = Modules.QoL.SkipCutscenes.AllowSkippingNonskippable;
            menuAnimSavedSkipWithoutPrompt = Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt;
        }

        private void RestoreMenuAnimationSnapshot()
        {
            if (!menuAnimMasterHasSnapshot)
            {
                return;
            }

            SetDoorDefaultBeginEnabled(menuAnimSavedDoorDefaultBegin);
            SetMemorizeBindingsEnabled(menuAnimSavedMemorizeBindings);
            SetFasterLoadsEnabled(menuAnimSavedFasterLoads);
            SetFastMenusEnabled(menuAnimSavedFastMenus);
            SetFastTextEnabled(menuAnimSavedFastText);
            Modules.QoL.SkipCutscenes.AutoSkipCinematics = menuAnimSavedAutoSkip;
            Modules.QoL.SkipCutscenes.AllowSkippingNonskippable = menuAnimSavedAllowSkipping;
            Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt = menuAnimSavedSkipWithoutPrompt;
        }

        private void SetMenuAnimationAll(bool value)
        {
            SetDoorDefaultBeginEnabled(value);
            SetMemorizeBindingsEnabled(value);
            SetFasterLoadsEnabled(value);
            SetFastMenusEnabled(value);
            SetFastTextEnabled(value);
            Modules.QoL.SkipCutscenes.AutoSkipCinematics = value;
            Modules.QoL.SkipCutscenes.AllowSkippingNonskippable = value;
            Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt = value;
        }

        private bool GetQolMasterEnabled() => qolMasterEnabled;

        private void SetQolMasterEnabled(bool value)
        {
            if (qolMasterEnabled == value)
            {
                return;
            }

            qolMasterEnabled = value;
            if (!value)
            {
                CaptureQolSnapshot();
                SetQolAll(false);
            }
            else
            {
                RestoreQolSnapshot();
            }

            RefreshQolUi();
            UpdateQuickMenuEntryStateColors();
            SaveMasterSettings();
        }

        private void CaptureQolSnapshot()
        {
            qolMasterHasSnapshot = true;
            qolSavedFastDreamWarp = GetFastDreamWarpEnabled();
            qolSavedShortDeath = GetShortDeathAnimationEnabled();
            qolSavedHallOfGods = Modules.QoL.SkipCutscenes.HallOfGodsStatues;
            qolSavedUnlockAllModes = GetUnlockAllModesEnabled();
            qolSavedUnlockPantheons = GetUnlockPantheonsEnabled();
            qolSavedUnlockRadiance = GetUnlockRadianceEnabled();
            qolSavedUnlockRadiant = GetUnlockRadiantEnabled();
            qolSavedInvincibleIndicator = GetInvincibleIndicatorEnabled();
            qolSavedScreenShake = GetScreenShakeEnabled();
        }

        private void RestoreQolSnapshot()
        {
            if (!qolMasterHasSnapshot)
            {
                return;
            }

            SetFastDreamWarpEnabled(qolSavedFastDreamWarp);
            SetShortDeathAnimationEnabled(qolSavedShortDeath);
            Modules.QoL.SkipCutscenes.HallOfGodsStatues = qolSavedHallOfGods;
            SetUnlockAllModesEnabled(qolSavedUnlockAllModes);
            SetUnlockPantheonsEnabled(qolSavedUnlockPantheons);
            SetUnlockRadianceEnabled(qolSavedUnlockRadiance);
            SetUnlockRadiantEnabled(qolSavedUnlockRadiant);
            SetInvincibleIndicatorEnabled(qolSavedInvincibleIndicator);
            SetScreenShakeEnabled(qolSavedScreenShake);
        }

        private void SetQolAll(bool value)
        {
            SetFastDreamWarpEnabled(value);
            SetShortDeathAnimationEnabled(value);
            Modules.QoL.SkipCutscenes.HallOfGodsStatues = value;
            SetUnlockAllModesEnabled(value);
            SetUnlockPantheonsEnabled(value);
            SetUnlockRadianceEnabled(value);
            SetUnlockRadiantEnabled(value);
            SetInvincibleIndicatorEnabled(value);
            SetScreenShakeEnabled(value);
        }

        private void UpdateQolInteractivity()
        {
            SetContentInteractivity(qolContent, qolMasterEnabled, "QolEnableRow");
        }

        private void UpdateMenuAnimationInteractivity()
        {
            SetContentInteractivity(menuAnimationContent, menuAnimMasterEnabled, "MenuAnimEnableRow");
        }

        private void UpdateBossChallengeInteractivity()
        {
            SetContentInteractivity(bossChallengeContent, bossChallengeMasterEnabled, "BossChallengeEnableRow");
        }

        private void UpdateBossAnimationInteractivity()
        {
            SetContentInteractivity(bossAnimationContent, bossAnimMasterEnabled, "BossAnimEnableRow");
        }

        private void UpdateRandomPantheonsInteractivity()
        {
            SetContentInteractivity(randomPantheonsContent, randomPantheonsMasterEnabled, "RandomPantheonsToggleRow");
        }

        private void UpdateTrueBossRushInteractivity()
        {
            SetContentInteractivity(trueBossRushContent, trueBossRushMasterEnabled, "TrueBossRushToggleRow");
        }

        private void UpdateCheatsInteractivity()
        {
            SetContentInteractivity(cheatsContent, cheatsMasterEnabled, "CheatsEnableRow");
        }

        private void UpdateFastSuperDashInteractivity()
        {
            SetContentInteractivity(fastSuperDashContent, GetModuleEnabled(), "ModuleToggleRow");
        }

        private void UpdateCollectorInteractivity()
        {
            SetContentInteractivity(collectorContent, GetCollectorPhasesEnabled(), "CollectorModuleToggleRow");
        }

        private void UpdateFastReloadInteractivity()
        {
            SetContentInteractivity(fastReloadContent, GetFastReloadEnabled(), "FastReloadToggleRow");
        }

        private void UpdateDreamshieldInteractivity()
        {
            SetContentInteractivity(dreamshieldContent, GetDreamshieldEnabled(), "DreamshieldToggleRow");
        }

        private void UpdateShowHpOnDeathInteractivity()
        {
            SetContentInteractivity(showHpOnDeathContent, GetShowHpOnDeathEnabled(), "ShowHPOnDeathGlobalRow");
        }

        private void UpdateMaskDamageInteractivity()
        {
            SetContentInteractivity(maskDamageContent, GetMaskDamageEnabled(), "MaskDamageEnableRow");
        }

        private void UpdateFreezeHitboxesInteractivity()
        {
            SetContentInteractivity(freezeHitboxesContent, GetFreezeHitboxesEnabled(), "FreezeHitboxesEnableRow");
        }

        private void UpdateSpeedChangerInteractivity()
        {
            SetContentInteractivity(speedChangerContent, SpeedChanger.globalSwitch, "SpeedChangerGlobalRow");
        }

        private void UpdateTeleportKitInteractivity()
        {
            SetContentInteractivity(teleportKitContent, GetTeleportKitEnabled(), "TeleportKitToggleRow");
        }

        private void UpdateGearSwitcherInteractivity()
        {
            SetContentInteractivity(gearSwitcherContent, GetGearSwitcherEnabled(), "GearSwitcherEnableRow");
        }

        private void UpdateZoteHelperInteractivity()
        {
            SetContentInteractivity(zoteHelperContent, GetZoteHelperEnabled(), "ZoteHelperEnableRow");
        }

        private static void SetContentInteractivity(RectTransform? content, bool enabled, string masterRowName)
        {
            if (content == null)
            {
                return;
            }

            float dimAlpha = enabled ? 1f : DisabledContentAlpha;
            int childCount = content.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = content.GetChild(i);
                CanvasGroup group = child.GetComponent<CanvasGroup>() ?? child.gameObject.AddComponent<CanvasGroup>();
                if (!enabled && string.Equals(child.name, masterRowName, StringComparison.Ordinal))
                {
                    group.alpha = 1f;
                }
                else
                {
                    group.alpha = dimAlpha;
                }
            }

            Selectable[] selectables = content.GetComponentsInChildren<Selectable>(true);
            foreach (Selectable selectable in selectables)
            {
                bool allow = enabled;
                if (!enabled)
                {
                    Transform? current = selectable.transform;
                    while (current != null && current != content)
                    {
                        if (string.Equals(current.name, masterRowName, StringComparison.Ordinal))
                        {
                            allow = true;
                            break;
                        }
                        current = current.parent;
                    }
                }

                selectable.interactable = allow;
            }
        }

        private bool GetFasterLoadsEnabled()
        {
            Module? module = GetFasterLoadsModule();
            return module?.Enabled ?? false;
        }

        private void SetFasterLoadsEnabled(bool value)
        {
            Module? module = GetFasterLoadsModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetFastMenusEnabled()
        {
            Module? module = GetFastMenusModule();
            return module?.Enabled ?? false;
        }

        private void SetFastMenusEnabled(bool value)
        {
            Module? module = GetFastMenusModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetFastTextEnabled()
        {
            Module? module = GetFastTextModule();
            return module?.Enabled ?? false;
        }

        private void SetFastTextEnabled(bool value)
        {
            Module? module = GetFastTextModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetFastDreamWarpEnabled()
        {
            Module? module = GetFastDreamWarpModule();
            return module?.Enabled ?? false;
        }

        private void SetFastDreamWarpEnabled(bool value)
        {
            Module? module = GetFastDreamWarpModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetShortDeathAnimationEnabled()
        {
            Module? module = GetShortDeathAnimationModule();
            return module?.Enabled ?? false;
        }

        private void SetShortDeathAnimationEnabled(bool value)
        {
            Module? module = GetShortDeathAnimationModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetInvincibleIndicatorEnabled()
        {
            Module? module = GetInvincibleIndicatorModule();
            return module?.Enabled ?? false;
        }

        private void SetInvincibleIndicatorEnabled(bool value)
        {
            Module? module = GetInvincibleIndicatorModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetScreenShakeEnabled()
        {
            Module? module = GetScreenShakeModule();
            return module?.Enabled ?? false;
        }

        private void SetScreenShakeEnabled(bool value)
        {
            Module? module = GetScreenShakeModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private int GetBossGpzIndex()
        {
            Module? module = GetForceGreyPrinceModule();
            if (module?.Enabled != true)
            {
                return 0;
            }

            return Modules.BossChallenge.ForceGreyPrinceEnterType.gpzEnterType switch
            {
                Modules.BossChallenge.ForceGreyPrinceEnterType.EnterType.Long => 1,
                Modules.BossChallenge.ForceGreyPrinceEnterType.EnterType.Short => 2,
                _ => 0
            };
        }

        private void ApplyBossGpzOption(int index)
        {
            int clamped = ClampOptionIndex(index, BossChallengeGpzOptions.Length);
            Module? module = GetForceGreyPrinceModule();

            if (clamped == 0)
            {
                if (module != null)
                {
                    module.Enabled = false;
                }
            }
            else
            {
                Modules.BossChallenge.ForceGreyPrinceEnterType.gpzEnterType = clamped == 1
                    ? Modules.BossChallenge.ForceGreyPrinceEnterType.EnterType.Long
                    : Modules.BossChallenge.ForceGreyPrinceEnterType.EnterType.Short;

                if (module != null)
                {
                    module.Enabled = true;
                }
            }

            if (bossGpzValue != null)
            {
                bossGpzValue.text = BossChallengeGpzOptions[clamped];
            }
        }

        private string GetBossGpzLabel()
        {
            int index = GetBossGpzIndex();
            return BossChallengeGpzOptions[index];
        }

        private void SetSpeedChangerGlobalSwitch(bool value)
        {
            SpeedChanger? module = GetSpeedChangerModule();
            if (module != null)
            {
                MethodInfo? method = typeof(SpeedChanger).GetMethod("ChangeGlobalSwitchState", BindingFlags.Instance | BindingFlags.NonPublic);
                if (method != null)
                {
                    method.Invoke(module, new object[] { value, false });
                }
                else
                {
                    SpeedChanger.globalSwitch = value;
                }
            }
            else
            {
                SpeedChanger.globalSwitch = value;
            }

            UpdateSpeedChangerInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void ApplySpeedChangerDisplayStyle(int value)
        {
            int style = ClampOptionIndex(value, SpeedChangerDisplayOptions.Length);
            SpeedChanger.displayStyle = style;

            if (style == 2)
            {
                ModDisplay.Instance?.Destroy();
                ModDisplay.Instance = null;
            }
            else
            {
                ModDisplay.Instance ??= new ModDisplay();
            }

            if (speedChangerDisplayValue != null)
            {
                speedChangerDisplayValue.text = SpeedChangerDisplayOptions[style];
            }
        }

        private static GearSwitcherSettings GearSwitcherSettings =>
            GearSwitcher.Settings;

        private bool GetSpeedChangerEnabled()
        {
            SpeedChanger? module = GetSpeedChangerModule();
            return module?.Enabled ?? false;
        }

        private string[] GetGearSwitcherPresetOptions()
        {
            return GearSwitcher.GetPresetOrder().ToArray();
        }

        private static bool IsFullGearPreset(string presetName) =>
            string.Equals(presetName, "FullGear", StringComparison.OrdinalIgnoreCase);

        private string GetFullGearDisplayName()
        {
            string name = GearSwitcherSettings.FullGearDisplayName;
            return string.IsNullOrWhiteSpace(name) ? "FullGear" : name;
        }

        private void SetFullGearDisplayName(string name)
        {
            GearSwitcherSettings.FullGearDisplayName = name;
            GodhomeQoL.SaveGlobalSettingsSafe();
        }

        private bool IsGearSwitcherPresetNameTaken(string name, string? ignoreName = null)
        {
            foreach (string preset in GetGearSwitcherPresetOptions())
            {
                if (!string.IsNullOrEmpty(ignoreName)
                    && string.Equals(preset, ignoreName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (IsFullGearPreset(preset))
                {
                    continue;
                }

                if (string.Equals(preset, name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private string GetGearSwitcherPresetDisplayName(string presetName)
        {
            return IsFullGearPreset(presetName) ? GetFullGearDisplayName() : presetName;
        }

        private string GetSelectedPresetDisplayLabel(string presetName)
        {
            string displayName = GetGearSwitcherPresetDisplayName(presetName);
            if (IsBuiltinPresetName(presetName) && IsBuiltinPresetEdited(presetName))
            {
                return $"[Edited] {displayName}";
            }

            return displayName;
        }

        private void UpdateGearSwitcherSelectedPresetColor(string presetName)
        {
            if (gearSwitcherSelectedPresetValue == null)
            {
                return;
            }

            bool edited = IsBuiltinPresetEdited(presetName);
            gearSwitcherSelectedPresetValue.color = edited ? GearSwitcherPresetEditedColor : Color.white;
        }

        private void MarkGearSwitcherPresetEdited()
        {
            UpdateGearSwitcherSelectedPresetLabel(gearSwitcherSelectedPreset);
            UpdateGearSwitcherSelectedPresetColor(gearSwitcherSelectedPreset);
        }

        private void UpdateGearSwitcherSelectedPresetLabel(string presetName)
        {
            if (gearSwitcherSelectedPresetValue == null)
            {
                return;
            }

            gearSwitcherSelectedPresetValue.text = string.IsNullOrWhiteSpace(presetName)
                ? string.Empty
                : GetSelectedPresetDisplayLabel(presetName);
        }

        private bool IsBuiltinPresetEdited(string presetName)
        {
            if (!IsBuiltinPresetName(presetName))
            {
                return false;
            }

            if (GearSwitcherSettings.Presets == null
                || !GearSwitcherSettings.Presets.TryGetValue(presetName, out GearPreset current))
            {
                return false;
            }

            Dictionary<string, GearPreset> defaults = GearPresetDefaults.CreateDefaults();
            if (!defaults.TryGetValue(presetName, out GearPreset baseline))
            {
                return false;
            }

            return !ArePresetsEqual(current, baseline);
        }

        private static bool ArePresetsEqual(GearPreset current, GearPreset baseline)
        {
            if (current.MaxHealth != baseline.MaxHealth
                || current.SoulVessels != baseline.SoulVessels
                || current.NailDamage != baseline.NailDamage
                || current.CharmSlots != baseline.CharmSlots
                || current.MainSoulGain != baseline.MainSoulGain
                || current.ReserveSoulGain != baseline.ReserveSoulGain
                || current.GatheringSwarmCost != baseline.GatheringSwarmCost
                || current.WaywardCompassCost != baseline.WaywardCompassCost
                || current.StalwartShellCost != baseline.StalwartShellCost
                || current.SoulCatcherCost != baseline.SoulCatcherCost
                || current.ShamanStoneCost != baseline.ShamanStoneCost
                || current.SoulEaterCost != baseline.SoulEaterCost
                || current.DashmasterCost != baseline.DashmasterCost
                || current.SprintmasterCost != baseline.SprintmasterCost
                || current.GrubsongCost != baseline.GrubsongCost
                || current.GrubberflysElegyCost != baseline.GrubberflysElegyCost
                || current.UnbreakableHeartCost != baseline.UnbreakableHeartCost
                || current.UnbreakableGreedCost != baseline.UnbreakableGreedCost
                || current.UnbreakableStrengthCost != baseline.UnbreakableStrengthCost
                || current.SpellTwisterCost != baseline.SpellTwisterCost
                || current.SteadyBodyCost != baseline.SteadyBodyCost
                || current.HeavyBlowCost != baseline.HeavyBlowCost
                || current.QuickSlashCost != baseline.QuickSlashCost
                || current.LongnailCost != baseline.LongnailCost
                || current.MarkOfPrideCost != baseline.MarkOfPrideCost
                || current.FuryOfTheFallenCost != baseline.FuryOfTheFallenCost
                || current.ThornsOfAgonyCost != baseline.ThornsOfAgonyCost
                || current.BaldurShellCost != baseline.BaldurShellCost
                || current.FlukenestCost != baseline.FlukenestCost
                || current.DefendersCrestCost != baseline.DefendersCrestCost
                || current.GlowingWombCost != baseline.GlowingWombCost
                || current.QuickFocusCost != baseline.QuickFocusCost
                || current.DeepFocusCost != baseline.DeepFocusCost
                || current.LifebloodHeartCost != baseline.LifebloodHeartCost
                || current.LifebloodCoreCost != baseline.LifebloodCoreCost
                || current.JonisBlessingCost != baseline.JonisBlessingCost
                || current.HivebloodCost != baseline.HivebloodCost
                || current.SporeShroomCost != baseline.SporeShroomCost
                || current.SharpShadowCost != baseline.SharpShadowCost
                || current.ShapeOfUnnCost != baseline.ShapeOfUnnCost
                || current.NailmastersGloryCost != baseline.NailmastersGloryCost
                || current.WeaversongCost != baseline.WeaversongCost
                || current.DreamWielderCost != baseline.DreamWielderCost
                || current.DreamshieldCost != baseline.DreamshieldCost
                || current.CarefreeMelodyCost != baseline.CarefreeMelodyCost
                || current.GrimmchildCost != baseline.GrimmchildCost
                || current.VoidHeartCost != baseline.VoidHeartCost
                || current.KingsoulCost != baseline.KingsoulCost
                || current.UseVoidHeart != baseline.UseVoidHeart
                || current.UseGrimmchild != baseline.UseGrimmchild
                || current.DreamNailLevel != baseline.DreamNailLevel
                || current.Nailless != baseline.Nailless
                || current.Overcharmed != baseline.Overcharmed)
            {
                return false;
            }

            if (!EffectiveBoolsEqual(current.HasMoveAbilities, current.HasAllMoveAbilities,
                    baseline.HasMoveAbilities, baseline.HasAllMoveAbilities,
                    new[] { "AcidArmour", "Dash", "Walljump", "SuperDash", "ShadowDash", "DoubleJump" })
                || !DictionaryEquals(current.SpellsLevel, baseline.SpellsLevel)
                || !DictionaryEquals(current.HasNailArts, baseline.HasNailArts)
                || !EffectiveBoolsEqual(current.Bindings, current.HasAllBindings,
                    baseline.Bindings, baseline.HasAllBindings,
                    new[] { "CharmsBinding", "NailBinding", "ShellBinding", "SoulBinding" }))
            {
                return false;
            }

            return ListsEqual(current.EquippedCharms, baseline.EquippedCharms);
        }

        private static bool DictionaryEquals<TKey, TValue>(Dictionary<TKey, TValue>? current, Dictionary<TKey, TValue>? baseline)
            where TKey : notnull
        {
            if (ReferenceEquals(current, baseline))
            {
                return true;
            }

            if (current == null || baseline == null)
            {
                return false;
            }

            if (current.Count != baseline.Count)
            {
                return false;
            }

            foreach (KeyValuePair<TKey, TValue> pair in current)
            {
                if (!baseline.TryGetValue(pair.Key, out TValue? value)
                    || !EqualityComparer<TValue>.Default.Equals(pair.Value, value))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool EffectiveBoolsEqual(
            Dictionary<string, bool>? current,
            bool currentAll,
            Dictionary<string, bool>? baseline,
            bool baselineAll,
            string[] keys)
        {
            foreach (string key in keys)
            {
                bool currentValue = currentAll
                    || (current != null && current.TryGetValue(key, out bool currentValueResult) && currentValueResult);
                bool baselineValue = baselineAll
                    || (baseline != null && baseline.TryGetValue(key, out bool baselineValueResult) && baselineValueResult);
                if (currentValue != baselineValue)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ListsEqual(List<int>? current, List<int>? baseline)
        {
            if (current == null || current.Count == 0)
            {
                return baseline == null || baseline.Count == 0;
            }

            if (baseline == null || baseline.Count == 0)
            {
                return false;
            }

            if (current.Count != baseline.Count)
            {
                return false;
            }

            List<int> currentSorted = new(current);
            List<int> baselineSorted = new(baseline);
            currentSorted.Sort();
            baselineSorted.Sort();

            for (int i = 0; i < currentSorted.Count; i++)
            {
                if (currentSorted[i] != baselineSorted[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsBuiltinPresetName(string presetName)
        {
            foreach (string name in GearPresetDefaults.DefaultOrder())
            {
                if (string.Equals(presetName, name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private string NormalizeCustomPresetName(string currentName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return currentName;
            }

            if (string.Equals(value, currentName, StringComparison.OrdinalIgnoreCase))
            {
                return currentName;
            }

            if (IsBuiltinPresetName(value))
            {
                return currentName;
            }

            if (IsGearSwitcherPresetNameTaken(value, currentName))
            {
                return currentName;
            }

            return value;
        }
    }
}
