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
    internal static void ReapplyLiveSettings()
    {
        if (!moduleActive || !ModuleManager.TryGetLoadedModule(typeof(CollectorPhases), out _))
        {
            return;
        }

        Scene activeScene = USceneManager.GetActiveScene();
        if (!activeScene.IsValid() || !activeScene.isLoaded || !IsCollectorScene(activeScene.name))
        {
            return;
        }

        try
        {
            foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
            {
                if (fsm == null || fsm.gameObject == null || !ShouldApplySettings(fsm.gameObject))
                {
                    continue;
                }

                if (fsm.gameObject.name == "Jar Collector" && fsm.FsmName == "Control")
                {
                    IntCompare? compare = GetFirstActionOfType<IntCompare>(fsm, "Resummon?");
                    if (compare != null)
                    {
                        compare.integer2.Value = IgnoreInitialJarLimit ? 0 : 3;
                    }

                    SetCollectorSummonHp(fsm);
                    FilterCollectorSummonPool(fsm);
                    ApplySummonCounts(fsm);
                }
                else
                {
                    TryGateSpawnerFSM(fsm.gameObject);
                }
            }

            foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
            {
                if (hm != null && IsCollector(hm))
                {
                    ApplyCollectorHealth(hm.gameObject, hm);
                }
            }
        }
        catch (Exception e)
        {
            LogWarn($"Failed to reapply Collector settings: {e.Message}");
        }
    }

    private const int VanillaCollectorP5Hp = 900;
    private const int MinCollectorHp = 100;
    private const int MaxCollectorHp = 99999;

    internal static void SetP5HpEnabled(bool value)
    {
        if (value)
        {
            if (!collectorP5Hp)
            {
                collectorMaxHPBeforeP5 = ClampCollectorHp(collectorMaxHP);
                collectorHasStoredMaxHpBeforeP5 = true;
            }

            collectorP5Hp = true;
            UseMaxHP = true;
            collectorMaxHP = VanillaCollectorP5Hp;
        }
        else
        {
            if (collectorP5Hp && collectorHasStoredMaxHpBeforeP5)
            {
                collectorMaxHP = ClampCollectorHp(collectorMaxHPBeforeP5);
            }

            collectorP5Hp = false;
            collectorHasStoredMaxHpBeforeP5 = false;
        }

        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!collectorP5Hp)
        {
            return;
        }

        if (!collectorHasStoredMaxHpBeforeP5)
        {
            collectorMaxHPBeforeP5 = ClampCollectorHp(collectorMaxHP);
            collectorHasStoredMaxHpBeforeP5 = true;
        }

        UseMaxHP = true;
        collectorMaxHP = VanillaCollectorP5Hp;
    }

    private static int ClampCollectorHp(int value)
    {
        if (value < MinCollectorHp)
        {
            return MinCollectorHp;
        }

        return value > MaxCollectorHp ? MaxCollectorHp : value;
    }

    private static void ApplyCollectorHealth(GameObject collector, HealthManager? hm = null)
    {
        if (!ShouldApplySettings(collector))
        {
            return;
        }
        if (!UseMaxHP)
        {
            return;
        }

        collector.manageHealth(collectorMaxHP);

        hm ??= collector.GetComponent<HealthManager>();
        if (hm != null)
        {
            hm.hp = collectorMaxHP;
        }
    }

    private static void SetCollectorSummonHp(PlayMakerFSM fsm)
    {
        try
        {
            SetIntVariable(fsm, "Buzzer HP", Math.Max(buzzerHP, 1));
            SetIntVariable(fsm, "Roller HP", Math.Max(rollerHP, 1));
            SetIntVariable(fsm, "Spitter HP", Math.Max(spitterHP, 1));
        }
        catch (Exception e)
        {
            LogWarn($"Failed to set collector summon HP: {e.Message}");
        }
    }
}
