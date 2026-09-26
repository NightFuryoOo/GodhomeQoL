using Mono.Cecil.Cil;
using MonoMod.Cil;
using IL;
using InControl;
using GodhomeQoL.Modules.BossChallenge;
using GodhomeQoL.Modules.QoL;
using ToggleableBindings;
using ToggleableBindings.VanillaBindings;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class GearSwitcher : Module
{
    private static void ClearPendingComponentApplies()
    {
        pendingSpellsApply = false;
        pendingNailArtsApply = false;
        pendingAbilitiesApply = false;
        pendingDreamNailApply = false;
        pendingBindingsApply = false;
        pendingStatsApply = false;
        pendingCharmCostApply = false;
        pendingNailInputApply = false;
        pendingOvercharmedApply = false;
        pendingStatsParts = StatsPart.None;
        pendingSpellsPreset = null;
        pendingNailArtsPreset = null;
        pendingAbilitiesPreset = null;
        pendingDreamNailPreset = null;
        pendingBindingsPreset = null;
        pendingStatsPreset = null;
        pendingCharmCostPreset = null;
        pendingNailInputPreset = null;
        pendingOvercharmedPreset = null;
    }

    internal static void ClearPendingApplies()
    {
        pendingApply = false;
        pendingPresetName = string.Empty;
        ClearPendingComponentApplies();
    }

    private static void EnsurePendingCoroutine()
    {
        if (pendingCoroutineRunning)
        {
            return;
        }

        pendingCoroutineRunning = true;
        int generation = coroutineGeneration;
        _ = GlobalCoroutineExecutor.Start(ApplyWhenSafe(generation));
    }

    private static void ScheduleOvercharmedReapply()
    {
        int generation = coroutineGeneration;
        _ = GlobalCoroutineExecutor.Start(DelayedOvercharmedReapply(generation));
    }

    private static void ScheduleNailInputReapply()
    {
        int generation = coroutineGeneration;
        _ = GlobalCoroutineExecutor.Start(DelayedNailInputReapply(generation));
    }

    private static void ReapplyForcedOvercharmed()
    {
        if (!TryGetPreset(GetLastPresetName(), out GearPreset preset))
        {
            return;
        }

        if (!preset.Overcharmed)
        {
            return;
        }

        ApplyOvercharmedImmediate(preset);
    }

    private static IEnumerator DelayedOvercharmedReapply(int generation)
    {
        for (int i = 0; i < 5; i++)
        {
            if (generation != coroutineGeneration)
            {
                yield break;
            }

            yield return null;
        }

        if (generation != coroutineGeneration)
        {
            yield break;
        }

        if (!TryGetPreset(GetLastPresetName(), out GearPreset preset))
        {
            yield break;
        }

        ApplyOvercharmedImmediate(preset);
    }

    private static IEnumerator DelayedNailInputReapply(int generation)
    {
        for (int i = 0; i < 30; i++)
        {
            if (generation != coroutineGeneration)
            {
                yield break;
            }

            yield return null;
        }

        if (generation != coroutineGeneration)
        {
            yield break;
        }

        EnsureNailInputState();

        for (int i = 0; i < 30; i++)
        {
            if (generation != coroutineGeneration)
            {
                yield break;
            }

            yield return null;
        }

        if (generation != coroutineGeneration)
        {
            yield break;
        }

        EnsureNailInputState();
    }

    private static void ScheduleShellBindingResync()
    {
        int generation = coroutineGeneration;
        _ = GlobalCoroutineExecutor.Start(DelayedShellBindingResync(generation));
    }

    private static IEnumerator DelayedShellBindingResync(int generation)
    {
        for (int i = 0; i < 2; i++)
        {
            if (generation != coroutineGeneration)
            {
                yield break;
            }

            yield return null;
        }

        if (generation != coroutineGeneration)
        {
            yield break;
        }

        SyncBindingsWithLastPreset();

        for (int i = 0; i < 20; i++)
        {
            if (generation != coroutineGeneration)
            {
                yield break;
            }

            yield return null;
        }

        if (generation != coroutineGeneration)
        {
            yield break;
        }

        SyncBindingsWithLastPreset();

        for (int i = 0; i < 40; i++)
        {
            if (generation != coroutineGeneration)
            {
                yield break;
            }

            yield return null;
        }

        if (generation != coroutineGeneration)
        {
            yield break;
        }

        SyncBindingsWithLastPreset();

        for (int i = 0; i < 120; i++)
        {
            if (generation != coroutineGeneration)
            {
                yield break;
            }

            yield return null;
        }

        if (generation != coroutineGeneration)
        {
            yield break;
        }

        SyncBindingsWithLastPreset();
    }

    private static void SyncBindingsWithLastPreset()
    {
        SyncShellBindingWithLastPreset();
        EnforceBindingsWithLastPreset();
        EnforceCharmLoadoutWithLastPreset();
    }

    private static void ScheduleDisabledBindingsCleanup()
    {
        int generation = coroutineGeneration;
        _ = GlobalCoroutineExecutor.Start(DelayedDisabledBindingsCleanup(generation));
    }

    private static IEnumerator DelayedDisabledBindingsCleanup(int generation)
    {
        foreach (int waitFrames in new[] { 10, 60 })
        {
            for (int i = 0; i < waitFrames; i++)
            {
                yield return null;
                if (generation != coroutineGeneration)
                {
                    yield break;
                }
            }

            if (IsGloballyEnabled)
            {
                yield break;
            }

            RestoreAllBindingsImmediate(force: true);
        }
    }

    private static IEnumerator ApplyWhenSafe(int generation)
    {
        try
        {
            while (
                generation == coroutineGeneration
                && (pendingApply || pendingSpellsApply || pendingNailArtsApply || pendingAbilitiesApply || pendingDreamNailApply || pendingBindingsApply || pendingStatsApply || pendingCharmCostApply || pendingNailInputApply || pendingOvercharmedApply)
            )
            {
                if (!IsGloballyEnabled)
                {
                    ClearPendingApplies();
                    break;
                }

                if (IsSafeToApply())
                {
                    TryApplyPendingWhenSafe();
                }

                yield return null;
            }
        }
        finally
        {
            if (generation == coroutineGeneration)
            {
                pendingCoroutineRunning = false;
            }
        }
    }

    private static void TryApplyPendingWhenSafe()
    {
        try
        {
            if (pendingApply)
            {
                pendingApply = false;
                string presetToApply = pendingPresetName;
                pendingPresetName = string.Empty;
                ClearPendingComponentApplies();
                if (!string.IsNullOrWhiteSpace(presetToApply))
                {
                    ApplyPreset(presetToApply, false);
                }

                return;
            }

            if (pendingStatsApply)
            {
                pendingStatsApply = false;
                StatsPart parts = pendingStatsParts;
                pendingStatsParts = StatsPart.None;
                if (pendingStatsPreset != null)
                {
                    ApplyStatsParts(pendingStatsPreset, parts);
                }
            }

            if (pendingSpellsApply)
            {
                pendingSpellsApply = false;
                if (pendingSpellsPreset != null)
                {
                    ApplySpells(pendingSpellsPreset);
                }
            }

            if (pendingNailArtsApply)
            {
                pendingNailArtsApply = false;
                if (pendingNailArtsPreset != null)
                {
                    ApplyNailArts(pendingNailArtsPreset);
                }
            }

            if (pendingAbilitiesApply)
            {
                pendingAbilitiesApply = false;
                if (pendingAbilitiesPreset != null)
                {
                    ApplyAbilities(pendingAbilitiesPreset);
                }
            }

            if (pendingDreamNailApply)
            {
                pendingDreamNailApply = false;
                if (pendingDreamNailPreset != null)
                {
                    ApplyDreamNail(pendingDreamNailPreset);
                }
            }

            if (pendingBindingsApply)
            {
                pendingBindingsApply = false;
                if (pendingBindingsPreset != null)
                {
                    ApplyBindings(pendingBindingsPreset);
                }
            }

            if (pendingNailInputApply)
            {
                pendingNailInputApply = false;
                if (pendingNailInputPreset != null)
                {
                    ApplyNailInput(pendingNailInputPreset);
                }
            }

            if (pendingOvercharmedApply)
            {
                pendingOvercharmedApply = false;
                if (pendingOvercharmedPreset != null)
                {
                    ApplyOvercharmed(pendingOvercharmedPreset);
                }
            }

            if (pendingCharmCostApply)
            {
                pendingCharmCostApply = false;
                if (pendingCharmCostPreset != null)
                {
                    ApplyCharmCosts(pendingCharmCostPreset);
                }
            }
        }
        catch (Exception ex)
        {
            LogError($"[GearSwitcher] ApplyWhenSafe failed: {ex}");
            ClearPendingApplies();
        }
    }

    private static void ReapplyNonSoulActiveBindings()
    {
        ReapplyBindingIfActive<NailBinding>();
        ReapplyBindingIfActive<ShellBinding>();
        ReapplyBindingIfActive<CharmsBinding>();
    }

    private static void ReapplyBindingIfActive<T>() where T : Binding
    {
        try
        {
            if (BindingManager.TryGetBinding<T>(out T? binding) && binding != null && binding.IsApplied)
            {
                BindingManager.RestoreBinding<T>();
                BindingManager.ApplyBinding<T>();
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "GearSwitcher.Scheduling.cs");
        }
    }

    private static void ScheduleDelayedSoulBindingIndicatorResync(bool soulApplied)
    {
        int generation = coroutineGeneration;
        int token = ++soulBindingHudResyncToken;
        _ = GlobalCoroutineExecutor.Start(WaitForSoulBindingHudEventAndResync(generation, token, soulApplied));
    }

    private static void ScheduleBindingHudResync(bool includeSoul = true)
    {
        bindingHudResyncSoulPending |= includeSoul;
        int generation = coroutineGeneration;
        int token = ++bindingHudResyncToken;
        _ = GlobalCoroutineExecutor.Start(DelayedBindingHudResync(generation, token));
    }

    private static IEnumerator DelayedBindingHudResync(int generation, int token)
    {
        for (int i = 0; i < 3; i++)
        {
            if (generation != coroutineGeneration || token != bindingHudResyncToken)
            {
                yield break;
            }

            yield return null;
        }

        if (generation != coroutineGeneration || token != bindingHudResyncToken)
        {
            yield break;
        }

        ResyncBindingHudIndicators(bindingHudResyncSoulPending);

        for (int i = 0; i < 12; i++)
        {
            if (generation != coroutineGeneration || token != bindingHudResyncToken)
            {
                yield break;
            }

            yield return null;
        }

        if (generation != coroutineGeneration || token != bindingHudResyncToken)
        {
            yield break;
        }

        ResyncBindingHudIndicators(bindingHudResyncSoulPending);

        for (int i = 0; i < 30; i++)
        {
            if (generation != coroutineGeneration || token != bindingHudResyncToken)
            {
                yield break;
            }

            yield return null;
        }

        if (generation != coroutineGeneration || token != bindingHudResyncToken)
        {
            yield break;
        }

        ResyncBindingHudIndicators(bindingHudResyncSoulPending);

        for (int i = 0; i < 90; i++)
        {
            if (generation != coroutineGeneration || token != bindingHudResyncToken)
            {
                yield break;
            }

            yield return null;
        }

        if (generation != coroutineGeneration || token != bindingHudResyncToken)
        {
            yield break;
        }

        ResyncBindingHudIndicators(bindingHudResyncSoulPending);

        for (int i = 0; i < 180; i++)
        {
            if (generation != coroutineGeneration || token != bindingHudResyncToken)
            {
                yield break;
            }

            yield return null;
        }

        if (generation != coroutineGeneration || token != bindingHudResyncToken)
        {
            yield break;
        }

        ResyncBindingHudIndicators(bindingHudResyncSoulPending);
        bindingHudResyncSoulPending = false;
    }

    private static bool IsSafeToApply()
    {
        GameManager? manager = GameManager.instance;
        if (manager == null || manager.gameState != GameState.PLAYING)
        {
            return false;
        }

        if (manager.IsInSceneTransition)
        {
            return false;
        }

        HeroController? hero = HeroController.instance;
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
            return hero.acceptingInput && !hero.controlReqlinquished;
        }

        return true;
    }
}
