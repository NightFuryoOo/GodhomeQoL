using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void Update()
        {
            bool wasRebinding = IsAnyRebinding();
            bool suppressHotkeysThisFrame = overlayHotkeySuppressFrames > 0;
            if (overlayHotkeySuppressFrames > 0)
            {
                overlayHotkeySuppressFrames--;
            }
            if (fastReloadVisible)
            {
                HandleFastReloadRebind();
            }

            if (showHpOnDeathVisible)
            {
                HandleShowHpOnDeathRebind();
            }

            if (maskDamageVisible)
            {
                HandleMaskDamageUiRebind();
            }

            if (freezeHitboxesVisible)
            {
                HandleFreezeHitboxesRebind();
            }

            if (speedChangerVisible)
            {
                HandleSpeedChangerRebind();
            }

            if (teleportKitVisible)
            {
                HandleTeleportKitRebind();
            }

            if (waitingForFastDreamWarpRebind)
            {
                HandleFastDreamWarpRebind();
            }

            if (waitingForQuickMenuHotkeyRebind)
            {
                HandleQuickMenuHotkeyRebind();
            }

            if (waitingForOverlayHotkeyRebind)
            {
                HandleOverlayHotkeyRebind();
            }

            if (quickVisible)
            {
                HandleQuickMenuRename();
            }

            if (gearSwitcherPresetVisible)
            {
                HandleGearSwitcherPresetRename();
            }

            if (!wasRebinding && !IsAnyRebinding() && !IsHotkeyInputBlocked() && !suppressHotkeysThisFrame)
            {
                HandleFastDreamWarpHotkey();
                HandleOverlayHotkeys();
            }

            KeyCode toggleKey = GetQuickMenuToggleKey();
            if (!wasRebinding && !IsHotkeyInputBlocked() && !suppressHotkeysThisFrame && toggleKey != KeyCode.None && Input.GetKeyDown(toggleKey))
            {
                ToggleMenu();
            }

            if (quickVisible || quickSettingsVisible || overlayVisible || collectorVisible || fastReloadVisible || dreamshieldVisible || showHpOnDeathVisible || maskDamageVisible || freezeHitboxesVisible || speedChangerVisible || teleportKitVisible || bossChallengeVisible || randomPantheonsVisible || alwaysFuriousVisible || gearSwitcherVisible || gearSwitcherCharmCostVisible || gearSwitcherPresetVisible || qolVisible || menuAnimationVisible || bossAnimationVisible || zoteHelperVisible)
            {
                MaintainUiInteraction();
            }

            UpdateStatusMessage();
        }

        private void LateUpdate()
        {
            if (quickVisible || quickSettingsVisible || overlayVisible || collectorVisible || fastReloadVisible || dreamshieldVisible || showHpOnDeathVisible || maskDamageVisible || speedChangerVisible || teleportKitVisible || bossChallengeVisible || randomPantheonsVisible || alwaysFuriousVisible || gearSwitcherVisible || gearSwitcherCharmCostVisible || gearSwitcherPresetVisible || qolVisible || menuAnimationVisible || bossAnimationVisible || zoteHelperVisible)
            {
                MaintainUiInteraction();
            }

            if (IsOverlayHighlightActive())
            {
                UpdateManualHighlight();
            }
            else
            {
                ClearManualHighlight();
            }
        }

        private bool IsOverlayHighlightActive()
        {
            return quickSettingsVisible
                || overlayVisible
                || collectorVisible
                || fastReloadVisible
                || dreamshieldVisible
                || showHpOnDeathVisible
                || speedChangerVisible
                || teleportKitVisible
                || bossChallengeVisible
                || randomPantheonsVisible
                || alwaysFuriousVisible
                || gearSwitcherVisible
                || gearSwitcherCharmCostVisible
                || gearSwitcherPresetVisible
                || qolVisible
                || menuAnimationVisible
                || bossAnimationVisible
                || zoteHelperVisible;
        }

        private void UpdateManualHighlight()
        {
            EventSystem? eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                ClearManualHighlight();
                return;
            }

            RowHighlight? target = null;
            PointerEventData data = GetHoverEventData(eventSystem);
            hoverResults.Clear();
            eventSystem.RaycastAll(data, hoverResults);

            for (int i = 0; i < hoverResults.Count; i++)
            {
                RowHighlight? highlight = GetHighlightFromObject(hoverResults[i].gameObject);
                if (highlight != null)
                {
                    target = highlight;
                    break;
                }
            }

            if (target == null)
            {
                GameObject? selected = eventSystem.currentSelectedGameObject;
                if (selected != null)
                {
                    target = GetHighlightFromObject(selected);
                }
            }

            SetManualHighlight(target);
        }

        private void ClearManualHighlight()
        {
            SetManualHighlight(null);
        }

        private void SetManualHighlight(RowHighlight? highlight)
        {
            if (manualHighlight == highlight)
            {
                return;
            }

            if (manualHighlight != null)
            {
                manualHighlight.SetManualActive(false);
            }

            manualHighlight = highlight;

            if (manualHighlight != null)
            {
                manualHighlight.SetManualActive(true);
            }
        }

        private PointerEventData GetHoverEventData(EventSystem eventSystem)
        {
            if (hoverEventData == null || hoverEventSystem != eventSystem)
            {
                hoverEventData = new PointerEventData(eventSystem);
                hoverEventSystem = eventSystem;
            }

            hoverEventData.position = Input.mousePosition;
            hoverEventData.delta = Vector2.zero;
            hoverEventData.scrollDelta = Vector2.zero;
            hoverEventData.button = PointerEventData.InputButton.Left;
            return hoverEventData;
        }

        private static RowHighlight? GetHighlightFromObject(GameObject obj)
        {
            RowHighlight? highlight = obj.GetComponentInParent<RowHighlight>();
            if (highlight != null)
            {
                return highlight;
            }

            RowHighlightTrigger? trigger = obj.GetComponentInParent<RowHighlightTrigger>();
            return trigger?.Highlight;
        }
    }
}
