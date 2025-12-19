using Satchel;
using Satchel.Futils;
<<<<<<< HEAD

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class ForceGreyPrinceEnterType : Module
{
	[GlobalSetting]
	public static EnterType gpzEnterType = EnterType.Long;

    private static readonly SceneEdit handle = new(
        new("GG_Grey_Prince_Zote", "Grey Prince"),
        go =>
        {
            PlayMakerFSM fsm = go.LocateMyFSM("Control");
            fsm.AddCustomAction(
                "Enter 1",
                () => fsm.GetVariable<FsmBool>("Faced Zote").Value = gpzEnterType switch
                {
                    EnterType.Long => false,
                    EnterType.Short => true,
                    _ => false
                }
            );

            LogDebug("Grey Prince enter type modified");
        }
    );

    public override bool DefaultEnabled => true;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load() => handle.Enable();

    private protected override void Unload() => handle.Disable();

	public enum EnterType
	{
		Off,
=======
namespace SafeGodseekerQoL.Modules.BossChallenge;

public sealed class ForceGreyPrinceEnterType : Module {
	[GlobalSetting]
	[EnumOption]
	public static EnterType gpzEnterType = EnterType.Long;

	private static readonly SceneEdit handle = new(
		new("GG_Grey_Prince_Zote", "Grey Prince"),
		go => {
			PlayMakerFSM fsm = go.LocateMyFSM("Control");
			fsm.AddCustomAction(
				"Enter 1",
				() => fsm.GetVariable<FsmBool>("Faced Zote").Value = gpzEnterType switch {
					EnterType.Long => false,
					EnterType.Short => true,
					_ => throw new IndexOutOfRangeException()
				}
			);

			LogDebug("Grey Prince enter type modified");
		}
	);

	public override bool DefaultEnabled => true;

	public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

	private protected override void Load() =>
		handle.Enable();

	private protected override void Unload() =>
		handle.Disable();

	public enum EnterType {
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265
		Long,
		Short
	}
}
