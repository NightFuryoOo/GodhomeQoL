using Modding;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class HiveKnightHelper : Module
{
    private protected override void Load()
    {
        moduleActive = true;
        BossManipulateEntryGuard.EnsureHooks();
        NormalizeP5State();
        NormalizePhaseThresholdState();
        vanillaHpByInstance.Clear();
        vanillaPhaseThresholdsByFsm.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_HiveKnight;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_HiveKnight;
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
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_HiveKnight;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_HiveKnight;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
        vanillaPhaseThresholdsByFsm.Clear();
        hoGEntryAllowed = false;
    }

    private static void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsHiveKnight(self))
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
            ApplyHiveKnightHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsHiveKnight(self))
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
            ApplyHiveKnightHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnPlayMakerFsmOnEnable_HiveKnight(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsHiveKnightPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static void OnPlayMakerFsmStart_HiveKnight(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsHiveKnightPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
        _ = self.StartCoroutine(DeferredApplyPhaseThresholds(self));
    }

    private static IEnumerator DeferredApplyPhaseThresholds(PlayMakerFSM fsm)
    {
        yield return null;

        if (!moduleActive || fsm == null || fsm.gameObject == null || !IsHiveKnightPhaseControlFsm(fsm))
        {
            yield break;
        }

        ApplyPhaseThresholdSettings(fsm);

        yield return new WaitForSeconds(0.01f);
        if (moduleActive && fsm != null && fsm.gameObject != null && IsHiveKnightPhaseControlFsm(fsm))
        {
            ApplyPhaseThresholdSettings(fsm);
        }
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsHiveKnight(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyHiveKnightHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsHiveKnight(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyHiveKnightHealth(hm.gameObject, hm);
            }
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, HiveKnightScene, StringComparison.Ordinal))
        {
            vanillaHpByInstance.Clear();
            vanillaPhaseThresholdsByFsm.Clear();
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyHiveKnightHealthIfPresent();
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
