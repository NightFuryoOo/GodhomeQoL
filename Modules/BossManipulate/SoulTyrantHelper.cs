using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class SoulTyrantHelper : Module
{
    private const string SoulTyrantScene = "GG_Soul_Tyrant";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string SoulTyrantName = "Dream Mage Lord";
    private const string SoulTyrantPhase2Marker = "Phase2";
    private const int DefaultSoulTyrantPhase1Hp = 1200;
    private const int DefaultSoulTyrantPhase2Hp = 650;
    private const int P5SoulTyrantPhase1Hp = 900;
    private const int P5SoulTyrantPhase2Hp = 350;
    private const int MinSoulTyrantHp = 1;
    private const int MaxSoulTyrantHp = 999999;

    [LocalSetting]
    [BoolOption]
    internal static bool soulTyrantP5Hp = false;

    [LocalSetting]
    internal static int soulTyrantPhase1Hp = DefaultSoulTyrantPhase1Hp;

    [LocalSetting]
    internal static int soulTyrantPhase2Hp = DefaultSoulTyrantPhase2Hp;

    [LocalSetting]
    internal static int soulTyrantPhase1HpBeforeP5 = DefaultSoulTyrantPhase1Hp;

    [LocalSetting]
    internal static int soulTyrantPhase2HpBeforeP5 = DefaultSoulTyrantPhase2Hp;

    [LocalSetting]
    internal static bool soulTyrantHasStoredStateBeforeP5 = false;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        NormalizeP5State();
        vanillaHpByInstance.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake_SoulTyrant;
        On.HealthManager.Start += OnHealthManagerStart_SoulTyrant;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresent();
        moduleActive = false;
        On.HealthManager.Awake -= OnHealthManagerAwake_SoulTyrant;
        On.HealthManager.Start -= OnHealthManagerStart_SoulTyrant;
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

        ApplySoulTyrantHealthIfPresent();
    }

    internal static void SetP5HpEnabled(bool value)
    {
        if (value)
        {
            if (!soulTyrantP5Hp)
            {
                soulTyrantPhase1HpBeforeP5 = ClampSoulTyrantHp(soulTyrantPhase1Hp);
                soulTyrantPhase2HpBeforeP5 = ClampSoulTyrantHp(soulTyrantPhase2Hp);
                soulTyrantHasStoredStateBeforeP5 = true;
            }

            soulTyrantP5Hp = true;
            soulTyrantPhase1Hp = P5SoulTyrantPhase1Hp;
            soulTyrantPhase2Hp = P5SoulTyrantPhase2Hp;
        }
        else
        {
            if (soulTyrantP5Hp && soulTyrantHasStoredStateBeforeP5)
            {
                soulTyrantPhase1Hp = ClampSoulTyrantHp(soulTyrantPhase1HpBeforeP5);
                soulTyrantPhase2Hp = ClampSoulTyrantHp(soulTyrantPhase2HpBeforeP5);
            }

            soulTyrantP5Hp = false;
            soulTyrantHasStoredStateBeforeP5 = false;
        }

        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!soulTyrantP5Hp)
        {
            soulTyrantPhase1Hp = ClampSoulTyrantHp(soulTyrantPhase1Hp);
            soulTyrantPhase2Hp = ClampSoulTyrantHp(soulTyrantPhase2Hp);
            return;
        }

        if (!soulTyrantHasStoredStateBeforeP5)
        {
            soulTyrantPhase1HpBeforeP5 = ClampSoulTyrantHp(soulTyrantPhase1Hp);
            soulTyrantPhase2HpBeforeP5 = ClampSoulTyrantHp(soulTyrantPhase2Hp);
            soulTyrantHasStoredStateBeforeP5 = true;
        }

        soulTyrantPhase1Hp = P5SoulTyrantPhase1Hp;
        soulTyrantPhase2Hp = P5SoulTyrantPhase2Hp;
    }

    internal static void ApplySoulTyrantHealthIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || !IsSoulTyrant(hm) || hm.gameObject == null || !ShouldApplySettings(hm.gameObject))
            {
                continue;
            }

            if (IsSoulTyrantPhase2Object(hm.gameObject))
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
            if (hm == null || !IsSoulTyrant(hm) || hm.gameObject == null)
            {
                continue;
            }

            RestoreVanillaHealth(hm.gameObject, hm);
        }
    }

    private static void OnHealthManagerAwake_SoulTyrant(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsSoulTyrant(self))
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        if (IsSoulTyrantPhase2Object(self.gameObject))
        {
            ApplyPhase2Health(self.gameObject, self);
        }
        else
        {
            ApplyPhase1Health(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart_SoulTyrant(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsSoulTyrant(self))
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        if (IsSoulTyrantPhase2Object(self.gameObject))
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
        if (!string.Equals(to.name, SoulTyrantScene, StringComparison.Ordinal))
        {
            vanillaHpByInstance.Clear();
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        ApplySoulTyrantHealthIfPresent();
    }

    private static string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);
        return newSceneName;
    }

    private static bool IsSoulTyrant(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsSoulTyrantObject(hm.gameObject);
    }

    private static bool IsSoulTyrantObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, SoulTyrantScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(SoulTyrantName, StringComparison.Ordinal);
    }

    private static bool IsSoulTyrantPhase2Object(GameObject gameObject)
    {
        return gameObject != null
            && IsSoulTyrantObject(gameObject)
            && gameObject.name.IndexOf(SoulTyrantPhase2Marker, StringComparison.Ordinal) >= 0;
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsSoulTyrantObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, SoulTyrantScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, SoulTyrantScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

        int targetHp = ClampSoulTyrantHp(soulTyrantPhase1Hp);
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

        int targetHp = ClampSoulTyrantHp(soulTyrantPhase2Hp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsSoulTyrantObject(boss))
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

        int targetHp = ClampSoulTyrantHp(vanillaHp);
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

    private static int ClampSoulTyrantHp(int value)
    {
        if (value < MinSoulTyrantHp)
        {
            return MinSoulTyrantHp;
        }

        return value > MaxSoulTyrantHp ? MaxSoulTyrantHp : value;
    }
}
