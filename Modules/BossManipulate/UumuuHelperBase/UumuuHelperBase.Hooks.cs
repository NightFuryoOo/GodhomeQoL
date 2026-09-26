using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class UumuuHelperBase : Module
{
    private protected abstract override void Load();

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresentCore();
        RestoreVanillaSummonHealthIfPresentCore();
        moduleActive = false;
        On.SetHP.OnEnter -= OnSetHpEnter;
        On.HealthManager.OnEnable -= OnHealthManagerOnEnable;
        On.HealthManager.Awake -= OnHealthManagerAwake;
        On.HealthManager.Start -= OnHealthManagerStart;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
        vanillaSummonHpByInstance.Clear();
        hoGEntryAllowed = false;
    }

    private protected void OnSetHpEnter(On.SetHP.orig_OnEnter orig, SetHP self)
    {
        orig(self);

        if (!moduleActive || self == null || !ShouldUseCustomSummonHp())
        {
            return;
        }

        GameObject targetObject = self.target.GetSafe(self);
        if (targetObject == null || !IsBossSummonObject(targetObject) || !ShouldApplySummonSettings(targetObject))
        {
            return;
        }

        HealthManager hm = targetObject.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        RememberVanillaSummonHp(hm);
        ApplyBossSummonHealth(targetObject, hm);
    }

    private protected void OnHealthManagerOnEnable(On.HealthManager.orig_OnEnable orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsBossSummon(self))
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
            ApplyBossSummonHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaSummonHealth(self.gameObject, self);
        }
    }

    private protected void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive)
        {
            return;
        }

        if (IsBoss(self))
        {
            RememberVanillaHp(self);
            if (!ShouldApplySettings(self.gameObject))
            {
                return;
            }

            if (ShouldUseCustomHp())
            {
                ApplyBossHealth(self.gameObject, self);
            }
            else
            {
                RestoreVanillaHealth(self.gameObject, self);
            }

            return;
        }

        if (!IsBossSummon(self))
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
            ApplyBossSummonHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaSummonHealth(self.gameObject, self);
        }
    }

    private protected void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive)
        {
            return;
        }

        if (IsBoss(self))
        {
            RememberVanillaHp(self);
            if (!ShouldApplySettings(self.gameObject))
            {
                return;
            }

            if (ShouldUseCustomHp())
            {
                ApplyBossHealth(self.gameObject, self);
                _ = self.StartCoroutine(DeferredApply(self));
            }
            else
            {
                RestoreVanillaHealth(self.gameObject, self);
            }

            return;
        }

        if (!IsBossSummon(self))
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
            ApplyBossSummonHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaSummonHealth(self.gameObject, self);
        }
    }

    private protected IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null)
        {
            yield break;
        }

        if (IsBoss(hm))
        {
            if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyBossHealth(hm.gameObject, hm);
                yield return new WaitForSeconds(0.01f);
                if (moduleActive && hm != null && hm.gameObject != null && IsBoss(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
                {
                    ApplyBossHealth(hm.gameObject, hm);
                }
            }

            yield break;
        }

        if (!IsBossSummon(hm))
        {
            yield break;
        }

        if (!ShouldApplySummonSettings(hm.gameObject))
        {
            yield break;
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyBossSummonHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsBossSummon(hm) && ShouldUseCustomSummonHp() && ShouldApplySummonSettings(hm.gameObject))
            {
                ApplyBossSummonHealth(hm.gameObject, hm);
            }
        }
        else
        {
            RestoreVanillaSummonHealth(hm.gameObject, hm);
        }
    }

    private protected void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, SceneName, StringComparison.Ordinal))
        {
            vanillaHpByInstance.Clear();
            vanillaSummonHpByInstance.Clear();
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyHealthIfPresentCore();
        }
        else
        {
            RestoreVanillaHealthIfPresentCore();
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplySummonHealthIfPresentCore();
        }
        else
        {
            RestoreVanillaSummonHealthIfPresentCore();
        }
    }

    private protected string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);
        return newSceneName;
    }
}
