using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ToggleableBindings;
using ToggleableBindings.VanillaBindings;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
		private sealed class CharmCostDefinition
		{
			public CharmCostDefinition(string key, string spriteFile, Func<GearPreset, int> get, Action<GearPreset, int> set, int aheCost)
			{
				Key = key;
				SpriteFile = spriteFile;
				Get = get;
				Set = set;
				AheCost = aheCost;
			}

			public string Key { get; }

			public string SpriteFile { get; }

			public Func<GearPreset, int> Get { get; }

			public Action<GearPreset, int> Set { get; }

			public int AheCost { get; }
		}

		private static readonly CharmCostDefinition[] CharmCostDefinitions = new CharmCostDefinition[]
		{
			new CharmCostDefinition("WaywardCompass", "Wayward Compass.png", (GearPreset preset) => preset.WaywardCompassCost, (GearPreset preset, int value) => preset.WaywardCompassCost = value, 0),
			new CharmCostDefinition("GatheringSwarm", "Gathering Swarm.png", (GearPreset preset) => preset.GatheringSwarmCost, (GearPreset preset, int value) => preset.GatheringSwarmCost = value, 0),
			new CharmCostDefinition("StalwartShell", "Stalwart Shell.png", (GearPreset preset) => preset.StalwartShellCost, (GearPreset preset, int value) => preset.StalwartShellCost = value, 1),
			new CharmCostDefinition("SoulCatcher", "Soul Catcher.png", (GearPreset preset) => preset.SoulCatcherCost, (GearPreset preset, int value) => preset.SoulCatcherCost = value, 1),
			new CharmCostDefinition("ShamanStone", "Shaman Stone.png", (GearPreset preset) => preset.ShamanStoneCost, (GearPreset preset, int value) => preset.ShamanStoneCost = value, 3),
			new CharmCostDefinition("SoulEater", "Soul Eater.png", (GearPreset preset) => preset.SoulEaterCost, (GearPreset preset, int value) => preset.SoulEaterCost = value, 3),
			new CharmCostDefinition("Dashmaster", "Dashmaster.png", (GearPreset preset) => preset.DashmasterCost, (GearPreset preset, int value) => preset.DashmasterCost = value, 1),
			new CharmCostDefinition("Sprintmaster", "Sprintmaster.png", (GearPreset preset) => preset.SprintmasterCost, (GearPreset preset, int value) => preset.SprintmasterCost = value, 0),
			new CharmCostDefinition("Grubsong", "Grubsong.png", (GearPreset preset) => preset.GrubsongCost, (GearPreset preset, int value) => preset.GrubsongCost = value, 1),
			new CharmCostDefinition("GrubberflysElegy", "Grubberflys Elegy.png", (GearPreset preset) => preset.GrubberflysElegyCost, (GearPreset preset, int value) => preset.GrubberflysElegyCost = value, 2),
			new CharmCostDefinition("UnbreakableHeart", "Unbreakable Heart.png", (GearPreset preset) => preset.UnbreakableHeartCost, (GearPreset preset, int value) => preset.UnbreakableHeartCost = value, 2),
			new CharmCostDefinition("UnbreakableGreed", "Unbreakable Greed.png", (GearPreset preset) => preset.UnbreakableGreedCost, (GearPreset preset, int value) => preset.UnbreakableGreedCost = value, 0),
			new CharmCostDefinition("UnbreakableStrength", "Unbreakable Strength.png", (GearPreset preset) => preset.UnbreakableStrengthCost, (GearPreset preset, int value) => preset.UnbreakableStrengthCost = value, 3),
			new CharmCostDefinition("SpellTwister", "Spell Twister.png", (GearPreset preset) => preset.SpellTwisterCost, (GearPreset preset, int value) => preset.SpellTwisterCost = value, 2),
			new CharmCostDefinition("SteadyBody", "Steady Body.png", (GearPreset preset) => preset.SteadyBodyCost, (GearPreset preset, int value) => preset.SteadyBodyCost = value, 1),
			new CharmCostDefinition("HeavyBlow", "Heavy Blow.png", (GearPreset preset) => preset.HeavyBlowCost, (GearPreset preset, int value) => preset.HeavyBlowCost = value, 0),
			new CharmCostDefinition("QuickSlash", "Quick Slash.png", (GearPreset preset) => preset.QuickSlashCost, (GearPreset preset, int value) => preset.QuickSlashCost = value, 2),
			new CharmCostDefinition("Longnail", "Longnail.png", (GearPreset preset) => preset.LongnailCost, (GearPreset preset, int value) => preset.LongnailCost = value, 1),
			new CharmCostDefinition("MarkOfPride", "Mark of Pride.png", (GearPreset preset) => preset.MarkOfPrideCost, (GearPreset preset, int value) => preset.MarkOfPrideCost = value, 2),
			new CharmCostDefinition("FuryOfTheFallen", "Fury of the Fallen.png", (GearPreset preset) => preset.FuryOfTheFallenCost, (GearPreset preset, int value) => preset.FuryOfTheFallenCost = value, 1),
			new CharmCostDefinition("ThornsOfAgony", "Thorns of Agony.png", (GearPreset preset) => preset.ThornsOfAgonyCost, (GearPreset preset, int value) => preset.ThornsOfAgonyCost = value, 1),
			new CharmCostDefinition("BaldurShell", "Baldur Shell.png", (GearPreset preset) => preset.BaldurShellCost, (GearPreset preset, int value) => preset.BaldurShellCost = value, 1),
			new CharmCostDefinition("Flukenest", "Flukenest.png", (GearPreset preset) => preset.FlukenestCost, (GearPreset preset, int value) => preset.FlukenestCost = value, 2),
			new CharmCostDefinition("DefendersCrest", "Defenders Crest.png", (GearPreset preset) => preset.DefendersCrestCost, (GearPreset preset, int value) => preset.DefendersCrestCost = value, 1),
			new CharmCostDefinition("GlowingWomb", "Glowing Womb.png", (GearPreset preset) => preset.GlowingWombCost, (GearPreset preset, int value) => preset.GlowingWombCost = value, 0),
			new CharmCostDefinition("QuickFocus", "Quick Focus.png", (GearPreset preset) => preset.QuickFocusCost, (GearPreset preset, int value) => preset.QuickFocusCost = value, 2),
			new CharmCostDefinition("DeepFocus", "Deep Focus.png", (GearPreset preset) => preset.DeepFocusCost, (GearPreset preset, int value) => preset.DeepFocusCost = value, 3),
			new CharmCostDefinition("LifebloodHeart", "Lifeblood Heart.png", (GearPreset preset) => preset.LifebloodHeartCost, (GearPreset preset, int value) => preset.LifebloodHeartCost = value, 1),
			new CharmCostDefinition("LifebloodCore", "Lifeblood Core.png", (GearPreset preset) => preset.LifebloodCoreCost, (GearPreset preset, int value) => preset.LifebloodCoreCost = value, 2),
			new CharmCostDefinition("JonisBlessing", "Jonis Blessing.png", (GearPreset preset) => preset.JonisBlessingCost, (GearPreset preset, int value) => preset.JonisBlessingCost = value, 0),
			new CharmCostDefinition("Hiveblood", "Hiveblood.png", (GearPreset preset) => preset.HivebloodCost, (GearPreset preset, int value) => preset.HivebloodCost = value, 2),
			new CharmCostDefinition("SporeShroom", "Spore Shroom.png", (GearPreset preset) => preset.SporeShroomCost, (GearPreset preset, int value) => preset.SporeShroomCost = value, 1),
			new CharmCostDefinition("SharpShadow", "Sharp Shadow.png", (GearPreset preset) => preset.SharpShadowCost, (GearPreset preset, int value) => preset.SharpShadowCost = value, 1),
			new CharmCostDefinition("ShapeOfUnn", "Shape of Unn.png", (GearPreset preset) => preset.ShapeOfUnnCost, (GearPreset preset, int value) => preset.ShapeOfUnnCost = value, 2),
			new CharmCostDefinition("NailmastersGlory", "Nailmasters Glory.png", (GearPreset preset) => preset.NailmastersGloryCost, (GearPreset preset, int value) => preset.NailmastersGloryCost = value, 1),
			new CharmCostDefinition("Weaversong", "Weaversong.png", (GearPreset preset) => preset.WeaversongCost, (GearPreset preset, int value) => preset.WeaversongCost = value, 1),
			new CharmCostDefinition("DreamWielder", "Dream Wielder.png", (GearPreset preset) => preset.DreamWielderCost, (GearPreset preset, int value) => preset.DreamWielderCost = value, 1),
			new CharmCostDefinition("Dreamshield", "Dreamshield.png", (GearPreset preset) => preset.DreamshieldCost, (GearPreset preset, int value) => preset.DreamshieldCost = value, 1),
		};

		private static bool AreCharmCostsEqual(GearPreset current, GearPreset baseline)
		{
			foreach (CharmCostDefinition definition in CharmCostDefinitions)
			{
				if (definition.Get(current) != definition.Get(baseline))
				{
					return false;
				}
			}
			return true;
		}
    }
}
