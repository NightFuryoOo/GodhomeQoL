using GodhomeQoL.Modules.BossChallenge;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private const int CustomPantheonCanvasSortOrder = 10078;
        private const int CustomPantheonNameLimit = 32;
        private const int CustomPantheonPickerColumns = 11;
        private const int CustomPantheonPickerMinRows = 4;
        private static readonly Color CustomPantheonDimColor = new(0.7f, 0.68f, 0.64f, 1f);
        private static readonly Color CustomPantheonSelectedRowColor = new(1f, 0.85f, 0.45f, 0.12f);

        private Text? customPantheonSelectLabel;

        private GameObject? customPantheonRoot;
        private bool customPantheonVisible;
        private int customPantheonEditing = -1;
        private int customPantheonFightEditing = -1;
        private bool customPantheonPicking;
        private int customPantheonPickerTab = CustomPantheonCatalog.PantheonCount;
        private InputField? customPantheonNameField;
        private Text? customPantheonTitle;
        private GameObject? customPantheonConfirmRoot;
        private Text? customPantheonConfirmLabel;
        private Action? customPantheonConfirmAction;
        private bool customPantheonConfirmVisible;
        private readonly Dictionary<string, Text> customPantheonCardCounts = new(StringComparer.Ordinal);

        private static List<CustomPantheonChain> CustomPantheonChains
        {
            get
            {
                CustomPantheonSettings settings = GodhomeQoL.GlobalSettings.CustomPantheon ??= new CustomPantheonSettings();
                return settings.Chains ??= new List<CustomPantheonChain>();
            }
        }

        private CustomPantheonChain? EditedCustomPantheonChain =>
            customPantheonEditing >= 0 && customPantheonEditing < CustomPantheonChains.Count ? CustomPantheonChains[customPantheonEditing] : null;

        private void BuildCustomPantheonOverlayUi()
        {
            customPantheonNameField = null;
            customPantheonTitle = null;
            customPantheonSelectLabel = null;
            customPantheonConfirmAction = null;
            customPantheonConfirmVisible = false;
            customPantheonCardCounts.Clear();

            CustomPantheonChain? chain = EditedCustomPantheonChain;
            if (chain == null)
            {
                customPantheonEditing = -1;
                customPantheonFightEditing = -1;
                customPantheonPicking = false;
                if (customPantheonTransfer == CustomPantheonTransferMode.Export)
                {
                    BuildCustomPantheonExportView();
                }
                else if (customPantheonTransfer == CustomPantheonTransferMode.Import)
                {
                    BuildCustomPantheonImportView();
                }
                else
                {
                    BuildCustomPantheonList();
                }
            }
            else if (customPantheonFightEditing >= 0 && customPantheonFightEditing < chain.Fights.Count)
            {
                BuildCustomPantheonFightView(chain, customPantheonFightEditing);
            }
            else if (customPantheonPicking)
            {
                BuildCustomPantheonPicker(chain);
            }
            else
            {
                BuildCustomPantheonEditor(chain);
            }
        }

        private void BuildCustomPantheonList()
        {
            customPantheonRoot = CreateOverlayFrame("CustomPantheonCanvas", CustomPantheonCanvasSortOrder, "CustomPantheonPanel", PanelHeight, "CustomPantheon/Title".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);
            float resetY = GetFixedResetY(PanelHeight);
            float startY = resetY + (FixedResetOffset - FixedBackOffset);
            float transferY = startY + (FixedResetOffset - FixedBackOffset);
            float backY = GetFixedBackY(PanelHeight);
            float topOffset = GetScrollTopOffset(PanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(PanelHeight, transferY);
            float viewHeight = Mathf.Max(0f, PanelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, PanelHeight, topOffset, bottomOffset);

            float rowY = GetRowStartY(PanelHeight, RowStartY, topOffset);
            float lastY = rowY;
            List<CustomPantheonChain> chains = CustomPantheonChains;
            if (chains.Count == 0)
            {
                GameObject hintRow = CreateRow(content, "CustomPantheonEmptyRow", rowY, new Vector2(RowWidth, RowHeight * 2f));
                Text hint = CreateText(hintRow.transform, "Label", "CustomPantheon/NoChains".Localize(), 26, TextAnchor.MiddleCenter);
                StretchCustomPantheonText(hint.rectTransform);
                hint.horizontalOverflow = HorizontalWrapMode.Wrap;
                hint.color = CustomPantheonDimColor;
            }

            for (int i = 0; i < chains.Count; i++)
            {
                CreateCustomPantheonChainRow(content, i, chains[i], rowY);
                lastY = rowY;
                rowY += RowSpacing;
            }

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateCustomPantheonSideButton(panel.transform, "CustomPantheonExportRow", "CustomPantheon/Export".Localize(), -CustomPantheonTransferButtonX, transferY, OnCustomPantheonExportClicked);
            CreateCustomPantheonSideButton(panel.transform, "CustomPantheonImportRow", "CustomPantheon/Import".Localize(), CustomPantheonTransferButtonX, transferY, OnCustomPantheonImportClicked);
            CreateButtonRow(panel.transform, "CustomPantheonStartRow", "CustomPantheon/Start".Localize(), startY, OnCustomPantheonStartClicked);
            CreateButtonRow(panel.transform, "CustomPantheonNewRow", "CustomPantheon/New".Localize(), resetY, OnCustomPantheonNewClicked);
            CreateButtonRow(panel.transform, "CustomPantheonBackRow", "Back", backY, OnCustomPantheonBackClicked);
            CreateCustomPantheonConfirm(panel.transform);
            SetCustomPantheonConfirmVisible(false);
        }

        private void CreateCustomPantheonChainRow(Transform parent, int index, CustomPantheonChain chain, float y)
        {
            GameObject row = CreateRow(parent, $"CustomPantheonChain{index}", y, new Vector2(RowWidth, RowHeight));
            bool selected = chain.Selected;
            Image image = row.AddComponent<Image>();
            image.color = selected ? CustomPantheonSelectedRowColor : new Color(0f, 0f, 0f, 0f);
            Button button = row.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = image;
            int chainIndex = index;
            button.onClick.AddListener(() => OpenCustomPantheonChain(chainIndex));
            RowHighlight highlight = CreateRowHighlight(row, image);
            AttachRowHighlight(row, highlight);
            if (selected)
            {
                AddCustomPantheonSelectedFrame(row.transform);
            }

            string name = GetCustomPantheonChainName(chain);
            Text label = CreateRowLabel(row.transform, selected ? string.Format("CustomPantheon/SelectedRow".Localize(), name) : name);
            label.rectTransform.sizeDelta = new Vector2(520f, RowHeight);
            if (selected)
            {
                label.color = HkValueColor;
            }

            Text value = CreateRowValue(row.transform, FormatCustomPantheonBossCount(chain.Fights));
            RectTransform valueRect = value.rectTransform;
            valueRect.anchoredPosition = new Vector2(-60f, 0f);
            valueRect.sizeDelta = new Vector2(220f, RowHeight);

            Button? delete = CreateGearSwitcherPresetDeleteButton(row.transform, () => StartCustomPantheonDelete(chainIndex));
            if (delete != null)
            {
                AttachRowHighlight(delete.gameObject, highlight);
            }
        }

        private Button CreateCustomPantheonTextButton(Transform parent, string name, string text, Vector2 position, Action onClick)
        {
            GameObject buttonObject = new(name);
            buttonObject.transform.SetParent(parent, false);
            RectTransform rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(84f, 34f);
            Image image = buttonObject.AddComponent<Image>();
            Button button = buttonObject.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = image;
            button.onClick.AddListener(() => onClick());
            Text label = CreateText(buttonObject.transform, "Label", text, 18, TextAnchor.MiddleCenter);
            StretchCustomPantheonText(label.rectTransform);
            StyleHkMiniButton(buttonObject, image, label);
            return button;
        }

        private void BuildCustomPantheonPicker(CustomPantheonChain chain)
        {
            customPantheonRoot = CreateOverlayFrame("CustomPantheonCanvas", CustomPantheonCanvasSortOrder, "CustomPantheonPanel", BossManipulatePanelHeight, "CustomPantheon/PickTitle".Localize(), 52, 0f, out GameObject panel, out RectTransform titleRect);
            RectTransform panelRect = (RectTransform)panel.transform;
            panelRect.sizeDelta = new Vector2(BossManipulatePanelWidth, BossManipulatePanelHeight);
            titleRect.anchoredPosition = new Vector2(0f, (BossManipulatePanelHeight * 0.5f) - 70f);
            titleRect.sizeDelta = new Vector2(BossManipulatePanelWidth - 120f, 60f);

            float tabsY = titleRect.anchoredPosition.y - 112f;
            for (int pantheon = 1; pantheon <= CustomPantheonCatalog.PantheonCount; pantheon++)
            {
                int tab = pantheon;
                float x = (pantheon - 3) * 130f;
                Button tabButton = CreateCustomPantheonTextButton(panel.transform, $"Tab{pantheon}", $"P{pantheon}", new Vector2(x, tabsY), () => SelectCustomPantheonTab(tab));
                RectTransform tabRect = (RectTransform)tabButton.transform;
                tabRect.sizeDelta = new Vector2(110f, 40f);
                Text? tabLabel = tabButton.GetComponentInChildren<Text>();
                if (tabLabel != null)
                {
                    tabLabel.fontSize = 24;
                    tabLabel.color = pantheon == customPantheonPickerTab ? HkValueColor : HkOffValueColor;
                }
            }

            float backY = GetFixedBackY(BossManipulatePanelHeight);
            float gridTop = tabsY - 42f;
            float gridBottom = backY + 50f;
            GameObject gridObject = new("CustomPantheonGrid");
            gridObject.transform.SetParent(panel.transform, false);
            RectTransform gridRect = gridObject.AddComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0.5f, 0.5f);
            gridRect.anchorMax = new Vector2(0.5f, 0.5f);
            gridRect.pivot = new Vector2(0.5f, 0.5f);
            gridRect.anchoredPosition = new Vector2(0f, (gridTop + gridBottom) * 0.5f);
            gridRect.sizeDelta = new Vector2(BossManipulatePanelWidth - 70f, Mathf.Max(240f, gridTop - gridBottom));
            BuildCustomPantheonCards(gridRect, chain);

            CreateButtonRow(panel.transform, "CustomPantheonDoneRow", "CustomPantheon/Done".Localize(), backY, OnCustomPantheonPickerDoneClicked);
        }

        private void BuildCustomPantheonCards(RectTransform gridRoot, CustomPantheonChain chain)
        {
            const float spacingX = 10f;
            const float spacingY = 12f;
            IReadOnlyList<CustomPantheonBoss> bosses = CustomPantheonCatalog.ForPantheon(customPantheonPickerTab);
            int columns = CustomPantheonPickerColumns;
            int rows = Math.Max(CustomPantheonPickerMinRows, Mathf.CeilToInt(bosses.Count / (float)columns));
            float cardWidth = (gridRoot.sizeDelta.x - ((columns - 1) * spacingX)) / columns;
            float cardHeight = (gridRoot.sizeDelta.y - ((rows - 1) * spacingY)) / rows;
            float startX = -((cardWidth * columns) + ((columns - 1) * spacingX)) * 0.5f + (cardWidth * 0.5f);
            float startY = ((cardHeight * rows) + ((rows - 1) * spacingY)) * 0.5f - (cardHeight * 0.5f);

            for (int index = 0; index < bosses.Count; index++)
            {
                float x = startX + ((index % columns) * (cardWidth + spacingX));
                float y = startY - ((index / columns) * (cardHeight + spacingY));
                CreateCustomPantheonCard(gridRoot, index, bosses[index], chain, x, y, cardWidth, cardHeight);
            }
        }

        private void CreateCustomPantheonCard(Transform parent, int index, CustomPantheonBoss boss, CustomPantheonChain chain, float x, float y, float width, float height)
        {
            GameObject card = new($"CustomPantheonCard{index + 1:D2}");
            card.transform.SetParent(parent, false);
            RectTransform cardRect = card.AddComponent<RectTransform>();
            cardRect.anchorMin = new Vector2(0.5f, 0.5f);
            cardRect.anchorMax = new Vector2(0.5f, 0.5f);
            cardRect.pivot = new Vector2(0.5f, 0.5f);
            cardRect.anchoredPosition = new Vector2(x, y);
            cardRect.sizeDelta = new Vector2(width, height);
            Image cardImage = card.AddComponent<Image>();
            cardImage.color = new Color(1f, 1f, 1f, 0.07f);
            Button button = card.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = cardImage;
            button.onClick.AddListener(() => AddCustomPantheonScene(chain, boss.Scene));
            RowHighlight highlight = CreateHkCardHighlight(card, cardImage);
            AttachRowHighlight(card, highlight);

            const float labelHeight = 48f;
            float imageHeight = Mathf.Max(50f, height - labelHeight - 10f);
            GameObject imageObject = new("Image");
            imageObject.transform.SetParent(card.transform, false);
            RectTransform imageRect = imageObject.AddComponent<RectTransform>();
            imageRect.anchorMin = new Vector2(0.5f, 0.5f);
            imageRect.anchorMax = new Vector2(0.5f, 0.5f);
            imageRect.pivot = new Vector2(0.5f, 0.5f);
            imageRect.anchoredPosition = new Vector2(0f, (height * 0.5f) - (imageHeight * 0.5f) - 4f);
            imageRect.sizeDelta = new Vector2(width - 12f, imageHeight);
            Image image = imageObject.AddComponent<Image>();
            image.raycastTarget = false;
            image.preserveAspect = true;
            BossHistoryBoss? historyBoss = BossHistory.Find(boss.Key);
            if (historyBoss != null)
            {
                ApplyBossHistorySprite(image, historyBoss);
            }

            Text nameText = CreateText(card.transform, "Label", boss.Key, 17, TextAnchor.MiddleCenter);
            nameText.resizeTextForBestFit = true;
            nameText.resizeTextMaxSize = 17;
            nameText.resizeTextMinSize = 11;
            RectTransform nameRect = nameText.rectTransform;
            nameRect.anchorMin = new Vector2(0f, 0f);
            nameRect.anchorMax = new Vector2(1f, 0f);
            nameRect.pivot = new Vector2(0.5f, 0f);
            nameRect.anchoredPosition = new Vector2(0f, 20f);
            nameRect.sizeDelta = new Vector2(-6f, 26f);

            Text countLabel = CreateText(card.transform, "Count", string.Empty, 15, TextAnchor.MiddleCenter);
            RectTransform countRect = countLabel.rectTransform;
            countRect.anchorMin = new Vector2(0f, 0f);
            countRect.anchorMax = new Vector2(1f, 0f);
            countRect.pivot = new Vector2(0.5f, 0f);
            countRect.anchoredPosition = new Vector2(0f, 2f);
            countRect.sizeDelta = new Vector2(0f, 20f);
            customPantheonCardCounts[boss.Scene] = countLabel;
            UpdateCustomPantheonCardCount(chain, boss.Scene);
        }

        private void UpdateCustomPantheonCardCount(CustomPantheonChain chain, string scene)
        {
            if (!customPantheonCardCounts.TryGetValue(scene, out Text? label) || label == null)
            {
                return;
            }

            int count = chain.Fights.Count(item => string.Equals(item.Scene, scene, StringComparison.Ordinal));
            label.text = count > 0 ? string.Format("CustomPantheon/InChain".Localize(), count) : string.Empty;
            label.color = HkValueColor;
        }

        private void CreateCustomPantheonConfirm(Transform parent)
        {
            customPantheonConfirmRoot = new GameObject("CustomPantheonConfirm");
            customPantheonConfirmRoot.transform.SetParent(parent, false);
            RectTransform rootRect = customPantheonConfirmRoot.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;
            Image dim = customPantheonConfirmRoot.AddComponent<Image>();
            dim.color = new Color(0f, 0f, 0f, 0.6f);
            CanvasGroup group = customPantheonConfirmRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dialog = new("Dialog");
            dialog.transform.SetParent(customPantheonConfirmRoot.transform, false);
            RectTransform dialogRect = dialog.AddComponent<RectTransform>();
            dialogRect.anchorMin = new Vector2(0.5f, 0.5f);
            dialogRect.anchorMax = new Vector2(0.5f, 0.5f);
            dialogRect.pivot = new Vector2(0.5f, 0.5f);
            dialogRect.anchoredPosition = Vector2.zero;
            dialogRect.sizeDelta = new Vector2(680f, 280f);
            Image dialogImage = dialog.AddComponent<Image>();
            StyleHkDialog(dialogImage);

            Text text = CreateText(dialog.transform, "Label", string.Empty, 26, TextAnchor.MiddleCenter);
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            RectTransform textRect = text.rectTransform;
            textRect.anchorMin = new Vector2(0.5f, 1f);
            textRect.anchorMax = new Vector2(0.5f, 1f);
            textRect.pivot = new Vector2(0.5f, 1f);
            textRect.anchoredPosition = new Vector2(0f, -24f);
            textRect.sizeDelta = new Vector2(620f, 130f);
            customPantheonConfirmLabel = text;

            CreateButtonRow(dialog.transform, "CustomPantheonConfirmYesRow", "Yes", -70f, OnCustomPantheonConfirmYes);
            CreateButtonRow(dialog.transform, "CustomPantheonConfirmNoRow", "No", -122f, OnCustomPantheonConfirmNo);
        }

        private void SetCustomPantheonConfirmVisible(bool value)
        {
            customPantheonConfirmVisible = value;
            if (customPantheonConfirmRoot != null)
            {
                customPantheonConfirmRoot.SetActive(value);
            }

            if (!value)
            {
                customPantheonConfirmAction = null;
            }
        }

        private void ShowCustomPantheonConfirm(string message, Action onYes)
        {
            if (customPantheonConfirmLabel != null)
            {
                customPantheonConfirmLabel.text = message;
            }

            SetCustomPantheonConfirmVisible(true);
            customPantheonConfirmAction = onYes;
        }

        private void OnCustomPantheonConfirmYes()
        {
            Action? action = customPantheonConfirmAction;
            SetCustomPantheonConfirmVisible(false);
            action?.Invoke();
        }

        private void OnCustomPantheonConfirmNo()
        {
            SetCustomPantheonConfirmVisible(false);
        }

        private void SetCustomPantheonVisible(bool value)
        {
            customPantheonVisible = value;
            if (value)
            {
                RebuildCustomPantheonOverlay(false);
            }
            else
            {
                SetCustomPantheonConfirmVisible(false);
                customPantheonPicking = false;
                customPantheonEditing = -1;
                customPantheonNameField = null;
                customPantheonFightEditing = -1;
                if (customPantheonRoot != null)
                {
                    customPantheonRoot.SetActive(false);
                }
            }

            UpdateUiState();
        }

        private void RebuildCustomPantheonOverlay(bool keepScroll)
        {
            float? scroll = null;
            if (keepScroll && customPantheonRoot != null)
            {
                ScrollRect? oldScroll = customPantheonRoot.GetComponentInChildren<ScrollRect>();
                if (oldScroll != null && oldScroll.content != null)
                {
                    scroll = oldScroll.content.anchoredPosition.y;
                }
            }

            DestroyRoot(ref customPantheonRoot);
            BuildCustomPantheonOverlayUi();
            if (customPantheonRoot == null)
            {
                return;
            }

            customPantheonRoot.SetActive(customPantheonVisible);
            ScrollRect? newScroll = customPantheonRoot.GetComponentInChildren<ScrollRect>();
            if (scroll != null && newScroll != null && newScroll.content != null)
            {
                Vector2 position = newScroll.content.anchoredPosition;
                newScroll.content.anchoredPosition = new Vector2(position.x, scroll.Value);
            }
        }

        private bool IsCustomPantheonNameFocused() =>
            customPantheonVisible && customPantheonNameField != null && customPantheonNameField.isFocused;

        private void OnQuickCustomPantheonClicked()
        {
            customPantheonEditing = -1;
            customPantheonFightEditing = -1;
            customPantheonPicking = false;
            customPantheonTransfer = CustomPantheonTransferMode.None;
            OpenPanelFromQuick(SetCustomPantheonVisible);
        }

        private void OnCustomPantheonBackClicked()
        {
            if (customPantheonEditing < 0 && LeaveCustomPantheonTransfer())
            {
                return;
            }

            if (customPantheonEditing >= 0 && customPantheonFightEditing >= 0)
            {
                customPantheonFightEditing = -1;
                RebuildCustomPantheonOverlay(false);
                return;
            }

            if (customPantheonEditing >= 0)
            {
                CommitCustomPantheonName();
                customPantheonEditing = -1;
                customPantheonPicking = false;
                RebuildCustomPantheonOverlay(false);
                return;
            }

            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;
            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetCustomPantheonVisible(false);
        }

        private void OpenCustomPantheonChain(int index)
        {
            customPantheonSelected = -1;
            if (index < 0 || index >= CustomPantheonChains.Count)
            {
                return;
            }

            customPantheonEditing = index;
            customPantheonPicking = false;
            RebuildCustomPantheonOverlay(false);
        }

        private void OnCustomPantheonNewClicked()
        {
            List<CustomPantheonChain> chains = CustomPantheonChains;
            CustomPantheonChain chain = new()
            {
                Name = MakeUniqueCustomPantheonName(string.Format("CustomPantheon/DefaultName".Localize(), chains.Count + 1), null)
            };
            chains.Add(chain);
            GodhomeQoL.SaveGlobalSettingsSafe();
            OpenCustomPantheonChain(chains.Count - 1);
        }

        private void StartCustomPantheonDelete(int index)
        {
            if (index < 0 || index >= CustomPantheonChains.Count)
            {
                return;
            }

            CustomPantheonChain chain = CustomPantheonChains[index];
            ShowCustomPantheonConfirm(string.Format("CustomPantheon/ConfirmDelete".Localize(), GetCustomPantheonChainName(chain)), () =>
            {
                CustomPantheonChains.Remove(chain);
                GodhomeQoL.SaveGlobalSettingsSafe();
                RebuildCustomPantheonOverlay(true);
                ShowStatusMessage(string.Format("CustomPantheon/Deleted".Localize(), GetCustomPantheonChainName(chain)));
            });
        }

        private void OnCustomPantheonClearClicked()
        {
            CustomPantheonChain? chain = EditedCustomPantheonChain;
            if (chain == null || chain.Fights.Count == 0)
            {
                return;
            }

            ShowCustomPantheonConfirm(string.Format("CustomPantheon/ConfirmClear".Localize(), GetCustomPantheonChainName(chain)), () =>
            {
                chain.Fights.Clear();
                GodhomeQoL.SaveGlobalSettingsSafe();
                RebuildCustomPantheonOverlay(false);
            });
        }

        private void OnCustomPantheonAddBossClicked()
        {
            CommitCustomPantheonName();
            customPantheonPicking = true;
            RebuildCustomPantheonOverlay(false);
        }

        private void OnCustomPantheonPickerDoneClicked()
        {
            customPantheonPicking = false;
            RebuildCustomPantheonOverlay(false);
        }

        private void SelectCustomPantheonTab(int pantheon)
        {
            if (pantheon == customPantheonPickerTab)
            {
                return;
            }

            customPantheonPickerTab = pantheon;
            RebuildCustomPantheonOverlay(false);
        }

        private void OnCustomPantheonAddBenchClicked()
        {
            CustomPantheonChain? chain = EditedCustomPantheonChain;
            if (chain == null)
            {
                return;
            }

            CommitCustomPantheonName();
            AddCustomPantheonScene(chain, CustomPantheonCatalog.BenchScene);
            RebuildCustomPantheonOverlay(true);
        }

        private void AddCustomPantheonScene(CustomPantheonChain chain, string scene)
        {
            if (chain.Fights.Count >= CustomPantheonCatalog.MaxChainLength)
            {
                ShowStatusMessage(string.Format("CustomPantheon/Full".Localize(), CustomPantheonCatalog.MaxChainLength));
                return;
            }

            int insertAt = customPantheonSelected >= 0 && customPantheonSelected < chain.Fights.Count ? customPantheonSelected + 1 : chain.Fights.Count;
            chain.Fights.Insert(insertAt, new CustomPantheonFight { Scene = scene });
            customPantheonSelected = insertAt;
            GodhomeQoL.SaveGlobalSettingsSafe();
            UpdateCustomPantheonCardCount(chain, scene);
            ShowStatusMessage(string.Format("CustomPantheon/Added".Localize(), FormatCustomPantheonEntry(scene), insertAt + 1));
        }

        private void RemoveCustomPantheonEntry(int index)
        {
            CustomPantheonChain? chain = EditedCustomPantheonChain;
            if (chain == null || index < 0 || index >= chain.Fights.Count)
            {
                return;
            }

            CommitCustomPantheonName();
            chain.Fights.RemoveAt(index);
            customPantheonSelected = chain.Fights.Count == 0 ? -1 : Math.Min(index, chain.Fights.Count - 1);
            GodhomeQoL.SaveGlobalSettingsSafe();
            RebuildCustomPantheonOverlay(true);
        }

        private void CommitCustomPantheonName()
        {
            CustomPantheonChain? chain = EditedCustomPantheonChain;
            if (chain != null && customPantheonNameField != null)
            {
                RenameCustomPantheonChain(chain, customPantheonNameField.text);
            }
        }

        private void RenameCustomPantheonChain(CustomPantheonChain chain, string? value)
        {
            string trimmed = (value ?? string.Empty).Trim();
            if (trimmed.Length == 0)
            {
                if (customPantheonNameField != null)
                {
                    customPantheonNameField.text = chain.Name;
                }

                return;
            }

            string unique = MakeUniqueCustomPantheonName(trimmed, chain);
            if (!string.Equals(unique, chain.Name, StringComparison.Ordinal))
            {
                chain.Name = unique;
                GodhomeQoL.SaveGlobalSettingsSafe();
            }

            if (customPantheonNameField != null && !string.Equals(customPantheonNameField.text, unique, StringComparison.Ordinal))
            {
                customPantheonNameField.text = unique;
            }

            if (customPantheonTitle != null)
            {
                customPantheonTitle.text = GetCustomPantheonChainName(chain);
            }
        }

        private static string MakeUniqueCustomPantheonName(string name, CustomPantheonChain? self)
        {
            string baseName = name.Length > CustomPantheonNameLimit ? name.Substring(0, CustomPantheonNameLimit) : name;
            string candidate = baseName;
            int suffix = 2;
            while (CustomPantheonChains.Any(chain => !ReferenceEquals(chain, self) && string.Equals(chain.Name, candidate, StringComparison.OrdinalIgnoreCase)))
            {
                candidate = $"{baseName} ({suffix++})";
            }

            return candidate;
        }

        private static void AddCustomPantheonSelectedFrame(Transform row)
        {
            GameObject frameObject = new("SelectedFrame");
            frameObject.transform.SetParent(row, false);
            RectTransform frameRect = frameObject.AddComponent<RectTransform>();
            frameRect.anchorMin = Vector2.zero;
            frameRect.anchorMax = Vector2.one;
            frameRect.offsetMin = Vector2.zero;
            frameRect.offsetMax = Vector2.zero;
            Image frame = frameObject.AddComponent<Image>();
            frame.sprite = GetHkChamferSprite(true);
            frame.type = Image.Type.Sliced;
            frame.color = HkFrameOnColor;
            frame.raycastTarget = false;
        }

        private void OnCustomPantheonSelectClicked()
        {
            CustomPantheonChain? chain = EditedCustomPantheonChain;
            if (chain == null)
            {
                return;
            }

            CommitCustomPantheonName();
            CustomPantheonSettings settings = GodhomeQoL.GlobalSettings.CustomPantheon ??= new CustomPantheonSettings();
            settings.Select(chain);
            GodhomeQoL.SaveGlobalSettingsSafe();
            if (customPantheonSelectLabel != null)
            {
                customPantheonSelectLabel.text = "CustomPantheon/SelectedButton".Localize();
                customPantheonSelectLabel.color = HkValueColor;
            }

            ShowStatusMessage(string.Format("CustomPantheon/SelectedChain".Localize(), GetCustomPantheonChainName(chain)));
        }

        private void OnCustomPantheonStartClicked()
        {
            CustomPantheonChain? chain = (GodhomeQoL.GlobalSettings.CustomPantheon ??= new CustomPantheonSettings()).SelectedChain;
            if (chain == null)
            {
                ShowStatusMessage("CustomPantheon/NoSelection".Localize());
                return;
            }

            if (!CustomPantheonRunner.TryStart(chain, out string messageKey))
            {
                ShowStatusMessage(messageKey.Localize());
                return;
            }

            ShowStatusMessage(string.Format("CustomPantheon/Starting".Localize(), GetCustomPantheonChainName(chain)));
            returnToQuickOnClose = false;
            SetQuickVisible(false);
            SetCustomPantheonVisible(false);
        }

        private static string GetCustomPantheonChainName(CustomPantheonChain chain) =>
            string.IsNullOrWhiteSpace(chain.Name) ? "CustomPantheon/Title".Localize() : chain.Name;

        private static string FormatCustomPantheonBossCount(IEnumerable<CustomPantheonFight>? fights) =>
            string.Format("CustomPantheon/BossCount".Localize(), CustomPantheonCatalog.CountBosses(fights));

        private void HideCustomPantheonForSubPanel()
        {
            SetCustomPantheonConfirmVisible(false);
            customPantheonVisible = false;
            if (customPantheonRoot != null)
            {
                customPantheonRoot.SetActive(false);
            }
        }

        private void ReturnToCustomPantheon(int chainIndex, int fightIndex)
        {
            customPantheonEditing = chainIndex;
            customPantheonFightEditing = fightIndex;
            customPantheonPicking = false;
            SetCustomPantheonVisible(true);
        }

        private static GearPreset CreateCustomPantheonFullGear() =>
            GearPresetDefaults.CreateDefaults().TryGetValue("FullGear", out GearPreset? full) && full != null ? full : new GearPreset();

        private void OpenCustomPantheonFight(int index)
        {
            CustomPantheonChain? chain = EditedCustomPantheonChain;
            if (chain == null || index < 0 || index >= chain.Fights.Count)
            {
                return;
            }

            CommitCustomPantheonName();
            customPantheonFightEditing = index;
            RebuildCustomPantheonOverlay(false);
        }

        private void BuildCustomPantheonFightView(CustomPantheonChain chain, int index)
        {
            CustomPantheonFight fight = chain.Fights[index];
            string title = $"{index + 1}. {FormatCustomPantheonEntry(fight.Scene)}";
            float width = BossManipulatePanelWidth;
            float height = BossManipulatePanelHeight;
            customPantheonRoot = CreateOverlayFrame("CustomPantheonCanvas", CustomPantheonCanvasSortOrder, "CustomPantheonPanel", height, title, 46, 0f, out GameObject panel, out RectTransform titleRect);
            ((RectTransform)panel.transform).sizeDelta = new Vector2(width, height);
            titleRect.anchoredPosition = new Vector2(0f, (height * 0.5f) - 70f);
            titleRect.sizeDelta = new Vector2(width - 120f, 60f);
            int chainIndex = customPantheonEditing;

            float resetY = GetFixedResetY(height);
            float backY = GetFixedBackY(height);
            float topOffset = CustomPantheonFightTopOffset;
            float bottomOffset = GetScrollBottomOffset(height, resetY);
            float viewHeight = Mathf.Max(0f, height - topOffset - bottomOffset);
            RectTransform content = CreateCustomPantheonFightColumn(panel.transform, -CustomPantheonFightColumnX, height, topOffset, bottomOffset);
            RectTransform side = CreateCustomPantheonFightColumn(panel.transform, CustomPantheonFightColumnX, height, topOffset, bottomOffset);

            float rowY = CustomPantheonFightFirstRowY;
            CreateCustomPantheonCaptionRow(content, fight, rowY);
            rowY += RowSpacing;

            CreateSelectRow(content, "CustomPantheonFightGearRow", "CustomPantheon/Gear".Localize(), rowY, () => fight.Gear == null ? "FullGear" : "CustomPantheon/GearCustom".Localize(), () =>
            {
                fight.Gear ??= CreateCustomPantheonFullGear();
                fight.Gear.Name = title;
                GodhomeQoL.SaveGlobalSettingsSafe();
                HideCustomPantheonForSubPanel();
                OpenGearSwitcherChainEditor(fight.Gear, string.Format("CustomPantheon/GearTitle".Localize(), title), () => ReturnToCustomPantheon(chainIndex, index));
            }, out _);
            rowY += RowSpacing;

            if (fight.Gear != null)
            {
                CreateButtonRow(content, "CustomPantheonFightGearResetRow", "CustomPantheon/UseFullGear".Localize(), rowY, () =>
                {
                    fight.Gear = null;
                    GodhomeQoL.SaveGlobalSettingsSafe();
                    RebuildCustomPantheonOverlay(true);
                });
                rowY += RowSpacing;
            }

            CreateFloatSliderRow(content, "CustomPantheonFightSpeedRow", "CustomPantheon/GameSpeed".Localize(), rowY, rowY + 40f, 0.1f, CustomPantheonFight.MaxGameSpeed, fight.GameSpeed > 0f ? fight.GameSpeed : 1f, 2, out Text speedValue, out Slider speedSlider);
            InputField speedField = CreateCustomPantheonSpeedField(speedValue, fight.GameSpeed > 0f ? fight.GameSpeed : 1f);
            speedSlider.onValueChanged.AddListener(value =>
            {
                fight.GameSpeed = Mathf.Clamp((float)Math.Round(value * 20f) / 20f, 0.1f, CustomPantheonFight.MaxGameSpeed);
                if (!speedField.isFocused)
                {
                    speedField.text = FormatCustomPantheonSpeed(fight.GameSpeed);
                }
            });
            speedField.onEndEdit.AddListener(text =>
            {
                if (CustomPantheonFight.TryParseGameSpeed(text, out float typed))
                {
                    fight.GameSpeed = typed;
                    speedSlider.SetValueWithoutNotify(typed);
                    GodhomeQoL.SaveGlobalSettingsSafe();
                }

                speedField.text = FormatCustomPantheonSpeed(fight.GameSpeed > 0f ? fight.GameSpeed : 1f);
            });
            rowY += RowSpacing + 40f;

            CreateAdjustInputRow(content, "CustomPantheonFightDamageRow", "CustomPantheon/MaskDamage".Localize(), rowY, () => Math.Max(1, fight.MaskDamage), value =>
            {
                fight.MaskDamage = Mathf.Clamp(value, 1, 999);
            }, 1, 999, 1, out _);
            rowY += RowSpacing;

            CreateToggleRow(content, "CustomPantheonFightRestoreHealthRow", "CustomPantheon/RestoreHealth".Localize(), rowY, () => fight.RestoreHealth, value =>
            {
                fight.RestoreHealth = value;
                GodhomeQoL.SaveGlobalSettingsSafe();
            }, out _);
            rowY += RowSpacing;

            CreateToggleRow(content, "CustomPantheonFightInfiniteHpRow", "CustomPantheon/InfiniteHp".Localize(), rowY, () => fight.InfiniteHp, value =>
            {
                fight.InfiniteHp = value;
                GodhomeQoL.SaveGlobalSettingsSafe();
            }, out _);
            rowY += RowSpacing;

            CreateToggleRow(content, "CustomPantheonFightArrivalRow", "CustomPantheon/ArrivalAnimation".Localize(), rowY, () => fight.ArrivalAnimation, value =>
            {
                fight.ArrivalAnimation = value;
                GodhomeQoL.SaveGlobalSettingsSafe();
            }, out _);
            SetScrollContentHeight(content, viewHeight, rowY, RowHeight);

            float sideY = CustomPantheonFightFirstRowY;
            CreateToggleRow(side, "CustomPantheonFightSoulSetRow", "CustomPantheon/SetSoul".Localize(), sideY, () => fight.Soul.HasValue, value =>
            {
                fight.Soul = value ? fight.Soul ?? 99 : null;
                GodhomeQoL.SaveGlobalSettingsSafe();
                RebuildCustomPantheonOverlay(true);
            }, out _);
            sideY += RowSpacing;
            if (fight.Soul.HasValue)
            {
                CreateAdjustInputRow(side, "CustomPantheonFightSoulRow", "CustomPantheon/Soul".Localize(), sideY, () => fight.Soul ?? 0, value =>
                {
                    fight.Soul = Mathf.Clamp(value, 0, CustomPantheonFight.MaxSoul);
                }, 0, CustomPantheonFight.MaxSoul, 11, out _);
                sideY += RowSpacing;
            }

            CreateToggleRow(side, "CustomPantheonFightLifebloodSetRow", "CustomPantheon/SetLifeblood".Localize(), sideY, () => fight.Lifeblood.HasValue, value =>
            {
                fight.Lifeblood = value ? fight.Lifeblood ?? 4 : null;
                GodhomeQoL.SaveGlobalSettingsSafe();
                RebuildCustomPantheonOverlay(true);
            }, out _);
            float lastY = sideY;
            sideY += RowSpacing;
            if (fight.Lifeblood.HasValue)
            {
                CreateAdjustInputRow(side, "CustomPantheonFightLifebloodRow", "CustomPantheon/Lifeblood".Localize(), sideY, () => fight.Lifeblood ?? 0, value =>
                {
                    fight.Lifeblood = Mathf.Clamp(value, 0, CustomPantheonFight.MaxLifeblood);
                }, 0, CustomPantheonFight.MaxLifeblood, 1, out _);
                lastY = sideY;
                sideY += RowSpacing;
            }

            if (CanEditCustomPantheonBoss(fight))
            {
                CreateReadOnlyValueRow(side, "CustomPantheonFightBossRow", "CustomPantheon/BossSettings".Localize(), sideY, () => fight.HasBossSettings ? "ON" : "OFF", out Text bossValue);
                UpdateToggleValue(bossValue, fight.HasBossSettings);
                sideY += RowSpacing;
                CreateButtonRow(side, "CustomPantheonFightBossEditRow", "CustomPantheon/EditBoss".Localize(), sideY, () => StartCustomPantheonBossEdit(chainIndex, index));
                lastY = sideY;
                sideY += RowSpacing;
                if (fight.BossSettings != null)
                {
                    CreateButtonRow(side, "CustomPantheonFightBossClearRow", "CustomPantheon/ClearBoss".Localize(), sideY, () =>
                    {
                        fight.BossSettings = null;
                        fight.BossEnabled = false;
                        GodhomeQoL.SaveGlobalSettingsSafe();
                        RebuildCustomPantheonOverlay(true);
                    });
                    lastY = sideY;
                }
            }
            else
            {
                GameObject note = CreateRow(side, "CustomPantheonFightNoBossRow", sideY, new Vector2(RowWidth, RowHeight));
                Text noteText = CreateText(note.transform, "Label", "CustomPantheon/NoBossSettings".Localize(), 22, TextAnchor.MiddleCenter);
                StretchCustomPantheonText(noteText.rectTransform);
                noteText.color = CustomPantheonDimColor;
                lastY = sideY;
            }

            SetScrollContentHeight(side, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "CustomPantheonFightResetRow", "CustomPantheon/ResetFight".Localize(), resetY, () =>
            {
                fight.Gear = null;
                fight.BossSettings = null;
                fight.BossEnabled = false;
                fight.GameSpeed = 1f;
                fight.MaskDamage = 1;
                fight.RestoreHealth = true;
                fight.InfiniteHp = false;
                fight.ArrivalAnimation = false;
                fight.Soul = null;
                fight.Lifeblood = null;
                GodhomeQoL.SaveGlobalSettingsSafe();
                RebuildCustomPantheonOverlay(false);
            });
            CreateButtonRow(panel.transform, "CustomPantheonBackRow", "Back", backY, OnCustomPantheonBackClicked);
            CreateCustomPantheonConfirm(panel.transform);
            SetCustomPantheonConfirmVisible(false);
        }

        private InputField CreateCustomPantheonSpeedField(Text valueText, float value)
        {
            Transform row = valueText.transform.parent;
            valueText.gameObject.SetActive(false);
            InputField field = CreateInputField(row, FormatCustomPantheonSpeed(value), new Vector2(InputControlValueRight, 0f), new Vector2(InputControlValueWidth, RowHeight));
            field.contentType = InputField.ContentType.Standard;
            field.characterLimit = 6;
            field.text = FormatCustomPantheonSpeed(value);
            return field;
        }

        private static string FormatCustomPantheonSpeed(float value) =>
            value.ToString("0.0#", System.Globalization.CultureInfo.InvariantCulture);

        private const float CustomPantheonFightColumnX = PanelWidth * 0.5f;
        private const float CustomPantheonFightTopOffset = 175f;
        private const float CustomPantheonFightFirstRowY = 30f;

        private static RectTransform CreateCustomPantheonFightColumn(Transform panel, float x, float height, float topOffset, float bottomOffset)
        {
            RectTransform column = CreateScrollContent(panel, PanelWidth, height, topOffset, bottomOffset);
            RectTransform? scroll = column.parent != null ? column.parent.parent as RectTransform : null;
            if (scroll != null)
            {
                scroll.anchoredPosition = new Vector2(x, scroll.anchoredPosition.y);
            }

            return column;
        }

        private static string FormatCustomPantheonFightTags(CustomPantheonFight fight)
        {
            List<string> tags = new();
            if (fight.Gear != null)
            {
                tags.Add("CustomPantheon/TagGear".Localize());
            }

            if (fight.HasBossSettings)
            {
                tags.Add("CustomPantheon/TagBoss".Localize());
            }

            if (fight.GameSpeed > 0f && !Mathf.Approximately(fight.GameSpeed, 1f))
            {
                tags.Add($"x{fight.GameSpeed:0.##}");
            }

            if (fight.MaskDamage > 1)
            {
                tags.Add(string.Format("CustomPantheon/TagDamage".Localize(), fight.MaskDamage));
            }

            if (!fight.RestoreHealth)
            {
                tags.Add("CustomPantheon/TagNoHeal".Localize());
            }

            if (fight.InfiniteHp)
            {
                tags.Add("CustomPantheon/TagInfiniteHp".Localize());
            }

            if (fight.ArrivalAnimation)
            {
                tags.Add("CustomPantheon/TagArrival".Localize());
            }

            if (fight.Soul.HasValue)
            {
                tags.Add(string.Format("CustomPantheon/TagSoul".Localize(), fight.Soul.Value));
            }

            if (fight.Lifeblood.HasValue)
            {
                tags.Add(string.Format("CustomPantheon/TagLifeblood".Localize(), fight.Lifeblood.Value));
            }

            if (tags.Count == 0)
            {
                return string.Empty;
            }

            string gold = ColorUtility.ToHtmlStringRGB(HkValueColor);
            return $"  <color=#{gold}><size=20>{string.Join(", ", tags)}</size></color>";
        }

        private static string FormatCustomPantheonEntry(string scene)
        {
            if (CustomPantheonCatalog.IsBench(scene))
            {
                return "CustomPantheon/Bench".Localize();
            }

            if (!CustomPantheonCatalog.TryGet(scene, out CustomPantheonBoss boss))
            {
                return scene;
            }

            string tag = CustomPantheonCatalog.VersionTag(scene);
            return tag.Length > 0 ? $"{boss.Key} ({tag})" : boss.Key;
        }

        private static void StretchCustomPantheonText(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
