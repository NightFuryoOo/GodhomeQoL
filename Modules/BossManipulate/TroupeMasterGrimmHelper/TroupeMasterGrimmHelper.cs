using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class TroupeMasterGrimmHelper : Module
{
    private const string TroupeMasterGrimmScene = "GG_Grimm";
    private const string TroupeMasterGrimmName = "Grimm Boss";
    private const string BalloonCheckStateName = "Balloon?";
    private const int DefaultTroupeMasterGrimmMaxHp = 1000;
    private const int DefaultTroupeMasterGrimmVanillaHp = 1000;
    private const int P5TroupeMasterGrimmHp = 1000;
    private const int MinTroupeMasterGrimmHp = 1;
    private const int MaxTroupeMasterGrimmHp = 999999;
    [LocalSetting]
    [BoolOption]
    internal static bool troupeMasterGrimmUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool troupeMasterGrimmP5Hp = false;

    [LocalSetting]
    internal static int troupeMasterGrimmMaxHp = DefaultTroupeMasterGrimmMaxHp;

    [LocalSetting]
    internal static bool troupeMasterGrimmUseCustomPhase = false;

    [LocalSetting]
    internal static int troupeMasterGrimmPhase2Hp = DefaultTroupeMasterGrimmPhase2Hp;

    [LocalSetting]
    internal static int troupeMasterGrimmPhase3Hp = DefaultTroupeMasterGrimmPhase3Hp;

    [LocalSetting]
    internal static int troupeMasterGrimmPhase4Hp = DefaultTroupeMasterGrimmPhase4Hp;

    [LocalSetting]
    internal static int troupeMasterGrimmMaxHpBeforeP5 = DefaultTroupeMasterGrimmMaxHp;

    [LocalSetting]
    internal static bool troupeMasterGrimmUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int troupeMasterGrimmPhase2HpBeforeP5 = DefaultTroupeMasterGrimmPhase2Hp;

    [LocalSetting]
    internal static int troupeMasterGrimmPhase3HpBeforeP5 = DefaultTroupeMasterGrimmPhase3Hp;

    [LocalSetting]
    internal static int troupeMasterGrimmPhase4HpBeforeP5 = DefaultTroupeMasterGrimmPhase4Hp;

    [LocalSetting]
    internal static bool troupeMasterGrimmUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool troupeMasterGrimmHasStoredStateBeforeP5 = false;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;
}
