using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class WhiteDefenderHelper : Module
{
    private const string WhiteDefenderScene = "GG_White_Defender";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string WhiteDefenderName = "White Defender";
    private const string WhiteDefenderPhaseFsmName = "Dung Defender";
    private const string WhiteDefenderPhaseCheckStateName = "Rage?";
    private const string WhiteDefenderPhase2VariableName = "Rage HP";
    private const int DefaultWhiteDefenderMaxHp = 1600;
    private const int DefaultWhiteDefenderVanillaHp = 1600;
    private const int DefaultWhiteDefenderPhase2Hp = 600;
    private const int P5WhiteDefenderHp = 1600;
    private const int MinWhiteDefenderHp = 1;
    private const int MaxWhiteDefenderHp = 999999;
    private const int MinWhiteDefenderPhase2Hp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool whiteDefenderUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool whiteDefenderP5Hp = false;

    [LocalSetting]
    internal static int whiteDefenderMaxHp = DefaultWhiteDefenderMaxHp;

    [LocalSetting]
    internal static int whiteDefenderMaxHpBeforeP5 = DefaultWhiteDefenderMaxHp;

    [LocalSetting]
    internal static bool whiteDefenderUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool whiteDefenderHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool whiteDefenderUseCustomPhase = false;

    [LocalSetting]
    internal static int whiteDefenderPhase2Hp = DefaultWhiteDefenderPhase2Hp;

    [LocalSetting]
    internal static bool whiteDefenderUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int whiteDefenderPhase2HpBeforeP5 = DefaultWhiteDefenderPhase2Hp;

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
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_WhiteDefender;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_WhiteDefender;
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
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_WhiteDefender;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_WhiteDefender;
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
            ApplyWhiteDefenderHealthIfPresent();
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
            if (!whiteDefenderP5Hp)
            {
                whiteDefenderMaxHpBeforeP5 = ClampWhiteDefenderHp(whiteDefenderMaxHp);
                whiteDefenderUseMaxHpBeforeP5 = whiteDefenderUseMaxHp;
                whiteDefenderUseCustomPhaseBeforeP5 = whiteDefenderUseCustomPhase;
                whiteDefenderPhase2HpBeforeP5 = ClampWhiteDefenderPhase2Hp(whiteDefenderPhase2Hp, ResolvePhase2MaxHp());
                whiteDefenderHasStoredStateBeforeP5 = true;
            }

            whiteDefenderP5Hp = true;
            whiteDefenderUseMaxHp = true;
            whiteDefenderUseCustomPhase = false;
            whiteDefenderMaxHp = P5WhiteDefenderHp;
        }
        else
        {
            if (whiteDefenderP5Hp && whiteDefenderHasStoredStateBeforeP5)
            {
                whiteDefenderMaxHp = ClampWhiteDefenderHp(whiteDefenderMaxHpBeforeP5);
                whiteDefenderUseMaxHp = whiteDefenderUseMaxHpBeforeP5;
                whiteDefenderUseCustomPhase = whiteDefenderUseCustomPhaseBeforeP5;
                whiteDefenderPhase2Hp = ClampWhiteDefenderPhase2Hp(whiteDefenderPhase2HpBeforeP5, ResolvePhase2MaxHp());
            }

            whiteDefenderP5Hp = false;
            whiteDefenderHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!whiteDefenderP5Hp)
        {
            return;
        }

        if (!whiteDefenderHasStoredStateBeforeP5)
        {
            whiteDefenderMaxHpBeforeP5 = ClampWhiteDefenderHp(whiteDefenderMaxHp);
            whiteDefenderUseMaxHpBeforeP5 = whiteDefenderUseMaxHp;
            whiteDefenderUseCustomPhaseBeforeP5 = whiteDefenderUseCustomPhase;
            whiteDefenderPhase2HpBeforeP5 = ClampWhiteDefenderPhase2Hp(whiteDefenderPhase2Hp, ResolvePhase2MaxHp());
            whiteDefenderHasStoredStateBeforeP5 = true;
        }

        whiteDefenderUseMaxHp = true;
        whiteDefenderUseCustomPhase = false;
        whiteDefenderMaxHp = P5WhiteDefenderHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        whiteDefenderPhase2Hp = ClampWhiteDefenderPhase2Hp(whiteDefenderPhase2Hp, ResolvePhase2MaxHp());
    }

    internal static void ApplyWhiteDefenderHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindWhiteDefenderHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyWhiteDefenderHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindWhiteDefenderHealthManager(out HealthManager? hm))
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

        if (!moduleActive || !IsWhiteDefender(self))
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
            ApplyWhiteDefenderHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsWhiteDefender(self))
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
            ApplyWhiteDefenderHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnPlayMakerFsmOnEnable_WhiteDefender(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsWhiteDefenderPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static void OnPlayMakerFsmStart_WhiteDefender(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsWhiteDefenderPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsWhiteDefender(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyWhiteDefenderHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsWhiteDefender(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyWhiteDefenderHealth(hm.gameObject, hm);
            }
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, WhiteDefenderScene, StringComparison.Ordinal))
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
            ApplyWhiteDefenderHealthIfPresent();
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

    private static bool IsWhiteDefender(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsWhiteDefenderObject(hm.gameObject);
    }

    private static bool IsWhiteDefenderObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, WhiteDefenderScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(WhiteDefenderName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsWhiteDefenderObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => whiteDefenderUseMaxHp;
    private static bool ShouldUseCustomPhaseThreshold() => whiteDefenderUseCustomPhase && !whiteDefenderP5Hp;

    internal static void ApplyPhaseThresholdSettingsIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsWhiteDefenderPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsWhiteDefenderPhaseControlFsm(fsm))
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
        if (string.Equals(nextScene, WhiteDefenderScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, WhiteDefenderScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsWhiteDefenderPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsWhiteDefenderObject(fsm.gameObject))
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, WhiteDefenderPhaseFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState(WhiteDefenderPhaseCheckStateName) != null
            && fsm.FsmVariables.GetFsmInt(WhiteDefenderPhase2VariableName) != null;
    }

    private static bool ShouldApplyPhaseSettings(GameObject? gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return IsWhiteDefenderObject(gameObject) && hoGEntryAllowed;
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsWhiteDefenderPhaseControlFsm(fsm))
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
            : ClampWhiteDefenderPhase2Hp(whiteDefenderPhase2Hp, ResolvePhase2MaxHp());

        SetPhase2ThresholdOnFsm(fsm, targetThreshold, useVanillaVariable);
    }

    private static void SetPhase2ThresholdOnFsm(PlayMakerFSM fsm, int value, bool useVanillaVariable)
    {
        if (fsm == null)
        {
            return;
        }

        int threshold = useVanillaVariable
            ? Math.Max(MinWhiteDefenderPhase2Hp, value)
            : ClampWhiteDefenderPhase2Hp(value, ResolvePhase2MaxHp());

        FsmInt? rageHpVariable = fsm.FsmVariables.GetFsmInt(WhiteDefenderPhase2VariableName);
        if (rageHpVariable != null)
        {
            rageHpVariable.Value = threshold;
        }

        FsmState? checkState = fsm.Fsm?.GetState(WhiteDefenderPhaseCheckStateName);
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

            FsmInt? thresholdOperand = ResolveThresholdOperand(compare, WhiteDefenderPhase2VariableName);
            if (thresholdOperand == null)
            {
                continue;
            }

            if (useVanillaVariable)
            {
                thresholdOperand.UseVariable = true;
                thresholdOperand.Name = WhiteDefenderPhase2VariableName;
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
        return DefaultWhiteDefenderPhase2Hp;
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

    private static void ApplyWhiteDefenderHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampWhiteDefenderHp(whiteDefenderMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsWhiteDefenderObject(boss))
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

        int targetHp = ClampWhiteDefenderHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindWhiteDefenderHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsWhiteDefender(candidate))
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

        hp = Math.Max(hp, DefaultWhiteDefenderVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultWhiteDefenderVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultWhiteDefenderVanillaHp);
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

    private static int ClampWhiteDefenderHp(int value)
    {
        if (value < MinWhiteDefenderHp)
        {
            return MinWhiteDefenderHp;
        }

        return value > MaxWhiteDefenderHp ? MaxWhiteDefenderHp : value;
    }

    internal static int GetPhase2MaxHpForUi()
    {
        return ResolvePhase2MaxHp();
    }

    private static int ClampWhiteDefenderPhase2Hp(int value, int maxHp)
    {
        int clampedMaxHp = ClampWhiteDefenderHp(maxHp);
        if (value < MinWhiteDefenderPhase2Hp)
        {
            return MinWhiteDefenderPhase2Hp;
        }

        return value > clampedMaxHp ? clampedMaxHp : value;
    }

    private static int ResolvePhase2MaxHp()
    {
        if (ShouldUseCustomHp())
        {
            return ClampWhiteDefenderHp(whiteDefenderMaxHp);
        }

        if (TryFindWhiteDefenderHealthManager(out HealthManager? hm) && hm != null && TryGetVanillaHp(hm, out int vanillaHp))
        {
            return ClampWhiteDefenderHp(vanillaHp);
        }

        return DefaultWhiteDefenderVanillaHp;
    }
}
