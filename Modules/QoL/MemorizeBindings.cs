namespace GodhomeQoL.Modules.QoL;

public sealed class MemorizeBindings : Module {
	[LocalSetting] public static bool boundNail = false;
	[LocalSetting] public static bool boundHeart = false;
	[LocalSetting] public static bool boundCharms = false;
	[LocalSetting] public static bool boundSoul = false;
	[LocalSetting] private static bool hasRecordedBindings = false;

	internal static bool IsApplyingBindingStates { get; private set; }

	public override bool DefaultEnabled => true;

	private protected override void Load() {
		On.BossDoorChallengeUI.ShowSequence += ApplyBindingStates;
		On.BossDoorChallengeUI.HideSequence += RecordBindingStates;
	}

	private protected override void Unload() {
		On.BossDoorChallengeUI.ShowSequence -= ApplyBindingStates;
		On.BossDoorChallengeUI.HideSequence -= RecordBindingStates;
	}

	private static IEnumerator ApplyBindingStates(On.BossDoorChallengeUI.orig_ShowSequence orig, BossDoorChallengeUI self) {
		yield return orig(self);

		if (!hasRecordedBindings) {
			yield break;
		}

		IsApplyingBindingStates = true;
		try {
			SetButtonState(self.boundNailButton, boundNail);
			SetButtonState(self.boundHeartButton, boundHeart);
			SetButtonState(self.boundCharmsButton, boundCharms);
			SetButtonState(self.boundSoulButton, boundSoul);
			CaptureBindingStates(self);
			DoorDefaultBegin.TrySelectBeginAfterBindings(self);
			LogDebug("Binding states applied");
		}
		finally {
			IsApplyingBindingStates = false;
		}
	}

	private static IEnumerator RecordBindingStates(On.BossDoorChallengeUI.orig_HideSequence orig, BossDoorChallengeUI self, bool sendEvent) {
		CaptureBindingStates(self);
		LogDebug("Binding states recorded");

		yield return orig(self, sendEvent);
	}

	internal static bool TryGetRecordedShellBinding(out bool selected) {
		selected = boundHeart;
		return hasRecordedBindings;
	}

	internal static void OverrideRecordedShellBinding(bool selected) {
		if (!hasRecordedBindings) {
			return;
		}

		boundHeart = selected;
	}

	internal static void CaptureBindingStates(BossDoorChallengeUI? ui) {
		if (ui == null) {
			return;
		}

		boundNail = ui.boundNailButton != null && ui.boundNailButton.Selected;
		boundHeart = ui.boundHeartButton != null && ui.boundHeartButton.Selected;
		boundCharms = ui.boundCharmsButton != null && ui.boundCharmsButton.Selected;
		boundSoul = ui.boundSoulButton != null && ui.boundSoulButton.Selected;
		hasRecordedBindings = true;
	}

	private static void SetButtonState(BossDoorChallengeUIBindingButton self, bool state) {
		if (self != null && self.Selected != state) {
			self.OnSubmit(null);
		}
	}
}
