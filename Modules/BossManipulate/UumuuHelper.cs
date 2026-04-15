using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class UumuuHelper : Module
{
    private const string UumuuScene = "GG_Uumuu_V";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string UumuuName = "Mega Jellyfish GG";
    private const string UumuuSummonNameHint = "Jellyfish";
    private const string UumuuSpawnerNameHint = "Spawner";
    private const string UumuuMegaNameHint = "Mega Jellyfish";
    private const int DefaultUumuuMaxHp = 700;
    private const int DefaultUumuuSummonHp = 1;
    private const int DefaultUumuuVanillaHp = 700;
    private const int P5UumuuHp = 350;
    private const int MinUumuuHp = 1;
    private const int MaxUumuuHp = 999999;

    [LocalSetting]
    [BoolOption]
    internal static bool uumuuUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool uumuuP5Hp = false;

    [LocalSetting]
    internal static int uumuuMaxHp = DefaultUumuuMaxHp;

    [LocalSetting]
    internal static bool uumuuUseCustomSummonHp = false;

    [LocalSetting]
    internal static int uumuuSummonHp = DefaultUumuuSummonHp;

    [LocalSetting]
    internal static int uumuuMaxHpBeforeP5 = DefaultUumuuMaxHp;

    [LocalSetting]
    internal static bool uumuuUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool uumuuUseCustomSummonHpBeforeP5 = false;

    [LocalSetting]
    internal static int uumuuSummonHpBeforeP5 = DefaultUumuuSummonHp;

    [LocalSetting]
    internal static bool uumuuHasStoredStateBeforeP5 = false;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static readonly Dictionary<int, int> vanillaSummonHpByInstance = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        NormalizeP5State();
        vanillaHpByInstance.Clear();
        vanillaSummonHpByInstance.Clear();
        On.SetHP.OnEnter += OnSetHpEnter;
        On.HealthManager.OnEnable += OnHealthManagerOnEnable;
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresent();
        RestoreVanillaSummonHealthIfPresent();
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

    internal static void ReapplyLiveSettings()
    {
        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyUumuuHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyUumuuSummonHealthIfPresent();
        }
        else
        {
            RestoreVanillaSummonHealthIfPresent();
        }
    }

    internal static void SetP5HpEnabled(bool value)
    {
        if (value)
        {
            if (!uumuuP5Hp)
            {
                uumuuMaxHpBeforeP5 = ClampUumuuHp(uumuuMaxHp);
                uumuuUseMaxHpBeforeP5 = uumuuUseMaxHp;
                uumuuUseCustomSummonHpBeforeP5 = uumuuUseCustomSummonHp;
                uumuuSummonHpBeforeP5 = ClampUumuuHp(uumuuSummonHp);
                uumuuHasStoredStateBeforeP5 = true;
            }

            uumuuP5Hp = true;
            uumuuUseMaxHp = true;
            uumuuUseCustomSummonHp = false;
            uumuuMaxHp = P5UumuuHp;
        }
        else
        {
            if (uumuuP5Hp && uumuuHasStoredStateBeforeP5)
            {
                uumuuMaxHp = ClampUumuuHp(uumuuMaxHpBeforeP5);
                uumuuUseMaxHp = uumuuUseMaxHpBeforeP5;
                uumuuUseCustomSummonHp = uumuuUseCustomSummonHpBeforeP5;
                uumuuSummonHp = ClampUumuuHp(uumuuSummonHpBeforeP5);
            }

            uumuuP5Hp = false;
            uumuuHasStoredStateBeforeP5 = false;
        }

        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!uumuuP5Hp)
        {
            return;
        }

        if (!uumuuHasStoredStateBeforeP5)
        {
            uumuuMaxHpBeforeP5 = ClampUumuuHp(uumuuMaxHp);
            uumuuUseMaxHpBeforeP5 = uumuuUseMaxHp;
            uumuuUseCustomSummonHpBeforeP5 = uumuuUseCustomSummonHp;
            uumuuSummonHpBeforeP5 = ClampUumuuHp(uumuuSummonHp);
            uumuuHasStoredStateBeforeP5 = true;
        }

        uumuuUseMaxHp = true;
        uumuuUseCustomSummonHp = false;
        uumuuMaxHp = P5UumuuHp;
    }

    internal static void ApplyUumuuHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindUumuuHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyUumuuHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindUumuuHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            RestoreVanillaHealth(hm.gameObject, hm);
        }
    }

    internal static void ApplyUumuuSummonHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomSummonHp())
        {
            return;
        }

        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsUumuuSummon(hm))
            {
                continue;
            }

            ApplyUumuuSummonHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaSummonHealthIfPresent()
    {
        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsUumuuSummon(hm))
            {
                continue;
            }

            RestoreVanillaSummonHealth(hm.gameObject, hm);
        }
    }

    private static void OnSetHpEnter(On.SetHP.orig_OnEnter orig, SetHP self)
    {
        orig(self);

        if (!moduleActive || self == null || !ShouldUseCustomSummonHp())
        {
            return;
        }

        GameObject targetObject = self.target.GetSafe(self);
        if (targetObject == null || !IsUumuuSummonObject(targetObject) || !ShouldApplySummonSettings(targetObject))
        {
            return;
        }

        HealthManager hm = targetObject.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        RememberVanillaSummonHp(hm);
        ApplyUumuuSummonHealth(targetObject, hm);
    }

    private static void OnHealthManagerOnEnable(On.HealthManager.orig_OnEnable orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsUumuuSummon(self))
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
            ApplyUumuuSummonHealth(self.gameObject, self);
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

        if (IsUumuu(self))
        {
            RememberVanillaHp(self);
            if (!ShouldApplySettings(self.gameObject))
            {
                return;
            }

            if (ShouldUseCustomHp())
            {
                ApplyUumuuHealth(self.gameObject, self);
            }
            else
            {
                RestoreVanillaHealth(self.gameObject, self);
            }

            return;
        }

        if (!IsUumuuSummon(self))
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
            ApplyUumuuSummonHealth(self.gameObject, self);
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

        if (IsUumuu(self))
        {
            RememberVanillaHp(self);
            if (!ShouldApplySettings(self.gameObject))
            {
                return;
            }

            if (ShouldUseCustomHp())
            {
                ApplyUumuuHealth(self.gameObject, self);
                _ = self.StartCoroutine(DeferredApply(self));
            }
            else
            {
                RestoreVanillaHealth(self.gameObject, self);
            }

            return;
        }

        if (!IsUumuuSummon(self))
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
            ApplyUumuuSummonHealth(self.gameObject, self);
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

        if (IsUumuu(hm))
        {
            if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyUumuuHealth(hm.gameObject, hm);
                yield return new WaitForSeconds(0.01f);
                if (moduleActive && hm != null && hm.gameObject != null && IsUumuu(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
                {
                    ApplyUumuuHealth(hm.gameObject, hm);
                }
            }

            yield break;
        }

        if (!IsUumuuSummon(hm))
        {
            yield break;
        }

        if (!ShouldApplySummonSettings(hm.gameObject))
        {
            yield break;
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyUumuuSummonHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsUumuuSummon(hm) && ShouldUseCustomSummonHp() && ShouldApplySummonSettings(hm.gameObject))
            {
                ApplyUumuuSummonHealth(hm.gameObject, hm);
            }
        }
        else
        {
            RestoreVanillaSummonHealth(hm.gameObject, hm);
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, UumuuScene, StringComparison.Ordinal))
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
            ApplyUumuuHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyUumuuSummonHealthIfPresent();
        }
        else
        {
            RestoreVanillaSummonHealthIfPresent();
        }
    }

    private static string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);
        return newSceneName;
    }

    private static bool IsUumuu(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsUumuuObject(hm.gameObject);
    }

    private static bool IsUumuuSummon(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsUumuuSummonObject(hm.gameObject);
    }

    private static bool IsUumuuObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, UumuuScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(UumuuName, StringComparison.Ordinal);
    }

    private static bool IsUumuuSummonObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        string name = gameObject.name;
        if (name.StartsWith(UumuuName, StringComparison.Ordinal))
        {
            return false;
        }

        if (name.IndexOf(UumuuMegaNameHint, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return false;
        }

        if (name.IndexOf(UumuuSpawnerNameHint, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return false;
        }

        if (name.IndexOf(UumuuSummonNameHint, StringComparison.OrdinalIgnoreCase) < 0)
        {
            return false;
        }

        if (string.Equals(gameObject.scene.name, UumuuScene, StringComparison.Ordinal))
        {
            return true;
        }

        // Summons can be pooled and retain prefab scene metadata.
        return hoGEntryAllowed && string.Equals(USceneManager.GetActiveScene().name, UumuuScene, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsUumuuObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldApplySummonSettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsUumuuSummonObject(gameObject))
        {
            return false;
        }

        if (!hoGEntryAllowed)
        {
            return false;
        }

        string activeScene = USceneManager.GetActiveScene().name;
        return string.Equals(activeScene, UumuuScene, StringComparison.Ordinal)
            || string.Equals(gameObject.scene.name, UumuuScene, StringComparison.Ordinal);
    }

    private static bool ShouldUseCustomHp() => uumuuUseMaxHp;
    private static bool ShouldUseCustomSummonHp() => uumuuUseCustomSummonHp && !uumuuP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, UumuuScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, UumuuScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static void ApplyUumuuHealth(GameObject boss, HealthManager? hm = null)
    {
        if (!ShouldApplySettings(boss) || !ShouldUseCustomHp())
        {
            return;
        }

        hm ??= boss.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        RememberVanillaHp(hm);
        int targetHp = ClampUumuuHp(uumuuMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void ApplyUumuuSummonHealth(GameObject summon, HealthManager? hm = null)
    {
        if (!ShouldApplySummonSettings(summon) || !ShouldUseCustomSummonHp())
        {
            return;
        }

        hm ??= summon.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        RememberVanillaSummonHp(hm);
        int targetHp = ClampUumuuHp(uumuuSummonHp);
        summon.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsUumuuObject(boss))
        {
            return;
        }

        hm ??= boss.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        if (!TryGetVanillaHp(hm, out int vanillaHp))
        {
            return;
        }

        int targetHp = ClampUumuuHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaSummonHealth(GameObject summon, HealthManager? hm = null)
    {
        if (summon == null || !IsUumuuSummonObject(summon))
        {
            return;
        }

        hm ??= summon.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        if (!TryGetVanillaSummonHp(hm, out int vanillaHp))
        {
            return;
        }

        int targetHp = ClampUumuuHp(vanillaHp);
        summon.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindUumuuHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsUumuu(candidate))
            {
                hm = candidate;
                return true;
            }
        }

        return false;
    }

    private static void RememberVanillaHp(HealthManager hm)
    {
        int instanceId = hm.GetInstanceID();
        if (vanillaHpByInstance.ContainsKey(instanceId))
        {
            return;
        }

        int hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultUumuuVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultUumuuVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultUumuuVanillaHp);
        return hp > 0;
    }

    private static void RememberVanillaSummonHp(HealthManager hm)
    {
        int instanceId = hm.GetInstanceID();
        if (vanillaSummonHpByInstance.ContainsKey(instanceId))
        {
            return;
        }

        int hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        if (hp > 0)
        {
            vanillaSummonHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaSummonHp(HealthManager hm, out int hp)
    {
        if (vanillaSummonHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        return hp > 0;
    }

    private static int ReadMaxHp(HealthManager hm)
    {
        try
        {
            int maxHp = ReflectionHelper.GetField<HealthManager, int>(hm, "maxHP");
            if (maxHp > 0)
            {
                return maxHp;
            }
        }
        catch
        {
            // Ignore if field is unavailable.
        }

        return hm.hp;
    }

    private static void TrySetMaxHp(HealthManager hm, int value)
    {
        try
        {
            ReflectionHelper.SetField(hm, "maxHP", value);
        }
        catch
        {
            // Ignore if field is unavailable.
        }
    }

    private static int ClampUumuuHp(int value)
    {
        if (value < MinUumuuHp)
        {
            return MinUumuuHp;
        }

        return value > MaxUumuuHp ? MaxUumuuHp : value;
    }
}
