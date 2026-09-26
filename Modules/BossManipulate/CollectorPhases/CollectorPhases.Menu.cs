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
    internal static IEnumerable<Element> MenuElements()
    {
        return new Element[]
        {
            new CustomSlider(
                "Settings/CollectorPhases/CollectorPhase".Localize(),
                val =>
                {
                    collectorPhase = (int)val;
                },
                () => collectorPhase,
                1f,
                3f,
                true
            ),
            Blueprints.HorizontalBoolOption(
                "Settings/CollectorPhases/CollectorImmortal".Localize(),
                "",
                b => CollectorImmortal = b,
                () => CollectorImmortal
            ),
            Blueprints.HorizontalBoolOption(
                "Settings/CollectorPhases/IgnoreInitialJarLimit".Localize(),
                "",
                b => IgnoreInitialJarLimit = b,
                () => IgnoreInitialJarLimit
            ),
            Blueprints.HorizontalBoolOption(
                "Settings/CollectorPhases/UseCustomPhase2Threshold".Localize(),
                "",
                b => UseCustomPhase2Threshold = b,
                () => UseCustomPhase2Threshold
            ),
            Blueprints.IntInputField(
                "Settings/CollectorPhases/CustomPhase2Threshold".Localize(),
                val =>
                {
                    CustomPhase2Threshold = Mathf.Clamp(val, 1, 99999);
                    GodhomeQoL.MarkMenuDirty();
                },
                () => CustomPhase2Threshold,
                850,
                "HP",
                6
            ),
            Blueprints.HorizontalBoolOption(
                "Settings/CollectorPhases/UseMaxHP".Localize(),
                "",
                b => UseMaxHP = b,
                () => UseMaxHP
            ),
            Blueprints.IntInputField(
                "Settings/CollectorPhases/CollectorMaxHP".Localize(),
                val =>
                {
                    collectorMaxHP = Mathf.Max(val, 100);
                    GodhomeQoL.MarkMenuDirty();
                },
                () => collectorMaxHP,
                5,
                "HP",
                5
            ),
            Blueprints.IntInputField(
                "Settings/CollectorPhases/BuzzerHP".Localize(),
                val =>
                {
                    buzzerHP = Math.Max(val, 1);
                    GodhomeQoL.MarkMenuDirty();
                },
                () => buzzerHP,
                26,
                "HP",
                5
            ),
            Blueprints.HorizontalBoolOption(
                "Settings/CollectorPhases/SpawnBuzzer".Localize(),
                "",
                b => spawnBuzzer = b,
                () => spawnBuzzer
            ),
            Blueprints.IntInputField(
                "Settings/CollectorPhases/RollerHP".Localize(),
                val =>
                {
                    rollerHP = Math.Max(val, 1);
                    GodhomeQoL.MarkMenuDirty();
                },
                () => rollerHP,
                26,
                "HP",
                5
            ),
            Blueprints.HorizontalBoolOption(
                "Settings/CollectorPhases/SpawnRoller".Localize(),
                "",
                b => spawnRoller = b,
                () => spawnRoller
            ),
            Blueprints.IntInputField(
                "Settings/CollectorPhases/SpitterHP".Localize(),
                val =>
                {
                    spitterHP = Math.Max(val, 1);
                    GodhomeQoL.MarkMenuDirty();
                },
                () => spitterHP,
                26,
                "HP",
                5
            ),
            Blueprints.HorizontalBoolOption(
                "Settings/CollectorPhases/SpawnSpitter".Localize(),
                "",
                b => spawnSpitter = b,
                () => spawnSpitter
            ),
            Blueprints.HorizontalBoolOption(
                "Settings/CollectorPhases/DisableSummonLimit".Localize(),
                "",
                b => DisableSummonLimit = b,
                () => DisableSummonLimit
            ),
            Blueprints.IntInputField(
                "Settings/CollectorPhases/CustomSummonLimit".Localize(),
                val =>
                {
                    CustomSummonLimit = Mathf.Clamp(val, 2, 999);
                    GodhomeQoL.MarkMenuDirty();
                },
                () => CustomSummonLimit,
                DefaultCustomSummonLimit,
                "CNT",
                3
            ),
            new MenuButton(
                "Settings/CollectorPhases/Reset".Localize(),
                "",
                _ =>
                {
                    ResetDefaults();
                    GodhomeQoL.MarkMenuDirty();
                },
                true
            )
        };
    }

    private static void ResetDefaults()
    {
        collectorPhase = 3;
        collectorMaxHP = DefaultCollectorHp;
        UseMaxHP = true;
        collectorP5Hp = false;
        collectorMaxHPBeforeP5 = DefaultCollectorHp;
        collectorHasStoredMaxHpBeforeP5 = false;
        UseCustomPhase2Threshold = false;
        CustomPhase2Threshold = 850;
        buzzerHP = DefaultBuzzerHp;
        rollerHP = DefaultRollerHp;
        spitterHP = DefaultSpitterHp;
        spawnBuzzer = true;
        spawnRoller = true;
        spawnSpitter = true;
        CollectorImmortal = false;
        IgnoreInitialJarLimit = false;
        DisableSummonLimit = false;
        CustomSummonLimit = DefaultCustomSummonLimit;
    }
}
