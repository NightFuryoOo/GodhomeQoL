using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class RageBossHelper : Module
{

    private protected readonly Dictionary<int, int> vanillaHpByInstance = new();
    private protected bool moduleActive;
    private protected bool hoGEntryAllowed;
    private protected readonly HashSet<int> forcedCustomRageByInstance = new();

    private protected virtual int P5Hp => throw new NotSupportedException();
    private protected abstract string SceneName { get; }
    private protected abstract string BossObjectName { get; }
    private protected abstract string PhaseFsmName { get; }
    private protected abstract string PhaseCheckStateName { get; }
    private protected virtual string Phase2VariableName => throw new NotSupportedException();
    private protected virtual string AttackingFsmName => throw new NotSupportedException();
    private protected virtual string RageVariableName => throw new NotSupportedException();
    private protected virtual string SummonEventName => throw new NotSupportedException();
    private protected abstract int DefaultVanillaHp { get; }
    private protected abstract int MinHp { get; }
    private protected abstract int MaxHpLimit { get; }
    private protected abstract int MinPhase2Hp { get; }
    private protected abstract bool UseMaxHp { get; set; }
    private protected virtual bool P5HpEnabled { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    private protected abstract int MaxHp { get; set; }
    private protected virtual int MaxHpBeforeP5 { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    private protected virtual bool UseMaxHpBeforeP5 { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    private protected virtual bool HasStoredStateBeforeP5 { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    private protected abstract bool UseCustomPhase { get; set; }
    private protected abstract int Phase2Hp { get; set; }
    private protected virtual bool UseCustomPhaseBeforeP5 { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    private protected virtual int Phase2HpBeforeP5 { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;
}
