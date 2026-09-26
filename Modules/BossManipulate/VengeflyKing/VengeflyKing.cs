using Modding;
using Satchel;
using Satchel.Futils;
using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class VengeflyKing : Module
{
    private const string VengeflyScene = "GG_Vengefly_V";
    private const string LeftVengeflyName = "Giant Buzzer Col (1)";
    private const string RightVengeflyName = "Giant Buzzer Col";
    [LocalSetting]
    [BoolOption]
    internal static bool vengeflyKingUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool vengeflyKingP5Hp = false;

    [LocalSetting]
    internal static int vengeflyKingLeftMaxHp = DefaultLeftMaxHp;

    [LocalSetting]
    internal static int vengeflyKingRightMaxHp = DefaultRightMaxHp;

    [LocalSetting]
    internal static int vengeflyKingSummonMaxHp = DefaultSummonMaxHp;

    [LocalSetting]
    internal static bool vengeflyKingUseCustomSummonLimit = false;

    [LocalSetting]
    internal static int vengeflyKingLeftSummonLimit = DefaultSummonLimit;

    [LocalSetting]
    internal static int vengeflyKingRightSummonLimit = DefaultSummonLimit;

    [LocalSetting]
    internal static int vengeflyKingLeftSummonAttackLimit = DefaultSummonAttackLimit;

    [LocalSetting]
    internal static int vengeflyKingRightSummonAttackLimit = DefaultSummonAttackLimit;

    [LocalSetting]
    internal static int vengeflyKingLeftMaxHpBeforeP5 = DefaultLeftMaxHp;

    [LocalSetting]
    internal static int vengeflyKingRightMaxHpBeforeP5 = DefaultRightMaxHp;

    [LocalSetting]
    internal static int vengeflyKingSummonMaxHpBeforeP5 = DefaultSummonMaxHp;

    [LocalSetting]
    internal static bool vengeflyKingUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool vengeflyKingUseCustomSummonLimitBeforeP5 = false;

    [LocalSetting]
    internal static bool vengeflyKingHasStoredStateBeforeP5 = false;

    private enum VengeflySide
    {
        Unknown = 0,
        Left = 1,
        Right = 2,
        Summon = 3,
    }

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static readonly Dictionary<int, int> vanillaSummonAttackLimitByFsm = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;
}
