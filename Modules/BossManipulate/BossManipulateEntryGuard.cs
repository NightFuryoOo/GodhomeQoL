namespace GodhomeQoL.Modules.BossChallenge;

internal static class BossManipulateEntryGuard
{
    private static readonly HashSet<string> AllowedEntryScenes = new(StringComparer.Ordinal)
    {
        "GG_Workshop",
        "GG_Atrium",
        "GG_Atrium_Roof"
    };

    private static bool hooksInstalled;
    private static bool bossSequenceTransitionPending;

    internal static void EnsureHooks()
    {
        if (hooksInstalled)
        {
            return;
        }

        hooksInstalled = true;
        On.BossSequenceController.SetupNewSequence += OnSetupNewSequence;
        USceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    internal static bool IsAllowedBossEntry(string currentScene, string nextScene)
    {
        EnsureHooks();

        if (string.IsNullOrEmpty(currentScene) || string.IsNullOrEmpty(nextScene))
        {
            return false;
        }

        if (!AllowedEntryScenes.Contains(currentScene))
        {
            return false;
        }

        if (BossSequenceController.IsInSequence || bossSequenceTransitionPending)
        {
            return false;
        }

        return true;
    }

    private static void OnSetupNewSequence(
        On.BossSequenceController.orig_SetupNewSequence orig,
        BossSequence sequence,
        BossSequenceController.ChallengeBindings bindings,
        string playerData
    )
    {
        bossSequenceTransitionPending = true;
        orig(sequence, bindings, playerData);
    }

    private static void OnActiveSceneChanged(Scene from, Scene to)
    {
        if (!BossSequenceController.IsInSequence && AllowedEntryScenes.Contains(to.name))
        {
            bossSequenceTransitionPending = false;
        }
    }
}