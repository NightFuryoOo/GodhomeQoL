using HutongGames.PlayMaker;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class NightmareKingGrimmHelper : Module
{
    private const string NightmareKingGrimmScene = "GG_Grimm_Nightmare";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string NightmareKingGrimmName = "Nightmare Grimm Boss";
    private const string RagePhase1VariableName = "Rage HP 1";
    private const string RagePhase2VariableName = "Rage HP 2";
    private const string RagePhase3VariableName = "Rage HP 3";
    private const int DefaultNightmareKingGrimmMaxHp = 1650;
    private const int DefaultNightmareKingGrimmVanillaHp = 1650;
    private const int DefaultNightmareKingGrimmRagePhase1Hp = 1238;
    private const int DefaultNightmareKingGrimmRagePhase2Hp = 826;
    private const int DefaultNightmareKingGrimmRagePhase3Hp = 414;
    private const int P5NightmareKingGrimmHp = 1250;
    private const int MinNightmareKingGrimmHp = 1;
    private const int MaxNightmareKingGrimmHp = 999999;
    private const int MinNightmareKingGrimmRagePhase1Hp = 3;
    private const int MaxNightmareKingGrimmRagePhase1Hp = DefaultNightmareKingGrimmVanillaHp;
    private const int MinNightmareKingGrimmRagePhase2Hp = 2;
    private const int MinNightmareKingGrimmRagePhase3Hp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool nightmareKingGrimmUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool nightmareKingGrimmP5Hp = false;

    [LocalSetting]
    internal static int nightmareKingGrimmMaxHp = DefaultNightmareKingGrimmMaxHp;

    [LocalSetting]
    internal static bool nightmareKingGrimmUseCustomPhase = false;

    [LocalSetting]
    internal static int nightmareKingGrimmRagePhase1Hp = DefaultNightmareKingGrimmRagePhase1Hp;

    [LocalSetting]
    internal static int nightmareKingGrimmRagePhase2Hp = DefaultNightmareKingGrimmRagePhase2Hp;

    [LocalSetting]
    internal static int nightmareKingGrimmRagePhase3Hp = DefaultNightmareKingGrimmRagePhase3Hp;

    [LocalSetting]
    internal static int nightmareKingGrimmMaxHpBeforeP5 = DefaultNightmareKingGrimmMaxHp;

    [LocalSetting]
    internal static bool nightmareKingGrimmUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int nightmareKingGrimmRagePhase1HpBeforeP5 = DefaultNightmareKingGrimmRagePhase1Hp;

    [LocalSetting]
    internal static int nightmareKingGrimmRagePhase2HpBeforeP5 = DefaultNightmareKingGrimmRagePhase2Hp;

    [LocalSetting]
    internal static int nightmareKingGrimmRagePhase3HpBeforeP5 = DefaultNightmareKingGrimmRagePhase3Hp;

    [LocalSetting]
    internal static bool nightmareKingGrimmUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool nightmareKingGrimmHasStoredStateBeforeP5 = false;

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
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_NightmareKingGrimm;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_NightmareKingGrimm;
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
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_NightmareKingGrimm;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_NightmareKingGrimm;
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
            ApplyNightmareKingGrimmHealthIfPresent();
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
            if (!nightmareKingGrimmP5Hp)
            {
                nightmareKingGrimmMaxHpBeforeP5 = ClampNightmareKingGrimmHp(nightmareKingGrimmMaxHp);
                nightmareKingGrimmUseMaxHpBeforeP5 = nightmareKingGrimmUseMaxHp;
                nightmareKingGrimmUseCustomPhaseBeforeP5 = nightmareKingGrimmUseCustomPhase;
                nightmareKingGrimmRagePhase1HpBeforeP5 = ClampNightmareKingGrimmRagePhase1Hp(nightmareKingGrimmRagePhase1Hp);
                nightmareKingGrimmRagePhase2HpBeforeP5 = ClampNightmareKingGrimmRagePhase2Hp(nightmareKingGrimmRagePhase2Hp, nightmareKingGrimmRagePhase1Hp);
                nightmareKingGrimmRagePhase3HpBeforeP5 = ClampNightmareKingGrimmRagePhase3Hp(nightmareKingGrimmRagePhase3Hp, nightmareKingGrimmRagePhase2Hp);
                nightmareKingGrimmHasStoredStateBeforeP5 = true;
            }

            nightmareKingGrimmP5Hp = true;
            nightmareKingGrimmUseMaxHp = true;
            nightmareKingGrimmUseCustomPhase = false;
            nightmareKingGrimmMaxHp = P5NightmareKingGrimmHp;
        }
        else
        {
            if (nightmareKingGrimmP5Hp && nightmareKingGrimmHasStoredStateBeforeP5)
            {
                nightmareKingGrimmMaxHp = ClampNightmareKingGrimmHp(nightmareKingGrimmMaxHpBeforeP5);
                nightmareKingGrimmUseMaxHp = nightmareKingGrimmUseMaxHpBeforeP5;
                nightmareKingGrimmUseCustomPhase = nightmareKingGrimmUseCustomPhaseBeforeP5;
                nightmareKingGrimmRagePhase1Hp = ClampNightmareKingGrimmRagePhase1Hp(nightmareKingGrimmRagePhase1HpBeforeP5);
                nightmareKingGrimmRagePhase2Hp = ClampNightmareKingGrimmRagePhase2Hp(nightmareKingGrimmRagePhase2HpBeforeP5, nightmareKingGrimmRagePhase1Hp);
                nightmareKingGrimmRagePhase3Hp = ClampNightmareKingGrimmRagePhase3Hp(nightmareKingGrimmRagePhase3HpBeforeP5, nightmareKingGrimmRagePhase2Hp);
            }

            nightmareKingGrimmP5Hp = false;
            nightmareKingGrimmHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!nightmareKingGrimmP5Hp)
        {
            return;
        }

        if (!nightmareKingGrimmHasStoredStateBeforeP5)
        {
            nightmareKingGrimmMaxHpBeforeP5 = ClampNightmareKingGrimmHp(nightmareKingGrimmMaxHp);
            nightmareKingGrimmUseMaxHpBeforeP5 = nightmareKingGrimmUseMaxHp;
            nightmareKingGrimmUseCustomPhaseBeforeP5 = nightmareKingGrimmUseCustomPhase;
            nightmareKingGrimmRagePhase1HpBeforeP5 = ClampNightmareKingGrimmRagePhase1Hp(nightmareKingGrimmRagePhase1Hp);
            nightmareKingGrimmRagePhase2HpBeforeP5 = ClampNightmareKingGrimmRagePhase2Hp(nightmareKingGrimmRagePhase2Hp, nightmareKingGrimmRagePhase1Hp);
            nightmareKingGrimmRagePhase3HpBeforeP5 = ClampNightmareKingGrimmRagePhase3Hp(nightmareKingGrimmRagePhase3Hp, nightmareKingGrimmRagePhase2Hp);
            nightmareKingGrimmHasStoredStateBeforeP5 = true;
        }

        nightmareKingGrimmUseMaxHp = true;
        nightmareKingGrimmUseCustomPhase = false;
        nightmareKingGrimmMaxHp = P5NightmareKingGrimmHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        nightmareKingGrimmRagePhase1Hp = ClampNightmareKingGrimmRagePhase1Hp(nightmareKingGrimmRagePhase1Hp);
        nightmareKingGrimmRagePhase2Hp = ClampNightmareKingGrimmRagePhase2Hp(nightmareKingGrimmRagePhase2Hp, nightmareKingGrimmRagePhase1Hp);
        nightmareKingGrimmRagePhase3Hp = ClampNightmareKingGrimmRagePhase3Hp(nightmareKingGrimmRagePhase3Hp, nightmareKingGrimmRagePhase2Hp);
    }

    internal static void ApplyNightmareKingGrimmHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindNightmareKingGrimmHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyNightmareKingGrimmHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindNightmareKingGrimmHealthManager(out HealthManager? hm))
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
            if (fsm == null || fsm.gameObject == null || !IsNightmareKingGrimmPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsNightmareKingGrimmPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            (int ragePhase1Hp, int ragePhase2Hp, int ragePhase3Hp) = GetVanillaRageThresholds(fsm.gameObject);
            SetRageThresholdsOnFsm(fsm, ragePhase1Hp, ragePhase2Hp, ragePhase3Hp);
        }
    }

    private static void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsNightmareKingGrimm(self))
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
            ApplyNightmareKingGrimmHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsNightmareKingGrimm(self))
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
            ApplyNightmareKingGrimmHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnPlayMakerFsmOnEnable_NightmareKingGrimm(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsNightmareKingGrimmPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static void OnPlayMakerFsmStart_NightmareKingGrimm(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsNightmareKingGrimmPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsNightmareKingGrimm(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyNightmareKingGrimmHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsNightmareKingGrimm(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyNightmareKingGrimmHealth(hm.gameObject, hm);
            }

            ApplyPhaseThresholdSettingsIfPresent();
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, NightmareKingGrimmScene, StringComparison.Ordinal))
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
            ApplyNightmareKingGrimmHealthIfPresent();
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

    private static bool IsNightmareKingGrimm(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsNightmareKingGrimmObject(hm.gameObject);
    }

    private static bool IsNightmareKingGrimmObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, NightmareKingGrimmScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(NightmareKingGrimmName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsNightmareKingGrimmObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => nightmareKingGrimmUseMaxHp;

    private static bool ShouldUseCustomPhaseThresholds() => nightmareKingGrimmUseCustomPhase && !nightmareKingGrimmP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, NightmareKingGrimmScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, NightmareKingGrimmScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsNightmareKingGrimmPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsNightmareKingGrimmObject(fsm.gameObject))
        {
            return false;
        }

        return string.Equals(fsm.FsmName, "Control", StringComparison.Ordinal)
            || fsm.Fsm?.GetState("Balloon?") != null;
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsNightmareKingGrimmPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        (int ragePhase1Hp, int ragePhase2Hp, int ragePhase3Hp) thresholds = ShouldUseCustomPhaseThresholds()
            ? (
                ClampNightmareKingGrimmRagePhase1Hp(nightmareKingGrimmRagePhase1Hp),
                ClampNightmareKingGrimmRagePhase2Hp(nightmareKingGrimmRagePhase2Hp, nightmareKingGrimmRagePhase1Hp),
                ClampNightmareKingGrimmRagePhase3Hp(nightmareKingGrimmRagePhase3Hp, nightmareKingGrimmRagePhase2Hp)
            )
            : GetVanillaRageThresholds(fsm.gameObject);

        SetRageThresholdsOnFsm(fsm, thresholds.ragePhase1Hp, thresholds.ragePhase2Hp, thresholds.ragePhase3Hp);
    }

    private static void SetRageThresholdsOnFsm(PlayMakerFSM fsm, int ragePhase1Hp, int ragePhase2Hp, int ragePhase3Hp)
    {
        int targetRagePhase1Hp = ClampNightmareKingGrimmRagePhase1Hp(ragePhase1Hp);
        int targetRagePhase2Hp = ClampNightmareKingGrimmRagePhase2Hp(ragePhase2Hp, targetRagePhase1Hp);
        int targetRagePhase3Hp = ClampNightmareKingGrimmRagePhase3Hp(ragePhase3Hp, targetRagePhase2Hp);

        FsmInt? ragePhase1Variable = fsm.FsmVariables.GetFsmInt(RagePhase1VariableName);
        if (ragePhase1Variable != null)
        {
            ragePhase1Variable.Value = targetRagePhase1Hp;
        }

        FsmInt? ragePhase2Variable = fsm.FsmVariables.GetFsmInt(RagePhase2VariableName);
        if (ragePhase2Variable != null)
        {
            ragePhase2Variable.Value = targetRagePhase2Hp;
        }

        FsmInt? ragePhase3Variable = fsm.FsmVariables.GetFsmInt(RagePhase3VariableName);
        if (ragePhase3Variable != null)
        {
            ragePhase3Variable.Value = targetRagePhase3Hp;
        }
    }

    private static (int ragePhase1Hp, int ragePhase2Hp, int ragePhase3Hp) GetVanillaRageThresholds(GameObject bossObject)
    {
        int referenceHp = GetPhaseReferenceHp(bossObject);
        int quarterHp = referenceHp / 4;

        int ragePhase1Hp = referenceHp - quarterHp;
        int ragePhase2Hp = ragePhase1Hp - quarterHp;
        int ragePhase3Hp = ragePhase2Hp - quarterHp;

        int clampedRagePhase1Hp = ClampNightmareKingGrimmRagePhase1Hp(ragePhase1Hp);
        int clampedRagePhase2Hp = ClampNightmareKingGrimmRagePhase2Hp(ragePhase2Hp, clampedRagePhase1Hp);
        int clampedRagePhase3Hp = ClampNightmareKingGrimmRagePhase3Hp(ragePhase3Hp, clampedRagePhase2Hp);
        return (clampedRagePhase1Hp, clampedRagePhase2Hp, clampedRagePhase3Hp);
    }

    private static int GetPhaseReferenceHp(GameObject bossObject)
    {
        if (bossObject == null)
        {
            return DefaultNightmareKingGrimmVanillaHp;
        }

        HealthManager? hm = bossObject.GetComponent<HealthManager>();
        if (hm != null)
        {
            int maxHp = ReadMaxHp(hm);
            if (maxHp > 0)
            {
                return ClampNightmareKingGrimmHp(maxHp);
            }

            if (hm.hp > 0)
            {
                return ClampNightmareKingGrimmHp(hm.hp);
            }
        }

        int configuredHp = ClampNightmareKingGrimmHp(nightmareKingGrimmMaxHp);
        return configuredHp > 0 ? configuredHp : DefaultNightmareKingGrimmVanillaHp;
    }

    private static void ApplyNightmareKingGrimmHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampNightmareKingGrimmHp(nightmareKingGrimmMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsNightmareKingGrimmObject(boss))
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

        int targetHp = ClampNightmareKingGrimmHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindNightmareKingGrimmHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsNightmareKingGrimm(candidate))
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

        hp = Math.Max(hp, DefaultNightmareKingGrimmVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultNightmareKingGrimmVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultNightmareKingGrimmVanillaHp);
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

    private static int ClampNightmareKingGrimmHp(int value)
    {
        if (value < MinNightmareKingGrimmHp)
        {
            return MinNightmareKingGrimmHp;
        }

        return value > MaxNightmareKingGrimmHp ? MaxNightmareKingGrimmHp : value;
    }

    private static int ClampNightmareKingGrimmRagePhase1Hp(int value)
    {
        if (value < MinNightmareKingGrimmRagePhase1Hp)
        {
            return MinNightmareKingGrimmRagePhase1Hp;
        }

        return value > MaxNightmareKingGrimmRagePhase1Hp ? MaxNightmareKingGrimmRagePhase1Hp : value;
    }

    private static int ClampNightmareKingGrimmRagePhase2Hp(int value, int ragePhase1Hp)
    {
        int clampedRagePhase1Hp = ClampNightmareKingGrimmRagePhase1Hp(ragePhase1Hp);
        int maxRagePhase2Hp = Math.Max(MinNightmareKingGrimmRagePhase2Hp, clampedRagePhase1Hp - 1);

        if (value < MinNightmareKingGrimmRagePhase2Hp)
        {
            return MinNightmareKingGrimmRagePhase2Hp;
        }

        return value > maxRagePhase2Hp ? maxRagePhase2Hp : value;
    }

    private static int ClampNightmareKingGrimmRagePhase3Hp(int value, int ragePhase2Hp)
    {
        int clampedRagePhase2Hp = ragePhase2Hp < MinNightmareKingGrimmRagePhase2Hp
            ? MinNightmareKingGrimmRagePhase2Hp
            : ragePhase2Hp;
        int maxRagePhase3Hp = Math.Max(MinNightmareKingGrimmRagePhase3Hp, clampedRagePhase2Hp - 1);

        if (value < MinNightmareKingGrimmRagePhase3Hp)
        {
            return MinNightmareKingGrimmRagePhase3Hp;
        }

        return value > maxRagePhase3Hp ? maxRagePhase3Hp : value;
    }
}
