using Satchel.BetterMenus;
using GodhomeQoL.Modules.BossChallenge;
using GodhomeQoL.Modules.QoL;
using GodhomeQoL.Modules.Tools;

namespace GodhomeQoL.Modules;

public sealed class FastReload : Module {
    private const string WorkshopScene = "GG_Workshop";
    private const string AtriumScene = "GG_Atrium";
    private const string AtriumRoofScene = "GG_Atrium_Roof";
    private const string SpaScene = "GG_Spa";
    private const string SetupEventUnavailableUserMessage = "Please re-enter the boss fight or die on it so that the mod works.";
    private const float SetupEventUnavailableMessageCooldownSeconds = 2f;

    [GlobalSetting]
    public static int reloadKeyCode = (int)KeyCode.None;

    private static MenuButton? reloadButton;

    private static bool waitingForReloadRebind;

    private static KeyCode reloadPrevKey;

    private static RebindListener? listener;

    private static bool suppressReloadUntilKeyRelease;

    private static bool wasQuickMenuUiVisible;
    private static float nextSetupEventUnavailableMessageAt;

    public override bool DefaultEnabled => false;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.AnyTime;

    private protected override void Load() {
        ModHooks.HeroUpdateHook += OnHeroUpdate;
        On.GameManager.BeginSceneTransition += CaptureSetupEventForTargetScene;
        On.BossSceneController.Awake += CaptureSetupEvent;
        EnsureListener();
    }

    private protected override void Unload() {
        ModHooks.HeroUpdateHook -= OnHeroUpdate;
        On.GameManager.BeginSceneTransition -= CaptureSetupEventForTargetScene;
        On.BossSceneController.Awake -= CaptureSetupEvent;
        DisposeListener();

        waitingForReloadRebind = false;
        suppressReloadUntilKeyRelease = false;
        wasQuickMenuUiVisible = false;
        nextSetupEventUnavailableMessageAt = 0f;
    }

    private static void OnHeroUpdate() {
        if (Ref.GM == null || Ref.HC == null) {
            return;
        }

        bool quickMenuUiVisible = QuickMenu.IsAnyUiVisible();
        KeyCode reloadKey = ReloadKey;
        if (reloadKey == KeyCode.None) {
            suppressReloadUntilKeyRelease = false;
            wasQuickMenuUiVisible = quickMenuUiVisible;
            return;
        }

        if (quickMenuUiVisible || (wasQuickMenuUiVisible && Input.GetKey(reloadKey))) {
            if (Input.GetKey(reloadKey)) {
                suppressReloadUntilKeyRelease = true;
            }
        }
        wasQuickMenuUiVisible = quickMenuUiVisible;

        if (quickMenuUiVisible) {
            return;
        }

        if (waitingForReloadRebind) {
            return;
        }

        if (QuickMenu.IsHotkeyInputBlocked()) {
            return;
        }

        if (suppressReloadUntilKeyRelease) {
            if (!Input.GetKey(reloadKey)) {
                suppressReloadUntilKeyRelease = false;
            }

            return;
        }

        if (Input.GetKeyUp(reloadKey)
            && BossSceneController.IsBossScene
            && !BossSequenceController.IsInSequence
            && Ref.GM.sceneName.StartsWith("GG_", StringComparison.Ordinal)
            && !Ref.GM.IsInSceneTransition
            && Ref.HC.acceptingInput) {
            _ = Ref.HC.StartCoroutine(ReloadCurrentBoss());
        }

    }

    private static IEnumerator ReloadCurrentBoss() {
        yield return null;

        if (Ref.GM.IsInSceneTransition) {
            yield break;
        }

        if (BossSequenceController.IsInSequence) {
            yield break;
        }

        string scene = Ref.GM.sceneName;

        if (string.IsNullOrEmpty(scene)
            || scene == WorkshopScene
            || !scene.StartsWith("GG_", StringComparison.Ordinal)) {
            yield break;
        }

        // If module was enabled mid-fight, capture lazily and then restore
        // scene-specific setup event before forcing the reload transition.
        BossFightRestartCompatibility.RecordCurrentSetupEvent(scene);
        if (!BossFightRestartCompatibility.TryApplySetupEventForScene(scene)) {
            if (Time.unscaledTime >= nextSetupEventUnavailableMessageAt) {
                nextSetupEventUnavailableMessageAt = Time.unscaledTime + SetupEventUnavailableMessageCooldownSeconds;
                QuickMenu.ShowStatusMessageFromExternal(SetupEventUnavailableUserMessage);
            }

            LogError("FastReload: setup event is not available; aborting reload.");
            yield break;
        }

        Ref.HC.ClearMP();
        Ref.HC.ClearMPSendEvents();
        Ref.HC.EnterWithoutInput(true);
        Ref.HC.AcceptInput();

        if (ModuleManager.TryGetLoadedModule<CarefreeMelodyReset>(out _)) {
            CarefreeMelodyReset.TryResetNow(ignoreBossScene: true);
        }

        using (BossFightRestartCompatibility.EnterFastReloadTransitionScope()) {
            Ref.GM.BeginSceneTransition(new GameManager.SceneLoadInfo {
                SceneName = scene,
                EntryGateName = "door_dreamEnter",
                EntryDelay = 0f,
                Visualization = GameManager.SceneLoadVisualizations.GodsAndGlory,
                PreventCameraFadeOut = true
            });
        }

        LogDebug($"FastReload: reloading {scene}");
    }

