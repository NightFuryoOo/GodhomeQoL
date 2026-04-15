namespace GodhomeQoL.Modules.BossChallenge;

internal static class BossFightRestartCompatibility
{
    private static readonly Dictionary<string, BossSceneController.SetupEventDelegate> setupEventsByScene = new(StringComparer.Ordinal);
    private static BossSceneController.SetupEventDelegate? lastSetupEvent;
    private static string? lastSetupSceneName;
    private static BossSceneController.SetupEventDelegate? transientSetupEventBase;
    private static BossSceneController.SetupEventDelegate? transientSetupEventWrapper;
    private static int fastReloadTransitionScopeDepth;

    internal static bool IsFastReloadTransitionInProgress => fastReloadTransitionScopeDepth > 0;

    internal static void RecordCurrentSetupEvent(string? sceneNameOverride = null)
    {
        if (BossSequenceController.IsInSequence)
        {
            return;
        }

        BossSceneController.SetupEventDelegate? setupEvent = BossSceneController.SetupEvent;
        if (setupEvent == null)
        {
            return;
        }

        if (ReferenceEquals(setupEvent, transientSetupEventWrapper) && transientSetupEventBase != null)
        {
            setupEvent = transientSetupEventBase;
        }

        lastSetupEvent = setupEvent;

        string? sceneName = !string.IsNullOrEmpty(sceneNameOverride)
            ? sceneNameOverride
            : GameManager.instance?.sceneName;
        if (!string.IsNullOrEmpty(sceneName))
        {
            lastSetupSceneName = sceneName;
            setupEventsByScene[sceneName!] = setupEvent;
        }
    }

    internal static bool TryApplySetupEventForScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            return false;
        }

        ClearTransientSetupEventOverride();

        if (setupEventsByScene.TryGetValue(sceneName, out BossSceneController.SetupEventDelegate? setupEvent))
        {
            BossSceneController.SetupEvent = setupEvent;
            return true;
        }

        if (lastSetupEvent != null
            && !string.IsNullOrEmpty(lastSetupSceneName)
            && string.Equals(lastSetupSceneName, sceneName, StringComparison.Ordinal))
        {
            BossSceneController.SetupEvent = lastSetupEvent;
            return true;
        }

        return false;
    }

    internal static bool InstallTransientSetupEventOverride(Action<BossSceneController> afterSetupAction)
    {
        if (afterSetupAction == null)
        {
            return false;
        }

        BossSceneController.SetupEventDelegate? currentSetupEvent = BossSceneController.SetupEvent;
        if (currentSetupEvent == null)
        {
            return false;
        }

        ClearTransientSetupEventOverride();

        BossSceneController.SetupEventDelegate baseSetupEvent = currentSetupEvent;
        transientSetupEventBase = baseSetupEvent;

        BossSceneController.SetupEventDelegate? wrapper = null;
        wrapper = self =>
        {
            try
            {
                baseSetupEvent(self);
                afterSetupAction(self);
            }
            finally
            {
                if (ReferenceEquals(BossSceneController.SetupEvent, wrapper))
                {
                    BossSceneController.SetupEvent = baseSetupEvent;
                }

                if (ReferenceEquals(transientSetupEventWrapper, wrapper))
                {
                    transientSetupEventBase = null;
                    transientSetupEventWrapper = null;
                }
            }
        };

        transientSetupEventWrapper = wrapper;
        BossSceneController.SetupEvent = wrapper;
        return true;
    }

    internal static void ClearTransientSetupEventOverride()
    {
        if (transientSetupEventWrapper == null)
        {
            return;
        }

        if (ReferenceEquals(BossSceneController.SetupEvent, transientSetupEventWrapper))
        {
            BossSceneController.SetupEvent = transientSetupEventBase;
        }

        transientSetupEventBase = null;
        transientSetupEventWrapper = null;
    }

    internal static IDisposable EnterFastReloadTransitionScope()
    {
        fastReloadTransitionScopeDepth++;
        return new FastReloadTransitionScope();
    }

    private sealed class FastReloadTransitionScope : IDisposable
    {
        private bool disposed;

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            if (fastReloadTransitionScopeDepth > 0)
            {
                fastReloadTransitionScopeDepth--;
            }
        }
    }
}
