using InControl;
using Modding.Converters;
using Newtonsoft.Json;

namespace GodhomeQoL.Settings;

public sealed class ShowHPOnDeathSettings
{
    public bool EnabledMod = true;
    public bool ShowPB = true;
    public bool HideAfter10Sec = true;
    public float HudFadeSeconds = 5f;

    [JsonConverter(typeof(PlayerActionSetConverter))]
    public ShowHPOnDeathKeyBinds Keybinds = new();
}

public sealed class ShowHPOnDeathKeyBinds : PlayerActionSet
{
    public PlayerAction Hide;

    public ShowHPOnDeathKeyBinds()
    {
        Hide = CreatePlayerAction("Hide");
        Hide.AddDefaultBinding(Key.H);
    }
}
