using System;
using System.Collections.Generic;
using InControl;
using UnityEngine;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private static Key? GetFastDreamWarpKeyBinding()
        {
            InputHandler.KeyOrMouseBinding binding = KeybindUtil.GetKeyOrMouseBinding(FastDreamWarpSettings.Keybinds.Toggle);
            if (TryGetBindingKey(binding, out Key key) && !EqualityComparer<Key>.Default.Equals(key, default))
            {
                return key;
            }

            return null;
        }

        private Sprite? GetToggleIconSprite(bool isOn)
        {
            if (isOn)
            {
                toggleShadeSprite ??= LoadCollectorIconSprite("Shade.png", "ShadeToggle");
                if (toggleShadeSprite != null)
                {
                    return toggleShadeSprite;
                }
            }

            quickHandleSprite ??= LoadQuickHandleSprite();
            return quickHandleSprite;
        }

        private Module? GetFastSuperDashModule()
        {
            return GetCachedModule(ref fastSuperDashModule, typeof(Modules.QoL.FastSuperDash));
        }

        private Module? GetCollectorPhasesModule()
        {
            return GetCachedModule(ref collectorPhasesModule, typeof(Modules.CollectorPhases.CollectorPhases));
        }

        private Module? GetFastReloadModule()
        {
            return GetCachedModule(ref fastReloadModule, typeof(Modules.FastReload));
        }

        private SpeedChanger? GetSpeedChangerModule()
        {
            if (speedChangerModule == null)
            {
                ModuleManager.TryGetModule(typeof(SpeedChanger), out speedChangerModule);
            }

            return speedChangerModule as SpeedChanger ?? SpeedChanger.Instance;
        }

        private Module? GetTeleportKitModule()
        {
            return GetCachedModule(ref teleportKitModule, typeof(Modules.QoL.TeleportKit));
        }

        private Module? GetFpsBoostModule()
        {
            if (fpsBoostModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.Performance.FpsBoost), out fpsBoostModule);
            }

            return fpsBoostModule;
        }

        private Module? GetZoteHelperModule()
        {
            return GetCachedModule(ref zoteHelperModule, typeof(Modules.BossChallenge.ZoteHelper));
        }

        private Module? GetGruzMotherHelperModule()
        {
            return GetCachedModule(ref gruzMotherHelperModule, typeof(Modules.BossChallenge.GruzMotherHelper));
        }

        private Module? GetHornetProtectorHelperModule()
        {
            return GetCachedModule(ref hornetProtectorHelperModule, typeof(Modules.BossChallenge.HornetProtectorHelper));
        }

        private Module? GetBroodingMawlekHelperModule()
        {
            return GetCachedModule(ref broodingMawlekHelperModule, typeof(Modules.BossChallenge.BroodingMawlekHelper));
        }

        private Module? GetMassiveMossChargerHelperModule()
        {
            return GetCachedModule(ref massiveMossChargerHelperModule, typeof(Modules.BossChallenge.MassiveMossChargerHelper));
        }

        private Module? GetCrystalGuardianHelperModule()
        {
            return GetCachedModule(ref crystalGuardianHelperModule, typeof(Modules.BossChallenge.CrystalGuardianHelper));
        }

        private Module? GetEnragedGuardianHelperModule()
        {
            return GetCachedModule(ref enragedGuardianHelperModule, typeof(Modules.BossChallenge.EnragedGuardianHelper));
        }

        private Module? GetHornetSentinelHelperModule()
        {
            return GetCachedModule(ref hornetSentinelHelperModule, typeof(Modules.BossChallenge.HornetSentinelHelper));
        }

        private Module? GetInfiniteChallengeModule()
        {
            return GetCachedModule(ref infiniteChallengeModule, typeof(Modules.BossChallenge.InfiniteChallenge));
        }

        private Module? GetRandomPantheonsModule()
        {
            return GetCachedModule(ref randomPantheonsModule, typeof(Modules.BossChallenge.RandomPantheons));
        }

        private Module? GetTrueBossRushModule()
        {
            return GetCachedModule(ref trueBossRushModule, typeof(Modules.BossChallenge.TrueBossRush));
        }

        private Module? GetCheatsModule()
        {
            return GetCachedModule(ref cheatsModule, typeof(Modules.Cheats.Cheats));
        }

        private Module? GetAlwaysFuriousModule()
        {
            return GetCachedModule(ref alwaysFuriousModule, typeof(Modules.BossChallenge.AlwaysFurious));
        }

        private Module? GetInfiniteGrimmPufferfishModule()
        {
            return GetCachedModule(ref infiniteGrimmModule, typeof(Modules.BossChallenge.InfiniteGrimmPufferfish));
        }

        private Module? GetInfiniteRadianceClimbingModule()
        {
            return GetCachedModule(ref infiniteRadianceModule, typeof(Modules.BossChallenge.InfiniteRadianceClimbing));
        }

        private Module? GetForceArriveAnimationModule()
        {
            return GetCachedModule(ref forceArriveAnimationModule, typeof(Modules.BossChallenge.ForceArriveAnimation));
        }

        private Module? GetSegmentedP5Module()
        {
            return GetCachedModule(ref segmentedP5Module, typeof(Modules.BossChallenge.SegmentedP5));
        }

        private Module? GetAddLifebloodModule()
        {
            return GetCachedModule(ref addLifebloodModule, typeof(Modules.BossChallenge.AddLifeblood));
        }

        private Module? GetAddSoulModule()
        {
            return GetCachedModule(ref addSoulModule, typeof(Modules.BossChallenge.AddSoul));
        }

        private Module? GetForceGreyPrinceModule()
        {
            return GetCachedModule(ref forceGreyPrinceModule, typeof(Modules.BossChallenge.ForceGreyPrinceEnterType));
        }

        private Module? GetCollectorRoarModule()
        {
            return GetCachedModule(ref collectorRoarModule, typeof(Modules.QoL.CollectorRoarMute));
        }

        private Module? GetUnlockAllModesModule()
        {
            return GetCachedModule(ref unlockAllModesModule, typeof(Modules.QoL.UnlockAllModes));
        }

        private Module? GetUnlockPantheonsModule()
        {
            return GetCachedModule(ref unlockPantheonsModule, typeof(Modules.QoL.UnlockPantheons));
        }

        private Module? GetUnlockRadianceModule()
        {
            return GetCachedModule(ref unlockRadianceModule, typeof(Modules.QoL.UnlockRadiance));
        }

        private Module? GetUnlockRadiantModule()
        {
            return GetCachedModule(ref unlockRadiantModule, typeof(Modules.QoL.UnlockRadiant));
        }

        private Module? GetDoorDefaultBeginModule()
        {
            return GetCachedModule(ref doorDefaultBeginModule, typeof(Modules.QoL.DoorDefaultBegin));
        }

        private Module? GetFasterLoadsModule()
        {
            return GetCachedModule(ref fasterLoadsModule, typeof(Modules.QoL.FasterLoads));
        }

        private Module? GetFastMenusModule()
        {
            return GetCachedModule(ref fastMenusModule, typeof(Modules.QoL.FastMenus));
        }

        private Module? GetFastTextModule()
        {
            return GetCachedModule(ref fastTextModule, typeof(Modules.QoL.FastText));
        }

        private Module? GetFastDreamWarpModule()
        {
            return GetCachedModule(ref fastDreamWarpModule, typeof(Modules.QoL.FastDreamWarp));
        }

        private Module? GetShortDeathAnimationModule()
        {
            return GetCachedModule(ref shortDeathAnimationModule, typeof(Modules.QoL.ShortDeathAnimation));
        }

        private Module? GetInvincibleIndicatorModule()
        {
            return GetCachedModule(ref invincibleIndicatorModule, typeof(Modules.QoL.InvincibleIndicator));
        }

        private Module? GetScreenShakeModule()
        {
            return GetCachedModule(ref screenShakeModule, typeof(Modules.QoL.ScreenShake));
        }

        private static void SetModuleEnabled<T>(bool value) where T : Module
        {
            if (ModuleManager.TryGetModule(typeof(T), out Module? module))
            {
                module.Enabled = value;
                return;
            }

            string name = typeof(T).Name;
            if (Setting.Global.Modules.ContainsKey(name))
            {
                Setting.Global.Modules[name] = value;
            }
        }

        private static Sprite? GetBindingDefaultSprite<TBinding>() where TBinding : ToggleableBindings.Binding
        {
            try
            {
                if (ToggleableBindings.BindingManager.TryGetBinding<TBinding>(out TBinding? binding) && binding != null)
                {
                    return binding.DefaultSprite;
                }
            }
            catch (Exception e)
            {
                LogDebug($"QuickMenu: failed to load binding sprite - {e.Message}");
            }

            return null;
        }

        private static Sprite? GetBindingSelectedSprite<TBinding>() where TBinding : ToggleableBindings.Binding
        {
            try
            {
                if (ToggleableBindings.BindingManager.TryGetBinding<TBinding>(out TBinding? binding) && binding != null)
                {
                    return binding.SelectedSprite;
                }
            }
            catch (Exception e)
            {
                LogDebug($"QuickMenu: failed to load binding selected sprite - {e.Message}");
            }

            return null;
        }

        private static ShowHPOnDeathSettings ShowHpSettings =>
            GodhomeQoL.GlobalSettings.ShowHPOnDeath ??= new ShowHPOnDeathSettings();

        private static FastDreamWarpSettings FastDreamWarpSettings =>
            GodhomeQoL.GlobalSettings.FastDreamWarp ??= new FastDreamWarpSettings();
    }
}
