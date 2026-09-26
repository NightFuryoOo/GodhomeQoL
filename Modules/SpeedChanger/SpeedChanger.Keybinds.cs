using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Satchel.BetterMenus;
using System.Globalization;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class SpeedChanger : Module {
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

	private static string FormatKeyLabelStatic(string storedKey) {
		if (string.IsNullOrWhiteSpace(storedKey) || storedKey.Equals("Not Set", StringComparison.OrdinalIgnoreCase)) {
			return "SpeedChanger/NotSet".Localize();
		}

		return storedKey;
	}
}
