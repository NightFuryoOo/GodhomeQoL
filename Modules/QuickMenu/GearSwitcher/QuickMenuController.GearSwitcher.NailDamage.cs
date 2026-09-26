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
		private void UpdateGearSwitcherBaseNailDamage()
		{
			GearSwitcherBaseNailDamageDisplayLayout layout = BuildGearSwitcherBaseNailDamageDisplayLayout();
			if (gearSwitcherBaseNailDamageValue != null)
			{
				gearSwitcherBaseNailDamageValue.text = layout.DisplayText;
			}

			UpdateGearSwitcherBaseNailDamageEffectIcons(layout);

		}

		private static string GetGearSwitcherBaseNailDamageDisplay()
		{
			return BuildGearSwitcherBaseNailDamageDisplayLayout().DisplayText;
		}

		private const string GearSwitcherBaseNailDamageSlot = "    ";
		private const string GearSwitcherBaseNailDamageLeftStrengthToken = "{LS}";
		private const string GearSwitcherBaseNailDamageRightStrengthToken = "{RS}";
		private const string GearSwitcherBaseNailDamageRightFuryToken = "{RF}";
		private const string GearSwitcherBaseNailDamageNailBindingToken = "{NB}";

		private static GearSwitcherBaseNailDamageDisplayLayout BuildGearSwitcherBaseNailDamageDisplayLayout()
		{
			PlayerData? data = PlayerData.instance;
			if (data == null)
			{
				return new GearSwitcherBaseNailDamageDisplayLayout("0", -1, -1, -1, -1);
			}

			int flatDamage = Mathf.Max(0, data.nailDamage);
			bool furyEquipped = IsCharmEquipped(data, 6);
			bool strengthEquipped = IsCharmEquipped(data, 25);
			bool nailBindingApplied = IsNailBindingApplied();
			int effectiveBaseDamage = ApplyNailBindingBaseDamage(flatDamage, nailBindingApplied);

			int baseDamage = ComputeDisplayedNailDamage(effectiveBaseDamage, strengthEquipped, furyMultiplier: false);
			int furyDamage = ComputeDisplayedNailDamage(effectiveBaseDamage, strengthEquipped, furyMultiplier: true);

			StringBuilder rawBuilder = new StringBuilder();
			rawBuilder.Append(baseDamage);
			if (strengthEquipped)
			{
				rawBuilder.Append(GearSwitcherBaseNailDamageLeftStrengthToken);
			}

			if (furyEquipped)
			{
				rawBuilder.Append('/');
				rawBuilder.Append(furyDamage);
				if (strengthEquipped)
				{
					rawBuilder.Append(GearSwitcherBaseNailDamageRightStrengthToken);
				}
				rawBuilder.Append(GearSwitcherBaseNailDamageRightFuryToken);
			}

			rawBuilder.Append(" (Flat. ");
			rawBuilder.Append(effectiveBaseDamage);
			rawBuilder.Append(')');
			if (nailBindingApplied)
			{
				rawBuilder.Append(GearSwitcherBaseNailDamageNailBindingToken);
			}

			string rawText = rawBuilder.ToString();
			int leftStrengthSlotIndex = rawText.IndexOf(GearSwitcherBaseNailDamageLeftStrengthToken, StringComparison.Ordinal);
			int rightStrengthSlotIndex = rawText.IndexOf(GearSwitcherBaseNailDamageRightStrengthToken, StringComparison.Ordinal);
			int rightFurySlotIndex = rawText.IndexOf(GearSwitcherBaseNailDamageRightFuryToken, StringComparison.Ordinal);
			int nailBindingSlotIndex = rawText.IndexOf(GearSwitcherBaseNailDamageNailBindingToken, StringComparison.Ordinal);

			string displayText = rawText
				.Replace(GearSwitcherBaseNailDamageLeftStrengthToken, GearSwitcherBaseNailDamageSlot)
				.Replace(GearSwitcherBaseNailDamageRightStrengthToken, GearSwitcherBaseNailDamageSlot)
				.Replace(GearSwitcherBaseNailDamageRightFuryToken, GearSwitcherBaseNailDamageSlot)
				.Replace(GearSwitcherBaseNailDamageNailBindingToken, GearSwitcherBaseNailDamageSlot);

			return new GearSwitcherBaseNailDamageDisplayLayout(
				displayText,
				leftStrengthSlotIndex,
				rightStrengthSlotIndex,
				rightFurySlotIndex,
				nailBindingSlotIndex);
		}

		private static bool IsNailBindingApplied()
		{
			try
			{
				return BindingManager.TryGetBinding<NailBinding>(out NailBinding? binding) && binding != null && binding.IsApplied;
			}
			catch (Exception swallowed)
			{
				LogSuppressed(swallowed, "QuickMenuController.GearSwitcher.NailDamage.cs");
				return false;
			}
		}

		private static int ComputeDisplayedNailDamage(int flatDamage, bool strengthEquipped, bool furyMultiplier)
		{
			int damage = Mathf.Max(0, flatDamage);

			if (strengthEquipped)
			{
				damage = Mathf.Max(0, Mathf.RoundToInt(damage * 1.5f));
			}

			if (furyMultiplier)
			{
				damage = Mathf.Max(0, Mathf.RoundToInt(damage * 1.75f));
			}

			return damage;
		}

		private static int ApplyNailBindingBaseDamage(int baseDamage, bool nailBindingApplied)
		{
			int clampedBaseDamage = Mathf.Max(0, baseDamage);
			if (!nailBindingApplied)
			{
				return clampedBaseDamage;
			}

			if (clampedBaseDamage > 13)
			{
				return 13;
			}

			return Mathf.Max(0, Mathf.RoundToInt(clampedBaseDamage * 0.8f));
		}

		private static bool IsCharmEquipped(PlayerData data, int charmId)
		{
			try
			{
				return data.GetBoolInternal($"equippedCharm_{charmId}");
			}
			catch
			{
				try
				{
					return data.GetBool($"equippedCharm_{charmId}");
				}
				catch (Exception swallowed)
				{
					LogSuppressed(swallowed, "QuickMenuController.GearSwitcher.NailDamage.cs");
					return false;
				}
			}
		}

		private void CreateGearSwitcherBaseNailDamageEffectIcons()
		{
			if (gearSwitcherBaseNailDamageFuryIcon != null
				&& gearSwitcherBaseNailDamageStrengthIcon != null
				&& gearSwitcherBaseNailDamageRightStrengthIcon != null
				&& gearSwitcherBaseNailDamageNailBindingIcon != null
				&& gearSwitcherBaseNailDamageClosingParenValue != null)
			{
				UpdateGearSwitcherBaseNailDamageEffectIcons(BuildGearSwitcherBaseNailDamageDisplayLayout());
				return;
			}

			if (gearSwitcherBaseNailDamageValue == null)
			{
				return;
			}

			Transform? rowTransform = gearSwitcherBaseNailDamageValue.transform.parent;
			if (rowTransform == null)
			{
				return;
			}

			gearSwitcherBaseNailDamageStrengthIcon = CreateGearSwitcherBaseNailDamageEffectIcon(rowTransform, "BaseNailDamageStrengthIcon", -148f);
			gearSwitcherBaseNailDamageRightStrengthIcon = CreateGearSwitcherBaseNailDamageEffectIcon(rowTransform, "BaseNailDamageRightStrengthIcon", -116f);
			gearSwitcherBaseNailDamageFuryIcon = CreateGearSwitcherBaseNailDamageEffectIcon(rowTransform, "BaseNailDamageFuryIcon", -84f);
			gearSwitcherBaseNailDamageNailBindingIcon = CreateGearSwitcherBaseNailDamageEffectIcon(rowTransform, "BaseNailDamageNailBindingIcon", -52f);
			gearSwitcherBaseNailDamageClosingParenValue = CreateGearSwitcherBaseNailDamageClosingParenLabel(rowTransform);
			UpdateGearSwitcherBaseNailDamageEffectIcons(BuildGearSwitcherBaseNailDamageDisplayLayout());
		}

		private static Image CreateGearSwitcherBaseNailDamageEffectIcon(Transform parent, string name, float xOffset)
		{
			GameObject iconObject = new GameObject(name);
			iconObject.transform.SetParent(parent, worldPositionStays: false);

			RectTransform rect = iconObject.AddComponent<RectTransform>();
			rect.anchorMin = new Vector2(1f, 0.5f);
			rect.anchorMax = new Vector2(1f, 0.5f);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = new Vector2(xOffset, 0f);
			rect.sizeDelta = new Vector2(28f, 28f);

			Image icon = iconObject.AddComponent<Image>();
			icon.preserveAspect = true;
			iconObject.SetActive(false);
			return icon;
		}

		private Text CreateGearSwitcherBaseNailDamageClosingParenLabel(Transform parent)
		{
			Text label = CreateText(parent, "BaseNailDamageClosingParen", ")", 26, TextAnchor.MiddleRight);
			RectTransform rect = label.rectTransform;
			rect.anchorMin = new Vector2(1f, 0f);
			rect.anchorMax = new Vector2(1f, 1f);
			rect.pivot = new Vector2(1f, 0.5f);
			rect.anchoredPosition = new Vector2(-20f, 0f);
			rect.sizeDelta = new Vector2(18f, RowHeight);
			return label;
		}

		private void UpdateGearSwitcherBaseNailDamageEffectIcons(GearSwitcherBaseNailDamageDisplayLayout layout)
		{
			PlayerData? data = PlayerData.instance;
			bool furyEquipped = data != null && IsCharmEquipped(data, 6);
			bool strengthEquipped = data != null && IsCharmEquipped(data, 25);
			bool nailBindingApplied = IsNailBindingApplied();

			unbreakableStrengthSprite ??= LoadCollectorIconSprite("Unbreakable Strength.png", "UnbreakableStrength");
			furyOfTheFallenSprite ??= LoadCollectorIconSprite("Fury of the Fallen.png", "FuryOfTheFallen");
			nailBindingSprite ??= LoadCollectorIconSprite("Nail Binding.png", "NailBinding") ?? GetBindingDefaultSprite<NailBinding>();

			if (gearSwitcherBaseNailDamageStrengthIcon != null)
			{
				gearSwitcherBaseNailDamageStrengthIcon.sprite = unbreakableStrengthSprite;
			}

			if (gearSwitcherBaseNailDamageRightStrengthIcon != null)
			{
				gearSwitcherBaseNailDamageRightStrengthIcon.sprite = unbreakableStrengthSprite;
			}

			if (gearSwitcherBaseNailDamageFuryIcon != null)
			{
				gearSwitcherBaseNailDamageFuryIcon.sprite = furyOfTheFallenSprite;
			}

			if (gearSwitcherBaseNailDamageNailBindingIcon != null)
			{
				gearSwitcherBaseNailDamageNailBindingIcon.sprite = nailBindingSprite;
			}

			if (gearSwitcherBaseNailDamageClosingParenValue != null)
			{
				gearSwitcherBaseNailDamageClosingParenValue.gameObject.SetActive(false);
			}

			UpdateGearSwitcherBaseNailDamageValueLayout();

			bool leftStrengthVisible = strengthEquipped && unbreakableStrengthSprite != null && layout.LeftStrengthSlotIndex >= 0;
			bool rightStrengthVisible = furyEquipped && strengthEquipped && unbreakableStrengthSprite != null && layout.RightStrengthSlotIndex >= 0;
			bool rightFuryVisible = furyEquipped && furyOfTheFallenSprite != null && layout.RightFurySlotIndex >= 0;
			bool nailBindingVisible = nailBindingApplied && nailBindingSprite != null && layout.NailBindingSlotIndex >= 0;

			SetGearSwitcherBaseNailDamageEffectIconAtSlot(gearSwitcherBaseNailDamageStrengthIcon, leftStrengthVisible, layout.LeftStrengthSlotIndex, layout.DisplayText);
			SetGearSwitcherBaseNailDamageEffectIconAtSlot(gearSwitcherBaseNailDamageRightStrengthIcon, rightStrengthVisible, layout.RightStrengthSlotIndex, layout.DisplayText);
			SetGearSwitcherBaseNailDamageEffectIconAtSlot(gearSwitcherBaseNailDamageFuryIcon, rightFuryVisible, layout.RightFurySlotIndex, layout.DisplayText);
			SetGearSwitcherBaseNailDamageEffectIconAtSlot(gearSwitcherBaseNailDamageNailBindingIcon, nailBindingVisible, layout.NailBindingSlotIndex, layout.DisplayText);
		}

		private void UpdateGearSwitcherBaseNailDamageValueLayout()
		{
			if (gearSwitcherBaseNailDamageValue == null)
			{
				return;
			}

			RectTransform valueRect = gearSwitcherBaseNailDamageValue.rectTransform;
			valueRect.anchoredPosition = new Vector2(-20f, 0f);
			valueRect.sizeDelta = new Vector2(320f, RowHeight);
		}

		private float GetGearSwitcherBaseNailDamageTextWidth(string text)
		{
			if (gearSwitcherBaseNailDamageValue == null || string.IsNullOrEmpty(text))
			{
				return 0f;
			}

			TextGenerationSettings settings = gearSwitcherBaseNailDamageValue.GetGenerationSettings(new Vector2(1000f, RowHeight));
			settings.horizontalOverflow = HorizontalWrapMode.Overflow;
			settings.verticalOverflow = VerticalWrapMode.Overflow;
			settings.generateOutOfBounds = true;

			float width = gearSwitcherBaseNailDamageValue.cachedTextGeneratorForLayout.GetPreferredWidth(text, settings);
			float pixelsPerUnit = Mathf.Max(1f, gearSwitcherBaseNailDamageValue.pixelsPerUnit);
			return width / pixelsPerUnit;
		}

		private void SetGearSwitcherBaseNailDamageEffectIconAtSlot(Image? icon, bool visible, int slotStartIndex, string displayText)
		{
			if (icon == null)
			{
				return;
			}

			bool slotExists = slotStartIndex >= 0 && slotStartIndex + GearSwitcherBaseNailDamageSlot.Length <= displayText.Length;
			icon.gameObject.SetActive(visible && slotExists);
			if (!visible || !slotExists)
			{
				return;
			}

			int suffixStart = slotStartIndex + GearSwitcherBaseNailDamageSlot.Length;
			string suffix = displayText.Substring(suffixStart);
			float suffixWidth = GetGearSwitcherBaseNailDamageTextWidth(suffix);
			float slotWidth = GetGearSwitcherBaseNailDamageTextWidth(GearSwitcherBaseNailDamageSlot);
			float rightEdgeDistance = gearSwitcherBaseNailDamageValue != null
				? Mathf.Max(0f, -gearSwitcherBaseNailDamageValue.rectTransform.anchoredPosition.x)
				: 20f;

			float xOffset = -(rightEdgeDistance + suffixWidth + (slotWidth * 0.5f));
			icon.rectTransform.anchoredPosition = new Vector2(xOffset, 0f);
		}

		private void SetSelectedPresetNailDamage(int value)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			int num = Math.Max(-99999, Math.Min(99999, value));
			if (selectedPreset.NailDamage != num)
			{
				selectedPreset.NailDamage = num;
				GodhomeQoL.SaveGlobalSettingsSafe();
				GearSwitcher.ApplyStatsImmediate(selectedPreset, GearSwitcher.StatsPart.NailDamage);
				MarkGearSwitcherPresetEdited();
			}
		}
    }
}
