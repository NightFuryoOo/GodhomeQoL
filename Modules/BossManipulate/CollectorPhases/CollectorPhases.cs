using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Satchel;
using Satchel.BetterMenus;
using Satchel.Futils;
using GodhomeQoL.Utils;

namespace GodhomeQoL.Modules.CollectorPhases;

public sealed partial class CollectorPhases : Module
{
    private const int DefaultCollectorHp = 1200;
    private const int DefaultBuzzerHp = 26;
    private const int DefaultRollerHp = 26;
    private const int DefaultSpitterHp = 26;
    private const int DefaultCustomSummonLimit = 4;

    [LocalSetting]
    internal static int collectorPhase = 3;

    [LocalSetting]
    internal static bool CollectorImmortal = false;

    [LocalSetting]
    internal static bool IgnoreInitialJarLimit = false;

    [LocalSetting]
    internal static bool DisableSummonLimit = false;

    [LocalSetting]
    internal static int CustomSummonLimit = DefaultCustomSummonLimit;

    [LocalSetting]
    internal static int collectorMaxHP = DefaultCollectorHp;

    [LocalSetting]
    internal static bool UseMaxHP = false;

    [LocalSetting]
    [BoolOption]
    internal static bool collectorP5Hp = false;

    [LocalSetting]
    internal static int collectorMaxHPBeforeP5 = DefaultCollectorHp;

    [LocalSetting]
    internal static bool collectorHasStoredMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool UseCustomPhase2Threshold = false;

    [LocalSetting]
    internal static int CustomPhase2Threshold = 850;

    [LocalSetting]
    internal static int buzzerHP = DefaultBuzzerHp;

    [LocalSetting]
    internal static int rollerHP = DefaultRollerHp;

    [LocalSetting]
    internal static int spitterHP = DefaultSpitterHp;

    [LocalSetting]
    [BoolOption]
    internal static bool spawnBuzzer = true;

    [LocalSetting]
    [BoolOption]
    internal static bool spawnRoller = true;

    [LocalSetting]
    [BoolOption]
    internal static bool spawnSpitter = true;

    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    #region Menu

    #endregion

}
