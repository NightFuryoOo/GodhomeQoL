using System.Collections.Generic;
using System.Linq;
using Satchel.Futils;
using Vasi;
using Random = System.Random;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class RandomPantheons : Module
{
    private static void SyncFirstScene(BossSequence sequence)
    {
        try
        {
            CacheLiveSequenceDoors();
            if (!TryGetFirstSceneName(sequence, out string firstScene))
            {
                return;
            }
            HashSet<int> syncedDoors = new();

            if (SequenceDoors.TryGetValue(sequence, out List<BossSequenceDoor>? doors))
            {
                doors.RemoveAll(door => door == null);
                foreach (BossSequenceDoor door in doors)
                {
                    if (door == null)
                    {
                        continue;
                    }

                    _ = syncedDoors.Add(door.GetInstanceID());
                    TrySetDoorFirstScene(door, firstScene);
                }
            }

            foreach (BossSequenceDoor door in Resources.FindObjectsOfTypeAll<BossSequenceDoor>())
            {
                if (door == null || door.bossSequence != sequence)
                {
                    continue;
                }

                if (syncedDoors.Contains(door.GetInstanceID()))
                {
                    continue;
                }

                TrySetDoorFirstScene(door, firstScene);
            }
        }
        catch (Exception ex)
        {
            LogDebug($"RandomPantheons: failed to sync first scene - {ex.Message}");
        }
    }

    private static bool TryGetFirstSceneName(BossSequence sequence, out string firstScene)
    {
        firstScene = string.Empty;

        try
        {
            if (sequence == null || sequence.Count <= 0)
            {
                return false;
            }

            string sceneName = sequence.GetSceneAt(0);
            if (string.IsNullOrEmpty(sceneName))
            {
                return false;
            }

            firstScene = sceneName;
            return true;
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "RandomPantheons.FirstScene.cs");
            return false;
        }
    }

    private static void TrySetDoorFirstScene(BossSequenceDoor door, string firstScene)
    {
        int doorId = door.GetInstanceID();
        int generation = DoorFirstSceneSyncGenerations.TryGetValue(doorId, out int current) ? current + 1 : 1;
        DoorFirstSceneSyncGenerations[doorId] = generation;

        try
        {
            PlayMakerFSM? challengeFsm = door.challengeFSM;
            if (challengeFsm != null)
            {
                challengeFsm.GetVariable<FsmString>("To Scene").Value = firstScene;
                return;
            }
        }
        catch (Exception ex)
        {
            LogDebug($"RandomPantheons: failed to set door first scene immediately - {ex.Message}");
        }

        _ = GlobalCoroutineExecutor.Start(RetrySetDoorFirstScene(door, doorId, firstScene, generation));
    }

    private static IEnumerator RetrySetDoorFirstScene(BossSequenceDoor door, int doorId, string firstScene, int generation)
    {
        for (int frame = 0; frame < FirstSceneSyncRetryFrames; frame++)
        {
            if (door == null)
            {
                yield break;
            }

            if (!DoorFirstSceneSyncGenerations.TryGetValue(doorId, out int activeGeneration) || activeGeneration != generation)
            {
                yield break;
            }

            PlayMakerFSM? challengeFsm = door.challengeFSM;
            if (challengeFsm != null)
            {
                try
                {
                    challengeFsm.GetVariable<FsmString>("To Scene").Value = firstScene;
                }
                catch (Exception ex)
                {
                    LogDebug($"RandomPantheons: failed to set door first scene on retry - {ex.Message}");
                }

                yield break;
            }

            yield return null;
        }
    }
}
