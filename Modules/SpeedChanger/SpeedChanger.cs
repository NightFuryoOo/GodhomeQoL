using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Satchel.BetterMenus;
using System.Globalization;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed class SpeedChanger : Module {
	private static readonly string[] ToggleKeyOptions = new[] {
		"F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12",
		"BackQuote", "Tab", "CapsLock", "LeftShift", "RightShift", "LeftControl", "RightControl", "LeftAlt", "RightAlt", "Space",
		"Alpha1", "Alpha2", "Alpha3", "Alpha4", "Alpha5", "Alpha6", "Alpha7", "Alpha8", "Alpha9", "Alpha0",
		"Minus", "Equals",
		"Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P",
		"A", "S", "D", "F", "G", "H", "J", "K", "L",
		"Z", "X", "C", "V", "B", "N", "M",
		"Insert", "Home", "PageUp", "Delete", "End", "PageDown",
		"UpArrow", "DownArrow", "LeftArrow", "RightArrow"
	};

	private static readonly string[] AllowedRoomsForToggle = new[] {
		"GG_Workshop",
		"GG_Atrium",
		"GG_Atrium_Roof"
	};

	private static readonly MethodInfo[] FreezeCoroutines = (
		from method in typeof(GameManager).GetMethods()
		where method.Name.StartsWith("FreezeMoment", StringComparison.Ordinal)
		where method.ReturnType == typeof(IEnumerator)
		select method.GetCustomAttribute<IteratorStateMachineAttribute>() into attr
		select attr.StateMachineType into type
		select type.GetMethod("MoveNext", BindingFlags.NonPublic | BindingFlags.Instance)
	).ToArray();

	internal static SpeedChanger? Instance { get; private set; }
	private static readonly Dictionary<int, float> timeScaleOverrideValues = new();
	private static readonly HashSet<int> timeScaleFreezeLocks = new();
	private static int nextTimeScaleOverrideHandle;
	private static int nextFreezeLockHandle;

	public override bool DefaultEnabled => false;
	public override bool Hidden => true;
	public override bool AlwaysEnabled => true;

	[GlobalSetting] public static bool globalSwitch = false;
	[GlobalSetting] public static bool restrictToggleToRooms = false;
	[GlobalSetting] public static bool unlimitedSpeed = false;
	[GlobalSetting] public static int displayStyle = 0;
	[GlobalSetting] public static string toggleKeybind = string.Empty;
	[GlobalSetting] public static string inputSpeedKeybind = string.Empty;
	[GlobalSetting] public static float speed = 1f;

	private KeyCode toggleKey;
	private KeyCode inputSpeedKey;
	private static KeyCode suppressToggleKey = KeyCode.None;
	private static KeyCode suppressInputKey = KeyCode.None;

	private bool togglePaused;
	private bool togglePrevOverlayVisible;
	private bool togglePrevDisplayVisible;

	private bool waitingForToggleRebind;
	private KeyCode currentToggleKeyBeforeRebind;
	private static readonly string[] toggleMenuValues = new string[1];

	private bool waitingForInputRebind;
	private KeyCode currentInputKeyBeforeRebind;
	private static readonly string[] inputMenuValues = new string[1];

	private bool waitingForSpeedInput;
	private string speedInputBuffer = string.Empty;
	private bool wasPausedLastTick;

	private bool isLoaded;
	private ILHook[]? coroutineHooks;
	private RebindListener? rebindListener;

	private static HorizontalOption? toggleOption;
	private static HorizontalOption? inputOption;

	public SpeedChanger() => Instance = this;

	private protected override void Load() {
		RefreshKeybinds();
		EnsureRebindListener();
		wasPausedLastTick = IsGamePausedNow();
		ChangeGlobalSwitchState(globalSwitch, true);
	}

	private protected override void Unload() {
		DisableSpeedChanger();
		DisposeRebindListener();
	}

	private static KeyCode ParseKeycode(string value, KeyCode fallback) {
		if (string.IsNullOrWhiteSpace(value) || value.Equals("Not Set", StringComparison.OrdinalIgnoreCase)) {
			return KeyCode.None;
		}

		return Enum.TryParse(value, true, out KeyCode parsed) ? parsed : fallback;
	}

	private void RefreshKeybinds() {
		toggleKey = ParseKeycode(toggleKeybind, KeyCode.F10);
		inputSpeedKey = ParseKeycode(inputSpeedKeybind, KeyCode.F11);
	}

	private void ChangeGlobalSwitchState(bool state, bool force = false) {
		if (!force && globalSwitch == state && isLoaded == state) {
			return;
		}

		globalSwitch = state;

		if (!state) {
			togglePaused = false;
			togglePrevOverlayVisible = false;
			togglePrevDisplayVisible = false;
			if (isLoaded) {
				DisableSpeedChanger();
			}
			return;
		}

		if (!isLoaded) {
			EnableSpeedChanger();
		}
	}

	private static float NormalizeSpeed(float value) => (float)Math.Round(value, 2);
	private static bool IsFinitePositive(float value) => value > 0f && !float.IsNaN(value) && !float.IsInfinity(value);

	private static float SanitizeSpeed(float value, float fallback = 1f) {
		if (!IsFinitePositive(value)) {
			return fallback;
		}

		float normalized = NormalizeSpeed(value);
		return IsFinitePositive(normalized) ? normalized : fallback;
	}

	private float SpeedMultiplier {
		get => globalSwitch ? speed : 1f;
		set {
			if (value <= 0f) {
				return;
			}

			float normalized = SanitizeSpeed(value);

			if (Time.timeScale != 0f) {
				Time.timeScale = normalized;
			}

			speed = normalized;
		}
	}

	internal static bool IsTimeScaleFrozen => timeScaleFreezeLocks.Count > 0;

	internal static bool TryBeginTimeScaleOverride(float value, out int handle) {
		handle = 0;

		if (value <= 0f || float.IsNaN(value) || float.IsInfinity(value)) {
			return false;
		}

		handle = ++nextTimeScaleOverrideHandle;
		timeScaleOverrideValues[handle] = value;
		ApplyResolvedTimeScale();
		return true;
	}

	internal static void EndTimeScaleOverride(int handle) {
		if (handle == 0 || !timeScaleOverrideValues.Remove(handle)) {
			return;
		}

		ApplyResolvedTimeScale();
	}

	internal static int BeginTimeScaleFreezeLock() {
		int handle = ++nextFreezeLockHandle;
		timeScaleFreezeLocks.Add(handle);
		ApplyResolvedTimeScale();
		return handle;
	}

	internal static void EndTimeScaleFreezeLock(int handle) {
		if (handle == 0 || !timeScaleFreezeLocks.Remove(handle)) {
			return;
		}

		ApplyResolvedTimeScale();
	}

	internal static void SuppressToggleUntilRelease(KeyCode key) {
		suppressToggleKey = key;
	}

	internal static void SuppressInputUntilRelease(KeyCode key) {
		suppressInputKey = key;
	}

	private static bool HasTimeScaleOverride => timeScaleOverrideValues.Count > 0;
	private static bool HasManagedTimeScale => timeScaleFreezeLocks.Count > 0 || HasTimeScaleOverride;
	internal static bool HasManagedTimeScaleControl => HasManagedTimeScale;

	private static float GetLatestOverrideScale() {
		int latestHandle = int.MinValue;
		float scale = 1f;

		foreach ((int handle, float value) in timeScaleOverrideValues) {
			if (handle > latestHandle) {
				latestHandle = handle;
				scale = value;
			}
		}

		return scale;
	}

	private static bool IsGamePausedNow() =>
		GameManager.instance != null && GameManager.instance.IsGamePaused();

	private static void ApplyResolvedTimeScale() {
		if (timeScaleFreezeLocks.Count > 0) {
			Time.timeScale = 0f;
			return;
		}

		if (IsGamePausedNow()) {
			return;
		}

		if (HasTimeScaleOverride) {
			Time.timeScale = GetLatestOverrideScale();
			return;
		}

		SpeedChanger? instance = Instance;
		if (instance == null || !instance.isLoaded) {
			Time.timeScale = 1f;
			return;
		}

		if (instance.togglePaused || !globalSwitch) {
			Time.timeScale = 1f;
			return;
		}

		float baseSpeed = SanitizeSpeed(speed);
		if (!Mathf.Approximately(baseSpeed, speed)) {
			speed = baseSpeed;
		}

		Time.timeScale = baseSpeed;
	}

	private static bool IsFreezeScalingActive() {
		if (HasManagedTimeScale) {
			return false;
		}

		SpeedChanger? instance = Instance;
		if (instance == null || !instance.isLoaded) {
			return false;
		}

		if (!globalSwitch || instance.togglePaused) {
			return false;
		}

		return IsFinitePositive(speed);
	}

	private static float GetFreezeScale() => IsFreezeScalingActive() ? speed : 1f;

	private void EnableSpeedChanger() {
		if (isLoaded) {
			return;
		}

		isLoaded = true;
		RefreshKeybinds();
		speed = SanitizeSpeed(speed);
		SpeedMultiplier = speed;

		ModHooks.HeroUpdateHook += OnHeroUpdate;

		coroutineHooks = new ILHook[FreezeCoroutines.Length];
		foreach ((MethodInfo coro, int idx) in FreezeCoroutines.Select((mi, idx) => (mi, idx))) {
			coroutineHooks[idx] = new ILHook(coro, ScaleFreeze);
		}

		ModDisplay.Instance?.Destroy();
		ModDisplay.Instance = null;
		EnsureDisplay();
	}

	private void DisableSpeedChanger() {
		if (!isLoaded) {
			return;
		}

		isLoaded = false;

		if (ModDisplay.Instance != null) {
			ModDisplay.Instance.Destroy();
			ModDisplay.Instance = null;
		}

		if (coroutineHooks != null) {
			foreach (ILHook hook in coroutineHooks) {
				hook?.Dispose();
			}
		}

		ModHooks.HeroUpdateHook -= OnHeroUpdate;
		wasPausedLastTick = IsGamePausedNow();

		if (!HasManagedTimeScale && Time.timeScale != 0f) {
			Time.timeScale = 1f;
		} else {
			ApplyResolvedTimeScale();
		}
	}

	private void OnHeroUpdate() {
		if (HasManagedTimeScale) {
			ApplyResolvedTimeScale();
			return;
		}

		if (togglePaused) {
			if (Time.timeScale != 0f) {
				Time.timeScale = 1f;
			}
			return;
		}

		UpdateSpeedDisplay();
		SpeedMultiplier = SpeedMultiplier;

		if (QuickMenu.IsHotkeyInputBlocked() || QuickMenu.IsAnyUiVisible()) {
			return;
		}
	}

	private void ListenForToggle() {
		if (toggleKey != KeyCode.None && toggleKey == suppressToggleKey) {
			if (Input.GetKey(toggleKey)) {
				return;
			}

			suppressToggleKey = KeyCode.None;
		}

		if (Input.GetKeyDown(toggleKey)) {
			if (!globalSwitch) {
				return;
			}

			bool inMainMenu = IsInMainMenu();

			if (!inMainMenu && restrictToggleToRooms && !IsInAllowedRoom()) {
				return;
			}

			togglePaused = !togglePaused;

				if (togglePaused) {
					togglePrevOverlayVisible = QuickMenu.IsSpeedChangerOverlayVisible();
					if (togglePrevOverlayVisible) {
						QuickMenu.SetSpeedChangerOverlayVisible(false);
					}

				togglePrevDisplayVisible = ModDisplay.Instance != null;
				if (togglePrevDisplayVisible) {
					ModDisplay.Instance?.Destroy();
					ModDisplay.Instance = null;
				}

					if (!HasManagedTimeScale && Time.timeScale != 0f) {
						Time.timeScale = 1f;
					}
				} else {
					if (!HasManagedTimeScale) {
						SpeedMultiplier = speed;
					} else {
						ApplyResolvedTimeScale();
					}

				if (togglePrevDisplayVisible && displayStyle != 2) {
					ModDisplay.Instance ??= new ModDisplay();
				}

				if (togglePrevOverlayVisible) {
					QuickMenu.SetSpeedChangerOverlayVisible(true);
				}

				togglePrevOverlayVisible = false;
				togglePrevDisplayVisible = false;
			}
		}

		if (togglePaused) {
			return;
		}

		if (inputSpeedKey != KeyCode.None && inputSpeedKey == suppressInputKey) {
			if (Input.GetKey(inputSpeedKey)) {
				return;
			}

			suppressInputKey = KeyCode.None;
		}

		if (inputSpeedKey != KeyCode.None && Input.GetKeyDown(inputSpeedKey)) {
			if (waitingForSpeedInput) {
				waitingForSpeedInput = false;
				speedInputBuffer = string.Empty;
				ModDisplay.Instance?.HideInput();
				return;
			}

			if (!globalSwitch) {
				return;
			}

			bool inMainMenu = IsInMainMenu();

			if (!inMainMenu && restrictToggleToRooms && !IsInAllowedRoom()) {
				return;
			}

			StartSpeedEntry();
		}
	}

	private void StartToggleRebind() {
		toggleMenuValues[0] = "Set Key...";
		UpdateToggleButton(toggleMenuValues[0]);
		waitingForToggleRebind = true;
		currentToggleKeyBeforeRebind = toggleKey;
		if (ModDisplay.Instance != null && displayStyle != 2) {
			ModDisplay.Instance.Display("Press a key for toggle (Esc to cancel)");
		}
	}

	private bool HandleToggleRebind() {
		if (!waitingForToggleRebind) {
			return false;
		}

		foreach (KeyCode key in Enum.GetValues(typeof(KeyCode))) {
			if (!Input.GetKeyDown(key)) {
				continue;
			}

			if (key == KeyCode.Escape) {
				waitingForToggleRebind = false;
				UpdateToggleButton(FormatKeyLabel(toggleKeybind));
				return true;
			}

			if (!TryApplyRebindKey(
				key,
				currentToggleKeyBeforeRebind,
				"SpeedChanger/ToggleKey".Localize(),
				"toggle",
				value => toggleKeybind = value,
				() => UpdateToggleButton("Set Key...")
			)) {
				return true;
			}

			RefreshKeybinds();
			waitingForToggleRebind = false;
			SuppressToggleUntilRelease(toggleKey);

			string prev = toggleMenuValues[0];
			toggleMenuValues[0] = FormatKeyLabel(toggleKeybind);
			UpdateToggleButton(toggleMenuValues[0], prev);

			if (ModDisplay.Instance != null && displayStyle != 2) {
				ModDisplay.Instance.Display(string.IsNullOrEmpty(toggleKeybind)
					? "Toggle key cleared"
					: $"Toggle key set to {toggleKeybind}");
			}

			GodhomeQoL.MarkMenuDirty();
			return true;
		}

		return true;
	}

	private void StartInputRebind() {
		inputMenuValues[0] = "Set Key...";
		UpdateInputButton(inputMenuValues[0]);
		waitingForInputRebind = true;
		currentInputKeyBeforeRebind = inputSpeedKey;
		if (ModDisplay.Instance != null && displayStyle != 2) {
			ModDisplay.Instance.Display("Press a key for input (Esc to cancel)");
		}
	}

	private bool HandleInputRebind() {
		if (!waitingForInputRebind) {
			return false;
		}

		foreach (KeyCode key in Enum.GetValues(typeof(KeyCode))) {
			if (!Input.GetKeyDown(key)) {
				continue;
			}

			if (key == KeyCode.Escape) {
				waitingForInputRebind = false;
				UpdateInputButton(FormatKeyLabel(inputSpeedKeybind));
				return true;
			}

			if (!TryApplyRebindKey(
				key,
				currentInputKeyBeforeRebind,
				"SpeedChanger/InputKey".Localize(),
				"input",
				value => inputSpeedKeybind = value,
				() => UpdateInputButton("Set Key...")
			)) {
				return true;
			}

			RefreshKeybinds();
			waitingForInputRebind = false;
			SuppressInputUntilRelease(inputSpeedKey);

			string prev = inputMenuValues[0];
			inputMenuValues[0] = FormatKeyLabel(inputSpeedKeybind);
			UpdateInputButton(inputMenuValues[0], prev);

			if (ModDisplay.Instance != null && displayStyle != 2) {
				ModDisplay.Instance.Display(string.IsNullOrEmpty(inputSpeedKeybind)
					? "Input key cleared"
					: $"Input key set to {inputSpeedKeybind}");
			}

			GodhomeQoL.MarkMenuDirty();
			return true;
		}

		return true;
	}

	private void EnsureRebindListener() {
		if (rebindListener != null) {
			return;
		}

		GameObject go = new("SGQOL_SpeedChanger_RebindListener");
		UObject.DontDestroyOnLoad(go);
			rebindListener = go.AddComponent<RebindListener>();
			rebindListener.Tick = () => {
				bool isPausedNow = IsGamePausedNow();
				if (wasPausedLastTick && !isPausedNow) {
					ApplyResolvedTimeScale();
				}
				wasPausedLastTick = isPausedNow;

				if (QuickMenu.IsHotkeyInputBlocked() || QuickMenu.IsAnyUiVisible()) {
					UpdateSpeedDisplay();
					return;
				}

			bool consumedInput = HandleToggleRebind()
				|| HandleInputRebind()
				|| HandleSpeedEntry();

			if (!consumedInput) {
				ListenForToggle();
			}

			UpdateSpeedDisplay();
		};
	}

	private void DisposeRebindListener() {
		if (rebindListener != null) {
			UObject.Destroy(rebindListener.gameObject);
		}

		rebindListener = null;
	}

	private string FormatKeyLabel(string storedKey) {
		if (string.IsNullOrWhiteSpace(storedKey) || storedKey.Equals("Not Set", StringComparison.OrdinalIgnoreCase)) {
			return "SpeedChanger/NotSet".Localize();
		}

		return storedKey;
	}

	private bool IsKeyInUse(string keyName, string except = "") {
		if (string.IsNullOrWhiteSpace(keyName)) {
			return false;
		}

		bool inToggle = !except.Equals("toggle", StringComparison.OrdinalIgnoreCase) && string.Equals(toggleKeybind, keyName, StringComparison.OrdinalIgnoreCase);
		bool inInput = !except.Equals("input", StringComparison.OrdinalIgnoreCase) && string.Equals(inputSpeedKeybind, keyName, StringComparison.OrdinalIgnoreCase);

		return inToggle || inInput;
	}

	private static string FormatRuntimeKeyLabel(KeyCode key) => key == KeyCode.None
		? "SpeedChanger/NotSet".Localize()
		: key.ToString();

	private bool TryApplyRebindKey(
		KeyCode key,
		KeyCode previousKey,
		string selfOwner,
		string internalSlot,
		Action<string> applyKeybind,
		Action onConflict
	) {
		if (key == previousKey) {
			applyKeybind(string.Empty);
			return true;
		}

		string keyName = key.ToString();
		if (IsKeyInUse(keyName, internalSlot)) {
			applyKeybind(string.Empty);
			return true;
		}

		if (QuickMenu.TryGetHotkeyConflictOwnersExceptSelf(key, selfOwner, out string owners)) {
			if (ModDisplay.Instance != null && displayStyle != 2) {
				ModDisplay.Instance.Display($"HOTKEY {FormatRuntimeKeyLabel(key)} occupied by: {owners}");
			}
			onConflict();
			return false;
		}

		applyKeybind(keyName);
		return true;
	}

	private bool IsInAllowedRoom() {
		string? sceneName = GameManager.instance?.GetSceneNameString();
		return !string.IsNullOrEmpty(sceneName) && AllowedRoomsForToggle.Contains(sceneName);
	}

	private static bool IsInMainMenu() {
		string? sceneName = GameManager.instance?.GetSceneNameString();
		return sceneName == "Menu_Title";
	}

	private void StartSpeedEntry() {
		waitingForSpeedInput = true;
		speedInputBuffer = string.Empty;

		if (displayStyle != 2) {
			EnsureDisplay();
			ModDisplay.Instance?.Display("Enter speed % (digits, Enter=apply, Esc=cancel)");
			ModDisplay.Instance?.DisplayInput("SetGameSpeed: ");
		}
	}

	private bool HandleSpeedEntry() {
		if (!waitingForSpeedInput) {
			return false;
		}

		if (inputSpeedKey != KeyCode.None && Input.GetKeyDown(inputSpeedKey)) {
			waitingForSpeedInput = false;
			speedInputBuffer = string.Empty;
			ModDisplay.Instance?.HideInput();
			return true;
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			waitingForSpeedInput = false;
			ModDisplay.Instance?.HideInput();
			return true;
		}

		if (Input.GetKeyDown(KeyCode.Backspace)) {
			if (speedInputBuffer.Length > 0) {
				speedInputBuffer = speedInputBuffer.Substring(0, speedInputBuffer.Length - 1);
				ModDisplay.Instance?.DisplayInput("SetGameSpeed: " + speedInputBuffer);
			}
			return true;
		}

		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
			if (speedInputBuffer.Length == 0) {
				waitingForSpeedInput = false;
				ModDisplay.Instance?.HideInput();
				return true;
			}

			if (int.TryParse(speedInputBuffer, out int percent) && percent > 0) {
				if (!unlimitedSpeed && percent > 10000) {
					waitingForSpeedInput = false;
					ModDisplay.Instance?.HideInput();
					return true;
				}

				float newSpeed = percent / 100f;
				SpeedMultiplier = newSpeed;

				if (ModDisplay.Instance != null && displayStyle != 2) {
					ModDisplay.Instance.Display($"Set speed to {percent}%");
				}
			}

			waitingForSpeedInput = false;
			ModDisplay.Instance?.HideInput();
			return true;
		}

		for (KeyCode key = KeyCode.Alpha0; key <= KeyCode.Alpha9; key++) {
			if (Input.GetKeyDown(key)) {
				speedInputBuffer += (char)('0' + (key - KeyCode.Alpha0));
				ModDisplay.Instance?.DisplayInput("SetGameSpeed: " + speedInputBuffer);
				return true;
			}
		}
		for (KeyCode key = KeyCode.Keypad0; key <= KeyCode.Keypad9; key++) {
			if (Input.GetKeyDown(key)) {
				speedInputBuffer += (char)('0' + (key - KeyCode.Keypad0));
				ModDisplay.Instance?.DisplayInput("SetGameSpeed: " + speedInputBuffer);
				return true;
			}
		}

		return true;
	}

	private void UpdateSpeedDisplay() {
		if (togglePaused || displayStyle == 2 || !globalSwitch) {
			return;
		}

		EnsureDisplay();

		string speedString = displayStyle == 0
			? SpeedMultiplier.ToString(SpeedMultiplier >= 10f ? "00.00" : "0.00", CultureInfo.InvariantCulture)
			: (Math.Round(SpeedMultiplier * 100)).ToString("0.##\\%");

		ModDisplay.Instance!.Display($"Game Speed: {speedString}");
	}

	private void ScaleFreeze(ILContext il) {
		ILCursor cursor = new(il);

		if (!cursor.TryGotoNext(
			MoveType.After,
			x => x.MatchLdfld(out _),
			x => x.MatchCall<Time>("get_unscaledDeltaTime")
		)) {
			LogWarn("SpeedChanger: Freeze scaling IL pattern not found, skipped one FreezeMoment coroutine.");
			return;
		}

		try {
			cursor.EmitDelegate<Func<float>>(GetFreezeScale);
			cursor.Emit(OpCodes.Mul);
		} catch (Exception e) {
			LogWarn($"SpeedChanger: Failed to inject freeze scaling IL hook - {e.Message}");
		}
	}

	private void EnsureDisplay() {
		if (displayStyle == 2) {
			return;
		}

		if (ModDisplay.Instance == null) {
			ModDisplay.Instance = new ModDisplay();
		}
	}

	private static string ToggleLabel => "SpeedChanger/ToggleKey".Localize();
	private static string InputLabel => "SpeedChanger/InputKey".Localize();

	private static void UpdateHorizontalOption(HorizontalOption? option, string label, string[] values) {
		if (option == null) {
			return;
		}

		option.Name = label;
		option.Values = values;
		if (option.gameObject != null) {
			option.Update();
		}
	}

	private static void UpdateToggleButton(string value, string? previousValue = null) {
		toggleMenuValues[0] = value;
		UpdateHorizontalOption(toggleOption, ToggleLabel, toggleMenuValues);
	}

	private static void UpdateInputButton(string value, string? previousValue = null) {
		inputMenuValues[0] = value;
		UpdateHorizontalOption(inputOption, InputLabel, inputMenuValues);
	}

	private sealed class RebindListener : MonoBehaviour {
		public Action? Tick;

		private void Update() => Tick?.Invoke();
	}

	internal static MenuScreen GetMenu(MenuScreen parent) {
		_ = ModuleManager.TryGetModule(typeof(SpeedChanger), out Module? module);
		SpeedChanger? mod = module as SpeedChanger ?? Instance;

		string globalSwitchLabel = "SpeedChanger/GlobalSwitch".Localize();
		string restrictLabel = "SpeedChanger/RestrictRooms".Localize();
		string unlimitedLabel = "SpeedChanger/UnlimitedSpeed".Localize();
		string displayLabel = "SpeedChanger/DisplayStyle".Localize();

		string KeyLabel(string stored) => mod?.FormatKeyLabel(stored) ?? FormatKeyLabelStatic(stored);

		toggleMenuValues[0] = KeyLabel(toggleKeybind);
		inputMenuValues[0] = KeyLabel(inputSpeedKeybind);

		toggleOption = new HorizontalOption(
			ToggleLabel,
			"SpeedChanger/ToggleDesc".Localize(),
			toggleMenuValues,
			_ => mod?.StartToggleRebind(),
			() => {
				toggleMenuValues[0] = KeyLabel(toggleKeybind);
				return 0;
			}
		);

		inputOption = new HorizontalOption(
			InputLabel,
			"SpeedChanger/InputDesc".Localize(),
			inputMenuValues,
			_ => mod?.StartInputRebind(),
			() => {
				inputMenuValues[0] = KeyLabel(inputSpeedKeybind);
				return 0;
			}
		);

		Menu menu = new("SpeedChanger".Localize(), [
			new HorizontalOption(
				globalSwitchLabel,
				"",
				new[] { "Off", "On" },
				opt => mod?.ChangeGlobalSwitchState(opt == 1),
				() => globalSwitch ? 1 : 0
			),
			new HorizontalOption(
				restrictLabel,
				"",
				new[] { "Off", "On" },
				opt => restrictToggleToRooms = opt == 1,
				() => restrictToggleToRooms ? 1 : 0
			),
			toggleOption,
			new HorizontalOption(
				unlimitedLabel,
				"",
				new[] { "Off", "On" },
				opt => unlimitedSpeed = opt == 1,
				() => unlimitedSpeed ? 1 : 0
			),
			inputOption,
				new HorizontalOption(
					displayLabel,
					"",
				new[] { "#.##", "%", "Off" },
				opt => {
					displayStyle = opt;
					if (opt == 2 && ModDisplay.Instance != null) {
						ModDisplay.Instance.Destroy();
						ModDisplay.Instance = null;
					} else if (opt != 2) {
						ModDisplay.Instance ??= new ModDisplay();
					}
					},
					() => displayStyle
				)
			]);

		return menu.GetMenuScreen(parent);
	}

	private static string FormatKeyLabelStatic(string storedKey) {
		if (string.IsNullOrWhiteSpace(storedKey) || storedKey.Equals("Not Set", StringComparison.OrdinalIgnoreCase)) {
			return "SpeedChanger/NotSet".Localize();
		}

		return storedKey;
	}
}

