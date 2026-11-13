namespace GodhomeQoL.Modules.BossChallenge;

internal sealed class CustomPantheonBoss
{
    internal CustomPantheonBoss(string scene, string key, string image, int pantheon)
    {
        Scene = scene;
        Key = key;
        Image = image;
        Pantheon = pantheon;
    }

    internal string Scene { get; }

    internal string Key { get; }

    internal string Image { get; }

    internal int Pantheon { get; }
}

internal static class CustomPantheonCatalog
{
    internal const string BenchScene = "GG_Spa";
    internal const int PantheonCount = 5;
    internal const int MaxChainLength = 420;

    private static List<CustomPantheonBoss>[]? byPantheon;
    private static Dictionary<string, CustomPantheonBoss>? byScene;
    private static HashSet<string>? multiVersionKeys;

    internal static IReadOnlyList<CustomPantheonBoss> ForPantheon(int pantheon)
    {
        EnsureBuilt();
        return pantheon >= 1 && pantheon <= PantheonCount ? byPantheon![pantheon - 1] : Array.Empty<CustomPantheonBoss>();
    }

    internal static bool TryGet(string? scene, out CustomPantheonBoss boss)
    {
        EnsureBuilt();
        return byScene!.TryGetValue(scene ?? string.Empty, out boss!);
    }

    internal static bool IsBench(string? scene) => string.Equals(scene, BenchScene, StringComparison.Ordinal);

    internal static bool IsKnown(string? scene) => IsBench(scene) || TryGet(scene, out _);

    internal static string VersionTag(string? scene)
    {
        EnsureBuilt();
        return TryGet(scene, out CustomPantheonBoss boss) && multiVersionKeys!.Contains(boss.Key) ? $"P{boss.Pantheon}" : string.Empty;
    }

    internal static int CountBosses(IEnumerable<string>? scenes) =>
        (scenes ?? Array.Empty<string>()).Count(scene => !IsBench(scene) && TryGet(scene, out _));

    internal static List<string> Sanitize(IEnumerable<string>? scenes) =>
        SanitizeFights((scenes ?? Array.Empty<string>()).Select(scene => new CustomPantheonFight { Scene = scene }))
            .Select(fight => fight.Scene)
            .ToList();

    internal static List<CustomPantheonFight> SanitizeFights(IEnumerable<CustomPantheonFight>? fights)
    {
        List<CustomPantheonFight> result = new();
        foreach (CustomPantheonFight fight in fights ?? Array.Empty<CustomPantheonFight>())
        {
            if (fight == null || !IsKnown(fight.Scene))
            {
                continue;
            }

            if (IsBench(fight.Scene) && (result.Count == 0 || IsBench(result[result.Count - 1].Scene)))
            {
                continue;
            }

            result.Add(fight);
            if (result.Count >= MaxChainLength)
            {
                break;
            }
        }

        while (result.Count > 0 && IsBench(result[result.Count - 1].Scene))
        {
            result.RemoveAt(result.Count - 1);
        }

        return result;
    }

    internal static int CountBosses(IEnumerable<CustomPantheonFight>? fights) =>
        CountBosses((fights ?? Array.Empty<CustomPantheonFight>()).Select(fight => fight.Scene));

    private static void EnsureBuilt()
    {
        if (byScene != null)
        {
            return;
        }

        Dictionary<string, CustomPantheonBoss> scenes = new(StringComparer.Ordinal);
        Dictionary<string, HashSet<string>> scenesByKey = new(StringComparer.Ordinal);
        List<CustomPantheonBoss>[] lists = new List<CustomPantheonBoss>[PantheonCount];

        for (int pantheon = 1; pantheon <= PantheonCount; pantheon++)
        {
            List<CustomPantheonBoss> list = new();
            HashSet<string> seen = new(StringComparer.Ordinal);
            foreach (string scene in RandomPantheons.GetVanillaOrder(pantheon))
            {
                if (!seen.Add(scene) || !Tools.BossHistory.TryGetBossForScene(scene, out string key))
                {
                    continue;
                }

                string image = Tools.BossHistory.Find(key)?.Image ?? string.Empty;
                list.Add(new CustomPantheonBoss(scene, key, image, pantheon));
                if (!scenes.ContainsKey(scene))
                {
                    scenes[scene] = new CustomPantheonBoss(scene, key, image, pantheon);
                }

                if (!scenesByKey.TryGetValue(key, out HashSet<string>? keyScenes))
                {
                    keyScenes = new HashSet<string>(StringComparer.Ordinal);
                    scenesByKey[key] = keyScenes;
                }

                keyScenes.Add(scene);
            }

            lists[pantheon - 1] = list;
        }

        multiVersionKeys = new HashSet<string>(scenesByKey.Where(pair => pair.Value.Count > 1).Select(pair => pair.Key), StringComparer.Ordinal);
        byPantheon = lists;
        byScene = scenes;
    }
}
