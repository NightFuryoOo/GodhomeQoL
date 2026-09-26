using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class WhiteDefenderHelper : RageBossHelper
{
    private const string WhiteDefenderScene = "GG_White_Defender";
    private const string WhiteDefenderName = "White Defender";
    private const string WhiteDefenderPhaseFsmName = "Dung Defender";
    private const string WhiteDefenderPhaseCheckStateName = "Rage?";
    private const string WhiteDefenderPhase2VariableName = "Rage HP";
    private const int DefaultWhiteDefenderMaxHp = 1600;
    private const int DefaultWhiteDefenderVanillaHp = 1600;
    private const int DefaultWhiteDefenderPhase2Hp = 600;
    private const int P5WhiteDefenderHp = 1600;
    private const int MinWhiteDefenderHp = 1;
    private const int MaxWhiteDefenderHp = 999999;
    private const int MinWhiteDefenderPhase2Hp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool whiteDefenderUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool whiteDefenderP5Hp = false;

    [LocalSetting]
    internal static int whiteDefenderMaxHp = DefaultWhiteDefenderMaxHp;

    [LocalSetting]
    internal static int whiteDefenderMaxHpBeforeP5 = DefaultWhiteDefenderMaxHp;

    [LocalSetting]
    internal static bool whiteDefenderUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool whiteDefenderHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool whiteDefenderUseCustomPhase = false;

    [LocalSetting]
    internal static int whiteDefenderPhase2Hp = DefaultWhiteDefenderPhase2Hp;

    [LocalSetting]
    internal static bool whiteDefenderUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int whiteDefenderPhase2HpBeforeP5 = DefaultWhiteDefenderPhase2Hp;

    private static WhiteDefenderHelper? instance;

    public WhiteDefenderHelper() => instance = this;

    private protected override int P5Hp => P5WhiteDefenderHp;
    private protected override string SceneName => WhiteDefenderScene;
    private protected override string BossObjectName => WhiteDefenderName;
    private protected override string PhaseFsmName => WhiteDefenderPhaseFsmName;
    private protected override string PhaseCheckStateName => WhiteDefenderPhaseCheckStateName;
    private protected override string Phase2VariableName => WhiteDefenderPhase2VariableName;
    private protected override int DefaultVanillaHp => DefaultWhiteDefenderVanillaHp;
    private protected override int MinHp => MinWhiteDefenderHp;
    private protected override int MaxHpLimit => MaxWhiteDefenderHp;
    private protected override int MinPhase2Hp => MinWhiteDefenderPhase2Hp;
    private protected override bool UseMaxHp { get => whiteDefenderUseMaxHp; set => whiteDefenderUseMaxHp = value; }
    private protected override bool P5HpEnabled { get => whiteDefenderP5Hp; set => whiteDefenderP5Hp = value; }
    private protected override int MaxHp { get => whiteDefenderMaxHp; set => whiteDefenderMaxHp = value; }
    private protected override int MaxHpBeforeP5 { get => whiteDefenderMaxHpBeforeP5; set => whiteDefenderMaxHpBeforeP5 = value; }
    private protected override bool UseMaxHpBeforeP5 { get => whiteDefenderUseMaxHpBeforeP5; set => whiteDefenderUseMaxHpBeforeP5 = value; }
    private protected override bool HasStoredStateBeforeP5 { get => whiteDefenderHasStoredStateBeforeP5; set => whiteDefenderHasStoredStateBeforeP5 = value; }
    private protected override bool UseCustomPhase { get => whiteDefenderUseCustomPhase; set => whiteDefenderUseCustomPhase = value; }
    private protected override int Phase2Hp { get => whiteDefenderPhase2Hp; set => whiteDefenderPhase2Hp = value; }
    private protected override bool UseCustomPhaseBeforeP5 { get => whiteDefenderUseCustomPhaseBeforeP5; set => whiteDefenderUseCustomPhaseBeforeP5 = value; }
    private protected override int Phase2HpBeforeP5 { get => whiteDefenderPhase2HpBeforeP5; set => whiteDefenderPhase2HpBeforeP5 = value; }

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

    private protected override void SetPhase2ThresholdOnFsm(PlayMakerFSM fsm, int value, bool useVanillaVariable)
    {
        if (fsm == null)
        {
            return;
        }

        int threshold = useVanillaVariable
            ? Math.Max(MinWhiteDefenderPhase2Hp, value)
            : ClampBossPhase2Hp(value, ResolvePhase2MaxHp());

        FsmInt? rageHpVariable = fsm.FsmVariables.GetFsmInt(WhiteDefenderPhase2VariableName);
        if (rageHpVariable != null)
        {
            rageHpVariable.Value = threshold;
        }

        FsmState? checkState = fsm.Fsm?.GetState(WhiteDefenderPhaseCheckStateName);
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

            FsmInt? thresholdOperand = ResolveThresholdOperand(compare, WhiteDefenderPhase2VariableName);
            if (thresholdOperand == null)
            {
                continue;
            }

            if (useVanillaVariable)
            {
                thresholdOperand.UseVariable = true;
                thresholdOperand.Name = WhiteDefenderPhase2VariableName;
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
        return DefaultWhiteDefenderPhase2Hp;
    }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyWhiteDefenderHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyPhaseThresholdSettingsIfPresent() => instance?.ApplyPhaseThresholdSettingsIfPresentCore();

    internal static void RestoreVanillaPhaseThresholdsIfPresent() => instance?.RestoreVanillaPhaseThresholdsIfPresentCore();

    internal static int GetPhase2MaxHpForUi() => instance?.GetPhase2MaxHpForUiCore() ?? default;
}
