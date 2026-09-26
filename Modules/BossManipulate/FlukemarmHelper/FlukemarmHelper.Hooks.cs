using Modding;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class FlukemarmHelper : Module
{
    private protected override void Load()
    {
        moduleActive = true;
        BossManipulateEntryGuard.EnsureHooks();
        NormalizeP5State();
        vanillaHpByInstance.Clear();
        vanillaSummonLimitByFsm.Clear();
        customFlukeFlyPoolEntries.Clear();
        vanillaFlukeFlyPoolSize = -1;
        On.SetHP.OnEnter += OnSetHpEnter;
        On.HealthManager.OnEnable += OnHealthManagerEnable;
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
        RestoreVanillaSummonLimitsIfPresent();
        moduleActive = false;
        On.SetHP.OnEnter -= OnSetHpEnter;
        On.HealthManager.OnEnable -= OnHealthManagerEnable;
        On.HealthManager.Awake -= OnHealthManagerAwake;
        On.HealthManager.Start -= OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
        vanillaSummonLimitByFsm.Clear();
        customFlukeFlyPoolEntries.Clear();
        vanillaFlukeFlyPoolSize = -1;
        hoGEntryAllowed = false;
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

    private static void OnHealthManagerEnable(On.HealthManager.orig_OnEnable orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsFlukemarm(self))
        {
            return;
        }

        if (GetFlukemarmTarget(self.gameObject) != FlukemarmTarget.Fly)
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        if (ShouldUseCustomHp(FlukemarmTarget.Fly))
        {
            ApplyFlukemarmHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnSetHpEnter(On.SetHP.orig_OnEnter orig, SetHP self)
    {
        orig(self);

        if (!moduleActive || self == null)
        {
            return;
        }

        GameObject targetObject = self.target.GetSafe(self);
        FlukemarmTarget target = GetFlukemarmTarget(targetObject);
        if (targetObject == null || target != FlukemarmTarget.Fly || !ShouldUseCustomHp(target))
        {
            return;
        }

        HealthManager hm = targetObject.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        RememberVanillaHp(hm);
        if (!ShouldApplySettings(targetObject))
        {
            return;
        }

        ApplyFlukemarmHealth(targetObject, hm);
    }

    private static void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsFlukemarm(self))
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        FlukemarmTarget target = GetFlukemarmTarget(self.gameObject);
        if (ShouldUseCustomHp(target))
        {
            ApplyFlukemarmHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsFlukemarm(self))
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        FlukemarmTarget target = GetFlukemarmTarget(self.gameObject);
        if (ShouldUseCustomHp(target))
        {
            ApplyFlukemarmHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsFlukemarm(hm))
        {
            yield break;
        }

        if (ShouldApplySettings(hm.gameObject))
        {
            FlukemarmTarget target = GetFlukemarmTarget(hm.gameObject);
            if (ShouldUseCustomHp(target))
            {
                ApplyFlukemarmHealth(hm.gameObject, hm);
                yield return new WaitForSeconds(0.01f);
                if (moduleActive && hm != null && hm.gameObject != null && IsFlukemarm(hm) && ShouldApplySettings(hm.gameObject) && ShouldUseCustomHp(GetFlukemarmTarget(hm.gameObject)))
                {
                    ApplyFlukemarmHealth(hm.gameObject, hm);
                }
            }
            else
            {
                RestoreVanillaHealth(hm.gameObject, hm);
            }
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, FlukemarmScene, StringComparison.Ordinal))
        {
            DestroyCustomFlukeFlyPoolEntries();
            vanillaHpByInstance.Clear();
            vanillaSummonLimitByFsm.Clear();
            vanillaFlukeFlyPoolSize = -1;
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        ApplyFlukemarmHealthIfPresent();
        RestoreVanillaHealthIfPresent();
        ApplySummonLimitSettingsIfPresent();
    }

    private static string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);
        return newSceneName;
    }
}
