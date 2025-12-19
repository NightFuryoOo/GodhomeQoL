<<<<<<< HEAD
namespace GodhomeQoL.Modules.BossChallenge;

public sealed class AddLifeblood : Module {
	[GlobalSetting]
=======
namespace SafeGodseekerQoL.Modules.BossChallenge;

public sealed class AddLifeblood : Module {
	[GlobalSetting]
	[IntOption(0, 35, OptionType.Slider)]
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265
	public static int lifebloodAmount = 0;

	public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

	private protected override void Load() =>
		On.BossSceneController.Start += Add;

	private protected override void Unload() =>
		On.BossSceneController.Start -= Add;

	private static IEnumerator Add(On.BossSceneController.orig_Start orig, BossSceneController self) {
		yield return orig(self);

		if (BossSequenceController.IsInSequence) {
			yield break;
		}

		Add();
	}

	internal static void Add() {
		FixBlueHealthFSM();

		for (int i = 0; i < lifebloodAmount; i++) {
			EventRegister.SendEvent("ADD BLUE HEALTH");
		}

		LogDebug("Lifeblood added");
	}

<<<<<<< HEAD
	
=======
	// Fix for Toggleable Bindings Shell Binding bug.
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265
	private static void FixBlueHealthFSM() {
		PlayMakerFSM fsm = Ref.GC.hudCanvas.Child("Health")!.LocateMyFSM("Blue Health Control");
		if (fsm.ActiveStateName == "Wait") {
			fsm.SendEvent("LAST HP ADDED");
		}
	}
}
