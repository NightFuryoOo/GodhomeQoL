using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class WingedNoskHelper : Module
{
    private const string WingedNoskScene = "GG_Nosk_Hornet";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string WingedNoskName = "Hornet Nosk";
    private static readonly string[] WingedNoskSummonNameHints =
    {
        "Parasite Balloon Spawner",
        "Parasite Balloon",
        "Buzzer",
    };
    private const string WingedNoskHalfHpVariableName = "Half HP";
    private const string WingedNoskHpVariableName = "HP";
    private const string WingedNoskEnemyCountVariableName = "Enemy Count";
    private const string WingedNoskPhaseCheckStateName = "Choose Attack";
    private const string WingedNoskSummonStateName = "Summon";
    private const string WingedNoskSummonRoarStateName = "Summon Roar";
    private const int DefaultWingedNoskMaxHp = 1050;
    private const int DefaultWingedNoskVanillaHp = 1050;
    private const int DefaultWingedNoskPhase2Hp = 525;
    private const int DefaultWingedNoskSummonHp = 1;
    private const int DefaultWingedNoskSummonVanillaHp = 8;
    private const int DefaultWingedNoskSummonLimit = 5;
    private const int DefaultWingedNoskVanillaSummonLimit = 5;
    private const int P5WingedNoskHp = 750;
    private const int MinWingedNoskHp = 1;
    private const int MaxWingedNoskHp = 999999;
    private const int MinWingedNoskPhase2Hp = 1;
    private const int MinWingedNoskSummonLimit = 0;
    private const int MaxWingedNoskSummonLimit = 999;

    [LocalSetting]
    [BoolOption]
    internal static bool wingedNoskUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool wingedNoskP5Hp = false;

    [LocalSetting]
    internal static int wingedNoskMaxHp = DefaultWingedNoskMaxHp;

    [LocalSetting]
    internal static int wingedNoskMaxHpBeforeP5 = DefaultWingedNoskMaxHp;

    [LocalSetting]
    internal static bool wingedNoskUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool wingedNoskHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool wingedNoskUseCustomPhase = false;

    [LocalSetting]
    internal static int wingedNoskPhase2Hp = DefaultWingedNoskPhase2Hp;

    [LocalSetting]
    internal static bool wingedNoskUseCustomSummonHp = false;

    [LocalSetting]
    internal static int wingedNoskSummonHp = DefaultWingedNoskSummonHp;

    [LocalSetting]
    internal static bool wingedNoskUseCustomSummonLimit = false;

    [LocalSetting]
    internal static int wingedNoskSummonLimit = DefaultWingedNoskSummonLimit;

    [LocalSetting]
    internal static bool wingedNoskUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int wingedNoskPhase2HpBeforeP5 = DefaultWingedNoskPhase2Hp;

    [LocalSetting]
    internal static bool wingedNoskUseCustomSummonHpBeforeP5 = false;

    [LocalSetting]
    internal static int wingedNoskSummonHpBeforeP5 = DefaultWingedNoskSummonHp;

    [LocalSetting]
    internal static bool wingedNoskUseCustomSummonLimitBeforeP5 = false;

    [LocalSetting]
    internal static int wingedNoskSummonLimitBeforeP5 = DefaultWingedNoskSummonLimit;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static readonly Dictionary<int, int> vanillaSummonHpByInstance = new();
    private static readonly Dictionary<int, int> vanillaSummonLimitByFsm = new();
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
        On.SetHP.OnEnter += OnSetHpEnter;
        On.HealthManager.OnEnable += OnHealthManagerOnEnable;
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart;
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
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
        vanillaSummonHpByInstance.Clear();
        vanillaSummonLimitByFsm.Clear();
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
            ApplyWingedNoskHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyWingedNoskSummonHealthIfPresent();
        }
        else
        {
            RestoreVanillaSummonHealthIfPresent();
        }

        if (ShouldUseCustomSummonLimit())
        {
            ApplySummonLimitSettingsIfPresent();
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
            if (!wingedNoskP5Hp)
            {
                wingedNoskMaxHpBeforeP5 = ClampWingedNoskHp(wingedNoskMaxHp);
                wingedNoskUseMaxHpBeforeP5 = wingedNoskUseMaxHp;
                wingedNoskUseCustomPhaseBeforeP5 = wingedNoskUseCustomPhase;
                wingedNoskPhase2HpBeforeP5 = ClampWingedNoskPhase2Hp(wingedNoskPhase2Hp, ResolvePhase2MaxHp());
                wingedNoskUseCustomSummonHpBeforeP5 = wingedNoskUseCustomSummonHp;
                wingedNoskSummonHpBeforeP5 = ClampWingedNoskHp(wingedNoskSummonHp);
                wingedNoskUseCustomSummonLimitBeforeP5 = wingedNoskUseCustomSummonLimit;
                wingedNoskSummonLimitBeforeP5 = ClampWingedNoskSummonLimit(wingedNoskSummonLimit);
                wingedNoskHasStoredStateBeforeP5 = true;
            }

            wingedNoskP5Hp = true;
            wingedNoskUseMaxHp = true;
            wingedNoskUseCustomPhase = false;
            wingedNoskUseCustomSummonHp = false;
            wingedNoskUseCustomSummonLimit = false;
            wingedNoskMaxHp = P5WingedNoskHp;
        }
        else
        {
            if (wingedNoskP5Hp && wingedNoskHasStoredStateBeforeP5)
            {
                wingedNoskMaxHp = ClampWingedNoskHp(wingedNoskMaxHpBeforeP5);
                wingedNoskUseMaxHp = wingedNoskUseMaxHpBeforeP5;
                wingedNoskUseCustomPhase = wingedNoskUseCustomPhaseBeforeP5;
                wingedNoskPhase2Hp = ClampWingedNoskPhase2Hp(wingedNoskPhase2HpBeforeP5, ResolvePhase2MaxHp());
                wingedNoskUseCustomSummonHp = wingedNoskUseCustomSummonHpBeforeP5;
                wingedNoskSummonHp = ClampWingedNoskHp(wingedNoskSummonHpBeforeP5);
                wingedNoskUseCustomSummonLimit = wingedNoskUseCustomSummonLimitBeforeP5;
                wingedNoskSummonLimit = ClampWingedNoskSummonLimit(wingedNoskSummonLimitBeforeP5);
            }

            wingedNoskP5Hp = false;
            wingedNoskHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!wingedNoskP5Hp)
        {
            return;
        }

        if (!wingedNoskHasStoredStateBeforeP5)
        {
            wingedNoskMaxHpBeforeP5 = ClampWingedNoskHp(wingedNoskMaxHp);
            wingedNoskUseMaxHpBeforeP5 = wingedNoskUseMaxHp;
            wingedNoskUseCustomPhaseBeforeP5 = wingedNoskUseCustomPhase;
            wingedNoskPhase2HpBeforeP5 = ClampWingedNoskPhase2Hp(wingedNoskPhase2Hp, ResolvePhase2MaxHp());
            wingedNoskUseCustomSummonHpBeforeP5 = wingedNoskUseCustomSummonHp;
            wingedNoskSummonHpBeforeP5 = ClampWingedNoskHp(wingedNoskSummonHp);
            wingedNoskUseCustomSummonLimitBeforeP5 = wingedNoskUseCustomSummonLimit;
            wingedNoskSummonLimitBeforeP5 = ClampWingedNoskSummonLimit(wingedNoskSummonLimit);
            wingedNoskHasStoredStateBeforeP5 = true;
        }

        wingedNoskUseMaxHp = true;
        wingedNoskUseCustomPhase = false;
        wingedNoskUseCustomSummonHp = false;
        wingedNoskUseCustomSummonLimit = false;
        wingedNoskMaxHp = P5WingedNoskHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        wingedNoskPhase2Hp = ClampWingedNoskPhase2Hp(wingedNoskPhase2Hp, ResolvePhase2MaxHp());
    }

    internal static void ApplyWingedNoskHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindWingedNoskHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyWingedNoskHealth(hm.gameObject, hm);
        }
    }

    internal static void ApplyWingedNoskSummonHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomSummonHp())
        {
            return;
        }

        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsWingedNoskSummon(hm))
            {
                continue;
            }

            ApplyWingedNoskSummonHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindWingedNoskHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            RestoreVanillaHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaSummonHealthIfPresent()
    {
        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsWingedNoskSummon(hm))
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
            if (fsm == null || fsm.gameObject == null || !IsWingedNoskSummonControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            RememberVanillaSummonLimit(fsm);
            SetSummonLimitOnFsm(fsm, GetVanillaSummonLimit(fsm), useExactCustomLimit: false);
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
            if (fsm == null || fsm.gameObject == null || !IsWingedNoskPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsWingedNoskPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            SetPhase2ThresholdOnFsm(fsm, GetVanillaPhase2Hp(), useVanillaVariable: true);
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
        if (targetObject == null || !IsWingedNoskSummonObject(targetObject) || !ShouldApplySummonSettings(targetObject))
        {
            return;
        }

        HealthManager hm = targetObject.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        RememberVanillaSummonHp(hm);
        ApplyWingedNoskSummonHealth(targetObject, hm);
    }

    private static void OnHealthManagerOnEnable(On.HealthManager.orig_OnEnable orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsWingedNoskSummon(self))
        {
            return;
        }

        RememberVanillaSummonHp(self);
        if (!ShouldApplySummonSettings(self.gameObject))
        {
            return;
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyWingedNoskSummonHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaSummonHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive)
        {
            return;
        }

        if (IsWingedNosk(self))
        {
            RememberVanillaHp(self);
            if (!ShouldApplySettings(self.gameObject))
            {
                return;
            }

            if (ShouldUseCustomHp())
            {
                ApplyWingedNoskHealth(self.gameObject, self);
            }
            else
            {
                RestoreVanillaHealth(self.gameObject, self);
            }

            return;
        }

        if (!IsWingedNoskSummon(self))
        {
            return;
        }

        RememberVanillaSummonHp(self);
        if (!ShouldApplySummonSettings(self.gameObject))
        {
            return;
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyWingedNoskSummonHealth(self.gameObject, self);
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

        if (IsWingedNosk(self))
        {
            RememberVanillaHp(self);
            if (!ShouldApplySettings(self.gameObject))
            {
                return;
            }

            if (ShouldUseCustomHp())
            {
                ApplyWingedNoskHealth(self.gameObject, self);
                _ = self.StartCoroutine(DeferredApply(self));
            }
            else
            {
                RestoreVanillaHealth(self.gameObject, self);
            }

            ApplyPhaseThresholdSettingsIfPresent();
            return;
        }

        if (!IsWingedNoskSummon(self))
        {
            return;
        }

        RememberVanillaSummonHp(self);
        if (!ShouldApplySummonSettings(self.gameObject))
        {
            return;
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyWingedNoskSummonHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaSummonHealth(self.gameObject, self);
        }
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null)
        {
            yield break;
        }

        if (IsWingedNosk(hm))
        {
            if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyWingedNoskHealth(hm.gameObject, hm);
                yield return new WaitForSeconds(0.01f);
                if (moduleActive && hm != null && hm.gameObject != null && IsWingedNosk(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
                {
                    ApplyWingedNoskHealth(hm.gameObject, hm);
                }
            }

            ApplyPhaseThresholdSettingsIfPresent();
            yield break;
        }

        if (!IsWingedNoskSummon(hm))
        {
            yield break;
        }

        if (!ShouldApplySummonSettings(hm.gameObject))
        {
            yield break;
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyWingedNoskSummonHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsWingedNoskSummon(hm) && ShouldUseCustomSummonHp() && ShouldApplySummonSettings(hm.gameObject))
            {
                ApplyWingedNoskSummonHealth(hm.gameObject, hm);
            }
        }
        else
        {
            RestoreVanillaSummonHealth(hm.gameObject, hm);
        }
    }

    private static void OnPlayMakerFsmOnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null)
        {
            return;
        }

        ApplySummonLimitSettings(self);
        ApplyPhaseThresholdSettings(self);
    }

    private static void OnPlayMakerFsmStart(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null)
        {
            return;
        }

        ApplySummonLimitSettings(self);
        ApplyPhaseThresholdSettings(self);
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, WingedNoskScene, StringComparison.Ordinal))
        {
            vanillaHpByInstance.Clear();
            vanillaSummonHpByInstance.Clear();
            vanillaSummonLimitByFsm.Clear();
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyWingedNoskHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyWingedNoskSummonHealthIfPresent();
        }
        else
        {
            RestoreVanillaSummonHealthIfPresent();
        }

        if (ShouldUseCustomSummonLimit())
        {
            ApplySummonLimitSettingsIfPresent();
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

    private static bool IsWingedNosk(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsWingedNoskObject(hm.gameObject);
    }

    private static bool IsWingedNoskObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, WingedNoskScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(WingedNoskName, StringComparison.Ordinal);
    }

    private static bool IsWingedNoskSummon(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsWingedNoskSummonObject(hm.gameObject);
    }

    private static bool IsWingedNoskSummonObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        if (gameObject.name.StartsWith(WingedNoskName, StringComparison.Ordinal))
        {
            return false;
        }

        bool matchesSummonName = false;
        foreach (string hint in WingedNoskSummonNameHints)
        {
            if (gameObject.name.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                matchesSummonName = true;
                break;
            }
        }

        if (!matchesSummonName)
        {
            return false;
        }

        if (string.Equals(gameObject.scene.name, WingedNoskScene, StringComparison.Ordinal))
        {
            return true;
        }

        // Summons can come from pooled prefabs and keep a non-arena scene.
        return hoGEntryAllowed && string.Equals(USceneManager.GetActiveScene().name, WingedNoskScene, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsWingedNoskObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldApplySummonSettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsWingedNoskSummonObject(gameObject))
        {
            return false;
        }

        if (!hoGEntryAllowed)
        {
            return false;
        }

        string activeScene = USceneManager.GetActiveScene().name;
        return string.Equals(activeScene, WingedNoskScene, StringComparison.Ordinal)
            || string.Equals(gameObject.scene.name, WingedNoskScene, StringComparison.Ordinal);
    }

    private static bool ShouldUseCustomHp() => wingedNoskUseMaxHp;
    private static bool ShouldUseCustomPhaseThreshold() => wingedNoskUseCustomPhase && !wingedNoskP5Hp;
    private static bool ShouldUseCustomSummonHp() => wingedNoskUseCustomSummonHp && !wingedNoskP5Hp;
    private static bool ShouldUseCustomSummonLimit() => wingedNoskUseCustomSummonLimit && !wingedNoskP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, WingedNoskScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, WingedNoskScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    private static void ApplyWingedNoskHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampWingedNoskHp(wingedNoskMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void ApplyWingedNoskSummonHealth(GameObject summon, HealthManager? hm = null)
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
        int targetHp = ClampWingedNoskHp(wingedNoskSummonHp);
        summon.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void ApplySummonLimitSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsWingedNoskSummonControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        RememberVanillaSummonLimit(fsm);
        bool useCustomSummonLimit = ShouldUseCustomSummonLimit();
        int targetLimit = useCustomSummonLimit
            ? ClampWingedNoskSummonLimit(wingedNoskSummonLimit)
            : GetVanillaSummonLimit(fsm);
        SetSummonLimitOnFsm(fsm, targetLimit, useCustomSummonLimit);
    }

    private static bool IsWingedNoskSummonControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsWingedNoskObject(fsm.gameObject))
        {
            return false;
        }

        return FindSummonEnemyCountCompareAction(fsm) != null;
    }

    private static void SetSummonLimitOnFsm(PlayMakerFSM fsm, int value, bool useExactCustomLimit)
    {
        IntCompare? compare = FindSummonEnemyCountCompareAction(fsm);
        if (compare == null)
        {
            return;
        }

        int clampedLimit = ClampWingedNoskSummonLimit(value);
        int thresholdValue = useExactCustomLimit
            ? ConvertCustomSummonLimitToThreshold(clampedLimit)
            : clampedLimit;
        if (compare.integer1 != null
            && string.Equals(compare.integer1.Name, WingedNoskEnemyCountVariableName, StringComparison.Ordinal)
            && compare.integer2 != null)
        {
            compare.integer2.UseVariable = false;
            compare.integer2.Name = string.Empty;
            compare.integer2.Value = thresholdValue;
            return;
        }

        if (compare.integer2 != null
            && string.Equals(compare.integer2.Name, WingedNoskEnemyCountVariableName, StringComparison.Ordinal)
            && compare.integer1 != null)
        {
            compare.integer1.UseVariable = false;
            compare.integer1.Name = string.Empty;
            compare.integer1.Value = thresholdValue;
            return;
        }

        if (compare.integer2 != null)
        {
            compare.integer2.UseVariable = false;
            compare.integer2.Name = string.Empty;
            compare.integer2.Value = thresholdValue;
        }
    }

    private static int ConvertCustomSummonLimitToThreshold(int desiredLimit)
    {
        int clampedDesiredLimit = ClampWingedNoskSummonLimit(desiredLimit);
        return clampedDesiredLimit <= int.MinValue + 1 ? int.MinValue : clampedDesiredLimit - 1;
    }

    private static IntCompare? FindSummonEnemyCountCompareAction(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.Fsm == null)
        {
            return null;
        }

        IntCompare? inSummon = FindEnemyCountCompareInState(fsm, WingedNoskSummonStateName);
        if (inSummon != null)
        {
            return inSummon;
        }

        return FindEnemyCountCompareInState(fsm, WingedNoskSummonRoarStateName);
    }

    private static IntCompare? FindEnemyCountCompareInState(PlayMakerFSM fsm, string stateName)
    {
        FsmState? state = fsm.Fsm?.GetState(stateName);
        if (state?.Actions == null)
        {
            return null;
        }

        foreach (FsmStateAction action in state.Actions)
        {
            if (action is IntCompare compare && IsEnemyCountCompare(compare))
            {
                return compare;
            }
        }

        return null;
    }

    private static bool IsEnemyCountCompare(IntCompare compare)
    {
        return (compare.integer1 != null
            && string.Equals(compare.integer1.Name, WingedNoskEnemyCountVariableName, StringComparison.Ordinal))
            || (compare.integer2 != null
                && string.Equals(compare.integer2.Name, WingedNoskEnemyCountVariableName, StringComparison.Ordinal));
    }

    private static void RememberVanillaSummonLimit(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSummonLimitByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int limit = DefaultWingedNoskVanillaSummonLimit;
        IntCompare? compare = FindSummonEnemyCountCompareAction(fsm);
        if (compare != null)
        {
            limit = ReadEnemyCountThreshold(compare, DefaultWingedNoskVanillaSummonLimit);
        }

        vanillaSummonLimitByFsm[fsmId] = ClampWingedNoskSummonLimit(limit);
    }

    private static int GetVanillaSummonLimit(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSummonLimitByFsm.TryGetValue(fsmId, out int limit))
        {
            return ClampWingedNoskSummonLimit(limit);
        }

        RememberVanillaSummonLimit(fsm);
        if (vanillaSummonLimitByFsm.TryGetValue(fsmId, out limit))
        {
            return ClampWingedNoskSummonLimit(limit);
        }

        return DefaultWingedNoskVanillaSummonLimit;
    }

    private static int ReadEnemyCountThreshold(IntCompare compare, int fallback)
    {
        if (compare.integer1 != null
            && string.Equals(compare.integer1.Name, WingedNoskEnemyCountVariableName, StringComparison.Ordinal)
            && compare.integer2 != null)
        {
            return compare.integer2.Value;
        }

        if (compare.integer2 != null
            && string.Equals(compare.integer2.Name, WingedNoskEnemyCountVariableName, StringComparison.Ordinal)
            && compare.integer1 != null)
        {
            return compare.integer1.Value;
        }

        if (compare.integer2 != null)
        {
            return compare.integer2.Value;
        }

        return fallback;
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsWingedNoskPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        bool useVanillaVariable = !ShouldUseCustomPhaseThreshold();
        int targetThreshold = useVanillaVariable
            ? GetVanillaPhase2Hp()
            : ClampWingedNoskPhase2Hp(wingedNoskPhase2Hp, ResolvePhase2MaxHp());
        SetPhase2ThresholdOnFsm(fsm, targetThreshold, useVanillaVariable);
    }

    private static bool IsWingedNoskPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsWingedNoskObject(fsm.gameObject))
        {
            return false;
        }

        return fsm.Fsm?.GetState(WingedNoskPhaseCheckStateName) != null
            && fsm.FsmVariables.GetFsmInt(WingedNoskHalfHpVariableName) != null;
    }

    private static void SetPhase2ThresholdOnFsm(PlayMakerFSM fsm, int value, bool useVanillaVariable)
    {
        int threshold = ClampWingedNoskPhase2Hp(value, ResolvePhase2MaxHp());
        FsmInt? halfHpVariable = fsm.FsmVariables.GetFsmInt(WingedNoskHalfHpVariableName);
        if (halfHpVariable != null)
        {
            halfHpVariable.Value = threshold;
        }

        IntCompare? compare = FindPhase2HpCompareAction(fsm);
        if (compare == null)
        {
            return;
        }

        FsmInt? thresholdOperand = null;
        if (compare.integer1 != null && string.Equals(compare.integer1.Name, WingedNoskHalfHpVariableName, StringComparison.Ordinal))
        {
            thresholdOperand = compare.integer1;
        }
        else if (compare.integer2 != null && string.Equals(compare.integer2.Name, WingedNoskHalfHpVariableName, StringComparison.Ordinal))
        {
            thresholdOperand = compare.integer2;
        }
        else if (compare.integer1 != null && string.Equals(compare.integer1.Name, WingedNoskHpVariableName, StringComparison.Ordinal))
        {
            thresholdOperand = compare.integer2;
        }
        else if (compare.integer2 != null && string.Equals(compare.integer2.Name, WingedNoskHpVariableName, StringComparison.Ordinal))
        {
            thresholdOperand = compare.integer1;
        }
        else
        {
            thresholdOperand = compare.integer2 ?? compare.integer1;
        }

        if (thresholdOperand == null)
        {
            return;
        }

        if (useVanillaVariable)
        {
            thresholdOperand.UseVariable = true;
            thresholdOperand.Name = WingedNoskHalfHpVariableName;
            thresholdOperand.Value = threshold;
        }
        else
        {
            thresholdOperand.UseVariable = false;
            thresholdOperand.Name = string.Empty;
            thresholdOperand.Value = threshold;
        }
    }

    private static IntCompare? FindPhase2HpCompareAction(PlayMakerFSM fsm)
    {
        FsmState? state = fsm.Fsm?.GetState(WingedNoskPhaseCheckStateName);
        if (state?.Actions == null)
        {
            return null;
        }

        foreach (FsmStateAction action in state.Actions)
        {
            if (action is IntCompare compare && IsHpCompare(compare))
            {
                return compare;
            }
        }

        return null;
    }

    private static bool IsHpCompare(IntCompare compare)
    {
        return (compare.integer1 != null
            && string.Equals(compare.integer1.Name, WingedNoskHpVariableName, StringComparison.Ordinal))
            || (compare.integer2 != null
                && string.Equals(compare.integer2.Name, WingedNoskHpVariableName, StringComparison.Ordinal));
    }

    private static int GetVanillaPhase2Hp()
    {
        int maxHp = ResolvePhase2MaxHp();
        int threshold = maxHp / 2;
        return ClampWingedNoskPhase2Hp(threshold, maxHp);
    }

    private static int ResolvePhase2MaxHp()
    {
        if (ShouldUseCustomHp())
        {
            return ClampWingedNoskHp(wingedNoskMaxHp);
        }

        if (TryFindWingedNoskHealthManager(out HealthManager? hm) && hm != null && TryGetVanillaHp(hm, out int vanillaHp))
        {
            return ClampWingedNoskHp(vanillaHp);
        }

        return DefaultWingedNoskVanillaHp;
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsWingedNoskObject(boss))
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

        int targetHp = ClampWingedNoskHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaSummonHealth(GameObject summon, HealthManager? hm = null)
    {
        if (summon == null || !IsWingedNoskSummonObject(summon))
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

        int targetHp = ClampWingedNoskHp(vanillaHp);
        summon.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindWingedNoskHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsWingedNosk(candidate))
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

        hp = Math.Max(hp, DefaultWingedNoskVanillaHp);
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
            hp = DefaultWingedNoskSummonVanillaHp;
        }

        vanillaSummonHpByInstance[instanceId] = hp;
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultWingedNoskVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultWingedNoskVanillaHp);
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
            hp = DefaultWingedNoskSummonVanillaHp;
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

    private static int ClampWingedNoskHp(int value)
    {
        if (value < MinWingedNoskHp)
        {
            return MinWingedNoskHp;
        }

        return value > MaxWingedNoskHp ? MaxWingedNoskHp : value;
    }

    private static int ClampWingedNoskPhase2Hp(int value, int maxHp)
    {
        int clampedMaxHp = ClampWingedNoskHp(maxHp);
        if (value < MinWingedNoskPhase2Hp)
        {
            return MinWingedNoskPhase2Hp;
        }

        return value > clampedMaxHp ? clampedMaxHp : value;
    }

    private static int ClampWingedNoskSummonLimit(int value)
    {
        if (value < MinWingedNoskSummonLimit)
        {
            return MinWingedNoskSummonLimit;
        }

        return value > MaxWingedNoskSummonLimit ? MaxWingedNoskSummonLimit : value;
    }
}
