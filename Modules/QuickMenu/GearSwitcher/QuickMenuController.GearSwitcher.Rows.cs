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
		private const float GearSwitcherGroupFrameHeight = 72f;
		private const float GearSwitcherFirstGroupSpacing = 72f;
		private const float GearSwitcherGroupSpacing = 82f;
		private const float GearSwitcherHpSoulSpacing = 54f;
		private const float GearSwitcherAfterGroupsSpacing = 70f;
		private const float GearSwitcherPromptSpacing = 98f;
		private const float GearSwitcherBigIconFrameHeight = 80f;
		private const float GearSwitcherBigIconFrameY = -24f;

		private void CreateGearSwitcherSpellsRow(Transform parent, float y)
		{
			GameObject gameObject = CreateRow(parent, "GearSwitcherSpellsRow", y, new Vector2(820f, 44f));
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			AddHkGroupFrame(gameObject.transform, 270f, GearSwitcherGroupFrameHeight, 0f);
			CreateGearSwitcherSpellIcon(gameObject.transform, "FireballIcon", -90f, "fireballLevel", UpdateGearSwitcherFireballIcon, out gearSwitcherFireballIcon, out gearSwitcherFireballGlow);
			CreateGearSwitcherSpellIcon(gameObject.transform, "QuakeIcon", 0f, "quakeLevel", UpdateGearSwitcherQuakeIcon, out gearSwitcherQuakeIcon, out gearSwitcherQuakeGlow);
			CreateGearSwitcherSpellIcon(gameObject.transform, "ScreamIcon", 90f, "screamLevel", UpdateGearSwitcherScreamIcon, out gearSwitcherScreamIcon, out gearSwitcherScreamGlow);
			UpdateGearSwitcherFireballIcon();
			UpdateGearSwitcherQuakeIcon();
			UpdateGearSwitcherScreamIcon();
		}

		private void CreateGearSwitcherNailArtsRow(Transform parent, float y)
		{
			GameObject gameObject = CreateRow(parent, "GearSwitcherNailArtsRow", y, new Vector2(820f, 44f));
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			AddHkGroupFrame(gameObject.transform, 270f, GearSwitcherGroupFrameHeight, 0f);
			CreateGearSwitcherNailArtIcon(gameObject.transform, "CycloneSlashIcon", -90f, "hasCyclone", UpdateGearSwitcherCycloneIcon, out gearSwitcherCycloneIcon, out gearSwitcherCycloneGlow);
			CreateGearSwitcherNailArtIcon(gameObject.transform, "DashSlashIcon", 0f, "hasUpwardSlash", UpdateGearSwitcherDashSlashIcon, out gearSwitcherDashSlashIcon, out gearSwitcherDashSlashGlow);
			CreateGearSwitcherNailArtIcon(gameObject.transform, "GreatSlashIcon", 90f, "hasDashSlash", UpdateGearSwitcherGreatSlashIcon, out gearSwitcherGreatSlashIcon, out gearSwitcherGreatSlashGlow);
			UpdateGearSwitcherCycloneIcon();
			UpdateGearSwitcherDashSlashIcon();
			UpdateGearSwitcherGreatSlashIcon();
		}

		private void CreateGearSwitcherCloakRow(Transform parent, float y)
		{
			GameObject gameObject = CreateRow(parent, "GearSwitcherCloakRow", y, new Vector2(820f, 44f));
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			AddHkGroupFrame(gameObject.transform, 610f, GearSwitcherGroupFrameHeight, 0f);
			gearSwitcherCloakRowIconRects.Clear();
			CreateGearSwitcherCloakIcon(gameObject.transform, -264f);
			CreateGearSwitcherMoveAbilityIcon(gameObject.transform, "MantisClawIcon", -176f, "Walljump", out gearSwitcherMantisClawIcon, out gearSwitcherMantisClawGlow);
			CreateGearSwitcherMoveAbilityIcon(gameObject.transform, "MonarchWingsIcon", -88f, "DoubleJump", out gearSwitcherMonarchWingsIcon, out gearSwitcherMonarchWingsGlow);
			CreateGearSwitcherMoveAbilityIcon(gameObject.transform, "CrystalHeartIcon", 0f, "SuperDash", out gearSwitcherCrystalHeartIcon, out gearSwitcherCrystalHeartGlow);
			CreateGearSwitcherMoveAbilityIcon(gameObject.transform, "IsmasTearIcon", 88f, "AcidArmour", out gearSwitcherIsmasTearIcon, out gearSwitcherIsmasTearGlow);
			CreateGearSwitcherDreamNailIcon(gameObject.transform, 176f);
			CreateGearSwitcherDreamgateIcon(gameObject.transform, 264f);
			UpdateGearSwitcherCloakRowIcons();
		}

		private void CreateGearSwitcherBindingsRow(Transform parent, float y)
		{
			GameObject gameObject = CreateRow(parent, "GearSwitcherBindingsRow", y, new Vector2(820f, 44f));
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			AddHkGroupFrame(gameObject.transform, 340f, GearSwitcherGroupFrameHeight, 0f);
			gearSwitcherBindingsRowIconRects.Clear();
			CreateGearSwitcherBindingIcon(gameObject.transform, "BindingNailIcon", -132f, "NailBinding", out gearSwitcherBindingNailIcon, out gearSwitcherBindingNailGlow);
			CreateGearSwitcherBindingIcon(gameObject.transform, "BindingShellIcon", -44f, "ShellBinding", out gearSwitcherBindingShellIcon, out gearSwitcherBindingShellGlow);
			CreateGearSwitcherBindingIcon(gameObject.transform, "BindingCharmsIcon", 44f, "CharmsBinding", out gearSwitcherBindingCharmsIcon, out gearSwitcherBindingCharmsGlow);
			CreateGearSwitcherBindingIcon(gameObject.transform, "BindingSoulIcon", 132f, "SoulBinding", out gearSwitcherBindingSoulIcon, out gearSwitcherBindingSoulGlow);
			UpdateGearSwitcherBindingsRowIcons();
		}

		private void CreateGearSwitcherHpMaskRow(Transform parent, float y)
		{
			GameObject gameObject = CreateRow(parent, "GearSwitcherHpMaskRow", y, new Vector2(820f, 44f));
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			AddHkGroupFrame(gameObject.transform, 600f, GearSwitcherGroupFrameHeight + GearSwitcherHpSoulSpacing, -GearSwitcherHpSoulSpacing * 0.5f);
			gearSwitcherHpMaskIcons.Clear();
			gearSwitcherHpMaskGlows.Clear();
			gearSwitcherHpMaskIconRects.Clear();
			for (int i = 0; i < 9; i++)
			{
				CreateGearSwitcherHpMaskIcon(gameObject.transform, (float)(i - 4) * 62f, i + 1);
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
				CreateGearSwitcherSoulVesselIcon(gameObject.transform, (float)(i - 1) * 78f, i + 1);
			}
			UpdateGearSwitcherSoulVesselIcons();
		}

		private void CreateGearSwitcherNaillessRow(Transform parent, float y)
		{
			GameObject gameObject = CreateRow(parent, "GearSwitcherNaillessRow", y, new Vector2(820f, 44f));
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			AddHkGroupFrame(gameObject.transform, 220f, GearSwitcherBigIconFrameHeight, GearSwitcherBigIconFrameY);
			GameObject gameObject2 = new GameObject("NaillessIcon");
			gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
			RectTransform rectTransform = gameObject2.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(-50f, -24f);
			rectTransform.sizeDelta = new Vector2(GearSwitcherBigIconSize, GearSwitcherBigIconSize);
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
			rectTransform2.sizeDelta = new Vector2(GearSwitcherBigIconSize, GearSwitcherBigIconSize);
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
			AddHkGroupFrame(gameObject.transform, 120f, GearSwitcherBigIconFrameHeight, GearSwitcherBigIconFrameY);
			GameObject gameObject2 = new GameObject("CharmsPromptIcon");
			gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
			RectTransform rectTransform = gameObject2.AddComponent<RectTransform>();
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(0f, -24f);
			rectTransform.sizeDelta = new Vector2(GearSwitcherBigIconSize, GearSwitcherBigIconSize);
			Image image2 = gameObject2.AddComponent<Image>();
			image2.preserveAspect = true;
			Button button = gameObject2.AddComponent<Button>();
			button.transition = Selectable.Transition.None;
			button.targetGraphic = image2;
			button.onClick.AddListener(OnGearSwitcherCharmPromptClicked);
			gearSwitcherCharmPromptIcon = image2;
			UpdateGearSwitcherCharmPromptIcon();

			GameObject strip = new GameObject("EquippedCharms");
			strip.transform.SetParent(gameObject.transform, worldPositionStays: false);
			RectTransform stripRect = strip.AddComponent<RectTransform>();
			stripRect.anchorMin = new Vector2(0.5f, 0.5f);
			stripRect.anchorMax = new Vector2(0.5f, 0.5f);
			stripRect.pivot = new Vector2(0.5f, 1f);
			stripRect.anchoredPosition = new Vector2(0f, GearSwitcherBigIconFrameY - (GearSwitcherBigIconFrameHeight * 0.5f) - 8f);
			stripRect.sizeDelta = new Vector2(GearSwitcherEquippedCharmStep * GearSwitcherEquippedCharmsPerLine, GearSwitcherEquippedCharmsSpace);
			gearSwitcherEquippedCharmsRoot = stripRect;
			UpdateGearSwitcherEquippedCharms();
		}

		private const float GearSwitcherEquippedCharmSize = 34f;
		private const float GearSwitcherEquippedCharmStep = 40f;
		private const int GearSwitcherEquippedCharmsPerLine = 16;
		private const int GearSwitcherEquippedCharmLines = 2;
		private const float GearSwitcherEquippedCharmsSpace = 84f;

		private RectTransform? gearSwitcherEquippedCharmsRoot;
		private readonly Dictionary<int, Sprite?> gearSwitcherEquippedCharmSprites = new();

		private void UpdateGearSwitcherEquippedCharms()
		{
			RectTransform? root = gearSwitcherEquippedCharmsRoot;
			if (root == null)
			{
				return;
			}

			for (int i = root.childCount - 1; i >= 0; i--)
			{
				UObject.Destroy(root.GetChild(i).gameObject);
			}

			List<int>? charms = GetSelectedPreset().EquippedCharms;
			if (charms == null || charms.Count == 0)
			{
				Text note = CreateText(root, "Note", (charms == null ? "GearSwitcher/CharmsKept" : "GearSwitcher/NoCharms").Localize(), 20, TextAnchor.MiddleCenter);
				RectTransform noteRect = note.rectTransform;
				noteRect.anchorMin = new Vector2(0.5f, 1f);
				noteRect.anchorMax = new Vector2(0.5f, 1f);
				noteRect.pivot = new Vector2(0.5f, 1f);
				noteRect.anchoredPosition = Vector2.zero;
				noteRect.sizeDelta = new Vector2(root.sizeDelta.x, GearSwitcherEquippedCharmStep);
				note.color = HkOffValueColor;
				return;
			}

			int capacity = GearSwitcherEquippedCharmsPerLine * GearSwitcherEquippedCharmLines;
			bool overflow = charms.Count > capacity;
			int shown = overflow ? capacity - 1 : charms.Count;
			int slots = overflow ? capacity : charms.Count;
			for (int slot = 0; slot < slots; slot++)
			{
				int line = slot / GearSwitcherEquippedCharmsPerLine;
				int inLine = Math.Min(GearSwitcherEquippedCharmsPerLine, slots - (line * GearSwitcherEquippedCharmsPerLine));
				int column = slot % GearSwitcherEquippedCharmsPerLine;
				Vector2 position = new Vector2((column - ((inLine - 1) * 0.5f)) * GearSwitcherEquippedCharmStep, -(GearSwitcherEquippedCharmStep * 0.5f) - (line * GearSwitcherEquippedCharmStep));
				if (slot >= shown)
				{
					Text more = CreateText(root, "More", $"+{charms.Count - shown}", 20, TextAnchor.MiddleCenter);
					PlaceEquippedCharm(more.rectTransform, position);
					more.color = HkValueColor;
					continue;
				}

				GameObject icon = new GameObject($"Charm{charms[slot]}");
				icon.transform.SetParent(root, worldPositionStays: false);
				RectTransform iconRect = icon.AddComponent<RectTransform>();
				PlaceEquippedCharm(iconRect, position);
				Image image = icon.AddComponent<Image>();
				image.preserveAspect = true;
				image.raycastTarget = false;
				Sprite? sprite = GetEquippedCharmSprite(charms[slot]);
				if (sprite != null)
				{
					image.sprite = sprite;
				}
				else
				{
					image.enabled = false;
				}
			}
		}

		private static void PlaceEquippedCharm(RectTransform rect, Vector2 position)
		{
			rect.anchorMin = new Vector2(0.5f, 1f);
			rect.anchorMax = new Vector2(0.5f, 1f);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.anchoredPosition = position;
			rect.sizeDelta = new Vector2(GearSwitcherEquippedCharmSize, GearSwitcherEquippedCharmSize);
		}

		private Sprite? GetEquippedCharmSprite(int charmId)
		{
			if (gearSwitcherEquippedCharmSprites.TryGetValue(charmId, out Sprite? cached) && cached != null)
			{
				return cached;
			}

			Sprite? sprite = null;
			foreach (CharmCostDefinition definition in CharmCostDefinitions)
			{
				if (GearSwitcher.TryGetCharmId(definition.Key, out int id) && id == charmId)
				{
					sprite = LoadCollectorIconSprite(definition.SpriteFile, definition.Key);
					break;
				}
			}

			if (sprite == null)
			{
				try
				{
					sprite = CharmIconList.Instance != null ? CharmIconList.Instance.GetSprite(charmId) : null;
				}
				catch (Exception ex)
				{
					LogSuppressed(ex, "QuickMenuController.GearSwitcher.Rows.cs/CharmSprite");
				}
			}

			gearSwitcherEquippedCharmSprites[charmId] = sprite;
			return sprite;
		}
    }
}
