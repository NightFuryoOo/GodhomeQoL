using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Satchel.BetterMenus;
using System.Globalization;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class SpeedChanger : Module {
	private static float NormalizeSpeed(float value) => (float)Math.Round(value, 2);
	private static bool IsFinitePositive(float value) => value > 0f && !float.IsNaN(value) && !float.IsInfinity(value);

	private static float SanitizeSpeed(float value, float fallback = 1f) {
		if (!IsFinitePositive(value)) {
			return fallback;
		}

		float normalized = NormalizeSpeed(value);
		return IsFinitePositive(normalized) ? normalized : fallback;
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
}
