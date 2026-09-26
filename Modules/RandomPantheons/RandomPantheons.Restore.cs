using System.Collections.Generic;
using System.Linq;
using Satchel.Futils;
using Vasi;
using Random = System.Random;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class RandomPantheons : Module
{
    private static void ClearSequenceCaches()
    {
        OriginalSequences.Clear();
        SequenceDoors.Clear();
    }

    private static void PruneSequenceCaches()
    {
        foreach (BossSequence sequence in OriginalSequences.Keys.ToArray())
        {
            if (sequence == null)
            {
                if (sequence is not null)
                {
                    _ = OriginalSequences.Remove(sequence);
                }
            }
        }

        foreach ((BossSequence sequence, List<BossSequenceDoor> doors) in SequenceDoors.ToArray())
        {
            if (sequence == null)
            {
                if (sequence is not null)
                {
                    _ = SequenceDoors.Remove(sequence);
                }
                continue;
            }

            doors.RemoveAll(door => door == null);
            if (doors.Count == 0)
            {
                _ = SequenceDoors.Remove(sequence);
            }
        }
    }

    private static void RestoreCachedSequences()
    {
        PruneSequenceCaches();

        foreach ((BossSequence sequence, BossScene[] original) in OriginalSequences.ToArray())
        {
            if (sequence == null || original == null || original.Length == 0)
            {
                continue;
            }

            try
            {
                if (SetBossScenes(sequence, original.ToArray()))
                {
                    SyncFirstScene(sequence);
                }
            }
            catch (Exception swallowed)
            {
                LogSuppressed(swallowed, "RandomPantheons.Restore.cs");
            }
        }
    }

    private static void RestoreAllKnownSequencesOnDisable()
    {
        foreach (BossSequence sequence in EnumerateKnownSequences().ToArray())
        {
            TryRestoreDisabledSequence(sequence);
        }

        RestoreCachedSequences();
    }

    private static void CacheSequenceDoor(On.BossSequenceDoor.orig_Start orig, BossSequenceDoor self)
    {
        orig(self);

        CacheSequenceDoorEntry(self);
        if (Instance == null || self.bossSequence == null || PantheonSequenceCompatibility.ShouldSkipRandomPantheons())
        {
            return;
        }

        try
        {
            if (AnyPantheonEnabled)
            {
                Instance.ApplySequence(self.bossSequence);
            }
            else
            {
                TryRestoreDisabledSequence(self.bossSequence);
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "RandomPantheons.Restore.cs");
        }
    }

    private static void CacheLiveSequenceDoors()
    {
        try
        {
            foreach (BossSequenceDoor door in UObject.FindObjectsOfType<BossSequenceDoor>())
            {
                CacheSequenceDoorEntry(door);
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "RandomPantheons.Restore.cs");
        }
    }

    private static void CacheSequenceDoorEntry(BossSequenceDoor? door)
    {
        if (door == null || door.bossSequence == null)
        {
            return;
        }

        if (!SequenceDoors.TryGetValue(door.bossSequence, out List<BossSequenceDoor>? doors))
        {
            doors = new List<BossSequenceDoor>();
            SequenceDoors[door.bossSequence] = doors;
        }

        doors.RemoveAll(cachedDoor => cachedDoor == null);
        if (!doors.Contains(door))
        {
            doors.Add(door);
        }
    }

    internal static void ForceRestoreNow()
    {
        RestoreAllKnownSequencesOnDisable();
    }

    private static IEnumerable<BossSequence> EnumerateKnownSequences()
    {
        PruneSequenceCaches();
        CacheLiveSequenceDoors();

        HashSet<BossSequence> knownSequences = new();
        foreach (BossSequence sequence in SequenceDoors.Keys)
        {
            if (sequence != null)
            {
                _ = knownSequences.Add(sequence);
            }
        }

        foreach (BossSequence sequence in OriginalSequences.Keys)
        {
            if (sequence != null)
            {
                _ = knownSequences.Add(sequence);
            }
        }

        try
        {
            foreach (BossSequence sequence in Resources.FindObjectsOfTypeAll<BossSequence>())
            {
                if (sequence != null)
                {
                    _ = knownSequences.Add(sequence);
                }
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "RandomPantheons.Restore.cs");
        }

        return knownSequences;
    }

    private static void TryRestoreDisabledSequence(BossSequence? sequence)
    {
        if (sequence == null)
        {
            return;
        }

        try
        {
            BossScene[]? bossScenes = GetBossScenes(sequence);
            if (bossScenes == null || bossScenes.Length <= 1)
            {
                return;
            }

            List<BossScene> scenes = bossScenes.ToList();
            Pantheon pantheon = GetPantheon(scenes);
            _ = TryApplyRestoreOrDefault(sequence, pantheon, bossScenes);
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "RandomPantheons.Restore.cs");
        }
    }

    private static bool TryApplyRestoreOrDefault(BossSequence sequence, Pantheon pantheon, BossScene[] currentBossScenes)
    {
        List<BossScene> scenes = currentBossScenes.ToList();
        bool applied = false;
        BossScene[]? nextScenes = null;

        if (!TryApplyDefaultSequence(scenes, pantheon, out BossScene[] defaultScenes))
        {
            if (TryRestoreOriginalSequence(sequence, out BossScene[] restored))
            {
                nextScenes = restored;
                applied = true;
            }
        }
        else
        {
            nextScenes = defaultScenes;
            applied = true;
        }

        if (applied && nextScenes != null && SetBossScenes(sequence, nextScenes))
        {
            SyncFirstScene(sequence);
            return true;
        }

        return false;
    }

    private static void CacheOriginalSequence(BossSequence seq, BossScene[] scenes)
    {
        if (!OriginalSequences.ContainsKey(seq))
        {
            OriginalSequences[seq] = scenes.ToArray();
        }
    }

    private static bool TryRestoreOriginalSequence(BossSequence seq, out BossScene[] restored)
    {
        if (OriginalSequences.TryGetValue(seq, out BossScene[]? original))
        {
            restored = original.ToArray();
            return true;
        }

        restored = Array.Empty<BossScene>();
        return false;
    }
}