internal sealed class ModDisplay {
	internal static ModDisplay? Instance;

	private string DisplayText = "";
	private Vector2 TextSize = new(800, 500);
	private Vector2 TextPosition = new(0.22f, 0.243f);
	private Vector2 InputTextSize = new(500, 80);
	private Vector2 InputTextPosition = new(0.02f, 0.08f);

	private GameObject? canvas;
	private UnityEngine.UI.Text? text;
	private UnityEngine.UI.Text? inputText;

	public ModDisplay() => Create();

	private void Create() {
		if (canvas != null) {
			return;
		}

		canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));

		CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;

		UObject.DontDestroyOnLoad(canvas);

		text = CanvasUtil.CreateTextPanel(
			canvas, "", 24, TextAnchor.LowerLeft,
			new CanvasUtil.RectData(TextSize, Vector2.zero, TextPosition, TextPosition),
			CanvasUtil.GetFont("Perpetua")
		).GetComponent<UnityEngine.UI.Text>();

		inputText = CanvasUtil.CreateTextPanel(
			canvas, "", 22, TextAnchor.LowerLeft,
			new CanvasUtil.RectData(InputTextSize, new Vector2(30, 30), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f)),
			CanvasUtil.GetFont("Perpetua")
		).GetComponent<UnityEngine.UI.Text>();
		inputText.color = new Color(1f, 1f, 1f, 0.9f);
		inputText.gameObject.SetActive(false);
	}

	public void Destroy() {
		if (canvas != null) {
			UObject.Destroy(canvas);
		}

		canvas = null;
		text = null;
		inputText = null;
	}

	public void Update() {
		if (text != null && canvas != null) {
			text.text = DisplayText;
			canvas.SetActive(true);
		}
	}

	public void DisplayInput(string value) {
		if (inputText == null) {
			return;
		}

		inputText.text = value;
		inputText.gameObject.SetActive(true);
	}

	public void HideInput() {
		if (inputText == null) {
			return;
		}

		inputText.gameObject.SetActive(false);
	}

	public void Display(string value) {
		DisplayText = value.Trim();
		Update();
	}
}

