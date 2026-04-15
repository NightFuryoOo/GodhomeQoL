using System.IO;
using System.Text;

namespace GodhomeQoL.Modules.QoL;

internal sealed class TeleportKitLogger : IDisposable
{
    private readonly string logFilePath;
    private readonly object sync = new();
    private StreamWriter? writer;

    internal TeleportKitLogger()
    {
        logFilePath = Path.Combine(Application.persistentDataPath, "QoLTeleportKit.log");
        TryOpenWriter();
    }

    internal void Write(string message)
    {
        try
        {
            lock (sync)
            {
                if (writer == null)
                {
                    TryOpenWriter();
                }

                writer?.WriteLine($"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] {message}");
            }
        }
        catch (Exception e)
        {
            Logger.LogError($"[TeleportKit] Failed to write log: {e.Message}");
        }
    }

    public void Dispose()
    {
        lock (sync)
        {
            writer?.Dispose();
            writer = null;
        }
    }

    private void TryOpenWriter()
    {
        try
        {
            var stream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
        }
        catch (Exception e)
        {
            writer = null;
            Logger.LogError($"[TeleportKit] Failed to open log writer: {e.Message}");
        }
    }
}
