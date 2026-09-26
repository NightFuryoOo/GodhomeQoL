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
		private string GetSelectedPresetLabel()
		{
			string[] gearSwitcherPresetOptions = GetGearSwitcherPresetOptions();
			if (gearSwitcherPresetOptions.Length == 0)
			{
				return string.Empty;
			}
			int gearSwitcherPresetIndex = GetGearSwitcherPresetIndex(gearSwitcherPresetOptions);
			return GetGearSwitcherPresetDisplayName(gearSwitcherPresetOptions[gearSwitcherPresetIndex]);
		}

		private int GetGearSwitcherPresetIndex(string[] options)
		{
			if (options.Length == 0)
			{
				return 0;
			}
			string persisted = GodhomeQoL.GlobalSettings.GearSwitcher?.LastPreset ?? string.Empty;
			if (!string.IsNullOrWhiteSpace(persisted))
			{
				string? persistedMatch = options.FirstOrDefault((string option) => string.Equals(option, persisted, StringComparison.OrdinalIgnoreCase));
				if (!string.IsNullOrWhiteSpace(persistedMatch)
					&& !string.Equals(gearSwitcherSelectedPreset, persistedMatch, StringComparison.OrdinalIgnoreCase))
				{
					gearSwitcherSelectedPreset = persistedMatch;
				}
			}
			if (string.IsNullOrWhiteSpace(gearSwitcherSelectedPreset) || !options.Contains<string>(gearSwitcherSelectedPreset))
			{
				string value = persisted;
				if (!string.IsNullOrWhiteSpace(value) && options.Contains<string>(value))
				{
					gearSwitcherSelectedPreset = value;
				}
				else
				{
					gearSwitcherSelectedPreset = options[0];
				}
			}
			return Array.IndexOf<string>(options, gearSwitcherSelectedPreset);
		}

		private GearPreset GetSelectedPreset()
		{
			if (GearSwitcherSettings.Presets == null || GearSwitcherSettings.Presets.Count == 0)
			{
				GearSwitcherSettings.Presets = GearPresetDefaults.CreateDefaults();
			}
			string text = gearSwitcherSelectedPreset;
			if (string.IsNullOrWhiteSpace(text))
			{
				string text2 = GodhomeQoL.GlobalSettings.GearSwitcher?.LastPreset ?? string.Empty;
				if (!string.IsNullOrWhiteSpace(text2))
				{
					text = text2;
				}
			}
			if (string.IsNullOrWhiteSpace(text) || !GearSwitcherSettings.Presets.TryGetValue(text, out GearPreset value))
			{
				string key = GearSwitcher.GetPresetOrder().FirstOrDefault() ?? "FullGear";
				if (!GearSwitcherSettings.Presets.TryGetValue(key, out value))
				{
					GearSwitcherSettings.Presets = GearPresetDefaults.CreateDefaults();
					value = GearSwitcherSettings.Presets.Values.First();
				}
				gearSwitcherSelectedPreset = value.Name;
				return value;
			}
			gearSwitcherSelectedPreset = text;
			return value;
		}

		private int GetEffectiveSelectedPresetMaxHealth()
		{
			if (IsAlwaysFuriousHealthLockActive())
			{
				return 1;
			}

			return Math.Max(1, Math.Min(9, GetSelectedPreset().MaxHealth));
		}

		private bool SetSelectedPresetMaxHealth(int value)
		{
			if (IsAlwaysFuriousHealthLockActive())
			{
				if (value != 1)
				{
					ShowStatusMessage("OFF Always Furious");
				}

				return false;
			}

			GearPreset selectedPreset = GetSelectedPreset();
			int clamped = Math.Max(1, Math.Min(9, value));
			if (selectedPreset.MaxHealth == clamped)
			{
				return false;
			}

			selectedPreset.MaxHealth = clamped;
			GodhomeQoL.SaveGlobalSettingsSafe();
			MarkGearSwitcherPresetEdited();
			return true;
		}

		private void SetSelectedPresetSoulVessels(int value)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			selectedPreset.SoulVessels = Math.Max(0, Math.Min(3, value));
			GodhomeQoL.SaveGlobalSettingsSafe();
			MarkGearSwitcherPresetEdited();
		}

		private void SetSelectedPresetCharmSlots(int value)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			int num = Math.Max(3, Math.Min(999, value));
			if (selectedPreset.CharmSlots != num)
			{
				selectedPreset.CharmSlots = num;
				GodhomeQoL.SaveGlobalSettingsSafe();
				GearSwitcher.ApplyStatsImmediate(selectedPreset, GearSwitcher.StatsPart.CharmSlots);
				MarkGearSwitcherPresetEdited();
			}
		}

		private void SetSelectedPresetMainSoulGain(int value)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			int num = Math.Max(0, Math.Min(198, value));
			if (selectedPreset.MainSoulGain != num)
			{
				selectedPreset.MainSoulGain = num;
				GodhomeQoL.SaveGlobalSettingsSafe();
				MarkGearSwitcherPresetEdited();
			}
		}

		private void SetSelectedPresetReserveSoulGain(int value)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			int num = Math.Max(0, Math.Min(198, value));
			if (selectedPreset.ReserveSoulGain != num)
			{
				selectedPreset.ReserveSoulGain = num;
				GodhomeQoL.SaveGlobalSettingsSafe();
				MarkGearSwitcherPresetEdited();
			}
		}

		private void SetSelectedPresetDreamNail(int value)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			selectedPreset.DreamNailLevel = Math.Max(0, Math.Min(3, value));
			GodhomeQoL.SaveGlobalSettingsSafe();
			UpdateGearSwitcherCloakRowIcons();
			MarkGearSwitcherPresetEdited();
		}
    }
}
