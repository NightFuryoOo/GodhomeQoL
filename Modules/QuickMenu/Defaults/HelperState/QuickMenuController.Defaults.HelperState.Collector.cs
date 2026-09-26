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
            catch (Exception swallowed)
            {
                LogSuppressed(swallowed, "QuickMenuController.Defaults.HelperState.Collector.cs");
            }

            Modules.CollectorPhases.CollectorPhases.collectorPhase = 3;
            Modules.CollectorPhases.CollectorPhases.collectorMaxHP = 1200;
            Modules.CollectorPhases.CollectorPhases.UseMaxHP = true;
            Modules.CollectorPhases.CollectorPhases.collectorP5Hp = false;
            Modules.CollectorPhases.CollectorPhases.collectorMaxHPBeforeP5 = 1200;
            Modules.CollectorPhases.CollectorPhases.collectorHasStoredMaxHpBeforeP5 = false;
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
            Modules.CollectorPhases.CollectorPhases.CustomSummonLimit = 4;
        }

        private bool GetCollectorPhasesEnabled()
        {
            return GetCollectorPhasesModule()?.Enabled ?? false;
        }

        private void SetCollectorPhasesEnabled(bool value)
        {
            SetModuleEnabledFlag(GetCollectorPhasesModule(), value);
            UpdateCollectorInteractivity();
            UpdateQuickMenuEntryStateColors();
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

        private void SetCollectorP5HpEnabled(bool value)
        {
            Modules.CollectorPhases.CollectorPhases.SetP5HpEnabled(value);
            RefreshCollectorPhasesUi();
        }

        private void SetCollectorUseMaxHpEnabled(bool value)
        {
            if (Modules.CollectorPhases.CollectorPhases.collectorP5Hp)
            {
                Modules.CollectorPhases.CollectorPhases.UseMaxHP = true;
                RefreshCollectorPhasesUi();
                return;
            }

            Modules.CollectorPhases.CollectorPhases.UseMaxHP = value;
            ReapplyCollectorPhasesLiveSettings();
        }

        private void SetCollectorMaxHp(int value)
        {
            if (Modules.CollectorPhases.CollectorPhases.collectorP5Hp)
            {
                Modules.CollectorPhases.CollectorPhases.collectorMaxHP = 900;
                RefreshCollectorPhasesUi();
                return;
            }

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
    }
}
