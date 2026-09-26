using Modding;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class FlukemarmHelper : Module
{
    private const string FlukemarmScene = "GG_Flukemarm";
    private const string FlukemarmName = "Fluke Mother";
    private const int DefaultFlukemarmMaxHp = 900;
    private const int DefaultFlukemarmVanillaHp = 900;
    private const int P5FlukemarmHp = 500;
    private const int MinFlukemarmHp = 1;
    private const int MaxFlukemarmHp = 999999;
    [LocalSetting]
    [BoolOption]
    internal static bool flukemarmUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool flukemarmP5Hp = false;

    [LocalSetting]
    internal static int flukemarmMaxHp = DefaultFlukemarmMaxHp;

    [LocalSetting]
    internal static int flukemarmFlyHp = DefaultFlukeFlyHp;

    [LocalSetting]
    internal static bool flukemarmUseCustomSummonLimit = false;

    [LocalSetting]
    internal static int flukemarmSummonLimit = DefaultFlukemarmSummonLimit;

    [LocalSetting]
    internal static int flukemarmMaxHpBeforeP5 = DefaultFlukemarmMaxHp;

    [LocalSetting]
    internal static int flukemarmFlyHpBeforeP5 = DefaultFlukeFlyHp;

    [LocalSetting]
    internal static bool flukemarmUseCustomSummonLimitBeforeP5 = false;

    [LocalSetting]
    internal static int flukemarmSummonLimitBeforeP5 = DefaultFlukemarmSummonLimit;

    [LocalSetting]
    internal static bool flukemarmUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool flukemarmHasStoredStateBeforeP5 = false;

    private enum FlukemarmTarget
    {
        Unknown = 0,
        Boss = 1,
        Fly = 2,
    }

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static readonly Dictionary<int, int> vanillaSummonLimitByFsm = new();
    private static readonly List<GameObject> customFlukeFlyPoolEntries = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;
    private static int vanillaFlukeFlyPoolSize = -1;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;
}
