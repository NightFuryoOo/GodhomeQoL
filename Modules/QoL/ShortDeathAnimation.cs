using Osmi.FsmActions;
using GodhomeQoL.Modules.BossChallenge;
using GodhomeQoL.Modules.Tools;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.QoL;

public sealed class ShortDeathAnimation : Module {
	private sealed class DeathFsmSnapshot {
		public PlayMakerFSM? Fsm { get; init; }
		public Dictionary<string, FsmStateAction[]> OriginalStateActionsByName { get; } = new(StringComparer.Ordinal);
		public bool Patched { get; set; }
	}

	public override bool DefaultEnabled => true;

	private static readonly Dictionary<int, DeathFsmSnapshot> deathSnapshots = new();
	private static bool timeScaleOverrideInFlight;
	private static int timeScaleOverrideHandle;
	private static int timeScaleOverrideGeneration;
	private static readonly HashSet<string> PantheonLikeScenes = new(StringComparer.OrdinalIgnoreCase) {
		"gg dryya",
		"gg hegemol",
		"gg zemer",
		"gg isma"
	};

	private protected override void Load() {
		timeScaleOverrideGeneration++;
		On.HeroController.Start += ModifyHeroDeathFSM;
		On.HeroController.Die += OnHeroDie;
	}

	private protected override void Unload() {
		On.HeroController.Start -= ModifyHeroDeathFSM;
		On.HeroController.Die -= OnHeroDie;
		RestoreModifiedFsms();
		CancelPendingTimeScaleNormalization();
	}

	private void ModifyHeroDeathFSM(On.HeroController.orig_Start orig, HeroController self) {
		orig(self);

		PlayMakerFSM? fsm = self.heroDeathPrefab?.LocateMyFSM("Hero Death Anim");
		if (fsm == null) {
			return;
		}

		ModifyHeroDeathFSM(fsm);

		LogDebug("Hero Death FSM modified");
	}

	private static IEnumerator OnHeroDie(On.HeroController.orig_Die orig, HeroController self) {
		BeginTimeScaleNormalization();
		ClearBossReturnFlags();

		IEnumerator origEnum = orig(self);
		while (origEnum.MoveNext()) {
			yield return origEnum.Current;
		}
	}

	private static void ClearBossReturnFlags() {
		if (!IsPantheonLikeScene()) {
			return;
		}

		try {
			if (!InfiniteChallenge.OwnsFinishedBossReturningFlag()) {
				StaticVariableList.SetValue("finishedBossReturning", false);
			}
		}
		catch {
			// ignore if variable missing
		}

		try {
			PlayMakerFSM? fsm = HeroController.instance?.gameObject?.LocateMyFSM("Dream Return");
			if (fsm == null) {
				return;
			}

			FsmBool? dreamReturning = fsm.GetVariable<FsmBool>("Dream Returning");
			if (dreamReturning != null) {
				dreamReturning.Value = false;
			}
		}
		catch {
			// ignore if FSM or variable missing
		}
	}

	private void ModifyHeroDeathFSM(PlayMakerFSM fsm) {
		DeathFsmSnapshot snapshot = GetOrCreateSnapshot(fsm);
		if (snapshot.Patched) {
			return;
		}

		CaptureStateActions(snapshot, fsm, "Init");
		CaptureStateActions(snapshot, fsm, "Start");
		CaptureStateActions(snapshot, fsm, "Bursting");

		TryInsertTimeNormalization(fsm);

		fsm.AddAction("Bursting",
			new InvokePredicate(() => Loaded && BossSceneController.IsBossScene && !IsPantheonLikeScene())
			{
				trueEvent = FsmEvent.Finished
			}
		);

		snapshot.Patched = true;
	}

	private static bool IsPantheonLikeScene() {
		if (BossSequenceController.IsInSequence) {
			return true;
		}

		GameManager? manager = GameManager.instance;
		string sceneName = manager?.sceneName ?? manager?.GetSceneNameString() ?? string.Empty;
		if (sceneName.Length == 0) {
			return false;
		}

		return PantheonLikeScenes.Contains(sceneName);
	}

	private static void TryInsertTimeNormalization(PlayMakerFSM fsm) {
		if (TryInsertAction(fsm, "Init")) {
			return;
		}

		if (TryInsertAction(fsm, "Start")) {
			return;
		}

		_ = TryInsertAction(fsm, "Bursting");
	}

	private static bool TryInsertAction(PlayMakerFSM fsm, string stateName) {
		try {
			fsm.InsertAction(stateName, new InvokeMethod(BeginTimeScaleNormalization), 0);
			return true;
		}
		catch {
			return false;
		}
	}

