using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class PhaseThresholdBossHelper : Module
{

    private protected readonly Dictionary<int, int> vanillaHpByInstance = new();
    private protected readonly Dictionary<int, (int phase2Hp, int phase3Hp)> vanillaPhaseThresholdsByFsm = new();
    private protected bool moduleActive;
    private protected bool hoGEntryAllowed;

    private protected virtual int P5Hp => throw new NotSupportedException();
    private protected abstract int PhaseThresholdReapplyAttempts { get; }
    private protected abstract float PhaseThresholdReapplyInterval { get; }
    private protected abstract string SceneName { get; }
    private protected abstract string BossObjectName { get; }
    private protected abstract string PhaseFsmName { get; }
    private protected abstract string PhaseCheckState1Name { get; }
    private protected abstract string PhaseCheckState2Name { get; }
    private protected abstract string Phase2VariableName { get; }
    private protected abstract string Phase3VariableName { get; }
    private protected abstract int DefaultPhase2Hp { get; }
    private protected abstract int DefaultPhase3Hp { get; }
    private protected abstract int DefaultVanillaHp { get; }
    private protected abstract int MinHp { get; }
    private protected abstract int MaxHpLimit { get; }
    private protected abstract int MinPhase2Hp { get; }
    private protected abstract int MinPhase3Hp { get; }
    private protected virtual string Phase2TransitionStateName => throw new NotSupportedException();
    private protected virtual string Phase3TransitionStateName => throw new NotSupportedException();
    private protected virtual string PhaseIdleStateName => throw new NotSupportedException();
    private protected virtual string PhaseIdle2StateName => throw new NotSupportedException();
    private protected virtual string PhaseEscalateState1Name => throw new NotSupportedException();
    private protected virtual string PhaseEscalateState2Name => throw new NotSupportedException();
    private protected abstract bool UseMaxHp { get; set; }
    private protected virtual bool P5HpEnabled { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    private protected abstract int MaxHp { get; set; }
    private protected virtual int MaxHpBeforeP5 { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    private protected abstract bool UseCustomPhase { get; set; }
    private protected abstract int Phase2Hp { get; set; }
    private protected abstract int Phase3Hp { get; set; }
    private protected virtual bool UseMaxHpBeforeP5 { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    private protected virtual bool HasStoredStateBeforeP5 { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    private protected virtual bool UseCustomPhaseBeforeP5 { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    private protected virtual int Phase2HpBeforeP5 { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    private protected virtual int Phase3HpBeforeP5 { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    private protected (int phase2Hp, int phase3Hp) GetVanillaPhaseThresholds(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaPhaseThresholdsByFsm.TryGetValue(fsmId, out (int phase2Hp, int phase3Hp) thresholds))
        {
            return (
                ClampBossPhase2Hp(thresholds.phase2Hp, ResolvePhase2MaxHp()),
                ClampBossPhase3Hp(thresholds.phase3Hp, thresholds.phase2Hp)
            );
        }

        RememberVanillaPhaseThresholds(fsm);
        if (vanillaPhaseThresholdsByFsm.TryGetValue(fsmId, out thresholds))
        {
            return (
                ClampBossPhase2Hp(thresholds.phase2Hp, ResolvePhase2MaxHp()),
                ClampBossPhase3Hp(thresholds.phase3Hp, thresholds.phase2Hp)
            );
        }

        return (DefaultPhase2Hp, DefaultPhase3Hp);
    }

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;
}
