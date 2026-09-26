using GodhomeQoL.Modules.Tools;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class AlwaysFurious : Module
{
    private static void TryForceRecheck(PlayMakerFSM fsm)
    {
        try
        {
            PlayerData? pd = PlayerData.instance;
            if (pd != null)
            {
                int totalHealth = pd.health + pd.healthBlue;
                if (totalHealth > 1 && fsm.Fsm.GetState("Deactivate") != null)
                {
                    fsm.Fsm.SetState("Deactivate");
                    return;
                }
            }

            if (fsm.Fsm.GetState("Check HP") != null)
            {
                fsm.Fsm.SetState("Check HP");
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "AlwaysFurious.Fury.cs");
        }
    }

    private static void TryForceDisableFuryEffectIfUnequipped()
    {
        if (IsFuryEquipped())
        {
            return;
        }

        try
        {
            GameObject? charmEffects = GameObject.Find("Charm Effects");
            PlayMakerFSM? fsm = charmEffects?.LocateMyFSM("Fury");
            if (fsm != null)
            {
                if (fsm.Fsm.GetState("Deactivate") != null)
                {
                    fsm.Fsm.SetState("Deactivate");
                }
                else if (fsm.Fsm.GetState("Check HP") != null)
                {
                    fsm.Fsm.SetState("Check HP");
                }
            }

            PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");

            HeroController? hero = HeroController.instance;
            if (hero != null && !charmUpdateInProgress && !GearSwitcher.IsApplyingPreset)
            {
                hero.CharmUpdate();
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "AlwaysFurious.Fury.cs");
        }
    }

    private static void TryForceEnable(PlayMakerFSM fsm)
    {
        try
        {
            if (!IsFuryEquipped())
            {
                return;
            }

            if (!IsSafeToAdjustHealth())
            {
                return;
            }

            HeroController? hero = HeroController.instance;
            if (hero != null && !charmUpdateInProgress && !GearSwitcher.IsApplyingPreset)
            {
                hero.CharmUpdate();
            }

            PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
            PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");

            if (fsm.Fsm.GetState("Check HP") != null)
            {
                fsm.SendEvent("CHARM EQUIP CHECK");
                return;
            }

            if (fsm.Fsm.GetState("Activate") != null)
            {
                fsm.Fsm.SetState("Activate");
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "AlwaysFurious.Fury.cs");
        }
    }

    private static bool IsFuryEquipped()
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return false;
        }

        return CharmUtil.EquippedCharm(Charm.FuryOfTheFallen);
    }
}
