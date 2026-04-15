using Newtonsoft.Json;
using System.Globalization;
using System.IO;

namespace Osmi
{
    public sealed class Dict
    {
        private readonly Dictionary<string, string> data = new(StringComparer.Ordinal);

        public Dict(Assembly assembly, string resourcePrefix)
        {
            if (assembly == null || string.IsNullOrWhiteSpace(resourcePrefix))
            {
                return;
            }

            try
            {
                string normalizedPrefix = resourcePrefix.Replace('\\', '.').Trim('.');
                string langSuffix = $"{normalizedPrefix}.en.json";
                string? resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(name =>
                        name.EndsWith(langSuffix, StringComparison.OrdinalIgnoreCase)
                        || name.Equals(langSuffix, StringComparison.OrdinalIgnoreCase));

                if (resourceName == null)
                {
                    resourceName = assembly.GetManifestResourceNames()
                        .FirstOrDefault(name => name.EndsWith("en.json", StringComparison.OrdinalIgnoreCase));
                }

                if (resourceName == null)
                {
                    return;
                }

                using Stream? stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    return;
                }

                using StreamReader reader = new(stream);
                string json = reader.ReadToEnd();
                Dictionary<string, string>? parsed = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (parsed == null)
                {
                    return;
                }

                foreach ((string key, string value) in parsed)
                {
                    data[key] = value;
                }
            }
            catch
            {
                // Keep localization fallback behavior when resource loading fails.
            }
        }

        public string Localize(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            if (data.TryGetValue(key, out string? value))
            {
                return value;
            }

            return key;
        }
    }

    public static class Ref
    {
        public static HeroController? HC => HeroController.instance;
        public static GameManager? GM => GameManager.instance;
        public static GameCameras? GC => GameCameras.instance;
        public static PlayerData? PD => PlayerData.instance;
        public static SceneData? SD => SceneData.instance;
    }
}

namespace Osmi.Game
{
    public static class OsmiHooks
    {
        public static event Action<Scene, Scene>? SceneChangeHook;

        static OsmiHooks()
        {
            USceneManager.activeSceneChanged += HandleSceneChanged;
        }

        private static void HandleSceneChanged(Scene from, Scene to) => SceneChangeHook?.Invoke(from, to);
    }

    public static class GameObjectUtil
    {
        public static T CreateHolder<T>(string name) where T : Component
        {
            GameObject go = new(string.IsNullOrWhiteSpace(name) ? typeof(T).Name : name);
            UObject.DontDestroyOnLoad(go);
            return go.AddComponent<T>();
        }
    }

    public static class HealthCompatExtensions
    {
        public static void manageHealth(this GameObject gameObject, int hp)
        {
            if (gameObject == null)
            {
                return;
            }

            HealthManager? manager = gameObject.GetComponent<HealthManager>();
            if (manager == null)
            {
                return;
            }

            int value = Mathf.Max(1, hp);
            manager.hp = value;
            try
            {
                ReflectionHelper.SetField(manager, "maxHP", value);
            }
            catch
            {
                // Ignore if maxHP is not available in the current game version.
            }
        }
    }
}

namespace Osmi.Utils
{
    public static class EnumerableCompatExtensions
    {
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> source, Func<T, bool> predicate) =>
            source.Where(predicate);

        public static IEnumerable<T> Reject<T>(this IEnumerable<T> source, Func<T, bool> predicate) =>
            source.Where(item => !predicate(item));

        public static IEnumerable<TResult> Map<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector) =>
            source.Select(selector);

        public static IEnumerable<TResult> FlatMap<T, TResult>(this IEnumerable<T> source, Func<T, IEnumerable<TResult>> selector) =>
            source.SelectMany(selector);

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source) =>
            source.SelectMany(items => items);

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
            {
                action(item);
            }
        }

        public static string Join<T>(this IEnumerable<T> source, string separator) =>
            string.Join(separator, source);
    }

    public static class ReflectionCompatExtensions
    {
        public static (Func<T> getter, Action<T> setter) GetFastStaticAccessors<T>(this FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            return
            (
                getter: () =>
                {
                    object? value = fieldInfo.GetValue(null);
                    if (value is T typed)
                    {
                        return typed;
                    }

                    if (value == null)
                    {
                        return default!;
                    }

                    return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                },
                setter: value => fieldInfo.SetValue(null, value)
            );
        }
    }
}

namespace Osmi.Utils.Tap
{
    public static class TapCompatExtensions
    {
        public static T Tap<T>(this T value, Action<T> action)
        {
            action?.Invoke(value);
            return value;
        }
    }
}

namespace Osmi.FsmActions
{
    public sealed class InvokeMethod : FsmStateAction
    {
        private readonly Action action;

        public InvokeMethod(Action action)
        {
            this.action = action ?? (() => { });
        }

        public override void OnEnter()
        {
            action.Invoke();
            Finish();
        }
    }
}
