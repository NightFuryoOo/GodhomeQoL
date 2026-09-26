using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class PureVesselHelper : Module
{
    private const string PureVesselScene = "GG_Hollow_Knight";
    private const string PureVesselName = "HK Prime";
    [LocalSetting]
    [BoolOption]
    internal static bool pureVesselUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool pureVesselP5Hp = false;

    [LocalSetting]
    internal static int pureVesselMaxHp = DefaultPureVesselMaxHp;

    [LocalSetting]
    internal static bool pureVesselUseCustomPhase = false;

    [LocalSetting]
    internal static int pureVesselPhase2Hp = DefaultPureVesselPhase2Hp;

    [LocalSetting]
    internal static int pureVesselPhase3Hp = DefaultPureVesselPhase3Hp;

    [LocalSetting]
    internal static int pureVesselMaxHpBeforeP5 = DefaultPureVesselMaxHp;

    [LocalSetting]
    internal static bool pureVesselUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int pureVesselPhase2HpBeforeP5 = DefaultPureVesselPhase2Hp;

    [LocalSetting]
    internal static int pureVesselPhase3HpBeforeP5 = DefaultPureVesselPhase3Hp;

    [LocalSetting]
    internal static bool pureVesselUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool pureVesselHasStoredStateBeforeP5 = false;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;
}
