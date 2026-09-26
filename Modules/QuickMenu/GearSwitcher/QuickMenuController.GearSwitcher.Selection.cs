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
		private bool IsAlwaysFuriousHealthLockActive()
		{
			return global::GodhomeQoL.Modules.BossChallenge.AlwaysFurious.IsGearSwitcherHealthLockActive();
		}

		private bool GetSelectedNailless()
		{
			return GetSelectedPreset().Nailless;
		}

		private void SetSelectedNailless(bool value)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			if (selectedPreset.Nailless != value)
			{
				selectedPreset.Nailless = value;
				GodhomeQoL.SaveGlobalSettingsSafe();
				MarkGearSwitcherPresetEdited();
			}
		}

		private bool GetSelectedOvercharmed()
		{
			return GetSelectedPreset().Overcharmed;
		}

		private void SetSelectedOvercharmed(bool value)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			if (selectedPreset.Overcharmed != value)
			{
				selectedPreset.Overcharmed = value;
				GodhomeQoL.SaveGlobalSettingsSafe();
				MarkGearSwitcherPresetEdited();
			}
		}

		private bool GetSelectedMoveAbility(string key)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			if (selectedPreset.HasMoveAbilities != null && selectedPreset.HasMoveAbilities.TryGetValue(key, out var value))
			{
				return value;
			}
			return false;
		}

		private void SetSelectedMoveAbility(string key, bool value)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			GearPreset gearPreset = selectedPreset;
			if (gearPreset.HasMoveAbilities == null)
			{
				Dictionary<string, bool> dictionary = (gearPreset.HasMoveAbilities = new Dictionary<string, bool>());
			}
			selectedPreset.HasMoveAbilities[key] = value;
			selectedPreset.HasAllMoveAbilities = false;
			GodhomeQoL.SaveGlobalSettingsSafe();
			MarkGearSwitcherPresetEdited();
		}

		private int GetSelectedCloakLevel()
		{
			bool selectedMoveAbility = GetSelectedMoveAbility("Dash");
			bool selectedMoveAbility2 = GetSelectedMoveAbility("ShadowDash");
			if (!selectedMoveAbility)
			{
				return 0;
			}
			return (!selectedMoveAbility2) ? 1 : 2;
		}

		private void ApplyCloakLevel(int level)
		{
			bool value = level > 0;
			bool value2 = level > 1;
			SetSelectedMoveAbility("Dash", value);
			SetSelectedMoveAbility("ShadowDash", value2);
		}

		private bool GetSelectedDreamgateEnabled()
		{
			return GetSelectedPreset().DreamNailLevel >= 2;
		}

		private void SetSelectedDreamgateEnabled(bool enabled)
		{
			int dreamNailLevel = GetSelectedPreset().DreamNailLevel;
			if (enabled)
			{
				if (dreamNailLevel <= 0)
				{
					SetSelectedPresetDreamNail(2);
				}
				else if (dreamNailLevel == 1)
				{
					SetSelectedPresetDreamNail(2);
				}
				else if (dreamNailLevel >= 3)
				{
					SetSelectedPresetDreamNail(3);
				}
				else
				{
					SetSelectedPresetDreamNail(2);
				}
			}
			else if (dreamNailLevel >= 2)
			{
				SetSelectedPresetDreamNail(1);
			}
		}

		private int GetSelectedSpellLevel(string key)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			if (selectedPreset.SpellsLevel != null && selectedPreset.SpellsLevel.TryGetValue(key, out var value))
			{
				return value;
			}
			return 0;
		}

		private void SetSelectedSpellLevel(string key, int value)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			GearPreset gearPreset = selectedPreset;
			if (gearPreset.SpellsLevel == null)
			{
				Dictionary<string, int> dictionary = (gearPreset.SpellsLevel = new Dictionary<string, int>());
			}
			selectedPreset.SpellsLevel[key] = Math.Max(0, Math.Min(2, value));
			GodhomeQoL.SaveGlobalSettingsSafe();
			MarkGearSwitcherPresetEdited();
		}

		private bool GetSelectedNailArt(string key)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			if (selectedPreset.HasNailArts != null && selectedPreset.HasNailArts.TryGetValue(key, out var value))
			{
				return value;
			}
			return false;
		}

		private void SetSelectedNailArt(string key, bool value)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			GearPreset gearPreset = selectedPreset;
			if (gearPreset.HasNailArts == null)
			{
				Dictionary<string, bool> dictionary = (gearPreset.HasNailArts = new Dictionary<string, bool>());
			}
			selectedPreset.HasNailArts[key] = value;
			GodhomeQoL.SaveGlobalSettingsSafe();
			MarkGearSwitcherPresetEdited();
		}

		private bool GetSelectedBinding(string key)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			if (selectedPreset.HasAllBindings)
			{
				return true;
			}
			if (selectedPreset.Bindings != null && selectedPreset.Bindings.TryGetValue(key, out var value))
			{
				return value;
			}
			return false;
		}

		private void SetSelectedBinding(string key, bool value)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			GearPreset gearPreset = selectedPreset;
			if (gearPreset.Bindings == null)
			{
				Dictionary<string, bool> dictionary = (gearPreset.Bindings = new Dictionary<string, bool>());
			}
			if (selectedPreset.HasAllBindings)
			{
				selectedPreset.Bindings["CharmsBinding"] = true;
				selectedPreset.Bindings["NailBinding"] = true;
				selectedPreset.Bindings["ShellBinding"] = true;
				selectedPreset.Bindings["SoulBinding"] = true;
			}
			selectedPreset.Bindings[key] = value;
			selectedPreset.HasAllBindings = false;
			GodhomeQoL.SaveGlobalSettingsSafe();
			MarkGearSwitcherPresetEdited();
		}
    }
}
