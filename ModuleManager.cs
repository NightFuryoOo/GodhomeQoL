using Modding.Utils;
using Osmi.Utils.Tap;
using System.Diagnostics.CodeAnalysis;

namespace GodhomeQoL;

public static class ModuleManager
{
    private static readonly Lazy<Dictionary<string, Module>> modules = new(BuildModuleMap);

    internal static Dictionary<string, Module> Modules => modules.Value;

    internal static readonly Dictionary<int, (string suppressor, Module[] modules)> suppressions = [];
    private static int lastSuppressionHandle = 0;

    private static Dictionary<string, Module> BuildModuleMap()
    {
        Dictionary<string, Module> map = new(StringComparer.Ordinal);
        foreach (Type type in Assembly
            .GetExecutingAssembly()
            .GetTypesSafely()
            .Where(type => type.IsSubclassOf(typeof(Module)) && !type.IsAbstract)
            .OrderBy(type => type.FullName, StringComparer.Ordinal))
        {
            if (type.GetConstructor(Type.EmptyTypes) == null)
            {
                LogError($"Default constructor not found on module type {type.FullName}");
                continue;
            }

#if DEBUG
            if (!type.IsSealed)
            {
                LogWarn($"Module type {type.FullName} is not sealed");
            }
#endif

            try
            {
                if (Activator.CreateInstance(type) is not Module module)
                {
                    LogError($"Failed to initialize module {type.FullName}: constructed value is not a Module");
                    continue;
                }

                if (map.ContainsKey(module.Name))
                {
                    LogError($"Duplicate module name detected: {module.Name} ({type.FullName})");
                    continue;
                }

                map.Add(module.Name, module);
            }
            catch (Exception ex)
            {
                LogError($"Failed to initialize module {type.FullName} - {ex}");
            }
        }

        return map;
    }

    private static void ClearAllSuppressions(string reason, bool updateStatus)
    {
        if (suppressions.Count == 0)
        {
            return;
        }

        foreach ((int handle, (string suppressor, Module[] modules)) in suppressions.ToArray())
        {
            foreach (Module module in modules)
            {
                if (module == null)
                {
                    continue;
                }

                _ = module.suppressorMap.Remove(handle);
                if (!updateStatus)
                {
                    continue;
                }

                try
                {
                    module.UpdateStatus();
                }
                catch (Exception ex)
                {
                    LogError($"Failed to update module status while clearing suppression {handle} - {ex}");
                }
            }
        }

        int removed = suppressions.Count;
        suppressions.Clear();
        LogWarn($"Cleared {removed} stale module suppressions during {reason}");
    }

    internal static void Load()
    {
        ClearAllSuppressions("load", updateStatus: false);

        foreach (Module module in Modules.Values)
        {
            try
            {
                module.Active = true;
            }
            catch (Exception ex)
            {
                LogError($"Failed to activate module {module.Name} - {ex}");
            }
        }
    }


    internal static void Unload()
    {
        foreach (Module module in Modules.Values)
        {
            try
            {
                module.Active = false;
            }
            catch (Exception ex)
            {
                LogError($"Failed to deactivate module {module.Name} - {ex}");
            }
        }

        ClearAllSuppressions("unload", updateStatus: false);
    }

    public static Module? GetModule<T>() where T : Module
    {
        try
        {
            return TryGetModule(typeof(T).Name, out Module? module) ? module : null;

        }
        catch (Exception ex)
        {
            Logger.Log(ex.Message);
            return null;
        }

    }

    public static bool TryGetModule(Type type, [NotNullWhen(true)] out Module? module)
    {
        try
        {
            return TryGetModule(type.Name, out module);

        }
        catch (Exception ex)
        {
            Logger.Log(ex.Message);
            module = null;
            return false;
        }

    }

    public static bool TryGetModule(string name, [NotNullWhen(true)] out Module? module)
    {
        try
        {
            module = Modules.TryGetValue(name, out Module? m) ? m : null;
            return module != null;
        }
        catch (Exception ex)
        {
            Logger.Log(ex.Message);
            module = null;
            return false;
        }

    }

