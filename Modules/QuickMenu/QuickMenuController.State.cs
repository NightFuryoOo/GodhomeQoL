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
            return GetInfiniteRadianceClimbingModule()?.Enabled ?? false;
        }

        private void SetInfiniteRadianceClimbingEnabled(bool value)
        {
            Module? module = GetInfiniteRadianceClimbingModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetSegmentedP5Enabled()
        {
            return GetSegmentedP5Module()?.Enabled ?? false;
        }

        private void SetSegmentedP5Enabled(bool value)
        {
            if (value)
            {
                bool hadRandomPantheons =
                    GetRandomPantheonsMasterEnabled()
                    || GetRandomPantheonsEnabled()
                    || Modules.BossChallenge.RandomPantheons.AnyPantheonEnabled;
                bool hadTrueBossRush =
                    GetTrueBossRushMasterEnabled()
                    || Modules.BossChallenge.TrueBossRush.AnyPantheonEnabled;

                if (hadRandomPantheons)
                {
                    SetRandomPantheonsMasterEnabled(false);
                    SetRandomPantheonsEnabled(false);
                    _ = Modules.BossChallenge.PantheonSequenceCompatibility.DisableRandomPantheons();
                }

                if (hadTrueBossRush)
                {
                    SetTrueBossRushMasterEnabled(false);
                    _ = Modules.BossChallenge.PantheonSequenceCompatibility.DisableTrueBossRush();
                }

                if (hadRandomPantheons || hadTrueBossRush)
                {
                    ShowStatusMessage("Segmented P5 disabled Random Pantheons / True Boss Rush.");
                }
            }

            Module? module = GetSegmentedP5Module();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetAddLifebloodEnabled()
        {
            return GetAddLifebloodModule()?.Enabled ?? false;
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
            return GetAddSoulModule()?.Enabled ?? false;
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
            return GetForceArriveAnimationModule()?.Enabled ?? false;
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
            return GetCollectorRoarModule()?.Enabled ?? false;
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
            return GetUnlockAllModesModule()?.Enabled ?? false;
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
            return GetUnlockPantheonsModule()?.Enabled ?? false;
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
            return GetUnlockRadianceModule()?.Enabled ?? false;
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
            return GetUnlockRadiantModule()?.Enabled ?? false;
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
            return GetDoorDefaultBeginModule()?.Enabled ?? false;
        }

        private void SetDoorDefaultBeginEnabled(bool value)
        {
            Module? module = GetDoorDefaultBeginModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetFasterLoadsEnabled()
        {
            return GetFasterLoadsModule()?.Enabled ?? false;
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
            return GetFastMenusModule()?.Enabled ?? false;
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
            return GetFastTextModule()?.Enabled ?? false;
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
            return GetFastDreamWarpModule()?.Enabled ?? false;
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
            return GetShortDeathAnimationModule()?.Enabled ?? false;
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
            return GetInvincibleIndicatorModule()?.Enabled ?? false;
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
            return GetScreenShakeModule()?.Enabled ?? false;
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

        private bool GetSpeedChangerEnabled()
        {
            SpeedChanger? module = GetSpeedChangerModule();
            return module?.Enabled ?? false;
        }
    }
}
