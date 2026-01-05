using InControl;
using Modding.Converters;
using Newtonsoft.Json;

namespace GodhomeQoL.Settings;

public sealed class FastDreamWarpSettings
{
    [JsonConverter(typeof(PlayerActionSetConverter))]
    public FastDreamWarpKeyBinds Keybinds = new();
}

public sealed class FastDreamWarpKeyBinds : PlayerActionSet
{
    public PlayerAction Toggle;

    public FastDreamWarpKeyBinds()
    {
        Toggle = CreatePlayerAction("Toggle");
    }
}
