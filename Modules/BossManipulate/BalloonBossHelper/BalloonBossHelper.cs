using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class BalloonBossHelper : Module
{
    private protected const string BossSpawnStateName = "Spawn";
    private protected const string BossCheckState1Name = "Check";
    private protected const string BossCheckState2Name = "Check 2";
    private protected const string BossCheckState3Name = "Check 3";
    private protected const string BossHpVariableName = "HP";
    private protected const string BossToken1VariableName = "Token 1";
    private protected const string BossToken2VariableName = "Token 2";
    private protected const string BossToken3VariableName = "Token 3";
    private protected const int MinBossHp = 1;
    private protected const int MaxBossHp = 999999;
    private static readonly string[] BossSummonNameHints = { "Bursting", "Bouncer", "Balloon", "Zombie" };

    private readonly Dictionary<int, int> vanillaHpByInstance = new();
    private readonly Dictionary<int, int> vanillaSummonHpByInstance = new();
    private readonly Dictionary<int, int> vanillaSummonLimitByFsm = new();
    private readonly Dictionary<int, (int phase3Hp, int phase4Hp, int phase5Hp)> vanillaShakeThresholdsByFsm = new();
    private readonly Dictionary<int, int> vanillaSpawnThresholdByFsm = new();
    private bool moduleActive;
    private bool hoGEntryAllowed;

    private protected abstract string SceneName { get; }
    private protected abstract string BossObjectName { get; }
    private protected abstract int DefaultVanillaHp { get; }
    private protected abstract int DefaultVanillaSummonLimit { get; }
    private protected abstract int DefaultPhase2Hp { get; }
    private protected abstract int DefaultPhase3Hp { get; }
    private protected abstract int DefaultPhase4Hp { get; }
    private protected abstract int DefaultPhase5Hp { get; }
    private protected abstract int P5Hp { get; }
    private protected abstract bool UseMaxHp { get; set; }
    private protected abstract bool P5HpEnabled { get; set; }
    private protected abstract int MaxHp { get; set; }
    private protected abstract int MaxHpBeforeP5 { get; set; }
    private protected abstract bool UseCustomSummonHp { get; set; }
    private protected abstract int SummonHp { get; set; }
    private protected abstract bool UseCustomSummonLimit { get; set; }
    private protected abstract int SummonLimit { get; set; }
    private protected abstract bool UseCustomPhase { get; set; }
    private protected abstract int Phase2Hp { get; set; }
    private protected abstract int Phase3Hp { get; set; }
    private protected abstract int Phase4Hp { get; set; }
    private protected abstract int Phase5Hp { get; set; }
    private protected abstract bool UseMaxHpBeforeP5 { get; set; }
    private protected abstract bool HasStoredStateBeforeP5 { get; set; }
    private protected abstract bool UseCustomSummonHpBeforeP5 { get; set; }
    private protected abstract int SummonHpBeforeP5 { get; set; }
    private protected abstract bool UseCustomSummonLimitBeforeP5 { get; set; }
    private protected abstract int SummonLimitBeforeP5 { get; set; }
    private protected abstract bool UseCustomPhaseBeforeP5 { get; set; }
    private protected abstract int Phase2HpBeforeP5 { get; set; }
    private protected abstract int Phase3HpBeforeP5 { get; set; }
    private protected abstract int Phase4HpBeforeP5 { get; set; }
    private protected abstract int Phase5HpBeforeP5 { get; set; }

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    internal void ReapplyLiveSettingsCore()
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

        if (ShouldUseCustomSummonHp())
        {
            ApplySummonHealthIfPresentCore();
        }
        else
        {
            RestoreVanillaSummonHealthIfPresentCore();
        }

        if (ShouldUseCustomSummonLimit())
        {
            ApplySummonLimitSettingsIfPresentCore();
            EnforceCustomSummonLimitIfPresentCore();
        }
        else
        {
            RestoreVanillaSummonLimitsIfPresentCore();
        }

        ApplyPhaseThresholdSettingsIfPresentCore();
    }

    internal void SetP5HpEnabledCore(bool value)
    {
        if (value)
        {
            if (!P5HpEnabled)
            {
                MaxHpBeforeP5 = ClampBossHp(MaxHp);
                UseMaxHpBeforeP5 = UseMaxHp;
                UseCustomSummonHpBeforeP5 = UseCustomSummonHp;
                SummonHpBeforeP5 = ClampBossHp(SummonHp);
                UseCustomSummonLimitBeforeP5 = UseCustomSummonLimit;
                SummonLimitBeforeP5 = ClampBossSummonLimit(SummonLimit);
                UseCustomPhaseBeforeP5 = UseCustomPhase;
                Phase2HpBeforeP5 = ClampBossPhase2Hp(Phase2Hp, ResolvePhase2MaxHp());
                Phase3HpBeforeP5 = ClampBossPhase3Hp(Phase3Hp, Phase2HpBeforeP5);
                Phase4HpBeforeP5 = ClampBossPhase4Hp(Phase4Hp, Phase3HpBeforeP5);
                Phase5HpBeforeP5 = ClampBossPhase5Hp(Phase5Hp, Phase4HpBeforeP5);
                HasStoredStateBeforeP5 = true;
            }

            P5HpEnabled = true;
            UseMaxHp = true;
            UseCustomSummonHp = false;
            UseCustomSummonLimit = false;
            UseCustomPhase = false;
            MaxHp = P5Hp;
        }
        else
        {
            if (P5HpEnabled && HasStoredStateBeforeP5)
            {
                MaxHp = ClampBossHp(MaxHpBeforeP5);
                UseMaxHp = UseMaxHpBeforeP5;
                UseCustomSummonHp = UseCustomSummonHpBeforeP5;
                SummonHp = ClampBossHp(SummonHpBeforeP5);
                UseCustomSummonLimit = UseCustomSummonLimitBeforeP5;
                SummonLimit = ClampBossSummonLimit(SummonLimitBeforeP5);
                UseCustomPhase = UseCustomPhaseBeforeP5;
                Phase2Hp = ClampBossPhase2Hp(Phase2HpBeforeP5, ResolvePhase2MaxHp());
                Phase3Hp = ClampBossPhase3Hp(Phase3HpBeforeP5, Phase2Hp);
                Phase4Hp = ClampBossPhase4Hp(Phase4HpBeforeP5, Phase3Hp);
                Phase5Hp = ClampBossPhase5Hp(Phase5HpBeforeP5, Phase4Hp);
            }

            P5HpEnabled = false;
            HasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettingsCore();
    }

    private void NormalizeP5State()
    {
        if (!P5HpEnabled)
        {
            return;
        }

        if (!HasStoredStateBeforeP5)
        {
            MaxHpBeforeP5 = ClampBossHp(MaxHp);
            UseMaxHpBeforeP5 = UseMaxHp;
            UseCustomSummonHpBeforeP5 = UseCustomSummonHp;
            SummonHpBeforeP5 = ClampBossHp(SummonHp);
            UseCustomSummonLimitBeforeP5 = UseCustomSummonLimit;
            SummonLimitBeforeP5 = ClampBossSummonLimit(SummonLimit);
            UseCustomPhaseBeforeP5 = UseCustomPhase;
            Phase2HpBeforeP5 = ClampBossPhase2Hp(Phase2Hp, ResolvePhase2MaxHp());
            Phase3HpBeforeP5 = ClampBossPhase3Hp(Phase3Hp, Phase2HpBeforeP5);
            Phase4HpBeforeP5 = ClampBossPhase4Hp(Phase4Hp, Phase3HpBeforeP5);
            Phase5HpBeforeP5 = ClampBossPhase5Hp(Phase5Hp, Phase4HpBeforeP5);
            HasStoredStateBeforeP5 = true;
        }

        UseMaxHp = true;
        UseCustomSummonHp = false;
        UseCustomSummonLimit = false;
        UseCustomPhase = false;
        MaxHp = P5Hp;
        NormalizePhaseThresholdState();
    }

    internal void ApplyHealthIfPresentCore()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindBossHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyBossHealth(hm.gameObject, hm);
        }
    }

    private (int phase3Hp, int phase4Hp, int phase5Hp) GetVanillaShakeThresholds(PlayMakerFSM fsm)
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

        return (DefaultPhase3Hp, DefaultPhase4Hp, DefaultPhase5Hp);
    }
}
