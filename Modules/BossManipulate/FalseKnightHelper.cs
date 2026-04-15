using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using Modding;
using Satchel;
using Satchel.Futils;
using UnityEngine;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class FalseKnightHelper : Module
{
    private const string FalseKnightScene = "GG_False_Knight";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string ArmorNamePrefix = "False Knight";
    private const string MainPhase2StateName = "To Phase 2";
    private const string MainPhase3StateName = "To Phase 3";
    private const string MainOpened2StateName = "Opened 2";
    private const string MainHit2StateName = "Hit 2";
    private const string RecoverVariableName = "Recover HP";
    private const string RageCountVariableName = "Rages";
    private const string StunnedAmountVariableName = "Stunned Amount";
    private const int DefaultArmorPhaseHp = 560;
    private const int P5ArmorPhaseHp = 260;
    private const int MinHp = 1;
    private const int MaxHp = 999999;

    [LocalSetting]
    [BoolOption]
    internal static bool falseKnightP5Hp = false;

    [LocalSetting]
    internal static int falseKnightArmorPhase1Hp = DefaultArmorPhaseHp;

    [LocalSetting]
    internal static int falseKnightArmorPhase2Hp = DefaultArmorPhaseHp;

    [LocalSetting]
    internal static int falseKnightArmorPhase3Hp = DefaultArmorPhaseHp;

    [LocalSetting]
    internal static int falseKnightArmorPhase1HpBeforeP5 = DefaultArmorPhaseHp;

    [LocalSetting]
    internal static int falseKnightArmorPhase2HpBeforeP5 = DefaultArmorPhaseHp;

    [LocalSetting]
    internal static int falseKnightArmorPhase3HpBeforeP5 = DefaultArmorPhaseHp;

    [LocalSetting]
    internal static bool falseKnightHasStoredStateBeforeP5 = false;

    private static readonly Dictionary<int, int> vanillaArmorHpByInstance = new();
    private static readonly Dictionary<int, int> vanillaRecoverHpByFsm = new();
    private static readonly Dictionary<int, int> trackedPhaseByArmorInstance = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        NormalizeP5State();
        NormalizeConfiguredHpState();
        vanillaArmorHpByInstance.Clear();
        vanillaRecoverHpByFsm.Clear();
        trackedPhaseByArmorInstance.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.HealthManager.Update += OnHealthManagerUpdate;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_FalseKnight;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_FalseKnight;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresent();
        moduleActive = false;
        On.HealthManager.Awake -= OnHealthManagerAwake;
        On.HealthManager.Start -= OnHealthManagerStart;
        On.HealthManager.Update -= OnHealthManagerUpdate;
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_FalseKnight;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_FalseKnight;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaArmorHpByInstance.Clear();
        vanillaRecoverHpByFsm.Clear();
        trackedPhaseByArmorInstance.Clear();
        hoGEntryAllowed = false;
    }

    internal static void ReapplyLiveSettings()
    {
        if (!moduleActive)
        {
            return;
        }

        NormalizeConfiguredHpState();
        ApplyFalseKnightSettingsIfPresent(forceReapply: true);
    }

    internal static void SetP5HpEnabled(bool value)
    {
        if (value)
        {
            if (!falseKnightP5Hp)
            {
                falseKnightArmorPhase1HpBeforeP5 = ClampHp(falseKnightArmorPhase1Hp);
                falseKnightArmorPhase2HpBeforeP5 = ClampHp(falseKnightArmorPhase2Hp);
                falseKnightArmorPhase3HpBeforeP5 = ClampHp(falseKnightArmorPhase3Hp);
                falseKnightHasStoredStateBeforeP5 = true;
            }

            falseKnightP5Hp = true;
            falseKnightArmorPhase1Hp = P5ArmorPhaseHp;
            falseKnightArmorPhase2Hp = P5ArmorPhaseHp;
            falseKnightArmorPhase3Hp = P5ArmorPhaseHp;
        }
        else
        {
            if (falseKnightP5Hp && falseKnightHasStoredStateBeforeP5)
            {
                falseKnightArmorPhase1Hp = ClampHp(falseKnightArmorPhase1HpBeforeP5);
                falseKnightArmorPhase2Hp = ClampHp(falseKnightArmorPhase2HpBeforeP5);
                falseKnightArmorPhase3Hp = ClampHp(falseKnightArmorPhase3HpBeforeP5);
            }

            falseKnightP5Hp = false;
            falseKnightHasStoredStateBeforeP5 = false;
        }

        NormalizeConfiguredHpState();
        ApplyFalseKnightSettingsIfPresent(forceReapply: true);
    }

    internal static void ApplyFalseKnightSettingsIfPresent(bool forceReapply = false)
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsArmor(hm) || !ShouldApplySettings(hm.gameObject))
            {
                continue;
            }

            ApplyPhaseSettings(hm, forceReapply);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsArmor(hm))
            {
                continue;
            }

            RestoreVanillaArmorHealth(hm);
            RestoreVanillaRecoverHp(hm.gameObject);
        }

        trackedPhaseByArmorInstance.Clear();
    }

    private static void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsArmor(self))
        {
            return;
        }

        RememberVanillaArmorHp(self);
        if (ShouldApplySettings(self.gameObject))
        {
            ApplyPhaseSettings(self, forceReapply: true);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsArmor(self))
        {
            return;
        }

        RememberVanillaArmorHp(self);
        if (ShouldApplySettings(self.gameObject))
        {
            ApplyPhaseSettings(self, forceReapply: true);
            _ = self.StartCoroutine(DeferredApply(self));
        }
    }

    private static void OnHealthManagerUpdate(On.HealthManager.orig_Update orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsArmor(self) || !ShouldApplySettings(self.gameObject))
        {
            return;
        }

        ApplyPhaseSettings(self, forceReapply: false);
    }

    private static void OnPlayMakerFsmOnEnable_FalseKnight(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsFalseKnightScene(self.gameObject.scene.name))
        {
            return;
        }

        if (!hoGEntryAllowed)
        {
            return;
        }

        if (IsRecoverFsm(self) || IsMainFsm(self))
        {
            ApplyFalseKnightSettingsIfPresent(forceReapply: true);
        }
    }

    private static void OnPlayMakerFsmStart_FalseKnight(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsFalseKnightScene(self.gameObject.scene.name))
        {
            return;
        }

        if (!hoGEntryAllowed)
        {
            return;
        }

        if (IsRecoverFsm(self) || IsMainFsm(self))
        {
            ApplyFalseKnightSettingsIfPresent(forceReapply: true);
        }
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsArmor(hm) || !ShouldApplySettings(hm.gameObject))
        {
            yield break;
        }

        ApplyPhaseSettings(hm, forceReapply: true);
        yield return new WaitForSeconds(0.01f);
        if (moduleActive && hm != null && hm.gameObject != null && IsArmor(hm) && ShouldApplySettings(hm.gameObject))
        {
            ApplyPhaseSettings(hm, forceReapply: true);
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!IsFalseKnightScene(to.name))
        {
            vanillaArmorHpByInstance.Clear();
            vanillaRecoverHpByFsm.Clear();
            trackedPhaseByArmorInstance.Clear();
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        ApplyFalseKnightSettingsIfPresent(forceReapply: true);
    }

    private static string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);
        return newSceneName;
    }

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (IsFalseKnightScene(nextScene))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (IsFalseKnightScene(currentScene) && hoGEntryAllowed)
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

    private static bool IsFalseKnightScene(string? sceneName)
    {
        return string.Equals(sceneName, FalseKnightScene, StringComparison.Ordinal);
    }

    private static bool IsArmor(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        GameObject gameObject = hm.gameObject;
        return IsFalseKnightScene(gameObject.scene.name)
            && gameObject.name.StartsWith(ArmorNamePrefix, StringComparison.Ordinal);
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsFalseKnightScene(gameObject.scene.name))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static int ResolvePhase(HealthManager armorHm)
    {
        PlayMakerFSM? mainFsm = FindMainFsm(armorHm.gameObject);
        mainFsm ??= FindMainFsmInScene(armorHm.gameObject.scene.name);
        if (mainFsm == null)
        {
            return 1;
        }

        return ResolvePhaseFromMainFsm(mainFsm);
    }

    private static int ResolvePhaseFromMainFsm(PlayMakerFSM mainFsm)
    {
        int phase = 1;
        int rages = mainFsm.FsmVariables.GetFsmInt(RageCountVariableName)?.Value ?? 0;
        if (rages >= 2)
        {
            phase = 3;
        }
        else if (rages >= 1)
        {
            phase = 2;
        }

        int stunnedAmount = mainFsm.FsmVariables.GetFsmInt(StunnedAmountVariableName)?.Value ?? 0;
        if (stunnedAmount >= 2)
        {
            phase = Math.Max(phase, 3);
        }
        else if (stunnedAmount >= 1)
        {
            phase = Math.Max(phase, 2);
        }

        string activeState = mainFsm.ActiveStateName ?? string.Empty;
        if (string.Equals(activeState, MainPhase3StateName, StringComparison.Ordinal)
            || string.Equals(activeState, MainOpened2StateName, StringComparison.Ordinal)
            || string.Equals(activeState, MainHit2StateName, StringComparison.Ordinal))
        {
            phase = 3;
        }
        else if (string.Equals(activeState, MainPhase2StateName, StringComparison.Ordinal))
        {
            phase = Math.Max(phase, 2);
        }

        return Mathf.Clamp(phase, 1, 3);
    }

    private static void ApplyPhaseSettings(HealthManager armorHm, bool forceReapply)
    {
        int armorInstanceId = armorHm.GetInstanceID();
        int phase = ResolvePhase(armorHm);
        bool phaseChanged = !trackedPhaseByArmorInstance.TryGetValue(armorInstanceId, out int trackedPhase) || trackedPhase != phase;
        if (!phaseChanged && !forceReapply)
        {
            return;
        }

        trackedPhaseByArmorInstance[armorInstanceId] = phase;

        int armorHp = GetArmorHpForPhase(phase);
        ApplyArmorHealth(armorHm, armorHp);
        ApplyRecoverHp(armorHm.gameObject, armorHp);
    }

    private static int GetArmorHpForPhase(int phase)
    {
        return phase switch
        {
            2 => ClampHp(falseKnightArmorPhase2Hp),
            3 => ClampHp(falseKnightArmorPhase3Hp),
            _ => ClampHp(falseKnightArmorPhase1Hp),
        };
    }

    private static void ApplyArmorHealth(HealthManager armorHm, int hp)
    {
        if (armorHm == null || armorHm.gameObject == null)
        {
            return;
        }

        RememberVanillaArmorHp(armorHm);
        int targetHp = ClampHp(hp);
        armorHm.gameObject.manageHealth(targetHp);
        armorHm.hp = targetHp;
        TrySetMaxHp(armorHm, targetHp);
    }

    private static void ApplyRecoverHp(GameObject armorObject, int hp)
    {
        if (armorObject == null)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in armorObject.GetComponents<PlayMakerFSM>())
        {
            if (fsm == null || !IsRecoverFsm(fsm))
            {
                continue;
            }

            RememberVanillaRecoverHp(fsm);
            int targetHp = ClampHp(hp);
            FsmInt? recoverHp = fsm.FsmVariables.GetFsmInt(RecoverVariableName);
            if (recoverHp != null)
            {
                recoverHp.Value = targetHp;
            }

            FsmState? stunState = fsm.Fsm?.GetState("Stun");
            if (stunState?.Actions == null)
            {
                continue;
            }

            foreach (FsmStateAction action in stunState.Actions)
            {
                if (action is SetHP setHp && setHp.hp != null)
                {
                    setHp.hp.Value = targetHp;
                }
            }
        }
    }

    private static void RememberVanillaArmorHp(HealthManager hm)
    {
        int instanceId = hm.GetInstanceID();
        if (vanillaArmorHpByInstance.ContainsKey(instanceId))
        {
            return;
        }

        int hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        if (hp > 0)
        {
            vanillaArmorHpByInstance[instanceId] = hp;
        }
    }

    private static void RememberVanillaRecoverHp(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaRecoverHpByFsm.ContainsKey(fsmId))
        {
            return;
        }

        int recoverHp = fsm.FsmVariables.GetFsmInt(RecoverVariableName)?.Value ?? DefaultArmorPhaseHp;
        if (recoverHp <= 0)
        {
            recoverHp = DefaultArmorPhaseHp;
        }

        vanillaRecoverHpByFsm[fsmId] = recoverHp;
    }

    private static void RestoreVanillaArmorHealth(HealthManager hm)
    {
        if (!vanillaArmorHpByInstance.TryGetValue(hm.GetInstanceID(), out int vanillaHp) || vanillaHp <= 0)
        {
            return;
        }

        int targetHp = ClampHp(vanillaHp);
        hm.gameObject.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void RestoreVanillaRecoverHp(GameObject armorObject)
    {
        if (armorObject == null)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in armorObject.GetComponents<PlayMakerFSM>())
        {
            if (fsm == null || !IsRecoverFsm(fsm))
            {
                continue;
            }

            int fsmId = fsm.GetInstanceID();
            if (!vanillaRecoverHpByFsm.TryGetValue(fsmId, out int vanillaRecoverHp) || vanillaRecoverHp <= 0)
            {
                continue;
            }

            FsmInt? recoverHp = fsm.FsmVariables.GetFsmInt(RecoverVariableName);
            if (recoverHp != null)
            {
                recoverHp.Value = vanillaRecoverHp;
            }

            FsmState? stunState = fsm.Fsm?.GetState("Stun");
            if (stunState?.Actions == null)
            {
                continue;
            }

            foreach (FsmStateAction action in stunState.Actions)
            {
                if (action is SetHP setHp && setHp.hp != null)
                {
                    setHp.hp.Value = vanillaRecoverHp;
                }
            }
        }
    }

    private static PlayMakerFSM? FindMainFsm(GameObject armorObject)
    {
        if (armorObject == null)
        {
            return null;
        }

        foreach (PlayMakerFSM fsm in armorObject.GetComponents<PlayMakerFSM>())
        {
            if (fsm != null && IsMainFsm(fsm))
            {
                return fsm;
            }
        }

        return null;
    }

    private static PlayMakerFSM? FindMainFsmInScene(string sceneName)
    {
        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !string.Equals(fsm.gameObject.scene.name, sceneName, StringComparison.Ordinal))
            {
                continue;
            }

            if (IsMainFsm(fsm))
            {
                return fsm;
            }
        }

        return null;
    }

    private static bool IsMainFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsFalseKnightScene(fsm.gameObject.scene.name))
        {
            return false;
        }

        return fsm.Fsm?.GetState(MainPhase2StateName) != null
            && fsm.Fsm?.GetState(MainPhase3StateName) != null;
    }

    private static bool IsRecoverFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsFalseKnightScene(fsm.gameObject.scene.name))
        {
            return false;
        }

        return fsm.Fsm?.GetState("Check") != null
            && fsm.Fsm?.GetState("Stun") != null
            && fsm.FsmVariables.GetFsmInt(RecoverVariableName) != null;
    }

    private static void NormalizeP5State()
    {
        NormalizeConfiguredHpState();

        if (!falseKnightP5Hp)
        {
            return;
        }

        if (!falseKnightHasStoredStateBeforeP5)
        {
            falseKnightArmorPhase1HpBeforeP5 = ClampHp(falseKnightArmorPhase1Hp);
            falseKnightArmorPhase2HpBeforeP5 = ClampHp(falseKnightArmorPhase2Hp);
            falseKnightArmorPhase3HpBeforeP5 = ClampHp(falseKnightArmorPhase3Hp);
            falseKnightHasStoredStateBeforeP5 = true;
        }

        falseKnightArmorPhase1Hp = P5ArmorPhaseHp;
        falseKnightArmorPhase2Hp = P5ArmorPhaseHp;
        falseKnightArmorPhase3Hp = P5ArmorPhaseHp;
    }

    private static void NormalizeConfiguredHpState()
    {
        falseKnightArmorPhase1Hp = ClampHp(falseKnightArmorPhase1Hp);
        falseKnightArmorPhase2Hp = ClampHp(falseKnightArmorPhase2Hp);
        falseKnightArmorPhase3Hp = ClampHp(falseKnightArmorPhase3Hp);
        falseKnightArmorPhase1HpBeforeP5 = ClampHp(falseKnightArmorPhase1HpBeforeP5);
        falseKnightArmorPhase2HpBeforeP5 = ClampHp(falseKnightArmorPhase2HpBeforeP5);
        falseKnightArmorPhase3HpBeforeP5 = ClampHp(falseKnightArmorPhase3HpBeforeP5);
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

    private static int ClampHp(int value)
    {
        if (value < MinHp)
        {
            return MinHp;
        }

        return value > MaxHp ? MaxHp : value;
    }
}
