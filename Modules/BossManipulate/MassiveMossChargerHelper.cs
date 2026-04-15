using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class MassiveMossChargerHelper : Module
{
    private const string MassiveMossScene = "GG_Mega_Moss_Charger";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string MassiveMossName = "Mega Moss Charger";
    private const int DefaultMassiveMossMaxHp = 850;
    private const int DefaultMassiveMossVanillaHp = 850;
    private const int P5MassiveMossHp = 480;
    private const int MinMassiveMossHp = 1;
    private const int MaxMassiveMossHp = 999999;

    [LocalSetting]
    [BoolOption]
    internal static bool massiveMossUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool massiveMossP5Hp = false;

    [LocalSetting]
    internal static int massiveMossMaxHp = DefaultMassiveMossMaxHp;

    [LocalSetting]
    internal static int massiveMossMaxHpBeforeP5 = DefaultMassiveMossMaxHp;

    [LocalSetting]
    internal static bool massiveMossUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool massiveMossHasStoredStateBeforeP5 = false;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        NormalizeP5State();
        vanillaHpByInstance.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake_MassiveMoss;
        On.HealthManager.Start += OnHealthManagerStart_MassiveMoss;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresent();
        moduleActive = false;
        On.HealthManager.Awake -= OnHealthManagerAwake_MassiveMoss;
        On.HealthManager.Start -= OnHealthManagerStart_MassiveMoss;
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
            ApplyMassiveMossHealthIfPresent();
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
            if (!massiveMossP5Hp)
            {
                massiveMossMaxHpBeforeP5 = ClampMassiveMossHp(massiveMossMaxHp);
                massiveMossUseMaxHpBeforeP5 = massiveMossUseMaxHp;
                massiveMossHasStoredStateBeforeP5 = true;
            }

            massiveMossP5Hp = true;
            massiveMossUseMaxHp = true;
            massiveMossMaxHp = P5MassiveMossHp;
        }
        else
        {
            if (massiveMossP5Hp && massiveMossHasStoredStateBeforeP5)
            {
                massiveMossMaxHp = ClampMassiveMossHp(massiveMossMaxHpBeforeP5);
                massiveMossUseMaxHp = massiveMossUseMaxHpBeforeP5;
            }

            massiveMossP5Hp = false;
            massiveMossHasStoredStateBeforeP5 = false;
        }

        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!massiveMossP5Hp)
        {
            return;
        }

        if (!massiveMossHasStoredStateBeforeP5)
        {
            massiveMossMaxHpBeforeP5 = ClampMassiveMossHp(massiveMossMaxHp);
            massiveMossUseMaxHpBeforeP5 = massiveMossUseMaxHp;
            massiveMossHasStoredStateBeforeP5 = true;
        }

        massiveMossUseMaxHp = true;
        massiveMossMaxHp = P5MassiveMossHp;
    }

    internal static void ApplyMassiveMossHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindMassiveMossHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyMassiveMossHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindMassiveMossHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            RestoreVanillaHealth(hm.gameObject, hm);
        }
    }

    private static void OnHealthManagerAwake_MassiveMoss(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsMassiveMoss(self))
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
            ApplyMassiveMossHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart_MassiveMoss(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsMassiveMoss(self))
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
            ApplyMassiveMossHealth(self.gameObject, self);
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

        if (!moduleActive || hm == null || hm.gameObject == null || !IsMassiveMoss(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyMassiveMossHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsMassiveMoss(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyMassiveMossHealth(hm.gameObject, hm);
            }
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, MassiveMossScene, StringComparison.Ordinal))
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
            ApplyMassiveMossHealthIfPresent();
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

    private static bool IsMassiveMoss(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsMassiveMossObject(hm.gameObject);
    }

    private static bool IsMassiveMossObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, MassiveMossScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(MassiveMossName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsMassiveMossObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => massiveMossUseMaxHp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, MassiveMossScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, MassiveMossScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static void ApplyMassiveMossHealth(GameObject mawlek, HealthManager? hm = null)
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
        int targetHp = ClampMassiveMossHp(massiveMossMaxHp);
        mawlek.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject mawlek, HealthManager? hm = null)
    {
        if (mawlek == null || !IsMassiveMossObject(mawlek))
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

        int targetHp = ClampMassiveMossHp(vanillaHp);
        mawlek.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindMassiveMossHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsMassiveMoss(candidate))
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

        hp = Math.Max(hp, DefaultMassiveMossVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultMassiveMossVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultMassiveMossVanillaHp);
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

    private static int ClampMassiveMossHp(int value)
    {
        if (value < MinMassiveMossHp)
        {
            return MinMassiveMossHp;
        }

        return value > MaxMassiveMossHp ? MaxMassiveMossHp : value;
    }
}


