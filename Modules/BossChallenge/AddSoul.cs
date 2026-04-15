namespace GodhomeQoL.Modules.BossChallenge;

public sealed class AddSoul : Module {
	[GlobalSetting]
	public static int soulAmount = 0;

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

		if (Ref.HC == null || soulAmount <= 0) {
			yield break;
		}

		_ = Ref.HC.StartCoroutine(ApplySoulWhenReady(soulAmount));
	}

	private static IEnumerator ApplySoulWhenReady(int amount) {
		yield return new WaitUntil(() => Ref.GM != null && Ref.GM.gameState == GameState.PLAYING);
		// Let restart flows normalize MP for this frame first, then apply configured start soul.
		yield return null;

		if (Ref.HC == null) {
			yield break;
		}

		Ref.HC.AddMPCharge(amount);

		Ref.GC?.soulOrbFSM?.SendEvent("MP GAIN SPA");
		Ref.GC?.soulVesselFSM?.SendEvent("MP RESERVE UP");

		LogDebug("Soul added");
	}
}
