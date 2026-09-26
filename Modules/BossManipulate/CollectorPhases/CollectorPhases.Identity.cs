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
    private static bool IsCollector(HealthManager hm)
    {
        if (hm == null)
        {
            return false;
        }

        string scene = hm.gameObject.scene.name;

        return hm.gameObject.name == "Jar Collector"
            && (scene == "GG_Collector" || scene == "GG_Collector_V" || scene.StartsWith("Ruins2_11", StringComparison.Ordinal));
    }

    private static bool IsCollectorScene(string scene) =>
        scene == "GG_Collector" || scene == "GG_Collector_V" || scene.StartsWith("Ruins2_11", StringComparison.Ordinal);

    private static bool IsBuzzerName(string name) =>
        name.IndexOf("Buzzer", StringComparison.OrdinalIgnoreCase) >= 0
        || name.IndexOf("Mosquito", StringComparison.OrdinalIgnoreCase) >= 0;

    private static bool IsRollerName(string name) =>
        name.IndexOf("Roller", StringComparison.OrdinalIgnoreCase) >= 0
        || name.IndexOf("Baldur", StringComparison.OrdinalIgnoreCase) >= 0;

    private static bool IsSpitterName(string name) =>
        name.IndexOf("Spitter", StringComparison.OrdinalIgnoreCase) >= 0
        || name.IndexOf("Aspid", StringComparison.OrdinalIgnoreCase) >= 0;

    internal static bool IsCollectorPatchingActiveFor(GameObject? target)
    {
        if (!ModuleManager.TryGetLoadedModule(typeof(CollectorPhases), out _))
        {
            return false;
        }

        if (!ShouldApplySettings(target))
        {
            return false;
        }

        return collectorPhase != 3
            || UseCustomPhase2Threshold
            || UseMaxHP
            || CollectorImmortal
            || IgnoreInitialJarLimit
            || DisableSummonLimit
            || !spawnBuzzer
            || !spawnRoller
            || !spawnSpitter
            || buzzerHP != DefaultBuzzerHp
            || rollerHP != DefaultRollerHp
            || spitterHP != DefaultSpitterHp;
    }

    internal static bool IsCollectorImmortalityActiveFor(GameObject? target)
    {
        return CollectorImmortal && IsCollectorPatchingActiveFor(target);
    }
}
