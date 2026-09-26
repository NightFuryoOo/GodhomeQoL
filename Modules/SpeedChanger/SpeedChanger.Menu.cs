using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Satchel.BetterMenus;
using System.Globalization;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class SpeedChanger : Module {
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
}
