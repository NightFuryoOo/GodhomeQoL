using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class SoulWarriorHelper : Module
{
    private protected override void Load()
    {
        moduleActive = true;
        BossManipulateEntryGuard.EnsureHooks();
        NormalizeP5State();
        vanillaHpByInstance.Clear();
        vanillaSummonHpByInstance.Clear();
        vanillaSummonLimitByFsm.Clear();
        vanillaSummonCountByFsm.Clear();
        vanillaSummonPoolsByFsm.Clear();
        DestroyAllCreatedSummonClones();
        On.SetHP.OnEnter += OnSetHpEnter;
        On.HealthManager.OnEnable += OnHealthManagerOnEnable;
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresent();
        RestoreVanillaSummonHealthIfPresent();
        RestoreVanillaSummonLimitsIfPresent();
        moduleActive = false;
        On.SetHP.OnEnter -= OnSetHpEnter;
        On.HealthManager.OnEnable -= OnHealthManagerOnEnable;
        On.HealthManager.Awake -= OnHealthManagerAwake;
        On.HealthManager.Start -= OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
        vanillaSummonHpByInstance.Clear();
        vanillaSummonLimitByFsm.Clear();
        vanillaSummonCountByFsm.Clear();
        vanillaSummonPoolsByFsm.Clear();
        DestroyAllCreatedSummonClones();
        hoGEntryAllowed = false;
    }

    private static void OnSetHpEnter(On.SetHP.orig_OnEnter orig, SetHP self)
    {
        orig(self);

        if (!moduleActive || self == null || !ShouldUseCustomSummonHp())
        {
            return;
        }

        GameObject targetObject = self.target.GetSafe(self);
        if (targetObject == null || !IsSoulWarriorSummonObject(targetObject) || !ShouldApplySummonSettings(targetObject))
        {
            return;
        }

        HealthManager hm = targetObject.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        RememberVanillaSummonHp(hm);
        ApplySoulWarriorSummonHealth(targetObject, hm);
    }

    private static void OnHealthManagerOnEnable(On.HealthManager.orig_OnEnable orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsSoulWarriorSummon(self))
        {
            return;
        }

        RememberVanillaSummonHp(self);
        if (!ShouldApplySummonSettings(self.gameObject))
        {
            return;
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplySoulWarriorSummonHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaSummonHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive)
        {
            return;
        }

        if (IsSoulWarrior(self))
        {
            RememberVanillaHp(self);
            if (!ShouldApplySettings(self.gameObject))
            {
                return;
            }

            if (ShouldUseCustomHp())
            {
                ApplySoulWarriorHealth(self.gameObject, self);
            }
            else
            {
                RestoreVanillaHealth(self.gameObject, self);
            }

            return;
        }

        if (!IsSoulWarriorSummon(self))
        {
            return;
        }

        RememberVanillaSummonHp(self);
        if (!ShouldApplySummonSettings(self.gameObject))
        {
            return;
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplySoulWarriorSummonHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaSummonHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive)
        {
            return;
        }

        if (IsSoulWarrior(self))
        {
            RememberVanillaHp(self);
            if (!ShouldApplySettings(self.gameObject))
            {
                return;
            }

            if (ShouldUseCustomHp())
            {
                ApplySoulWarriorHealth(self.gameObject, self);
                _ = self.StartCoroutine(DeferredApply(self));
            }
            else
            {
                RestoreVanillaHealth(self.gameObject, self);
            }

            ApplySummonLimitSettingsIfPresent();
            return;
        }

        if (!IsSoulWarriorSummon(self))
        {
            return;
        }

        RememberVanillaSummonHp(self);
        if (!ShouldApplySummonSettings(self.gameObject))
        {
            return;
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplySoulWarriorSummonHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaSummonHealth(self.gameObject, self);
        }
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null)
        {
            yield break;
        }

        if (IsSoulWarrior(hm))
        {
            if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplySoulWarriorHealth(hm.gameObject, hm);
                yield return new WaitForSeconds(0.01f);
                if (moduleActive && hm != null && hm.gameObject != null && IsSoulWarrior(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
                {
                    ApplySoulWarriorHealth(hm.gameObject, hm);
                }
            }

            ApplySummonLimitSettingsIfPresent();
            yield break;
        }

        if (!IsSoulWarriorSummon(hm))
        {
            yield break;
        }

        if (!ShouldApplySummonSettings(hm.gameObject))
        {
            yield break;
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplySoulWarriorSummonHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsSoulWarriorSummon(hm) && ShouldUseCustomSummonHp() && ShouldApplySummonSettings(hm.gameObject))
            {
                ApplySoulWarriorSummonHealth(hm.gameObject, hm);
            }
        }
        else
        {
            RestoreVanillaSummonHealth(hm.gameObject, hm);
        }
    }

    private static void OnPlayMakerFsmOnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null)
        {
            return;
        }

        ApplySummonLimitSettings(self);
    }

    private static void OnPlayMakerFsmStart(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null)
        {
            return;
        }

        ApplySummonLimitSettings(self);
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, SoulWarriorScene, StringComparison.Ordinal))
        {
            vanillaHpByInstance.Clear();
            vanillaSummonHpByInstance.Clear();
            vanillaSummonLimitByFsm.Clear();
            vanillaSummonCountByFsm.Clear();
            vanillaSummonPoolsByFsm.Clear();
            DestroyAllCreatedSummonClones();
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplySoulWarriorHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplySoulWarriorSummonHealthIfPresent();
        }
        else
        {
            RestoreVanillaSummonHealthIfPresent();
        }

        if (ShouldUseCustomSummonLimit())
        {
            ApplySummonLimitSettingsIfPresent();
        }
        else
        {
            RestoreVanillaSummonLimitsIfPresent();
        }
    }

    private static string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);
        return newSceneName;
    }
}
