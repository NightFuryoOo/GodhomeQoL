using MonoMod.RuntimeDetour;
using Satchel;
<<<<<<< HEAD

namespace GodhomeQoL.Modules.BossChallenge;
=======
namespace SafeGodseekerQoL.Modules.BossChallenge;
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265

public sealed class P5Health : Module {
	private static readonly Detour levelGetterDetour = new(
		Info.OfPropertyGet<BossSceneController>(nameof(BossSceneController.BossLevel)),
		Info.OfMethod<P5Health>(nameof(OverrideLevel)),
		new() { ManualApply = true }
	);

	private static readonly SceneEdit ascendedMarkothHandle = new(
		new("GG_Markoth_V", "Warrior", "Ghost Warrior Markoth"),
		go => go.manageHealth(650)
	);

	public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

	private protected override void Load() {
		levelGetterDetour.Apply();
		ascendedMarkothHandle.Enable();

		ModHooks.TakeDamageHook += FixDamage;
	}

	private protected override void Unload() {
		levelGetterDetour.Undo();
		ascendedMarkothHandle.Disable();

		ModHooks.TakeDamageHook -= FixDamage;
	}

	private static int OverrideLevel(BossSceneController _1, int _2) => 0;

	private static int FixDamage(ref int hazardType, int damage) => damage switch {
		int i when i <= 0 => i,
		int i when BossSceneController.IsBossScene is false => i,
<<<<<<< HEAD
		int i => BossSceneController.Instance.Reflect().bossLevel switch {
=======
		int i => BossSceneController.Instance?.Reflect().bossLevel switch {
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265
			1 => i * 2,
			2 => 9999,
			_ => i,
		}
	};
}
