using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class PaintmasterSheoHelper : Module
{
    private const string PaintmasterSheoScene = "GG_Painter";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string PaintmasterSheoName = "Sheo Boss";
    private const int DefaultPaintmasterSheoMaxHp = 1450;
    private const int DefaultPaintmasterSheoVanillaHp = 1450;
    private const int P5PaintmasterSheoHp = 950;
    private const int MinPaintmasterSheoHp = 1;
    private const int MaxPaintmasterSheoHp = 999999;

    [LocalSetting]
    [BoolOption]
    internal static bool paintmasterSheoUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool paintmasterSheoP5Hp = false;

    [LocalSetting]
    internal static int paintmasterSheoMaxHp = DefaultPaintmasterSheoMaxHp;

    [LocalSetting]
    internal static int paintmasterSheoMaxHpBeforeP5 = DefaultPaintmasterSheoMaxHp;

    [LocalSetting]
    internal static bool paintmasterSheoUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool paintmasterSheoHasStoredStateBeforeP5 = false;

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
            ApplyPaintmasterSheoHealthIfPresent();
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
            if (!paintmasterSheoP5Hp)
            {
                paintmasterSheoMaxHpBeforeP5 = ClampPaintmasterSheoHp(paintmasterSheoMaxHp);
                paintmasterSheoUseMaxHpBeforeP5 = paintmasterSheoUseMaxHp;
                paintmasterSheoHasStoredStateBeforeP5 = true;
            }

            paintmasterSheoP5Hp = true;
            paintmasterSheoUseMaxHp = true;
            paintmasterSheoMaxHp = P5PaintmasterSheoHp;
        }
        else
        {
            if (paintmasterSheoP5Hp && paintmasterSheoHasStoredStateBeforeP5)
            {
                paintmasterSheoMaxHp = ClampPaintmasterSheoHp(paintmasterSheoMaxHpBeforeP5);
                paintmasterSheoUseMaxHp = paintmasterSheoUseMaxHpBeforeP5;
            }

            paintmasterSheoP5Hp = false;
            paintmasterSheoHasStoredStateBeforeP5 = false;
        }

        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!paintmasterSheoP5Hp)
        {
            return;
        }

        if (!paintmasterSheoHasStoredStateBeforeP5)
        {
            paintmasterSheoMaxHpBeforeP5 = ClampPaintmasterSheoHp(paintmasterSheoMaxHp);
            paintmasterSheoUseMaxHpBeforeP5 = paintmasterSheoUseMaxHp;
            paintmasterSheoHasStoredStateBeforeP5 = true;
        }

        paintmasterSheoUseMaxHp = true;
        paintmasterSheoMaxHp = P5PaintmasterSheoHp;
    }

    internal static void ApplyPaintmasterSheoHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindPaintmasterSheoHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyPaintmasterSheoHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindPaintmasterSheoHealthManager(out HealthManager? hm))
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

        if (!moduleActive || !IsPaintmasterSheo(self))
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
            ApplyPaintmasterSheoHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsPaintmasterSheo(self))
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
            ApplyPaintmasterSheoHealth(self.gameObject, self);
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

        if (!moduleActive || hm == null || hm.gameObject == null || !IsPaintmasterSheo(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyPaintmasterSheoHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsPaintmasterSheo(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyPaintmasterSheoHealth(hm.gameObject, hm);
            }
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, PaintmasterSheoScene, StringComparison.Ordinal))
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
            ApplyPaintmasterSheoHealthIfPresent();
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

    private static bool IsPaintmasterSheo(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsPaintmasterSheoObject(hm.gameObject);
    }

    private static bool IsPaintmasterSheoObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, PaintmasterSheoScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(PaintmasterSheoName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsPaintmasterSheoObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => paintmasterSheoUseMaxHp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, PaintmasterSheoScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, PaintmasterSheoScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static void ApplyPaintmasterSheoHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampPaintmasterSheoHp(paintmasterSheoMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsPaintmasterSheoObject(boss))
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

        int targetHp = ClampPaintmasterSheoHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindPaintmasterSheoHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsPaintmasterSheo(candidate))
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

        hp = Math.Max(hp, DefaultPaintmasterSheoVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultPaintmasterSheoVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultPaintmasterSheoVanillaHp);
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

    private static int ClampPaintmasterSheoHp(int value)
    {
        if (value < MinPaintmasterSheoHp)
        {
            return MinPaintmasterSheoHp;
        }

        return value > MaxPaintmasterSheoHp ? MaxPaintmasterSheoHp : value;
    }
}
