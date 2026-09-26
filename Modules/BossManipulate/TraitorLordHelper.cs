using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class TraitorLordHelper : RageBossHelper
{
    private const string TraitorLordScene = "GG_Traitor_Lord";
    private const string TraitorLordName = "Mantis Traitor Lord";
    private const string TraitorLordPhaseFsmName = "Mantis";
    private const string TraitorLordPhaseCheckStateName = "Slam?";
    private const int DefaultTraitorLordMaxHp = 1300;
    private const int DefaultTraitorLordVanillaHp = 1300;
    private const int DefaultTraitorLordPhase2Hp = 500;
    private const int P5TraitorLordHp = 800;
    private const int MinTraitorLordHp = 1;
    private const int MaxTraitorLordHp = 999999;
    private const int MinTraitorLordPhase2Hp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool traitorLordUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool traitorLordP5Hp = false;

    [LocalSetting]
    internal static int traitorLordMaxHp = DefaultTraitorLordMaxHp;

    [LocalSetting]
    internal static int traitorLordMaxHpBeforeP5 = DefaultTraitorLordMaxHp;

    [LocalSetting]
    internal static bool traitorLordUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool traitorLordHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool traitorLordUseCustomPhase = false;

    [LocalSetting]
    internal static int traitorLordPhase2Hp = DefaultTraitorLordPhase2Hp;

    [LocalSetting]
    internal static bool traitorLordUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int traitorLordPhase2HpBeforeP5 = DefaultTraitorLordPhase2Hp;

    private static TraitorLordHelper? instance;

    public TraitorLordHelper() => instance = this;

    private protected override int P5Hp => P5TraitorLordHp;
    private protected override string SceneName => TraitorLordScene;
    private protected override string BossObjectName => TraitorLordName;
    private protected override string PhaseFsmName => TraitorLordPhaseFsmName;
    private protected override string PhaseCheckStateName => TraitorLordPhaseCheckStateName;
    private protected override int DefaultVanillaHp => DefaultTraitorLordVanillaHp;
    private protected override int MinHp => MinTraitorLordHp;
    private protected override int MaxHpLimit => MaxTraitorLordHp;
    private protected override int MinPhase2Hp => MinTraitorLordPhase2Hp;
    private protected override bool UseMaxHp { get => traitorLordUseMaxHp; set => traitorLordUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => traitorLordP5Hp; set => traitorLordP5Hp = value; }
    private protected override int MaxHp { get => traitorLordMaxHp; set => traitorLordMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => traitorLordMaxHpBeforeP5; set => traitorLordMaxHpBeforeP5 = value; }
    private protected override bool UseMaxHpBeforeP5 { get => traitorLordUseMaxHpBeforeP5; set => traitorLordUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => traitorLordHasStoredStateBeforeP5; set => traitorLordHasStoredStateBeforeP5 = value; }
    private protected override bool UseCustomPhase { get => traitorLordUseCustomPhase; set => traitorLordUseCustomPhase = value; }
    private protected override int Phase2Hp { get => traitorLordPhase2Hp; set => traitorLordPhase2Hp = value; }
    private protected override bool UseCustomPhaseBeforeP5 { get => traitorLordUseCustomPhaseBeforeP5; set => traitorLordUseCustomPhaseBeforeP5 = value; }
    private protected override int Phase2HpBeforeP5 { get => traitorLordPhase2HpBeforeP5; set => traitorLordPhase2HpBeforeP5 = value; }

    private protected override void Load()
    {
        moduleActive = true;
        BossManipulateEntryGuard.EnsureHooks();
        NormalizeP5State();
        NormalizePhaseThresholdState();
        vanillaHpByInstance.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
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
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_Boss;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_Boss;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
        hoGEntryAllowed = false;
    }

    private protected override bool IsBossPhaseControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsBossObject(fsm.gameObject))
        {
            return false;
        }

        if (string.Equals(fsm.FsmName, TraitorLordPhaseFsmName, StringComparison.Ordinal))
        {
            return true;
        }

        return fsm.Fsm?.GetState(TraitorLordPhaseCheckStateName) != null;
    }

    private protected override void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsBossPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplyPhaseSettings(fsm.gameObject))
        {
            return;
        }

        int threshold = ShouldUseCustomPhaseThreshold()
            ? ClampBossPhase2Hp(traitorLordPhase2Hp, ResolvePhase2MaxHp())
            : GetVanillaPhase2Hp();

        SetPhase2ThresholdOnFsm(fsm, threshold);
    }

    private void SetPhase2ThresholdOnFsm(PlayMakerFSM fsm, int value)
    {
        if (fsm == null)
        {
            return;
        }

        int threshold = Math.Max(MinTraitorLordPhase2Hp, value);
        FsmState? checkState = fsm.Fsm?.GetState(TraitorLordPhaseCheckStateName);
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

            FsmInt? thresholdOperand = ResolveThresholdOperand(compare);
            if (thresholdOperand == null)
            {
                continue;
            }

            thresholdOperand.UseVariable = false;
            thresholdOperand.Name = string.Empty;
            thresholdOperand.Value = threshold;
        }
    }

    private protected override int GetVanillaPhase2Hp()
    {
        return DefaultTraitorLordPhase2Hp;
    }

    private FsmInt? ResolveThresholdOperand(IntCompare compare)
    {
        if (compare == null)
        {
            return null;
        }

        FsmInt? integer1 = compare.integer1;
        FsmInt? integer2 = compare.integer2;
        if (integer1 == null && integer2 == null)
        {
            return null;
        }

        if (integer1 != null && string.Equals(integer1.Name, "HP", StringComparison.Ordinal))
        {
            return integer2 ?? integer1;
        }

        if (integer2 != null && string.Equals(integer2.Name, "HP", StringComparison.Ordinal))
        {
            return integer1 ?? integer2;
        }

        return integer2 ?? integer1;
    }

    private protected override void RestoreVanillaPhaseThresholdsIfPresentCore()
    {
        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsBossPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplyPhaseSettings(fsm.gameObject))
            {
                continue;
            }

            SetPhase2ThresholdOnFsm(fsm, GetVanillaPhase2Hp());
        }
    }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyTraitorLordHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyPhaseThresholdSettingsIfPresent() => instance?.ApplyPhaseThresholdSettingsIfPresentCore();

    internal static void RestoreVanillaPhaseThresholdsIfPresent() => instance?.RestoreVanillaPhaseThresholdsIfPresentCore();

    internal static int GetPhase2MaxHpForUi() => instance?.GetPhase2MaxHpForUiCore() ?? default;
}
