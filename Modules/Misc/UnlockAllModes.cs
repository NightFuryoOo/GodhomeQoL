<<<<<<< HEAD
namespace GodhomeQoL.Modules.QoL;
=======
namespace SafeGodseekerQoL.Modules.Misc;
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265

public sealed class UnlockAllModes : Module {
	public override bool DefaultEnabled => true;

	private protected override void Load() {
		Platform.ISharedData data = Platform.Current.EncryptedSharedData;
		data.SetInt("RecPermadeathMode", 1);
		data.SetInt("RecBossRushMode", 1);
	}
}
