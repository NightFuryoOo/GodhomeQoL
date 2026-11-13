using System.IO;
using GodhomeQoL.Modules.BossChallenge;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private enum CustomPantheonTransferMode
        {
            None,
            Export,
            Import
        }

        private const float CustomPantheonTransferButtonX = 150f;

        private CustomPantheonTransferMode customPantheonTransfer;
        private readonly HashSet<CustomPantheonChain> customPantheonExportPicks = new();

        private void CreateCustomPantheonSideButton(Transform panel, string name, string label, float x, float y, Action onClick)
        {
            CreateButtonRow(panel, name, label, y, onClick);
            if (panel.Find(name) is RectTransform rect)
            {
                rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
            }
        }

        private void OnCustomPantheonExportClicked()
        {
            customPantheonExportPicks.Clear();
            foreach (CustomPantheonChain chain in CustomPantheonChains)
            {
                customPantheonExportPicks.Add(chain);
            }

            customPantheonTransfer = CustomPantheonTransferMode.Export;
            RebuildCustomPantheonOverlay(false);
        }

        private void OnCustomPantheonImportClicked()
        {
            customPantheonTransfer = CustomPantheonTransferMode.Import;
            RebuildCustomPantheonOverlay(false);
        }

        private bool LeaveCustomPantheonTransfer()
        {
            if (customPantheonTransfer == CustomPantheonTransferMode.None)
            {
                return false;
            }

            customPantheonTransfer = CustomPantheonTransferMode.None;
            customPantheonExportPicks.Clear();
            RebuildCustomPantheonOverlay(false);
            return true;
        }

        private RectTransform CreateCustomPantheonTransferFrame(string titleKey, out GameObject panel, out float firstButtonY, out float secondButtonY, out float backY, out float rowY, out float viewHeight)
        {
            customPantheonRoot = CreateOverlayFrame("CustomPantheonCanvas", CustomPantheonCanvasSortOrder, "CustomPantheonPanel", PanelHeight, titleKey.Localize(), 52, 210f, out panel, out RectTransform titleRect);
            secondButtonY = GetFixedResetY(PanelHeight);
            firstButtonY = secondButtonY + (FixedResetOffset - FixedBackOffset);
            backY = GetFixedBackY(PanelHeight);
            float topOffset = GetScrollTopOffset(PanelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(PanelHeight, firstButtonY);
            viewHeight = Mathf.Max(0f, PanelHeight - topOffset - bottomOffset);
            rowY = GetRowStartY(PanelHeight, RowStartY, topOffset);
            return CreateScrollContent(panel.transform, PanelWidth, PanelHeight, topOffset, bottomOffset);
        }

        private void CreateCustomPantheonHintRow(RectTransform content, string name, string text, float y)
        {
            GameObject hintRow = CreateRow(content, name, y, new Vector2(RowWidth, RowHeight * 2f));
            Text hint = CreateText(hintRow.transform, "Label", text, 24, TextAnchor.MiddleCenter);
            StretchCustomPantheonText(hint.rectTransform);
            hint.horizontalOverflow = HorizontalWrapMode.Wrap;
            hint.color = CustomPantheonDimColor;
        }

        private void BuildCustomPantheonExportView()
        {
            RectTransform content = CreateCustomPantheonTransferFrame("CustomPantheon/ExportTitle", out GameObject panel, out float saveY, out float allY, out float backY, out float rowY, out float viewHeight);
            List<CustomPantheonChain> chains = CustomPantheonChains;
            float lastY = rowY;
            if (chains.Count == 0)
            {
                CreateCustomPantheonHintRow(content, "CustomPantheonExportEmptyRow", "CustomPantheon/NoChains".Localize(), rowY);
            }

            for (int i = 0; i < chains.Count; i++)
            {
                CustomPantheonChain chain = chains[i];
                string label = string.Format("CustomPantheon/ExportRow".Localize(), GetCustomPantheonChainName(chain), CustomPantheonCatalog.CountBosses(chain.Fights));
                CreateToggleRow(content, $"CustomPantheonExport{i}", label, rowY, () => customPantheonExportPicks.Contains(chain), value =>
                {
                    if (value)
                    {
                        customPantheonExportPicks.Add(chain);
                    }
                    else
                    {
                        customPantheonExportPicks.Remove(chain);
                    }
                }, out _);
                lastY = rowY;
                rowY += RowSpacing;
            }

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "CustomPantheonExportSaveRow", "CustomPantheon/ExportSave".Localize(), saveY, OnCustomPantheonExportSaveClicked);
            CreateButtonRow(panel.transform, "CustomPantheonExportAllRow", "CustomPantheon/ExportAll".Localize(), allY, OnCustomPantheonExportAllClicked);
            CreateButtonRow(panel.transform, "CustomPantheonBackRow", "Back", backY, OnCustomPantheonBackClicked);
        }

        private void OnCustomPantheonExportAllClicked()
        {
            List<CustomPantheonChain> chains = CustomPantheonChains;
            bool allPicked = chains.Count > 0 && chains.All(customPantheonExportPicks.Contains);
            customPantheonExportPicks.Clear();
            if (!allPicked)
            {
                foreach (CustomPantheonChain chain in chains)
                {
                    customPantheonExportPicks.Add(chain);
                }
            }

            RebuildCustomPantheonOverlay(true);
        }

        private void OnCustomPantheonExportSaveClicked()
        {
            List<CustomPantheonChain> picked = CustomPantheonChains.Where(customPantheonExportPicks.Contains).ToList();
            if (picked.Count == 0)
            {
                ShowStatusMessage("CustomPantheon/ExportNone".Localize());
                return;
            }

            if (!CustomPantheonTransfer.TryExport(picked, out string path))
            {
                ShowStatusMessage("CustomPantheon/ExportFailed".Localize());
                return;
            }

            CustomPantheonTransfer.OpenFolder();
            LeaveCustomPantheonTransfer();
            ShowStatusMessage(string.Format("CustomPantheon/Exported".Localize(), picked.Count, Path.GetFileName(path)));
        }

        private void BuildCustomPantheonImportView()
        {
            RectTransform content = CreateCustomPantheonTransferFrame("CustomPantheon/ImportTitle", out GameObject panel, out float folderY, out float unusedY, out float backY, out float rowY, out float viewHeight);
            List<FileInfo> files = CustomPantheonTransfer.ListFiles();
            float lastY = rowY;
            if (files.Count == 0)
            {
                CreateCustomPantheonHintRow(content, "CustomPantheonImportEmptyRow", string.Format("CustomPantheon/NoFiles".Localize(), CustomPantheonTransfer.FolderName), rowY);
            }

            for (int i = 0; i < files.Count; i++)
            {
                FileInfo file = files[i];
                string date = file.LastWriteTime.ToString("dd.MM.yyyy HH:mm");
                CreateSelectRow(content, $"CustomPantheonImport{i}", Path.GetFileNameWithoutExtension(file.Name), rowY, () => date, () => ImportCustomPantheonFile(file.FullName), out _);
                lastY = rowY;
                rowY += RowSpacing;
            }

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "CustomPantheonImportFolderRow", "CustomPantheon/OpenFolder".Localize(), unusedY, CustomPantheonTransfer.OpenFolder);
            CreateButtonRow(panel.transform, "CustomPantheonBackRow", "Back", backY, OnCustomPantheonBackClicked);
        }

        private void ImportCustomPantheonFile(string path)
        {
            List<CustomPantheonChain>? chains = CustomPantheonTransfer.TryRead(path);
            if (chains == null)
            {
                ShowStatusMessage("CustomPantheon/ImportFailed".Localize());
                return;
            }

            if (chains.Count == 0)
            {
                ShowStatusMessage("CustomPantheon/ImportEmpty".Localize());
                return;
            }

            List<CustomPantheonChain> target = CustomPantheonChains;
            foreach (CustomPantheonChain chain in chains)
            {
                string name = string.IsNullOrWhiteSpace(chain.Name)
                    ? string.Format("CustomPantheon/DefaultName".Localize(), target.Count + 1)
                    : chain.Name;
                chain.Name = MakeUniqueCustomPantheonName(name, null);
                target.Add(chain);
            }

            GodhomeQoL.SaveGlobalSettingsSafe();
            LeaveCustomPantheonTransfer();
            ShowStatusMessage(string.Format("CustomPantheon/Imported".Localize(), chains.Count, Path.GetFileName(path)));
        }
    }
}
