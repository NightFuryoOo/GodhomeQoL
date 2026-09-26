using HutongGames.PlayMaker;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class NightmareKingGrimmHelper : Module
{
    private const string NightmareKingGrimmScene = "GG_Grimm_Nightmare";
    private const string NightmareKingGrimmName = "Nightmare Grimm Boss";
    private const int DefaultNightmareKingGrimmMaxHp = 1650;
    private const int DefaultNightmareKingGrimmVanillaHp = 1650;
    private const int P5NightmareKingGrimmHp = 1250;
    private const int MinNightmareKingGrimmHp = 1;
    private const int MaxNightmareKingGrimmHp = 999999;
    [LocalSetting]
    [BoolOption]
    internal static bool nightmareKingGrimmUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool nightmareKingGrimmP5Hp = false;

    [LocalSetting]
    internal static int nightmareKingGrimmMaxHp = DefaultNightmareKingGrimmMaxHp;

    [LocalSetting]
    internal static bool nightmareKingGrimmUseCustomPhase = false;

    [LocalSetting]
    internal static int nightmareKingGrimmRagePhase1Hp = DefaultNightmareKingGrimmRagePhase1Hp;

    [LocalSetting]
    internal static int nightmareKingGrimmRagePhase2Hp = DefaultNightmareKingGrimmRagePhase2Hp;

    [LocalSetting]
    internal static int nightmareKingGrimmRagePhase3Hp = DefaultNightmareKingGrimmRagePhase3Hp;

    [LocalSetting]
    internal static int nightmareKingGrimmMaxHpBeforeP5 = DefaultNightmareKingGrimmMaxHp;

    [LocalSetting]
    internal static bool nightmareKingGrimmUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int nightmareKingGrimmRagePhase1HpBeforeP5 = DefaultNightmareKingGrimmRagePhase1Hp;

    [LocalSetting]
    internal static int nightmareKingGrimmRagePhase2HpBeforeP5 = DefaultNightmareKingGrimmRagePhase2Hp;

    [LocalSetting]
    internal static int nightmareKingGrimmRagePhase3HpBeforeP5 = DefaultNightmareKingGrimmRagePhase3Hp;

    [LocalSetting]
    internal static bool nightmareKingGrimmUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool nightmareKingGrimmHasStoredStateBeforeP5 = false;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;
}
