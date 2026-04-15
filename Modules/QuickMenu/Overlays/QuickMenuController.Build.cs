using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildUi()
        {
            BuildStatusUi();
            BuildQuickUi();
            BuildQuickSettingsOverlayUi();
            BuildOverlayUi();
            BuildBossManipulateOverlayUi();
            BuildBossManipulateOtherRoomsOverlayUi();
            BuildCollectorOverlayUi();
            BuildFastReloadOverlayUi();
            BuildDreamshieldOverlayUi();
            BuildShowHpOnDeathOverlayUi();
            BuildMaskDamageOverlayUi();
            BuildFreezeHitboxesOverlayUi();
            BuildSpeedChangerOverlayUi();
            BuildTeleportKitOverlayUi();
            BuildBossChallengeOverlayUi();
            BuildRandomPantheonsOverlayUi();
            BuildTrueBossRushOverlayUi();
            BuildCheatsOverlayUi();
            BuildAlwaysFuriousOverlayUi();
            BuildGearSwitcherOverlayUi();
            BuildGearSwitcherCharmCostOverlayUi();
            BuildGearSwitcherPresetOverlayUi();
            BuildQolOverlayUi();
            BuildMenuAnimationOverlayUi();
            BuildBossAnimationOverlayUi();
            BuildZoteHelperOverlayUi();
            BuildGruzHelperOverlayUi();
            BuildGruzMotherP1HelperOverlayUi();
            BuildVengeflyKingP1HelperOverlayUi();
            BuildBroodingMawlekP1HelperOverlayUi();
            BuildNoskP2HelperOverlayUi();
            BuildUumuuP3HelperOverlayUi();
            BuildSoulWarriorP1HelperOverlayUi();
            BuildNoEyesP4HelperOverlayUi();
            BuildMarmuP2HelperOverlayUi();
            BuildXeroP2HelperOverlayUi();
            BuildMarkothP4HelperOverlayUi();
            BuildGorbP1HelperOverlayUi();
            BuildHornetHelperOverlayUi();
            BuildMawlekHelperOverlayUi();
            BuildMassiveMossHelperOverlayUi();
            BuildCrystalGuardianHelperOverlayUi();
            BuildEnragedGuardianHelperOverlayUi();
            BuildHornetSentinelHelperOverlayUi();
            BuildAdditionalGhostHelpersOverlayUi();
        }

        private void BuildQuickUi()
        {
            quickRoot = new GameObject("QuickMenuCanvas");
            quickRoot.transform.SetParent(transform, false);

            Canvas canvas = quickRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = QuickCanvasSortOrder;

            CanvasScaler scaler = quickRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            quickRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = quickRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject panel = CreateQuickPanel(quickRoot.transform);
            quickHandleSprite ??= LoadQuickHandleSprite();
            quickPanelRect = panel.GetComponent<RectTransform>();
            GameObject backplate = CreateQuickPanelBackplate(panel.transform);
            quickPanelBackplateRect = backplate.GetComponent<RectTransform>();
            quickPanelImage = backplate.GetComponent<Image>();
            quickPanelContentRect = CreateQuickPanelContent(panel.transform);
            quickPanelGroup = quickPanelContentRect.GetComponent<CanvasGroup>() ?? quickPanelContentRect.gameObject.AddComponent<CanvasGroup>();
            quickEntries.Clear();

            List<QuickMenuItemDefinition> orderedItems = GetVisibleQuickMenuDefinitions();
            for (int i = 0; i < orderedItems.Count; i++)
            {
                QuickMenuEntry entry = CreateQuickEntry(quickPanelContentRect.transform, orderedItems[i], GetQuickRowY(i));
                quickEntries.Add(entry);
            }

            ApplyQuickMenuLayout();
            ApplyQuickMenuOpacity();
            UpdateFreeMenuLabel();
            UpdateRenameModeLabel();
            UpdateQuickMenuEntryStateColors();
        }

        private void BuildQuickSettingsOverlayUi()
        {
            quickSettingsRoot = new GameObject("QuickMenuSettingsOverlayCanvas");
            quickSettingsRoot.transform.SetParent(transform, false);

            Canvas canvas = quickSettingsRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = QuickSettingsCanvasSortOrder;

            CanvasScaler scaler = quickSettingsRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            quickSettingsRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = quickSettingsRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(quickSettingsRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("QuickMenuSettingsPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, QuickSettingsPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "QuickMenu/Settings".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 420f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = QuickSettingsPanelHeight;
            float backY = GetFixedBackY(panelHeight);
            float resetY = GetFixedResetY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);

            quickSettingsToggleValues.Clear();
            quickSettingsHotkeyValues.Clear();

            float rowY = GetRowStartY(panelHeight, QuickSettingsRowStartY, topOffset);
            float lastY = rowY;
            foreach (QuickMenuItemDefinition def in GetOrderedQuickMenuDefinitions())
            {
                if (string.Equals(def.Id, "Settings", StringComparison.Ordinal)
                    || string.Equals(def.Id, "FreeMenu", StringComparison.Ordinal)
                    || string.Equals(def.Id, "RenameMode", StringComparison.Ordinal))
                {
                    continue;
                }

                string label = GetQuickMenuSettingsLabel(def);
                CreateToggleRow(
                    content,
                    $"{def.Id}VisibilityRow",
                    label,
                    rowY,
                    () => IsQuickMenuItemVisible(def.Id),
                    value =>
                    {
                        SetQuickMenuItemVisible(def.Id, value);
                        RebuildQuickMenuEntries();
                    },
                    out Text valueText
                );
                quickSettingsToggleValues[def.Id] = valueText;
                lastY = rowY;
                rowY += RowSpacing;

                if (IsOverlayHotkeySupported(def.Id))
                {
                    string hotkeyLabel = $"{label} Hotkey";
                    CreateKeybindRow(
                        content,
                        $"{def.Id}HotkeyRow",
                        hotkeyLabel,
                        rowY,
                        () => GetOverlayHotkeyLabel(def.Id),
                        () => StartOverlayHotkeyRebind(def.Id),
                        out Text hotkeyValueText
                    );
                    quickSettingsHotkeyValues[def.Id] = hotkeyValueText;
                    lastY = rowY;
                    rowY += RowSpacing;
                }
            }

            CreateFloatSliderRow(
                content,
                "QuickMenuOpacityRow",
                "QuickMenu/Opacity".Localize(),
                rowY,
                rowY + 40f,
                1f,
                100f,
                GetQuickMenuOpacity(),
                0,
                out quickMenuOpacityValue,
                out quickMenuOpacitySlider
            );
            if (quickMenuOpacitySlider != null)
            {
                quickMenuOpacitySlider.wholeNumbers = true;
                quickMenuOpacitySlider.onValueChanged.AddListener(OnQuickMenuOpacityChanged);
            }

            lastY = rowY + 40f;
            rowY += RowSpacing + 40f;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "QuickMenuSettingsResetRow", "Reset Defaults", resetY, OnQuickSettingsResetDefaultsClicked);
            CreateButtonRow(panel.transform, "QuickMenuSettingsBackRow", "Back", backY, OnQuickMenuSettingsBackClicked);
            CreateQuickSettingsResetConfirm(panel.transform);
            SetQuickSettingsResetConfirmVisible(false);
        }

        private void BuildStatusUi()
        {
            statusRoot = new GameObject("StatusCanvas");
            statusRoot.transform.SetParent(transform, false);

            Canvas canvas = statusRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = StatusCanvasSortOrder;

            CanvasScaler scaler = statusRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            Text text = CreateText(statusRoot.transform, "StatusText", string.Empty, 26, TextAnchor.UpperLeft);
            RectTransform rect = text.rectTransform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(QuickPanelDefaultLeft, QuickPanelDefaultTop);
            rect.sizeDelta = new Vector2(600f, 40f);

            statusText = text;
            statusText.gameObject.SetActive(false);
        }

        private void BuildOverlayUi()
        {
            overlayRoot = new GameObject("FastSuperDashOverlayCanvas");
            overlayRoot.transform.SetParent(transform, false);

            Canvas canvas = overlayRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = OverlayCanvasSortOrder;

            CanvasScaler scaler = overlayRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            overlayRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = overlayRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(overlayRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("FastSuperDashPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/FastSuperDash".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = PanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            randomPantheonsContent = content;
            fastSuperDashContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "ModuleToggleRow",
                "Settings/FastSuperDash/Enable".Localize(),
                rowY,
                GetModuleEnabled,
                SetModuleEnabled,
                out moduleToggleValue,
                out moduleToggleIcon
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "InstantToggleRow",
                "Settings/instantSuperDash".Localize(),
                rowY,
                () => Modules.QoL.FastSuperDash.instantSuperDash,
                value =>
                {
                    Modules.QoL.FastSuperDash.instantSuperDash = value;
                    GodhomeQoL.SaveGlobalSettingsSafe();
                },
                out instantToggleValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "EverywhereToggleRow",
                "Settings/fastSuperDashEverywhere".Localize(),
                rowY,
                () => Modules.QoL.FastSuperDash.fastSuperDashEverywhere,
                value =>
                {
                    Modules.QoL.FastSuperDash.fastSuperDashEverywhere = value;
                    GodhomeQoL.SaveGlobalSettingsSafe();
                },
                out everywhereToggleValue
            );

            rowY += RowSpacing;
            float speedLabelY = rowY;
            CreateSpeedRow(content, speedLabelY, speedLabelY + 40f);
            lastY = speedLabelY + 40f;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "FastSuperDashResetRow", "Settings/FastSuperDash/Reset".Localize(), resetY, OnFastSuperDashResetDefaultsClicked);
            CreateButtonRow(panel.transform, "BackRow", "Back", backY, OnOverlayBackClicked);
        }

        private void BuildBossManipulateOverlayUi()
        {
            bossManipulateRoot = new GameObject("BossManipulateOverlayCanvas");
            bossManipulateRoot.transform.SetParent(transform, false);

            Canvas canvas = bossManipulateRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = BossManipulateCanvasSortOrder;

            CanvasScaler scaler = bossManipulateRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            bossManipulateRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = bossManipulateRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(bossManipulateRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("BossManipulatePanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(BossManipulatePanelWidth, BossManipulatePanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Categories/BossManipulate".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, (BossManipulatePanelHeight * 0.5f) - 70f);
            titleRect.sizeDelta = new Vector2(BossManipulatePanelWidth - 120f, 60f);

            float backY = GetFixedBackY(BossManipulatePanelHeight);
            float controlsTopY = Mathf.Max(BossManipulateOtherRoomsY, BossManipulateGlobalP5Y) + (RowHeight * 0.5f);
            RectTransform gridRoot = CreateBossManipulateGridRoot(panel.transform, titleRect, controlsTopY);
            BuildBossManipulateImageGrid(gridRoot);
            CreateToggleRow(
                panel.transform,
                "BossManipulateGlobalP5Row",
                "P5 HP",
                BossManipulateGlobalP5Y,
                GetBossManipulateGlobalP5Enabled,
                SetBossManipulateGlobalP5Enabled,
                out bossManipulateGlobalP5Value
            );
            CreateButtonRow(panel.transform, "BossManipulateOtherRoomsRow", "Other Rooms", BossManipulateOtherRoomsY, OnBossManipulateOtherRoomsClicked);
            CreateButtonRow(panel.transform, "BossManipulateResetRow", "Reset Default", BossManipulateResetY, OnBossManipulateResetAllClicked);
            CreateButtonRow(panel.transform, "BossManipulateBackRow", "Back", backY, OnBossManipulateBackClicked);
            CreateBossManipulateResetConfirm(panel.transform);
            SetBossManipulateResetConfirmVisible(false);
            RefreshBossManipulateCardVisuals();
            RefreshBossManipulateGlobalUi();
        }

        private static RectTransform CreateBossManipulateGridRoot(Transform panel, RectTransform titleRect, float controlsTopY)
        {
            float titleBottomY = titleRect.anchoredPosition.y - (titleRect.sizeDelta.y * 0.5f);
            float gridTopY = titleBottomY - 20f;
            float gridBottomY = controlsTopY + 18f;

            GameObject gridObj = new("BossManipulateGrid");
            gridObj.transform.SetParent(panel, false);

            RectTransform gridRect = gridObj.AddComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0.5f, 0.5f);
            gridRect.anchorMax = new Vector2(0.5f, 0.5f);
            gridRect.pivot = new Vector2(0.5f, 0.5f);
            gridRect.anchoredPosition = new Vector2(0f, (gridTopY + gridBottomY) * 0.5f);
            gridRect.sizeDelta = new Vector2(BossManipulatePanelWidth - 70f, Mathf.Max(240f, gridTopY - gridBottomY));
            return gridRect;
        }

        private void BuildBossManipulateImageGrid(RectTransform gridRoot)
        {
            bossManipulateCards.Clear();

            var cards = new (string Label, string ImageFile, Type ModuleType, Action OnClick)[]
            {
                ("Gruz Mother", "Gruz Mother.png", typeof(Modules.BossChallenge.GruzMotherHelper), OnBossManipulateGruzClicked),
                ("Vengefly King", "Vengefly King.png", typeof(Modules.BossChallenge.VengeflyKing), OnBossManipulateVengeflyKingClicked),
                ("Brooding Mawlek", "Brooding Mawlek.png", typeof(Modules.BossChallenge.BroodingMawlekHelper), OnBossManipulateMawlekClicked),
                ("False Knight", "False Knight.png", typeof(Modules.BossChallenge.FalseKnightHelper), OnBossManipulateFalseKnightClicked),
                ("Failed Champion", "Failed Champion.png", typeof(Modules.BossChallenge.FailedChampionHelper), OnBossManipulateFailedChampionClicked),
                ("Hornet Protector", "Hornet Protector.png", typeof(Modules.BossChallenge.HornetProtectorHelper), OnBossManipulateHornetClicked),
                ("Hornet Sentinel", "Hornet Sentinel.png", typeof(Modules.BossChallenge.HornetSentinelHelper), OnBossManipulateHornetSentinelClicked),
                ("Massive Moss Charger", "Massive Moss Charger.png", typeof(Modules.BossChallenge.MassiveMossChargerHelper), OnBossManipulateMassiveMossClicked),
                ("Flukemarm", "Flukemarm.png", typeof(Modules.BossChallenge.FlukemarmHelper), OnBossManipulateFlukemarmClicked),
                ("Mantis Lords", "Mantis Lords.png", typeof(Modules.BossChallenge.MantisLordHelper), OnBossManipulateMantisLordClicked),
                ("Sisters of Battle", "Sisters of Battle.png", typeof(Modules.BossChallenge.SisterOfBattleHelper), OnBossManipulateSisterOfBattleClicked),
                ("Oblobbles", "Oblobbles.png", typeof(Modules.BossChallenge.OblobblesHelper), OnBossManipulateOblobblesClicked),
                ("Hive Knight", "Hive Knight.png", typeof(Modules.BossChallenge.HiveKnightHelper), OnBossManipulateHiveKnightClicked),
                ("Broken Vessel", "Broken Vessel.png", typeof(Modules.BossChallenge.BrokenVesselHelper), OnBossManipulateBrokenVesselClicked),
                ("Lost Kin", "Lost Kin.png", typeof(Modules.BossChallenge.LostKinHelper), OnBossManipulateLostKinClicked),
                ("Nosk", "Nosk.png", typeof(Modules.BossChallenge.NoskHelper), OnBossManipulateNoskClicked),
                ("Winged Nosk", "Winged Nosk.png", typeof(Modules.BossChallenge.WingedNoskHelper), OnBossManipulateWingedNoskClicked),
                ("The Collector", "The Collector.png", typeof(Modules.CollectorPhases.CollectorPhases), OnBossManipulateCollectorClicked),
                ("God Tamer", "God Tamer.png", typeof(Modules.BossChallenge.GodTamerHelper), OnBossManipulateGodTamerClicked),
                ("Crystal Guardian", "Crystal Guardian.png", typeof(Modules.BossChallenge.CrystalGuardianHelper), OnBossManipulateCrystalGuardianClicked),
                ("Enraged Guardian", "Enraged Guardian.png", typeof(Modules.BossChallenge.EnragedGuardianHelper), OnBossManipulateEnragedGuardianClicked),
                ("Uumuu", "Uumuu.png", typeof(Modules.BossChallenge.UumuuHelper), OnBossManipulateUumuuClicked),
                ("Traitor Lord", "Traitor Lord.png", typeof(Modules.BossChallenge.TraitorLordHelper), OnBossManipulateTraitorLordClicked),
                ("Grey Prince Zote", "Grey Prince Zote.png", typeof(Modules.BossChallenge.ZoteHelper), OnBossManipulateZoteClicked),
                ("Soul Warrior", "Soul Warrior.png", typeof(Modules.BossChallenge.SoulWarriorHelper), OnBossManipulateSoulWarriorClicked),
                ("Soul Master", "Soul Master.png", typeof(Modules.BossChallenge.SoulMasterHelper), OnBossManipulateSoulMasterClicked),
                ("Soul Tyrant", "Soul Tyrant.png", typeof(Modules.BossChallenge.SoulTyrantHelper), OnBossManipulateSoulTyrantClicked),
                ("Dung Defender", "Dung Defender.png", typeof(Modules.BossChallenge.DungDefenderHelper), OnBossManipulateDungDefenderClicked),
                ("White Defender", "White Defender.png", typeof(Modules.BossChallenge.WhiteDefenderHelper), OnBossManipulateWhiteDefenderClicked),
                ("Watcher Knight", "Watcher Knight.png", typeof(Modules.BossChallenge.WatcherKnightHelper), OnBossManipulateWatcherKnightClicked),
                ("No Eyes", "No Eyes.png", typeof(Modules.BossChallenge.NoEyesHelper), OnBossManipulateNoEyesClicked),
                ("Marmu", "Marmu.png", typeof(Modules.BossChallenge.MarmuHelper), OnBossManipulateMarmuClicked),
                ("Xero", "Xero.png", typeof(Modules.BossChallenge.XeroHelper), OnBossManipulateXeroClicked),
                ("Markoth", "Markoth.png", typeof(Modules.BossChallenge.MarkothHelper), OnBossManipulateMarkothClicked),
                ("Galien", "Galien.png", typeof(Modules.BossChallenge.GalienHelper), OnBossManipulateGalienClicked),
                ("Gorb", "Gorb.png", typeof(Modules.BossChallenge.GorbHelper), OnBossManipulateGorbClicked),
                ("Elder Hu", "Elder Hu.png", typeof(Modules.BossChallenge.ElderHuHelper), OnBossManipulateElderHuClicked),
                ("Oro & Mato", "Oro & Mato.png", typeof(Modules.BossChallenge.OroMatoHelper), OnBossManipulateOroMatoClicked),
                ("Paintmaster Sheo", "Paintmaster Sheo.png", typeof(Modules.BossChallenge.PaintmasterSheoHelper), OnBossManipulatePaintmasterSheoClicked),
                ("Nailsage Sly", "Great Nailsage Sly.png", typeof(Modules.BossChallenge.NailsageSlyHelper), OnBossManipulateNailsageSlyClicked),
                ("Pure Vessel", "Pure Vessel.png", typeof(Modules.BossChallenge.PureVesselHelper), OnBossManipulatePureVesselClicked),
                ("Grimm", "Grimm.png", typeof(Modules.BossChallenge.TroupeMasterGrimmHelper), OnBossManipulateTroupeMasterGrimmClicked),
                ("Nightmare King", "Nightmare King.png", typeof(Modules.BossChallenge.NightmareKingGrimmHelper), OnBossManipulateNightmareKingGrimmClicked),
                ("Absolute Radiance", "Absolute Radiance.png", typeof(Modules.BossChallenge.AbsoluteRadianceHelper), OnBossManipulateAbsoluteRadianceClicked),
            };

            const int columns = 11;
            const float spacingX = 10f;
            const float spacingY = 12f;
            int rows = Mathf.CeilToInt(cards.Length / (float)columns);

            float gridWidth = gridRoot.sizeDelta.x;
            float gridHeight = gridRoot.sizeDelta.y;
            float cardWidth = (gridWidth - ((columns - 1) * spacingX)) / columns;
            float cardHeight = (gridHeight - ((rows - 1) * spacingY)) / rows;

            float totalWidth = (cardWidth * columns) + ((columns - 1) * spacingX);
            float totalHeight = (cardHeight * rows) + ((rows - 1) * spacingY);
            float startX = -(totalWidth * 0.5f) + (cardWidth * 0.5f);
            float startY = (totalHeight * 0.5f) - (cardHeight * 0.5f);

            for (int index = 0; index < cards.Length; index++)
            {
                int row = index / columns;
                int column = index % columns;
                float x = startX + (column * (cardWidth + spacingX));
                float y = startY - (row * (cardHeight + spacingY));
                CreateBossManipulateImageCard(
                    gridRoot,
                    $"BossManipulateCard{index + 1:D2}",
                    cards[index].Label,
                    cards[index].ImageFile,
                    cards[index].ModuleType,
                    x,
                    y,
                    cardWidth,
                    cardHeight,
                    cards[index].OnClick);
            }
        }

        private void CreateBossManipulateImageCard(
            Transform parent,
            string name,
            string label,
            string imageFile,
            Type moduleType,
            float x,
            float y,
            float width,
            float height,
            Action onClick)
        {
            GameObject card = new(name);
            card.transform.SetParent(parent, false);

            RectTransform cardRect = card.AddComponent<RectTransform>();
            cardRect.anchorMin = new Vector2(0.5f, 0.5f);
            cardRect.anchorMax = new Vector2(0.5f, 0.5f);
            cardRect.pivot = new Vector2(0.5f, 0.5f);
            cardRect.anchoredPosition = new Vector2(x, y);
            cardRect.sizeDelta = new Vector2(width, height);
            CanvasGroup group = card.AddComponent<CanvasGroup>();

            Image cardImage = card.AddComponent<Image>();
            cardImage.color = new Color(1f, 1f, 1f, 0.07f);

            Button button = card.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = cardImage;
            button.onClick.AddListener(() => onClick());

            RowHighlight highlight = CreateRowHighlight(card, cardImage);
            AttachRowHighlight(card, highlight);

            const float labelHeight = 30f;
            float imageHeight = Mathf.Max(50f, height - labelHeight - 10f);
            GameObject imageObj = new("Image");
            imageObj.transform.SetParent(card.transform, false);

            RectTransform imageRect = imageObj.AddComponent<RectTransform>();
            imageRect.anchorMin = new Vector2(0.5f, 0.5f);
            imageRect.anchorMax = new Vector2(0.5f, 0.5f);
            imageRect.pivot = new Vector2(0.5f, 0.5f);
            imageRect.anchoredPosition = new Vector2(0f, (height * 0.5f) - (imageHeight * 0.5f) - 4f);
            imageRect.sizeDelta = new Vector2(width - 12f, imageHeight);

            Image image = imageObj.AddComponent<Image>();
            image.raycastTarget = false;
            image.preserveAspect = true;

            Sprite? sprite = LoadBossManipulateIconSprite(imageFile, $"BossManipulate_{label}");
            if (sprite != null)
            {
                image.sprite = sprite;
                image.color = Color.white;
            }
            else
            {
                image.color = new Color(1f, 1f, 1f, 0.12f);
                LogDebug($"QuickMenu: missing Boss Manipulate image {imageFile}");
            }

            Text nameText = CreateText(card.transform, "Label", label, 17, TextAnchor.MiddleCenter);
            RectTransform labelRect = nameText.rectTransform;
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(1f, 0f);
            labelRect.pivot = new Vector2(0.5f, 0f);
            labelRect.anchoredPosition = new Vector2(0f, 2f);
            labelRect.sizeDelta = new Vector2(0f, labelHeight);

            bossManipulateCards.Add(new BossManipulateCardVisual(group, moduleType));
        }

        private void CreateBossManipulateResetConfirm(Transform parent)
        {
            bossManipulateResetConfirmRoot = new GameObject("BossManipulateResetConfirm");
            bossManipulateResetConfirmRoot.transform.SetParent(parent, false);

            RectTransform rootRect = bossManipulateResetConfirmRoot.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            Image dim = bossManipulateResetConfirmRoot.AddComponent<Image>();
            dim.color = new Color(0f, 0f, 0f, 0.35f);

            CanvasGroup group = bossManipulateResetConfirmRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dialogObj = new("Dialog");
            dialogObj.transform.SetParent(bossManipulateResetConfirmRoot.transform, false);
            RectTransform dialogRect = dialogObj.AddComponent<RectTransform>();
            dialogRect.anchorMin = new Vector2(0.5f, 0.5f);
            dialogRect.anchorMax = new Vector2(0.5f, 0.5f);
            dialogRect.pivot = new Vector2(0.5f, 0.5f);
            dialogRect.anchoredPosition = Vector2.zero;
            dialogRect.sizeDelta = new Vector2(740f, 340f);

            Image dialogImage = dialogObj.AddComponent<Image>();
            dialogImage.color = OverlayPanelColor;

            Text label = CreateText(dialogObj.transform, "Label", "Do you really want to reset everything?", 24, TextAnchor.MiddleCenter);
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            RectTransform labelRect = label.rectTransform;
            labelRect.anchorMin = new Vector2(0.5f, 1f);
            labelRect.anchorMax = new Vector2(0.5f, 1f);
            labelRect.pivot = new Vector2(0.5f, 1f);
            labelRect.anchoredPosition = new Vector2(0f, -28f);
            labelRect.sizeDelta = new Vector2(680f, 110f);

            CreateButtonRow(dialogObj.transform, "BossManipulateResetYesRow", "Yes", -70f, OnBossManipulateResetConfirmYes);
            CreateButtonRow(dialogObj.transform, "BossManipulateResetNoRow", "No", -130f, OnBossManipulateResetConfirmNo);
        }

        private void SetBossManipulateResetConfirmVisible(bool value)
        {
            bossManipulateResetConfirmVisible = value;
            if (bossManipulateResetConfirmRoot != null)
            {
                bossManipulateResetConfirmRoot.SetActive(value);
            }
        }

        private void BuildCollectorOverlayUi()
        {
            collectorRoot = new GameObject("CollectorPhasesOverlayCanvas");
            collectorRoot.transform.SetParent(transform, false);

            Canvas canvas = collectorRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = CollectorCanvasSortOrder;

            CanvasScaler scaler = collectorRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            collectorRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = collectorRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(collectorRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("CollectorPhasesPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(CollectorPanelWidth, CollectorPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/CollectorPhases".Localize(), CollectorTitleFontSize, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 420f);
            titleRect.sizeDelta = new Vector2(CollectorRowWidth, 60f);

            float panelHeight = CollectorPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, CollectorPanelWidth, panelHeight, topOffset, bottomOffset);
            collectorContent = content;

            float rowY = GetRowStartY(panelHeight, CollectorRowStartY, topOffset);
            float lastY = rowY;
            CreateCollectorToggleRowWithIcon(
                content,
                "CollectorModuleToggleRow",
                "Settings/CollectorHelper/Enable".Localize(),
                rowY,
                GetCollectorPhasesEnabled,
                SetCollectorPhasesEnabled,
                out collectorModuleToggleValue,
                out collectorModuleToggleIcon
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;
            CreateCollectorToggleRow(
                content,
                "QolCollectorRoarRow",
                "Modules/CollectorRoarMute".Localize(),
                rowY,
                GetCollectorRoarEnabled,
                SetCollectorRoarEnabled,
                out qolCollectorRoarValue
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;
            CreateCollectorAdjustRow(
                content,
                "CollectorPhaseRow",
                "Settings/CollectorPhases/CollectorPhase".Localize(),
                rowY,
                () => Modules.CollectorPhases.CollectorPhases.collectorPhase,
                SetCollectorPhase,
                1,
                3,
                1,
                out collectorPhaseValue
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;
            CreateCollectorToggleRow(
                content,
                "CollectorImmortalRow",
                "Settings/CollectorPhases/CollectorImmortal".Localize(),
                rowY,
                () => Modules.CollectorPhases.CollectorPhases.CollectorImmortal,
                SetCollectorImmortalEnabled,
                out collectorImmortalValue
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;
            CreateCollectorToggleRow(
                content,
                "IgnoreInitialJarLimitRow",
                "Settings/CollectorPhases/IgnoreInitialJarLimit".Localize(),
                rowY,
                () => Modules.CollectorPhases.CollectorPhases.IgnoreInitialJarLimit,
                SetCollectorIgnoreInitialJarLimitEnabled,
                out ignoreInitialJarLimitValue
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;
            CreateCollectorToggleRow(
                content,
                "UseCustomPhase2ThresholdRow",
                "Settings/CollectorPhases/UseCustomPhase2Threshold".Localize(),
                rowY,
                () => Modules.CollectorPhases.CollectorPhases.UseCustomPhase2Threshold,
                SetCollectorUseCustomPhase2ThresholdEnabled,
                out useCustomPhase2ThresholdValue
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;
            CreateCollectorAdjustInputRow(
                content,
                "CustomPhase2ThresholdRow",
                "Settings/CollectorPhases/CustomPhase2Threshold".Localize(),
                rowY,
                () => Modules.CollectorPhases.CollectorPhases.CustomPhase2Threshold,
                SetCollectorCustomPhase2Threshold,
                1,
                99999,
                1,
                out customPhase2ThresholdField
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;
            CreateCollectorToggleRow(
                content,
                "UseMaxHpRow",
                "Settings/CollectorPhases/UseMaxHP".Localize(),
                rowY,
                () => Modules.CollectorPhases.CollectorPhases.UseMaxHP,
                SetCollectorUseMaxHpEnabled,
                out useMaxHpValue
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;
            CreateCollectorAdjustInputRow(
                content,
                "CollectorMaxHpRow",
                "Settings/CollectorPhases/CollectorMaxHP".Localize(),
                rowY,
                () => Modules.CollectorPhases.CollectorPhases.collectorMaxHP,
                SetCollectorMaxHp,
                100,
                99999,
                1,
                out collectorMaxHpField
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;
            CreateCollectorAdjustInputRow(
                content,
                "BuzzerHpRow",
                "Settings/CollectorPhases/BuzzerHP".Localize(),
                rowY,
                () => Modules.CollectorPhases.CollectorPhases.buzzerHP,
                SetCollectorBuzzerHp,
                1,
                9999,
                1,
                out buzzerHpField
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;
            CreateCollectorToggleRow(
                content,
                "SpawnBuzzerRow",
                "Settings/CollectorPhases/SpawnBuzzer".Localize(),
                rowY,
                () => Modules.CollectorPhases.CollectorPhases.spawnBuzzer,
                SetCollectorSpawnBuzzerEnabled,
                out spawnBuzzerValue
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;
            CreateCollectorAdjustInputRow(
                content,
                "RollerHpRow",
                "Settings/CollectorPhases/RollerHP".Localize(),
                rowY,
                () => Modules.CollectorPhases.CollectorPhases.rollerHP,
                SetCollectorRollerHp,
                1,
                9999,
                1,
                out rollerHpField
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;
            CreateCollectorToggleRow(
                content,
                "SpawnRollerRow",
                "Settings/CollectorPhases/SpawnRoller".Localize(),
                rowY,
                () => Modules.CollectorPhases.CollectorPhases.spawnRoller,
                SetCollectorSpawnRollerEnabled,
                out spawnRollerValue
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;
            CreateCollectorAdjustInputRow(
                content,
                "SpitterHpRow",
                "Settings/CollectorPhases/SpitterHP".Localize(),
                rowY,
                () => Modules.CollectorPhases.CollectorPhases.spitterHP,
                SetCollectorSpitterHp,
                1,
                9999,
                1,
                out spitterHpField
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;
            CreateCollectorToggleRow(
                content,
                "SpawnSpitterRow",
                "Settings/CollectorPhases/SpawnSpitter".Localize(),
                rowY,
                () => Modules.CollectorPhases.CollectorPhases.spawnSpitter,
                SetCollectorSpawnSpitterEnabled,
                out spawnSpitterValue
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;
            CreateCollectorToggleRow(
                content,
                "DisableSummonLimitRow",
                "Settings/CollectorPhases/DisableSummonLimit".Localize(),
                rowY,
                () => Modules.CollectorPhases.CollectorPhases.DisableSummonLimit,
                SetCollectorDisableSummonLimitEnabled,
                out disableSummonLimitValue
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;
            CreateCollectorAdjustInputRow(
                content,
                "CustomSummonLimitRow",
                "Settings/CollectorPhases/CustomSummonLimit".Localize(),
                rowY,
                () => Modules.CollectorPhases.CollectorPhases.CustomSummonLimit,
                SetCollectorCustomSummonLimit,
                2,
                999,
                1,
                out customSummonLimitField
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, CollectorRowHeight);
            CreateButtonRow(panel.transform, "CollectorResetRow", "Settings/CollectorPhases/Reset".Localize(), resetY, OnCollectorResetClicked);
            CreateButtonRow(panel.transform, "CollectorBackRow", "Back", backY, OnCollectorBackClicked);
        }

        private void BuildFastReloadOverlayUi()
        {
            fastReloadRoot = new GameObject("FastReloadOverlayCanvas");
            fastReloadRoot.transform.SetParent(transform, false);

            Canvas canvas = fastReloadRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = FastReloadCanvasSortOrder;

            CanvasScaler scaler = fastReloadRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            fastReloadRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = fastReloadRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(fastReloadRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("FastReloadPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/FastReload".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = PanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            fastReloadContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "FastReloadToggleRow",
                "Settings/FastReload/Enable".Localize(),
                rowY,
                GetFastReloadEnabled,
                SetFastReloadEnabled,
                out fastReloadToggleValue,
                out fastReloadToggleIcon
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateKeybindRow(
                content,
                "ReloadKeyRow",
                "Settings/reloadBossKey".Localize(),
                rowY,
                () => FormatKeyLabel(GetReloadKey()),
                StartReloadRebind,
                out reloadKeyValue
            );

            lastY = rowY;
            rowY += RowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "FastReloadResetRow", "Settings/FastReload/Reset".Localize(), resetY, OnFastReloadResetDefaultsClicked);
            CreateButtonRow(panel.transform, "FastReloadBackRow", "Back", backY, OnFastReloadBackClicked);
        }

        private void BuildDreamshieldOverlayUi()
        {
            dreamshieldRoot = new GameObject("DreamshieldOverlayCanvas");
            dreamshieldRoot.transform.SetParent(transform, false);

            Canvas canvas = dreamshieldRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = DreamshieldCanvasSortOrder;

            CanvasScaler scaler = dreamshieldRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            dreamshieldRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = dreamshieldRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(dreamshieldRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("DreamshieldPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "DreamshieldSettings".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = PanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            dreamshieldContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "DreamshieldToggleRow",
                "Settings/DreamshieldStartAngle/Enable".Localize(),
                rowY,
                GetDreamshieldEnabled,
                SetDreamshieldEnabled,
                out dreamshieldToggleValue,
                out dreamshieldToggleIcon
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateFloatSliderRow(
                content,
                "DreamshieldDelayRow",
                "Settings/DreamshieldStartAngle/rotationDelay".Localize(),
                rowY,
                rowY + 40f,
                0f,
                10f,
                Modules.QoL.DreamshieldStartAngle.rotationDelay,
                2,
                out dreamshieldDelayValue,
                out dreamshieldDelaySlider
            );
            if (dreamshieldDelaySlider != null)
            {
                dreamshieldDelaySlider.onValueChanged.AddListener(OnDreamshieldDelayChanged);
            }

            lastY = rowY + 40f;
            rowY += RowSpacing + 40f;
            CreateFloatSliderRow(
                content,
                "DreamshieldSpeedRow",
                "Settings/DreamshieldStartAngle/rotationSpeed".Localize(),
                rowY,
                rowY + 40f,
                0f,
                10f,
                Modules.QoL.DreamshieldStartAngle.rotationSpeed,
                2,
                out dreamshieldSpeedValue,
                out dreamshieldSpeedSlider
            );
            if (dreamshieldSpeedSlider != null)
            {
                dreamshieldSpeedSlider.onValueChanged.AddListener(OnDreamshieldSpeedChanged);
            }

            lastY = rowY + 40f;
            rowY += RowSpacing + 40f;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "DreamshieldResetRow", "Settings/DreamshieldStartAngle/Reset".Localize(), resetY, OnDreamshieldResetDefaultsClicked);
            CreateButtonRow(panel.transform, "DreamshieldBackRow", "Back", backY, OnDreamshieldBackClicked);
        }

        private void BuildShowHpOnDeathOverlayUi()
        {
            showHpOnDeathRoot = new GameObject("ShowHPOnDeathOverlayCanvas");
            showHpOnDeathRoot.transform.SetParent(transform, false);

            Canvas canvas = showHpOnDeathRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = ShowHpOnDeathCanvasSortOrder;

            CanvasScaler scaler = showHpOnDeathRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            showHpOnDeathRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = showHpOnDeathRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(showHpOnDeathRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("ShowHPOnDeathPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "ShowHPOnDeath".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = PanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            showHpOnDeathContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "ShowHPOnDeathGlobalRow",
                "ShowHPOnDeath/Enable".Localize(),
                rowY,
                GetShowHpOnDeathEnabled,
                SetShowHpOnDeathEnabled,
                out showHpGlobalValue,
                out showHpGlobalIcon
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "ShowHPOnDeathShowPbRow",
                "ShowHPOnDeath/ShowPB".Localize(),
                rowY,
                () => ShowHpSettings.ShowPB,
                value => ShowHpSettings.ShowPB = value,
                out showHpShowPbValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "ShowHPOnDeathAutoHideRow",
                "ShowHPOnDeath/AutoHide".Localize(),
                rowY,
                () => ShowHpSettings.HideAfter10Sec,
                value => ShowHpSettings.HideAfter10Sec = value,
                out showHpAutoHideValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateKeybindRow(
                content,
                "ShowHPOnDeathKeyRow",
                "ShowHPOnDeath/HudToggleKey".Localize(),
                rowY,
                GetShowHpBindingLabel,
                StartShowHpOnDeathRebind,
                out showHpHudToggleKeyValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateFloatSliderRow(
                content,
                "ShowHPOnDeathFadeRow",
                "ShowHPOnDeath/HudFadeTime".Localize(),
                rowY,
                rowY + 40f,
                1f,
                10f,
                ShowHpSettings.HudFadeSeconds,
                1,
                out showHpFadeValue,
                out showHpFadeSlider
            );
            if (showHpFadeSlider != null)
            {
                showHpFadeSlider.onValueChanged.AddListener(OnShowHpFadeChanged);
            }

            lastY = rowY + 40f;
            rowY += RowSpacing + 40f;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "ShowHPOnDeathResetRow", "ShowHPOnDeath/Reset".Localize(), resetY, OnShowHpOnDeathResetDefaultsClicked);
            CreateButtonRow(panel.transform, "ShowHPOnDeathBackRow", "Back", backY, OnShowHpOnDeathBackClicked);
        }

        private void BuildMaskDamageOverlayUi()
        {
            maskDamageRoot = new GameObject("MaskDamageOverlayCanvas");
            maskDamageRoot.transform.SetParent(transform, false);

            Canvas canvas = maskDamageRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = MaskDamageCanvasSortOrder;

            CanvasScaler scaler = maskDamageRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            maskDamageRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = maskDamageRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(maskDamageRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("MaskDamagePanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/MaskDamage".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 420f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = PanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            maskDamageContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "MaskDamageEnableRow",
                "Settings/MaskDamage/Enable".Localize(),
                rowY,
                GetMaskDamageEnabled,
                SetMaskDamageEnabled,
                out maskDamageToggleValue,
                out maskDamageToggleIcon
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "MaskDamageShowUiRow",
                "Settings/MaskDamage/ShowUI".Localize(),
                rowY,
                GetMaskDamageUiVisible,
                SetMaskDamageUiVisible,
                out maskDamageShowUiValue
            );
            maskDamageShowUiIcon = null;

            lastY = rowY;
            rowY += RowSpacing;
            CreateAdjustFloatInputRow(
                content,
                "MaskDamageMultiplierRow",
                "Settings/MaskDamage/Multiplier".Localize(),
                rowY,
                MaskDamage.GetMultiplier,
                MaskDamage.SetMultiplier,
                0.5f,
                999f,
                1f,
                out maskDamageMultiplierField
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateKeybindRow(
                content,
                "MaskDamageToggleUiRow",
                "Settings/MaskDamage/ToggleUI".Localize(),
                rowY,
                GetMaskDamageToggleUiKeyLabel,
                StartMaskDamageUiRebind,
                out maskDamageToggleUiKeyValue
            );

            lastY = rowY;
            rowY += RowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "MaskDamageResetRow", "Reset Defaults", resetY, OnMaskDamageResetDefaultsClicked);
            CreateButtonRow(panel.transform, "MaskDamageBackRow", "Back", backY, OnMaskDamageBackClicked);
        }

        private void BuildFreezeHitboxesOverlayUi()
        {
            freezeHitboxesRoot = new GameObject("FreezeHitboxesOverlayCanvas");
            freezeHitboxesRoot.transform.SetParent(transform, false);

            Canvas canvas = freezeHitboxesRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = FreezeHitboxesCanvasSortOrder;

            CanvasScaler scaler = freezeHitboxesRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            freezeHitboxesRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = freezeHitboxesRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(freezeHitboxesRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("FreezeHitboxesPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Freeze Hitboxes", 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = PanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            freezeHitboxesContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "FreezeHitboxesEnableRow",
                "Enable Freeze Hitboxes",
                rowY,
                GetFreezeHitboxesEnabled,
                SetFreezeHitboxesEnabled,
                out freezeHitboxesToggleValue,
                out freezeHitboxesToggleIcon
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateSelectRow(
                content,
                "FreezeHitboxesModeRow",
                "Freeze Mode",
                rowY,
                GetFreezeHitboxesModeLabel,
                ToggleFreezeHitboxesMode,
                out freezeHitboxesModeValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateKeybindRow(
                content,
                "FreezeHitboxesUnfreezeKeyRow",
                "Unfreeze Hotkey",
                rowY,
                GetFreezeHitboxesKeyLabel,
                StartFreezeHitboxesRebind,
                out freezeHitboxesUnfreezeKeyValue
            );

            lastY = rowY;
            rowY += RowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "FreezeHitboxesResetRow", "Reset Defaults", resetY, OnFreezeHitboxesResetDefaultsClicked);
            CreateButtonRow(panel.transform, "FreezeHitboxesBackRow", "Back", backY, OnFreezeHitboxesBackClicked);
        }

        private void BuildSpeedChangerOverlayUi()
        {
            speedChangerRoot = new GameObject("SpeedChangerOverlayCanvas");
            speedChangerRoot.transform.SetParent(transform, false);

            Canvas canvas = speedChangerRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = SpeedChangerCanvasSortOrder;

            CanvasScaler scaler = speedChangerRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            speedChangerRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = speedChangerRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(speedChangerRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("SpeedChangerPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, SpeedChangerPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "SpeedChanger".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 300f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);


            float panelHeight = SpeedChangerPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            speedChangerContent = content;

            float rowY = GetRowStartY(panelHeight, SpeedChangerRowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "SpeedChangerGlobalRow",
                "SpeedChanger/Enable".Localize(),
                rowY,
                () => SpeedChanger.globalSwitch,
                SetSpeedChangerGlobalSwitch,
                out speedChangerGlobalValue,
                out speedChangerGlobalIcon
            );

            lastY = rowY;
            rowY += SpeedChangerRowSpacing;
            CreateToggleRow(
                content,
                "SpeedChangerRestrictRow",
                "SpeedChanger/RestrictRooms".Localize(),
                rowY,
                () => SpeedChanger.restrictToggleToRooms,
                value => SpeedChanger.restrictToggleToRooms = value,
                out speedChangerRestrictValue
            );

            lastY = rowY;
            rowY += SpeedChangerRowSpacing;
            CreateKeybindRow(
                content,
                "SpeedChangerToggleKeyRow",
                "SpeedChanger/ToggleKey".Localize(),
                rowY,
                () => FormatSpeedChangerKeyLabel(SpeedChanger.toggleKeybind),
                StartSpeedChangerToggleRebind,
                out speedChangerToggleKeyValue
            );

            lastY = rowY;
            rowY += SpeedChangerRowSpacing;
            CreateToggleRow(
                content,
                "SpeedChangerUnlimitedRow",
                "SpeedChanger/UnlimitedSpeed".Localize(),
                rowY,
                () => SpeedChanger.unlimitedSpeed,
                value => SpeedChanger.unlimitedSpeed = value,
                out speedChangerUnlimitedValue
            );

            lastY = rowY;
            rowY += SpeedChangerRowSpacing;
            CreateKeybindRow(
                content,
                "SpeedChangerInputKeyRow",
                "SpeedChanger/InputKey".Localize(),
                rowY,
                () => FormatSpeedChangerKeyLabel(SpeedChanger.inputSpeedKeybind),
                StartSpeedChangerInputRebind,
                out speedChangerInputKeyValue
            );

            lastY = rowY;
            rowY += SpeedChangerRowSpacing;
            CreateCycleRow(
                content,
                "SpeedChangerDisplayRow",
                "SpeedChanger/DisplayStyle".Localize(),
                rowY,
                SpeedChangerDisplayOptions,
                () => SpeedChanger.displayStyle,
                ApplySpeedChangerDisplayStyle,
                out speedChangerDisplayValue
            );

            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "SpeedChangerResetRow", "SpeedChanger/Reset".Localize(), resetY, OnSpeedChangerResetDefaultsClicked);
            CreateButtonRow(panel.transform, "SpeedChangerBackRow", "Back", backY, OnSpeedChangerBackClicked);
        }

        private void BuildTeleportKitOverlayUi()
        {
            teleportKitRoot = new GameObject("TeleportKitOverlayCanvas");
            teleportKitRoot.transform.SetParent(transform, false);

            Canvas canvas = teleportKitRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = TeleportKitCanvasSortOrder;

            CanvasScaler scaler = teleportKitRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            teleportKitRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = teleportKitRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(teleportKitRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("TeleportKitPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "TeleportKit".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = PanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            teleportKitContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "TeleportKitToggleRow",
                "Settings/TeleportKit/Enable".Localize(),
                rowY,
                GetTeleportKitEnabled,
                SetTeleportKitEnabled,
                out teleportKitToggleValue,
                out teleportKitToggleIcon
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateKeybindRow(
                content,
                "TeleportKitMenuKeyRow",
                "TeleportKit/MenuHotkey".Localize(),
                rowY,
                GetTeleportKitMenuKeyLabel,
                StartTeleportKitMenuRebind,
                out teleportKitMenuKeyValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateKeybindRow(
                content,
                "TeleportKitSaveKeyRow",
                "TeleportKit/SaveHotkey".Localize(),
                rowY,
                GetTeleportKitSaveKeyLabel,
                StartTeleportKitSaveRebind,
                out teleportKitSaveKeyValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateKeybindRow(
                content,
                "TeleportKitTeleportKeyRow",
                "TeleportKit/TeleportHotkey".Localize(),
                rowY,
                GetTeleportKitTeleportKeyLabel,
                StartTeleportKitTeleportRebind,
                out teleportKitTeleportKeyValue
            );

            lastY = rowY;
            rowY += RowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "TeleportKitResetRow", "TeleportKit/Reset".Localize(), resetY, OnTeleportKitResetDefaultsClicked);
            CreateButtonRow(panel.transform, "TeleportKitBackRow", "Back", backY, OnTeleportKitBackClicked);
        }

        private void BuildBossChallengeOverlayUi()
        {
            bossChallengeRoot = new GameObject("BossChallengeOverlayCanvas");
            bossChallengeRoot.transform.SetParent(transform, false);

            Canvas canvas = bossChallengeRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = BossChallengeCanvasSortOrder;

            CanvasScaler scaler = bossChallengeRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            bossChallengeRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = bossChallengeRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(bossChallengeRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("BossChallengePanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, BossChallengePanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Categories/BossChallenge".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 420f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = BossChallengePanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            bossChallengeContent = content;

            float rowY = GetRowStartY(panelHeight, BossChallengeRowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "BossChallengeEnableRow",
                "Settings/BossChallenge/Enable".Localize(),
                rowY,
                GetBossChallengeMasterEnabled,
                SetBossChallengeMasterEnabled,
                out bossChallengeEnableValue,
                out bossChallengeEnableIcon
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossInfiniteChallengeRow",
                "Modules/InfiniteChallenge".Localize(),
                rowY,
                GetInfiniteChallengeEnabled,
                SetInfiniteChallengeEnabled,
                out bossInfiniteChallengeValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossRestartOnSuccessRow",
                "Settings/restartFightOnSuccess".Localize(),
                rowY,
                () => Modules.BossChallenge.InfiniteChallenge.restartFightOnSuccess,
                value => Modules.BossChallenge.InfiniteChallenge.restartFightOnSuccess = value,
                out bossRestartOnSuccessValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossRestartAndMusicRow",
                "Settings/restartFightAndMusic".Localize(),
                rowY,
                () => Modules.BossChallenge.InfiniteChallenge.restartFightAndMusic,
                value => Modules.BossChallenge.InfiniteChallenge.restartFightAndMusic = value,
                out bossRestartAndMusicValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateSelectRow(
                content,
                "QolCarefreeMelodyRow",
                "Modules/CarefreeMelodyReset".Localize(),
                rowY,
                GetCarefreeMelodyModeLabel,
                ToggleCarefreeMelodyMode,
                out qolCarefreeMelodyValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossForceArriveRow",
                "Modules/ForceArriveAnimation".Localize(),
                rowY,
                GetForceArriveAnimationEnabled,
                SetForceArriveAnimationEnabled,
                out bossForceArriveValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateCycleRow(
                content,
                "BossChallengeGpzRow",
                "Modules/ForceGreyPrinceEnterType".Localize(),
                rowY,
                BossChallengeGpzOptions,
                GetBossGpzIndex,
                ApplyBossGpzOption,
                out bossGpzValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossInfiniteGrimmRow",
                "Modules/InfiniteGrimmPufferfish".Localize(),
                rowY,
                GetInfiniteGrimmPufferfishEnabled,
                SetInfiniteGrimmPufferfishEnabled,
                out bossInfiniteGrimmValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossInfiniteRadianceRow",
                "Modules/InfiniteRadianceClimbing".Localize(),
                rowY,
                GetInfiniteRadianceClimbingEnabled,
                SetInfiniteRadianceClimbingEnabled,
                out bossInfiniteRadianceValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossSegmentedP5Row",
                "Modules/SegmentedP5".Localize(),
                rowY,
                GetSegmentedP5Enabled,
                SetSegmentedP5Enabled,
                out bossSegmentedP5Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossAddLifebloodRow",
                "Modules/AddLifeblood".Localize(),
                rowY,
                GetAddLifebloodEnabled,
                SetAddLifebloodEnabled,
                out bossAddLifebloodValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateAdjustInputRow(
                content,
                "BossLifebloodAmountRow",
                "Settings/lifebloodAmount".Localize(),
                rowY,
                () => Modules.BossChallenge.AddLifeblood.lifebloodAmount,
                value => Modules.BossChallenge.AddLifeblood.lifebloodAmount = Math.Max(0, Math.Min(99, value)),
                0,
                99,
                1,
                out bossLifebloodAmountField
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossAddSoulRow",
                "Modules/AddSoul".Localize(),
                rowY,
                GetAddSoulEnabled,
                SetAddSoulEnabled,
                out bossAddSoulValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateAdjustInputRow(
                content,
                "BossSoulAmountRow",
                "Settings/soulAmount".Localize(),
                rowY,
                () => Modules.BossChallenge.AddSoul.soulAmount,
                value => Modules.BossChallenge.AddSoul.soulAmount = Math.Max(0, Math.Min(999, value)),
                0,
                999,
                11,
                out bossSoulAmountField
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "BossChallengeResetRow", "BossChallenge/Reset".Localize(), resetY, OnBossChallengeResetDefaultsClicked);
            CreateButtonRow(panel.transform, "BossChallengeBackRow", "Back", backY, OnBossChallengeBackClicked);
        }

        private void BuildRandomPantheonsOverlayUi()
        {
            randomPantheonsRoot = new GameObject("RandomPantheonsOverlayCanvas");
            randomPantheonsRoot.transform.SetParent(transform, false);

            Canvas canvas = randomPantheonsRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = RandomPantheonsCanvasSortOrder;

            CanvasScaler scaler = randomPantheonsRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            randomPantheonsRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = randomPantheonsRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(randomPantheonsRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("RandomPantheonsPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/RandomPantheons".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = PanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            randomPantheonsContent = content;

            float rowY = GetRowStartY(panelHeight, GearSwitcherRowStartY, topOffset) + GearSwitcherTopPadding;
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "RandomPantheonsToggleRow",
                "Modules/RandomPantheons".Localize(),
                rowY,
                GetRandomPantheonsMasterEnabled,
                SetRandomPantheonsMasterEnabled,
                out randomPantheonsToggleValue,
                out randomPantheonsToggleIcon
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "RandomPantheonsP1Row",
                "Pantheon 1",
                rowY,
                GetRandomPantheonsP1Enabled,
                SetRandomPantheonsP1Enabled,
                out randomPantheonsP1Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "RandomPantheonsP2Row",
                "Pantheon 2",
                rowY,
                GetRandomPantheonsP2Enabled,
                SetRandomPantheonsP2Enabled,
                out randomPantheonsP2Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "RandomPantheonsP3Row",
                "Pantheon 3",
                rowY,
                GetRandomPantheonsP3Enabled,
                SetRandomPantheonsP3Enabled,
                out randomPantheonsP3Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "RandomPantheonsP4Row",
                "Pantheon 4",
                rowY,
                GetRandomPantheonsP4Enabled,
                SetRandomPantheonsP4Enabled,
                out randomPantheonsP4Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "RandomPantheonsP5Row",
                "Pantheon 5",
                rowY,
                GetRandomPantheonsP5Enabled,
                SetRandomPantheonsP5Enabled,
                out randomPantheonsP5Value
            );

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "RandomPantheonsResetRow", "Settings/RandomPantheons/Reset".Localize(), resetY, OnRandomPantheonsResetDefaultsClicked);
            CreateButtonRow(panel.transform, "RandomPantheonsBackRow", "Back", backY, OnRandomPantheonsBackClicked);
        }

        private void BuildTrueBossRushOverlayUi()
        {
            trueBossRushRoot = new GameObject("TrueBossRushOverlayCanvas");
            trueBossRushRoot.transform.SetParent(transform, false);

            Canvas canvas = trueBossRushRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = TrueBossRushCanvasSortOrder;

            CanvasScaler scaler = trueBossRushRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            trueBossRushRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = trueBossRushRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(trueBossRushRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("TrueBossRushPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/TrueBossRush".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = PanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            trueBossRushContent = content;

            float rowY = GetRowStartY(panelHeight, GearSwitcherRowStartY, topOffset) + GearSwitcherTopPadding;
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "TrueBossRushToggleRow",
                "Settings/TrueBossRush/Enable".Localize(),
                rowY,
                GetTrueBossRushMasterEnabled,
                SetTrueBossRushMasterEnabled,
                out trueBossRushToggleValue,
                out trueBossRushToggleIcon
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "TrueBossRushP1Row",
                "Pantheon 1",
                rowY,
                GetTrueBossRushP1Enabled,
                SetTrueBossRushP1Enabled,
                out trueBossRushP1Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "TrueBossRushP2Row",
                "Pantheon 2",
                rowY,
                GetTrueBossRushP2Enabled,
                SetTrueBossRushP2Enabled,
                out trueBossRushP2Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "TrueBossRushP3Row",
                "Pantheon 3",
                rowY,
                GetTrueBossRushP3Enabled,
                SetTrueBossRushP3Enabled,
                out trueBossRushP3Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "TrueBossRushP4Row",
                "Pantheon 4",
                rowY,
                GetTrueBossRushP4Enabled,
                SetTrueBossRushP4Enabled,
                out trueBossRushP4Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "TrueBossRushP5Row",
                "Pantheon 5",
                rowY,
                GetTrueBossRushP5Enabled,
                SetTrueBossRushP5Enabled,
                out trueBossRushP5Value
            );

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "TrueBossRushResetRow", "Settings/TrueBossRush/Reset".Localize(), resetY, OnTrueBossRushResetDefaultsClicked);
            CreateButtonRow(panel.transform, "TrueBossRushBackRow", "Back", backY, OnTrueBossRushBackClicked);
        }

        private void BuildCheatsOverlayUi()
        {
            cheatsRoot = new GameObject("CheatsOverlayCanvas");
            cheatsRoot.transform.SetParent(transform, false);

            Canvas canvas = cheatsRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = CheatsCanvasSortOrder;

            CanvasScaler scaler = cheatsRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            cheatsRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = cheatsRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(cheatsRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("CheatsPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/Cheats".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = PanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            cheatsContent = content;

            float rowY = GetRowStartY(panelHeight, GearSwitcherRowStartY, topOffset) + GearSwitcherTopPadding;
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "CheatsEnableRow",
                "Settings/Cheats/Enable".Localize(),
                rowY,
                GetCheatsMasterEnabled,
                SetCheatsMasterEnabled,
                out cheatsEnableValue,
                out cheatsEnableIcon
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "CheatsInfiniteSoulRow",
                "Settings/Cheats/InfiniteSoul".Localize(),
                rowY,
                GetCheatsInfiniteSoulEnabled,
                SetCheatsInfiniteSoulEnabled,
                out cheatsInfiniteSoulValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "CheatsInfiniteHpRow",
                "Settings/Cheats/InfiniteHp".Localize(),
                rowY,
                GetCheatsInfiniteHpEnabled,
                SetCheatsInfiniteHpEnabled,
                out cheatsInfiniteHpValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "CheatsInvincibilityRow",
                "Settings/Cheats/Invincibility".Localize(),
                rowY,
                GetCheatsInvincibilityEnabled,
                SetCheatsInvincibilityEnabled,
                out cheatsInvincibilityValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "CheatsNoclipRow",
                "Settings/Cheats/Noclip".Localize(),
                rowY,
                GetCheatsNoclipEnabled,
                SetCheatsNoclipEnabled,
                out cheatsNoclipValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateButtonRow(
                content,
                "CheatsKillAllRow",
                "Settings/Cheats/KillAll".Localize(),
                rowY,
                OnCheatsKillAllClicked
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateKeybindRow(
                content,
                "CheatsKillAllHotkeyRow",
                "Settings/Cheats/KillAllHotkey".Localize(),
                rowY,
                GetCheatsKillAllHotkeyLabel,
                StartCheatsKillAllRebind,
                out cheatsKillAllHotkeyValue
            );

            lastY = rowY;
            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "CheatsResetRow", "Settings/Cheats/Reset".Localize(), resetY, OnCheatsResetDefaultsClicked);
            CreateButtonRow(panel.transform, "CheatsBackRow", "Back", backY, OnCheatsBackClicked);
        }

        private void BuildAlwaysFuriousOverlayUi()
        {
            alwaysFuriousRoot = new GameObject("AlwaysFuriousOverlayCanvas");
            alwaysFuriousRoot.transform.SetParent(transform, false);

            Canvas canvas = alwaysFuriousRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = AlwaysFuriousCanvasSortOrder;

            CanvasScaler scaler = alwaysFuriousRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            alwaysFuriousRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = alwaysFuriousRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(alwaysFuriousRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("AlwaysFuriousPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/AlwaysFurious".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = PanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);

            float rowY = GetRowStartY(panelHeight, GearSwitcherRowStartY, topOffset) + GearSwitcherTopPadding;
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "AlwaysFuriousToggleRow",
                "Settings/AlwaysFurious/Enable".Localize(),
                rowY,
                GetAlwaysFuriousEnabled,
                SetAlwaysFuriousEnabled,
                out alwaysFuriousToggleValue,
                out alwaysFuriousToggleIcon
            );

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "AlwaysFuriousResetRow", "Settings/AlwaysFurious/Reset".Localize(), resetY, OnAlwaysFuriousResetDefaultsClicked);
            CreateButtonRow(panel.transform, "AlwaysFuriousBackRow", "Back", backY, OnAlwaysFuriousBackClicked);
        }

        private void UpdateGearSwitcherHeaderLayout()
        {
            if (gearSwitcherTitleRect != null)
            {
                gearSwitcherTitleRect.anchoredPosition = new Vector2(0f, GearSwitcherTitleY);
                gearSwitcherTitleRect.sizeDelta = new Vector2(RowWidth, 60f);
            }

            if (gearSwitcherHintRect != null)
            {
                gearSwitcherHintRect.anchoredPosition = new Vector2(0f, GearSwitcherHintY);
                gearSwitcherHintRect.sizeDelta = new Vector2(RowWidth, GearSwitcherHintHeight);
            }
        }

        private void BuildGearSwitcherOverlayUi()
        {
            gearSwitcherRoot = new GameObject("GearSwitcherOverlayCanvas");
            gearSwitcherRoot.transform.SetParent(transform, false);

            Canvas canvas = gearSwitcherRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = GearSwitcherCanvasSortOrder;

            CanvasScaler scaler = gearSwitcherRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            gearSwitcherRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = gearSwitcherRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(gearSwitcherRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("GearSwitcherPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/GearSwitcher".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            Text hint = CreateText(panel.transform, "Hint", "The mod effects apply as soon as you are standing on the ground", 22, TextAnchor.MiddleCenter);
            hint.color = new Color(1f, 1f, 1f, 0.75f);
            RectTransform hintRect = hint.rectTransform;
            hintRect.anchorMin = new Vector2(0.5f, 0.5f);
            hintRect.anchorMax = new Vector2(0.5f, 0.5f);
            hintRect.pivot = new Vector2(0.5f, 0.5f);
            hintRect.sizeDelta = new Vector2(RowWidth, GearSwitcherHintHeight);

            gearSwitcherTitleRect = titleRect;
            gearSwitcherHintRect = hintRect;
            UpdateGearSwitcherHeaderLayout();

            float panelHeight = PanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, hintRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            gearSwitcherContent = content;

            float rowY = GetRowStartY(panelHeight, GearSwitcherContentRowStartY, topOffset) + GearSwitcherContentTopPadding;
            float lastY = rowY;

            CreateToggleRowWithIcon(
                content,
                "GearSwitcherEnableRow",
                "Enable GearSwitcher",
                rowY,
                GetGearSwitcherEnabled,
                SetGearSwitcherEnabled,
                out gearSwitcherEnableValue,
                out gearSwitcherEnableIcon
            );

            lastY = rowY;
            rowY += GearSwitcherRowSpacing;

            CreateGearSwitcherSpellsRow(content, rowY);

            lastY = rowY;
            rowY += GearSwitcherRowSpacing + 10f;
            CreateGearSwitcherNailArtsRow(content, rowY);

            lastY = rowY;
            rowY += GearSwitcherRowSpacing;
            CreateGearSwitcherCloakRow(content, rowY);

            lastY = rowY;
            rowY += GearSwitcherRowSpacing;
            CreateGearSwitcherBindingsRow(content, rowY);

            lastY = rowY;
            rowY += GearSwitcherRowSpacing;
            CreateGearSwitcherHpMaskRow(content, rowY);

            lastY = rowY;
            rowY += GearSwitcherRowSpacing;
            CreateGearSwitcherSoulVesselRow(content, rowY);

            lastY = rowY;
            rowY += GearSwitcherRowSpacing;
            CreateGearSwitcherNaillessRow(content, rowY);

            lastY = rowY;
            float charmPromptSpacing = GearSwitcherRowSpacing + ((GearSwitcherCharmPromptRowHeight - RowHeight) * 0.5f);
            rowY += charmPromptSpacing;
            CreateGearSwitcherCharmPromptRow(content, rowY);

            lastY = rowY;
            rowY += charmPromptSpacing;
            CreateSelectRow(
                content,
                "GearSwitcherPresetRow",
                "GearSwitcher/SelectedPreset".Localize(),
                rowY,
                GetSelectedPresetLabel,
                OnGearSwitcherPresetClicked,
                out gearSwitcherSelectedPresetValue
            );

            lastY = rowY;
            rowY += GearSwitcherRowSpacing;
            CreateAdjustInputRow(
                content,
                "GearSwitcherNailDamageRow",
                "GearSwitcher/NailDamage".Localize(),
                rowY,
                () => GetSelectedPreset().NailDamage,
                SetSelectedPresetNailDamage,
                -99999,
                99999,
                4,
                out gearSwitcherNailDamageField
            );

            lastY = rowY;
            rowY += GearSwitcherRowSpacing;
            CreateReadOnlyValueRow(
                content,
                "GearSwitcherBaseNailDamageRow",
                "GearSwitcher/BaseNailDamage".Localize(),
                rowY,
                GetGearSwitcherBaseNailDamageDisplay,
                out gearSwitcherBaseNailDamageValue
            );

            lastY = rowY;
            rowY += GearSwitcherRowSpacing;
            CreateAdjustInputRow(
                content,
                "GearSwitcherCharmSlotsRow",
                "GearSwitcher/CharmSlots".Localize(),
                rowY,
                () => GetSelectedPreset().CharmSlots,
                SetSelectedPresetCharmSlots,
                3,
                999,
                1,
                out gearSwitcherCharmSlotsField
            );
            lastY = rowY;

            rowY += GearSwitcherRowSpacing;
            CreateAdjustInputRow(
                content,
                "GearSwitcherMainSoulGainRow",
                "GearSwitcher/MainSoulGain".Localize(),
                rowY,
                () => GetSelectedPreset().MainSoulGain,
                SetSelectedPresetMainSoulGain,
                0,
                198,
                11,
                out gearSwitcherMainSoulGainField
            );
            lastY = rowY;

            rowY += GearSwitcherRowSpacing;
            CreateAdjustInputRow(
                content,
                "GearSwitcherReserveSoulGainRow",
                "GearSwitcher/ReserveSoulGain".Localize(),
                rowY,
                () => GetSelectedPreset().ReserveSoulGain,
                SetSelectedPresetReserveSoulGain,
                0,
                198,
                6,
                out gearSwitcherReserveSoulGainField
            );
            lastY = rowY;

            // Move ability toggles removed; handled by icons above.

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "GearSwitcherResetRow", "GearSwitcher/Reset".Localize(), resetY, OnGearSwitcherResetDefaultsClicked);
            CreateButtonRow(panel.transform, "GearSwitcherBackRow", "Back", backY, OnGearSwitcherBackClicked);

            CreateGearSwitcherResetConfirm(panel.transform);
            SetGearSwitcherResetConfirmVisible(false);
        }

        private void BuildGearSwitcherCharmCostOverlayUi()
        {
            gearSwitcherCharmCostGlows.Clear();
            gearSwitcherCharmCostHighlightEntries.Clear();

            gearSwitcherCharmCostRoot = new GameObject("GearSwitcherCharmCostOverlayCanvas");
            gearSwitcherCharmCostRoot.transform.SetParent(transform, false);

            Canvas canvas = gearSwitcherCharmCostRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = GearSwitcherCharmCostCanvasSortOrder;

            CanvasScaler scaler = gearSwitcherCharmCostRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            gearSwitcherCharmCostRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = gearSwitcherCharmCostRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(gearSwitcherCharmCostRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("GearSwitcherCharmCostPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "GearSwitcher/CharmCosts".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = PanelHeight;
            float backY = GetFixedBackY(panelHeight);
            float buttonSpacing = 10f;
            float resetY = backY + (ButtonRowHeight + buttonSpacing) * 2f;
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;

            Transform row = CreateGearSwitcherCharmCostsRow(content, "GearSwitcherCharmCostsRow1", rowY);
            CreateCharmCostColumn(row, "WaywardCompass", CharmCostColumnX(0), "Wayward Compass.png", "WaywardCompass", ref gearSwitcherWaywardCostValue, AdjustWaywardCompassCost);
            CreateCharmCostColumn(row, "GatheringSwarm", CharmCostColumnX(1), "Gathering Swarm.png", "GatheringSwarm", ref gearSwitcherGatheringCostValue, AdjustGatheringSwarmCost);
            CreateCharmCostColumn(row, "StalwartShell", CharmCostColumnX(2), "Stalwart Shell.png", "StalwartShell", ref gearSwitcherStalwartShellCostValue, AdjustStalwartShellCost);
            CreateCharmCostColumn(row, "SoulCatcher", CharmCostColumnX(3), "Soul Catcher.png", "SoulCatcher", ref gearSwitcherSoulCatcherCostValue, AdjustSoulCatcherCost);
            CreateCharmCostColumn(row, "ShamanStone", CharmCostColumnX(4), "Shaman Stone.png", "ShamanStone", ref gearSwitcherShamanStoneCostValue, AdjustShamanStoneCost);
            lastY = rowY;

            rowY += GearSwitcherCharmCostRowSpacing;
            row = CreateGearSwitcherCharmCostsRow(content, "GearSwitcherCharmCostsRow2", rowY);
            CreateCharmCostColumn(row, "SoulEater", CharmCostColumnX(0), "Soul Eater.png", "SoulEater", ref gearSwitcherSoulEaterCostValue, AdjustSoulEaterCost);
            CreateCharmCostColumn(row, "Dashmaster", CharmCostColumnX(1), "Dashmaster.png", "Dashmaster", ref gearSwitcherDashmasterCostValue, AdjustDashmasterCost);
            CreateCharmCostColumn(row, "Sprintmaster", CharmCostColumnX(2), "Sprintmaster.png", "Sprintmaster", ref gearSwitcherSprintmasterCostValue, AdjustSprintmasterCost);
            CreateCharmCostColumn(row, "Grubsong", CharmCostColumnX(3), "Grubsong.png", "Grubsong", ref gearSwitcherGrubsongCostValue, AdjustGrubsongCost);
            CreateCharmCostColumn(row, "GrubberflysElegy", CharmCostColumnX(4), "Grubberflys Elegy.png", "GrubberflysElegy", ref gearSwitcherGrubberflysElegyCostValue, AdjustGrubberflysElegyCost);
            lastY = rowY;

            rowY += GearSwitcherCharmCostRowSpacing;
            row = CreateGearSwitcherCharmCostsRow(content, "GearSwitcherCharmCostsRow3", rowY);
            CreateCharmCostColumn(row, "UnbreakableHeart", CharmCostColumnX(0), "Unbreakable Heart.png", "UnbreakableHeart", ref gearSwitcherUnbreakableHeartCostValue, AdjustUnbreakableHeartCost);
            CreateCharmCostColumn(row, "UnbreakableGreed", CharmCostColumnX(1), "Unbreakable Greed.png", "UnbreakableGreed", ref gearSwitcherUnbreakableGreedCostValue, AdjustUnbreakableGreedCost);
            CreateCharmCostColumn(row, "UnbreakableStrength", CharmCostColumnX(2), "Unbreakable Strength.png", "UnbreakableStrength", ref gearSwitcherUnbreakableStrengthCostValue, AdjustUnbreakableStrengthCost);
            CreateCharmCostColumn(row, "SpellTwister", CharmCostColumnX(3), "Spell Twister.png", "SpellTwister", ref gearSwitcherSpellTwisterCostValue, AdjustSpellTwisterCost);
            CreateCharmCostColumn(row, "SteadyBody", CharmCostColumnX(4), "Steady Body.png", "SteadyBody", ref gearSwitcherSteadyBodyCostValue, AdjustSteadyBodyCost);
            lastY = rowY;

            rowY += GearSwitcherCharmCostRowSpacing;
            row = CreateGearSwitcherCharmCostsRow(content, "GearSwitcherCharmCostsRow4", rowY);
            CreateCharmCostColumn(row, "HeavyBlow", CharmCostColumnX(0), "Heavy Blow.png", "HeavyBlow", ref gearSwitcherHeavyBlowCostValue, AdjustHeavyBlowCost);
            CreateCharmCostColumn(row, "QuickSlash", CharmCostColumnX(1), "Quick Slash.png", "QuickSlash", ref gearSwitcherQuickSlashCostValue, AdjustQuickSlashCost);
            CreateCharmCostColumn(row, "Longnail", CharmCostColumnX(2), "Longnail.png", "Longnail", ref gearSwitcherLongnailCostValue, AdjustLongnailCost);
            CreateCharmCostColumn(row, "MarkOfPride", CharmCostColumnX(3), "Mark of Pride.png", "MarkOfPride", ref gearSwitcherMarkOfPrideCostValue, AdjustMarkOfPrideCost);
            CreateCharmCostColumn(row, "FuryOfTheFallen", CharmCostColumnX(4), "Fury of the Fallen.png", "FuryOfTheFallen", ref gearSwitcherFuryOfTheFallenCostValue, AdjustFuryOfTheFallenCost);
            lastY = rowY;

            rowY += GearSwitcherCharmCostRowSpacing;
            row = CreateGearSwitcherCharmCostsRow(content, "GearSwitcherCharmCostsRow5", rowY);
            CreateCharmCostColumn(row, "ThornsOfAgony", CharmCostColumnX(0), "Thorns of Agony.png", "ThornsOfAgony", ref gearSwitcherThornsOfAgonyCostValue, AdjustThornsOfAgonyCost);
            CreateCharmCostColumn(row, "BaldurShell", CharmCostColumnX(1), "Baldur Shell.png", "BaldurShell", ref gearSwitcherBaldurShellCostValue, AdjustBaldurShellCost);
            CreateCharmCostColumn(row, "Flukenest", CharmCostColumnX(2), "Flukenest.png", "Flukenest", ref gearSwitcherFlukenestCostValue, AdjustFlukenestCost);
            CreateCharmCostColumn(row, "DefendersCrest", CharmCostColumnX(3), "Defenders Crest.png", "DefendersCrest", ref gearSwitcherDefendersCrestCostValue, AdjustDefendersCrestCost);
            CreateCharmCostColumn(row, "GlowingWomb", CharmCostColumnX(4), "Glowing Womb.png", "GlowingWomb", ref gearSwitcherGlowingWombCostValue, AdjustGlowingWombCost);
            lastY = rowY;

            rowY += GearSwitcherCharmCostRowSpacing;
            row = CreateGearSwitcherCharmCostsRow(content, "GearSwitcherCharmCostsRow6", rowY);
            CreateCharmCostColumn(row, "QuickFocus", CharmCostColumnX(0), "Quick Focus.png", "QuickFocus", ref gearSwitcherQuickFocusCostValue, AdjustQuickFocusCost);
            CreateCharmCostColumn(row, "DeepFocus", CharmCostColumnX(1), "Deep Focus.png", "DeepFocus", ref gearSwitcherDeepFocusCostValue, AdjustDeepFocusCost);
            CreateCharmCostColumn(row, "LifebloodHeart", CharmCostColumnX(2), "Lifeblood Heart.png", "LifebloodHeart", ref gearSwitcherLifebloodHeartCostValue, AdjustLifebloodHeartCost);
            CreateCharmCostColumn(row, "LifebloodCore", CharmCostColumnX(3), "Lifeblood Core.png", "LifebloodCore", ref gearSwitcherLifebloodCoreCostValue, AdjustLifebloodCoreCost);
            CreateCharmCostColumn(row, "JonisBlessing", CharmCostColumnX(4), "Jonis Blessing.png", "JonisBlessing", ref gearSwitcherJonisBlessingCostValue, AdjustJonisBlessingCost);
            lastY = rowY;

            rowY += GearSwitcherCharmCostRowSpacing;
            row = CreateGearSwitcherCharmCostsRow(content, "GearSwitcherCharmCostsRow7", rowY);
            CreateCharmCostColumn(row, "Hiveblood", CharmCostColumnX(0), "Hiveblood.png", "Hiveblood", ref gearSwitcherHivebloodCostValue, AdjustHivebloodCost);
            CreateCharmCostColumn(row, "SporeShroom", CharmCostColumnX(1), "Spore Shroom.png", "SporeShroom", ref gearSwitcherSporeShroomCostValue, AdjustSporeShroomCost);
            CreateCharmCostColumn(row, "SharpShadow", CharmCostColumnX(2), "Sharp Shadow.png", "SharpShadow", ref gearSwitcherSharpShadowCostValue, AdjustSharpShadowCost);
            CreateCharmCostColumn(row, "ShapeOfUnn", CharmCostColumnX(3), "Shape of Unn.png", "ShapeOfUnn", ref gearSwitcherShapeOfUnnCostValue, AdjustShapeOfUnnCost);
            CreateCharmCostColumn(row, "NailmastersGlory", CharmCostColumnX(4), "Nailmasters Glory.png", "NailmastersGlory", ref gearSwitcherNailmastersGloryCostValue, AdjustNailmastersGloryCost);
            lastY = rowY;

            rowY += GearSwitcherCharmCostRowSpacing;
            row = CreateGearSwitcherCharmCostsRow(content, "GearSwitcherCharmCostsRow8", rowY);
            CreateCharmCostColumn(row, "Weaversong", CharmCostColumnX(0), "Weaversong.png", "Weaversong", ref gearSwitcherWeaversongCostValue, AdjustWeaversongCost);
            CreateCharmCostColumn(row, "DreamWielder", CharmCostColumnX(1), "Dream Wielder.png", "DreamWielder", ref gearSwitcherDreamWielderCostValue, AdjustDreamWielderCost);
            CreateCharmCostColumn(row, "Dreamshield", CharmCostColumnX(2), "Dreamshield.png", "Dreamshield", ref gearSwitcherDreamshieldCostValue, AdjustDreamshieldCost);
            gearSwitcherCarefreeIcon = CreateCharmCostColumn(
                row,
                "CarefreeMelody",
                CharmCostColumnX(3),
                "Carefree Melody.png",
                "CarefreeMelody",
                ref gearSwitcherCarefreeMelodyCostValue,
                AdjustCarefreeMelodyCost,
                OnGearSwitcherCarefreeToggle,
                false);
            CreateCharmCostAdjustButtons(row, "CarefreeMelody", CharmCostColumnX(3), AdjustCarefreeMelodyCost);
            gearSwitcherVoidHeartIcon = CreateCharmCostColumn(
                row,
                "VoidHeart",
                CharmCostColumnX(4),
                "Void Heart.png",
                "VoidHeart",
                ref gearSwitcherVoidHeartCostValue,
                AdjustVoidHeartCost,
                OnGearSwitcherVoidHeartToggle,
                false);
            CreateCharmCostAdjustButtons(row, "VoidHeart", CharmCostColumnX(4), AdjustVoidHeartCost);
            AddCharmSwapBadge(gearSwitcherCarefreeIcon);
            AddCharmSwapBadge(gearSwitcherVoidHeartIcon);
            lastY = rowY;

            RegisterGearSwitcherCharmCostHighlights();

            SetScrollContentHeight(content, viewHeight, lastY, GearSwitcherCharmCostRowHeight);
            CreateButtonRow(panel.transform, "GearSwitcherCharmCostResetRow", "Reset Defaults Cost", resetY, OnGearSwitcherCharmCostResetDefaultsClicked);
            CreateButtonRow(panel.transform, "GearSwitcherCharmCostAheRow", "AHE Charm Costs", backY + ButtonRowHeight + buttonSpacing, OnGearSwitcherCharmCostAheClicked);
            CreateButtonRow(panel.transform, "GearSwitcherCharmCostBackRow", "Back", backY, OnGearSwitcherCharmCostBackClicked);
        }

        private void BuildGearSwitcherPresetOverlayUi()
        {
            gearSwitcherPresetEntries.Clear();
            gearSwitcherPresetDeleteTargetName = null;
            gearSwitcherPresetDeleteVisible = false;

            gearSwitcherPresetRoot = new GameObject("GearSwitcherPresetCanvas");
            gearSwitcherPresetRoot.transform.SetParent(transform, false);

            Canvas canvas = gearSwitcherPresetRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = GearSwitcherPresetCanvasSortOrder;

            CanvasScaler scaler = gearSwitcherPresetRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            gearSwitcherPresetRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = gearSwitcherPresetRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(gearSwitcherPresetRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("GearSwitcherPresetPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "GearSwitcher/SelectedPreset".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = PanelHeight;
            float createY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, createY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;

            string[] presets = GetGearSwitcherPresetOptions();
            for (int i = 0; i < presets.Length; i++)
            {
                CreateGearSwitcherPresetRow(content, i, presets[i], rowY);
                lastY = rowY;
                rowY += RowSpacing;
            }

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "GearSwitcherPresetCreateRow", "Create Preset", createY, OnGearSwitcherPresetCreateClicked);
            CreateButtonRow(panel.transform, "GearSwitcherPresetBackRow", "Back", backY, OnGearSwitcherPresetBackClicked);
            CreateGearSwitcherPresetDeleteConfirm(panel.transform);
            SetGearSwitcherPresetDeleteVisible(false);
        }

        private void BuildQolOverlayUi()
        {
            qolRoot = new GameObject("QolOverlayCanvas");
            qolRoot.transform.SetParent(transform, false);

            Canvas canvas = qolRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = QolCanvasSortOrder;

            CanvasScaler scaler = qolRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            qolRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = qolRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(qolRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("QolPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Categories/QoL".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = PanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            qolContent = content;
            qolScrollRect = content.GetComponentInParent<ScrollRect>();

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "QolEnableRow",
                "Settings/QoL/Enable".Localize(),
                rowY,
                GetQolMasterEnabled,
                SetQolMasterEnabled,
                out qolEnableValue,
                out qolEnableIcon
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateToggleRow(
                content,
                "QolFastDreamWarpRow",
                "Modules/FastDreamWarp".Localize(),
                rowY,
                GetFastDreamWarpEnabled,
                SetFastDreamWarpEnabled,
                out bossAnimFastDreamWarpValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateKeybindRow(
                content,
                "QolFastDreamWarpKeyRow",
                "Settings/FastDreamWarp/Hotkey".Localize(),
                rowY,
                GetFastDreamWarpBindingLabel,
                StartFastDreamWarpRebind,
                out fastDreamWarpKeyValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateToggleRow(
                content,
                "QolShortDeathRow",
                "Modules/ShortDeathAnimation".Localize(),
                rowY,
                GetShortDeathAnimationEnabled,
                SetShortDeathAnimationEnabled,
                out bossAnimShortDeathValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateToggleRow(
                content,
                "QolUnlockAllModesRow",
                "Modules/UnlockAllModes".Localize(),
                rowY,
                GetUnlockAllModesEnabled,
                SetUnlockAllModesEnabled,
                out qolUnlockAllModesValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateToggleRow(
                content,
                "QolUnlockPantheonsRow",
                "Modules/UnlockPantheons".Localize(),
                rowY,
                GetUnlockPantheonsEnabled,
                SetUnlockPantheonsEnabled,
                out qolUnlockPantheonsValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateToggleRow(
                content,
                "QolUnlockRadianceRow",
                "Modules/UnlockRadiance".Localize(),
                rowY,
                GetUnlockRadianceEnabled,
                SetUnlockRadianceEnabled,
                out qolUnlockRadianceValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateToggleRow(
                content,
                "QolUnlockRadiantRow",
                "Modules/UnlockRadiant".Localize(),
                rowY,
                GetUnlockRadiantEnabled,
                SetUnlockRadiantEnabled,
                out qolUnlockRadiantValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateToggleRow(
                content,
                "QolInvincibleIndicatorRow",
                "Modules/InvincibleIndicator".Localize(),
                rowY,
                GetInvincibleIndicatorEnabled,
                SetInvincibleIndicatorEnabled,
                out qolInvincibleIndicatorValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateToggleRow(
                content,
                "QolScreenShakeRow",
                "Disable ScreenShake",
                rowY,
                GetScreenShakeEnabled,
                SetScreenShakeEnabled,
                out qolScreenShakeValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "QolResetRow", "Settings/QoL/Reset".Localize(), resetY, OnQolResetDefaultsClicked);
            CreateButtonRow(panel.transform, "QolBackRow", "Back", backY, OnQolBackClicked);
        }

        private void BuildMenuAnimationOverlayUi()
        {
            menuAnimationRoot = new GameObject("MenuAnimationOverlayCanvas");
            menuAnimationRoot.transform.SetParent(transform, false);

            Canvas canvas = menuAnimationRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = MenuAnimationCanvasSortOrder;

            CanvasScaler scaler = menuAnimationRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            menuAnimationRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = menuAnimationRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(menuAnimationRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("MenuAnimationPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, MenuAnimationPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Categories/MenuAnimationSkipping".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 260f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = MenuAnimationPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            menuAnimationContent = content;

            float rowY = GetRowStartY(panelHeight, MenuAnimationRowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "MenuAnimEnableRow",
                "Settings/MenuAnimation/Enable".Localize(),
                rowY,
                GetMenuAnimationMasterEnabled,
                SetMenuAnimationMasterEnabled,
                out menuAnimEnableValue,
                out menuAnimEnableIcon
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "MenuAnimDoorDefaultRow",
                "Modules/DoorDefaultBegin".Localize(),
                rowY,
                GetDoorDefaultBeginEnabled,
                SetDoorDefaultBeginEnabled,
                out menuAnimDoorDefaultValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "MenuAnimMemorizeBindingsRow",
                "Modules/MemorizeBindings".Localize(),
                rowY,
                GetMemorizeBindingsEnabled,
                SetMemorizeBindingsEnabled,
                out menuAnimMemorizeBindingsValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "MenuAnimFasterLoadsRow",
                "Modules/FasterLoads".Localize(),
                rowY,
                GetFasterLoadsEnabled,
                SetFasterLoadsEnabled,
                out menuAnimFasterLoadsValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "MenuAnimFastMenusRow",
                "Modules/FastMenus".Localize(),
                rowY,
                GetFastMenusEnabled,
                SetFastMenusEnabled,
                out menuAnimFastMenusValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "MenuAnimFastTextRow",
                "Modules/FastText".Localize(),
                rowY,
                GetFastTextEnabled,
                SetFastTextEnabled,
                out menuAnimFastTextValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "MenuAnimAutoSkipRow",
                "Settings/AutoSkipCinematics".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.AutoSkipCinematics,
                value => Modules.QoL.SkipCutscenes.AutoSkipCinematics = value,
                out menuAnimAutoSkipValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "MenuAnimAllowSkippingRow",
                "Settings/AllowSkippingNonskippable".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.AllowSkippingNonskippable,
                value => Modules.QoL.SkipCutscenes.AllowSkippingNonskippable = value,
                out menuAnimAllowSkippingValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "MenuAnimSkipWithoutPromptRow",
                "Settings/SkipCutscenesWithoutPrompt".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt,
                value => Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt = value,
                out menuAnimSkipWithoutPromptValue
            );

            lastY = rowY;
            rowY += RowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "MenuAnimationResetRow", "Settings/MenuAnimation/Reset".Localize(), resetY, OnMenuAnimationResetDefaultsClicked);
            CreateButtonRow(panel.transform, "MenuAnimationBackRow", "Back", backY, OnMenuAnimationBackClicked);
        }

        private void BuildBossAnimationOverlayUi()
        {
            bossAnimationRoot = new GameObject("BossAnimationOverlayCanvas");
            bossAnimationRoot.transform.SetParent(transform, false);

            Canvas canvas = bossAnimationRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = BossAnimationCanvasSortOrder;

            CanvasScaler scaler = bossAnimationRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            bossAnimationRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = bossAnimationRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(bossAnimationRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("BossAnimationPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, BossAnimationPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Categories/BossAnimationSkipping".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 340f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = BossAnimationPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            bossAnimationContent = content;

            float rowY = GetRowStartY(panelHeight, BossAnimationRowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "BossAnimEnableRow",
                "Settings/BossAnimation/Enable".Localize(),
                rowY,
                GetBossAnimationMasterEnabled,
                SetBossAnimationMasterEnabled,
                out bossAnimEnableValue,
                out bossAnimEnableIcon
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "BossAnimHallOfGodsRow",
                "Settings/HallOfGodsStatues".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.HallOfGodsStatues,
                value => Modules.QoL.SkipCutscenes.HallOfGodsStatues = value,
                out bossAnimHallOfGodsValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "BossAnimAbsoluteRadianceRow",
                "Settings/AbsoluteRadiance".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.AbsoluteRadiance,
                value => Modules.QoL.SkipCutscenes.AbsoluteRadiance = value,
                out bossAnimAbsoluteRadianceValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "BossAnimPureVesselRow",
                "Settings/PureVesselRoar".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.PureVesselRoar,
                value => Modules.QoL.SkipCutscenes.PureVesselRoar = value,
                out bossAnimPureVesselValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "BossAnimGrimmNightmareRow",
                "Settings/GrimmNightmare".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.GrimmNightmare,
                value => Modules.QoL.SkipCutscenes.GrimmNightmare = value,
                out bossAnimGrimmNightmareValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "BossAnimGreyPrinceRow",
                "Settings/GreyPrinceZote".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.GreyPrinceZote,
                value => Modules.QoL.SkipCutscenes.GreyPrinceZote = value,
                out bossAnimGreyPrinceValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "BossAnimCollectorRow",
                "Settings/Collector".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.Collector,
                value => Modules.QoL.SkipCutscenes.Collector = value,
                out bossAnimCollectorValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "BossAnimSoulMasterRow",
                "Settings/SoulMasterPhaseTransitionSkip".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip,
                value => Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip = value,
                out bossAnimSoulMasterValue
            );

            lastY = rowY;
            rowY += RowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "BossAnimationResetRow", "Settings/BossAnimation/Reset".Localize(), resetY, OnBossAnimationResetDefaultsClicked);
            CreateButtonRow(panel.transform, "BossAnimationBackRow", "Back", backY, OnBossAnimationBackClicked);
        }

        private void BuildZoteHelperOverlayUi()
        {
            zoteHelperRoot = new GameObject("ZoteHelperOverlayCanvas");
            zoteHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = zoteHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = ZoteHelperCanvasSortOrder;

            CanvasScaler scaler = zoteHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            zoteHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = zoteHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(zoteHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("ZoteHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, ZoteHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/ZoteHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = ZoteHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            zoteHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "ZoteHelperEnableRow",
                "Settings/ZoteHelper/Enable".Localize(),
                rowY,
                GetZoteHelperEnabled,
                SetZoteHelperEnabled,
                out zoteHelperToggleValue,
                out zoteHelperToggleIcon
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateToggleRow(
                content,
                "ZoteUseCustomBossHpRow",
                "Settings/ZoteHelper/UseCustomBossHP".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteUseCustomBossHp,
                SetZoteUseCustomBossHpEnabled,
                out zoteUseCustomBossHpValue
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "ZoteBossHpRow",
                "Settings/ZoteHelper/ZoteBossHP".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteBossHp,
                SetZoteBossHp,
                100,
                999999,
                10,
                out zoteBossHpField
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateToggleRow(
                content,
                "ZoteImmortalRow",
                "Settings/ZoteHelper/ZoteImmortal".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteImmortal,
                SetZoteImmortalEnabled,
                out zoteImmortalValue
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateToggleRow(
                content,
                "ZoteSpawnFlyingRow",
                "Settings/ZoteHelper/ZoteSpawnFlying".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteSpawnFlying,
                SetZoteSpawnFlyingEnabled,
                out zoteSpawnFlyingValue
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateToggleRow(
                content,
                "ZoteSpawnHoppingRow",
                "Settings/ZoteHelper/ZoteSpawnHopping".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteSpawnHopping,
                SetZoteSpawnHoppingEnabled,
                out zoteSpawnHoppingValue
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateToggleRow(
                content,
                "ZoteUseCustomFlyingHpRow",
                "Settings/ZoteHelper/UseCustomFlyingHP".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteUseCustomFlyingHp,
                SetZoteUseCustomFlyingHpEnabled,
                out zoteUseCustomFlyingHpValue
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "ZoteFlyingHpRow",
                "Settings/ZoteHelper/ZoteFlyingHP".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteSummonFlyingHp,
                SetZoteFlyingHp,
                0,
                99,
                1,
                out zoteSummonFlyingHpField
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateToggleRow(
                content,
                "ZoteUseCustomHoppingHpRow",
                "Settings/ZoteHelper/UseCustomHoppingHP".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteUseCustomHoppingHp,
                SetZoteUseCustomHoppingHpEnabled,
                out zoteUseCustomHoppingHpValue
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "ZoteHoppingHpRow",
                "Settings/ZoteHelper/ZoteHoppingHP".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteSummonHoppingHp,
                SetZoteHoppingHp,
                0,
                99,
                1,
                out zoteSummonHoppingHpField
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateToggleRow(
                content,
                "ZoteUseCustomSummonLimitRow",
                "Settings/ZoteHelper/UseCustomSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteUseCustomSummonLimit,
                SetZoteUseCustomSummonLimitEnabled,
                out zoteUseCustomSummonLimitValue
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "ZoteSummonLimitRow",
                "Settings/ZoteHelper/ZoteSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteSummonLimit,
                SetZoteSummonLimit,
                0,
                99,
                1,
                out zoteSummonLimitField
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "ZoteHelperResetRow", "Settings/ZoteHelper/Reset".Localize(), resetY, OnZoteHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "ZoteHelperBackRow", "Back", backY, OnZoteHelperBackClicked);
        }

        private void BuildGruzHelperOverlayUi()
        {
            gruzHelperRoot = new GameObject("GruzHelperOverlayCanvas");
            gruzHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = gruzHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = GruzHelperCanvasSortOrder;

            CanvasScaler scaler = gruzHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            gruzHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = gruzHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(gruzHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("GruzHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, GruzHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/GruzMotherHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = GruzHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            gruzHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "GruzHelperEnableRow",
                "Settings/GruzMotherHelper/Enable".Localize(),
                rowY,
                GetGruzMotherHelperEnabled,
                SetGruzMotherHelperEnabled,
                out gruzHelperToggleValue,
                out gruzHelperToggleIcon
            );

            lastY = rowY;
            rowY += GruzHelperRowSpacing;
            CreateToggleRow(
                content,
                "GruzP5HpRow",
                "Settings/GruzMotherHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.GruzMotherHelper.gruzP5Hp,
                SetGruzP5HpEnabled,
                out gruzP5HpValue
            );

            lastY = rowY;
            rowY += GruzHelperRowSpacing;
            CreateToggleRow(
                content,
                "GruzUseMaxHpRow",
                "Settings/GruzMotherHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.GruzMotherHelper.gruzUseMaxHp,
                SetGruzUseMaxHpEnabled,
                out gruzUseMaxHpValue
            );

            lastY = rowY;
            rowY += GruzHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GruzMaxHpRow",
                "Settings/GruzMotherHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.GruzMotherHelper.gruzMaxHp,
                SetGruzMaxHp,
                1,
                999999,
                10,
                out gruzMaxHpField
            );

            lastY = rowY;
            rowY += GruzHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "GruzHelperResetRow", "Settings/GruzMotherHelper/Reset".Localize(), resetY, OnGruzHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "GruzHelperBackRow", "Back", backY, OnGruzHelperBackClicked);
        }

        private void BuildHornetHelperOverlayUi()
        {
            hornetHelperRoot = new GameObject("HornetHelperOverlayCanvas");
            hornetHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = hornetHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = HornetHelperCanvasSortOrder;

            CanvasScaler scaler = hornetHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            hornetHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = hornetHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(hornetHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("HornetHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, HornetHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/HornetProtectorHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = HornetHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            hornetHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "HornetHelperEnableRow",
                "Settings/HornetProtectorHelper/Enable".Localize(),
                rowY,
                GetHornetProtectorHelperEnabled,
                SetHornetProtectorHelperEnabled,
                out hornetHelperToggleValue,
                out hornetHelperToggleIcon
            );

            lastY = rowY;
            rowY += HornetHelperRowSpacing;
            CreateToggleRow(
                content,
                "HornetP5HpRow",
                "Settings/HornetProtectorHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.HornetProtectorHelper.hornetP5Hp,
                SetHornetP5HpEnabled,
                out hornetP5HpValue
            );

            lastY = rowY;
            rowY += HornetHelperRowSpacing;
            CreateToggleRow(
                content,
                "HornetUseMaxHpRow",
                "Settings/HornetProtectorHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.HornetProtectorHelper.hornetUseMaxHp,
                SetHornetUseMaxHpEnabled,
                out hornetUseMaxHpValue
            );

            lastY = rowY;
            rowY += HornetHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "HornetMaxHpRow",
                "Settings/HornetProtectorHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.HornetProtectorHelper.hornetMaxHp,
                SetHornetMaxHp,
                1,
                999999,
                10,
                out hornetMaxHpField
            );

            lastY = rowY;
            rowY += HornetHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "HornetHelperResetRow", "Settings/HornetProtectorHelper/Reset".Localize(), resetY, OnHornetHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "HornetHelperBackRow", "Back", backY, OnHornetHelperBackClicked);
        }

        private void BuildMawlekHelperOverlayUi()
        {
            mawlekHelperRoot = new GameObject("MawlekHelperOverlayCanvas");
            mawlekHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = mawlekHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = MawlekHelperCanvasSortOrder;

            CanvasScaler scaler = mawlekHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            mawlekHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = mawlekHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(mawlekHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("MawlekHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, MawlekHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/BroodingMawlekHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = MawlekHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            mawlekHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "MawlekHelperEnableRow",
                "Settings/BroodingMawlekHelper/Enable".Localize(),
                rowY,
                GetBroodingMawlekHelperEnabled,
                SetBroodingMawlekHelperEnabled,
                out mawlekHelperToggleValue,
                out mawlekHelperToggleIcon
            );

            lastY = rowY;
            rowY += MawlekHelperRowSpacing;
            CreateToggleRow(
                content,
                "MawlekP5HpRow",
                "Settings/BroodingMawlekHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.BroodingMawlekHelper.mawlekP5Hp,
                SetMawlekP5HpEnabled,
                out mawlekP5HpValue
            );

            lastY = rowY;
            rowY += MawlekHelperRowSpacing;
            CreateToggleRow(
                content,
                "MawlekUseMaxHpRow",
                "Settings/BroodingMawlekHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.BroodingMawlekHelper.mawlekUseMaxHp,
                SetMawlekUseMaxHpEnabled,
                out mawlekUseMaxHpValue
            );

            lastY = rowY;
            rowY += MawlekHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MawlekMaxHpRow",
                "Settings/BroodingMawlekHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.BroodingMawlekHelper.mawlekMaxHp,
                SetMawlekMaxHp,
                1,
                999999,
                10,
                out mawlekMaxHpField
            );

            lastY = rowY;
            rowY += MawlekHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "MawlekHelperResetRow", "Settings/BroodingMawlekHelper/Reset".Localize(), resetY, OnMawlekHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "MawlekHelperBackRow", "Back", backY, OnMawlekHelperBackClicked);
        }

        private void BuildMassiveMossHelperOverlayUi()
        {
            massiveMossHelperRoot = new GameObject("MassiveMossHelperOverlayCanvas");
            massiveMossHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = massiveMossHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = MassiveMossHelperCanvasSortOrder;

            CanvasScaler scaler = massiveMossHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            massiveMossHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = massiveMossHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(massiveMossHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("MassiveMossHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, MassiveMossHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/MassiveMossChargerHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = MassiveMossHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            massiveMossHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "MassiveMossHelperEnableRow",
                "Settings/MassiveMossChargerHelper/Enable".Localize(),
                rowY,
                GetMassiveMossChargerHelperEnabled,
                SetMassiveMossChargerHelperEnabled,
                out massiveMossHelperToggleValue,
                out massiveMossHelperToggleIcon
            );

            lastY = rowY;
            rowY += MassiveMossHelperRowSpacing;
            CreateToggleRow(
                content,
                "MassiveMossP5HpRow",
                "Settings/MassiveMossChargerHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.MassiveMossChargerHelper.massiveMossP5Hp,
                SetMassiveMossP5HpEnabled,
                out massiveMossP5HpValue
            );

            lastY = rowY;
            rowY += MassiveMossHelperRowSpacing;
            CreateToggleRow(
                content,
                "MassiveMossUseMaxHpRow",
                "Settings/MassiveMossChargerHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.MassiveMossChargerHelper.massiveMossUseMaxHp,
                SetMassiveMossUseMaxHpEnabled,
                out massiveMossUseMaxHpValue
            );

            lastY = rowY;
            rowY += MassiveMossHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MassiveMossMaxHpRow",
                "Settings/MassiveMossChargerHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.MassiveMossChargerHelper.massiveMossMaxHp,
                SetMassiveMossMaxHp,
                1,
                999999,
                10,
                out massiveMossMaxHpField
            );

            lastY = rowY;
            rowY += MassiveMossHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "MassiveMossHelperResetRow", "Settings/MassiveMossChargerHelper/Reset".Localize(), resetY, OnMassiveMossHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "MassiveMossHelperBackRow", "Back", backY, OnMassiveMossHelperBackClicked);
        }

        private void BuildCrystalGuardianHelperOverlayUi()
        {
            crystalGuardianHelperRoot = new GameObject("CrystalGuardianHelperOverlayCanvas");
            crystalGuardianHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = crystalGuardianHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = CrystalGuardianHelperCanvasSortOrder;

            CanvasScaler scaler = crystalGuardianHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            crystalGuardianHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = crystalGuardianHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(crystalGuardianHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("CrystalGuardianHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, CrystalGuardianHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/CrystalGuardianHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = CrystalGuardianHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            crystalGuardianHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "CrystalGuardianHelperEnableRow",
                "Settings/CrystalGuardianHelper/Enable".Localize(),
                rowY,
                GetCrystalGuardianHelperEnabled,
                SetCrystalGuardianHelperEnabled,
                out crystalGuardianHelperToggleValue,
                out crystalGuardianHelperToggleIcon
            );

            lastY = rowY;
            rowY += CrystalGuardianHelperRowSpacing;
            CreateToggleRow(
                content,
                "CrystalGuardianP5HpRow",
                "Settings/CrystalGuardianHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianP5Hp,
                SetCrystalGuardianP5HpEnabled,
                out crystalGuardianP5HpValue
            );

            lastY = rowY;
            rowY += CrystalGuardianHelperRowSpacing;
            CreateToggleRow(
                content,
                "CrystalGuardianUseMaxHpRow",
                "Settings/CrystalGuardianHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianUseMaxHp,
                SetCrystalGuardianUseMaxHpEnabled,
                out crystalGuardianUseMaxHpValue
            );

            lastY = rowY;
            rowY += CrystalGuardianHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "CrystalGuardianMaxHpRow",
                "Settings/CrystalGuardianHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianMaxHp,
                SetCrystalGuardianMaxHp,
                1,
                999999,
                10,
                out crystalGuardianMaxHpField
            );

            lastY = rowY;
            rowY += CrystalGuardianHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "CrystalGuardianHelperResetRow", "Settings/CrystalGuardianHelper/Reset".Localize(), resetY, OnCrystalGuardianHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "CrystalGuardianHelperBackRow", "Back", backY, OnCrystalGuardianHelperBackClicked);
        }

        private void BuildEnragedGuardianHelperOverlayUi()
        {
            enragedGuardianHelperRoot = new GameObject("EnragedGuardianHelperOverlayCanvas");
            enragedGuardianHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = enragedGuardianHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = EnragedGuardianHelperCanvasSortOrder;

            CanvasScaler scaler = enragedGuardianHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            enragedGuardianHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = enragedGuardianHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(enragedGuardianHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("EnragedGuardianHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, EnragedGuardianHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/EnragedGuardianHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = EnragedGuardianHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            enragedGuardianHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "EnragedGuardianHelperEnableRow",
                "Settings/EnragedGuardianHelper/Enable".Localize(),
                rowY,
                GetEnragedGuardianHelperEnabled,
                SetEnragedGuardianHelperEnabled,
                out enragedGuardianHelperToggleValue,
                out enragedGuardianHelperToggleIcon
            );

            lastY = rowY;
            rowY += EnragedGuardianHelperRowSpacing;
            CreateToggleRow(
                content,
                "EnragedGuardianP5HpRow",
                "Settings/EnragedGuardianHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianP5Hp,
                SetEnragedGuardianP5HpEnabled,
                out enragedGuardianP5HpValue
            );

            lastY = rowY;
            rowY += EnragedGuardianHelperRowSpacing;
            CreateToggleRow(
                content,
                "EnragedGuardianUseMaxHpRow",
                "Settings/EnragedGuardianHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianUseMaxHp,
                SetEnragedGuardianUseMaxHpEnabled,
                out enragedGuardianUseMaxHpValue
            );

            lastY = rowY;
            rowY += EnragedGuardianHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "EnragedGuardianMaxHpRow",
                "Settings/EnragedGuardianHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianMaxHp,
                SetEnragedGuardianMaxHp,
                1,
                999999,
                10,
                out enragedGuardianMaxHpField
            );

            lastY = rowY;
            rowY += EnragedGuardianHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "EnragedGuardianHelperResetRow", "Settings/EnragedGuardianHelper/Reset".Localize(), resetY, OnEnragedGuardianHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "EnragedGuardianHelperBackRow", "Back", backY, OnEnragedGuardianHelperBackClicked);
        }

        private void BuildHornetSentinelHelperOverlayUi()
        {
            hornetSentinelHelperRoot = new GameObject("HornetSentinelHelperOverlayCanvas");
            hornetSentinelHelperRoot.transform.SetParent(transform, false);

            Canvas canvas = hornetSentinelHelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = HornetSentinelHelperCanvasSortOrder;

            CanvasScaler scaler = hornetSentinelHelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            hornetSentinelHelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = hornetSentinelHelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(hornetSentinelHelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("HornetSentinelHelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, HornetSentinelHelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Modules/HornetSentinelHelper".Localize(), 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 210f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = HornetSentinelHelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            hornetSentinelHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "HornetSentinelHelperEnableRow",
                "Settings/HornetSentinelHelper/Enable".Localize(),
                rowY,
                GetHornetSentinelHelperEnabled,
                SetHornetSentinelHelperEnabled,
                out hornetSentinelHelperToggleValue,
                out hornetSentinelHelperToggleIcon
            );

            lastY = rowY;
            rowY += HornetSentinelHelperRowSpacing;
            CreateToggleRow(
                content,
                "HornetSentinelP5HpRow",
                "Settings/HornetSentinelHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.HornetSentinelHelper.hornetSentinelP5Hp,
                SetHornetSentinelP5HpEnabled,
                out hornetSentinelP5HpValue
            );

            lastY = rowY;
            rowY += HornetSentinelHelperRowSpacing;
            CreateToggleRow(
                content,
                "HornetSentinelUseMaxHpRow",
                "Settings/HornetSentinelHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseMaxHp,
                SetHornetSentinelUseMaxHpEnabled,
                out hornetSentinelUseMaxHpValue
            );

            lastY = rowY;
            rowY += HornetSentinelHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "HornetSentinelMaxHpRow",
                "Settings/HornetSentinelHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.HornetSentinelHelper.hornetSentinelMaxHp,
                SetHornetSentinelMaxHp,
                1,
                999999,
                10,
                out hornetSentinelMaxHpField
            );

            lastY = rowY;
            rowY += HornetSentinelHelperRowSpacing;
            CreateToggleRow(
                content,
                "HornetSentinelUseCustomPhaseRow",
                "Settings/HornetSentinelHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseCustomPhase,
                SetHornetSentinelUseCustomPhaseEnabled,
                out hornetSentinelUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += HornetSentinelHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "HornetSentinelPhase2HpRow",
                "Settings/HornetSentinelHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.HornetSentinelHelper.hornetSentinelPhase2Hp,
                SetHornetSentinelPhase2Hp,
                1,
                1200,
                10,
                out hornetSentinelPhase2HpField
            );

            lastY = rowY;
            rowY += HornetSentinelHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "HornetSentinelHelperResetRow", "Settings/HornetSentinelHelper/Reset".Localize(), resetY, OnHornetSentinelHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "HornetSentinelHelperBackRow", "Back", backY, OnHornetSentinelHelperBackClicked);
        }
    }
}
