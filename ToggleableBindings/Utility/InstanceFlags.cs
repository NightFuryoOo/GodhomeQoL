using UnityEngine;

namespace ToggleableBindings.Utility
{
    [System.Flags]
    internal enum InstanceFlags
    {
        Default = 0,

        DontDestroyOnLoad = 1 << 0,

        StartInactive = 1 << 2,

        WorldPositionStays = 1 << 3
    }
}