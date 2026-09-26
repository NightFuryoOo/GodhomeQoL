using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class XeroP2Helper : RageBossHelper
{
    private const string XeroScene = "GG_Ghost_Xero";
    private const string XeroName = "Ghost Warrior Xero";
    private const string XeroPhaseFsmName = "Sword Summon";
    private const string XeroPhaseCheckStateName = "Check";
    private const string XeroPhase2VariableName = "Half HP";
    private const string XeroAttackingFsmName = "Attacking";
    private const string XeroRageVariableName = "Rage";
    private const string XeroSummonEventName = "SUMMON";
    private const int DefaultXeroMaxHp = 650;
    private const int DefaultXeroVanillaHp = 650;
    private const int DefaultXeroPhase2Hp = 325;
    private const int MinXeroHp = 1;
    private const int MaxXeroHp = 999999;
    private const int MinXeroPhase2Hp = 1;

    [LocalSetting]
    [BoolOption]
    internal static bool xeroP2UseMaxHp = false;

    [LocalSetting]
    internal static int xeroP2MaxHp = DefaultXeroMaxHp;

    [LocalSetting]
    internal static bool xeroP2UseCustomPhase = false;

    [LocalSetting]
    internal static int xeroP2Phase2Hp = DefaultXeroPhase2Hp;

    private static XeroP2Helper? instance;

    public XeroP2Helper() => instance = this;

    private protected override string SceneName => XeroScene;
    private protected override string BossObjectName => XeroName;
    private protected override string PhaseFsmName => XeroPhaseFsmName;
    private protected override string PhaseCheckStateName => XeroPhaseCheckStateName;
    private protected override string Phase2VariableName => XeroPhase2VariableName;
    private protected override string AttackingFsmName => XeroAttackingFsmName;
    private protected override string RageVariableName => XeroRageVariableName;
    private protected override string SummonEventName => XeroSummonEventName;
    private protected override int DefaultVanillaHp => DefaultXeroVanillaHp;
    private protected override int MinHp => MinXeroHp;
    private protected override int MaxHpLimit => MaxXeroHp;
    private protected override int MinPhase2Hp => MinXeroPhase2Hp;
    private protected override bool UseMaxHp { get => xeroP2UseMaxHp; set => xeroP2UseMaxHp = value; }
    private protected override int MaxHp { get => xeroP2MaxHp; set => xeroP2MaxHp = value; }
    private protected override bool UseCustomPhase { get => xeroP2UseCustomPhase; set => xeroP2UseCustomPhase = value; }
    private protected override int Phase2Hp { get => xeroP2Phase2Hp; set => xeroP2Phase2Hp = value; }

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

    private protected override bool ShouldUseCustomPhaseThreshold() => xeroP2UseCustomPhase;

    private protected override bool IsBossPhaseControlFsm(PlayMakerFSM fsm)
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

    private protected override bool ShouldApplyPhaseSettings(GameObject? gameObject)
    {
        return ShouldApplySettings(gameObject);
    }

    private protected override void SetPhase2ThresholdOnFsm(PlayMakerFSM fsm, int value, bool useVanillaVariable)
    {
        if (fsm == null)
        {
            return;
        }

        int threshold = ClampBossPhase2Hp(value, ResolvePhase2MaxHp());
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

    private protected override int GetPhase2MaxHpForUiCore() => ResolvePhase2MaxHp();

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void ApplyXeroHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyPhaseThresholdSettingsIfPresent() => instance?.ApplyPhaseThresholdSettingsIfPresentCore();

    internal static void RestoreVanillaPhaseThresholdsIfPresent() => instance?.RestoreVanillaPhaseThresholdsIfPresentCore();

    internal static int GetPhase2MaxHpForUi() => instance?.GetPhase2MaxHpForUiCore() ?? default;
}
