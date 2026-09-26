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
    }
}
