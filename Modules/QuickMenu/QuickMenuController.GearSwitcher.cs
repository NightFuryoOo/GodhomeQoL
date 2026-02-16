using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ToggleableBindings;
using ToggleableBindings.VanillaBindings;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
		private void CreateGearSwitcherSpellsRow(Transform parent, float y)
		{
			GameObject gameObject = CreateRow(parent, "GearSwitcherSpellsRow", y, new Vector2(820f, 44f));
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			CreateGearSwitcherSpellIcon(gameObject.transform, "FireballIcon", -73f, "fireballLevel", UpdateGearSwitcherFireballIcon, out gearSwitcherFireballIcon, out gearSwitcherFireballGlow);
			CreateGearSwitcherSpellIcon(gameObject.transform, "QuakeIcon", 0f, "quakeLevel", UpdateGearSwitcherQuakeIcon, out gearSwitcherQuakeIcon, out gearSwitcherQuakeGlow);
			CreateGearSwitcherSpellIcon(gameObject.transform, "ScreamIcon", 70f, "screamLevel", UpdateGearSwitcherScreamIcon, out gearSwitcherScreamIcon, out gearSwitcherScreamGlow);
			UpdateGearSwitcherFireballIcon();
			UpdateGearSwitcherQuakeIcon();
			UpdateGearSwitcherScreamIcon();
		}

		private void CreateGearSwitcherNailArtsRow(Transform parent, float y)
		{
			GameObject gameObject = CreateRow(parent, "GearSwitcherNailArtsRow", y, new Vector2(820f, 44f));
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			CreateGearSwitcherNailArtIcon(gameObject.transform, "CycloneSlashIcon", -70f, "hasCyclone", UpdateGearSwitcherCycloneIcon, out gearSwitcherCycloneIcon, out gearSwitcherCycloneGlow);
			CreateGearSwitcherNailArtIcon(gameObject.transform, "DashSlashIcon", 0f, "hasDashSlash", UpdateGearSwitcherDashSlashIcon, out gearSwitcherDashSlashIcon, out gearSwitcherDashSlashGlow);
			CreateGearSwitcherNailArtIcon(gameObject.transform, "GreatSlashIcon", 70f, "hasUpwardSlash", UpdateGearSwitcherGreatSlashIcon, out gearSwitcherGreatSlashIcon, out gearSwitcherGreatSlashGlow);
			UpdateGearSwitcherCycloneIcon();
			UpdateGearSwitcherDashSlashIcon();
			UpdateGearSwitcherGreatSlashIcon();
		}

		private void CreateGearSwitcherCloakRow(Transform parent, float y)
		{
			GameObject gameObject = CreateRow(parent, "GearSwitcherCloakRow", y, new Vector2(820f, 44f));
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			gearSwitcherCloakRowIconRects.Clear();
			CreateGearSwitcherCloakIcon(gameObject.transform, -210f);
			CreateGearSwitcherMoveAbilityIcon(gameObject.transform, "MantisClawIcon", -140f, "Walljump", out gearSwitcherMantisClawIcon, out gearSwitcherMantisClawGlow);
			CreateGearSwitcherMoveAbilityIcon(gameObject.transform, "MonarchWingsIcon", -70f, "DoubleJump", out gearSwitcherMonarchWingsIcon, out gearSwitcherMonarchWingsGlow);
			CreateGearSwitcherMoveAbilityIcon(gameObject.transform, "CrystalHeartIcon", 0f, "SuperDash", out gearSwitcherCrystalHeartIcon, out gearSwitcherCrystalHeartGlow);
			CreateGearSwitcherMoveAbilityIcon(gameObject.transform, "IsmasTearIcon", 70f, "AcidArmour", out gearSwitcherIsmasTearIcon, out gearSwitcherIsmasTearGlow);
			CreateGearSwitcherDreamNailIcon(gameObject.transform, 140f);
			CreateGearSwitcherDreamgateIcon(gameObject.transform, 210f);
			UpdateGearSwitcherCloakRowIcons();
		}

		private void CreateGearSwitcherBindingsRow(Transform parent, float y)
		{
			GameObject gameObject = CreateRow(parent, "GearSwitcherBindingsRow", y, new Vector2(820f, 44f));
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			gearSwitcherBindingsRowIconRects.Clear();
			CreateGearSwitcherBindingIcon(gameObject.transform, "BindingNailIcon", -105f, "NailBinding", out gearSwitcherBindingNailIcon, out gearSwitcherBindingNailGlow);
			CreateGearSwitcherBindingIcon(gameObject.transform, "BindingShellIcon", -35f, "ShellBinding", out gearSwitcherBindingShellIcon, out gearSwitcherBindingShellGlow);
			CreateGearSwitcherBindingIcon(gameObject.transform, "BindingCharmsIcon", 35f, "CharmsBinding", out gearSwitcherBindingCharmsIcon, out gearSwitcherBindingCharmsGlow);
			CreateGearSwitcherBindingIcon(gameObject.transform, "BindingSoulIcon", 105f, "SoulBinding", out gearSwitcherBindingSoulIcon, out gearSwitcherBindingSoulGlow);
			UpdateGearSwitcherBindingsRowIcons();
		}

		private void CreateGearSwitcherHpMaskRow(Transform parent, float y)
		{
			GameObject gameObject = CreateRow(parent, "GearSwitcherHpMaskRow", y, new Vector2(820f, 44f));
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			gearSwitcherHpMaskIcons.Clear();
			gearSwitcherHpMaskGlows.Clear();
			gearSwitcherHpMaskIconRects.Clear();
			for (int i = 0; i < 9; i++)
			{
				CreateGearSwitcherHpMaskIcon(gameObject.transform, (float)(i - 4) * 50f, i + 1);
			}
			UpdateGearSwitcherHpMaskIcons();
		}

		private void CreateGearSwitcherHpMaskIcon(Transform parent, float offsetX, int value)
		{
			GameObject gameObject = new GameObject($"HpMask_{value}");
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(offsetX, 5f);
			rectTransform.sizeDelta = new Vector2(44f, 44f);
			Image image = gameObject.AddComponent<Image>();
			image.preserveAspect = true;
			Outline outline = gameObject.AddComponent<Outline>();
			outline.effectColor = GearSwitcherSpellGlowColor;
			outline.effectDistance = new Vector2(2f, -2f);
			outline.enabled = false;
			Button button = gameObject.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.targetGraphic = image;
			button.onClick.AddListener(delegate
			{
				SetSelectedPresetMaxHealth(value);
				GearSwitcher.ApplyStatsImmediate(GetSelectedPreset());
				UpdateGearSwitcherHpMaskIcons();
			});
			gearSwitcherHpMaskIcons.Add(image);
			gearSwitcherHpMaskGlows.Add(outline);
			gearSwitcherHpMaskIconRects.Add(rectTransform);
		}

		private void CreateGearSwitcherSoulVesselRow(Transform parent, float y)
		{
			GameObject gameObject = CreateRow(parent, "GearSwitcherSoulVesselRow", y, new Vector2(820f, 44f));
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			gearSwitcherSoulVesselIcons.Clear();
			gearSwitcherSoulVesselGlows.Clear();
			gearSwitcherSoulVesselIconRects.Clear();
			for (int i = 0; i < 3; i++)
			{
				CreateGearSwitcherSoulVesselIcon(gameObject.transform, (float)(i - 1) * 60f, i + 1);
			}
			UpdateGearSwitcherSoulVesselIcons();
		}

		private void CreateGearSwitcherSoulVesselIcon(Transform parent, float offsetX, int value)
		{
			GameObject gameObject = new GameObject($"SoulVessel_{value}");
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(offsetX, 0f);
			rectTransform.sizeDelta = new Vector2(44f, 44f);
			Image image = gameObject.AddComponent<Image>();
			image.preserveAspect = true;
			Outline outline = gameObject.AddComponent<Outline>();
			outline.effectColor = GearSwitcherSpellGlowColor;
			outline.effectDistance = new Vector2(2f, -2f);
			outline.enabled = false;
			Button button = gameObject.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.targetGraphic = image;
			button.onClick.AddListener(delegate
			{
				int soulVessels = GetSelectedPreset().SoulVessels;
				int selectedPresetSoulVessels = value;
				if (value == 1 && soulVessels == 1)
				{
					selectedPresetSoulVessels = 0;
				}
				SetSelectedPresetSoulVessels(selectedPresetSoulVessels);
				GearSwitcher.ApplyStatsImmediate(GetSelectedPreset());
				UpdateGearSwitcherSoulVesselIcons();
			});
			gearSwitcherSoulVesselIcons.Add(image);
			gearSwitcherSoulVesselGlows.Add(outline);
			gearSwitcherSoulVesselIconRects.Add(rectTransform);
		}

		private void CreateGearSwitcherNaillessRow(Transform parent, float y)
		{
			GameObject gameObject = CreateRow(parent, "GearSwitcherNaillessRow", y, new Vector2(820f, 44f));
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			GameObject gameObject2 = new GameObject("NaillessIcon");
			gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
			RectTransform rectTransform = gameObject2.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(-50f, -24f);
			rectTransform.sizeDelta = new Vector2(88f, 88f);
			Image image2 = gameObject2.AddComponent<Image>();
			image2.preserveAspect = true;
			Outline outline = gameObject2.AddComponent<Outline>();
			outline.effectColor = GearSwitcherSpellGlowColor;
			outline.effectDistance = new Vector2(2f, -2f);
			outline.enabled = false;
			Button button = gameObject2.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.targetGraphic = image2;
			button.onClick.AddListener(delegate
			{
				bool selectedNailless = GetSelectedNailless();
				SetSelectedNailless(!selectedNailless);
				GearSwitcher.ApplyNailInputImmediate(GetSelectedPreset());
				UpdateGearSwitcherNaillessIcon();
			});
			gearSwitcherNaillessIcon = image2;
			gearSwitcherNaillessGlow = outline;
			UpdateGearSwitcherNaillessIcon();
			GameObject gameObject3 = new GameObject("OvercharmedIcon");
			gameObject3.transform.SetParent(gameObject.transform, worldPositionStays: false);
			RectTransform rectTransform2 = gameObject3.AddComponent<RectTransform>();
			rectTransform2.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform2.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform2.pivot = new Vector2(0.5f, 0.5f);
			rectTransform2.anchoredPosition = new Vector2(50f, -24f);
			rectTransform2.sizeDelta = new Vector2(88f, 88f);
			Image image3 = gameObject3.AddComponent<Image>();
			image3.preserveAspect = true;
			Outline outline2 = gameObject3.AddComponent<Outline>();
			outline2.effectColor = GearSwitcherSpellGlowColor;
			outline2.effectDistance = new Vector2(2f, -2f);
			outline2.enabled = false;
			Button button2 = gameObject3.AddComponent<Button>();
			button2.transition = Selectable.Transition.None;
			button2.targetGraphic = image3;
			button2.onClick.AddListener(delegate
			{
				bool selectedOvercharmed = GetSelectedOvercharmed();
				SetSelectedOvercharmed(!selectedOvercharmed);
				GearSwitcher.ApplyOvercharmedImmediate(GetSelectedPreset());
				UpdateGearSwitcherOvercharmedIcon();
			});
			gearSwitcherOvercharmedIcon = image3;
			gearSwitcherOvercharmedGlow = outline2;
			UpdateGearSwitcherOvercharmedIcon();
		}

		private void CreateGearSwitcherCharmPromptRow(Transform parent, float y)
		{
			GameObject gameObject = CreateRow(parent, "GearSwitcherCharmPromptRow", y, new Vector2(820f, 88f));
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			GameObject gameObject2 = new GameObject("CharmsPromptIcon");
			gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
			RectTransform rectTransform = gameObject2.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(0f, -24f);
			rectTransform.sizeDelta = new Vector2(88f, 88f);
			Image image2 = gameObject2.AddComponent<Image>();
			image2.preserveAspect = true;
			Button button = gameObject2.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.targetGraphic = image2;
			button.onClick.AddListener(OnGearSwitcherCharmPromptClicked);
			gearSwitcherCharmPromptIcon = image2;
			UpdateGearSwitcherCharmPromptIcon();
		}

		private void CreateGearSwitcherPresetRow(Transform parent, int index, string presetName, float y)
		{
			GameObject gameObject = CreateRow(parent, $"GearSwitcherPresetRow{index}", y, new Vector2(820f, 44f));
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			Button button = gameObject.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.targetGraphic = image;
			Text label = CreateRowLabel(gameObject.transform, GetGearSwitcherPresetDisplayName(presetName));
			button.onClick.AddListener(delegate
			{
				OnGearSwitcherPresetSelected(index);
			});
			RowHighlight highlight = CreateRowHighlight(gameObject, image);
			AttachRowHighlight(gameObject, highlight);
			gearSwitcherPresetEntries.Add(new GearSwitcherPresetEntry(index, label, highlight));
			if (!IsBuiltinPresetName(presetName))
			{
				Button? button2 = CreateGearSwitcherPresetEditButton(gameObject.transform, delegate
				{
					StartGearSwitcherPresetRename(label, presetName);
				});
				if (button2 != null)
				{
					AttachRowHighlight(button2.gameObject, highlight);
				}
			}
			if (!IsBuiltinPresetName(presetName))
			{
				Button? button3 = CreateGearSwitcherPresetDeleteButton(gameObject.transform, delegate
				{
					StartGearSwitcherPresetDeleteByIndex(index);
				});
				if (button3 != null)
				{
					AttachRowHighlight(button3.gameObject, highlight);
				}
			}
		}

		private Button? CreateGearSwitcherPresetEditButton(Transform parent, Action onClick)
		{
			gearSwitcherPresetEditSprite ??= LoadCollectorIconSprite("Edit.png", "GearPresetEdit");
			if (gearSwitcherPresetEditSprite == null)
			{
				return null;
			}

			return CreateGearSwitcherPresetIconButton(parent, "PresetEdit", gearSwitcherPresetEditSprite, new Vector2(340f, 0f), onClick);
		}

		private Button? CreateGearSwitcherPresetDeleteButton(Transform parent, Action onClick)
		{
			gearSwitcherPresetDeleteSprite ??= LoadCollectorIconSprite("Del.png", "GearPresetDelete");
			if (gearSwitcherPresetDeleteSprite == null)
			{
				return null;
			}

			return CreateGearSwitcherPresetIconButton(parent, "PresetDelete", gearSwitcherPresetDeleteSprite, new Vector2(380f, 0f), onClick);
		}

		private Button CreateGearSwitcherPresetIconButton(Transform parent, string name, Sprite sprite, Vector2 position, Action onClick)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = position;
			rectTransform.sizeDelta = new Vector2(28f, 28f);
			Image image = gameObject.AddComponent<Image>();
			image.sprite = sprite;
			image.preserveAspect = true;
			Button button = gameObject.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.targetGraphic = image;
			button.onClick.AddListener(delegate
			{
				onClick();
			});
			return button;
		}

		private void CreateGearSwitcherPresetDeleteConfirm(Transform parent)
		{
			gearSwitcherPresetDeleteRoot = new GameObject("GearSwitcherPresetDeleteConfirm");
			gearSwitcherPresetDeleteRoot.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = gearSwitcherPresetDeleteRoot.AddComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			Image image = gearSwitcherPresetDeleteRoot.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0.6f);
			CanvasGroup canvasGroup = gearSwitcherPresetDeleteRoot.AddComponent<CanvasGroup>();
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			GameObject gameObject = new GameObject("Dialog");
			gameObject.transform.SetParent(gearSwitcherPresetDeleteRoot.transform, worldPositionStays: false);
			RectTransform rectTransform2 = gameObject.AddComponent<RectTransform>();
			rectTransform2.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform2.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform2.pivot = new Vector2(0.5f, 0.5f);
			rectTransform2.anchoredPosition = Vector2.zero;
			rectTransform2.sizeDelta = new Vector2(520f, 220f);
			Image image2 = gameObject.AddComponent<Image>();
			image2.color = OverlayPanelColor;
			Text text = CreateText(gameObject.transform, "Label", "Delete preset?", 28, TextAnchor.MiddleCenter);
			RectTransform rectTransform3 = text.rectTransform;
			rectTransform3.anchorMin = new Vector2(0.5f, 1f);
			rectTransform3.anchorMax = new Vector2(0.5f, 1f);
			rectTransform3.pivot = new Vector2(0.5f, 1f);
			rectTransform3.anchoredPosition = new Vector2(0f, -20f);
			rectTransform3.sizeDelta = new Vector2(480f, 50f);
			gearSwitcherPresetDeleteLabel = text;
			CreateButtonRow(gameObject.transform, "GearSwitcherPresetDeleteYesRow", "Yes", -30f, OnGearSwitcherPresetDeleteYes);
			CreateButtonRow(gameObject.transform, "GearSwitcherPresetDeleteNoRow", "No", -80f, OnGearSwitcherPresetDeleteNo);
		}

		private void CreateGearSwitcherResetConfirm(Transform parent)
		{
			gearSwitcherResetConfirmRoot = new GameObject("GearSwitcherResetConfirm");
			gearSwitcherResetConfirmRoot.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = gearSwitcherResetConfirmRoot.AddComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			Image image = gearSwitcherResetConfirmRoot.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0.6f);
			CanvasGroup canvasGroup = gearSwitcherResetConfirmRoot.AddComponent<CanvasGroup>();
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			GameObject gameObject = new GameObject("Dialog");
			gameObject.transform.SetParent(gearSwitcherResetConfirmRoot.transform, worldPositionStays: false);
			RectTransform rectTransform2 = gameObject.AddComponent<RectTransform>();
			rectTransform2.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform2.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform2.pivot = new Vector2(0.5f, 0.5f);
			rectTransform2.anchoredPosition = Vector2.zero;
			rectTransform2.sizeDelta = new Vector2(640f, 260f);
			Image image2 = gameObject.AddComponent<Image>();
			image2.color = OverlayPanelColor;
			Text text = CreateText(gameObject.transform, "Label", "Are you sure? This will remove all presets you've created. Do you want to continue?", 24, TextAnchor.MiddleCenter);
			text.horizontalOverflow = HorizontalWrapMode.Wrap;
			text.verticalOverflow = VerticalWrapMode.Overflow;
			RectTransform rectTransform3 = text.rectTransform;
			rectTransform3.anchorMin = new Vector2(0.5f, 1f);
			rectTransform3.anchorMax = new Vector2(0.5f, 1f);
			rectTransform3.pivot = new Vector2(0.5f, 1f);
			rectTransform3.anchoredPosition = new Vector2(0f, -20f);
			rectTransform3.sizeDelta = new Vector2(600f, 120f);
			CreateButtonRow(gameObject.transform, "GearSwitcherResetYesRow", "Yes", -70f, OnGearSwitcherResetConfirmYes);
			CreateButtonRow(gameObject.transform, "GearSwitcherResetNoRow", "No", -120f, OnGearSwitcherResetConfirmNo);
		}

		private void SetGearSwitcherPresetDeleteVisible(bool value)
		{
			gearSwitcherPresetDeleteVisible = value;
			if (gearSwitcherPresetDeleteRoot != null)
			{
				gearSwitcherPresetDeleteRoot.SetActive(value);
			}
			if (!value)
			{
				gearSwitcherPresetDeleteTargetName = null;
			}
		}

		private void SetGearSwitcherResetConfirmVisible(bool value)
		{
			gearSwitcherResetConfirmVisible = value;
			if (gearSwitcherResetConfirmRoot != null)
			{
				gearSwitcherResetConfirmRoot.SetActive(value);
			}
		}

		private void CreateQuickSettingsResetConfirm(Transform parent)
		{
			quickSettingsResetConfirmRoot = new GameObject("QuickSettingsResetConfirm");
			quickSettingsResetConfirmRoot.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = quickSettingsResetConfirmRoot.AddComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			Image image = quickSettingsResetConfirmRoot.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0.6f);
			CanvasGroup canvasGroup = quickSettingsResetConfirmRoot.AddComponent<CanvasGroup>();
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			GameObject gameObject = new GameObject("Dialog");
			gameObject.transform.SetParent(quickSettingsResetConfirmRoot.transform, worldPositionStays: false);
			RectTransform rectTransform2 = gameObject.AddComponent<RectTransform>();
			rectTransform2.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform2.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform2.pivot = new Vector2(0.5f, 0.5f);
			rectTransform2.anchoredPosition = Vector2.zero;
			rectTransform2.sizeDelta = new Vector2(760f, 300f);
			Image image2 = gameObject.AddComponent<Image>();
			image2.color = OverlayPanelColor;
			Text text = CreateText(gameObject.transform, "Label", "Are you serious? This will reset all mod settings to the default ones and delete all current settings", 24, TextAnchor.MiddleCenter);
			text.horizontalOverflow = HorizontalWrapMode.Wrap;
			text.verticalOverflow = VerticalWrapMode.Overflow;
			RectTransform rectTransform3 = text.rectTransform;
			rectTransform3.anchorMin = new Vector2(0.5f, 1f);
			rectTransform3.anchorMax = new Vector2(0.5f, 1f);
			rectTransform3.pivot = new Vector2(0.5f, 1f);
			rectTransform3.anchoredPosition = new Vector2(0f, -24f);
			rectTransform3.sizeDelta = new Vector2(700f, 150f);
			CreateButtonRow(gameObject.transform, "QuickSettingsResetYesRow", "Yes", -90f, OnQuickSettingsResetConfirmYes);
			CreateButtonRow(gameObject.transform, "QuickSettingsResetNoRow", "No", -150f, OnQuickSettingsResetConfirmNo);
		}

		private void SetQuickSettingsResetConfirmVisible(bool value)
		{
			quickSettingsResetConfirmVisible = value;
			if (quickSettingsResetConfirmRoot != null)
			{
				quickSettingsResetConfirmRoot.SetActive(value);
			}
		}

		private Transform CreateGearSwitcherCharmCostsRow(Transform parent, string name, float y)
		{
			GameObject gameObject = CreateRow(parent, name, y, new Vector2(820f, 120f));
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			return gameObject.transform;
		}

		private float CharmCostColumnX(int index)
		{
			return -260f + 130f * (float)index;
		}

		private Image CreateCharmCostColumn(Transform parent, string name, float columnCenterX, string spriteName, string spriteKey, ref Text? valueField, Action<int> adjustCost, Action? iconClick = null, bool allowAdjust = true)
		{
			GameObject gameObject = new GameObject(name + "Icon");
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(columnCenterX, 16f);
			rectTransform.sizeDelta = new Vector2(44f, 44f);
			Image image = gameObject.AddComponent<Image>();
			image.preserveAspect = true;
			Outline outline = gameObject.AddComponent<Outline>();
			outline.effectColor = GearSwitcherCharmCostGlowColor;
			outline.effectDistance = new Vector2(4f, -4f);
			outline.enabled = false;
			gearSwitcherCharmCostGlows[name] = outline;
			Sprite? sprite = LoadCollectorIconSprite(spriteName, spriteKey);
			if (sprite != null)
			{
				image.sprite = sprite;
				float num = Mathf.Min(1.4f, 64f / sprite.rect.height);
				rectTransform.sizeDelta = new Vector2(sprite.rect.width * num, sprite.rect.height * num);
			}
			if (iconClick != null)
			{
				Button button = gameObject.AddComponent<Button>();
				button.transition = Selectable.Transition.None;
				button.targetGraphic = image;
				button.onClick.AddListener(delegate
				{
					iconClick();
				});
			}
			GameObject gameObject2 = new GameObject(name + "Value");
			gameObject2.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform2 = gameObject2.AddComponent<RectTransform>();
			rectTransform2.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform2.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform2.pivot = new Vector2(0.5f, 0.5f);
			rectTransform2.anchoredPosition = new Vector2(columnCenterX, -24f);
			rectTransform2.sizeDelta = new Vector2(40f, 44f);
			Text text = CreateText(gameObject2.transform, "Value", "1", 22, TextAnchor.MiddleCenter);
			RectTransform rectTransform3 = text.rectTransform;
			rectTransform3.anchorMin = Vector2.zero;
			rectTransform3.anchorMax = Vector2.one;
			rectTransform3.offsetMin = Vector2.zero;
			rectTransform3.offsetMax = Vector2.zero;
			valueField = text;
			if (allowAdjust)
			{
				Button button2 = CreateCenteredMiniButton(parent, name + "Minus", "-", new Vector2(columnCenterX - 22f, -24f));
				button2.onClick.AddListener(delegate
				{
					adjustCost(-1);
				});
				Button button3 = CreateCenteredMiniButton(parent, name + "Plus", "+", new Vector2(columnCenterX + 22f, -24f));
				button3.onClick.AddListener(delegate
				{
					adjustCost(1);
				});
			}
			return image;
		}

		private void AddCharmSwapBadge(Image? icon)
		{
			if (!(icon == null))
			{
				GameObject gameObject = new GameObject("SwapBadge");
				gameObject.transform.SetParent(icon.transform, worldPositionStays: false);
				RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
				rectTransform.anchorMin = new Vector2(1f, 1f);
				rectTransform.anchorMax = new Vector2(1f, 1f);
				rectTransform.pivot = new Vector2(1f, 1f);
				rectTransform.anchoredPosition = new Vector2(-2f, -2f);
				rectTransform.sizeDelta = new Vector2(34f, 16f);
				Text text = gameObject.AddComponent<Text>();
				text.text = "SWAP";
				text.font = GetMenuFont();
				text.fontSize = 12;
				text.alignment = TextAnchor.MiddleCenter;
				text.color = new Color(1f, 1f, 1f, 0.9f);
				text.raycastTarget = false;
				Outline outline = gameObject.AddComponent<Outline>();
				outline.effectColor = new Color(0f, 0f, 0f, 0.8f);
				outline.effectDistance = new Vector2(1f, -1f);
			}
		}

		private void CreateCharmCostAdjustButtons(Transform parent, string name, float columnCenterX, Action<int> adjustCost)
		{
			Button button = CreateCenteredMiniButton(parent, name + "Minus", "-", new Vector2(columnCenterX - 22f, -24f));
			button.onClick.AddListener(delegate
			{
				adjustCost(-1);
			});
			button.transform.SetAsLastSibling();
			Button button2 = CreateCenteredMiniButton(parent, name + "Plus", "+", new Vector2(columnCenterX + 22f, -24f));
			button2.onClick.AddListener(delegate
			{
				adjustCost(1);
			});
			button2.transform.SetAsLastSibling();
		}

		private void RegisterCharmCostHighlight(string key, Text? valueText, Func<GearPreset, int> currentCost, Func<GearPreset, int> defaultCost)
		{
			if (!(valueText == null) && gearSwitcherCharmCostGlows.TryGetValue(key, out Outline value))
			{
				gearSwitcherCharmCostHighlightEntries.Add(new CharmCostHighlightEntry(valueText, value, currentCost, defaultCost));
			}
		}

		private void RegisterGearSwitcherCharmCostHighlights()
		{
			gearSwitcherCharmCostHighlightEntries.Clear();
			RegisterCharmCostHighlight("WaywardCompass", gearSwitcherWaywardCostValue, (GearPreset preset) => preset.WaywardCompassCost, (GearPreset _) => DefaultGearPreset.WaywardCompassCost);
			RegisterCharmCostHighlight("GatheringSwarm", gearSwitcherGatheringCostValue, (GearPreset preset) => preset.GatheringSwarmCost, (GearPreset _) => DefaultGearPreset.GatheringSwarmCost);
			RegisterCharmCostHighlight("StalwartShell", gearSwitcherStalwartShellCostValue, (GearPreset preset) => preset.StalwartShellCost, (GearPreset _) => DefaultGearPreset.StalwartShellCost);
			RegisterCharmCostHighlight("SoulCatcher", gearSwitcherSoulCatcherCostValue, (GearPreset preset) => preset.SoulCatcherCost, (GearPreset _) => DefaultGearPreset.SoulCatcherCost);
			RegisterCharmCostHighlight("ShamanStone", gearSwitcherShamanStoneCostValue, (GearPreset preset) => preset.ShamanStoneCost, (GearPreset _) => DefaultGearPreset.ShamanStoneCost);
			RegisterCharmCostHighlight("SoulEater", gearSwitcherSoulEaterCostValue, (GearPreset preset) => preset.SoulEaterCost, (GearPreset _) => DefaultGearPreset.SoulEaterCost);
			RegisterCharmCostHighlight("Dashmaster", gearSwitcherDashmasterCostValue, (GearPreset preset) => preset.DashmasterCost, (GearPreset _) => DefaultGearPreset.DashmasterCost);
			RegisterCharmCostHighlight("Sprintmaster", gearSwitcherSprintmasterCostValue, (GearPreset preset) => preset.SprintmasterCost, (GearPreset _) => DefaultGearPreset.SprintmasterCost);
			RegisterCharmCostHighlight("Grubsong", gearSwitcherGrubsongCostValue, (GearPreset preset) => preset.GrubsongCost, (GearPreset _) => DefaultGearPreset.GrubsongCost);
			RegisterCharmCostHighlight("GrubberflysElegy", gearSwitcherGrubberflysElegyCostValue, (GearPreset preset) => preset.GrubberflysElegyCost, (GearPreset _) => DefaultGearPreset.GrubberflysElegyCost);
			RegisterCharmCostHighlight("UnbreakableHeart", gearSwitcherUnbreakableHeartCostValue, (GearPreset preset) => preset.UnbreakableHeartCost, (GearPreset _) => DefaultGearPreset.UnbreakableHeartCost);
			RegisterCharmCostHighlight("UnbreakableGreed", gearSwitcherUnbreakableGreedCostValue, (GearPreset preset) => preset.UnbreakableGreedCost, (GearPreset _) => DefaultGearPreset.UnbreakableGreedCost);
			RegisterCharmCostHighlight("UnbreakableStrength", gearSwitcherUnbreakableStrengthCostValue, (GearPreset preset) => preset.UnbreakableStrengthCost, (GearPreset _) => DefaultGearPreset.UnbreakableStrengthCost);
			RegisterCharmCostHighlight("SpellTwister", gearSwitcherSpellTwisterCostValue, (GearPreset preset) => preset.SpellTwisterCost, (GearPreset _) => DefaultGearPreset.SpellTwisterCost);
			RegisterCharmCostHighlight("SteadyBody", gearSwitcherSteadyBodyCostValue, (GearPreset preset) => preset.SteadyBodyCost, (GearPreset _) => DefaultGearPreset.SteadyBodyCost);
			RegisterCharmCostHighlight("HeavyBlow", gearSwitcherHeavyBlowCostValue, (GearPreset preset) => preset.HeavyBlowCost, (GearPreset _) => DefaultGearPreset.HeavyBlowCost);
			RegisterCharmCostHighlight("QuickSlash", gearSwitcherQuickSlashCostValue, (GearPreset preset) => preset.QuickSlashCost, (GearPreset _) => DefaultGearPreset.QuickSlashCost);
			RegisterCharmCostHighlight("Longnail", gearSwitcherLongnailCostValue, (GearPreset preset) => preset.LongnailCost, (GearPreset _) => DefaultGearPreset.LongnailCost);
			RegisterCharmCostHighlight("MarkOfPride", gearSwitcherMarkOfPrideCostValue, (GearPreset preset) => preset.MarkOfPrideCost, (GearPreset _) => DefaultGearPreset.MarkOfPrideCost);
			RegisterCharmCostHighlight("FuryOfTheFallen", gearSwitcherFuryOfTheFallenCostValue, (GearPreset preset) => preset.FuryOfTheFallenCost, (GearPreset _) => DefaultGearPreset.FuryOfTheFallenCost);
			RegisterCharmCostHighlight("ThornsOfAgony", gearSwitcherThornsOfAgonyCostValue, (GearPreset preset) => preset.ThornsOfAgonyCost, (GearPreset _) => DefaultGearPreset.ThornsOfAgonyCost);
			RegisterCharmCostHighlight("BaldurShell", gearSwitcherBaldurShellCostValue, (GearPreset preset) => preset.BaldurShellCost, (GearPreset _) => DefaultGearPreset.BaldurShellCost);
			RegisterCharmCostHighlight("Flukenest", gearSwitcherFlukenestCostValue, (GearPreset preset) => preset.FlukenestCost, (GearPreset _) => DefaultGearPreset.FlukenestCost);
			RegisterCharmCostHighlight("DefendersCrest", gearSwitcherDefendersCrestCostValue, (GearPreset preset) => preset.DefendersCrestCost, (GearPreset _) => DefaultGearPreset.DefendersCrestCost);
			RegisterCharmCostHighlight("GlowingWomb", gearSwitcherGlowingWombCostValue, (GearPreset preset) => preset.GlowingWombCost, (GearPreset _) => DefaultGearPreset.GlowingWombCost);
			RegisterCharmCostHighlight("QuickFocus", gearSwitcherQuickFocusCostValue, (GearPreset preset) => preset.QuickFocusCost, (GearPreset _) => DefaultGearPreset.QuickFocusCost);
			RegisterCharmCostHighlight("DeepFocus", gearSwitcherDeepFocusCostValue, (GearPreset preset) => preset.DeepFocusCost, (GearPreset _) => DefaultGearPreset.DeepFocusCost);
			RegisterCharmCostHighlight("LifebloodHeart", gearSwitcherLifebloodHeartCostValue, (GearPreset preset) => preset.LifebloodHeartCost, (GearPreset _) => DefaultGearPreset.LifebloodHeartCost);
			RegisterCharmCostHighlight("LifebloodCore", gearSwitcherLifebloodCoreCostValue, (GearPreset preset) => preset.LifebloodCoreCost, (GearPreset _) => DefaultGearPreset.LifebloodCoreCost);
			RegisterCharmCostHighlight("JonisBlessing", gearSwitcherJonisBlessingCostValue, (GearPreset preset) => preset.JonisBlessingCost, (GearPreset _) => DefaultGearPreset.JonisBlessingCost);
			RegisterCharmCostHighlight("Hiveblood", gearSwitcherHivebloodCostValue, (GearPreset preset) => preset.HivebloodCost, (GearPreset _) => DefaultGearPreset.HivebloodCost);
			RegisterCharmCostHighlight("SporeShroom", gearSwitcherSporeShroomCostValue, (GearPreset preset) => preset.SporeShroomCost, (GearPreset _) => DefaultGearPreset.SporeShroomCost);
			RegisterCharmCostHighlight("SharpShadow", gearSwitcherSharpShadowCostValue, (GearPreset preset) => preset.SharpShadowCost, (GearPreset _) => DefaultGearPreset.SharpShadowCost);
			RegisterCharmCostHighlight("ShapeOfUnn", gearSwitcherShapeOfUnnCostValue, (GearPreset preset) => preset.ShapeOfUnnCost, (GearPreset _) => DefaultGearPreset.ShapeOfUnnCost);
			RegisterCharmCostHighlight("NailmastersGlory", gearSwitcherNailmastersGloryCostValue, (GearPreset preset) => preset.NailmastersGloryCost, (GearPreset _) => DefaultGearPreset.NailmastersGloryCost);
			RegisterCharmCostHighlight("Weaversong", gearSwitcherWeaversongCostValue, (GearPreset preset) => preset.WeaversongCost, (GearPreset _) => DefaultGearPreset.WeaversongCost);
			RegisterCharmCostHighlight("DreamWielder", gearSwitcherDreamWielderCostValue, (GearPreset preset) => preset.DreamWielderCost, (GearPreset _) => DefaultGearPreset.DreamWielderCost);
			RegisterCharmCostHighlight("Dreamshield", gearSwitcherDreamshieldCostValue, (GearPreset preset) => preset.DreamshieldCost, (GearPreset _) => DefaultGearPreset.DreamshieldCost);
			RegisterCharmCostHighlight("CarefreeMelody", gearSwitcherCarefreeMelodyCostValue, (GearPreset preset) => GetPresetUseGrimmchild(preset) ? preset.GrimmchildCost : preset.CarefreeMelodyCost, (GearPreset preset) => GetPresetUseGrimmchild(preset) ? DefaultGearPreset.GrimmchildCost : DefaultGearPreset.CarefreeMelodyCost);
			RegisterCharmCostHighlight("VoidHeart", gearSwitcherVoidHeartCostValue, (GearPreset preset) => GetPresetUseVoidHeart(preset) ? preset.VoidHeartCost : preset.KingsoulCost, (GearPreset preset) => GetPresetUseVoidHeart(preset) ? DefaultGearPreset.VoidHeartCost : DefaultGearPreset.KingsoulCost);
		}

		private void UpdateGearSwitcherCharmCostHighlights()
		{
			if (gearSwitcherCharmCostHighlightEntries.Count == 0)
			{
				return;
			}
			GearPreset selectedPreset = GetSelectedPreset();
			foreach (CharmCostHighlightEntry gearSwitcherCharmCostHighlightEntry in gearSwitcherCharmCostHighlightEntries)
			{
				int num = Math.Max(0, Math.Min(99, gearSwitcherCharmCostHighlightEntry.CurrentCost(selectedPreset)));
				int num2 = Math.Max(0, Math.Min(99, gearSwitcherCharmCostHighlightEntry.DefaultCost(selectedPreset)));
				bool flag = num != num2;
				gearSwitcherCharmCostHighlightEntry.Glow.enabled = flag;
				gearSwitcherCharmCostHighlightEntry.ValueText.color = (flag ? GearSwitcherCharmCostChangedValueColor : Color.white);
			}
		}

		private void CreateGearSwitcherCloakIcon(Transform parent, float offsetX)
		{
			GameObject gameObject = new GameObject("CloakIcon");
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(offsetX, 0f);
			rectTransform.sizeDelta = new Vector2(44f, 44f);
			Image image = gameObject.AddComponent<Image>();
			image.preserveAspect = true;
			Outline outline = gameObject.AddComponent<Outline>();
			outline.effectColor = GearSwitcherSpellGlowColor;
			outline.effectDistance = new Vector2(2f, -2f);
			outline.enabled = false;
			Button button = gameObject.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.targetGraphic = image;
			button.onClick.AddListener(delegate
			{
				int selectedCloakLevel = GetSelectedCloakLevel();
				int num = selectedCloakLevel + 1;
				if (num > 2)
				{
					num = 0;
				}
				ApplyCloakLevel(num);
				GearSwitcher.ApplyAbilitiesImmediate(GetSelectedPreset());
				UpdateGearSwitcherCloakRowIcons();
			});
			gearSwitcherCloakRowIconRects.Add(rectTransform);
			gearSwitcherCloakIcon = image;
			gearSwitcherCloakGlow = outline;
		}

		private void CreateGearSwitcherMoveAbilityIcon(Transform parent, string name, float offsetX, string abilityKey, out Image icon, out Outline glow)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(offsetX, 0f);
			rectTransform.sizeDelta = new Vector2(44f, 44f);
			Image image = gameObject.AddComponent<Image>();
			image.preserveAspect = true;
			Outline outline = gameObject.AddComponent<Outline>();
			outline.effectColor = GearSwitcherBindingGlowColor;
			outline.effectDistance = new Vector2(2f, -2f);
			outline.enabled = false;
			Button button = gameObject.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.targetGraphic = image;
			button.onClick.AddListener(delegate
			{
				bool selectedMoveAbility = GetSelectedMoveAbility(abilityKey);
				SetSelectedMoveAbility(abilityKey, !selectedMoveAbility);
				GearSwitcher.ApplyAbilitiesImmediate(GetSelectedPreset());
				UpdateGearSwitcherCloakRowIcons();
			});
			gearSwitcherCloakRowIconRects.Add(rectTransform);
			icon = image;
			glow = outline;
		}

		private void CreateGearSwitcherBindingIcon(Transform parent, string name, float offsetX, string bindingKey, out Image icon, out Outline glow)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(offsetX, 0f);
			rectTransform.sizeDelta = new Vector2(44f, 44f);
			Image image = gameObject.AddComponent<Image>();
			image.preserveAspect = true;
			Outline outline = gameObject.AddComponent<Outline>();
			outline.effectColor = GearSwitcherSpellGlowColor;
			outline.effectDistance = new Vector2(2f, -2f);
			outline.enabled = false;
			Button button = gameObject.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.targetGraphic = image;
			button.onClick.AddListener(delegate
			{
				bool selectedBinding = GetSelectedBinding(bindingKey);
				SetSelectedBinding(bindingKey, !selectedBinding);
				GearSwitcher.ApplyBindingsImmediate(GetSelectedPreset());
				UpdateGearSwitcherBindingsRowIcons();
			});
			gearSwitcherBindingsRowIconRects.Add(rectTransform);
			icon = image;
			glow = outline;
		}

		private void CreateGearSwitcherDreamNailIcon(Transform parent, float offsetX)
		{
			GameObject gameObject = new GameObject("DreamNailIcon");
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(offsetX, 0f);
			rectTransform.sizeDelta = new Vector2(44f, 44f);
			Image image = gameObject.AddComponent<Image>();
			image.preserveAspect = true;
			Outline outline = gameObject.AddComponent<Outline>();
			outline.effectColor = GearSwitcherSpellGlowColor;
			outline.effectDistance = new Vector2(2f, -2f);
			outline.enabled = false;
			Button button = gameObject.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.targetGraphic = image;
			button.onClick.AddListener(delegate
			{
				int selectedDreamNailIconLevel = GetSelectedDreamNailIconLevel();
				int num = selectedDreamNailIconLevel + 1;
				if (num > 2)
				{
					num = 0;
				}
				ApplyDreamNailIconLevel(num);
				GearSwitcher.ApplyDreamNailImmediate(GetSelectedPreset());
				UpdateGearSwitcherCloakRowIcons();
			});
			gearSwitcherCloakRowIconRects.Add(rectTransform);
			gearSwitcherDreamNailIcon = image;
			gearSwitcherDreamNailGlow = outline;
		}

		private void CreateGearSwitcherDreamgateIcon(Transform parent, float offsetX)
		{
			GameObject gameObject = new GameObject("DreamgateIcon");
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(offsetX, 0f);
			rectTransform.sizeDelta = new Vector2(44f, 44f);
			Image image = gameObject.AddComponent<Image>();
			image.preserveAspect = true;
			Outline outline = gameObject.AddComponent<Outline>();
			outline.effectColor = GearSwitcherSpellGlowColor;
			outline.effectDistance = new Vector2(2f, -2f);
			outline.enabled = false;
			Button button = gameObject.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.targetGraphic = image;
			button.onClick.AddListener(delegate
			{
				bool selectedDreamgateEnabled = GetSelectedDreamgateEnabled();
				SetSelectedDreamgateEnabled(!selectedDreamgateEnabled);
				GearSwitcher.ApplyDreamNailImmediate(GetSelectedPreset());
				UpdateGearSwitcherCloakRowIcons();
			});
			gearSwitcherCloakRowIconRects.Add(rectTransform);
			gearSwitcherDreamgateIcon = image;
			gearSwitcherDreamgateGlow = outline;
		}

		private void CreateGearSwitcherNailArtIcon(Transform parent, string name, float offsetX, string artKey, Action updateAction, out Image icon, out Outline glow)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(offsetX, 0f);
			rectTransform.sizeDelta = new Vector2(44f, 44f);
			Image image = gameObject.AddComponent<Image>();
			image.preserveAspect = true;
			Outline outline = gameObject.AddComponent<Outline>();
			outline.effectColor = GearSwitcherSpellGlowColor;
			outline.effectDistance = new Vector2(2f, -2f);
			outline.enabled = false;
			Button button = gameObject.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.targetGraphic = image;
			button.onClick.AddListener(delegate
			{
				bool selectedNailArt = GetSelectedNailArt(artKey);
				SetSelectedNailArt(artKey, !selectedNailArt);
				GearSwitcher.ApplyNailArtsImmediate(GetSelectedPreset());
				updateAction();
			});
			icon = image;
			glow = outline;
		}

		private void CreateGearSwitcherSpellIcon(Transform parent, string name, float offsetX, string spellKey, Action updateAction, out Image icon, out Outline glow)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(offsetX, 0f);
			rectTransform.sizeDelta = new Vector2(44f, 44f);
			Image image = gameObject.AddComponent<Image>();
			image.preserveAspect = true;
			Outline outline = gameObject.AddComponent<Outline>();
			outline.effectColor = GearSwitcherSpellGlowColor;
			outline.effectDistance = new Vector2(2f, -2f);
			outline.enabled = false;
			Button button = gameObject.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.targetGraphic = image;
			button.onClick.AddListener(delegate
			{
				int num = Mathf.Clamp(GetSelectedSpellLevel(spellKey), 0, 2);
				int num2 = num + 1;
				if (num2 > 2)
				{
					num2 = 0;
				}
				SetSelectedSpellLevel(spellKey, num2);
				GearSwitcher.ApplySpellsImmediate(GetSelectedPreset());
				updateAction();
			});
			icon = image;
			glow = outline;
		}

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

		private void OnGearSwitcherPresetClicked()
		{
			SetGearSwitcherVisible(value: false);
			SetGearSwitcherPresetVisible(value: true);
		}

		private void OnGearSwitcherCharmPromptClicked()
		{
			SetGearSwitcherVisible(value: false);
			SetGearSwitcherCharmCostVisible(value: true);
		}

		private void OnGearSwitcherPresetBackClicked()
		{
			SetGearSwitcherPresetVisible(value: false);
			SetGearSwitcherVisible(value: true);
		}

		private void OnGearSwitcherPresetCreateClicked()
		{
			string value = GearSwitcher.CreateCustomPresetFromFullGear();
			if (!string.IsNullOrWhiteSpace(value))
			{
				gearSwitcherSelectedPreset = value;
				GearSwitcher.ApplyPreset(gearSwitcherSelectedPreset, allowQueue: true);
				RebuildGearSwitcherPresetOverlay();
				SetGearSwitcherPresetVisible(value: false);
				SetGearSwitcherVisible(value: true);
			}
		}

		private void OnGearSwitcherPresetSelected(int index)
		{
			string[] gearSwitcherPresetOptions = GetGearSwitcherPresetOptions();
			if (index >= 0 && index < gearSwitcherPresetOptions.Length)
			{
				gearSwitcherSelectedPreset = gearSwitcherPresetOptions[index];
				if (IsFullGearPreset(gearSwitcherSelectedPreset))
				{
					ResetFullGearBindings();
					GearSwitcher.RestoreAllBindingsImmediate();
				}
				GearSwitcher.ApplyPreset(gearSwitcherSelectedPreset, allowQueue: true);
				SetGearSwitcherPresetVisible(value: false);
				SetGearSwitcherVisible(value: true);
			}
		}

		private void RebuildGearSwitcherPresetOverlay()
		{
			bool active = gearSwitcherPresetVisible;
			DestroyRoot(ref gearSwitcherPresetRoot);
			BuildGearSwitcherPresetOverlayUi();
			if (gearSwitcherPresetRoot != null)
			{
				gearSwitcherPresetRoot.SetActive(active);
			}
		}

		private void ResetFullGearBindings()
		{
			if (GearSwitcherSettings.Presets == null || GearSwitcherSettings.Presets.Count == 0)
			{
				GearSwitcherSettings.Presets = GearPresetDefaults.CreateDefaults();
			}
			if (GearSwitcherSettings.Presets.TryGetValue("FullGear", out GearPreset value))
			{
				value.HasAllBindings = false;
				GearPreset gearPreset = value;
				if (gearPreset.Bindings == null)
				{
					Dictionary<string, bool> dictionary = (gearPreset.Bindings = new Dictionary<string, bool>());
				}
				value.Bindings["CharmsBinding"] = false;
				value.Bindings["NailBinding"] = false;
				value.Bindings["ShellBinding"] = false;
				value.Bindings["SoulBinding"] = false;
				GodhomeQoL.SaveGlobalSettingsSafe();
			}
		}

		private void HandleGearSwitcherPresetRename()
		{
			if (!(gearSwitcherPresetRenameField == null))
			{
				if (IsRenameConfirmPressed())
				{
					gearSwitcherPresetRenameSubmitPending = true;
					gearSwitcherPresetRenameField.DeactivateInputField();
				}
				else if (Input.GetKeyDown(KeyCode.Escape))
				{
					gearSwitcherPresetRenameCancelled = true;
					CancelGearSwitcherPresetRename();
				}
			}
		}

		private void StartGearSwitcherPresetRename(Text label, string presetName)
		{
			if (gearSwitcherPresetRenameField != null)
			{
				CommitGearSwitcherPresetRename(gearSwitcherPresetRenameField.text);
			}
			SetGearSwitcherPresetDeleteVisible(value: false);
			gearSwitcherPresetRenameCancelled = false;
			gearSwitcherPresetRenameSubmitPending = false;
			gearSwitcherPresetRenameLabel = label;
			gearSwitcherPresetRenameOriginalLabel = label.text;
			gearSwitcherPresetRenameTargetName = presetName;
			UnityEngine.UI.InputField inputField = CreateGearSwitcherPresetRenameField(label.transform.parent, label.text, label.fontSize);
			inputField.onEndEdit.AddListener(OnGearSwitcherPresetRenameEndEdit);
			gearSwitcherPresetRenameField = inputField;
			label.gameObject.SetActive(value: false);
			inputField.ActivateInputField();
			inputField.Select();
			inputField.MoveTextEnd(shift: false);
		}

		private void OnGearSwitcherPresetRenameEndEdit(string value)
		{
			if (gearSwitcherPresetRenameCancelled)
			{
				gearSwitcherPresetRenameCancelled = false;
			}
			else if (!gearSwitcherPresetRenameSubmitPending)
			{
				if (gearSwitcherPresetRenameField != null)
				{
					gearSwitcherPresetRenameField.ActivateInputField();
					gearSwitcherPresetRenameField.Select();
				}
			}
			else
			{
				gearSwitcherPresetRenameSubmitPending = false;
				CommitGearSwitcherPresetRename(value);
			}
		}

		private static bool IsRenameConfirmPressed()
		{
			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				return true;
			}
			try
			{
				HeroActions? heroActions = InputHandler.Instance?.inputActions;
				if (heroActions != null && heroActions.menuSubmit.WasPressed)
				{
					return true;
				}
			}
			catch
			{
			}
			return false;
		}

		private void CommitGearSwitcherPresetRename(string value)
		{
			if (gearSwitcherPresetRenameLabel == null)
			{
				return;
			}
			string text = value.Trim();
			if (text.Length > 40)
			{
				text = text.Substring(0, 40);
			}
			string text2 = gearSwitcherPresetRenameTargetName ?? string.Empty;
			string text3;
			if (IsFullGearPreset(text2))
			{
				text3 = NormalizeFullGearPresetName(text);
				SetFullGearDisplayName(text3);
			}
			else
			{
				text3 = NormalizeCustomPresetName(text2, text);
				if (!string.Equals(text3, text2, StringComparison.OrdinalIgnoreCase))
				{
					if (GearSwitcher.RenameCustomPreset(text2, text3))
					{
						if (string.Equals(gearSwitcherSelectedPreset, text2, StringComparison.OrdinalIgnoreCase))
						{
							gearSwitcherSelectedPreset = text3;
						}
					}
					else
					{
						text3 = text2;
					}
				}
			}
			gearSwitcherPresetRenameLabel.text = GetGearSwitcherPresetDisplayName(text3);
			gearSwitcherPresetRenameLabel.gameObject.SetActive(value: true);
			if (gearSwitcherPresetRenameField != null)
			{
				UnityEngine.Object.Destroy(gearSwitcherPresetRenameField.gameObject);
			}
			gearSwitcherPresetRenameField = null;
			gearSwitcherPresetRenameLabel = null;
			gearSwitcherPresetRenameOriginalLabel = null;
			gearSwitcherPresetRenameTargetName = null;
			RebuildGearSwitcherPresetOverlay();
			SetGearSwitcherPresetVisible(value: true);
			RefreshGearSwitcherUi();
		}

		private void CancelGearSwitcherPresetRename()
		{
			if (!(gearSwitcherPresetRenameLabel == null))
			{
				if (gearSwitcherPresetRenameOriginalLabel != null)
				{
					gearSwitcherPresetRenameLabel.text = gearSwitcherPresetRenameOriginalLabel;
				}
				gearSwitcherPresetRenameLabel.gameObject.SetActive(value: true);
				if (gearSwitcherPresetRenameField != null)
				{
					UnityEngine.Object.Destroy(gearSwitcherPresetRenameField.gameObject);
				}
				gearSwitcherPresetRenameField = null;
				gearSwitcherPresetRenameLabel = null;
				gearSwitcherPresetRenameOriginalLabel = null;
				gearSwitcherPresetRenameTargetName = null;
			}
		}

		private void StartGearSwitcherPresetDelete(string presetName)
		{
			if (!IsBuiltinPresetName(presetName))
			{
				CancelGearSwitcherPresetRename();
				gearSwitcherPresetDeleteTargetName = presetName;
				if (gearSwitcherPresetDeleteLabel != null)
				{
					string gearSwitcherPresetDisplayName = GetGearSwitcherPresetDisplayName(presetName);
					gearSwitcherPresetDeleteLabel.text = "Delete \"" + gearSwitcherPresetDisplayName + "\"?";
				}
				SetGearSwitcherPresetDeleteVisible(value: true);
			}
		}

		private void StartGearSwitcherPresetDeleteByIndex(int index)
		{
			string[] gearSwitcherPresetOptions = GetGearSwitcherPresetOptions();
			if (index >= 0 && index < gearSwitcherPresetOptions.Length)
			{
				StartGearSwitcherPresetDelete(gearSwitcherPresetOptions[index]);
			}
		}

		private void OnGearSwitcherPresetDeleteYes()
		{
			string text = gearSwitcherPresetDeleteTargetName ?? string.Empty;
			SetGearSwitcherPresetDeleteVisible(value: false);
			if (!string.IsNullOrWhiteSpace(text) && GearSwitcher.DeleteCustomPreset(text))
			{
				if (string.Equals(gearSwitcherSelectedPreset, text, StringComparison.OrdinalIgnoreCase))
				{
					gearSwitcherSelectedPreset = "FullGear";
					GearSwitcher.ApplyPreset(gearSwitcherSelectedPreset, allowQueue: true);
				}
				RebuildGearSwitcherPresetOverlay();
				SetGearSwitcherPresetVisible(value: true);
			}
		}

		private void OnGearSwitcherPresetDeleteNo()
		{
			SetGearSwitcherPresetDeleteVisible(value: false);
		}

		private string NormalizeFullGearPresetName(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return "FullGear";
			}
			if (string.Equals(value, "FullGear", StringComparison.OrdinalIgnoreCase))
			{
				return "FullGear";
			}
			string fullGearDisplayName = GetFullGearDisplayName();
			if (string.Equals(value, fullGearDisplayName, StringComparison.OrdinalIgnoreCase))
			{
				return "FullGear";
			}
			if (IsGearSwitcherPresetNameTaken(value))
			{
				return "FullGear";
			}
			return value;
		}

		private UnityEngine.UI.InputField CreateGearSwitcherPresetRenameField(Transform parent, string text, int fontSize)
		{
			GameObject gameObject = new GameObject("PresetRenameField");
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(1f, 1f, 1f, 0.08f);
			UnityEngine.UI.InputField inputField = gameObject.AddComponent<UnityEngine.UI.InputField>();
			inputField.lineType = UnityEngine.UI.InputField.LineType.SingleLine;
			inputField.contentType = UnityEngine.UI.InputField.ContentType.Standard;
			inputField.caretColor = Color.white;
			inputField.selectionColor = new Color(1f, 1f, 1f, 0.25f);
			inputField.targetGraphic = image;
			inputField.text = text;
			inputField.characterLimit = 40;
			Text text2 = CreateText(gameObject.transform, "Text", text, fontSize, TextAnchor.MiddleLeft);
			RectTransform rectTransform2 = text2.rectTransform;
			rectTransform2.anchorMin = new Vector2(0f, 0f);
			rectTransform2.anchorMax = new Vector2(1f, 1f);
			rectTransform2.offsetMin = new Vector2(20f, 0f);
			rectTransform2.offsetMax = new Vector2(-40f, 0f);
			inputField.textComponent = text2;
			return inputField;
		}

		private void OnGearSwitcherCharmCostBackClicked()
		{
			SetGearSwitcherCharmCostVisible(value: false);
			SetGearSwitcherVisible(value: true);
		}

		private void OnGearSwitcherCharmCostResetDefaultsClicked()
		{
			GearPreset selectedPreset = GetSelectedPreset();
			selectedPreset.WaywardCompassCost = DefaultGearPreset.WaywardCompassCost;
			selectedPreset.GatheringSwarmCost = DefaultGearPreset.GatheringSwarmCost;
			selectedPreset.StalwartShellCost = DefaultGearPreset.StalwartShellCost;
			selectedPreset.SoulCatcherCost = DefaultGearPreset.SoulCatcherCost;
			selectedPreset.ShamanStoneCost = DefaultGearPreset.ShamanStoneCost;
			selectedPreset.SoulEaterCost = DefaultGearPreset.SoulEaterCost;
			selectedPreset.DashmasterCost = DefaultGearPreset.DashmasterCost;
			selectedPreset.SprintmasterCost = DefaultGearPreset.SprintmasterCost;
			selectedPreset.GrubsongCost = DefaultGearPreset.GrubsongCost;
			selectedPreset.GrubberflysElegyCost = DefaultGearPreset.GrubberflysElegyCost;
			selectedPreset.UnbreakableHeartCost = DefaultGearPreset.UnbreakableHeartCost;
			selectedPreset.UnbreakableGreedCost = DefaultGearPreset.UnbreakableGreedCost;
			selectedPreset.UnbreakableStrengthCost = DefaultGearPreset.UnbreakableStrengthCost;
			selectedPreset.SpellTwisterCost = DefaultGearPreset.SpellTwisterCost;
			selectedPreset.SteadyBodyCost = DefaultGearPreset.SteadyBodyCost;
			selectedPreset.HeavyBlowCost = DefaultGearPreset.HeavyBlowCost;
			selectedPreset.QuickSlashCost = DefaultGearPreset.QuickSlashCost;
			selectedPreset.LongnailCost = DefaultGearPreset.LongnailCost;
			selectedPreset.MarkOfPrideCost = DefaultGearPreset.MarkOfPrideCost;
			selectedPreset.FuryOfTheFallenCost = DefaultGearPreset.FuryOfTheFallenCost;
			selectedPreset.ThornsOfAgonyCost = DefaultGearPreset.ThornsOfAgonyCost;
			selectedPreset.BaldurShellCost = DefaultGearPreset.BaldurShellCost;
			selectedPreset.FlukenestCost = DefaultGearPreset.FlukenestCost;
			selectedPreset.DefendersCrestCost = DefaultGearPreset.DefendersCrestCost;
			selectedPreset.GlowingWombCost = DefaultGearPreset.GlowingWombCost;
			selectedPreset.QuickFocusCost = DefaultGearPreset.QuickFocusCost;
			selectedPreset.DeepFocusCost = DefaultGearPreset.DeepFocusCost;
			selectedPreset.LifebloodHeartCost = DefaultGearPreset.LifebloodHeartCost;
			selectedPreset.LifebloodCoreCost = DefaultGearPreset.LifebloodCoreCost;
			selectedPreset.JonisBlessingCost = DefaultGearPreset.JonisBlessingCost;
			selectedPreset.HivebloodCost = DefaultGearPreset.HivebloodCost;
			selectedPreset.SporeShroomCost = DefaultGearPreset.SporeShroomCost;
			selectedPreset.SharpShadowCost = DefaultGearPreset.SharpShadowCost;
			selectedPreset.ShapeOfUnnCost = DefaultGearPreset.ShapeOfUnnCost;
			selectedPreset.NailmastersGloryCost = DefaultGearPreset.NailmastersGloryCost;
			selectedPreset.WeaversongCost = DefaultGearPreset.WeaversongCost;
			selectedPreset.DreamWielderCost = DefaultGearPreset.DreamWielderCost;
			selectedPreset.DreamshieldCost = DefaultGearPreset.DreamshieldCost;
			selectedPreset.CarefreeMelodyCost = DefaultGearPreset.CarefreeMelodyCost;
			selectedPreset.GrimmchildCost = DefaultGearPreset.GrimmchildCost;
			selectedPreset.VoidHeartCost = DefaultGearPreset.VoidHeartCost;
			selectedPreset.KingsoulCost = DefaultGearPreset.KingsoulCost;
			selectedPreset.CarefreeMelodyCostInitialized = true;
			selectedPreset.KingsoulCostInitialized = true;
			GodhomeQoL.SaveGlobalSettingsSafe();
			GearSwitcher.ApplyCharmCostsImmediate(selectedPreset);
			RefreshGearSwitcherCharmCostUi();
		}

		private void OnGearSwitcherCharmCostAheClicked()
		{
			GearPreset selectedPreset = GetSelectedPreset();
			selectedPreset.WaywardCompassCost = 0;
			selectedPreset.GatheringSwarmCost = 0;
			selectedPreset.StalwartShellCost = 1;
			selectedPreset.SoulCatcherCost = 1;
			selectedPreset.ShamanStoneCost = 3;
			selectedPreset.SoulEaterCost = 3;
			selectedPreset.DashmasterCost = 1;
			selectedPreset.SprintmasterCost = 0;
			selectedPreset.GrubsongCost = 1;
			selectedPreset.GrubberflysElegyCost = 2;
			selectedPreset.UnbreakableHeartCost = 2;
			selectedPreset.UnbreakableGreedCost = 0;
			selectedPreset.UnbreakableStrengthCost = 3;
			selectedPreset.SpellTwisterCost = 2;
			selectedPreset.SteadyBodyCost = 1;
			selectedPreset.HeavyBlowCost = 0;
			selectedPreset.QuickSlashCost = 2;
			selectedPreset.LongnailCost = 1;
			selectedPreset.MarkOfPrideCost = 2;
			selectedPreset.FuryOfTheFallenCost = 1;
			selectedPreset.ThornsOfAgonyCost = 1;
			selectedPreset.BaldurShellCost = 1;
			selectedPreset.FlukenestCost = 2;
			selectedPreset.DefendersCrestCost = 1;
			selectedPreset.GlowingWombCost = 0;
			selectedPreset.QuickFocusCost = 2;
			selectedPreset.DeepFocusCost = 3;
			selectedPreset.LifebloodHeartCost = 1;
			selectedPreset.LifebloodCoreCost = 2;
			selectedPreset.JonisBlessingCost = 0;
			selectedPreset.HivebloodCost = 2;
			selectedPreset.SporeShroomCost = 1;
			selectedPreset.SharpShadowCost = 1;
			selectedPreset.ShapeOfUnnCost = 2;
			selectedPreset.NailmastersGloryCost = 1;
			selectedPreset.WeaversongCost = 1;
			selectedPreset.DreamWielderCost = 1;
			selectedPreset.DreamshieldCost = 1;
			selectedPreset.GrimmchildCost = 0;
			selectedPreset.CarefreeMelodyCost = 2;
			selectedPreset.KingsoulCost = 2;
			selectedPreset.VoidHeartCost = 0;
			selectedPreset.CarefreeMelodyCostInitialized = true;
			selectedPreset.KingsoulCostInitialized = true;
			GodhomeQoL.SaveGlobalSettingsSafe();
			GearSwitcher.ApplyCharmCostsImmediate(selectedPreset);
			RefreshGearSwitcherCharmCostUi();
			MarkGearSwitcherPresetEdited();
		}

		private void OnGearSwitcherResetDefaultsClicked()
		{
			SetGearSwitcherResetConfirmVisible(value: true);
		}

		private void OnGearSwitcherResetConfirmYes()
		{
			GearSwitcher.ResetDefaults();
			gearSwitcherSelectedPreset = "FullGear";
			SetGearSwitcherEnabled(value: false);
			GodhomeQoL.SaveGlobalSettingsSafe();
			RefreshGearSwitcherUi();
			SetGearSwitcherResetConfirmVisible(value: false);
		}

		private void OnGearSwitcherResetConfirmNo()
		{
			SetGearSwitcherResetConfirmVisible(value: false);
		}

		private bool GetGearSwitcherEnabled()
		{
			return GearSwitcher.IsGloballyEnabled;
		}

		private void SetGearSwitcherEnabled(bool value)
		{
			if (GearSwitcher.IsGloballyEnabled != value)
			{
				GearSwitcher.IsGloballyEnabled = value;
				GodhomeQoL.SaveGlobalSettingsSafe();
				if (value)
				{
					GearSwitcher.ApplyPreset(gearSwitcherSelectedPreset, allowQueue: true);
				}
				else
				{
					GearSwitcher.ClearPendingApplies();
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
			UpdateGearSwitcherInteractivity();
		}

		private void RefreshGearSwitcherCharmCostUi()
		{
			UpdateGearSwitcherWaywardCostValue();
			UpdateGearSwitcherGatheringCostValue();
			UpdateGearSwitcherStalwartShellCostValue();
			UpdateGearSwitcherSoulCatcherCostValue();
			UpdateGearSwitcherShamanStoneCostValue();
			UpdateGearSwitcherSoulEaterCostValue();
			UpdateGearSwitcherDashmasterCostValue();
			UpdateGearSwitcherSprintmasterCostValue();
			UpdateGearSwitcherGrubsongCostValue();
			UpdateGearSwitcherGrubberflysElegyCostValue();
			UpdateGearSwitcherUnbreakableHeartCostValue();
			UpdateGearSwitcherUnbreakableGreedCostValue();
			UpdateGearSwitcherUnbreakableStrengthCostValue();
			UpdateGearSwitcherSpellTwisterCostValue();
			UpdateGearSwitcherSteadyBodyCostValue();
			UpdateGearSwitcherHeavyBlowCostValue();
			UpdateGearSwitcherQuickSlashCostValue();
			UpdateGearSwitcherLongnailCostValue();
			UpdateGearSwitcherMarkOfPrideCostValue();
			UpdateGearSwitcherFuryOfTheFallenCostValue();
			UpdateGearSwitcherThornsOfAgonyCostValue();
			UpdateGearSwitcherBaldurShellCostValue();
			UpdateGearSwitcherFlukenestCostValue();
			UpdateGearSwitcherDefendersCrestCostValue();
			UpdateGearSwitcherGlowingWombCostValue();
			UpdateGearSwitcherQuickFocusCostValue();
			UpdateGearSwitcherDeepFocusCostValue();
			UpdateGearSwitcherLifebloodHeartCostValue();
			UpdateGearSwitcherLifebloodCoreCostValue();
			UpdateGearSwitcherJonisBlessingCostValue();
			UpdateGearSwitcherHivebloodCostValue();
			UpdateGearSwitcherSporeShroomCostValue();
			UpdateGearSwitcherSharpShadowCostValue();
			UpdateGearSwitcherShapeOfUnnCostValue();
			UpdateGearSwitcherNailmastersGloryCostValue();
			UpdateGearSwitcherWeaversongCostValue();
			UpdateGearSwitcherDreamWielderCostValue();
			UpdateGearSwitcherDreamshieldCostValue();
			UpdateGearSwitcherCarefreeMelodyCostValue();
			UpdateGearSwitcherVoidHeartCostValue();
			UpdateGearSwitcherVoidHeartIcon();
			UpdateGearSwitcherCarefreeIcon();
			UpdateGearSwitcherCharmCostHighlights();
		}

		private void RefreshGearSwitcherPresetSelectUi()
		{
			string[] gearSwitcherPresetOptions = GetGearSwitcherPresetOptions();
			int gearSwitcherPresetIndex = GetGearSwitcherPresetIndex(gearSwitcherPresetOptions);
			foreach (GearSwitcherPresetEntry gearSwitcherPresetEntry in gearSwitcherPresetEntries)
			{
				string presetName = ((gearSwitcherPresetEntry.Index >= 0 && gearSwitcherPresetEntry.Index < gearSwitcherPresetOptions.Length) ? gearSwitcherPresetOptions[gearSwitcherPresetEntry.Index] : string.Empty);
				gearSwitcherPresetEntry.Label.text = GetGearSwitcherPresetDisplayName(presetName);
				gearSwitcherPresetEntry.Highlight.SetManualActive(gearSwitcherPresetEntry.Index == gearSwitcherPresetIndex);
			}
		}

		private void RefreshGearSwitcherPresetFields()
		{
			GearPreset selectedPreset = GetSelectedPreset();
			UpdateIntInputValue(gearSwitcherNailDamageField, selectedPreset.NailDamage);
			UpdateGearSwitcherBaseNailDamage();
			UpdateIntInputValue(gearSwitcherCharmSlotsField, selectedPreset.CharmSlots);
			UpdateIntInputValue(gearSwitcherMainSoulGainField, selectedPreset.MainSoulGain);
			UpdateIntInputValue(gearSwitcherReserveSoulGainField, selectedPreset.ReserveSoulGain);
			UpdateGearSwitcherFireballIcon();
			UpdateGearSwitcherQuakeIcon();
			UpdateGearSwitcherScreamIcon();
			UpdateGearSwitcherCycloneIcon();
			UpdateGearSwitcherDashSlashIcon();
			UpdateGearSwitcherGreatSlashIcon();
			UpdateGearSwitcherCloakRowIcons();
			UpdateGearSwitcherBindingsRowIcons();
			UpdateGearSwitcherHpMaskIcons();
			UpdateGearSwitcherSoulVesselIcons();
			UpdateGearSwitcherNaillessIcon();
			UpdateGearSwitcherOvercharmedIcon();
			UpdateGearSwitcherCharmPromptIcon();
			RefreshGearSwitcherCharmCostUi();
			UpdateGearSwitcherSelectedPresetColor(gearSwitcherSelectedPreset);
		}

		private void UpdateGearSwitcherBaseNailDamage()
		{
			if (gearSwitcherBaseNailDamageValue != null)
			{
				gearSwitcherBaseNailDamageValue.text = GetGearSwitcherBaseNailDamageDisplay();
			}
		}

		private static string GetGearSwitcherBaseNailDamageDisplay()
		{
			PlayerData? data = PlayerData.instance;
			int flatDamage = data?.nailDamage ?? 0;

			PlayMakerFSM? slashFsm = GetKnightSlashFsm();
			if (slashFsm == null)
			{
				return flatDamage.ToString();
			}

			int damageDealt = flatDamage;
			float multiplier = 1f;

			FsmInt? damageVar = slashFsm.FsmVariables.GetFsmInt("damageDealt");
			if (damageVar != null)
			{
				damageDealt = damageVar.Value;
			}

			FsmFloat? multiplierVar = slashFsm.FsmVariables.GetFsmFloat("Multiplier");
			if (multiplierVar != null)
			{
				multiplier = multiplierVar.Value;
			}

			return $"{damageDealt} (Flat {flatDamage}, x{multiplier:0.##})";
		}

		private static PlayMakerFSM? GetKnightSlashFsm()
		{
			HeroController? hero = HeroController.instance;
			Transform? slash = hero?.transform.Find("Attacks/Slash");
			return slash != null ? slash.GetComponent<PlayMakerFSM>() : null;
		}

		private void UpdateGearSwitcherFireballIcon()
		{
			if (!(gearSwitcherFireballIcon == null))
			{
				if (vengefulSpiritSprite is null)
				{
					vengefulSpiritSprite = LoadCollectorIconSprite("Vengeful Spirit.png", "VengefulSpirit");
				}
				if (shadeSoulSprite is null)
				{
					shadeSoulSprite = LoadCollectorIconSprite("Shade Soul.png", "ShadeSoul");
				}
				int num = Mathf.Clamp(GetSelectedSpellLevel("fireballLevel"), 0, 2);
				Sprite? sprite = ((num >= 2) ? shadeSoulSprite : vengefulSpiritSprite);
				if (sprite != null)
				{
					gearSwitcherFireballIcon.sprite = sprite;
					gearSwitcherFireballIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				}
				else
				{
					gearSwitcherFireballIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
				bool flag = num > 0;
				gearSwitcherFireballIcon.color = (flag ? GearSwitcherSpellActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherFireballGlow != null)
				{
					gearSwitcherFireballGlow.enabled = flag;
				}
			}
		}

		private void UpdateGearSwitcherQuakeIcon()
		{
			if (!(gearSwitcherQuakeIcon == null))
			{
				if (desolateDiveSprite is null)
				{
					desolateDiveSprite = LoadCollectorIconSprite("Desolate Dive.png", "DesolateDive");
				}
				if (descendingDarkSprite is null)
				{
					descendingDarkSprite = LoadCollectorIconSprite("Descending Dark.png", "DescendingDark");
				}
				int num = Mathf.Clamp(GetSelectedSpellLevel("quakeLevel"), 0, 2);
				Sprite? sprite = ((num >= 2) ? descendingDarkSprite : desolateDiveSprite);
				if (sprite != null)
				{
					gearSwitcherQuakeIcon.sprite = sprite;
					gearSwitcherQuakeIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				}
				else
				{
					gearSwitcherQuakeIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
				bool flag = num > 0;
				gearSwitcherQuakeIcon.color = (flag ? GearSwitcherSpellActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherQuakeGlow != null)
				{
					gearSwitcherQuakeGlow.enabled = flag;
				}
			}
		}

		private void UpdateGearSwitcherScreamIcon()
		{
			if (!(gearSwitcherScreamIcon == null))
			{
				if (howlingWraithsSprite is null)
				{
					howlingWraithsSprite = LoadCollectorIconSprite("Howling Wraiths.png", "HowlingWraiths");
				}
				if (abyssShriekSprite is null)
				{
					abyssShriekSprite = LoadCollectorIconSprite("Abyss Shriek.png", "AbyssShriek");
				}
				int num = Mathf.Clamp(GetSelectedSpellLevel("screamLevel"), 0, 2);
				Sprite? sprite = ((num >= 2) ? abyssShriekSprite : howlingWraithsSprite);
				if (sprite != null)
				{
					gearSwitcherScreamIcon.sprite = sprite;
					gearSwitcherScreamIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				}
				else
				{
					gearSwitcherScreamIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
				bool flag = num > 0;
				gearSwitcherScreamIcon.color = (flag ? GearSwitcherSpellActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherScreamGlow != null)
				{
					gearSwitcherScreamGlow.enabled = flag;
				}
			}
		}

		private void UpdateGearSwitcherCycloneIcon()
		{
			if (!(gearSwitcherCycloneIcon == null))
			{
				if (cycloneSlashSprite is null)
				{
					cycloneSlashSprite = LoadCollectorIconSprite("Cyclone Slash.png", "CycloneSlash");
				}
				Sprite? sprite = cycloneSlashSprite;
				if (sprite != null)
				{
					gearSwitcherCycloneIcon.sprite = sprite;
					gearSwitcherCycloneIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				}
				else
				{
					gearSwitcherCycloneIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
				bool selectedNailArt = GetSelectedNailArt("hasCyclone");
				gearSwitcherCycloneIcon.color = (selectedNailArt ? GearSwitcherSpellActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherCycloneGlow != null)
				{
					gearSwitcherCycloneGlow.enabled = selectedNailArt;
				}
			}
		}

		private void UpdateGearSwitcherDashSlashIcon()
		{
			if (!(gearSwitcherDashSlashIcon == null))
			{
				if (dashSlashSprite is null)
				{
					dashSlashSprite = LoadCollectorIconSprite("Great Slash.png", "DashSlash");
				}
				Sprite? sprite = dashSlashSprite;
				if (sprite != null)
				{
					gearSwitcherDashSlashIcon.sprite = sprite;
					gearSwitcherDashSlashIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				}
				else
				{
					gearSwitcherDashSlashIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
				bool selectedNailArt = GetSelectedNailArt("hasDashSlash");
				gearSwitcherDashSlashIcon.color = (selectedNailArt ? GearSwitcherSpellActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherDashSlashGlow != null)
				{
					gearSwitcherDashSlashGlow.enabled = selectedNailArt;
				}
			}
		}

		private void UpdateGearSwitcherGreatSlashIcon()
		{
			if (!(gearSwitcherGreatSlashIcon == null))
			{
				if (greatSlashSprite is null)
				{
					greatSlashSprite = LoadCollectorIconSprite("Dash Slash.png", "GreatSlash");
				}
				Sprite? sprite = greatSlashSprite;
				if (sprite != null)
				{
					gearSwitcherGreatSlashIcon.sprite = sprite;
					gearSwitcherGreatSlashIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				}
				else
				{
					gearSwitcherGreatSlashIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
				bool selectedNailArt = GetSelectedNailArt("hasUpwardSlash");
				gearSwitcherGreatSlashIcon.color = (selectedNailArt ? GearSwitcherSpellActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherGreatSlashGlow != null)
				{
					gearSwitcherGreatSlashGlow.enabled = selectedNailArt;
				}
			}
		}

		private void UpdateGearSwitcherCloakRowIcons()
		{
			UpdateGearSwitcherCloakIcon();
			UpdateGearSwitcherMantisClawIcon();
			UpdateGearSwitcherMonarchWingsIcon();
			UpdateGearSwitcherCrystalHeartIcon();
			UpdateGearSwitcherIsmasTearIcon();
			UpdateGearSwitcherDreamNailIcon();
			UpdateGearSwitcherDreamgateIcon();
			UpdateGearSwitcherCloakRowScale();
		}

		private void UpdateGearSwitcherCloakIcon()
		{
			if (!(gearSwitcherCloakIcon == null))
			{
				if (mothwingCloakSprite is null)
				{
					mothwingCloakSprite = LoadCollectorIconSprite("Mothwing Cloak.png", "MothwingCloak");
				}
				if (shadeCloakSprite is null)
				{
					shadeCloakSprite = LoadCollectorIconSprite("Shade Cloak.png", "ShadeCloak");
				}
				int selectedCloakLevel = GetSelectedCloakLevel();
				Sprite? sprite = ((selectedCloakLevel >= 2) ? shadeCloakSprite : mothwingCloakSprite);
				if (sprite != null)
				{
					gearSwitcherCloakIcon.sprite = sprite;
					gearSwitcherCloakIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				}
				else
				{
					gearSwitcherCloakIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
				bool flag = selectedCloakLevel > 0;
				gearSwitcherCloakIcon.color = (flag ? GearSwitcherSpellActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherCloakGlow != null)
				{
					gearSwitcherCloakGlow.enabled = flag;
				}
			}
		}

		private void UpdateGearSwitcherMantisClawIcon()
		{
			if (!(gearSwitcherMantisClawIcon == null))
			{
				if (mantisClawSprite is null)
				{
					mantisClawSprite = LoadCollectorIconSprite("Mantis Claw.png", "MantisClaw");
				}
				Sprite? sprite = mantisClawSprite;
				if (sprite != null)
				{
					gearSwitcherMantisClawIcon.sprite = sprite;
					gearSwitcherMantisClawIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				}
				else
				{
					gearSwitcherMantisClawIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
				bool selectedMoveAbility = GetSelectedMoveAbility("Walljump");
				gearSwitcherMantisClawIcon.color = (selectedMoveAbility ? GearSwitcherSpellActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherMantisClawGlow != null)
				{
					gearSwitcherMantisClawGlow.enabled = selectedMoveAbility;
				}
			}
		}

		private void UpdateGearSwitcherMonarchWingsIcon()
		{
			if (!(gearSwitcherMonarchWingsIcon == null))
			{
				if (monarchWingsSprite is null)
				{
					monarchWingsSprite = LoadCollectorIconSprite("Monarch Wings.png", "MonarchWings");
				}
				Sprite? sprite = monarchWingsSprite;
				if (sprite != null)
				{
					gearSwitcherMonarchWingsIcon.sprite = sprite;
					gearSwitcherMonarchWingsIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				}
				else
				{
					gearSwitcherMonarchWingsIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
				bool selectedMoveAbility = GetSelectedMoveAbility("DoubleJump");
				gearSwitcherMonarchWingsIcon.color = (selectedMoveAbility ? GearSwitcherSpellActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherMonarchWingsGlow != null)
				{
					gearSwitcherMonarchWingsGlow.enabled = selectedMoveAbility;
				}
			}
		}

		private void UpdateGearSwitcherCrystalHeartIcon()
		{
			if (!(gearSwitcherCrystalHeartIcon == null))
			{
				if (crystalHeartSprite is null)
				{
					crystalHeartSprite = LoadCollectorIconSprite("Crystal Heart.png", "CrystalHeart");
				}
				Sprite? sprite = crystalHeartSprite;
				if (sprite != null)
				{
					gearSwitcherCrystalHeartIcon.sprite = sprite;
					gearSwitcherCrystalHeartIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				}
				else
				{
					gearSwitcherCrystalHeartIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
				bool selectedMoveAbility = GetSelectedMoveAbility("SuperDash");
				gearSwitcherCrystalHeartIcon.color = (selectedMoveAbility ? GearSwitcherSpellActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherCrystalHeartGlow != null)
				{
					gearSwitcherCrystalHeartGlow.enabled = selectedMoveAbility;
				}
			}
		}

		private void UpdateGearSwitcherIsmasTearIcon()
		{
			if (!(gearSwitcherIsmasTearIcon == null))
			{
				if (ismasTearSprite is null)
				{
					ismasTearSprite = LoadCollectorIconSprite("Isma's Tear.png", "IsmasTear");
				}
				Sprite? sprite = ismasTearSprite;
				if (sprite != null)
				{
					gearSwitcherIsmasTearIcon.sprite = sprite;
					gearSwitcherIsmasTearIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				}
				else
				{
					gearSwitcherIsmasTearIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
				bool selectedMoveAbility = GetSelectedMoveAbility("AcidArmour");
				gearSwitcherIsmasTearIcon.color = (selectedMoveAbility ? GearSwitcherSpellActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherIsmasTearGlow != null)
				{
					gearSwitcherIsmasTearGlow.enabled = selectedMoveAbility;
				}
			}
		}

		private void UpdateGearSwitcherDreamNailIcon()
		{
			if (!(gearSwitcherDreamNailIcon == null))
			{
				if (dreamNailSprite is null)
				{
					dreamNailSprite = LoadCollectorIconSprite("Dream Nail.png", "DreamNail");
				}
				if (awokenDreamNailSprite is null)
				{
					awokenDreamNailSprite = LoadCollectorIconSprite("Awoken Dream Nail.png", "AwokenDreamNail");
				}
				int selectedDreamNailIconLevel = GetSelectedDreamNailIconLevel();
				Sprite? sprite = ((selectedDreamNailIconLevel >= 2) ? awokenDreamNailSprite : dreamNailSprite);
				if (sprite != null)
				{
					gearSwitcherDreamNailIcon.sprite = sprite;
					gearSwitcherDreamNailIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				}
				else
				{
					gearSwitcherDreamNailIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
				bool flag = selectedDreamNailIconLevel > 0;
				gearSwitcherDreamNailIcon.color = (flag ? GearSwitcherSpellActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherDreamNailGlow != null)
				{
					gearSwitcherDreamNailGlow.enabled = flag;
				}
			}
		}

		private void UpdateGearSwitcherDreamgateIcon()
		{
			if (!(gearSwitcherDreamgateIcon == null))
			{
				if (dreamgateSprite is null)
				{
					dreamgateSprite = LoadCollectorIconSprite("Dreamgate.png", "Dreamgate");
				}
				Sprite? sprite = dreamgateSprite;
				if (sprite != null)
				{
					gearSwitcherDreamgateIcon.sprite = sprite;
					gearSwitcherDreamgateIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				}
				else
				{
					gearSwitcherDreamgateIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
				bool selectedDreamgateEnabled = GetSelectedDreamgateEnabled();
				gearSwitcherDreamgateIcon.color = (selectedDreamgateEnabled ? GearSwitcherSpellActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherDreamgateGlow != null)
				{
					gearSwitcherDreamgateGlow.enabled = selectedDreamgateEnabled;
				}
			}
		}

		private void UpdateGearSwitcherBindingsRowIcons()
		{
			UpdateGearSwitcherBindingNailIcon();
			UpdateGearSwitcherBindingShellIcon();
			UpdateGearSwitcherBindingCharmsIcon();
			UpdateGearSwitcherBindingSoulIcon();
			UpdateGearSwitcherBindingsRowScale();
		}

		private void UpdateGearSwitcherHpMaskIcons()
		{
			if (gearSwitcherHpMaskIcons.Count == 0)
			{
				return;
			}
			if (hpMaskSprite is null)
			{
				hpMaskSprite = LoadCollectorIconSprite("HPMask.png", "HPMask");
			}
			int maxHealth = GetSelectedPreset().MaxHealth;
			for (int i = 0; i < gearSwitcherHpMaskIcons.Count; i++)
			{
				Image image = gearSwitcherHpMaskIcons[i];
				Outline outline = gearSwitcherHpMaskGlows[i];
				if (hpMaskSprite != null)
				{
					image.sprite = hpMaskSprite;
				}
				image.rectTransform.sizeDelta = new Vector2(44f, 44f);
				bool flag = i < maxHealth;
				image.color = (flag ? GearSwitcherSpellActiveColor : GearSwitcherSpellInactiveColor);
				outline.enabled = flag;
			}
			UpdateGearSwitcherHpMaskRowScale();
		}

		private void UpdateGearSwitcherHpMaskRowScale()
		{
			if (gearSwitcherHpMaskIconRects.Count == 0)
			{
				return;
			}
			float num = float.MaxValue;
			float num2 = float.MinValue;
			foreach (RectTransform gearSwitcherHpMaskIconRect in gearSwitcherHpMaskIconRects)
			{
				float num3 = gearSwitcherHpMaskIconRect.sizeDelta.x * 0.5f;
				float x = gearSwitcherHpMaskIconRect.anchoredPosition.x;
				num = Math.Min(num, x - num3);
				num2 = Math.Max(num2, x + num3);
			}
			float num4 = num2 - num;
			float num5 = 800f;
			if (num4 <= 0f)
			{
				return;
			}
			float num6 = ((num4 > num5) ? (num5 / num4) : 1f);
			if (num6 > 1f)
			{
				num6 = 1f;
			}
			foreach (RectTransform gearSwitcherHpMaskIconRect2 in gearSwitcherHpMaskIconRects)
			{
				gearSwitcherHpMaskIconRect2.localScale = new Vector3(num6, num6, 1f);
			}
		}

		private void UpdateGearSwitcherSoulVesselIcons()
		{
			if (gearSwitcherSoulVesselIcons.Count == 0)
			{
				return;
			}
			if (soulVesselSprite is null)
			{
				soulVesselSprite = LoadCollectorIconSprite("Soul Vessel.png", "SoulVessel");
			}
			int num = Math.Max(0, Math.Min(3, GetSelectedPreset().SoulVessels));
			for (int i = 0; i < gearSwitcherSoulVesselIcons.Count; i++)
			{
				Image image = gearSwitcherSoulVesselIcons[i];
				Outline outline = gearSwitcherSoulVesselGlows[i];
				if (soulVesselSprite != null)
				{
					image.sprite = soulVesselSprite;
				}
				image.rectTransform.sizeDelta = new Vector2(44f, 44f);
				bool flag = i < num;
				image.color = (flag ? GearSwitcherSpellActiveColor : GearSwitcherSpellInactiveColor);
				outline.enabled = flag;
			}
			UpdateGearSwitcherSoulVesselRowScale();
		}

		private void UpdateGearSwitcherSoulVesselRowScale()
		{
			if (gearSwitcherSoulVesselIconRects.Count == 0)
			{
				return;
			}
			float num = float.MaxValue;
			float num2 = float.MinValue;
			foreach (RectTransform gearSwitcherSoulVesselIconRect in gearSwitcherSoulVesselIconRects)
			{
				float num3 = gearSwitcherSoulVesselIconRect.sizeDelta.x * 0.5f;
				float x = gearSwitcherSoulVesselIconRect.anchoredPosition.x;
				num = Math.Min(num, x - num3);
				num2 = Math.Max(num2, x + num3);
			}
			float num4 = num2 - num;
			float num5 = 800f;
			if (num4 <= 0f)
			{
				return;
			}
			float num6 = ((num4 > num5) ? (num5 / num4) : 1f);
			if (num6 > 1f)
			{
				num6 = 1f;
			}
			foreach (RectTransform gearSwitcherSoulVesselIconRect2 in gearSwitcherSoulVesselIconRects)
			{
				gearSwitcherSoulVesselIconRect2.localScale = new Vector3(num6, num6, 1f);
			}
		}

		private void UpdateGearSwitcherNaillessIcon()
		{
			if (!(gearSwitcherNaillessIcon == null))
			{
				if (naillessSprite is null)
				{
					naillessSprite = LoadCollectorIconSprite("Nailless.png", "Nailless");
				}
				if (naillessSprite != null)
				{
					gearSwitcherNaillessIcon.sprite = naillessSprite;
				}
				gearSwitcherNaillessIcon.rectTransform.sizeDelta = new Vector2(88f, 88f);
				bool selectedNailless = GetSelectedNailless();
				gearSwitcherNaillessIcon.color = (selectedNailless ? GearSwitcherSpellActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherNaillessGlow != null)
				{
					gearSwitcherNaillessGlow.enabled = selectedNailless;
				}
			}
		}

		private void UpdateGearSwitcherOvercharmedIcon()
		{
			if (!(gearSwitcherOvercharmedIcon == null))
			{
				if (overcharmedSprite is null)
				{
					overcharmedSprite = LoadCollectorIconSprite("OVERCHARMED.png", "Overcharmed");
				}
				if (overcharmedSprite != null)
				{
					gearSwitcherOvercharmedIcon.sprite = overcharmedSprite;
				}
				gearSwitcherOvercharmedIcon.rectTransform.sizeDelta = new Vector2(88f, 88f);
				bool selectedOvercharmed = GetSelectedOvercharmed();
				gearSwitcherOvercharmedIcon.color = (selectedOvercharmed ? GearSwitcherSpellActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherOvercharmedGlow != null)
				{
					gearSwitcherOvercharmedGlow.enabled = selectedOvercharmed;
				}
			}
		}

		private void UpdateGearSwitcherCharmPromptIcon()
		{
			if (!(gearSwitcherCharmPromptIcon == null))
			{
				if (charmsPromptSprite is null)
				{
					charmsPromptSprite = LoadCollectorIconSprite("Charms Prompt.png", "CharmsPrompt");
				}
				if (charmsPromptSprite != null)
				{
					gearSwitcherCharmPromptIcon.sprite = charmsPromptSprite;
					float num = 88f / charmsPromptSprite.rect.height;
					float x = charmsPromptSprite.rect.width * num;
					gearSwitcherCharmPromptIcon.rectTransform.sizeDelta = new Vector2(x, 88f);
				}
				else
				{
					gearSwitcherCharmPromptIcon.rectTransform.sizeDelta = new Vector2(88f, 88f);
				}
				gearSwitcherCharmPromptIcon.color = GearSwitcherSpellActiveColor;
			}
		}

		private void UpdateGearSwitcherBindingNailIcon()
		{
			if (!(gearSwitcherBindingNailIcon == null))
			{
				if (nailBindingSprite is null)
				{
					nailBindingSprite = LoadCollectorIconSprite("Nail Binding.png", "NailBinding") ?? GetBindingDefaultSprite<NailBinding>();
				}
				if (nailBindingSelectedSprite is null)
				{
					nailBindingSelectedSprite = GetBindingSelectedSprite<NailBinding>();
				}
				bool selectedBinding = GetSelectedBinding("NailBinding");
				Sprite? sprite = (selectedBinding ? (nailBindingSelectedSprite ?? nailBindingSprite) : nailBindingSprite);
				if (sprite != null)
				{
					gearSwitcherBindingNailIcon.sprite = sprite;
					gearSwitcherBindingNailIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				}
				else
				{
					gearSwitcherBindingNailIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
				gearSwitcherBindingNailIcon.color = (selectedBinding ? GearSwitcherBindingActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherBindingNailGlow != null)
				{
					gearSwitcherBindingNailGlow.enabled = selectedBinding;
				}
			}
		}

		private void UpdateGearSwitcherBindingShellIcon()
		{
			if (!(gearSwitcherBindingShellIcon == null))
			{
				if (shellBindingSprite is null)
				{
					shellBindingSprite = LoadCollectorIconSprite("Shell Binding.png", "ShellBinding") ?? GetBindingDefaultSprite<ShellBinding>();
				}
				if (shellBindingSelectedSprite is null)
				{
					shellBindingSelectedSprite = GetBindingSelectedSprite<ShellBinding>();
				}
				bool selectedBinding = GetSelectedBinding("ShellBinding");
				Sprite? sprite = (selectedBinding ? (shellBindingSelectedSprite ?? shellBindingSprite) : shellBindingSprite);
				if (sprite != null)
				{
					gearSwitcherBindingShellIcon.sprite = sprite;
					gearSwitcherBindingShellIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				}
				else
				{
					gearSwitcherBindingShellIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
				gearSwitcherBindingShellIcon.color = (selectedBinding ? GearSwitcherBindingActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherBindingShellGlow != null)
				{
					gearSwitcherBindingShellGlow.enabled = selectedBinding;
				}
			}
		}

		private void UpdateGearSwitcherBindingCharmsIcon()
		{
			if (!(gearSwitcherBindingCharmsIcon == null))
			{
				if (charmsBindingSprite is null)
				{
					charmsBindingSprite = LoadCollectorIconSprite("Charms Binding.png", "CharmsBinding") ?? GetBindingDefaultSprite<CharmsBinding>();
				}
				if (charmsBindingSelectedSprite is null)
				{
					charmsBindingSelectedSprite = GetBindingSelectedSprite<CharmsBinding>();
				}
				bool selectedBinding = GetSelectedBinding("CharmsBinding");
				Sprite? sprite = (selectedBinding ? (charmsBindingSelectedSprite ?? charmsBindingSprite) : charmsBindingSprite);
				if (sprite != null)
				{
					gearSwitcherBindingCharmsIcon.sprite = sprite;
					gearSwitcherBindingCharmsIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				}
				else
				{
					gearSwitcherBindingCharmsIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
				gearSwitcherBindingCharmsIcon.color = (selectedBinding ? GearSwitcherBindingActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherBindingCharmsGlow != null)
				{
					gearSwitcherBindingCharmsGlow.enabled = selectedBinding;
				}
			}
		}

		private void UpdateGearSwitcherBindingSoulIcon()
		{
			if (!(gearSwitcherBindingSoulIcon == null))
			{
				if (soulBindingSprite is null)
				{
					soulBindingSprite = LoadCollectorIconSprite("Soul Binding.png", "SoulBinding") ?? GetBindingDefaultSprite<SoulBinding>();
				}
				if (soulBindingSelectedSprite is null)
				{
					soulBindingSelectedSprite = GetBindingSelectedSprite<SoulBinding>();
				}
				bool selectedBinding = GetSelectedBinding("SoulBinding");
				Sprite? sprite = (selectedBinding ? (soulBindingSelectedSprite ?? soulBindingSprite) : soulBindingSprite);
				if (sprite != null)
				{
					gearSwitcherBindingSoulIcon.sprite = sprite;
					gearSwitcherBindingSoulIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				}
				else
				{
					gearSwitcherBindingSoulIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
				gearSwitcherBindingSoulIcon.color = (selectedBinding ? GearSwitcherBindingActiveColor : GearSwitcherSpellInactiveColor);
				if (gearSwitcherBindingSoulGlow != null)
				{
					gearSwitcherBindingSoulGlow.enabled = selectedBinding;
				}
			}
		}

		private void UpdateGearSwitcherBindingsRowScale()
		{
			if (gearSwitcherBindingsRowIconRects.Count == 0)
			{
				return;
			}
			float num = float.MaxValue;
			float num2 = float.MinValue;
			foreach (RectTransform gearSwitcherBindingsRowIconRect in gearSwitcherBindingsRowIconRects)
			{
				float num3 = gearSwitcherBindingsRowIconRect.sizeDelta.x * 0.5f * 0.7f;
				float x = gearSwitcherBindingsRowIconRect.anchoredPosition.x;
				num = Math.Min(num, x - num3);
				num2 = Math.Max(num2, x + num3);
			}
			float num4 = num2 - num;
			float num5 = 800f;
			if (num4 <= 0f)
			{
				return;
			}
			float num6 = ((num4 > num5) ? (num5 / num4) : 1f);
			if (num6 > 1f)
			{
				num6 = 1f;
			}
			foreach (RectTransform gearSwitcherBindingsRowIconRect2 in gearSwitcherBindingsRowIconRects)
			{
				float num7 = num6 * 0.7f;
				gearSwitcherBindingsRowIconRect2.localScale = new Vector3(num7, num7, 1f);
			}
		}

		private void UpdateGearSwitcherCloakRowScale()
		{
			if (gearSwitcherCloakRowIconRects.Count == 0)
			{
				return;
			}
			foreach (RectTransform gearSwitcherCloakRowIconRect in gearSwitcherCloakRowIconRects)
			{
				gearSwitcherCloakRowIconRect.localScale = Vector3.one;
			}
			float num = float.MaxValue;
			float num2 = float.MinValue;
			foreach (RectTransform gearSwitcherCloakRowIconRect2 in gearSwitcherCloakRowIconRects)
			{
				float num3 = gearSwitcherCloakRowIconRect2.sizeDelta.x * 0.5f;
				float x = gearSwitcherCloakRowIconRect2.anchoredPosition.x;
				num = Math.Min(num, x - num3);
				num2 = Math.Max(num2, x + num3);
			}
			float num4 = num2 - num;
			float num5 = 800f;
			if (num4 <= 0f)
			{
				return;
			}
			float num6 = ((num4 > num5) ? (num5 / num4) : 1f);
			if (num6 > 1f)
			{
				num6 = 1f;
			}
			foreach (RectTransform gearSwitcherCloakRowIconRect3 in gearSwitcherCloakRowIconRects)
			{
				gearSwitcherCloakRowIconRect3.localScale = new Vector3(num6, num6, 1f);
			}
		}

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
			// Keep UI selection synced with persisted GearSwitcher preset
			// (important on save load when startup logic forces FullGear).
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

		private void SetGearSwitcherPresetIndex(string[] options, int index)
		{
			if (options.Length != 0)
			{
				int num = ClampOptionIndex(index, options.Length);
				gearSwitcherSelectedPreset = options[num];
				RefreshGearSwitcherPresetFields();
			}
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

		private void SetSelectedPresetMaxHealth(int value)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			selectedPreset.MaxHealth = Math.Max(1, Math.Min(9, value));
			GodhomeQoL.SaveGlobalSettingsSafe();
			MarkGearSwitcherPresetEdited();
		}

		private void SetSelectedPresetSoulVessels(int value)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			selectedPreset.SoulVessels = Math.Max(0, Math.Min(3, value));
			GodhomeQoL.SaveGlobalSettingsSafe();
			MarkGearSwitcherPresetEdited();
		}

		private void AdjustWaywardCompassCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.WaywardCompassCost, delegate(GearPreset preset, int value)
			{
				preset.WaywardCompassCost = value;
			}, UpdateGearSwitcherWaywardCostValue, delta);
		}

		private void AdjustGatheringSwarmCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.GatheringSwarmCost, delegate(GearPreset preset, int value)
			{
				preset.GatheringSwarmCost = value;
			}, UpdateGearSwitcherGatheringCostValue, delta);
		}

		private void AdjustStalwartShellCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.StalwartShellCost, delegate(GearPreset preset, int value)
			{
				preset.StalwartShellCost = value;
			}, UpdateGearSwitcherStalwartShellCostValue, delta);
		}

		private void AdjustSoulCatcherCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.SoulCatcherCost, delegate(GearPreset preset, int value)
			{
				preset.SoulCatcherCost = value;
			}, UpdateGearSwitcherSoulCatcherCostValue, delta);
		}

		private void AdjustShamanStoneCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.ShamanStoneCost, delegate(GearPreset preset, int value)
			{
				preset.ShamanStoneCost = value;
			}, UpdateGearSwitcherShamanStoneCostValue, delta);
		}

		private void AdjustSoulEaterCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.SoulEaterCost, delegate(GearPreset preset, int value)
			{
				preset.SoulEaterCost = value;
			}, UpdateGearSwitcherSoulEaterCostValue, delta);
		}

		private void AdjustDashmasterCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.DashmasterCost, delegate(GearPreset preset, int value)
			{
				preset.DashmasterCost = value;
			}, UpdateGearSwitcherDashmasterCostValue, delta);
		}

		private void AdjustSprintmasterCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.SprintmasterCost, delegate(GearPreset preset, int value)
			{
				preset.SprintmasterCost = value;
			}, UpdateGearSwitcherSprintmasterCostValue, delta);
		}

		private void AdjustGrubsongCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.GrubsongCost, delegate(GearPreset preset, int value)
			{
				preset.GrubsongCost = value;
			}, UpdateGearSwitcherGrubsongCostValue, delta);
		}

		private void AdjustGrubberflysElegyCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.GrubberflysElegyCost, delegate(GearPreset preset, int value)
			{
				preset.GrubberflysElegyCost = value;
			}, UpdateGearSwitcherGrubberflysElegyCostValue, delta);
		}

		private void AdjustUnbreakableHeartCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.UnbreakableHeartCost, delegate(GearPreset preset, int value)
			{
				preset.UnbreakableHeartCost = value;
			}, UpdateGearSwitcherUnbreakableHeartCostValue, delta);
		}

		private void AdjustUnbreakableGreedCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.UnbreakableGreedCost, delegate(GearPreset preset, int value)
			{
				preset.UnbreakableGreedCost = value;
			}, UpdateGearSwitcherUnbreakableGreedCostValue, delta);
		}

		private void AdjustUnbreakableStrengthCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.UnbreakableStrengthCost, delegate(GearPreset preset, int value)
			{
				preset.UnbreakableStrengthCost = value;
			}, UpdateGearSwitcherUnbreakableStrengthCostValue, delta);
		}

		private void AdjustSpellTwisterCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.SpellTwisterCost, delegate(GearPreset preset, int value)
			{
				preset.SpellTwisterCost = value;
			}, UpdateGearSwitcherSpellTwisterCostValue, delta);
		}

		private void AdjustSteadyBodyCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.SteadyBodyCost, delegate(GearPreset preset, int value)
			{
				preset.SteadyBodyCost = value;
			}, UpdateGearSwitcherSteadyBodyCostValue, delta);
		}

		private void AdjustHeavyBlowCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.HeavyBlowCost, delegate(GearPreset preset, int value)
			{
				preset.HeavyBlowCost = value;
			}, UpdateGearSwitcherHeavyBlowCostValue, delta);
		}

		private void AdjustQuickSlashCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.QuickSlashCost, delegate(GearPreset preset, int value)
			{
				preset.QuickSlashCost = value;
			}, UpdateGearSwitcherQuickSlashCostValue, delta);
		}

		private void AdjustLongnailCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.LongnailCost, delegate(GearPreset preset, int value)
			{
				preset.LongnailCost = value;
			}, UpdateGearSwitcherLongnailCostValue, delta);
		}

		private void AdjustMarkOfPrideCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.MarkOfPrideCost, delegate(GearPreset preset, int value)
			{
				preset.MarkOfPrideCost = value;
			}, UpdateGearSwitcherMarkOfPrideCostValue, delta);
		}

		private void AdjustFuryOfTheFallenCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.FuryOfTheFallenCost, delegate(GearPreset preset, int value)
			{
				preset.FuryOfTheFallenCost = value;
			}, UpdateGearSwitcherFuryOfTheFallenCostValue, delta);
		}

		private void AdjustThornsOfAgonyCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.ThornsOfAgonyCost, delegate(GearPreset preset, int value)
			{
				preset.ThornsOfAgonyCost = value;
			}, UpdateGearSwitcherThornsOfAgonyCostValue, delta);
		}

		private void AdjustBaldurShellCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.BaldurShellCost, delegate(GearPreset preset, int value)
			{
				preset.BaldurShellCost = value;
			}, UpdateGearSwitcherBaldurShellCostValue, delta);
		}

		private void AdjustFlukenestCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.FlukenestCost, delegate(GearPreset preset, int value)
			{
				preset.FlukenestCost = value;
			}, UpdateGearSwitcherFlukenestCostValue, delta);
		}

		private void AdjustDefendersCrestCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.DefendersCrestCost, delegate(GearPreset preset, int value)
			{
				preset.DefendersCrestCost = value;
			}, UpdateGearSwitcherDefendersCrestCostValue, delta);
		}

		private void AdjustGlowingWombCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.GlowingWombCost, delegate(GearPreset preset, int value)
			{
				preset.GlowingWombCost = value;
			}, UpdateGearSwitcherGlowingWombCostValue, delta);
		}

		private void AdjustQuickFocusCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.QuickFocusCost, delegate(GearPreset preset, int value)
			{
				preset.QuickFocusCost = value;
			}, UpdateGearSwitcherQuickFocusCostValue, delta);
		}

		private void AdjustDeepFocusCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.DeepFocusCost, delegate(GearPreset preset, int value)
			{
				preset.DeepFocusCost = value;
			}, UpdateGearSwitcherDeepFocusCostValue, delta);
		}

		private void AdjustLifebloodHeartCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.LifebloodHeartCost, delegate(GearPreset preset, int value)
			{
				preset.LifebloodHeartCost = value;
			}, UpdateGearSwitcherLifebloodHeartCostValue, delta);
		}

		private void AdjustLifebloodCoreCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.LifebloodCoreCost, delegate(GearPreset preset, int value)
			{
				preset.LifebloodCoreCost = value;
			}, UpdateGearSwitcherLifebloodCoreCostValue, delta);
		}

		private void AdjustJonisBlessingCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.JonisBlessingCost, delegate(GearPreset preset, int value)
			{
				preset.JonisBlessingCost = value;
			}, UpdateGearSwitcherJonisBlessingCostValue, delta);
		}

		private void AdjustHivebloodCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.HivebloodCost, delegate(GearPreset preset, int value)
			{
				preset.HivebloodCost = value;
			}, UpdateGearSwitcherHivebloodCostValue, delta);
		}

		private void AdjustSporeShroomCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.SporeShroomCost, delegate(GearPreset preset, int value)
			{
				preset.SporeShroomCost = value;
			}, UpdateGearSwitcherSporeShroomCostValue, delta);
		}

		private void AdjustSharpShadowCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.SharpShadowCost, delegate(GearPreset preset, int value)
			{
				preset.SharpShadowCost = value;
			}, UpdateGearSwitcherSharpShadowCostValue, delta);
		}

		private void AdjustShapeOfUnnCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.ShapeOfUnnCost, delegate(GearPreset preset, int value)
			{
				preset.ShapeOfUnnCost = value;
			}, UpdateGearSwitcherShapeOfUnnCostValue, delta);
		}

		private void AdjustNailmastersGloryCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.NailmastersGloryCost, delegate(GearPreset preset, int value)
			{
				preset.NailmastersGloryCost = value;
			}, UpdateGearSwitcherNailmastersGloryCostValue, delta);
		}

		private void AdjustWeaversongCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.WeaversongCost, delegate(GearPreset preset, int value)
			{
				preset.WeaversongCost = value;
			}, UpdateGearSwitcherWeaversongCostValue, delta);
		}

		private void AdjustDreamWielderCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.DreamWielderCost, delegate(GearPreset preset, int value)
			{
				preset.DreamWielderCost = value;
			}, UpdateGearSwitcherDreamWielderCostValue, delta);
		}

		private void AdjustDreamshieldCost(int delta)
		{
			AdjustCharmCost((GearPreset preset) => preset.DreamshieldCost, delegate(GearPreset preset, int value)
			{
				preset.DreamshieldCost = value;
			}, UpdateGearSwitcherDreamshieldCostValue, delta);
		}

		private void AdjustCarefreeMelodyCost(int delta)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			if (GetPresetUseGrimmchild(selectedPreset))
			{
				AdjustCharmCost((GearPreset p) => p.GrimmchildCost, delegate(GearPreset p, int value)
				{
					p.GrimmchildCost = value;
				}, UpdateGearSwitcherCarefreeMelodyCostValue, delta);
				return;
			}
			selectedPreset.CarefreeMelodyCostInitialized = true;
			AdjustCharmCost((GearPreset p) => p.CarefreeMelodyCost, delegate(GearPreset p, int value)
			{
				p.CarefreeMelodyCost = value;
			}, UpdateGearSwitcherCarefreeMelodyCostValue, delta);
		}

		private void AdjustVoidHeartCost(int delta)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			if (GetPresetUseVoidHeart(selectedPreset))
			{
				AdjustCharmCost((GearPreset p) => p.VoidHeartCost, delegate(GearPreset p, int value)
				{
					p.VoidHeartCost = value;
				}, UpdateGearSwitcherVoidHeartCostValue, delta);
				return;
			}
			selectedPreset.KingsoulCostInitialized = true;
			AdjustCharmCost((GearPreset p) => p.KingsoulCost, delegate(GearPreset p, int value)
			{
				p.KingsoulCost = value;
			}, UpdateGearSwitcherVoidHeartCostValue, delta);
		}

		private bool GetPresetUseVoidHeart(GearPreset preset)
		{
			return preset.UseVoidHeart;
		}

		private bool GetPresetUseGrimmchild(GearPreset preset)
		{
			return preset.UseGrimmchild;
		}

		private void OnGearSwitcherCarefreeToggle()
		{
			GearPreset selectedPreset = GetSelectedPreset();
			bool flag = (selectedPreset.UseGrimmchild = !GetPresetUseGrimmchild(selectedPreset));
			if (!flag && !selectedPreset.CarefreeMelodyCostInitialized)
			{
				selectedPreset.CarefreeMelodyCost = 3;
				selectedPreset.CarefreeMelodyCostInitialized = true;
			}
			if (flag && selectedPreset.GrimmchildCost < 0)
			{
				selectedPreset.GrimmchildCost = 2;
			}
			GodhomeQoL.SaveGlobalSettingsSafe();
			GearSwitcher.ApplyCharmCostsImmediate(selectedPreset);
			UpdateGearSwitcherCarefreeIcon();
			UpdateGearSwitcherCarefreeMelodyCostValue();
			UpdateGearSwitcherCharmCostHighlights();
			MarkGearSwitcherPresetEdited();
		}

		private void OnGearSwitcherVoidHeartToggle()
		{
			GearPreset selectedPreset = GetSelectedPreset();
			bool flag = (selectedPreset.UseVoidHeart = !GetPresetUseVoidHeart(selectedPreset));
			if (!flag && !selectedPreset.KingsoulCostInitialized)
			{
				selectedPreset.KingsoulCost = 5;
				selectedPreset.KingsoulCostInitialized = true;
			}
			GodhomeQoL.SaveGlobalSettingsSafe();
			GearSwitcher.ApplyCharmCostsImmediate(selectedPreset);
			UpdateGearSwitcherVoidHeartIcon();
			UpdateGearSwitcherVoidHeartCostValue();
			UpdateGearSwitcherCharmCostHighlights();
			MarkGearSwitcherPresetEdited();
		}

		private void UpdateGearSwitcherWaywardCostValue()
		{
			UpdateCharmCostValue(gearSwitcherWaywardCostValue, (GearPreset preset) => preset.WaywardCompassCost);
		}

		private void UpdateGearSwitcherGatheringCostValue()
		{
			UpdateCharmCostValue(gearSwitcherGatheringCostValue, (GearPreset preset) => preset.GatheringSwarmCost);
		}

		private void UpdateGearSwitcherStalwartShellCostValue()
		{
			UpdateCharmCostValue(gearSwitcherStalwartShellCostValue, (GearPreset preset) => preset.StalwartShellCost);
		}

		private void UpdateGearSwitcherSoulCatcherCostValue()
		{
			UpdateCharmCostValue(gearSwitcherSoulCatcherCostValue, (GearPreset preset) => preset.SoulCatcherCost);
		}

		private void UpdateGearSwitcherShamanStoneCostValue()
		{
			UpdateCharmCostValue(gearSwitcherShamanStoneCostValue, (GearPreset preset) => preset.ShamanStoneCost);
		}

		private void UpdateGearSwitcherSoulEaterCostValue()
		{
			UpdateCharmCostValue(gearSwitcherSoulEaterCostValue, (GearPreset preset) => preset.SoulEaterCost);
		}

		private void UpdateGearSwitcherDashmasterCostValue()
		{
			UpdateCharmCostValue(gearSwitcherDashmasterCostValue, (GearPreset preset) => preset.DashmasterCost);
		}

		private void UpdateGearSwitcherSprintmasterCostValue()
		{
			UpdateCharmCostValue(gearSwitcherSprintmasterCostValue, (GearPreset preset) => preset.SprintmasterCost);
		}

		private void UpdateGearSwitcherGrubsongCostValue()
		{
			UpdateCharmCostValue(gearSwitcherGrubsongCostValue, (GearPreset preset) => preset.GrubsongCost);
		}

		private void UpdateGearSwitcherGrubberflysElegyCostValue()
		{
			UpdateCharmCostValue(gearSwitcherGrubberflysElegyCostValue, (GearPreset preset) => preset.GrubberflysElegyCost);
		}

		private void UpdateGearSwitcherUnbreakableHeartCostValue()
		{
			UpdateCharmCostValue(gearSwitcherUnbreakableHeartCostValue, (GearPreset preset) => preset.UnbreakableHeartCost);
		}

		private void UpdateGearSwitcherUnbreakableGreedCostValue()
		{
			UpdateCharmCostValue(gearSwitcherUnbreakableGreedCostValue, (GearPreset preset) => preset.UnbreakableGreedCost);
		}

		private void UpdateGearSwitcherUnbreakableStrengthCostValue()
		{
			UpdateCharmCostValue(gearSwitcherUnbreakableStrengthCostValue, (GearPreset preset) => preset.UnbreakableStrengthCost);
		}

		private void UpdateGearSwitcherSpellTwisterCostValue()
		{
			UpdateCharmCostValue(gearSwitcherSpellTwisterCostValue, (GearPreset preset) => preset.SpellTwisterCost);
		}

		private void UpdateGearSwitcherSteadyBodyCostValue()
		{
			UpdateCharmCostValue(gearSwitcherSteadyBodyCostValue, (GearPreset preset) => preset.SteadyBodyCost);
		}

		private void UpdateGearSwitcherHeavyBlowCostValue()
		{
			UpdateCharmCostValue(gearSwitcherHeavyBlowCostValue, (GearPreset preset) => preset.HeavyBlowCost);
		}

		private void UpdateGearSwitcherQuickSlashCostValue()
		{
			UpdateCharmCostValue(gearSwitcherQuickSlashCostValue, (GearPreset preset) => preset.QuickSlashCost);
		}

		private void UpdateGearSwitcherLongnailCostValue()
		{
			UpdateCharmCostValue(gearSwitcherLongnailCostValue, (GearPreset preset) => preset.LongnailCost);
		}

		private void UpdateGearSwitcherMarkOfPrideCostValue()
		{
			UpdateCharmCostValue(gearSwitcherMarkOfPrideCostValue, (GearPreset preset) => preset.MarkOfPrideCost);
		}

		private void UpdateGearSwitcherFuryOfTheFallenCostValue()
		{
			UpdateCharmCostValue(gearSwitcherFuryOfTheFallenCostValue, (GearPreset preset) => preset.FuryOfTheFallenCost);
		}

		private void UpdateGearSwitcherThornsOfAgonyCostValue()
		{
			UpdateCharmCostValue(gearSwitcherThornsOfAgonyCostValue, (GearPreset preset) => preset.ThornsOfAgonyCost);
		}

		private void UpdateGearSwitcherBaldurShellCostValue()
		{
			UpdateCharmCostValue(gearSwitcherBaldurShellCostValue, (GearPreset preset) => preset.BaldurShellCost);
		}

		private void UpdateGearSwitcherFlukenestCostValue()
		{
			UpdateCharmCostValue(gearSwitcherFlukenestCostValue, (GearPreset preset) => preset.FlukenestCost);
		}

		private void UpdateGearSwitcherDefendersCrestCostValue()
		{
			UpdateCharmCostValue(gearSwitcherDefendersCrestCostValue, (GearPreset preset) => preset.DefendersCrestCost);
		}

		private void UpdateGearSwitcherGlowingWombCostValue()
		{
			UpdateCharmCostValue(gearSwitcherGlowingWombCostValue, (GearPreset preset) => preset.GlowingWombCost);
		}

		private void UpdateGearSwitcherQuickFocusCostValue()
		{
			UpdateCharmCostValue(gearSwitcherQuickFocusCostValue, (GearPreset preset) => preset.QuickFocusCost);
		}

		private void UpdateGearSwitcherDeepFocusCostValue()
		{
			UpdateCharmCostValue(gearSwitcherDeepFocusCostValue, (GearPreset preset) => preset.DeepFocusCost);
		}

		private void UpdateGearSwitcherLifebloodHeartCostValue()
		{
			UpdateCharmCostValue(gearSwitcherLifebloodHeartCostValue, (GearPreset preset) => preset.LifebloodHeartCost);
		}

		private void UpdateGearSwitcherLifebloodCoreCostValue()
		{
			UpdateCharmCostValue(gearSwitcherLifebloodCoreCostValue, (GearPreset preset) => preset.LifebloodCoreCost);
		}

		private void UpdateGearSwitcherJonisBlessingCostValue()
		{
			UpdateCharmCostValue(gearSwitcherJonisBlessingCostValue, (GearPreset preset) => preset.JonisBlessingCost);
		}

		private void UpdateGearSwitcherHivebloodCostValue()
		{
			UpdateCharmCostValue(gearSwitcherHivebloodCostValue, (GearPreset preset) => preset.HivebloodCost);
		}

		private void UpdateGearSwitcherSporeShroomCostValue()
		{
			UpdateCharmCostValue(gearSwitcherSporeShroomCostValue, (GearPreset preset) => preset.SporeShroomCost);
		}

		private void UpdateGearSwitcherSharpShadowCostValue()
		{
			UpdateCharmCostValue(gearSwitcherSharpShadowCostValue, (GearPreset preset) => preset.SharpShadowCost);
		}

		private void UpdateGearSwitcherShapeOfUnnCostValue()
		{
			UpdateCharmCostValue(gearSwitcherShapeOfUnnCostValue, (GearPreset preset) => preset.ShapeOfUnnCost);
		}

		private void UpdateGearSwitcherNailmastersGloryCostValue()
		{
			UpdateCharmCostValue(gearSwitcherNailmastersGloryCostValue, (GearPreset preset) => preset.NailmastersGloryCost);
		}

		private void UpdateGearSwitcherWeaversongCostValue()
		{
			UpdateCharmCostValue(gearSwitcherWeaversongCostValue, (GearPreset preset) => preset.WeaversongCost);
		}

		private void UpdateGearSwitcherDreamWielderCostValue()
		{
			UpdateCharmCostValue(gearSwitcherDreamWielderCostValue, (GearPreset preset) => preset.DreamWielderCost);
		}

		private void UpdateGearSwitcherDreamshieldCostValue()
		{
			UpdateCharmCostValue(gearSwitcherDreamshieldCostValue, (GearPreset preset) => preset.DreamshieldCost);
		}

		private void UpdateGearSwitcherCarefreeMelodyCostValue()
		{
			if (!(gearSwitcherCarefreeMelodyCostValue == null))
			{
				GearPreset selectedPreset = GetSelectedPreset();
				int val = (GetPresetUseGrimmchild(selectedPreset) ? selectedPreset.GrimmchildCost : selectedPreset.CarefreeMelodyCost);
				gearSwitcherCarefreeMelodyCostValue.text = Math.Max(0, Math.Min(99, val)).ToString();
			}
		}

		private void UpdateGearSwitcherVoidHeartCostValue()
		{
			if (!(gearSwitcherVoidHeartCostValue == null))
			{
				GearPreset selectedPreset = GetSelectedPreset();
				int val = (GetPresetUseVoidHeart(selectedPreset) ? selectedPreset.VoidHeartCost : selectedPreset.KingsoulCost);
				gearSwitcherVoidHeartCostValue.text = Math.Max(0, Math.Min(99, val)).ToString();
			}
		}

		private void UpdateGearSwitcherVoidHeartIcon()
		{
			if (!(gearSwitcherVoidHeartIcon == null))
			{
				if (voidHeartSprite is null)
				{
					voidHeartSprite = LoadCollectorIconSprite("Void Heart.png", "VoidHeart");
				}
				if (kingsoulSprite is null)
				{
					kingsoulSprite = LoadCollectorIconSprite("Kingsoul.png", "Kingsoul");
				}
				GearPreset selectedPreset = GetSelectedPreset();
				Sprite? sprite = (GetPresetUseVoidHeart(selectedPreset) ? voidHeartSprite : kingsoulSprite);
				if (sprite != null)
				{
					gearSwitcherVoidHeartIcon.sprite = sprite;
					float num = Mathf.Min(1.4f, 64f / sprite.rect.height);
					gearSwitcherVoidHeartIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width * num, sprite.rect.height * num);
				}
				else
				{
					gearSwitcherVoidHeartIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
			}
		}

		private void UpdateGearSwitcherCarefreeIcon()
		{
			if (!(gearSwitcherCarefreeIcon == null))
			{
				if (grimmchildSprite is null)
				{
					grimmchildSprite = LoadCollectorIconSprite("Grimmchild.png", "Grimmchild");
				}
				if (carefreeMelodySprite is null)
				{
					carefreeMelodySprite = LoadCollectorIconSprite("Carefree Melody.png", "CarefreeMelody");
				}
				GearPreset selectedPreset = GetSelectedPreset();
				Sprite? sprite = (GetPresetUseGrimmchild(selectedPreset) ? grimmchildSprite : carefreeMelodySprite);
				if (sprite != null)
				{
					gearSwitcherCarefreeIcon.sprite = sprite;
					float num = Mathf.Min(1.4f, 64f / sprite.rect.height);
					gearSwitcherCarefreeIcon.rectTransform.sizeDelta = new Vector2(sprite.rect.width * num, sprite.rect.height * num);
				}
				else
				{
					gearSwitcherCarefreeIcon.rectTransform.sizeDelta = new Vector2(44f, 44f);
				}
			}
		}

		private void AdjustCharmCost(Func<GearPreset, int> getter, Action<GearPreset, int> setter, Action updateAction, int delta)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			int num = Math.Max(0, Math.Min(99, getter(selectedPreset)));
			int num2 = Math.Max(0, Math.Min(99, num + delta));
			if (num2 != num)
			{
				setter(selectedPreset, num2);
				GodhomeQoL.SaveGlobalSettingsSafe();
				GearSwitcher.ApplyCharmCostsImmediate(selectedPreset);
				updateAction();
				UpdateGearSwitcherCharmCostHighlights();
				MarkGearSwitcherPresetEdited();
			}
		}

		private void UpdateCharmCostValue(Text? valueField, Func<GearPreset, int> getter)
		{
			if (!(valueField == null))
			{
				GearPreset selectedPreset = GetSelectedPreset();
				valueField.text = Math.Max(0, Math.Min(99, getter(selectedPreset))).ToString();
			}
		}

		private void SetSelectedPresetNailDamage(int value)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			int num = Math.Max(-99999, Math.Min(99999, value));
			if (selectedPreset.NailDamage != num)
			{
				selectedPreset.NailDamage = num;
				GodhomeQoL.SaveGlobalSettingsSafe();
				GearSwitcher.ApplyStatsImmediate(selectedPreset);
				MarkGearSwitcherPresetEdited();
			}
		}

		private void SetSelectedPresetCharmSlots(int value)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			int num = Math.Max(3, Math.Min(999, value));
			if (selectedPreset.CharmSlots != num)
			{
				selectedPreset.CharmSlots = num;
				GodhomeQoL.SaveGlobalSettingsSafe();
				GearSwitcher.ApplyStatsImmediate(selectedPreset);
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
				GearSwitcher.ApplySoulGainImmediate(selectedPreset);
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
				GearSwitcher.ApplySoulGainImmediate(selectedPreset);
				MarkGearSwitcherPresetEdited();
			}
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

		private int GetSelectedDreamNailIconLevel()
		{
			int num = Math.Max(0, Math.Min(3, GetSelectedPreset().DreamNailLevel));
			if (num >= 3)
			{
				return 2;
			}
			return (num >= 1) ? 1 : 0;
		}

		private bool GetSelectedDreamgateEnabled()
		{
			return GetSelectedPreset().DreamNailLevel >= 2;
		}

		private void ApplyDreamNailIconLevel(int iconLevel)
		{
			iconLevel = Math.Max(0, Math.Min(2, iconLevel));
			int dreamNailLevel = GetSelectedPreset().DreamNailLevel;
			bool flag = dreamNailLevel >= 2;
			switch (iconLevel)
			{
			case 0:
				SetSelectedPresetDreamNail(0);
				break;
			case 1:
				SetSelectedPresetDreamNail((!flag) ? 1 : 2);
				break;
			default:
				SetSelectedPresetDreamNail(3);
				break;
			}
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

		private void SetSelectedPresetDreamNail(int value)
		{
			GearPreset selectedPreset = GetSelectedPreset();
			selectedPreset.DreamNailLevel = Math.Max(0, Math.Min(3, value));
			GodhomeQoL.SaveGlobalSettingsSafe();
			UpdateGearSwitcherCloakRowIcons();
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
