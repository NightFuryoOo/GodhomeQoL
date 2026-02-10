using System.Collections.Generic;

namespace GodhomeQoL.Settings;

public sealed class GearPreset
{
    public string Name { get; set; } = "Preset";

    public int MaxHealth { get; set; } = 5;
    public int SoulVessels { get; set; } = 0;
    public int NailDamage { get; set; } = 5;
    public int CharmSlots { get; set; } = 3;
    public int MainSoulGain { get; set; } = 11;
    public int ReserveSoulGain { get; set; } = 6;
    public int GatheringSwarmCost { get; set; } = 1;
    public int WaywardCompassCost { get; set; } = 1;
    public int StalwartShellCost { get; set; } = 2;
    public int SoulCatcherCost { get; set; } = 2;
    public int ShamanStoneCost { get; set; } = 3;
    public int SoulEaterCost { get; set; } = 4;
    public int DashmasterCost { get; set; } = 2;
    public int SprintmasterCost { get; set; } = 1;
    public int GrubsongCost { get; set; } = 1;
    public int GrubberflysElegyCost { get; set; } = 3;
    public int UnbreakableHeartCost { get; set; } = 2;
    public int UnbreakableGreedCost { get; set; } = 2;
    public int UnbreakableStrengthCost { get; set; } = 3;
    public int SpellTwisterCost { get; set; } = 2;
    public int SteadyBodyCost { get; set; } = 1;
    public int HeavyBlowCost { get; set; } = 2;
    public int QuickSlashCost { get; set; } = 3;
    public int LongnailCost { get; set; } = 2;
    public int MarkOfPrideCost { get; set; } = 3;
    public int FuryOfTheFallenCost { get; set; } = 2;
    public int ThornsOfAgonyCost { get; set; } = 1;
    public int BaldurShellCost { get; set; } = 2;
    public int FlukenestCost { get; set; } = 3;
    public int DefendersCrestCost { get; set; } = 1;
    public int GlowingWombCost { get; set; } = 2;
    public int QuickFocusCost { get; set; } = 3;
    public int DeepFocusCost { get; set; } = 4;
    public int LifebloodHeartCost { get; set; } = 2;
    public int LifebloodCoreCost { get; set; } = 3;
    public int JonisBlessingCost { get; set; } = 4;
    public int HivebloodCost { get; set; } = 4;
    public int SporeShroomCost { get; set; } = 1;
    public int SharpShadowCost { get; set; } = 2;
    public int ShapeOfUnnCost { get; set; } = 2;
    public int NailmastersGloryCost { get; set; } = 1;
    public int WeaversongCost { get; set; } = 2;
    public int DreamWielderCost { get; set; } = 1;
    public int DreamshieldCost { get; set; } = 3;
    public int CarefreeMelodyCost { get; set; } = 3;
    public int GrimmchildCost { get; set; } = 2;
    public int VoidHeartCost { get; set; } = 0;
    public int KingsoulCost { get; set; } = 5;
    public bool UseVoidHeart { get; set; } = true;
    public bool KingsoulCostInitialized { get; set; } = false;
    public bool UseGrimmchild { get; set; } = true;
    public bool CarefreeMelodyCostInitialized { get; set; } = false;

    public List<int>? EquippedCharms { get; set; }

    public bool HasAllMoveAbilities { get; set; } = false;
    public Dictionary<string, bool> HasMoveAbilities { get; set; } = new()
    {
        { "AcidArmour", false },
        { "Dash", false },
        { "Walljump", false },
        { "SuperDash", false },
        { "ShadowDash", false },
        { "DoubleJump", false }
    };

    public Dictionary<string, int> SpellsLevel { get; set; } = new()
    {
        { "fireballLevel", 0 },
        { "quakeLevel", 0 },
        { "screamLevel", 0 }
    };

    public Dictionary<string, bool> HasNailArts { get; set; } = new()
    {
        { "hasCyclone", false },
        { "hasDashSlash", false },
        { "hasUpwardSlash", false }
    };

    public int DreamNailLevel { get; set; } = 0;
    public bool Nailless { get; set; } = false;
    public bool Overcharmed { get; set; } = false;

    public bool HasAllBindings { get; set; } = false;
    public Dictionary<string, bool> Bindings { get; set; } = new()
    {
        { "CharmsBinding", false },
        { "NailBinding", false },
        { "ShellBinding", false },
        { "SoulBinding", false }
    };
}
