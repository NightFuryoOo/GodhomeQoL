using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class DungDefenderHelper : Module
{
    private const string DungDefenderScene = "GG_Dung_Defender";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string DungDefenderName = "Dung Defender";
    private const string DungDefenderPhaseFsmName = "Dung Defender";
    private const string DungDefenderPhaseCheckStateName = "Rage?";
    private const string DungDefenderPhase2VariableName = "Rage HP";
    private const string DungDefenderRagedVariableName = "Raged";
    private const string DungDefenderRageEventName = "RAGE";
    private const int DefaultDungDefenderMaxHp = 1100;
    private const int DefaultDungDefenderVanillaHp = 1100;
    private const int DefaultDungDefenderPhase2Hp = 350;
    private const int P5DungDefenderHp = 800;
    private const int MinDungDefenderHp = 1;
    private const int MaxDungDefenderHp = 999999;
    private const int MinDungDefenderPhase2Hp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool dungDefenderUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool dungDefenderP5Hp = false;

    [LocalSetting]
    internal static int dungDefenderMaxHp = DefaultDungDefenderMaxHp;

    [LocalSetting]
    internal static int dungDefenderMaxHpBeforeP5 = DefaultDungDefenderMaxHp;

    [LocalSetting]
    internal static bool dungDefenderUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool dungDefenderHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool dungDefenderUseCustomPhase = false;

    [LocalSetting]
    internal static int dungDefenderPhase2Hp = DefaultDungDefenderPhase2Hp;

    [LocalSetting]
    internal static bool dungDefenderUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int dungDefenderPhase2HpBeforeP5 = DefaultDungDefenderPhase2Hp;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static readonly HashSet<int> forcedCustomRageByInstance = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        NormalizeP5State();
        NormalizePhaseThresholdState();
        vanillaHpByInstance.Clear();
        forcedCustomRageByInstance.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.HealthManager.Update += OnHealthManagerUpdate;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_DungDefender;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_DungDefender;
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
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_DungDefender;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_DungDefender;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
        forcedCustomRageByInstance.Clear();
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
            ApplyDungDefenderHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        if (!ShouldUseCustomPhaseThreshold())
        {
            forcedCustomRageByInstance.Clear();
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    internal static void SetP5HpEnabled(bool value)
    {
        if (value)
        {
            if (!dungDefenderP5Hp)
            {
                dungDefenderMaxHpBeforeP5 = ClampDungDefenderHp(dungDefenderMaxHp);
                dungDefenderUseMaxHpBeforeP5 = dungDefenderUseMaxHp;
                dungDefenderUseCustomPhaseBeforeP5 = dungDefenderUseCustomPhase;
                dungDefenderPhase2HpBeforeP5 = ClampDungDefenderPhase2Hp(dungDefenderPhase2Hp, ResolvePhase2MaxHp());
                dungDefenderHasStoredStateBeforeP5 = true;
            }

            dungDefenderP5Hp = true;
            dungDefenderUseMaxHp = true;
            dungDefenderUseCustomPhase = false;
            dungDefenderMaxHp = P5DungDefenderHp;
        }
        else
        {
            if (dungDefenderP5Hp && dungDefenderHasStoredStateBeforeP5)
            {
                dungDefenderMaxHp = ClampDungDefenderHp(dungDefenderMaxHpBeforeP5);
                dungDefenderUseMaxHp = dungDefenderUseMaxHpBeforeP5;
                dungDefenderUseCustomPhase = dungDefenderUseCustomPhaseBeforeP5;
                dungDefenderPhase2Hp = ClampDungDefenderPhase2Hp(dungDefenderPhase2HpBeforeP5, ResolvePhase2MaxHp());
            }

            dungDefenderP5Hp = false;
            dungDefenderHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!dungDefenderP5Hp)
        {
            return;
        }

        if (!dungDefenderHasStoredStateBeforeP5)
        {
            dungDefenderMaxHpBeforeP5 = ClampDungDefenderHp(dungDefenderMaxHp);
            dungDefenderUseMaxHpBeforeP5 = dungDefenderUseMaxHp;
            dungDefenderUseCustomPhaseBeforeP5 = dungDefenderUseCustomPhase;
            dungDefenderPhase2HpBeforeP5 = ClampDungDefenderPhase2Hp(dungDefenderPhase2Hp, ResolvePhase2MaxHp());
            dungDefenderHasStoredStateBeforeP5 = true;
        }

        dungDefenderUseMaxHp = true;
        dungDefenderUseCustomPhase = false;
        dungDefenderMaxHp = P5DungDefenderHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        dungDefenderPhase2Hp = ClampDungDefenderPhase2Hp(dungDefenderPhase2Hp, ResolvePhase2MaxHp());
    }

    internal static void ApplyDungDefenderHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindDungDefenderHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyDungDefenderHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindDungDefenderHealthManager(out HealthManager? hm))
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

        if (!moduleActive || !IsDungDefender(self))
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
            ApplyDungDefenderHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerUpdate(On.HealthManager.orig_Update orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsDungDefender(self))
        {
            return;
        }

        if (!ShouldApplySettings(self.gameObject) || !ShouldUseCustomPhaseThreshold())
        {
            return;
        }

        int threshold = ClampDungDefenderPhase2Hp(dungDefenderPhase2Hp, ResolvePhase2MaxHp());
        if (self.hp > threshold)
        {
            return;
        }

        int instanceId = self.GetInstanceID();
        if (forcedCustomRageByInstance.Contains(instanceId))
        {
            return;
        }

        if (TryForceDungDefenderRageFromCheckState(self.gameObject))
        {
            forcedCustomRageByInstance.Add(instanceId);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsDungDefender(self))
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
            ApplyDungDefenderHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnPlayMakerFsmOnEnable_DungDefender(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsDungDefenderPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static void OnPlayMakerFsmStart_DungDefender(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsDungDefenderPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsDungDefender(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyDungDefenderHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsDungDefender(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyDungDefenderHealth(hm.gameObject, hm);
            }
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, DungDefenderScene, StringComparison.Ordinal))
        {
            vanillaHpByInstance.Clear();
            forcedCustomRageByInstance.Clear();
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyDungDefenderHealthIfPresent();
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

    private static bool IsDungDefender(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsDungDefenderObject(hm.gameObject);
    }

    private static bool IsDungDefenderObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, DungDefenderScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(DungDefenderName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsDungDefenderObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => dungDefenderUseMaxHp;
    private static bool ShouldUseCustomPhaseThreshold() => dungDefenderUseCustomPhase && !dungDefenderP5Hp;

    internal static void ApplyPhaseThresholdSettingsIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsDungDefenderPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsDungDefenderPhaseControlFsm(fsm))
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

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, DungDefenderScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, DungDefenderScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsDungDefenderPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsDungDefenderObject(fsm.gameObject))
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, DungDefenderPhaseFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState(DungDefenderPhaseCheckStateName) != null
            && fsm.FsmVariables.GetFsmInt(DungDefenderPhase2VariableName) != null;
    }

    private static bool ShouldApplyPhaseSettings(GameObject? gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return IsDungDefenderObject(gameObject) && hoGEntryAllowed;
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsDungDefenderPhaseControlFsm(fsm))
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
            : ClampDungDefenderPhase2Hp(dungDefenderPhase2Hp, ResolvePhase2MaxHp());

        SetPhase2ThresholdOnFsm(fsm, targetThreshold, useVanillaVariable);
    }

    private static void SetPhase2ThresholdOnFsm(PlayMakerFSM fsm, int value, bool useVanillaVariable)
    {
        if (fsm == null)
        {
            return;
        }

        int threshold = useVanillaVariable
            ? Math.Max(MinDungDefenderPhase2Hp, value)
            : ClampDungDefenderPhase2Hp(value, ResolvePhase2MaxHp());

        FsmInt? rageHpVariable = fsm.FsmVariables.GetFsmInt(DungDefenderPhase2VariableName);
        if (rageHpVariable != null)
        {
            rageHpVariable.Value = threshold;
        }

        FsmState? checkState = fsm.Fsm?.GetState(DungDefenderPhaseCheckStateName);
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

            FsmInt? thresholdOperand = ResolveThresholdOperand(compare, DungDefenderPhase2VariableName);
            if (thresholdOperand == null)
            {
                continue;
            }

            if (useVanillaVariable)
            {
                thresholdOperand.UseVariable = true;
                thresholdOperand.Name = DungDefenderPhase2VariableName;
            }
            else
            {
                thresholdOperand.UseVariable = false;
                thresholdOperand.Name = string.Empty;
                thresholdOperand.Value = threshold;
            }
        }
    }

    private static int GetVanillaPhase2Hp()
    {
        return DefaultDungDefenderPhase2Hp;
    }

    private static bool TryForceDungDefenderRageFromCheckState(GameObject bossObject)
    {
        if (bossObject == null || !ShouldApplyPhaseSettings(bossObject))
        {
            return false;
        }

        foreach (PlayMakerFSM fsm in bossObject.GetComponents<PlayMakerFSM>())
        {
            if (fsm == null || !IsDungDefenderPhaseControlFsm(fsm))
            {
                continue;
            }

            FsmBool? ragedFlag = fsm.FsmVariables.GetFsmBool(DungDefenderRagedVariableName);
            if (ragedFlag != null && ragedFlag.Value)
            {
                return true;
            }

            if (!string.Equals(fsm.ActiveStateName, DungDefenderPhaseCheckStateName, StringComparison.Ordinal))
            {
                continue;
            }

            fsm.SendEvent(DungDefenderRageEventName);
            return true;
        }

        return false;
    }

    private static FsmInt? ResolveThresholdOperand(IntCompare compare, string thresholdVariableName)
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

        if (integer1 != null && string.Equals(integer1.Name, thresholdVariableName, StringComparison.Ordinal))
        {
            return integer1;
        }

        if (integer2 != null && string.Equals(integer2.Name, thresholdVariableName, StringComparison.Ordinal))
        {
            return integer2;
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

    private static void ApplyDungDefenderHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampDungDefenderHp(dungDefenderMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsDungDefenderObject(boss))
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

        int targetHp = ClampDungDefenderHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindDungDefenderHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsDungDefender(candidate))
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

        hp = Math.Max(hp, DefaultDungDefenderVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultDungDefenderVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultDungDefenderVanillaHp);
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

    private static int ClampDungDefenderHp(int value)
    {
        if (value < MinDungDefenderHp)
        {
            return MinDungDefenderHp;
        }

        return value > MaxDungDefenderHp ? MaxDungDefenderHp : value;
    }

    internal static int GetPhase2MaxHpForUi()
    {
        return ResolvePhase2MaxHp();
    }

    private static int ClampDungDefenderPhase2Hp(int value, int maxHp)
    {
        int clampedMaxHp = ClampDungDefenderHp(maxHp);
        if (value < MinDungDefenderPhase2Hp)
        {
            return MinDungDefenderPhase2Hp;
        }

        return value > clampedMaxHp ? clampedMaxHp : value;
    }

    private static int ResolvePhase2MaxHp()
    {
        if (ShouldUseCustomHp())
        {
            return ClampDungDefenderHp(dungDefenderMaxHp);
        }

        if (TryFindDungDefenderHealthManager(out HealthManager? hm) && hm != null && TryGetVanillaHp(hm, out int vanillaHp))
        {
            return ClampDungDefenderHp(vanillaHp);
        }

        return DefaultDungDefenderVanillaHp;
    }
}
