using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class SoulMasterHelper : Module
{
    private const string SoulMasterScene = "GG_Soul_Master";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string SoulMasterName = "Mage Lord";
    private const string SoulMasterPhase2Marker = "Phase2";
    private const int DefaultSoulMasterPhase1Hp = 900;
    private const int DefaultSoulMasterPhase2Hp = 600;
    private const int P5SoulMasterPhase1Hp = 600;
    private const int P5SoulMasterPhase2Hp = 350;
    private const int MinSoulMasterHp = 1;
    private const int MaxSoulMasterHp = 999999;

    [LocalSetting]
    [BoolOption]
    internal static bool soulMasterP5Hp = false;

    [LocalSetting]
    internal static int soulMasterPhase1Hp = DefaultSoulMasterPhase1Hp;

    [LocalSetting]
    internal static int soulMasterPhase2Hp = DefaultSoulMasterPhase2Hp;

    [LocalSetting]
    internal static int soulMasterPhase1HpBeforeP5 = DefaultSoulMasterPhase1Hp;

    [LocalSetting]
    internal static int soulMasterPhase2HpBeforeP5 = DefaultSoulMasterPhase2Hp;

    [LocalSetting]
    internal static bool soulMasterHasStoredStateBeforeP5 = false;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        NormalizeP5State();
        vanillaHpByInstance.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake_SoulMaster;
        On.HealthManager.Start += OnHealthManagerStart_SoulMaster;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresent();
        moduleActive = false;
        On.HealthManager.Awake -= OnHealthManagerAwake_SoulMaster;
        On.HealthManager.Start -= OnHealthManagerStart_SoulMaster;
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

        ApplySoulMasterHealthIfPresent();
    }

    internal static void SetP5HpEnabled(bool value)
    {
        if (value)
        {
            if (!soulMasterP5Hp)
            {
                soulMasterPhase1HpBeforeP5 = ClampSoulMasterHp(soulMasterPhase1Hp);
                soulMasterPhase2HpBeforeP5 = ClampSoulMasterHp(soulMasterPhase2Hp);
                soulMasterHasStoredStateBeforeP5 = true;
            }

            soulMasterP5Hp = true;
            soulMasterPhase1Hp = P5SoulMasterPhase1Hp;
            soulMasterPhase2Hp = P5SoulMasterPhase2Hp;
        }
        else
        {
            if (soulMasterP5Hp && soulMasterHasStoredStateBeforeP5)
            {
                soulMasterPhase1Hp = ClampSoulMasterHp(soulMasterPhase1HpBeforeP5);
                soulMasterPhase2Hp = ClampSoulMasterHp(soulMasterPhase2HpBeforeP5);
            }

            soulMasterP5Hp = false;
            soulMasterHasStoredStateBeforeP5 = false;
        }

        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!soulMasterP5Hp)
        {
            soulMasterPhase1Hp = ClampSoulMasterHp(soulMasterPhase1Hp);
            soulMasterPhase2Hp = ClampSoulMasterHp(soulMasterPhase2Hp);
            return;
        }

        if (!soulMasterHasStoredStateBeforeP5)
        {
            soulMasterPhase1HpBeforeP5 = ClampSoulMasterHp(soulMasterPhase1Hp);
            soulMasterPhase2HpBeforeP5 = ClampSoulMasterHp(soulMasterPhase2Hp);
            soulMasterHasStoredStateBeforeP5 = true;
        }

        soulMasterPhase1Hp = P5SoulMasterPhase1Hp;
        soulMasterPhase2Hp = P5SoulMasterPhase2Hp;
    }

    internal static void ApplySoulMasterHealthIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || !IsSoulMaster(hm) || hm.gameObject == null || !ShouldApplySettings(hm.gameObject))
            {
                continue;
            }

            if (IsSoulMasterPhase2Object(hm.gameObject))
            {
                ApplyPhase2Health(hm.gameObject, hm);
            }
            else
            {
                ApplyPhase1Health(hm.gameObject, hm);
            }
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || !IsSoulMaster(hm) || hm.gameObject == null)
            {
                continue;
            }

            RestoreVanillaHealth(hm.gameObject, hm);
        }
    }

    private static void OnHealthManagerAwake_SoulMaster(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsSoulMaster(self))
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        if (IsSoulMasterPhase2Object(self.gameObject))
        {
            ApplyPhase2Health(self.gameObject, self);
        }
        else
        {
            ApplyPhase1Health(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart_SoulMaster(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsSoulMaster(self))
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        if (IsSoulMasterPhase2Object(self.gameObject))
        {
            ApplyPhase2Health(self.gameObject, self);
        }
        else
        {
            ApplyPhase1Health(self.gameObject, self);
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, SoulMasterScene, StringComparison.Ordinal))
        {
            vanillaHpByInstance.Clear();
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        ApplySoulMasterHealthIfPresent();
    }

    private static string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);
        return newSceneName;
    }

    private static bool IsSoulMaster(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsSoulMasterObject(hm.gameObject);
    }

    private static bool IsSoulMasterObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, SoulMasterScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(SoulMasterName, StringComparison.Ordinal);
    }

    private static bool IsSoulMasterPhase2Object(GameObject gameObject)
    {
        return gameObject != null
            && IsSoulMasterObject(gameObject)
            && gameObject.name.IndexOf(SoulMasterPhase2Marker, StringComparison.Ordinal) >= 0;
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsSoulMasterObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, SoulMasterScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, SoulMasterScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static void ApplyPhase1Health(GameObject boss, HealthManager? hm = null)
    {
        if (!ShouldApplySettings(boss))
        {
            return;
        }

        hm ??= boss.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        int targetHp = ClampSoulMasterHp(soulMasterPhase1Hp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void ApplyPhase2Health(GameObject boss, HealthManager? hm = null)
    {
        if (!ShouldApplySettings(boss))
        {
            return;
        }

        hm ??= boss.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        int targetHp = ClampSoulMasterHp(soulMasterPhase2Hp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsSoulMasterObject(boss))
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

        int targetHp = ClampSoulMasterHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
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

        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
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

    private static int ClampSoulMasterHp(int value)
    {
        if (value < MinSoulMasterHp)
        {
            return MinSoulMasterHp;
        }

        return value > MaxSoulMasterHp ? MaxSoulMasterHp : value;
    }
}
