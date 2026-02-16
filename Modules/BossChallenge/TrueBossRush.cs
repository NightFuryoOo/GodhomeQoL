using System.Collections.Generic;
using System.Linq;
using Satchel.Futils;
using Vasi;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class TrueBossRush : Module
{
    internal static TrueBossRush? Instance;

    [GlobalSetting] public static bool TrueBossRushPantheon1Enabled = false;
    [GlobalSetting] public static bool TrueBossRushPantheon2Enabled = false;
    [GlobalSetting] public static bool TrueBossRushPantheon3Enabled = false;
    [GlobalSetting] public static bool TrueBossRushPantheon4Enabled = false;
    [GlobalSetting] public static bool TrueBossRushPantheon5Enabled = false;

    public override bool DefaultEnabled => false;
    public override bool Hidden => true;

    private static readonly Dictionary<BossSequence, BossScene[]> OriginalSequences = new();
    private static readonly Dictionary<BossSequence, List<BossSequenceDoor>> SequenceDoors = new();

    private static readonly HashSet<string> BenchScenes = new(StringComparer.OrdinalIgnoreCase)
    {
        "GG_Spa"
    };

    private static readonly HashSet<string> TransitionScenes = new(StringComparer.OrdinalIgnoreCase)
    {
        "GG_Engine",
        "GG_Engine_Prime",
        "GG_Engine_Root",
        "GG_Unn",
        "GG_Wyrm"
    };

    private static readonly HashSet<string> VanishedHud = new(StringComparer.OrdinalIgnoreCase)
    {
        "GG_Hollow_Knight",
        "GG_Radiance"
    };

    private enum Pantheon
    {
        Unknown,
        P1,
        P2,
        P3,
        P4,
        P5
    }

    private protected override void Load()
    {
        Instance = this;
        On.BossSequenceDoor.Start += CacheSequenceDoor;
        On.BossSequenceController.SetupNewSequence += ApplySequenceToStart;
        USceneManager.activeSceneChanged += OnSceneChange;
    }

    private protected override void Unload()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        On.BossSequenceDoor.Start -= CacheSequenceDoor;
        On.BossSequenceController.SetupNewSequence -= ApplySequenceToStart;
        USceneManager.activeSceneChanged -= OnSceneChange;
    }

    private void OnSceneChange(Scene prev, Scene next)
    {
        if (Ref.GM == null || !VanishedHud.Contains(prev.name))
        {
            return;
        }

        Ref.GM.StartCoroutine(EnsureHudVisible());
    }

    private static IEnumerator EnsureHudVisible()
    {
        yield return null;

        if (GameCameras.instance != null)
        {
            GameCameras.instance.hudCanvas.LocateMyFSM("Slide Out").SendEvent("IN");
        }
    }

    private static void CacheSequenceDoor(On.BossSequenceDoor.orig_Start orig, BossSequenceDoor self)
    {
        if (self.bossSequence != null)
        {
            if (!SequenceDoors.TryGetValue(self.bossSequence, out List<BossSequenceDoor>? doors))
            {
                doors = new List<BossSequenceDoor>();
                SequenceDoors[self.bossSequence] = doors;
            }

            doors.RemoveAll(door => door == null);
            if (!doors.Contains(self))
            {
                doors.Add(self);
            }
        }

        orig(self);
    }

    private void ApplySequenceToStart(
        On.BossSequenceController.orig_SetupNewSequence orig,
        BossSequence sequence,
        BossSequenceController.ChallengeBindings bindings,
        string playerData
    )
    {
        orig(sequence, bindings, playerData);

        try
        {
            ApplySequence(sequence);
        }
        catch (Exception ex)
        {
            LogError($"TrueBossRush failed: {ex}");
        }
    }

    private void ApplySequence(BossSequence? sequence)
    {
        if (sequence == null)
        {
            return;
        }

        ref BossScene[] bossScenes = ref Mirror.GetFieldRef<BossSequence, BossScene[]>(sequence, "bossScenes");
        if (bossScenes == null || bossScenes.Length <= 1)
        {
            return;
        }

        List<BossScene> scenes = bossScenes.ToList();
        Pantheon pantheon = GetPantheon(scenes);
        CacheOriginalSequence(sequence, bossScenes);

        if (!IsPantheonEnabled(pantheon))
        {
            if (TryRestoreOriginalSequence(sequence, ref bossScenes))
            {
                SyncFirstScene(sequence);
            }

            return;
        }

        if (!TryBuildSequence(scenes, out List<BossScene>? rebuilt))
        {
            return;
        }

        bossScenes = rebuilt.ToArray();
        SyncFirstScene(sequence);
    }

    private static bool TryBuildSequence(List<BossScene> scenes, out List<BossScene> result)
    {
        result = scenes
            .Where(scene => !IsTransitionOrBench(scene.sceneName))
            .ToList();

        return result.Count > 0;
    }

    internal static void RefreshPantheon(int pantheonNumber)
    {
        if (Instance == null)
        {
            return;
        }

        Pantheon pantheon = pantheonNumber switch
        {
            1 => Pantheon.P1,
            2 => Pantheon.P2,
            3 => Pantheon.P3,
            4 => Pantheon.P4,
            5 => Pantheon.P5,
            _ => Pantheon.Unknown
        };

        if (pantheon == Pantheon.Unknown)
        {
            return;
        }

        BossSequence[] sequences = SequenceDoors.Keys.ToArray();
        foreach (BossSequence sequence in sequences)
        {
            try
            {
                ref BossScene[] bossScenes = ref Mirror.GetFieldRef<BossSequence, BossScene[]>(sequence, "bossScenes");
                if (bossScenes == null || bossScenes.Length <= 1)
                {
                    continue;
                }

                List<BossScene> scenes = bossScenes.ToList();
                if (GetPantheon(scenes) != pantheon)
                {
                    continue;
                }

                Instance.ApplySequence(sequence);
            }
            catch
            {
                // Ignore refresh failures for partially loaded/changed sequences.
            }
        }
    }

    private static void CacheOriginalSequence(BossSequence sequence, BossScene[] scenes)
    {
        if (!OriginalSequences.ContainsKey(sequence))
        {
            OriginalSequences[sequence] = scenes.ToArray();
        }
    }

    private static bool TryRestoreOriginalSequence(BossSequence sequence, ref BossScene[] bossScenes)
    {
        if (!OriginalSequences.TryGetValue(sequence, out BossScene[]? original))
        {
            return false;
        }

        bossScenes = original.ToArray();
        return true;
    }

    private static Pantheon GetPantheon(IReadOnlyList<BossScene> scenes)
    {
        HashSet<string> names = new(scenes.Select(scene => scene.sceneName), StringComparer.OrdinalIgnoreCase);

        if (names.Contains("GG_Wyrm")
            || names.Contains("GG_Radiance")
            || names.Contains("GG_Engine_Root")
            || names.Contains("GG_Grimm_Nightmare"))
        {
            return Pantheon.P5;
        }

        if (names.Contains("GG_Engine_Prime") || names.Contains("GG_Hollow_Knight"))
        {
            return Pantheon.P4;
        }

        if (names.Contains("GG_Sly"))
        {
            return Pantheon.P3;
        }

        if (names.Contains("GG_Painter"))
        {
            return Pantheon.P2;
        }

        if (names.Contains("GG_Nailmasters"))
        {
            return Pantheon.P1;
        }

        return Pantheon.Unknown;
    }

    private static bool IsPantheonEnabled(Pantheon pantheon)
    {
        return pantheon switch
        {
            Pantheon.P1 => TrueBossRushPantheon1Enabled,
            Pantheon.P2 => TrueBossRushPantheon2Enabled,
            Pantheon.P3 => TrueBossRushPantheon3Enabled,
            Pantheon.P4 => TrueBossRushPantheon4Enabled,
            Pantheon.P5 => TrueBossRushPantheon5Enabled,
            _ => false
        };
    }

    private static bool IsTransitionOrBench(string sceneName) =>
        BenchScenes.Contains(sceneName) || TransitionScenes.Contains(sceneName);

    private static void SyncFirstScene(BossSequence sequence)
    {
        try
        {
            BossSequenceControllerR.bossIndex = -1;
            BossSequenceControllerR.IncrementBossIndex();

            if (SequenceDoors.TryGetValue(sequence, out List<BossSequenceDoor>? doors))
            {
                int firstSceneIndex = BossSequenceController.BossIndex;
                string firstScene = sequence.GetSceneAt(firstSceneIndex);

                doors.RemoveAll(door => door == null);
                foreach (BossSequenceDoor door in doors)
                {
                    if (door.challengeFSM == null)
                    {
                        continue;
                    }

                    door.challengeFSM.GetVariable<FsmString>("To Scene").Value = firstScene;
                }
            }
        }
        catch
        {
            // Ignore sync failures.
        }
    }
}