    public static bool TryGetLoadedModule<T>([NotNullWhen(true)] out T? module) where T : Module
    {
        try
        {
            bool ret = TryGetLoadedModule(typeof(T).Name, out Module? m);
            module = m as T;
            return ret;
        }
        catch (Exception ex)
        {
            Logger.Log(ex.Message);
            module = null;
            return false;
        }
        
    }

    public static bool TryGetLoadedModule(Type type, [NotNullWhen(true)] out Module? module) =>
        TryGetLoadedModule(type.Name, out module);

    public static bool TryGetLoadedModule(string name, [NotNullWhen(true)] out Module? module)
    {
        try
        {
            module = Modules.TryGetValue(name, out Module? m) && m.Loaded ? m : null;
            return module != null;
        }
        catch (Exception ex)
        {
            Logger.Log(ex.Message);
            module = null;
            return false;
        }
        
    }

    public static bool IsModuleLoaded<T>() where T : Module
    {
        try
        {
            return TryGetLoadedModule<T>(out _);
        }
        catch (Exception ex)
        {
            Logger.Log(ex.Message);
            return false;
        }

    }


    public static bool IsModuleLoaded(Type type)
    {
        try
        {
            return TryGetLoadedModule(type, out _);
        }
        catch (Exception ex)
        {
            Logger.Log(ex.Message);
            return false;
        }

    }


    public static bool IsModuleLoaded(string name)
    {
        try
        {
            return TryGetLoadedModule(name, out _);
        }
        catch (Exception ex)
        {
            Logger.Log(ex.Message);
            return false;
        }

    }
   


    public static int SuppressModules(string suppressor, params Module[] modules)
    {
        try
        {
            Module[] validModules = modules.Where(module => module != null).ToArray();
            if (validModules.Length == 0)
            {
                return 0;
            }

            int handle = ++lastSuppressionHandle;

            suppressions.Add(handle, (suppressor, validModules));

            foreach (Module module in validModules)
            {
                module.suppressorMap.Add(handle, suppressor);
                module.UpdateStatus();
            }

            Log(suppressor + " starts to suppress modules " + validModules.Map(m => m.Name).Join(", ") + " with handle " + handle);

            return handle;
        }
        catch (Exception ex)
        {
            Logger.Log(ex.Message);
            return 0;
        }
        
    }

    public static int SuppressModule<T>(string suppressor) where T : Module
    {
        try
        {
            if (!TryGetModule(typeof(T), out Module? module))
            {
                LogError($"Cannot suppress unknown module {typeof(T).Name}");
                return 0;
            }

            return SuppressModules(suppressor, module);
        }
        catch (Exception ex)
        {
            Logger.Log(ex.Message);
            return 0;
        }
    }

    public static int SuppressModules(string suppressor, params string[] modules)
    {
        try
        {

            return SuppressModules(suppressor, modules.Map(name => TryGetModule(name, out Module? m)
            ? m
            : throw new InvalidOperationException("Unknown module " + name)
        ).ToArray());
        }
        catch (Exception ex)
        {
            Logger.Log(ex.Message);
            return 0;
        }
    }
   

    public static void CancelSuppression(int handle)
    {
        try
        {

            if (!suppressions.TryGetValue(handle, out (string suppressor, Module[] modules) suppression))
            {
                LogError("Failed attempt to end unknown suppresion with handle " + handle);
                return;
            }

            _ = suppressions.Remove(handle);
            (string suppressor, Module[] modules) = suppression;

            foreach (Module module in modules)
            {
                _ = module.suppressorMap.Remove(handle);
                module.UpdateStatus();
            }

            Log(suppressor + " end to suppress modules " + modules.Map(m => m.Name).Join(", ") + " with handle " + handle);
        }
        catch (Exception ex)
        {
            Logger.Log(ex.Message);
        }
       
    }
}
