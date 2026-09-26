using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class ZoteHelper : Module
{
    private protected override void Load()
    {
        moduleActive = true;
        BossManipulateEntryGuard.EnsureHooks();
        ZoteHoGOnly = true;
        activeZotelings.Clear();
        On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
        On.HealthManager.TakeDamage += OnGreyPrinceTakeDamage;
        On.HealthManager.Update += OnHealthManagerUpdate;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        moduleActive = false;
        On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
        On.HealthManager.TakeDamage -= OnGreyPrinceTakeDamage;
        On.HealthManager.Update -= OnHealthManagerUpdate;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        activeZotelings.Clear();
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        if (to.name != ZoteScene)
        {
            activeZotelings.Clear();
            return;
        }

        UpdateHoGEntryAllowed(from.name, to.name);

        activeZotelings.Clear();
        if (hoGEntryAllowed)
        {
            ApplyBossHealthIfPresent();
        }
    }

    private static string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);

        return newSceneName;
    }

    private static bool ShouldApplySettings(GameObject? go)
    {
        if (go == null)
        {
            return false;
        }

        return hoGEntryAllowed
            && string.Equals(go.scene.name, ZoteScene, StringComparison.Ordinal);
    }

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, ZoteScene, StringComparison.Ordinal))
        {
            if (BossManipulateEntryGuard.IsAllowedBossEntry(currentScene, nextScene))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, ZoteScene, StringComparison.Ordinal) && hoGEntryAllowed)
            {
                hoGEntryAllowed = true;
            }
            else
            {
                hoGEntryAllowed = false;
            }

            return;
        }

        hoGEntryAllowed = false;
    }

    private static void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        if (self.gameObject.scene.name == ZoteScene
            && self.gameObject.name == GreyPrinceName
            && self.FsmName == "Control")
        {
            ApplyGreyPrinceChanges(self);
        }
        else if (self.gameObject.scene.name == ZoteScene
            && IsZotelingObject(self.gameObject)
            && self.FsmName == "Control")
        {
            ApplyZotelingChanges(self);
        }
    }

    private static void OnGreyPrinceTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        if (!moduleActive || !ShouldApplySettings(self.gameObject) || !zoteImmortal || !IsGreyPrince(self))
        {
            orig(self, hitInstance);
            return;
        }

        if (self.hp <= 1)
        {
            hitInstance.DamageDealt = 0;
        }
        else
        {
            float multiplier = hitInstance.Multiplier > 0f ? hitInstance.Multiplier : 1f;
            int projectedDamage = Mathf.RoundToInt(hitInstance.DamageDealt * multiplier);
            if (projectedDamage >= self.hp)
            {
                hitInstance.DamageDealt = Mathf.Max(0, Mathf.FloorToInt((self.hp - 1) / multiplier));
            }
        }

        orig(self, hitInstance);
    }

    private static void OnHealthManagerUpdate(On.HealthManager.orig_Update orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive)
        {
            return;
        }

        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        if (!zoteImmortal || !IsGreyPrince(self))
        {
            return;
        }

        int targetHp = GetImmortalBossTargetHp(self);
        if (self.hp != targetHp)
        {
            if (zoteUseCustomBossHp)
            {
                ApplyBossHealth(self.gameObject, self, targetHp);
            }
            else
            {
                self.hp = targetHp;
            }
        }
    }

    internal static void ReapplyLiveSettings()
    {
        if (!moduleActive)
        {
            return;
        }

        ApplyBossHealthIfPresent();
        ApplyZotelingHealthIfPresent();
    }
}
