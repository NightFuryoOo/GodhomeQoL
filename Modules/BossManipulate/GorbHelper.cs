using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class GorbHelper : Module
{
    private const string GorbScene = "GG_Ghost_Gorb_V";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string GorbName = "Ghost Warrior Slug";
    private const string GorbPhaseFsmName = "Attacking";
    private const string GorbPhaseCheckState1Name = "Double?";
    private const string GorbPhaseCheckState2Name = "Triple?";
    private const string GorbPhase2TransitionStateName = "Double Pause";
    private const string GorbPhase3TransitionStateName = "Anim";
    private const string GorbPhase2VariableName = "Double HP";
    private const string GorbPhase3VariableName = "Triple HP";
    private const int DefaultGorbMaxHp = 1000;
    private const int DefaultGorbVanillaHp = 1000;
    private const int DefaultGorbPhase2Hp = 700;
    private const int DefaultGorbPhase3Hp = 400;
    private const int P5GorbHp = 650;
    private const int MinGorbHp = 1;
    private const int MaxGorbHp = 999999;
    private const int MinGorbPhase2Hp = 2;
    private const int MinGorbPhase3Hp = 1;
    private const int GorbPhaseThresholdReapplyAttempts = 8;
    private const float GorbPhaseThresholdReapplyInterval = 0.15f;

    [LocalSetting]
    [BoolOption]
    internal static bool gorbUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool gorbP5Hp = false;

    [LocalSetting]
    internal static int gorbMaxHp = DefaultGorbMaxHp;

    [LocalSetting]
    internal static int gorbMaxHpBeforeP5 = DefaultGorbMaxHp;

    [LocalSetting]
    internal static bool gorbUseCustomPhase = false;

    [LocalSetting]
    internal static int gorbPhase2Hp = DefaultGorbPhase2Hp;

    [LocalSetting]
    internal static int gorbPhase3Hp = DefaultGorbPhase3Hp;

    [LocalSetting]
    internal static bool gorbUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool gorbHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool gorbUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int gorbPhase2HpBeforeP5 = DefaultGorbPhase2Hp;

    [LocalSetting]
    internal static int gorbPhase3HpBeforeP5 = DefaultGorbPhase3Hp;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static readonly Dictionary<int, (int phase2Hp, int phase3Hp)> vanillaPhaseThresholdsByFsm = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        NormalizeP5State();
        NormalizePhaseThresholdState();
        vanillaHpByInstance.Clear();
        vanillaPhaseThresholdsByFsm.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.HealthManager.Update += OnHealthManagerUpdate;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_Gorb;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_Gorb;
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
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_Gorb;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_Gorb;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
        vanillaPhaseThresholdsByFsm.Clear();
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
            ApplyGorbHealthIfPresent();
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
            if (!gorbP5Hp)
            {
                gorbMaxHpBeforeP5 = ClampGorbHp(gorbMaxHp);
                gorbUseMaxHpBeforeP5 = gorbUseMaxHp;
                gorbUseCustomPhaseBeforeP5 = gorbUseCustomPhase;
                gorbPhase2HpBeforeP5 = ClampGorbPhase2Hp(gorbPhase2Hp, ResolvePhase2MaxHp());
                gorbPhase3HpBeforeP5 = ClampGorbPhase3Hp(gorbPhase3Hp, gorbPhase2Hp);
                gorbHasStoredStateBeforeP5 = true;
            }

            gorbP5Hp = true;
            gorbUseMaxHp = true;
            gorbUseCustomPhase = false;
            gorbMaxHp = P5GorbHp;
        }
        else
        {
            if (gorbP5Hp && gorbHasStoredStateBeforeP5)
            {
                gorbMaxHp = ClampGorbHp(gorbMaxHpBeforeP5);
                gorbUseMaxHp = gorbUseMaxHpBeforeP5;
                gorbUseCustomPhase = gorbUseCustomPhaseBeforeP5;
                gorbPhase2Hp = ClampGorbPhase2Hp(gorbPhase2HpBeforeP5, ResolvePhase2MaxHp());
                gorbPhase3Hp = ClampGorbPhase3Hp(gorbPhase3HpBeforeP5, gorbPhase2Hp);
            }

            gorbP5Hp = false;
            gorbHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!gorbP5Hp)
        {
            return;
        }

        if (!gorbHasStoredStateBeforeP5)
        {
            gorbMaxHpBeforeP5 = ClampGorbHp(gorbMaxHp);
            gorbUseMaxHpBeforeP5 = gorbUseMaxHp;
            gorbUseCustomPhaseBeforeP5 = gorbUseCustomPhase;
            gorbPhase2HpBeforeP5 = ClampGorbPhase2Hp(gorbPhase2Hp, ResolvePhase2MaxHp());
            gorbPhase3HpBeforeP5 = ClampGorbPhase3Hp(gorbPhase3Hp, gorbPhase2Hp);
            gorbHasStoredStateBeforeP5 = true;
        }

        gorbUseMaxHp = true;
        gorbUseCustomPhase = false;
        gorbMaxHp = P5GorbHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        gorbPhase2Hp = ClampGorbPhase2Hp(gorbPhase2Hp, ResolvePhase2MaxHp());
        gorbPhase3Hp = ClampGorbPhase3Hp(gorbPhase3Hp, gorbPhase2Hp);
    }

    internal static void ApplyGorbHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindGorbHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyGorbHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindGorbHealthManager(out HealthManager? hm))
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
            if (fsm == null || fsm.gameObject == null || !IsGorbPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsGorbPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            RememberVanillaPhaseThresholds(fsm);
            (int phase2Hp, int phase3Hp) = GetVanillaPhaseThresholds(fsm);
            SetPhaseThresholdsOnFsm(fsm, phase2Hp, phase3Hp, useExactCustomThresholds: false);
        }
    }

    private static void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsGorb(self))
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
            ApplyGorbHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsGorb(self))
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
            ApplyGorbHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }

        _ = self.StartCoroutine(DeferredApplyPhaseThresholds(self));
    }

    private static void OnPlayMakerFsmOnEnable_Gorb(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsGorbPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static void OnPlayMakerFsmStart_Gorb(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsGorbPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static void OnHealthManagerUpdate(On.HealthManager.orig_Update orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsGorb(self))
        {
            return;
        }

        if (!ShouldApplySettings(self.gameObject) || !ShouldUseCustomPhaseThresholds())
        {
            return;
        }

        TryForceGorbPhaseTransitions(self.gameObject, self.hp);
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsGorb(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyGorbHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsGorb(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyGorbHealth(hm.gameObject, hm);
            }
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    private static IEnumerator DeferredApplyPhaseThresholds(HealthManager hm)
    {
        for (int attempt = 0; attempt < GorbPhaseThresholdReapplyAttempts; attempt++)
        {
            if (!moduleActive || hm == null || hm.gameObject == null || !IsGorb(hm) || !ShouldApplySettings(hm.gameObject))
            {
                yield break;
            }

            if (!ShouldUseCustomPhaseThresholds())
            {
                yield break;
            }

            ApplyPhaseThresholdSettingsIfPresent();
            yield return new WaitForSeconds(GorbPhaseThresholdReapplyInterval);
        }
    }

    private static void TryForceGorbPhaseTransitions(GameObject bossObject, int currentHp)
    {
        if (bossObject == null || !ShouldApplySettings(bossObject) || !ShouldUseCustomPhaseThresholds())
        {
            return;
        }

        int phase2Threshold = ClampGorbPhase2Hp(gorbPhase2Hp, ResolvePhase2MaxHp());
        int phase3Threshold = ClampGorbPhase3Hp(gorbPhase3Hp, phase2Threshold);

        foreach (PlayMakerFSM fsm in bossObject.GetComponents<PlayMakerFSM>())
        {
            if (fsm == null || !IsGorbPhaseControlFsm(fsm))
            {
                continue;
            }

            string activeState = fsm.ActiveStateName ?? string.Empty;
            if (string.Equals(activeState, GorbPhaseCheckState1Name, StringComparison.Ordinal) && currentHp <= phase2Threshold)
            {
                fsm.Fsm.SetState(GorbPhase2TransitionStateName);
                continue;
            }

            if (string.Equals(activeState, GorbPhaseCheckState2Name, StringComparison.Ordinal) && currentHp <= phase3Threshold)
            {
                fsm.Fsm.SetState(GorbPhase3TransitionStateName);
            }
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, GorbScene, StringComparison.Ordinal))
        {
            vanillaHpByInstance.Clear();
            vanillaPhaseThresholdsByFsm.Clear();
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyGorbHealthIfPresent();
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

    private static bool IsGorb(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsGorbObject(hm.gameObject);
    }

    private static bool IsGorbObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, GorbScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(GorbName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsGorbObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => gorbUseMaxHp;

    private static bool ShouldUseCustomPhaseThresholds() => gorbUseCustomPhase && !gorbP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, GorbScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, GorbScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsGorbPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsGorbObject(fsm.gameObject))
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, GorbPhaseFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState(GorbPhaseCheckState1Name) != null
            && fsm.Fsm?.GetState(GorbPhaseCheckState2Name) != null
            && (
                fsm.FsmVariables.GetFsmInt(GorbPhase2VariableName) != null
                || fsm.FsmVariables.GetFsmInt(GorbPhase3VariableName) != null
            );
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsGorbPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        RememberVanillaPhaseThresholds(fsm);
        int phase2MaxHp = ResolvePhase2MaxHp();
        (int phase2Hp, int phase3Hp) thresholds = ShouldUseCustomPhaseThresholds()
            ? (
                ClampGorbPhase2Hp(gorbPhase2Hp, phase2MaxHp),
                ClampGorbPhase3Hp(gorbPhase3Hp, gorbPhase2Hp)
            )
            : GetVanillaPhaseThresholds(fsm);

        SetPhaseThresholdsOnFsm(fsm, thresholds.phase2Hp, thresholds.phase3Hp, ShouldUseCustomPhaseThresholds());
    }

    private static void SetPhaseThresholdsOnFsm(PlayMakerFSM fsm, int phase2Hp, int phase3Hp, bool useExactCustomThresholds)
    {
        int targetPhase2Hp = ClampGorbPhase2Hp(phase2Hp, ResolvePhase2MaxHp());
        int targetPhase3Hp = ClampGorbPhase3Hp(phase3Hp, targetPhase2Hp);

        FsmInt? phase2Variable = fsm.FsmVariables.GetFsmInt(GorbPhase2VariableName);
        if (phase2Variable != null)
        {
            phase2Variable.Value = targetPhase2Hp;
        }

        FsmInt? phase3Variable = fsm.FsmVariables.GetFsmInt(GorbPhase3VariableName);
        if (phase3Variable != null)
        {
            phase3Variable.Value = targetPhase3Hp;
        }

        SetThresholdOnCheckState(fsm, GorbPhaseCheckState1Name, GorbPhase2VariableName, targetPhase2Hp);
        SetThresholdOnCheckState(fsm, GorbPhaseCheckState2Name, GorbPhase3VariableName, targetPhase3Hp);
    }

    private static void SetThresholdOnCheckState(PlayMakerFSM fsm, string stateName, string variableName, int targetValue)
    {
        FsmState? checkState = fsm.Fsm?.GetState(stateName);
        if (checkState?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in checkState.Actions)
        {
            if (action is IntCompare compare && compare.integer2 != null)
            {
                bool assigned = false;

                if (compare.integer1 != null && string.Equals(compare.integer1.Name, variableName, StringComparison.Ordinal))
                {
                    compare.integer1.UseVariable = true;
                    compare.integer1.Name = variableName;
                    compare.integer1.Value = targetValue;
                    assigned = true;
                }

                if (compare.integer2 != null && string.Equals(compare.integer2.Name, variableName, StringComparison.Ordinal))
                {
                    compare.integer2.UseVariable = true;
                    compare.integer2.Name = variableName;
                    compare.integer2.Value = targetValue;
                    assigned = true;
                }

                if (!assigned && compare.integer2 != null)
                {
                    compare.integer2.UseVariable = true;
                    compare.integer2.Name = variableName;
                    compare.integer2.Value = targetValue;
                }
            }
        }
    }

    private static void RememberVanillaPhaseThresholds(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaPhaseThresholdsByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int phase2Hp = DefaultGorbPhase2Hp;
        int phase3Hp = DefaultGorbPhase3Hp;

        bool phase2Captured = TryReadThresholdFromCheckState(fsm, GorbPhaseCheckState1Name, out int capturedPhase2Hp);
        if (phase2Captured)
        {
            phase2Hp = capturedPhase2Hp;
        }

        bool phase3Captured = TryReadThresholdFromCheckState(fsm, GorbPhaseCheckState2Name, out int capturedPhase3Hp);
        if (phase3Captured)
        {
            phase3Hp = capturedPhase3Hp;
        }

        if (!phase2Captured)
        {
            FsmInt? phase2Variable = fsm.FsmVariables.GetFsmInt(GorbPhase2VariableName);
            if (phase2Variable != null && phase2Variable.Value > 0)
            {
                phase2Hp = phase2Variable.Value;
            }
        }

        if (!phase3Captured)
        {
            FsmInt? phase3Variable = fsm.FsmVariables.GetFsmInt(GorbPhase3VariableName);
            if (phase3Variable != null && phase3Variable.Value > 0)
            {
                phase3Hp = phase3Variable.Value;
            }
        }

        phase2Hp = ClampGorbPhase2Hp(phase2Hp, ResolvePhase2MaxHp());
        phase3Hp = ClampGorbPhase3Hp(phase3Hp, phase2Hp);
        vanillaPhaseThresholdsByFsm[fsmId] = (phase2Hp, phase3Hp);
    }

    private static bool TryReadThresholdFromCheckState(PlayMakerFSM fsm, string stateName, out int threshold)
    {
        threshold = 0;
        FsmState? state = fsm.Fsm?.GetState(stateName);
        if (state?.Actions == null)
        {
            return false;
        }

        foreach (FsmStateAction action in state.Actions)
        {
            if (action is not IntCompare compare || compare.integer2 == null)
            {
                continue;
            }

            if (compare.integer2.UseVariable && !string.IsNullOrEmpty(compare.integer2.Name))
            {
                FsmInt? variable = fsm.FsmVariables.GetFsmInt(compare.integer2.Name);
                if (variable != null)
                {
                    threshold = variable.Value;
                    return true;
                }
            }

            threshold = compare.integer2.Value;
            return true;
        }

        return false;
    }

    private static (int phase2Hp, int phase3Hp) GetVanillaPhaseThresholds(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaPhaseThresholdsByFsm.TryGetValue(fsmId, out (int phase2Hp, int phase3Hp) thresholds))
        {
            return (
                ClampGorbPhase2Hp(thresholds.phase2Hp, ResolvePhase2MaxHp()),
                ClampGorbPhase3Hp(thresholds.phase3Hp, thresholds.phase2Hp)
            );
        }

        RememberVanillaPhaseThresholds(fsm);
        if (vanillaPhaseThresholdsByFsm.TryGetValue(fsmId, out thresholds))
        {
            return (
                ClampGorbPhase2Hp(thresholds.phase2Hp, ResolvePhase2MaxHp()),
                ClampGorbPhase3Hp(thresholds.phase3Hp, thresholds.phase2Hp)
            );
        }

        return (DefaultGorbPhase2Hp, DefaultGorbPhase3Hp);
    }

    private static void ApplyGorbHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampGorbHp(gorbMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsGorbObject(boss))
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

        int targetHp = ClampGorbHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindGorbHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsGorb(candidate))
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

        hp = Math.Max(hp, DefaultGorbVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultGorbVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultGorbVanillaHp);
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

    private static int ClampGorbHp(int value)
    {
        if (value < MinGorbHp)
        {
            return MinGorbHp;
        }

        return value > MaxGorbHp ? MaxGorbHp : value;
    }

    internal static int GetPhase2MaxHpForUi() => ResolvePhase2MaxHp();

    private static int ClampGorbPhase2Hp(int value, int maxHp)
    {
        int clampedMaxHp = ClampGorbHp(maxHp);
        if (value < MinGorbPhase2Hp)
        {
            return MinGorbPhase2Hp;
        }

        return value > clampedMaxHp ? clampedMaxHp : value;
    }

    private static int ClampGorbPhase3Hp(int value, int phase2Hp)
    {
        int clampedPhase2Hp = ClampGorbPhase2Hp(phase2Hp, ResolvePhase2MaxHp());
        int maxPhase3Hp = Math.Max(MinGorbPhase3Hp, clampedPhase2Hp - 1);

        if (value < MinGorbPhase3Hp)
        {
            return MinGorbPhase3Hp;
        }

        return value > maxPhase3Hp ? maxPhase3Hp : value;
    }

    private static int ResolvePhase2MaxHp()
    {
        if (ShouldUseCustomHp())
        {
            return ClampGorbHp(gorbMaxHp);
        }

        if (TryFindGorbHealthManager(out HealthManager? hm) && hm != null && TryGetVanillaHp(hm, out int vanillaHp))
        {
            return ClampGorbHp(vanillaHp);
        }

        return DefaultGorbVanillaHp;
    }

}
