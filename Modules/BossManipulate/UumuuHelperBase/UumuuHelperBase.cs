using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class UumuuHelperBase : Module
{

    private protected readonly Dictionary<int, int> vanillaHpByInstance = new();
    private protected readonly Dictionary<int, int> vanillaSummonHpByInstance = new();
    private protected bool moduleActive;
    private protected bool hoGEntryAllowed;

    private protected abstract string SceneName { get; }
    private protected abstract string BossObjectName { get; }
    private protected abstract int DefaultVanillaHp { get; }
    private protected abstract int MinHp { get; }
    private protected abstract int MaxHpLimit { get; }
    private protected abstract bool UseMaxHp { get; set; }
    private protected abstract int MaxHp { get; set; }
    private protected abstract int SummonHp { get; set; }

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;
}
