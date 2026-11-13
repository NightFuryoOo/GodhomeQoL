using GodhomeQoL.Modules.BossChallenge;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private static readonly Color CustomPantheonDropColor = new(1f, 0.95f, 0.75f, 1f);

        private Vector2 customPantheonDragPointer;

        private void BeginCustomPantheonDrag(int index, PointerEventData data)
        {
            if (data.button != PointerEventData.InputButton.Left || !customPantheonVisible)
            {
                return;
            }

            CancelCustomPantheonDrag();
            customPantheonDragSource = index;
            customPantheonDragTarget = index;
            customPantheonDragStart = data.pressPosition != Vector2.zero ? data.pressPosition : data.position;
            customPantheonDragPointer = data.position;
        }

        private void MoveCustomPantheonDrag(GameObject card, PointerEventData data)
        {
            if (customPantheonDragSource < 0)
            {
                return;
            }

            customPantheonDragPointer = data.position;
            if (!customPantheonDragActive)
            {
                if ((data.position - customPantheonDragStart).magnitude < CustomPantheonDragThreshold)
                {
                    return;
                }

                StartCustomPantheonDragVisual(card);
            }

            UpdateCustomPantheonDragPosition();
        }

        private void EndCustomPantheonDrag(PointerEventData data)
        {
            if (customPantheonDragSource < 0)
            {
                return;
            }

            customPantheonDragPointer = data.position;
            bool active = customPantheonDragActive;
            int source = customPantheonDragSource;
            int target = active ? GetCustomPantheonDropIndex(data.position) : source;
            CancelCustomPantheonDrag();
            if (!active)
            {
                return;
            }

            CustomPantheonChain? chain = EditedCustomPantheonChain;
            if (chain != null && source >= 0 && source < chain.Fights.Count && target >= chain.Fights.Count)
            {
                CopyCustomPantheonFight(chain, source);
                return;
            }

            if (chain == null || source < 0 || source >= chain.Fights.Count || target < 0 || target >= chain.Fights.Count || source == target)
            {
                UpdateCustomPantheonSelection();
                return;
            }

            CommitCustomPantheonName();
            CustomPantheonFight fight = chain.Fights[source];
            chain.Fights.RemoveAt(source);
            chain.Fights.Insert(target, fight);
            customPantheonSelected = target;
            GodhomeQoL.SaveGlobalSettingsSafe();
            RebuildCustomPantheonOverlay(true);
            ShowStatusMessage(string.Format("CustomPantheon/Moved".Localize(), source + 1, target + 1));
        }

        private void CopyCustomPantheonFight(CustomPantheonChain chain, int source)
        {
            if (chain.Fights.Count >= CustomPantheonCatalog.MaxChainLength)
            {
                ShowStatusMessage(string.Format("CustomPantheon/Full".Localize(), CustomPantheonCatalog.MaxChainLength));
                UpdateCustomPantheonSelection();
                return;
            }

            CustomPantheonFight? copy = JsonConvert.DeserializeObject<CustomPantheonFight>(JsonConvert.SerializeObject(chain.Fights[source]));
            if (copy == null)
            {
                UpdateCustomPantheonSelection();
                return;
            }

            CommitCustomPantheonName();
            chain.Fights.Add(copy);
            customPantheonSelected = chain.Fights.Count - 1;
            GodhomeQoL.SaveGlobalSettingsSafe();
            RebuildCustomPantheonOverlay(true);
            ShowStatusMessage(string.Format("CustomPantheon/Copied".Localize(), source + 1, chain.Fights.Count));
        }

        private void StartCustomPantheonDragVisual(GameObject card)
        {
            customPantheonDragActive = true;
            if (customPantheonDragSource >= 0 && customPantheonDragSource < customPantheonCardGroups.Count)
            {
                CanvasGroup group = customPantheonCardGroups[customPantheonDragSource];
                if (group != null)
                {
                    group.alpha = 0.3f;
                }
            }

            if (customPantheonRoot == null)
            {
                return;
            }

            GameObject ghost = UObject.Instantiate(card, customPantheonRoot.transform, false);
            ghost.name = "DragGhost";
            foreach (MonoBehaviour behaviour in ghost.GetComponents<MonoBehaviour>())
            {
                if (behaviour is CustomPantheonCardDrag || behaviour is Button)
                {
                    behaviour.enabled = false;
                }
            }

            CanvasGroup ghostGroup = ghost.GetComponent<CanvasGroup>() ?? ghost.AddComponent<CanvasGroup>();
            ghostGroup.alpha = 0.9f;
            ghostGroup.blocksRaycasts = false;
            ghostGroup.interactable = false;
            RectTransform rect = (RectTransform)ghost.transform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localScale = new Vector3(1.1f, 1.1f, 1f);
            ghost.transform.SetAsLastSibling();
            customPantheonDragGhost = rect;
        }

        private void UpdateCustomPantheonDragPosition()
        {
            if (!customPantheonDragActive)
            {
                return;
            }

            if (customPantheonDragGhost != null && customPantheonRoot != null
                && RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)customPantheonRoot.transform, customPantheonDragPointer, null, out Vector2 local))
            {
                customPantheonDragGhost.anchoredPosition = local;
            }

            int target = GetCustomPantheonDropIndex(customPantheonDragPointer);
            if (target == customPantheonDragTarget)
            {
                return;
            }

            customPantheonDragTarget = target;
            int count = customPantheonCardFrames.Count;
            bool copy = target >= count;
            for (int i = 0; i < count; i++)
            {
                Image frame = customPantheonCardFrames[i];
                if (frame != null)
                {
                    frame.color = !copy && i == target ? CustomPantheonDropColor : i == customPantheonSelected ? HkValueColor : Color.clear;
                }
            }

            bool canCopy = copy && count < CustomPantheonCatalog.MaxChainLength;
            if (customPantheonDropIndicator != null)
            {
                customPantheonDropIndicator.gameObject.SetActive(canCopy);
                if (canCopy)
                {
                    customPantheonDropIndicator.anchoredPosition = GetCustomPantheonCellCenter(count);
                    customPantheonDropIndicator.SetAsLastSibling();
                }
            }

            if (customPantheonInfoText != null && target >= 0)
            {
                customPantheonInfoText.text = copy
                    ? canCopy
                        ? string.Format("CustomPantheon/CopyHint".Localize(), customPantheonDragSource + 1, count + 1)
                        : string.Format("CustomPantheon/Full".Localize(), CustomPantheonCatalog.MaxChainLength)
                    : string.Format("CustomPantheon/MoveHint".Localize(), customPantheonDragSource + 1, target + 1);
            }
        }

        private int GetCustomPantheonDropIndex(Vector2 screen)
        {
            CustomPantheonChain? chain = EditedCustomPantheonChain;
            if (chain == null || chain.Fights.Count == 0 || customPantheonGridContent == null || customPantheonGridCellWidth <= 0f)
            {
                return customPantheonDragSource;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(customPantheonGridContent, screen, null, out Vector2 local))
            {
                return customPantheonDragTarget;
            }

            int column = Mathf.Clamp(Mathf.FloorToInt((local.x - customPantheonGridLeft) / customPantheonGridCellWidth), 0, CustomPantheonGridColumns - 1);
            int row = Mathf.Max(0, Mathf.FloorToInt((customPantheonGridTop - local.y) / CustomPantheonCardStepY));
            return Mathf.Max(0, (row * CustomPantheonGridColumns) + column);
        }

        private void UpdateCustomPantheonDrag()
        {
            if (!customPantheonDragActive || customPantheonGridScroll == null || customPantheonGridContent == null)
            {
                return;
            }

            RectTransform? viewport = customPantheonGridScroll.viewport;
            if (viewport == null
                || !RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, customPantheonDragPointer, null, out Vector2 local))
            {
                return;
            }

            Rect area = viewport.rect;
            float direction = local.y > area.yMax - CustomPantheonAutoScrollZone ? -1f
                : local.y < area.yMin + CustomPantheonAutoScrollZone ? 1f
                : 0f;
            if (direction == 0f)
            {
                return;
            }

            float maxOffset = Mathf.Max(0f, customPantheonGridContent.rect.height - area.height);
            Vector2 position = customPantheonGridContent.anchoredPosition;
            position.y = Mathf.Clamp(position.y + (direction * CustomPantheonAutoScrollSpeed * Time.unscaledDeltaTime), 0f, maxOffset);
            customPantheonGridContent.anchoredPosition = position;
            customPantheonGridScroll.velocity = Vector2.zero;
            UpdateCustomPantheonDragPosition();
        }

        private void CancelCustomPantheonDrag()
        {
            if (customPantheonDragGhost != null)
            {
                UObject.Destroy(customPantheonDragGhost.gameObject);
                customPantheonDragGhost = null;
            }

            if (customPantheonDropIndicator != null)
            {
                customPantheonDropIndicator.gameObject.SetActive(false);
            }

            if (customPantheonDragSource >= 0 && customPantheonDragSource < customPantheonCardGroups.Count)
            {
                CanvasGroup group = customPantheonCardGroups[customPantheonDragSource];
                if (group != null)
                {
                    group.alpha = 1f;
                }
            }

            bool wasActive = customPantheonDragActive;
            customPantheonDragSource = -1;
            customPantheonDragTarget = -1;
            customPantheonDragActive = false;
            if (wasActive)
            {
                UpdateCustomPantheonSelection();
            }
        }
    }
}
