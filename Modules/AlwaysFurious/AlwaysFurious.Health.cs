using GodhomeQoL.Modules.Tools;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class AlwaysFurious : Module
{
    private void TryApplyForcedHealth()
    {
        if (GearSwitcher.IsApplyingPreset)
        {
            RequestApplyWhenSafe();
            return;
        }

        if (!IsFuryEquipped())
        {
            return;
        }

        if (!IsSafeToAdjustHealth())
        {
            RequestApplyWhenSafe();
            return;
        }

        if (Ref.HC == null)
        {
            return;
        }

        PlayerData? pd = PlayerData.instance;
        if (pd == null || pd.health <= 0)
        {
            return;
        }

        bool hasSavedSnapshot = savedHealth.HasValue || savedMaxHealth.HasValue || savedMaxHealthBase.HasValue;
        if (!hasSavedSnapshot)
        {
            savedHealth = pd.health;
            savedMaxHealth = pd.maxHealth;
            savedMaxHealthBase = pd.maxHealthBase;
        }
        else
        {
            if (pd.maxHealth > 1)
            {
                savedMaxHealth = pd.maxHealth;
            }

            if (pd.maxHealthBase > 1)
            {
                savedMaxHealthBase = pd.maxHealthBase;
            }

            if (pd.health > 1)
            {
                savedHealth = pd.health;
            }
        }

        if (pd.maxHealth != 1 || pd.maxHealthBase != 1)
        {
            SetMaxHealth(1, 1);
        }

        if (pd.health != 1)
        {
            SetNormalHealth(1);
        }
    }

    private bool TryRestoreForcedHealth(bool allowDeferredWhenUnsafe = true, bool ignoreSafetyChecks = false)
    {
        if (!savedHealth.HasValue && !savedMaxHealth.HasValue && !savedMaxHealthBase.HasValue)
        {
            pendingRestore = false;
            return true;
        }

        if (!ignoreSafetyChecks && GearSwitcher.IsApplyingPreset)
        {
            if (allowDeferredWhenUnsafe)
            {
                RequestRestoreWhenSafe();
            }

            return false;
        }

        if (!ignoreSafetyChecks && !IsSafeToAdjustHealth())
        {
            if (allowDeferredWhenUnsafe)
            {
                RequestRestoreWhenSafe();
            }

            return false;
        }

        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            if (allowDeferredWhenUnsafe)
            {
                RequestRestoreWhenSafe();
            }

            return false;
        }

        int targetMaxBase = Math.Max(1, savedMaxHealthBase ?? pd.maxHealthBase);
        int targetMax = Math.Max(1, savedMaxHealth ?? pd.maxHealth);

        bool requestRefresh = Loaded && Enabled;
        SetMaxHealth(targetMax, targetMaxBase, requestRefresh);

        int targetHealth = savedHealth ?? pd.health;
        targetHealth = Math.Max(1, Math.Min(targetHealth, targetMax));

        savedHealth = null;
        savedMaxHealth = null;
        savedMaxHealthBase = null;
        pendingRestore = false;
        SetNormalHealth(targetHealth, requestRefresh);

        if (!requestRefresh)
        {
            TryRefreshHeroHealth(ignoreSafetyChecks: true);
            TryRefreshHudMasks(ignoreSafetyChecks: true);
        }

        return true;
    }

    private static void SetNormalHealth(int value, bool requestRefresh = true)
    {
        PlayerDataR.health = value;
        if (requestRefresh)
        {
            RequestHeroRefresh();
        }
    }

    private static void SetMaxHealth(int maxHealth, int maxHealthBase, bool requestRefresh = true)
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        pd.maxHealth = maxHealth;
        pd.maxHealthBase = maxHealthBase;
        if (requestRefresh)
        {
            RequestHeroRefresh();
            RequestHudRefresh();
        }
    }

    private static bool IsSafeToAdjustHealth()
    {
        if (GearSwitcher.IsApplyingPreset)
        {
            return false;
        }

        if (Ref.GM == null || Ref.GM.gameState != GameState.PLAYING)
        {
            return false;
        }

        HeroController? hero = Ref.HC;
        if (hero == null)
        {
            return false;
        }

        if (!hero.acceptingInput || hero.controlReqlinquished)
        {
            return false;
        }

        PlayerData? pd = PlayerData.instance;
        if (pd != null && pd.atBench)
        {
            return false;
        }

        return true;
    }

    private static void RequestApplyWhenSafe()
    {
        pendingApply = true;
        pendingRestore = false;
        EnsureHealthActionCoroutine();
    }

    private static void RequestRestoreWhenSafe()
    {
        pendingRestore = true;
        pendingApply = false;
        EnsureHealthActionCoroutine();
    }

    private static void EnsureHealthActionCoroutine()
    {
        if (healthActionCoroutineRunning)
        {
            return;
        }

        healthActionCoroutineRunning = true;
        _ = GlobalCoroutineExecutor.Start(ProcessPendingHealthActions());
    }

    private static IEnumerator ProcessPendingHealthActions()
    {
        while (pendingApply || pendingRestore)
        {
            if (GearSwitcher.IsApplyingPreset)
            {
                yield return null;
                continue;
            }

            if (IsSafeToAdjustHealth())
            {
                if (!ModuleManager.TryGetModule(typeof(AlwaysFurious), out Module? module))
                {
                    pendingApply = false;
                    pendingRestore = false;
                    break;
                }

                AlwaysFurious? alwaysFurious = module as AlwaysFurious;
                if (alwaysFurious == null)
                {
                    pendingApply = false;
                    pendingRestore = false;
                    break;
                }

                if (pendingRestore)
                {
                    pendingRestore = false;
                    alwaysFurious.TryRestoreForcedHealth();
                }
                else if (pendingApply)
                {
                    if (!alwaysFurious.Loaded || !IsFuryEquipped())
                    {
                        pendingApply = false;
                        alwaysFurious.TryRestoreForcedHealth();
                        TryForceDisableFuryEffectIfUnequipped();
                    }
                    else
                    {
                        pendingApply = false;
                        alwaysFurious.TryApplyForcedHealth();
                    }
                }
            }

            yield return null;
        }

        healthActionCoroutineRunning = false;
    }

    internal static void NotifyGearSwitcherApplied()
    {
        if (!ModuleManager.TryGetModule(typeof(AlwaysFurious), out Module? module))
        {
            return;
        }

        if (module is not AlwaysFurious alwaysFurious)
        {
            return;
        }

        if (!module.Enabled || !IsFuryEquipped())
        {
            alwaysFurious.TryRestoreForcedHealth();
            TryForceDisableFuryEffectIfUnequipped();
            return;
        }

        alwaysFurious.ForceReapplyFromCurrent();
    }

    internal static bool IsGearSwitcherHealthLockActive()
    {
        if (!ModuleManager.TryGetModule(typeof(AlwaysFurious), out Module? module))
        {
            return false;
        }

        if (module is not AlwaysFurious || !module.Enabled || !module.Loaded)
        {
            return false;
        }

        return IsFuryEquipped();
    }

    internal static bool IsHealthLockActive()
    {
        if (!ModuleManager.TryGetModule(typeof(AlwaysFurious), out Module? module))
        {
            return false;
        }

        if (module is not AlwaysFurious || !module.Enabled || !module.Loaded)
        {
            return false;
        }

        return IsFuryEquipped();
    }

    private void ForceReapplyFromCurrent()
    {
        savedHealth = null;
        savedMaxHealth = null;
        savedMaxHealthBase = null;

        if (!IsFuryEquipped())
        {
            return;
        }

        if (!IsSafeToAdjustHealth())
        {
            RequestApplyWhenSafe();
            return;
        }

        TryApplyForcedHealth();
    }
}
