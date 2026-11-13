using System.IO;

namespace GodhomeQoL.Modules.BossChallenge;

internal static class CustomPantheonTransfer
{
    internal const string FolderName = "GodhomeQoL Chains";
    internal const string FormatName = "GodhomeQoL Custom Pantheon";
    internal const int FormatVersion = 1;
    private const long MaxFileBytes = 32L * 1024 * 1024;

    internal sealed class ChainFile
    {
        public string Format = FormatName;
        public int Version = FormatVersion;
        public string ModVersion = string.Empty;
        public List<CustomPantheonChain> Chains = new();
    }

    internal static string Folder => Path.Combine(Application.persistentDataPath, FolderName);

    internal static string Serialize(IEnumerable<CustomPantheonChain> chains)
    {
        ChainFile file = new()
        {
            ModVersion = ModInfo.Version,
            Chains = chains.Where(chain => chain != null).Select(CloneForFile).ToList()
        };
        return JsonConvert.SerializeObject(file, Formatting.Indented);
    }

    internal static List<CustomPantheonChain>? Parse(string text)
    {
        ChainFile? file;
        try
        {
            file = JsonConvert.DeserializeObject<ChainFile>(text);
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "CustomPantheonTransfer.cs/Parse");
            return null;
        }

        if (file == null || file.Chains == null || !string.Equals(file.Format, FormatName, StringComparison.Ordinal))
        {
            return null;
        }

        List<CustomPantheonChain> result = new();
        foreach (CustomPantheonChain chain in file.Chains)
        {
            if (chain == null)
            {
                continue;
            }

            chain.Selected = false;
            chain.Normalize();
            chain.Fights = CustomPantheonCatalog.SanitizeFights(chain.Fights);
            chain.Name = (chain.Name ?? string.Empty).Trim();
            result.Add(chain);
        }

        return result;
    }

    internal static bool TryExport(IEnumerable<CustomPantheonChain> chains, out string path)
    {
        path = string.Empty;
        try
        {
            Directory.CreateDirectory(Folder);
            path = Path.Combine(Folder, $"Custom Pantheon {DateTime.Now:yyyy-MM-dd HH-mm-ss}.json");
            File.WriteAllText(path, Serialize(chains));
            return true;
        }
        catch (Exception ex)
        {
            LogError($"[CustomPantheon] Export failed: {ex}");
            return false;
        }
    }

    internal static List<FileInfo> ListFiles()
    {
        try
        {
            DirectoryInfo folder = new(Folder);
            return folder.Exists
                ? folder.GetFiles("*.json").OrderByDescending(file => file.LastWriteTimeUtc).ToList()
                : new List<FileInfo>();
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "CustomPantheonTransfer.cs/List");
            return new List<FileInfo>();
        }
    }

    internal static List<CustomPantheonChain>? TryRead(string path)
    {
        try
        {
            FileInfo info = new(path);
            if (!info.Exists || info.Length > MaxFileBytes)
            {
                return null;
            }

            return Parse(File.ReadAllText(path));
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "CustomPantheonTransfer.cs/Read");
            return null;
        }
    }

    internal static void OpenFolder()
    {
        try
        {
            Directory.CreateDirectory(Folder);
            Application.OpenURL(new Uri(Folder + Path.DirectorySeparatorChar).AbsoluteUri);
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "CustomPantheonTransfer.cs/Open");
        }
    }

    private static CustomPantheonChain CloneForFile(CustomPantheonChain chain)
    {
        CustomPantheonChain copy = JsonConvert.DeserializeObject<CustomPantheonChain>(JsonConvert.SerializeObject(chain)) ?? new CustomPantheonChain();
        copy.Selected = false;
        copy.Scenes = new List<string>();
        return copy;
    }
}
