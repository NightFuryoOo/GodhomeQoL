using InControl;
using Satchel.BetterMenus;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class ShowHPOnDeath : Module
{
    private void OnPlayerDead()
    {
        if (!shouldTrackBosses)
        {
            return;
        }

        ShowBossDisplayForScene();
    }

    private void ShowBossDisplayForScene()
    {
        string displayText = BuildDisplayText();
        DisplayWithAutoHide(displayText);
    }

    private string BuildDisplayText()
    {
        var snapshot = new List<(string Key, string Name, int HP)>();
        bool suppressCollector = IsCollectorImmortalTrackingSuppressed();
        bool suppressZote = IsZoteImmortalTrackingSuppressed();
        foreach (string bossKey in trackedBossOrder)
        {
            if (!trackedBossesByKey.TryGetValue(bossKey, out BossState? state))
            {
                continue;
            }

            if (suppressCollector && string.Equals(state.DisplayName, "The Collector", StringComparison.Ordinal))
            {
                continue;
            }

            if (suppressZote && string.Equals(state.DisplayName, "Grey Prince Zote", StringComparison.Ordinal))
            {
                continue;
            }

            int hp = Math.Max(state.CurrentHP, 0);
            if (hp <= 0)
            {
                continue;
            }

            snapshot.Add((bossKey, state.DisplayName, hp));
            if (!personalBestByBoss.TryGetValue(bossKey, out int best) || hp < best)
            {
                personalBestByBoss[bossKey] = hp;
            }
        }

        string hideKey = Settings.Keybinds.Hide.UnfilteredBindings.Count > 0
            ? Settings.Keybinds.Hide.UnfilteredBindings[0].Name
            : "Unbound";
        string displayText = $"Press [{hideKey}] to hide\n";

        Dictionary<string, int> nameInstances = [];
        foreach (var (bossKey, name, currentHP) in snapshot)
        {
            int count = nameInstances.TryGetValue(name, out int existingCount) ? existingCount + 1 : 1;
            nameInstances[name] = count;
            string finalName = count > 1 ? $"{name} ({count})" : name;

            if (phaseBosses.Contains(name) && phaseTrackers.TryGetValue(name, out PhaseTracker? tracker))
            {
                int fallbackInitialHp = initialHPs.TryGetValue(bossKey, out int initialForPhase) ? initialForPhase : 0;
                string pbText = personalBestByBoss.TryGetValue(bossKey, out int pb) ? pb.ToString() : "-";
                displayText += $"[{finalName}]\n";
                displayText += BuildPhaseLines(name, tracker, fallbackInitialHp);
                if (Settings.ShowPB)
                {
                    displayText += $"PB: {pbText}\n";
                }
                continue;
            }

            if (initialHPs.TryGetValue(bossKey, out int initialHP))
            {
                string pbText = personalBestByBoss.TryGetValue(bossKey, out int pb) ? pb.ToString() : "-";
                displayText += $"[{finalName}]\nHP: {currentHP} / {initialHP}\n";
                if (Settings.ShowPB)
                {
                    displayText += $"PB: {pbText}\n";
                }
            }
            else
            {
                displayText += $"[{finalName}]\nHP: {currentHP}\n";
            }
        }

        return displayText;
    }

    private void DisplayWithAutoHide(string text)
    {
        int displayGeneration = ++autoHideGeneration;

        lastDisplayText = text;
        UpdateDisplay(text);

        if (Settings.HideAfter10Sec && !string.IsNullOrWhiteSpace(text))
        {
            float seconds = Math.Max(1f, Math.Min(10f, Settings.HudFadeSeconds));
            _ = GlobalCoroutineExecutor.Start(HideDisplayAfterDelay(displayGeneration, seconds));
        }
    }

    private static IEnumerator HideDisplayAfterDelay(int displayGeneration, float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        if (displayGeneration != autoHideGeneration)
        {
            yield break;
        }

        UpdateDisplay("");
    }

    private void CreateUI()
    {
        if (ShowHPOnDeathDisplay.Instance == null)
        {
            _ = new ShowHPOnDeathDisplay();
        }
    }

    private static void EnsureDisplayUi()
    {
        if (ShowHPOnDeathDisplay.Instance != null || activeInstance == null || !activeInstance.Loaded)
        {
            return;
        }

        activeInstance.CreateUI();
    }

    private static void UpdateDisplay(string text)
    {
        if (ShowHPOnDeathDisplay.Instance == null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                isHudVisible = false;
                return;
            }

            EnsureDisplayUi();
        }

        ShowHPOnDeathDisplay.Instance?.Display(text);
        isHudVisible = !string.IsNullOrWhiteSpace(text);
    }

    private void ToggleHud()
    {
        if (isHudVisible)
        {
            CancelAutoHide();
            UpdateDisplay("");
            return;
        }

        if (string.IsNullOrWhiteSpace(lastDisplayText))
        {
            return;
        }

        DisplayWithAutoHide(lastDisplayText);
    }
}
