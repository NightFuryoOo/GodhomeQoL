<<<<<<< HEAD
using GodhomeQoL.Modules.Misc;

namespace GodhomeQoL.Modules.QoL;
=======
using SafeGodseekerQoL.Modules.Misc;

namespace SafeGodseekerQoL.Modules.QoL;
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265

public sealed class UnlockRadiant : Module {
	public override bool DefaultEnabled => true;

	internal static void Unlock(Action orig, BossStatue statue, ref BossStatue.Completion completion) {
		if (completion.completedTier2) {
			orig.Invoke();
			return;
		}

		completion.completedTier2 = true;
		completion.seenTier3Unlock = true;
		BossChallengeUIEdit.SetStatueCompletion(statue, completion);

		orig.Invoke();

		completion.completedTier2 = false;

		LogDebug($"Unlocked Radiant for {statue.name}");
	}
}
