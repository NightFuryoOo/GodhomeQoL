using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class PureVesselHelper : Module
{
    private const string PureVesselScene = "GG_Hollow_Knight";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string PureVesselName = "HK Prime";
    private const string PureVesselPhase2VariableName = "Half HP";
    private const string PureVesselPhase3VariableName = "Quarter HP";
    private const int DefaultPureVesselMaxHp = 1850;
    private const int DefaultPureVesselVanillaHp = 1850;
    private const int DefaultPureVesselPhase2Hp = 1232;
    private const int DefaultPureVesselPhase3Hp = 616;
    private const int P5PureVesselHp = 1600;
    private const int MinPureVesselHp = 1;
    private const int MaxPureVesselHp = 999999;
    private const int MinPureVesselPhase2Hp = 2;
    private const int MaxPureVesselPhase2Hp = DefaultPureVesselVanillaHp;
    private const int MinPureVesselPhase3Hp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool pureVesselUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool pureVesselP5Hp = false;

    [LocalSetting]
    internal static int pureVesselMaxHp = DefaultPureVesselMaxHp;

    [LocalSetting]
    internal static bool pureVesselUseCustomPhase = false;

    [LocalSetting]
    internal static int pureVesselPhase2Hp = DefaultPureVesselPhase2Hp;

    [LocalSetting]
    internal static int pureVesselPhase3Hp = DefaultPureVesselPhase3Hp;

    [LocalSetting]
    internal static int pureVesselMaxHpBeforeP5 = DefaultPureVesselMaxHp;

    [LocalSetting]
    internal static bool pureVesselUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int pureVesselPhase2HpBeforeP5 = DefaultPureVesselPhase2Hp;

    [LocalSetting]
    internal static int pureVesselPhase3HpBeforeP5 = DefaultPureVesselPhase3Hp;

    [LocalSetting]
    internal static bool pureVesselUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool pureVesselHasStoredStateBeforeP5 = false;

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
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_PureVessel;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_PureVessel;
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
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_PureVessel;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_PureVessel;
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
            ApplyPureVesselHealthIfPresent();
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
            if (!pureVesselP5Hp)
            {
                pureVesselMaxHpBeforeP5 = ClampPureVesselHp(pureVesselMaxHp);
                pureVesselUseMaxHpBeforeP5 = pureVesselUseMaxHp;
                pureVesselUseCustomPhaseBeforeP5 = pureVesselUseCustomPhase;
                pureVesselPhase2HpBeforeP5 = ClampPureVesselPhase2Hp(pureVesselPhase2Hp);
                pureVesselPhase3HpBeforeP5 = ClampPureVesselPhase3Hp(pureVesselPhase3Hp, pureVesselPhase2Hp);
                pureVesselHasStoredStateBeforeP5 = true;
            }

            pureVesselP5Hp = true;
            pureVesselUseMaxHp = true;
            pureVesselUseCustomPhase = false;
            pureVesselMaxHp = P5PureVesselHp;
        }
        else
        {
            if (pureVesselP5Hp && pureVesselHasStoredStateBeforeP5)
            {
                pureVesselMaxHp = ClampPureVesselHp(pureVesselMaxHpBeforeP5);
                pureVesselUseMaxHp = pureVesselUseMaxHpBeforeP5;
                pureVesselUseCustomPhase = pureVesselUseCustomPhaseBeforeP5;
                pureVesselPhase2Hp = ClampPureVesselPhase2Hp(pureVesselPhase2HpBeforeP5);
                pureVesselPhase3Hp = ClampPureVesselPhase3Hp(pureVesselPhase3HpBeforeP5, pureVesselPhase2Hp);
            }

            pureVesselP5Hp = false;
            pureVesselHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!pureVesselP5Hp)
        {
            return;
        }

        if (!pureVesselHasStoredStateBeforeP5)
        {
            pureVesselMaxHpBeforeP5 = ClampPureVesselHp(pureVesselMaxHp);
            pureVesselUseMaxHpBeforeP5 = pureVesselUseMaxHp;
            pureVesselUseCustomPhaseBeforeP5 = pureVesselUseCustomPhase;
            pureVesselPhase2HpBeforeP5 = ClampPureVesselPhase2Hp(pureVesselPhase2Hp);
            pureVesselPhase3HpBeforeP5 = ClampPureVesselPhase3Hp(pureVesselPhase3Hp, pureVesselPhase2Hp);
            pureVesselHasStoredStateBeforeP5 = true;
        }

        pureVesselUseMaxHp = true;
        pureVesselUseCustomPhase = false;
        pureVesselMaxHp = P5PureVesselHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        pureVesselPhase2Hp = ClampPureVesselPhase2Hp(pureVesselPhase2Hp);
        pureVesselPhase3Hp = ClampPureVesselPhase3Hp(pureVesselPhase3Hp, pureVesselPhase2Hp);
    }

    internal static void ApplyPureVesselHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindPureVesselHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyPureVesselHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindPureVesselHealthManager(out HealthManager? hm))
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
            if (fsm == null || fsm.gameObject == null || !IsPureVesselPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsPureVesselPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            SetPhaseThresholdsOnFsm(
                fsm,
                DefaultPureVesselPhase2Hp,
                DefaultPureVesselPhase3Hp,
                useVanillaVariables: true
            );
        }
    }

    private static void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsPureVessel(self))
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
            ApplyPureVesselHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsPureVessel(self))
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
            ApplyPureVesselHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnPlayMakerFsmOnEnable_PureVessel(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsPureVesselPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static void OnPlayMakerFsmStart_PureVessel(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsPureVesselPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsPureVessel(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyPureVesselHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsPureVessel(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyPureVesselHealth(hm.gameObject, hm);
            }
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, PureVesselScene, StringComparison.Ordinal))
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
            ApplyPureVesselHealthIfPresent();
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

    private static bool IsPureVessel(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsPureVesselObject(hm.gameObject);
    }

    private static bool IsPureVesselObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, PureVesselScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(PureVesselName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsPureVesselObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => pureVesselUseMaxHp;

    private static bool ShouldUseCustomPhaseThresholds() => pureVesselUseCustomPhase && !pureVesselP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, PureVesselScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, PureVesselScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsPureVesselPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsPureVesselObject(fsm.gameObject))
        {
            return false;
        }

        return string.Equals(fsm.FsmName, "Control", StringComparison.Ordinal)
            || fsm.Fsm?.GetState("Phase?") != null;
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsPureVesselPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        if (ShouldUseCustomPhaseThresholds())
        {
            SetPhaseThresholdsOnFsm(
                fsm,
                ClampPureVesselPhase2Hp(pureVesselPhase2Hp),
                ClampPureVesselPhase3Hp(pureVesselPhase3Hp, pureVesselPhase2Hp),
                useVanillaVariables: false
            );
        }
        else
        {
            SetPhaseThresholdsOnFsm(
                fsm,
                DefaultPureVesselPhase2Hp,
                DefaultPureVesselPhase3Hp,
                useVanillaVariables: true
            );
        }
    }

    private static void SetPhaseThresholdsOnFsm(PlayMakerFSM fsm, int phase2Hp, int phase3Hp, bool useVanillaVariables)
    {
        FsmState? phaseState = fsm.Fsm?.GetState("Phase?");
        if (phaseState?.Actions == null)
        {
            return;
        }

        int targetPhase2Hp = ClampPureVesselPhase2Hp(phase2Hp);
        int targetPhase3Hp = ClampPureVesselPhase3Hp(phase3Hp, targetPhase2Hp);

        if (!useVanillaVariables)
        {
            FsmInt? phase2Variable = fsm.FsmVariables.GetFsmInt(PureVesselPhase2VariableName);
            if (phase2Variable != null)
            {
                phase2Variable.Value = targetPhase2Hp;
            }

            FsmInt? phase3Variable = fsm.FsmVariables.GetFsmInt(PureVesselPhase3VariableName);
            if (phase3Variable != null)
            {
                phase3Variable.Value = targetPhase3Hp;
            }
        }

        int compareIndex = 0;
        foreach (FsmStateAction action in phaseState.Actions)
        {
            if (action is not IntCompare compare || compare.integer2 == null)
            {
                continue;
            }

            if (useVanillaVariables)
            {
                compare.integer2.UseVariable = true;
                compare.integer2.Name = compareIndex == 0 ? PureVesselPhase2VariableName : PureVesselPhase3VariableName;
            }
            else
            {
                compare.integer2.UseVariable = false;
                compare.integer2.Name = string.Empty;
                compare.integer2.Value = compareIndex == 0 ? targetPhase2Hp : targetPhase3Hp;
            }

            compareIndex++;
            if (compareIndex >= 2)
            {
                break;
            }
        }
    }

    private static void ApplyPureVesselHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampPureVesselHp(pureVesselMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsPureVesselObject(boss))
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

        int targetHp = ClampPureVesselHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindPureVesselHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsPureVessel(candidate))
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

        hp = Math.Max(hp, DefaultPureVesselVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultPureVesselVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultPureVesselVanillaHp);
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

    private static int ClampPureVesselHp(int value)
    {
        if (value < MinPureVesselHp)
        {
            return MinPureVesselHp;
        }

        return value > MaxPureVesselHp ? MaxPureVesselHp : value;
    }

    private static int ClampPureVesselPhase2Hp(int value)
    {
        if (value < MinPureVesselPhase2Hp)
        {
            return MinPureVesselPhase2Hp;
        }

        return value > MaxPureVesselPhase2Hp ? MaxPureVesselPhase2Hp : value;
    }

    private static int ClampPureVesselPhase3Hp(int value, int phase2Hp)
    {
        int clampedPhase2Hp = ClampPureVesselPhase2Hp(phase2Hp);
        int maxPhase3Hp = Math.Max(MinPureVesselPhase3Hp, clampedPhase2Hp - 1);

        if (value < MinPureVesselPhase3Hp)
        {
            return MinPureVesselPhase3Hp;
        }

        return value > maxPhase3Hp ? maxPhase3Hp : value;
    }
}
