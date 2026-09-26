using Modding;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class HiveKnightHelper : Module
{
    private const string HiveKnightScene = "GG_Hive_Knight";
    private const string HiveKnightName = "Hive Knight";
    private const int DefaultHiveKnightMaxHp = 1300;
    private const int DefaultHiveKnightVanillaHp = 1300;
    private const int P5HiveKnightHp = 850;
    private const int MinHiveKnightHp = 1;
    private const int MaxHiveKnightHp = 999999;
    [LocalSetting]
    [BoolOption]
    internal static bool hiveKnightUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool hiveKnightP5Hp = false;

    [LocalSetting]
    internal static int hiveKnightMaxHp = DefaultHiveKnightMaxHp;

    [LocalSetting]
    internal static int hiveKnightMaxHpBeforeP5 = DefaultHiveKnightMaxHp;

    [LocalSetting]
    internal static bool hiveKnightUseCustomPhase = false;

    [LocalSetting]
    internal static int hiveKnightPhase2Hp = DefaultHiveKnightPhase2Hp;

    [LocalSetting]
    internal static int hiveKnightPhase3Hp = DefaultHiveKnightPhase3Hp;

    [LocalSetting]
    internal static bool hiveKnightUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool hiveKnightHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool hiveKnightUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int hiveKnightPhase2HpBeforeP5 = DefaultHiveKnightPhase2Hp;

    [LocalSetting]
    internal static int hiveKnightPhase3HpBeforeP5 = DefaultHiveKnightPhase3Hp;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static readonly Dictionary<int, (int phase2Hp, int phase3Hp)> vanillaPhaseThresholdsByFsm = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;
}
