namespace GodhomeQoL.Modules.BossChallenge;

internal static class ReturnGateGuard
{
    private const string GatePrefix = "door_dreamReturnGG";
    private const string GateFsmName = "Return from boss";
    private const string WaitingState = "Transition In";
    private const string WakeEvent = "DREAM WAKE";
    private const string HeroFsmName = "Dream Return";
    private const string HeroIdleState = "Idle";
    private const float PollSeconds = 0.25f;
    private const float IdleConfirmSeconds = 1f;
    private const float QuietExitSeconds = 15f;

    private static bool installed;
    private static int generation;

    internal static void Install()
    {
        if (installed)
        {
            return;
        }

        installed = true;
        USceneManager.activeSceneChanged += OnSceneChanged;
    }

    internal static void Uninstall()
    {
        if (!installed)
        {
            return;
        }

        installed = false;
        generation++;
        USceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private static void OnSceneChanged(Scene current, Scene next)
    {
        int token = ++generation;
        if (string.Equals(next.name, CustomPantheonStatue.WorkshopScene, StringComparison.Ordinal))
        {
            _ = GlobalCoroutineExecutor.Start(Watch(token));
        }
    }

    private static IEnumerator Watch(int token)
    {
        float started = Time.realtimeSinceStartup;
        float idleSince = -1f;
        while (token == generation)
        {
            yield return new WaitForSecondsRealtime(PollSeconds);
            if (token != generation)
            {
                yield break;
            }

            PlayMakerFSM? gate = FindWaitingGate();
            if (gate == null)
            {
                if (Time.realtimeSinceStartup - started > QuietExitSeconds)
                {
                    yield break;
                }

                idleSince = -1f;
                continue;
            }

            if (!IsHeroUp())
            {
                idleSince = -1f;
                continue;
            }

            if (idleSince < 0f)
            {
                idleSince = Time.realtimeSinceStartup;
                continue;
            }

            if (Time.realtimeSinceStartup - idleSince < IdleConfirmSeconds)
            {
                continue;
            }

            LogDebug($"[ReturnGate] '{gate.gameObject.name}' was still waiting for the hero to get up; waking it so the statue can be used again");
            gate.SendEvent(WakeEvent);
            yield break;
        }
    }

    private static PlayMakerFSM? FindWaitingGate()
    {
        try
        {
            foreach (TransitionPoint point in TransitionPoint.TransitionPoints)
            {
                if (point == null || !point.name.StartsWith(GatePrefix, StringComparison.Ordinal))
                {
                    continue;
                }

                PlayMakerFSM? fsm = point.gameObject.LocateMyFSM(GateFsmName);
                if (fsm != null && fsm.enabled && string.Equals(fsm.ActiveStateName, WaitingState, StringComparison.Ordinal))
                {
                    return fsm;
                }
            }
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "ReturnGateGuard.cs/Find");
        }

        return null;
    }

    private static bool IsHeroUp()
    {
        HeroController? hero = HeroController.instance;
        if (hero == null || !hero.acceptingInput || hero.controlReqlinquished || hero.cState.dead)
        {
            return false;
        }

        PlayMakerFSM? dreamReturn = hero.gameObject.LocateMyFSM(HeroFsmName);
        return dreamReturn != null && string.Equals(dreamReturn.ActiveStateName, HeroIdleState, StringComparison.Ordinal);
    }
}
