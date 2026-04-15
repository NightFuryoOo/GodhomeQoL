//using GodhomeQoL.Modules.GodseekerMode;
using Satchel;
using Satchel.Futils;
using Osmi.FsmActions;
using GodhomeQoL.Modules.Tools;

namespace GodhomeQoL.Modules.QoL;

public sealed class FastDreamWarp : Module
{
	private sealed class DreamNailFsmSnapshot
	{
		public PlayMakerFSM? Fsm { get; init; }
		public Dictionary<string, FsmStateAction[]> OriginalStateActionsByName { get; } = new(StringComparer.Ordinal);
		public bool Patched { get; set; }
	}

	private static readonly GameObjectRef knightRef = new(GameObjectRef.DONT_DESTROY_ON_LOAD, "Knight");
	private static readonly Dictionary<int, DreamNailFsmSnapshot> fsmSnapshots = new();
	private static bool timeScaleOverrideInFlight;
	private static int timeScaleOverrideHandle;
	private static int timeScaleOverrideGeneration;

	public override bool DefaultEnabled => false;

	private protected override void Load()
	{
		timeScaleOverrideGeneration++;
		On.PlayMakerFSM.Start += ModifyDreamNailFSM;
	}

	private protected override void Unload()
	{
		On.PlayMakerFSM.Start -= ModifyDreamNailFSM;
		RestorePatchedFsms();
		CancelPendingTimeScaleNormalization();
	}

	private static bool ShouldActivate()
	{
		if (BossSceneController.IsBossScene)
		{
			return true;
		}

		GameManager? manager = GameManager.instance;
		if (manager != null && PlayerDataR.bossRushMode)
		{
			string scene = manager.sceneName ?? string.Empty;
			return scene is "Room_Colosseum_01" or "Room_Colosseum_Bronze" or "Room_Colosseum_Silver" or "Room_Colosseum_Gold";
		}

		return false;
	}

