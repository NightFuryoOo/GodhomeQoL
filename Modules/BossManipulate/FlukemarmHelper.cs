using Modding;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class FlukemarmHelper : Module
{
    private const string FlukemarmScene = "GG_Flukemarm";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string FlukemarmName = "Fluke Mother";
    private const string FlukeFlyName = "Fluke Fly";
    private const int DefaultFlukemarmMaxHp = 900;
    private const int DefaultFlukemarmVanillaHp = 900;
    private const int DefaultFlukeFlyHp = 35;
    private const int DefaultFlukeFlyVanillaHp = 35;
    private const int DefaultFlukeFlyPoolSize = 6;
    private const int DefaultFlukemarmSummonLimit = 6;
    private const int DefaultFlukemarmVanillaSummonLimit = 8;
    private const int P5FlukemarmHp = 500;
    private const int P5FlukeFlyHp = 35;
    private const int MinFlukemarmHp = 1;
    private const int MaxFlukemarmHp = 999999;
    private const int MinSummonLimit = DefaultFlukemarmSummonLimit;
    private const int MaxSummonLimit = 999;

    [LocalSetting]
    [BoolOption]
    internal static bool flukemarmUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool flukemarmP5Hp = false;

    [LocalSetting]
    internal static int flukemarmMaxHp = DefaultFlukemarmMaxHp;

    [LocalSetting]
    internal static int flukemarmFlyHp = DefaultFlukeFlyHp;

    [LocalSetting]
    internal static bool flukemarmUseCustomSummonLimit = false;

    [LocalSetting]
    internal static int flukemarmSummonLimit = DefaultFlukemarmSummonLimit;

    [LocalSetting]
    internal static int flukemarmMaxHpBeforeP5 = DefaultFlukemarmMaxHp;

    [LocalSetting]
    internal static int flukemarmFlyHpBeforeP5 = DefaultFlukeFlyHp;

    [LocalSetting]
    internal static bool flukemarmUseCustomSummonLimitBeforeP5 = false;

    [LocalSetting]
    internal static int flukemarmSummonLimitBeforeP5 = DefaultFlukemarmSummonLimit;

    [LocalSetting]
    internal static bool flukemarmUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool flukemarmHasStoredStateBeforeP5 = false;

    private enum FlukemarmTarget
    {
        Unknown = 0,
        Boss = 1,
        Fly = 2,
    }

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static readonly Dictionary<int, int> vanillaSummonLimitByFsm = new();
    private static readonly List<GameObject> customFlukeFlyPoolEntries = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;
    private static int vanillaFlukeFlyPoolSize = -1;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        NormalizeP5State();
        vanillaHpByInstance.Clear();
        vanillaSummonLimitByFsm.Clear();
        customFlukeFlyPoolEntries.Clear();
        vanillaFlukeFlyPoolSize = -1;
        On.SetHP.OnEnter += OnSetHpEnter;
        On.HealthManager.OnEnable += OnHealthManagerEnable;
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
        RestoreVanillaSummonLimitsIfPresent();
        moduleActive = false;
        On.SetHP.OnEnter -= OnSetHpEnter;
        On.HealthManager.OnEnable -= OnHealthManagerEnable;
        On.HealthManager.Awake -= OnHealthManagerAwake;
        On.HealthManager.Start -= OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
        vanillaSummonLimitByFsm.Clear();
        customFlukeFlyPoolEntries.Clear();
        vanillaFlukeFlyPoolSize = -1;
        hoGEntryAllowed = false;
    }

    internal static void ReapplyLiveSettings()
    {
        if (!moduleActive)
        {
            return;
        }

        ApplyFlukemarmHealthIfPresent();
        RestoreVanillaHealthIfPresent();
        ApplySummonLimitSettingsIfPresent();
    }

    internal static void SetP5HpEnabled(bool value)
    {
        if (value)
        {
            if (!flukemarmP5Hp)
            {
                flukemarmMaxHpBeforeP5 = ClampFlukemarmHp(flukemarmMaxHp);
                flukemarmFlyHpBeforeP5 = ClampFlukemarmHp(flukemarmFlyHp);
                flukemarmUseMaxHpBeforeP5 = flukemarmUseMaxHp;
                flukemarmUseCustomSummonLimitBeforeP5 = flukemarmUseCustomSummonLimit;
                flukemarmSummonLimitBeforeP5 = ClampSummonLimit(flukemarmSummonLimit);
                flukemarmHasStoredStateBeforeP5 = true;
            }

            flukemarmP5Hp = true;
            flukemarmUseMaxHp = true;
            flukemarmUseCustomSummonLimit = false;
            flukemarmMaxHp = P5FlukemarmHp;
            flukemarmFlyHp = P5FlukeFlyHp;
        }
        else
        {
            if (flukemarmP5Hp && flukemarmHasStoredStateBeforeP5)
            {
                flukemarmMaxHp = ClampFlukemarmHp(flukemarmMaxHpBeforeP5);
                flukemarmFlyHp = ClampFlukemarmHp(flukemarmFlyHpBeforeP5);
                flukemarmUseMaxHp = flukemarmUseMaxHpBeforeP5;
                flukemarmUseCustomSummonLimit = flukemarmUseCustomSummonLimitBeforeP5;
                flukemarmSummonLimit = ClampSummonLimit(flukemarmSummonLimitBeforeP5);
            }

            flukemarmP5Hp = false;
            flukemarmHasStoredStateBeforeP5 = false;
        }

        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        flukemarmSummonLimit = ClampSummonLimit(flukemarmSummonLimit);
        if (!flukemarmP5Hp)
        {
            return;
        }

        if (!flukemarmHasStoredStateBeforeP5)
        {
            flukemarmMaxHpBeforeP5 = ClampFlukemarmHp(flukemarmMaxHp);
            flukemarmFlyHpBeforeP5 = ClampFlukemarmHp(flukemarmFlyHp);
            flukemarmUseMaxHpBeforeP5 = flukemarmUseMaxHp;
            flukemarmUseCustomSummonLimitBeforeP5 = flukemarmUseCustomSummonLimit;
            flukemarmSummonLimitBeforeP5 = ClampSummonLimit(flukemarmSummonLimit);
            flukemarmHasStoredStateBeforeP5 = true;
        }

        flukemarmUseMaxHp = true;
        flukemarmUseCustomSummonLimit = false;
        flukemarmMaxHp = P5FlukemarmHp;
        flukemarmFlyHp = P5FlukeFlyHp;
    }

    internal static void ApplyFlukemarmHealthIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsFlukemarm(hm))
            {
                continue;
            }

            FlukemarmTarget target = GetFlukemarmTarget(hm.gameObject);
            if (!ShouldUseCustomHp(target))
            {
                continue;
            }

            ApplyFlukemarmHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsFlukemarm(hm))
            {
                continue;
            }

            FlukemarmTarget target = GetFlukemarmTarget(hm.gameObject);
            if (ShouldUseCustomHp(target))
            {
                continue;
            }

            RestoreVanillaHealth(hm.gameObject, hm);
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

    internal static void RestoreVanillaSummonLimitsIfPresent()
    {
        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsFlukemarmSummonControlFsm(fsm))
            {
                continue;
            }

            if (!IsFlukemarmObject(fsm.gameObject))
            {
                continue;
            }

            RememberVanillaSummonLimit(fsm);
            SetSummonLimitOnFlukemarmFsm(fsm, GetVanillaSummonLimit(fsm));
        }

        DestroyCustomFlukeFlyPoolEntries();
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

    private static void OnHealthManagerEnable(On.HealthManager.orig_OnEnable orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsFlukemarm(self))
        {
            return;
        }

        // Fluke Fly can be pooled and re-enabled without Awake/Start.
        if (GetFlukemarmTarget(self.gameObject) != FlukemarmTarget.Fly)
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        if (ShouldUseCustomHp(FlukemarmTarget.Fly))
        {
            ApplyFlukemarmHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnSetHpEnter(On.SetHP.orig_OnEnter orig, SetHP self)
    {
        orig(self);

        if (!moduleActive || self == null)
        {
            return;
        }

        GameObject targetObject = self.target.GetSafe(self);
        FlukemarmTarget target = GetFlukemarmTarget(targetObject);
        if (targetObject == null || target != FlukemarmTarget.Fly || !ShouldUseCustomHp(target))
        {
            return;
        }

        HealthManager hm = targetObject.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        RememberVanillaHp(hm);
        if (!ShouldApplySettings(targetObject))
        {
            return;
        }

        // Vanilla Fluke Fly spawner writes hp=13 in Reset. Reapply custom HP after it.
        ApplyFlukemarmHealth(targetObject, hm);
    }

    private static void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsFlukemarm(self))
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        FlukemarmTarget target = GetFlukemarmTarget(self.gameObject);
        if (ShouldUseCustomHp(target))
        {
            ApplyFlukemarmHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsFlukemarm(self))
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        FlukemarmTarget target = GetFlukemarmTarget(self.gameObject);
        if (ShouldUseCustomHp(target))
        {
            ApplyFlukemarmHealth(self.gameObject, self);
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

        if (!moduleActive || hm == null || hm.gameObject == null || !IsFlukemarm(hm))
        {
            yield break;
        }

        if (ShouldApplySettings(hm.gameObject))
        {
            FlukemarmTarget target = GetFlukemarmTarget(hm.gameObject);
            if (ShouldUseCustomHp(target))
            {
                ApplyFlukemarmHealth(hm.gameObject, hm);
                yield return new WaitForSeconds(0.01f);
                if (moduleActive && hm != null && hm.gameObject != null && IsFlukemarm(hm) && ShouldApplySettings(hm.gameObject) && ShouldUseCustomHp(GetFlukemarmTarget(hm.gameObject)))
                {
                    ApplyFlukemarmHealth(hm.gameObject, hm);
                }
            }
            else
            {
                RestoreVanillaHealth(hm.gameObject, hm);
            }
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, FlukemarmScene, StringComparison.Ordinal))
        {
            DestroyCustomFlukeFlyPoolEntries();
            vanillaHpByInstance.Clear();
            vanillaSummonLimitByFsm.Clear();
            vanillaFlukeFlyPoolSize = -1;
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        ApplyFlukemarmHealthIfPresent();
        RestoreVanillaHealthIfPresent();
        ApplySummonLimitSettingsIfPresent();
    }

    private static string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);
        return newSceneName;
    }

    private static bool IsFlukemarm(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsFlukemarmObject(hm.gameObject);
    }

    private static bool IsFlukemarmObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        if (!string.Equals(gameObject.scene.name, FlukemarmScene, StringComparison.Ordinal))
        {
            return false;
        }

        return GetFlukemarmTarget(gameObject) != FlukemarmTarget.Unknown;
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsFlukemarmObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp(FlukemarmTarget target)
    {
        return target switch
        {
            FlukemarmTarget.Boss => flukemarmUseMaxHp,
            FlukemarmTarget.Fly => flukemarmUseMaxHp && !flukemarmP5Hp,
            _ => false,
        };
    }

    private static bool ShouldUseCustomSummonLimit() => flukemarmUseCustomSummonLimit && !flukemarmP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, FlukemarmScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, FlukemarmScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static void ApplyFlukemarmHealth(GameObject target, HealthManager? hm = null)
    {
        if (!ShouldApplySettings(target))
        {
            return;
        }

        FlukemarmTarget flukemarmTarget = GetFlukemarmTarget(target);
        if (flukemarmTarget == FlukemarmTarget.Unknown)
        {
            return;
        }

        if (!ShouldUseCustomHp(flukemarmTarget))
        {
            return;
        }

        hm ??= target.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        int configuredHp = GetConfiguredHp(flukemarmTarget);
        if (configuredHp <= 0)
        {
            return;
        }

        RememberVanillaHp(hm);
        int targetHp = ClampFlukemarmHp(configuredHp);
        target.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void ApplySummonLimitSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsFlukemarmSummonControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        RememberVanillaSummonLimit(fsm);
        int targetLimit = ShouldUseCustomSummonLimit()
            ? ClampSummonLimit(flukemarmSummonLimit)
            : GetVanillaSummonLimit(fsm);
        SyncFlukeFlyPoolSize(fsm, targetLimit);
        SetSummonLimitOnFlukemarmFsm(fsm, targetLimit);
    }

    private static void SyncFlukeFlyPoolSize(PlayMakerFSM fsm, int targetLimit)
    {
        RemoveDestroyedPoolEntries();

        if (!ShouldUseCustomSummonLimit())
        {
            DestroyCustomFlukeFlyPoolEntries();
            return;
        }

        GameObject? cage = ResolveCageObject(fsm);
        if (cage == null)
        {
            return;
        }

        int currentPoolSize = CountFlukeFlyPoolEntries(cage);
        if (vanillaFlukeFlyPoolSize < 0)
        {
            vanillaFlukeFlyPoolSize = Math.Max(DefaultFlukeFlyPoolSize, currentPoolSize);
        }

        int requiredPoolSize = Math.Max(
            Math.Max(DefaultFlukeFlyPoolSize, vanillaFlukeFlyPoolSize),
            ClampSummonLimit(targetLimit)
        );

        int toCreate = requiredPoolSize - currentPoolSize;
        if (toCreate <= 0)
        {
            return;
        }

        GameObject? template = FindFlukeFlyPoolTemplate(cage);
        if (template == null)
        {
            return;
        }

        for (int i = 0; i < toCreate; i++)
        {
            GameObject clone = UObject.Instantiate(template, cage.transform);
            clone.name = $"{template.name} (Custom Pool)";
            customFlukeFlyPoolEntries.Add(clone);

            HealthManager hm = clone.GetComponent<HealthManager>();
            if (hm != null)
            {
                RememberVanillaHp(hm);
                if (ShouldUseCustomHp(FlukemarmTarget.Fly) && ShouldApplySettings(clone))
                {
                    ApplyFlukemarmHealth(clone, hm);
                }
            }
        }
    }

    private static GameObject? ResolveCageObject(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null)
        {
            return null;
        }

        FsmGameObject? cageVar = fsm.FsmVariables?.FindFsmGameObject("Cage");
        if (cageVar?.Value != null)
        {
            return cageVar.Value;
        }

        Transform bossRoot = fsm.gameObject.transform;
        Transform? directCage = bossRoot.Find("Cage");
        if (directCage != null)
        {
            return directCage.gameObject;
        }

        foreach (Transform child in bossRoot.GetComponentsInChildren<Transform>(true))
        {
            if (child != null && string.Equals(child.name, "Cage", StringComparison.Ordinal))
            {
                return child.gameObject;
            }
        }

        return null;
    }

    private static GameObject? FindFlukeFlyPoolTemplate(GameObject cage)
    {
        if (cage == null)
        {
            return null;
        }

        foreach (Transform child in cage.transform)
        {
            if (child == null || child.gameObject == null)
            {
                continue;
            }

            if (GetFlukemarmTarget(child.gameObject) == FlukemarmTarget.Fly)
            {
                return child.gameObject;
            }
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsFlukemarmObject(fsm.gameObject))
            {
                continue;
            }

            if (!string.Equals(fsm.FsmName, "Fluke Fly", StringComparison.Ordinal))
            {
                continue;
            }

            return fsm.gameObject;
        }

        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsFlukemarmObject(hm.gameObject))
            {
                continue;
            }

            if (GetFlukemarmTarget(hm.gameObject) == FlukemarmTarget.Fly)
            {
                return hm.gameObject;
            }
        }

        return null;
    }

    private static int CountFlukeFlyPoolEntries(GameObject cage)
    {
        if (cage == null)
        {
            return 0;
        }

        int count = 0;
        foreach (Transform child in cage.transform)
        {
            if (child == null || child.gameObject == null)
            {
                continue;
            }

            if (GetFlukemarmTarget(child.gameObject) == FlukemarmTarget.Fly)
            {
                count++;
            }
        }

        return count;
    }

    private static void RemoveDestroyedPoolEntries()
    {
        for (int i = customFlukeFlyPoolEntries.Count - 1; i >= 0; i--)
        {
            if (customFlukeFlyPoolEntries[i] == null)
            {
                customFlukeFlyPoolEntries.RemoveAt(i);
            }
        }
    }

    private static void DestroyCustomFlukeFlyPoolEntries()
    {
        if (customFlukeFlyPoolEntries.Count == 0)
        {
            return;
        }

        for (int i = customFlukeFlyPoolEntries.Count - 1; i >= 0; i--)
        {
            GameObject entry = customFlukeFlyPoolEntries[i];
            if (entry != null)
            {
                UObject.Destroy(entry);
            }
        }

        customFlukeFlyPoolEntries.Clear();
    }

    private static bool IsFlukemarmSummonControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null)
        {
            return false;
        }

        if (GetFlukemarmTarget(fsm.gameObject) != FlukemarmTarget.Boss)
        {
            return false;
        }

        return fsm.FsmVariables?.FindFsmInt("Spawned Max") != null
            || fsm.Fsm?.GetState("Check Spawn") != null;
    }

    private static void SetSummonLimitOnFlukemarmFsm(PlayMakerFSM fsm, int value)
    {
        int clampedLimit = ClampSummonLimit(value);

        FsmInt? spawnedMax = fsm.FsmVariables?.FindFsmInt("Spawned Max");
        if (spawnedMax != null)
        {
            spawnedMax.Value = clampedLimit;
        }

        FsmState? checkSpawn = fsm.Fsm?.GetState("Check Spawn");
        if (checkSpawn?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in checkSpawn.Actions)
        {
            if (action is IntCompare compare && compare.integer2 != null)
            {
                compare.integer2.Value = clampedLimit;
            }
        }
    }

    private static void RememberVanillaSummonLimit(PlayMakerFSM fsm)
    {
        int id = fsm.GetInstanceID();
        if (vanillaSummonLimitByFsm.ContainsKey(id))
        {
            return;
        }

        FsmInt? spawnedMax = fsm.FsmVariables?.FindFsmInt("Spawned Max");
        int value = spawnedMax?.Value ?? DefaultFlukemarmVanillaSummonLimit;
        vanillaSummonLimitByFsm[id] = ClampSummonLimit(value);
    }

    private static int GetVanillaSummonLimit(PlayMakerFSM fsm)
    {
        int id = fsm.GetInstanceID();
        if (vanillaSummonLimitByFsm.TryGetValue(id, out int stored))
        {
            return ClampSummonLimit(stored);
        }

        FsmInt? spawnedMax = fsm.FsmVariables?.FindFsmInt("Spawned Max");
        int value = ClampSummonLimit(spawnedMax?.Value ?? DefaultFlukemarmVanillaSummonLimit);
        vanillaSummonLimitByFsm[id] = value;
        return value;
    }

    private static void RestoreVanillaHealth(GameObject target, HealthManager? hm = null)
    {
        if (target == null || !IsFlukemarmObject(target))
        {
            return;
        }

        FlukemarmTarget flukemarmTarget = GetFlukemarmTarget(target);
        if (flukemarmTarget == FlukemarmTarget.Unknown)
        {
            return;
        }

        if (ShouldUseCustomHp(flukemarmTarget))
        {
            return;
        }

        hm ??= target.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        if (!TryGetVanillaHp(hm, flukemarmTarget, out int vanillaHp))
        {
            return;
        }

        int targetHp = ClampFlukemarmHp(vanillaHp);
        target.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static FlukemarmTarget GetFlukemarmTarget(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return FlukemarmTarget.Unknown;
        }

        string name = gameObject.name;
        if (name.StartsWith(FlukemarmName, StringComparison.Ordinal))
        {
            return FlukemarmTarget.Boss;
        }

        if (name.StartsWith(FlukeFlyName, StringComparison.Ordinal) || name.IndexOf(FlukeFlyName, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return FlukemarmTarget.Fly;
        }

        return FlukemarmTarget.Unknown;
    }

    private static int GetConfiguredHp(FlukemarmTarget target)
    {
        return target switch
        {
            FlukemarmTarget.Boss => flukemarmMaxHp,
            FlukemarmTarget.Fly => flukemarmFlyHp,
            _ => 0,
        };
    }

    private static int GetDefaultVanillaHp(FlukemarmTarget target)
    {
        return target switch
        {
            FlukemarmTarget.Fly => DefaultFlukeFlyVanillaHp,
            _ => DefaultFlukemarmVanillaHp,
        };
    }

    private static void RememberVanillaHp(HealthManager hm)
    {
        int instanceId = hm.GetInstanceID();
        if (vanillaHpByInstance.ContainsKey(instanceId))
        {
            return;
        }

        FlukemarmTarget target = GetFlukemarmTarget(hm.gameObject);
        if (target == FlukemarmTarget.Unknown)
        {
            return;
        }

        int hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, GetDefaultVanillaHp(target));
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, FlukemarmTarget target, out int hp)
    {
        int minimumVanillaHp = GetDefaultVanillaHp(target);
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, minimumVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, minimumVanillaHp);
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

    private static int ClampFlukemarmHp(int value)
    {
        if (value < MinFlukemarmHp)
        {
            return MinFlukemarmHp;
        }

        return value > MaxFlukemarmHp ? MaxFlukemarmHp : value;
    }

    private static int ClampSummonLimit(int value)
    {
        if (value < MinSummonLimit)
        {
            return MinSummonLimit;
        }

        return value > MaxSummonLimit ? MaxSummonLimit : value;
    }
}
