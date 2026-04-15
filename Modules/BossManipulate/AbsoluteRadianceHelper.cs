using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class AbsoluteRadianceHelper : Module
{
    private const string AbsoluteRadianceScene = "GG_Radiance";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string AbsoluteRadianceName = "Absolute Radiance";
    private const string AbsoluteRadianceControlFsmName = "Control";
    private const string AbsoluteRadiancePhaseControlFsmName = "Phase Control";
    private const string AbsoluteRadianceFinalPhaseStateName = "Scream";
    private const string AbsoluteRadiancePhase2VariableName = "P2 Spike Waves";
    private const string AbsoluteRadiancePhase3VariableName = "P3 A1 Rage";
    private const string AbsoluteRadiancePhase4VariableName = "P4 Stun1";
    private const string AbsoluteRadiancePhase5VariableName = "P5 Acend";
    private const int DefaultAbsoluteRadianceMaxHp = 3000;
    private const int DefaultAbsoluteRadianceVanillaHp = 3000;
    private const int DefaultAbsoluteRadiancePhase2Hp = 2600;
    private const int DefaultAbsoluteRadiancePhase3Hp = 2150;
    private const int DefaultAbsoluteRadiancePhase4Hp = 1850;
    private const int DefaultAbsoluteRadiancePhase5Hp = 1100;
    private const int DefaultAbsoluteRadianceFinalPhaseHp = 1000;
    private const int P5AbsoluteRadianceHp = 3000;
    private const int MinAbsoluteRadianceHp = 1;
    private const int MaxAbsoluteRadianceHp = 999999;
    private const int MinAbsoluteRadiancePhaseHp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool absoluteRadianceUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool absoluteRadianceP5Hp = false;

    [LocalSetting]
    internal static int absoluteRadianceMaxHp = DefaultAbsoluteRadianceMaxHp;

    [LocalSetting]
    internal static bool absoluteRadianceUseCustomPhase = false;

    [LocalSetting]
    internal static int absoluteRadiancePhase2Hp = DefaultAbsoluteRadiancePhase2Hp;

    [LocalSetting]
    internal static int absoluteRadiancePhase3Hp = DefaultAbsoluteRadiancePhase3Hp;

    [LocalSetting]
    internal static int absoluteRadiancePhase4Hp = DefaultAbsoluteRadiancePhase4Hp;

    [LocalSetting]
    internal static int absoluteRadiancePhase5Hp = DefaultAbsoluteRadiancePhase5Hp;

    [LocalSetting]
    internal static int absoluteRadianceFinalPhaseHp = DefaultAbsoluteRadianceFinalPhaseHp;

    [LocalSetting]
    internal static int absoluteRadianceMaxHpBeforeP5 = DefaultAbsoluteRadianceMaxHp;

    [LocalSetting]
    internal static bool absoluteRadianceUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int absoluteRadiancePhase2HpBeforeP5 = DefaultAbsoluteRadiancePhase2Hp;

    [LocalSetting]
    internal static int absoluteRadiancePhase3HpBeforeP5 = DefaultAbsoluteRadiancePhase3Hp;

    [LocalSetting]
    internal static int absoluteRadiancePhase4HpBeforeP5 = DefaultAbsoluteRadiancePhase4Hp;

    [LocalSetting]
    internal static int absoluteRadiancePhase5HpBeforeP5 = DefaultAbsoluteRadiancePhase5Hp;

    [LocalSetting]
    internal static int absoluteRadianceFinalPhaseHpBeforeP5 = DefaultAbsoluteRadianceFinalPhaseHp;

    [LocalSetting]
    internal static bool absoluteRadianceUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool absoluteRadianceHasStoredStateBeforeP5 = false;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static readonly Dictionary<int, (int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp)> vanillaPhaseThresholdsByFsm = new();
    private static readonly Dictionary<int, int> vanillaFinalPhaseHpByFsm = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        NormalizeP5State();
        NormalizeCustomPhaseThresholdState();
        vanillaHpByInstance.Clear();
        vanillaPhaseThresholdsByFsm.Clear();
        vanillaFinalPhaseHpByFsm.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_AbsoluteRadiance;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_AbsoluteRadiance;
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
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_AbsoluteRadiance;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_AbsoluteRadiance;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
        vanillaPhaseThresholdsByFsm.Clear();
        vanillaFinalPhaseHpByFsm.Clear();
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
            ApplyAbsoluteRadianceHealthIfPresent();
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
            if (!absoluteRadianceP5Hp)
            {
                absoluteRadianceMaxHpBeforeP5 = ClampAbsoluteRadianceHp(absoluteRadianceMaxHp);
                absoluteRadianceUseMaxHpBeforeP5 = absoluteRadianceUseMaxHp;
                absoluteRadianceUseCustomPhaseBeforeP5 = absoluteRadianceUseCustomPhase;
                absoluteRadiancePhase2HpBeforeP5 = absoluteRadiancePhase2Hp;
                absoluteRadiancePhase3HpBeforeP5 = absoluteRadiancePhase3Hp;
                absoluteRadiancePhase4HpBeforeP5 = absoluteRadiancePhase4Hp;
                absoluteRadiancePhase5HpBeforeP5 = absoluteRadiancePhase5Hp;
                absoluteRadianceFinalPhaseHpBeforeP5 = absoluteRadianceFinalPhaseHp;
                absoluteRadianceHasStoredStateBeforeP5 = true;
            }

            absoluteRadianceP5Hp = true;
            absoluteRadianceUseMaxHp = true;
            absoluteRadianceUseCustomPhase = false;
            absoluteRadianceMaxHp = P5AbsoluteRadianceHp;
        }
        else
        {
            if (absoluteRadianceP5Hp && absoluteRadianceHasStoredStateBeforeP5)
            {
                absoluteRadianceMaxHp = ClampAbsoluteRadianceHp(absoluteRadianceMaxHpBeforeP5);
                absoluteRadianceUseMaxHp = absoluteRadianceUseMaxHpBeforeP5;
                absoluteRadianceUseCustomPhase = absoluteRadianceUseCustomPhaseBeforeP5;
                absoluteRadiancePhase2Hp = absoluteRadiancePhase2HpBeforeP5;
                absoluteRadiancePhase3Hp = absoluteRadiancePhase3HpBeforeP5;
                absoluteRadiancePhase4Hp = absoluteRadiancePhase4HpBeforeP5;
                absoluteRadiancePhase5Hp = absoluteRadiancePhase5HpBeforeP5;
                absoluteRadianceFinalPhaseHp = absoluteRadianceFinalPhaseHpBeforeP5;
            }

            absoluteRadianceP5Hp = false;
            absoluteRadianceHasStoredStateBeforeP5 = false;
        }

        NormalizeCustomPhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!absoluteRadianceP5Hp)
        {
            return;
        }

        if (!absoluteRadianceHasStoredStateBeforeP5)
        {
            absoluteRadianceMaxHpBeforeP5 = ClampAbsoluteRadianceHp(absoluteRadianceMaxHp);
            absoluteRadianceUseMaxHpBeforeP5 = absoluteRadianceUseMaxHp;
            absoluteRadianceUseCustomPhaseBeforeP5 = absoluteRadianceUseCustomPhase;
            absoluteRadiancePhase2HpBeforeP5 = absoluteRadiancePhase2Hp;
            absoluteRadiancePhase3HpBeforeP5 = absoluteRadiancePhase3Hp;
            absoluteRadiancePhase4HpBeforeP5 = absoluteRadiancePhase4Hp;
            absoluteRadiancePhase5HpBeforeP5 = absoluteRadiancePhase5Hp;
            absoluteRadianceFinalPhaseHpBeforeP5 = absoluteRadianceFinalPhaseHp;
            absoluteRadianceHasStoredStateBeforeP5 = true;
        }

        absoluteRadianceUseMaxHp = true;
        absoluteRadianceUseCustomPhase = false;
        absoluteRadianceMaxHp = P5AbsoluteRadianceHp;
        NormalizeCustomPhaseThresholdState();
    }

    private static void NormalizeCustomPhaseThresholdState()
    {
        int maxHp = ClampAbsoluteRadianceHp(absoluteRadianceMaxHp);
        absoluteRadiancePhase2Hp = ClampAbsoluteRadiancePhase2Hp(absoluteRadiancePhase2Hp, maxHp);
        absoluteRadiancePhase3Hp = ClampAbsoluteRadiancePhaseHpBelowPrevious(absoluteRadiancePhase3Hp, absoluteRadiancePhase2Hp);
        absoluteRadiancePhase4Hp = ClampAbsoluteRadiancePhaseHpBelowPrevious(absoluteRadiancePhase4Hp, absoluteRadiancePhase3Hp);
        absoluteRadiancePhase5Hp = ClampAbsoluteRadiancePhaseHpBelowPrevious(absoluteRadiancePhase5Hp, absoluteRadiancePhase4Hp);
        absoluteRadianceFinalPhaseHp = ClampAbsoluteRadianceFinalPhaseHp(absoluteRadianceFinalPhaseHp);
    }

    internal static void ApplyAbsoluteRadianceHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindAbsoluteRadianceHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyAbsoluteRadianceHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindAbsoluteRadianceHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            RestoreVanillaHealth(hm.gameObject, hm);
        }
    }

    internal static void ApplyPhaseThresholdSettingsIfPresent()
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

            if (IsAbsoluteRadiancePhaseControlFsm(fsm))
            {
                ApplyPhaseThresholdSettings(fsm);
            }

            if (IsAbsoluteRadianceControlFsm(fsm))
            {
                ApplyFinalPhaseHpSetting(fsm);
            }
        }
    }

    internal static void RestoreVanillaPhaseThresholdsIfPresent()
    {
        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null)
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            if (IsAbsoluteRadiancePhaseControlFsm(fsm))
            {
                RememberVanillaPhaseThresholds(fsm);
                (int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp) = GetVanillaPhaseThresholds(fsm);
                SetPhaseThresholdsOnFsm(fsm, phase2Hp, phase3Hp, phase4Hp, phase5Hp);
            }

            if (IsAbsoluteRadianceControlFsm(fsm))
            {
                RememberVanillaFinalPhaseHp(fsm);
                SetFinalPhaseHpOnFsm(fsm, GetVanillaFinalPhaseHp(fsm));
            }
        }
    }

    private static void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsAbsoluteRadiance(self))
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
            ApplyAbsoluteRadianceHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsAbsoluteRadiance(self))
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
            ApplyAbsoluteRadianceHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnPlayMakerFsmOnEnable_AbsoluteRadiance(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null)
        {
            return;
        }

        if (IsAbsoluteRadiancePhaseControlFsm(self))
        {
            ApplyPhaseThresholdSettings(self);
        }

        if (IsAbsoluteRadianceControlFsm(self))
        {
            ApplyFinalPhaseHpSetting(self);
        }
    }

    private static void OnPlayMakerFsmStart_AbsoluteRadiance(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null)
        {
            return;
        }

        if (IsAbsoluteRadiancePhaseControlFsm(self))
        {
            ApplyPhaseThresholdSettings(self);
        }

        if (IsAbsoluteRadianceControlFsm(self))
        {
            ApplyFinalPhaseHpSetting(self);
        }
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsAbsoluteRadiance(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyAbsoluteRadianceHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsAbsoluteRadiance(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyAbsoluteRadianceHealth(hm.gameObject, hm);
            }
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, AbsoluteRadianceScene, StringComparison.Ordinal))
        {
            vanillaHpByInstance.Clear();
            vanillaPhaseThresholdsByFsm.Clear();
            vanillaFinalPhaseHpByFsm.Clear();
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyAbsoluteRadianceHealthIfPresent();
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

    private static bool IsAbsoluteRadiance(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsAbsoluteRadianceObject(hm.gameObject);
    }

    private static bool IsAbsoluteRadianceObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, AbsoluteRadianceScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(AbsoluteRadianceName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsAbsoluteRadianceObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => absoluteRadianceUseMaxHp;

    private static bool ShouldUseCustomPhaseThresholds() => absoluteRadianceUseCustomPhase && !absoluteRadianceP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, AbsoluteRadianceScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, AbsoluteRadianceScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsAbsoluteRadiancePhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsAbsoluteRadianceObject(fsm.gameObject))
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, AbsoluteRadiancePhaseControlFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        if (string.Equals(fsm.Fsm?.Name, AbsoluteRadiancePhaseControlFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState("Check 1") != null
            && fsm.Fsm?.GetState("Check 4") != null
            && fsm.FsmVariables.GetFsmInt(AbsoluteRadiancePhase2VariableName) != null;
    }

    private static bool IsAbsoluteRadianceControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsAbsoluteRadianceObject(fsm.gameObject))
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, AbsoluteRadianceControlFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        if (string.Equals(fsm.Fsm?.Name, AbsoluteRadianceControlFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return TryGetFinalPhaseSetHpAction(fsm, out _);
    }

    private static bool TryGetFinalPhaseSetHpAction(PlayMakerFSM fsm, out SetHP? setHp)
    {
        setHp = null;
        FsmState? scream = fsm.Fsm?.GetState(AbsoluteRadianceFinalPhaseStateName);
        if (scream?.Actions == null)
        {
            return false;
        }

        foreach (FsmStateAction action in scream.Actions)
        {
            if (action is SetHP setHpAction)
            {
                setHp = setHpAction;
                return true;
            }
        }

        return false;
    }

    private static void ApplyFinalPhaseHpSetting(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsAbsoluteRadianceControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        RememberVanillaFinalPhaseHp(fsm);
        int targetFinalHp = ShouldUseCustomPhaseThresholds()
            ? ClampAbsoluteRadianceFinalPhaseHp(absoluteRadianceFinalPhaseHp)
            : GetVanillaFinalPhaseHp(fsm);

        SetFinalPhaseHpOnFsm(fsm, targetFinalHp);
    }

    private static void RememberVanillaFinalPhaseHp(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaFinalPhaseHpByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int vanillaHp = DefaultAbsoluteRadianceFinalPhaseHp;
        if (TryGetFinalPhaseSetHpAction(fsm, out SetHP? setHp) && setHp?.hp != null && !setHp.hp.IsNone)
        {
            int candidate = setHp.hp.Value;
            if (candidate > 0)
            {
                vanillaHp = candidate;
            }
        }

        vanillaFinalPhaseHpByFsm[fsmId] = ClampAbsoluteRadianceFinalPhaseHp(vanillaHp);
    }

    private static int GetVanillaFinalPhaseHp(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaFinalPhaseHpByFsm.TryGetValue(fsmId, out int hp) && hp > 0)
        {
            return ClampAbsoluteRadianceFinalPhaseHp(hp);
        }

        RememberVanillaFinalPhaseHp(fsm);
        return vanillaFinalPhaseHpByFsm.TryGetValue(fsmId, out hp)
            ? ClampAbsoluteRadianceFinalPhaseHp(hp)
            : DefaultAbsoluteRadianceFinalPhaseHp;
    }

    private static void SetFinalPhaseHpOnFsm(PlayMakerFSM fsm, int hp)
    {
        if (!TryGetFinalPhaseSetHpAction(fsm, out SetHP? setHp) || setHp?.hp == null)
        {
            return;
        }

        setHp.hp.UseVariable = false;
        setHp.hp.Value = ClampAbsoluteRadianceFinalPhaseHp(hp);
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsAbsoluteRadiancePhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        RememberVanillaPhaseThresholds(fsm);
        (int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp) thresholds = ShouldUseCustomPhaseThresholds()
            ? GetCustomPhaseThresholds()
            : GetVanillaPhaseThresholds(fsm);

        SetPhaseThresholdsOnFsm(
            fsm,
            thresholds.phase2Hp,
            thresholds.phase3Hp,
            thresholds.phase4Hp,
            thresholds.phase5Hp
        );
    }

    private static (int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp) GetCustomPhaseThresholds()
    {
        NormalizeCustomPhaseThresholdState();
        return (absoluteRadiancePhase2Hp, absoluteRadiancePhase3Hp, absoluteRadiancePhase4Hp, absoluteRadiancePhase5Hp);
    }

    private static void SetPhaseThresholdsOnFsm(PlayMakerFSM fsm, int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp)
    {
        (int p2, int p3, int p4, int p5) = NormalizePhaseThresholdChain(phase2Hp, phase3Hp, phase4Hp, phase5Hp);

        SetFsmIntVariableValue(fsm, AbsoluteRadiancePhase2VariableName, p2);
        SetFsmIntVariableValue(fsm, AbsoluteRadiancePhase3VariableName, p3);
        SetFsmIntVariableValue(fsm, AbsoluteRadiancePhase4VariableName, p4);
        SetFsmIntVariableValue(fsm, AbsoluteRadiancePhase5VariableName, p5);

        SetCheckStateThreshold(fsm, "Check 1", AbsoluteRadiancePhase2VariableName, p2);
        SetCheckStateThreshold(fsm, "Check 2", AbsoluteRadiancePhase3VariableName, p3);
        SetCheckStateThreshold(fsm, "Check 3", AbsoluteRadiancePhase4VariableName, p4);
        SetCheckStateThreshold(fsm, "Check 4", AbsoluteRadiancePhase5VariableName, p5);
    }

    private static void SetFsmIntVariableValue(PlayMakerFSM fsm, string variableName, int value)
    {
        FsmInt? fsmInt = fsm.FsmVariables.GetFsmInt(variableName);
        if (fsmInt != null)
        {
            fsmInt.Value = Math.Max(MinAbsoluteRadiancePhaseHp, value);
        }
    }

    private static void SetCheckStateThreshold(PlayMakerFSM fsm, string stateName, string variableName, int value)
    {
        FsmState? state = fsm.Fsm?.GetState(stateName);
        if (state?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in state.Actions)
        {
            if (action is not IntCompare compare || compare.integer2 == null)
            {
                continue;
            }

            compare.integer2.UseVariable = true;
            compare.integer2.Name = variableName;
            compare.integer2.Value = Math.Max(MinAbsoluteRadiancePhaseHp, value);
            break;
        }
    }

    private static void RememberVanillaPhaseThresholds(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaPhaseThresholdsByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int phase2Hp = ReadFsmIntVariableValue(fsm, AbsoluteRadiancePhase2VariableName, DefaultAbsoluteRadiancePhase2Hp);
        int phase3Hp = ReadFsmIntVariableValue(fsm, AbsoluteRadiancePhase3VariableName, DefaultAbsoluteRadiancePhase3Hp);
        int phase4Hp = ReadFsmIntVariableValue(fsm, AbsoluteRadiancePhase4VariableName, DefaultAbsoluteRadiancePhase4Hp);
        int phase5Hp = ReadFsmIntVariableValue(fsm, AbsoluteRadiancePhase5VariableName, DefaultAbsoluteRadiancePhase5Hp);

        phase2Hp = ReadCheckStateThreshold(fsm, "Check 1", phase2Hp);
        phase3Hp = ReadCheckStateThreshold(fsm, "Check 2", phase3Hp);
        phase4Hp = ReadCheckStateThreshold(fsm, "Check 3", phase4Hp);
        phase5Hp = ReadCheckStateThreshold(fsm, "Check 4", phase5Hp);

        vanillaPhaseThresholdsByFsm[fsmId] = NormalizePhaseThresholdChain(phase2Hp, phase3Hp, phase4Hp, phase5Hp);
    }

    private static int ReadFsmIntVariableValue(PlayMakerFSM fsm, string variableName, int fallback)
    {
        FsmInt? fsmInt = fsm.FsmVariables.GetFsmInt(variableName);
        if (fsmInt != null && fsmInt.Value > 0)
        {
            return fsmInt.Value;
        }

        return fallback;
    }

    private static int ReadCheckStateThreshold(PlayMakerFSM fsm, string stateName, int fallback)
    {
        FsmState? state = fsm.Fsm?.GetState(stateName);
        if (state?.Actions == null)
        {
            return fallback;
        }

        foreach (FsmStateAction action in state.Actions)
        {
            if (action is IntCompare compare && compare.integer2 != null && compare.integer2.Value > 0)
            {
                return compare.integer2.Value;
            }
        }

        return fallback;
    }

    private static (int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp) GetVanillaPhaseThresholds(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaPhaseThresholdsByFsm.TryGetValue(fsmId, out (int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp) thresholds))
        {
            return thresholds;
        }

        RememberVanillaPhaseThresholds(fsm);
        if (vanillaPhaseThresholdsByFsm.TryGetValue(fsmId, out thresholds))
        {
            return thresholds;
        }

        return (
            DefaultAbsoluteRadiancePhase2Hp,
            DefaultAbsoluteRadiancePhase3Hp,
            DefaultAbsoluteRadiancePhase4Hp,
            DefaultAbsoluteRadiancePhase5Hp
        );
    }

    private static (int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp) NormalizePhaseThresholdChain(int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp)
    {
        int p2 = Math.Max(MinAbsoluteRadiancePhaseHp, phase2Hp);
        int p3 = ClampAbsoluteRadiancePhaseHpBelowPrevious(phase3Hp, p2);
        int p4 = ClampAbsoluteRadiancePhaseHpBelowPrevious(phase4Hp, p3);
        int p5 = ClampAbsoluteRadiancePhaseHpBelowPrevious(phase5Hp, p4);
        return (p2, p3, p4, p5);
    }

    private static void ApplyAbsoluteRadianceHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampAbsoluteRadianceHp(absoluteRadianceMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsAbsoluteRadianceObject(boss))
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

        int targetHp = ClampAbsoluteRadianceHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindAbsoluteRadianceHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsAbsoluteRadiance(candidate))
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

        hp = Math.Max(hp, DefaultAbsoluteRadianceVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultAbsoluteRadianceVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultAbsoluteRadianceVanillaHp);
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

    private static int ClampAbsoluteRadianceHp(int value)
    {
        if (value < MinAbsoluteRadianceHp)
        {
            return MinAbsoluteRadianceHp;
        }

        return value > MaxAbsoluteRadianceHp ? MaxAbsoluteRadianceHp : value;
    }

    private static int ClampAbsoluteRadianceFinalPhaseHp(int value) => ClampAbsoluteRadianceHp(value);

    private static int ClampAbsoluteRadiancePhase2Hp(int value, int maxHp)
    {
        int clampedMaxHp = Math.Max(MinAbsoluteRadiancePhaseHp, ClampAbsoluteRadianceHp(maxHp));
        if (value < MinAbsoluteRadiancePhaseHp)
        {
            return MinAbsoluteRadiancePhaseHp;
        }

        return value > clampedMaxHp ? clampedMaxHp : value;
    }

    private static int ClampAbsoluteRadiancePhaseHpBelowPrevious(int value, int previousPhaseHp)
    {
        int maxValue = Math.Max(MinAbsoluteRadiancePhaseHp, previousPhaseHp - 1);
        if (value < MinAbsoluteRadiancePhaseHp)
        {
            return MinAbsoluteRadiancePhaseHp;
        }

        return value > maxValue ? maxValue : value;
    }
}
