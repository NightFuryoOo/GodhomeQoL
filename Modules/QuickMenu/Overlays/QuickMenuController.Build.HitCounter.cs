using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private const int HitCounterCanvasSortOrder = 10021;
        private const float HitCounterResetConfirmSeconds = 4f;

        private GameObject? hitCounterRoot;
        private RectTransform? hitCounterContent;
        private bool hitCounterVisible;
        private Module? hitCounterModule;
        private Text? hitCounterToggleValue;
        private Image? hitCounterToggleIcon;
        private Text? hitCounterHallOfGodsValue;
        private Text? hitCounterKeyValue;
        private Text? hitCounterSplitsValue;
        private Slider? hitCounterSplitsSlider;
        private Text? hitCounterOpacityValue;
        private Slider? hitCounterOpacitySlider;
        private Text? hitCounterScaleValue;
        private Slider? hitCounterScaleSlider;
        private Text? hitCounterPosXValue;
        private Slider? hitCounterPosXSlider;
        private Text? hitCounterPosYValue;
        private Slider? hitCounterPosYSlider;
        private bool hitCounterSliderSync;
        private bool hitCounterMoveMode;
        private bool waitingForHitCounterRebind;
        private string hitCounterPrevKey = string.Empty;
        private float hitCounterResetArmedUntil = -1f;

        private void BuildHitCounterOverlayUi()
        {
            hitCounterRoot = CreateOverlayFrame("HitCounterOverlayCanvas", HitCounterCanvasSortOrder, "HitCounterPanel", PanelHeight, "Modules/HitCounter".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = PanelHeight;
            RectTransform content = CreateOverlayContent(panel, PanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            hitCounterContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "HitCounterEnableRow",
                "HitCounter/Enable".Localize(),
                rowY,
                GetHitCounterEnabled,
                SetHitCounterEnabled,
                out hitCounterToggleValue,
                out hitCounterToggleIcon
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "HitCounterHallOfGodsRow",
                "HitCounter/HallOfGods".Localize(),
                rowY,
                () => HitCounter.ShowInHallOfGods,
                HitCounter.SetShowInHallOfGods,
                out hitCounterHallOfGodsValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateKeybindRow(
                content,
                "HitCounterKeyRow",
                "HitCounter/ToggleKey".Localize(),
                rowY,
                GetHitCounterKeyLabel,
                StartHitCounterRebind,
                out hitCounterKeyValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateFloatSliderRow(
                content,
                "HitCounterSplitsRow",
                "HitCounter/VisibleSplits".Localize(),
                rowY,
                rowY + 40f,
                HitCounter.MinVisibleSplits,
                HitCounter.MaxVisibleSplits,
                HitCounter.VisibleSplitsClamped,
                0,
                out hitCounterSplitsValue,
                out hitCounterSplitsSlider
            );
            hitCounterSplitsSlider?.onValueChanged.AddListener(OnHitCounterSplitsChanged);

            lastY = rowY + 40f;
            rowY += RowSpacing + 40f;
            CreateFloatSliderRow(
                content,
                "HitCounterOpacityRow",
                "HitCounter/Opacity".Localize(),
                rowY,
                rowY + 40f,
                HitCounter.MinOpacity * 100f,
                100f,
                HitCounter.OpacityPercent,
                0,
                out hitCounterOpacityValue,
                out hitCounterOpacitySlider
            );
            hitCounterOpacitySlider?.onValueChanged.AddListener(OnHitCounterOpacityChanged);

            lastY = rowY + 40f;
            rowY += RowSpacing + 40f;
            CreateFloatSliderRow(
                content,
                "HitCounterScaleRow",
                "HitCounter/Size".Localize(),
                rowY,
                rowY + 40f,
                HitCounter.MinScale * 100f,
                HitCounter.MaxScale * 100f,
                HitCounter.ScalePercent,
                0,
                out hitCounterScaleValue,
                out hitCounterScaleSlider
            );
            hitCounterScaleSlider?.onValueChanged.AddListener(OnHitCounterScaleChanged);

            lastY = rowY + 40f;
            rowY += RowSpacing + 40f;
            CreateFloatSliderRow(
                content,
                "HitCounterPosXRow",
                "HitCounter/PositionX".Localize(),
                rowY,
                rowY + 40f,
                0f,
                100f,
                HitCounter.PositionXPercent,
                0,
                out hitCounterPosXValue,
                out hitCounterPosXSlider
            );
            hitCounterPosXSlider?.onValueChanged.AddListener(OnHitCounterPosXChanged);

            lastY = rowY + 40f;
            rowY += RowSpacing + 40f;
            CreateFloatSliderRow(
                content,
                "HitCounterPosYRow",
                "HitCounter/PositionY".Localize(),
                rowY,
                rowY + 40f,
                0f,
                100f,
                HitCounter.PositionYPercent,
                0,
                out hitCounterPosYValue,
                out hitCounterPosYSlider
            );
            hitCounterPosYSlider?.onValueChanged.AddListener(OnHitCounterPosYChanged);

            lastY = rowY + 40f;
            rowY += RowSpacing + 40f;
            CreateButtonRow(content, "HitCounterMoveRow", "HitCounter/Move".Localize(), rowY, StartHitCounterMove);
            lastY = rowY;
            rowY += RowSpacing;
            CreateButtonRow(content, "HitCounterResetPbRow", "HitCounter/ResetPbs".Localize(), rowY, OnHitCounterResetPbsClicked);
            lastY = rowY;
            rowY += RowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "HitCounterResetRow", "HitCounter/Reset".Localize(), resetY, OnHitCounterResetDefaultsClicked);
            CreateButtonRow(panel.transform, "HitCounterBackRow", "Back", backY, OnHitCounterBackClicked);
        }

        private Module? GetHitCounterModule()
        {
            return GetCachedModule(ref hitCounterModule, typeof(HitCounter));
        }

        private bool GetHitCounterEnabled()
        {
            return GetHitCounterModule()?.Enabled ?? false;
        }

        private void SetHitCounterEnabled(bool value)
        {
            SetModuleEnabledFlag(GetHitCounterModule(), value);
            UpdateHitCounterInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetHitCounterVisible(bool value)
        {
            hitCounterVisible = value;
            if (hitCounterRoot != null)
            {
                hitCounterRoot.SetActive(value);
            }

            if (value)
            {
                RefreshHitCounterUi();
            }
            else
            {
                CancelHitCounterRebind();
                hitCounterResetArmedUntil = -1f;
            }

            UpdateUiState();
        }

        private void RefreshHitCounterUi()
        {
            bool enabled = GetHitCounterEnabled();
            UpdateToggleValue(hitCounterToggleValue, enabled);
            UpdateToggleIcon(hitCounterToggleIcon, enabled);
            UpdateToggleValue(hitCounterHallOfGodsValue, HitCounter.ShowInHallOfGods);
            UpdateKeybindValue(hitCounterKeyValue, GetHitCounterKeyLabel());

            hitCounterSliderSync = true;
            SyncHitCounterSlider(hitCounterSplitsSlider, hitCounterSplitsValue, HitCounter.VisibleSplitsClamped);
            SyncHitCounterSlider(hitCounterOpacitySlider, hitCounterOpacityValue, HitCounter.OpacityPercent);
            SyncHitCounterSlider(hitCounterScaleSlider, hitCounterScaleValue, HitCounter.ScalePercent);
            SyncHitCounterSlider(hitCounterPosXSlider, hitCounterPosXValue, HitCounter.PositionXPercent);
            SyncHitCounterSlider(hitCounterPosYSlider, hitCounterPosYValue, HitCounter.PositionYPercent);
            hitCounterSliderSync = false;

            UpdateHitCounterInteractivity();
        }

        private void UpdateHitCounterInteractivity()
        {
            SetContentInteractivity(hitCounterContent, GetHitCounterEnabled(), "HitCounterEnableRow");
        }

        private void OnQuickHitCounterClicked()
        {
            OpenPanelFromQuick(SetHitCounterVisible);
        }

        private void OnHitCounterBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetHitCounterVisible(false);
        }

        private void OnHitCounterResetDefaultsClicked()
        {
            waitingForHitCounterRebind = false;
            hitCounterPrevKey = string.Empty;
            hitCounterResetArmedUntil = -1f;
            SetHitCounterEnabled(false);
            HitCounter.ResetSettings();
            RefreshHitCounterUi();
            UpdateQuickMenuEntryStateColors();
        }

        private void OnHitCounterResetPbsClicked()
        {
            if (hitCounterResetArmedUntil < 0f || Time.unscaledTime > hitCounterResetArmedUntil)
            {
                hitCounterResetArmedUntil = Time.unscaledTime + HitCounterResetConfirmSeconds;
                ShowStatusMessage("HitCounter/ResetPbsConfirm".Localize());
                return;
            }

            hitCounterResetArmedUntil = -1f;
            HitCounter.ResetRecordsForCurrentSave();
            ShowStatusMessage("HitCounter/ResetPbsDone".Localize());
        }

        private void OnHitCounterSplitsChanged(float value)
        {
            HitCounter.SetVisibleSplits(value);
            SyncHitCounterSlider(hitCounterSplitsSlider, hitCounterSplitsValue, HitCounter.VisibleSplitsClamped);
            PreviewHitCounter();
        }

        private void OnHitCounterOpacityChanged(float value)
        {
            HitCounter.SetOpacityPercent(value);
            SyncHitCounterSlider(hitCounterOpacitySlider, hitCounterOpacityValue, HitCounter.OpacityPercent);
            PreviewHitCounter();
        }

        private void OnHitCounterScaleChanged(float value)
        {
            HitCounter.SetScalePercent(value);
            SyncHitCounterSlider(hitCounterScaleSlider, hitCounterScaleValue, HitCounter.ScalePercent);
            PreviewHitCounter();
        }

        private void OnHitCounterPosXChanged(float value)
        {
            HitCounter.SetPositionXPercent(value);
            SyncHitCounterSlider(hitCounterPosXSlider, hitCounterPosXValue, HitCounter.PositionXPercent);
            PreviewHitCounter();
        }

        private void OnHitCounterPosYChanged(float value)
        {
            HitCounter.SetPositionYPercent(value);
            SyncHitCounterSlider(hitCounterPosYSlider, hitCounterPosYValue, HitCounter.PositionYPercent);
            PreviewHitCounter();
        }

        private void StartHitCounterMove()
        {
            if (!GetHitCounterEnabled() || hitCounterMoveMode)
            {
                return;
            }

            hitCounterMoveMode = true;
            HitCounter.SetMoveMode(true);
            SetHitCounterVisible(false);
            ShowStatusMessage("HitCounter/MoveHint".Localize());
        }

        private void StopHitCounterMove(bool reopenPanel)
        {
            if (!hitCounterMoveMode)
            {
                return;
            }

            hitCounterMoveMode = false;
            HitCounter.SetMoveMode(false);
            GodhomeQoL.SaveGlobalSettingsSafe();
            if (reopenPanel)
            {
                SetHitCounterVisible(true);
            }
            else
            {
                UpdateUiState();
            }
        }

        private void HandleHitCounterMoveInput()
        {
            if (!hitCounterMoveMode)
            {
                return;
            }

            GameManager? manager = GameManager.instance;
            if (manager == null || manager.isPaused || manager.IsInSceneTransition || !HitCounter.IsActive)
            {
                StopHitCounterMove(false);
                return;
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                StopHitCounterMove(true);
            }
        }

        private void PreviewHitCounter()
        {
            if (!hitCounterSliderSync)
            {
                HitCounter.ShowPreview();
            }
        }

        private void SyncHitCounterSlider(Slider? slider, Text? valueText, int value)
        {
            UpdateFloatValueText(valueText, value, 0);
            if (slider == null || Math.Abs(slider.value - value) <= 0.0001f)
            {
                return;
            }

            bool previous = hitCounterSliderSync;
            hitCounterSliderSync = true;
            slider.value = value;
            hitCounterSliderSync = previous;
        }

        private string GetHitCounterKeyLabel()
        {
            return FormatMaskDamageKeyLabel(HitCounter.ToggleKeybind);
        }

        private bool IsHitCounterRebinding()
        {
            return waitingForHitCounterRebind;
        }

        private void StartHitCounterRebind()
        {
            waitingForHitCounterRebind = true;
            hitCounterPrevKey = HitCounter.ToggleKeybind ?? string.Empty;
            UpdateKeybindValue(hitCounterKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void CancelHitCounterRebind()
        {
            if (!waitingForHitCounterRebind)
            {
                return;
            }

            waitingForHitCounterRebind = false;
            UpdateKeybindValue(hitCounterKeyValue, GetHitCounterKeyLabel());
        }

        private void HandleHitCounterRebind()
        {
            if (!waitingForHitCounterRebind)
            {
                return;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key) || key == KeyCode.None || (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6))
                {
                    continue;
                }

                ApplyHitCounterRebind(key);
                return;
            }
        }

        private void ApplyHitCounterRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForHitCounterRebind = false;
                UpdateKeybindValue(hitCounterKeyValue, GetHitCounterKeyLabel());
                return;
            }

            string keyName = key.ToString();
            bool clear = !string.IsNullOrEmpty(hitCounterPrevKey)
                && string.Equals(hitCounterPrevKey, keyName, StringComparison.OrdinalIgnoreCase);

            if (!clear && TryGetHotkeyConflictOwnersExceptSelf(key, "HitCounter/ToggleKey".Localize(), out string conflictOwners))
            {
                ShowStatusMessage(string.Format("QuickMenu/HotkeyInUse".Localize(), FormatKeyLabel(key), conflictOwners));
                UpdateKeybindValue(hitCounterKeyValue, "Settings/FastReload/SetKey".Localize());
                return;
            }

            HitCounter.SetToggleKeybind(clear ? string.Empty : keyName);
            waitingForHitCounterRebind = false;
            UpdateKeybindValue(hitCounterKeyValue, GetHitCounterKeyLabel());
        }
    }
}
