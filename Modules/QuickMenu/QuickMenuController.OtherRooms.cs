using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private const int BossManipulateOtherRoomsCanvasSortOrder = 10066;
        private const int GruzMotherP1HelperCanvasSortOrder = 10067;
        private const int VengeflyKingP1HelperCanvasSortOrder = 10068;
        private const int BroodingMawlekP1HelperCanvasSortOrder = 10069;
        private const int NoskP2HelperCanvasSortOrder = 10070;
        private const int UumuuP3HelperCanvasSortOrder = 10071;
        private const int SoulWarriorP1HelperCanvasSortOrder = 10072;
        private const int NoEyesP4HelperCanvasSortOrder = 10073;
        private const int MarmuP2HelperCanvasSortOrder = 10074;
        private const int XeroP2HelperCanvasSortOrder = 10075;
        private const int MarkothP4HelperCanvasSortOrder = 10076;
        private const int GorbP1HelperCanvasSortOrder = 10077;
        private const float BossManipulateOtherRoomsY = -300f;
        private const float GruzMotherP1HelperPanelHeight = PanelHeight;
        private const float GruzMotherP1HelperRowSpacing = 44f;
        private const float VengeflyKingP1HelperPanelHeight = PanelHeight;
        private const float VengeflyKingP1HelperRowSpacing = 44f;
        private const float BroodingMawlekP1HelperPanelHeight = PanelHeight;
        private const float BroodingMawlekP1HelperRowSpacing = 44f;
        private const float NoskP2HelperPanelHeight = PanelHeight;
        private const float NoskP2HelperRowSpacing = 44f;
        private const float UumuuP3HelperPanelHeight = PanelHeight;
        private const float UumuuP3HelperRowSpacing = 44f;
        private const float SoulWarriorP1HelperPanelHeight = PanelHeight;
        private const float SoulWarriorP1HelperRowSpacing = 44f;
        private const float NoEyesP4HelperPanelHeight = PanelHeight;
        private const float NoEyesP4HelperRowSpacing = 44f;
        private const float MarmuP2HelperPanelHeight = PanelHeight;
        private const float MarmuP2HelperRowSpacing = 44f;
        private const float XeroP2HelperPanelHeight = PanelHeight;
        private const float XeroP2HelperRowSpacing = 44f;
        private const float MarkothP4HelperPanelHeight = PanelHeight;
        private const float MarkothP4HelperRowSpacing = 44f;
        private const float GorbP1HelperPanelHeight = PanelHeight;
        private const float GorbP1HelperRowSpacing = 44f;

        private GameObject? bossManipulateOtherRoomsRoot;
        private bool bossManipulateOtherRoomsVisible;
        private bool returnToBossManipulateOtherRoomsOnClose;
        private GameObject? bossManipulateOtherRoomsResetConfirmRoot;
        private bool bossManipulateOtherRoomsResetConfirmVisible;

        private GameObject? gruzMotherP1HelperRoot;
        private RectTransform? gruzMotherP1HelperContent;
        private bool gruzMotherP1HelperVisible;
        private Text? gruzMotherP1HelperToggleValue;
        private Image? gruzMotherP1HelperToggleIcon;
        private Text? gruzMotherP1UseMaxHpValue;
        private InputField? gruzMotherP1MaxHpField;
        private Module? gruzMotherP1HelperModule;

        private GameObject? vengeflyKingP1HelperRoot;
        private RectTransform? vengeflyKingP1HelperContent;
        private bool vengeflyKingP1HelperVisible;
        private Text? vengeflyKingP1HelperToggleValue;
        private Image? vengeflyKingP1HelperToggleIcon;
        private Text? vengeflyKingP1UseMaxHpValue;
        private InputField? vengeflyKingP1MaxHpField;
        private Module? vengeflyKingP1HelperModule;

        private GameObject? broodingMawlekP1HelperRoot;
        private RectTransform? broodingMawlekP1HelperContent;
        private bool broodingMawlekP1HelperVisible;
        private Text? broodingMawlekP1HelperToggleValue;
        private Image? broodingMawlekP1HelperToggleIcon;
        private Text? broodingMawlekP1UseMaxHpValue;
        private InputField? broodingMawlekP1MaxHpField;
        private Module? broodingMawlekP1HelperModule;

        private GameObject? noskP2HelperRoot;
        private RectTransform? noskP2HelperContent;
        private bool noskP2HelperVisible;
        private Text? noskP2HelperToggleValue;
        private Image? noskP2HelperToggleIcon;
        private Text? noskP2UseMaxHpValue;
        private InputField? noskP2MaxHpField;
        private Text? noskP2UseCustomPhaseValue;
        private InputField? noskP2Phase2HpField;
        private Module? noskP2HelperModule;

        private GameObject? uumuuP3HelperRoot;
        private RectTransform? uumuuP3HelperContent;
        private bool uumuuP3HelperVisible;
        private Text? uumuuP3HelperToggleValue;
        private Image? uumuuP3HelperToggleIcon;
        private Text? uumuuP3UseMaxHpValue;
        private InputField? uumuuP3MaxHpField;
        private Text? uumuuP3UseCustomSummonHpValue;
        private InputField? uumuuP3SummonHpField;
        private Module? uumuuP3HelperModule;

        private GameObject? soulWarriorP1HelperRoot;
        private RectTransform? soulWarriorP1HelperContent;
        private bool soulWarriorP1HelperVisible;
        private Text? soulWarriorP1HelperToggleValue;
        private Image? soulWarriorP1HelperToggleIcon;
        private Text? soulWarriorP1UseMaxHpValue;
        private InputField? soulWarriorP1MaxHpField;
        private Module? soulWarriorP1HelperModule;

        private GameObject? noEyesP4HelperRoot;
        private RectTransform? noEyesP4HelperContent;
        private bool noEyesP4HelperVisible;
        private Text? noEyesP4HelperToggleValue;
        private Image? noEyesP4HelperToggleIcon;
        private Text? noEyesP4UseMaxHpValue;
        private InputField? noEyesP4MaxHpField;
        private Text? noEyesP4UseCustomPhaseValue;
        private InputField? noEyesP4Phase2HpField;
        private InputField? noEyesP4Phase3HpField;
        private Module? noEyesP4HelperModule;

        private GameObject? marmuP2HelperRoot;
        private RectTransform? marmuP2HelperContent;
        private bool marmuP2HelperVisible;
        private Text? marmuP2HelperToggleValue;
        private Image? marmuP2HelperToggleIcon;
        private Text? marmuP2UseMaxHpValue;
        private InputField? marmuP2MaxHpField;
        private Module? marmuP2HelperModule;

        private GameObject? xeroP2HelperRoot;
        private RectTransform? xeroP2HelperContent;
        private bool xeroP2HelperVisible;
        private Text? xeroP2HelperToggleValue;
        private Image? xeroP2HelperToggleIcon;
        private Text? xeroP2UseMaxHpValue;
        private InputField? xeroP2MaxHpField;
        private Text? xeroP2UseCustomPhaseValue;
        private InputField? xeroP2Phase2HpField;
        private Module? xeroP2HelperModule;

        private GameObject? markothP4HelperRoot;
        private RectTransform? markothP4HelperContent;
        private bool markothP4HelperVisible;
        private Text? markothP4HelperToggleValue;
        private Image? markothP4HelperToggleIcon;
        private Text? markothP4UseMaxHpValue;
        private InputField? markothP4MaxHpField;
        private Text? markothP4UseCustomPhaseValue;
        private InputField? markothP4Phase2HpField;
        private Module? markothP4HelperModule;

        private GameObject? gorbP1HelperRoot;
        private RectTransform? gorbP1HelperContent;
        private bool gorbP1HelperVisible;
        private Text? gorbP1HelperToggleValue;
        private Image? gorbP1HelperToggleIcon;
        private Text? gorbP1UseMaxHpValue;
        private InputField? gorbP1MaxHpField;
        private Text? gorbP1UseCustomPhaseValue;
        private InputField? gorbP1Phase2HpField;
        private InputField? gorbP1Phase3HpField;
        private Module? gorbP1HelperModule;

        private void BuildBossManipulateOtherRoomsOverlayUi()
        {
            bossManipulateOtherRoomsRoot = new GameObject("BossManipulateOtherRoomsOverlayCanvas");
            bossManipulateOtherRoomsRoot.transform.SetParent(transform, false);

            Canvas canvas = bossManipulateOtherRoomsRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = BossManipulateOtherRoomsCanvasSortOrder;

            CanvasScaler scaler = bossManipulateOtherRoomsRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            bossManipulateOtherRoomsRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = bossManipulateOtherRoomsRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new("Dim");
            dim.transform.SetParent(bossManipulateOtherRoomsRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new("BossManipulateOtherRoomsPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(BossManipulatePanelWidth, BossManipulatePanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Other Rooms", 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, (BossManipulatePanelHeight * 0.5f) - 70f);
            titleRect.sizeDelta = new Vector2(BossManipulatePanelWidth - 120f, 60f);

            float backY = GetFixedBackY(BossManipulatePanelHeight);
            float resetY = BossManipulateResetY;
            float controlsTopY = resetY + (ButtonRowHeight * 0.5f);
            RectTransform gridRoot = CreateBossManipulateGridRoot(panel.transform, titleRect, controlsTopY);
            BuildBossManipulateOtherRoomsImageGrid(gridRoot);
            CreateButtonRow(panel.transform, "BossManipulateOtherRoomsResetRow", "Reset Default", resetY, OnBossManipulateOtherRoomsResetAllClicked);
            CreateButtonRow(panel.transform, "BossManipulateOtherRoomsBackRow", "Back", backY, OnBossManipulateOtherRoomsBackClicked);
            CreateBossManipulateOtherRoomsResetConfirm(panel.transform);
            SetBossManipulateOtherRoomsResetConfirmVisible(false);
            RefreshBossManipulateCardVisuals();
        }

        private void BuildBossManipulateOtherRoomsImageGrid(RectTransform gridRoot)
        {
            var cards = new (string Label, string ImageFile, Type ModuleType, Action OnClick)[]
            {
                ("Vengefly King P1", "Vengefly King.png", typeof(Modules.BossChallenge.VengeflyKingP1Helper), OnBossManipulateOtherRoomsVengeflyKingP1Clicked),
                ("Gruz Mother P1", "Gruz Mother.png", typeof(Modules.BossChallenge.GruzMotherP1Helper), OnBossManipulateOtherRoomsGruzMotherP1Clicked),
                ("Gorb P1", "Gorb.png", typeof(Modules.BossChallenge.GorbP1Helper), OnBossManipulateOtherRoomsGorbP1Clicked),
                ("Soul Warrior P1", "Soul Warrior.png", typeof(Modules.BossChallenge.SoulWarriorP1Helper), OnBossManipulateOtherRoomsSoulWarriorP1Clicked),
                ("Brooding Mawlek P1", "Brooding Mawlek.png", typeof(Modules.BossChallenge.BroodingMawlekP1Helper), OnBossManipulateOtherRoomsBroodingMawlekP1Clicked),
                ("Xero P2", "Xero.png", typeof(Modules.BossChallenge.XeroP2Helper), OnBossManipulateOtherRoomsXeroP2Clicked),
                ("Marmu P2", "Marmu.png", typeof(Modules.BossChallenge.MarmuP2Helper), OnBossManipulateOtherRoomsMarmuP2Clicked),
                ("Nosk P2", "Nosk.png", typeof(Modules.BossChallenge.NoskP2Helper), OnBossManipulateOtherRoomsNoskP2Clicked),
                ("Uumuu P3", "Uumuu.png", typeof(Modules.BossChallenge.UumuuP3Helper), OnBossManipulateOtherRoomsUumuuP3Clicked),
                ("No Eyes P4", "No Eyes.png", typeof(Modules.BossChallenge.NoEyesP4Helper), OnBossManipulateOtherRoomsNoEyesP4Clicked),
                ("Markoth P4", "Markoth.png", typeof(Modules.BossChallenge.MarkothP4Helper), OnBossManipulateOtherRoomsMarkothP4Clicked),
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
                    $"BossManipulateOtherRoomsCard{index + 1:D2}",
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

        private void CreateBossManipulateOtherRoomsResetConfirm(Transform parent)
        {
            bossManipulateOtherRoomsResetConfirmRoot = new GameObject("BossManipulateOtherRoomsResetConfirm");
            bossManipulateOtherRoomsResetConfirmRoot.transform.SetParent(parent, false);

            RectTransform rootRect = bossManipulateOtherRoomsResetConfirmRoot.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            Image dim = bossManipulateOtherRoomsResetConfirmRoot.AddComponent<Image>();
            dim.color = new Color(0f, 0f, 0f, 0.35f);

            CanvasGroup group = bossManipulateOtherRoomsResetConfirmRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dialogObj = new("Dialog");
            dialogObj.transform.SetParent(bossManipulateOtherRoomsResetConfirmRoot.transform, false);
            RectTransform dialogRect = dialogObj.AddComponent<RectTransform>();
            dialogRect.anchorMin = new Vector2(0.5f, 0.5f);
            dialogRect.anchorMax = new Vector2(0.5f, 0.5f);
            dialogRect.pivot = new Vector2(0.5f, 0.5f);
            dialogRect.anchoredPosition = Vector2.zero;
            dialogRect.sizeDelta = new Vector2(740f, 340f);

            Image dialogImage = dialogObj.AddComponent<Image>();
            dialogImage.color = OverlayPanelColor;

            Text label = CreateText(dialogObj.transform, "Label", "Do you really want to reset Other Rooms?", 24, TextAnchor.MiddleCenter);
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            RectTransform labelRect = label.rectTransform;
            labelRect.anchorMin = new Vector2(0.5f, 1f);
            labelRect.anchorMax = new Vector2(0.5f, 1f);
            labelRect.pivot = new Vector2(0.5f, 1f);
            labelRect.anchoredPosition = new Vector2(0f, -28f);
            labelRect.sizeDelta = new Vector2(680f, 110f);

            CreateButtonRow(dialogObj.transform, "BossManipulateOtherRoomsResetYesRow", "Yes", -70f, OnBossManipulateOtherRoomsResetConfirmYes);
            CreateButtonRow(dialogObj.transform, "BossManipulateOtherRoomsResetNoRow", "No", -130f, OnBossManipulateOtherRoomsResetConfirmNo);
        }

        private void SetBossManipulateOtherRoomsResetConfirmVisible(bool value)
        {
            bossManipulateOtherRoomsResetConfirmVisible = value;
            if (bossManipulateOtherRoomsResetConfirmRoot != null)
            {
                bossManipulateOtherRoomsResetConfirmRoot.SetActive(value);
            }
        }

        private void BuildGruzMotherP1HelperOverlayUi()
        {
            gruzMotherP1HelperRoot = new GameObject("GruzMotherP1HelperOverlayCanvas");
            gruzMotherP1HelperRoot.transform.SetParent(transform, false);

            Canvas canvas = gruzMotherP1HelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = GruzMotherP1HelperCanvasSortOrder;

            CanvasScaler scaler = gruzMotherP1HelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            gruzMotherP1HelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = gruzMotherP1HelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new("Dim");
            dim.transform.SetParent(gruzMotherP1HelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new("GruzMotherP1HelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, GruzMotherP1HelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Gruz Mother P1", 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 420f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = GruzMotherP1HelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            gruzMotherP1HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "GruzMotherP1EnableRow",
                "Enable Gruz Mother P1",
                rowY,
                GetGruzMotherP1HelperEnabled,
                SetGruzMotherP1HelperEnabled,
                out gruzMotherP1HelperToggleValue,
                out gruzMotherP1HelperToggleIcon);

            rowY += GruzMotherP1HelperRowSpacing;
            CreateToggleRow(
                content,
                "GruzMotherP1UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1UseMaxHp,
                SetGruzMotherP1UseMaxHpEnabled,
                out gruzMotherP1UseMaxHpValue);
            lastY = rowY;

            rowY += GruzMotherP1HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GruzMotherP1MaxHpRow",
                "Gruz Mother P1 Max HP",
                rowY,
                () => Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1MaxHp,
                SetGruzMotherP1MaxHp,
                1,
                999999,
                1,
                out gruzMotherP1MaxHpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "GruzMotherP1ResetRow", "Reset Default", resetY, OnGruzMotherP1HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "GruzMotherP1BackRow", "Back", backY, OnGruzMotherP1HelperBackClicked);
        }

        private void BuildVengeflyKingP1HelperOverlayUi()
        {
            vengeflyKingP1HelperRoot = new GameObject("VengeflyKingP1HelperOverlayCanvas");
            vengeflyKingP1HelperRoot.transform.SetParent(transform, false);

            Canvas canvas = vengeflyKingP1HelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = VengeflyKingP1HelperCanvasSortOrder;

            CanvasScaler scaler = vengeflyKingP1HelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            vengeflyKingP1HelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = vengeflyKingP1HelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new("Dim");
            dim.transform.SetParent(vengeflyKingP1HelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new("VengeflyKingP1HelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, VengeflyKingP1HelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Vengefly King P1", 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 420f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = VengeflyKingP1HelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            vengeflyKingP1HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "VengeflyKingP1EnableRow",
                "Enable Vengefly King P1",
                rowY,
                GetVengeflyKingP1HelperEnabled,
                SetVengeflyKingP1HelperEnabled,
                out vengeflyKingP1HelperToggleValue,
                out vengeflyKingP1HelperToggleIcon);

            rowY += VengeflyKingP1HelperRowSpacing;
            CreateToggleRow(
                content,
                "VengeflyKingP1UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1UseMaxHp,
                SetVengeflyKingP1UseMaxHpEnabled,
                out vengeflyKingP1UseMaxHpValue);
            lastY = rowY;

            rowY += VengeflyKingP1HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "VengeflyKingP1MaxHpRow",
                "Vengefly King P1 Max HP",
                rowY,
                () => Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1MaxHp,
                SetVengeflyKingP1MaxHp,
                1,
                999999,
                1,
                out vengeflyKingP1MaxHpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "VengeflyKingP1ResetRow", "Reset Default", resetY, OnVengeflyKingP1HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "VengeflyKingP1BackRow", "Back", backY, OnVengeflyKingP1HelperBackClicked);
        }

        private void BuildBroodingMawlekP1HelperOverlayUi()
        {
            broodingMawlekP1HelperRoot = new GameObject("BroodingMawlekP1HelperOverlayCanvas");
            broodingMawlekP1HelperRoot.transform.SetParent(transform, false);

            Canvas canvas = broodingMawlekP1HelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = BroodingMawlekP1HelperCanvasSortOrder;

            CanvasScaler scaler = broodingMawlekP1HelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            broodingMawlekP1HelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = broodingMawlekP1HelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new("Dim");
            dim.transform.SetParent(broodingMawlekP1HelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new("BroodingMawlekP1HelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, BroodingMawlekP1HelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Brooding Mawlek P1", 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 420f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = BroodingMawlekP1HelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            broodingMawlekP1HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "BroodingMawlekP1EnableRow",
                "Enable Brooding Mawlek P1",
                rowY,
                GetBroodingMawlekP1HelperEnabled,
                SetBroodingMawlekP1HelperEnabled,
                out broodingMawlekP1HelperToggleValue,
                out broodingMawlekP1HelperToggleIcon);

            rowY += BroodingMawlekP1HelperRowSpacing;
            CreateToggleRow(
                content,
                "BroodingMawlekP1UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1UseMaxHp,
                SetBroodingMawlekP1UseMaxHpEnabled,
                out broodingMawlekP1UseMaxHpValue);
            lastY = rowY;

            rowY += BroodingMawlekP1HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "BroodingMawlekP1MaxHpRow",
                "Brooding Mawlek P1 Max HP",
                rowY,
                () => Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1MaxHp,
                SetBroodingMawlekP1MaxHp,
                1,
                999999,
                1,
                out broodingMawlekP1MaxHpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "BroodingMawlekP1ResetRow", "Reset Default", resetY, OnBroodingMawlekP1HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "BroodingMawlekP1BackRow", "Back", backY, OnBroodingMawlekP1HelperBackClicked);
        }

        private void BuildNoskP2HelperOverlayUi()
        {
            noskP2HelperRoot = new GameObject("NoskP2HelperOverlayCanvas");
            noskP2HelperRoot.transform.SetParent(transform, false);

            Canvas canvas = noskP2HelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = NoskP2HelperCanvasSortOrder;

            CanvasScaler scaler = noskP2HelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            noskP2HelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = noskP2HelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new("Dim");
            dim.transform.SetParent(noskP2HelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new("NoskP2HelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, NoskP2HelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Nosk P2", 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 420f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = NoskP2HelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            noskP2HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "NoskP2EnableRow",
                "Enable Nosk P2",
                rowY,
                GetNoskP2HelperEnabled,
                SetNoskP2HelperEnabled,
                out noskP2HelperToggleValue,
                out noskP2HelperToggleIcon);

            rowY += NoskP2HelperRowSpacing;
            CreateToggleRow(
                content,
                "NoskP2UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.NoskP2Helper.noskP2UseMaxHp,
                SetNoskP2UseMaxHpEnabled,
                out noskP2UseMaxHpValue);
            lastY = rowY;

            rowY += NoskP2HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoskP2MaxHpRow",
                "Nosk P2 Max HP",
                rowY,
                () => Modules.BossChallenge.NoskP2Helper.noskP2MaxHp,
                SetNoskP2MaxHp,
                1,
                999999,
                1,
                out noskP2MaxHpField);
            lastY = rowY;

            rowY += NoskP2HelperRowSpacing;
            CreateToggleRow(
                content,
                "NoskP2UseCustomPhaseRow",
                "Use Custom Phase",
                rowY,
                () => Modules.BossChallenge.NoskP2Helper.noskP2UseCustomPhase,
                SetNoskP2UseCustomPhaseEnabled,
                out noskP2UseCustomPhaseValue);
            lastY = rowY;

            rowY += NoskP2HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoskP2Phase2HpRow",
                "Phase 2 HP",
                rowY,
                () => Modules.BossChallenge.NoskP2Helper.noskP2Phase2Hp,
                SetNoskP2Phase2Hp,
                1,
                999999,
                1,
                out noskP2Phase2HpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "NoskP2ResetRow", "Reset Default", resetY, OnNoskP2HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "NoskP2BackRow", "Back", backY, OnNoskP2HelperBackClicked);
        }

        private void BuildUumuuP3HelperOverlayUi()
        {
            uumuuP3HelperRoot = new GameObject("UumuuP3HelperOverlayCanvas");
            uumuuP3HelperRoot.transform.SetParent(transform, false);

            Canvas canvas = uumuuP3HelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = UumuuP3HelperCanvasSortOrder;

            CanvasScaler scaler = uumuuP3HelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            uumuuP3HelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = uumuuP3HelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new("Dim");
            dim.transform.SetParent(uumuuP3HelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new("UumuuP3HelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, UumuuP3HelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Uumuu P3", 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 420f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = UumuuP3HelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            uumuuP3HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "UumuuP3EnableRow",
                "Enable Uumuu P3",
                rowY,
                GetUumuuP3HelperEnabled,
                SetUumuuP3HelperEnabled,
                out uumuuP3HelperToggleValue,
                out uumuuP3HelperToggleIcon);

            rowY += UumuuP3HelperRowSpacing;
            CreateToggleRow(
                content,
                "UumuuP3UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.UumuuP3Helper.uumuuP3UseMaxHp,
                SetUumuuP3UseMaxHpEnabled,
                out uumuuP3UseMaxHpValue);
            lastY = rowY;

            rowY += UumuuP3HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "UumuuP3MaxHpRow",
                "Uumuu P3 Max HP",
                rowY,
                () => Modules.BossChallenge.UumuuP3Helper.uumuuP3MaxHp,
                SetUumuuP3MaxHp,
                1,
                999999,
                1,
                out uumuuP3MaxHpField);
            lastY = rowY;

            rowY += UumuuP3HelperRowSpacing;
            CreateToggleRow(
                content,
                "UumuuP3UseCustomSummonHpRow",
                "Use Custom Summon HP",
                rowY,
                () => Modules.BossChallenge.UumuuP3Helper.uumuuP3UseCustomSummonHp,
                SetUumuuP3UseCustomSummonHpEnabled,
                out uumuuP3UseCustomSummonHpValue);
            lastY = rowY;

            rowY += UumuuP3HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "UumuuP3SummonHpRow",
                "Summon HP",
                rowY,
                () => Modules.BossChallenge.UumuuP3Helper.uumuuP3SummonHp,
                SetUumuuP3SummonHp,
                1,
                999999,
                1,
                out uumuuP3SummonHpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "UumuuP3ResetRow", "Reset Default", resetY, OnUumuuP3HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "UumuuP3BackRow", "Back", backY, OnUumuuP3HelperBackClicked);
        }

        private void BuildSoulWarriorP1HelperOverlayUi()
        {
            soulWarriorP1HelperRoot = new GameObject("SoulWarriorP1HelperOverlayCanvas");
            soulWarriorP1HelperRoot.transform.SetParent(transform, false);

            Canvas canvas = soulWarriorP1HelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = SoulWarriorP1HelperCanvasSortOrder;

            CanvasScaler scaler = soulWarriorP1HelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            soulWarriorP1HelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = soulWarriorP1HelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new("Dim");
            dim.transform.SetParent(soulWarriorP1HelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new("SoulWarriorP1HelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, SoulWarriorP1HelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Soul Warrior P1", 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 420f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = SoulWarriorP1HelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            soulWarriorP1HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "SoulWarriorP1EnableRow",
                "Enable Soul Warrior P1",
                rowY,
                GetSoulWarriorP1HelperEnabled,
                SetSoulWarriorP1HelperEnabled,
                out soulWarriorP1HelperToggleValue,
                out soulWarriorP1HelperToggleIcon);

            rowY += SoulWarriorP1HelperRowSpacing;
            CreateToggleRow(
                content,
                "SoulWarriorP1UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1UseMaxHp,
                SetSoulWarriorP1UseMaxHpEnabled,
                out soulWarriorP1UseMaxHpValue);
            lastY = rowY;

            rowY += SoulWarriorP1HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "SoulWarriorP1MaxHpRow",
                "Soul Warrior P1 Max HP",
                rowY,
                () => Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1MaxHp,
                SetSoulWarriorP1MaxHp,
                1,
                999999,
                1,
                out soulWarriorP1MaxHpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "SoulWarriorP1ResetRow", "Reset Default", resetY, OnSoulWarriorP1HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "SoulWarriorP1BackRow", "Back", backY, OnSoulWarriorP1HelperBackClicked);
        }

        private void BuildNoEyesP4HelperOverlayUi()
        {
            noEyesP4HelperRoot = new GameObject("NoEyesP4HelperOverlayCanvas");
            noEyesP4HelperRoot.transform.SetParent(transform, false);

            Canvas canvas = noEyesP4HelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = NoEyesP4HelperCanvasSortOrder;

            CanvasScaler scaler = noEyesP4HelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            noEyesP4HelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = noEyesP4HelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new("Dim");
            dim.transform.SetParent(noEyesP4HelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new("NoEyesP4HelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, NoEyesP4HelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "No Eyes P4", 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 420f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = NoEyesP4HelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            noEyesP4HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "NoEyesP4EnableRow",
                "Enable No Eyes P4",
                rowY,
                GetNoEyesP4HelperEnabled,
                SetNoEyesP4HelperEnabled,
                out noEyesP4HelperToggleValue,
                out noEyesP4HelperToggleIcon);

            rowY += NoEyesP4HelperRowSpacing;
            CreateToggleRow(
                content,
                "NoEyesP4UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseMaxHp,
                SetNoEyesP4UseMaxHpEnabled,
                out noEyesP4UseMaxHpValue);
            lastY = rowY;

            rowY += NoEyesP4HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoEyesP4MaxHpRow",
                "No Eyes P4 Max HP",
                rowY,
                () => Modules.BossChallenge.NoEyesP4Helper.noEyesP4MaxHp,
                SetNoEyesP4MaxHp,
                1,
                999999,
                1,
                out noEyesP4MaxHpField);
            lastY = rowY;

            rowY += NoEyesP4HelperRowSpacing;
            CreateToggleRow(
                content,
                "NoEyesP4UseCustomPhaseRow",
                "Use Custom Phase",
                rowY,
                () => Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase,
                SetNoEyesP4UseCustomPhaseEnabled,
                out noEyesP4UseCustomPhaseValue);
            lastY = rowY;

            rowY += NoEyesP4HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoEyesP4Phase2HpRow",
                "Phase 2 HP",
                rowY,
                () => Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase2Hp,
                SetNoEyesP4Phase2Hp,
                1,
                999999,
                1,
                out noEyesP4Phase2HpField);
            lastY = rowY;

            rowY += NoEyesP4HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "NoEyesP4Phase3HpRow",
                "Phase 3 HP",
                rowY,
                () => Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase3Hp,
                SetNoEyesP4Phase3Hp,
                1,
                999999,
                1,
                out noEyesP4Phase3HpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "NoEyesP4ResetRow", "Reset Default", resetY, OnNoEyesP4HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "NoEyesP4BackRow", "Back", backY, OnNoEyesP4HelperBackClicked);
        }

        private void BuildMarmuP2HelperOverlayUi()
        {
            marmuP2HelperRoot = new GameObject("MarmuP2HelperOverlayCanvas");
            marmuP2HelperRoot.transform.SetParent(transform, false);

            Canvas canvas = marmuP2HelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = MarmuP2HelperCanvasSortOrder;

            CanvasScaler scaler = marmuP2HelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            marmuP2HelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = marmuP2HelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new("Dim");
            dim.transform.SetParent(marmuP2HelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new("MarmuP2HelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, MarmuP2HelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Marmu P2", 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 420f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = MarmuP2HelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            marmuP2HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "MarmuP2EnableRow",
                "Enable Marmu P2",
                rowY,
                GetMarmuP2HelperEnabled,
                SetMarmuP2HelperEnabled,
                out marmuP2HelperToggleValue,
                out marmuP2HelperToggleIcon);

            rowY += MarmuP2HelperRowSpacing;
            CreateToggleRow(
                content,
                "MarmuP2UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.MarmuP2Helper.marmuP2UseMaxHp,
                SetMarmuP2UseMaxHpEnabled,
                out marmuP2UseMaxHpValue);
            lastY = rowY;

            rowY += MarmuP2HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MarmuP2MaxHpRow",
                "Marmu P2 Max HP",
                rowY,
                () => Modules.BossChallenge.MarmuP2Helper.marmuP2MaxHp,
                SetMarmuP2MaxHp,
                1,
                999999,
                1,
                out marmuP2MaxHpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "MarmuP2ResetRow", "Reset Default", resetY, OnMarmuP2HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "MarmuP2BackRow", "Back", backY, OnMarmuP2HelperBackClicked);
        }

        private void BuildXeroP2HelperOverlayUi()
        {
            xeroP2HelperRoot = new GameObject("XeroP2HelperOverlayCanvas");
            xeroP2HelperRoot.transform.SetParent(transform, false);

            Canvas canvas = xeroP2HelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = XeroP2HelperCanvasSortOrder;

            CanvasScaler scaler = xeroP2HelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            xeroP2HelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = xeroP2HelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new("Dim");
            dim.transform.SetParent(xeroP2HelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new("XeroP2HelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, XeroP2HelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Xero P2", 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 420f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = XeroP2HelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            xeroP2HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "XeroP2EnableRow",
                "Enable Xero P2",
                rowY,
                GetXeroP2HelperEnabled,
                SetXeroP2HelperEnabled,
                out xeroP2HelperToggleValue,
                out xeroP2HelperToggleIcon);

            rowY += XeroP2HelperRowSpacing;
            CreateToggleRow(
                content,
                "XeroP2UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.XeroP2Helper.xeroP2UseMaxHp,
                SetXeroP2UseMaxHpEnabled,
                out xeroP2UseMaxHpValue);
            lastY = rowY;

            rowY += XeroP2HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "XeroP2MaxHpRow",
                "Xero P2 Max HP",
                rowY,
                () => Modules.BossChallenge.XeroP2Helper.xeroP2MaxHp,
                SetXeroP2MaxHp,
                1,
                999999,
                1,
                out xeroP2MaxHpField);
            lastY = rowY;

            rowY += XeroP2HelperRowSpacing;
            CreateToggleRow(
                content,
                "XeroP2UseCustomPhaseRow",
                "Use Custom Phase",
                rowY,
                () => Modules.BossChallenge.XeroP2Helper.xeroP2UseCustomPhase,
                SetXeroP2UseCustomPhaseEnabled,
                out xeroP2UseCustomPhaseValue);
            lastY = rowY;

            rowY += XeroP2HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "XeroP2Phase2HpRow",
                "Phase 2 HP",
                rowY,
                () => Modules.BossChallenge.XeroP2Helper.xeroP2Phase2Hp,
                SetXeroP2Phase2Hp,
                1,
                999999,
                1,
                out xeroP2Phase2HpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "XeroP2ResetRow", "Reset Default", resetY, OnXeroP2HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "XeroP2BackRow", "Back", backY, OnXeroP2HelperBackClicked);
        }

        private void BuildMarkothP4HelperOverlayUi()
        {
            markothP4HelperRoot = new GameObject("MarkothP4HelperOverlayCanvas");
            markothP4HelperRoot.transform.SetParent(transform, false);

            Canvas canvas = markothP4HelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = MarkothP4HelperCanvasSortOrder;

            CanvasScaler scaler = markothP4HelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            markothP4HelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = markothP4HelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new("Dim");
            dim.transform.SetParent(markothP4HelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new("MarkothP4HelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, MarkothP4HelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Markoth P4", 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 420f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = MarkothP4HelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            markothP4HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "MarkothP4EnableRow",
                "Enable Markoth P4",
                rowY,
                GetMarkothP4HelperEnabled,
                SetMarkothP4HelperEnabled,
                out markothP4HelperToggleValue,
                out markothP4HelperToggleIcon);

            rowY += MarkothP4HelperRowSpacing;
            CreateToggleRow(
                content,
                "MarkothP4UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.MarkothP4Helper.markothP4UseMaxHp,
                SetMarkothP4UseMaxHpEnabled,
                out markothP4UseMaxHpValue);
            lastY = rowY;

            rowY += MarkothP4HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MarkothP4MaxHpRow",
                "Markoth P4 Max HP",
                rowY,
                () => Modules.BossChallenge.MarkothP4Helper.markothP4MaxHp,
                SetMarkothP4MaxHp,
                1,
                999999,
                1,
                out markothP4MaxHpField);
            lastY = rowY;

            rowY += MarkothP4HelperRowSpacing;
            CreateToggleRow(
                content,
                "MarkothP4UseCustomPhaseRow",
                "Use Custom Phase",
                rowY,
                () => Modules.BossChallenge.MarkothP4Helper.markothP4UseCustomPhase,
                SetMarkothP4UseCustomPhaseEnabled,
                out markothP4UseCustomPhaseValue);
            lastY = rowY;

            rowY += MarkothP4HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MarkothP4Phase2HpRow",
                "Phase 2 HP",
                rowY,
                () => Modules.BossChallenge.MarkothP4Helper.markothP4Phase2Hp,
                SetMarkothP4Phase2Hp,
                1,
                999999,
                1,
                out markothP4Phase2HpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "MarkothP4ResetRow", "Reset Default", resetY, OnMarkothP4HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "MarkothP4BackRow", "Back", backY, OnMarkothP4HelperBackClicked);
        }

        private void BuildGorbP1HelperOverlayUi()
        {
            gorbP1HelperRoot = new GameObject("GorbP1HelperOverlayCanvas");
            gorbP1HelperRoot.transform.SetParent(transform, false);

            Canvas canvas = gorbP1HelperRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = GorbP1HelperCanvasSortOrder;

            CanvasScaler scaler = gorbP1HelperRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            gorbP1HelperRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = gorbP1HelperRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new("Dim");
            dim.transform.SetParent(gorbP1HelperRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new("GorbP1HelperPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, GorbP1HelperPanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "Gorb P1", 52, TextAnchor.MiddleCenter);
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.anchoredPosition = new Vector2(0f, 420f);
            titleRect.sizeDelta = new Vector2(RowWidth, 60f);

            float panelHeight = GorbP1HelperPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);
            gorbP1HelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "GorbP1EnableRow",
                "Enable Gorb P1",
                rowY,
                GetGorbP1HelperEnabled,
                SetGorbP1HelperEnabled,
                out gorbP1HelperToggleValue,
                out gorbP1HelperToggleIcon);

            rowY += GorbP1HelperRowSpacing;
            CreateToggleRow(
                content,
                "GorbP1UseMaxHpRow",
                "Use Max HP",
                rowY,
                () => Modules.BossChallenge.GorbP1Helper.gorbP1UseMaxHp,
                SetGorbP1UseMaxHpEnabled,
                out gorbP1UseMaxHpValue);
            lastY = rowY;

            rowY += GorbP1HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GorbP1MaxHpRow",
                "Gorb P1 Max HP",
                rowY,
                () => Modules.BossChallenge.GorbP1Helper.gorbP1MaxHp,
                SetGorbP1MaxHp,
                1,
                999999,
                1,
                out gorbP1MaxHpField);
            lastY = rowY;

            rowY += GorbP1HelperRowSpacing;
            CreateToggleRow(
                content,
                "GorbP1UseCustomPhaseRow",
                "Use Custom Phase",
                rowY,
                () => Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase,
                SetGorbP1UseCustomPhaseEnabled,
                out gorbP1UseCustomPhaseValue);
            lastY = rowY;

            rowY += GorbP1HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GorbP1Phase2HpRow",
                "Phase 2 HP",
                rowY,
                () => Modules.BossChallenge.GorbP1Helper.gorbP1Phase2Hp,
                SetGorbP1Phase2Hp,
                1,
                999999,
                1,
                out gorbP1Phase2HpField);
            lastY = rowY;

            rowY += GorbP1HelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GorbP1Phase3HpRow",
                "Phase 3 HP",
                rowY,
                () => Modules.BossChallenge.GorbP1Helper.gorbP1Phase3Hp,
                SetGorbP1Phase3Hp,
                1,
                999999,
                1,
                out gorbP1Phase3HpField);
            lastY = rowY;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);

            CreateButtonRow(panel.transform, "GorbP1ResetRow", "Reset Default", resetY, OnGorbP1HelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "GorbP1BackRow", "Back", backY, OnGorbP1HelperBackClicked);
        }

        private Module? GetGruzMotherP1HelperModule()
        {
            if (gruzMotherP1HelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.GruzMotherP1Helper), out gruzMotherP1HelperModule);
            }

            return gruzMotherP1HelperModule;
        }

        private bool GetGruzMotherP1HelperEnabled()
        {
            Module? module = GetGruzMotherP1HelperModule();
            return module?.Enabled ?? false;
        }

        private void SetGruzMotherP1HelperEnabled(bool value)
        {
            Module? module = GetGruzMotherP1HelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateGruzMotherP1HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetGruzMotherP1UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1UseMaxHp = value;
            Modules.BossChallenge.GruzMotherP1Helper.ReapplyLiveSettings();
            RefreshGruzMotherP1HelperUi();
        }

        private void SetGruzMotherP1MaxHp(int value)
        {
            Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1UseMaxHp)
            {
                Modules.BossChallenge.GruzMotherP1Helper.ApplyGruzHealthIfPresent();
            }
        }

        private Module? GetVengeflyKingP1HelperModule()
        {
            if (vengeflyKingP1HelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.VengeflyKingP1Helper), out vengeflyKingP1HelperModule);
            }

            return vengeflyKingP1HelperModule;
        }

        private bool GetVengeflyKingP1HelperEnabled()
        {
            Module? module = GetVengeflyKingP1HelperModule();
            return module?.Enabled ?? false;
        }

        private void SetVengeflyKingP1HelperEnabled(bool value)
        {
            Module? module = GetVengeflyKingP1HelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateVengeflyKingP1HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetVengeflyKingP1UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1UseMaxHp = value;
            Modules.BossChallenge.VengeflyKingP1Helper.ReapplyLiveSettings();
            RefreshVengeflyKingP1HelperUi();
        }

        private void SetVengeflyKingP1MaxHp(int value)
        {
            Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1UseMaxHp)
            {
                Modules.BossChallenge.VengeflyKingP1Helper.ApplyVengeflyHealthIfPresent();
            }
        }

        private Module? GetBroodingMawlekP1HelperModule()
        {
            if (broodingMawlekP1HelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.BroodingMawlekP1Helper), out broodingMawlekP1HelperModule);
            }

            return broodingMawlekP1HelperModule;
        }

        private bool GetBroodingMawlekP1HelperEnabled()
        {
            Module? module = GetBroodingMawlekP1HelperModule();
            return module?.Enabled ?? false;
        }

        private void SetBroodingMawlekP1HelperEnabled(bool value)
        {
            Module? module = GetBroodingMawlekP1HelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateBroodingMawlekP1HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetBroodingMawlekP1UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1UseMaxHp = value;
            Modules.BossChallenge.BroodingMawlekP1Helper.ReapplyLiveSettings();
            RefreshBroodingMawlekP1HelperUi();
        }

        private void SetBroodingMawlekP1MaxHp(int value)
        {
            Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1UseMaxHp)
            {
                Modules.BossChallenge.BroodingMawlekP1Helper.ApplyMawlekHealthIfPresent();
            }
        }

        private Module? GetNoskP2HelperModule()
        {
            if (noskP2HelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.NoskP2Helper), out noskP2HelperModule);
            }

            return noskP2HelperModule;
        }

        private bool GetNoskP2HelperEnabled()
        {
            Module? module = GetNoskP2HelperModule();
            return module?.Enabled ?? false;
        }

        private void SetNoskP2HelperEnabled(bool value)
        {
            Module? module = GetNoskP2HelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateNoskP2HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetNoskP2UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.NoskP2Helper.noskP2UseMaxHp = value;
            Modules.BossChallenge.NoskP2Helper.ReapplyLiveSettings();
            RefreshNoskP2HelperUi();
        }

        private void SetNoskP2MaxHp(int value)
        {
            Modules.BossChallenge.NoskP2Helper.noskP2MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.NoskP2Helper.noskP2UseMaxHp)
            {
                Modules.BossChallenge.NoskP2Helper.ApplyNoskHealthIfPresent();
            }

            Modules.BossChallenge.NoskP2Helper.noskP2Phase2Hp = Mathf.Clamp(
                Modules.BossChallenge.NoskP2Helper.noskP2Phase2Hp,
                1,
                GetNoskP2Phase2MaxHp());
            if (Modules.BossChallenge.NoskP2Helper.noskP2UseCustomPhase)
            {
                Modules.BossChallenge.NoskP2Helper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshNoskP2HelperUi();
        }

        private void SetNoskP2UseCustomPhaseEnabled(bool value)
        {
            Modules.BossChallenge.NoskP2Helper.noskP2Phase2Hp = Mathf.Clamp(
                Modules.BossChallenge.NoskP2Helper.noskP2Phase2Hp,
                1,
                GetNoskP2Phase2MaxHp());
            Modules.BossChallenge.NoskP2Helper.noskP2UseCustomPhase = value;
            Modules.BossChallenge.NoskP2Helper.ReapplyLiveSettings();
            RefreshNoskP2HelperUi();
        }

        private void SetNoskP2Phase2Hp(int value)
        {
            Modules.BossChallenge.NoskP2Helper.noskP2Phase2Hp = Mathf.Clamp(value, 1, GetNoskP2Phase2MaxHp());
            if (Modules.BossChallenge.NoskP2Helper.noskP2UseCustomPhase)
            {
                Modules.BossChallenge.NoskP2Helper.ApplyPhaseThresholdSettingsIfPresent();
            }

            RefreshNoskP2HelperUi();
        }

        private int GetNoskP2Phase2MaxHp()
        {
            return Mathf.Max(1, Modules.BossChallenge.NoskP2Helper.GetPhase2MaxHpForUi());
        }

        private Module? GetUumuuP3HelperModule()
        {
            if (uumuuP3HelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.UumuuP3Helper), out uumuuP3HelperModule);
            }

            return uumuuP3HelperModule;
        }

        private bool GetUumuuP3HelperEnabled()
        {
            Module? module = GetUumuuP3HelperModule();
            return module?.Enabled ?? false;
        }

        private void SetUumuuP3HelperEnabled(bool value)
        {
            Module? module = GetUumuuP3HelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateUumuuP3HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetUumuuP3UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.UumuuP3Helper.uumuuP3UseMaxHp = value;
            Modules.BossChallenge.UumuuP3Helper.ReapplyLiveSettings();
            RefreshUumuuP3HelperUi();
        }

        private void SetUumuuP3MaxHp(int value)
        {
            Modules.BossChallenge.UumuuP3Helper.uumuuP3MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.UumuuP3Helper.uumuuP3UseMaxHp)
            {
                Modules.BossChallenge.UumuuP3Helper.ApplyUumuuHealthIfPresent();
            }
        }

        private void SetUumuuP3UseCustomSummonHpEnabled(bool value)
        {
            Modules.BossChallenge.UumuuP3Helper.uumuuP3UseCustomSummonHp = value;
            Modules.BossChallenge.UumuuP3Helper.ReapplyLiveSettings();
            RefreshUumuuP3HelperUi();
        }

        private void SetUumuuP3SummonHp(int value)
        {
            Modules.BossChallenge.UumuuP3Helper.uumuuP3SummonHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.UumuuP3Helper.uumuuP3UseCustomSummonHp)
            {
                Modules.BossChallenge.UumuuP3Helper.ApplyUumuuSummonHealthIfPresent();
            }
        }

        private Module? GetSoulWarriorP1HelperModule()
        {
            if (soulWarriorP1HelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.SoulWarriorP1Helper), out soulWarriorP1HelperModule);
            }

            return soulWarriorP1HelperModule;
        }

        private bool GetSoulWarriorP1HelperEnabled()
        {
            Module? module = GetSoulWarriorP1HelperModule();
            return module?.Enabled ?? false;
        }

        private void SetSoulWarriorP1HelperEnabled(bool value)
        {
            Module? module = GetSoulWarriorP1HelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateSoulWarriorP1HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetSoulWarriorP1UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1UseMaxHp = value;
            Modules.BossChallenge.SoulWarriorP1Helper.ReapplyLiveSettings();
            RefreshSoulWarriorP1HelperUi();
        }

        private void SetSoulWarriorP1MaxHp(int value)
        {
            Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1UseMaxHp)
            {
                Modules.BossChallenge.SoulWarriorP1Helper.ApplySoulWarriorHealthIfPresent();
            }
        }

        private Module? GetNoEyesP4HelperModule()
        {
            if (noEyesP4HelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.NoEyesP4Helper), out noEyesP4HelperModule);
            }

            return noEyesP4HelperModule;
        }

        private bool GetNoEyesP4HelperEnabled()
        {
            Module? module = GetNoEyesP4HelperModule();
            return module?.Enabled ?? false;
        }

        private void SetNoEyesP4HelperEnabled(bool value)
        {
            Module? module = GetNoEyesP4HelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateNoEyesP4HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetNoEyesP4UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseMaxHp = value;
            Modules.BossChallenge.NoEyesP4Helper.ReapplyLiveSettings();
            RefreshNoEyesP4HelperUi();
        }

        private void SetNoEyesP4MaxHp(int value)
        {
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4MaxHp = Mathf.Clamp(value, 1, 999999);
            NormalizeNoEyesP4PhaseThresholdInputs();
            if (Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseMaxHp || Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase)
            {
                Modules.BossChallenge.NoEyesP4Helper.ReapplyLiveSettings();
            }

            RefreshNoEyesP4HelperUi();
        }

        private void SetNoEyesP4UseCustomPhaseEnabled(bool value)
        {
            NormalizeNoEyesP4PhaseThresholdInputs();
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase = value;
            Modules.BossChallenge.NoEyesP4Helper.ReapplyLiveSettings();
            RefreshNoEyesP4HelperUi();
        }

        private void SetNoEyesP4Phase2Hp(int value)
        {
            int phase2Hp = Mathf.Clamp(value, 2, GetNoEyesP4Phase2MaxHp());
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase2Hp = phase2Hp;
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase3Hp =
                Mathf.Clamp(Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase3Hp, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase)
            {
                Modules.BossChallenge.NoEyesP4Helper.ReapplyLiveSettings();
            }

            RefreshNoEyesP4HelperUi();
        }

        private void SetNoEyesP4Phase3Hp(int value)
        {
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase2Hp, 2, GetNoEyesP4Phase2MaxHp());
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase2Hp = phase2Hp;
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase3Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase)
            {
                Modules.BossChallenge.NoEyesP4Helper.ReapplyLiveSettings();
            }

            RefreshNoEyesP4HelperUi();
        }

        private int GetNoEyesP4Phase2MaxHp()
        {
            return Mathf.Clamp(Modules.BossChallenge.NoEyesP4Helper.GetPhase2MaxHpForUi(), 2, 999999);
        }

        private void NormalizeNoEyesP4PhaseThresholdInputs()
        {
            int phase2MaxHp = GetNoEyesP4Phase2MaxHp();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase2Hp, 2, phase2MaxHp);
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase2Hp = phase2Hp;
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase3Hp =
                Mathf.Clamp(Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
        }

        private Module? GetMarmuP2HelperModule()
        {
            if (marmuP2HelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.MarmuP2Helper), out marmuP2HelperModule);
            }

            return marmuP2HelperModule;
        }

        private bool GetMarmuP2HelperEnabled()
        {
            Module? module = GetMarmuP2HelperModule();
            return module?.Enabled ?? false;
        }

        private void SetMarmuP2HelperEnabled(bool value)
        {
            Module? module = GetMarmuP2HelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateMarmuP2HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetMarmuP2UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.MarmuP2Helper.marmuP2UseMaxHp = value;
            Modules.BossChallenge.MarmuP2Helper.ReapplyLiveSettings();
            RefreshMarmuP2HelperUi();
        }

        private void SetMarmuP2MaxHp(int value)
        {
            Modules.BossChallenge.MarmuP2Helper.marmuP2MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.MarmuP2Helper.marmuP2UseMaxHp)
            {
                Modules.BossChallenge.MarmuP2Helper.ApplyMarmuHealthIfPresent();
            }
        }

        private Module? GetXeroP2HelperModule()
        {
            if (xeroP2HelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.XeroP2Helper), out xeroP2HelperModule);
            }

            return xeroP2HelperModule;
        }

        private bool GetXeroP2HelperEnabled()
        {
            Module? module = GetXeroP2HelperModule();
            return module?.Enabled ?? false;
        }

        private void SetXeroP2HelperEnabled(bool value)
        {
            Module? module = GetXeroP2HelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateXeroP2HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetXeroP2UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.XeroP2Helper.xeroP2UseMaxHp = value;
            Modules.BossChallenge.XeroP2Helper.ReapplyLiveSettings();
            RefreshXeroP2HelperUi();
        }

        private void SetXeroP2MaxHp(int value)
        {
            Modules.BossChallenge.XeroP2Helper.xeroP2MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.XeroP2Helper.xeroP2UseMaxHp)
            {
                Modules.BossChallenge.XeroP2Helper.ApplyXeroHealthIfPresent();
            }

            Modules.BossChallenge.XeroP2Helper.xeroP2Phase2Hp = Mathf.Clamp(
                Modules.BossChallenge.XeroP2Helper.xeroP2Phase2Hp,
                1,
                GetXeroP2Phase2MaxHp());
            if (Modules.BossChallenge.XeroP2Helper.xeroP2UseCustomPhase)
            {
                Modules.BossChallenge.XeroP2Helper.ReapplyLiveSettings();
            }

            RefreshXeroP2HelperUi();
        }

        private void SetXeroP2UseCustomPhaseEnabled(bool value)
        {
            Modules.BossChallenge.XeroP2Helper.xeroP2Phase2Hp = Mathf.Clamp(
                Modules.BossChallenge.XeroP2Helper.xeroP2Phase2Hp,
                1,
                GetXeroP2Phase2MaxHp());
            Modules.BossChallenge.XeroP2Helper.xeroP2UseCustomPhase = value;
            Modules.BossChallenge.XeroP2Helper.ReapplyLiveSettings();
            RefreshXeroP2HelperUi();
        }

        private void SetXeroP2Phase2Hp(int value)
        {
            Modules.BossChallenge.XeroP2Helper.xeroP2Phase2Hp = Mathf.Clamp(value, 1, GetXeroP2Phase2MaxHp());
            if (Modules.BossChallenge.XeroP2Helper.xeroP2UseCustomPhase)
            {
                Modules.BossChallenge.XeroP2Helper.ReapplyLiveSettings();
            }

            RefreshXeroP2HelperUi();
        }

        private int GetXeroP2Phase2MaxHp()
        {
            return Mathf.Max(1, Modules.BossChallenge.XeroP2Helper.GetPhase2MaxHpForUi());
        }

        private Module? GetMarkothP4HelperModule()
        {
            if (markothP4HelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.MarkothP4Helper), out markothP4HelperModule);
            }

            return markothP4HelperModule;
        }

        private bool GetMarkothP4HelperEnabled()
        {
            Module? module = GetMarkothP4HelperModule();
            return module?.Enabled ?? false;
        }

        private void SetMarkothP4HelperEnabled(bool value)
        {
            Module? module = GetMarkothP4HelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateMarkothP4HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetMarkothP4UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.MarkothP4Helper.markothP4UseMaxHp = value;
            Modules.BossChallenge.MarkothP4Helper.ReapplyLiveSettings();
            RefreshMarkothP4HelperUi();
        }

        private void SetMarkothP4MaxHp(int value)
        {
            Modules.BossChallenge.MarkothP4Helper.markothP4MaxHp = Mathf.Clamp(value, 1, 999999);
            if (Modules.BossChallenge.MarkothP4Helper.markothP4UseMaxHp)
            {
                Modules.BossChallenge.MarkothP4Helper.ApplyMarkothHealthIfPresent();
            }

            Modules.BossChallenge.MarkothP4Helper.markothP4Phase2Hp = Mathf.Clamp(
                Modules.BossChallenge.MarkothP4Helper.markothP4Phase2Hp,
                1,
                GetMarkothP4Phase2MaxHp());
            if (Modules.BossChallenge.MarkothP4Helper.markothP4UseCustomPhase)
            {
                Modules.BossChallenge.MarkothP4Helper.ReapplyLiveSettings();
            }

            RefreshMarkothP4HelperUi();
        }

        private void SetMarkothP4UseCustomPhaseEnabled(bool value)
        {
            Modules.BossChallenge.MarkothP4Helper.markothP4Phase2Hp = Mathf.Clamp(
                Modules.BossChallenge.MarkothP4Helper.markothP4Phase2Hp,
                1,
                GetMarkothP4Phase2MaxHp());
            Modules.BossChallenge.MarkothP4Helper.markothP4UseCustomPhase = value;
            Modules.BossChallenge.MarkothP4Helper.ReapplyLiveSettings();
            RefreshMarkothP4HelperUi();
        }

        private void SetMarkothP4Phase2Hp(int value)
        {
            Modules.BossChallenge.MarkothP4Helper.markothP4Phase2Hp = Mathf.Clamp(value, 1, GetMarkothP4Phase2MaxHp());
            if (Modules.BossChallenge.MarkothP4Helper.markothP4UseCustomPhase)
            {
                Modules.BossChallenge.MarkothP4Helper.ReapplyLiveSettings();
            }

            RefreshMarkothP4HelperUi();
        }

        private int GetMarkothP4Phase2MaxHp()
        {
            return Mathf.Max(1, Modules.BossChallenge.MarkothP4Helper.GetPhase2MaxHpForUi());
        }

        private Module? GetGorbP1HelperModule()
        {
            if (gorbP1HelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.GorbP1Helper), out gorbP1HelperModule);
            }

            return gorbP1HelperModule;
        }

        private bool GetGorbP1HelperEnabled()
        {
            Module? module = GetGorbP1HelperModule();
            return module?.Enabled ?? false;
        }

        private void SetGorbP1HelperEnabled(bool value)
        {
            Module? module = GetGorbP1HelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateGorbP1HelperInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetGorbP1UseMaxHpEnabled(bool value)
        {
            Modules.BossChallenge.GorbP1Helper.gorbP1UseMaxHp = value;
            Modules.BossChallenge.GorbP1Helper.ReapplyLiveSettings();
            RefreshGorbP1HelperUi();
        }

        private void SetGorbP1MaxHp(int value)
        {
            Modules.BossChallenge.GorbP1Helper.gorbP1MaxHp = Mathf.Clamp(value, 1, 999999);
            NormalizeGorbP1PhaseThresholdInputs();
            if (Modules.BossChallenge.GorbP1Helper.gorbP1UseMaxHp)
            {
                Modules.BossChallenge.GorbP1Helper.ApplyGorbHealthIfPresent();
            }

            if (Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase)
            {
                Modules.BossChallenge.GorbP1Helper.ReapplyLiveSettings();
            }

            RefreshGorbP1HelperUi();
        }

        private void SetGorbP1UseCustomPhaseEnabled(bool value)
        {
            NormalizeGorbP1PhaseThresholdInputs();
            Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase = value;
            Modules.BossChallenge.GorbP1Helper.ReapplyLiveSettings();
            RefreshGorbP1HelperUi();
        }

        private void SetGorbP1Phase2Hp(int value)
        {
            int phase2Hp = Mathf.Clamp(value, 2, GetGorbP1Phase2MaxHp());
            Modules.BossChallenge.GorbP1Helper.gorbP1Phase2Hp = phase2Hp;
            Modules.BossChallenge.GorbP1Helper.gorbP1Phase3Hp =
                Mathf.Clamp(Modules.BossChallenge.GorbP1Helper.gorbP1Phase3Hp, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase)
            {
                Modules.BossChallenge.GorbP1Helper.ReapplyLiveSettings();
            }

            RefreshGorbP1HelperUi();
        }

        private void SetGorbP1Phase3Hp(int value)
        {
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.GorbP1Helper.gorbP1Phase2Hp, 2, GetGorbP1Phase2MaxHp());
            Modules.BossChallenge.GorbP1Helper.gorbP1Phase2Hp = phase2Hp;
            Modules.BossChallenge.GorbP1Helper.gorbP1Phase3Hp = Mathf.Clamp(value, 1, Mathf.Max(1, phase2Hp - 1));

            if (Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase)
            {
                Modules.BossChallenge.GorbP1Helper.ReapplyLiveSettings();
            }

            RefreshGorbP1HelperUi();
        }

        private int GetGorbP1Phase2MaxHp()
        {
            return Mathf.Clamp(Modules.BossChallenge.GorbP1Helper.GetPhase2MaxHpForUi(), 2, 999999);
        }

        private void NormalizeGorbP1PhaseThresholdInputs()
        {
            int phase2MaxHp = GetGorbP1Phase2MaxHp();
            int phase2Hp = Mathf.Clamp(Modules.BossChallenge.GorbP1Helper.gorbP1Phase2Hp, 2, phase2MaxHp);
            Modules.BossChallenge.GorbP1Helper.gorbP1Phase2Hp = phase2Hp;
            Modules.BossChallenge.GorbP1Helper.gorbP1Phase3Hp =
                Mathf.Clamp(Modules.BossChallenge.GorbP1Helper.gorbP1Phase3Hp, 1, Mathf.Max(1, phase2Hp - 1));
        }

        private void RefreshGruzMotherP1HelperUi()
        {
            Module? module = GetGruzMotherP1HelperModule();
            UpdateToggleValue(gruzMotherP1HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(gruzMotherP1HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(gruzMotherP1UseMaxHpValue, Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1UseMaxHp);
            UpdateIntInputValue(gruzMotherP1MaxHpField, Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1MaxHp);

            UpdateGruzMotherP1HelperInteractivity();
        }

        private void RefreshVengeflyKingP1HelperUi()
        {
            Module? module = GetVengeflyKingP1HelperModule();
            UpdateToggleValue(vengeflyKingP1HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(vengeflyKingP1HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(vengeflyKingP1UseMaxHpValue, Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1UseMaxHp);
            UpdateIntInputValue(vengeflyKingP1MaxHpField, Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1MaxHp);

            UpdateVengeflyKingP1HelperInteractivity();
        }

        private void RefreshBroodingMawlekP1HelperUi()
        {
            Module? module = GetBroodingMawlekP1HelperModule();
            UpdateToggleValue(broodingMawlekP1HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(broodingMawlekP1HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(broodingMawlekP1UseMaxHpValue, Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1UseMaxHp);
            UpdateIntInputValue(broodingMawlekP1MaxHpField, Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1MaxHp);

            UpdateBroodingMawlekP1HelperInteractivity();
        }

        private void RefreshNoskP2HelperUi()
        {
            Module? module = GetNoskP2HelperModule();
            UpdateToggleValue(noskP2HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(noskP2HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(noskP2UseMaxHpValue, Modules.BossChallenge.NoskP2Helper.noskP2UseMaxHp);
            UpdateToggleValue(noskP2UseCustomPhaseValue, Modules.BossChallenge.NoskP2Helper.noskP2UseCustomPhase);
            UpdateIntInputValue(noskP2MaxHpField, Modules.BossChallenge.NoskP2Helper.noskP2MaxHp);
            UpdateIntInputValue(noskP2Phase2HpField, Modules.BossChallenge.NoskP2Helper.noskP2Phase2Hp);

            UpdateNoskP2HelperInteractivity();
        }

        private void RefreshUumuuP3HelperUi()
        {
            Module? module = GetUumuuP3HelperModule();
            UpdateToggleValue(uumuuP3HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(uumuuP3HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(uumuuP3UseMaxHpValue, Modules.BossChallenge.UumuuP3Helper.uumuuP3UseMaxHp);
            UpdateIntInputValue(uumuuP3MaxHpField, Modules.BossChallenge.UumuuP3Helper.uumuuP3MaxHp);
            UpdateToggleValue(uumuuP3UseCustomSummonHpValue, Modules.BossChallenge.UumuuP3Helper.uumuuP3UseCustomSummonHp);
            UpdateIntInputValue(uumuuP3SummonHpField, Modules.BossChallenge.UumuuP3Helper.uumuuP3SummonHp);

            UpdateUumuuP3HelperInteractivity();
        }

        private void RefreshSoulWarriorP1HelperUi()
        {
            Module? module = GetSoulWarriorP1HelperModule();
            UpdateToggleValue(soulWarriorP1HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(soulWarriorP1HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(soulWarriorP1UseMaxHpValue, Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1UseMaxHp);
            UpdateIntInputValue(soulWarriorP1MaxHpField, Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1MaxHp);

            UpdateSoulWarriorP1HelperInteractivity();
        }

        private void RefreshNoEyesP4HelperUi()
        {
            Module? module = GetNoEyesP4HelperModule();
            UpdateToggleValue(noEyesP4HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(noEyesP4HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(noEyesP4UseMaxHpValue, Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseMaxHp);
            UpdateToggleValue(noEyesP4UseCustomPhaseValue, Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase);
            UpdateIntInputValue(noEyesP4MaxHpField, Modules.BossChallenge.NoEyesP4Helper.noEyesP4MaxHp);
            UpdateIntInputValue(noEyesP4Phase2HpField, Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase2Hp);
            UpdateIntInputValue(noEyesP4Phase3HpField, Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase3Hp);

            UpdateNoEyesP4HelperInteractivity();
        }

        private void RefreshMarmuP2HelperUi()
        {
            Module? module = GetMarmuP2HelperModule();
            UpdateToggleValue(marmuP2HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(marmuP2HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(marmuP2UseMaxHpValue, Modules.BossChallenge.MarmuP2Helper.marmuP2UseMaxHp);
            UpdateIntInputValue(marmuP2MaxHpField, Modules.BossChallenge.MarmuP2Helper.marmuP2MaxHp);

            UpdateMarmuP2HelperInteractivity();
        }

        private void RefreshXeroP2HelperUi()
        {
            Module? module = GetXeroP2HelperModule();
            UpdateToggleValue(xeroP2HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(xeroP2HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(xeroP2UseMaxHpValue, Modules.BossChallenge.XeroP2Helper.xeroP2UseMaxHp);
            UpdateToggleValue(xeroP2UseCustomPhaseValue, Modules.BossChallenge.XeroP2Helper.xeroP2UseCustomPhase);
            UpdateIntInputValue(xeroP2MaxHpField, Modules.BossChallenge.XeroP2Helper.xeroP2MaxHp);
            UpdateIntInputValue(xeroP2Phase2HpField, Modules.BossChallenge.XeroP2Helper.xeroP2Phase2Hp);

            UpdateXeroP2HelperInteractivity();
        }

        private void RefreshMarkothP4HelperUi()
        {
            Module? module = GetMarkothP4HelperModule();
            UpdateToggleValue(markothP4HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(markothP4HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(markothP4UseMaxHpValue, Modules.BossChallenge.MarkothP4Helper.markothP4UseMaxHp);
            UpdateToggleValue(markothP4UseCustomPhaseValue, Modules.BossChallenge.MarkothP4Helper.markothP4UseCustomPhase);
            UpdateIntInputValue(markothP4MaxHpField, Modules.BossChallenge.MarkothP4Helper.markothP4MaxHp);
            UpdateIntInputValue(markothP4Phase2HpField, Modules.BossChallenge.MarkothP4Helper.markothP4Phase2Hp);

            UpdateMarkothP4HelperInteractivity();
        }

        private void RefreshGorbP1HelperUi()
        {
            Module? module = GetGorbP1HelperModule();
            UpdateToggleValue(gorbP1HelperToggleValue, module?.Enabled ?? false);
            UpdateToggleIcon(gorbP1HelperToggleIcon, module?.Enabled ?? false);
            UpdateToggleValue(gorbP1UseMaxHpValue, Modules.BossChallenge.GorbP1Helper.gorbP1UseMaxHp);
            UpdateToggleValue(gorbP1UseCustomPhaseValue, Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase);
            UpdateIntInputValue(gorbP1MaxHpField, Modules.BossChallenge.GorbP1Helper.gorbP1MaxHp);
            UpdateIntInputValue(gorbP1Phase2HpField, Modules.BossChallenge.GorbP1Helper.gorbP1Phase2Hp);
            UpdateIntInputValue(gorbP1Phase3HpField, Modules.BossChallenge.GorbP1Helper.gorbP1Phase3Hp);

            UpdateGorbP1HelperInteractivity();
        }

        private void UpdateGruzMotherP1HelperInteractivity()
        {
            SetContentInteractivity(gruzMotherP1HelperContent, GetGruzMotherP1HelperEnabled(), "GruzMotherP1EnableRow");
            if (!GetGruzMotherP1HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(gruzMotherP1HelperContent, "GruzMotherP1MaxHpRow", Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1UseMaxHp);
        }

        private void UpdateVengeflyKingP1HelperInteractivity()
        {
            SetContentInteractivity(vengeflyKingP1HelperContent, GetVengeflyKingP1HelperEnabled(), "VengeflyKingP1EnableRow");
            if (!GetVengeflyKingP1HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(vengeflyKingP1HelperContent, "VengeflyKingP1MaxHpRow", Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1UseMaxHp);
        }

        private void UpdateBroodingMawlekP1HelperInteractivity()
        {
            SetContentInteractivity(broodingMawlekP1HelperContent, GetBroodingMawlekP1HelperEnabled(), "BroodingMawlekP1EnableRow");
            if (!GetBroodingMawlekP1HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(broodingMawlekP1HelperContent, "BroodingMawlekP1MaxHpRow", Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1UseMaxHp);
        }

        private void UpdateNoskP2HelperInteractivity()
        {
            SetContentInteractivity(noskP2HelperContent, GetNoskP2HelperEnabled(), "NoskP2EnableRow");
            if (!GetNoskP2HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(noskP2HelperContent, "NoskP2MaxHpRow", Modules.BossChallenge.NoskP2Helper.noskP2UseMaxHp);
            SetRowInteractivity(noskP2HelperContent, "NoskP2Phase2HpRow", Modules.BossChallenge.NoskP2Helper.noskP2UseCustomPhase);
        }

        private void UpdateUumuuP3HelperInteractivity()
        {
            SetContentInteractivity(uumuuP3HelperContent, GetUumuuP3HelperEnabled(), "UumuuP3EnableRow");
            if (!GetUumuuP3HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(uumuuP3HelperContent, "UumuuP3MaxHpRow", Modules.BossChallenge.UumuuP3Helper.uumuuP3UseMaxHp);
            SetRowInteractivity(uumuuP3HelperContent, "UumuuP3SummonHpRow", Modules.BossChallenge.UumuuP3Helper.uumuuP3UseCustomSummonHp);
        }

        private void UpdateSoulWarriorP1HelperInteractivity()
        {
            SetContentInteractivity(soulWarriorP1HelperContent, GetSoulWarriorP1HelperEnabled(), "SoulWarriorP1EnableRow");
            if (!GetSoulWarriorP1HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(soulWarriorP1HelperContent, "SoulWarriorP1MaxHpRow", Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1UseMaxHp);
        }

        private void UpdateNoEyesP4HelperInteractivity()
        {
            SetContentInteractivity(noEyesP4HelperContent, GetNoEyesP4HelperEnabled(), "NoEyesP4EnableRow");
            if (!GetNoEyesP4HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(noEyesP4HelperContent, "NoEyesP4MaxHpRow", Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseMaxHp);
            SetRowInteractivity(noEyesP4HelperContent, "NoEyesP4Phase2HpRow", Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase);
            SetRowInteractivity(noEyesP4HelperContent, "NoEyesP4Phase3HpRow", Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase);
        }

        private void UpdateMarmuP2HelperInteractivity()
        {
            SetContentInteractivity(marmuP2HelperContent, GetMarmuP2HelperEnabled(), "MarmuP2EnableRow");
            if (!GetMarmuP2HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(marmuP2HelperContent, "MarmuP2MaxHpRow", Modules.BossChallenge.MarmuP2Helper.marmuP2UseMaxHp);
        }

        private void UpdateXeroP2HelperInteractivity()
        {
            SetContentInteractivity(xeroP2HelperContent, GetXeroP2HelperEnabled(), "XeroP2EnableRow");
            if (!GetXeroP2HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(xeroP2HelperContent, "XeroP2MaxHpRow", Modules.BossChallenge.XeroP2Helper.xeroP2UseMaxHp);
            SetRowInteractivity(xeroP2HelperContent, "XeroP2Phase2HpRow", Modules.BossChallenge.XeroP2Helper.xeroP2UseCustomPhase);
        }

        private void UpdateMarkothP4HelperInteractivity()
        {
            SetContentInteractivity(markothP4HelperContent, GetMarkothP4HelperEnabled(), "MarkothP4EnableRow");
            if (!GetMarkothP4HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(markothP4HelperContent, "MarkothP4MaxHpRow", Modules.BossChallenge.MarkothP4Helper.markothP4UseMaxHp);
            SetRowInteractivity(markothP4HelperContent, "MarkothP4Phase2HpRow", Modules.BossChallenge.MarkothP4Helper.markothP4UseCustomPhase);
        }

        private void UpdateGorbP1HelperInteractivity()
        {
            SetContentInteractivity(gorbP1HelperContent, GetGorbP1HelperEnabled(), "GorbP1EnableRow");
            if (!GetGorbP1HelperEnabled())
            {
                return;
            }

            SetRowInteractivity(gorbP1HelperContent, "GorbP1MaxHpRow", Modules.BossChallenge.GorbP1Helper.gorbP1UseMaxHp);
            SetRowInteractivity(gorbP1HelperContent, "GorbP1Phase2HpRow", Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase);
            SetRowInteractivity(gorbP1HelperContent, "GorbP1Phase3HpRow", Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase);
        }

        private void SetBossManipulateOtherRoomsVisible(bool value)
        {
            bossManipulateOtherRoomsVisible = value;
            if (bossManipulateOtherRoomsRoot != null)
            {
                bossManipulateOtherRoomsRoot.SetActive(value);
            }

            if (value)
            {
                RefreshBossManipulateCardVisuals();
                SetBossManipulateOtherRoomsResetConfirmVisible(false);
            }
            else
            {
                SetBossManipulateOtherRoomsResetConfirmVisible(false);
            }

            UpdateUiState();
        }

        private void SetGruzMotherP1HelperVisible(bool value)
        {
            gruzMotherP1HelperVisible = value;
            if (gruzMotherP1HelperRoot != null)
            {
                gruzMotherP1HelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshGruzMotherP1HelperUi();
            }

            UpdateUiState();
        }

        private void SetVengeflyKingP1HelperVisible(bool value)
        {
            vengeflyKingP1HelperVisible = value;
            if (vengeflyKingP1HelperRoot != null)
            {
                vengeflyKingP1HelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshVengeflyKingP1HelperUi();
            }

            UpdateUiState();
        }

        private void SetBroodingMawlekP1HelperVisible(bool value)
        {
            broodingMawlekP1HelperVisible = value;
            if (broodingMawlekP1HelperRoot != null)
            {
                broodingMawlekP1HelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshBroodingMawlekP1HelperUi();
            }

            UpdateUiState();
        }

        private void SetNoskP2HelperVisible(bool value)
        {
            noskP2HelperVisible = value;
            if (noskP2HelperRoot != null)
            {
                noskP2HelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshNoskP2HelperUi();
            }

            UpdateUiState();
        }

        private void SetUumuuP3HelperVisible(bool value)
        {
            uumuuP3HelperVisible = value;
            if (uumuuP3HelperRoot != null)
            {
                uumuuP3HelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshUumuuP3HelperUi();
            }

            UpdateUiState();
        }

        private void SetSoulWarriorP1HelperVisible(bool value)
        {
            soulWarriorP1HelperVisible = value;
            if (soulWarriorP1HelperRoot != null)
            {
                soulWarriorP1HelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshSoulWarriorP1HelperUi();
            }

            UpdateUiState();
        }

        private void SetNoEyesP4HelperVisible(bool value)
        {
            noEyesP4HelperVisible = value;
            if (noEyesP4HelperRoot != null)
            {
                noEyesP4HelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshNoEyesP4HelperUi();
            }

            UpdateUiState();
        }

        private void SetMarmuP2HelperVisible(bool value)
        {
            marmuP2HelperVisible = value;
            if (marmuP2HelperRoot != null)
            {
                marmuP2HelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshMarmuP2HelperUi();
            }

            UpdateUiState();
        }

        private void SetXeroP2HelperVisible(bool value)
        {
            xeroP2HelperVisible = value;
            if (xeroP2HelperRoot != null)
            {
                xeroP2HelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshXeroP2HelperUi();
            }

            UpdateUiState();
        }

        private void SetMarkothP4HelperVisible(bool value)
        {
            markothP4HelperVisible = value;
            if (markothP4HelperRoot != null)
            {
                markothP4HelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshMarkothP4HelperUi();
            }

            UpdateUiState();
        }

        private void SetGorbP1HelperVisible(bool value)
        {
            gorbP1HelperVisible = value;
            if (gorbP1HelperRoot != null)
            {
                gorbP1HelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshGorbP1HelperUi();
            }

            UpdateUiState();
        }

        private static void ResetGruzMotherP1HelperDefaults()
        {
            Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1UseMaxHp = false;
            Modules.BossChallenge.GruzMotherP1Helper.gruzMotherP1MaxHp = 650;
        }

        private static void ResetVengeflyKingP1HelperDefaults()
        {
            Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1UseMaxHp = false;
            Modules.BossChallenge.VengeflyKingP1Helper.vengeflyKingP1MaxHp = 450;
        }

        private static void ResetBroodingMawlekP1HelperDefaults()
        {
            Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1UseMaxHp = false;
            Modules.BossChallenge.BroodingMawlekP1Helper.broodingMawlekP1MaxHp = 1050;
        }

        private static void ResetNoskP2HelperDefaults()
        {
            Modules.BossChallenge.NoskP2Helper.noskP2UseMaxHp = false;
            Modules.BossChallenge.NoskP2Helper.noskP2MaxHp = 680;
            Modules.BossChallenge.NoskP2Helper.noskP2UseCustomPhase = false;
            Modules.BossChallenge.NoskP2Helper.noskP2Phase2Hp = 560;
        }

        private static void ResetUumuuP3HelperDefaults()
        {
            Modules.BossChallenge.UumuuP3Helper.uumuuP3UseMaxHp = false;
            Modules.BossChallenge.UumuuP3Helper.uumuuP3MaxHp = 350;
            Modules.BossChallenge.UumuuP3Helper.uumuuP3UseCustomSummonHp = false;
            Modules.BossChallenge.UumuuP3Helper.uumuuP3SummonHp = 1;
        }

        private static void ResetSoulWarriorP1HelperDefaults()
        {
            Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1UseMaxHp = false;
            Modules.BossChallenge.SoulWarriorP1Helper.soulWarriorP1MaxHp = 750;
        }

        private static void ResetNoEyesP4HelperDefaults()
        {
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseMaxHp = false;
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4MaxHp = 570;
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4UseCustomPhase = false;
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase2Hp = 150;
            Modules.BossChallenge.NoEyesP4Helper.noEyesP4Phase3Hp = 90;
        }

        private static void ResetMarmuP2HelperDefaults()
        {
            Modules.BossChallenge.MarmuP2Helper.marmuP2UseMaxHp = false;
            Modules.BossChallenge.MarmuP2Helper.marmuP2MaxHp = 416;
        }

        private static void ResetXeroP2HelperDefaults()
        {
            Modules.BossChallenge.XeroP2Helper.xeroP2UseMaxHp = false;
            Modules.BossChallenge.XeroP2Helper.xeroP2MaxHp = 650;
            Modules.BossChallenge.XeroP2Helper.xeroP2UseCustomPhase = false;
            Modules.BossChallenge.XeroP2Helper.xeroP2Phase2Hp = 325;
        }

        private static void ResetMarkothP4HelperDefaults()
        {
            Modules.BossChallenge.MarkothP4Helper.markothP4UseMaxHp = false;
            Modules.BossChallenge.MarkothP4Helper.markothP4MaxHp = 650;
            Modules.BossChallenge.MarkothP4Helper.markothP4UseCustomPhase = false;
            Modules.BossChallenge.MarkothP4Helper.markothP4Phase2Hp = 325;
        }

        private static void ResetGorbP1HelperDefaults()
        {
            Modules.BossChallenge.GorbP1Helper.gorbP1UseMaxHp = false;
            Modules.BossChallenge.GorbP1Helper.gorbP1MaxHp = 650;
            Modules.BossChallenge.GorbP1Helper.gorbP1UseCustomPhase = false;
            Modules.BossChallenge.GorbP1Helper.gorbP1Phase2Hp = 455;
            Modules.BossChallenge.GorbP1Helper.gorbP1Phase3Hp = 260;
        }

        private void OnBossManipulateOtherRoomsClicked()
        {
            returnToBossManipulateOnClose = true;
            SetBossManipulateVisible(false);
            SetBossManipulateOtherRoomsVisible(true);
        }

        private void OnBossManipulateOtherRoomsBackClicked()
        {
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetBossManipulateOtherRoomsVisible(false);
        }

        private void OnBossManipulateOtherRoomsResetAllClicked()
        {
            SetBossManipulateOtherRoomsResetConfirmVisible(true);
        }

        private void OnBossManipulateOtherRoomsResetConfirmYes()
        {
            SetBossManipulateOtherRoomsResetConfirmVisible(false);

            OnVengeflyKingP1HelperResetDefaultsClicked();
            OnGruzMotherP1HelperResetDefaultsClicked();
            OnGorbP1HelperResetDefaultsClicked();
            OnSoulWarriorP1HelperResetDefaultsClicked();
            OnBroodingMawlekP1HelperResetDefaultsClicked();
            OnXeroP2HelperResetDefaultsClicked();
            OnMarmuP2HelperResetDefaultsClicked();
            OnNoskP2HelperResetDefaultsClicked();
            OnUumuuP3HelperResetDefaultsClicked();
            OnNoEyesP4HelperResetDefaultsClicked();
            OnMarkothP4HelperResetDefaultsClicked();

            RefreshBossManipulateCardVisuals();
            UpdateQuickMenuEntryStateColors();
        }

        private void OnBossManipulateOtherRoomsResetConfirmNo()
        {
            SetBossManipulateOtherRoomsResetConfirmVisible(false);
        }

        private void OnBossManipulateOtherRoomsGruzMotherP1Clicked()
        {
            returnToBossManipulateOtherRoomsOnClose = true;
            SetBossManipulateOtherRoomsVisible(false);
            SetGruzMotherP1HelperVisible(true);
        }

        private void OnBossManipulateOtherRoomsVengeflyKingP1Clicked()
        {
            returnToBossManipulateOtherRoomsOnClose = true;
            SetBossManipulateOtherRoomsVisible(false);
            SetVengeflyKingP1HelperVisible(true);
        }

        private void OnBossManipulateOtherRoomsBroodingMawlekP1Clicked()
        {
            returnToBossManipulateOtherRoomsOnClose = true;
            SetBossManipulateOtherRoomsVisible(false);
            SetBroodingMawlekP1HelperVisible(true);
        }

        private void OnBossManipulateOtherRoomsNoskP2Clicked()
        {
            returnToBossManipulateOtherRoomsOnClose = true;
            SetBossManipulateOtherRoomsVisible(false);
            SetNoskP2HelperVisible(true);
        }

        private void OnBossManipulateOtherRoomsUumuuP3Clicked()
        {
            returnToBossManipulateOtherRoomsOnClose = true;
            SetBossManipulateOtherRoomsVisible(false);
            SetUumuuP3HelperVisible(true);
        }

        private void OnBossManipulateOtherRoomsSoulWarriorP1Clicked()
        {
            returnToBossManipulateOtherRoomsOnClose = true;
            SetBossManipulateOtherRoomsVisible(false);
            SetSoulWarriorP1HelperVisible(true);
        }

        private void OnBossManipulateOtherRoomsNoEyesP4Clicked()
        {
            returnToBossManipulateOtherRoomsOnClose = true;
            SetBossManipulateOtherRoomsVisible(false);
            SetNoEyesP4HelperVisible(true);
        }

        private void OnBossManipulateOtherRoomsMarmuP2Clicked()
        {
            returnToBossManipulateOtherRoomsOnClose = true;
            SetBossManipulateOtherRoomsVisible(false);
            SetMarmuP2HelperVisible(true);
        }

        private void OnBossManipulateOtherRoomsXeroP2Clicked()
        {
            returnToBossManipulateOtherRoomsOnClose = true;
            SetBossManipulateOtherRoomsVisible(false);
            SetXeroP2HelperVisible(true);
        }

        private void OnBossManipulateOtherRoomsMarkothP4Clicked()
        {
            returnToBossManipulateOtherRoomsOnClose = true;
            SetBossManipulateOtherRoomsVisible(false);
            SetMarkothP4HelperVisible(true);
        }

        private void OnBossManipulateOtherRoomsGorbP1Clicked()
        {
            returnToBossManipulateOtherRoomsOnClose = true;
            SetBossManipulateOtherRoomsVisible(false);
            SetGorbP1HelperVisible(true);
        }

        private void OnGruzMotherP1HelperBackClicked()
        {
            bool reopenOtherRooms = returnToBossManipulateOtherRoomsOnClose;
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOtherRoomsOnClose = false;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenOtherRooms)
            {
                returnToBossManipulateOnClose = true;
                SetBossManipulateOtherRoomsVisible(true);
            }
            else if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetGruzMotherP1HelperVisible(false);
        }

        private void OnVengeflyKingP1HelperBackClicked()
        {
            bool reopenOtherRooms = returnToBossManipulateOtherRoomsOnClose;
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOtherRoomsOnClose = false;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenOtherRooms)
            {
                returnToBossManipulateOnClose = true;
                SetBossManipulateOtherRoomsVisible(true);
            }
            else if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetVengeflyKingP1HelperVisible(false);
        }

        private void OnBroodingMawlekP1HelperBackClicked()
        {
            bool reopenOtherRooms = returnToBossManipulateOtherRoomsOnClose;
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOtherRoomsOnClose = false;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenOtherRooms)
            {
                returnToBossManipulateOnClose = true;
                SetBossManipulateOtherRoomsVisible(true);
            }
            else if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetBroodingMawlekP1HelperVisible(false);
        }

        private void OnNoskP2HelperBackClicked()
        {
            bool reopenOtherRooms = returnToBossManipulateOtherRoomsOnClose;
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOtherRoomsOnClose = false;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenOtherRooms)
            {
                returnToBossManipulateOnClose = true;
                SetBossManipulateOtherRoomsVisible(true);
            }
            else if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetNoskP2HelperVisible(false);
        }

        private void OnUumuuP3HelperBackClicked()
        {
            bool reopenOtherRooms = returnToBossManipulateOtherRoomsOnClose;
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOtherRoomsOnClose = false;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenOtherRooms)
            {
                returnToBossManipulateOnClose = true;
                SetBossManipulateOtherRoomsVisible(true);
            }
            else if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetUumuuP3HelperVisible(false);
        }

        private void OnSoulWarriorP1HelperBackClicked()
        {
            bool reopenOtherRooms = returnToBossManipulateOtherRoomsOnClose;
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOtherRoomsOnClose = false;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenOtherRooms)
            {
                returnToBossManipulateOnClose = true;
                SetBossManipulateOtherRoomsVisible(true);
            }
            else if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetSoulWarriorP1HelperVisible(false);
        }

        private void OnNoEyesP4HelperBackClicked()
        {
            bool reopenOtherRooms = returnToBossManipulateOtherRoomsOnClose;
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOtherRoomsOnClose = false;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenOtherRooms)
            {
                returnToBossManipulateOnClose = true;
                SetBossManipulateOtherRoomsVisible(true);
            }
            else if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetNoEyesP4HelperVisible(false);
        }

        private void OnMarmuP2HelperBackClicked()
        {
            bool reopenOtherRooms = returnToBossManipulateOtherRoomsOnClose;
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOtherRoomsOnClose = false;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenOtherRooms)
            {
                returnToBossManipulateOnClose = true;
                SetBossManipulateOtherRoomsVisible(true);
            }
            else if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetMarmuP2HelperVisible(false);
        }

        private void OnXeroP2HelperBackClicked()
        {
            bool reopenOtherRooms = returnToBossManipulateOtherRoomsOnClose;
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOtherRoomsOnClose = false;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenOtherRooms)
            {
                returnToBossManipulateOnClose = true;
                SetBossManipulateOtherRoomsVisible(true);
            }
            else if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetXeroP2HelperVisible(false);
        }

        private void OnMarkothP4HelperBackClicked()
        {
            bool reopenOtherRooms = returnToBossManipulateOtherRoomsOnClose;
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOtherRoomsOnClose = false;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenOtherRooms)
            {
                returnToBossManipulateOnClose = true;
                SetBossManipulateOtherRoomsVisible(true);
            }
            else if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetMarkothP4HelperVisible(false);
        }

        private void OnGorbP1HelperBackClicked()
        {
            bool reopenOtherRooms = returnToBossManipulateOtherRoomsOnClose;
            bool reopenBossManipulate = returnToBossManipulateOnClose;
            bool reopenQuick = returnToQuickOnClose;
            returnToBossManipulateOtherRoomsOnClose = false;
            returnToBossManipulateOnClose = false;
            returnToQuickOnClose = false;

            if (reopenOtherRooms)
            {
                returnToBossManipulateOnClose = true;
                SetBossManipulateOtherRoomsVisible(true);
            }
            else if (reopenBossManipulate)
            {
                returnToQuickOnClose = reopenQuick;
                SetBossManipulateVisible(true);
            }
            else if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetGorbP1HelperVisible(false);
        }

        private void OnGruzMotherP1HelperResetDefaultsClicked()
        {
            ResetGruzMotherP1HelperDefaults();
            SetGruzMotherP1HelperEnabled(false);
            Modules.BossChallenge.GruzMotherP1Helper.RestoreVanillaHealthIfPresent();
            RefreshGruzMotherP1HelperUi();
        }

        private void OnVengeflyKingP1HelperResetDefaultsClicked()
        {
            ResetVengeflyKingP1HelperDefaults();
            SetVengeflyKingP1HelperEnabled(false);
            Modules.BossChallenge.VengeflyKingP1Helper.RestoreVanillaHealthIfPresent();
            RefreshVengeflyKingP1HelperUi();
        }

        private void OnBroodingMawlekP1HelperResetDefaultsClicked()
        {
            ResetBroodingMawlekP1HelperDefaults();
            SetBroodingMawlekP1HelperEnabled(false);
            Modules.BossChallenge.BroodingMawlekP1Helper.RestoreVanillaHealthIfPresent();
            RefreshBroodingMawlekP1HelperUi();
        }

        private void OnNoskP2HelperResetDefaultsClicked()
        {
            ResetNoskP2HelperDefaults();
            SetNoskP2HelperEnabled(false);
            Modules.BossChallenge.NoskP2Helper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.NoskP2Helper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshNoskP2HelperUi();
        }

        private void OnUumuuP3HelperResetDefaultsClicked()
        {
            ResetUumuuP3HelperDefaults();
            SetUumuuP3HelperEnabled(false);
            Modules.BossChallenge.UumuuP3Helper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.UumuuP3Helper.RestoreVanillaSummonHealthIfPresent();
            RefreshUumuuP3HelperUi();
        }

        private void OnSoulWarriorP1HelperResetDefaultsClicked()
        {
            ResetSoulWarriorP1HelperDefaults();
            SetSoulWarriorP1HelperEnabled(false);
            Modules.BossChallenge.SoulWarriorP1Helper.RestoreVanillaHealthIfPresent();
            RefreshSoulWarriorP1HelperUi();
        }

        private void OnNoEyesP4HelperResetDefaultsClicked()
        {
            ResetNoEyesP4HelperDefaults();
            SetNoEyesP4HelperEnabled(false);
            Modules.BossChallenge.NoEyesP4Helper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.NoEyesP4Helper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshNoEyesP4HelperUi();
        }

        private void OnMarmuP2HelperResetDefaultsClicked()
        {
            ResetMarmuP2HelperDefaults();
            SetMarmuP2HelperEnabled(false);
            Modules.BossChallenge.MarmuP2Helper.RestoreVanillaHealthIfPresent();
            RefreshMarmuP2HelperUi();
        }

        private void OnXeroP2HelperResetDefaultsClicked()
        {
            ResetXeroP2HelperDefaults();
            SetXeroP2HelperEnabled(false);
            Modules.BossChallenge.XeroP2Helper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.XeroP2Helper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshXeroP2HelperUi();
        }

        private void OnMarkothP4HelperResetDefaultsClicked()
        {
            ResetMarkothP4HelperDefaults();
            SetMarkothP4HelperEnabled(false);
            Modules.BossChallenge.MarkothP4Helper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.MarkothP4Helper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshMarkothP4HelperUi();
        }

        private void OnGorbP1HelperResetDefaultsClicked()
        {
            ResetGorbP1HelperDefaults();
            SetGorbP1HelperEnabled(false);
            Modules.BossChallenge.GorbP1Helper.RestoreVanillaHealthIfPresent();
            Modules.BossChallenge.GorbP1Helper.RestoreVanillaPhaseThresholdsIfPresent();
            RefreshGorbP1HelperUi();
        }
    }
}
