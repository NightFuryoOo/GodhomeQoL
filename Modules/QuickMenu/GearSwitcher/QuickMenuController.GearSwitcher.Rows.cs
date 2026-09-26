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
    }
}
