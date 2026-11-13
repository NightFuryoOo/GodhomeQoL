using GodhomeQoL.Modules.Tools;
using Osmi.FsmActions;
using Satchel;
using Satchel.Futils;
using WaitUntil = UnityEngine.WaitUntil;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class InfiniteRadianceClimbing : Module {
    private static readonly float heroX = 60.4987f;
    private static readonly float heroY = 34.6678f;
    private const float pitAscendTargetY = 30f;
    private const float pitAscendTolerance = 0.05f;
    private const string AnyRadianceAssembly = "AnyRadiance";
    private const string AnyRadianceTwoComponent = "Radiance";
    private static readonly Vector2 anyRadianceTwoStart = new(64.66f, 34.6f);
    private static readonly Vector2 anyRadianceTwoPlatform = new(64.66f, 32.5f);

    private sealed class TransitionSnapshot {
        public string StateName = string.Empty;
        public string EventName = string.Empty;
        public string? TargetState;
    }

    private sealed class FsmPatchSnapshot {
        public PlayMakerFSM? Fsm;
        public Dictionary<string, FsmStateAction[]> StateActionsByName = new(StringComparer.Ordinal);
        public List<TransitionSnapshot> TransitionSnapshots = [];
    }

    private static bool running = false;
    private static GameObject? bossCtrl;
    private static PlayMakerFSM? radCtrl;
    private static PlayMakerFSM? pitCtrl;
    private static Coroutine? rewindCoro;
    private static int invincibilityClaimHandle;
    private static FsmPatchSnapshot? bossControlSnapshot;
    private static FsmPatchSnapshot? radianceControlSnapshot;
    private static bool anyRadianceTwo;
    private static GameObject? startPlatform;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load() =>
        OsmiHooks.SceneChangeHook += SetupScene;

    private protected override void Unload() {
        OsmiHooks.SceneChangeHook -= SetupScene;

        if (running) {
            Quit(true);
        } else {
            RestorePatchedFsms();
        }

        ReleaseInvincibilityClaim();
    }

    private static void SetupScene(Scene prev, Scene next) {
        if (next.name != "GG_Radiance") {
            if (running) {
                Quit();
            }

            return;
        }

        if (BossSequenceController.IsInSequence) {
            if (running) {
                Quit();
            }

            return;
        }

        if (running) {
            Quit();
        }

        running = true;
        bossCtrl = next.GetGameObjectByName("Boss Control");
        radCtrl = bossCtrl.Child("Absolute Radiance")!.LocateMyFSM("Control");
        pitCtrl = bossCtrl.Child("Abyss Pit")!.LocateMyFSM("Ascend");

        bossCtrl!
            .Child("Ascend Respawns", "Hazard Respawn Trigger v2 (15)")!
            .SetActive(false);

        PlayMakerFSM bossControlFsm = bossCtrl!.LocateMyFSM("Control");
        bossControlSnapshot = CaptureSnapshot(bossControlFsm, "Battle Start");
        bossControlFsm.RemoveAction("Battle Start", 3);

        ModifyAbsRadFSM(radCtrl);

        radCtrl!.gameObject.LocateMyFSM("Phase Control")
            .Fsm.SetState("Set Ascend");

        LogDebug("Scene setup finished");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ModifyAbsRadFSM(PlayMakerFSM fsm) {
        radianceControlSnapshot = CaptureSnapshot(fsm, "Set Arena 1", "Climb Plats1", "Scream");
        CaptureTransition(radianceControlSnapshot, fsm, "Set Arena 1", FsmEvent.Finished.Name);

        fsm.RemoveAction("Set Arena 1", 3);

        fsm.ChangeTransition("Set Arena 1", FsmEvent.Finished.Name, "Climb Plats1");

        fsm.InsertAction("Set Arena 1", new InvokeCoroutine(TeleportSetup), 0);

        FsmState spawnPlatsState = fsm.GetValidState("Climb Plats1");
        FsmStateAction[] spawnActions = spawnPlatsState.Actions ?? Array.Empty<FsmStateAction>();
        if (spawnActions.Length > 2) {
            spawnPlatsState.Actions = [
                spawnActions[2],
                new InvokeMethod(() => fsm.gameObject.manageHealth(ClimbHealth(fsm.gameObject)))
            ];

            if (spawnPlatsState.Actions[0] is SendEventByName sendEvent) {
                sendEvent.delay = 0;
            } else {
                LogWarn("InfiniteRadianceClimbing: expected SendEventByName at Climb Plats1[0], delay not adjusted.");
            }
        } else {
            LogWarn($"InfiniteRadianceClimbing: unexpected action layout in Climb Plats1 (count={spawnActions.Length}).");
        }

        FsmState screamState = fsm.GetValidState("Scream");
        FsmStateAction[] screamActions = screamState.Actions ?? Array.Empty<FsmStateAction>();
        if (screamActions.Length > 7) {
            FsmStateAction[] patched = new FsmStateAction[screamActions.Length];
            patched[0] = screamActions[0];
            patched[1] = new InvokeMethod(() => {
                if (radCtrl != null) {
                    rewindCoro ??= radCtrl.StartCoroutine(Rewind());
                }
            });
            patched[2] = new Wait() { time = 60f };
            for (int i = 3; i < patched.Length; i++) {
                patched[i] = new InvokeMethod(() => { });
            }

            patched[7] = screamActions[7];
            screamState.Actions = patched;
        } else {
            LogWarn($"InfiniteRadianceClimbing: unexpected action layout in Scream (count={screamActions.Length}).");
        }
    }

    private static IEnumerator TeleportSetup() {
        SpriteFlash flasher = Ref.HC.GetComponent<SpriteFlash>();

        Ref.HC.RelinquishControl();
        flasher.FlashingSuperDash();

        anyRadianceTwo = radCtrl != null && IsAnyRadianceTwo(radCtrl.gameObject);
        if (anyRadianceTwo) {
            yield return ShowStartPlatform();
        }

        yield return new WaitForSeconds(0.5f);

        Vector2 start = StartPosition();
        Ref.HC.transform.SetPosition2D(start.x, start.y);
        Ref.HC.FaceRight();
        Ref.HC.SetHazardRespawn(Ref.HC.transform.position, true);

        if (anyRadianceTwo) {
            SetArenaTwo();
        }

        bossCtrl!.Child("Intro Wall")!.SetActive(false);

        yield return new WaitForSeconds(3.75f);

        flasher.CancelFlash();
        Ref.HC.RegainControl();

        LogDebug("Hero teleported");
        if (radCtrl != null) {
            LogAnyRadianceState(radCtrl.gameObject);
        }
    }

    private static void LogAnyRadianceState(GameObject radiance) {
        try {
            string component = AnyRadianceComponentName(radiance) ?? "none";
            FsmStateAction[] aim = radiance.LocateMyFSM("Attack Commands")?.Fsm.GetState("Aim")?.Actions ?? Array.Empty<FsmStateAction>();
            string endDelay = aim.Length > 10 && aim[10] is SendEventByName end ? end.delay.Value.ToString("0.##") : "?";
            string repeat = aim.Length > 11 && aim[11] is Wait wait ? wait.time.Value.ToString("0.##") : "?";
            int? hp = radiance.GetComponent<HealthManager>()?.hp;
            int? arena = radiance.LocateMyFSM("Attack Choices")?.FsmVariables.FindFsmInt("Arena")?.Value;
            LogDebug($"[InfiniteRadianceClimbing] AnyRadiance component: {component}; ascend beam end delay {endDelay}s, repeat {repeat}s; hp {hp}, arena {arena}");
        } catch (Exception ex) {
            LogSuppressed(ex, "InfiniteRadianceClimbing.cs/AnyRadiance");
        }
    }

    private static string? AnyRadianceComponentName(GameObject radiance) =>
        radiance.GetComponents<MonoBehaviour>()
            .Where(c => c != null && c.GetType().Assembly.GetName().Name == AnyRadianceAssembly)
            .Select(c => c.GetType().Name)
            .FirstOrDefault();

    private static bool IsAnyRadianceTwo(GameObject radiance) =>
        AnyRadianceComponentName(radiance) == AnyRadianceTwoComponent;

    private static Vector2 StartPosition() =>
        anyRadianceTwo ? anyRadianceTwoStart : new Vector2(heroX, heroY);

    private static int ClimbHealth(GameObject radiance) {
        if (!IsAnyRadianceTwo(radiance)) {
            return int.MaxValue;
        }

        FsmInt? ascend = radiance.LocateMyFSM("Phase Control")?.FsmVariables.FindFsmInt("P5 Acend");
        return ascend != null ? Math.Max(1, ascend.Value - 1) : int.MaxValue;
    }

    private static void SetArenaTwo() {
        FsmInt? arena = radCtrl?.gameObject.LocateMyFSM("Attack Choices")?.FsmVariables.FindFsmInt("Arena");
        if (arena != null) {
            arena.Value = 2;
        }
    }

    private static IEnumerator ShowStartPlatform() {
        GameObject? source = bossCtrl?.Child("Plat Sets", "Ascend Set", "Radiant Plat Small (4)");
        if (source == null) {
            LogWarn("InfiniteRadianceClimbing: start platform source not found.");
            yield break;
        }

        if (startPlatform == null) {
            startPlatform = UObject.Instantiate(source, new Vector3(anyRadianceTwoPlatform.x, anyRadianceTwoPlatform.y, source.transform.position.z), Quaternion.identity);
            startPlatform.name = "GodhomeQoL Climb Start Plat";
            startPlatform.SetActive(true);
        }

        PlayMakerFSM? plat = startPlatform.LocateMyFSM("radiant_plat");
        float deadline = Time.time + 1f;
        while (plat != null && plat.ActiveStateName != "Out" && Time.time < deadline) {
            yield return null;
        }

        plat?.SendEvent("APPEAR");
        GameObject? colliders = startPlatform.Child("Colliders");
        deadline = Time.time + 1.5f;
        while (colliders != null && !colliders.activeSelf && Time.time < deadline) {
            yield return null;
        }

        LogDebug($"[InfiniteRadianceClimbing] AnyRadiance 2.0 start platform: state {plat?.ActiveStateName}, colliders {colliders?.activeSelf}");
    }

    private static IEnumerator Rewind() {
        LogDebug("AbsRad final phase started, rewinding...");

        if (Ref.HC == null || radCtrl == null || pitCtrl == null || bossCtrl == null) {
            yield break;
        }

        PlayMakerFSM localRadCtrl = radCtrl;
        PlayMakerFSM localPitCtrl = pitCtrl;
        GameObject localBossCtrl = bossCtrl;
        SpriteFlash flasher = Ref.HC.GetComponent<SpriteFlash>();
        GameObject? beam = localRadCtrl.gameObject.Child("Eye Beam Glow", "Ascend Beam");
        if (beam == null) {
            yield break;
        }

        AcquireInvincibilityClaim();

        try {
            localRadCtrl.gameObject.LocateMyFSM("Attack Commands").Fsm.SetState("Idle");
            beam.SetActive(false);

            localPitCtrl.GetVariable<FsmFloat>("Hero Y").Value = 33f;
            localPitCtrl.SendEvent("ASCEND");

            Ref.HC.RelinquishControl();
            Vector2 start = StartPosition();
            Ref.HC.transform.SetPosition2D(start.x, start.y);
            Ref.HC.FaceRight();
            Ref.HC.SetHazardRespawn(Ref.HC.transform.position, true);
            flasher.FlashingSuperDash();

            yield return new WaitUntil(() =>
                pitCtrl == null
                || Mathf.Abs(pitCtrl.transform.position.y - pitAscendTargetY) <= pitAscendTolerance);

            if (pitCtrl == null || bossCtrl == null || radCtrl == null) {
                yield break;
            }

            if (anyRadianceTwo) {
                localRadCtrl.gameObject.manageHealth(ClimbHealth(localRadCtrl.gameObject));
                LogAnyRadianceState(localRadCtrl.gameObject);
            }

            localBossCtrl
                .Child("Ascend Respawns")!
                .GetChildren()
                .Filter(go => go.name.StartsWith("Hazard Respawn Trigger v2"))
                .ForEach(go => {
                    go.GetComponent<HazardRespawnTrigger>().Reflect().inactive = false;
                    go.LocateMyFSM("raise_abyss_pit").Fsm.SetState("Idle");
                });

            flasher.CancelFlash();
            Ref.HC.RegainControl();

            yield return new WaitForSeconds(1.5f);

            localRadCtrl.Fsm.SetState("Ascend Cast");
            localRadCtrl.transform.SetPositionX(62.94f);
            beam.transform.parent.gameObject.SetActive(true);
            beam.SetActive(true);
        } finally {
            ReleaseInvincibilityClaim();
            rewindCoro = null;
        }
    }

    private static void Quit(bool killPlayer = false) {
        if (rewindCoro != null && radCtrl != null) {
            radCtrl.StopCoroutine(rewindCoro);
            rewindCoro = null;
        }

        RestorePatchedFsms();
        ReleaseInvincibilityClaim();
        running = false;
        bossCtrl = null;
        radCtrl = null;
        pitCtrl = null;
        anyRadianceTwo = false;
        if (startPlatform != null) {
            UObject.Destroy(startPlatform);
        }

        startPlatform = null;

        if (killPlayer) {
            _ = Ref.HC.StartCoroutine(DelayedKill());
        }
    }

    private static IEnumerator DelayedKill() {
        yield return new WaitUntil(() => Ref.GM.gameState == GameState.PLAYING);
        _ = Ref.HC.StartCoroutine(HeroControllerR.Die());
    }

    private static void AcquireInvincibilityClaim() {
        if (invincibilityClaimHandle != 0) {
            return;
        }

        invincibilityClaimHandle = InvincibilityClaims.Acquire(nameof(InfiniteRadianceClimbing));
    }

    private static void ReleaseInvincibilityClaim() {
        if (invincibilityClaimHandle == 0) {
            return;
        }

        InvincibilityClaims.Release(invincibilityClaimHandle);
        invincibilityClaimHandle = 0;
    }

    private static FsmPatchSnapshot CaptureSnapshot(PlayMakerFSM fsm, params string[] stateNames) {
        FsmPatchSnapshot snapshot = new() {
            Fsm = fsm
        };

        foreach (string stateName in stateNames) {
            FsmState? state = fsm.Fsm.GetState(stateName);
            if (state == null) {
                continue;
            }

            snapshot.StateActionsByName[stateName] = state.Actions?.ToArray() ?? Array.Empty<FsmStateAction>();
        }

        return snapshot;
    }

    private static void CaptureTransition(FsmPatchSnapshot snapshot, PlayMakerFSM fsm, string stateName, string eventName) {
        snapshot.TransitionSnapshots.Add(new TransitionSnapshot {
            StateName = stateName,
            EventName = eventName,
            TargetState = GetTransitionTarget(fsm, stateName, eventName)
        });
    }

    private static string? GetTransitionTarget(PlayMakerFSM fsm, string stateName, string eventName) {
        FsmState? state = fsm.Fsm.GetState(stateName);
        if (state == null) {
            return null;
        }

        foreach (FsmTransition transition in state.Transitions) {
            if (transition.EventName == eventName) {
                return transition.ToState;
            }
        }

        return null;
    }

    private static void RestorePatchedFsms() {
        RestoreSnapshot(radianceControlSnapshot);
        RestoreSnapshot(bossControlSnapshot);
        radianceControlSnapshot = null;
        bossControlSnapshot = null;
    }

    private static void RestoreSnapshot(FsmPatchSnapshot? snapshot) {
        if (snapshot == null || snapshot.Fsm == null) {
            return;
        }

        PlayMakerFSM fsm = snapshot.Fsm;

        foreach ((string stateName, FsmStateAction[] actions) in snapshot.StateActionsByName) {
            FsmState? state = fsm.Fsm.GetState(stateName);
            if (state == null) {
                continue;
            }

            state.Actions = actions.ToArray();
        }

        foreach (TransitionSnapshot transition in snapshot.TransitionSnapshots) {
            string? targetState = transition.TargetState;
            if (string.IsNullOrEmpty(targetState)) {
                continue;
            }

            TrySetTransition(fsm, transition.StateName, transition.EventName, targetState!);
        }
    }

    private static void TrySetTransition(PlayMakerFSM fsm, string stateName, string eventName, string targetState) {
        try {
            fsm.ChangeTransition(stateName, eventName, targetState);
        } catch (Exception swallowed) {
            LogSuppressed(swallowed, "InfiniteRadianceClimbing.cs");
        }
    }
}
