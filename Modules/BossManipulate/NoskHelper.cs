using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class NoskHelper : Module
{
    private const string NoskScene = "GG_Nosk_V";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string NoskName = "Mimic Spider";
    private const string NoskPhaseCheckStateName = "Roof Jump?";
    private const int DefaultNoskMaxHp = 980;
    private const int DefaultNoskVanillaHp = 680;
    private const int DefaultNoskPhase2Hp = 560;
    private const int MinNoskHp = 1;
    private const int MaxNoskHp = 999999;
    private const int MinNoskPhase2Hp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool noskUseMaxHp = false;

    [LocalSetting]
    internal static int noskMaxHp = DefaultNoskMaxHp;

    [LocalSetting]
    internal static bool noskUseCustomPhase = false;

    [LocalSetting]
    internal static int noskPhase2Hp = DefaultNoskPhase2Hp;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        NormalizePhaseThresholdState();
        vanillaHpByInstance.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_Nosk;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_Nosk;
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
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_Nosk;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_Nosk;
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
            ApplyNoskHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    private static void NormalizePhaseThresholdState()
    {
        noskPhase2Hp = ClampNoskPhase2Hp(noskPhase2Hp, ResolvePhase2MaxHp());
    }

    internal static void ApplyNoskHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindNoskHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyNoskHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindNoskHealthManager(out HealthManager? hm))
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

        if (!moduleActive || !IsNosk(self))
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
            ApplyNoskHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsNosk(self))
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
            ApplyNoskHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnPlayMakerFsmOnEnable_Nosk(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsNoskPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static void OnPlayMakerFsmStart_Nosk(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsNoskPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsNosk(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyNoskHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsNosk(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyNoskHealth(hm.gameObject, hm);
            }
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, NoskScene, StringComparison.Ordinal))
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
            ApplyNoskHealthIfPresent();
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

    private static bool IsNosk(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsNoskObject(hm.gameObject);
    }

    private static bool IsNoskObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, NoskScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(NoskName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsNoskObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => noskUseMaxHp;
    private static bool ShouldUseCustomPhaseThreshold() => noskUseCustomPhase;

    internal static void ApplyPhaseThresholdSettingsIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsNoskPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsNoskPhaseControlFsm(fsm))
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
        if (string.Equals(nextScene, NoskScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, NoskScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsNoskPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsNoskObject(fsm.gameObject))
        {
            return false;
        }

        return fsm.Fsm?.GetState(NoskPhaseCheckStateName) != null;
    }

    private static bool ShouldApplyPhaseSettings(GameObject? gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return IsNoskObject(gameObject) && hoGEntryAllowed;
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsNoskPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplyPhaseSettings(fsm.gameObject))
        {
            return;
        }

        int threshold = ShouldUseCustomPhaseThreshold()
            ? ClampNoskPhase2Hp(noskPhase2Hp, ResolvePhase2MaxHp())
            : GetVanillaPhase2Hp();

        SetPhase2ThresholdOnFsm(fsm, threshold);
    }

    private static void SetPhase2ThresholdOnFsm(PlayMakerFSM fsm, int value)
    {
        if (fsm == null)
        {
            return;
        }

        int threshold = Math.Max(MinNoskPhase2Hp, value);
        FsmState? checkState = fsm.Fsm?.GetState(NoskPhaseCheckStateName);
        if (checkState?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in checkState.Actions)
        {
            if (action is not IntCompare compare || !IsHpCompare(compare))
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
        return DefaultNoskPhase2Hp;
    }

    private static bool IsHpCompare(IntCompare compare)
    {
        return (compare.integer1 != null
            && string.Equals(compare.integer1.Name, "HP", StringComparison.Ordinal))
            || (compare.integer2 != null
                && string.Equals(compare.integer2.Name, "HP", StringComparison.Ordinal));
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

    private static void ApplyNoskHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampNoskHp(noskMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsNoskObject(boss))
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

        int targetHp = ClampNoskHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindNoskHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsNosk(candidate))
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

        hp = Math.Max(hp, DefaultNoskVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultNoskVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultNoskVanillaHp);
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

    private static int ClampNoskHp(int value)
    {
        if (value < MinNoskHp)
        {
            return MinNoskHp;
        }

        return value > MaxNoskHp ? MaxNoskHp : value;
    }

    internal static int GetPhase2MaxHpForUi()
    {
        return ResolvePhase2MaxHp();
    }

    private static int ClampNoskPhase2Hp(int value, int maxHp)
    {
        int clampedMaxHp = ClampNoskHp(maxHp);
        if (value < MinNoskPhase2Hp)
        {
            return MinNoskPhase2Hp;
        }

        return value > clampedMaxHp ? clampedMaxHp : value;
    }

    private static int ResolvePhase2MaxHp()
    {
        if (ShouldUseCustomHp())
        {
            return ClampNoskHp(noskMaxHp);
        }

        if (TryFindNoskHealthManager(out HealthManager? hm) && hm != null && TryGetVanillaHp(hm, out int vanillaHp))
        {
            return ClampNoskHp(vanillaHp);
        }

        return DefaultNoskVanillaHp;
    }
}
