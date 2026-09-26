
namespace GodhomeQoL.Utils
{
    internal static class Logger
    {
        private static readonly SimpleLogger logger = new(nameof(GodhomeQoL));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Log(string message) => logger.Log(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void LogDebug(string message) =>
#if DEBUG
            logger.Log(message);
#else
		logger.LogDebug(message);
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void LogError(string message) => logger.LogError(message);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void LogFine(string message) =>
#if DEBUG
            logger.Log(message);
#else
		logger.LogFine(message);
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void LogWarn(string message) =>
#if DEBUG
            logger.LogError(message);
#else
		logger.LogWarn(message);
#endif

        private static readonly Dictionary<string, int> suppressedCounts = new(StringComparer.Ordinal);

        internal static void LogSuppressed(
            Exception ex,
            string source,
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            try
            {
                string site = $"{source}:{line}";
                int count;
                lock (suppressedCounts)
                {
                    suppressedCounts.TryGetValue(site, out count);
                    suppressedCounts[site] = ++count;
                }

                if (!IsPowerOfTen(count))
                {
                    return;
                }

                LogWarn(count == 1
                    ? $"Suppressed exception at {site} ({member}): {ex}"
                    : $"Suppressed exception at {site} ({member}) x{count}: {ex.GetType().Name}: {ex.Message}");
            }
            catch
            {
            }
        }

        private static bool IsPowerOfTen(int value)
        {
            while (value >= 10 && value % 10 == 0)
            {
                value /= 10;
            }

            return value == 1;
        }
    }
}