	private static void BeginTimeScaleNormalization() {
		if (FreezeHitboxes.ShouldBlockDeathTimeScaleNormalization()) {
			return;
		}

		if (timeScaleOverrideInFlight) {
			return;
		}

		if (Math.Abs(Time.timeScale - 1f) < 0.001f) {
			return;
		}

		if (!SpeedChanger.TryBeginTimeScaleOverride(1f, out int handle)) {
			return;
		}

		timeScaleOverrideInFlight = true;
		timeScaleOverrideHandle = handle;
		int generation = timeScaleOverrideGeneration;
		_ = GlobalCoroutineExecutor.Start(RestoreTimeScaleAfterDeath(generation, handle));
	}

	private static IEnumerator RestoreTimeScaleAfterDeath(int generation, int handle) {
		try {
			yield return null;
			yield return new UnityEngine.WaitUntil(() => GameManager.instance != null);

			float elapsed = 0f;
			const float timeout = 20f;

			while (elapsed < timeout) {
				GameManager? manager = GameManager.instance;
				if (manager == null) {
					break;
				}

				if (manager.IsInSceneTransition) {
					elapsed += Time.unscaledDeltaTime;
					yield return null;
					continue;
				}

				if (manager.gameState == GameState.PLAYING && !IsDeathAnimationActive()) {
					break;
				}

				elapsed += Time.unscaledDeltaTime;
				yield return null;
			}

			if (generation == timeScaleOverrideGeneration
				&& timeScaleOverrideInFlight
				&& timeScaleOverrideHandle == handle) {
				SpeedChanger.EndTimeScaleOverride(handle);
				timeScaleOverrideHandle = 0;
			}
		}
		finally {
			if (generation == timeScaleOverrideGeneration && timeScaleOverrideHandle == handle) {
				timeScaleOverrideInFlight = false;
			}
		}
	}

	private static void CancelPendingTimeScaleNormalization() {
		timeScaleOverrideGeneration++;
		if (timeScaleOverrideInFlight && timeScaleOverrideHandle != 0) {
			SpeedChanger.EndTimeScaleOverride(timeScaleOverrideHandle);
		}

		timeScaleOverrideHandle = 0;
		timeScaleOverrideInFlight = false;
	}

	private static DeathFsmSnapshot GetOrCreateSnapshot(PlayMakerFSM fsm) {
		CleanupSnapshotCache();

		int id = fsm.GetInstanceID();
		if (deathSnapshots.TryGetValue(id, out DeathFsmSnapshot? snapshot)) {
			return snapshot;
		}

		snapshot = new DeathFsmSnapshot {
			Fsm = fsm
		};
		deathSnapshots[id] = snapshot;
		return snapshot;
	}

	private static void CaptureStateActions(DeathFsmSnapshot snapshot, PlayMakerFSM fsm, string stateName) {
		if (snapshot.OriginalStateActionsByName.ContainsKey(stateName)) {
			return;
		}

		FsmState? state = fsm.Fsm.GetState(stateName);
		if (state == null) {
			return;
		}

		FsmStateAction[] actions = state.Actions ?? Array.Empty<FsmStateAction>();
		snapshot.OriginalStateActionsByName[stateName] = (FsmStateAction[])actions.Clone();
	}

	private static void RestoreModifiedFsms() {
		foreach ((int id, DeathFsmSnapshot snapshot) in deathSnapshots.ToArray()) {
			RestoreSnapshot(snapshot);
			if (snapshot.Fsm == null) {
				deathSnapshots.Remove(id);
			}
		}

		deathSnapshots.Clear();
	}

	private static void RestoreSnapshot(DeathFsmSnapshot snapshot) {
		PlayMakerFSM? fsm = snapshot.Fsm;
		if (fsm == null) {
			return;
		}

		try {
			foreach ((string stateName, FsmStateAction[] originalActions) in snapshot.OriginalStateActionsByName) {
				FsmState? state = fsm.Fsm.GetState(stateName);
				if (state == null) {
					continue;
				}

				state.Actions = (FsmStateAction[])originalActions.Clone();
			}
		}
		catch {
			// ignore restore failures for already-destroyed FSMs
		}

		snapshot.Patched = false;
	}

	private static void CleanupSnapshotCache() {
		foreach ((int id, DeathFsmSnapshot snapshot) in deathSnapshots.ToArray()) {
			if (snapshot.Fsm == null) {
				deathSnapshots.Remove(id);
			}
		}
	}

	private static bool IsDeathAnimationActive() {
		HeroController? hero = HeroController.instance;
		if (hero == null) {
			return false;
		}

		GameObject deathPrefab = hero.heroDeathPrefab;
		return deathPrefab != null && deathPrefab.activeSelf;
	}
}
