using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class MarkothP4Helper : RageBossHelper
{
    private const string MarkothScene = "GG_Ghost_Markoth";
    private const string MarkothName = "Ghost Warrior Markoth";
    private const string MarkothPhaseFsmName = "Rage Check";
    private const string MarkothPhaseCheckStateName = "Check";
    private const string MarkothPhase2VariableName = "Rage HP";
    private const string MarkothAttackingFsmName = "Attacking";
    private const string MarkothShieldAttackFsmName = "Shield Attack";
    private const string MarkothRageVariableName = "Rage";
    private const string MarkothRageEventName = "RAGE";
    private const int DefaultMarkothMaxHp = 650;
    private const int DefaultMarkothVanillaHp = 650;
    private const int DefaultMarkothPhase2Hp = 325;
    private const int MinMarkothHp = 1;
    private const int MaxMarkothHp = 999999;
    private const int MinMarkothPhase2Hp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool markothP4UseMaxHp = false;

    [LocalSetting]
    internal static int markothP4MaxHp = DefaultMarkothMaxHp;

    [LocalSetting]
    internal static bool markothP4UseCustomPhase = false;

    [LocalSetting]
    internal static int markothP4Phase2Hp = DefaultMarkothPhase2Hp;

    private static MarkothP4Helper? instance;

    public MarkothP4Helper() => instance = this;

    private protected override string SceneName => MarkothScene;
    private protected override string BossObjectName => MarkothName;
    private protected override string PhaseFsmName => MarkothPhaseFsmName;
    private protected override string PhaseCheckStateName => MarkothPhaseCheckStateName;
    private protected override string Phase2VariableName => MarkothPhase2VariableName;
    private protected override string AttackingFsmName => MarkothAttackingFsmName;
    private protected override string RageVariableName => MarkothRageVariableName;
    private protected override int DefaultVanillaHp => DefaultMarkothVanillaHp;
    private protected override int MinHp => MinMarkothHp;
    private protected override int MaxHpLimit => MaxMarkothHp;
    private protected override int MinPhase2Hp => MinMarkothPhase2Hp;
    private protected override bool UseMaxHp { get => markothP4UseMaxHp; set => markothP4UseMaxHp = value; }
    private protected override int MaxHp { get => markothP4MaxHp; set => markothP4MaxHp = value; }
    private protected override bool UseCustomPhase { get => markothP4UseCustomPhase; set => markothP4UseCustomPhase = value; }
    private protected override int Phase2Hp { get => markothP4Phase2Hp; set => markothP4Phase2Hp = value; }

    private protected override void Load()
    {
        moduleActive = true;
        BossManipulateEntryGuard.EnsureHooks();
        NormalizePhaseThresholdState();
        vanillaHpByInstance.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.HealthManager.Update += OnHealthManagerUpdate;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_Boss;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_Boss;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override bool ShouldUseCustomPhaseThreshold() => markothP4UseCustomPhase;

    private protected override bool ShouldApplyPhaseSettings(GameObject? gameObject)
    {
        return ShouldApplySettings(gameObject);
    }

    private protected override void TryForceBossRage(GameObject bossObject)
    {
        if (bossObject == null || !ShouldApplyPhaseSettings(bossObject))
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsBossObject(fsm.gameObject))
            {
                continue;
            }

            if (!string.Equals(fsm.FsmName, MarkothAttackingFsmName, StringComparison.Ordinal)
                && !string.Equals(fsm.FsmName, MarkothShieldAttackFsmName, StringComparison.Ordinal))
            {
                continue;
            }

            FsmBool? rageFlag = fsm.FsmVariables.GetFsmBool(MarkothRageVariableName);
            if (rageFlag == null || rageFlag.Value)
            {
                continue;
            }

            rageFlag.Value = true;
            fsm.SendEvent(MarkothRageEventName);
        }
    }

    private protected override int GetPhase2MaxHpForUiCore() => ResolvePhase2MaxHp();

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void ApplyMarkothHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyPhaseThresholdSettingsIfPresent() => instance?.ApplyPhaseThresholdSettingsIfPresentCore();

    internal static void RestoreVanillaPhaseThresholdsIfPresent() => instance?.RestoreVanillaPhaseThresholdsIfPresentCore();

    internal static int GetPhase2MaxHpForUi() => instance?.GetPhase2MaxHpForUiCore() ?? default;
}