    #region Key handling

    private static KeyCode ReloadKey => (KeyCode)reloadKeyCode;

    private static void EnsureListener() {
        if (listener != null) {
            return;
        }

        GameObject go = new("SGQOL_FastReload_RebindListener");
        UObject.DontDestroyOnLoad(go);
        listener = go.AddComponent<RebindListener>();
    }

    private static void DisposeListener() {
        if (listener != null) {
            UObject.Destroy(listener.gameObject);
        }

        listener = null;
    }

    private static void CaptureSetupEventForTargetScene(On.GameManager.orig_BeginSceneTransition orig, GameManager self, GameManager.SceneLoadInfo info) {
        string? targetScene = info.SceneName;
        if (IsReloadCandidateScene(targetScene)) {
            // Capture setup event against the intended target scene before transition consumes it.
            BossFightRestartCompatibility.RecordCurrentSetupEvent(targetScene);
        }

        orig(self, info);
    }

    private static void CaptureSetupEvent(On.BossSceneController.orig_Awake orig, BossSceneController self) {
        BossFightRestartCompatibility.RecordCurrentSetupEvent();
        orig(self);
        BossFightRestartCompatibility.RecordCurrentSetupEvent();
    }

    private static bool IsReloadCandidateScene(string? sceneName) {
        if (sceneName == null || sceneName.Length == 0) {
            return false;
        }

        return sceneName.StartsWith("GG_", StringComparison.Ordinal)
            && sceneName != WorkshopScene
            && sceneName != AtriumScene
            && sceneName != AtriumRoofScene
            && sceneName != SpaScene;
    }

    private sealed class RebindListener : MonoBehaviour {
        private void Update() {
            if (QuickMenu.IsHotkeyInputBlocked()) {
                return;
            }

            HandleReloadRebind();
        }
    }

    internal static MenuButton ReloadBindButton() =>
        reloadButton = new MenuButton(
            FormatButtonName("Settings/reloadBossKey".Localize(), ReloadKey),
            "Settings/FastReload/ReloadDesc".Localize(),
            _ => StartReloadRebind(),
            false
        );

    private static void StartReloadRebind() {
        waitingForReloadRebind = true;
        reloadPrevKey = ReloadKey;
        UpdateReloadButton("Settings/FastReload/SetKey".Localize());
    }

    private static void HandleReloadRebind() {
        if (!waitingForReloadRebind) {
            return;
        }

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode))) {
            if (!Input.GetKeyDown(key)) {
                continue;
            }

            if (key == KeyCode.Escape) {
                waitingForReloadRebind = false;
                UpdateReloadButton(FormatKeyLabel(ReloadKey));
                return;
            }

            if (key != reloadPrevKey
                && QuickMenu.TryGetHotkeyConflictOwnersExceptSelf(
                    key,
                    "Settings/reloadBossKey".Localize(),
                    out string conflictOwners))
            {
                LogWarn($"FastReload hotkey {FormatKeyLabel(key)} is already used by: {conflictOwners}");
                UpdateReloadButton("Settings/FastReload/SetKey".Localize());
                return;
            }

            reloadKeyCode = key == reloadPrevKey
                ? (int)KeyCode.None
                : (int)key;

            waitingForReloadRebind = false;
            UpdateReloadButton(FormatKeyLabel(ReloadKey));
            GodhomeQoL.MarkMenuDirty();
            return;
        }
    }

    private static void UpdateReloadButton(string value) {
        if (reloadButton == null) {
            return;
        }

        reloadButton.Name = FormatButtonName("Settings/reloadBossKey".Localize(), value);
        reloadButton.Update();
    }

    private static string FormatButtonName(string title, string value) => $"{title}: {value}";

    private static string FormatButtonName(string title, KeyCode key) => $"{title}: {FormatKeyLabel(key)}";

    private static string FormatKeyLabel(KeyCode key) => key == KeyCode.None
        ? "Settings/FastReload/NotSet".Localize()
        : key.ToString();

    #endregion
}
