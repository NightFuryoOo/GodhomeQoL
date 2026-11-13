using System.IO;

namespace GodhomeQoL.Modules.Tools;

internal sealed class HitCounterPantheonRecord
{
    public int PbTotal = -1;
    public Dictionary<string, int> PbSplits = new(StringComparer.Ordinal);
    public Dictionary<string, int> BestSplits = new(StringComparer.Ordinal);
}

internal sealed class HitCounterFile
{
    public Dictionary<string, HitCounterPantheonRecord> Pantheons = new(StringComparer.Ordinal);
    public Dictionary<string, int> HallOfGods = new(StringComparer.Ordinal);
}

internal static class HitCounterRecords
{
    private static HitCounterFile? loaded;
    private static int loadedSlot = -1;

    private static string PantheonKey(int tier) => "P" + tier;

    private static string HallOfGodsKey(string scene, int level) => scene + "|" + level;

    internal static HitCounterPantheonRecord? GetPantheon(int tier)
    {
        HitCounterFile? file = Load(BossHistory.CurrentSlot);
        return file != null && file.Pantheons.TryGetValue(PantheonKey(tier), out HitCounterPantheonRecord? record) ? record : null;
    }

    internal static int GetHallOfGodsBest(string scene, int level)
    {
        HitCounterFile? file = Load(BossHistory.CurrentSlot);
        return file != null && file.HallOfGods.TryGetValue(HallOfGodsKey(scene, level), out int best) ? best : -1;
    }

    internal static void SavePantheonPb(int tier, int total, Dictionary<string, int> splits)
    {
        Update(file =>
        {
            HitCounterPantheonRecord record = GetOrCreate(file, tier);
            record.PbTotal = total;
            record.PbSplits = new Dictionary<string, int>(splits, StringComparer.Ordinal);
        });
    }

    internal static void SavePantheonBest(int tier, string scene, int hits)
    {
        Update(file => GetOrCreate(file, tier).BestSplits[scene] = hits);
    }

    internal static void SaveHallOfGodsBest(string scene, int level, int hits)
    {
        Update(file => file.HallOfGods[HallOfGodsKey(scene, level)] = hits);
    }

    internal static void ClearCurrentSlot()
    {
        Update(file =>
        {
            file.Pantheons.Clear();
            file.HallOfGods.Clear();
        });
    }

    internal static void DeleteSlot(int slot)
    {
        if (slot <= 0)
        {
            return;
        }

        if (slot == loadedSlot)
        {
            loaded = null;
            loadedSlot = -1;
        }

        try
        {
            string path = GetPath(slot);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception ex)
        {
            LogError($"Hit counter records of save {slot} could not be deleted: {ex.Message}");
        }
    }

    private static HitCounterPantheonRecord GetOrCreate(HitCounterFile file, int tier)
    {
        string key = PantheonKey(tier);
        if (!file.Pantheons.TryGetValue(key, out HitCounterPantheonRecord? record) || record == null)
        {
            record = new HitCounterPantheonRecord();
            file.Pantheons[key] = record;
        }

        record.PbSplits ??= new Dictionary<string, int>(StringComparer.Ordinal);
        record.BestSplits ??= new Dictionary<string, int>(StringComparer.Ordinal);
        return record;
    }

    private static void Update(Action<HitCounterFile> change)
    {
        int slot = BossHistory.CurrentSlot;
        HitCounterFile? file = Load(slot);
        if (file == null)
        {
            return;
        }

        change(file);
        Save(slot, file);
    }

    private static string GetPath(int slot) => Path.Combine(Application.persistentDataPath, $"{ModInfo.Name}.HitCounter.user{slot}.json");

    private static HitCounterFile? Load(int slot)
    {
        if (slot <= 0)
        {
            return null;
        }

        if (slot == loadedSlot && loaded != null)
        {
            return loaded;
        }

        HitCounterFile file = new();
        string path = GetPath(slot);
        try
        {
            if (File.Exists(path))
            {
                HitCounterFile? read = JsonConvert.DeserializeObject<HitCounterFile>(File.ReadAllText(path));
                if (read != null)
                {
                    file.Pantheons = new Dictionary<string, HitCounterPantheonRecord>(
                        (read.Pantheons ?? new Dictionary<string, HitCounterPantheonRecord>())
                            .Where(pair => pair.Value != null)
                            .ToDictionary(pair => pair.Key, pair => pair.Value),
                        StringComparer.Ordinal);
                    file.HallOfGods = new Dictionary<string, int>(read.HallOfGods ?? new Dictionary<string, int>(), StringComparer.Ordinal);
                }
            }
        }
        catch (Exception ex)
        {
            LogError($"Hit counter records of save {slot} could not be read: {ex.Message}");
            try
            {
                File.Copy(path, path + ".broken", true);
            }
            catch (Exception copyEx)
            {
                LogError($"Hit counter records backup failed: {copyEx.Message}");
            }
        }

        loaded = file;
        loadedSlot = slot;
        return file;
    }

    private static void Save(int slot, HitCounterFile file)
    {
        string path = GetPath(slot);
        string temp = path + ".tmp";
        try
        {
            File.WriteAllText(temp, JsonConvert.SerializeObject(file, Formatting.None));
            File.Copy(temp, path, true);
            File.Delete(temp);
        }
        catch (Exception ex)
        {
            LogError($"Hit counter records of save {slot} could not be saved: {ex.Message}");
        }
    }
}
