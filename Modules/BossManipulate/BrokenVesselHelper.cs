using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class BrokenVesselHelper : Module
{
    private const string BrokenVesselScene = "GG_Broken_Vessel";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string BrokenVesselName = "Infected Knight";
    private static readonly string[] BrokenVesselSummonNameHints = { "Bursting", "Bouncer", "Balloon", "Zombie" };
    private const string BrokenVesselSpawnBalloonFsmName = "Spawn Balloon";
    private const string BrokenVesselShakeTokenControlFsmName = "Shake Token Control";
    private const string BrokenVesselSpawnStateName = "Spawn";
    private const string BrokenVesselCheckState1Name = "Check";
    private const string BrokenVesselCheckState2Name = "Check 2";
    private const string BrokenVesselCheckState3Name = "Check 3";
    private const string BrokenVesselHpVariableName = "HP";
    private const string BrokenVesselToken1VariableName = "Token 1";
    private const string BrokenVesselToken2VariableName = "Token 2";
    private const string BrokenVesselToken3VariableName = "Token 3";
    private const int DefaultBrokenVesselMaxHp = 1000;
    private const int DefaultBrokenVesselVanillaHp = 1000;
    private const int DefaultBrokenVesselSummonHp = 1;
    private const int DefaultBrokenVesselSummonVanillaHp = 13;
    private const int DefaultBrokenVesselSummonLimit = 3;
    private const int DefaultBrokenVesselVanillaSummonLimit = 3;
    private const int DefaultBrokenVesselPhase2Hp = 420;
    private const int DefaultBrokenVesselPhase3Hp = 370;
    private const int DefaultBrokenVesselPhase4Hp = 220;
    private const int DefaultBrokenVesselPhase5Hp = 110;
    private const int P5BrokenVesselHp = 700;
    private const int MinBrokenVesselHp = 1;
    private const int MaxBrokenVesselHp = 999999;
    private const int MinBrokenVesselPhase2Hp = 4;
    private const int MinBrokenVesselPhase3Hp = 3;
    private const int MinBrokenVesselPhase4Hp = 2;
    private const int MinBrokenVesselPhase5Hp = 1;
    private const int MinBrokenVesselSummonLimit = 0;
    private const int MaxBrokenVesselSummonLimit = 999;

    [LocalSetting]
    [BoolOption]
    internal static bool brokenVesselUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool brokenVesselP5Hp = false;

    [LocalSetting]
    internal static int brokenVesselMaxHp = DefaultBrokenVesselMaxHp;

    [LocalSetting]
    internal static int brokenVesselMaxHpBeforeP5 = DefaultBrokenVesselMaxHp;

    [LocalSetting]
    internal static bool brokenVesselUseCustomSummonHp = false;

    [LocalSetting]
    internal static int brokenVesselSummonHp = DefaultBrokenVesselSummonHp;

    [LocalSetting]
    internal static bool brokenVesselUseCustomSummonLimit = false;

    [LocalSetting]
    internal static int brokenVesselSummonLimit = DefaultBrokenVesselSummonLimit;

    [LocalSetting]
    internal static bool brokenVesselUseCustomPhase = false;

    [LocalSetting]
    internal static int brokenVesselPhase2Hp = DefaultBrokenVesselPhase2Hp;

    [LocalSetting]
    internal static int brokenVesselPhase3Hp = DefaultBrokenVesselPhase3Hp;

    [LocalSetting]
    internal static int brokenVesselPhase4Hp = DefaultBrokenVesselPhase4Hp;

    [LocalSetting]
    internal static int brokenVesselPhase5Hp = DefaultBrokenVesselPhase5Hp;

    [LocalSetting]
    internal static bool brokenVesselUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool brokenVesselHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool brokenVesselUseCustomSummonHpBeforeP5 = false;

    [LocalSetting]
    internal static int brokenVesselSummonHpBeforeP5 = DefaultBrokenVesselSummonHp;

    [LocalSetting]
    internal static bool brokenVesselUseCustomSummonLimitBeforeP5 = false;

    [LocalSetting]
    internal static int brokenVesselSummonLimitBeforeP5 = DefaultBrokenVesselSummonLimit;

    [LocalSetting]
    internal static bool brokenVesselUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int brokenVesselPhase2HpBeforeP5 = DefaultBrokenVesselPhase2Hp;

    [LocalSetting]
    internal static int brokenVesselPhase3HpBeforeP5 = DefaultBrokenVesselPhase3Hp;

    [LocalSetting]
    internal static int brokenVesselPhase4HpBeforeP5 = DefaultBrokenVesselPhase4Hp;

    [LocalSetting]
    internal static int brokenVesselPhase5HpBeforeP5 = DefaultBrokenVesselPhase5Hp;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static readonly Dictionary<int, int> vanillaSummonHpByInstance = new();
    private static readonly Dictionary<int, int> vanillaSummonLimitByFsm = new();
    private static readonly Dictionary<int, (int phase3Hp, int phase4Hp, int phase5Hp)> vanillaShakeThresholdsByFsm = new();
    private static readonly Dictionary<int, int> vanillaSpawnThresholdByFsm = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        NormalizeP5State();
        NormalizePhaseThresholdState();
        vanillaHpByInstance.Clear();
        vanillaSummonHpByInstance.Clear();
        vanillaSummonLimitByFsm.Clear();
        vanillaShakeThresholdsByFsm.Clear();
        vanillaSpawnThresholdByFsm.Clear();
        On.SetHP.OnEnter += OnSetHpEnter;
        On.HealthManager.OnEnable += OnHealthManagerOnEnable;
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_BrokenVessel;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_BrokenVessel;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresent();
        RestoreVanillaSummonHealthIfPresent();
        RestoreVanillaSummonLimitsIfPresent();
        RestoreVanillaPhaseThresholdsIfPresent();
        moduleActive = false;
        On.SetHP.OnEnter -= OnSetHpEnter;
        On.HealthManager.OnEnable -= OnHealthManagerOnEnable;
        On.HealthManager.Awake -= OnHealthManagerAwake;
        On.HealthManager.Start -= OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_BrokenVessel;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_BrokenVessel;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
        vanillaSummonHpByInstance.Clear();
        vanillaSummonLimitByFsm.Clear();
        vanillaShakeThresholdsByFsm.Clear();
        vanillaSpawnThresholdByFsm.Clear();
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
            ApplyBrokenVesselHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyBrokenVesselSummonHealthIfPresent();
        }
        else
        {
            RestoreVanillaSummonHealthIfPresent();
        }

        if (ShouldUseCustomSummonLimit())
        {
            ApplySummonLimitSettingsIfPresent();
            EnforceCustomSummonLimitIfPresent();
        }
        else
        {
            RestoreVanillaSummonLimitsIfPresent();
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    internal static void SetP5HpEnabled(bool value)
    {
        if (value)
        {
            if (!brokenVesselP5Hp)
            {
                brokenVesselMaxHpBeforeP5 = ClampBrokenVesselHp(brokenVesselMaxHp);
                brokenVesselUseMaxHpBeforeP5 = brokenVesselUseMaxHp;
                brokenVesselUseCustomSummonHpBeforeP5 = brokenVesselUseCustomSummonHp;
                brokenVesselSummonHpBeforeP5 = ClampBrokenVesselHp(brokenVesselSummonHp);
                brokenVesselUseCustomSummonLimitBeforeP5 = brokenVesselUseCustomSummonLimit;
                brokenVesselSummonLimitBeforeP5 = ClampBrokenVesselSummonLimit(brokenVesselSummonLimit);
                brokenVesselUseCustomPhaseBeforeP5 = brokenVesselUseCustomPhase;
                brokenVesselPhase2HpBeforeP5 = ClampBrokenVesselPhase2Hp(brokenVesselPhase2Hp, ResolvePhase2MaxHp());
                brokenVesselPhase3HpBeforeP5 = ClampBrokenVesselPhase3Hp(brokenVesselPhase3Hp, brokenVesselPhase2HpBeforeP5);
                brokenVesselPhase4HpBeforeP5 = ClampBrokenVesselPhase4Hp(brokenVesselPhase4Hp, brokenVesselPhase3HpBeforeP5);
                brokenVesselPhase5HpBeforeP5 = ClampBrokenVesselPhase5Hp(brokenVesselPhase5Hp, brokenVesselPhase4HpBeforeP5);
                brokenVesselHasStoredStateBeforeP5 = true;
            }

            brokenVesselP5Hp = true;
            brokenVesselUseMaxHp = true;
            brokenVesselUseCustomSummonHp = false;
            brokenVesselUseCustomSummonLimit = false;
            brokenVesselUseCustomPhase = false;
            brokenVesselMaxHp = P5BrokenVesselHp;
        }
        else
        {
            if (brokenVesselP5Hp && brokenVesselHasStoredStateBeforeP5)
            {
                brokenVesselMaxHp = ClampBrokenVesselHp(brokenVesselMaxHpBeforeP5);
                brokenVesselUseMaxHp = brokenVesselUseMaxHpBeforeP5;
                brokenVesselUseCustomSummonHp = brokenVesselUseCustomSummonHpBeforeP5;
                brokenVesselSummonHp = ClampBrokenVesselHp(brokenVesselSummonHpBeforeP5);
                brokenVesselUseCustomSummonLimit = brokenVesselUseCustomSummonLimitBeforeP5;
                brokenVesselSummonLimit = ClampBrokenVesselSummonLimit(brokenVesselSummonLimitBeforeP5);
                brokenVesselUseCustomPhase = brokenVesselUseCustomPhaseBeforeP5;
                brokenVesselPhase2Hp = ClampBrokenVesselPhase2Hp(brokenVesselPhase2HpBeforeP5, ResolvePhase2MaxHp());
                brokenVesselPhase3Hp = ClampBrokenVesselPhase3Hp(brokenVesselPhase3HpBeforeP5, brokenVesselPhase2Hp);
                brokenVesselPhase4Hp = ClampBrokenVesselPhase4Hp(brokenVesselPhase4HpBeforeP5, brokenVesselPhase3Hp);
                brokenVesselPhase5Hp = ClampBrokenVesselPhase5Hp(brokenVesselPhase5HpBeforeP5, brokenVesselPhase4Hp);
            }

            brokenVesselP5Hp = false;
            brokenVesselHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!brokenVesselP5Hp)
        {
            return;
        }

        if (!brokenVesselHasStoredStateBeforeP5)
        {
            brokenVesselMaxHpBeforeP5 = ClampBrokenVesselHp(brokenVesselMaxHp);
            brokenVesselUseMaxHpBeforeP5 = brokenVesselUseMaxHp;
            brokenVesselUseCustomSummonHpBeforeP5 = brokenVesselUseCustomSummonHp;
            brokenVesselSummonHpBeforeP5 = ClampBrokenVesselHp(brokenVesselSummonHp);
            brokenVesselUseCustomSummonLimitBeforeP5 = brokenVesselUseCustomSummonLimit;
            brokenVesselSummonLimitBeforeP5 = ClampBrokenVesselSummonLimit(brokenVesselSummonLimit);
            brokenVesselUseCustomPhaseBeforeP5 = brokenVesselUseCustomPhase;
            brokenVesselPhase2HpBeforeP5 = ClampBrokenVesselPhase2Hp(brokenVesselPhase2Hp, ResolvePhase2MaxHp());
            brokenVesselPhase3HpBeforeP5 = ClampBrokenVesselPhase3Hp(brokenVesselPhase3Hp, brokenVesselPhase2HpBeforeP5);
            brokenVesselPhase4HpBeforeP5 = ClampBrokenVesselPhase4Hp(brokenVesselPhase4Hp, brokenVesselPhase3HpBeforeP5);
            brokenVesselPhase5HpBeforeP5 = ClampBrokenVesselPhase5Hp(brokenVesselPhase5Hp, brokenVesselPhase4HpBeforeP5);
            brokenVesselHasStoredStateBeforeP5 = true;
        }

        brokenVesselUseMaxHp = true;
        brokenVesselUseCustomSummonHp = false;
        brokenVesselUseCustomSummonLimit = false;
        brokenVesselUseCustomPhase = false;
        brokenVesselMaxHp = P5BrokenVesselHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        brokenVesselPhase2Hp = ClampBrokenVesselPhase2Hp(brokenVesselPhase2Hp, ResolvePhase2MaxHp());
        brokenVesselPhase3Hp = ClampBrokenVesselPhase3Hp(brokenVesselPhase3Hp, brokenVesselPhase2Hp);
        brokenVesselPhase4Hp = ClampBrokenVesselPhase4Hp(brokenVesselPhase4Hp, brokenVesselPhase3Hp);
        brokenVesselPhase5Hp = ClampBrokenVesselPhase5Hp(brokenVesselPhase5Hp, brokenVesselPhase4Hp);
    }

    internal static void ApplyBrokenVesselHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindBrokenVesselHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyBrokenVesselHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindBrokenVesselHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            RestoreVanillaHealth(hm.gameObject, hm);
        }
    }

    internal static void ApplyBrokenVesselSummonHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomSummonHp())
        {
            return;
        }

        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsBrokenVesselSummon(hm))
            {
                continue;
            }

            ApplyBrokenVesselSummonHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaSummonHealthIfPresent()
    {
        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsBrokenVesselSummon(hm))
            {
                continue;
            }

            RestoreVanillaSummonHealth(hm.gameObject, hm);
        }
    }

    internal static void ApplySummonLimitSettingsIfPresent()
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

            ApplySummonLimitSettings(fsm);
        }
    }

    internal static void RestoreVanillaSummonLimitsIfPresent()
    {
        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsBrokenVesselSpawnBalloonFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            RememberVanillaSummonLimit(fsm);
            SetSummonLimitOnSpawnBalloonFsm(fsm, GetVanillaSummonLimit(fsm));
        }
    }

    internal static void EnforceCustomSummonLimitIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomSummonLimit())
        {
            return;
        }

        int maxAllowed = ClampBrokenVesselSummonLimit(brokenVesselSummonLimit);
        int currentAlive = 0;
        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsBrokenVesselSummon(hm))
            {
                continue;
            }

            if (!ShouldApplySummonSettings(hm.gameObject) || !hm.gameObject.activeInHierarchy)
            {
                continue;
            }

            currentAlive++;
            if (currentAlive > maxAllowed)
            {
                DespawnBrokenVesselSummon(hm.gameObject);
            }
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
            if (fsm == null || fsm.gameObject == null || !IsBrokenVesselPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsBrokenVesselPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            if (IsBrokenVesselShakeTokenFsm(fsm))
            {
                RememberVanillaShakeThresholds(fsm);
                (int phase3Hp, int phase4Hp, int phase5Hp) thresholds = GetVanillaShakeThresholds(fsm);
                SetShakeThresholdsOnFsm(fsm, thresholds.phase3Hp, thresholds.phase4Hp, thresholds.phase5Hp, useExactCustomThresholds: false);
                continue;
            }

            if (IsBrokenVesselSpawnBalloonFsm(fsm))
            {
                RememberVanillaSpawnThreshold(fsm);
                int vanillaPhase2Hp = GetVanillaSpawnThreshold(fsm);
                SetSpawnThresholdOnFsm(fsm, vanillaPhase2Hp, useExactCustomThresholds: false);
            }
        }
    }

    private static void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive)
        {
            return;
        }

        if (IsBrokenVessel(self))
        {
            RememberVanillaHp(self);
            if (!ShouldApplySettings(self.gameObject))
            {
                return;
            }

            if (ShouldUseCustomHp())
            {
                ApplyBrokenVesselHealth(self.gameObject, self);
            }
            else
            {
                RestoreVanillaHealth(self.gameObject, self);
            }

            return;
        }

        if (!IsBrokenVesselSummon(self))
        {
            return;
        }

        RememberVanillaSummonHp(self);
        if (!ShouldApplySummonSettings(self.gameObject))
        {
            return;
        }

        TryEnforceCustomSummonLimit(self);
        if (self == null || self.gameObject == null || !self.gameObject.activeInHierarchy)
        {
            return;
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyBrokenVesselSummonHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaSummonHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive)
        {
            return;
        }

        if (IsBrokenVessel(self))
        {
            RememberVanillaHp(self);
            if (!ShouldApplySettings(self.gameObject))
            {
                return;
            }

            if (ShouldUseCustomHp())
            {
                ApplyBrokenVesselHealth(self.gameObject, self);
                _ = self.StartCoroutine(DeferredApply(self));
            }
            else
            {
                RestoreVanillaHealth(self.gameObject, self);
            }

            ApplyPhaseThresholdSettingsIfPresent();
            return;
        }

        if (!IsBrokenVesselSummon(self))
        {
            return;
        }

        RememberVanillaSummonHp(self);
        if (!ShouldApplySummonSettings(self.gameObject))
        {
            return;
        }

        TryEnforceCustomSummonLimit(self);
        if (self == null || self.gameObject == null || !self.gameObject.activeInHierarchy)
        {
            return;
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyBrokenVesselSummonHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaSummonHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerOnEnable(On.HealthManager.orig_OnEnable orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsBrokenVesselSummon(self))
        {
            return;
        }

        RememberVanillaSummonHp(self);
        if (!ShouldApplySummonSettings(self.gameObject))
        {
            return;
        }

        TryEnforceCustomSummonLimit(self);
        if (self == null || self.gameObject == null || !self.gameObject.activeInHierarchy)
        {
            return;
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyBrokenVesselSummonHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaSummonHealth(self.gameObject, self);
        }
    }

    private static void OnSetHpEnter(On.SetHP.orig_OnEnter orig, SetHP self)
    {
        orig(self);

        if (!moduleActive || self == null || !ShouldUseCustomSummonHp())
        {
            return;
        }

        GameObject targetObject = self.target.GetSafe(self);
        if (targetObject == null || !IsBrokenVesselSummonObject(targetObject) || !ShouldApplySummonSettings(targetObject))
        {
            return;
        }

        HealthManager hm = targetObject.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        RememberVanillaSummonHp(hm);
        ApplyBrokenVesselSummonHealth(targetObject, hm);
    }

    private static void OnPlayMakerFsmOnEnable_BrokenVessel(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsBrokenVesselPhaseControlFsm(self))
        {
            return;
        }

        ApplySummonLimitSettings(self);
        ApplyPhaseThresholdSettings(self);
    }

    private static void OnPlayMakerFsmStart_BrokenVessel(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsBrokenVesselPhaseControlFsm(self))
        {
            return;
        }

        ApplySummonLimitSettings(self);
        ApplyPhaseThresholdSettings(self);
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null)
        {
            yield break;
        }

        if (IsBrokenVessel(hm))
        {
            if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyBrokenVesselHealth(hm.gameObject, hm);
                yield return new WaitForSeconds(0.01f);
                if (moduleActive && hm != null && hm.gameObject != null && IsBrokenVessel(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
                {
                    ApplyBrokenVesselHealth(hm.gameObject, hm);
                }
            }

            ApplyPhaseThresholdSettingsIfPresent();
            yield break;
        }

        if (!IsBrokenVesselSummon(hm))
        {
            yield break;
        }

        if (ShouldApplySummonSettings(hm.gameObject))
        {
            if (ShouldUseCustomSummonHp())
            {
                ApplyBrokenVesselSummonHealth(hm.gameObject, hm);
                yield return new WaitForSeconds(0.01f);
                if (moduleActive && hm != null && hm.gameObject != null && IsBrokenVesselSummon(hm) && ShouldUseCustomSummonHp() && ShouldApplySummonSettings(hm.gameObject))
                {
                    ApplyBrokenVesselSummonHealth(hm.gameObject, hm);
                }
            }
            else
            {
                RestoreVanillaSummonHealth(hm.gameObject, hm);
            }
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, BrokenVesselScene, StringComparison.Ordinal))
        {
            vanillaHpByInstance.Clear();
            vanillaSummonHpByInstance.Clear();
            vanillaSummonLimitByFsm.Clear();
            vanillaShakeThresholdsByFsm.Clear();
            vanillaSpawnThresholdByFsm.Clear();
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyBrokenVesselHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyBrokenVesselSummonHealthIfPresent();
        }
        else
        {
            RestoreVanillaSummonHealthIfPresent();
        }

        if (ShouldUseCustomSummonLimit())
        {
            ApplySummonLimitSettingsIfPresent();
            EnforceCustomSummonLimitIfPresent();
        }
        else
        {
            RestoreVanillaSummonLimitsIfPresent();
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    private static string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);
        return newSceneName;
    }

    private static bool IsBrokenVessel(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsBrokenVesselObject(hm.gameObject);
    }

    private static bool IsBrokenVesselObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, BrokenVesselScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(BrokenVesselName, StringComparison.Ordinal);
    }

    private static bool IsBrokenVesselSummon(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsBrokenVesselSummonObject(hm.gameObject);
    }

    private static bool IsBrokenVesselSummonObject(GameObject gameObject)
    {
        if (gameObject == null || IsBrokenVesselObject(gameObject))
        {
            return false;
        }

        bool isBrokenSceneObject = string.Equals(gameObject.scene.name, BrokenVesselScene, StringComparison.Ordinal);
        bool isBrokenSceneActive = string.Equals(USceneManager.GetActiveScene().name, BrokenVesselScene, StringComparison.Ordinal);
        if (!isBrokenSceneObject && !isBrokenSceneActive)
        {
            return false;
        }

        string name = gameObject.name ?? string.Empty;
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        foreach (string hint in BrokenVesselSummonNameHints)
        {
            if (name.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return gameObject.GetComponent<HealthManager>() != null;
            }
        }

        return false;
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsBrokenVesselObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldApplySummonSettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsBrokenVesselSummonObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => brokenVesselUseMaxHp;

    private static bool ShouldUseCustomSummonHp() => brokenVesselUseCustomSummonHp && !brokenVesselP5Hp;

    private static bool ShouldUseCustomSummonLimit() => brokenVesselUseCustomSummonLimit && !brokenVesselP5Hp;

    private static bool ShouldUseCustomPhaseThresholds() => brokenVesselUseCustomPhase && !brokenVesselP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, BrokenVesselScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, BrokenVesselScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsBrokenVesselPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsBrokenVesselObject(fsm.gameObject))
        {
            return false;
        }

        return IsBrokenVesselShakeTokenFsm(fsm) || IsBrokenVesselSpawnBalloonFsm(fsm);
    }

    private static bool IsBrokenVesselShakeTokenFsm(PlayMakerFSM fsm)
    {
        if (fsm == null)
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, BrokenVesselShakeTokenControlFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState(BrokenVesselCheckState1Name) != null
            && fsm.Fsm?.GetState(BrokenVesselCheckState2Name) != null
            && fsm.Fsm?.GetState(BrokenVesselCheckState3Name) != null
            && fsm.FsmVariables.GetFsmInt(BrokenVesselToken1VariableName) != null
            && fsm.FsmVariables.GetFsmInt(BrokenVesselToken2VariableName) != null
            && fsm.FsmVariables.GetFsmInt(BrokenVesselToken3VariableName) != null;
    }

    private static bool IsBrokenVesselSpawnBalloonFsm(PlayMakerFSM fsm)
    {
        if (fsm == null)
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, BrokenVesselSpawnBalloonFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState(BrokenVesselSpawnStateName) != null
            && FindSpawnHpCompareAction(fsm) != null;
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsBrokenVesselPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        bool useExactCustomThresholds = ShouldUseCustomPhaseThresholds();
        int customPhase2Hp = ClampBrokenVesselPhase2Hp(brokenVesselPhase2Hp, ResolvePhase2MaxHp());
        int customPhase3Hp = ClampBrokenVesselPhase3Hp(brokenVesselPhase3Hp, customPhase2Hp);
        int customPhase4Hp = ClampBrokenVesselPhase4Hp(brokenVesselPhase4Hp, customPhase3Hp);
        int customPhase5Hp = ClampBrokenVesselPhase5Hp(brokenVesselPhase5Hp, customPhase4Hp);

        if (IsBrokenVesselShakeTokenFsm(fsm))
        {
            RememberVanillaShakeThresholds(fsm);
            (int phase3Hp, int phase4Hp, int phase5Hp) thresholds = useExactCustomThresholds
                ? (customPhase3Hp, customPhase4Hp, customPhase5Hp)
                : GetVanillaShakeThresholds(fsm);

            SetShakeThresholdsOnFsm(fsm, thresholds.phase3Hp, thresholds.phase4Hp, thresholds.phase5Hp, useExactCustomThresholds);
            return;
        }

        if (IsBrokenVesselSpawnBalloonFsm(fsm))
        {
            RememberVanillaSpawnThreshold(fsm);
            int phase2Hp = useExactCustomThresholds ? customPhase2Hp : GetVanillaSpawnThreshold(fsm);
            SetSpawnThresholdOnFsm(fsm, phase2Hp, useExactCustomThresholds);
        }
    }

    private static void ApplySummonLimitSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsBrokenVesselSpawnBalloonFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        RememberVanillaSummonLimit(fsm);
        int targetLimit = ShouldUseCustomSummonLimit()
            ? ClampBrokenVesselSummonLimit(brokenVesselSummonLimit)
            : GetVanillaSummonLimit(fsm);
        SetSummonLimitOnSpawnBalloonFsm(fsm, targetLimit);
    }

    private static void SetSummonLimitOnSpawnBalloonFsm(PlayMakerFSM fsm, int value)
    {
        IntCompare? compare = FindSpawnEnemyCountCompareAction(fsm);
        if (compare?.integer2 == null)
        {
            return;
        }

        compare.integer2.UseVariable = false;
        compare.integer2.Name = string.Empty;
        compare.integer2.Value = ClampBrokenVesselSummonLimit(value);
    }

    private static void SetShakeThresholdsOnFsm(PlayMakerFSM fsm, int phase3Hp, int phase4Hp, int phase5Hp, bool useExactCustomThresholds)
    {
        int targetPhase3Hp = Math.Max(MinBrokenVesselPhase3Hp, phase3Hp);
        int targetPhase4Hp = Math.Max(MinBrokenVesselPhase4Hp, phase4Hp);
        int targetPhase5Hp = Math.Max(MinBrokenVesselPhase5Hp, phase5Hp);

        FsmInt? token1 = fsm.FsmVariables.GetFsmInt(BrokenVesselToken1VariableName);
        if (token1 != null)
        {
            token1.Value = targetPhase3Hp;
        }

        FsmInt? token2 = fsm.FsmVariables.GetFsmInt(BrokenVesselToken2VariableName);
        if (token2 != null)
        {
            token2.Value = targetPhase4Hp;
        }

        FsmInt? token3 = fsm.FsmVariables.GetFsmInt(BrokenVesselToken3VariableName);
        if (token3 != null)
        {
            token3.Value = targetPhase5Hp;
        }

        SetThresholdOnCheckState(
            fsm,
            BrokenVesselCheckState1Name,
            BrokenVesselToken1VariableName,
            targetPhase3Hp,
            useExactCustomThresholds);
        SetThresholdOnCheckState(
            fsm,
            BrokenVesselCheckState2Name,
            BrokenVesselToken2VariableName,
            targetPhase4Hp,
            useExactCustomThresholds);
        SetThresholdOnCheckState(
            fsm,
            BrokenVesselCheckState3Name,
            BrokenVesselToken3VariableName,
            targetPhase5Hp,
            useExactCustomThresholds);
    }

    private static void SetThresholdOnCheckState(
        PlayMakerFSM fsm,
        string stateName,
        string variableName,
        int targetValue,
        bool useExactCustomThresholds)
    {
        FsmState? checkState = fsm.Fsm?.GetState(stateName);
        if (checkState?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in checkState.Actions)
        {
            if (action is not IntCompare compare || compare.integer2 == null || !IsHpCompare(compare))
            {
                continue;
            }

            if (useExactCustomThresholds)
            {
                compare.integer2.UseVariable = false;
                compare.integer2.Name = string.Empty;
                compare.integer2.Value = targetValue;
            }
            else
            {
                compare.integer2.UseVariable = true;
                compare.integer2.Name = variableName;
                compare.integer2.Value = targetValue;
            }

            break;
        }
    }

    private static void SetSpawnThresholdOnFsm(PlayMakerFSM fsm, int phase2Hp, bool useExactCustomThresholds)
    {
        IntCompare? hpCompare = FindSpawnHpCompareAction(fsm);
        if (hpCompare?.integer2 == null)
        {
            return;
        }

        int targetPhase2Hp = Math.Max(MinBrokenVesselPhase2Hp, phase2Hp);
        hpCompare.integer2.UseVariable = false;
        hpCompare.integer2.Name = string.Empty;
        hpCompare.integer2.Value = useExactCustomThresholds
            ? targetPhase2Hp
            : targetPhase2Hp;
    }

    private static IntCompare? FindSpawnHpCompareAction(PlayMakerFSM fsm)
    {
        FsmState? spawnState = fsm.Fsm?.GetState(BrokenVesselSpawnStateName);
        if (spawnState?.Actions == null)
        {
            return null;
        }

        foreach (FsmStateAction action in spawnState.Actions)
        {
            if (action is IntCompare compare && IsHpCompare(compare))
            {
                return compare;
            }
        }

        return null;
    }

    private static IntCompare? FindSpawnEnemyCountCompareAction(PlayMakerFSM fsm)
    {
        FsmState? spawnState = fsm.Fsm?.GetState(BrokenVesselSpawnStateName);
        if (spawnState?.Actions == null)
        {
            return null;
        }

        foreach (FsmStateAction action in spawnState.Actions)
        {
            if (action is IntCompare compare && IsEnemyCountCompare(compare))
            {
                return compare;
            }
        }

        return null;
    }

    private static bool IsHpCompare(IntCompare compare)
    {
        return (compare.integer1 != null
            && string.Equals(compare.integer1.Name, BrokenVesselHpVariableName, StringComparison.Ordinal))
            || (compare.integer2 != null
                && string.Equals(compare.integer2.Name, BrokenVesselHpVariableName, StringComparison.Ordinal));
    }

    private static bool IsEnemyCountCompare(IntCompare compare)
    {
        return (compare.integer1 != null
            && string.Equals(compare.integer1.Name, "Enemy Count", StringComparison.Ordinal))
            || (compare.integer2 != null
                && string.Equals(compare.integer2.Name, "Enemy Count", StringComparison.Ordinal));
    }

    private static void RememberVanillaShakeThresholds(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaShakeThresholdsByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int phase3Hp = DefaultBrokenVesselPhase3Hp;
        int phase4Hp = DefaultBrokenVesselPhase4Hp;
        int phase5Hp = DefaultBrokenVesselPhase5Hp;

        FsmInt? token1 = fsm.FsmVariables.GetFsmInt(BrokenVesselToken1VariableName);
        if (token1 != null && token1.Value > 0)
        {
            phase3Hp = token1.Value;
        }

        FsmInt? token2 = fsm.FsmVariables.GetFsmInt(BrokenVesselToken2VariableName);
        if (token2 != null && token2.Value > 0)
        {
            phase4Hp = token2.Value;
        }

        FsmInt? token3 = fsm.FsmVariables.GetFsmInt(BrokenVesselToken3VariableName);
        if (token3 != null && token3.Value > 0)
        {
            phase5Hp = token3.Value;
        }

        vanillaShakeThresholdsByFsm[fsmId] = (phase3Hp, phase4Hp, phase5Hp);
    }

    private static (int phase3Hp, int phase4Hp, int phase5Hp) GetVanillaShakeThresholds(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaShakeThresholdsByFsm.TryGetValue(fsmId, out (int phase3Hp, int phase4Hp, int phase5Hp) thresholds))
        {
            return thresholds;
        }

        RememberVanillaShakeThresholds(fsm);
        if (vanillaShakeThresholdsByFsm.TryGetValue(fsmId, out thresholds))
        {
            return thresholds;
        }

        return (DefaultBrokenVesselPhase3Hp, DefaultBrokenVesselPhase4Hp, DefaultBrokenVesselPhase5Hp);
    }

    private static void RememberVanillaSpawnThreshold(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSpawnThresholdByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int phase2Hp = DefaultBrokenVesselPhase2Hp;
        IntCompare? hpCompare = FindSpawnHpCompareAction(fsm);
        if (hpCompare?.integer2 != null && hpCompare.integer2.Value > 0)
        {
            phase2Hp = hpCompare.integer2.Value;
        }

        vanillaSpawnThresholdByFsm[fsmId] = phase2Hp;
    }

    private static int GetVanillaSpawnThreshold(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSpawnThresholdByFsm.TryGetValue(fsmId, out int threshold) && threshold > 0)
        {
            return threshold;
        }

        RememberVanillaSpawnThreshold(fsm);
        if (vanillaSpawnThresholdByFsm.TryGetValue(fsmId, out threshold) && threshold > 0)
        {
            return threshold;
        }

        return DefaultBrokenVesselPhase2Hp;
    }

    private static void RememberVanillaSummonLimit(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSummonLimitByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int limit = DefaultBrokenVesselVanillaSummonLimit;
        IntCompare? compare = FindSpawnEnemyCountCompareAction(fsm);
        if (compare?.integer2 != null)
        {
            limit = ClampBrokenVesselSummonLimit(compare.integer2.Value);
        }

        vanillaSummonLimitByFsm[fsmId] = limit;
    }

    private static int GetVanillaSummonLimit(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSummonLimitByFsm.TryGetValue(fsmId, out int limit))
        {
            return ClampBrokenVesselSummonLimit(limit);
        }

        RememberVanillaSummonLimit(fsm);
        if (vanillaSummonLimitByFsm.TryGetValue(fsmId, out limit))
        {
            return ClampBrokenVesselSummonLimit(limit);
        }

        return DefaultBrokenVesselVanillaSummonLimit;
    }

    private static void ApplyBrokenVesselHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampBrokenVesselHp(brokenVesselMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void ApplyBrokenVesselSummonHealth(GameObject summon, HealthManager? hm = null)
    {
        if (!ShouldApplySummonSettings(summon) || !ShouldUseCustomSummonHp())
        {
            return;
        }

        hm ??= summon.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        RememberVanillaSummonHp(hm);
        int targetHp = ClampBrokenVesselHp(brokenVesselSummonHp);
        summon.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsBrokenVesselObject(boss))
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

        int targetHp = ClampBrokenVesselHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaSummonHealth(GameObject summon, HealthManager? hm = null)
    {
        if (summon == null || !IsBrokenVesselSummonObject(summon))
        {
            return;
        }

        hm ??= summon.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        if (!TryGetVanillaSummonHp(hm, out int vanillaHp))
        {
            return;
        }

        int targetHp = ClampBrokenVesselHp(vanillaHp);
        summon.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void TryEnforceCustomSummonLimit(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null || !ShouldUseCustomSummonLimit())
        {
            return;
        }

        if (!ShouldApplySummonSettings(hm.gameObject) || !hm.gameObject.activeInHierarchy)
        {
            return;
        }

        int maxAllowed = ClampBrokenVesselSummonLimit(brokenVesselSummonLimit);
        int activeSummons = CountActiveBrokenVesselSummons();
        if (activeSummons > maxAllowed)
        {
            DespawnBrokenVesselSummon(hm.gameObject);
        }
    }

    private static int CountActiveBrokenVesselSummons()
    {
        int count = 0;
        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsBrokenVesselSummon(hm))
            {
                continue;
            }

            if (!ShouldApplySummonSettings(hm.gameObject) || !hm.gameObject.activeInHierarchy)
            {
                continue;
            }

            count++;
        }

        return count;
    }

    private static void DespawnBrokenVesselSummon(GameObject summon)
    {
        if (summon == null)
        {
            return;
        }

        try
        {
            if (ObjectPool.IsSpawned(summon))
            {
                ObjectPool.Recycle(summon);
                return;
            }
        }
        catch
        {
            // Ignore ObjectPool failures and fall back to deactivation.
        }

        summon.SetActive(false);
    }

    private static bool TryFindBrokenVesselHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsBrokenVessel(candidate))
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

        hp = Math.Max(hp, DefaultBrokenVesselVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static void RememberVanillaSummonHp(HealthManager hm)
    {
        int instanceId = hm.GetInstanceID();
        if (vanillaSummonHpByInstance.ContainsKey(instanceId))
        {
            return;
        }

        int hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        if (hp <= 0)
        {
            hp = DefaultBrokenVesselSummonVanillaHp;
        }

        vanillaSummonHpByInstance[instanceId] = hp;
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultBrokenVesselVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultBrokenVesselVanillaHp);
        return hp > 0;
    }

    private static bool TryGetVanillaSummonHp(HealthManager hm, out int hp)
    {
        if (vanillaSummonHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        if (hp <= 0)
        {
            hp = DefaultBrokenVesselSummonVanillaHp;
        }

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

    private static int ResolvePhase2MaxHp()
    {
        int configuredMaxHp = ClampBrokenVesselHp(brokenVesselMaxHp);
        return Math.Max(MinBrokenVesselPhase2Hp, configuredMaxHp);
    }

    private static int ClampBrokenVesselHp(int value)
    {
        if (value < MinBrokenVesselHp)
        {
            return MinBrokenVesselHp;
        }

        return value > MaxBrokenVesselHp ? MaxBrokenVesselHp : value;
    }

    private static int ClampBrokenVesselPhase2Hp(int value, int phase2MaxHp)
    {
        int maxPhase2Hp = Math.Max(MinBrokenVesselPhase2Hp, phase2MaxHp);
        if (value < MinBrokenVesselPhase2Hp)
        {
            return MinBrokenVesselPhase2Hp;
        }

        return value > maxPhase2Hp ? maxPhase2Hp : value;
    }

    private static int ClampBrokenVesselPhase3Hp(int value, int phase2Hp)
    {
        int clampedPhase2Hp = phase2Hp < MinBrokenVesselPhase2Hp
            ? MinBrokenVesselPhase2Hp
            : phase2Hp;
        int maxPhase3Hp = Math.Max(MinBrokenVesselPhase3Hp, clampedPhase2Hp - 1);
        if (value < MinBrokenVesselPhase3Hp)
        {
            return MinBrokenVesselPhase3Hp;
        }

        return value > maxPhase3Hp ? maxPhase3Hp : value;
    }

    private static int ClampBrokenVesselPhase4Hp(int value, int phase3Hp)
    {
        int clampedPhase3Hp = phase3Hp < MinBrokenVesselPhase3Hp
            ? MinBrokenVesselPhase3Hp
            : phase3Hp;
        int maxPhase4Hp = Math.Max(MinBrokenVesselPhase4Hp, clampedPhase3Hp - 1);
        if (value < MinBrokenVesselPhase4Hp)
        {
            return MinBrokenVesselPhase4Hp;
        }

        return value > maxPhase4Hp ? maxPhase4Hp : value;
    }

    private static int ClampBrokenVesselPhase5Hp(int value, int phase4Hp)
    {
        int clampedPhase4Hp = phase4Hp < MinBrokenVesselPhase4Hp
            ? MinBrokenVesselPhase4Hp
            : phase4Hp;
        int maxPhase5Hp = Math.Max(MinBrokenVesselPhase5Hp, clampedPhase4Hp - 1);
        if (value < MinBrokenVesselPhase5Hp)
        {
            return MinBrokenVesselPhase5Hp;
        }

        return value > maxPhase5Hp ? maxPhase5Hp : value;
    }

    private static int ClampBrokenVesselSummonLimit(int value)
    {
        if (value < MinBrokenVesselSummonLimit)
        {
            return MinBrokenVesselSummonLimit;
        }

        return value > MaxBrokenVesselSummonLimit ? MaxBrokenVesselSummonLimit : value;
    }

}
