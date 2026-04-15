using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class CrystalGuardianHelper : Module
{
    private const string CrystalGuardianScene = "GG_Crystal_Guardian";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string CrystalGuardianName = "Mega Zombie Beam Miner (1)";
    private const int DefaultCrystalGuardianMaxHp = 900;
    private const int DefaultCrystalGuardianVanillaHp = 900;
    private const int P5CrystalGuardianHp = 650;
    private const int MinCrystalGuardianHp = 1;
    private const int MaxCrystalGuardianHp = 999999;

    [LocalSetting]
    [BoolOption]
    internal static bool crystalGuardianUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool crystalGuardianP5Hp = false;

    [LocalSetting]
    internal static int crystalGuardianMaxHp = DefaultCrystalGuardianMaxHp;

    [LocalSetting]
    internal static int crystalGuardianMaxHpBeforeP5 = DefaultCrystalGuardianMaxHp;

    [LocalSetting]
    internal static bool crystalGuardianUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool crystalGuardianHasStoredStateBeforeP5 = false;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        NormalizeP5State();
        vanillaHpByInstance.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake_CrystalGuardian;
        On.HealthManager.Start += OnHealthManagerStart_CrystalGuardian;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresent();
        moduleActive = false;
        On.HealthManager.Awake -= OnHealthManagerAwake_CrystalGuardian;
        On.HealthManager.Start -= OnHealthManagerStart_CrystalGuardian;
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
            ApplyCrystalGuardianHealthIfPresent();
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
            if (!crystalGuardianP5Hp)
            {
                crystalGuardianMaxHpBeforeP5 = ClampCrystalGuardianHp(crystalGuardianMaxHp);
                crystalGuardianUseMaxHpBeforeP5 = crystalGuardianUseMaxHp;
                crystalGuardianHasStoredStateBeforeP5 = true;
            }

            crystalGuardianP5Hp = true;
            crystalGuardianUseMaxHp = true;
            crystalGuardianMaxHp = P5CrystalGuardianHp;
        }
        else
        {
            if (crystalGuardianP5Hp && crystalGuardianHasStoredStateBeforeP5)
            {
                crystalGuardianMaxHp = ClampCrystalGuardianHp(crystalGuardianMaxHpBeforeP5);
                crystalGuardianUseMaxHp = crystalGuardianUseMaxHpBeforeP5;
            }

            crystalGuardianP5Hp = false;
            crystalGuardianHasStoredStateBeforeP5 = false;
        }

        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!crystalGuardianP5Hp)
        {
            return;
        }

        if (!crystalGuardianHasStoredStateBeforeP5)
        {
            crystalGuardianMaxHpBeforeP5 = ClampCrystalGuardianHp(crystalGuardianMaxHp);
            crystalGuardianUseMaxHpBeforeP5 = crystalGuardianUseMaxHp;
            crystalGuardianHasStoredStateBeforeP5 = true;
        }

        crystalGuardianUseMaxHp = true;
        crystalGuardianMaxHp = P5CrystalGuardianHp;
    }

    internal static void ApplyCrystalGuardianHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindCrystalGuardianHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyCrystalGuardianHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindCrystalGuardianHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            RestoreVanillaHealth(hm.gameObject, hm);
        }
    }

    private static void OnHealthManagerAwake_CrystalGuardian(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsCrystalGuardian(self))
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
            ApplyCrystalGuardianHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart_CrystalGuardian(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsCrystalGuardian(self))
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
            ApplyCrystalGuardianHealth(self.gameObject, self);
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

        if (!moduleActive || hm == null || hm.gameObject == null || !IsCrystalGuardian(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyCrystalGuardianHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsCrystalGuardian(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyCrystalGuardianHealth(hm.gameObject, hm);
            }
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, CrystalGuardianScene, StringComparison.Ordinal))
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
            ApplyCrystalGuardianHealthIfPresent();
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

    private static bool IsCrystalGuardian(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsCrystalGuardianObject(hm.gameObject);
    }

    private static bool IsCrystalGuardianObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, CrystalGuardianScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(CrystalGuardianName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsCrystalGuardianObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => crystalGuardianUseMaxHp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, CrystalGuardianScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, CrystalGuardianScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static void ApplyCrystalGuardianHealth(GameObject mawlek, HealthManager? hm = null)
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
        int targetHp = ClampCrystalGuardianHp(crystalGuardianMaxHp);
        mawlek.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject mawlek, HealthManager? hm = null)
    {
        if (mawlek == null || !IsCrystalGuardianObject(mawlek))
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

        int targetHp = ClampCrystalGuardianHp(vanillaHp);
        mawlek.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindCrystalGuardianHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsCrystalGuardian(candidate))
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

        hp = Math.Max(hp, DefaultCrystalGuardianVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultCrystalGuardianVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultCrystalGuardianVanillaHp);
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

    private static int ClampCrystalGuardianHp(int value)
    {
        if (value < MinCrystalGuardianHp)
        {
            return MinCrystalGuardianHp;
        }

        return value > MaxCrystalGuardianHp ? MaxCrystalGuardianHp : value;
    }
}


