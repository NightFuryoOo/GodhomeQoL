using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class FlukemarmHelper : Module
{
    internal enum FlukeSpawnMode
    {
        Disabled,
        Enabled,
        Forced
    }

    internal const int SpawnPointCount = 4;
    internal const int VanillaFlukesPerWave = 2;
    internal const int MaxFlukesPerWave = 4;

    private const string FlukeMotherFsmName = "Fluke Mother";

    [LocalSetting]
    internal static FlukeSpawnMode flukemarmSpawnTopLeft = FlukeSpawnMode.Enabled;

    [LocalSetting]
    internal static FlukeSpawnMode flukemarmSpawnTopRight = FlukeSpawnMode.Enabled;

    [LocalSetting]
    internal static FlukeSpawnMode flukemarmSpawnBottomLeft = FlukeSpawnMode.Enabled;

    [LocalSetting]
    internal static FlukeSpawnMode flukemarmSpawnBottomRight = FlukeSpawnMode.Enabled;

    [LocalSetting]
    internal static int flukemarmFlukesPerWave = VanillaFlukesPerWave;

    private static readonly Dictionary<int, FlukeSpawnPatch> spawnPatches = new();

    internal static FlukeSpawnMode GetSpawnMode(int point) => point switch
    {
        0 => flukemarmSpawnTopLeft,
        1 => flukemarmSpawnTopRight,
        2 => flukemarmSpawnBottomLeft,
        _ => flukemarmSpawnBottomRight
    };

    private static void SetSpawnModeRaw(int point, FlukeSpawnMode mode)
    {
        switch (point)
        {
            case 0:
                flukemarmSpawnTopLeft = mode;
                break;
            case 1:
                flukemarmSpawnTopRight = mode;
                break;
            case 2:
                flukemarmSpawnBottomLeft = mode;
                break;
            default:
                flukemarmSpawnBottomRight = mode;
                break;
        }
    }

    internal static int ForcedSpawnCount()
    {
        int count = 0;
        for (int point = 0; point < SpawnPointCount; point++)
        {
            if (GetSpawnMode(point) == FlukeSpawnMode.Forced)
            {
                count++;
            }
        }

        return count;
    }

    internal static int ClampFlukesPerWave(int value) => Mathf.Clamp(value, 0, MaxFlukesPerWave);

    internal static void CycleSpawnMode(int point)
    {
        if (point < 0 || point >= SpawnPointCount)
        {
            return;
        }

        FlukeSpawnMode current = GetSpawnMode(point);
        FlukeSpawnMode next = current switch
        {
            FlukeSpawnMode.Disabled => FlukeSpawnMode.Enabled,
            FlukeSpawnMode.Enabled => FlukeSpawnMode.Forced,
            _ => FlukeSpawnMode.Disabled
        };

        if (next == FlukeSpawnMode.Forced && ForcedSpawnCount() + 1 > ClampFlukesPerWave(flukemarmFlukesPerWave))
        {
            next = FlukeSpawnMode.Disabled;
        }

        SetSpawnModeRaw(point, next);
        ReapplyLiveSettings();
    }

    internal static void SetFlukesPerWave(int value)
    {
        flukemarmFlukesPerWave = Mathf.Clamp(value, ForcedSpawnCount(), MaxFlukesPerWave);
        ReapplyLiveSettings();
    }

    internal static void ResetSpawnSettings()
    {
        for (int point = 0; point < SpawnPointCount; point++)
        {
            SetSpawnModeRaw(point, FlukeSpawnMode.Enabled);
        }

        flukemarmFlukesPerWave = VanillaFlukesPerWave;
    }

    private static bool IsVanillaSpawnPattern()
    {
        for (int point = 0; point < SpawnPointCount; point++)
        {
            if (GetSpawnMode(point) != FlukeSpawnMode.Enabled)
            {
                return false;
            }
        }

        return ClampFlukesPerWave(flukemarmFlukesPerWave) == VanillaFlukesPerWave;
    }

    private static bool NeedsSpawnPatch() => !IsVanillaSpawnPattern() || ShouldUseCustomSummonLimit();

    private static void TryApplySpawnPatch(PlayMakerFSM fsm)
    {
        if (fsm == null
            || !string.Equals(fsm.FsmName, FlukeMotherFsmName, StringComparison.Ordinal)
            || fsm.Fsm == null
            || !fsm.Fsm.Started
            || !NeedsSpawnPatch())
        {
            return;
        }

        int id = fsm.GetInstanceID();
        if (spawnPatches.ContainsKey(id))
        {
            return;
        }

        try
        {
            FlukeSpawnPatch? patch = FlukeSpawnPatch.TryCreate(fsm);
            if (patch != null)
            {
                spawnPatches[id] = patch;
                LogDebug($"[FlukemarmHelper] Fluke spawns patched in '{fsm.gameObject.scene.name}'");
            }
        }
        catch (Exception ex)
        {
            LogError($"[FlukemarmHelper] Patching fluke spawns failed: {ex}");
        }
    }

    private static void ClearSpawnPatches() => spawnPatches.Clear();

    private sealed class FlukeSpawnPatch
    {
        private const string SpawnPointFsmName = "flukemother_spawn_point";
        private const string ChooseState = "GodhomeQoL Fluke Choose";
        private const string AnticState = "GodhomeQoL Fluke Antic";
        private const string NextState = "GodhomeQoL Fluke Next";
        private const string EventAntic = "GODHOMEQOL FLUKE ANTIC";
        private const string EventNone = "GODHOMEQOL FLUKE NONE";
        private const string EventSpawnFirst = "GODHOMEQOL FLUKE SPAWN FIRST";
        private const string EventSpawnMore = "GODHOMEQOL FLUKE SPAWN MORE";
        private const string EventDone = "GODHOMEQOL FLUKE DONE";
        private const float VanillaAnticSeconds = 0.75f;

        private static readonly string[] RequiredStates = { "Rage", "Select Point 2", "Spawn 2", "Spawn 2 Cancel", "Spawn", "Spawn 1 Cancel" };

        private readonly PlayMakerFSM fsm;
        private readonly GameObject[] points;
        private readonly PlayMakerFSM?[] pointFsms;
        private readonly FsmGameObject spawnPoint;
        private readonly FsmGameObject spawnPoint2;
        private readonly Transform cage;
        private readonly HashSet<GameObject> knownFlies = new();
        private readonly List<int> candidates = new(SpawnPointCount);
        private readonly Queue<int> waveQueue = new(SpawnPointCount);
        private int waveBudget;
        private bool firstSpawnDone;

        private FlukeSpawnPatch(PlayMakerFSM fsm, GameObject[] points, Transform cage, FsmGameObject spawnPoint, FsmGameObject spawnPoint2)
        {
            this.fsm = fsm;
            this.points = points;
            this.cage = cage;
            this.spawnPoint = spawnPoint;
            this.spawnPoint2 = spawnPoint2;
            pointFsms = points.Select(point => point.LocateMyFSM(SpawnPointFsmName)).ToArray();
            RememberCageFlies();
        }

        internal static FlukeSpawnPatch? TryCreate(PlayMakerFSM fsm)
        {
            if (Satchel.FsmUtil.TryGetState(fsm, ChooseState, out FsmState _))
            {
                return null;
            }

            string? missing = RequiredStates.FirstOrDefault(state => !Satchel.FsmUtil.TryGetState(fsm, state, out FsmState _));
            if (missing != null)
            {
                LogWarn($"[FlukemarmHelper] Fluke Mother has no state '{missing}', spawns stay vanilla");
                return null;
            }

            GameObject[]? points = ClassifySpawnPoints(fsm.transform.Find("Spawn Points"));
            if (points == null)
            {
                LogWarn("[FlukemarmHelper] Could not identify the 4 fluke spawn points, spawns stay vanilla");
                return null;
            }

            GameObject? cage = ResolveCageObject(fsm);
            FsmGameObject? spawnPoint = fsm.FsmVariables.FindFsmGameObject("Spawn Point");
            FsmGameObject? spawnPoint2 = fsm.FsmVariables.FindFsmGameObject("Spawn Point 2");
            if (cage == null || cage.transform.childCount == 0 || spawnPoint == null || spawnPoint2 == null)
            {
                LogWarn("[FlukemarmHelper] Fluke Mother has no fluke cage or spawn point variables, spawns stay vanilla");
                return null;
            }

            FlukeSpawnPatch patch = new(fsm, points, cage.transform, spawnPoint, spawnPoint2);
            patch.Apply();
            return patch;
        }

        private static GameObject[]? ClassifySpawnPoints(Transform? root)
        {
            if (root == null || root.childCount != SpawnPointCount)
            {
                return null;
            }

            GameObject[] quadrants = new GameObject[SpawnPointCount];
            Vector3 center = root.position;
            foreach (Transform child in root)
            {
                if (child.gameObject.LocateMyFSM(SpawnPointFsmName) == null)
                {
                    return null;
                }

                Vector3 offset = child.position - center;
                int index = offset.y > 0f
                    ? (offset.x < 0f ? 0 : 1)
                    : (offset.x < 0f ? 2 : 3);
                if (quadrants[index] != null)
                {
                    return null;
                }

                quadrants[index] = child.gameObject;
            }

            return quadrants;
        }

        private void Apply()
        {
            FsmState rage = Satchel.FsmUtil.GetValidState(fsm, "Rage");
            FsmState selectPoint2 = Satchel.FsmUtil.GetValidState(fsm, "Select Point 2");
            FsmState spawn2 = Satchel.FsmUtil.GetValidState(fsm, "Spawn 2");
            FsmState spawn2Cancel = Satchel.FsmUtil.GetValidState(fsm, "Spawn 2 Cancel");
            FsmState spawn1 = Satchel.FsmUtil.GetValidState(fsm, "Spawn");
            FsmState spawn1Cancel = Satchel.FsmUtil.GetValidState(fsm, "Spawn 1 Cancel");

            FsmState choose = Satchel.FsmUtil.AddState(fsm, ChooseState);
            Satchel.FsmUtil.AddCustomAction(choose, Choose);

            FsmState antic = Satchel.FsmUtil.AddState(fsm, AnticState);
            Wait? vanillaWait = Satchel.FsmUtil.GetActions<Wait>(selectPoint2).FirstOrDefault();
            Satchel.FsmUtil.AddAction(antic, new Wait
            {
                time = vanillaWait != null ? vanillaWait.time : new FsmFloat(VanillaAnticSeconds),
                finishEvent = FsmEvent.Finished,
                realTime = false
            });

            FsmState next = Satchel.FsmUtil.AddState(fsm, NextState);
            Satchel.FsmUtil.AddCustomAction(next, Next);

            Satchel.FsmUtil.ChangeTransition(rage, "SPAWN", ChooseState);
            Satchel.FsmUtil.AddTransition(choose, EventAntic, AnticState);
            Satchel.FsmUtil.AddTransition(choose, EventNone, "Rage");
            Satchel.FsmUtil.AddTransition(antic, "FINISHED", NextState);
            Satchel.FsmUtil.AddTransition(next, EventSpawnFirst, "Spawn 2");
            Satchel.FsmUtil.AddTransition(next, EventSpawnMore, "Spawn");
            Satchel.FsmUtil.AddTransition(next, EventDone, "Rage");
            Satchel.FsmUtil.ChangeTransition(spawn2, "FINISHED", NextState);
            Satchel.FsmUtil.ChangeTransition(spawn2Cancel, "FINISHED", NextState);
            Satchel.FsmUtil.ChangeTransition(spawn1, "FINISHED", NextState);
            Satchel.FsmUtil.ChangeTransition(spawn1Cancel, "FINISHED", NextState);
        }

        private void RememberCageFlies()
        {
            foreach (Transform child in cage)
            {
                if (child != null)
                {
                    knownFlies.Add(child.gameObject);
                }
            }
        }

        private int CountAliveFlies()
        {
            RememberCageFlies();
            knownFlies.RemoveWhere(fly => fly == null);
            int alive = 0;
            foreach (GameObject fly in knownFlies)
            {
                if (fly.transform.parent != cage)
                {
                    alive++;
                }
            }

            return alive;
        }

        private void Choose()
        {
            bool custom = moduleActive && ShouldApplySettings(fsm.gameObject);
            int count = custom ? ClampFlukesPerWave(flukemarmFlukesPerWave) : VanillaFlukesPerWave;
            waveBudget = custom && ShouldUseCustomSummonLimit()
                ? ClampSummonLimit(flukemarmSummonLimit) - CountAliveFlies()
                : int.MaxValue;
            firstSpawnDone = false;
            waveQueue.Clear();

            Enqueue(FlukeSpawnMode.Forced, custom, count);
            Enqueue(FlukeSpawnMode.Enabled, custom, count);

            if (waveQueue.Count == 0)
            {
                fsm.SendEvent(EventNone);
                return;
            }

            foreach (int point in waveQueue)
            {
                pointFsms[point]?.SendEvent("ANTIC");
            }

            fsm.SendEvent(EventAntic);
        }

        private void Enqueue(FlukeSpawnMode mode, bool custom, int count)
        {
            candidates.Clear();
            for (int point = 0; point < points.Length; point++)
            {
                FlukeSpawnMode pointMode = custom ? GetSpawnMode(point) : FlukeSpawnMode.Enabled;
                if (pointMode == mode)
                {
                    candidates.Add(point);
                }
            }

            Shuffle(candidates);
            for (int i = 0; i < candidates.Count && waveQueue.Count < count; i++)
            {
                waveQueue.Enqueue(candidates[i]);
            }
        }

        private void Next()
        {
            while (waveQueue.Count > 0)
            {
                int point = waveQueue.Dequeue();
                if (waveBudget <= 0)
                {
                    pointFsms[point]?.SendEvent("CANCEL");
                    continue;
                }

                waveBudget--;
                if (!firstSpawnDone)
                {
                    firstSpawnDone = true;
                    spawnPoint2.Value = points[point];
                    fsm.SendEvent(EventSpawnFirst);
                }
                else
                {
                    spawnPoint.Value = points[point];
                    fsm.SendEvent(EventSpawnMore);
                }

                return;
            }

            fsm.SendEvent(EventDone);
        }

        private static void Shuffle(List<int> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
