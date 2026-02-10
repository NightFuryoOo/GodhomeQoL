using System.Collections.Generic;
using System.Linq;
using Satchel.Futils;
using Vasi;
using Random = System.Random;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class RandomPantheons : Module
{
    internal static RandomPantheons? Instance;

    [GlobalSetting] public static bool Pantheon1Enabled = false;
    [GlobalSetting] public static bool Pantheon2Enabled = false;
    [GlobalSetting] public static bool Pantheon3Enabled = false;
    [GlobalSetting] public static bool Pantheon4Enabled = false;
    [GlobalSetting] public static bool Pantheon5Enabled = false;

    public override bool DefaultEnabled => false;

    private static readonly Dictionary<BossSequence, BossScene[]> OriginalSequences = new();
    private static readonly Dictionary<BossSequence, List<BossSequenceDoor>> SequenceDoors = new();

    private static readonly Dictionary<Pantheon, string[]> DefaultPantheonOrder = new()
    {
        {
            Pantheon.P1,
            new[]
            {
                "GG_Vengefly",
                "GG_Gruz_Mother",
                "GG_False_Knight",
                "GG_Mega_Moss_Charger",
                "GG_Hornet_1",
                "GG_Spa",
                "GG_Ghost_Gorb",
                "GG_Dung_Defender",
                "GG_Mage_Knight",
                "GG_Brooding_Mawlek",
                "GG_Engine",
                "GG_Nailmasters"
            }
        },
        {
            Pantheon.P2,
            new[]
            {
                "GG_Ghost_Xero",
                "GG_Crystal_Guardian",
                "GG_Soul_Master",
                "GG_Oblobbles",
                "GG_Mantis_Lords",
                "GG_Spa",
                "GG_Ghost_Marmu",
                "GG_Nosk",
                "GG_Flukemarm",
                "GG_Broken_Vessel",
                "GG_Engine",
                "GG_Painter"
            }
        },
        {
            Pantheon.P3,
            new[]
            {
                "GG_Hive_Knight",
                "GG_Ghost_Hu",
                "GG_Collector",
                "GG_God_Tamer",
                "GG_Grimm",
                "GG_Spa",
                "GG_Ghost_Galien",
                "GG_Grey_Prince_Zote",
                "GG_Uumuu",
                "GG_Hornet_2",
                "GG_Engine",
                "GG_Sly"
            }
        },
        {
            Pantheon.P4,
            new[]
            {
                "GG_Crystal_Guardian_2",
                "GG_Lost_Kin",
                "GG_Ghost_No_Eyes",
                "GG_Traitor_Lord",
                "GG_White_Defender",
                "GG_Spa",
                "GG_Failed_Champion",
                "GG_Ghost_Markoth",
                "GG_Watcher_Knights",
                "GG_Soul_Tyrant",
                "GG_Engine_Prime",
                "GG_Hollow_Knight"
            }
        },
        {
            Pantheon.P5,
            new[]
            {
                "GG_Vengefly_V",
                "GG_Gruz_Mother_V",
                "GG_False_Knight",
                "GG_Mega_Moss_Charger",
                "GG_Hornet_1",
                "GG_Engine",
                "GG_Ghost_Gorb_V",
                "GG_Dung_Defender",
                "GG_Mage_Knight_V",
                "GG_Brooding_Mawlek_V",
                "GG_Nailmasters",
                "GG_Spa",
                "GG_Ghost_Xero_V",
                "GG_Crystal_Guardian",
                "GG_Soul_Master",
                "GG_Oblobbles",
                "GG_Mantis_Lords_V",
                "GG_Spa",
                "GG_Ghost_Marmu_V",
                "GG_Flukemarm",
                "GG_Broken_Vessel",
                "GG_Ghost_Galien",
                "GG_Painter",
                "GG_Spa",
                "GG_Hive_Knight",
                "GG_Ghost_Hu",
                "GG_Collector_V",
                "GG_God_Tamer",
                "GG_Grimm",
                "GG_Spa",
                "GG_Unn",
                "GG_Watcher_Knights",
                "GG_Uumuu_V",
                "GG_Nosk_Hornet",
                "GG_Sly",
                "GG_Hornet_2",
                "GG_Spa",
                "GG_Crystal_Guardian_2",
                "GG_Lost_Kin",
                "GG_Ghost_No_Eyes_V",
                "GG_Traitor_Lord",
                "GG_White_Defender",
                "GG_Spa",
                "GG_Engine_Root",
                "GG_Soul_Tyrant",
                "GG_Ghost_Markoth_V",
                "GG_Grey_Prince_Zote",
                "GG_Failed_Champion",
                "GG_Grimm_Nightmare",
                "GG_Spa",
                "GG_Wyrm",
                "GG_Hollow_Knight",
                "GG_Radiance"
            }
        }
    };

    private static readonly HashSet<string> InvalidFirst = new(StringComparer.OrdinalIgnoreCase)
    {
        "GG_Unn",
        "GG_Wyrm",
        "GG_Engine",
        "GG_Engine_Prime",
        "GG_Engine_Root",
        "GG_Spa",
        "GG_Gruz_Mother",
        "GG_Gruz_Mother_V"
    };

    private static readonly HashSet<string> InvalidLast = new(StringComparer.OrdinalIgnoreCase)
    {
        "GG_Unn",
        "GG_Wyrm",
        "GG_Engine",
        "GG_Engine_Prime",
        "GG_Engine_Root",
        "GG_Spa"
    };

    private static readonly HashSet<string> VanishedHud = new(StringComparer.OrdinalIgnoreCase)
    {
        "GG_Hollow_Knight",
        "GG_Radiance"
    };

    private readonly Random rand = new();

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
        On.PlayMakerFSM.Start += ModifyRadiance;
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
        On.PlayMakerFSM.Start -= ModifyRadiance;
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

    private static void ModifyRadiance(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (self is { name: "Absolute Radiance", FsmName: "Control" })
        {
            self.GetAction<SetStaticVariable>("Ending Scene", 1).setValue.boolValue = false;
        }
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
            LogError($"RandomPantheons failed: {ex}");
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

        bool applied = false;
        if (!IsPantheonEnabled(pantheon))
        {
            if (!TryApplyDefaultSequence(scenes, pantheon, ref bossScenes))
            {
                applied = TryRestoreOriginalSequence(sequence, ref bossScenes);
            }
            else
            {
                applied = true;
            }

            if (applied)
            {
                SyncFirstScene(sequence);
            }

            return;
        }

        if (!TryBuildSequence(scenes, pantheon, out List<BossScene>? rebuilt))
        {
            return;
        }

        bossScenes = rebuilt.ToArray();
        SyncFirstScene(sequence);
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
                // ignore refresh failures
            }
        }
    }

    private static void CacheOriginalSequence(BossSequence seq, BossScene[] scenes)
    {
        if (!OriginalSequences.ContainsKey(seq))
        {
            OriginalSequences[seq] = scenes.ToArray();
        }
    }

    private static bool TryRestoreOriginalSequence(BossSequence seq, ref BossScene[] bossScenes)
    {
        if (OriginalSequences.TryGetValue(seq, out BossScene[]? original))
        {
            bossScenes = original.ToArray();
            return true;
        }

        return false;
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
            Pantheon.P1 => Pantheon1Enabled,
            Pantheon.P2 => Pantheon2Enabled,
            Pantheon.P3 => Pantheon3Enabled,
            Pantheon.P4 => Pantheon4Enabled,
            Pantheon.P5 => Pantheon5Enabled,
            _ => false
        };
    }

    private static bool IsInvalidSequence(IReadOnlyList<BossScene> scenes)
    {
        if (InvalidFirst.Contains(scenes[0].sceneName))
        {
            return true;
        }

        if (InvalidLast.Contains(scenes[scenes.Count - 1].sceneName))
        {
            return true;
        }

        for (int i = 0; i < scenes.Count - 1; i++)
        {
            if (scenes[i].sceneName == "GG_Spa" && scenes[i + 1].sceneName == "GG_Spa")
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryApplyDefaultSequence(List<BossScene> scenes, Pantheon pantheon, ref BossScene[] bossScenes)
    {
        if (pantheon != Pantheon.Unknown && TryBuildDefaultSequence(pantheon, scenes, ref bossScenes))
        {
            return true;
        }

        foreach (Pantheon candidate in DefaultPantheonOrder.Keys)
        {
            if (TryBuildDefaultSequence(candidate, scenes, ref bossScenes))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryBuildDefaultSequence(Pantheon pantheon, List<BossScene> scenes, ref BossScene[] bossScenes)
    {
        if (!DefaultPantheonOrder.TryGetValue(pantheon, out string[]? defaultOrder))
        {
            return false;
        }

        if (scenes.Count != defaultOrder.Length)
        {
            return false;
        }

        BossScene? spaTemplate = scenes.FirstOrDefault(scene => IsSpa(scene.sceneName));
        if (spaTemplate == null)
        {
            return false;
        }

        Dictionary<string, BossScene> sceneMap = new(StringComparer.OrdinalIgnoreCase);
        foreach (BossScene scene in scenes)
        {
            if (IsSpa(scene.sceneName))
            {
                continue;
            }

            if (!sceneMap.ContainsKey(scene.sceneName))
            {
                sceneMap[scene.sceneName] = scene;
            }
        }

        List<BossScene> rebuilt = new(defaultOrder.Length);
        foreach (string name in defaultOrder)
        {
            if (IsSpa(name))
            {
                rebuilt.Add(spaTemplate);
                continue;
            }

            if (!sceneMap.TryGetValue(name, out BossScene scene))
            {
                return false;
            }

            rebuilt.Add(scene);
        }

        bossScenes = rebuilt.ToArray();
        return true;
    }

    private bool TryBuildSequence(List<BossScene> scenes, Pantheon pantheon, out List<BossScene> result)
    {
        result = new List<BossScene>(scenes.Count);
        if (pantheon == Pantheon.Unknown)
        {
            return false;
        }

        int targetSpaCount = pantheon == Pantheon.P5 ? 7 : 1;
        BossScene? spaTemplate = scenes.FirstOrDefault(scene => IsSpa(scene.sceneName));
        if (spaTemplate == null)
        {
            return false;
        }

        Dictionary<string, BossScene> uniqueBosses = new(StringComparer.OrdinalIgnoreCase);
        foreach (BossScene scene in scenes)
        {
            if (IsSpa(scene.sceneName))
            {
                continue;
            }

            if (!uniqueBosses.ContainsKey(scene.sceneName))
            {
                uniqueBosses[scene.sceneName] = scene;
            }
        }

        List<BossScene> bosses = uniqueBosses.Values.ToList();
        if (bosses.Count < 2 || targetSpaCount > bosses.Count - 1)
        {
            return false;
        }

        const int maxAttempts = 200;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            bosses = bosses.OrderBy(_ => rand.Next()).ToList();
            if (InvalidFirst.Contains(bosses[0].sceneName) || InvalidLast.Contains(bosses[bosses.Count - 1].sceneName))
            {
                continue;
            }

            List<int> gaps = Enumerable.Range(0, bosses.Count - 1).OrderBy(_ => rand.Next()).ToList();
            HashSet<int> selectedGaps = new(gaps.Take(targetSpaCount));

            result.Clear();
            for (int i = 0; i < bosses.Count; i++)
            {
                result.Add(bosses[i]);
                if (selectedGaps.Contains(i))
                {
                    result.Add(spaTemplate);
                }
            }

            if (!IsInvalidSequence(result))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsSpa(string sceneName) => sceneName.Equals("GG_Spa", StringComparison.OrdinalIgnoreCase);

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
            // ignore sync failures
        }
    }
}
