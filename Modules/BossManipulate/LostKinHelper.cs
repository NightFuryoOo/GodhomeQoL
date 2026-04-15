using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class LostKinHelper : Module
{
    private const string LostKinScene = "GG_Lost_Kin";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string LostKinName = "Lost Kin";
    private static readonly string[] LostKinSummonNameHints = { "Bursting", "Bouncer", "Balloon", "Zombie" };
    private const string LostKinSpawnBalloonFsmName = "Spawn Balloon";
    private const string LostKinShakeTokenControlFsmName = "Shake Token Control";
    private const string LostKinSpawnStateName = "Spawn";
    private const string LostKinCheckState1Name = "Check";
    private const string LostKinCheckState2Name = "Check 2";
    private const string LostKinCheckState3Name = "Check 3";
    private const string LostKinHpVariableName = "HP";
    private const string LostKinToken1VariableName = "Token 1";
    private const string LostKinToken2VariableName = "Token 2";
    private const string LostKinToken3VariableName = "Token 3";
    private const int DefaultLostKinMaxHp = 1650;
    private const int DefaultLostKinVanillaHp = 1650;
    private const int DefaultLostKinSummonHp = 1;
    private const int DefaultLostKinSummonVanillaHp = 13;
    private const int DefaultLostKinSummonLimit = 5;
    private const int DefaultLostKinVanillaSummonLimit = 5;
    private const int DefaultLostKinPhase2Hp = 1150;
    private const int DefaultLostKinPhase3Hp = 550;
    private const int DefaultLostKinPhase4Hp = 350;
    private const int DefaultLostKinPhase5Hp = 175;
    private const int P5LostKinHp = 1200;
    private const int MinLostKinHp = 1;
    private const int MaxLostKinHp = 999999;
    private const int MinLostKinPhase2Hp = 4;
    private const int MinLostKinPhase3Hp = 3;
    private const int MinLostKinPhase4Hp = 2;
    private const int MinLostKinPhase5Hp = 1;
    private const int MinLostKinSummonLimit = 0;
    private const int MaxLostKinSummonLimit = 999;

    [LocalSetting]
    [BoolOption]
    internal static bool lostKinUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool lostKinP5Hp = false;

    [LocalSetting]
    internal static int lostKinMaxHp = DefaultLostKinMaxHp;

    [LocalSetting]
    internal static int lostKinMaxHpBeforeP5 = DefaultLostKinMaxHp;

    [LocalSetting]
    internal static bool lostKinUseCustomSummonHp = false;

    [LocalSetting]
    internal static int lostKinSummonHp = DefaultLostKinSummonHp;

    [LocalSetting]
    internal static bool lostKinUseCustomSummonLimit = false;

    [LocalSetting]
    internal static int lostKinSummonLimit = DefaultLostKinSummonLimit;

    [LocalSetting]
    internal static bool lostKinUseCustomPhase = false;

    [LocalSetting]
    internal static int lostKinPhase2Hp = DefaultLostKinPhase2Hp;

    [LocalSetting]
    internal static int lostKinPhase3Hp = DefaultLostKinPhase3Hp;

    [LocalSetting]
    internal static int lostKinPhase4Hp = DefaultLostKinPhase4Hp;

    [LocalSetting]
    internal static int lostKinPhase5Hp = DefaultLostKinPhase5Hp;

    [LocalSetting]
    internal static bool lostKinUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool lostKinHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool lostKinUseCustomSummonHpBeforeP5 = false;

    [LocalSetting]
    internal static int lostKinSummonHpBeforeP5 = DefaultLostKinSummonHp;

    [LocalSetting]
    internal static bool lostKinUseCustomSummonLimitBeforeP5 = false;

    [LocalSetting]
    internal static int lostKinSummonLimitBeforeP5 = DefaultLostKinSummonLimit;

    [LocalSetting]
    internal static bool lostKinUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int lostKinPhase2HpBeforeP5 = DefaultLostKinPhase2Hp;

    [LocalSetting]
    internal static int lostKinPhase3HpBeforeP5 = DefaultLostKinPhase3Hp;

    [LocalSetting]
    internal static int lostKinPhase4HpBeforeP5 = DefaultLostKinPhase4Hp;

    [LocalSetting]
    internal static int lostKinPhase5HpBeforeP5 = DefaultLostKinPhase5Hp;

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
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_LostKin;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_LostKin;
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
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_LostKin;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_LostKin;
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
            ApplyLostKinHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyLostKinSummonHealthIfPresent();
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
            if (!lostKinP5Hp)
            {
                lostKinMaxHpBeforeP5 = ClampLostKinHp(lostKinMaxHp);
                lostKinUseMaxHpBeforeP5 = lostKinUseMaxHp;
                lostKinUseCustomSummonHpBeforeP5 = lostKinUseCustomSummonHp;
                lostKinSummonHpBeforeP5 = ClampLostKinHp(lostKinSummonHp);
                lostKinUseCustomSummonLimitBeforeP5 = lostKinUseCustomSummonLimit;
                lostKinSummonLimitBeforeP5 = ClampLostKinSummonLimit(lostKinSummonLimit);
                lostKinUseCustomPhaseBeforeP5 = lostKinUseCustomPhase;
                lostKinPhase2HpBeforeP5 = ClampLostKinPhase2Hp(lostKinPhase2Hp, ResolvePhase2MaxHp());
                lostKinPhase3HpBeforeP5 = ClampLostKinPhase3Hp(lostKinPhase3Hp, lostKinPhase2HpBeforeP5);
                lostKinPhase4HpBeforeP5 = ClampLostKinPhase4Hp(lostKinPhase4Hp, lostKinPhase3HpBeforeP5);
                lostKinPhase5HpBeforeP5 = ClampLostKinPhase5Hp(lostKinPhase5Hp, lostKinPhase4HpBeforeP5);
                lostKinHasStoredStateBeforeP5 = true;
            }

            lostKinP5Hp = true;
            lostKinUseMaxHp = true;
            lostKinUseCustomSummonHp = false;
            lostKinUseCustomSummonLimit = false;
            lostKinUseCustomPhase = false;
            lostKinMaxHp = P5LostKinHp;
        }
        else
        {
            if (lostKinP5Hp && lostKinHasStoredStateBeforeP5)
            {
                lostKinMaxHp = ClampLostKinHp(lostKinMaxHpBeforeP5);
                lostKinUseMaxHp = lostKinUseMaxHpBeforeP5;
                lostKinUseCustomSummonHp = lostKinUseCustomSummonHpBeforeP5;
                lostKinSummonHp = ClampLostKinHp(lostKinSummonHpBeforeP5);
                lostKinUseCustomSummonLimit = lostKinUseCustomSummonLimitBeforeP5;
                lostKinSummonLimit = ClampLostKinSummonLimit(lostKinSummonLimitBeforeP5);
                lostKinUseCustomPhase = lostKinUseCustomPhaseBeforeP5;
                lostKinPhase2Hp = ClampLostKinPhase2Hp(lostKinPhase2HpBeforeP5, ResolvePhase2MaxHp());
                lostKinPhase3Hp = ClampLostKinPhase3Hp(lostKinPhase3HpBeforeP5, lostKinPhase2Hp);
                lostKinPhase4Hp = ClampLostKinPhase4Hp(lostKinPhase4HpBeforeP5, lostKinPhase3Hp);
                lostKinPhase5Hp = ClampLostKinPhase5Hp(lostKinPhase5HpBeforeP5, lostKinPhase4Hp);
            }

            lostKinP5Hp = false;
            lostKinHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!lostKinP5Hp)
        {
            return;
        }

        if (!lostKinHasStoredStateBeforeP5)
        {
            lostKinMaxHpBeforeP5 = ClampLostKinHp(lostKinMaxHp);
            lostKinUseMaxHpBeforeP5 = lostKinUseMaxHp;
            lostKinUseCustomSummonHpBeforeP5 = lostKinUseCustomSummonHp;
            lostKinSummonHpBeforeP5 = ClampLostKinHp(lostKinSummonHp);
            lostKinUseCustomSummonLimitBeforeP5 = lostKinUseCustomSummonLimit;
            lostKinSummonLimitBeforeP5 = ClampLostKinSummonLimit(lostKinSummonLimit);
            lostKinUseCustomPhaseBeforeP5 = lostKinUseCustomPhase;
            lostKinPhase2HpBeforeP5 = ClampLostKinPhase2Hp(lostKinPhase2Hp, ResolvePhase2MaxHp());
            lostKinPhase3HpBeforeP5 = ClampLostKinPhase3Hp(lostKinPhase3Hp, lostKinPhase2HpBeforeP5);
            lostKinPhase4HpBeforeP5 = ClampLostKinPhase4Hp(lostKinPhase4Hp, lostKinPhase3HpBeforeP5);
            lostKinPhase5HpBeforeP5 = ClampLostKinPhase5Hp(lostKinPhase5Hp, lostKinPhase4HpBeforeP5);
            lostKinHasStoredStateBeforeP5 = true;
        }

        lostKinUseMaxHp = true;
        lostKinUseCustomSummonHp = false;
        lostKinUseCustomSummonLimit = false;
        lostKinUseCustomPhase = false;
        lostKinMaxHp = P5LostKinHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        lostKinPhase2Hp = ClampLostKinPhase2Hp(lostKinPhase2Hp, ResolvePhase2MaxHp());
        lostKinPhase3Hp = ClampLostKinPhase3Hp(lostKinPhase3Hp, lostKinPhase2Hp);
        lostKinPhase4Hp = ClampLostKinPhase4Hp(lostKinPhase4Hp, lostKinPhase3Hp);
        lostKinPhase5Hp = ClampLostKinPhase5Hp(lostKinPhase5Hp, lostKinPhase4Hp);
    }

    internal static void ApplyLostKinHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindLostKinHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyLostKinHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindLostKinHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            RestoreVanillaHealth(hm.gameObject, hm);
        }
    }

    internal static void ApplyLostKinSummonHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomSummonHp())
        {
            return;
        }

        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsLostKinSummon(hm))
            {
                continue;
            }

            ApplyLostKinSummonHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaSummonHealthIfPresent()
    {
        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsLostKinSummon(hm))
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
            if (fsm == null || fsm.gameObject == null || !IsLostKinSpawnBalloonFsm(fsm))
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

        int maxAllowed = ClampLostKinSummonLimit(lostKinSummonLimit);
        int currentAlive = 0;
        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsLostKinSummon(hm))
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
                DespawnLostKinSummon(hm.gameObject);
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
            if (fsm == null || fsm.gameObject == null || !IsLostKinPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsLostKinPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            if (IsLostKinShakeTokenFsm(fsm))
            {
                RememberVanillaShakeThresholds(fsm);
                (int phase3Hp, int phase4Hp, int phase5Hp) thresholds = GetVanillaShakeThresholds(fsm);
                SetShakeThresholdsOnFsm(fsm, thresholds.phase3Hp, thresholds.phase4Hp, thresholds.phase5Hp, useExactCustomThresholds: false);
                continue;
            }

            if (IsLostKinSpawnBalloonFsm(fsm))
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

        if (IsLostKin(self))
        {
            RememberVanillaHp(self);
            if (!ShouldApplySettings(self.gameObject))
            {
                return;
            }

            if (ShouldUseCustomHp())
            {
                ApplyLostKinHealth(self.gameObject, self);
            }
            else
            {
                RestoreVanillaHealth(self.gameObject, self);
            }

            return;
        }

        if (!IsLostKinSummon(self))
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
            ApplyLostKinSummonHealth(self.gameObject, self);
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

        if (IsLostKin(self))
        {
            RememberVanillaHp(self);
            if (!ShouldApplySettings(self.gameObject))
            {
                return;
            }

            if (ShouldUseCustomHp())
            {
                ApplyLostKinHealth(self.gameObject, self);
                _ = self.StartCoroutine(DeferredApply(self));
            }
            else
            {
                RestoreVanillaHealth(self.gameObject, self);
            }

            ApplyPhaseThresholdSettingsIfPresent();
            return;
        }

        if (!IsLostKinSummon(self))
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
            ApplyLostKinSummonHealth(self.gameObject, self);
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

        if (!moduleActive || !IsLostKinSummon(self))
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
            ApplyLostKinSummonHealth(self.gameObject, self);
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
        if (targetObject == null || !IsLostKinSummonObject(targetObject) || !ShouldApplySummonSettings(targetObject))
        {
            return;
        }

        HealthManager hm = targetObject.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        RememberVanillaSummonHp(hm);
        ApplyLostKinSummonHealth(targetObject, hm);
    }

    private static void OnPlayMakerFsmOnEnable_LostKin(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsLostKinPhaseControlFsm(self))
        {
            return;
        }

        ApplySummonLimitSettings(self);
        ApplyPhaseThresholdSettings(self);
    }

    private static void OnPlayMakerFsmStart_LostKin(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsLostKinPhaseControlFsm(self))
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

        if (IsLostKin(hm))
        {
            if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyLostKinHealth(hm.gameObject, hm);
                yield return new WaitForSeconds(0.01f);
                if (moduleActive && hm != null && hm.gameObject != null && IsLostKin(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
                {
                    ApplyLostKinHealth(hm.gameObject, hm);
                }
            }

            ApplyPhaseThresholdSettingsIfPresent();
            yield break;
        }

        if (!IsLostKinSummon(hm))
        {
            yield break;
        }

        if (ShouldApplySummonSettings(hm.gameObject))
        {
            if (ShouldUseCustomSummonHp())
            {
                ApplyLostKinSummonHealth(hm.gameObject, hm);
                yield return new WaitForSeconds(0.01f);
                if (moduleActive && hm != null && hm.gameObject != null && IsLostKinSummon(hm) && ShouldUseCustomSummonHp() && ShouldApplySummonSettings(hm.gameObject))
                {
                    ApplyLostKinSummonHealth(hm.gameObject, hm);
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
        if (!string.Equals(to.name, LostKinScene, StringComparison.Ordinal))
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
            ApplyLostKinHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyLostKinSummonHealthIfPresent();
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

    private static bool IsLostKin(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsLostKinObject(hm.gameObject);
    }

    private static bool IsLostKinObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, LostKinScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(LostKinName, StringComparison.Ordinal);
    }

    private static bool IsLostKinSummon(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsLostKinSummonObject(hm.gameObject);
    }

    private static bool IsLostKinSummonObject(GameObject gameObject)
    {
        if (gameObject == null || IsLostKinObject(gameObject))
        {
            return false;
        }

        bool isBrokenSceneObject = string.Equals(gameObject.scene.name, LostKinScene, StringComparison.Ordinal);
        bool isBrokenSceneActive = string.Equals(USceneManager.GetActiveScene().name, LostKinScene, StringComparison.Ordinal);
        if (!isBrokenSceneObject && !isBrokenSceneActive)
        {
            return false;
        }

        string name = gameObject.name ?? string.Empty;
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        foreach (string hint in LostKinSummonNameHints)
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
        if (gameObject == null || !IsLostKinObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldApplySummonSettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsLostKinSummonObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => lostKinUseMaxHp;

    private static bool ShouldUseCustomSummonHp() => lostKinUseCustomSummonHp && !lostKinP5Hp;

    private static bool ShouldUseCustomSummonLimit() => lostKinUseCustomSummonLimit && !lostKinP5Hp;

    private static bool ShouldUseCustomPhaseThresholds() => lostKinUseCustomPhase && !lostKinP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, LostKinScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, LostKinScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static bool IsLostKinPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsLostKinObject(fsm.gameObject))
        {
            return false;
        }

        return IsLostKinShakeTokenFsm(fsm) || IsLostKinSpawnBalloonFsm(fsm);
    }

    private static bool IsLostKinShakeTokenFsm(PlayMakerFSM fsm)
    {
        if (fsm == null)
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, LostKinShakeTokenControlFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState(LostKinCheckState1Name) != null
            && fsm.Fsm?.GetState(LostKinCheckState2Name) != null
            && fsm.Fsm?.GetState(LostKinCheckState3Name) != null
            && fsm.FsmVariables.GetFsmInt(LostKinToken1VariableName) != null
            && fsm.FsmVariables.GetFsmInt(LostKinToken2VariableName) != null
            && fsm.FsmVariables.GetFsmInt(LostKinToken3VariableName) != null;
    }

    private static bool IsLostKinSpawnBalloonFsm(PlayMakerFSM fsm)
    {
        if (fsm == null)
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, LostKinSpawnBalloonFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState(LostKinSpawnStateName) != null
            && FindSpawnHpCompareAction(fsm) != null;
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsLostKinPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        bool useExactCustomThresholds = ShouldUseCustomPhaseThresholds();
        int customPhase2Hp = ClampLostKinPhase2Hp(lostKinPhase2Hp, ResolvePhase2MaxHp());
        int customPhase3Hp = ClampLostKinPhase3Hp(lostKinPhase3Hp, customPhase2Hp);
        int customPhase4Hp = ClampLostKinPhase4Hp(lostKinPhase4Hp, customPhase3Hp);
        int customPhase5Hp = ClampLostKinPhase5Hp(lostKinPhase5Hp, customPhase4Hp);

        if (IsLostKinShakeTokenFsm(fsm))
        {
            RememberVanillaShakeThresholds(fsm);
            (int phase3Hp, int phase4Hp, int phase5Hp) thresholds = useExactCustomThresholds
                ? (customPhase3Hp, customPhase4Hp, customPhase5Hp)
                : GetVanillaShakeThresholds(fsm);

            SetShakeThresholdsOnFsm(fsm, thresholds.phase3Hp, thresholds.phase4Hp, thresholds.phase5Hp, useExactCustomThresholds);
            return;
        }

        if (IsLostKinSpawnBalloonFsm(fsm))
        {
            RememberVanillaSpawnThreshold(fsm);
            int phase2Hp = useExactCustomThresholds ? customPhase2Hp : GetVanillaSpawnThreshold(fsm);
            SetSpawnThresholdOnFsm(fsm, phase2Hp, useExactCustomThresholds);
        }
    }

    private static void ApplySummonLimitSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsLostKinSpawnBalloonFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        RememberVanillaSummonLimit(fsm);
        int targetLimit = ShouldUseCustomSummonLimit()
            ? ClampLostKinSummonLimit(lostKinSummonLimit)
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
        compare.integer2.Value = ClampLostKinSummonLimit(value);
    }

    private static void SetShakeThresholdsOnFsm(PlayMakerFSM fsm, int phase3Hp, int phase4Hp, int phase5Hp, bool useExactCustomThresholds)
    {
        int targetPhase3Hp = Math.Max(MinLostKinPhase3Hp, phase3Hp);
        int targetPhase4Hp = Math.Max(MinLostKinPhase4Hp, phase4Hp);
        int targetPhase5Hp = Math.Max(MinLostKinPhase5Hp, phase5Hp);

        FsmInt? token1 = fsm.FsmVariables.GetFsmInt(LostKinToken1VariableName);
        if (token1 != null)
        {
            token1.Value = targetPhase3Hp;
        }

        FsmInt? token2 = fsm.FsmVariables.GetFsmInt(LostKinToken2VariableName);
        if (token2 != null)
        {
            token2.Value = targetPhase4Hp;
        }

        FsmInt? token3 = fsm.FsmVariables.GetFsmInt(LostKinToken3VariableName);
        if (token3 != null)
        {
            token3.Value = targetPhase5Hp;
        }

        SetThresholdOnCheckState(
            fsm,
            LostKinCheckState1Name,
            LostKinToken1VariableName,
            targetPhase3Hp,
            useExactCustomThresholds);
        SetThresholdOnCheckState(
            fsm,
            LostKinCheckState2Name,
            LostKinToken2VariableName,
            targetPhase4Hp,
            useExactCustomThresholds);
        SetThresholdOnCheckState(
            fsm,
            LostKinCheckState3Name,
            LostKinToken3VariableName,
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

        int targetPhase2Hp = Math.Max(MinLostKinPhase2Hp, phase2Hp);
        hpCompare.integer2.UseVariable = false;
        hpCompare.integer2.Name = string.Empty;
        hpCompare.integer2.Value = useExactCustomThresholds
            ? targetPhase2Hp
            : targetPhase2Hp;
    }

    private static IntCompare? FindSpawnHpCompareAction(PlayMakerFSM fsm)
    {
        FsmState? spawnState = fsm.Fsm?.GetState(LostKinSpawnStateName);
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
        FsmState? spawnState = fsm.Fsm?.GetState(LostKinSpawnStateName);
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
            && string.Equals(compare.integer1.Name, LostKinHpVariableName, StringComparison.Ordinal))
            || (compare.integer2 != null
                && string.Equals(compare.integer2.Name, LostKinHpVariableName, StringComparison.Ordinal));
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

        int phase3Hp = DefaultLostKinPhase3Hp;
        int phase4Hp = DefaultLostKinPhase4Hp;
        int phase5Hp = DefaultLostKinPhase5Hp;

        FsmInt? token1 = fsm.FsmVariables.GetFsmInt(LostKinToken1VariableName);
        if (token1 != null && token1.Value > 0)
        {
            phase3Hp = token1.Value;
        }

        FsmInt? token2 = fsm.FsmVariables.GetFsmInt(LostKinToken2VariableName);
        if (token2 != null && token2.Value > 0)
        {
            phase4Hp = token2.Value;
        }

        FsmInt? token3 = fsm.FsmVariables.GetFsmInt(LostKinToken3VariableName);
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

        return (DefaultLostKinPhase3Hp, DefaultLostKinPhase4Hp, DefaultLostKinPhase5Hp);
    }

    private static void RememberVanillaSpawnThreshold(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSpawnThresholdByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int phase2Hp = DefaultLostKinPhase2Hp;
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

        return DefaultLostKinPhase2Hp;
    }

    private static void RememberVanillaSummonLimit(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSummonLimitByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int limit = DefaultLostKinVanillaSummonLimit;
        IntCompare? compare = FindSpawnEnemyCountCompareAction(fsm);
        if (compare?.integer2 != null)
        {
            limit = ClampLostKinSummonLimit(compare.integer2.Value);
        }

        vanillaSummonLimitByFsm[fsmId] = limit;
    }

    private static int GetVanillaSummonLimit(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSummonLimitByFsm.TryGetValue(fsmId, out int limit))
        {
            return ClampLostKinSummonLimit(limit);
        }

        RememberVanillaSummonLimit(fsm);
        if (vanillaSummonLimitByFsm.TryGetValue(fsmId, out limit))
        {
            return ClampLostKinSummonLimit(limit);
        }

        return DefaultLostKinVanillaSummonLimit;
    }

    private static void ApplyLostKinHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampLostKinHp(lostKinMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void ApplyLostKinSummonHealth(GameObject summon, HealthManager? hm = null)
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
        int targetHp = ClampLostKinHp(lostKinSummonHp);
        summon.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsLostKinObject(boss))
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

        int targetHp = ClampLostKinHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaSummonHealth(GameObject summon, HealthManager? hm = null)
    {
        if (summon == null || !IsLostKinSummonObject(summon))
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

        int targetHp = ClampLostKinHp(vanillaHp);
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

        int maxAllowed = ClampLostKinSummonLimit(lostKinSummonLimit);
        int activeSummons = CountActiveLostKinSummons();
        if (activeSummons > maxAllowed)
        {
            DespawnLostKinSummon(hm.gameObject);
        }
    }

    private static int CountActiveLostKinSummons()
    {
        int count = 0;
        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsLostKinSummon(hm))
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

    private static void DespawnLostKinSummon(GameObject summon)
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

    private static bool TryFindLostKinHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsLostKin(candidate))
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

        hp = Math.Max(hp, DefaultLostKinVanillaHp);
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
            hp = DefaultLostKinSummonVanillaHp;
        }

        vanillaSummonHpByInstance[instanceId] = hp;
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultLostKinVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultLostKinVanillaHp);
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
            hp = DefaultLostKinSummonVanillaHp;
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
        int configuredMaxHp = ClampLostKinHp(lostKinMaxHp);
        return Math.Max(MinLostKinPhase2Hp, configuredMaxHp);
    }

    private static int ClampLostKinHp(int value)
    {
        if (value < MinLostKinHp)
        {
            return MinLostKinHp;
        }

        return value > MaxLostKinHp ? MaxLostKinHp : value;
    }

    private static int ClampLostKinPhase2Hp(int value, int phase2MaxHp)
    {
        int maxPhase2Hp = Math.Max(MinLostKinPhase2Hp, phase2MaxHp);
        if (value < MinLostKinPhase2Hp)
        {
            return MinLostKinPhase2Hp;
        }

        return value > maxPhase2Hp ? maxPhase2Hp : value;
    }

    private static int ClampLostKinPhase3Hp(int value, int phase2Hp)
    {
        int clampedPhase2Hp = phase2Hp < MinLostKinPhase2Hp
            ? MinLostKinPhase2Hp
            : phase2Hp;
        int maxPhase3Hp = Math.Max(MinLostKinPhase3Hp, clampedPhase2Hp - 1);
        if (value < MinLostKinPhase3Hp)
        {
            return MinLostKinPhase3Hp;
        }

        return value > maxPhase3Hp ? maxPhase3Hp : value;
    }

    private static int ClampLostKinPhase4Hp(int value, int phase3Hp)
    {
        int clampedPhase3Hp = phase3Hp < MinLostKinPhase3Hp
            ? MinLostKinPhase3Hp
            : phase3Hp;
        int maxPhase4Hp = Math.Max(MinLostKinPhase4Hp, clampedPhase3Hp - 1);
        if (value < MinLostKinPhase4Hp)
        {
            return MinLostKinPhase4Hp;
        }

        return value > maxPhase4Hp ? maxPhase4Hp : value;
    }

    private static int ClampLostKinPhase5Hp(int value, int phase4Hp)
    {
        int clampedPhase4Hp = phase4Hp < MinLostKinPhase4Hp
            ? MinLostKinPhase4Hp
            : phase4Hp;
        int maxPhase5Hp = Math.Max(MinLostKinPhase5Hp, clampedPhase4Hp - 1);
        if (value < MinLostKinPhase5Hp)
        {
            return MinLostKinPhase5Hp;
        }

        return value > maxPhase5Hp ? maxPhase5Hp : value;
    }

    private static int ClampLostKinSummonLimit(int value)
    {
        if (value < MinLostKinSummonLimit)
        {
            return MinLostKinSummonLimit;
        }

        return value > MaxLostKinSummonLimit ? MaxLostKinSummonLimit : value;
    }

}

