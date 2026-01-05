using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed class QuickMenu : Module
{
    public override bool DefaultEnabled => true;
    public override bool Hidden => true;

    private GameObject? root;
    private static QuickMenuController? controller;
    private static Satchel.BetterMenus.MenuButton? quickMenuHotkeyButton;
    private const string DefaultQuickMenuHotkey = "F3";

    internal static Satchel.BetterMenus.MenuButton QuickMenuHotkeyButton() =>
        quickMenuHotkeyButton = new Satchel.BetterMenus.MenuButton(
            FormatQuickMenuButtonName("Settings/QuickMenu/Hotkey".Localize(), GetQuickMenuHotkeyLabel()),
            "Settings/QuickMenu/HotkeyDesc".Localize(),
            _ => StartQuickMenuHotkeyRebind(),
            false
        );

    internal static void StartQuickMenuHotkeyRebind()
    {
        controller?.StartQuickMenuHotkeyRebind();
    }

    internal static void ResetFreeMenuPositions()
    {
        GodhomeQoL.GlobalSettings.QuickMenuPositions = new Dictionary<string, QuickMenuEntryPosition>();
        GodhomeQoL.SaveGlobalSettingsSafe();
        controller?.ResetFreeMenuPositions();
    }

    internal static void ApplyInitialDefaults()
    {
        QuickMenuController.ApplyInitialDefaults();
    }

    private static void SetController(QuickMenuController? value)
    {
        controller = value;
    }

    private static void UpdateQuickMenuHotkeyButton(string value)
    {
        if (quickMenuHotkeyButton == null)
        {
            return;
        }

        quickMenuHotkeyButton.Name = FormatQuickMenuButtonName("Settings/QuickMenu/Hotkey".Localize(), value);
        quickMenuHotkeyButton.Update();
    }

    private static string GetQuickMenuHotkeyLabel() => FormatQuickMenuKeyLabel(GetQuickMenuToggleKey());

    private static string FormatQuickMenuButtonName(string title, string value) => $"{title}: {value}";

    private static string FormatQuickMenuKeyLabel(KeyCode key) => key == KeyCode.None
        ? "Settings/FastReload/NotSet".Localize()
        : key.ToString();

    private static KeyCode GetQuickMenuToggleKey()
    {
        string? raw = GodhomeQoL.GlobalSettings.QuickMenuHotkey;
        if (raw == null)
        {
            raw = DefaultQuickMenuHotkey;
        }

        if (raw.Length == 0)
        {
            return KeyCode.None;
        }

        return Enum.TryParse(raw, true, out KeyCode key)
            ? key
            : KeyCode.F3;
    }

    private protected override void Load()
    {
        if (root != null)
        {
            return;
        }

        root = new GameObject("GodhomeQoL_QuickMenu");
        UObject.DontDestroyOnLoad(root);
        root.AddComponent<QuickMenuController>();
        LogDebug("QuickMenu loaded");
    }

    private protected override void Unload()
    {
        if (root != null)
        {
            UObject.Destroy(root);
            root = null;
        }
    }

    private sealed class QuickMenuController : MonoBehaviour
    {
        private const int QuickCanvasSortOrder = 10000;
        private const int OverlayCanvasSortOrder = 10001;
        private const int CollectorCanvasSortOrder = 10002;
        private const int FastReloadCanvasSortOrder = 10003;
        private const int DreamshieldCanvasSortOrder = 10004;
        private const int ShowHpOnDeathCanvasSortOrder = 10005;
        private const int SpeedChangerCanvasSortOrder = 10006;
        private const int TeleportKitCanvasSortOrder = 10007;
        private const int BossChallengeCanvasSortOrder = 10008;
        private const int QolCanvasSortOrder = 10009;
        private const int MenuAnimationCanvasSortOrder = 10010;
        private const int BossAnimationCanvasSortOrder = 10011;
        private const int BindingsCanvasSortOrder = 10012;
        private const int ZoteHelperCanvasSortOrder = 10013;
        private const int QuickSettingsCanvasSortOrder = 10014;
        private const int StatusCanvasSortOrder = 10050;
        private const float QuickMenuFadeSeconds = 0.4f;
        private const float QuickPanelBackplateAlpha = 0f;
        private const float QuickPanelWidth = 300f;
        private const float QuickPanelHeight = 520f;
        private const float QuickPanelDefaultLeft = 20f;
        private const float QuickPanelDefaultTop = -20f;
        private const float QuickPanelFreeWidth = 1920f;
        private const float QuickPanelFreeHeight = 1080f;
        private const float QuickSettingsPanelHeight = 1100f;
        private const float QuickSettingsRowStartY = 320f;
        private static readonly Color OverlayDimColor = new Color(0f, 0f, 0f, 0.55f);
        private static readonly Color OverlayPanelColor = new Color(0f, 0f, 0f, 0.75f);
        private const float QuickButtonWidth = 260f;
        private const float QuickButtonHeight = 36f;
        private const float QuickHandleWidth = 28f;
        private const float QuickHandleGap = 6f;
        private const float QuickRowWidth = QuickButtonWidth + QuickHandleWidth + QuickHandleGap;
        private const float QuickButtonLeft = 6f;
        private const float QuickButtonTop = -12f;
        private const float QuickButtonSpacing = 3f;
        private const float QuickCollectorIconSize = 32f;
        private const float QuickCollectorIconPadding = 6f;
        private const float QuickZoteIconSize = 32f;
        private const float PanelWidth = 960f;
        private const float PanelHeight = 1100f;
        private const float ZoteHelperPanelHeight = PanelHeight;
        private const float MenuAnimationPanelHeight = PanelHeight;
        private const float MenuAnimationRowStartY = 200f;
        private const float BossAnimationPanelHeight = PanelHeight;
        private const float BossAnimationRowStartY = 260f;
        private const float SpeedChangerPanelHeight = PanelHeight;
        private const float SpeedChangerRowStartY = 200f;
        private const float SpeedChangerRowSpacing = 60f;
        private const float BossChallengePanelHeight = PanelHeight;
        private const float BossChallengeRowStartY = 320f;
        private const float BossChallengeRowSpacing = 60f;
        private const float RowWidth = 820f;
        private const float RowHeight = 44f;
        private const float RowStartY = 120f;
        private const float RowSpacing = 60f;
        private const float ButtonRowHeight = 48f;
        private const float FixedBackOffset = 110f;
        private const float FixedResetOffset = 170f;
        private const float ScrollTopPadding = 20f;
        private const float ScrollBottomPadding = 10f;
        private const float ScrollbarWidth = 8f;
        private const float ScrollbarRightPadding = 10f;
        private const float CollectorPanelWidth = PanelWidth;
        private const float CollectorPanelHeight = PanelHeight;
        private const float CollectorRowWidth = 920f;
        private const float CollectorRowHeight = 40f;
        private const float CollectorRowStartY = 320f;
        private const float CollectorRowSpacing = 44f;
        private const float CollectorLabelWidth = 700f;
        private const int CollectorTitleFontSize = 46;
        private const int CollectorLabelFontSize = 26;
        private const int CollectorValueFontSize = 26;
        private const float CollectorControlButtonWidth = 32f;
        private const float CollectorControlButtonHeight = 28f;
        private const float CollectorControlValueWidth = 80f;
        private const float CollectorControlGap = 8f;
        private const float CollectorControlPlusRight = -16f;
        private const float CollectorControlValueRight = CollectorControlPlusRight - CollectorControlButtonWidth - CollectorControlGap;
        private const float CollectorControlMinusRight = CollectorControlValueRight - CollectorControlValueWidth - CollectorControlGap;
        private const float InputControlButtonWidth = 32f;
        private const float InputControlButtonHeight = 28f;
        private const float InputControlValueWidth = 80f;
        private const float InputControlGap = 8f;
        private const float InputControlPlusRight = -16f;
        private const float InputControlValueRight = InputControlPlusRight - InputControlButtonWidth - InputControlGap;
        private const float InputControlMinusRight = InputControlValueRight - InputControlValueWidth - InputControlGap;
        private static readonly string[] SpeedChangerDisplayOptions = new[] { "#.##", "%", "Off" };
        private static readonly string[] BossChallengeGpzOptions = new[]
        {
            "Settings/gpzEnterType/Off".Localize(),
            "Settings/gpzEnterType/Long".Localize(),
            "Settings/gpzEnterType/Short".Localize()
        };
        private static Sprite? quickHandleSprite;
        private static Sprite? collectorIconLeftSprite;
        private static Sprite? collectorIconRightSprite;
        private static Sprite? zoteIconLeftSprite;
        private static Sprite? zoteIconRightSprite;

        private sealed class QuickMenuItemDefinition
        {
            public QuickMenuItemDefinition(string id, string label, Action onClick)
            {
                Id = id;
                Label = label;
                OnClick = onClick;
            }

            public string Id { get; }
            public string Label { get; }
            public Action OnClick { get; }
        }

        private sealed class QuickMenuEntry
        {
            public QuickMenuEntry(string id, RectTransform rect, Button button, Text label, string defaultLabel)
            {
                Id = id;
                Rect = rect;
                Button = button;
                Label = label;
                DefaultLabel = defaultLabel;
            }

            public string Id { get; }
            public RectTransform Rect { get; }
            public Button Button { get; }
            public Text Label { get; }
            public string DefaultLabel { get; }
        }

        private sealed class QuickMenuDragHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
        {
            public QuickMenuController? Owner { get; set; }
            public QuickMenuEntry? Entry { get; set; }

            public void OnBeginDrag(PointerEventData eventData)
            {
                if (eventData.button != PointerEventData.InputButton.Left)
                {
                    return;
                }

                Owner?.BeginQuickMenuDrag(Entry, eventData);
            }

            public void OnDrag(PointerEventData eventData)
            {
                if (eventData.button != PointerEventData.InputButton.Left)
                {
                    return;
                }

                Owner?.UpdateQuickMenuDrag(eventData);
            }

            public void OnEndDrag(PointerEventData eventData)
            {
                if (eventData.button != PointerEventData.InputButton.Left)
                {
                    return;
                }

                Owner?.EndQuickMenuDrag();
            }
        }

        private sealed class RowHighlight : MonoBehaviour
        {
            private Image? background;
            private Color baseColor;
            private Color highlightColor;
            private int hoverCount;
            private int selectCount;
            private bool manualActive;

            public void Initialize(Image backgroundImage, Color highlight)
            {
                background = backgroundImage;
                baseColor = backgroundImage.color;
                highlightColor = highlight;
                SetActive(false);
            }

            public void AdjustHover(int delta)
            {
                hoverCount = Mathf.Max(0, hoverCount + delta);
                UpdateActive();
            }

            public void AdjustSelected(int delta)
            {
                selectCount = Mathf.Max(0, selectCount + delta);
                UpdateActive();
            }

            public void SetManualActive(bool active)
            {
                manualActive = active;
                UpdateActive();
            }

            private void OnDisable()
            {
                hoverCount = 0;
                selectCount = 0;
                manualActive = false;
                SetActive(false);
            }

            private void UpdateActive()
            {
                SetActive(manualActive || hoverCount > 0 || selectCount > 0);
            }

            private void SetActive(bool active)
            {
                if (background != null)
                {
                    background.color = active ? highlightColor : baseColor;
                }
            }
        }

        private sealed class RowHighlightTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
        {
            public RowHighlight? Highlight { get; set; }

            public void OnPointerEnter(PointerEventData eventData)
            {
                Highlight?.AdjustHover(1);
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                Highlight?.AdjustHover(-1);
            }

            public void OnSelect(BaseEventData eventData)
            {
                Highlight?.AdjustSelected(1);
            }

            public void OnDeselect(BaseEventData eventData)
            {
                Highlight?.AdjustSelected(-1);
            }
        }

        private GameObject? quickRoot;
        private RectTransform? quickPanelRect;
        private RectTransform? quickPanelBackplateRect;
        private RectTransform? quickPanelContentRect;
        private Image? quickPanelImage;
        private CanvasGroup? quickPanelGroup;
        private Coroutine? quickMenuFadeCoroutine;
        private float quickMenuFadeAlpha = 1f;
        private float quickMenuOpacityAlpha = 1f;
        private GameObject? quickSettingsRoot;
        private GameObject? overlayRoot;
        private GameObject? collectorRoot;
        private GameObject? fastReloadRoot;
        private GameObject? dreamshieldRoot;
        private GameObject? showHpOnDeathRoot;
        private GameObject? speedChangerRoot;
        private GameObject? teleportKitRoot;
        private GameObject? bossChallengeRoot;
        private GameObject? qolRoot;
        private GameObject? menuAnimationRoot;
        private GameObject? bossAnimationRoot;
        private GameObject? bindingsRoot;
        private GameObject? zoteHelperRoot;
        private GameObject? statusRoot;
        private bool quickVisible;
        private bool quickSettingsVisible;
        private bool overlayVisible;
        private bool collectorVisible;
        private bool fastReloadVisible;
        private bool dreamshieldVisible;
        private bool showHpOnDeathVisible;
        private bool speedChangerVisible;
        private bool teleportKitVisible;
        private bool bossChallengeVisible;
        private bool qolVisible;
        private bool menuAnimationVisible;
        private bool bossAnimationVisible;
        private bool bindingsVisible;
        private bool zoteHelperVisible;
        private readonly List<QuickMenuEntry> quickEntries = new();
        private readonly Dictionary<string, Text> quickSettingsToggleValues = new();
        private Text? quickMenuOpacityValue;
        private Slider? quickMenuOpacitySlider;
        private QuickMenuEntry? draggingQuickEntry;
        private Vector2 draggingQuickOffset;
        private int draggingQuickIndex = -1;
        private bool quickRenameMode;
        private QuickMenuEntry? renameEntry;
        private InputField? renameField;
        private string? renameOriginalLabel;
        private bool renameCancelled;
        private bool uiActive;
        private bool returnToQuickOnClose;
        private bool returnToQolOnClose;
        private bool prevCursorVisible;
        private CursorLockMode prevCursorLock;
        private bool prevAllowMouseInput;
        private bool prevForceModuleActive;
        private bool cursorHookActive;
        private RowHighlight? manualHighlight;
        private PointerEventData? hoverEventData;
        private EventSystem? hoverEventSystem;
        private readonly List<RaycastResult> hoverResults = new();
        private Text? statusText;
        private float statusHideTime;
        private bool waitingForReloadRebind;
        private bool waitingForTeleportRebind;
        private bool waitingForShowHpRebind;
        private bool waitingForSpeedToggleRebind;
        private bool waitingForSpeedInputRebind;
        private bool waitingForSpeedUpRebind;
        private bool waitingForSpeedDownRebind;
        private bool waitingForTeleportKitMenuRebind;
        private bool waitingForTeleportKitSaveRebind;
        private bool waitingForTeleportKitTeleportRebind;
        private bool waitingForFastDreamWarpRebind;
        private bool waitingForQuickMenuHotkeyRebind;
        private bool waitingForBindingsMenuRebind;
        private KeyCode reloadPrevKey;
        private KeyCode teleportPrevKey;
        private KeyCode quickMenuPrevKey;
        private KeyCode bindingsPrevKey;
        private string showHpPrevBindingRaw = string.Empty;
        private string speedTogglePrevKey = string.Empty;
        private string speedInputPrevKey = string.Empty;
        private string speedUpPrevKey = string.Empty;
        private string speedDownPrevKey = string.Empty;
        private string fastDreamWarpPrevKeyRaw = string.Empty;
        private InputControlType fastDreamWarpPrevButton;

        private Text? moduleToggleValue;
        private Text? instantToggleValue;
        private Text? everywhereToggleValue;
        private Text? speedValueText;
        private Slider? speedSlider;
        private Module? fastSuperDashModule;
        private Text? collectorModuleToggleValue;
        private Text? collectorHoGOnlyValue;
        private Text? collectorPhaseValue;
        private Text? collectorImmortalValue;
        private Text? ignoreInitialJarLimitValue;
        private Text? useCustomPhase2ThresholdValue;
        private InputField? customPhase2ThresholdField;
        private Text? useMaxHpValue;
        private InputField? collectorMaxHpField;
        private InputField? buzzerHpField;
        private Text? spawnBuzzerValue;
        private InputField? rollerHpField;
        private Text? spawnRollerValue;
        private InputField? spitterHpField;
        private Text? spawnSpitterValue;
        private Text? disableSummonLimitValue;
        private InputField? customSummonLimitField;
        private Module? collectorPhasesModule;
        private Text? fastReloadToggleValue;
        private Text? reloadKeyValue;
        private Text? teleportKeyValue;
        private Module? fastReloadModule;
        private Text? dreamshieldToggleValue;
        private Text? dreamshieldDelayValue;
        private Text? dreamshieldSpeedValue;
        private Slider? dreamshieldDelaySlider;
        private Slider? dreamshieldSpeedSlider;
        private Text? showHpGlobalValue;
        private Text? showHpShowPbValue;
        private Text? showHpAutoHideValue;
        private Text? showHpHudToggleKeyValue;
        private Text? fastDreamWarpKeyValue;
        private Text? bindingsHotkeyValue;
        private Text? bindingsEverywhereValue;
        private Text? showHpFadeValue;
        private Slider? showHpFadeSlider;
        private Text? speedChangerGlobalValue;
        private Text? speedChangerLockValue;
        private Text? speedChangerRestrictValue;
        private Text? speedChangerUnlimitedValue;
        private Text? speedChangerDisplayValue;
        private Text? speedChangerToggleKeyValue;
        private Text? speedChangerInputKeyValue;
        private Text? speedChangerSpeedDownKeyValue;
        private Text? speedChangerSpeedUpKeyValue;
        private Module? speedChangerModule;
        private Text? teleportKitToggleValue;
        private Text? teleportKitMenuKeyValue;
        private Text? teleportKitSaveKeyValue;
        private Text? teleportKitTeleportKeyValue;
        private Module? teleportKitModule;
        private Text? bossInfiniteChallengeValue;
        private Text? bossRestartOnSuccessValue;
        private Text? bossRestartAndMusicValue;
        private Text? bossInfiniteGrimmValue;
        private Text? bossInfiniteRadianceValue;
        private Text? bossP5HealthValue;
        private Text? bossSegmentedP5Value;
        private Text? bossHalveAscendedValue;
        private Text? bossHalveAttunedValue;
        private Text? bossAddLifebloodValue;
        private InputField? bossLifebloodAmountField;
        private Text? bossAddSoulValue;
        private InputField? bossSoulAmountField;
        private Text? bossGpzValue;
        private Module? infiniteChallengeModule;
        private Module? infiniteGrimmModule;
        private Module? infiniteRadianceModule;
        private Module? p5HealthModule;
        private Module? segmentedP5Module;
        private Module? halveAscendedModule;
        private Module? halveAttunedModule;
        private Module? addLifebloodModule;
        private Module? addSoulModule;
        private Module? forceGreyPrinceModule;
        private Text? qolCarefreeMelodyValue;
        private Text? qolCollectorRoarValue;
        private Text? qolUnlockAllModesValue;
        private Text? qolUnlockPantheonsValue;
        private Text? qolUnlockRadianceValue;
        private Text? qolUnlockRadiantValue;
        private Text? menuAnimDoorDefaultValue;
        private Text? menuAnimMemorizeBindingsValue;
        private Text? menuAnimFasterLoadsValue;
        private Text? menuAnimFastMenusValue;
        private Text? menuAnimFastTextValue;
        private Text? menuAnimAutoSkipValue;
        private Text? menuAnimAllowSkippingValue;
        private Text? menuAnimSkipWithoutPromptValue;
        private Text? bossAnimFastDreamWarpValue;
        private Text? bossAnimShortDeathValue;
        private Text? bossAnimHallOfGodsValue;
        private Text? bossAnimAbsoluteRadianceValue;
        private Text? bossAnimPureVesselValue;
        private Text? bossAnimGrimmNightmareValue;
        private Text? bossAnimGreyPrinceValue;
        private Text? bossAnimCollectorValue;
        private Text? bossAnimSoulMasterValue;
        private Text? bossAnimFirstTimeValue;
        private Text? zoteHelperToggleValue;
        private Text? zoteImmortalValue;
        private Text? zoteSpawnFlyingValue;
        private Text? zoteSpawnHoppingValue;
        private InputField? zoteBossHpField;
        private InputField? zoteSummonFlyingHpField;
        private InputField? zoteSummonHoppingHpField;
        private InputField? zoteSummonLimitField;
        private Module? carefreeMelodyModule;
        private Module? collectorRoarModule;
        private Module? unlockAllModesModule;
        private Module? unlockPantheonsModule;
        private Module? unlockRadianceModule;
        private Module? unlockRadiantModule;
        private Module? doorDefaultBeginModule;
        private Module? memorizeBindingsModule;
        private Module? fasterLoadsModule;
        private Module? fastMenusModule;
        private Module? fastTextModule;
        private Module? fastDreamWarpModule;
        private Module? shortDeathAnimationModule;
        private Module? zoteHelperModule;

        private void Awake()
        {
            SetController(this);
            BuildUi();
            SetQuickVisible(false, true);
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetBindingsVisible(false);
            SetZoteHelperVisible(false);
            SetQuickSettingsVisible(false);
        }

        private void OnDestroy()
        {
            DisableCursorHook();
            if (uiActive)
            {
                RestoreUiInteraction();
                uiActive = false;
            }

            if (controller == this)
            {
                SetController(null);
            }

            DestroyRoot(ref quickRoot);
            DestroyRoot(ref overlayRoot);
            DestroyRoot(ref collectorRoot);
            DestroyRoot(ref fastReloadRoot);
            DestroyRoot(ref dreamshieldRoot);
            DestroyRoot(ref showHpOnDeathRoot);
            DestroyRoot(ref speedChangerRoot);
            DestroyRoot(ref teleportKitRoot);
            DestroyRoot(ref bossChallengeRoot);
            DestroyRoot(ref qolRoot);
            DestroyRoot(ref menuAnimationRoot);
            DestroyRoot(ref bossAnimationRoot);
            DestroyRoot(ref bindingsRoot);
            DestroyRoot(ref zoteHelperRoot);
            DestroyRoot(ref quickSettingsRoot);
            DestroyRoot(ref statusRoot);
        }

        private void Update()
        {
            bool wasRebinding = IsAnyRebinding();
            if (fastReloadVisible)
            {
                HandleFastReloadRebind();
            }

            if (showHpOnDeathVisible)
            {
                HandleShowHpOnDeathRebind();
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

            if (bindingsVisible)
            {
                HandleBindingsMenuRebind();
            }

            if (quickVisible)
            {
                HandleQuickMenuRename();
            }

            if (!IsAnyRebinding())
            {
                HandleFastDreamWarpHotkey();
            }

            KeyCode toggleKey = GetQuickMenuToggleKey();
            if (!wasRebinding && toggleKey != KeyCode.None && Input.GetKeyDown(toggleKey))
            {
                ToggleMenu();
            }

            if (quickVisible || quickSettingsVisible || overlayVisible || collectorVisible || fastReloadVisible || dreamshieldVisible || showHpOnDeathVisible || speedChangerVisible || teleportKitVisible || bossChallengeVisible || qolVisible || menuAnimationVisible || bossAnimationVisible || bindingsVisible || zoteHelperVisible)
            {
                MaintainUiInteraction();
            }

            UpdateStatusMessage();
        }

        private void LateUpdate()
        {
            if (quickVisible || quickSettingsVisible || overlayVisible || collectorVisible || fastReloadVisible || dreamshieldVisible || showHpOnDeathVisible || speedChangerVisible || teleportKitVisible || bossChallengeVisible || qolVisible || menuAnimationVisible || bossAnimationVisible || bindingsVisible || zoteHelperVisible)
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
                || qolVisible
                || menuAnimationVisible
                || bossAnimationVisible
                || bindingsVisible
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

        private void BuildUi()
        {
            BuildStatusUi();
            BuildQuickUi();
            BuildQuickSettingsOverlayUi();
            BuildOverlayUi();
            BuildCollectorOverlayUi();
            BuildFastReloadOverlayUi();
            BuildDreamshieldOverlayUi();
            BuildShowHpOnDeathOverlayUi();
            BuildSpeedChangerOverlayUi();
            BuildTeleportKitOverlayUi();
            BuildBossChallengeOverlayUi();
            BuildQolOverlayUi();
            BuildMenuAnimationOverlayUi();
            BuildBossAnimationOverlayUi();
            BuildZoteHelperOverlayUi();
            BuildBindingsOverlayUi();
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
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, backY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);

            quickSettingsToggleValues.Clear();

            float rowY = GetRowStartY(panelHeight, QuickSettingsRowStartY, topOffset);
            float lastY = rowY;
            foreach (QuickMenuItemDefinition def in GetOrderedQuickMenuDefinitions())
            {
                if (string.Equals(def.Id, "Settings", StringComparison.Ordinal))
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
            CreateButtonRow(panel.transform, "QuickMenuSettingsBackRow", "Back", backY, OnQuickMenuSettingsBackClicked);
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

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRow(
                content,
                "ModuleToggleRow",
                "Modules/FastSuperDash".Localize(),
                rowY,
                GetModuleEnabled,
                SetModuleEnabled,
                out moduleToggleValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "InstantToggleRow",
                "Settings/instantSuperDash".Localize(),
                rowY,
                () => Modules.QoL.FastSuperDash.instantSuperDash,
                value => Modules.QoL.FastSuperDash.instantSuperDash = value,
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
                value => Modules.QoL.FastSuperDash.fastSuperDashEverywhere = value,
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

            float rowY = GetRowStartY(panelHeight, CollectorRowStartY, topOffset);
            float lastY = rowY;
            CreateCollectorToggleRow(
                content,
                "CollectorModuleToggleRow",
                "Modules/CollectorPhases".Localize(),
                rowY,
                GetCollectorPhasesEnabled,
                SetCollectorPhasesEnabled,
                out collectorModuleToggleValue
            );

            lastY = rowY;
            rowY += CollectorRowSpacing;
            CreateCollectorToggleRow(
                content,
                "CollectorHoGOnlyRow",
                "Settings/CollectorPhases/HoGOnly".Localize(),
                rowY,
                () => Modules.CollectorPhases.CollectorPhases.HoGOnly,
                value => Modules.CollectorPhases.CollectorPhases.HoGOnly = value,
                out collectorHoGOnlyValue
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
                value => Modules.CollectorPhases.CollectorPhases.CollectorImmortal = value,
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
                value => Modules.CollectorPhases.CollectorPhases.IgnoreInitialJarLimit = value,
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
                value => Modules.CollectorPhases.CollectorPhases.UseCustomPhase2Threshold = value,
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
                value => Modules.CollectorPhases.CollectorPhases.CustomPhase2Threshold = Mathf.Clamp(value, 1, 99999),
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
                value => Modules.CollectorPhases.CollectorPhases.UseMaxHP = value,
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
                value => Modules.CollectorPhases.CollectorPhases.collectorMaxHP = Math.Max(value, 100),
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
                value => Modules.CollectorPhases.CollectorPhases.buzzerHP = Math.Max(value, 1),
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
                value => Modules.CollectorPhases.CollectorPhases.spawnBuzzer = value,
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
                value => Modules.CollectorPhases.CollectorPhases.rollerHP = Math.Max(value, 1),
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
                value => Modules.CollectorPhases.CollectorPhases.spawnRoller = value,
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
                value => Modules.CollectorPhases.CollectorPhases.spitterHP = Math.Max(value, 1),
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
                value => Modules.CollectorPhases.CollectorPhases.spawnSpitter = value,
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
                value => Modules.CollectorPhases.CollectorPhases.DisableSummonLimit = value,
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
                value => Modules.CollectorPhases.CollectorPhases.CustomSummonLimit = Mathf.Clamp(value, 2, 999),
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

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRow(
                content,
                "FastReloadToggleRow",
                "Modules/FastReload".Localize(),
                rowY,
                GetFastReloadEnabled,
                SetFastReloadEnabled,
                out fastReloadToggleValue
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
            CreateKeybindRow(
                content,
                "TeleportKeyRow",
                "Settings/teleportHoGKey".Localize(),
                rowY,
                () => FormatKeyLabel(GetTeleportKey()),
                StartTeleportRebind,
                out teleportKeyValue
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

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRow(
                content,
                "DreamshieldToggleRow",
                "Modules/DreamshieldStartAngle".Localize(),
                rowY,
                GetDreamshieldEnabled,
                SetDreamshieldEnabled,
                out dreamshieldToggleValue
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

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRow(
                content,
                "ShowHPOnDeathGlobalRow",
                "ShowHPOnDeath/GlobalSwitch".Localize(),
                rowY,
                () => ShowHpSettings.EnabledMod,
                value => ShowHpSettings.EnabledMod = value,
                out showHpGlobalValue
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

            string speedDownLabel = string.Format("SpeedChanger/SpeedDown".Localize(), SpeedChanger.step);
            string speedUpLabel = string.Format("SpeedChanger/SpeedUp".Localize(), SpeedChanger.step);

            float panelHeight = SpeedChangerPanelHeight;
            float resetY = GetFixedResetY(panelHeight);
            float backY = GetFixedBackY(panelHeight);
            float topOffset = GetScrollTopOffset(panelHeight, titleRect);
            float bottomOffset = GetScrollBottomOffset(panelHeight, resetY);
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);
            RectTransform content = CreateScrollContent(panel.transform, PanelWidth, panelHeight, topOffset, bottomOffset);

            float rowY = GetRowStartY(panelHeight, SpeedChangerRowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRow(
                content,
                "SpeedChangerGlobalRow",
                "SpeedChanger/GlobalSwitch".Localize(),
                rowY,
                () => SpeedChanger.globalSwitch,
                SetSpeedChangerGlobalSwitch,
                out speedChangerGlobalValue
            );

            lastY = rowY;
            rowY += SpeedChangerRowSpacing;
            CreateToggleRow(
                content,
                "SpeedChangerLockRow",
                "SpeedChanger/LockSwitch".Localize(),
                rowY,
                () => SpeedChanger.lockSwitch,
                value => SpeedChanger.lockSwitch = value,
                out speedChangerLockValue
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
            rowY += SpeedChangerRowSpacing;
            CreateKeybindRow(
                content,
                "SpeedChangerSpeedDownRow",
                speedDownLabel,
                rowY,
                () => FormatSpeedChangerKeyLabel(SpeedChanger.slowDownKeybind),
                StartSpeedChangerSpeedDownRebind,
                out speedChangerSpeedDownKeyValue
            );

            lastY = rowY;
            rowY += SpeedChangerRowSpacing;
            CreateKeybindRow(
                content,
                "SpeedChangerSpeedUpRow",
                speedUpLabel,
                rowY,
                () => FormatSpeedChangerKeyLabel(SpeedChanger.speedUpKeybind),
                StartSpeedChangerSpeedUpRebind,
                out speedChangerSpeedUpKeyValue
            );

            lastY = rowY;
            rowY += SpeedChangerRowSpacing;

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

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRow(
                content,
                "TeleportKitToggleRow",
                "Modules/TeleportKit".Localize(),
                rowY,
                GetTeleportKitEnabled,
                SetTeleportKitEnabled,
                out teleportKitToggleValue
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

            float rowY = GetRowStartY(panelHeight, BossChallengeRowStartY, topOffset);
            float lastY = rowY;
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
            CreateToggleRow(
                content,
                "QolCarefreeMelodyRow",
                "Modules/CarefreeMelodyReset".Localize(),
                rowY,
                GetCarefreeMelodyEnabled,
                SetCarefreeMelodyEnabled,
                out qolCarefreeMelodyValue
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
                "BossP5HealthRow",
                "Modules/P5Health".Localize(),
                rowY,
                GetP5HealthEnabled,
                SetP5HealthEnabled,
                out bossP5HealthValue
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
                "BossHalveAscendedRow",
                "Modules/HalveDamageHoGAscendedOrAbove".Localize(),
                rowY,
                GetHalveAscendedEnabled,
                SetHalveAscendedEnabled,
                out bossHalveAscendedValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossHalveAttunedRow",
                "Modules/HalveDamageHoGAttuned".Localize(),
                rowY,
                GetHalveAttunedEnabled,
                SetHalveAttunedEnabled,
                out bossHalveAttunedValue
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
                1,
                out bossSoulAmountField
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "BossChallengeResetRow", "BossChallenge/Reset".Localize(), resetY, OnBossChallengeResetDefaultsClicked);
            CreateButtonRow(panel.transform, "BossChallengeBackRow", "Back", backY, OnBossChallengeBackClicked);
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

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
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
            rowY += RowSpacing;
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
            rowY += RowSpacing;
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
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "QolHallOfGodsRow",
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
                "QolUnlockAllModesRow",
                "Modules/UnlockAllModes".Localize(),
                rowY,
                GetUnlockAllModesEnabled,
                SetUnlockAllModesEnabled,
                out qolUnlockAllModesValue
            );

            lastY = rowY;
            rowY += RowSpacing;
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
            rowY += RowSpacing;
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
            rowY += RowSpacing;
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
            rowY += RowSpacing;

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

            float rowY = GetRowStartY(panelHeight, MenuAnimationRowStartY, topOffset);
            float lastY = rowY;
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

            float rowY = GetRowStartY(panelHeight, BossAnimationRowStartY, topOffset);
            float lastY = rowY;
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
            CreateToggleRow(
                content,
                "BossAnimFirstTimeRow",
                "Settings/FirstTimeBosses".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.FirstTimeBosses,
                value => Modules.QoL.SkipCutscenes.FirstTimeBosses = value,
                out bossAnimFirstTimeValue
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

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRow(
                content,
                "ZoteHelperEnableRow",
                "Settings/ZoteHelper/Enable".Localize(),
                rowY,
                GetZoteHelperEnabled,
                SetZoteHelperEnabled,
                out zoteHelperToggleValue
            );

            lastY = rowY;
            rowY += RowSpacing;
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
            rowY += RowSpacing;
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
            rowY += RowSpacing;
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
            rowY += RowSpacing;
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
            rowY += RowSpacing;
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
            rowY += RowSpacing;
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
            rowY += RowSpacing;
            CreateAdjustInputRow(
                content,
                "ZoteSummonLimitRow",
                "Settings/ZoteHelper/ZoteSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteSummonLimit,
                value => Modules.BossChallenge.ZoteHelper.zoteSummonLimit = Mathf.Clamp(value, 0, 99),
                0,
                99,
                1,
                out zoteSummonLimitField
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateCycleRow(
                content,
                "ZoteHelperGpzRow",
                "Modules/ForceGreyPrinceEnterType".Localize(),
                rowY,
                BossChallengeGpzOptions,
                GetBossGpzIndex,
                ApplyBossGpzOption,
                out bossGpzValue
            );

            lastY = rowY;
            rowY += RowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "ZoteHelperResetRow", "Settings/ZoteHelper/Reset".Localize(), resetY, OnZoteHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "ZoteHelperBackRow", "Back", backY, OnZoteHelperBackClicked);
        }

        private void BuildBindingsOverlayUi()
        {
            bindingsRoot = new GameObject("BindingsOverlayCanvas");
            bindingsRoot.transform.SetParent(transform, false);

            Canvas canvas = bindingsRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = BindingsCanvasSortOrder;

            CanvasScaler scaler = bindingsRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            bindingsRoot.AddComponent<GraphicRaycaster>();

            CanvasGroup group = bindingsRoot.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.blocksRaycasts = true;

            GameObject dim = new GameObject("Dim");
            dim.transform.SetParent(bindingsRoot.transform, false);
            RectTransform dimRect = dim.AddComponent<RectTransform>();
            dimRect.anchorMin = Vector2.zero;
            dimRect.anchorMax = Vector2.one;
            dimRect.offsetMin = Vector2.zero;
            dimRect.offsetMax = Vector2.zero;

            Image dimImage = dim.AddComponent<Image>();
            dimImage.color = OverlayDimColor;

            GameObject panel = new GameObject("BindingsPanel");
            panel.transform.SetParent(dim.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(PanelWidth, PanelHeight);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = OverlayPanelColor;

            Text title = CreateText(panel.transform, "Title", "BindingsMenu".Localize(), 52, TextAnchor.MiddleCenter);
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

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRow(
                content,
                "BindingsEverywhereRow",
                "Settings/BindingsMenu/AllowEverywhere".Localize(),
                rowY,
                GetBindingsEverywhereEnabled,
                SetBindingsEverywhereEnabled,
                out bindingsEverywhereValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateKeybindRow(
                content,
                "BindingsHotkeyRow",
                "Settings/BindingsMenu/Hotkey".Localize(),
                rowY,
                GetBindingsMenuHotkeyLabel,
                StartBindingsMenuHotkeyRebind,
                out bindingsHotkeyValue
            );

            lastY = rowY;
            rowY += RowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "BindingsResetRow", "Settings/BindingsMenu/Reset".Localize(), resetY, OnBindingsResetDefaultsClicked);
            CreateButtonRow(panel.transform, "BindingsBackRow", "Back", backY, OnBindingsBackClicked);
        }

        private static Font GetMenuFont()
        {
            return Modding.Menu.MenuResources.Perpetua ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private static Sprite? LoadQuickHandleSprite()
        {
            try
            {
                Assembly asm = typeof(QuickMenu).Assembly;
                string? resourceName = asm
                    .GetManifestResourceNames()
                    .FirstOrDefault(name => name.EndsWith("Pin_Knight.png", StringComparison.OrdinalIgnoreCase));

                if (resourceName != null)
                {
                    using Stream? stream = asm.GetManifestResourceStream(resourceName);
                    if (stream != null)
                    {
                        return LoadSpriteFromStream(stream, "Pin_Knight");
                    }
                }

                string dir = Path.GetDirectoryName(asm.Location)
                    ?? AppDomain.CurrentDomain.BaseDirectory
                    ?? Environment.CurrentDirectory;
                string path = Path.Combine(dir, "Pin_Knight.png");

                if (File.Exists(path))
                {
                    using FileStream stream = File.OpenRead(path);
                    return LoadSpriteFromStream(stream, "Pin_Knight");
                }
            }
            catch (Exception e)
            {
                LogDebug($"QuickMenu: failed to load Pin_Knight sprite - {e.Message}");
            }

            return null;
        }

        private static Sprite? LoadCollectorIconSprite(string fileName, string name)
        {
            try
            {
                Assembly asm = typeof(QuickMenu).Assembly;
                string? resourceName = asm
                    .GetManifestResourceNames()
                    .FirstOrDefault(resource => resource.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

                if (resourceName != null)
                {
                    using Stream? stream = asm.GetManifestResourceStream(resourceName);
                    if (stream != null)
                    {
                        return LoadSpriteFromStream(stream, name);
                    }
                }

                string dir = Path.GetDirectoryName(asm.Location)
                    ?? AppDomain.CurrentDomain.BaseDirectory
                    ?? Environment.CurrentDirectory;
                string path = Path.Combine(dir, fileName);

                if (File.Exists(path))
                {
                    using FileStream stream = File.OpenRead(path);
                    return LoadSpriteFromStream(stream, name);
                }
            }
            catch (Exception e)
            {
                LogDebug($"QuickMenu: failed to load {fileName} - {e.Message}");
            }

            return null;
        }

        private static Sprite? LoadSpriteFromStream(Stream stream, string name)
        {
            using MemoryStream ms = new();
            stream.CopyTo(ms);
            byte[] data = ms.ToArray();

            Texture2D texture = new(2, 2, TextureFormat.ARGB32, false);
            if (!texture.LoadImage(data))
            {
                return null;
            }

            texture.name = name;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            texture.Apply();

            Rect rect = new(0f, 0f, texture.width, texture.height);
            return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f);
        }

        private static GameObject CreateQuickPanel(Transform parent)
        {
            GameObject panel = new GameObject("QuickMenuPanel");
            panel.transform.SetParent(parent, false);

            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(20f, -20f);
            rect.sizeDelta = new Vector2(QuickPanelWidth, QuickPanelHeight);

            return panel;
        }

        private static GameObject CreateQuickPanelBackplate(Transform parent)
        {
            GameObject backplate = new("QuickMenuBackplate");
            backplate.transform.SetParent(parent, false);

            RectTransform rect = backplate.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(QuickPanelWidth, QuickPanelHeight);

            Image image = backplate.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, QuickPanelBackplateAlpha);
            image.raycastTarget = false;

            return backplate;
        }

        private static RectTransform CreateQuickPanelContent(Transform parent)
        {
            GameObject content = new("QuickMenuContent");
            content.transform.SetParent(parent, false);

            RectTransform rect = content.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0f, 1f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;

            return rect;
        }

        private List<QuickMenuItemDefinition> GetQuickMenuDefinitions()
        {
            return new List<QuickMenuItemDefinition>
            {
                new("FastSuperDash", "Modules/FastSuperDash".Localize(), OnQuickFastSuperDashClicked),
                new("CollectorPhases", "Modules/CollectorPhases".Localize(), OnQuickCollectorPhasesClicked),
                new("FastReload", "Modules/FastReload".Localize(), OnQuickFastReloadClicked),
                new("DreamshieldSettings", "DreamshieldSettings".Localize(), OnQuickDreamshieldClicked),
                new("ShowHPOnDeath", "ShowHPOnDeath".Localize(), OnQuickShowHpOnDeathClicked),
                new("SpeedChanger", "SpeedChanger".Localize(), OnQuickSpeedChangerClicked),
                new("TeleportKit", "TeleportKit".Localize(), OnQuickTeleportKitClicked),
                new("BossChallenge", "Categories/BossChallenge".Localize(), OnQuickBossChallengeClicked),
                new("ZoteHelper", "Modules/ZoteHelper".Localize(), OnQuickZoteHelperClicked),
                new("QualityOfLife", "Categories/QoL".Localize(), OnQuickQolClicked),
                new("BindingsMenu", "BindingsMenu".Localize(), OnQuickBindingsMenuClicked),
                new("BossAnimationSkipping", "Categories/BossAnimationSkipping".Localize(), OnQuickBossAnimationClicked),
                new("MenuAnimationSkipping", "Categories/MenuAnimationSkipping".Localize(), OnQuickMenuAnimationClicked),
                new("FreeMenu", GetFreeMenuLabel(), OnQuickFreeMenuClicked),
                new("RenameMode", GetRenameModeLabel(), OnQuickRenameModeClicked),
                new("Settings", "QuickMenu/Settings".Localize(), OnQuickSettingsClicked)
            };
        }

        private List<QuickMenuItemDefinition> GetOrderedQuickMenuDefinitions()
        {
            List<QuickMenuItemDefinition> definitions = GetQuickMenuDefinitions();
            List<string> savedOrder = GodhomeQoL.GlobalSettings.QuickMenuOrder ?? new List<string>();
            List<QuickMenuItemDefinition> ordered = new();
            HashSet<string> used = new();
            Dictionary<string, QuickMenuItemDefinition> lookup = definitions.ToDictionary(def => def.Id);

            foreach (string id in savedOrder)
            {
                if (used.Add(id) && lookup.TryGetValue(id, out QuickMenuItemDefinition? def))
                {
                    ordered.Add(def);
                }
            }

            foreach (QuickMenuItemDefinition def in definitions)
            {
                if (used.Add(def.Id))
                {
                    ordered.Add(def);
                }
            }

            return ordered;
        }

        private List<QuickMenuItemDefinition> GetVisibleQuickMenuDefinitions()
        {
            List<QuickMenuItemDefinition> ordered = GetOrderedQuickMenuDefinitions();
            return ordered
                .Where(def => IsQuickMenuItemVisible(def.Id))
                .ToList();
        }

        private bool IsQuickMenuItemVisible(string id)
        {
            if (string.Equals(id, "Settings", StringComparison.Ordinal))
            {
                return true;
            }

            Dictionary<string, bool> visibility = GodhomeQoL.GlobalSettings.QuickMenuVisibility ??= new Dictionary<string, bool>();
            return !visibility.TryGetValue(id, out bool isVisible) || isVisible;
        }

        private void SetQuickMenuItemVisible(string id, bool value)
        {
            if (string.Equals(id, "Settings", StringComparison.Ordinal))
            {
                return;
            }

            Dictionary<string, bool> visibility = GodhomeQoL.GlobalSettings.QuickMenuVisibility ??= new Dictionary<string, bool>();
            visibility[id] = value;
            GodhomeQoL.SaveGlobalSettingsSafe();
        }

        private string GetQuickMenuSettingsLabel(QuickMenuItemDefinition def)
        {
            if (string.Equals(def.Id, "FreeMenu", StringComparison.Ordinal))
            {
                return "QuickMenu/FreeMenu".Localize();
            }

            if (string.Equals(def.Id, "RenameMode", StringComparison.Ordinal))
            {
                return "QuickMenu/RenameMode".Localize();
            }

            string defaultLabel = def.Label;
            if (TryGetQuickMenuCustomLabel(def.Id, out string custom)
                && !string.Equals(custom, defaultLabel, StringComparison.Ordinal))
            {
                return $"{custom} ({defaultLabel})";
            }

            return defaultLabel;
        }

        private string GetFreeMenuLabel()
        {
            string label = "QuickMenu/FreeMenu".Localize();
            string state = IsQuickMenuFreeLayoutEnabled() ? "ON" : "OFF";
            return $"{label}: {state}";
        }

        private void UpdateFreeMenuLabel()
        {
            QuickMenuEntry? entry = quickEntries.FirstOrDefault(item => item.Id == "FreeMenu");
            if (entry == null)
            {
                return;
            }

            entry.Label.text = GetFreeMenuLabel();
        }

        private string GetRenameModeLabel()
        {
            string label = "QuickMenu/RenameMode".Localize();
            string state = quickRenameMode ? "ON" : "OFF";
            return $"{label}: {state}";
        }

        private void UpdateRenameModeLabel()
        {
            QuickMenuEntry? entry = quickEntries.FirstOrDefault(item => item.Id == "RenameMode");
            if (entry == null)
            {
                return;
            }

            entry.Label.text = GetRenameModeLabel();
        }

        private static bool IsQuickMenuRenameAllowed(string id)
        {
            return id != "FreeMenu" && id != "RenameMode" && id != "Settings";
        }

        private static string GetQuickMenuDisplayLabel(string id, string defaultLabel)
        {
            if (!IsQuickMenuRenameAllowed(id))
            {
                return defaultLabel;
            }

            Dictionary<string, string> labels = GodhomeQoL.GlobalSettings.QuickMenuCustomLabels ??= new Dictionary<string, string>();
            if (labels.TryGetValue(id, out string? custom) && !string.IsNullOrWhiteSpace(custom))
            {
                return custom;
            }

            return defaultLabel;
        }

        private static bool TryGetQuickMenuCustomLabel(string id, out string customLabel)
        {
            customLabel = string.Empty;
            if (!IsQuickMenuRenameAllowed(id))
            {
                return false;
            }

            Dictionary<string, string> labels = GodhomeQoL.GlobalSettings.QuickMenuCustomLabels ??= new Dictionary<string, string>();
            if (labels.TryGetValue(id, out string? custom) && !string.IsNullOrWhiteSpace(custom))
            {
                customLabel = custom;
                return true;
            }

            return false;
        }

        private float GetQuickRowY(int index)
        {
            return QuickButtonTop - index * (QuickButtonHeight + QuickButtonSpacing);
        }

        private QuickMenuEntry CreateQuickEntry(Transform parent, QuickMenuItemDefinition definition, float y)
        {
            GameObject rowObj = new GameObject($"{definition.Id}Row");
            rowObj.transform.SetParent(parent, false);

            RectTransform rowRect = rowObj.AddComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0f, 1f);
            rowRect.anchorMax = new Vector2(0f, 1f);
            rowRect.pivot = new Vector2(0f, 1f);
            rowRect.anchoredPosition = new Vector2(QuickButtonLeft, y);
            rowRect.sizeDelta = new Vector2(QuickRowWidth, QuickButtonHeight);

            GameObject handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(rowObj.transform, false);

            RectTransform handleRect = handleObj.AddComponent<RectTransform>();
            handleRect.anchorMin = new Vector2(0f, 0.5f);
            handleRect.anchorMax = new Vector2(0f, 0.5f);
            handleRect.pivot = new Vector2(0f, 0.5f);
            handleRect.anchoredPosition = Vector2.zero;
            handleRect.sizeDelta = new Vector2(QuickHandleWidth, QuickButtonHeight);

            Image handleImage = handleObj.AddComponent<Image>();
            handleImage.color = new Color(1f, 1f, 1f, 0f);

            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(handleObj.transform, false);

            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 0.5f);
            iconRect.anchorMax = new Vector2(0.5f, 0.5f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.sizeDelta = new Vector2(QuickHandleWidth, QuickButtonHeight);

            Sprite? handleSprite = quickHandleSprite ??= LoadQuickHandleSprite();
            if (handleSprite != null)
            {
                Image iconImage = iconObj.AddComponent<Image>();
                iconImage.sprite = handleSprite;
                iconImage.preserveAspect = true;
                iconImage.color = Color.white;
                iconImage.raycastTarget = false;
            }
            else
            {
                Text handleText = CreateText(iconObj.transform, "Label", "|||", 18, TextAnchor.MiddleCenter);
                handleText.raycastTarget = false;
                RectTransform handleTextRect = handleText.rectTransform;
                handleTextRect.anchorMin = Vector2.zero;
                handleTextRect.anchorMax = Vector2.one;
                handleTextRect.offsetMin = Vector2.zero;
                handleTextRect.offsetMax = Vector2.zero;
            }

            GameObject buttonObj = new GameObject("Button");
            buttonObj.transform.SetParent(rowObj.transform, false);

            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0f, 0.5f);
            buttonRect.anchorMax = new Vector2(0f, 0.5f);
            buttonRect.pivot = new Vector2(0f, 0.5f);
            buttonRect.anchoredPosition = new Vector2(QuickHandleWidth + QuickHandleGap, 0f);
            buttonRect.sizeDelta = new Vector2(QuickButtonWidth, QuickButtonHeight);

            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0f, 0f, 0f, 0.85f);

            Outline outline = buttonObj.AddComponent<Outline>();
            outline.effectColor = Color.white;
            outline.effectDistance = new Vector2(1f, -1f);

            Button button = buttonObj.AddComponent<Button>();

            string displayLabel = GetQuickMenuDisplayLabel(definition.Id, definition.Label);
            Text text = CreateText(buttonObj.transform, "Text", displayLabel, 24, TextAnchor.MiddleCenter);
            RectTransform textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            QuickMenuEntry entry = new(definition.Id, rowRect, button, text, definition.Label);
            QuickMenuDragHandle dragHandle = handleObj.AddComponent<QuickMenuDragHandle>();
            dragHandle.Owner = this;
            dragHandle.Entry = entry;
            button.onClick.AddListener(() => OnQuickEntryClicked(definition, entry));

            return entry;
        }

        private static void TryAddCollectorIcons(Transform parent, Text text, string id)
        {
            if (!string.Equals(id, "CollectorPhases", StringComparison.Ordinal))
            {
                return;
            }

            collectorIconLeftSprite ??= LoadCollectorIconSprite("25px-The_Collector_Icon RIGHT.png", "CollectorIconLeft");
            collectorIconRightSprite ??= LoadCollectorIconSprite("25px-The_Collector_Icon LEFT.png", "CollectorIconRight");

            if (collectorIconLeftSprite == null && collectorIconRightSprite == null)
            {
                return;
            }

            float offset = CalculateQuickMenuIconOffset(text, QuickCollectorIconSize, QuickButtonWidth);

            if (collectorIconLeftSprite != null)
            {
                CreateQuickMenuIcon(parent, "CollectorIconLeft", collectorIconLeftSprite, -offset, QuickCollectorIconSize);
            }

            if (collectorIconRightSprite != null)
            {
                CreateQuickMenuIcon(parent, "CollectorIconRight", collectorIconRightSprite, offset, QuickCollectorIconSize);
            }
        }

        private static void TryAddZoteIcons(Transform parent, Text text, string id)
        {
            if (!string.Equals(id, "ZoteHelper", StringComparison.Ordinal))
            {
                return;
            }

            zoteIconLeftSprite ??= LoadCollectorIconSprite("32px-Grey_Prince_Zote_Circle LEFT.png", "ZoteIconLeft");
            zoteIconRightSprite ??= LoadCollectorIconSprite("32px-Grey_Prince_Zote_Circle RIGHT.png", "ZoteIconRight");

            if (zoteIconLeftSprite == null && zoteIconRightSprite == null)
            {
                return;
            }

            float offset = CalculateQuickMenuIconOffset(text, QuickZoteIconSize, QuickButtonWidth);

            if (zoteIconLeftSprite != null)
            {
                CreateQuickMenuIcon(parent, "ZoteIconLeft", zoteIconLeftSprite, -offset, QuickZoteIconSize);
            }

            if (zoteIconRightSprite != null)
            {
                CreateQuickMenuIcon(parent, "ZoteIconRight", zoteIconRightSprite, offset, QuickZoteIconSize);
            }
        }

        private static void TryAddZoteTitleIcons(Text title)
        {
            if (title == null)
            {
                return;
            }

            zoteIconLeftSprite ??= LoadCollectorIconSprite("32px-Grey_Prince_Zote_Circle LEFT.png", "ZoteTitleIconLeft");
            zoteIconRightSprite ??= LoadCollectorIconSprite("32px-Grey_Prince_Zote_Circle RIGHT.png", "ZoteTitleIconRight");

            if (zoteIconLeftSprite == null && zoteIconRightSprite == null)
            {
                return;
            }

            float offset = CalculateQuickMenuIconOffset(title, QuickZoteIconSize, RowWidth);

            if (zoteIconLeftSprite != null)
            {
                CreateQuickMenuIcon(title.transform, "ZoteTitleIconLeft", zoteIconLeftSprite, -offset, QuickZoteIconSize);
            }

            if (zoteIconRightSprite != null)
            {
                CreateQuickMenuIcon(title.transform, "ZoteTitleIconRight", zoteIconRightSprite, offset, QuickZoteIconSize);
            }
        }

        private static float CalculateQuickMenuIconOffset(Text text, float iconSize, float rowWidth)
        {
            float textWidth = text.preferredWidth;
            if (textWidth <= 1f)
            {
                textWidth = text.text.Length * text.fontSize * 0.5f;
            }

            float halfTextWidth = textWidth * 0.5f;
            float maxOffset = rowWidth * 0.5f - iconSize * 0.5f - QuickCollectorIconPadding;
            float desiredOffset = halfTextWidth + iconSize * 0.5f + QuickCollectorIconPadding;
            float offset = Mathf.Min(desiredOffset, maxOffset);
            if (offset < iconSize * 0.5f)
            {
                offset = iconSize * 0.5f;
            }

            return offset;
        }

        private static void UpdateQuickMenuIconOffsets(Transform parent, Text text, float iconSize, string leftName, string rightName, float rowWidth)
        {
            float offset = CalculateQuickMenuIconOffset(text, iconSize, rowWidth);

            Transform? left = parent.Find(leftName);
            if (left != null && left.TryGetComponent(out RectTransform leftRect))
            {
                leftRect.anchoredPosition = new Vector2(-offset, 0f);
            }

            Transform? right = parent.Find(rightName);
            if (right != null && right.TryGetComponent(out RectTransform rightRect))
            {
                rightRect.anchoredPosition = new Vector2(offset, 0f);
            }
        }

        private void UpdateQuickMenuEntryIcons(QuickMenuEntry entry)
        {
            if (entry.Id == "CollectorPhases")
            {
                UpdateQuickMenuIconOffsets(entry.Button.transform, entry.Label, QuickCollectorIconSize, "CollectorIconLeft", "CollectorIconRight", QuickButtonWidth);
            }
            else if (entry.Id == "ZoteHelper")
            {
                UpdateQuickMenuIconOffsets(entry.Button.transform, entry.Label, QuickZoteIconSize, "ZoteIconLeft", "ZoteIconRight", QuickButtonWidth);
            }
        }

        private static void CreateQuickMenuIcon(Transform parent, string name, Sprite sprite, float x, float size)
        {
            GameObject iconObj = new GameObject(name);
            iconObj.transform.SetParent(parent, false);

            RectTransform rect = iconObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(x, 0f);
            rect.sizeDelta = new Vector2(size, size);

            Image image = iconObj.AddComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.color = Color.white;
            image.raycastTarget = false;
        }

        private void OnQuickEntryClicked(QuickMenuItemDefinition definition, QuickMenuEntry entry)
        {
            if (renameField != null && (!quickRenameMode || !IsQuickMenuRenameAllowed(entry.Id)))
            {
                CommitQuickMenuRename(renameField.text);
            }

            if (quickRenameMode && IsQuickMenuRenameAllowed(entry.Id))
            {
                StartQuickMenuRename(entry);
                return;
            }

            definition.OnClick();
        }

        private void HandleQuickMenuRename()
        {
            if (renameField == null)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                renameCancelled = true;
                CancelQuickMenuRename();
            }
        }

        private void StartQuickMenuRename(QuickMenuEntry entry)
        {
            if (!IsQuickMenuRenameAllowed(entry.Id))
            {
                return;
            }

            if (renameField != null && renameEntry != null)
            {
                CommitQuickMenuRename(renameField.text);
            }

            renameCancelled = false;
            renameEntry = entry;
            renameOriginalLabel = entry.Label.text;

            InputField field = CreateQuickMenuRenameField(entry.Button.transform, entry.Label.text, entry.Label.fontSize);
            field.onEndEdit.AddListener(OnQuickMenuRenameEndEdit);
            renameField = field;

            entry.Label.gameObject.SetActive(false);
            field.ActivateInputField();
            field.Select();
            field.MoveTextEnd(false);
        }

        private void OnQuickMenuRenameEndEdit(string value)
        {
            if (renameCancelled)
            {
                renameCancelled = false;
                return;
            }

            CommitQuickMenuRename(value);
        }

        private void CommitQuickMenuRename(string value)
        {
            if (renameEntry == null)
            {
                return;
            }

            string trimmed = value.Trim();
            if (string.IsNullOrEmpty(trimmed) || string.Equals(trimmed, renameEntry.DefaultLabel, StringComparison.Ordinal))
            {
                SetQuickMenuCustomLabel(renameEntry.Id, null);
                renameEntry.Label.text = renameEntry.DefaultLabel;
            }
            else
            {
                SetQuickMenuCustomLabel(renameEntry.Id, trimmed);
                renameEntry.Label.text = trimmed;
            }

            renameEntry.Label.gameObject.SetActive(true);
            UpdateQuickMenuEntryIcons(renameEntry);

            if (renameField != null)
            {
                UObject.Destroy(renameField.gameObject);
            }

            renameField = null;
            renameEntry = null;
            renameOriginalLabel = null;
        }

        private void CancelQuickMenuRename()
        {
            if (renameEntry == null)
            {
                return;
            }

            if (renameOriginalLabel != null)
            {
                renameEntry.Label.text = renameOriginalLabel;
                UpdateQuickMenuEntryIcons(renameEntry);
            }

            renameEntry.Label.gameObject.SetActive(true);

            if (renameField != null)
            {
                UObject.Destroy(renameField.gameObject);
            }

            renameField = null;
            renameEntry = null;
            renameOriginalLabel = null;
        }

        private InputField CreateQuickMenuRenameField(Transform parent, string text, int fontSize)
        {
            GameObject fieldObj = new GameObject("RenameField");
            fieldObj.transform.SetParent(parent, false);

            RectTransform rect = fieldObj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = fieldObj.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.08f);

            InputField input = fieldObj.AddComponent<InputField>();
            input.lineType = InputField.LineType.SingleLine;
            input.contentType = InputField.ContentType.Standard;
            input.caretColor = Color.white;
            input.selectionColor = new Color(1f, 1f, 1f, 0.25f);
            input.targetGraphic = image;
            input.text = text;

            Text inputText = CreateText(fieldObj.transform, "Text", text, fontSize, TextAnchor.MiddleCenter);
            RectTransform textRect = inputText.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(6f, 0f);
            textRect.offsetMax = new Vector2(-6f, 0f);
            input.textComponent = inputText;

            return input;
        }

        private void SetQuickMenuCustomLabel(string id, string? value)
        {
            Dictionary<string, string> labels = GodhomeQoL.GlobalSettings.QuickMenuCustomLabels ??= new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(value))
            {
                labels.Remove(id);
            }
            else
            {
                labels[id] = value!;
            }

            GodhomeQoL.SaveGlobalSettingsSafe();
        }

        private void BeginQuickMenuDrag(QuickMenuEntry? entry, PointerEventData eventData)
        {
            if (entry == null || quickPanelRect == null || !quickVisible)
            {
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    quickPanelRect,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 localPoint))
            {
                return;
            }

            draggingQuickEntry = entry;
            draggingQuickIndex = quickEntries.IndexOf(entry);
            draggingQuickOffset = entry.Rect.anchoredPosition - localPoint;
            entry.Rect.SetAsLastSibling();
        }

        private void UpdateQuickMenuDrag(PointerEventData eventData)
        {
            if (draggingQuickEntry == null || quickPanelRect == null)
            {
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    quickPanelRect,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 localPoint))
            {
                return;
            }

            if (IsQuickMenuFreeLayoutEnabled())
            {
                Vector2 newPos = localPoint + draggingQuickOffset;
                draggingQuickEntry.Rect.anchoredPosition = ClampQuickMenuFreePosition(newPos);
                return;
            }

            float minY = GetQuickRowY(quickEntries.Count - 1);
            float maxY = GetQuickRowY(0);
            float newY = Mathf.Clamp(localPoint.y + draggingQuickOffset.y, minY, maxY);
            draggingQuickEntry.Rect.anchoredPosition = new Vector2(QuickButtonLeft, newY);

            int newIndex = Mathf.Clamp(
                Mathf.RoundToInt((QuickButtonTop - newY) / (QuickButtonHeight + QuickButtonSpacing)),
                0,
                quickEntries.Count - 1);

            if (newIndex != draggingQuickIndex && draggingQuickIndex >= 0)
            {
                quickEntries.Remove(draggingQuickEntry);
                quickEntries.Insert(newIndex, draggingQuickEntry);
                draggingQuickIndex = newIndex;
                UpdateQuickEntryPositions(false);
            }
        }

        private void EndQuickMenuDrag()
        {
            if (draggingQuickEntry == null)
            {
                return;
            }

            draggingQuickEntry = null;
            draggingQuickIndex = -1;
            if (IsQuickMenuFreeLayoutEnabled())
            {
                SaveQuickMenuPositions();
                return;
            }

            UpdateQuickEntryPositions(true);
            SaveQuickMenuOrder();
        }

        private void CancelQuickMenuDrag()
        {
            if (draggingQuickEntry == null)
            {
                return;
            }

            draggingQuickEntry = null;
            draggingQuickIndex = -1;
            if (IsQuickMenuFreeLayoutEnabled())
            {
                ApplyQuickMenuFreePositions();
                return;
            }

            UpdateQuickEntryPositions(true);
        }

        private void UpdateQuickEntryPositions(bool includeDragging)
        {
            if (IsQuickMenuFreeLayoutEnabled())
            {
                ApplyQuickMenuFreePositions();
                return;
            }

            for (int i = 0; i < quickEntries.Count; i++)
            {
                QuickMenuEntry entry = quickEntries[i];
                if (!includeDragging && entry == draggingQuickEntry)
                {
                    continue;
                }

                entry.Rect.anchoredPosition = new Vector2(QuickButtonLeft, GetQuickRowY(i));
            }
        }

        private void ApplyQuickMenuLayout()
        {
            if (quickPanelRect == null)
            {
                return;
            }

            if (quickPanelBackplateRect != null)
            {
                quickPanelBackplateRect.anchoredPosition = Vector2.zero;
                quickPanelBackplateRect.sizeDelta = new Vector2(QuickPanelWidth, QuickPanelHeight);
            }

            if (IsQuickMenuFreeLayoutEnabled())
            {
                quickPanelRect.anchoredPosition = Vector2.zero;
                quickPanelRect.sizeDelta = new Vector2(QuickPanelFreeWidth, QuickPanelFreeHeight);

                if (quickPanelImage != null)
                {
                    Color color = quickPanelImage.color;
                    quickPanelImage.color = new Color(color.r, color.g, color.b, QuickPanelBackplateAlpha);
                }

                EnsureQuickMenuPositions();
                ApplyQuickMenuFreePositions();
            }
            else
            {
                quickPanelRect.anchoredPosition = new Vector2(QuickPanelDefaultLeft, QuickPanelDefaultTop);
                quickPanelRect.sizeDelta = new Vector2(QuickPanelWidth, QuickPanelHeight);

                if (quickPanelImage != null)
                {
                    Color color = quickPanelImage.color;
                    quickPanelImage.color = new Color(color.r, color.g, color.b, QuickPanelBackplateAlpha);
                }

                UpdateQuickEntryPositions(true);
            }

            ApplyQuickMenuOpacity();
        }

        private void EnsureQuickMenuPositions()
        {
            Dictionary<string, QuickMenuEntryPosition> positions = GodhomeQoL.GlobalSettings.QuickMenuPositions ??= new Dictionary<string, QuickMenuEntryPosition>();
            bool changed = false;

            for (int i = 0; i < quickEntries.Count; i++)
            {
                QuickMenuEntry entry = quickEntries[i];
                if (!positions.ContainsKey(entry.Id))
                {
                    positions[entry.Id] = new QuickMenuEntryPosition(QuickButtonLeft, GetQuickRowY(i));
                    changed = true;
                }
            }

            if (changed)
            {
                GodhomeQoL.SaveGlobalSettingsSafe();
            }
        }

        private void ApplyQuickMenuFreePositions()
        {
            Dictionary<string, QuickMenuEntryPosition> positions = GodhomeQoL.GlobalSettings.QuickMenuPositions ??= new Dictionary<string, QuickMenuEntryPosition>();

            for (int i = 0; i < quickEntries.Count; i++)
            {
                QuickMenuEntry entry = quickEntries[i];
                Vector2 fallback = new(QuickButtonLeft, GetQuickRowY(i));
                if (positions.TryGetValue(entry.Id, out QuickMenuEntryPosition? stored))
                {
                    entry.Rect.anchoredPosition = ClampQuickMenuFreePosition(new Vector2(stored.X, stored.Y));
                }
                else
                {
                    entry.Rect.anchoredPosition = ClampQuickMenuFreePosition(fallback);
                }
            }
        }

        private void SaveQuickMenuPositions()
        {
            Dictionary<string, QuickMenuEntryPosition> positions = new();
            foreach (QuickMenuEntry entry in quickEntries)
            {
                Vector2 pos = entry.Rect.anchoredPosition;
                positions[entry.Id] = new QuickMenuEntryPosition(pos.x, pos.y);
            }

            GodhomeQoL.GlobalSettings.QuickMenuPositions = positions;
            GodhomeQoL.SaveGlobalSettingsSafe();
        }

        private void SaveQuickMenuOrder()
        {
            List<string> order = quickEntries.Select(entry => entry.Id).ToList();
            GodhomeQoL.GlobalSettings.QuickMenuOrder = order;
            GodhomeQoL.SaveGlobalSettingsSafe();
        }

        private void RebuildQuickMenuEntries()
        {
            if (quickPanelRect == null)
            {
                return;
            }

            CancelQuickMenuRename();

            foreach (QuickMenuEntry entry in quickEntries)
            {
                UObject.Destroy(entry.Rect.gameObject);
            }

            quickEntries.Clear();
            quickHandleSprite ??= LoadQuickHandleSprite();

            List<QuickMenuItemDefinition> orderedItems = GetVisibleQuickMenuDefinitions();
            Transform parent = quickPanelContentRect != null ? quickPanelContentRect.transform : quickPanelRect.transform;
            for (int i = 0; i < orderedItems.Count; i++)
            {
                QuickMenuEntry entry = CreateQuickEntry(parent, orderedItems[i], GetQuickRowY(i));
                quickEntries.Add(entry);
            }

            ApplyQuickMenuLayout();
            UpdateFreeMenuLabel();
            UpdateRenameModeLabel();
        }

        private Vector2 ClampQuickMenuFreePosition(Vector2 position)
        {
            if (quickPanelRect == null)
            {
                return position;
            }

            float width = quickPanelRect.rect.width;
            float height = quickPanelRect.rect.height;
            float minX = 0f;
            float maxX = Mathf.Max(0f, width - QuickRowWidth);
            float maxY = 0f;
            float minY = -(height - QuickButtonHeight);

            float x = Mathf.Clamp(position.x, minX, maxX);
            float y = Mathf.Clamp(position.y, minY, maxY);
            return new Vector2(x, y);
        }

        private sealed class ScrollContentMarker : MonoBehaviour
        {
        }

        private static RectTransform CreateScrollContent(Transform panel, float panelWidth, float panelHeight, float topOffset, float bottomOffset)
        {
            float viewHeight = Mathf.Max(0f, panelHeight - topOffset - bottomOffset);

            GameObject scrollObj = new("Scroll");
            scrollObj.transform.SetParent(panel, false);

            RectTransform scrollRect = scrollObj.AddComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0.5f, 0.5f);
            scrollRect.anchorMax = new Vector2(0.5f, 0.5f);
            scrollRect.pivot = new Vector2(0.5f, 0.5f);
            float topEdgeY = (panelHeight * 0.5f) - topOffset;
            float centerY = topEdgeY - (viewHeight * 0.5f);
            scrollRect.anchoredPosition = new Vector2(0f, centerY);
            scrollRect.sizeDelta = new Vector2(panelWidth, viewHeight);

            ScrollRect scroll = scrollObj.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.inertia = true;
            scroll.scrollSensitivity = 30f;

            GameObject viewport = new("Viewport");
            viewport.transform.SetParent(scrollObj.transform, false);
            RectTransform viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.pivot = new Vector2(0.5f, 0.5f);
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            Image viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = new Color(1f, 1f, 1f, 0f);
            viewportImage.raycastTarget = true;

            _ = viewport.AddComponent<RectMask2D>();

            GameObject contentObj = new("Content");
            contentObj.transform.SetParent(viewport.transform, false);
            RectTransform contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0f, viewHeight);

            contentObj.AddComponent<ScrollContentMarker>();

            GameObject scrollbarObj = new("Scrollbar");
            scrollbarObj.transform.SetParent(scrollObj.transform, false);
            RectTransform scrollbarRect = scrollbarObj.AddComponent<RectTransform>();
            scrollbarRect.anchorMin = new Vector2(1f, 0f);
            scrollbarRect.anchorMax = new Vector2(1f, 1f);
            scrollbarRect.pivot = new Vector2(1f, 0.5f);
            scrollbarRect.anchoredPosition = new Vector2(-ScrollbarRightPadding, 0f);
            scrollbarRect.sizeDelta = new Vector2(ScrollbarWidth, 0f);

            Image scrollbarImage = scrollbarObj.AddComponent<Image>();
            scrollbarImage.color = new Color(1f, 1f, 1f, 0.12f);

            GameObject handleObj = new("Handle");
            handleObj.transform.SetParent(scrollbarObj.transform, false);
            RectTransform handleRect = handleObj.AddComponent<RectTransform>();
            handleRect.anchorMin = Vector2.zero;
            handleRect.anchorMax = Vector2.one;
            handleRect.offsetMin = Vector2.zero;
            handleRect.offsetMax = Vector2.zero;

            Image handleImage = handleObj.AddComponent<Image>();
            handleImage.color = new Color(1f, 1f, 1f, 0.55f);

            Scrollbar scrollbar = scrollbarObj.AddComponent<Scrollbar>();
            scrollbar.handleRect = handleRect;
            scrollbar.targetGraphic = handleImage;
            scrollbar.direction = Scrollbar.Direction.BottomToTop;

            scroll.viewport = viewportRect;
            scroll.content = contentRect;
            scroll.verticalScrollbar = scrollbar;
            scroll.verticalNormalizedPosition = 1f;

            return contentRect;
        }

        private static bool IsScrollContent(Transform parent) => parent.GetComponent<ScrollContentMarker>() != null;

        private static float GetFixedBackY(float panelHeight) => -(panelHeight * 0.5f) + FixedBackOffset;

        private static float GetFixedResetY(float panelHeight) => -(panelHeight * 0.5f) + FixedResetOffset;

        private static float GetScrollTopOffset(float panelHeight, RectTransform titleRect)
        {
            float titleBottomY = titleRect.anchoredPosition.y - (titleRect.sizeDelta.y * 0.5f);
            return Mathf.Max(0f, (panelHeight * 0.5f) - titleBottomY + ScrollTopPadding);
        }

        private static float GetScrollBottomOffset(float panelHeight, float highestButtonY)
        {
            float buttonTopY = highestButtonY + (ButtonRowHeight * 0.5f);
            return Mathf.Max(0f, (panelHeight * 0.5f) + buttonTopY + ScrollBottomPadding);
        }

        private static float ToTopY(float panelHeight, float centerY) => (panelHeight * 0.5f) - centerY;

        private static float GetRowStartY(float panelHeight, float rowStartCenterY, float topOffset)
        {
            return Mathf.Max(0f, ToTopY(panelHeight, rowStartCenterY) - topOffset);
        }

        private static void SetScrollContentHeight(RectTransform content, float viewHeight, float lastCenterY, float rowHeight)
        {
            float contentHeight = lastCenterY + (rowHeight * 0.5f) + 40f;
            content.sizeDelta = new Vector2(0f, Mathf.Max(contentHeight, viewHeight));
        }

        private static void ConfigureRowRect(RectTransform rect, Transform parent, float y, Vector2 size)
        {
            if (IsScrollContent(parent))
            {
                rect.anchorMin = new Vector2(0.5f, 1f);
                rect.anchorMax = new Vector2(0.5f, 1f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = new Vector2(0f, -y);
            }
            else
            {
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = new Vector2(0f, y);
            }

            rect.sizeDelta = size;
        }

        private static GameObject CreateRow(Transform parent, string name, float y, Vector2 size)
        {
            GameObject row = new GameObject(name);
            row.transform.SetParent(parent, false);

            RectTransform rect = row.AddComponent<RectTransform>();
            ConfigureRowRect(rect, parent, y, size);

            return row;
        }

        private RowHighlight CreateRowHighlight(GameObject row, Image? baseImage)
        {
            Image image = baseImage ?? row.GetComponent<Image>() ?? row.AddComponent<Image>();
            float baseAlpha = image.color.a;
            float highlightAlpha = Mathf.Clamp(baseAlpha + 0.12f, 0.12f, 0.18f);
            Color highlightColor = new(1f, 1f, 1f, highlightAlpha);

            RowHighlight highlight = row.AddComponent<RowHighlight>();
            highlight.Initialize(image, highlightColor);
            return highlight;
        }

        private static void AttachRowHighlight(GameObject target, RowHighlight highlight)
        {
            RowHighlightTrigger trigger = target.AddComponent<RowHighlightTrigger>();
            trigger.Highlight = highlight;
        }

        private void CreateToggleRow(
            Transform parent,
            string name,
            string label,
            float y,
            Func<bool> getter,
            Action<bool> setter,
            out Text valueText)
        {
            GameObject row = CreateRow(parent, name, y, new Vector2(RowWidth, RowHeight));
            Image image = row.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);

            Button button = row.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = image;

            _ = CreateRowLabel(row.transform, label);
            Text valueLabel = CreateRowValue(row.transform, getter() ? "ON" : "OFF");
            valueText = valueLabel;

            button.onClick.AddListener(() =>
            {
                bool newValue = !getter();
                setter(newValue);
                UpdateToggleValue(valueLabel, newValue);
            });

            RowHighlight highlight = CreateRowHighlight(row, image);
            AttachRowHighlight(row, highlight);
        }

        private void CreateCycleRow(
            Transform parent,
            string name,
            string label,
            float y,
            string[] options,
            Func<int> getter,
            Action<int> setter,
            out Text valueText)
        {
            GameObject row = CreateRow(parent, name, y, new Vector2(RowWidth, RowHeight));
            Image image = row.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);

            Button button = row.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = image;

            _ = CreateRowLabel(row.transform, label);

            int current = ClampOptionIndex(getter(), options.Length);
            Text valueLabel = CreateRowValue(row.transform, options[current]);
            valueText = valueLabel;

            button.onClick.AddListener(() =>
            {
                int next = ClampOptionIndex(getter(), options.Length) + 1;
                if (next >= options.Length)
                {
                    next = 0;
                }

                setter(next);
                valueLabel.text = options[next];
            });

            RowHighlight highlight = CreateRowHighlight(row, image);
            AttachRowHighlight(row, highlight);
        }

        private void CreateAdjustInputRow(
            Transform parent,
            string name,
            string label,
            float y,
            Func<int> getter,
            Action<int> setter,
            int minValue,
            int maxValue,
            int baseStep,
            out InputField valueField)
        {
            GameObject row = CreateRow(parent, name, y, new Vector2(RowWidth, RowHeight));
            Image image = row.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);

            _ = CreateRowLabel(row.transform, label);

            InputField input = CreateInputField(
                row.transform,
                getter().ToString(),
                new Vector2(InputControlValueRight, 0f),
                new Vector2(InputControlValueWidth, RowHeight));
            valueField = input;

            input.onEndEdit.AddListener(value =>
                ApplyInputValue(value, getter, setter, input, minValue, maxValue));

            Button minus = CreateMiniButton(row.transform, "Minus", "-", new Vector2(InputControlMinusRight, 0f));
            minus.onClick.AddListener(() => AdjustIntInputValue(getter, setter, input, -1, minValue, maxValue, baseStep));

            Button plus = CreateMiniButton(row.transform, "Plus", "+", new Vector2(InputControlPlusRight, 0f));
            plus.onClick.AddListener(() => AdjustIntInputValue(getter, setter, input, 1, minValue, maxValue, baseStep));

            RowHighlight highlight = CreateRowHighlight(row, image);
            AttachRowHighlight(row, highlight);
            AttachRowHighlight(input.gameObject, highlight);
            AttachRowHighlight(minus.gameObject, highlight);
            AttachRowHighlight(plus.gameObject, highlight);
        }

        private InputField CreateInputField(Transform parent, string text, Vector2 anchoredPosition, Vector2 size)
        {
            GameObject fieldObj = new GameObject("InputField");
            fieldObj.transform.SetParent(parent, false);

            RectTransform rect = fieldObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0.5f);
            rect.anchorMax = new Vector2(1f, 0.5f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            Image image = fieldObj.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.08f);

            InputField input = fieldObj.AddComponent<InputField>();
            input.contentType = InputField.ContentType.IntegerNumber;
            input.lineType = InputField.LineType.SingleLine;
            input.caretColor = Color.white;
            input.selectionColor = new Color(1f, 1f, 1f, 0.25f);
            input.targetGraphic = image;

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(fieldObj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(6f, 0f);
            textRect.offsetMax = new Vector2(-6f, 0f);

            Text valueText = textObj.AddComponent<Text>();
            valueText.text = text;
            valueText.font = GetMenuFont();
            valueText.fontSize = 26;
            valueText.alignment = TextAnchor.MiddleCenter;
            valueText.color = Color.white;
            valueText.raycastTarget = false;

            input.textComponent = valueText;
            input.text = text;

            return input;
        }

        private Button CreateMiniButton(Transform parent, string name, string label, Vector2 anchoredPosition)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0.5f);
            rect.anchorMax = new Vector2(1f, 0.5f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(InputControlButtonWidth, InputControlButtonHeight);

            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.08f);

            Outline outline = buttonObj.AddComponent<Outline>();
            outline.effectColor = Color.white;
            outline.effectDistance = new Vector2(1f, -1f);

            Button button = buttonObj.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = image;

            Text text = CreateText(buttonObj.transform, "Label", label, 22, TextAnchor.MiddleCenter);
            RectTransform textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            return button;
        }

        private void CreateSpeedRow(Transform parent, float labelY, float sliderY)
        {
            GameObject row = CreateRow(parent, "SpeedRow", labelY, new Vector2(RowWidth, RowHeight));
            Image image = row.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);
            _ = CreateRowLabel(row.transform, "Settings/fastSuperDashSpeedMultiplier".Localize());
            speedValueText = CreateRowValue(row.transform, FormatToggleValue(Modules.QoL.FastSuperDash.fastSuperDashSpeedMultiplier));

            GameObject sliderObj = new GameObject("SpeedSlider");
            sliderObj.transform.SetParent(parent, false);

            RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
            ConfigureRowRect(sliderRect, parent, sliderY, new Vector2(RowWidth - 120f, 24f));

            Image sliderBg = sliderObj.AddComponent<Image>();
            sliderBg.color = new Color(1f, 1f, 1f, 0.15f);

            Slider slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 1f;
            slider.maxValue = 10f;
            slider.value = Modules.QoL.FastSuperDash.fastSuperDashSpeedMultiplier;
            slider.wholeNumbers = false;
            slider.direction = Slider.Direction.LeftToRight;

            GameObject fillArea = new GameObject("FillArea");
            fillArea.transform.SetParent(sliderObj.transform, false);
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0f, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1f, 0.75f);
            fillAreaRect.offsetMin = new Vector2(10f, 0f);
            fillAreaRect.offsetMax = new Vector2(-10f, 0f);

            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            Image fillImage = fill.AddComponent<Image>();
            fillImage.color = new Color(1f, 1f, 1f, 0.75f);
            slider.fillRect = fillRect;

            GameObject handle = new GameObject("Handle");
            handle.transform.SetParent(sliderObj.transform, false);
            RectTransform handleRect = handle.AddComponent<RectTransform>();
            handleRect.anchorMin = new Vector2(0f, 0.5f);
            handleRect.anchorMax = new Vector2(0f, 0.5f);
            handleRect.pivot = new Vector2(0.5f, 0.5f);
            handleRect.sizeDelta = new Vector2(18f, 24f);

            Image handleImage = handle.AddComponent<Image>();
            handleImage.color = Color.white;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImage;

            slider.onValueChanged.AddListener(OnSpeedChanged);
            speedSlider = slider;

            RowHighlight highlight = CreateRowHighlight(row, image);
            AttachRowHighlight(row, highlight);
            AttachRowHighlight(sliderObj, highlight);
        }

        private void CreateFloatSliderRow(
            Transform parent,
            string name,
            string label,
            float labelY,
            float sliderY,
            float minValue,
            float maxValue,
            float value,
            int decimals,
            out Text valueText,
            out Slider slider)
        {
            GameObject row = CreateRow(parent, name, labelY, new Vector2(RowWidth, RowHeight));
            Image image = row.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);
            _ = CreateRowLabel(row.transform, label);
            valueText = CreateRowValue(row.transform, FormatFloatValue(value, decimals));

            GameObject sliderObj = new GameObject($"{name}Slider");
            sliderObj.transform.SetParent(parent, false);

            RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
            ConfigureRowRect(sliderRect, parent, sliderY, new Vector2(RowWidth - 120f, 24f));

            Image sliderBg = sliderObj.AddComponent<Image>();
            sliderBg.color = new Color(1f, 1f, 1f, 0.15f);

            Slider sliderComponent = sliderObj.AddComponent<Slider>();
            sliderComponent.minValue = minValue;
            sliderComponent.maxValue = maxValue;
            sliderComponent.value = Mathf.Clamp(value, minValue, maxValue);
            sliderComponent.wholeNumbers = false;
            sliderComponent.direction = Slider.Direction.LeftToRight;

            GameObject fillArea = new GameObject("FillArea");
            fillArea.transform.SetParent(sliderObj.transform, false);
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0f, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1f, 0.75f);
            fillAreaRect.offsetMin = new Vector2(10f, 0f);
            fillAreaRect.offsetMax = new Vector2(-10f, 0f);

            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            Image fillImage = fill.AddComponent<Image>();
            fillImage.color = new Color(1f, 1f, 1f, 0.75f);
            sliderComponent.fillRect = fillRect;

            GameObject handle = new GameObject("Handle");
            handle.transform.SetParent(sliderObj.transform, false);
            RectTransform handleRect = handle.AddComponent<RectTransform>();
            handleRect.anchorMin = new Vector2(0f, 0.5f);
            handleRect.anchorMax = new Vector2(0f, 0.5f);
            handleRect.pivot = new Vector2(0.5f, 0.5f);
            handleRect.sizeDelta = new Vector2(18f, 24f);

            Image handleImage = handle.AddComponent<Image>();
            handleImage.color = Color.white;
            sliderComponent.handleRect = handleRect;
            sliderComponent.targetGraphic = handleImage;

            slider = sliderComponent;

            RowHighlight highlight = CreateRowHighlight(row, image);
            AttachRowHighlight(row, highlight);
            AttachRowHighlight(sliderObj, highlight);
        }

        private void CreateButtonRow(Transform parent, string name, string label, float y, Action onClick)
        {
            GameObject row = CreateRow(parent, name, y, new Vector2(260f, ButtonRowHeight));
            Image image = row.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.08f);

            Button button = row.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = image;
            button.onClick.AddListener(() => onClick());

            Text text = CreateText(row.transform, "Label", label, 28, TextAnchor.MiddleCenter);
            RectTransform rect = text.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            RowHighlight highlight = CreateRowHighlight(row, image);
            AttachRowHighlight(row, highlight);
        }

        private void CreateKeybindRow(
            Transform parent,
            string name,
            string label,
            float y,
            Func<string> valueGetter,
            Action onClick,
            out Text valueText)
        {
            GameObject row = CreateRow(parent, name, y, new Vector2(RowWidth, RowHeight));
            Image image = row.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);

            Button button = row.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = image;
            button.onClick.AddListener(() => onClick());

            _ = CreateRowLabel(row.transform, label);

            Text valueLabel = CreateText(row.transform, "Value", valueGetter(), 26, TextAnchor.MiddleRight);
            RectTransform rect = valueLabel.rectTransform;
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = new Vector2(-20f, 0f);
            rect.sizeDelta = new Vector2(240f, RowHeight);
            valueText = valueLabel;

            RowHighlight highlight = CreateRowHighlight(row, image);
            AttachRowHighlight(row, highlight);
        }

        private void CreateCollectorToggleRow(
            Transform parent,
            string name,
            string label,
            float y,
            Func<bool> getter,
            Action<bool> setter,
            out Text valueText)
        {
            GameObject row = CreateRow(parent, name, y, new Vector2(CollectorRowWidth, CollectorRowHeight));
            Image image = row.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);

            Button button = row.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = image;

            _ = CreateCollectorRowLabel(row.transform, label);
            Text valueLabel = CreateCollectorRowValue(row.transform, getter() ? "ON" : "OFF");
            valueText = valueLabel;

            button.onClick.AddListener(() =>
            {
                bool newValue = !getter();
                setter(newValue);
                UpdateToggleValue(valueLabel, newValue);
            });

            RowHighlight highlight = CreateRowHighlight(row, image);
            AttachRowHighlight(row, highlight);
        }

        private void CreateCollectorAdjustRow(
            Transform parent,
            string name,
            string label,
            float y,
            Func<int> getter,
            Action<int> setter,
            int minValue,
            int maxValue,
            int baseStep,
            out Text valueText)
        {
            GameObject row = CreateRow(parent, name, y, new Vector2(CollectorRowWidth, CollectorRowHeight));
            Image image = row.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);

            _ = CreateCollectorRowLabel(row.transform, label);

            Text valueLabel = CreateText(row.transform, "Value", getter().ToString(), CollectorValueFontSize, TextAnchor.MiddleCenter);
            RectTransform valueRect = valueLabel.rectTransform;
            valueRect.anchorMin = new Vector2(1f, 0.5f);
            valueRect.anchorMax = new Vector2(1f, 0.5f);
            valueRect.pivot = new Vector2(1f, 0.5f);
            valueRect.anchoredPosition = new Vector2(CollectorControlValueRight, 0f);
            valueRect.sizeDelta = new Vector2(CollectorControlValueWidth, CollectorRowHeight);
            valueText = valueLabel;

            Button minus = CreateCollectorMiniButton(row.transform, "Minus", "-", new Vector2(CollectorControlMinusRight, 0f));
            minus.onClick.AddListener(() => AdjustIntValue(getter, setter, valueLabel, -1, minValue, maxValue, baseStep));

            Button plus = CreateCollectorMiniButton(row.transform, "Plus", "+", new Vector2(CollectorControlPlusRight, 0f));
            plus.onClick.AddListener(() => AdjustIntValue(getter, setter, valueLabel, 1, minValue, maxValue, baseStep));

            RowHighlight highlight = CreateRowHighlight(row, image);
            AttachRowHighlight(row, highlight);
            AttachRowHighlight(minus.gameObject, highlight);
            AttachRowHighlight(plus.gameObject, highlight);
        }

        private void CreateCollectorAdjustInputRow(
            Transform parent,
            string name,
            string label,
            float y,
            Func<int> getter,
            Action<int> setter,
            int minValue,
            int maxValue,
            int baseStep,
            out InputField valueField)
        {
            GameObject row = CreateRow(parent, name, y, new Vector2(CollectorRowWidth, CollectorRowHeight));
            Image image = row.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);

            _ = CreateCollectorRowLabel(row.transform, label);

            InputField input = CreateCollectorInputField(
                row.transform,
                getter().ToString(),
                new Vector2(CollectorControlValueRight, 0f),
                new Vector2(CollectorControlValueWidth, CollectorRowHeight));
            valueField = input;

            input.onEndEdit.AddListener(value =>
                ApplyInputValue(value, getter, setter, input, minValue, maxValue));

            Button minus = CreateCollectorMiniButton(row.transform, "Minus", "-", new Vector2(CollectorControlMinusRight, 0f));
            minus.onClick.AddListener(() => AdjustIntInputValue(getter, setter, input, -1, minValue, maxValue, baseStep));

            Button plus = CreateCollectorMiniButton(row.transform, "Plus", "+", new Vector2(CollectorControlPlusRight, 0f));
            plus.onClick.AddListener(() => AdjustIntInputValue(getter, setter, input, 1, minValue, maxValue, baseStep));

            RowHighlight highlight = CreateRowHighlight(row, image);
            AttachRowHighlight(row, highlight);
            AttachRowHighlight(input.gameObject, highlight);
            AttachRowHighlight(minus.gameObject, highlight);
            AttachRowHighlight(plus.gameObject, highlight);
        }

        private InputField CreateCollectorInputField(Transform parent, string text, Vector2 anchoredPosition, Vector2 size)
        {
            GameObject fieldObj = new GameObject("InputField");
            fieldObj.transform.SetParent(parent, false);

            RectTransform rect = fieldObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0.5f);
            rect.anchorMax = new Vector2(1f, 0.5f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            Image image = fieldObj.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.08f);

            InputField input = fieldObj.AddComponent<InputField>();
            input.contentType = InputField.ContentType.IntegerNumber;
            input.lineType = InputField.LineType.SingleLine;
            input.caretColor = Color.white;
            input.selectionColor = new Color(1f, 1f, 1f, 0.25f);
            input.targetGraphic = image;

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(fieldObj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(6f, 0f);
            textRect.offsetMax = new Vector2(-6f, 0f);

            Text valueText = textObj.AddComponent<Text>();
            valueText.text = text;
            valueText.font = GetMenuFont();
            valueText.fontSize = CollectorValueFontSize;
            valueText.alignment = TextAnchor.MiddleCenter;
            valueText.color = Color.white;
            valueText.raycastTarget = false;

            input.textComponent = valueText;
            input.text = text;

            return input;
        }

        private Text CreateCollectorRowLabel(Transform parent, string text)
        {
            Text label = CreateText(parent, "Label", text, CollectorLabelFontSize, TextAnchor.MiddleLeft);
            RectTransform rect = label.rectTransform;
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(20f, 0f);
            rect.sizeDelta = new Vector2(CollectorLabelWidth, CollectorRowHeight);
            return label;
        }

        private Text CreateCollectorRowValue(Transform parent, string text)
        {
            Text value = CreateText(parent, "Value", text, CollectorValueFontSize, TextAnchor.MiddleRight);
            RectTransform rect = value.rectTransform;
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = new Vector2(-20f, 0f);
            rect.sizeDelta = new Vector2(160f, CollectorRowHeight);
            return value;
        }

        private Button CreateCollectorMiniButton(Transform parent, string name, string label, Vector2 anchoredPosition)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0.5f);
            rect.anchorMax = new Vector2(1f, 0.5f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(CollectorControlButtonWidth, CollectorControlButtonHeight);

            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.08f);

            Outline outline = buttonObj.AddComponent<Outline>();
            outline.effectColor = Color.white;
            outline.effectDistance = new Vector2(1f, -1f);

            Button button = buttonObj.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = image;

            Text text = CreateText(buttonObj.transform, "Label", label, 22, TextAnchor.MiddleCenter);
            RectTransform textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            return button;
        }

        private void ToggleMenu()
        {
            if (quickVisible || quickSettingsVisible || overlayVisible || collectorVisible || fastReloadVisible || dreamshieldVisible || showHpOnDeathVisible || speedChangerVisible || teleportKitVisible || bossChallengeVisible || qolVisible || menuAnimationVisible || bossAnimationVisible || bindingsVisible || zoteHelperVisible)
            {
                returnToQuickOnClose = false;
                returnToQolOnClose = false;
                SetQuickVisible(false);
                SetOverlayVisible(false);
                SetCollectorVisible(false);
                SetFastReloadVisible(false);
                SetDreamshieldVisible(false);
                SetShowHpOnDeathVisible(false);
                SetSpeedChangerVisible(false);
                SetTeleportKitVisible(false);
                SetBossChallengeVisible(false);
                SetQolVisible(false);
                SetMenuAnimationVisible(false);
                SetBossAnimationVisible(false);
                SetBindingsVisible(false);
                SetZoteHelperVisible(false);
                SetQuickSettingsVisible(false);
                return;
            }

            SetQuickVisible(true);
        }


        private void SetQuickVisible(bool value, bool instant = false)
        {
            quickVisible = value;
            if (quickRoot == null)
            {
                UpdateUiState();
                return;
            }

            if (!value)
            {
                CancelQuickMenuDrag();
                CancelQuickMenuRename();

                if (quickPanelGroup != null)
                {
                    quickPanelGroup.interactable = false;
                    quickPanelGroup.blocksRaycasts = false;
                }

                StopQuickMenuFade();
                quickMenuFadeAlpha = 0f;
                UpdateQuickMenuAlpha();
                quickRoot.SetActive(false);
            }
            else
            {
                quickRoot.SetActive(true);
                ApplyQuickMenuLayout();
                UpdateFreeMenuLabel();
                UpdateRenameModeLabel();

                if (quickPanelGroup != null)
                {
                    quickPanelGroup.interactable = true;
                    quickPanelGroup.blocksRaycasts = true;
                }

                StopQuickMenuFade();
                quickMenuFadeAlpha = 1f;
                UpdateQuickMenuAlpha();
            }

            UpdateUiState();
        }

        private void SetQuickSettingsVisible(bool value)
        {
            quickSettingsVisible = value;
            if (quickSettingsRoot != null)
            {
                quickSettingsRoot.SetActive(value);
            }

            if (value)
            {
                RefreshQuickMenuSettingsUi();
            }

            UpdateUiState();
        }

        private void SetOverlayVisible(bool value)
        {
            overlayVisible = value;
            if (overlayRoot != null)
            {
                overlayRoot.SetActive(value);
            }

            if (value)
            {
                RefreshFastSuperDashUi();
            }

            UpdateUiState();
        }

        private void SetCollectorVisible(bool value)
        {
            collectorVisible = value;
            if (collectorRoot != null)
            {
                collectorRoot.SetActive(value);
            }

            if (value)
            {
                RefreshCollectorPhasesUi();
            }

            UpdateUiState();
        }

        private void SetFastReloadVisible(bool value)
        {
            fastReloadVisible = value;
            if (fastReloadRoot != null)
            {
                fastReloadRoot.SetActive(value);
            }

            if (value)
            {
                RefreshFastReloadUi();
            }
            else
            {
                CancelFastReloadRebind();
            }

            UpdateUiState();
        }

        private void SetDreamshieldVisible(bool value)
        {
            dreamshieldVisible = value;
            if (dreamshieldRoot != null)
            {
                dreamshieldRoot.SetActive(value);
            }

            if (value)
            {
                RefreshDreamshieldUi();
            }

            UpdateUiState();
        }

        private void SetShowHpOnDeathVisible(bool value)
        {
            showHpOnDeathVisible = value;
            if (showHpOnDeathRoot != null)
            {
                showHpOnDeathRoot.SetActive(value);
            }

            if (value)
            {
                RefreshShowHpOnDeathUi();
            }
            else
            {
                CancelShowHpOnDeathRebind();
            }

            UpdateUiState();
        }

        private void SetSpeedChangerVisible(bool value)
        {
            speedChangerVisible = value;
            if (speedChangerRoot != null)
            {
                speedChangerRoot.SetActive(value);
            }

            if (value)
            {
                RefreshSpeedChangerUi();
            }
            else
            {
                CancelSpeedChangerRebind();
            }

            UpdateUiState();
        }

        private void SetTeleportKitVisible(bool value)
        {
            teleportKitVisible = value;
            if (teleportKitRoot != null)
            {
                teleportKitRoot.SetActive(value);
            }

            if (value)
            {
                RefreshTeleportKitUi();
            }
            else
            {
                CancelTeleportKitRebind();
            }

            UpdateUiState();
        }

        private void SetBossChallengeVisible(bool value)
        {
            bossChallengeVisible = value;
            if (bossChallengeRoot != null)
            {
                bossChallengeRoot.SetActive(value);
            }

            if (value)
            {
                RefreshBossChallengeUi();
            }

            UpdateUiState();
        }

        private void SetQolVisible(bool value)
        {
            qolVisible = value;
            if (qolRoot != null)
            {
                qolRoot.SetActive(value);
            }

            if (value)
            {
                RefreshQolUi();
            }

            UpdateUiState();
        }

        private void SetMenuAnimationVisible(bool value)
        {
            menuAnimationVisible = value;
            if (menuAnimationRoot != null)
            {
                menuAnimationRoot.SetActive(value);
            }

            if (value)
            {
                RefreshMenuAnimationUi();
            }

            UpdateUiState();
        }

        private void SetBossAnimationVisible(bool value)
        {
            bossAnimationVisible = value;
            if (bossAnimationRoot != null)
            {
                bossAnimationRoot.SetActive(value);
            }

            if (value)
            {
                RefreshBossAnimationUi();
            }
            else
            {
                CancelFastDreamWarpRebind();
            }

            UpdateUiState();
        }

        private void SetBindingsVisible(bool value)
        {
            bindingsVisible = value;
            if (bindingsRoot != null)
            {
                bindingsRoot.SetActive(value);
            }

            if (value)
            {
                RefreshBindingsUi();
            }
            else
            {
                CancelBindingsMenuRebind();
            }

            UpdateUiState();
        }

        private void SetZoteHelperVisible(bool value)
        {
            zoteHelperVisible = value;
            if (zoteHelperRoot != null)
            {
                zoteHelperRoot.SetActive(value);
            }

            if (value)
            {
                RefreshZoteHelperUi();
            }

            UpdateUiState();
        }

        private void UpdateUiState()
        {
            bool anyVisible = quickVisible || quickSettingsVisible || overlayVisible || collectorVisible || fastReloadVisible || dreamshieldVisible || showHpOnDeathVisible || speedChangerVisible || teleportKitVisible || bossChallengeVisible || qolVisible || menuAnimationVisible || bossAnimationVisible || bindingsVisible || zoteHelperVisible;

            if (anyVisible && !uiActive)
            {
                EnableUiInteraction();
                EnableCursorHook();
                uiActive = true;
                return;
            }

            if (!anyVisible && uiActive)
            {
                DisableCursorHook();
                RestoreUiInteraction();
                uiActive = false;
            }
        }

        private void EnableCursorHook()
        {
            if (cursorHookActive)
            {
                return;
            }

            On.InputHandler.OnGUI += ForceCursorVisible;
            ModHooks.CursorHook += ForceCursorVisible;
            cursorHookActive = true;
        }

        private void DisableCursorHook()
        {
            if (!cursorHookActive)
            {
                return;
            }

            On.InputHandler.OnGUI -= ForceCursorVisible;
            ModHooks.CursorHook -= ForceCursorVisible;
            cursorHookActive = false;
        }

        private static void ForceCursorVisible(On.InputHandler.orig_OnGUI orig, InputHandler self)
        {
            orig(self);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private static void ForceCursorVisible()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void ResetFreeMenuPositions()
        {
            EnsureQuickMenuPositions();
            if (quickVisible && IsQuickMenuFreeLayoutEnabled())
            {
                ApplyQuickMenuFreePositions();
            }
        }

        private bool IsQuickMenuFreeLayoutEnabled()
        {
            return GodhomeQoL.GlobalSettings.QuickMenuFreeLayout;
        }

        private int GetQuickMenuOpacity()
        {
            return Mathf.Clamp(GodhomeQoL.GlobalSettings.QuickMenuOpacity, 1, 100);
        }

        private float GetQuickMenuOpacityAlpha()
        {
            return GetQuickMenuOpacity() / 100f;
        }

        private void ApplyQuickMenuOpacity()
        {
            quickMenuOpacityAlpha = GetQuickMenuOpacityAlpha();
            UpdateQuickMenuAlpha();
        }

        private void UpdateQuickMenuAlpha()
        {
            if (quickPanelGroup != null)
            {
                quickPanelGroup.alpha = quickMenuOpacityAlpha * quickMenuFadeAlpha;
            }
        }

        private void StartQuickMenuFade(bool show)
        {
            if (quickPanelGroup == null || quickRoot == null)
            {
                return;
            }

            StopQuickMenuFade();
            float target = show ? 1f : 0f;
            quickMenuFadeCoroutine = StartCoroutine(FadeQuickMenu(target, show));
        }

        private void StopQuickMenuFade()
        {
            if (quickMenuFadeCoroutine != null)
            {
                StopCoroutine(quickMenuFadeCoroutine);
                quickMenuFadeCoroutine = null;
            }
        }

        private IEnumerator FadeQuickMenu(float target, bool show)
        {
            float start = quickMenuFadeAlpha;
            float duration = QuickMenuFadeSeconds;
            if (Mathf.Abs(target - start) < 0.001f || duration <= 0f)
            {
                quickMenuFadeAlpha = target;
                UpdateQuickMenuAlpha();
                if (!show && quickRoot != null)
                {
                    quickRoot.SetActive(false);
                }

                quickMenuFadeCoroutine = null;
                yield break;
            }

            float time = 0f;
            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(time / duration);
                quickMenuFadeAlpha = Mathf.Lerp(start, target, t);
                UpdateQuickMenuAlpha();
                yield return null;
            }

            quickMenuFadeAlpha = target;
            UpdateQuickMenuAlpha();
            if (!show && quickRoot != null)
            {
                quickRoot.SetActive(false);
            }

            quickMenuFadeCoroutine = null;
        }

        private void SetQuickMenuFreeLayoutEnabled(bool value)
        {
            GodhomeQoL.GlobalSettings.QuickMenuFreeLayout = value;
            GodhomeQoL.SaveGlobalSettingsSafe();
        }

        private void OnQuickMenuOpacityChanged(float value)
        {
            int rounded = Mathf.Clamp(Mathf.RoundToInt(value), 1, 100);
            GodhomeQoL.GlobalSettings.QuickMenuOpacity = rounded;
            GodhomeQoL.SaveGlobalSettingsSafe();
            UpdateFloatValueText(quickMenuOpacityValue, rounded, 0);
            if (quickMenuOpacitySlider != null && Math.Abs(quickMenuOpacitySlider.value - rounded) > 0.0001f)
            {
                quickMenuOpacitySlider.value = rounded;
            }

            ApplyQuickMenuOpacity();
        }

        private void OnQuickFreeMenuClicked()
        {
            bool newValue = !IsQuickMenuFreeLayoutEnabled();
            SetQuickMenuFreeLayoutEnabled(newValue);
            ApplyQuickMenuLayout();
            UpdateFreeMenuLabel();
        }

        private void OnQuickRenameModeClicked()
        {
            quickRenameMode = !quickRenameMode;
            if (!quickRenameMode && renameField != null)
            {
                CommitQuickMenuRename(renameField.text);
            }

            UpdateRenameModeLabel();
        }

        private void OnQuickSettingsClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetBindingsVisible(false);
            SetZoteHelperVisible(false);
            SetQuickSettingsVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickFastSuperDashClicked()
        {
            returnToQuickOnClose = true;
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetBindingsVisible(false);
            SetZoteHelperVisible(false);
            SetOverlayVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickCollectorPhasesClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetBindingsVisible(false);
            SetZoteHelperVisible(false);
            SetCollectorVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickFastReloadClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetBindingsVisible(false);
            SetZoteHelperVisible(false);
            SetFastReloadVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickDreamshieldClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetShowHpOnDeathVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetBindingsVisible(false);
            SetZoteHelperVisible(false);
            SetDreamshieldVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickShowHpOnDeathClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetBindingsVisible(false);
            SetZoteHelperVisible(false);
            SetShowHpOnDeathVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickSpeedChangerClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetBindingsVisible(false);
            SetZoteHelperVisible(false);
            SetSpeedChangerVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickTeleportKitClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetSpeedChangerVisible(false);
            SetBossChallengeVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetBindingsVisible(false);
            SetZoteHelperVisible(false);
            SetTeleportKitVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickBossChallengeClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetBindingsVisible(false);
            SetZoteHelperVisible(false);
            SetBossChallengeVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickZoteHelperClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetBindingsVisible(false);
            SetZoteHelperVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickBossAnimationClicked()
        {
            returnToQuickOnClose = true;
            returnToQolOnClose = false;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBindingsVisible(false);
            SetZoteHelperVisible(false);
            SetBossAnimationVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickMenuAnimationClicked()
        {
            returnToQuickOnClose = true;
            returnToQolOnClose = false;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetQolVisible(false);
            SetBossAnimationVisible(false);
            SetBindingsVisible(false);
            SetZoteHelperVisible(false);
            SetMenuAnimationVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickQolClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetBindingsVisible(false);
            SetZoteHelperVisible(false);
            returnToQolOnClose = false;
            SetQolVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickBindingsMenuClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetBindingsVisible(true);
            SetQuickVisible(false);
        }

        private void OnOverlayBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetOverlayVisible(false);
        }

        private void OnCollectorBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetCollectorVisible(false);
        }

        private void OnFastReloadBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetFastReloadVisible(false);
        }

        private void OnFastReloadResetDefaultsClicked()
        {
            SetFastReloadEnabled(false);
            waitingForReloadRebind = false;
            waitingForTeleportRebind = false;
            Modules.FastReload.reloadKeyCode = (int)KeyCode.None;
            Modules.FastReload.teleportKeyCode = (int)KeyCode.None;
            RefreshFastReloadUi();
        }

        private void OnDreamshieldBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetDreamshieldVisible(false);
        }

        private void OnDreamshieldResetDefaultsClicked()
        {
            Modules.QoL.DreamshieldStartAngle.startAngleEnabled = false;
            Modules.QoL.DreamshieldStartAngle.rotationDelay = 0f;
            Modules.QoL.DreamshieldStartAngle.rotationSpeed = 1f;
            RefreshDreamshieldUi();
        }

        private void OnShowHpOnDeathBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetShowHpOnDeathVisible(false);
        }

        private void OnShowHpOnDeathResetDefaultsClicked()
        {
            waitingForShowHpRebind = false;
            showHpPrevBindingRaw = string.Empty;
            ShowHpSettings.EnabledMod = false;
            ShowHpSettings.ShowPB = false;
            ShowHpSettings.HideAfter10Sec = false;
            ShowHpSettings.HudFadeSeconds = 5f;
            ApplyShowHpBinding(null);
            RefreshShowHpOnDeathUi();
        }

        private void OnSpeedChangerBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetSpeedChangerVisible(false);
        }

        private void OnSpeedChangerResetDefaultsClicked()
        {
            waitingForSpeedToggleRebind = false;
            waitingForSpeedInputRebind = false;
            waitingForSpeedDownRebind = false;
            waitingForSpeedUpRebind = false;
            speedTogglePrevKey = string.Empty;
            speedInputPrevKey = string.Empty;
            speedDownPrevKey = string.Empty;
            speedUpPrevKey = string.Empty;

            SetSpeedChangerGlobalSwitch(false);
            SpeedChanger.lockSwitch = false;
            SpeedChanger.restrictToggleToRooms = false;
            SpeedChanger.unlimitedSpeed = false;
            SpeedChanger.displayStyle = 0;
            SpeedChanger.toggleKeybind = string.Empty;
            SpeedChanger.inputSpeedKeybind = string.Empty;
            SpeedChanger.slowDownKeybind = string.Empty;
            SpeedChanger.speedUpKeybind = string.Empty;
            SpeedChanger.speed = 1f;
            ApplySpeedChangerDisplayStyle(0);
            RefreshSpeedChangerKeybinds();
            RefreshSpeedChangerUi();
        }

        private void OnTeleportKitBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetTeleportKitVisible(false);
        }

        private void OnTeleportKitResetDefaultsClicked()
        {
            waitingForTeleportKitMenuRebind = false;
            waitingForTeleportKitSaveRebind = false;
            waitingForTeleportKitTeleportRebind = false;
            SetTeleportKitEnabled(false);
            Modules.QoL.TeleportKit.MenuHotkey = KeyCode.F6;
            Modules.QoL.TeleportKit.SaveTeleportHotkey = KeyCode.R;
            Modules.QoL.TeleportKit.TeleportHotkey = KeyCode.T;
            RefreshTeleportKitUi();
        }

        private void OnBossChallengeBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetBossChallengeVisible(false);
        }

        private void OnBossChallengeResetDefaultsClicked()
        {
            SetInfiniteChallengeEnabled(false);
            SetCarefreeMelodyEnabled(false);
            SetInfiniteGrimmPufferfishEnabled(false);
            SetInfiniteRadianceClimbingEnabled(false);
            SetP5HealthEnabled(false);
            SetSegmentedP5Enabled(false);
            SetHalveAscendedEnabled(false);
            SetHalveAttunedEnabled(false);
            SetAddLifebloodEnabled(false);
            SetAddSoulEnabled(false);

            Modules.BossChallenge.InfiniteChallenge.restartFightOnSuccess = false;
            Modules.BossChallenge.InfiniteChallenge.restartFightAndMusic = false;
            Modules.BossChallenge.AddLifeblood.lifebloodAmount = 0;
            Modules.BossChallenge.AddSoul.soulAmount = 0;
            Modules.BossChallenge.SegmentedP5.selectedP5Segment = 0;
            RefreshBossChallengeUi();
        }

        private void OnQolBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetQolVisible(false);
        }

        private void OnQolResetDefaultsClicked()
        {
            waitingForFastDreamWarpRebind = false;
            fastDreamWarpPrevKeyRaw = string.Empty;
            fastDreamWarpPrevButton = default;

            SetFastDreamWarpEnabled(false);
            FastDreamWarpSettings.Keybinds.Toggle.ClearBindings();
            SetShortDeathAnimationEnabled(true);
            Modules.QoL.SkipCutscenes.HallOfGodsStatues = true;
            SetUnlockAllModesEnabled(true);
            SetUnlockPantheonsEnabled(true);
            SetUnlockRadianceEnabled(true);
            SetUnlockRadiantEnabled(true);

            RefreshQolUi();
        }

        private void OnMenuAnimationBackClicked()
        {
            bool reopenQol = returnToQolOnClose;
            returnToQolOnClose = false;

            if (reopenQol)
            {
                SetQolVisible(true);
            }
            else
            {
                bool reopenQuick = returnToQuickOnClose;
                returnToQuickOnClose = false;
                if (reopenQuick)
                {
                    SetQuickVisible(true);
                }
            }

            SetMenuAnimationVisible(false);
        }

        private void OnMenuAnimationResetDefaultsClicked()
        {
            SetDoorDefaultBeginEnabled(true);
            SetMemorizeBindingsEnabled(true);
            SetFasterLoadsEnabled(true);
            SetFastMenusEnabled(true);
            SetFastTextEnabled(true);
            Modules.QoL.SkipCutscenes.AutoSkipCinematics = true;
            Modules.QoL.SkipCutscenes.AllowSkippingNonskippable = true;
            Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt = true;
            RefreshMenuAnimationUi();
        }

        private void OnBossAnimationBackClicked()
        {
            bool reopenQol = returnToQolOnClose;
            returnToQolOnClose = false;

            if (reopenQol)
            {
                SetQolVisible(true);
            }
            else
            {
                bool reopenQuick = returnToQuickOnClose;
                returnToQuickOnClose = false;
                if (reopenQuick)
                {
                    SetQuickVisible(true);
                }
            }

            SetBossAnimationVisible(false);
        }

        private void OnBossAnimationResetDefaultsClicked()
        {
            Modules.QoL.SkipCutscenes.AbsoluteRadiance = true;
            Modules.QoL.SkipCutscenes.PureVesselRoar = true;
            Modules.QoL.SkipCutscenes.GrimmNightmare = true;
            Modules.QoL.SkipCutscenes.GreyPrinceZote = true;
            Modules.QoL.SkipCutscenes.Collector = true;
            Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip = true;
            Modules.QoL.SkipCutscenes.FirstTimeBosses = true;
            RefreshBossAnimationUi();
        }

        private void OnBindingsBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetBindingsVisible(false);
        }

        private void OnBindingsResetDefaultsClicked()
        {
            waitingForBindingsMenuRebind = false;
            bindingsPrevKey = KeyCode.None;

            SetBindingsEverywhereEnabled(false);
            GodhomeQoL.GlobalSettings.BindingsMenuHotkey = string.Empty;
            GodhomeQoL.SaveGlobalSettingsSafe();

            RefreshBindingsUi();
        }

        private void OnQuickMenuSettingsBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetQuickSettingsVisible(false);
        }

        private void OnZoteHelperBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetZoteHelperVisible(false);
        }

        private void OnZoteHelperResetDefaultsClicked()
        {
            bool wasEnabled = GetZoteHelperEnabled();
            ResetZoteHelperDefaults();
            Modules.BossChallenge.ForceGreyPrinceEnterType.gpzEnterType =
                Modules.BossChallenge.ForceGreyPrinceEnterType.EnterType.Off;
            ApplyBossGpzOption(0);

            if (wasEnabled)
            {
                Modules.BossChallenge.ZoteHelper.ApplyBossHealthIfPresent();
                Modules.BossChallenge.ZoteHelper.ApplyZotelingHealthIfPresent();
            }

            SetZoteHelperEnabled(false);
            RefreshZoteHelperUi();
        }

        private void OnFastSuperDashResetDefaultsClicked()
        {
            SetModuleEnabled(false);
            Modules.QoL.FastSuperDash.instantSuperDash = false;
            Modules.QoL.FastSuperDash.fastSuperDashEverywhere = false;
            Modules.QoL.FastSuperDash.fastSuperDashSpeedMultiplier = 1f;
            RefreshFastSuperDashUi();
        }

        private void OnCollectorResetClicked()
        {
            ResetCollectorPhasesDefaults();
            Modules.CollectorPhases.CollectorPhases.HoGOnly = true;
            SetCollectorRoarEnabled(false);
            SetCollectorPhasesEnabled(false);
            RefreshCollectorPhasesUi();
        }

        private bool IsFastReloadRebinding()
        {
            return waitingForReloadRebind || waitingForTeleportRebind;
        }

        private bool IsAnyRebinding()
        {
            return IsFastReloadRebinding()
                || IsShowHpOnDeathRebinding()
                || IsSpeedChangerRebinding()
                || IsTeleportKitRebinding()
                || IsFastDreamWarpRebinding()
                || renameField != null
                || waitingForQuickMenuHotkeyRebind
                || waitingForBindingsMenuRebind;
        }

        internal void StartQuickMenuHotkeyRebind()
        {
            waitingForQuickMenuHotkeyRebind = true;
            quickMenuPrevKey = GetQuickMenuToggleKey();
            UpdateQuickMenuHotkeyButton("Settings/FastReload/SetKey".Localize());
        }

        private void StartReloadRebind()
        {
            waitingForReloadRebind = true;
            reloadPrevKey = GetReloadKey();
            UpdateKeybindValue(reloadKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void StartTeleportRebind()
        {
            waitingForTeleportRebind = true;
            teleportPrevKey = GetTeleportKey();
            UpdateKeybindValue(teleportKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void CancelFastReloadRebind()
        {
            if (waitingForReloadRebind)
            {
                waitingForReloadRebind = false;
                UpdateKeybindValue(reloadKeyValue, FormatKeyLabel(GetReloadKey()));
            }

            if (waitingForTeleportRebind)
            {
                waitingForTeleportRebind = false;
                UpdateKeybindValue(teleportKeyValue, FormatKeyLabel(GetTeleportKey()));
            }
        }

        private void HandleQuickMenuHotkeyRebind()
        {
            if (!waitingForQuickMenuHotkeyRebind)
            {
                return;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key) || key == KeyCode.None)
                {
                    continue;
                }

                if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
                {
                    continue;
                }

                ApplyQuickMenuHotkeyRebind(key);
                return;
            }
        }

        private void ApplyQuickMenuHotkeyRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForQuickMenuHotkeyRebind = false;
                UpdateQuickMenuHotkeyButton(GetQuickMenuHotkeyLabel());
                return;
            }

            GodhomeQoL.GlobalSettings.QuickMenuHotkey = key == quickMenuPrevKey ? string.Empty : key.ToString();
            GodhomeQoL.SaveGlobalSettingsSafe();
            GodhomeQoL.MarkMenuDirty();

            waitingForQuickMenuHotkeyRebind = false;
            UpdateQuickMenuHotkeyButton(GetQuickMenuHotkeyLabel());
        }

        private void StartBindingsMenuHotkeyRebind()
        {
            waitingForBindingsMenuRebind = true;
            bindingsPrevKey = GetBindingsMenuToggleKey();
            UpdateKeybindValue(bindingsHotkeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void CancelBindingsMenuRebind()
        {
            if (!waitingForBindingsMenuRebind)
            {
                return;
            }

            waitingForBindingsMenuRebind = false;
            UpdateKeybindValue(bindingsHotkeyValue, GetBindingsMenuHotkeyLabel());
        }

        private void HandleBindingsMenuRebind()
        {
            if (!waitingForBindingsMenuRebind)
            {
                return;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key) || key == KeyCode.None)
                {
                    continue;
                }

                if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
                {
                    continue;
                }

                ApplyBindingsMenuHotkeyRebind(key);
                return;
            }
        }

        private void ApplyBindingsMenuHotkeyRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForBindingsMenuRebind = false;
                UpdateKeybindValue(bindingsHotkeyValue, GetBindingsMenuHotkeyLabel());
                return;
            }

            GodhomeQoL.GlobalSettings.BindingsMenuHotkey = key == bindingsPrevKey ? string.Empty : key.ToString();
            GodhomeQoL.SaveGlobalSettingsSafe();

            waitingForBindingsMenuRebind = false;
            UpdateKeybindValue(bindingsHotkeyValue, GetBindingsMenuHotkeyLabel());
        }

        private void HandleFastReloadRebind()
        {
            if (!IsFastReloadRebinding())
            {
                return;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key) || key == KeyCode.None)
                {
                    continue;
                }

                if (key == KeyCode.Mouse0)
                {
                    continue;
                }

                if (waitingForReloadRebind)
                {
                    ApplyReloadRebind(key);
                    return;
                }

                if (waitingForTeleportRebind)
                {
                    ApplyTeleportRebind(key);
                    return;
                }
            }
        }

        private void ApplyReloadRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForReloadRebind = false;
                UpdateKeybindValue(reloadKeyValue, FormatKeyLabel(GetReloadKey()));
                return;
            }

            Modules.FastReload.reloadKeyCode = key == reloadPrevKey || IsFastReloadKeyInUse(key, forReload: true)
                ? (int)KeyCode.None
                : (int)key;

            waitingForReloadRebind = false;
            UpdateKeybindValue(reloadKeyValue, FormatKeyLabel(GetReloadKey()));
        }

        private void ApplyTeleportRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForTeleportRebind = false;
                UpdateKeybindValue(teleportKeyValue, FormatKeyLabel(GetTeleportKey()));
                return;
            }

            Modules.FastReload.teleportKeyCode = key == teleportPrevKey || IsFastReloadKeyInUse(key, forReload: false)
                ? (int)KeyCode.None
                : (int)key;

            waitingForTeleportRebind = false;
            UpdateKeybindValue(teleportKeyValue, FormatKeyLabel(GetTeleportKey()));
        }

        private static bool IsFastReloadKeyInUse(KeyCode key, bool forReload)
        {
            if (key == KeyCode.None)
            {
                return false;
            }

            KeyCode other = forReload ? GetTeleportKey() : GetReloadKey();
            return key == other;
        }

        private static KeyCode GetReloadKey() => (KeyCode)Modules.FastReload.reloadKeyCode;

        private static KeyCode GetTeleportKey() => (KeyCode)Modules.FastReload.teleportKeyCode;

        private static string FormatKeyLabel(KeyCode key)
        {
            return key == KeyCode.None
                ? "Settings/FastReload/NotSet".Localize()
                : key.ToString();
        }

        private static bool GetBindingsEverywhereEnabled()
        {
            return GodhomeQoL.GlobalSettings.BindingsMenuEverywhere;
        }

        private static void SetBindingsEverywhereEnabled(bool value)
        {
            GodhomeQoL.GlobalSettings.BindingsMenuEverywhere = value;
            GodhomeQoL.SaveGlobalSettingsSafe();
        }

        private static KeyCode GetBindingsMenuToggleKey()
        {
            string? raw = GodhomeQoL.GlobalSettings.BindingsMenuHotkey;
            if (string.IsNullOrWhiteSpace(raw) || raw.Equals("Not Set", StringComparison.OrdinalIgnoreCase))
            {
                return KeyCode.None;
            }

            return Enum.TryParse(raw, true, out KeyCode key) ? key : KeyCode.None;
        }

        private static string GetBindingsMenuHotkeyLabel()
        {
            return FormatKeyLabel(GetBindingsMenuToggleKey());
        }

        private void UpdateKeybindValue(Text? text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        private bool IsShowHpOnDeathRebinding()
        {
            return waitingForShowHpRebind;
        }

        private void StartShowHpOnDeathRebind()
        {
            waitingForShowHpRebind = true;
            showHpPrevBindingRaw = GetShowHpBindingRaw();
            UpdateKeybindValue(showHpHudToggleKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void CancelShowHpOnDeathRebind()
        {
            if (!waitingForShowHpRebind)
            {
                return;
            }

            waitingForShowHpRebind = false;
            UpdateKeybindValue(showHpHudToggleKeyValue, GetShowHpBindingLabel());
        }

        private void HandleShowHpOnDeathRebind()
        {
            if (!waitingForShowHpRebind)
            {
                return;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key) || key == KeyCode.None)
                {
                    continue;
                }

                if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
                {
                    continue;
                }

                ApplyShowHpOnDeathRebind(key);
                return;
            }
        }

        private void ApplyShowHpOnDeathRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForShowHpRebind = false;
                UpdateKeybindValue(showHpHudToggleKeyValue, GetShowHpBindingLabel());
                return;
            }

            if (!TryMapKeyCodeToInControlKey(key, out Key mapped))
            {
                return;
            }

            string newLabel = mapped.ToString();
            bool clear = !string.IsNullOrEmpty(showHpPrevBindingRaw)
                && string.Equals(showHpPrevBindingRaw, newLabel, StringComparison.Ordinal);

            ApplyShowHpBinding(clear ? (Key?)null : mapped);
            waitingForShowHpRebind = false;
            UpdateKeybindValue(showHpHudToggleKeyValue, GetShowHpBindingLabel());
        }

        private static void ApplyShowHpBinding(Key? key)
        {
            PlayerAction action = ShowHpSettings.Keybinds.Hide;
            InputControlType controllerBinding = KeybindUtil.GetControllerButtonBinding(action);

            action.ClearBindings();

            if (!EqualityComparer<InputControlType>.Default.Equals(controllerBinding, default))
            {
                KeybindUtil.AddInputControlType(action, controllerBinding);
            }

            if (key.HasValue && !EqualityComparer<Key>.Default.Equals(key.Value, default))
            {
                KeybindUtil.AddKeyOrMouseBinding(action, new InputHandler.KeyOrMouseBinding(key.Value));
            }
        }

        private string GetShowHpBindingLabel()
        {
            string raw = GetShowHpBindingRaw();
            return string.IsNullOrEmpty(raw) ? "Settings/FastReload/NotSet".Localize() : raw;
        }

        private static string GetShowHpBindingRaw()
        {
            InputHandler.KeyOrMouseBinding binding = KeybindUtil.GetKeyOrMouseBinding(ShowHpSettings.Keybinds.Hide);
            return FormatKeyOrMouseBinding(binding);
        }

        private static string FormatKeyOrMouseBinding(InputHandler.KeyOrMouseBinding binding)
        {
            if (TryGetBindingKey(binding, out Key key)
                && !EqualityComparer<Key>.Default.Equals(key, default))
            {
                return key.ToString();
            }

            if (TryGetBindingMouse(binding, out Mouse mouse)
                && !EqualityComparer<Mouse>.Default.Equals(mouse, default))
            {
                return mouse.ToString();
            }

            return string.Empty;
        }

        private static bool TryGetBindingKey(InputHandler.KeyOrMouseBinding binding, out Key key)
        {
            object boxed = binding;
            Type type = boxed.GetType();

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.PropertyType == typeof(Key))
                {
                    key = (Key)property.GetValue(boxed);
                    return true;
                }
            }

            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (field.FieldType == typeof(Key))
                {
                    key = (Key)field.GetValue(boxed);
                    return true;
                }
            }

            key = default;
            return false;
        }

        private static bool TryGetBindingMouse(InputHandler.KeyOrMouseBinding binding, out Mouse mouse)
        {
            object boxed = binding;
            Type type = boxed.GetType();

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.PropertyType == typeof(Mouse))
                {
                    mouse = (Mouse)property.GetValue(boxed);
                    return true;
                }
            }

            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (field.FieldType == typeof(Mouse))
                {
                    mouse = (Mouse)field.GetValue(boxed);
                    return true;
                }
            }

            mouse = default;
            return false;
        }

        private static bool TryMapKeyCodeToInControlKey(KeyCode keyCode, out Key key)
        {
            key = default;
            if (keyCode == KeyCode.None)
            {
                return false;
            }

            if (keyCode >= KeyCode.Alpha0 && keyCode <= KeyCode.Alpha9)
            {
                int digit = (int)keyCode - (int)KeyCode.Alpha0;
                return Enum.TryParse($"Key{digit}", out key);
            }

            if (keyCode >= KeyCode.Keypad0 && keyCode <= KeyCode.Keypad9)
            {
                int digit = (int)keyCode - (int)KeyCode.Keypad0;
                return Enum.TryParse($"Pad{digit}", out key);
            }

            if (keyCode == KeyCode.KeypadEnter)
            {
                if (Enum.TryParse("PadEnter", out key))
                {
                    return true;
                }

                return Enum.TryParse("KeypadEnter", out key);
            }

            return Enum.TryParse(keyCode.ToString(), out key);
        }

        private bool IsSpeedChangerRebinding()
        {
            return waitingForSpeedToggleRebind || waitingForSpeedInputRebind || waitingForSpeedUpRebind || waitingForSpeedDownRebind;
        }

        private void StartSpeedChangerToggleRebind()
        {
            waitingForSpeedToggleRebind = true;
            speedTogglePrevKey = SpeedChanger.toggleKeybind;
            UpdateKeybindValue(speedChangerToggleKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void StartSpeedChangerInputRebind()
        {
            waitingForSpeedInputRebind = true;
            speedInputPrevKey = SpeedChanger.inputSpeedKeybind;
            UpdateKeybindValue(speedChangerInputKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void StartSpeedChangerSpeedDownRebind()
        {
            waitingForSpeedDownRebind = true;
            speedDownPrevKey = SpeedChanger.slowDownKeybind;
            UpdateKeybindValue(speedChangerSpeedDownKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void StartSpeedChangerSpeedUpRebind()
        {
            waitingForSpeedUpRebind = true;
            speedUpPrevKey = SpeedChanger.speedUpKeybind;
            UpdateKeybindValue(speedChangerSpeedUpKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void CancelSpeedChangerRebind()
        {
            if (waitingForSpeedToggleRebind)
            {
                waitingForSpeedToggleRebind = false;
                UpdateKeybindValue(speedChangerToggleKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.toggleKeybind));
            }

            if (waitingForSpeedInputRebind)
            {
                waitingForSpeedInputRebind = false;
                UpdateKeybindValue(speedChangerInputKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.inputSpeedKeybind));
            }

            if (waitingForSpeedDownRebind)
            {
                waitingForSpeedDownRebind = false;
                UpdateKeybindValue(speedChangerSpeedDownKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.slowDownKeybind));
            }

            if (waitingForSpeedUpRebind)
            {
                waitingForSpeedUpRebind = false;
                UpdateKeybindValue(speedChangerSpeedUpKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.speedUpKeybind));
            }
        }

        private void HandleSpeedChangerRebind()
        {
            if (!IsSpeedChangerRebinding())
            {
                return;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key) || key == KeyCode.None)
                {
                    continue;
                }

                if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
                {
                    continue;
                }

                if (waitingForSpeedToggleRebind)
                {
                    ApplySpeedChangerToggleRebind(key);
                    return;
                }

                if (waitingForSpeedInputRebind)
                {
                    ApplySpeedChangerInputRebind(key);
                    return;
                }

                if (waitingForSpeedDownRebind)
                {
                    ApplySpeedChangerSpeedDownRebind(key);
                    return;
                }

                if (waitingForSpeedUpRebind)
                {
                    ApplySpeedChangerSpeedUpRebind(key);
                    return;
                }
            }
        }

        private void ApplySpeedChangerToggleRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForSpeedToggleRebind = false;
                UpdateKeybindValue(speedChangerToggleKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.toggleKeybind));
                return;
            }

            SetSpeedChangerKeybind(key, speedTogglePrevKey, "toggle", value => SpeedChanger.toggleKeybind = value);
            waitingForSpeedToggleRebind = false;
            UpdateKeybindValue(speedChangerToggleKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.toggleKeybind));
        }

        private void ApplySpeedChangerInputRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForSpeedInputRebind = false;
                UpdateKeybindValue(speedChangerInputKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.inputSpeedKeybind));
                return;
            }

            SetSpeedChangerKeybind(key, speedInputPrevKey, "input", value => SpeedChanger.inputSpeedKeybind = value);
            waitingForSpeedInputRebind = false;
            UpdateKeybindValue(speedChangerInputKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.inputSpeedKeybind));
        }

        private void ApplySpeedChangerSpeedDownRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForSpeedDownRebind = false;
                UpdateKeybindValue(speedChangerSpeedDownKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.slowDownKeybind));
                return;
            }

            SetSpeedChangerKeybind(key, speedDownPrevKey, "down", value => SpeedChanger.slowDownKeybind = value);
            waitingForSpeedDownRebind = false;
            UpdateKeybindValue(speedChangerSpeedDownKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.slowDownKeybind));
        }

        private void ApplySpeedChangerSpeedUpRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForSpeedUpRebind = false;
                UpdateKeybindValue(speedChangerSpeedUpKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.speedUpKeybind));
                return;
            }

            SetSpeedChangerKeybind(key, speedUpPrevKey, "up", value => SpeedChanger.speedUpKeybind = value);
            waitingForSpeedUpRebind = false;
            UpdateKeybindValue(speedChangerSpeedUpKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.speedUpKeybind));
        }

        private void SetSpeedChangerKeybind(KeyCode key, string previous, string slot, Action<string> setter)
        {
            string keyName = key.ToString();
            bool clear = string.Equals(previous, keyName, StringComparison.OrdinalIgnoreCase)
                || IsSpeedChangerKeyInUse(keyName, slot);

            setter(clear ? string.Empty : keyName);
            RefreshSpeedChangerKeybinds();
            GodhomeQoL.SaveGlobalSettingsSafe();
        }

        private static bool IsSpeedChangerKeyInUse(string keyName, string except)
        {
            if (string.IsNullOrWhiteSpace(keyName))
            {
                return false;
            }

            bool inToggle = !string.Equals(except, "toggle", StringComparison.OrdinalIgnoreCase)
                && string.Equals(SpeedChanger.toggleKeybind, keyName, StringComparison.OrdinalIgnoreCase);
            bool inInput = !string.Equals(except, "input", StringComparison.OrdinalIgnoreCase)
                && string.Equals(SpeedChanger.inputSpeedKeybind, keyName, StringComparison.OrdinalIgnoreCase);
            bool inUp = !string.Equals(except, "up", StringComparison.OrdinalIgnoreCase)
                && string.Equals(SpeedChanger.speedUpKeybind, keyName, StringComparison.OrdinalIgnoreCase);
            bool inDown = !string.Equals(except, "down", StringComparison.OrdinalIgnoreCase)
                && string.Equals(SpeedChanger.slowDownKeybind, keyName, StringComparison.OrdinalIgnoreCase);

            return inToggle || inInput || inUp || inDown;
        }

        private void RefreshSpeedChangerKeybinds()
        {
            SpeedChanger? module = GetSpeedChangerModule();
            if (module == null)
            {
                return;
            }

            MethodInfo? method = typeof(SpeedChanger).GetMethod("RefreshKeybinds", BindingFlags.Instance | BindingFlags.NonPublic);
            method?.Invoke(module, null);
        }

        private static string FormatSpeedChangerKeyLabel(string storedKey)
        {
            if (string.IsNullOrWhiteSpace(storedKey) || storedKey.Equals("Not Set", StringComparison.OrdinalIgnoreCase))
            {
                return "SpeedChanger/NotSet".Localize();
            }

            return storedKey;
        }

        private static string GetSpeedChangerDisplayValue()
        {
            int index = ClampOptionIndex(SpeedChanger.displayStyle, SpeedChangerDisplayOptions.Length);
            return SpeedChangerDisplayOptions[index];
        }

        private bool IsTeleportKitRebinding()
        {
            return waitingForTeleportKitMenuRebind || waitingForTeleportKitSaveRebind || waitingForTeleportKitTeleportRebind;
        }

        private void StartTeleportKitMenuRebind()
        {
            waitingForTeleportKitMenuRebind = true;
            UpdateKeybindValue(teleportKitMenuKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void StartTeleportKitSaveRebind()
        {
            waitingForTeleportKitSaveRebind = true;
            UpdateKeybindValue(teleportKitSaveKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void StartTeleportKitTeleportRebind()
        {
            waitingForTeleportKitTeleportRebind = true;
            UpdateKeybindValue(teleportKitTeleportKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void CancelTeleportKitRebind()
        {
            if (waitingForTeleportKitMenuRebind)
            {
                waitingForTeleportKitMenuRebind = false;
                UpdateKeybindValue(teleportKitMenuKeyValue, GetTeleportKitMenuKeyLabel());
            }

            if (waitingForTeleportKitSaveRebind)
            {
                waitingForTeleportKitSaveRebind = false;
                UpdateKeybindValue(teleportKitSaveKeyValue, GetTeleportKitSaveKeyLabel());
            }

            if (waitingForTeleportKitTeleportRebind)
            {
                waitingForTeleportKitTeleportRebind = false;
                UpdateKeybindValue(teleportKitTeleportKeyValue, GetTeleportKitTeleportKeyLabel());
            }
        }

        private void HandleTeleportKitRebind()
        {
            if (!IsTeleportKitRebinding())
            {
                return;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key) || key == KeyCode.None)
                {
                    continue;
                }

                if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
                {
                    continue;
                }

                if (waitingForTeleportKitMenuRebind)
                {
                    ApplyTeleportKitMenuRebind(key);
                    return;
                }

                if (waitingForTeleportKitSaveRebind)
                {
                    ApplyTeleportKitSaveRebind(key);
                    return;
                }

                if (waitingForTeleportKitTeleportRebind)
                {
                    ApplyTeleportKitTeleportRebind(key);
                    return;
                }
            }
        }

        private void ApplyTeleportKitMenuRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForTeleportKitMenuRebind = false;
                UpdateKeybindValue(teleportKitMenuKeyValue, GetTeleportKitMenuKeyLabel());
                return;
            }

            Modules.QoL.TeleportKit.MenuHotkey = key;
            waitingForTeleportKitMenuRebind = false;
            UpdateKeybindValue(teleportKitMenuKeyValue, GetTeleportKitMenuKeyLabel());
        }

        private void ApplyTeleportKitSaveRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForTeleportKitSaveRebind = false;
                UpdateKeybindValue(teleportKitSaveKeyValue, GetTeleportKitSaveKeyLabel());
                return;
            }

            Modules.QoL.TeleportKit.SaveTeleportHotkey = key;
            waitingForTeleportKitSaveRebind = false;
            UpdateKeybindValue(teleportKitSaveKeyValue, GetTeleportKitSaveKeyLabel());
        }

        private void ApplyTeleportKitTeleportRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForTeleportKitTeleportRebind = false;
                UpdateKeybindValue(teleportKitTeleportKeyValue, GetTeleportKitTeleportKeyLabel());
                return;
            }

            Modules.QoL.TeleportKit.TeleportHotkey = key;
            waitingForTeleportKitTeleportRebind = false;
            UpdateKeybindValue(teleportKitTeleportKeyValue, GetTeleportKitTeleportKeyLabel());
        }

        private bool IsFastDreamWarpRebinding()
        {
            return waitingForFastDreamWarpRebind;
        }

        private void StartFastDreamWarpRebind()
        {
            waitingForFastDreamWarpRebind = true;
            fastDreamWarpPrevKeyRaw = GetFastDreamWarpKeyBindingRaw();
            fastDreamWarpPrevButton = GetFastDreamWarpControllerBinding();
            UpdateKeybindValue(fastDreamWarpKeyValue, "Settings/FastReload/SetKey".Localize());
        }

        private void CancelFastDreamWarpRebind()
        {
            if (!waitingForFastDreamWarpRebind)
            {
                return;
            }

            waitingForFastDreamWarpRebind = false;
            UpdateKeybindValue(fastDreamWarpKeyValue, GetFastDreamWarpBindingLabel());
        }

        private void HandleFastDreamWarpRebind()
        {
            if (!waitingForFastDreamWarpRebind)
            {
                return;
            }

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key) || key == KeyCode.None)
                {
                    continue;
                }

                if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
                {
                    continue;
                }

                ApplyFastDreamWarpKeyRebind(key);
                return;
            }

            if (TryGetPressedControllerButton(out InputControlType button))
            {
                ApplyFastDreamWarpControllerRebind(button);
            }
        }

        private void ApplyFastDreamWarpKeyRebind(KeyCode key)
        {
            if (key == KeyCode.Escape)
            {
                waitingForFastDreamWarpRebind = false;
                UpdateKeybindValue(fastDreamWarpKeyValue, GetFastDreamWarpBindingLabel());
                return;
            }

            if (!TryMapKeyCodeToInControlKey(key, out Key mapped))
            {
                return;
            }

            string newLabel = mapped.ToString();
            bool clear = !string.IsNullOrEmpty(fastDreamWarpPrevKeyRaw)
                && string.Equals(fastDreamWarpPrevKeyRaw, newLabel, StringComparison.Ordinal);

            ApplyFastDreamWarpKeyBinding(clear ? (Key?)null : mapped);
            waitingForFastDreamWarpRebind = false;
            UpdateKeybindValue(fastDreamWarpKeyValue, GetFastDreamWarpBindingLabel());
        }

        private void ApplyFastDreamWarpControllerRebind(InputControlType button)
        {
            bool clear = !EqualityComparer<InputControlType>.Default.Equals(fastDreamWarpPrevButton, default)
                && EqualityComparer<InputControlType>.Default.Equals(fastDreamWarpPrevButton, button);

            ApplyFastDreamWarpControllerBinding(clear ? (InputControlType?)null : button);
            waitingForFastDreamWarpRebind = false;
            UpdateKeybindValue(fastDreamWarpKeyValue, GetFastDreamWarpBindingLabel());
        }

        private void HandleFastDreamWarpHotkey()
        {
            PlayerAction action = FastDreamWarpSettings.Keybinds.Toggle;
            if (!action.WasPressed)
            {
                return;
            }

            bool newValue = !GetFastDreamWarpEnabled();
            SetFastDreamWarpEnabled(newValue);
            UpdateToggleValue(bossAnimFastDreamWarpValue, newValue);
            string state = newValue ? "ON" : "OFF";
            string label = "Modules/FastDreamWarp".Localize();
            ShowStatusMessage($"{label}: {state}");
        }

        private static void ApplyFastDreamWarpKeyBinding(Key? key)
        {
            PlayerAction action = FastDreamWarpSettings.Keybinds.Toggle;
            InputControlType controllerBinding = KeybindUtil.GetControllerButtonBinding(action);

            action.ClearBindings();

            if (!EqualityComparer<InputControlType>.Default.Equals(controllerBinding, default))
            {
                KeybindUtil.AddInputControlType(action, controllerBinding);
            }

            if (key.HasValue && !EqualityComparer<Key>.Default.Equals(key.Value, default))
            {
                KeybindUtil.AddKeyOrMouseBinding(action, new InputHandler.KeyOrMouseBinding(key.Value));
            }
        }

        private static void ApplyFastDreamWarpControllerBinding(InputControlType? button)
        {
            PlayerAction action = FastDreamWarpSettings.Keybinds.Toggle;
            Key? key = GetFastDreamWarpKeyBinding();

            action.ClearBindings();

            if (button.HasValue && !EqualityComparer<InputControlType>.Default.Equals(button.Value, default))
            {
                KeybindUtil.AddInputControlType(action, button.Value);
            }

            if (key.HasValue && !EqualityComparer<Key>.Default.Equals(key.Value, default))
            {
                KeybindUtil.AddKeyOrMouseBinding(action, new InputHandler.KeyOrMouseBinding(key.Value));
            }
        }

        private string GetFastDreamWarpBindingLabel()
        {
            string keyLabel = GetFastDreamWarpKeyBindingRaw();
            string controllerLabel = GetFastDreamWarpControllerBindingLabel();

            if (string.IsNullOrEmpty(keyLabel) && string.IsNullOrEmpty(controllerLabel))
            {
                return "Settings/FastReload/NotSet".Localize();
            }

            if (string.IsNullOrEmpty(keyLabel))
            {
                return controllerLabel;
            }

            if (string.IsNullOrEmpty(controllerLabel))
            {
                return keyLabel;
            }

            return $"{keyLabel} / {controllerLabel}";
        }

        private static string GetFastDreamWarpKeyBindingRaw()
        {
            InputHandler.KeyOrMouseBinding binding = KeybindUtil.GetKeyOrMouseBinding(FastDreamWarpSettings.Keybinds.Toggle);
            return FormatKeyOrMouseBinding(binding);
        }

        private static Key? GetFastDreamWarpKeyBinding()
        {
            InputHandler.KeyOrMouseBinding binding = KeybindUtil.GetKeyOrMouseBinding(FastDreamWarpSettings.Keybinds.Toggle);
            if (TryGetBindingKey(binding, out Key key) && !EqualityComparer<Key>.Default.Equals(key, default))
            {
                return key;
            }

            return null;
        }

        private static InputControlType GetFastDreamWarpControllerBinding()
        {
            return KeybindUtil.GetControllerButtonBinding(FastDreamWarpSettings.Keybinds.Toggle);
        }

        private static string GetFastDreamWarpControllerBindingLabel()
        {
            InputControlType binding = GetFastDreamWarpControllerBinding();
            return EqualityComparer<InputControlType>.Default.Equals(binding, default)
                ? string.Empty
                : binding.ToString();
        }

        private static bool TryGetPressedControllerButton(out InputControlType button)
        {
            button = default;

            foreach (InputDevice device in InputManager.Devices)
            {
                if (device == null || !device.AnyButtonWasPressed)
                {
                    continue;
                }

                foreach (InputControl control in device.Controls)
                {
                    if (control != null && control.IsButton && control.WasPressed)
                    {
                        button = control.Target;
                        return true;
                    }
                }
            }

            return false;
        }

        private static string GetTeleportKitMenuKeyLabel()
        {
            return FormatTeleportKitKeyLabel(Modules.QoL.TeleportKit.MenuHotkey);
        }

        private static string GetTeleportKitSaveKeyLabel()
        {
            return FormatTeleportKitKeyLabel(GetTeleportKitSaveKeyDisplay());
        }

        private static string GetTeleportKitTeleportKeyLabel()
        {
            return FormatTeleportKitKeyLabel(GetTeleportKitTeleportKeyDisplay());
        }

        private static KeyCode GetTeleportKitSaveKeyDisplay()
        {
            return Modules.QoL.TeleportKit.SaveTeleportHotkey == KeyCode.None
                ? KeyCode.R
                : Modules.QoL.TeleportKit.SaveTeleportHotkey;
        }

        private static KeyCode GetTeleportKitTeleportKeyDisplay()
        {
            return Modules.QoL.TeleportKit.TeleportHotkey == KeyCode.None
                ? KeyCode.T
                : Modules.QoL.TeleportKit.TeleportHotkey;
        }

        private static string FormatTeleportKitKeyLabel(KeyCode key)
        {
            return key == KeyCode.None
                ? "Settings/FastReload/NotSet".Localize()
                : key.ToString();
        }

        private void EnableUiInteraction()
        {
            prevCursorVisible = Cursor.visible;
            prevCursorLock = Cursor.lockState;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (UIManager.instance?.inputModule != null)
            {
                prevAllowMouseInput = UIManager.instance.inputModule.allowMouseInput;
                prevForceModuleActive = UIManager.instance.inputModule.forceModuleActive;
                UIManager.instance.inputModule.allowMouseInput = true;
                UIManager.instance.inputModule.forceModuleActive = true;
                UIManager.instance.inputModule.ActivateModule();
            }

        }

        private void RestoreUiInteraction()
        {
            if (UIManager.instance?.inputModule != null)
            {
                UIManager.instance.inputModule.allowMouseInput = prevAllowMouseInput;
                UIManager.instance.inputModule.forceModuleActive = prevForceModuleActive;
                if (!prevForceModuleActive)
                {
                    UIManager.instance.inputModule.DeactivateModule();
                }
            }

            Cursor.visible = prevCursorVisible;
            Cursor.lockState = prevCursorLock;
        }

        private void MaintainUiInteraction()
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }

            if (UIManager.instance?.inputModule != null)
            {
                if (!UIManager.instance.inputModule.allowMouseInput)
                {
                    UIManager.instance.inputModule.allowMouseInput = true;
                }

                UIManager.instance.inputModule.forceModuleActive = true;
                UIManager.instance.inputModule.ActivateModule();
            }
        }

        private static void DestroyRoot(ref GameObject? root)
        {
            if (root != null)
            {
                UObject.Destroy(root);
                root = null;
            }
        }

        private Text CreateRowLabel(Transform parent, string text)
        {
            Text label = CreateText(parent, "Label", text, 30, TextAnchor.MiddleLeft);
            RectTransform rect = label.rectTransform;
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(20f, 0f);
            rect.sizeDelta = new Vector2(600f, RowHeight);
            return label;
        }

        private Text CreateRowValue(Transform parent, string text)
        {
            Text value = CreateText(parent, "Value", text, 30, TextAnchor.MiddleRight);
            RectTransform rect = value.rectTransform;
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = new Vector2(-20f, 0f);
            rect.sizeDelta = new Vector2(140f, RowHeight);
            return value;
        }

        private Text CreateText(Transform parent, string name, string text, int size, TextAnchor alignment)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);

            Text label = obj.AddComponent<Text>();
            label.text = text;
            label.font = GetMenuFont();
            label.fontSize = size;
            label.alignment = alignment;
            label.color = Color.white;
            label.raycastTarget = false;

            return label;
        }

        private void ShowStatusMessage(string message)
        {
            if (statusText == null)
            {
                return;
            }

            statusText.text = message;
            statusText.gameObject.SetActive(true);
            statusHideTime = Time.unscaledTime + 2f;
        }

        private void UpdateStatusMessage()
        {
            if (statusText == null || !statusText.gameObject.activeSelf)
            {
                return;
            }

            if (Time.unscaledTime >= statusHideTime)
            {
                statusText.gameObject.SetActive(false);
            }
        }

        private void RefreshFastSuperDashUi()
        {
            Module? module = GetFastSuperDashModule();
            UpdateToggleValue(moduleToggleValue, module?.Enabled ?? false);
            UpdateToggleValue(instantToggleValue, Modules.QoL.FastSuperDash.instantSuperDash);
            UpdateToggleValue(everywhereToggleValue, Modules.QoL.FastSuperDash.fastSuperDashEverywhere);

            float speed = Modules.QoL.FastSuperDash.fastSuperDashSpeedMultiplier;
            UpdateSpeedValueText(speed);
            if (speedSlider != null)
            {
                speedSlider.value = Mathf.Clamp(speed, speedSlider.minValue, speedSlider.maxValue);
            }
        }

        private void RefreshCollectorPhasesUi()
        {
            Module? module = GetCollectorPhasesModule();
            UpdateToggleValue(collectorModuleToggleValue, module?.Enabled ?? false);
            UpdateToggleValue(collectorHoGOnlyValue, Modules.CollectorPhases.CollectorPhases.HoGOnly);
            UpdateToggleValue(qolCollectorRoarValue, GetCollectorRoarEnabled());
            UpdateToggleValue(collectorImmortalValue, Modules.CollectorPhases.CollectorPhases.CollectorImmortal);
            UpdateToggleValue(ignoreInitialJarLimitValue, Modules.CollectorPhases.CollectorPhases.IgnoreInitialJarLimit);
            UpdateToggleValue(useCustomPhase2ThresholdValue, Modules.CollectorPhases.CollectorPhases.UseCustomPhase2Threshold);
            UpdateToggleValue(useMaxHpValue, Modules.CollectorPhases.CollectorPhases.UseMaxHP);
            UpdateToggleValue(spawnBuzzerValue, Modules.CollectorPhases.CollectorPhases.spawnBuzzer);
            UpdateToggleValue(spawnRollerValue, Modules.CollectorPhases.CollectorPhases.spawnRoller);
            UpdateToggleValue(spawnSpitterValue, Modules.CollectorPhases.CollectorPhases.spawnSpitter);
            UpdateToggleValue(disableSummonLimitValue, Modules.CollectorPhases.CollectorPhases.DisableSummonLimit);

            UpdateIntValueText(collectorPhaseValue, Modules.CollectorPhases.CollectorPhases.collectorPhase);
            UpdateIntInputValue(customPhase2ThresholdField, Modules.CollectorPhases.CollectorPhases.CustomPhase2Threshold);
            UpdateIntInputValue(collectorMaxHpField, Modules.CollectorPhases.CollectorPhases.collectorMaxHP);
            UpdateIntInputValue(buzzerHpField, Modules.CollectorPhases.CollectorPhases.buzzerHP);
            UpdateIntInputValue(rollerHpField, Modules.CollectorPhases.CollectorPhases.rollerHP);
            UpdateIntInputValue(spitterHpField, Modules.CollectorPhases.CollectorPhases.spitterHP);
            UpdateIntInputValue(customSummonLimitField, Modules.CollectorPhases.CollectorPhases.CustomSummonLimit);
        }

        private void RefreshFastReloadUi()
        {
            Module? module = GetFastReloadModule();
            UpdateToggleValue(fastReloadToggleValue, module?.Enabled ?? false);
            UpdateKeybindValue(reloadKeyValue, FormatKeyLabel(GetReloadKey()));
            UpdateKeybindValue(teleportKeyValue, FormatKeyLabel(GetTeleportKey()));
        }

        private void RefreshDreamshieldUi()
        {
            bool enabled = Modules.QoL.DreamshieldStartAngle.startAngleEnabled;
            UpdateToggleValue(dreamshieldToggleValue, enabled);

            float delay = Modules.QoL.DreamshieldStartAngle.rotationDelay;
            UpdateFloatValueText(dreamshieldDelayValue, delay, 2);
            if (dreamshieldDelaySlider != null)
            {
                dreamshieldDelaySlider.value = Mathf.Clamp(delay, dreamshieldDelaySlider.minValue, dreamshieldDelaySlider.maxValue);
            }

            float speed = Modules.QoL.DreamshieldStartAngle.rotationSpeed;
            UpdateFloatValueText(dreamshieldSpeedValue, speed, 2);
            if (dreamshieldSpeedSlider != null)
            {
                dreamshieldSpeedSlider.value = Mathf.Clamp(speed, dreamshieldSpeedSlider.minValue, dreamshieldSpeedSlider.maxValue);
            }

            UpdateDreamshieldSliderState();
        }

        private void RefreshShowHpOnDeathUi()
        {
            UpdateToggleValue(showHpGlobalValue, ShowHpSettings.EnabledMod);
            UpdateToggleValue(showHpShowPbValue, ShowHpSettings.ShowPB);
            UpdateToggleValue(showHpAutoHideValue, ShowHpSettings.HideAfter10Sec);

            UpdateKeybindValue(showHpHudToggleKeyValue, GetShowHpBindingLabel());

            float fade = ClampShowHpFade(ShowHpSettings.HudFadeSeconds);
            UpdateFloatValueText(showHpFadeValue, fade, 1);
            if (showHpFadeSlider != null)
            {
                showHpFadeSlider.value = Mathf.Clamp(fade, showHpFadeSlider.minValue, showHpFadeSlider.maxValue);
            }
        }

        private void RefreshSpeedChangerUi()
        {
            UpdateToggleValue(speedChangerGlobalValue, SpeedChanger.globalSwitch);
            UpdateToggleValue(speedChangerLockValue, SpeedChanger.lockSwitch);
            UpdateToggleValue(speedChangerRestrictValue, SpeedChanger.restrictToggleToRooms);
            UpdateToggleValue(speedChangerUnlimitedValue, SpeedChanger.unlimitedSpeed);

            UpdateKeybindValue(speedChangerToggleKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.toggleKeybind));
            UpdateKeybindValue(speedChangerInputKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.inputSpeedKeybind));
            UpdateKeybindValue(speedChangerSpeedDownKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.slowDownKeybind));
            UpdateKeybindValue(speedChangerSpeedUpKeyValue, FormatSpeedChangerKeyLabel(SpeedChanger.speedUpKeybind));

            UpdateKeybindValue(speedChangerDisplayValue, GetSpeedChangerDisplayValue());
        }

        private void RefreshTeleportKitUi()
        {
            Module? module = GetTeleportKitModule();
            UpdateToggleValue(teleportKitToggleValue, module?.Enabled ?? false);
            UpdateKeybindValue(teleportKitMenuKeyValue, GetTeleportKitMenuKeyLabel());
            UpdateKeybindValue(teleportKitSaveKeyValue, GetTeleportKitSaveKeyLabel());
            UpdateKeybindValue(teleportKitTeleportKeyValue, GetTeleportKitTeleportKeyLabel());
        }

        private void RefreshBossChallengeUi()
        {
            UpdateToggleValue(bossInfiniteChallengeValue, GetInfiniteChallengeEnabled());
            UpdateToggleValue(bossRestartOnSuccessValue, Modules.BossChallenge.InfiniteChallenge.restartFightOnSuccess);
            UpdateToggleValue(bossRestartAndMusicValue, Modules.BossChallenge.InfiniteChallenge.restartFightAndMusic);
            UpdateToggleValue(qolCarefreeMelodyValue, GetCarefreeMelodyEnabled());
            UpdateToggleValue(bossInfiniteGrimmValue, GetInfiniteGrimmPufferfishEnabled());
            UpdateToggleValue(bossInfiniteRadianceValue, GetInfiniteRadianceClimbingEnabled());
            UpdateToggleValue(bossP5HealthValue, GetP5HealthEnabled());
            UpdateToggleValue(bossSegmentedP5Value, GetSegmentedP5Enabled());
            UpdateToggleValue(bossHalveAscendedValue, GetHalveAscendedEnabled());
            UpdateToggleValue(bossHalveAttunedValue, GetHalveAttunedEnabled());
            UpdateToggleValue(bossAddLifebloodValue, GetAddLifebloodEnabled());
            UpdateToggleValue(bossAddSoulValue, GetAddSoulEnabled());

            UpdateIntInputValue(bossLifebloodAmountField, Modules.BossChallenge.AddLifeblood.lifebloodAmount);
            UpdateIntInputValue(bossSoulAmountField, Modules.BossChallenge.AddSoul.soulAmount);

            if (bossGpzValue != null)
            {
                bossGpzValue.text = GetBossGpzLabel();
            }
        }

        private void RefreshQolUi()
        {
            UpdateToggleValue(bossAnimFastDreamWarpValue, GetFastDreamWarpEnabled());
            UpdateKeybindValue(fastDreamWarpKeyValue, GetFastDreamWarpBindingLabel());
            UpdateToggleValue(bossAnimShortDeathValue, GetShortDeathAnimationEnabled());
            UpdateToggleValue(bossAnimHallOfGodsValue, Modules.QoL.SkipCutscenes.HallOfGodsStatues);
            UpdateToggleValue(qolUnlockAllModesValue, GetUnlockAllModesEnabled());
            UpdateToggleValue(qolUnlockPantheonsValue, GetUnlockPantheonsEnabled());
            UpdateToggleValue(qolUnlockRadianceValue, GetUnlockRadianceEnabled());
            UpdateToggleValue(qolUnlockRadiantValue, GetUnlockRadiantEnabled());
        }

        private void RefreshMenuAnimationUi()
        {
            UpdateToggleValue(menuAnimDoorDefaultValue, GetDoorDefaultBeginEnabled());
            UpdateToggleValue(menuAnimMemorizeBindingsValue, GetMemorizeBindingsEnabled());
            UpdateToggleValue(menuAnimFasterLoadsValue, GetFasterLoadsEnabled());
            UpdateToggleValue(menuAnimFastMenusValue, GetFastMenusEnabled());
            UpdateToggleValue(menuAnimFastTextValue, GetFastTextEnabled());
            UpdateToggleValue(menuAnimAutoSkipValue, Modules.QoL.SkipCutscenes.AutoSkipCinematics);
            UpdateToggleValue(menuAnimAllowSkippingValue, Modules.QoL.SkipCutscenes.AllowSkippingNonskippable);
            UpdateToggleValue(menuAnimSkipWithoutPromptValue, Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt);
        }

        private void RefreshBossAnimationUi()
        {
            UpdateToggleValue(bossAnimAbsoluteRadianceValue, Modules.QoL.SkipCutscenes.AbsoluteRadiance);
            UpdateToggleValue(bossAnimPureVesselValue, Modules.QoL.SkipCutscenes.PureVesselRoar);
            UpdateToggleValue(bossAnimGrimmNightmareValue, Modules.QoL.SkipCutscenes.GrimmNightmare);
            UpdateToggleValue(bossAnimGreyPrinceValue, Modules.QoL.SkipCutscenes.GreyPrinceZote);
            UpdateToggleValue(bossAnimCollectorValue, Modules.QoL.SkipCutscenes.Collector);
            UpdateToggleValue(bossAnimSoulMasterValue, Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip);
            UpdateToggleValue(bossAnimFirstTimeValue, Modules.QoL.SkipCutscenes.FirstTimeBosses);
        }

        private void RefreshBindingsUi()
        {
            UpdateToggleValue(bindingsEverywhereValue, GetBindingsEverywhereEnabled());
            UpdateKeybindValue(bindingsHotkeyValue, GetBindingsMenuHotkeyLabel());
        }

        private void RefreshZoteHelperUi()
        {
            Module? module = GetZoteHelperModule();
            UpdateToggleValue(zoteHelperToggleValue, module?.Enabled ?? false);
            UpdateIntInputValue(zoteBossHpField, Modules.BossChallenge.ZoteHelper.zoteBossHp);
            UpdateToggleValue(zoteImmortalValue, Modules.BossChallenge.ZoteHelper.zoteImmortal);
            UpdateToggleValue(zoteSpawnFlyingValue, Modules.BossChallenge.ZoteHelper.zoteSpawnFlying);
            UpdateToggleValue(zoteSpawnHoppingValue, Modules.BossChallenge.ZoteHelper.zoteSpawnHopping);
            UpdateIntInputValue(zoteSummonFlyingHpField, Modules.BossChallenge.ZoteHelper.zoteSummonFlyingHp);
            UpdateIntInputValue(zoteSummonHoppingHpField, Modules.BossChallenge.ZoteHelper.zoteSummonHoppingHp);
            UpdateIntInputValue(zoteSummonLimitField, Modules.BossChallenge.ZoteHelper.zoteSummonLimit);

            if (bossGpzValue != null)
            {
                bossGpzValue.text = GetBossGpzLabel();
            }
        }


        private void RefreshQuickMenuSettingsUi()
        {
            foreach (KeyValuePair<string, Text> entry in quickSettingsToggleValues)
            {
                UpdateToggleValue(entry.Value, IsQuickMenuItemVisible(entry.Key));
            }

            int opacity = GetQuickMenuOpacity();
            UpdateFloatValueText(quickMenuOpacityValue, opacity, 0);
            if (quickMenuOpacitySlider != null && Math.Abs(quickMenuOpacitySlider.value - opacity) > 0.0001f)
            {
                quickMenuOpacitySlider.value = opacity;
            }
        }

        private void UpdateDreamshieldSliderState()
        {
            bool enabled = Modules.QoL.DreamshieldStartAngle.startAngleEnabled;
            if (dreamshieldDelaySlider != null)
            {
                dreamshieldDelaySlider.interactable = enabled;
            }

            if (dreamshieldSpeedSlider != null)
            {
                dreamshieldSpeedSlider.interactable = enabled;
            }
        }

        internal static void ApplyInitialDefaults()
        {
            SetModuleEnabled<Modules.QoL.FastSuperDash>(false);
            Modules.QoL.FastSuperDash.instantSuperDash = false;
            Modules.QoL.FastSuperDash.fastSuperDashEverywhere = false;
            Modules.QoL.FastSuperDash.fastSuperDashSpeedMultiplier = 1f;

            ResetCollectorPhasesDefaults();
            Modules.CollectorPhases.CollectorPhases.HoGOnly = true;
            SetModuleEnabled<Modules.QoL.CollectorRoarMute>(false);
            SetModuleEnabled<Modules.CollectorPhases.CollectorPhases>(false);

            SetModuleEnabled<Modules.FastReload>(false);
            Modules.FastReload.reloadKeyCode = (int)KeyCode.None;
            Modules.FastReload.teleportKeyCode = (int)KeyCode.None;

            Modules.QoL.DreamshieldStartAngle.startAngleEnabled = false;
            Modules.QoL.DreamshieldStartAngle.rotationDelay = 0f;
            Modules.QoL.DreamshieldStartAngle.rotationSpeed = 1f;

            ShowHpSettings.EnabledMod = false;
            ShowHpSettings.ShowPB = false;
            ShowHpSettings.HideAfter10Sec = false;
            ShowHpSettings.HudFadeSeconds = 5f;
            ApplyShowHpBinding(null);

            SpeedChanger.globalSwitch = false;
            SpeedChanger.lockSwitch = false;
            SpeedChanger.restrictToggleToRooms = false;
            SpeedChanger.unlimitedSpeed = false;
            SpeedChanger.displayStyle = 0;
            SpeedChanger.toggleKeybind = string.Empty;
            SpeedChanger.inputSpeedKeybind = string.Empty;
            SpeedChanger.slowDownKeybind = string.Empty;
            SpeedChanger.speedUpKeybind = string.Empty;
            SpeedChanger.speed = 1f;

            SetModuleEnabled<Modules.QoL.TeleportKit>(false);
            Modules.QoL.TeleportKit.MenuHotkey = KeyCode.F6;
            Modules.QoL.TeleportKit.SaveTeleportHotkey = KeyCode.R;
            Modules.QoL.TeleportKit.TeleportHotkey = KeyCode.T;

            SetModuleEnabled<Modules.BossChallenge.InfiniteChallenge>(false);
            SetModuleEnabled<Modules.BossChallenge.InfiniteGrimmPufferfish>(false);
            SetModuleEnabled<Modules.BossChallenge.InfiniteRadianceClimbing>(false);
            SetModuleEnabled<Modules.QoL.CarefreeMelodyReset>(false);
            SetModuleEnabled<Modules.BossChallenge.P5Health>(false);
            SetModuleEnabled<Modules.BossChallenge.SegmentedP5>(false);
            SetModuleEnabled<Modules.BossChallenge.HalveDamageHoGAscendedOrAbove>(false);
            SetModuleEnabled<Modules.BossChallenge.HalveDamageHoGAttuned>(false);
            SetModuleEnabled<Modules.BossChallenge.AddLifeblood>(false);
            SetModuleEnabled<Modules.BossChallenge.AddSoul>(false);

            Modules.BossChallenge.InfiniteChallenge.restartFightOnSuccess = false;
            Modules.BossChallenge.InfiniteChallenge.restartFightAndMusic = false;
            Modules.BossChallenge.AddLifeblood.lifebloodAmount = 0;
            Modules.BossChallenge.AddSoul.soulAmount = 0;
            Modules.BossChallenge.SegmentedP5.selectedP5Segment = 0;

            ResetZoteHelperDefaults();
            Modules.BossChallenge.ForceGreyPrinceEnterType.gpzEnterType =
                Modules.BossChallenge.ForceGreyPrinceEnterType.EnterType.Off;
            SetModuleEnabled<Modules.BossChallenge.ZoteHelper>(false);

            SetModuleEnabled<Modules.QoL.FastDreamWarp>(false);
            FastDreamWarpSettings.Keybinds.Toggle.ClearBindings();
            SetModuleEnabled<Modules.QoL.ShortDeathAnimation>(true);
            Modules.QoL.SkipCutscenes.HallOfGodsStatues = true;
            SetModuleEnabled<Modules.QoL.UnlockAllModes>(true);
            SetModuleEnabled<Modules.QoL.UnlockPantheons>(true);
            SetModuleEnabled<Modules.QoL.UnlockRadiance>(true);
            SetModuleEnabled<Modules.QoL.UnlockRadiant>(true);

            GodhomeQoL.GlobalSettings.BindingsMenuEverywhere = false;
            GodhomeQoL.GlobalSettings.BindingsMenuHotkey = string.Empty;
            GodhomeQoL.GlobalSettings.QuickMenuOpacity = 100;

            SetModuleEnabled<Modules.QoL.DoorDefaultBegin>(true);
            SetModuleEnabled<Modules.QoL.MemorizeBindings>(true);
            SetModuleEnabled<Modules.QoL.FasterLoads>(true);
            SetModuleEnabled<Modules.QoL.FastMenus>(true);
            SetModuleEnabled<Modules.QoL.FastText>(true);
            Modules.QoL.SkipCutscenes.AutoSkipCinematics = true;
            Modules.QoL.SkipCutscenes.AllowSkippingNonskippable = true;
            Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt = true;

            Modules.QoL.SkipCutscenes.AbsoluteRadiance = true;
            Modules.QoL.SkipCutscenes.PureVesselRoar = true;
            Modules.QoL.SkipCutscenes.GrimmNightmare = true;
            Modules.QoL.SkipCutscenes.GreyPrinceZote = true;
            Modules.QoL.SkipCutscenes.Collector = true;
            Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip = true;
            Modules.QoL.SkipCutscenes.FirstTimeBosses = true;
        }

        private static void SetModuleEnabled<T>(bool value) where T : Module
        {
            if (ModuleManager.TryGetModule(typeof(T), out Module? module))
            {
                module.Enabled = value;
                return;
            }

            string name = typeof(T).Name;
            if (Setting.Global.Modules.ContainsKey(name))
            {
                Setting.Global.Modules[name] = value;
            }
        }

        private static void ResetCollectorPhasesDefaults()
        {
            try
            {
                MethodInfo? method = typeof(Modules.CollectorPhases.CollectorPhases)
                    .GetMethod("ResetDefaults", BindingFlags.NonPublic | BindingFlags.Static);
                if (method != null)
                {
                    method.Invoke(null, null);
                    return;
                }
            }
            catch
            {
                // Ignore reflection failures and fall back to hard-coded defaults.
            }

            Modules.CollectorPhases.CollectorPhases.collectorPhase = 3;
            Modules.CollectorPhases.CollectorPhases.collectorMaxHP = 1200;
            Modules.CollectorPhases.CollectorPhases.UseMaxHP = true;
            Modules.CollectorPhases.CollectorPhases.UseCustomPhase2Threshold = false;
            Modules.CollectorPhases.CollectorPhases.CustomPhase2Threshold = 850;
            Modules.CollectorPhases.CollectorPhases.buzzerHP = 26;
            Modules.CollectorPhases.CollectorPhases.rollerHP = 26;
            Modules.CollectorPhases.CollectorPhases.spitterHP = 26;
            Modules.CollectorPhases.CollectorPhases.spawnBuzzer = true;
            Modules.CollectorPhases.CollectorPhases.spawnRoller = true;
            Modules.CollectorPhases.CollectorPhases.spawnSpitter = true;
            Modules.CollectorPhases.CollectorPhases.CollectorImmortal = false;
            Modules.CollectorPhases.CollectorPhases.IgnoreInitialJarLimit = false;
            Modules.CollectorPhases.CollectorPhases.DisableSummonLimit = false;
            Modules.CollectorPhases.CollectorPhases.CustomSummonLimit = 20;
            Modules.CollectorPhases.CollectorPhases.HoGOnly = true;
        }

        private static void ResetZoteHelperDefaults()
        {
            Modules.BossChallenge.ZoteHelper.zoteBossHp = 1400;
            Modules.BossChallenge.ZoteHelper.zoteImmortal = false;
            Modules.BossChallenge.ZoteHelper.zoteSpawnFlying = true;
            Modules.BossChallenge.ZoteHelper.zoteSpawnHopping = true;
            Modules.BossChallenge.ZoteHelper.zoteSummonFlyingHp = 20;
            Modules.BossChallenge.ZoteHelper.zoteSummonHoppingHp = 20;
            Modules.BossChallenge.ZoteHelper.zoteSummonLimit = 3;
        }

        private void OnSpeedChanged(float value)
        {
            float rounded = RoundToTenth(value);
            Modules.QoL.FastSuperDash.fastSuperDashSpeedMultiplier = rounded;
            UpdateSpeedValueText(rounded);
        }

        private void OnDreamshieldDelayChanged(float value)
        {
            float rounded = (float)Math.Round(value, 2);
            Modules.QoL.DreamshieldStartAngle.rotationDelay = rounded;
            UpdateFloatValueText(dreamshieldDelayValue, rounded, 2);
            if (dreamshieldDelaySlider != null && Math.Abs(dreamshieldDelaySlider.value - rounded) > 0.0001f)
            {
                dreamshieldDelaySlider.value = rounded;
            }
        }

        private void OnDreamshieldSpeedChanged(float value)
        {
            float rounded = (float)Math.Round(value, 2);
            Modules.QoL.DreamshieldStartAngle.rotationSpeed = rounded;
            UpdateFloatValueText(dreamshieldSpeedValue, rounded, 2);
            if (dreamshieldSpeedSlider != null && Math.Abs(dreamshieldSpeedSlider.value - rounded) > 0.0001f)
            {
                dreamshieldSpeedSlider.value = rounded;
            }
        }

        private void OnShowHpFadeChanged(float value)
        {
            float rounded = (float)Math.Round(ClampShowHpFade(value), 1, MidpointRounding.AwayFromZero);
            ShowHpSettings.HudFadeSeconds = rounded;
            UpdateFloatValueText(showHpFadeValue, rounded, 1);
            if (showHpFadeSlider != null && Math.Abs(showHpFadeSlider.value - rounded) > 0.0001f)
            {
                showHpFadeSlider.value = rounded;
            }
        }

        private static float ClampShowHpFade(float value)
        {
            return Mathf.Clamp(value, 1f, 10f);
        }

        private static float RoundToTenth(float value)
        {
            return Mathf.Round(value * 10f) / 10f;
        }

        private static string FormatToggleValue(float value)
        {
            return value.ToString("0.0");
        }

        private static string FormatFloatValue(float value, int decimals)
        {
            string format = decimals <= 0 ? "0" : $"0.{new string('0', decimals)}";
            return value.ToString(format);
        }

        private void UpdateSpeedValueText(float value)
        {
            if (speedValueText != null)
            {
                speedValueText.text = FormatToggleValue(value);
            }
        }

        private void UpdateFloatValueText(Text? text, float value, int decimals)
        {
            if (text != null)
            {
                text.text = FormatFloatValue(value, decimals);
            }
        }

        private static int ClampOptionIndex(int value, int length)
        {
            if (length <= 0)
            {
                return 0;
            }

            return value < 0 || value >= length ? 0 : value;
        }

        private void UpdateIntValueText(Text? text, int value)
        {
            if (text != null)
            {
                text.text = value.ToString();
            }
        }

        private void UpdateIntInputValue(InputField? field, int value)
        {
            if (field != null)
            {
                field.text = value.ToString();
            }
        }

        private void AdjustIntValue(
            Func<int> getter,
            Action<int> setter,
            Text? valueText,
            int direction,
            int minValue,
            int maxValue,
            int baseStep)
        {
            int current = getter();
            int step = baseStep * GetStepMultiplier();
            long next = current + (long)direction * step;
            if (next < minValue)
            {
                next = minValue;
            }
            else if (next > maxValue)
            {
                next = maxValue;
            }

            setter((int)next);
            UpdateIntValueText(valueText, getter());
        }

        private void AdjustIntInputValue(
            Func<int> getter,
            Action<int> setter,
            InputField? valueField,
            int direction,
            int minValue,
            int maxValue,
            int baseStep)
        {
            int current = getter();
            int step = baseStep * GetStepMultiplier();
            long next = current + (long)direction * step;
            if (next < minValue)
            {
                next = minValue;
            }
            else if (next > maxValue)
            {
                next = maxValue;
            }

            setter((int)next);
            UpdateIntInputValue(valueField, getter());
        }

        private void ApplyInputValue(
            string rawValue,
            Func<int> getter,
            Action<int> setter,
            InputField? field,
            int minValue,
            int maxValue)
        {
            if (!int.TryParse(rawValue, out int parsed))
            {
                UpdateIntInputValue(field, getter());
                return;
            }

            parsed = Mathf.Clamp(parsed, minValue, maxValue);
            setter(parsed);
            UpdateIntInputValue(field, getter());
        }

        private static int GetStepMultiplier()
        {
            int mult = 1;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                mult *= 10;
            }

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                mult *= 100;
            }

            return mult;
        }

        private void UpdateToggleValue(Text? text, bool value)
        {
            if (text != null)
            {
                text.text = value ? "ON" : "OFF";
            }
        }

        private bool GetModuleEnabled()
        {
            Module? module = GetFastSuperDashModule();
            return module?.Enabled ?? false;
        }

        private void SetModuleEnabled(bool value)
        {
            Module? module = GetFastSuperDashModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetCollectorPhasesEnabled()
        {
            Module? module = GetCollectorPhasesModule();
            return module?.Enabled ?? false;
        }

        private void SetCollectorPhasesEnabled(bool value)
        {
            Module? module = GetCollectorPhasesModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetFastReloadEnabled()
        {
            Module? module = GetFastReloadModule();
            return module?.Enabled ?? false;
        }

        private void SetFastReloadEnabled(bool value)
        {
            Module? module = GetFastReloadModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetDreamshieldEnabled()
        {
            return Modules.QoL.DreamshieldStartAngle.startAngleEnabled;
        }

        private void SetDreamshieldEnabled(bool value)
        {
            Modules.QoL.DreamshieldStartAngle.startAngleEnabled = value;
            UpdateDreamshieldSliderState();
        }

        private bool GetTeleportKitEnabled()
        {
            Module? module = GetTeleportKitModule();
            return module?.Enabled ?? false;
        }

        private void SetTeleportKitEnabled(bool value)
        {
            Module? module = GetTeleportKitModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetZoteHelperEnabled()
        {
            Module? module = GetZoteHelperModule();
            return module?.Enabled ?? false;
        }

        private void SetZoteHelperEnabled(bool value)
        {
            Module? module = GetZoteHelperModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private void SetZoteBossHp(int value)
        {
            Modules.BossChallenge.ZoteHelper.zoteBossHp = Mathf.Clamp(value, 100, 999999);
            Modules.BossChallenge.ZoteHelper.ApplyBossHealthIfPresent();
        }

        private void SetZoteImmortalEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteImmortal = value;
            Modules.BossChallenge.ZoteHelper.ApplyBossHealthIfPresent();
        }

        private void SetZoteSpawnFlyingEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteSpawnFlying = value;
        }

        private void SetZoteSpawnHoppingEnabled(bool value)
        {
            Modules.BossChallenge.ZoteHelper.zoteSpawnHopping = value;
        }

        private void SetZoteFlyingHp(int value)
        {
            Modules.BossChallenge.ZoteHelper.zoteSummonFlyingHp = Mathf.Clamp(value, 0, 99);
            Modules.BossChallenge.ZoteHelper.ApplyZotelingHealthIfPresent();
        }

        private void SetZoteHoppingHp(int value)
        {
            Modules.BossChallenge.ZoteHelper.zoteSummonHoppingHp = Mathf.Clamp(value, 0, 99);
            Modules.BossChallenge.ZoteHelper.ApplyZotelingHealthIfPresent();
        }

        private void SetCollectorPhase(int value)
        {
            int phase = Mathf.Clamp(value, 1, 3);
            Modules.CollectorPhases.CollectorPhases.collectorPhase = phase;

            if (phase == 2 && !Modules.CollectorPhases.CollectorPhases.IgnoreInitialJarLimit)
            {
                Modules.CollectorPhases.CollectorPhases.IgnoreInitialJarLimit = true;
                UpdateToggleValue(ignoreInitialJarLimitValue, true);
            }
        }

        private bool GetInfiniteChallengeEnabled()
        {
            Module? module = GetInfiniteChallengeModule();
            return module?.Enabled ?? false;
        }

        private void SetInfiniteChallengeEnabled(bool value)
        {
            Module? module = GetInfiniteChallengeModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetInfiniteGrimmPufferfishEnabled()
        {
            Module? module = GetInfiniteGrimmPufferfishModule();
            return module?.Enabled ?? false;
        }

        private void SetInfiniteGrimmPufferfishEnabled(bool value)
        {
            Module? module = GetInfiniteGrimmPufferfishModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetInfiniteRadianceClimbingEnabled()
        {
            Module? module = GetInfiniteRadianceClimbingModule();
            return module?.Enabled ?? false;
        }

        private void SetInfiniteRadianceClimbingEnabled(bool value)
        {
            Module? module = GetInfiniteRadianceClimbingModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetP5HealthEnabled()
        {
            Module? module = GetP5HealthModule();
            return module?.Enabled ?? false;
        }

        private void SetP5HealthEnabled(bool value)
        {
            Module? module = GetP5HealthModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetSegmentedP5Enabled()
        {
            Module? module = GetSegmentedP5Module();
            return module?.Enabled ?? false;
        }

        private void SetSegmentedP5Enabled(bool value)
        {
            Module? module = GetSegmentedP5Module();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetHalveAscendedEnabled()
        {
            Module? module = GetHalveAscendedModule();
            return module?.Enabled ?? false;
        }

        private void SetHalveAscendedEnabled(bool value)
        {
            Module? module = GetHalveAscendedModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetHalveAttunedEnabled()
        {
            Module? module = GetHalveAttunedModule();
            return module?.Enabled ?? false;
        }

        private void SetHalveAttunedEnabled(bool value)
        {
            Module? module = GetHalveAttunedModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetAddLifebloodEnabled()
        {
            Module? module = GetAddLifebloodModule();
            return module?.Enabled ?? false;
        }

        private void SetAddLifebloodEnabled(bool value)
        {
            Module? module = GetAddLifebloodModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetAddSoulEnabled()
        {
            Module? module = GetAddSoulModule();
            return module?.Enabled ?? false;
        }

        private void SetAddSoulEnabled(bool value)
        {
            Module? module = GetAddSoulModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetCarefreeMelodyEnabled()
        {
            Module? module = GetCarefreeMelodyModule();
            return module?.Enabled ?? false;
        }

        private void SetCarefreeMelodyEnabled(bool value)
        {
            Module? module = GetCarefreeMelodyModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetCollectorRoarEnabled()
        {
            Module? module = GetCollectorRoarModule();
            return module?.Enabled ?? false;
        }

        private void SetCollectorRoarEnabled(bool value)
        {
            Module? module = GetCollectorRoarModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetUnlockAllModesEnabled()
        {
            Module? module = GetUnlockAllModesModule();
            return module?.Enabled ?? false;
        }

        private void SetUnlockAllModesEnabled(bool value)
        {
            Module? module = GetUnlockAllModesModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetUnlockPantheonsEnabled()
        {
            Module? module = GetUnlockPantheonsModule();
            return module?.Enabled ?? false;
        }

        private void SetUnlockPantheonsEnabled(bool value)
        {
            Module? module = GetUnlockPantheonsModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetUnlockRadianceEnabled()
        {
            Module? module = GetUnlockRadianceModule();
            return module?.Enabled ?? false;
        }

        private void SetUnlockRadianceEnabled(bool value)
        {
            Module? module = GetUnlockRadianceModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetUnlockRadiantEnabled()
        {
            Module? module = GetUnlockRadiantModule();
            return module?.Enabled ?? false;
        }

        private void SetUnlockRadiantEnabled(bool value)
        {
            Module? module = GetUnlockRadiantModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetDoorDefaultBeginEnabled()
        {
            Module? module = GetDoorDefaultBeginModule();
            return module?.Enabled ?? false;
        }

        private void SetDoorDefaultBeginEnabled(bool value)
        {
            Module? module = GetDoorDefaultBeginModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetMemorizeBindingsEnabled()
        {
            Module? module = GetMemorizeBindingsModule();
            return module?.Enabled ?? false;
        }

        private void SetMemorizeBindingsEnabled(bool value)
        {
            Module? module = GetMemorizeBindingsModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetFasterLoadsEnabled()
        {
            Module? module = GetFasterLoadsModule();
            return module?.Enabled ?? false;
        }

        private void SetFasterLoadsEnabled(bool value)
        {
            Module? module = GetFasterLoadsModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetFastMenusEnabled()
        {
            Module? module = GetFastMenusModule();
            return module?.Enabled ?? false;
        }

        private void SetFastMenusEnabled(bool value)
        {
            Module? module = GetFastMenusModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetFastTextEnabled()
        {
            Module? module = GetFastTextModule();
            return module?.Enabled ?? false;
        }

        private void SetFastTextEnabled(bool value)
        {
            Module? module = GetFastTextModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetFastDreamWarpEnabled()
        {
            Module? module = GetFastDreamWarpModule();
            return module?.Enabled ?? false;
        }

        private void SetFastDreamWarpEnabled(bool value)
        {
            Module? module = GetFastDreamWarpModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private bool GetShortDeathAnimationEnabled()
        {
            Module? module = GetShortDeathAnimationModule();
            return module?.Enabled ?? false;
        }

        private void SetShortDeathAnimationEnabled(bool value)
        {
            Module? module = GetShortDeathAnimationModule();
            if (module != null)
            {
                module.Enabled = value;
            }
        }

        private int GetBossGpzIndex()
        {
            Module? module = GetForceGreyPrinceModule();
            if (module?.Enabled != true)
            {
                return 0;
            }

            return Modules.BossChallenge.ForceGreyPrinceEnterType.gpzEnterType switch
            {
                Modules.BossChallenge.ForceGreyPrinceEnterType.EnterType.Long => 1,
                Modules.BossChallenge.ForceGreyPrinceEnterType.EnterType.Short => 2,
                _ => 0
            };
        }

        private void ApplyBossGpzOption(int index)
        {
            int clamped = ClampOptionIndex(index, BossChallengeGpzOptions.Length);
            Module? module = GetForceGreyPrinceModule();

            if (clamped == 0)
            {
                if (module != null)
                {
                    module.Enabled = false;
                }
            }
            else
            {
                Modules.BossChallenge.ForceGreyPrinceEnterType.gpzEnterType = clamped == 1
                    ? Modules.BossChallenge.ForceGreyPrinceEnterType.EnterType.Long
                    : Modules.BossChallenge.ForceGreyPrinceEnterType.EnterType.Short;

                if (module != null)
                {
                    module.Enabled = true;
                }
            }

            if (bossGpzValue != null)
            {
                bossGpzValue.text = BossChallengeGpzOptions[clamped];
            }
        }

        private string GetBossGpzLabel()
        {
            int index = GetBossGpzIndex();
            return BossChallengeGpzOptions[index];
        }

        private void SetSpeedChangerGlobalSwitch(bool value)
        {
            SpeedChanger? module = GetSpeedChangerModule();
            if (module != null)
            {
                MethodInfo? method = typeof(SpeedChanger).GetMethod("ChangeGlobalSwitchState", BindingFlags.Instance | BindingFlags.NonPublic);
                if (method != null)
                {
                    method.Invoke(module, new object[] { value, false });
                    return;
                }
            }

            SpeedChanger.globalSwitch = value;
        }

        private void ApplySpeedChangerDisplayStyle(int value)
        {
            int style = ClampOptionIndex(value, SpeedChangerDisplayOptions.Length);
            SpeedChanger.displayStyle = style;

            if (style == 2)
            {
                ModDisplay.Instance?.Destroy();
                ModDisplay.Instance = null;
            }
            else
            {
                ModDisplay.Instance ??= new ModDisplay();
            }

            if (speedChangerDisplayValue != null)
            {
                speedChangerDisplayValue.text = SpeedChangerDisplayOptions[style];
            }
        }

        private static ShowHPOnDeathSettings ShowHpSettings =>
            GodhomeQoL.GlobalSettings.ShowHPOnDeath ??= new ShowHPOnDeathSettings();

        private static FastDreamWarpSettings FastDreamWarpSettings =>
            GodhomeQoL.GlobalSettings.FastDreamWarp ??= new FastDreamWarpSettings();

        private Module? GetFastSuperDashModule()
        {
            if (fastSuperDashModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.FastSuperDash), out fastSuperDashModule);
            }

            return fastSuperDashModule;
        }

        private Module? GetCollectorPhasesModule()
        {
            if (collectorPhasesModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.CollectorPhases.CollectorPhases), out collectorPhasesModule);
            }

            return collectorPhasesModule;
        }

        private Module? GetFastReloadModule()
        {
            if (fastReloadModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.FastReload), out fastReloadModule);
            }

            return fastReloadModule;
        }

        private SpeedChanger? GetSpeedChangerModule()
        {
            if (speedChangerModule == null)
            {
                ModuleManager.TryGetModule(typeof(SpeedChanger), out speedChangerModule);
            }

            return speedChangerModule as SpeedChanger ?? SpeedChanger.Instance;
        }

        private Module? GetTeleportKitModule()
        {
            if (teleportKitModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.TeleportKit), out teleportKitModule);
            }

            return teleportKitModule;
        }

        private Module? GetZoteHelperModule()
        {
            if (zoteHelperModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.ZoteHelper), out zoteHelperModule);
            }

            return zoteHelperModule;
        }

        private Module? GetInfiniteChallengeModule()
        {
            if (infiniteChallengeModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.InfiniteChallenge), out infiniteChallengeModule);
            }

            return infiniteChallengeModule;
        }

        private Module? GetInfiniteGrimmPufferfishModule()
        {
            if (infiniteGrimmModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.InfiniteGrimmPufferfish), out infiniteGrimmModule);
            }

            return infiniteGrimmModule;
        }

        private Module? GetInfiniteRadianceClimbingModule()
        {
            if (infiniteRadianceModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.InfiniteRadianceClimbing), out infiniteRadianceModule);
            }

            return infiniteRadianceModule;
        }

        private Module? GetP5HealthModule()
        {
            if (p5HealthModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.P5Health), out p5HealthModule);
            }

            return p5HealthModule;
        }

        private Module? GetSegmentedP5Module()
        {
            if (segmentedP5Module == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.SegmentedP5), out segmentedP5Module);
            }

            return segmentedP5Module;
        }

        private Module? GetHalveAscendedModule()
        {
            if (halveAscendedModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.HalveDamageHoGAscendedOrAbove), out halveAscendedModule);
            }

            return halveAscendedModule;
        }

        private Module? GetHalveAttunedModule()
        {
            if (halveAttunedModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.HalveDamageHoGAttuned), out halveAttunedModule);
            }

            return halveAttunedModule;
        }

        private Module? GetAddLifebloodModule()
        {
            if (addLifebloodModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.AddLifeblood), out addLifebloodModule);
            }

            return addLifebloodModule;
        }

        private Module? GetAddSoulModule()
        {
            if (addSoulModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.AddSoul), out addSoulModule);
            }

            return addSoulModule;
        }

        private Module? GetForceGreyPrinceModule()
        {
            if (forceGreyPrinceModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.BossChallenge.ForceGreyPrinceEnterType), out forceGreyPrinceModule);
            }

            return forceGreyPrinceModule;
        }

        private Module? GetCarefreeMelodyModule()
        {
            if (carefreeMelodyModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.CarefreeMelodyReset), out carefreeMelodyModule);
            }

            return carefreeMelodyModule;
        }

        private Module? GetCollectorRoarModule()
        {
            if (collectorRoarModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.CollectorRoarMute), out collectorRoarModule);
            }

            return collectorRoarModule;
        }

        private Module? GetUnlockAllModesModule()
        {
            if (unlockAllModesModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.UnlockAllModes), out unlockAllModesModule);
            }

            return unlockAllModesModule;
        }

        private Module? GetUnlockPantheonsModule()
        {
            if (unlockPantheonsModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.UnlockPantheons), out unlockPantheonsModule);
            }

            return unlockPantheonsModule;
        }

        private Module? GetUnlockRadianceModule()
        {
            if (unlockRadianceModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.UnlockRadiance), out unlockRadianceModule);
            }

            return unlockRadianceModule;
        }

        private Module? GetUnlockRadiantModule()
        {
            if (unlockRadiantModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.UnlockRadiant), out unlockRadiantModule);
            }

            return unlockRadiantModule;
        }

        private Module? GetDoorDefaultBeginModule()
        {
            if (doorDefaultBeginModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.DoorDefaultBegin), out doorDefaultBeginModule);
            }

            return doorDefaultBeginModule;
        }

        private Module? GetMemorizeBindingsModule()
        {
            if (memorizeBindingsModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.MemorizeBindings), out memorizeBindingsModule);
            }

            return memorizeBindingsModule;
        }

        private Module? GetFasterLoadsModule()
        {
            if (fasterLoadsModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.FasterLoads), out fasterLoadsModule);
            }

            return fasterLoadsModule;
        }

        private Module? GetFastMenusModule()
        {
            if (fastMenusModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.FastMenus), out fastMenusModule);
            }

            return fastMenusModule;
        }

        private Module? GetFastTextModule()
        {
            if (fastTextModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.FastText), out fastTextModule);
            }

            return fastTextModule;
        }

        private Module? GetFastDreamWarpModule()
        {
            if (fastDreamWarpModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.FastDreamWarp), out fastDreamWarpModule);
            }

            return fastDreamWarpModule;
        }

        private Module? GetShortDeathAnimationModule()
        {
            if (shortDeathAnimationModule == null)
            {
                ModuleManager.TryGetModule(typeof(Modules.QoL.ShortDeathAnimation), out shortDeathAnimationModule);
            }

            return shortDeathAnimationModule;
        }
    }
}