	private void ModifyDreamNailFSM(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
	{
		orig(self);

		if (self.FsmName == "Dream Nail" && knightRef.MatchGameObject(self.gameObject))
		{
			ApplyDreamNailPatch(self);
		}
	}

	private void ApplyDreamNailPatch(PlayMakerFSM fsm)
	{
		DreamNailFsmSnapshot snapshot = GetOrCreateSnapshot(fsm);
		if (snapshot.Patched)
		{
			return;
		}

		CaptureStateActions(snapshot, fsm, "Take Control");
		CaptureStateActions(snapshot, fsm, "Warp Charge Start");
		CaptureStateActions(snapshot, fsm, "Warp End");

		try
		{
			fsm.Intercept(new TransitionInterceptor()
			{
				fromState = "Take Control",
				eventName = FsmEvent.Finished.Name,
				toStateDefault = "Start",
				toStateCustom = "Can Warp?",
				shouldIntercept = () =>
				{
					HeroActions? actions = InputHandler.Instance?.inputActions;
					return Loaded
						&& ShouldActivate()
						&& actions != null
						&& actions.dreamNail.IsPressed
						&& actions.up.IsPressed;
				},
				onIntercept = (_, _) => BeginTimeScaleNormalization()
			});

			fsm.Intercept(new TransitionInterceptor()
			{
				fromState = "Warp Charge Start",
				eventName = FsmEvent.Finished.Name,
				toStateDefault = "Warp Charge",
				toStateCustom = "Can Warp?",
				shouldIntercept = () => Loaded && ShouldActivate(),
				onIntercept = (_, _) => BeginTimeScaleNormalization()
			});

			FsmState? warpEnd = fsm.Fsm.GetState("Warp End");
			if (warpEnd?.Actions != null && warpEnd.Actions.Length > 8 && warpEnd.Actions[8] != null)
			{
				warpEnd.Actions[8].Enabled = false;
			}

			fsm.AddAction("Warp End", new InvokePredicate(() => Loaded && ShouldActivate())
			{
				trueEvent = FsmEvent.Finished
			});

			snapshot.Patched = true;
			LogDebug("Dream Warp FSM modified");
		}
		catch (Exception ex)
		{
			LogError($"FastDreamWarp: failed to patch Dream Nail FSM - {ex.Message}");
			RestoreSnapshot(snapshot);
		}
	}

	private static DreamNailFsmSnapshot GetOrCreateSnapshot(PlayMakerFSM fsm)
	{
		CleanupSnapshotCache();

		int id = fsm.GetInstanceID();
		if (fsmSnapshots.TryGetValue(id, out DreamNailFsmSnapshot? snapshot))
		{
			return snapshot;
		}

		snapshot = new DreamNailFsmSnapshot
		{
			Fsm = fsm
		};
		fsmSnapshots[id] = snapshot;
		return snapshot;
	}

	private static void CaptureStateActions(DreamNailFsmSnapshot snapshot, PlayMakerFSM fsm, string stateName)
	{
		if (snapshot.OriginalStateActionsByName.ContainsKey(stateName))
		{
			return;
		}

		FsmState? state = fsm.Fsm.GetState(stateName);
		if (state == null)
		{
			return;
		}

		FsmStateAction[] actions = state.Actions ?? Array.Empty<FsmStateAction>();
		snapshot.OriginalStateActionsByName[stateName] = (FsmStateAction[])actions.Clone();
	}

	private static void RestorePatchedFsms()
	{
		foreach ((int id, DreamNailFsmSnapshot snapshot) in fsmSnapshots.ToArray())
		{
			RestoreSnapshot(snapshot);
			if (snapshot.Fsm == null)
			{
				fsmSnapshots.Remove(id);
			}
		}

		fsmSnapshots.Clear();
	}

	private static void RestoreSnapshot(DreamNailFsmSnapshot snapshot)
	{
		PlayMakerFSM? fsm = snapshot.Fsm;
		if (fsm == null)
		{
			return;
		}

		try
		{
			foreach ((string stateName, FsmStateAction[] originalActions) in snapshot.OriginalStateActionsByName)
			{
				FsmState? state = fsm.Fsm.GetState(stateName);
				if (state == null)
				{
					continue;
				}

				state.Actions = (FsmStateAction[])originalActions.Clone();
			}
		}
		catch
		{
			// ignore restore failures for already-destroyed FSMs
		}

		snapshot.Patched = false;
	}

	private static void CleanupSnapshotCache()
	{
		foreach ((int id, DreamNailFsmSnapshot snapshot) in fsmSnapshots.ToArray())
		{
			if (snapshot.Fsm == null)
			{
				fsmSnapshots.Remove(id);
			}
		}
	}

	private static void CancelPendingTimeScaleNormalization()
	{
		timeScaleOverrideGeneration++;
		if (timeScaleOverrideInFlight && timeScaleOverrideHandle != 0)
		{
			SpeedChanger.EndTimeScaleOverride(timeScaleOverrideHandle);
		}

		timeScaleOverrideHandle = 0;
		timeScaleOverrideInFlight = false;
	}

	private static void BeginTimeScaleNormalization()
	{
		if (timeScaleOverrideInFlight || Time.timeScale <= 1f)
		{
			return;
		}

		if (!SpeedChanger.TryBeginTimeScaleOverride(1f, out int handle))
		{
			return;
		}

		timeScaleOverrideInFlight = true;
		timeScaleOverrideHandle = handle;
		int generation = timeScaleOverrideGeneration;
		_ = GlobalCoroutineExecutor.Start(RestoreTimeScaleAfterWarp(generation, handle));
	}

	private static IEnumerator RestoreTimeScaleAfterWarp(int generation, int handle)
	{
		try
		{
			yield return null;
			yield return new UnityEngine.WaitUntil(() => GameManager.instance != null);

			float timeout = 5f;
			while (timeout > 0f)
			{
				GameManager? manager = GameManager.instance;
				if (manager == null || manager.IsInSceneTransition)
				{
					break;
				}

				timeout -= Time.unscaledDeltaTime;
				yield return null;
			}

			if (GameManager.instance != null && GameManager.instance.IsInSceneTransition)
			{
				yield return new UnityEngine.WaitWhile(() => GameManager.instance != null && GameManager.instance.IsInSceneTransition);
			}

			yield return new UnityEngine.WaitUntil(() => GameManager.instance != null && GameManager.instance.gameState == GameState.PLAYING);
		}
		finally
		{
			if (generation == timeScaleOverrideGeneration
				&& timeScaleOverrideInFlight
				&& timeScaleOverrideHandle == handle)
			{
				SpeedChanger.EndTimeScaleOverride(handle);
				timeScaleOverrideHandle = 0;
				timeScaleOverrideInFlight = false;
			}
		}
	}
}
