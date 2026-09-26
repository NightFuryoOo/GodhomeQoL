using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class ZoteHelper : Module
{
    private const string GreyPrinceName = "Grey Prince";
    private const int DefaultBossHp = 1400;
    private const int MinBossHp = 100;
    private const int MaxBossHp = 999999;

    internal static void ApplyBossHealthIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        GameObject? greyPrince = GameObject.Find(GreyPrinceName);
        if (greyPrince == null || greyPrince.scene.name != ZoteScene)
        {
            return;
        }

        if (!ShouldApplySettings(greyPrince))
        {
            return;
        }

        if (!zoteUseCustomBossHp)
        {
            return;
        }

        ApplyBossHealth(greyPrince);
    }

    private static void ApplyGreyPrinceChanges(PlayMakerFSM fsm)
    {
        if (IsPatched(fsm))
        {
            ApplyBossHealthIfPresent();
            return;
        }

        bool doubleSummons = zoteDoubleSummons
            && fsm.Fsm.GetState("Spit L") != null
            && fsm.Fsm.GetState("Spit R") != null;
        if (doubleSummons)
        {
            fsm.CopyState("Spit L", "Spit L1");
            fsm.CopyState("Spit R", "Spit R1");
            fsm.ChangeTransition("Spit L", "FINISHED", "Spit R1");
            fsm.ChangeTransition("Spit R", "FINISHED", "Spit L1");
        }

        bool summonOnlyPhase = zoteAttackSummonsOnlyOnStart
            && fsm.Fsm.GetState("Move Choice 3") != null
            && fsm.Fsm.GetState("Respit?") != null
            && fsm.Fsm.GetState("Spit Set") != null
            && fsm.Fsm.GetState("Spit Antic") != null;
        if (summonOnlyPhase)
        {
            SetSummonOnlyPhase(fsm, true);
        }

        MarkPatched(fsm);

        FsmState? spitAntic = fsm.Fsm.GetState("Spit Antic");
        if (spitAntic != null)
        {
            spitAntic.RemoveAction(3);
            spitAntic.InsertCustomAction(() =>
            {
                if (!moduleActive)
                {
                    return;
                }

                PruneZotelings();
                int summonLimit = zoteUseCustomSummonLimit
                    ? Math.Max(0, zoteSummonLimit)
                    : DefaultSummonLimit;
                if (activeZotelings.Count >= summonLimit
                    || (doubleSummons && activeZotelings.Count + 1 >= summonLimit))
                {
                    fsm.SendEvent("CANCEL");
                    if (summonOnlyPhase)
                    {
                        summonOnlyPhase = false;
                        SetSummonOnlyPhase(fsm, false);
                    }
                }
            }, 0);
        }

        void SummonZoteling()
        {
            if (!moduleActive)
            {
                return;
            }

            FsmGameObject zotelingVar = GetFsmGameObjectVariable(fsm, "Zoteling");
            GameObject? zotelingPrefab = zotelingVar.Value;
            if (zotelingPrefab == null)
            {
                return;
            }

            GameObject newZoteling = UObject.Instantiate(zotelingPrefab);
            zotelingVar.Value = newZoteling;
            TrackZoteling(newZoteling);
        }

        fsm.InsertCustomAction("Spit L", SummonZoteling, 1);
        fsm.InsertCustomAction("Spit R", SummonZoteling, 1);
        if (doubleSummons)
        {
            fsm.InsertCustomAction("Spit L1", SummonZoteling, 1);
            fsm.InsertCustomAction("Spit R1", SummonZoteling, 1);
        }

        ApplyBossHealthAfterInit(fsm);
        ApplyBossHealth(fsm.gameObject);
    }

    private static void SetSummonOnlyPhase(PlayMakerFSM fsm, bool summonOnly)
    {
        fsm.ChangeTransition("Move Choice 3", "JUMP", summonOnly ? "Spit Set" : "Set Jumps");
        fsm.ChangeTransition("Move Choice 3", "CHARGE", summonOnly ? "Spit Set" : "Charge Antic");
        fsm.ChangeTransition("Move Choice 3", "JUMP SLASH", summonOnly ? "Spit Set" : "JS Antic");
        fsm.ChangeTransition("Move Choice 3", "BALLOON ROAR", summonOnly ? "Spit Set" : "B Roar Antic");
        fsm.ChangeTransition("Respit?", "FINISHED", summonOnly ? "Spit Antic" : "Idle Start");
    }

    private static int ClampBossHp(int value)
    {
        if (value < MinBossHp)
        {
            return MinBossHp;
        }

        return value > MaxBossHp ? MaxBossHp : value;
    }

    private static void ApplyBossHealth(GameObject greyPrince, HealthManager? hm = null, int? overrideHp = null)
    {
        if (!zoteUseCustomBossHp && !overrideHp.HasValue)
        {
            return;
        }

        int targetHp = ClampBossHp(overrideHp ?? zoteBossHp);
        greyPrince.manageHealth(targetHp);

        hm ??= greyPrince.GetComponent<HealthManager>();
        if (hm != null)
        {
            hm.hp = targetHp;
        }
    }

    private static void ApplyBossHealthAfterInit(PlayMakerFSM fsm)
    {
        FsmState? roarEnd = fsm.Fsm.GetState("Roar End");
        if (roarEnd != null)
        {
            roarEnd.AddCustomAction(ApplyBossHealthIfPresent);
        }

        FsmState? init = fsm.Fsm.GetState("Init");
        if (init != null)
        {
            init.AddCustomAction(ApplyBossHealthIfPresent);
        }
    }

    private static bool IsGreyPrince(HealthManager hm)
    {
        if (hm == null)
        {
            return false;
        }

        return hm.gameObject.scene.name == ZoteScene
            && hm.gameObject.name == GreyPrinceName;
    }

    private static int GetImmortalBossTargetHp(HealthManager hm)
    {
        if (zoteUseCustomBossHp)
        {
            return ClampBossHp(zoteBossHp);
        }

        return ClampBossHp(hm.hp > 0 ? hm.hp : DefaultBossHp);
    }

    internal static bool IsGreyPrinceImmortalityActiveFor(GameObject? target = null)
    {
        if (!zoteImmortal)
        {
            return false;
        }

        if (!ModuleManager.TryGetLoadedModule(typeof(ZoteHelper), out _))
        {
            return false;
        }

        if (target != null)
        {
            return string.Equals(target.name, GreyPrinceName, StringComparison.Ordinal)
                && ShouldApplySettings(target);
        }

        return hoGEntryAllowed;
    }
}
