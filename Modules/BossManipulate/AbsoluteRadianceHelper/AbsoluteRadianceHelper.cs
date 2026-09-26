using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class AbsoluteRadianceHelper : Module
{
    private const string AbsoluteRadianceScene = "GG_Radiance";
    private const string AbsoluteRadianceName = "Absolute Radiance";
    private const string AbsoluteRadianceControlFsmName = "Control";
    private const int DefaultAbsoluteRadianceMaxHp = 3000;
    private const int DefaultAbsoluteRadianceVanillaHp = 3000;
    private const int P5AbsoluteRadianceHp = 3000;
    private const int MinAbsoluteRadianceHp = 1;
    private const int MaxAbsoluteRadianceHp = 999999;
    [LocalSetting]
    [BoolOption]
    internal static bool absoluteRadianceUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool absoluteRadianceP5Hp = false;

    [LocalSetting]
    internal static int absoluteRadianceMaxHp = DefaultAbsoluteRadianceMaxHp;

    [LocalSetting]
    internal static bool absoluteRadianceUseCustomPhase = false;

    [LocalSetting]
    internal static int absoluteRadiancePhase2Hp = DefaultAbsoluteRadiancePhase2Hp;

    [LocalSetting]
    internal static int absoluteRadiancePhase3Hp = DefaultAbsoluteRadiancePhase3Hp;

    [LocalSetting]
    internal static int absoluteRadiancePhase4Hp = DefaultAbsoluteRadiancePhase4Hp;

    [LocalSetting]
    internal static int absoluteRadiancePhase5Hp = DefaultAbsoluteRadiancePhase5Hp;

    [LocalSetting]
    internal static int absoluteRadianceFinalPhaseHp = DefaultAbsoluteRadianceFinalPhaseHp;

    [LocalSetting]
    internal static int absoluteRadianceMaxHpBeforeP5 = DefaultAbsoluteRadianceMaxHp;

    [LocalSetting]
    internal static bool absoluteRadianceUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int absoluteRadiancePhase2HpBeforeP5 = DefaultAbsoluteRadiancePhase2Hp;

    [LocalSetting]
    internal static int absoluteRadiancePhase3HpBeforeP5 = DefaultAbsoluteRadiancePhase3Hp;

    [LocalSetting]
    internal static int absoluteRadiancePhase4HpBeforeP5 = DefaultAbsoluteRadiancePhase4Hp;

    [LocalSetting]
    internal static int absoluteRadiancePhase5HpBeforeP5 = DefaultAbsoluteRadiancePhase5Hp;

    [LocalSetting]
    internal static int absoluteRadianceFinalPhaseHpBeforeP5 = DefaultAbsoluteRadianceFinalPhaseHp;

    [LocalSetting]
    internal static bool absoluteRadianceUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool absoluteRadianceHasStoredStateBeforeP5 = false;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static readonly Dictionary<int, (int phase2Hp, int phase3Hp, int phase4Hp, int phase5Hp)> vanillaPhaseThresholdsByFsm = new();
    private static readonly Dictionary<int, int> vanillaFinalPhaseHpByFsm = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;
}
