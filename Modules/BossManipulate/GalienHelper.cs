using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class GalienHelper : Module
{
    private const string GalienScene = "GG_Ghost_Galien";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string GalienName = "Ghost Warrior Galien";
    private const string GalienPhaseFsmName = "Summon Minis";
    private const string GalienPhaseCheckState1Name = "Check";
    private const string GalienPhaseCheckState2Name = "Check 2";
    private const string GalienPhaseIdleStateName = "Idle";
    private const string GalienPhaseIdle2StateName = "Idle 2";
    private const string GalienPhaseSummonAnticStateName = "Summon Antic";
    private const string GalienPhaseSummonAntic2StateName = "Summon Antic 2";
    private const string GalienPhase2VariableName = "Summon HP1";
    private const string GalienPhase3VariableName = "Summon HP2";
    private const int DefaultGalienMaxHp = 1000;
    private const int DefaultGalienVanillaHp = 1000;
    private const int DefaultGalienPhase2Hp = 700;
    private const int DefaultGalienPhase3Hp = 400;
    private const int P5GalienHp = 650;
    private const int MinGalienHp = 1;
    private const int MaxGalienHp = 999999;
    private const int MinGalienPhase2Hp = 2;
    private const int MinGalienPhase3Hp = 1;
    private const int GalienPhaseThresholdReapplyAttempts = 8;
    private const float GalienPhaseThresholdReapplyInterval = 0.15f;

    [LocalSetting]
    [BoolOption]
    internal static bool galienUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool galienP5Hp = false;

    [LocalSetting]
    internal static int galienMaxHp = DefaultGalienMaxHp;

    [LocalSetting]
    internal static int galienMaxHpBeforeP5 = DefaultGalienMaxHp;

    [LocalSetting]
    internal static bool galienUseCustomPhase = false;

    [LocalSetting]
    internal static int galienPhase2Hp = DefaultGalienPhase2Hp;

    [LocalSetting]
    internal static int galienPhase3Hp = DefaultGalienPhase3Hp;

    [LocalSetting]
    internal static bool galienUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool galienHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool galienUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int galienPhase2HpBeforeP5 = DefaultGalienPhase2Hp;

    [LocalSetting]
    internal static int galienPhase3HpBeforeP5 = DefaultGalienPhase3Hp;

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
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_Galien;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_Galien;
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
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_Galien;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_Galien;
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
            ApplyGalienHealthIfPresent();
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
            if (!galienP5Hp)
            {
                galienMaxHpBeforeP5 = ClampGalienHp(galienMaxHp);
                galienUseMaxHpBeforeP5 = galienUseMaxHp;
                galienUseCustomPhaseBeforeP5 = galienUseCustomPhase;
                galienPhase2HpBeforeP5 = ClampGalienPhase2Hp(galienPhase2Hp, ResolvePhase2MaxHp());
                galienPhase3HpBeforeP5 = ClampGalienPhase3Hp(galienPhase3Hp, galienPhase2Hp);
                galienHasStoredStateBeforeP5 = true;
            }

            galienP5Hp = true;
            galienUseMaxHp = true;
            galienUseCustomPhase = false;
            galienMaxHp = P5GalienHp;
        }
        else
        {
            if (galienP5Hp && galienHasStoredStateBeforeP5)
            {
                galienMaxHp = ClampGalienHp(galienMaxHpBeforeP5);
                galienUseMaxHp = galienUseMaxHpBeforeP5;
                galienUseCustomPhase = galienUseCustomPhaseBeforeP5;
                galienPhase2Hp = ClampGalienPhase2Hp(galienPhase2HpBeforeP5, ResolvePhase2MaxHp());
                galienPhase3Hp = ClampGalienPhase3Hp(galienPhase3HpBeforeP5, galienPhase2Hp);
            }

            galienP5Hp = false;
            galienHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!galienP5Hp)
        {
            return;
        }

        if (!galienHasStoredStateBeforeP5)
        {
            galienMaxHpBeforeP5 = ClampGalienHp(galienMaxHp);
            galienUseMaxHpBeforeP5 = galienUseMaxHp;
            galienUseCustomPhaseBeforeP5 = galienUseCustomPhase;
            galienPhase2HpBeforeP5 = ClampGalienPhase2Hp(galienPhase2Hp, ResolvePhase2MaxHp());
            galienPhase3HpBeforeP5 = ClampGalienPhase3Hp(galienPhase3Hp, galienPhase2Hp);
            galienHasStoredStateBeforeP5 = true;
        }

        galienUseMaxHp = true;
        galienUseCustomPhase = false;
        galienMaxHp = P5GalienHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        galienPhase2Hp = ClampGalienPhase2Hp(galienPhase2Hp, ResolvePhase2MaxHp());
        galienPhase3Hp = ClampGalienPhase3Hp(galienPhase3Hp, galienPhase2Hp);
    }

    internal static void ApplyGalienHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindGalienHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyGalienHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindGalienHealthManager(out HealthManager? hm))
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
            if (fsm == null || fsm.gameObject == null || !IsGalienPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsGalienPhaseControlFsm(fsm))
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

        if (!moduleActive || !IsGalien(self))
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
            ApplyGalienHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsGalien(self))
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
            ApplyGalienHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }

        _ = self.StartCoroutine(DeferredApplyPhaseThresholds(self));
    }

    private static void OnPlayMakerFsmOnEnable_Galien(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsGalienPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static void OnPlayMakerFsmStart_Galien(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsGalienPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static void OnHealthManagerUpdate(On.HealthManager.orig_Update orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsGalien(self))
        {
            return;
        }

        if (!ShouldApplySettings(self.gameObject) || !ShouldUseCustomPhaseThresholds())
        {
            return;
        }

        TryForceGalienSummonTransitions(self.gameObject, self.hp);
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsGalien(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyGalienHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsGalien(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyGalienHealth(hm.gameObject, hm);
            }
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    private static IEnumerator DeferredApplyPhaseThresholds(HealthManager hm)
    {
        for (int attempt = 0; attempt < GalienPhaseThresholdReapplyAttempts; attempt++)
        {
            if (!moduleActive || hm == null || hm.gameObject == null || !IsGalien(hm) || !ShouldApplySettings(hm.gameObject))
            {
                yield break;
            }

            if (!ShouldUseCustomPhaseThresholds())
            {
                yield break;
            }

            ApplyPhaseThresholdSettingsIfPresent();
            yield return new WaitForSeconds(GalienPhaseThresholdReapplyInterval);
        }
    }

    private static void TryForceGalienSummonTransitions(GameObject bossObject, int currentHp)
    {
        if (bossObject == null || !ShouldApplySettings(bossObject) || !ShouldUseCustomPhaseThresholds())
        {
            return;
        }

        int phase2Threshold = ClampGalienPhase2Hp(galienPhase2Hp, ResolvePhase2MaxHp());
        int phase3Threshold = ClampGalienPhase3Hp(galienPhase3Hp, phase2Threshold);

        foreach (PlayMakerFSM fsm in bossObject.GetComponents<PlayMakerFSM>())
        {
            if (fsm == null || !IsGalienPhaseControlFsm(fsm))
            {
                continue;
            }

            string activeState = fsm.ActiveStateName ?? string.Empty;
            if ((string.Equals(activeState, GalienPhaseIdleStateName, StringComparison.Ordinal)
                || string.Equals(activeState, GalienPhaseCheckState1Name, StringComparison.Ordinal))
                && currentHp <= phase2Threshold)
            {
                fsm.Fsm.SetState(GalienPhaseSummonAnticStateName);
                continue;
            }

            if ((string.Equals(activeState, GalienPhaseIdle2StateName, StringComparison.Ordinal)
                || string.Equals(activeState, GalienPhaseCheckState2Name, StringComparison.Ordinal))
                && currentHp <= phase3Threshold)
            {
                fsm.Fsm.SetState(GalienPhaseSummonAntic2StateName);
            }
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, GalienScene, StringComparison.Ordinal))
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
            ApplyGalienHealthIfPresent();
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

    private static bool IsGalien(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsGalienObject(hm.gameObject);
    }

    private static bool IsGalienObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, GalienScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(GalienName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsGalienObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => galienUseMaxHp;

    private static bool ShouldUseCustomPhaseThresholds() => galienUseCustomPhase && !galienP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, GalienScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, GalienScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsGalienPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsGalienObject(fsm.gameObject))
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, GalienPhaseFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState(GalienPhaseCheckState1Name) != null
            && fsm.Fsm?.GetState(GalienPhaseCheckState2Name) != null
            && (
                fsm.FsmVariables.GetFsmInt(GalienPhase2VariableName) != null
                || fsm.FsmVariables.GetFsmInt(GalienPhase3VariableName) != null
            );
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsGalienPhaseControlFsm(fsm))
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
                ClampGalienPhase2Hp(galienPhase2Hp, phase2MaxHp),
                ClampGalienPhase3Hp(galienPhase3Hp, galienPhase2Hp)
            )
            : GetVanillaPhaseThresholds(fsm);

        SetPhaseThresholdsOnFsm(fsm, thresholds.phase2Hp, thresholds.phase3Hp, ShouldUseCustomPhaseThresholds());
    }

    private static void SetPhaseThresholdsOnFsm(PlayMakerFSM fsm, int phase2Hp, int phase3Hp, bool useExactCustomThresholds)
    {
        int targetPhase2Hp = ClampGalienPhase2Hp(phase2Hp, ResolvePhase2MaxHp());
        int targetPhase3Hp = ClampGalienPhase3Hp(phase3Hp, targetPhase2Hp);

        FsmInt? phase2Variable = fsm.FsmVariables.GetFsmInt(GalienPhase2VariableName);
        if (phase2Variable != null)
        {
            phase2Variable.Value = targetPhase2Hp;
        }

        FsmInt? phase3Variable = fsm.FsmVariables.GetFsmInt(GalienPhase3VariableName);
        if (phase3Variable != null)
        {
            phase3Variable.Value = targetPhase3Hp;
        }

        SetThresholdOnCheckState(fsm, GalienPhaseCheckState1Name, GalienPhase2VariableName, targetPhase2Hp, useExactCustomThresholds);
        SetThresholdOnCheckState(fsm, GalienPhaseCheckState2Name, GalienPhase3VariableName, targetPhase3Hp, useExactCustomThresholds);

        FsmState? initState = fsm.Fsm?.GetState("Init");
        if (initState?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in initState.Actions)
        {
            if (action is not SetIntValue setIntValue || setIntValue.intVariable == null || setIntValue.intValue == null)
            {
                continue;
            }

            if (string.Equals(setIntValue.intVariable.Name, GalienPhase2VariableName, StringComparison.Ordinal))
            {
                setIntValue.intValue.Value = targetPhase2Hp;
                setIntValue.intVariable.Value = targetPhase2Hp;
            }
            else if (string.Equals(setIntValue.intVariable.Name, GalienPhase3VariableName, StringComparison.Ordinal))
            {
                setIntValue.intValue.Value = targetPhase3Hp;
                setIntValue.intVariable.Value = targetPhase3Hp;
            }
        }
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
            if (action is IntCompare compare && compare.integer2 != null)
            {
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
    }

    private static void RememberVanillaPhaseThresholds(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaPhaseThresholdsByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int phase2Hp = DefaultGalienPhase2Hp;
        int phase3Hp = DefaultGalienPhase3Hp;

        bool phase2Captured = TryReadThresholdFromCheckState(fsm, GalienPhaseCheckState1Name, out int capturedPhase2Hp);
        if (phase2Captured)
        {
            phase2Hp = capturedPhase2Hp;
        }

        bool phase3Captured = TryReadThresholdFromCheckState(fsm, GalienPhaseCheckState2Name, out int capturedPhase3Hp);
        if (phase3Captured)
        {
            phase3Hp = capturedPhase3Hp;
        }

        if (!phase2Captured)
        {
            FsmInt? phase2Variable = fsm.FsmVariables.GetFsmInt(GalienPhase2VariableName);
            if (phase2Variable != null && phase2Variable.Value > 0)
            {
                phase2Hp = phase2Variable.Value;
            }
        }

        if (!phase3Captured)
        {
            FsmInt? phase3Variable = fsm.FsmVariables.GetFsmInt(GalienPhase3VariableName);
            if (phase3Variable != null && phase3Variable.Value > 0)
            {
                phase3Hp = phase3Variable.Value;
            }
        }

        phase2Hp = ClampGalienPhase2Hp(phase2Hp, ResolvePhase2MaxHp());
        phase3Hp = ClampGalienPhase3Hp(phase3Hp, phase2Hp);
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
                ClampGalienPhase2Hp(thresholds.phase2Hp, ResolvePhase2MaxHp()),
                ClampGalienPhase3Hp(thresholds.phase3Hp, thresholds.phase2Hp)
            );
        }

        RememberVanillaPhaseThresholds(fsm);
        if (vanillaPhaseThresholdsByFsm.TryGetValue(fsmId, out thresholds))
        {
            return (
                ClampGalienPhase2Hp(thresholds.phase2Hp, ResolvePhase2MaxHp()),
                ClampGalienPhase3Hp(thresholds.phase3Hp, thresholds.phase2Hp)
            );
        }

        return (DefaultGalienPhase2Hp, DefaultGalienPhase3Hp);
    }

    private static void ApplyGalienHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampGalienHp(galienMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsGalienObject(boss))
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

        int targetHp = ClampGalienHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindGalienHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsGalien(candidate))
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

        hp = Math.Max(hp, DefaultGalienVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultGalienVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultGalienVanillaHp);
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

    private static int ClampGalienHp(int value)
    {
        if (value < MinGalienHp)
        {
            return MinGalienHp;
        }

        return value > MaxGalienHp ? MaxGalienHp : value;
    }

    internal static int GetPhase2MaxHpForUi() => ResolvePhase2MaxHp();

    private static int ClampGalienPhase2Hp(int value, int maxHp)
    {
        int clampedMaxHp = ClampGalienHp(maxHp);
        if (value < MinGalienPhase2Hp)
        {
            return MinGalienPhase2Hp;
        }

        return value > clampedMaxHp ? clampedMaxHp : value;
    }

    private static int ClampGalienPhase3Hp(int value, int phase2Hp)
    {
        int clampedPhase2Hp = ClampGalienPhase2Hp(phase2Hp, ResolvePhase2MaxHp());
        int maxPhase3Hp = Math.Max(MinGalienPhase3Hp, clampedPhase2Hp - 1);

        if (value < MinGalienPhase3Hp)
        {
            return MinGalienPhase3Hp;
        }

        return value > maxPhase3Hp ? maxPhase3Hp : value;
    }

    private static int ResolvePhase2MaxHp()
    {
        if (ShouldUseCustomHp())
        {
            return ClampGalienHp(galienMaxHp);
        }

        if (TryFindGalienHealthManager(out HealthManager? hm) && hm != null && TryGetVanillaHp(hm, out int vanillaHp))
        {
            return ClampGalienHp(vanillaHp);
        }

        return DefaultGalienVanillaHp;
    }

    private static int SafeIncrementThreshold(int value)
    {
        return value >= int.MaxValue ? int.MaxValue : value + 1;
    }
}
