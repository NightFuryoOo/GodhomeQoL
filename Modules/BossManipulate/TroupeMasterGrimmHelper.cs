using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class TroupeMasterGrimmHelper : Module
{
    private const string TroupeMasterGrimmScene = "GG_Grimm";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string TroupeMasterGrimmName = "Grimm Boss";
    private const string RagePhase1VariableName = "Rage HP 1";
    private const string RagePhase2VariableName = "Rage HP 2";
    private const string RagePhase3VariableName = "Rage HP 3";
    private const string BalloonCheckStateName = "Balloon?";
    private const int DefaultTroupeMasterGrimmMaxHp = 1000;
    private const int DefaultTroupeMasterGrimmVanillaHp = 1000;
    private const int DefaultTroupeMasterGrimmPhase2Hp = 750;
    private const int DefaultTroupeMasterGrimmPhase3Hp = 500;
    private const int DefaultTroupeMasterGrimmPhase4Hp = 250;
    private const int P5TroupeMasterGrimmHp = 1000;
    private const int MinTroupeMasterGrimmHp = 1;
    private const int MaxTroupeMasterGrimmHp = 999999;
    private const int MinTroupeMasterGrimmPhase2Hp = 1;
    private const int MinTroupeMasterGrimmPhase3Hp = 1;
    private const int MinTroupeMasterGrimmPhase4Hp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool troupeMasterGrimmUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool troupeMasterGrimmP5Hp = false;

    [LocalSetting]
    internal static int troupeMasterGrimmMaxHp = DefaultTroupeMasterGrimmMaxHp;

    [LocalSetting]
    internal static bool troupeMasterGrimmUseCustomPhase = false;

    [LocalSetting]
    internal static int troupeMasterGrimmPhase2Hp = DefaultTroupeMasterGrimmPhase2Hp;

    [LocalSetting]
    internal static int troupeMasterGrimmPhase3Hp = DefaultTroupeMasterGrimmPhase3Hp;

    [LocalSetting]
    internal static int troupeMasterGrimmPhase4Hp = DefaultTroupeMasterGrimmPhase4Hp;

    [LocalSetting]
    internal static int troupeMasterGrimmMaxHpBeforeP5 = DefaultTroupeMasterGrimmMaxHp;

    [LocalSetting]
    internal static bool troupeMasterGrimmUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int troupeMasterGrimmPhase2HpBeforeP5 = DefaultTroupeMasterGrimmPhase2Hp;

    [LocalSetting]
    internal static int troupeMasterGrimmPhase3HpBeforeP5 = DefaultTroupeMasterGrimmPhase3Hp;

    [LocalSetting]
    internal static int troupeMasterGrimmPhase4HpBeforeP5 = DefaultTroupeMasterGrimmPhase4Hp;

    [LocalSetting]
    internal static bool troupeMasterGrimmUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool troupeMasterGrimmHasStoredStateBeforeP5 = false;

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
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_TroupeMasterGrimm;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_TroupeMasterGrimm;
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
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_TroupeMasterGrimm;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_TroupeMasterGrimm;
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
            ApplyTroupeMasterGrimmHealthIfPresent();
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
            if (!troupeMasterGrimmP5Hp)
            {
                int referenceHp = GetPhaseReferenceHp(null);
                troupeMasterGrimmMaxHpBeforeP5 = ClampTroupeMasterGrimmHp(troupeMasterGrimmMaxHp);
                troupeMasterGrimmUseMaxHpBeforeP5 = troupeMasterGrimmUseMaxHp;
                troupeMasterGrimmUseCustomPhaseBeforeP5 = troupeMasterGrimmUseCustomPhase;
                troupeMasterGrimmPhase2HpBeforeP5 = ClampTroupeMasterGrimmPhase2Hp(troupeMasterGrimmPhase2Hp, referenceHp);
                troupeMasterGrimmPhase3HpBeforeP5 = ClampTroupeMasterGrimmPhase3Hp(troupeMasterGrimmPhase3Hp, troupeMasterGrimmPhase2HpBeforeP5);
                troupeMasterGrimmPhase4HpBeforeP5 = ClampTroupeMasterGrimmPhase4Hp(troupeMasterGrimmPhase4Hp, troupeMasterGrimmPhase3HpBeforeP5);
                troupeMasterGrimmHasStoredStateBeforeP5 = true;
            }

            troupeMasterGrimmP5Hp = true;
            troupeMasterGrimmUseMaxHp = true;
            troupeMasterGrimmUseCustomPhase = false;
            troupeMasterGrimmMaxHp = P5TroupeMasterGrimmHp;
        }
        else
        {
            if (troupeMasterGrimmP5Hp && troupeMasterGrimmHasStoredStateBeforeP5)
            {
                troupeMasterGrimmMaxHp = ClampTroupeMasterGrimmHp(troupeMasterGrimmMaxHpBeforeP5);
                troupeMasterGrimmUseMaxHp = troupeMasterGrimmUseMaxHpBeforeP5;
                troupeMasterGrimmUseCustomPhase = troupeMasterGrimmUseCustomPhaseBeforeP5;
                troupeMasterGrimmPhase2Hp = troupeMasterGrimmPhase2HpBeforeP5;
                troupeMasterGrimmPhase3Hp = troupeMasterGrimmPhase3HpBeforeP5;
                troupeMasterGrimmPhase4Hp = troupeMasterGrimmPhase4HpBeforeP5;
            }

            troupeMasterGrimmP5Hp = false;
            troupeMasterGrimmHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!troupeMasterGrimmP5Hp)
        {
            return;
        }

        if (!troupeMasterGrimmHasStoredStateBeforeP5)
        {
            int referenceHp = GetPhaseReferenceHp(null);
            troupeMasterGrimmMaxHpBeforeP5 = ClampTroupeMasterGrimmHp(troupeMasterGrimmMaxHp);
            troupeMasterGrimmUseMaxHpBeforeP5 = troupeMasterGrimmUseMaxHp;
            troupeMasterGrimmUseCustomPhaseBeforeP5 = troupeMasterGrimmUseCustomPhase;
            troupeMasterGrimmPhase2HpBeforeP5 = ClampTroupeMasterGrimmPhase2Hp(troupeMasterGrimmPhase2Hp, referenceHp);
            troupeMasterGrimmPhase3HpBeforeP5 = ClampTroupeMasterGrimmPhase3Hp(troupeMasterGrimmPhase3Hp, troupeMasterGrimmPhase2HpBeforeP5);
            troupeMasterGrimmPhase4HpBeforeP5 = ClampTroupeMasterGrimmPhase4Hp(troupeMasterGrimmPhase4Hp, troupeMasterGrimmPhase3HpBeforeP5);
            troupeMasterGrimmHasStoredStateBeforeP5 = true;
        }

        troupeMasterGrimmUseMaxHp = true;
        troupeMasterGrimmUseCustomPhase = false;
        troupeMasterGrimmMaxHp = P5TroupeMasterGrimmHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        int referenceHp = GetPhaseReferenceHp(null);
        troupeMasterGrimmPhase2Hp = ClampTroupeMasterGrimmPhase2Hp(troupeMasterGrimmPhase2Hp, referenceHp);
        troupeMasterGrimmPhase3Hp = ClampTroupeMasterGrimmPhase3Hp(troupeMasterGrimmPhase3Hp, troupeMasterGrimmPhase2Hp);
        troupeMasterGrimmPhase4Hp = ClampTroupeMasterGrimmPhase4Hp(troupeMasterGrimmPhase4Hp, troupeMasterGrimmPhase3Hp);
    }

    internal static void ApplyTroupeMasterGrimmHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindTroupeMasterGrimmHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyTroupeMasterGrimmHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindTroupeMasterGrimmHealthManager(out HealthManager? hm))
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
            if (fsm == null || fsm.gameObject == null || !IsTroupeMasterGrimmPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsTroupeMasterGrimmPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            (int phase2Hp, int phase3Hp, int phase4Hp) = GetVanillaPhaseThresholds(fsm.gameObject);
            SetPhaseThresholdsOnFsm(fsm, phase2Hp, phase3Hp, phase4Hp, useExactCustomThresholds: false);
        }
    }

    private static void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsTroupeMasterGrimm(self))
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
            ApplyTroupeMasterGrimmHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsTroupeMasterGrimm(self))
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
            ApplyTroupeMasterGrimmHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnPlayMakerFsmOnEnable_TroupeMasterGrimm(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsTroupeMasterGrimmPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static void OnPlayMakerFsmStart_TroupeMasterGrimm(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsTroupeMasterGrimmPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsTroupeMasterGrimm(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyTroupeMasterGrimmHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsTroupeMasterGrimm(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyTroupeMasterGrimmHealth(hm.gameObject, hm);
            }

            ApplyPhaseThresholdSettingsIfPresent();
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, TroupeMasterGrimmScene, StringComparison.Ordinal))
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
            ApplyTroupeMasterGrimmHealthIfPresent();
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

    private static bool IsTroupeMasterGrimm(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsTroupeMasterGrimmObject(hm.gameObject);
    }

    private static bool IsTroupeMasterGrimmObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, TroupeMasterGrimmScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(TroupeMasterGrimmName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsTroupeMasterGrimmObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => troupeMasterGrimmUseMaxHp;

    private static bool ShouldUseCustomPhaseThresholds() => troupeMasterGrimmUseCustomPhase && !troupeMasterGrimmP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, TroupeMasterGrimmScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, TroupeMasterGrimmScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsTroupeMasterGrimmPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsTroupeMasterGrimmObject(fsm.gameObject))
        {
            return false;
        }

        return string.Equals(fsm.FsmName, "Control", StringComparison.Ordinal)
            || fsm.Fsm?.GetState("Balloon?") != null;
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsTroupeMasterGrimmPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        int referenceHp = GetPhaseReferenceHp(fsm.gameObject);
        (int phase2Hp, int phase3Hp, int phase4Hp) thresholds = ShouldUseCustomPhaseThresholds()
            ? (
                ClampTroupeMasterGrimmPhase2Hp(troupeMasterGrimmPhase2Hp, referenceHp),
                ClampTroupeMasterGrimmPhase3Hp(troupeMasterGrimmPhase3Hp, troupeMasterGrimmPhase2Hp),
                ClampTroupeMasterGrimmPhase4Hp(troupeMasterGrimmPhase4Hp, troupeMasterGrimmPhase3Hp)
            )
            : GetVanillaPhaseThresholds(fsm.gameObject);

        SetPhaseThresholdsOnFsm(fsm, thresholds.phase2Hp, thresholds.phase3Hp, thresholds.phase4Hp, ShouldUseCustomPhaseThresholds());
    }

    private static void SetPhaseThresholdsOnFsm(PlayMakerFSM fsm, int phase2Hp, int phase3Hp, int phase4Hp, bool useExactCustomThresholds)
    {
        if (fsm == null)
        {
            return;
        }

        var fsmVariables = fsm.FsmVariables;
        if (fsmVariables == null)
        {
            return;
        }

        int referenceHp = GetPhaseReferenceHp(fsm?.gameObject);
        int targetPhase2Hp = ClampTroupeMasterGrimmPhase2Hp(phase2Hp, referenceHp);
        int targetPhase3Hp = ClampTroupeMasterGrimmPhase3Hp(phase3Hp, targetPhase2Hp);
        int targetPhase4Hp = ClampTroupeMasterGrimmPhase4Hp(phase4Hp, targetPhase3Hp);

        FsmInt? phase2Variable = fsmVariables.GetFsmInt(RagePhase1VariableName);
        if (phase2Variable != null)
        {
            phase2Variable.Value = targetPhase2Hp;
        }

        FsmInt? phase3Variable = fsmVariables.GetFsmInt(RagePhase2VariableName);
        if (phase3Variable != null)
        {
            phase3Variable.Value = targetPhase3Hp;
        }

        FsmInt? phase4Variable = fsmVariables.GetFsmInt(RagePhase3VariableName);
        if (phase4Variable != null)
        {
            phase4Variable.Value = targetPhase4Hp;
        }

        SetThresholdOnBalloonCheckState(fsm!, targetPhase2Hp, targetPhase3Hp, targetPhase4Hp, useExactCustomThresholds);
    }

    private static void SetThresholdOnBalloonCheckState(PlayMakerFSM fsm, int phase2Hp, int phase3Hp, int phase4Hp, bool useExactCustomThresholds)
    {
        FsmState? balloonCheckState = fsm.Fsm?.GetState(BalloonCheckStateName);
        if (balloonCheckState?.Actions == null)
        {
            return;
        }

        int thresholdActionIndex = 0;
        foreach (FsmStateAction action in balloonCheckState.Actions)
        {
            if (action is not IntTestToBool compare || compare.int2 == null)
            {
                continue;
            }

            string variableName = thresholdActionIndex switch
            {
                0 => RagePhase1VariableName,
                1 => RagePhase2VariableName,
                2 => RagePhase3VariableName,
                _ => string.Empty
            };
            int targetThreshold = thresholdActionIndex switch
            {
                0 => phase2Hp,
                1 => phase3Hp,
                2 => phase4Hp,
                _ => 0
            };
            thresholdActionIndex++;

            if (string.IsNullOrEmpty(variableName))
            {
                continue;
            }

            if (useExactCustomThresholds)
            {
                compare.int2.UseVariable = false;
                compare.int2.Name = string.Empty;
                compare.int2.Value = SafeIncrementThreshold(targetThreshold);
            }
            else
            {
                compare.int2.UseVariable = true;
                compare.int2.Name = variableName;
                compare.int2.Value = targetThreshold;
            }
        }
    }

    private static (int phase2Hp, int phase3Hp, int phase4Hp) GetVanillaPhaseThresholds(GameObject bossObject)
    {
        int referenceHp = GetPhaseReferenceHp(bossObject);
        int quarterHp = referenceHp / 4;
        int phase2Hp = referenceHp - quarterHp;
        int phase3Hp = phase2Hp - quarterHp;
        int phase4Hp = phase3Hp - quarterHp;

        int clampedPhase2Hp = ClampTroupeMasterGrimmPhase2Hp(phase2Hp, referenceHp);
        int clampedPhase3Hp = ClampTroupeMasterGrimmPhase3Hp(phase3Hp, clampedPhase2Hp);
        int clampedPhase4Hp = ClampTroupeMasterGrimmPhase4Hp(phase4Hp, clampedPhase3Hp);
        return (clampedPhase2Hp, clampedPhase3Hp, clampedPhase4Hp);
    }

    private static int GetPhaseReferenceHp(GameObject? bossObject)
    {
        if (bossObject != null)
        {
            HealthManager? hm = bossObject.GetComponent<HealthManager>();
            if (hm != null)
            {
                int maxHp = ReadMaxHp(hm);
                if (maxHp > 0)
                {
                    return ClampTroupeMasterGrimmHp(maxHp);
                }

                if (hm.hp > 0)
                {
                    return ClampTroupeMasterGrimmHp(hm.hp);
                }
            }
        }

        int configuredHp = ClampTroupeMasterGrimmHp(troupeMasterGrimmMaxHp);
        return configuredHp > 0 ? configuredHp : DefaultTroupeMasterGrimmVanillaHp;
    }

    private static void ApplyTroupeMasterGrimmHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampTroupeMasterGrimmHp(troupeMasterGrimmMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsTroupeMasterGrimmObject(boss))
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

        int targetHp = ClampTroupeMasterGrimmHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindTroupeMasterGrimmHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsTroupeMasterGrimm(candidate))
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

        hp = Math.Max(hp, DefaultTroupeMasterGrimmVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultTroupeMasterGrimmVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultTroupeMasterGrimmVanillaHp);
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

    private static int ClampTroupeMasterGrimmHp(int value)
    {
        if (value < MinTroupeMasterGrimmHp)
        {
            return MinTroupeMasterGrimmHp;
        }

        return value > MaxTroupeMasterGrimmHp ? MaxTroupeMasterGrimmHp : value;
    }

    private static int ClampTroupeMasterGrimmPhase2Hp(int value, int referenceHp)
    {
        int maxPhase2Hp = Math.Max(MinTroupeMasterGrimmPhase2Hp, ClampTroupeMasterGrimmHp(referenceHp));
        if (value < MinTroupeMasterGrimmPhase2Hp)
        {
            return MinTroupeMasterGrimmPhase2Hp;
        }

        return value > maxPhase2Hp ? maxPhase2Hp : value;
    }

    private static int ClampTroupeMasterGrimmPhase3Hp(int value, int phase2Hp)
    {
        int clampedPhase2Hp = phase2Hp < MinTroupeMasterGrimmPhase2Hp
            ? MinTroupeMasterGrimmPhase2Hp
            : phase2Hp;
        int maxPhase3Hp = Math.Max(MinTroupeMasterGrimmPhase3Hp, clampedPhase2Hp - 1);
        if (value < MinTroupeMasterGrimmPhase3Hp)
        {
            return MinTroupeMasterGrimmPhase3Hp;
        }

        return value > maxPhase3Hp ? maxPhase3Hp : value;
    }

    private static int ClampTroupeMasterGrimmPhase4Hp(int value, int phase3Hp)
    {
        int clampedPhase3Hp = phase3Hp < MinTroupeMasterGrimmPhase3Hp
            ? MinTroupeMasterGrimmPhase3Hp
            : phase3Hp;
        int maxPhase4Hp = Math.Max(MinTroupeMasterGrimmPhase4Hp, clampedPhase3Hp - 1);
        if (value < MinTroupeMasterGrimmPhase4Hp)
        {
            return MinTroupeMasterGrimmPhase4Hp;
        }

        return value > maxPhase4Hp ? maxPhase4Hp : value;
    }

    private static int SafeIncrementThreshold(int value)
    {
        return value >= int.MaxValue ? int.MaxValue : value + 1;
    }
}
