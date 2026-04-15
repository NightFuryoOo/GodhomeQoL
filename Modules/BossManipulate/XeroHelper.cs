using Modding;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class XeroHelper : Module
{
    private const string XeroScene = "GG_Ghost_Xero_V";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string XeroName = "Ghost Warrior Xero";
    private const string XeroPhaseFsmName = "Sword Summon";
    private const string XeroPhaseCheckStateName = "Check";
    private const string XeroPhase2VariableName = "Half HP";
    private const string XeroAttackingFsmName = "Attacking";
    private const string XeroRageVariableName = "Rage";
    private const string XeroSummonEventName = "SUMMON";
    private const int DefaultXeroMaxHp = 900;
    private const int DefaultXeroVanillaHp = 900;
    private const int DefaultXeroPhase2Hp = 450;
    private const int P5XeroHp = 650;
    private const int MinXeroHp = 1;
    private const int MaxXeroHp = 999999;
    private const int MinXeroPhase2Hp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool xeroUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool xeroP5Hp = false;

    [LocalSetting]
    internal static int xeroMaxHp = DefaultXeroMaxHp;

    [LocalSetting]
    internal static int xeroMaxHpBeforeP5 = DefaultXeroMaxHp;

    [LocalSetting]
    internal static bool xeroUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool xeroHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool xeroUseCustomPhase = false;

    [LocalSetting]
    internal static int xeroPhase2Hp = DefaultXeroPhase2Hp;

    [LocalSetting]
    internal static bool xeroUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int xeroPhase2HpBeforeP5 = DefaultXeroPhase2Hp;

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
        On.HealthManager.Update += OnHealthManagerUpdate;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_Xero;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_Xero;
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
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_Xero;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_Xero;
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
            ApplyXeroHealthIfPresent();
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
            if (!xeroP5Hp)
            {
                xeroMaxHpBeforeP5 = ClampXeroHp(xeroMaxHp);
                xeroUseMaxHpBeforeP5 = xeroUseMaxHp;
                xeroUseCustomPhaseBeforeP5 = xeroUseCustomPhase;
                xeroPhase2HpBeforeP5 = ClampXeroPhase2Hp(xeroPhase2Hp, ResolvePhase2MaxHp());
                xeroHasStoredStateBeforeP5 = true;
            }

            xeroP5Hp = true;
            xeroUseMaxHp = true;
            xeroUseCustomPhase = false;
            xeroMaxHp = P5XeroHp;
        }
        else
        {
            if (xeroP5Hp && xeroHasStoredStateBeforeP5)
            {
                xeroMaxHp = ClampXeroHp(xeroMaxHpBeforeP5);
                xeroUseMaxHp = xeroUseMaxHpBeforeP5;
                xeroUseCustomPhase = xeroUseCustomPhaseBeforeP5;
                xeroPhase2Hp = ClampXeroPhase2Hp(xeroPhase2HpBeforeP5, ResolvePhase2MaxHp());
            }

            xeroP5Hp = false;
            xeroHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!xeroP5Hp)
        {
            return;
        }

        if (!xeroHasStoredStateBeforeP5)
        {
            xeroMaxHpBeforeP5 = ClampXeroHp(xeroMaxHp);
            xeroUseMaxHpBeforeP5 = xeroUseMaxHp;
            xeroUseCustomPhaseBeforeP5 = xeroUseCustomPhase;
            xeroPhase2HpBeforeP5 = ClampXeroPhase2Hp(xeroPhase2Hp, ResolvePhase2MaxHp());
            xeroHasStoredStateBeforeP5 = true;
        }

        xeroUseMaxHp = true;
        xeroUseCustomPhase = false;
        xeroMaxHp = P5XeroHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        xeroPhase2Hp = ClampXeroPhase2Hp(xeroPhase2Hp, ResolvePhase2MaxHp());
    }

    internal static void ApplyXeroHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindXeroHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyXeroHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        if (!TryFindXeroHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            RestoreVanillaHealth(hm.gameObject, hm);
        }
    }

    private static void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsXero(self))
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
            ApplyXeroHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsXero(self))
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
            ApplyXeroHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerUpdate(On.HealthManager.orig_Update orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsXero(self))
        {
            return;
        }

        if (!ShouldApplySettings(self.gameObject) || !ShouldUseCustomPhaseThreshold())
        {
            return;
        }

        int threshold = ClampXeroPhase2Hp(xeroPhase2Hp, ResolvePhase2MaxHp());
        if (self.hp > threshold)
        {
            return;
        }

        TryForceXeroRage(self.gameObject);
    }

    private static void OnPlayMakerFsmOnEnable_Xero(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsXeroPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static void OnPlayMakerFsmStart_Xero(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsXeroPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsXero(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyXeroHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsXero(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyXeroHealth(hm.gameObject, hm);
            }
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, XeroScene, StringComparison.Ordinal))
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
            ApplyXeroHealthIfPresent();
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

    private static bool IsXero(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsXeroObject(hm.gameObject);
    }

    private static bool IsXeroObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, XeroScene, StringComparison.Ordinal)
            && gameObject.name.StartsWith(XeroName, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsXeroObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => xeroUseMaxHp;
    private static bool ShouldUseCustomPhaseThreshold() => xeroUseCustomPhase && !xeroP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, XeroScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, XeroScene, StringComparison.Ordinal) && hoGEntryAllowed)
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

    internal static void ApplyPhaseThresholdSettingsIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsXeroPhaseControlFsm(fsm))
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
            if (fsm == null || fsm.gameObject == null || !IsXeroPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplyPhaseSettings(fsm.gameObject))
            {
                continue;
            }

            SetPhase2ThresholdOnFsm(fsm, GetVanillaPhase2Hp(), useVanillaVariable: true);
        }
    }

    private static bool IsXeroPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null)
        {
            return false;
        }

        if (!string.Equals(fsm.gameObject.scene.name, XeroScene, StringComparison.Ordinal))
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, XeroPhaseFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState(XeroPhaseCheckStateName) != null
            && fsm.FsmVariables.GetFsmInt(XeroPhase2VariableName) != null;
    }

    private static bool ShouldApplyPhaseSettings(GameObject? gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        return string.Equals(gameObject.scene.name, XeroScene, StringComparison.Ordinal) && hoGEntryAllowed;
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsXeroPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplyPhaseSettings(fsm.gameObject))
        {
            return;
        }

        bool useVanillaVariable = !ShouldUseCustomPhaseThreshold();
        int targetThreshold = useVanillaVariable
            ? GetVanillaPhase2Hp()
            : ClampXeroPhase2Hp(xeroPhase2Hp, ResolvePhase2MaxHp());

        SetPhase2ThresholdOnFsm(fsm, targetThreshold, useVanillaVariable);
    }

    private static void SetPhase2ThresholdOnFsm(PlayMakerFSM fsm, int value, bool useVanillaVariable)
    {
        if (fsm == null)
        {
            return;
        }

        int threshold = ClampXeroPhase2Hp(value, ResolvePhase2MaxHp());
        FsmInt? halfHpVariable = fsm.FsmVariables.GetFsmInt(XeroPhase2VariableName);
        if (halfHpVariable != null)
        {
            halfHpVariable.Value = threshold;
        }

        FsmState? checkState = fsm.Fsm?.GetState(XeroPhaseCheckStateName);
        if (checkState?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in checkState.Actions)
        {
            if (action is IntCompare compare && compare.integer2 != null)
            {
                if (useVanillaVariable)
                {
                    compare.integer2.UseVariable = true;
                    compare.integer2.Name = XeroPhase2VariableName;
                }
                else
                {
                    compare.integer2.UseVariable = false;
                    compare.integer2.Name = string.Empty;
                    compare.integer2.Value = threshold;
                }
            }
        }
    }

    private static int GetVanillaPhase2Hp()
    {
        int maxHp = ResolvePhase2MaxHp();
        int threshold = maxHp / 2;
        return ClampXeroPhase2Hp(threshold, maxHp);
    }

    private static void TryForceXeroRage(GameObject bossObject)
    {
        if (bossObject == null || !ShouldApplyPhaseSettings(bossObject))
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsXeroObject(fsm.gameObject))
            {
                continue;
            }

            if (!string.Equals(fsm.FsmName, XeroAttackingFsmName, StringComparison.Ordinal))
            {
                continue;
            }

            FsmBool? rageFlag = fsm.FsmVariables.GetFsmBool(XeroRageVariableName);
            if (rageFlag == null || rageFlag.Value)
            {
                continue;
            }

            rageFlag.Value = true;
            fsm.SendEvent(XeroSummonEventName);
        }
    }

    private static void ApplyXeroHealth(GameObject boss, HealthManager? hm = null)
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
        int targetHp = ClampXeroHp(xeroMaxHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsXeroObject(boss))
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

        int targetHp = ClampXeroHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static bool TryFindXeroHealthManager(out HealthManager? hm)
    {
        hm = null;
        foreach (HealthManager candidate in UObject.FindObjectsOfType<HealthManager>())
        {
            if (candidate != null && IsXero(candidate))
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

        hp = Math.Max(hp, DefaultXeroVanillaHp);
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, DefaultXeroVanillaHp);
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, DefaultXeroVanillaHp);
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

    private static int ClampXeroHp(int value)
    {
        if (value < MinXeroHp)
        {
            return MinXeroHp;
        }

        return value > MaxXeroHp ? MaxXeroHp : value;
    }

    private static int ClampXeroPhase2Hp(int value, int maxHp)
    {
        int clampedMaxHp = ClampXeroHp(maxHp);
        if (value < MinXeroPhase2Hp)
        {
            return MinXeroPhase2Hp;
        }

        return value > clampedMaxHp ? clampedMaxHp : value;
    }

    private static int ResolvePhase2MaxHp()
    {
        if (ShouldUseCustomHp())
        {
            return ClampXeroHp(xeroMaxHp);
        }

        if (TryFindXeroHealthManager(out HealthManager? hm) && hm != null && TryGetVanillaHp(hm, out int vanillaHp))
        {
            return ClampXeroHp(vanillaHp);
        }

        return DefaultXeroVanillaHp;
    }
}
