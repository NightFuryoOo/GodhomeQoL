using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Satchel.BetterMenus;
using System.Globalization;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class SpeedChanger : Module {
	internal static void SuppressToggleUntilRelease(KeyCode key) {
		suppressToggleKey = key;
	}

	internal static void SuppressInputUntilRelease(KeyCode key) {
		suppressInputKey = key;
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
}
