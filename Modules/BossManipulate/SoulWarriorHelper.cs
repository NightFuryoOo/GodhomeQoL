using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class SoulWarriorHelper : Module
{
    private const string SoulWarriorScene = "GG_Mage_Knight_V";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string SoulWarriorName = "Mage Knight";
    private static readonly string[] SoulWarriorSummonNameHints =
    {
        "Mage Balloon Spawner",
        "Mage Balloon",
        "Balloon Spawner",
        "Balloon",
    };
    private static readonly string[] SoulWarriorSummonStateHints =
    {
        "Count Summon",
        "Summon Choice",
        "Summon Start",
        "Idle or Summoned?",
    };
    private static readonly string[] SoulWarriorSummonCountVariableNames =
    {
        "Balls",
        "Balloons",
    };
    private const int DefaultSoulWarriorMaxHp = 1000;
    private const int DefaultSoulWarriorVanillaHp = 1000;
    private const int DefaultSoulWarriorSummonHp = 13;
    private const int DefaultSoulWarriorSummonVanillaHp = 13;
    private const int DefaultSoulWarriorSummonLimit = 36;
    private const int DefaultSoulWarriorVanillaSummonLimit = 36;
    private const int P5SoulWarriorHp = 750;
    private const int MinSoulWarriorHp = 1;
    private const int MaxSoulWarriorHp = 999999;
    private const int MinSoulWarriorSummonLimit = DefaultSoulWarriorSummonLimit;
    private const int MaxSoulWarriorSummonLimit = 999;

    [LocalSetting]
    [BoolOption]
    internal static bool soulWarriorUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool soulWarriorP5Hp = false;

    [LocalSetting]
    internal static int soulWarriorMaxHp = DefaultSoulWarriorMaxHp;

    [LocalSetting]
    internal static int soulWarriorMaxHpBeforeP5 = DefaultSoulWarriorMaxHp;

    [LocalSetting]
    internal static bool soulWarriorUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool soulWarriorHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool soulWarriorUseCustomSummonHp = false;

    [LocalSetting]
    internal static int soulWarriorSummonHp = DefaultSoulWarriorSummonHp;

    [LocalSetting]
    internal static bool soulWarriorUseCustomSummonLimit = false;

    [LocalSetting]
    internal static int soulWarriorSummonLimit = DefaultSoulWarriorSummonLimit;

    [LocalSetting]
    internal static bool soulWarriorUseCustomSummonHpBeforeP5 = false;

    [LocalSetting]
    internal static int soulWarriorSummonHpBeforeP5 = DefaultSoulWarriorSummonHp;

    [LocalSetting]
    internal static bool soulWarriorUseCustomSummonLimitBeforeP5 = false;

    [LocalSetting]
    internal static int soulWarriorSummonLimitBeforeP5 = DefaultSoulWarriorSummonLimit;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static readonly Dictionary<int, int> vanillaSummonHpByInstance = new();
    private static readonly Dictionary<int, int> vanillaSummonLimitByFsm = new();
    private static readonly Dictionary<int, Dictionary<string, int>> vanillaSummonCountByFsm = new();
    private static readonly Dictionary<int, Dictionary<string, GameObject[]>> vanillaSummonPoolsByFsm = new();
    private static readonly Dictionary<int, List<GameObject>> createdSummonClonesByFsm = new();
    private static readonly List<GameObject> createdSceneSummonClones = [];
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        NormalizeP5State();
        vanillaHpByInstance.Clear();
        vanillaSummonHpByInstance.Clear();
        vanillaSummonLimitByFsm.Clear();
        vanillaSummonCountByFsm.Clear();
        vanillaSummonPoolsByFsm.Clear();
        DestroyAllCreatedSummonClones();
        On.SetHP.OnEnter += OnSetHpEnter;
        On.HealthManager.OnEnable += OnHealthManagerOnEnable;
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresent();
        RestoreVanillaSummonHealthIfPresent();
        RestoreVanillaSummonLimitsIfPresent();
        moduleActive = false;
        On.SetHP.OnEnter -= OnSetHpEnter;
        On.HealthManager.OnEnable -= OnHealthManagerOnEnable;
        On.HealthManager.Awake -= OnHealthManagerAwake;
        On.HealthManager.Start -= OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
        vanillaSummonHpByInstance.Clear();
        vanillaSummonLimitByFsm.Clear();
        vanillaSummonCountByFsm.Clear();
        vanillaSummonPoolsByFsm.Clear();
        DestroyAllCreatedSummonClones();
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
            ApplySoulWarriorHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplySoulWarriorSummonHealthIfPresent();
        }
        else
        {
            RestoreVanillaSummonHealthIfPresent();
        }

        if (ShouldUseCustomSummonLimit())
        {
            ApplySummonLimitSettingsIfPresent();
        }
        else
        {
            RestoreVanillaSummonLimitsIfPresent();
        }
    }

    internal static void SetP5HpEnabled(bool value)
    {
        if (value)
        {
            if (!soulWarriorP5Hp)
            {
                soulWarriorMaxHpBeforeP5 = ClampSoulWarriorHp(soulWarriorMaxHp);
                soulWarriorUseMaxHpBeforeP5 = soulWarriorUseMaxHp;
                soulWarriorUseCustomSummonHpBeforeP5 = soulWarriorUseCustomSummonHp;
                soulWarriorSummonHpBeforeP5 = ClampSoulWarriorHp(soulWarriorSummonHp);
                soulWarriorUseCustomSummonLimitBeforeP5 = soulWarriorUseCustomSummonLimit;
                soulWarriorSummonLimitBeforeP5 = ClampSoulWarriorSummonLimit(soulWarriorSummonLimit);
                soulWarriorHasStoredStateBeforeP5 = true;
            }

            soulWarriorP5Hp = true;
            soulWarriorUseMaxHp = true;
            soulWarriorUseCustomSummonHp = false;
            soulWarriorUseCustomSummonLimit = false;
            soulWarriorMaxHp = P5SoulWarriorHp;
        }
        else
        {
            if (soulWarriorP5Hp && soulWarriorHasStoredStateBeforeP5)
            {
                soulWarriorMaxHp = ClampSoulWarriorHp(soulWarriorMaxHpBeforeP5);
                soulWarriorUseMaxHp = soulWarriorUseMaxHpBeforeP5;
                soulWarriorUseCustomSummonHp = soulWarriorUseCustomSummonHpBeforeP5;
                soulWarriorSummonHp = ClampSoulWarriorHp(soulWarriorSummonHpBeforeP5);
                soulWarriorUseCustomSummonLimit = soulWarriorUseCustomSummonLimitBeforeP5;
                soulWarriorSummonLimit = ClampSoulWarriorSummonLimit(soulWarriorSummonLimitBeforeP5);
            }

            soulWarriorP5Hp = false;
            soulWarriorHasStoredStateBeforeP5 = false;
        }

        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        soulWarriorSummonHp = ClampSoulWarriorHp(soulWarriorSummonHp);
        soulWarriorSummonLimit = ClampSoulWarriorSummonLimit(soulWarriorSummonLimit);
        if (!soulWarriorP5Hp)
        {
            return;
        }

        if (!soulWarriorHasStoredStateBeforeP5)
        {
            soulWarriorMaxHpBeforeP5 = ClampSoulWarriorHp(soulWarriorMaxHp);
            soulWarriorUseMaxHpBeforeP5 = soulWarriorUseMaxHp;
            soulWarriorUseCustomSummonHpBeforeP5 = soulWarriorUseCustomSummonHp;
            soulWarriorSummonHpBeforeP5 = ClampSoulWarriorHp(soulWarriorSummonHp);
            soulWarriorUseCustomSummonLimitBeforeP5 = soulWarriorUseCustomSummonLimit;
            soulWarriorSummonLimitBeforeP5 = ClampSoulWarriorSummonLimit(soulWarriorSummonLimit);
            soulWarriorHasStoredStateBeforeP5 = true;
        }

        soulWarriorUseMaxHp = true;
        soulWarriorUseCustomSummonHp = false;
        soulWarriorUseCustomSummonLimit = false;
        soulWarriorMaxHp = P5SoulWarriorHp;
    }

    internal static void ApplySoulWarriorHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindSoulWarriorHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplySoulWarriorHealth(hm.gameObject, hm);
        }
    }

    internal static void ApplySoulWarriorSummonHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomSummonHp())
        {
            return;
        }

        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsSoulWarriorSummon(hm))
            {
                continue;
            }

            ApplySoulWarriorSummonHealth(hm.gameObject, hm);
        }
    }

    internal static void ApplySummonLimitSettingsIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null)
            {
                continue;
            }

            ApplySummonLimitSettings(fsm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindSoulWarriorHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            RestoreVanillaHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaSummonHealthIfPresent()
    {
        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsSoulWarriorSummon(hm))
            {
                continue;
            }

            RestoreVanillaSummonHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaSummonLimitsIfPresent()
    {
        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsSoulWarriorSummonControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySummonLimitSettings(fsm))
            {
                continue;
            }

            RememberVanillaSummonLimit(fsm);
            RememberVanillaSummonCount(fsm);
            SetSummonLimitOnFsm(fsm, GetVanillaSummonLimit(fsm));
            SetSummonCountOnFsm(fsm, GetVanillaSummonCount(fsm), custom: false);
            RestoreVanillaSummonPool(fsm);
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
        if (targetObject == null || !IsSoulWarriorSummonObject(targetObject) || !ShouldApplySummonSettings(targetObject))
        {
            return;
        }

        HealthManager hm = targetObject.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        RememberVanillaSummonHp(hm);
        ApplySoulWarriorSummonHealth(targetObject, hm);
    }

    private static void EnsureObjectPoolCapacity(GameObject? prefab, int desiredTotal)
    {
        if (prefab == null || !IsSoulWarriorSummonObject(prefab))
        {
            return;
        }

        int clampedDesired = ClampSoulWarriorSummonLimit(desiredTotal);
        if (clampedDesired <= 0)
        {
            return;
        }

        try
        {
            int pooled = ObjectPool.CountPooled(prefab);
            int spawned = ObjectPool.CountSpawned(prefab);
            int total = pooled + spawned;
            if (total >= clampedDesired)
            {
                return;
            }

            ObjectPool.CreatePool(prefab, clampedDesired);
            pooled = ObjectPool.CountPooled(prefab);
            spawned = ObjectPool.CountSpawned(prefab);
            total = pooled + spawned;

            int guard = 0;
            while (total < clampedDesired && guard < clampedDesired * 2)
            {
                GameObject spawnedObj = ObjectPool.Spawn(prefab);
                if (spawnedObj == null)
                {
                    break;
                }

                if (ShouldUseCustomSummonHp())
                {
                    HealthManager hm = spawnedObj.GetComponent<HealthManager>();
                    if (hm != null)
                    {
                        RememberVanillaSummonHp(hm);
                        ApplySoulWarriorSummonHealth(spawnedObj, hm);
                    }
                }

                ObjectPool.Recycle(spawnedObj);
                pooled = ObjectPool.CountPooled(prefab);
                spawned = ObjectPool.CountSpawned(prefab);
                total = pooled + spawned;
                guard++;
            }
        }
        catch
        {
            // Ignore ObjectPool errors.
        }
    }

    private static void OnHealthManagerOnEnable(On.HealthManager.orig_OnEnable orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsSoulWarriorSummon(self))
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
            ApplySoulWarriorSummonHealth(self.gameObject, self);
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

        if (IsSoulWarrior(self))
        {
            RememberVanillaHp(self);
            if (!ShouldApplySettings(self.gameObject))
            {
                return;
            }

            if (ShouldUseCustomHp())
            {
                ApplySoulWarriorHealth(self.gameObject, self);
            }
            else
            {
                RestoreVanillaHealth(self.gameObject, self);
            }

            return;
        }

        if (!IsSoulWarriorSummon(self))
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
            ApplySoulWarriorSummonHealth(self.gameObject, self);
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

        if (IsSoulWarrior(self))
        {
            RememberVanillaHp(self);
            if (!ShouldApplySettings(self.gameObject))
            {
                return;
            }

            if (ShouldUseCustomHp())
            {
                ApplySoulWarriorHealth(self.gameObject, self);
                _ = self.StartCoroutine(DeferredApply(self));
            }
            else
            {
                RestoreVanillaHealth(self.gameObject, self);
            }

            ApplySummonLimitSettingsIfPresent();
            return;
        }

        if (!IsSoulWarriorSummon(self))
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
            ApplySoulWarriorSummonHealth(self.gameObject, self);
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

        if (IsSoulWarrior(hm))
        {
            if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplySoulWarriorHealth(hm.gameObject, hm);
                yield return new WaitForSeconds(0.01f);
                if (moduleActive && hm != null && hm.gameObject != null && IsSoulWarrior(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
                {
                    ApplySoulWarriorHealth(hm.gameObject, hm);
                }
            }

            ApplySummonLimitSettingsIfPresent();
            yield break;
        }

        if (!IsSoulWarriorSummon(hm))
        {
            yield break;
        }

        if (!ShouldApplySummonSettings(hm.gameObject))
        {
            yield break;
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplySoulWarriorSummonHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsSoulWarriorSummon(hm) && ShouldUseCustomSummonHp() && ShouldApplySummonSettings(hm.gameObject))
            {
                ApplySoulWarriorSummonHealth(hm.gameObject, hm);
            }
        }
        else
        {
            RestoreVanillaSummonHealth(hm.gameObject, hm);
        }
    }

    private static void OnPlayMakerFsmOnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null)
        {
            return;
        }

        ApplySummonLimitSettings(self);
    }

    private static void OnPlayMakerFsmStart(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null)
        {
            return;
        }

        ApplySummonLimitSettings(self);
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, SoulWarriorScene, StringComparison.Ordinal))
        {
            vanillaHpByInstance.Clear();
            vanillaSummonHpByInstance.Clear();
            vanillaSummonLimitByFsm.Clear();
            vanillaSummonCountByFsm.Clear();
            vanillaSummonPoolsByFsm.Clear();
            DestroyAllCreatedSummonClones();
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplySoulWarriorHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplySoulWarriorSummonHealthIfPresent();
        }
        else
        {
            RestoreVanillaSummonHealthIfPresent();
        }

        if (ShouldUseCustomSummonLimit())
        {
            ApplySummonLimitSettingsIfPresent();
        }
        else
        {
            RestoreVanillaSummonLimitsIfPresent();
        }
    }

    private static string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);
        return newSceneName;
    }

    private static bool IsSoulWarrior(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsSoulWarriorObject(hm.gameObject);
    }

    private static bool IsSoulWarriorSummon(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsSoulWarriorSummonObject(hm.gameObject);
    }

    private static bool IsSoulWarriorObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, SoulWarriorScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(SoulWarriorName, StringComparison.Ordinal);
    }

    private static bool IsSoulWarriorSummonObject(GameObject gameObject)
    {
        if (gameObject == null || IsSoulWarriorObject(gameObject))
        {
            return false;
        }

        string name = gameObject.name;
        bool matchesSummonName = false;
        foreach (string hint in SoulWarriorSummonNameHints)
        {
            if (name.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                matchesSummonName = true;
                break;
            }
        }

        if (!matchesSummonName)
        {
            return false;
        }

        if (string.Equals(gameObject.scene.name, SoulWarriorScene, StringComparison.Ordinal))
        {
            return true;
        }

        // Summons can be pooled and keep a non-arena scene.
        return hoGEntryAllowed && string.Equals(USceneManager.GetActiveScene().name, SoulWarriorScene, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsSoulWarriorObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldApplySummonLimitSettings(PlayMakerFSM? fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsSoulWarriorSummonControlFsm(fsm))
        {
            return false;
        }

        if (!hoGEntryAllowed)
        {
            return false;
        }

        string activeScene = USceneManager.GetActiveScene().name;
        if (!string.Equals(activeScene, SoulWarriorScene, StringComparison.Ordinal))
        {
            return false;
        }

        return string.Equals(fsm.gameObject.scene.name, SoulWarriorScene, StringComparison.Ordinal)
            || IsSoulWarriorObject(fsm.gameObject);
    }

    private static bool ShouldApplySummonSettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsSoulWarriorSummonObject(gameObject))
        {
            return false;
        }

        if (!hoGEntryAllowed)
        {
            return false;
        }

        string activeScene = USceneManager.GetActiveScene().name;
        return string.Equals(activeScene, SoulWarriorScene, StringComparison.Ordinal)
            || string.Equals(gameObject.scene.name, SoulWarriorScene, StringComparison.Ordinal);
    }

    private static bool IsSoulWarriorArenaContext(GameObject gameObject)
    {
        if (gameObject == null || !hoGEntryAllowed)
        {
            return false;
        }

        if (string.Equals(USceneManager.GetActiveScene().name, SoulWarriorScene, StringComparison.Ordinal))
        {
            return true;
        }

        return string.Equals(gameObject.scene.name, SoulWarriorScene, StringComparison.Ordinal);
    }

    private static bool ShouldUseCustomHp() => soulWarriorUseMaxHp;
    private static bool ShouldUseCustomSummonHp() => soulWarriorUseCustomSummonHp && !soulWarriorP5Hp;
    private static bool ShouldUseCustomSummonLimit() => soulWarriorUseCustomSummonLimit && !soulWarriorP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, SoulWarriorScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, SoulWarriorScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static void ApplySoulWarriorHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampSoulWarriorHp(soulWarriorMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void ApplySoulWarriorSummonHealth(GameObject summon, HealthManager? hm = null)
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
        int targetHp = ClampSoulWarriorHp(soulWarriorSummonHp);
        summon.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void ApplySummonLimitSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsSoulWarriorSummonControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySummonLimitSettings(fsm))
        {
            return;
        }

        RememberVanillaSummonLimit(fsm);
        RememberVanillaSummonCount(fsm);
        RememberVanillaSummonPools(fsm);
        int targetLimit = ShouldUseCustomSummonLimit()
            ? ClampSoulWarriorSummonLimit(soulWarriorSummonLimit)
            : GetVanillaSummonLimit(fsm);
        SetSummonLimitOnFsm(fsm, targetLimit);
        SetSummonCountOnFsm(fsm, targetLimit, ShouldUseCustomSummonLimit());
        ApplySummonCountSetterOverrides(fsm, targetLimit);
        SetSummonPoolLimitOnFsm(fsm, targetLimit);
    }

    private static bool IsSoulWarriorSummonControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsSoulWarriorArenaContext(fsm.gameObject))
        {
            return false;
        }

        if (FindSummonCountVariables(fsm).Count > 0)
        {
            return true;
        }

        if (HasLikelySummonState(fsm))
        {
            return true;
        }

        return FindSummonPoolVariables(fsm).Count > 0;
    }

    private static void SetSummonLimitOnFsm(PlayMakerFSM fsm, int value)
    {
        List<IntCompare> compares = FindSummonEnemyCountCompareActions(fsm);
        if (compares.Count == 0)
        {
            return;
        }

        int clampedLimit = ClampSoulWarriorSummonLimit(value);
        foreach (IntCompare compare in compares)
        {
            if (compare == null)
            {
                continue;
            }

            if (IsLikelySummonCountVariable(compare.integer1) && compare.integer2 != null)
            {
                compare.integer2.UseVariable = false;
                compare.integer2.Name = string.Empty;
                compare.integer2.Value = clampedLimit;
                continue;
            }

            if (IsLikelySummonCountVariable(compare.integer2) && compare.integer1 != null)
            {
                compare.integer1.UseVariable = false;
                compare.integer1.Name = string.Empty;
                compare.integer1.Value = clampedLimit;
                continue;
            }

            if (compare.integer2 != null)
            {
                compare.integer2.UseVariable = false;
                compare.integer2.Name = string.Empty;
                compare.integer2.Value = clampedLimit;
                continue;
            }

            if (compare.integer1 != null)
            {
                compare.integer1.UseVariable = false;
                compare.integer1.Name = string.Empty;
                compare.integer1.Value = clampedLimit;
            }
        }
    }

    private static IntCompare? FindSummonEnemyCountCompareAction(PlayMakerFSM fsm)
    {
        List<IntCompare> compares = FindSummonEnemyCountCompareActions(fsm);
        return compares.Count > 0 ? compares[0] : null;
    }

    private static bool IsLikelySummonStateName(string? stateName)
    {
        if (string.IsNullOrWhiteSpace(stateName))
        {
            return false;
        }

        foreach (string hint in SoulWarriorSummonStateHints)
        {
            if (string.Equals(stateName, hint, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        string normalizedStateName = stateName!;
        return normalizedStateName.IndexOf("summon", StringComparison.OrdinalIgnoreCase) >= 0
            || normalizedStateName.IndexOf("balloon", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static List<FsmState> GetLikelySummonStates(PlayMakerFSM fsm)
    {
        List<FsmState> result = [];
        if (fsm?.Fsm?.States == null)
        {
            return result;
        }

        foreach (FsmState state in fsm.Fsm.States)
        {
            if (state != null && IsLikelySummonStateName(state.Name))
            {
                result.Add(state);
            }
        }

        if (result.Count > 0)
        {
            return result;
        }

        foreach (string hint in SoulWarriorSummonStateHints)
        {
            FsmState? state = fsm.Fsm.GetState(hint);
            if (state != null && !result.Contains(state))
            {
                result.Add(state);
            }
        }

        return result;
    }

    private static bool HasLikelySummonState(PlayMakerFSM fsm)
    {
        return GetLikelySummonStates(fsm).Count > 0;
    }

    private static List<IntCompare> FindSummonEnemyCountCompareActions(PlayMakerFSM fsm)
    {
        if (fsm?.Fsm == null)
        {
            return [];
        }

        IntCompare? best = null;
        int bestScore = int.MinValue;
        HashSet<IntCompare> matches = [];

        List<FsmState> candidateStates = GetLikelySummonStates(fsm);
        if (candidateStates.Count == 0 && fsm.Fsm.States != null)
        {
            candidateStates.AddRange(fsm.Fsm.States.Where(state => state != null));
        }

        foreach (FsmState state in candidateStates)
        {
            if (state?.Actions == null)
            {
                continue;
            }

            foreach (FsmStateAction action in state.Actions)
            {
                if (action is not IntCompare compare)
                {
                    continue;
                }

                int score = GetSummonLimitCompareScore(compare);
                if (score > bestScore)
                {
                    best = compare;
                    bestScore = score;
                }

                if (score > 0)
                {
                    matches.Add(compare);
                }
            }
        }

        if (bestScore > 0 && best != null)
        {
            matches.Add(best);
        }

        return matches.ToList();
    }

    private static int GetSummonLimitCompareScore(IntCompare compare)
    {
        if (compare == null)
        {
            return int.MinValue;
        }

        int score = 0;
        if (IsLikelySummonCountVariable(compare.integer1))
        {
            score += 100;
        }

        if (IsLikelySummonCountVariable(compare.integer2))
        {
            score += 100;
        }

        int candidateValue = ReadSummonLimitThreshold(compare, fallback: -1);
        if (candidateValue >= 0)
        {
            if (candidateValue >= 2 && candidateValue <= 200)
            {
                score += 20;
            }

            if (candidateValue == DefaultSoulWarriorVanillaSummonLimit)
            {
                score += 40;
            }
        }

        return score;
    }

    private static bool IsLikelySummonCountVariable(FsmInt? operand)
    {
        if (operand == null)
        {
            return false;
        }

        return IsKnownSummonCountVariableName(operand.Name);
    }

    private static bool IsKnownSummonCountVariableName(string? variableName)
    {
        if (string.IsNullOrWhiteSpace(variableName))
        {
            return false;
        }

        foreach (string knownName in SoulWarriorSummonCountVariableNames)
        {
            if (string.Equals(variableName, knownName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        string normalizedVariableName = variableName!;
        bool hasSummonToken = normalizedVariableName.IndexOf("ball", StringComparison.OrdinalIgnoreCase) >= 0
            || normalizedVariableName.IndexOf("summon", StringComparison.OrdinalIgnoreCase) >= 0;
        if (!hasSummonToken)
        {
            return false;
        }

        return normalizedVariableName.IndexOf("hp", StringComparison.OrdinalIgnoreCase) < 0;
    }

    private static int ReadSummonLimitThreshold(IntCompare compare, int fallback)
    {
        if (compare == null)
        {
            return fallback;
        }

        if (IsLikelySummonCountVariable(compare.integer1) && compare.integer2 != null)
        {
            return compare.integer2.Value;
        }

        if (IsLikelySummonCountVariable(compare.integer2) && compare.integer1 != null)
        {
            return compare.integer1.Value;
        }

        if (compare.integer2 != null)
        {
            return compare.integer2.Value;
        }

        if (compare.integer1 != null)
        {
            return compare.integer1.Value;
        }

        return fallback;
    }

    private static void RememberVanillaSummonLimit(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSummonLimitByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int limit = DefaultSoulWarriorVanillaSummonLimit;
        IntCompare? compare = FindSummonEnemyCountCompareAction(fsm);
        if (compare != null)
        {
            int readLimit = ReadSummonLimitThreshold(compare, DefaultSoulWarriorVanillaSummonLimit);
            if (readLimit > 0)
            {
                limit = readLimit;
            }
        }
        else
        {
            int poolCount = ResolveVanillaSummonPoolSize(fsm);
            if (poolCount > 0)
            {
                limit = poolCount;
            }
        }

        vanillaSummonLimitByFsm[fsmId] = ClampSoulWarriorSummonLimit(limit);
    }

    private static int GetVanillaSummonLimit(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSummonLimitByFsm.TryGetValue(fsmId, out int limit))
        {
            return ClampSoulWarriorSummonLimit(limit);
        }

        RememberVanillaSummonLimit(fsm);
        if (vanillaSummonLimitByFsm.TryGetValue(fsmId, out limit))
        {
            return ClampSoulWarriorSummonLimit(limit);
        }

        return DefaultSoulWarriorVanillaSummonLimit;
    }

    private static void RememberVanillaSummonCount(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSummonCountByFsm.ContainsKey(fsmId))
        {
            return;
        }

        Dictionary<string, int> stored = new(StringComparer.OrdinalIgnoreCase);
        foreach (FsmInt countVar in FindSummonCountVariables(fsm))
        {
            if (countVar == null)
            {
                continue;
            }

            int value = countVar.Value;
            if (value <= 0)
            {
                value = ResolveVanillaSummonPoolSize(fsm);
            }

            if (value <= 0)
            {
                value = DefaultSoulWarriorVanillaSummonLimit;
            }

            stored[countVar.Name] = ClampSoulWarriorSummonLimit(value);
        }

        if (stored.Count == 0)
        {
            stored["__fallback__"] = DefaultSoulWarriorVanillaSummonLimit;
        }

        vanillaSummonCountByFsm[fsmId] = stored;
    }

    private static int GetVanillaSummonCount(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSummonCountByFsm.TryGetValue(fsmId, out Dictionary<string, int>? stored) && stored != null)
        {
            foreach (int count in stored.Values)
            {
                return ClampSoulWarriorSummonLimit(count);
            }
        }

        RememberVanillaSummonCount(fsm);
        if (vanillaSummonCountByFsm.TryGetValue(fsmId, out stored) && stored != null)
        {
            foreach (int count in stored.Values)
            {
                return ClampSoulWarriorSummonLimit(count);
            }
        }

        return DefaultSoulWarriorVanillaSummonLimit;
    }

    private static void SetSummonCountOnFsm(PlayMakerFSM fsm, int value, bool custom)
    {
        int clamped = ClampSoulWarriorSummonLimit(value);
        List<FsmInt> countVars = FindSummonCountVariables(fsm);
        if (custom)
        {
            foreach (FsmInt count in countVars)
            {
                if (count != null)
                {
                    count.Value = clamped;
                }
            }

            return;
        }

        RememberVanillaSummonCount(fsm);
        int fsmId = fsm.GetInstanceID();
        if (!vanillaSummonCountByFsm.TryGetValue(fsmId, out Dictionary<string, int>? stored) || stored == null)
        {
            return;
        }

        foreach (FsmInt count in countVars)
        {
            if (count == null)
            {
                continue;
            }

            if (stored.TryGetValue(count.Name, out int vanilla))
            {
                count.Value = ClampSoulWarriorSummonLimit(vanilla);
            }
            else
            {
                count.Value = GetVanillaSummonCount(fsm);
            }
        }
    }

    private static void ApplySummonCountSetterOverrides(PlayMakerFSM fsm, int value)
    {
        if (fsm?.Fsm?.States == null)
        {
            return;
        }

        int clamped = ClampSoulWarriorSummonLimit(value);
        List<FsmState> candidateStates = GetLikelySummonStates(fsm);
        if (candidateStates.Count == 0)
        {
            return;
        }

        foreach (FsmState state in candidateStates)
        {
            if (state?.Actions == null)
            {
                continue;
            }

            foreach (FsmStateAction action in state.Actions)
            {
                if (action is SetIntValue setInt)
                {
                    if (!IsLikelySummonCountVariable(setInt.intVariable) || setInt.intValue == null)
                    {
                        continue;
                    }

                    setInt.intValue.UseVariable = false;
                    setInt.intValue.Name = string.Empty;
                    setInt.intValue.Value = clamped;
                    continue;
                }

                if (action is SetFsmInt setFsm)
                {
                    string variableName = setFsm.variableName?.Value ?? string.Empty;
                    if (!IsKnownSummonCountVariableName(variableName))
                    {
                        continue;
                    }

                    if (setFsm.setValue == null)
                    {
                        continue;
                    }

                    setFsm.setValue.UseVariable = false;
                    setFsm.setValue.Name = string.Empty;
                    setFsm.setValue.Value = clamped;
                }
            }
        }
    }

    private static List<FsmInt> FindSummonCountVariables(PlayMakerFSM fsm)
    {
        List<FsmInt> result = [];
        if (fsm?.FsmVariables?.IntVariables == null)
        {
            return result;
        }

        foreach (string varName in SoulWarriorSummonCountVariableNames)
        {
            FsmInt? exact = fsm.FsmVariables.GetFsmInt(varName);
            if (exact != null && !result.Contains(exact))
            {
                result.Add(exact);
            }
        }

        if (result.Count > 0)
        {
            return result;
        }

        foreach (FsmInt intVar in fsm.FsmVariables.IntVariables)
        {
            if (intVar == null || !IsKnownSummonCountVariableName(intVar.Name))
            {
                continue;
            }

            if (!result.Contains(intVar))
            {
                result.Add(intVar);
            }
        }

        return result;
    }

    private static void RememberVanillaSummonPools(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSummonPoolsByFsm.ContainsKey(fsmId))
        {
            return;
        }

        Dictionary<string, GameObject[]> pools = new(StringComparer.Ordinal);
        foreach ((string variableName, _, List<GameObject> entries) in FindSummonPoolVariables(fsm))
        {
            if (entries.Count == 0)
            {
                continue;
            }

            pools[variableName] = entries.Where(go => go != null).ToArray();
        }

        vanillaSummonPoolsByFsm[fsmId] = pools;
    }

    private static int ResolveVanillaSummonPoolSize(PlayMakerFSM fsm)
    {
        RememberVanillaSummonPools(fsm);
        int fsmId = fsm.GetInstanceID();
        if (!vanillaSummonPoolsByFsm.TryGetValue(fsmId, out Dictionary<string, GameObject[]>? pools) || pools == null || pools.Count == 0)
        {
            return 0;
        }

        int max = 0;
        foreach (GameObject[] entries in pools.Values)
        {
            if (entries != null && entries.Length > max)
            {
                max = entries.Length;
            }
        }

        return max;
    }

    private static List<(string VariableName, FsmArray Array, List<GameObject> Entries)> FindSummonPoolVariables(PlayMakerFSM fsm)
    {
        List<(string VariableName, FsmArray Array, List<GameObject> Entries)> result = [];
        if (fsm?.FsmVariables?.ArrayVariables == null)
        {
            return result;
        }

        foreach (FsmArray array in fsm.FsmVariables.ArrayVariables)
        {
            if (array == null || array.Values == null || array.Values.Length == 0)
            {
                continue;
            }

            List<GameObject> entries = [];
            foreach (object value in array.Values)
            {
                if (value is not GameObject go || go == null || !IsSoulWarriorSummonObject(go))
                {
                    continue;
                }

                entries.Add(go);
            }

            if (entries.Count == 0)
            {
                continue;
            }

            result.Add((array.Name, array, entries));
        }

        return result;
    }

    private static void SetSummonPoolLimitOnFsm(PlayMakerFSM fsm, int limit)
    {
        int fsmId = fsm.GetInstanceID();
        RememberVanillaSummonPools(fsm);
        if (!vanillaSummonPoolsByFsm.TryGetValue(fsmId, out Dictionary<string, GameObject[]>? pools) || pools == null || pools.Count == 0)
        {
            if (ShouldUseCustomSummonLimit())
            {
                EnsureSceneSummonPoolSize(ClampSoulWarriorSummonLimit(limit));
            }

            return;
        }

        DestroyCreatedSummonClones(fsmId);
        List<GameObject> createdClones = [];
        int clampedLimit = ClampSoulWarriorSummonLimit(limit);

        foreach (KeyValuePair<string, GameObject[]> pair in pools)
        {
            FsmArray? poolVar = fsm.FsmVariables.FindFsmArray(pair.Key);
            if (poolVar == null)
            {
                continue;
            }

            List<GameObject> baseEntries = pair.Value.Where(go => go != null).ToList();
            if (baseEntries.Count == 0)
            {
                poolVar.Values = Array.Empty<object>();
                continue;
            }

            EnsureObjectPoolCapacity(baseEntries[0], clampedLimit);

            if (clampedLimit <= 0)
            {
                poolVar.Values = Array.Empty<object>();
                continue;
            }

            List<GameObject> targetEntries = [];
            int copyCount = Math.Min(clampedLimit, baseEntries.Count);
            for (int i = 0; i < copyCount; i++)
            {
                targetEntries.Add(baseEntries[i]);
            }

            int cloneIndex = 0;
            while (targetEntries.Count < clampedLimit)
            {
                GameObject source = baseEntries[cloneIndex % baseEntries.Count];
                GameObject? clone = CreateSummonPoolClone(source, targetEntries.Count + 1);
                cloneIndex++;
                if (clone == null)
                {
                    break;
                }

                createdClones.Add(clone);
                targetEntries.Add(clone);
            }

            poolVar.Values = targetEntries.Cast<object>().ToArray();
        }

        if (createdClones.Count > 0)
        {
            createdSummonClonesByFsm[fsmId] = createdClones;
        }

        List<FsmInt> summonCountVars = FindSummonCountVariables(fsm);
        if (summonCountVars.Count > 0)
        {
            int vanillaPoolSize = ResolveVanillaSummonPoolSize(fsm);
            foreach (FsmInt countVar in summonCountVars)
            {
                if (countVar != null && (countVar.Value <= 0 || countVar.Value == vanillaPoolSize))
                {
                    countVar.Value = clampedLimit;
                }
            }
        }

        if (ShouldUseCustomSummonLimit())
        {
            EnsureSceneSummonPoolSize(clampedLimit);
        }
    }

    private static void RestoreVanillaSummonPool(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (!vanillaSummonPoolsByFsm.TryGetValue(fsmId, out Dictionary<string, GameObject[]>? pools) || pools == null || pools.Count == 0)
        {
            return;
        }

        foreach (KeyValuePair<string, GameObject[]> pair in pools)
        {
            FsmArray? poolVar = fsm.FsmVariables.FindFsmArray(pair.Key);
            if (poolVar == null)
            {
                continue;
            }

            poolVar.Values = pair.Value.Cast<object>().ToArray();
        }

        DestroyCreatedSummonClones(fsmId);
    }

    private static GameObject? CreateSummonPoolClone(GameObject source, int index)
    {
        if (source == null)
        {
            return null;
        }

        GameObject clone = UObject.Instantiate(source);
        if (clone == null)
        {
            return null;
        }

        Transform sourceTransform = source.transform;
        Transform cloneTransform = clone.transform;
        if (sourceTransform != null && cloneTransform != null)
        {
            cloneTransform.SetParent(sourceTransform.parent, false);
            cloneTransform.localPosition = sourceTransform.localPosition;
            cloneTransform.localRotation = sourceTransform.localRotation;
            cloneTransform.localScale = sourceTransform.localScale;
        }

        clone.name = source.name;
        clone.SetActive(false);

        if (ShouldUseCustomSummonHp())
        {
            HealthManager hm = clone.GetComponent<HealthManager>();
            if (hm != null)
            {
                RememberVanillaSummonHp(hm);
                ApplySoulWarriorSummonHealth(clone, hm);
            }
        }

        return clone;
    }

    private static void EnsureSceneSummonPoolSize(int desiredCount)
    {
        int clampedDesired = ClampSoulWarriorSummonLimit(desiredCount);
        if (clampedDesired <= 0)
        {
            return;
        }

        List<GameObject> current = GetSoulWarriorSummonObjects(includeInactive: true);
        if (current.Count >= clampedDesired)
        {
            return;
        }

        List<GameObject> sources = current.Where(go => go != null).ToList();
        if (sources.Count == 0)
        {
            return;
        }

        int cloneIndex = 0;
        while (current.Count < clampedDesired)
        {
            GameObject source = sources[cloneIndex % sources.Count];
            GameObject? clone = CreateSummonPoolClone(source, current.Count + 1);
            cloneIndex++;
            if (clone == null)
            {
                break;
            }

            createdSceneSummonClones.Add(clone);
            current.Add(clone);
        }
    }

    private static List<GameObject> GetSoulWarriorSummonObjects(bool includeInactive)
    {
        Dictionary<int, GameObject> unique = new();
        IEnumerable<HealthManager> healthManagers = includeInactive
            ? Resources.FindObjectsOfTypeAll<HealthManager>()
            : UObject.FindObjectsOfType<HealthManager>();

        foreach (HealthManager hm in healthManagers)
        {
            if (hm == null || hm.gameObject == null || !IsSoulWarriorSummonObject(hm.gameObject))
            {
                continue;
            }

            if (includeInactive)
            {
                Scene scene = hm.gameObject.scene;
                if (!scene.IsValid())
                {
                    continue;
                }
            }

            int id = hm.gameObject.GetInstanceID();
            if (!unique.ContainsKey(id))
            {
                unique[id] = hm.gameObject;
            }
        }

        return unique.Values.ToList();
    }

    private static void DestroyCreatedSummonClones(int fsmId)
    {
        if (!createdSummonClonesByFsm.TryGetValue(fsmId, out List<GameObject>? clones) || clones == null)
        {
            return;
        }

        foreach (GameObject clone in clones)
        {
            if (clone != null)
            {
                UObject.Destroy(clone);
            }
        }

        createdSummonClonesByFsm.Remove(fsmId);
    }

    private static void DestroyAllCreatedSummonClones()
    {
        if (createdSummonClonesByFsm.Count == 0)
        {
            return;
        }

        foreach (List<GameObject> clones in createdSummonClonesByFsm.Values)
        {
            if (clones == null)
            {
                continue;
            }

            foreach (GameObject clone in clones)
            {
                if (clone != null)
                {
                    UObject.Destroy(clone);
                }
            }
        }

        createdSummonClonesByFsm.Clear();

        foreach (GameObject clone in createdSceneSummonClones)
        {
            if (clone != null)
            {
                UObject.Destroy(clone);
            }
        }

        createdSceneSummonClones.Clear();
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsSoulWarriorObject(boss))
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

        int targetHp = ClampSoulWarriorHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaSummonHealth(GameObject summon, HealthManager? hm = null)
    {
        if (summon == null || !IsSoulWarriorSummonObject(summon))
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

        int targetHp = ClampSoulWarriorHp(vanillaHp);
        summon.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindSoulWarriorHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsSoulWarrior(candidate))
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

        hp = Math.Max(hp, DefaultSoulWarriorVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
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

        if (hp <= 0)
        {
            hp = DefaultSoulWarriorSummonVanillaHp;
        }

        vanillaSummonHpByInstance[instanceId] = hp;
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultSoulWarriorVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultSoulWarriorVanillaHp);
        return hp > 0;
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

        if (hp <= 0)
        {
            hp = DefaultSoulWarriorSummonVanillaHp;
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

    private static int ClampSoulWarriorHp(int value)
    {
        if (value < MinSoulWarriorHp)
        {
            return MinSoulWarriorHp;
        }

        return value > MaxSoulWarriorHp ? MaxSoulWarriorHp : value;
    }

    private static int ClampSoulWarriorSummonLimit(int value)
    {
        if (value < MinSoulWarriorSummonLimit)
        {
            return MinSoulWarriorSummonLimit;
        }

        return value > MaxSoulWarriorSummonLimit ? MaxSoulWarriorSummonLimit : value;
    }
}
