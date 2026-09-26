using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class GorbP1Helper : PhaseThresholdBossHelper
{
    private const string GorbScene = "GG_Ghost_Gorb";
    private const string GorbName = "Ghost Warrior Slug";
    private const string GorbPhaseFsmName = "Attacking";
    private const string GorbPhaseCheckState1Name = "Double?";
    private const string GorbPhaseCheckState2Name = "Triple?";
    private const string GorbPhase2TransitionStateName = "Double Pause";
    private const string GorbPhase3TransitionStateName = "Anim";
    private const string GorbPhase2VariableName = "Double HP";
    private const string GorbPhase3VariableName = "Triple HP";
    private const int DefaultGorbMaxHp = 650;
    private const int DefaultGorbVanillaHp = 650;
    private const int DefaultGorbPhase2Hp = 455;
    private const int DefaultGorbPhase3Hp = 260;
    private const int MinGorbHp = 1;
    private const int MaxGorbHp = 999999;
    private const int MinGorbPhase2Hp = 2;
    private const int MinGorbPhase3Hp = 1;
    private const int GorbPhaseThresholdReapplyAttempts = 8;
    private const float GorbPhaseThresholdReapplyInterval = 0.15f;

    [LocalSetting]
    [BoolOption]
    internal static bool gorbP1UseMaxHp = false;

    [LocalSetting]
    internal static int gorbP1MaxHp = DefaultGorbMaxHp;

    [LocalSetting]
    internal static bool gorbP1UseCustomPhase = false;

    [LocalSetting]
    internal static int gorbP1Phase2Hp = DefaultGorbPhase2Hp;

    [LocalSetting]
    internal static int gorbP1Phase3Hp = DefaultGorbPhase3Hp;

    private static GorbP1Helper? instance;

    public GorbP1Helper() => instance = this;

    private protected override int PhaseThresholdReapplyAttempts => GorbPhaseThresholdReapplyAttempts;
    private protected override float PhaseThresholdReapplyInterval => GorbPhaseThresholdReapplyInterval;
    private protected override string SceneName => GorbScene;
    private protected override string BossObjectName => GorbName;
    private protected override string PhaseFsmName => GorbPhaseFsmName;
    private protected override string PhaseCheckState1Name => GorbPhaseCheckState1Name;
    private protected override string PhaseCheckState2Name => GorbPhaseCheckState2Name;
    private protected override string Phase2VariableName => GorbPhase2VariableName;
    private protected override string Phase3VariableName => GorbPhase3VariableName;
    private protected override int DefaultPhase2Hp => DefaultGorbPhase2Hp;
    private protected override int DefaultPhase3Hp => DefaultGorbPhase3Hp;
    private protected override int DefaultVanillaHp => DefaultGorbVanillaHp;
    private protected override int MinHp => MinGorbHp;
    private protected override int MaxHpLimit => MaxGorbHp;
    private protected override int MinPhase2Hp => MinGorbPhase2Hp;
    private protected override int MinPhase3Hp => MinGorbPhase3Hp;
    private protected override string Phase2TransitionStateName => GorbPhase2TransitionStateName;
    private protected override string Phase3TransitionStateName => GorbPhase3TransitionStateName;
    private protected override bool UseMaxHp { get => gorbP1UseMaxHp; set => gorbP1UseMaxHp = value; }
    private protected override int MaxHp { get => gorbP1MaxHp; set => gorbP1MaxHp = value; }
    private protected override bool UseCustomPhase { get => gorbP1UseCustomPhase; set => gorbP1UseCustomPhase = value; }
    private protected override int Phase2Hp { get => gorbP1Phase2Hp; set => gorbP1Phase2Hp = value; }
    private protected override int Phase3Hp { get => gorbP1Phase3Hp; set => gorbP1Phase3Hp = value; }

    private protected override void Load()
    {
        moduleActive = true;
        BossManipulateEntryGuard.EnsureHooks();
        NormalizePhaseThresholdState();
        vanillaHpByInstance.Clear();
        vanillaPhaseThresholdsByFsm.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.HealthManager.Update += OnHealthManagerUpdate;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_Boss;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_Boss;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override bool ShouldUseCustomPhaseThresholds() => gorbP1UseCustomPhase;

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void ApplyGorbHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyPhaseThresholdSettingsIfPresent() => instance?.ApplyPhaseThresholdSettingsIfPresentCore();

    internal static void RestoreVanillaPhaseThresholdsIfPresent() => instance?.RestoreVanillaPhaseThresholdsIfPresentCore();

    internal static int GetPhase2MaxHpForUi() => instance?.GetPhase2MaxHpForUiCore() ?? default;
}
