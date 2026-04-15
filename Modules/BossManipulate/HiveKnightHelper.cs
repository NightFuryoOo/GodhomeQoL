using Modding;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class HiveKnightHelper : Module
{
    private const string HiveKnightScene = "GG_Hive_Knight";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string HiveKnightName = "Hive Knight";
    private const int DefaultHiveKnightMaxHp = 1300;
    private const int DefaultHiveKnightVanillaHp = 1300;
    private const int DefaultHiveKnightPhase2Hp = 580;
    private const int DefaultHiveKnightPhase3Hp = 350;
    private const int P5HiveKnightHp = 850;
    private const int MinHiveKnightHp = 1;
    private const int MaxHiveKnightHp = 999999;
    private const int MinHiveKnightPhase2Hp = 2;
    private const int MaxHiveKnightPhase2Hp = DefaultHiveKnightVanillaHp;
    private const int MinHiveKnightPhase3Hp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool hiveKnightUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool hiveKnightP5Hp = false;

    [LocalSetting]
    internal static int hiveKnightMaxHp = DefaultHiveKnightMaxHp;

    [LocalSetting]
    internal static int hiveKnightMaxHpBeforeP5 = DefaultHiveKnightMaxHp;

    [LocalSetting]
    internal static bool hiveKnightUseCustomPhase = false;

    [LocalSetting]
    internal static int hiveKnightPhase2Hp = DefaultHiveKnightPhase2Hp;

    [LocalSetting]
    internal static int hiveKnightPhase3Hp = DefaultHiveKnightPhase3Hp;

    [LocalSetting]
    internal static bool hiveKnightUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool hiveKnightHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool hiveKnightUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int hiveKnightPhase2HpBeforeP5 = DefaultHiveKnightPhase2Hp;

    [LocalSetting]
    internal static int hiveKnightPhase3HpBeforeP5 = DefaultHiveKnightPhase3Hp;

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
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_HiveKnight;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_HiveKnight;
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
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_HiveKnight;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_HiveKnight;
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
            ApplyHiveKnightHealthIfPresent();
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
            if (!hiveKnightP5Hp)
            {
                hiveKnightMaxHpBeforeP5 = ClampHiveKnightHp(hiveKnightMaxHp);
                hiveKnightUseMaxHpBeforeP5 = hiveKnightUseMaxHp;
                hiveKnightUseCustomPhaseBeforeP5 = hiveKnightUseCustomPhase;
                hiveKnightPhase2HpBeforeP5 = ClampHiveKnightPhase2Hp(hiveKnightPhase2Hp);
                hiveKnightPhase3HpBeforeP5 = ClampHiveKnightPhase3Hp(hiveKnightPhase3Hp, hiveKnightPhase2Hp);
                hiveKnightHasStoredStateBeforeP5 = true;
            }

            hiveKnightP5Hp = true;
            hiveKnightUseMaxHp = true;
            hiveKnightUseCustomPhase = false;
            hiveKnightMaxHp = P5HiveKnightHp;
        }
        else
        {
            if (hiveKnightP5Hp && hiveKnightHasStoredStateBeforeP5)
            {
                hiveKnightMaxHp = ClampHiveKnightHp(hiveKnightMaxHpBeforeP5);
                hiveKnightUseMaxHp = hiveKnightUseMaxHpBeforeP5;
                hiveKnightUseCustomPhase = hiveKnightUseCustomPhaseBeforeP5;
                hiveKnightPhase2Hp = ClampHiveKnightPhase2Hp(hiveKnightPhase2HpBeforeP5);
                hiveKnightPhase3Hp = ClampHiveKnightPhase3Hp(hiveKnightPhase3HpBeforeP5, hiveKnightPhase2Hp);
            }

            hiveKnightP5Hp = false;
            hiveKnightHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!hiveKnightP5Hp)
        {
            return;
        }

        if (!hiveKnightHasStoredStateBeforeP5)
        {
            hiveKnightMaxHpBeforeP5 = ClampHiveKnightHp(hiveKnightMaxHp);
            hiveKnightUseMaxHpBeforeP5 = hiveKnightUseMaxHp;
            hiveKnightUseCustomPhaseBeforeP5 = hiveKnightUseCustomPhase;
            hiveKnightPhase2HpBeforeP5 = ClampHiveKnightPhase2Hp(hiveKnightPhase2Hp);
            hiveKnightPhase3HpBeforeP5 = ClampHiveKnightPhase3Hp(hiveKnightPhase3Hp, hiveKnightPhase2Hp);
            hiveKnightHasStoredStateBeforeP5 = true;
        }

        hiveKnightUseMaxHp = true;
        hiveKnightUseCustomPhase = false;
        hiveKnightMaxHp = P5HiveKnightHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        hiveKnightPhase2Hp = ClampHiveKnightPhase2Hp(hiveKnightPhase2Hp);
        hiveKnightPhase3Hp = ClampHiveKnightPhase3Hp(hiveKnightPhase3Hp, hiveKnightPhase2Hp);
    }

    internal static void ApplyHiveKnightHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindHiveKnightHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyHiveKnightHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindHiveKnightHealthManager(out HealthManager? hm))
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
            if (fsm == null || fsm.gameObject == null || !IsHiveKnightPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsHiveKnightPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            RememberVanillaPhaseThresholds(fsm);
            (int phase2Hp, int phase3Hp) = GetVanillaPhaseThresholds(fsm);
            SetPhaseThresholdsOnFsm(fsm, phase2Hp, phase3Hp);
        }
    }

    private static void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsHiveKnight(self))
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
            ApplyHiveKnightHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsHiveKnight(self))
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
            ApplyHiveKnightHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnPlayMakerFsmOnEnable_HiveKnight(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsHiveKnightPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static void OnPlayMakerFsmStart_HiveKnight(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsHiveKnightPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsHiveKnight(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyHiveKnightHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsHiveKnight(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyHiveKnightHealth(hm.gameObject, hm);
            }
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, HiveKnightScene, StringComparison.Ordinal))
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
            ApplyHiveKnightHealthIfPresent();
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

    private static bool IsHiveKnight(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsHiveKnightObject(hm.gameObject);
    }

    private static bool IsHiveKnightObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, HiveKnightScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(HiveKnightName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsHiveKnightObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => hiveKnightUseMaxHp;

    private static bool ShouldUseCustomPhaseThresholds() => hiveKnightUseCustomPhase && !hiveKnightP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, HiveKnightScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, HiveKnightScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsHiveKnightPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsHiveKnightObject(fsm.gameObject))
        {
            return false;
        }

        return string.Equals(fsm.FsmName, "Control", StringComparison.Ordinal)
            || fsm.Fsm?.GetState("Phase Check") != null;
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsHiveKnightPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        RememberVanillaPhaseThresholds(fsm);
        (int phase2Hp, int phase3Hp) thresholds = ShouldUseCustomPhaseThresholds()
            ? (
                ClampHiveKnightPhase2Hp(hiveKnightPhase2Hp),
                ClampHiveKnightPhase3Hp(hiveKnightPhase3Hp, hiveKnightPhase2Hp)
            )
            : GetVanillaPhaseThresholds(fsm);

        SetPhaseThresholdsOnFsm(fsm, thresholds.phase2Hp, thresholds.phase3Hp);
    }

    private static void SetPhaseThresholdsOnFsm(PlayMakerFSM fsm, int phase2Hp, int phase3Hp)
    {
        int targetPhase2Hp = ClampHiveKnightPhase2Hp(phase2Hp);
        int targetPhase3Hp = ClampHiveKnightPhase3Hp(phase3Hp, targetPhase2Hp);

        FsmInt? phase2Variable = fsm.FsmVariables.GetFsmInt("P2 HP");
        if (phase2Variable != null)
        {
            phase2Variable.Value = targetPhase2Hp;
        }

        FsmInt? phase3Variable = fsm.FsmVariables.GetFsmInt("P3 HP");
        if (phase3Variable != null)
        {
            phase3Variable.Value = targetPhase3Hp;
        }

        FsmState? phaseCheck = fsm.Fsm?.GetState("Phase Check");
        if (phaseCheck?.Actions != null)
        {
            int compareIndex = 0;
            foreach (FsmStateAction action in phaseCheck.Actions)
            {
                if (action is not IntCompare compare || compare.integer2 == null)
                {
                    continue;
                }

                compare.integer2.Value = compareIndex == 0 ? targetPhase2Hp : targetPhase3Hp;
                compareIndex++;
                if (compareIndex >= 2)
                {
                    break;
                }
            }
        }

        FsmState? variant = fsm.Fsm?.GetState("Variant");
        if (variant?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in variant.Actions)
        {
            if (action is not SetIntValue setIntValue || setIntValue.intVariable == null || setIntValue.intValue == null)
            {
                continue;
            }

            if (string.Equals(setIntValue.intVariable.Name, "P2 HP", StringComparison.Ordinal))
            {
                setIntValue.intValue.Value = targetPhase2Hp;
                setIntValue.intVariable.Value = targetPhase2Hp;
            }
            else if (string.Equals(setIntValue.intVariable.Name, "P3 HP", StringComparison.Ordinal))
            {
                setIntValue.intValue.Value = targetPhase3Hp;
                setIntValue.intVariable.Value = targetPhase3Hp;
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

        int phase2Hp = DefaultHiveKnightPhase2Hp;
        int phase3Hp = DefaultHiveKnightPhase3Hp;
        bool capturedFromPhaseCheck = false;

        FsmState? phaseCheck = fsm.Fsm?.GetState("Phase Check");
        if (phaseCheck?.Actions != null)
        {
            int compareIndex = 0;
            foreach (FsmStateAction action in phaseCheck.Actions)
            {
                if (action is not IntCompare compare || compare.integer2 == null)
                {
                    continue;
                }

                if (compareIndex == 0)
                {
                    phase2Hp = compare.integer2.Value;
                    capturedFromPhaseCheck = true;
                }
                else if (compareIndex == 1)
                {
                    phase3Hp = compare.integer2.Value;
                    capturedFromPhaseCheck = true;
                    break;
                }

                compareIndex++;
            }
        }

        if (!capturedFromPhaseCheck)
        {
            FsmInt? phase2Variable = fsm.FsmVariables.GetFsmInt("P2 HP");
            if (phase2Variable != null && phase2Variable.Value > 0)
            {
                phase2Hp = phase2Variable.Value;
            }

            FsmInt? phase3Variable = fsm.FsmVariables.GetFsmInt("P3 HP");
            if (phase3Variable != null && phase3Variable.Value > 0)
            {
                phase3Hp = phase3Variable.Value;
            }
        }

        phase2Hp = ClampHiveKnightPhase2Hp(phase2Hp);
        phase3Hp = ClampHiveKnightPhase3Hp(phase3Hp, phase2Hp);
        vanillaPhaseThresholdsByFsm[fsmId] = (phase2Hp, phase3Hp);
    }

    private static (int phase2Hp, int phase3Hp) GetVanillaPhaseThresholds(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaPhaseThresholdsByFsm.TryGetValue(fsmId, out (int phase2Hp, int phase3Hp) thresholds))
        {
            return (
                ClampHiveKnightPhase2Hp(thresholds.phase2Hp),
                ClampHiveKnightPhase3Hp(thresholds.phase3Hp, thresholds.phase2Hp)
            );
        }

        RememberVanillaPhaseThresholds(fsm);
        if (vanillaPhaseThresholdsByFsm.TryGetValue(fsmId, out thresholds))
        {
            return (
                ClampHiveKnightPhase2Hp(thresholds.phase2Hp),
                ClampHiveKnightPhase3Hp(thresholds.phase3Hp, thresholds.phase2Hp)
            );
        }

        return (DefaultHiveKnightPhase2Hp, DefaultHiveKnightPhase3Hp);
    }

    private static void ApplyHiveKnightHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampHiveKnightHp(hiveKnightMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsHiveKnightObject(boss))
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

        int targetHp = ClampHiveKnightHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindHiveKnightHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsHiveKnight(candidate))
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

        hp = Math.Max(hp, DefaultHiveKnightVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultHiveKnightVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultHiveKnightVanillaHp);
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

    private static int ClampHiveKnightHp(int value)
    {
        if (value < MinHiveKnightHp)
        {
            return MinHiveKnightHp;
        }

        return value > MaxHiveKnightHp ? MaxHiveKnightHp : value;
    }

    private static int ClampHiveKnightPhase2Hp(int value)
    {
        if (value < MinHiveKnightPhase2Hp)
        {
            return MinHiveKnightPhase2Hp;
        }

        return value > MaxHiveKnightPhase2Hp ? MaxHiveKnightPhase2Hp : value;
    }

    private static int ClampHiveKnightPhase3Hp(int value, int phase2Hp)
    {
        int clampedPhase2Hp = ClampHiveKnightPhase2Hp(phase2Hp);
        int maxPhase3Hp = Math.Max(MinHiveKnightPhase3Hp, clampedPhase2Hp - 1);

        if (value < MinHiveKnightPhase3Hp)
        {
            return MinHiveKnightPhase3Hp;
        }

        return value > maxPhase3Hp ? maxPhase3Hp : value;
    }
}
