using GodhomeQoL.Modules.BossChallenge;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private const int CustomPantheonGridColumns = 21;
        private const float CustomPantheonCardStepY = 112f;
        private const float CustomPantheonCardInset = 4f;
        private const float CustomPantheonGridLine = 1.5f;
        private const int CustomPantheonCaptionLimit = 6;
        private static readonly Color CustomPantheonCardColor = new(1f, 1f, 1f, 0.05f);
        private static readonly Color CustomPantheonCardHoverColor = new(1f, 1f, 1f, 0.16f);
        private static readonly Color CustomPantheonGridLineColor = new(0.902f, 0.749f, 0.384f, 0.28f);

        private const float CustomPantheonDragThreshold = 12f;
        private const float CustomPantheonAutoScrollZone = 50f;
        private const float CustomPantheonAutoScrollSpeed = 900f;

        private sealed class CustomPantheonCardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
        {
            internal Action<PointerEventData>? Began;
            internal Action<PointerEventData>? Moved;
            internal Action<PointerEventData>? Ended;
            internal Action? Entered;
            internal Action? Exited;
            internal Action? RightClicked;

            public void OnBeginDrag(PointerEventData eventData) => Began?.Invoke(eventData);

            public void OnDrag(PointerEventData eventData) => Moved?.Invoke(eventData);

            public void OnEndDrag(PointerEventData eventData) => Ended?.Invoke(eventData);

            public void OnPointerEnter(PointerEventData eventData) => Entered?.Invoke();

            public void OnPointerExit(PointerEventData eventData) => Exited?.Invoke();

            public void OnPointerClick(PointerEventData eventData)
            {
                if (eventData.button == PointerEventData.InputButton.Right)
                {
                    RightClicked?.Invoke();
                }
            }
        }

        private int customPantheonSelected = -1;
        private Text? customPantheonInfoText;
        private readonly List<Image> customPantheonCardFrames = new();
        private readonly List<CanvasGroup> customPantheonCardGroups = new();
        private readonly List<Button> customPantheonSelectionButtons = new();
        private RectTransform? customPantheonGridContent;
        private ScrollRect? customPantheonGridScroll;
        private float customPantheonGridLeft;
        private float customPantheonGridTop;
        private float customPantheonGridCellWidth;
        private int customPantheonDragSource = -1;
        private int customPantheonDragTarget = -1;
        private bool customPantheonDragActive;
        private Vector2 customPantheonDragStart;
        private RectTransform? customPantheonDragGhost;
        private RectTransform? customPantheonDropIndicator;

        private RectTransform CreateCustomPantheonDropIndicator(RectTransform content, Vector2 size)
        {
            GameObject indicator = new("DropIndicator");
            indicator.transform.SetParent(content, false);
            RectTransform rect = indicator.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            Image fill = indicator.AddComponent<Image>();
            fill.color = new Color(0.902f, 0.749f, 0.384f, 0.14f);
            fill.raycastTarget = false;
            Image frame = AddHkChamferFrame(indicator.transform);
            frame.color = new Color(1f, 0.95f, 0.75f, 1f);
            Text plus = CreateText(indicator.transform, "Plus", "+", 40, TextAnchor.MiddleCenter);
            PlaceCustomPantheonRect(plus.rectTransform, Vector2.zero, size);
            plus.color = HkValueColor;
            indicator.SetActive(false);
            return rect;
        }

        private Vector2 GetCustomPantheonCellCenter(int index)
        {
            int column = index % CustomPantheonGridColumns;
            int row = index / CustomPantheonGridColumns;
            return new Vector2(
                customPantheonGridLeft + (column * customPantheonGridCellWidth) + (customPantheonGridCellWidth * 0.5f),
                customPantheonGridTop - (row * CustomPantheonCardStepY) - (CustomPantheonCardStepY * 0.5f));
        }

        private void BuildCustomPantheonEditor(CustomPantheonChain chain)
        {
            float width = BossManipulatePanelWidth;
            float height = BossManipulatePanelHeight;
            customPantheonRoot = CreateOverlayFrame("CustomPantheonCanvas", CustomPantheonCanvasSortOrder, "CustomPantheonPanel", height, GetCustomPantheonChainName(chain), 52, 0f, out GameObject panel, out RectTransform titleRect);
            ((RectTransform)panel.transform).sizeDelta = new Vector2(width, height);
            titleRect.anchoredPosition = new Vector2(0f, (height * 0.5f) - 60f);
            titleRect.sizeDelta = new Vector2(width - 120f, 60f);
            customPantheonTitle = titleRect.GetComponent<Text>();
            customPantheonCardFrames.Clear();
            customPantheonCardGroups.Clear();
            customPantheonSelectionButtons.Clear();
            CancelCustomPantheonDrag();
            if (customPantheonSelected >= chain.Fights.Count)
            {
                customPantheonSelected = chain.Fights.Count - 1;
            }

            float barY = (height * 0.5f) - 172f;
            Text nameLabel = CreateText(panel.transform, "NameLabel", "CustomPantheon/Name".Localize(), 26, TextAnchor.MiddleRight);
            PlaceCustomPantheonRect(nameLabel.rectTransform, new Vector2(-790f, barY), new Vector2(140f, 44f));
            StyleHkRowLabel(nameLabel);

            InputField field = CreateInputField(panel.transform, chain.Name ?? string.Empty, Vector2.zero, new Vector2(300f, 42f));
            PlaceCustomPantheonRect((RectTransform)field.transform, new Vector2(-560f, barY), new Vector2(300f, 42f));
            field.contentType = InputField.ContentType.Standard;
            field.characterLimit = CustomPantheonNameLimit;
            field.textComponent.alignment = TextAnchor.MiddleLeft;
            field.onEndEdit.AddListener(value => RenameCustomPantheonChain(chain, value));
            customPantheonNameField = field;

            Button select = CreateCustomPantheonBarButton(panel.transform, "Select", (chain.Selected ? "CustomPantheon/SelectedButton" : "CustomPantheon/Select").Localize(), new Vector2(-290f, barY), 170f, OnCustomPantheonSelectClicked);
            customPantheonSelectLabel = select.GetComponentInChildren<Text>();
            if (chain.Selected && customPantheonSelectLabel != null)
            {
                customPantheonSelectLabel.color = HkValueColor;
            }
            CreateCustomPantheonBarButton(panel.transform, "AddBoss", "CustomPantheon/AddBoss".Localize(), new Vector2(-80f, barY), 200f, OnCustomPantheonAddBossClicked);
            CreateCustomPantheonBarButton(panel.transform, "AddBench", "CustomPantheon/AddBench".Localize(), new Vector2(140f, barY), 200f, OnCustomPantheonAddBenchClicked);
            CreateCustomPantheonBarButton(panel.transform, "Clear", "CustomPantheon/Clear".Localize(), new Vector2(370f, barY), 220f, OnCustomPantheonClearClicked);

            Text count = CreateText(panel.transform, "Count", string.Format("CustomPantheon/FightCount".Localize(), chain.Fights.Count, CustomPantheonCatalog.MaxChainLength), 24, TextAnchor.MiddleRight);
            PlaceCustomPantheonRect(count.rectTransform, new Vector2(720f, barY), new Vector2(300f, 44f));
            StyleHkRowValue(count);

            float gridWidth = width - 60f;
            float gridTop = barY - 42f;
            float actionY = GetFixedBackY(height) + 72f;
            float gridBottom = actionY + 34f;
            float topOffset = (height * 0.5f) - gridTop;
            float bottomOffset = (height * 0.5f) + gridBottom;
            RectTransform content = CreateScrollContent(panel.transform, gridWidth, height, topOffset, bottomOffset);
            float viewHeight = Mathf.Max(0f, height - topOffset - bottomOffset);
            BuildCustomPantheonGrid(content, chain, gridWidth, viewHeight);

            customPantheonInfoText = CreateText(panel.transform, "Info", string.Empty, 24, TextAnchor.MiddleLeft);
            PlaceCustomPantheonRect(customPantheonInfoText.rectTransform, new Vector2(-440f, actionY), new Vector2(880f, 44f));
            customPantheonInfoText.supportRichText = true;
            customPantheonInfoText.resizeTextForBestFit = true;
            customPantheonInfoText.resizeTextMaxSize = 24;
            customPantheonInfoText.resizeTextMinSize = 14;
            StyleHkRowLabel(customPantheonInfoText);

            customPantheonSelectionButtons.Add(CreateCustomPantheonBarButton(panel.transform, "MoveLeft", "CustomPantheon/Left".Localize(), new Vector2(150f, actionY), 130f, () => MoveCustomPantheonSelected(-1)));
            customPantheonSelectionButtons.Add(CreateCustomPantheonBarButton(panel.transform, "MoveRight", "CustomPantheon/Right".Localize(), new Vector2(295f, actionY), 130f, () => MoveCustomPantheonSelected(1)));
            customPantheonSelectionButtons.Add(CreateCustomPantheonBarButton(panel.transform, "EditFight", "CustomPantheon/Set".Localize(), new Vector2(440f, actionY), 130f, () => OpenCustomPantheonFight(customPantheonSelected)));
            customPantheonSelectionButtons.Add(CreateCustomPantheonBarButton(panel.transform, "DeleteFight", "CustomPantheon/Delete".Localize(), new Vector2(585f, actionY), 130f, () => RemoveCustomPantheonEntry(customPantheonSelected)));

            CreateButtonRow(panel.transform, "CustomPantheonBackRow", "Back", GetFixedBackY(height), OnCustomPantheonBackClicked);
            CreateCustomPantheonConfirm(panel.transform);
            SetCustomPantheonConfirmVisible(false);
            UpdateCustomPantheonSelection();
        }

        private void BuildCustomPantheonGrid(RectTransform content, CustomPantheonChain chain, float gridWidth, float viewHeight)
        {
            float layoutWidth = gridWidth - 40f;
            float cellWidth = layoutWidth / CustomPantheonGridColumns;
            float left = -(layoutWidth * 0.5f) - 15f;
            float top = -4f;
            int count = chain.Fights.Count;
            int filled = count < CustomPantheonCatalog.MaxChainLength ? count + 1 : count;
            int rows = count == 0 ? 0 : Mathf.CeilToInt(filled / (float)CustomPantheonGridColumns);
            customPantheonGridContent = content;
            customPantheonGridScroll = content.GetComponentInParent<ScrollRect>();
            if (customPantheonGridScroll != null)
            {
                customPantheonGridScroll.scrollSensitivity = CustomPantheonCardStepY;
                customPantheonGridScroll.inertia = false;
            }

            customPantheonGridLeft = left;
            customPantheonGridTop = top;
            customPantheonGridCellWidth = cellWidth;
            Vector2 cardSize = new(cellWidth - (CustomPantheonCardInset * 2f), CustomPantheonCardStepY - (CustomPantheonCardInset * 2f));
            customPantheonDropIndicator = CreateCustomPantheonDropIndicator(content, cardSize);
            for (int i = 0; i < count; i++)
            {
                int column = i % CustomPantheonGridColumns;
                int row = i / CustomPantheonGridColumns;
                Vector2 position = new(left + (column * cellWidth) + (cellWidth * 0.5f), top - (row * CustomPantheonCardStepY) - (CustomPantheonCardStepY * 0.5f));
                CreateCustomPantheonCard(content, chain.Fights[i], i, position, cardSize);
            }

            if (rows > 0)
            {
                float gridHeight = rows * CustomPantheonCardStepY;
                for (int column = 0; column <= CustomPantheonGridColumns; column++)
                {
                    CreateCustomPantheonGridLine(content, new Vector2(left + (column * cellWidth), top - (gridHeight * 0.5f)), new Vector2(CustomPantheonGridLine, gridHeight));
                }

                for (int row = 0; row <= rows; row++)
                {
                    CreateCustomPantheonGridLine(content, new Vector2(left + (layoutWidth * 0.5f), top - (row * CustomPantheonCardStepY)), new Vector2(layoutWidth, CustomPantheonGridLine));
                }
            }

            if (count == 0)
            {
                Text hint = CreateText(content, "EmptyHint", "CustomPantheon/EmptyChain".Localize(), 26, TextAnchor.MiddleCenter);
                RectTransform hintRect = hint.rectTransform;
                hintRect.anchorMin = new Vector2(0.5f, 1f);
                hintRect.anchorMax = new Vector2(0.5f, 1f);
                hintRect.pivot = new Vector2(0.5f, 0.5f);
                hintRect.anchoredPosition = new Vector2(0f, -60f);
                hintRect.sizeDelta = new Vector2(gridWidth, 60f);
                hint.color = CustomPantheonDimColor;
            }

            content.sizeDelta = new Vector2(0f, Mathf.Max(viewHeight, (rows * CustomPantheonCardStepY) + 12f));
        }

        private static void CreateCustomPantheonGridLine(RectTransform parent, Vector2 center, Vector2 size)
        {
            GameObject line = new("GridLine");
            line.transform.SetParent(parent, false);
            RectTransform rect = line.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = center;
            rect.sizeDelta = size;
            Image image = line.AddComponent<Image>();
            image.color = CustomPantheonGridLineColor;
            image.raycastTarget = false;
        }

        private void CreateCustomPantheonCard(RectTransform parent, CustomPantheonFight fight, int index, Vector2 position, Vector2 size)
        {
            GameObject card = new($"Fight{index + 1}");
            card.transform.SetParent(parent, false);
            RectTransform rect = card.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Image background = card.AddComponent<Image>();
            background.color = CustomPantheonCardColor;
            Button button = card.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = background;
            int fightIndex = index;
            button.onClick.AddListener(() => SelectCustomPantheonFight(fightIndex));

            Image frame = AddHkChamferFrame(card.transform);
            frame.color = Color.clear;
            customPantheonCardFrames.Add(frame);
            customPantheonCardGroups.Add(card.AddComponent<CanvasGroup>());

            CustomPantheonCardDrag input = card.AddComponent<CustomPantheonCardDrag>();
            input.Began = data => BeginCustomPantheonDrag(fightIndex, data);
            input.Moved = data => MoveCustomPantheonDrag(card, data);
            input.Ended = data => EndCustomPantheonDrag(data);
            input.Entered = () =>
            {
                background.color = CustomPantheonCardHoverColor;
                if (!customPantheonDragActive)
                {
                    ShowCustomPantheonFightInfo(fightIndex);
                }
            };
            input.Exited = () =>
            {
                background.color = CustomPantheonCardColor;
                if (!customPantheonDragActive)
                {
                    ShowCustomPantheonFightInfo(customPantheonSelected);
                }
            };
            input.RightClicked = () =>
            {
                customPantheonSelected = fightIndex;
                OpenCustomPantheonFight(fightIndex);
            };

            Text number = CreateText(card.transform, "Number", (index + 1).ToString(), 15, TextAnchor.MiddleCenter);
            PlaceCustomPantheonEdge(number.rectTransform, top: true, 18f, 1f);
            number.color = CustomPantheonDimColor;

            if (CustomPantheonCatalog.TryGet(fight.Scene, out CustomPantheonBoss boss) && BossHistory.Find(boss.Key) is BossHistoryBoss historyBoss)
            {
                GameObject iconObject = new("Icon");
                iconObject.transform.SetParent(card.transform, false);
                RectTransform iconRect = iconObject.AddComponent<RectTransform>();
                iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                iconRect.pivot = new Vector2(0.5f, 0.5f);
                iconRect.anchoredPosition = new Vector2(0f, 1f);
                iconRect.sizeDelta = new Vector2(size.x - 10f, size.y - 46f);
                Image icon = iconObject.AddComponent<Image>();
                icon.raycastTarget = false;
                icon.preserveAspect = true;
                ApplyBossHistorySprite(icon, historyBoss);
            }
            else
            {
                Text benchText = CreateText(card.transform, "Bench", CustomPantheonCatalog.IsBench(fight.Scene) ? "CustomPantheon/Bench".Localize() : "?", 15, TextAnchor.MiddleCenter);
                RectTransform benchRect = benchText.rectTransform;
                benchRect.anchorMin = new Vector2(0.5f, 0.5f);
                benchRect.anchorMax = new Vector2(0.5f, 0.5f);
                benchRect.pivot = new Vector2(0.5f, 0.5f);
                benchRect.anchoredPosition = new Vector2(0f, 1f);
                benchRect.sizeDelta = new Vector2(size.x, size.y - 46f);
                benchText.color = CustomPantheonDimColor;
            }

            if (!string.IsNullOrEmpty(fight.Caption))
            {
                Text caption = CreateText(card.transform, "Caption", fight.Caption, 18, TextAnchor.MiddleCenter);
                PlaceCustomPantheonEdge(caption.rectTransform, top: false, 22f, 2f);
                caption.color = HkValueColor;
                caption.horizontalOverflow = HorizontalWrapMode.Overflow;
                caption.resizeTextForBestFit = true;
                caption.resizeTextMaxSize = 18;
                caption.resizeTextMinSize = 7;
            }

            if (fight.HasCustomSettings)
            {
                GameObject dotObject = new("SettingsDot");
                dotObject.transform.SetParent(card.transform, false);
                RectTransform dotRect = dotObject.AddComponent<RectTransform>();
                dotRect.anchorMin = new Vector2(1f, 1f);
                dotRect.anchorMax = new Vector2(1f, 1f);
                dotRect.pivot = new Vector2(1f, 1f);
                dotRect.anchoredPosition = new Vector2(-4f, -4f);
                dotRect.sizeDelta = new Vector2(8f, 8f);
                Image dot = dotObject.AddComponent<Image>();
                dot.color = HkValueColor;
                dot.raycastTarget = false;
            }
        }

        private static void PlaceCustomPantheonRect(RectTransform rect, Vector2 position, Vector2 size)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        private static void PlaceCustomPantheonEdge(RectTransform rect, bool top, float height, float inset)
        {
            float edge = top ? 1f : 0f;
            rect.anchorMin = new Vector2(0f, edge);
            rect.anchorMax = new Vector2(1f, edge);
            rect.pivot = new Vector2(0.5f, edge);
            rect.anchoredPosition = new Vector2(0f, top ? -inset : inset);
            rect.sizeDelta = new Vector2(0f, height);
        }

        private Button CreateCustomPantheonBarButton(Transform parent, string name, string text, Vector2 position, float width, Action onClick)
        {
            Button button = CreateCustomPantheonTextButton(parent, name, text, position, onClick);
            ((RectTransform)button.transform).sizeDelta = new Vector2(width, 44f);
            Text? label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                label.fontSize = 22;
            }

            return button;
        }

        private void SelectCustomPantheonFight(int index)
        {
            customPantheonSelected = customPantheonSelected == index ? -1 : index;
            UpdateCustomPantheonSelection();
        }

        private void UpdateCustomPantheonSelection()
        {
            for (int i = 0; i < customPantheonCardFrames.Count; i++)
            {
                Image frame = customPantheonCardFrames[i];
                if (frame != null)
                {
                    frame.color = i == customPantheonSelected ? HkValueColor : Color.clear;
                }
            }

            bool hasSelection = customPantheonSelected >= 0 && customPantheonSelected < customPantheonCardFrames.Count;
            foreach (Button button in customPantheonSelectionButtons)
            {
                if (button != null)
                {
                    button.interactable = hasSelection;
                    Text? label = button.GetComponentInChildren<Text>();
                    if (label != null)
                    {
                        label.color = hasSelection ? HkQuickEntryTextColor : HkOffValueColor;
                    }
                }
            }

            ShowCustomPantheonFightInfo(customPantheonSelected);
        }

        private void ShowCustomPantheonFightInfo(int index)
        {
            if (customPantheonInfoText == null)
            {
                return;
            }

            CustomPantheonChain? chain = EditedCustomPantheonChain;
            if (chain == null || index < 0 || index >= chain.Fights.Count)
            {
                customPantheonInfoText.text = chain != null && chain.Fights.Count > 0
                    ? "CustomPantheon/SelectHint".Localize()
                    : string.Empty;
                return;
            }

            CustomPantheonFight fight = chain.Fights[index];
            string caption = string.IsNullOrEmpty(fight.Caption) ? string.Empty : $"  «{fight.Caption}»";
            customPantheonInfoText.text = $"#{index + 1}  {FormatCustomPantheonEntry(fight.Scene)}{caption}{FormatCustomPantheonFightTags(fight)}";
        }

        private void MoveCustomPantheonSelected(int delta)
        {
            int index = customPantheonSelected;
            CustomPantheonChain? chain = EditedCustomPantheonChain;
            if (chain == null || index < 0 || index >= chain.Fights.Count)
            {
                return;
            }

            int target = index + delta;
            if (target < 0 || target >= chain.Fights.Count)
            {
                return;
            }

            CommitCustomPantheonName();
            (chain.Fights[index], chain.Fights[target]) = (chain.Fights[target], chain.Fights[index]);
            customPantheonSelected = target;
            GodhomeQoL.SaveGlobalSettingsSafe();
            RebuildCustomPantheonOverlay(true);
        }

        private void CreateCustomPantheonCaptionRow(Transform parent, CustomPantheonFight fight, float y)
        {
            GameObject row = CreateRow(parent, "CustomPantheonCaptionRow", y, new Vector2(RowWidth, RowHeight));
            _ = CreateRowLabel(row.transform, "CustomPantheon/Caption".Localize());
            InputField field = CreateInputField(row.transform, fight.Caption ?? string.Empty, new Vector2(-20f, 0f), new Vector2(240f, 40f));
            field.contentType = InputField.ContentType.Standard;
            field.characterLimit = CustomPantheonCaptionLimit;
            field.onEndEdit.AddListener(value =>
            {
                fight.Caption = (value ?? string.Empty).Trim();
                GodhomeQoL.SaveGlobalSettingsSafe();
            });
        }
    }
}
