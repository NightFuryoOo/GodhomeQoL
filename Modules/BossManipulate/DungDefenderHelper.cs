using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class DungDefenderHelper : RageBossHelper
{
    private const string DungDefenderScene = "GG_Dung_Defender";
    private const string DungDefenderName = "Dung Defender";
    private const string DungDefenderPhaseFsmName = "Dung Defender";
    private const string DungDefenderPhaseCheckStateName = "Rage?";
    private const string DungDefenderPhase2VariableName = "Rage HP";
    private const string DungDefenderRagedVariableName = "Raged";
    private const string DungDefenderRageEventName = "RAGE";
    private const int DefaultDungDefenderMaxHp = 1100;
    private const int DefaultDungDefenderVanillaHp = 1100;
    private const int DefaultDungDefenderPhase2Hp = 350;
    private const int P5DungDefenderHp = 800;
    private const int MinDungDefenderHp = 1;
    private const int MaxDungDefenderHp = 999999;
    private const int MinDungDefenderPhase2Hp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool dungDefenderUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool dungDefenderP5Hp = false;

    [LocalSetting]
    internal static int dungDefenderMaxHp = DefaultDungDefenderMaxHp;

    [LocalSetting]
    internal static int dungDefenderMaxHpBeforeP5 = DefaultDungDefenderMaxHp;

    [LocalSetting]
    internal static bool dungDefenderUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool dungDefenderHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool dungDefenderUseCustomPhase = false;

    [LocalSetting]
    internal static int dungDefenderPhase2Hp = DefaultDungDefenderPhase2Hp;

    [LocalSetting]
    internal static bool dungDefenderUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int dungDefenderPhase2HpBeforeP5 = DefaultDungDefenderPhase2Hp;

    private static DungDefenderHelper? instance;

    public DungDefenderHelper() => instance = this;

    private protected override int P5Hp => P5DungDefenderHp;
    private protected override string SceneName => DungDefenderScene;
    private protected override string BossObjectName => DungDefenderName;
    private protected override string PhaseFsmName => DungDefenderPhaseFsmName;
    private protected override string PhaseCheckStateName => DungDefenderPhaseCheckStateName;
    private protected override string Phase2VariableName => DungDefenderPhase2VariableName;
    private protected override int DefaultVanillaHp => DefaultDungDefenderVanillaHp;
    private protected override int MinHp => MinDungDefenderHp;
    private protected override int MaxHpLimit => MaxDungDefenderHp;
    private protected override int MinPhase2Hp => MinDungDefenderPhase2Hp;
    private protected override bool UseMaxHp { get => dungDefenderUseMaxHp; set => dungDefenderUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => dungDefenderP5Hp; set => dungDefenderP5Hp = value; }
    private protected override int MaxHp { get => dungDefenderMaxHp; set => dungDefenderMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => dungDefenderMaxHpBeforeP5; set => dungDefenderMaxHpBeforeP5 = value; }
    private protected override bool UseMaxHpBeforeP5 { get => dungDefenderUseMaxHpBeforeP5; set => dungDefenderUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => dungDefenderHasStoredStateBeforeP5; set => dungDefenderHasStoredStateBeforeP5 = value; }
    private protected override bool UseCustomPhase { get => dungDefenderUseCustomPhase; set => dungDefenderUseCustomPhase = value; }
    private protected override int Phase2Hp { get => dungDefenderPhase2Hp; set => dungDefenderPhase2Hp = value; }
    private protected override bool UseCustomPhaseBeforeP5 { get => dungDefenderUseCustomPhaseBeforeP5; set => dungDefenderUseCustomPhaseBeforeP5 = value; }
    private protected override int Phase2HpBeforeP5 { get => dungDefenderPhase2HpBeforeP5; set => dungDefenderPhase2HpBeforeP5 = value; }

    private protected override void Load()
    {
        moduleActive = true;
        BossManipulateEntryGuard.EnsureHooks();
        NormalizeP5State();
        NormalizePhaseThresholdState();
        vanillaHpByInstance.Clear();
        forcedCustomRageByInstance.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.HealthManager.Update += OnHealthManagerUpdate;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_Boss;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_Boss;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresentCore();
        RestoreVanillaPhaseThresholdsIfPresentCore();
        moduleActive = false;
        On.HealthManager.Awake -= OnHealthManagerAwake;
        On.HealthManager.Start -= OnHealthManagerStart;
        On.HealthManager.Update -= OnHealthManagerUpdate;
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_Boss;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_Boss;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
        forcedCustomRageByInstance.Clear();
        hoGEntryAllowed = false;
    }

    private protected override void OnHealthManagerUpdate(On.HealthManager.orig_Update orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsBoss(self))
        {
            return;
        }

        if (!ShouldApplySettings(self.gameObject) || !ShouldUseCustomPhaseThreshold())
        {
            return;
        }

        int threshold = ClampBossPhase2Hp(dungDefenderPhase2Hp, ResolvePhase2MaxHp());
        if (self.hp > threshold)
        {
            return;
        }

        int instanceId = self.GetInstanceID();
        if (forcedCustomRageByInstance.Contains(instanceId))
        {
            return;
        }

        if (TryForceDungDefenderRageFromCheckState(self.gameObject))
        {
            forcedCustomRageByInstance.Add(instanceId);
        }
    }

    private protected override void OnPlayMakerFsmStart_Boss(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsBossPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
        _ = self.StartCoroutine(DeferredApplyPhaseThresholds(self));
    }

    private IEnumerator DeferredApplyPhaseThresholds(PlayMakerFSM fsm)
    {
        yield return null;

        if (!moduleActive || fsm == null || fsm.gameObject == null || !IsBossPhaseControlFsm(fsm))
        {
            yield break;
        }

        ApplyPhaseThresholdSettings(fsm);

        yield return new WaitForSeconds(0.01f);
        if (moduleActive && fsm != null && fsm.gameObject != null && IsBossPhaseControlFsm(fsm))
        {
            ApplyPhaseThresholdSettings(fsm);
        }
    }

    private protected override void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, DungDefenderScene, StringComparison.Ordinal))
        {
            vanillaHpByInstance.Clear();
            forcedCustomRageByInstance.Clear();
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyHealthIfPresentCore();
        }
        else
        {
            RestoreVanillaHealthIfPresentCore();
        }

        ApplyPhaseThresholdSettingsIfPresentCore();
    }

    private protected override void SetPhase2ThresholdOnFsm(PlayMakerFSM fsm, int value, bool useVanillaVariable)
    {
        if (fsm == null)
        {
            return;
        }

        int threshold = useVanillaVariable
            ? Math.Max(MinDungDefenderPhase2Hp, value)
            : ClampBossPhase2Hp(value, ResolvePhase2MaxHp());

        FsmInt? rageHpVariable = fsm.FsmVariables.GetFsmInt(DungDefenderPhase2VariableName);
        if (rageHpVariable != null)
        {
            rageHpVariable.Value = threshold;
        }

        FsmState? checkState = fsm.Fsm?.GetState(DungDefenderPhaseCheckStateName);
        if (checkState?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in checkState.Actions)
        {
            if (action is not IntCompare compare)
            {
                continue;
            }

            FsmInt? thresholdOperand = ResolveThresholdOperand(compare, DungDefenderPhase2VariableName);
            if (thresholdOperand == null)
            {
                continue;
            }

            if (useVanillaVariable)
            {
                thresholdOperand.UseVariable = true;
                thresholdOperand.Name = DungDefenderPhase2VariableName;
            }
            else
            {
                thresholdOperand.UseVariable = false;
                thresholdOperand.Name = string.Empty;
                thresholdOperand.Value = threshold;
            }
        }
    }

    private protected override int GetVanillaPhase2Hp()
    {
        return DefaultDungDefenderPhase2Hp;
    }

    private bool TryForceDungDefenderRageFromCheckState(GameObject bossObject)
    {
        if (bossObject == null || !ShouldApplyPhaseSettings(bossObject))
        {
            return false;
        }

        foreach (PlayMakerFSM fsm in bossObject.GetComponents<PlayMakerFSM>())
        {
            if (fsm == null || !IsBossPhaseControlFsm(fsm))
            {
                continue;
            }

            ApplyPhaseThresholdSettings(fsm);

            FsmBool? ragedFlag = fsm.FsmVariables.GetFsmBool(DungDefenderRagedVariableName);
            if (ragedFlag != null && ragedFlag.Value)
            {
                return true;
            }

            if (!string.Equals(fsm.ActiveStateName, DungDefenderPhaseCheckStateName, StringComparison.Ordinal))
            {
                continue;
            }

            fsm.SendEvent(DungDefenderRageEventName);
            return true;
        }

        return false;
    }

    private protected override void ReapplyLiveSettingsCore()
    {
        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyHealthIfPresentCore();
        }
        else
        {
            RestoreVanillaHealthIfPresentCore();
        }

        if (!ShouldUseCustomPhaseThreshold())
        {
            forcedCustomRageByInstance.Clear();
        }

        ApplyPhaseThresholdSettingsIfPresentCore();
    }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyDungDefenderHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyPhaseThresholdSettingsIfPresent() => instance?.ApplyPhaseThresholdSettingsIfPresentCore();

    internal static void RestoreVanillaPhaseThresholdsIfPresent() => instance?.RestoreVanillaPhaseThresholdsIfPresentCore();

    internal static int GetPhase2MaxHpForUi() => instance?.GetPhase2MaxHpForUiCore() ?? default;
}
