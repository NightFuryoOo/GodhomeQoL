namespace GodhomeQoL.Modules.Tools;

internal static class InvincibilityClaims
{
    private static readonly Dictionary<int, string> activeClaims = new();
    private static int nextHandle;
    private static bool heroUpdateHookInstalled;
    private static bool snapshotCaptured;
    private static bool snapshotPlayerDataCaptured;
    private static bool snapshotPlayerDataInvincible;
    private static bool snapshotPlayerDataRefInvincible;

    internal static bool HasActiveClaims => activeClaims.Count > 0;

    internal static int Acquire(string owner)
    {
        int handle = ++nextHandle;
        bool wasEmpty = activeClaims.Count == 0;
        activeClaims[handle] = owner;

        if (wasEmpty)
        {
            CaptureSnapshot();
            EnsureHeroUpdateHook();
        }

        ApplyManagedState();
        return handle;
    }

    internal static void Release(int handle)
    {
        if (handle == 0 || !activeClaims.Remove(handle))
        {
            return;
        }

        if (activeClaims.Count == 0)
        {
            RestoreSnapshot();
            RemoveHeroUpdateHook();
            return;
        }

        ApplyManagedState();
    }

    private static void EnsureHeroUpdateHook()
    {
        if (heroUpdateHookInstalled)
        {
            return;
        }

        ModHooks.HeroUpdateHook += OnHeroUpdate;
        heroUpdateHookInstalled = true;
    }

    private static void RemoveHeroUpdateHook()
    {
        if (!heroUpdateHookInstalled)
        {
            return;
        }

        ModHooks.HeroUpdateHook -= OnHeroUpdate;
        heroUpdateHookInstalled = false;
    }

    private static void OnHeroUpdate()
    {
        if (!HasActiveClaims)
        {
            return;
        }

        ApplyManagedState();
    }

    private static void CaptureSnapshot()
    {
        if (snapshotCaptured)
        {
            return;
        }

        snapshotPlayerDataCaptured = false;
        PlayerData? pd = PlayerData.instance;
        if (pd != null)
        {
            snapshotPlayerDataCaptured = true;
            snapshotPlayerDataInvincible = pd.isInvincible;
        }

        snapshotPlayerDataRefInvincible = PlayerDataR.isInvincible;
        snapshotCaptured = true;
    }

    private static void RestoreSnapshot()
    {
        if (!snapshotCaptured)
        {
            ApplyState(false, false);
            return;
        }

        ApplyState(
            snapshotPlayerDataInvincible,
            snapshotPlayerDataRefInvincible,
            applyPlayerData: snapshotPlayerDataCaptured);
        snapshotCaptured = false;
        snapshotPlayerDataCaptured = false;
    }

    private static void ApplyManagedState() => ApplyState(true, true);

    private static void CaptureLatePlayerDataSnapshot(PlayerData pd)
    {
        if (!snapshotCaptured || snapshotPlayerDataCaptured || !HasActiveClaims)
        {
            return;
        }

        snapshotPlayerDataInvincible = pd.isInvincible;
        snapshotPlayerDataCaptured = true;
    }

    private static void ApplyState(bool playerDataInvincible, bool playerDataRefInvincible, bool applyPlayerData = true)
    {
        PlayerData? pd = PlayerData.instance;
        if (pd != null && applyPlayerData)
        {
            CaptureLatePlayerDataSnapshot(pd);
            pd.isInvincible = playerDataInvincible;
        }

        PlayerDataR.isInvincible = playerDataRefInvincible;
    }
}
