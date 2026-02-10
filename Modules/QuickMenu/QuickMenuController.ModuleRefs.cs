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
            if (fastSuperDashModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.FastSuperDash), out fastSuperDashModule);
            }

            return fastSuperDashModule;
        }

        private Module? GetCollectorPhasesModule()
        {
            if (collectorPhasesModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.CollectorPhases.CollectorPhases), out collectorPhasesModule);
            }

            return collectorPhasesModule;
        }

        private Module? GetFastReloadModule()
        {
            if (fastReloadModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.FastReload), out fastReloadModule);
            }

            return fastReloadModule;
        }

        private Module? GetMaskDamageModule()
        {
            if (maskDamageModule == null)
            {
                ModuleManager.TryGetModule(typeof(MaskDamage), out maskDamageModule);
            }

            return maskDamageModule;
        }

        private Module? GetFreezeHitboxesModule()
        {
            if (freezeHitboxesModule == null)
            {
                ModuleManager.TryGetModule(typeof(FreezeHitboxes), out freezeHitboxesModule);
            }

            return freezeHitboxesModule;
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
            if (teleportKitModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.TeleportKit), out teleportKitModule);
            }

            return teleportKitModule;
        }

        private Module? GetZoteHelperModule()
        {
            if (zoteHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.ZoteHelper), out zoteHelperModule);
            }

            return zoteHelperModule;
        }

        private Module? GetInfiniteChallengeModule()
        {
            if (infiniteChallengeModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.InfiniteChallenge), out infiniteChallengeModule);
            }

            return infiniteChallengeModule;
        }

        private Module? GetRandomPantheonsModule()
        {
            if (randomPantheonsModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.RandomPantheons), out randomPantheonsModule);
            }

            return randomPantheonsModule;
        }

        private Module? GetAlwaysFuriousModule()
        {
            if (alwaysFuriousModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.AlwaysFurious), out alwaysFuriousModule);
            }

            return alwaysFuriousModule;
        }

        private Module? GetInfiniteGrimmPufferfishModule()
        {
            if (infiniteGrimmModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.InfiniteGrimmPufferfish), out infiniteGrimmModule);
            }

            return infiniteGrimmModule;
        }

        private Module? GetInfiniteRadianceClimbingModule()
        {
            if (infiniteRadianceModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.InfiniteRadianceClimbing), out infiniteRadianceModule);
            }

            return infiniteRadianceModule;
        }

        private Module? GetForceArriveAnimationModule()
        {
            if (forceArriveAnimationModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.ForceArriveAnimation), out forceArriveAnimationModule);
            }

            return forceArriveAnimationModule;
        }

        private Module? GetP5HealthModule()
        {
            if (p5HealthModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.P5Health), out p5HealthModule);
            }

            return p5HealthModule;
        }

        private Module? GetSegmentedP5Module()
        {
            if (segmentedP5Module == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.SegmentedP5), out segmentedP5Module);
            }

            return segmentedP5Module;
        }

        private Module? GetHalveAscendedModule()
        {
            if (halveAscendedModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.HalveDamageHoGAscendedOrAbove), out halveAscendedModule);
            }

            return halveAscendedModule;
        }

        private Module? GetHalveAttunedModule()
        {
            if (halveAttunedModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.HalveDamageHoGAttuned), out halveAttunedModule);
            }

            return halveAttunedModule;
        }

        private Module? GetAddLifebloodModule()
        {
            if (addLifebloodModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.AddLifeblood), out addLifebloodModule);
            }

            return addLifebloodModule;
        }

        private Module? GetAddSoulModule()
        {
            if (addSoulModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.AddSoul), out addSoulModule);
            }

            return addSoulModule;
        }

        private Module? GetForceGreyPrinceModule()
        {
            if (forceGreyPrinceModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.ForceGreyPrinceEnterType), out forceGreyPrinceModule);
            }

            return forceGreyPrinceModule;
        }

        private Module? GetCarefreeMelodyModule()
        {
            if (carefreeMelodyModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.CarefreeMelodyReset), out carefreeMelodyModule);
            }

            return carefreeMelodyModule;
        }

        private Module? GetCollectorRoarModule()
        {
            if (collectorRoarModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.CollectorRoarMute), out collectorRoarModule);
            }

            return collectorRoarModule;
        }

        private Module? GetUnlockAllModesModule()
        {
            if (unlockAllModesModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.UnlockAllModes), out unlockAllModesModule);
            }

            return unlockAllModesModule;
        }

        private Module? GetUnlockPantheonsModule()
        {
            if (unlockPantheonsModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.UnlockPantheons), out unlockPantheonsModule);
            }

            return unlockPantheonsModule;
        }

        private Module? GetUnlockRadianceModule()
        {
            if (unlockRadianceModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.UnlockRadiance), out unlockRadianceModule);
            }

            return unlockRadianceModule;
        }

        private Module? GetUnlockRadiantModule()
        {
            if (unlockRadiantModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.UnlockRadiant), out unlockRadiantModule);
            }

            return unlockRadiantModule;
        }

        private Module? GetDoorDefaultBeginModule()
        {
            if (doorDefaultBeginModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.DoorDefaultBegin), out doorDefaultBeginModule);
            }

            return doorDefaultBeginModule;
        }

        private Module? GetMemorizeBindingsModule()
        {
            if (memorizeBindingsModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.MemorizeBindings), out memorizeBindingsModule);
            }

            return memorizeBindingsModule;
        }

        private Module? GetFasterLoadsModule()
        {
            if (fasterLoadsModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.FasterLoads), out fasterLoadsModule);
            }

            return fasterLoadsModule;
        }

        private Module? GetFastMenusModule()
        {
            if (fastMenusModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.FastMenus), out fastMenusModule);
            }

            return fastMenusModule;
        }

        private Module? GetFastTextModule()
        {
            if (fastTextModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.FastText), out fastTextModule);
            }

            return fastTextModule;
        }

        private Module? GetFastDreamWarpModule()
        {
            if (fastDreamWarpModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.FastDreamWarp), out fastDreamWarpModule);
            }

            return fastDreamWarpModule;
        }

        private Module? GetShortDeathAnimationModule()
        {
            if (shortDeathAnimationModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.ShortDeathAnimation), out shortDeathAnimationModule);
            }

            return shortDeathAnimationModule;
        }

        private Module? GetInvincibleIndicatorModule()
        {
            if (invincibleIndicatorModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.InvincibleIndicator), out invincibleIndicatorModule);
            }

            return invincibleIndicatorModule;
        }

        private Module? GetScreenShakeModule()
        {
            if (screenShakeModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.ScreenShake), out screenShakeModule);
            }

            return screenShakeModule;
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
