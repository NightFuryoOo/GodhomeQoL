using InControl;

namespace GodhomeQoL.Settings;

public sealed class GearSwitcherSettings
{
    public Dictionary<string, GearPreset> Presets { get; set; } = GearPresetDefaults.CreateDefaults();

    public List<string> PresetOrder { get; set; } = GearPresetDefaults.DefaultOrder();

    public string FullGearDisplayName { get; set; } = "FullGear";

    public string LastPreset { get; set; } = "FullGear";

    public bool Enabled { get; set; } = false;

    public string NailAttackKeyBinding { get; set; } = string.Empty;
    public string NailAttackMouseBinding { get; set; } = string.Empty;
    public InputControlType NailAttackControllerBinding { get; set; }
    public bool NailAttackBindingsStored { get; set; }
}
