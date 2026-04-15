using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class MarkothP4Helper : Module
{
    private const string MarkothScene = "GG_Ghost_Markoth";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string MarkothName = "Ghost Warrior Markoth";
    private const string MarkothPhaseFsmName = "Rage Check";
    private const string MarkothPhaseCheckStateName = "Check";
    private const string MarkothPhase2VariableName = "Rage HP";
    private const string MarkothAttackingFsmName = "Attacking";
    private const string MarkothShieldAttackFsmName = "Shield Attack";
    private const string MarkothRageVariableName = "Rage";
    private const string MarkothRageEventName = "RAGE";
    private const int DefaultMarkothMaxHp = 650;
    private const int DefaultMarkothVanillaHp = 650;
    private const int DefaultMarkothPhase2Hp = 325;
    private const int MinMarkothHp = 1;
    private const int MaxMarkothHp = 999999;
    private const int MinMarkothPhase2Hp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool markothP4UseMaxHp = false;

    [LocalSetting]
    internal static int markothP4MaxHp = DefaultMarkothMaxHp;

    [LocalSetting]
    internal static bool markothP4UseCustomPhase = false;

    [LocalSetting]
    internal static int markothP4Phase2Hp = DefaultMarkothPhase2Hp;

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
        On.HealthManager.Update += OnHealthManagerUpdate;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_Markoth;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_Markoth;
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
        On.HealthManager.Update -= OnHealthManagerUpdate;
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_Markoth;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_Markoth;
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
            ApplyMarkothHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    private static void NormalizePhaseThresholdState()
    {
        markothP4Phase2Hp = ClampMarkothPhase2Hp(markothP4Phase2Hp, ResolvePhase2MaxHp());
    }

    internal static void ApplyMarkothHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindMarkothHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyMarkothHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindMarkothHealthManager(out HealthManager? hm))
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

        if (!moduleActive || !IsMarkoth(self))
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
            ApplyMarkothHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsMarkoth(self))
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
            ApplyMarkothHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerUpdate(On.HealthManager.orig_Update orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsMarkoth(self))
        {
            return;
        }

        if (!ShouldApplySettings(self.gameObject) || !ShouldUseCustomPhaseThreshold())
        {
            return;
        }

        int threshold = ClampMarkothPhase2Hp(markothP4Phase2Hp, ResolvePhase2MaxHp());
        if (self.hp > threshold)
        {
            return;
        }

        TryForceMarkothRage(self.gameObject);
    }

    private static void OnPlayMakerFsmOnEnable_Markoth(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsMarkothPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static void OnPlayMakerFsmStart_Markoth(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsMarkothPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsMarkoth(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyMarkothHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsMarkoth(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyMarkothHealth(hm.gameObject, hm);
            }
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, MarkothScene, StringComparison.Ordinal))
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
            ApplyMarkothHealthIfPresent();
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

    private static bool IsMarkoth(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsMarkothObject(hm.gameObject);
    }

    private static bool IsMarkothObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, MarkothScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(MarkothName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsMarkothObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => markothP4UseMaxHp;
    private static bool ShouldUseCustomPhaseThreshold() => markothP4UseCustomPhase;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, MarkothScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, MarkothScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    internal static void ApplyPhaseThresholdSettingsIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsMarkothPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsMarkothPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplyPhaseSettings(fsm.gameObject))
            {
                continue;
            }

            SetPhase2ThresholdOnFsm(fsm, GetVanillaPhase2Hp(), useVanillaVariable: true);
        }
    }

    private static bool IsMarkothPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsMarkothObject(fsm.gameObject))
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, MarkothPhaseFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState(MarkothPhaseCheckStateName) != null
            && fsm.FsmVariables.GetFsmInt(MarkothPhase2VariableName) != null;
    }

    private static bool ShouldApplyPhaseSettings(GameObject? gameObject)
    {
        return ShouldApplySettings(gameObject);
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsMarkothPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplyPhaseSettings(fsm.gameObject))
        {
            return;
        }

        bool useVanillaVariable = !ShouldUseCustomPhaseThreshold();
        int targetThreshold = useVanillaVariable
            ? GetVanillaPhase2Hp()
            : ClampMarkothPhase2Hp(markothP4Phase2Hp, ResolvePhase2MaxHp());

        SetPhase2ThresholdOnFsm(fsm, targetThreshold, useVanillaVariable);
    }

    private static void SetPhase2ThresholdOnFsm(PlayMakerFSM fsm, int value, bool useVanillaVariable)
    {
        if (fsm == null)
        {
            return;
        }

        int threshold = ClampMarkothPhase2Hp(value, ResolvePhase2MaxHp());
        FsmInt? rageHpVariable = fsm.FsmVariables.GetFsmInt(MarkothPhase2VariableName);
        if (rageHpVariable != null)
        {
            rageHpVariable.Value = threshold;
        }

        FsmState? checkState = fsm.Fsm?.GetState(MarkothPhaseCheckStateName);
        if (checkState?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in checkState.Actions)
        {
            if (action is not IntCompare compare || compare.integer2 == null)
            {
                continue;
            }

            if (useVanillaVariable)
            {
                compare.integer2.UseVariable = true;
                compare.integer2.Name = MarkothPhase2VariableName;
            }
            else
            {
                compare.integer2.UseVariable = false;
                compare.integer2.Name = string.Empty;
                compare.integer2.Value = threshold;
            }
        }
    }

    private static int GetVanillaPhase2Hp()
    {
        int maxHp = ResolvePhase2MaxHp();
        int threshold = maxHp / 2;
        return ClampMarkothPhase2Hp(threshold, maxHp);
    }

    private static void TryForceMarkothRage(GameObject bossObject)
    {
        if (bossObject == null || !ShouldApplyPhaseSettings(bossObject))
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsMarkothObject(fsm.gameObject))
            {
                continue;
            }

            if (!string.Equals(fsm.FsmName, MarkothAttackingFsmName, StringComparison.Ordinal)
                && !string.Equals(fsm.FsmName, MarkothShieldAttackFsmName, StringComparison.Ordinal))
            {
                continue;
            }

            FsmBool? rageFlag = fsm.FsmVariables.GetFsmBool(MarkothRageVariableName);
            if (rageFlag == null || rageFlag.Value)
            {
                continue;
            }

            rageFlag.Value = true;
            fsm.SendEvent(MarkothRageEventName);
        }
    }

    private static void ApplyMarkothHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampMarkothHp(markothP4MaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsMarkothObject(boss))
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

        int targetHp = ClampMarkothHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindMarkothHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsMarkoth(candidate))
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

        hp = Math.Max(hp, DefaultMarkothVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultMarkothVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultMarkothVanillaHp);
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

    private static int ClampMarkothHp(int value)
    {
        if (value < MinMarkothHp)
        {
            return MinMarkothHp;
        }

        return value > MaxMarkothHp ? MaxMarkothHp : value;
    }

    internal static int GetPhase2MaxHpForUi() => ResolvePhase2MaxHp();

    private static int ClampMarkothPhase2Hp(int value, int maxHp)
    {
        int clampedMaxHp = ClampMarkothHp(maxHp);
        if (value < MinMarkothPhase2Hp)
        {
            return MinMarkothPhase2Hp;
        }

        return value > clampedMaxHp ? clampedMaxHp : value;
    }

    private static int ResolvePhase2MaxHp()
    {
        if (ShouldUseCustomHp())
        {
            return ClampMarkothHp(markothP4MaxHp);
        }

        if (TryFindMarkothHealthManager(out HealthManager? hm) && hm != null && TryGetVanillaHp(hm, out int vanillaHp))
        {
            return ClampMarkothHp(vanillaHp);
        }

        return DefaultMarkothVanillaHp;
    }
}