internal static class CanvasUtil {
	internal struct RectData {
		internal Vector2 size;
		internal Vector2 position;
		internal Vector2 anchorMin;
		internal Vector2 anchorMax;
		internal Vector2 pivot;

		internal RectData(Vector2 size, Vector2 position, Vector2 anchorMin, Vector2 anchorMax, Vector2? pivot = null) {
			this.size = size;
			this.position = position;
			this.anchorMin = anchorMin;
			this.anchorMax = anchorMax;
			this.pivot = pivot ?? new Vector2(0.5f, 0.5f);
		}
	}

	internal static GameObject CreateCanvas(RenderMode renderMode, Vector2 referenceResolution, string name = "SpeedChangerCanvas", int sortOrder = 9999) {
		GameObject canvasObject = new(name);
		Canvas canvas = canvasObject.AddComponent<Canvas>();
		canvas.renderMode = renderMode;
		canvas.sortingOrder = sortOrder;
		canvas.pixelPerfect = false;

		CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = referenceResolution;
		scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

		canvasObject.AddComponent<GraphicRaycaster>();
		canvasObject.AddComponent<CanvasGroup>();

		return canvasObject;
	}

	internal static GameObject CreateTextPanel(GameObject parent, string text, int fontSize, TextAnchor anchor, RectData rectData, Font? font = null) {
		GameObject go = new("SpeedChangerText");
		go.transform.SetParent(parent.transform, false);

		UnityEngine.UI.Text txt = go.AddComponent<UnityEngine.UI.Text>();
		txt.text = text;
		txt.fontSize = fontSize;
		txt.alignment = anchor;
		txt.horizontalOverflow = HorizontalWrapMode.Overflow;
		txt.verticalOverflow = VerticalWrapMode.Overflow;
		txt.font = font ?? GetFont("Arial");
		txt.color = Color.white;

		RectTransform rect = go.GetComponent<RectTransform>();
		rect.sizeDelta = rectData.size;
		rect.pivot = rectData.pivot;
		rect.anchorMin = rectData.anchorMin;
		rect.anchorMax = rectData.anchorMax;
		rect.anchoredPosition = rectData.position;

		return go;
	}

	internal static Font GetFont(string fontName) {
		Font? existing = Resources.FindObjectsOfTypeAll<Font>().FirstOrDefault(f => f != null && f.name.Equals(fontName, StringComparison.OrdinalIgnoreCase));
		if (existing != null) {
			return existing;
		}

		try {
			Font? builtin = Resources.GetBuiltinResource<Font>("Arial.ttf");
			if (builtin != null) {
				return builtin;
			}
		} catch {
			// ignored
		}

		return Font.CreateDynamicFontFromOSFont("Arial", 14);
	}
}
