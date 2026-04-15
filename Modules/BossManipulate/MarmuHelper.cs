using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class MarmuHelper : Module
{
    private const string MarmuScene = "GG_Ghost_Marmu_V";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string MarmuName = "Ghost Warrior Marmu";
    private const int DefaultMarmuMaxHp = 600;
    private const int DefaultMarmuVanillaHp = 600;
    private const int P5MarmuHp = 416;
    private const int MinMarmuHp = 1;
    private const int MaxMarmuHp = 999999;

    [LocalSetting]
    [BoolOption]
    internal static bool marmuUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool marmuP5Hp = false;

    [LocalSetting]
    internal static int marmuMaxHp = DefaultMarmuMaxHp;

    [LocalSetting]
    internal static int marmuMaxHpBeforeP5 = DefaultMarmuMaxHp;

    [LocalSetting]
    internal static bool marmuUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool marmuHasStoredStateBeforeP5 = false;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        NormalizeP5State();
        vanillaHpByInstance.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresent();
        moduleActive = false;
        On.HealthManager.Awake -= OnHealthManagerAwake;
        On.HealthManager.Start -= OnHealthManagerStart;
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
            ApplyMarmuHealthIfPresent();
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
            if (!marmuP5Hp)
            {
                marmuMaxHpBeforeP5 = ClampMarmuHp(marmuMaxHp);
                marmuUseMaxHpBeforeP5 = marmuUseMaxHp;
                marmuHasStoredStateBeforeP5 = true;
            }

            marmuP5Hp = true;
            marmuUseMaxHp = true;
            marmuMaxHp = P5MarmuHp;
        }
        else
        {
            if (marmuP5Hp && marmuHasStoredStateBeforeP5)
            {
                marmuMaxHp = ClampMarmuHp(marmuMaxHpBeforeP5);
                marmuUseMaxHp = marmuUseMaxHpBeforeP5;
            }

            marmuP5Hp = false;
            marmuHasStoredStateBeforeP5 = false;
        }

        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!marmuP5Hp)
        {
            return;
        }

        if (!marmuHasStoredStateBeforeP5)
        {
            marmuMaxHpBeforeP5 = ClampMarmuHp(marmuMaxHp);
            marmuUseMaxHpBeforeP5 = marmuUseMaxHp;
            marmuHasStoredStateBeforeP5 = true;
        }

        marmuUseMaxHp = true;
        marmuMaxHp = P5MarmuHp;
    }

    internal static void ApplyMarmuHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindMarmuHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyMarmuHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindMarmuHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            RestoreVanillaHealth(hm.gameObject, hm);
        }
    }

    private static void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsMarmu(self))
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
            ApplyMarmuHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsMarmu(self))
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
            ApplyMarmuHealth(self.gameObject, self);
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

        if (!moduleActive || hm == null || hm.gameObject == null || !IsMarmu(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyMarmuHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsMarmu(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyMarmuHealth(hm.gameObject, hm);
            }
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, MarmuScene, StringComparison.Ordinal))
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
            ApplyMarmuHealthIfPresent();
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

    private static bool IsMarmu(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsMarmuObject(hm.gameObject);
    }

    private static bool IsMarmuObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, MarmuScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(MarmuName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsMarmuObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => marmuUseMaxHp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, MarmuScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, MarmuScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static void ApplyMarmuHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampMarmuHp(marmuMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsMarmuObject(boss))
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

        int targetHp = ClampMarmuHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindMarmuHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsMarmu(candidate))
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

        hp = Math.Max(hp, DefaultMarmuVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultMarmuVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultMarmuVanillaHp);
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

    private static int ClampMarmuHp(int value)
    {
        if (value < MinMarmuHp)
        {
            return MinMarmuHp;
        }

        return value > MaxMarmuHp ? MaxMarmuHp : value;
    }
}
