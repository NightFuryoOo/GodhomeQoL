namespace GodhomeQoL.Modules.Tools;

public sealed partial class SpeedChanger : Module {
	private static bool chainSpeedActive;
	private static bool chainSavedSwitch;
	private static float chainSavedSpeed = 1f;

	internal static bool IsChainControlled => chainSpeedActive;

	internal static void ApplyChainSpeed(float value) {
		if (!chainSpeedActive) {
			chainSpeedActive = true;
			chainSavedSwitch = globalSwitch;
			chainSavedSpeed = speed;
		}

		float target = SanitizeSpeed(value);
		bool on = !Mathf.Approximately(target, 1f);
		SpeedChanger? instance = Instance;
		if (instance == null) {
			globalSwitch = on;
			speed = on ? target : chainSavedSpeed;
			return;
		}

		instance.togglePaused = false;
		instance.waitingForSpeedInput = false;
		if (on) {
			speed = target;
			instance.ChangeGlobalSwitchState(true, true);
			if (instance.isLoaded) {
				instance.SpeedMultiplier = target;
			}
		} else {
			instance.ChangeGlobalSwitchState(false);
			speed = chainSavedSpeed;
		}

		ApplyResolvedTimeScale();
	}

	internal static void EndChainSpeed() {
		if (!chainSpeedActive) {
			return;
		}

		chainSpeedActive = false;
		speed = SanitizeSpeed(chainSavedSpeed);
		SpeedChanger? instance = Instance;
		if (instance == null) {
			globalSwitch = chainSavedSwitch;
			return;
		}

		instance.togglePaused = false;
		instance.ChangeGlobalSwitchState(chainSavedSwitch, true);
		if (chainSavedSwitch && instance.isLoaded) {
			instance.SpeedMultiplier = speed;
		}

		ApplyResolvedTimeScale();
	}
}
