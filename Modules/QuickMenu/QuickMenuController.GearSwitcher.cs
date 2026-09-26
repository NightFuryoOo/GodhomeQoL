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
		private void OnGearSwitcherBackClicked()
		{
			bool flag = returnToQuickOnClose;
			returnToQuickOnClose = false;
			if (flag)
			{
				SetQuickVisible(value: true);
			}
			SetGearSwitcherVisible(value: false);
		}

		private void OnGearSwitcherCharmPromptClicked()
		{
			SetGearSwitcherVisible(value: false);
			SetGearSwitcherCharmCostVisible(value: true);
		}

		private bool GetGearSwitcherEnabled()
		{
			return GearSwitcher.IsGloballyEnabled;
		}

		private void SetGearSwitcherEnabled(bool value)
		{
			if (GearSwitcher.IsGloballyEnabled != value)
			{
				if (value)
				{
					GearSwitcher.PrepareForEnable();
					GearSwitcher.IsGloballyEnabled = true;
					GearSwitcher.ApplyPreset(gearSwitcherSelectedPreset, allowQueue: true);
				}
				else
				{
					GearSwitcher.DisableAndRestoreState();
				}
				RefreshGearSwitcherUi();
				UpdateGearSwitcherInteractivity();
				UpdateQuickMenuEntryStateColors();
			}
		}

		private void RefreshGearSwitcherUi()
		{
			UpdateToggleValue(gearSwitcherEnableValue, GetGearSwitcherEnabled());
			UpdateToggleIcon(gearSwitcherEnableIcon, GetGearSwitcherEnabled());
			string[] gearSwitcherPresetOptions = GetGearSwitcherPresetOptions();
			int gearSwitcherPresetIndex = GetGearSwitcherPresetIndex(gearSwitcherPresetOptions);
			if (gearSwitcherSelectedPresetValue != null)
			{
				gearSwitcherSelectedPresetValue.text = ((gearSwitcherPresetOptions.Length != 0) ? GetSelectedPresetDisplayLabel(gearSwitcherPresetOptions[gearSwitcherPresetIndex]) : string.Empty);
				UpdateGearSwitcherSelectedPresetColor((gearSwitcherPresetOptions.Length != 0) ? gearSwitcherPresetOptions[gearSwitcherPresetIndex] : string.Empty);
			}
			RefreshGearSwitcherPresetFields();
			UpdateGearSwitcherResetGearVisibility();
			UpdateGearSwitcherInteractivity();
		}

		private readonly struct GearSwitcherBaseNailDamageDisplayLayout
		{
			public GearSwitcherBaseNailDamageDisplayLayout(
				string displayText,
				int leftStrengthSlotIndex,
				int rightStrengthSlotIndex,
				int rightFurySlotIndex,
				int nailBindingSlotIndex)
			{
				DisplayText = displayText;
				LeftStrengthSlotIndex = leftStrengthSlotIndex;
				RightStrengthSlotIndex = rightStrengthSlotIndex;
				RightFurySlotIndex = rightFurySlotIndex;
				NailBindingSlotIndex = nailBindingSlotIndex;
			}

			public string DisplayText { get; }
			public int LeftStrengthSlotIndex { get; }
			public int RightStrengthSlotIndex { get; }
			public int RightFurySlotIndex { get; }
			public int NailBindingSlotIndex { get; }
		}
    }
}
