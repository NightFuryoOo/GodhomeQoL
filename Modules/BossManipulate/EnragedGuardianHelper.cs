using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class EnragedGuardianHelper : Module
{
    private const string EnragedGuardianScene = "GG_Crystal_Guardian_2";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string EnragedGuardianName = "Zombie Beam Miner Rematch";
    private const int DefaultEnragedGuardianMaxHp = 1250;
    private const int DefaultEnragedGuardianVanillaHp = 1250;
    private const int P5EnragedGuardianHp = 650;
    private const int MinEnragedGuardianHp = 1;
    private const int MaxEnragedGuardianHp = 999999;

    [LocalSetting]
    [BoolOption]
    internal static bool enragedGuardianUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool enragedGuardianP5Hp = false;

    [LocalSetting]
    internal static int enragedGuardianMaxHp = DefaultEnragedGuardianMaxHp;

    [LocalSetting]
    internal static int enragedGuardianMaxHpBeforeP5 = DefaultEnragedGuardianMaxHp;

    [LocalSetting]
    internal static bool enragedGuardianUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool enragedGuardianHasStoredStateBeforeP5 = false;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        NormalizeP5State();
        vanillaHpByInstance.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake_EnragedGuardian;
        On.HealthManager.Start += OnHealthManagerStart_EnragedGuardian;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresent();
        moduleActive = false;
        On.HealthManager.Awake -= OnHealthManagerAwake_EnragedGuardian;
        On.HealthManager.Start -= OnHealthManagerStart_EnragedGuardian;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
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
            ApplyEnragedGuardianHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }
    }

    internal static void SetP5HpEnabled(bool value)
    {
        if (value)
        {
            if (!enragedGuardianP5Hp)
            {
                enragedGuardianMaxHpBeforeP5 = ClampEnragedGuardianHp(enragedGuardianMaxHp);
                enragedGuardianUseMaxHpBeforeP5 = enragedGuardianUseMaxHp;
                enragedGuardianHasStoredStateBeforeP5 = true;
            }

            enragedGuardianP5Hp = true;
            enragedGuardianUseMaxHp = true;
            enragedGuardianMaxHp = P5EnragedGuardianHp;
        }
        else
        {
            if (enragedGuardianP5Hp && enragedGuardianHasStoredStateBeforeP5)
            {
                enragedGuardianMaxHp = ClampEnragedGuardianHp(enragedGuardianMaxHpBeforeP5);
                enragedGuardianUseMaxHp = enragedGuardianUseMaxHpBeforeP5;
            }

            enragedGuardianP5Hp = false;
            enragedGuardianHasStoredStateBeforeP5 = false;
        }

        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!enragedGuardianP5Hp)
        {
            return;
        }

        if (!enragedGuardianHasStoredStateBeforeP5)
        {
            enragedGuardianMaxHpBeforeP5 = ClampEnragedGuardianHp(enragedGuardianMaxHp);
            enragedGuardianUseMaxHpBeforeP5 = enragedGuardianUseMaxHp;
            enragedGuardianHasStoredStateBeforeP5 = true;
        }

        enragedGuardianUseMaxHp = true;
        enragedGuardianMaxHp = P5EnragedGuardianHp;
    }

    internal static void ApplyEnragedGuardianHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindEnragedGuardianHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyEnragedGuardianHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindEnragedGuardianHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            RestoreVanillaHealth(hm.gameObject, hm);
        }
    }

    private static void OnHealthManagerAwake_EnragedGuardian(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsEnragedGuardian(self))
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
            ApplyEnragedGuardianHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart_EnragedGuardian(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsEnragedGuardian(self))
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
            ApplyEnragedGuardianHealth(self.gameObject, self);
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

        if (!moduleActive || hm == null || hm.gameObject == null || !IsEnragedGuardian(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyEnragedGuardianHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsEnragedGuardian(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyEnragedGuardianHealth(hm.gameObject, hm);
            }
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, EnragedGuardianScene, StringComparison.Ordinal))
        {
            vanillaHpByInstance.Clear();
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyEnragedGuardianHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }
    }

    private static string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);
        return newSceneName;
    }

    private static bool IsEnragedGuardian(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsEnragedGuardianObject(hm.gameObject);
    }

    private static bool IsEnragedGuardianObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, EnragedGuardianScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(EnragedGuardianName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsEnragedGuardianObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => enragedGuardianUseMaxHp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, EnragedGuardianScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, EnragedGuardianScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static void ApplyEnragedGuardianHealth(GameObject mawlek, HealthManager? hm = null)
    {
        if (!ShouldApplySettings(mawlek) || !ShouldUseCustomHp())
        {
            return;
        }

        hm ??= mawlek.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        RememberVanillaHp(hm);
        int targetHp = ClampEnragedGuardianHp(enragedGuardianMaxHp);
        mawlek.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject mawlek, HealthManager? hm = null)
    {
        if (mawlek == null || !IsEnragedGuardianObject(mawlek))
        {
            return;
        }

        hm ??= mawlek.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        if (!TryGetVanillaHp(hm, out int vanillaHp))
        {
            return;
        }

        int targetHp = ClampEnragedGuardianHp(vanillaHp);
        mawlek.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindEnragedGuardianHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsEnragedGuardian(candidate))
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

        hp = Math.Max(hp, DefaultEnragedGuardianVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultEnragedGuardianVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultEnragedGuardianVanillaHp);
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

    private static int ClampEnragedGuardianHp(int value)
    {
        if (value < MinEnragedGuardianHp)
        {
            return MinEnragedGuardianHp;
        }

        return value > MaxEnragedGuardianHp ? MaxEnragedGuardianHp : value;
    }
}


