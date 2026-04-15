using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class TraitorLordHelper : Module
{
    private const string TraitorLordScene = "GG_Traitor_Lord";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string TraitorLordName = "Mantis Traitor Lord";
    private const string TraitorLordPhaseFsmName = "Mantis";
    private const string TraitorLordPhaseCheckStateName = "Slam?";
    private const int DefaultTraitorLordMaxHp = 1300;
    private const int DefaultTraitorLordVanillaHp = 1300;
    private const int DefaultTraitorLordPhase2Hp = 500;
    private const int P5TraitorLordHp = 800;
    private const int MinTraitorLordHp = 1;
    private const int MaxTraitorLordHp = 999999;
    private const int MinTraitorLordPhase2Hp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool traitorLordUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool traitorLordP5Hp = false;

    [LocalSetting]
    internal static int traitorLordMaxHp = DefaultTraitorLordMaxHp;

    [LocalSetting]
    internal static int traitorLordMaxHpBeforeP5 = DefaultTraitorLordMaxHp;

    [LocalSetting]
    internal static bool traitorLordUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool traitorLordHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool traitorLordUseCustomPhase = false;

    [LocalSetting]
    internal static int traitorLordPhase2Hp = DefaultTraitorLordPhase2Hp;

    [LocalSetting]
    internal static bool traitorLordUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int traitorLordPhase2HpBeforeP5 = DefaultTraitorLordPhase2Hp;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        NormalizeP5State();
        NormalizePhaseThresholdState();
        vanillaHpByInstance.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_TraitorLord;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_TraitorLord;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresent();
        RestoreVanillaPhaseThresholdsIfPresent();
        moduleActive = false;
        On.HealthManager.Awake -= OnHealthManagerAwake;
        On.HealthManager.Start -= OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_TraitorLord;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_TraitorLord;
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
            ApplyTraitorLordHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    internal static void SetP5HpEnabled(bool value)
    {
        if (value)
        {
            if (!traitorLordP5Hp)
            {
                traitorLordMaxHpBeforeP5 = ClampTraitorLordHp(traitorLordMaxHp);
                traitorLordUseMaxHpBeforeP5 = traitorLordUseMaxHp;
                traitorLordUseCustomPhaseBeforeP5 = traitorLordUseCustomPhase;
                traitorLordPhase2HpBeforeP5 = ClampTraitorLordPhase2Hp(traitorLordPhase2Hp, ResolvePhase2MaxHp());
                traitorLordHasStoredStateBeforeP5 = true;
            }

            traitorLordP5Hp = true;
            traitorLordUseMaxHp = true;
            traitorLordUseCustomPhase = false;
            traitorLordMaxHp = P5TraitorLordHp;
        }
        else
        {
            if (traitorLordP5Hp && traitorLordHasStoredStateBeforeP5)
            {
                traitorLordMaxHp = ClampTraitorLordHp(traitorLordMaxHpBeforeP5);
                traitorLordUseMaxHp = traitorLordUseMaxHpBeforeP5;
                traitorLordUseCustomPhase = traitorLordUseCustomPhaseBeforeP5;
                traitorLordPhase2Hp = ClampTraitorLordPhase2Hp(traitorLordPhase2HpBeforeP5, ResolvePhase2MaxHp());
            }

            traitorLordP5Hp = false;
            traitorLordHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!traitorLordP5Hp)
        {
            return;
        }

        if (!traitorLordHasStoredStateBeforeP5)
        {
            traitorLordMaxHpBeforeP5 = ClampTraitorLordHp(traitorLordMaxHp);
            traitorLordUseMaxHpBeforeP5 = traitorLordUseMaxHp;
            traitorLordUseCustomPhaseBeforeP5 = traitorLordUseCustomPhase;
            traitorLordPhase2HpBeforeP5 = ClampTraitorLordPhase2Hp(traitorLordPhase2Hp, ResolvePhase2MaxHp());
            traitorLordHasStoredStateBeforeP5 = true;
        }

        traitorLordUseMaxHp = true;
        traitorLordUseCustomPhase = false;
        traitorLordMaxHp = P5TraitorLordHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        traitorLordPhase2Hp = ClampTraitorLordPhase2Hp(traitorLordPhase2Hp, ResolvePhase2MaxHp());
    }

    internal static void ApplyTraitorLordHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindTraitorLordHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyTraitorLordHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindTraitorLordHealthManager(out HealthManager? hm))
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

        if (!moduleActive || !IsTraitorLord(self))
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
            ApplyTraitorLordHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsTraitorLord(self))
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
            ApplyTraitorLordHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnPlayMakerFsmOnEnable_TraitorLord(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsTraitorLordPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static void OnPlayMakerFsmStart_TraitorLord(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsTraitorLordPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsTraitorLord(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyTraitorLordHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsTraitorLord(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyTraitorLordHealth(hm.gameObject, hm);
            }
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, TraitorLordScene, StringComparison.Ordinal))
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
            ApplyTraitorLordHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    private static string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);
        return newSceneName;
    }

    private static bool IsTraitorLord(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsTraitorLordObject(hm.gameObject);
    }

    private static bool IsTraitorLordObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, TraitorLordScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(TraitorLordName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsTraitorLordObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => traitorLordUseMaxHp;
    private static bool ShouldUseCustomPhaseThreshold() => traitorLordUseCustomPhase && !traitorLordP5Hp;

    internal static void ApplyPhaseThresholdSettingsIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsTraitorLordPhaseControlFsm(fsm))
            {
                continue;
            }

            ApplyPhaseThresholdSettings(fsm);
        }
    }

    internal static void RestoreVanillaPhaseThresholdsIfPresent()
    {
        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsTraitorLordPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplyPhaseSettings(fsm.gameObject))
            {
                continue;
            }

            SetPhase2ThresholdOnFsm(fsm, GetVanillaPhase2Hp());
        }
    }

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, TraitorLordScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, TraitorLordScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsTraitorLordPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsTraitorLordObject(fsm.gameObject))
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, TraitorLordPhaseFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState(TraitorLordPhaseCheckStateName) != null;
    }

    private static bool ShouldApplyPhaseSettings(GameObject? gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return IsTraitorLordObject(gameObject) && hoGEntryAllowed;
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsTraitorLordPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplyPhaseSettings(fsm.gameObject))
        {
            return;
        }

        int threshold = ShouldUseCustomPhaseThreshold()
            ? ClampTraitorLordPhase2Hp(traitorLordPhase2Hp, ResolvePhase2MaxHp())
            : GetVanillaPhase2Hp();

        SetPhase2ThresholdOnFsm(fsm, threshold);
    }

    private static void SetPhase2ThresholdOnFsm(PlayMakerFSM fsm, int value)
    {
        if (fsm == null)
        {
            return;
        }

        int threshold = Math.Max(MinTraitorLordPhase2Hp, value);
        FsmState? checkState = fsm.Fsm?.GetState(TraitorLordPhaseCheckStateName);
        if (checkState?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in checkState.Actions)
        {
            if (action is not IntCompare compare)
            {
                continue;
            }

            FsmInt? thresholdOperand = ResolveThresholdOperand(compare);
            if (thresholdOperand == null)
            {
                continue;
            }

            thresholdOperand.UseVariable = false;
            thresholdOperand.Name = string.Empty;
            thresholdOperand.Value = threshold;
        }
    }

    private static int GetVanillaPhase2Hp()
    {
        return DefaultTraitorLordPhase2Hp;
    }

    private static FsmInt? ResolveThresholdOperand(IntCompare compare)
    {
        if (compare == null)
        {
            return null;
        }

        FsmInt? integer1 = compare.integer1;
        FsmInt? integer2 = compare.integer2;
        if (integer1 == null && integer2 == null)
        {
            return null;
        }

        if (integer1 != null && string.Equals(integer1.Name, "HP", StringComparison.Ordinal))
        {
            return integer2 ?? integer1;
        }

        if (integer2 != null && string.Equals(integer2.Name, "HP", StringComparison.Ordinal))
        {
            return integer1 ?? integer2;
        }

        return integer2 ?? integer1;
    }

    private static void ApplyTraitorLordHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampTraitorLordHp(traitorLordMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsTraitorLordObject(boss))
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

        int targetHp = ClampTraitorLordHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindTraitorLordHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsTraitorLord(candidate))
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

        hp = Math.Max(hp, DefaultTraitorLordVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultTraitorLordVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultTraitorLordVanillaHp);
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

    private static int ClampTraitorLordHp(int value)
    {
        if (value < MinTraitorLordHp)
        {
            return MinTraitorLordHp;
        }

        return value > MaxTraitorLordHp ? MaxTraitorLordHp : value;
    }

    internal static int GetPhase2MaxHpForUi()
    {
        return ResolvePhase2MaxHp();
    }

    private static int ClampTraitorLordPhase2Hp(int value, int maxHp)
    {
        int clampedMaxHp = ClampTraitorLordHp(maxHp);
        if (value < MinTraitorLordPhase2Hp)
        {
            return MinTraitorLordPhase2Hp;
        }

        return value > clampedMaxHp ? clampedMaxHp : value;
    }

    private static int ResolvePhase2MaxHp()
    {
        if (ShouldUseCustomHp())
        {
            return ClampTraitorLordHp(traitorLordMaxHp);
        }

        if (TryFindTraitorLordHealthManager(out HealthManager? hm) && hm != null && TryGetVanillaHp(hm, out int vanillaHp))
        {
            return ClampTraitorLordHp(vanillaHp);
        }

        return DefaultTraitorLordVanillaHp;
    }
}
