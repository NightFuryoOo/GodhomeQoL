using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class NoEyesHelper : Module
{
    private const string NoEyesScene = "GG_Ghost_No_Eyes_V";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string NoEyesName = "Ghost Warrior No Eyes";
    private const string NoEyesPhaseFsmName = "Escalation";
    private const string NoEyesPhaseCheckState1Name = "Check";
    private const string NoEyesPhaseCheckState2Name = "Check 2";
    private const string NoEyesPhaseIdleStateName = "Idle";
    private const string NoEyesPhaseIdle2StateName = "Idle 2";
    private const string NoEyesPhaseEscalateState1Name = "Escalate";
    private const string NoEyesPhaseEscalateState2Name = "Escalate 2";
    private const string NoEyesPhase2VariableName = "Esc 1";
    private const string NoEyesPhase3VariableName = "Esc 2";
    private const int DefaultNoEyesMaxHp = 800;
    private const int DefaultNoEyesVanillaHp = 800;
    private const int DefaultNoEyesPhase2Hp = 150;
    private const int DefaultNoEyesPhase3Hp = 90;
    private const int P5NoEyesHp = 570;
    private const int MinNoEyesHp = 1;
    private const int MaxNoEyesHp = 999999;
    private const int MinNoEyesPhase2Hp = 2;
    private const int MinNoEyesPhase3Hp = 1;
    private const int NoEyesPhaseThresholdReapplyAttempts = 8;
    private const float NoEyesPhaseThresholdReapplyInterval = 0.15f;

    [LocalSetting]
    [BoolOption]
    internal static bool noEyesUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool noEyesP5Hp = false;

    [LocalSetting]
    internal static int noEyesMaxHp = DefaultNoEyesMaxHp;

    [LocalSetting]
    internal static int noEyesMaxHpBeforeP5 = DefaultNoEyesMaxHp;

    [LocalSetting]
    internal static bool noEyesUseCustomPhase = false;

    [LocalSetting]
    internal static int noEyesPhase2Hp = DefaultNoEyesPhase2Hp;

    [LocalSetting]
    internal static int noEyesPhase3Hp = DefaultNoEyesPhase3Hp;

    [LocalSetting]
    internal static bool noEyesUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool noEyesHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool noEyesUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int noEyesPhase2HpBeforeP5 = DefaultNoEyesPhase2Hp;

    [LocalSetting]
    internal static int noEyesPhase3HpBeforeP5 = DefaultNoEyesPhase3Hp;

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
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_NoEyes;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_NoEyes;
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
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_NoEyes;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_NoEyes;
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
            ApplyNoEyesHealthIfPresent();
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
            if (!noEyesP5Hp)
            {
                noEyesMaxHpBeforeP5 = ClampNoEyesHp(noEyesMaxHp);
                noEyesUseMaxHpBeforeP5 = noEyesUseMaxHp;
                noEyesUseCustomPhaseBeforeP5 = noEyesUseCustomPhase;
                noEyesPhase2HpBeforeP5 = ClampNoEyesPhase2Hp(noEyesPhase2Hp, ResolvePhase2MaxHp());
                noEyesPhase3HpBeforeP5 = ClampNoEyesPhase3Hp(noEyesPhase3Hp, noEyesPhase2HpBeforeP5);
                noEyesHasStoredStateBeforeP5 = true;
            }

            noEyesP5Hp = true;
            noEyesUseMaxHp = true;
            noEyesUseCustomPhase = false;
            noEyesMaxHp = P5NoEyesHp;
        }
        else
        {
            if (noEyesP5Hp && noEyesHasStoredStateBeforeP5)
            {
                noEyesMaxHp = ClampNoEyesHp(noEyesMaxHpBeforeP5);
                noEyesUseMaxHp = noEyesUseMaxHpBeforeP5;
                noEyesUseCustomPhase = noEyesUseCustomPhaseBeforeP5;
                noEyesPhase2Hp = ClampNoEyesPhase2Hp(noEyesPhase2HpBeforeP5, ResolvePhase2MaxHp());
                noEyesPhase3Hp = ClampNoEyesPhase3Hp(noEyesPhase3HpBeforeP5, noEyesPhase2Hp);
            }

            noEyesP5Hp = false;
            noEyesHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!noEyesP5Hp)
        {
            return;
        }

        if (!noEyesHasStoredStateBeforeP5)
        {
            noEyesMaxHpBeforeP5 = ClampNoEyesHp(noEyesMaxHp);
            noEyesUseMaxHpBeforeP5 = noEyesUseMaxHp;
            noEyesUseCustomPhaseBeforeP5 = noEyesUseCustomPhase;
            noEyesPhase2HpBeforeP5 = ClampNoEyesPhase2Hp(noEyesPhase2Hp, ResolvePhase2MaxHp());
            noEyesPhase3HpBeforeP5 = ClampNoEyesPhase3Hp(noEyesPhase3Hp, noEyesPhase2HpBeforeP5);
            noEyesHasStoredStateBeforeP5 = true;
        }

        noEyesUseMaxHp = true;
        noEyesUseCustomPhase = false;
        noEyesMaxHp = P5NoEyesHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        noEyesPhase2Hp = ClampNoEyesPhase2Hp(noEyesPhase2Hp, ResolvePhase2MaxHp());
        noEyesPhase3Hp = ClampNoEyesPhase3Hp(noEyesPhase3Hp, noEyesPhase2Hp);
    }

    internal static void ApplyNoEyesHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindNoEyesHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyNoEyesHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindNoEyesHealthManager(out HealthManager? hm))
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
            if (fsm == null || fsm.gameObject == null || !IsNoEyesPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsNoEyesPhaseControlFsm(fsm))
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

        if (!moduleActive || !IsNoEyes(self))
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
            ApplyNoEyesHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsNoEyes(self))
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
            ApplyNoEyesHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }

        _ = self.StartCoroutine(DeferredApplyPhaseThresholds(self));
    }

    private static void OnHealthManagerUpdate(On.HealthManager.orig_Update orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsNoEyes(self))
        {
            return;
        }

        if (!ShouldApplySettings(self.gameObject) || !ShouldUseCustomPhaseThresholds())
        {
            return;
        }

        TryForceNoEyesEscalationTransitions(self.gameObject, self.hp);
    }

    private static void OnPlayMakerFsmOnEnable_NoEyes(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsNoEyesPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static void OnPlayMakerFsmStart_NoEyes(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsNoEyesPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsNoEyes(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyNoEyesHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsNoEyes(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyNoEyesHealth(hm.gameObject, hm);
            }
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    private static IEnumerator DeferredApplyPhaseThresholds(HealthManager hm)
    {
        for (int attempt = 0; attempt < NoEyesPhaseThresholdReapplyAttempts; attempt++)
        {
            if (!moduleActive || hm == null || hm.gameObject == null || !IsNoEyes(hm) || !ShouldApplySettings(hm.gameObject))
            {
                yield break;
            }

            if (!ShouldUseCustomPhaseThresholds())
            {
                yield break;
            }

            ApplyPhaseThresholdSettingsIfPresent();
            yield return new WaitForSeconds(NoEyesPhaseThresholdReapplyInterval);
        }
    }

    private static void TryForceNoEyesEscalationTransitions(GameObject bossObject, int currentHp)
    {
        if (bossObject == null || !ShouldApplySettings(bossObject) || !ShouldUseCustomPhaseThresholds())
        {
            return;
        }

        int phase2Threshold = ClampNoEyesPhase2Hp(noEyesPhase2Hp, ResolvePhase2MaxHp());
        int phase3Threshold = ClampNoEyesPhase3Hp(noEyesPhase3Hp, phase2Threshold);

        foreach (PlayMakerFSM fsm in bossObject.GetComponents<PlayMakerFSM>())
        {
            if (fsm == null || !IsNoEyesPhaseControlFsm(fsm))
            {
                continue;
            }

            string activeState = fsm.ActiveStateName ?? string.Empty;
            if ((string.Equals(activeState, NoEyesPhaseIdleStateName, StringComparison.Ordinal)
                || string.Equals(activeState, NoEyesPhaseCheckState1Name, StringComparison.Ordinal))
                && currentHp <= phase2Threshold)
            {
                fsm.Fsm.SetState(NoEyesPhaseEscalateState1Name);
                continue;
            }

            if ((string.Equals(activeState, NoEyesPhaseIdle2StateName, StringComparison.Ordinal)
                || string.Equals(activeState, NoEyesPhaseCheckState2Name, StringComparison.Ordinal))
                && currentHp <= phase3Threshold)
            {
                fsm.Fsm.SetState(NoEyesPhaseEscalateState2Name);
            }
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, NoEyesScene, StringComparison.Ordinal))
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
            ApplyNoEyesHealthIfPresent();
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

    private static bool IsNoEyes(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsNoEyesObject(hm.gameObject);
    }

    private static bool IsNoEyesObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, NoEyesScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(NoEyesName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsNoEyesObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => noEyesUseMaxHp;

    private static bool ShouldUseCustomPhaseThresholds() => noEyesUseCustomPhase && !noEyesP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, NoEyesScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, NoEyesScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsNoEyesPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsNoEyesObject(fsm.gameObject))
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, NoEyesPhaseFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState(NoEyesPhaseCheckState1Name) != null
            && fsm.Fsm?.GetState(NoEyesPhaseCheckState2Name) != null
            && (
                fsm.FsmVariables.GetFsmInt(NoEyesPhase2VariableName) != null
                || fsm.FsmVariables.GetFsmInt(NoEyesPhase3VariableName) != null
            );
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsNoEyesPhaseControlFsm(fsm))
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
                ClampNoEyesPhase2Hp(noEyesPhase2Hp, phase2MaxHp),
                ClampNoEyesPhase3Hp(noEyesPhase3Hp, noEyesPhase2Hp)
            )
            : GetVanillaPhaseThresholds(fsm);

        SetPhaseThresholdsOnFsm(fsm, thresholds.phase2Hp, thresholds.phase3Hp, ShouldUseCustomPhaseThresholds());
    }

    private static void SetPhaseThresholdsOnFsm(PlayMakerFSM fsm, int phase2Hp, int phase3Hp, bool useExactCustomThresholds)
    {
        int targetPhase2Hp = ClampNoEyesPhase2Hp(phase2Hp, ResolvePhase2MaxHp());
        int targetPhase3Hp = ClampNoEyesPhase3Hp(phase3Hp, targetPhase2Hp);

        FsmInt? phase2Variable = fsm.FsmVariables.GetFsmInt(NoEyesPhase2VariableName);
        if (phase2Variable != null)
        {
            phase2Variable.Value = targetPhase2Hp;
        }

        FsmInt? phase3Variable = fsm.FsmVariables.GetFsmInt(NoEyesPhase3VariableName);
        if (phase3Variable != null)
        {
            phase3Variable.Value = targetPhase3Hp;
        }

        SetThresholdOnCheckState(fsm, NoEyesPhaseCheckState1Name, NoEyesPhase2VariableName, targetPhase2Hp, useExactCustomThresholds);
        SetThresholdOnCheckState(fsm, NoEyesPhaseCheckState2Name, NoEyesPhase3VariableName, targetPhase3Hp, useExactCustomThresholds);
    }

    private static void SetThresholdOnCheckState(PlayMakerFSM fsm, string stateName, string variableName, int targetValue, bool useExactCustomThresholds)
    {
        FsmState? checkState = fsm.Fsm?.GetState(stateName);
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

            if (useExactCustomThresholds)
            {
                compare.integer2.UseVariable = false;
                compare.integer2.Name = string.Empty;
                compare.integer2.Value = SafeIncrementThreshold(targetValue);
            }
            else
            {
                compare.integer2.UseVariable = true;
                compare.integer2.Name = variableName;
                compare.integer2.Value = targetValue;
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

        int phase2Hp = DefaultNoEyesPhase2Hp;
        int phase3Hp = DefaultNoEyesPhase3Hp;

        bool phase2Captured = TryReadThresholdFromCheckState(fsm, NoEyesPhaseCheckState1Name, out int capturedPhase2Hp);
        if (phase2Captured)
        {
            phase2Hp = capturedPhase2Hp;
        }

        bool phase3Captured = TryReadThresholdFromCheckState(fsm, NoEyesPhaseCheckState2Name, out int capturedPhase3Hp);
        if (phase3Captured)
        {
            phase3Hp = capturedPhase3Hp;
        }

        if (!phase2Captured)
        {
            FsmInt? phase2Variable = fsm.FsmVariables.GetFsmInt(NoEyesPhase2VariableName);
            if (phase2Variable != null && phase2Variable.Value > 0)
            {
                phase2Hp = phase2Variable.Value;
            }
        }

        if (!phase3Captured)
        {
            FsmInt? phase3Variable = fsm.FsmVariables.GetFsmInt(NoEyesPhase3VariableName);
            if (phase3Variable != null && phase3Variable.Value > 0)
            {
                phase3Hp = phase3Variable.Value;
            }
        }

        phase2Hp = ClampNoEyesPhase2Hp(phase2Hp, ResolvePhase2MaxHp());
        phase3Hp = ClampNoEyesPhase3Hp(phase3Hp, phase2Hp);
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
                ClampNoEyesPhase2Hp(thresholds.phase2Hp, ResolvePhase2MaxHp()),
                ClampNoEyesPhase3Hp(thresholds.phase3Hp, thresholds.phase2Hp)
            );
        }

        RememberVanillaPhaseThresholds(fsm);
        if (vanillaPhaseThresholdsByFsm.TryGetValue(fsmId, out thresholds))
        {
            return (
                ClampNoEyesPhase2Hp(thresholds.phase2Hp, ResolvePhase2MaxHp()),
                ClampNoEyesPhase3Hp(thresholds.phase3Hp, thresholds.phase2Hp)
            );
        }

        return (DefaultNoEyesPhase2Hp, DefaultNoEyesPhase3Hp);
    }

    private static void ApplyNoEyesHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampNoEyesHp(noEyesMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsNoEyesObject(boss))
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

        int targetHp = ClampNoEyesHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindNoEyesHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsNoEyes(candidate))
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

        hp = Math.Max(hp, DefaultNoEyesVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultNoEyesVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultNoEyesVanillaHp);
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

    internal static int GetPhase2MaxHpForUi()
    {
        return ResolvePhase2MaxHp();
    }

    private static int ResolvePhase2MaxHp()
    {
        if (ShouldUseCustomHp())
        {
            return ClampNoEyesHp(noEyesMaxHp);
        }

        if (TryFindNoEyesHealthManager(out HealthManager? hm) && hm != null && TryGetVanillaHp(hm, out int vanillaHp))
        {
            return ClampNoEyesHp(vanillaHp);
        }

        return DefaultNoEyesVanillaHp;
    }

    private static int ClampNoEyesHp(int value)
    {
        if (value < MinNoEyesHp)
        {
            return MinNoEyesHp;
        }

        return value > MaxNoEyesHp ? MaxNoEyesHp : value;
    }

    private static int ClampNoEyesPhase2Hp(int value, int maxHp)
    {
        int maxPhase2Hp = Math.Max(MinNoEyesPhase2Hp, ClampNoEyesHp(maxHp));
        if (value < MinNoEyesPhase2Hp)
        {
            return MinNoEyesPhase2Hp;
        }

        return value > maxPhase2Hp ? maxPhase2Hp : value;
    }

    private static int ClampNoEyesPhase3Hp(int value, int phase2Hp)
    {
        int clampedPhase2Hp = phase2Hp < MinNoEyesPhase2Hp
            ? MinNoEyesPhase2Hp
            : phase2Hp;
        int maxPhase3Hp = Math.Max(MinNoEyesPhase3Hp, clampedPhase2Hp - 1);
        if (value < MinNoEyesPhase3Hp)
        {
            return MinNoEyesPhase3Hp;
        }

        return value > maxPhase3Hp ? maxPhase3Hp : value;
    }

    private static int SafeIncrementThreshold(int value)
    {
        return value >= int.MaxValue ? int.MaxValue : value + 1;
    }
}
