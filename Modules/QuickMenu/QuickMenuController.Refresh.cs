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
    {        private void RefreshFastSuperDashUi()
        {
            Module? module = GetFastSuperDashModule();
            UpdateToggleValue(moduleToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(moduleToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(instantToggleValue, Modules.QoL.FastSuperDash.instantSuperDash);
            UpdateToggleValue(everywhereToggleValue, Modules.QoL.FastSuperDash.fastSuperDashEverywhere);

            float speed = Modules.QoL.FastSuperDash.fastSuperDashSpeedMultiplier;
            UpdateSpeedValueText(speed);
            if (speedSlider != null)
            {
                speedSlider.value = Mathf.Clamp(speed, speedSlider.minValue, speedSlider.maxValue);
            }

            UpdateFastSuperDashInteractivity();
        }

        private void RefreshCollectorPhasesUi()
        {
            Module? module = GetCollectorPhasesModule();
            UpdateToggleValue(collectorModuleToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(collectorModuleToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(collectorHoGOnlyValue, Modules.CollectorPhases.CollectorPhases.HoGOnly);
            UpdateToggleValue(qolCollectorRoarValue, GetCollectorRoarEnabled());
            UpdateToggleValue(collectorImmortalValue, Modules.CollectorPhases.CollectorPhases.CollectorImmortal);
            UpdateToggleValue(ignoreInitialJarLimitValue, Modules.CollectorPhases.CollectorPhases.IgnoreInitialJarLimit);
            UpdateToggleValue(useCustomPhase2ThresholdValue, Modules.CollectorPhases.CollectorPhases.UseCustomPhase2Threshold);
            UpdateToggleValue(useMaxHpValue, Modules.CollectorPhases.CollectorPhases.UseMaxHP);
            UpdateToggleValue(spawnBuzzerValue, Modules.CollectorPhases.CollectorPhases.spawnBuzzer);
            UpdateToggleValue(spawnRollerValue, Modules.CollectorPhases.CollectorPhases.spawnRoller);
            UpdateToggleValue(spawnSpitterValue, Modules.CollectorPhases.CollectorPhases.spawnSpitter);
            UpdateToggleValue(disableSummonLimitValue, Modules.CollectorPhases.CollectorPhases.DisableSummonLimit);

            UpdateIntValueText(collectorPhaseValue, Modules.CollectorPhases.CollectorPhases.collectorPhase);
            UpdateIntInputValue(customPhase2ThresholdField, Modules.CollectorPhases.CollectorPhases.CustomPhase2Threshold);
            UpdateIntInputValue(collectorMaxHpField, Modules.CollectorPhases.CollectorPhases.collectorMaxHP);
            UpdateIntInputValue(buzzerHpField, Modules.CollectorPhases.CollectorPhases.buzzerHP);
            UpdateIntInputValue(rollerHpField, Modules.CollectorPhases.CollectorPhases.rollerHP);
            UpdateIntInputValue(spitterHpField, Modules.CollectorPhases.CollectorPhases.spitterHP);
            UpdateIntInputValue(customSummonLimitField, Modules.CollectorPhases.CollectorPhases.CustomSummonLimit);

            UpdateCollectorInteractivity();
        }

        private void RefreshFastReloadUi()
        {
            Module? module = GetFastReloadModule();
            UpdateToggleValue(fastReloadToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(fastReloadToggleIcon, module?.Enabled ?? false);
            UpdateKeybindValue(reloadKeyValue, FormatKeyLabel(GetReloadKey()));

            UpdateFastReloadInteractivity();
        }

        private void RefreshDreamshieldUi()
        {
            bool enabled = Modules.QoL.DreamshieldStartAngle.startAngleEnabled;
            UpdateToggleValue(dreamshieldToggleValue, enabled);
            UpdateToggleIcon(dreamshieldToggleIcon, enabled);

            float delay = Modules.QoL.DreamshieldStartAngle.rotationDelay;
            UpdateFloatValueText(dreamshieldDelayValue, delay, 2);
            if (dreamshieldDelaySlider != null)
            {
                dreamshieldDelaySlider.value = Mathf.Clamp(delay, dreamshieldDelaySlider.minValue, dreamshieldDelaySlider.maxValue);
            }

            float speed = Modules.QoL.DreamshieldStartAngle.rotationSpeed;
            UpdateFloatValueText(dreamshieldSpeedValue, speed, 2);
            if (dreamshieldSpeedSlider != null)
            {
                dreamshieldSpeedSlider.value = Mathf.Clamp(speed, dreamshieldSpeedSlider.minValue, dreamshieldSpeedSlider.maxValue);
            }

            UpdateDreamshieldSliderState();
            UpdateDreamshieldInteractivity();
        }

        private void RefreshShowHpOnDeathUi()
        {
            UpdateToggleValue(showHpGlobalValue, ShowHpSettings.EnabledMod);
            UpdateToggleIcon(showHpGlobalIcon, ShowHpSettings.EnabledMod);
            UpdateToggleValue(showHpShowPbValue, ShowHpSettings.ShowPB);
            UpdateToggleValue(showHpAutoHideValue, ShowHpSettings.HideAfter10Sec);

            UpdateKeybindValue(showHpHudToggleKeyValue, GetShowHpBindingLabel());

            float fade = ClampShowHpFade(ShowHpSettings.HudFadeSeconds);
            UpdateFloatValueText(showHpFadeValue, fade, 1);
            if (showHpFadeSlider != null)
            {
                showHpFadeSlider.value = Mathf.Clamp(fade, showHpFadeSlider.minValue, showHpFadeSlider.maxValue);
            }

            UpdateShowHpOnDeathInteractivity();
        }

        private void RefreshMaskDamageUi()
        {
            bool enabled = GetMaskDamageEnabled();
            UpdateToggleValue(maskDamageToggleValue, enabled);
            UpdateToggleIcon(maskDamageToggleIcon, enabled);
            UpdateIntInputValue(maskDamageMultiplierField, MaskDamage.GetMultiplier());
            UpdateKeybindValue(maskDamageToggleUiKeyValue, GetMaskDamageToggleUiKeyLabel());

            UpdateMaskDamageInteractivity();
        }

        private void RefreshFreezeHitboxesUi()
        {
            bool enabled = GetFreezeHitboxesEnabled();
            UpdateToggleValue(freezeHitboxesToggleValue, enabled);
            UpdateToggleIcon(freezeHitboxesToggleIcon, enabled);
            UpdateKeybindValue(freezeHitboxesModeValue, GetFreezeHitboxesModeLabel());
            UpdateKeybindValue(freezeHitboxesUnfreezeKeyValue, GetFreezeHitboxesKeyLabel());

            UpdateFreezeHitboxesInteractivity();
        }

        private void RefreshSpeedChangerUi()
        {
            UpdateToggleValue(speedChangerGlobalValue, SpeedChanger.globalSwitch);
            UpdateToggleIcon(speedChangerGlobalIcon, SpeedChanger.globalSwitch);
            UpdateToggleValue(speedChangerRestrictValue, SpeedChanger.restrictToggleToRooms);
            UpdateToggleValue(speedChangerUnlimitedValue, SpeedChanger.unlimitedSpeed);

            UpdateKeybindValue(speedChangerToggleKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.toggleKeybind));
            UpdateKeybindValue(speedChangerInputKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.inputSpeedKeybind));

            UpdateKeybindValue(speedChangerDisplayValue, GetSpeedChangerDisplayValue());

            UpdateSpeedChangerInteractivity();
        }

        private void RefreshTeleportKitUi()
        {
            Module? module = GetTeleportKitModule();
            UpdateToggleValue(teleportKitToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(teleportKitToggleIcon, module?.Enabled ?? false);
            UpdateKeybindValue(teleportKitMenuKeyValue, GetTeleportKitMenuKeyLabel());
            UpdateKeybindValue(teleportKitSaveKeyValue, GetTeleportKitSaveKeyLabel());
            UpdateKeybindValue(teleportKitTeleportKeyValue, GetTeleportKitTeleportKeyLabel());

            UpdateTeleportKitInteractivity();
        }

        private void RefreshBossChallengeUi()
        {
            UpdateToggleValue(bossChallengeEnableValue, GetBossChallengeMasterEnabled());
            UpdateToggleIcon(bossChallengeEnableIcon, GetBossChallengeMasterEnabled());
            UpdateToggleValue(bossInfiniteChallengeValue, GetInfiniteChallengeEnabled());
            UpdateToggleValue(bossRestartOnSuccessValue, Modules.BossChallenge.InfiniteChallenge.restartFightOnSuccess);
            UpdateToggleValue(bossRestartAndMusicValue, Modules.BossChallenge.InfiniteChallenge.restartFightAndMusic);
            if (qolCarefreeMelodyValue != null)
            {
                qolCarefreeMelodyValue.text = GetCarefreeMelodyModeLabel();
            }
            UpdateToggleValue(bossForceArriveValue, GetForceArriveAnimationEnabled());
            UpdateToggleValue(bossInfiniteGrimmValue, GetInfiniteGrimmPufferfishEnabled());
            UpdateToggleValue(bossInfiniteRadianceValue, GetInfiniteRadianceClimbingEnabled());
            UpdateToggleValue(bossP5HealthValue, GetP5HealthEnabled());
            UpdateToggleValue(bossSegmentedP5Value, GetSegmentedP5Enabled());
            UpdateToggleValue(bossHalveAscendedValue, GetHalveAscendedEnabled());
            UpdateToggleValue(bossHalveAttunedValue, GetHalveAttunedEnabled());
            UpdateToggleValue(bossAddLifebloodValue, GetAddLifebloodEnabled());
            UpdateToggleValue(bossAddSoulValue, GetAddSoulEnabled());

            UpdateIntInputValue(bossLifebloodAmountField, Modules.BossChallenge.AddLifeblood.lifebloodAmount);
            UpdateIntInputValue(bossSoulAmountField, Modules.BossChallenge.AddSoul.soulAmount);

            if (bossGpzValue != null)
            {
                bossGpzValue.text = GetBossGpzLabel();
            }
            UpdateBossChallengeInteractivity();
        }

        private void RefreshRandomPantheonsUi()
        {
            UpdateToggleValue(randomPantheonsToggleValue, GetRandomPantheonsMasterEnabled());
            UpdateToggleIcon(randomPantheonsToggleIcon, GetRandomPantheonsMasterEnabled());
            UpdateToggleValue(randomPantheonsP1Value, GetRandomPantheonsP1Enabled());
            UpdateToggleValue(randomPantheonsP2Value, GetRandomPantheonsP2Enabled());
            UpdateToggleValue(randomPantheonsP3Value, GetRandomPantheonsP3Enabled());
            UpdateToggleValue(randomPantheonsP4Value, GetRandomPantheonsP4Enabled());
            UpdateToggleValue(randomPantheonsP5Value, GetRandomPantheonsP5Enabled());
            UpdateRandomPantheonsInteractivity();
        }

        private void RefreshAlwaysFuriousUi()
        {
            UpdateToggleValue(alwaysFuriousToggleValue, GetAlwaysFuriousEnabled());
            UpdateToggleIcon(alwaysFuriousToggleIcon, GetAlwaysFuriousEnabled());
        }

        private void RefreshQolUi()
        {
            UpdateToggleValue(qolEnableValue, GetQolMasterEnabled());
            UpdateToggleIcon(qolEnableIcon, GetQolMasterEnabled());
            UpdateToggleValue(bossAnimFastDreamWarpValue, GetFastDreamWarpEnabled());
            UpdateKeybindValue(fastDreamWarpKeyValue, GetFastDreamWarpBindingLabel());
            UpdateToggleValue(bossAnimShortDeathValue, GetShortDeathAnimationEnabled());
            UpdateToggleValue(bossAnimHallOfGodsValue, Modules.QoL.SkipCutscenes.HallOfGodsStatues);
            UpdateToggleValue(qolUnlockAllModesValue, GetUnlockAllModesEnabled());
            UpdateToggleValue(qolUnlockPantheonsValue, GetUnlockPantheonsEnabled());
            UpdateToggleValue(qolUnlockRadianceValue, GetUnlockRadianceEnabled());
            UpdateToggleValue(qolUnlockRadiantValue, GetUnlockRadiantEnabled());
            UpdateToggleValue(qolInvincibleIndicatorValue, GetInvincibleIndicatorEnabled());
            UpdateToggleValue(qolScreenShakeValue, GetScreenShakeEnabled());
            UpdateQolInteractivity();
        }

        private void RefreshMenuAnimationUi()
        {
            UpdateToggleValue(menuAnimEnableValue, GetMenuAnimationMasterEnabled());
            UpdateToggleIcon(menuAnimEnableIcon, GetMenuAnimationMasterEnabled());
            UpdateToggleValue(menuAnimDoorDefaultValue, GetDoorDefaultBeginEnabled());
            UpdateToggleValue(menuAnimMemorizeBindingsValue, GetMemorizeBindingsEnabled());
            UpdateToggleValue(menuAnimFasterLoadsValue, GetFasterLoadsEnabled());
            UpdateToggleValue(menuAnimFastMenusValue, GetFastMenusEnabled());
            UpdateToggleValue(menuAnimFastTextValue, GetFastTextEnabled());
            UpdateToggleValue(menuAnimAutoSkipValue, Modules.QoL.SkipCutscenes.AutoSkipCinematics);
            UpdateToggleValue(menuAnimAllowSkippingValue, Modules.QoL.SkipCutscenes.AllowSkippingNonskippable);
            UpdateToggleValue(menuAnimSkipWithoutPromptValue, Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt);
            UpdateMenuAnimationInteractivity();
        }

        private void RefreshBossAnimationUi()
        {
            UpdateToggleValue(bossAnimEnableValue, GetBossAnimationMasterEnabled());
            UpdateToggleIcon(bossAnimEnableIcon, GetBossAnimationMasterEnabled());
            UpdateToggleValue(bossAnimAbsoluteRadianceValue, Modules.QoL.SkipCutscenes.AbsoluteRadiance);
            UpdateToggleValue(bossAnimPureVesselValue, Modules.QoL.SkipCutscenes.PureVesselRoar);
            UpdateToggleValue(bossAnimGrimmNightmareValue, Modules.QoL.SkipCutscenes.GrimmNightmare);
            UpdateToggleValue(bossAnimGreyPrinceValue, Modules.QoL.SkipCutscenes.GreyPrinceZote);
            UpdateToggleValue(bossAnimCollectorValue, Modules.QoL.SkipCutscenes.Collector);
            UpdateToggleValue(bossAnimSoulMasterValue, Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip);
            UpdateBossAnimationInteractivity();
        }

        private void RefreshZoteHelperUi()
        {
            Module? module = GetZoteHelperModule();
            UpdateToggleValue(zoteHelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(zoteHelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(zoteHoGOnlyValue, Modules.BossChallenge.ZoteHelper.ZoteHoGOnly);
            UpdateIntInputValue(zoteBossHpField, Modules.BossChallenge.ZoteHelper.zoteBossHp);
            UpdateToggleValue(zoteImmortalValue, Modules.BossChallenge.ZoteHelper.zoteImmortal);
            UpdateToggleValue(zoteSpawnFlyingValue, Modules.BossChallenge.ZoteHelper.zoteSpawnFlying);
            UpdateToggleValue(zoteSpawnHoppingValue, Modules.BossChallenge.ZoteHelper.zoteSpawnHopping);
            UpdateIntInputValue(zoteSummonFlyingHpField, Modules.BossChallenge.ZoteHelper.zoteSummonFlyingHp);
            UpdateIntInputValue(zoteSummonHoppingHpField, Modules.BossChallenge.ZoteHelper.zoteSummonHoppingHp);
            UpdateIntInputValue(zoteSummonLimitField, Modules.BossChallenge.ZoteHelper.zoteSummonLimit);

            if (bossGpzValue != null)
            {
                bossGpzValue.text = GetBossGpzLabel();
            }

            UpdateZoteHelperInteractivity();
        }

        private void RefreshQuickMenuSettingsUi()
        {
            foreach (KeyValuePair<string, Text> entry in quickSettingsToggleValues)
            {
                UpdateToggleValue(entry.Value, IsQuickMenuItemVisible(entry.Key));
                QuickMenuItemDefinition? def = GetOrderedQuickMenuDefinitions()
                    .FirstOrDefault(item => item.Id == entry.Key);
                if (def != null)
                {
                    Transform? row = entry.Value.transform.parent;
                    Text? label = row?.Find("Label")?.GetComponent<Text>();
                    if (label != null)
                    {
                        label.text = GetQuickMenuSettingsLabel(def);
                    }
                }
            }

            foreach (KeyValuePair<string, Text> entry in quickSettingsHotkeyValues)
            {
                UpdateKeybindValue(entry.Value, GetOverlayHotkeyLabel(entry.Key));
            }

            int opacity = GetQuickMenuOpacity();
            UpdateFloatValueText(quickMenuOpacityValue, opacity, 0);
            if (quickMenuOpacitySlider != null && Math.Abs(quickMenuOpacitySlider.value - opacity) > 0.0001f)
            {
                quickMenuOpacitySlider.value = opacity;
            }
        }

        private void UpdateDreamshieldSliderState()
        {
            bool enabled = Modules.QoL.DreamshieldStartAngle.startAngleEnabled;
            if (dreamshieldDelaySlider != null)
            {
                dreamshieldDelaySlider.interactable = enabled;
            }

            if (dreamshieldSpeedSlider != null)
            {
                dreamshieldSpeedSlider.interactable = enabled;
            }
        }
    }
}
