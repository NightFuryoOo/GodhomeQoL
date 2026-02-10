using System.Collections.Generic;

namespace GodhomeQoL.Settings;

public static class GearPresetDefaults
{
    public static Dictionary<string, GearPreset> CreateDefaults()
    {
        GearPreset fullGear = CreateFullGear();
        GearPreset ab = CreateAB();
        GearPreset o4 = CreateO4();
        GearPreset ow = CreateOw();
        GearPreset itemless = CreateItemless();

        return new Dictionary<string, GearPreset>
        {
            { fullGear.Name, fullGear },
            { ab.Name, ab },
            { o4.Name, o4 },
            { ow.Name, ow },
            { itemless.Name, itemless }
        };
    }

    public static List<string> DefaultOrder() => new()
    {
        "FullGear",
        "AB",
        "04",
        "0w",
        "Itemless"
    };

    private static GearPreset CreateFullGear()
    {
        GearPreset value = new()
        {
            Name = "FullGear",
            MaxHealth = 9,
            SoulVessels = 3,
            NailDamage = 21,
            CharmSlots = 11,
            MainSoulGain = 11,
            ReserveSoulGain = 6,
            DreamNailLevel = 3
        };

        value.UseGrimmchild = true;
        value.GrimmchildCost = 2;
        value.CarefreeMelodyCost = 3;
        value.CarefreeMelodyCostInitialized = true;

        SetAllMoveAbilities(value, true);
        value.SpellsLevel["fireballLevel"] = 2;
        value.SpellsLevel["quakeLevel"] = 2;
        value.SpellsLevel["screamLevel"] = 2;
        value.HasNailArts["hasCyclone"] = true;
        value.HasNailArts["hasDashSlash"] = true;
        value.HasNailArts["hasUpwardSlash"] = true;

        return value;
    }

    private static GearPreset CreateAB()
    {
        GearPreset value = CreateFullGear();
        value.Name = "AB";
        value.NailDamage = 21;
        value.HasAllBindings = true;
        return value;
    }

    private static GearPreset CreateO4()
    {
        GearPreset value = CreateFullGear();
        value.Name = "04";
        value.NailDamage = 5;
        value.SpellsLevel["fireballLevel"] = 1;
        value.SpellsLevel["quakeLevel"] = 1;
        value.SpellsLevel["screamLevel"] = 1;
        value.HasAllBindings = true;
        return value;
    }

    private static GearPreset CreateOw()
    {
        GearPreset value = CreateFullGear();
        value.Name = "0w";
        value.NailDamage = 5;
        value.SpellsLevel["fireballLevel"] = 1;
        value.SpellsLevel["quakeLevel"] = 1;
        value.SpellsLevel["screamLevel"] = 1;
        return value;
    }

    private static GearPreset CreateItemless()
    {
        GearPreset value = new()
        {
            Name = "Itemless"
        };

        SetAllMoveAbilities(value, false);
        return value;
    }

    private static void SetAllMoveAbilities(GearPreset preset, bool value)
    {
        preset.HasMoveAbilities["AcidArmour"] = value;
        preset.HasMoveAbilities["Dash"] = value;
        preset.HasMoveAbilities["Walljump"] = value;
        preset.HasMoveAbilities["SuperDash"] = value;
        preset.HasMoveAbilities["ShadowDash"] = value;
        preset.HasMoveAbilities["DoubleJump"] = value;
    }
}
