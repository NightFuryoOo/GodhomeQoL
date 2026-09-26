using InControl;
using Satchel.BetterMenus;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class ShowHPOnDeath : Module
{
    private static PhaseTracker GetPhaseTracker(string displayName)
    {
        if (!phaseTrackers.TryGetValue(displayName, out PhaseTracker? tracker))
        {
            tracker = new PhaseTracker();
            phaseTrackers[displayName] = tracker;
        }
        return tracker;
    }

    private static void UpdatePhaseTracker(string displayName, int hp)
    {
        PhaseTracker tracker = GetPhaseTracker(displayName);
        int phaseIndex = Math.Min(tracker.CurrentPhase, 2);
        int clampedHp = Math.Max(hp, 0);

        if (tracker.MaxHP[phaseIndex] == 0)
        {
            tracker.MaxHP[phaseIndex] = clampedHp;
            tracker.CurrentHP[phaseIndex] = clampedHp;
            tracker.LastHP = clampedHp;
            return;
        }

        if (clampedHp > tracker.MaxHP[phaseIndex] && !tracker.PhaseDamaged[phaseIndex])
        {
            tracker.MaxHP[phaseIndex] = clampedHp;
        }

        if (tracker.LastHP >= 0 && clampedHp > tracker.LastHP && tracker.PhaseDamaged[phaseIndex] && phaseIndex < 2)
        {
            tracker.CurrentHP[phaseIndex] = 0;
            tracker.PhaseDamaged[phaseIndex] = true;
            tracker.CurrentPhase++;
            phaseIndex = tracker.CurrentPhase;
            tracker.MaxHP[phaseIndex] = clampedHp;
            tracker.CurrentHP[phaseIndex] = clampedHp;
            tracker.LastHP = clampedHp;
            return;
        }

        tracker.CurrentHP[phaseIndex] = Math.Min(clampedHp, tracker.MaxHP[phaseIndex]);
        if (tracker.CurrentHP[phaseIndex] < tracker.MaxHP[phaseIndex])
        {
            tracker.PhaseDamaged[phaseIndex] = true;
        }
        tracker.LastHP = clampedHp;
    }

    private static bool TryGetPhaseBossForScene(out string bossName)
    {
        string scene = USceneManager.GetActiveScene().name;
        return phaseBossScenes.TryGetValue(scene, out bossName);
    }

    private static void UpdateHeadCandidate(HealthManager self)
    {
        if (!TryGetPhaseBossForScene(out string bossName))
        {
            return;
        }

        if (BossNames.Map.TryGetValue(self.gameObject.name, out string mappedName) && mappedName == bossName)
        {
            return;
        }

        int hp = Math.Max(self.hp, 0);
        string objName = self.gameObject.name;
        bool nameLooksHead = objName.IndexOf("Head", StringComparison.OrdinalIgnoreCase) >= 0
            || objName.IndexOf("Hornhead", StringComparison.OrdinalIgnoreCase) >= 0;

        if (nameLooksHead)
        {
            headHPs[bossName] = hp;
            headConfirmed.Add(bossName);
            return;
        }

        if (hp == 0 || headConfirmed.Contains(bossName))
        {
            return;
        }

        if (!headHPs.TryGetValue(bossName, out int existing) || hp < existing)
        {
            headHPs[bossName] = hp;
        }
    }

    private static string BuildPhaseLines(string bossName, PhaseTracker tracker, int fallbackInitialHp)
    {
        int fallbackMax = tracker.MaxHP[Math.Min(tracker.CurrentPhase, 2)];
        if (fallbackMax == 0 && fallbackInitialHp > 0)
        {
            fallbackMax = fallbackInitialHp;
        }

        string phaseText = "";
        for (int i = 0; i < 3; i++)
        {
            int max = tracker.MaxHP[i] > 0 ? tracker.MaxHP[i] : fallbackMax;
            if (max <= 0)
            {
                continue;
            }

            int current;
            if (i < tracker.CurrentPhase)
            {
                current = 0;
            }
            else if (i == tracker.CurrentPhase)
            {
                current = Math.Min(tracker.CurrentHP[i], max);
            }
            else
            {
                current = max;
            }

            phaseText += $"HP: {current} / {max}\n";
        }

        int headHp = headHPs.TryGetValue(bossName, out int head) ? head : 0;
        phaseText += $"HP: {headHp} / 0\n";

        return phaseText;
    }
}
