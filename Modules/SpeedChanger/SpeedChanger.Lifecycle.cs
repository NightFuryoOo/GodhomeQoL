using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Satchel.BetterMenus;
using System.Globalization;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class SpeedChanger : Module {
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
}
