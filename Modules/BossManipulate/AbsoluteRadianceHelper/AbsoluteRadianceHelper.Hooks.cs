using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class AbsoluteRadianceHelper : Module
{
    private protected override void Load()
    {
        moduleActive = true;
        BossManipulateEntryGuard.EnsureHooks();
        NormalizeP5State();
        NormalizeCustomPhaseThresholdState();
        vanillaHpByInstance.Clear();
        vanillaPhaseThresholdsByFsm.Clear();
        vanillaFinalPhaseHpByFsm.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_AbsoluteRadiance;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_AbsoluteRadiance;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresent();
        RestoreVanillaPhaseThresholdsIfPresent();
        moduleActive = false;
        On.HealthManager.Awake -= OnHealthManagerAwake;
        On.HealthManager.Start -= OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_AbsoluteRadiance;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_AbsoluteRadiance;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
        vanillaPhaseThresholdsByFsm.Clear();
        vanillaFinalPhaseHpByFsm.Clear();
        hoGEntryAllowed = false;
    }

    private static void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsAbsoluteRadiance(self))
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyAbsoluteRadianceHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsAbsoluteRadiance(self))
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyAbsoluteRadianceHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnPlayMakerFsmOnEnable_AbsoluteRadiance(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null)
        {
            return;
        }

        if (IsAbsoluteRadiancePhaseControlFsm(self))
        {
            ApplyPhaseThresholdSettings(self);
        }

        if (IsAbsoluteRadianceControlFsm(self))
        {
            ApplyFinalPhaseHpSetting(self);
        }
    }

    private static void OnPlayMakerFsmStart_AbsoluteRadiance(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null)
        {
            return;
        }

        if (IsAbsoluteRadiancePhaseControlFsm(self))
        {
            ApplyPhaseThresholdSettings(self);
        }

        if (IsAbsoluteRadianceControlFsm(self))
        {
            ApplyFinalPhaseHpSetting(self);
        }
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsAbsoluteRadiance(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyAbsoluteRadianceHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsAbsoluteRadiance(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyAbsoluteRadianceHealth(hm.gameObject, hm);
            }
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, AbsoluteRadianceScene, StringComparison.Ordinal))
        {
            vanillaHpByInstance.Clear();
            vanillaPhaseThresholdsByFsm.Clear();
            vanillaFinalPhaseHpByFsm.Clear();
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyAbsoluteRadianceHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    private static string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);
        return newSceneName;
    }
}
