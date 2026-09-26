using GodhomeQoL.Modules.Tools;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class AlwaysFurious : Module
{
    private static void RequestHeroRefresh()
    {
        heroRefreshPending = true;
        EnsureRefreshCoroutine();
    }

    private static void RequestHudRefresh()
    {
        hudRefreshPending = true;
        EnsureRefreshCoroutine();
    }

    private static void EnsureRefreshCoroutine()
    {
        if (refreshCoroutineRunning)
        {
            return;
        }

        refreshCoroutineRunning = true;
        _ = GlobalCoroutineExecutor.Start(RefreshWhenSafe());
    }

    private static void CancelPendingRefresh()
    {
        heroRefreshPending = false;
        hudRefreshPending = false;
        refreshCoroutineRunning = false;
    }

    private static IEnumerator RefreshWhenSafe()
    {
        while (heroRefreshPending || hudRefreshPending)
        {
            if (IsSafeToAdjustHealth())
            {
                if (heroRefreshPending)
                {
                    heroRefreshPending = false;
                    TryRefreshHeroHealth();
                }

                if (hudRefreshPending)
                {
                    hudRefreshPending = false;
                    TryRefreshHudMasks();
                }
            }

            yield return null;
        }

        refreshCoroutineRunning = false;
    }

    private static void TryRefreshHudMasks(bool ignoreSafetyChecks = false)
    {
        try
        {
            if (!ignoreSafetyChecks && !IsSafeToAdjustHealth())
            {
                return;
            }

            GameObject? hud = Ref.GC?.hudCanvas?.gameObject;
            if (hud == null)
            {
                return;
            }

            if (!hud.activeInHierarchy)
            {
                hud.SetActive(true);
                return;
            }

            hud.SetActive(false);
            hud.SetActive(true);
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "AlwaysFurious.Refresh.cs");
        }
    }

    private static void TryRefreshHeroHealth(bool ignoreSafetyChecks = false)
    {
        try
        {
            if (!ignoreSafetyChecks && !IsSafeToAdjustHealth())
            {
                return;
            }

            Ref.HC?.proxyFSM?.SendEvent("HeroCtrl-HeroDamaged");
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "AlwaysFurious.Refresh.cs");
        }
    }
}
