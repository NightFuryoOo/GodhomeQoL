using System;
using System.Collections;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using Satchel.BetterMenus;
using Satchel.BetterMenus.Config;

namespace GodhomeQoL.Modules.QoL;

public sealed class DreamshieldStartAngle : Module
{
    [GlobalSetting]
    [BoolOption]
    public static bool startAngleEnabled = false;

    [GlobalSetting]
    [FloatOption(0f, 10f, OptionType.Slider)]
    public static float rotationDelay = 0f;

    [GlobalSetting]
    [FloatOption(0.1f, 10f, OptionType.Slider)]
    public static float rotationSpeed = 1f;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;
    public override bool Hidden => true;
    public override bool AlwaysEnabled => true;

    private static readonly Dictionary<Rotate, (float x, float y, float z)> rotateOriginals = [];
    private static readonly Dictionary<int, PendingRotatePatch> pendingRotatePatches = [];
    private static readonly HashSet<int> runningDelayCoroutines = [];
    private static readonly HashSet<int> pendingDelayReapply = [];
    private static readonly Dictionary<int, PlayMakerFSM> trackedShieldFsms = [];
    private static bool hooksInstalled;

    private sealed class PendingRotatePatch
    {
        public PlayMakerFSM Fsm { get; init; } = null!;
        public List<(int idx, Rotate? rot)> RemovedActions { get; init; } = new();
    }

    internal static void SetEnabled(bool enabled)
    {
        if (startAngleEnabled == enabled)
        {
            if (enabled)
            {
                EnsureRuntimeTracking();
                ApplyToExistingShields();
                ApplyRotationSpeedToExistingShields();
            }

            return;
        }

        startAngleEnabled = enabled;
        if (enabled)
        {
            EnsureRuntimeTracking();
            ApplyToExistingShields();
            ApplyRotationSpeedToExistingShields();
        }
        else
        {
            RestorePatchedShieldsAndResetState();
            RemoveHooks();
            trackedShieldFsms.Clear();
        }
    }

    internal static void SetRotationDelay(float value)
    {
        rotationDelay = Mathf.Clamp((float)Math.Round(value, 2), 0f, 10f);
        if (startAngleEnabled)
        {
            ApplyToExistingShields();
        }
    }

    internal static void SetRotationSpeed(float value)
    {
        rotationSpeed = Mathf.Clamp((float)Math.Round(value, 2), 0f, 10f);
        if (!startAngleEnabled)
        {
            return;
        }

        ApplyRotationSpeedToExistingShields();
        ApplyRotationSpeedToPendingPatches();
    }

    internal static void ResetDefaults()
    {
        SetEnabled(false);
        rotationDelay = 0f;
        rotationSpeed = 1f;
    }

    private protected override void Load()
    {
        runningDelayCoroutines.Clear();
        pendingDelayReapply.Clear();
        pendingRotatePatches.Clear();
        trackedShieldFsms.Clear();
        hooksInstalled = false;
        if (startAngleEnabled)
        {
            EnsureRuntimeTracking();
        }
    }

    private protected override void Unload()
    {
        RemoveHooks();
        RestorePatchedShieldsAndResetState();
        pendingDelayReapply.Clear();
        trackedShieldFsms.Clear();
        hooksInstalled = false;
    }

    private static void OnFsmEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);
        TrackDreamshieldFsm(self);

        if (!ShouldHandle(self))
        {
            return;
        }

        _ = HeroController.instance?.StartCoroutine(DelayRotate(self, rotationDelay));
        LogFollowActions(self, "OnEnable");
    }

    private static void OnSceneChanged(Scene previous, Scene next)
    {
        ApplyToExistingShields();
        PruneRotateOriginals();
    }

    private static void ApplyToExistingShields()
    {
        if (!startAngleEnabled)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in GetTrackedShields())
        {
            if (!ShouldHandle(fsm))
            {
                continue;
            }

            HeroController.instance?.StartCoroutine(DelayRotate(fsm, rotationDelay));
            LogFollowActions(fsm, "ApplyExisting");
        }
    }

    private static void RestorePatchedShieldsAndResetState()
    {
        foreach (PendingRotatePatch patch in pendingRotatePatches.Values.ToArray())
        {
            PlayMakerFSM? fsm = patch.Fsm;
            if (fsm == null)
            {
                continue;
            }

            _ = TryRestoreRemovedRotateActions(fsm, patch.RemovedActions, applyScaledSpeed: false);
        }

        pendingRotatePatches.Clear();
        runningDelayCoroutines.Clear();

        foreach ((Rotate rotate, (float x, float y, float z) orig) in rotateOriginals.ToArray())
        {
            if (rotate == null)
            {
                continue;
            }

            SetFloat(rotate, "xAngle", orig.x);
            SetFloat(rotate, "yAngle", orig.y);
            SetFloat(rotate, "zAngle", orig.z);
        }

        rotateOriginals.Clear();
    }

    private static bool ShouldHandle(PlayMakerFSM fsm)
    {
        return startAngleEnabled && IsDreamshieldControlFsm(fsm);
    }

    private static bool IsDreamshieldControlFsm(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null)
        {
            return false;
        }

        bool isControl = string.Equals(fsm.FsmName, "Control", StringComparison.Ordinal);
        string objName = fsm.gameObject.name ?? string.Empty;
        bool isShield = objName.IndexOf("Orbit Shield", StringComparison.OrdinalIgnoreCase) >= 0
            || objName.IndexOf("Dreamshield", StringComparison.OrdinalIgnoreCase) >= 0
            || objName.IndexOf("Dream Shield", StringComparison.OrdinalIgnoreCase) >= 0;

        return isControl && isShield;
    }

    internal static IEnumerable<Element> MenuElements()
    {
        CustomSlider delaySlider = new(
            "Settings/DreamshieldStartAngle/rotationDelay".Localize(),
            SetRotationDelay,
            () => (float)Math.Round(rotationDelay, 2),
            0f,
            10f,
            false
        );
        delaySlider.isVisible = startAngleEnabled;

        CustomSlider speedSlider = new(
            "Settings/DreamshieldStartAngle/rotationSpeed".Localize(),
            SetRotationSpeed,
            () => rotationSpeed,
            0.0f,
            10f,
            false
        );
        speedSlider.isVisible = startAngleEnabled;

        Element toggle = Blueprints.HorizontalBoolOption(
            "Modules/DreamshieldStartAngle".Localize(),
            "ToggleableLevel/ChangeScene".Localize(),
            b =>
            {
                SetEnabled(b);
                if (b)
                {
                    delaySlider.Show();
                    speedSlider.Show();
                }
                else
                {
                    delaySlider.Hide();
                    speedSlider.Hide();
                }
            },
            () => startAngleEnabled
        );

        delaySlider.OnUpdate += (_, _) => delaySlider.isVisible = startAngleEnabled;
        speedSlider.OnUpdate += (_, _) => speedSlider.isVisible = startAngleEnabled;

        return new Element[] { toggle, delaySlider, speedSlider };
    }

    private static void LogFollowActions(PlayMakerFSM fsm, string tag)
    {
        FsmState? follow = fsm.Fsm.GetState("Follow");
        if (follow?.Actions == null)
        {
            return;
        }

        string types = follow.Actions
            .Select(a => a.GetType().FullName ?? a.GetType().Name)
            .Join(", ");
        LogDebug($"[DreamshieldStartAngle] {tag} Follow actions: {types}");
    }

    private static IEnumerator DelayRotate(PlayMakerFSM fsm, float delaySeconds)
    {
        int fsmId = fsm.GetInstanceID();
        if (!runningDelayCoroutines.Add(fsmId))
        {
            pendingDelayReapply.Add(fsmId);
            yield break;
        }

        PendingRotatePatch? patch = null;
        bool patchRestored = false;

        try
        {
            FsmState? follow = fsm.Fsm.GetState("Follow");
            if (follow?.Actions == null)
            {
                yield break;
            }

            List<FsmStateAction> actions = follow.Actions.ToList();
            List<(int idx, Rotate? rot)> removed = actions
                .Select((a, i) => (idx: i, rot: a as Rotate))
                .Where(t => t.rot != null)
                .ToList();

            if (removed.Count == 0)
            {
                yield break;
            }

            patch = new PendingRotatePatch
            {
                Fsm = fsm,
                RemovedActions = removed
            };
            pendingRotatePatches[fsmId] = patch;

            removed.OrderByDescending(t => t.idx).ForEach(t => actions.RemoveAt(t.idx));
            follow.Actions = actions.ToArray();

            if (delaySeconds > 0f)
            {
                yield return new WaitForSecondsRealtime(delaySeconds);
            }
            else
            {
                yield return null;
            }

            bool applyScaledSpeed = ShouldHandle(fsm);
            if (!TryRestoreRemovedRotateActions(fsm, removed, applyScaledSpeed))
            {
                yield break;
            }

            patchRestored = true;
            pendingRotatePatches.Remove(fsmId);
        }
        finally
        {
            if (patch != null && !patchRestored)
            {
                _ = TryRestoreRemovedRotateActions(fsm, patch.RemovedActions, applyScaledSpeed: false);
                pendingRotatePatches.Remove(fsmId);
            }

            runningDelayCoroutines.Remove(fsmId);

            bool restartRequested = pendingDelayReapply.Remove(fsmId);
            if (restartRequested && ShouldHandle(fsm))
            {
                _ = HeroController.instance?.StartCoroutine(DelayRotate(fsm, rotationDelay));
            }

            PruneRotateOriginals();
        }
    }

    private static bool TryRestoreRemovedRotateActions(PlayMakerFSM fsm, List<(int idx, Rotate? rot)> removed, bool applyScaledSpeed)
    {
        FsmState? follow = fsm.Fsm.GetState("Follow");
        if (follow == null)
        {
            return false;
        }

        List<FsmStateAction> actions = follow.Actions?.ToList() ?? [];
        foreach ((int idx, Rotate? rot) in removed.OrderBy(t => t.idx))
        {
            if (rot == null)
            {
                continue;
            }

            if (applyScaledSpeed)
            {
                ApplyRotationSpeed(rot);
            }
            else
            {
                RestoreRotationSpeed(rot);
            }

            if (!actions.Contains(rot))
            {
                int insertIdx = Mathf.Clamp(idx, 0, actions.Count);
                actions.Insert(insertIdx, rot);
            }
        }

        follow.Actions = actions.ToArray();
        if (applyScaledSpeed)
        {
            // Avoid forcing a transition from unrelated states.
            TryRefreshFollowState(fsm);
        }

        return true;
    }

    private static void ApplyRotationSpeedToPendingPatches()
    {
        foreach (PendingRotatePatch patch in pendingRotatePatches.Values)
        {
            foreach ((_, Rotate? rot) in patch.RemovedActions)
            {
                if (rot != null)
                {
                    ApplyRotationSpeed(rot);
                }
            }
        }
    }

    private static void ApplyRotationSpeedToExistingShields()
    {
        foreach (PlayMakerFSM fsm in GetTrackedShields())
        {
            if (!IsDreamshieldControlFsm(fsm))
            {
                continue;
            }

            FsmState? follow = fsm.Fsm.GetState("Follow");
            if (follow?.Actions == null)
            {
                continue;
            }

            bool anyRotates = false;
            foreach (Rotate rotate in follow.Actions.OfType<Rotate>())
            {
                anyRotates = true;
                ApplyRotationSpeed(rotate);
            }

            if (anyRotates && startAngleEnabled)
            {
                TryRefreshFollowState(fsm);
            }
        }

        PruneRotateOriginals();
    }

    private static void TrackDreamshieldFsm(PlayMakerFSM? fsm)
    {
        if (fsm == null || !IsDreamshieldControlFsm(fsm))
        {
            return;
        }

        trackedShieldFsms[fsm.GetInstanceID()] = fsm;
    }

    private static void SeedTrackedShields()
    {
        HeroController? hero = HeroController.instance;
        if (hero == null)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in hero.gameObject.GetComponentsInChildren<PlayMakerFSM>(true))
        {
            TrackDreamshieldFsm(fsm);
        }
    }

    private static void EnsureRuntimeTracking()
    {
        InstallHooks();
        if (trackedShieldFsms.Count == 0)
        {
            SeedTrackedShields();
        }
    }

    private static void InstallHooks()
    {
        if (hooksInstalled)
        {
            return;
        }

        On.PlayMakerFSM.OnEnable += OnFsmEnable;
        USceneManager.activeSceneChanged += OnSceneChanged;
        hooksInstalled = true;
    }

    private static void RemoveHooks()
    {
        if (!hooksInstalled)
        {
            return;
        }

        On.PlayMakerFSM.OnEnable -= OnFsmEnable;
        USceneManager.activeSceneChanged -= OnSceneChanged;
        hooksInstalled = false;
    }

    private static List<PlayMakerFSM> GetTrackedShields()
    {
        List<int> deadIds = [];
        List<PlayMakerFSM> result = [];
        foreach ((int id, PlayMakerFSM fsm) in trackedShieldFsms)
        {
            if (fsm == null || fsm.gameObject == null)
            {
                deadIds.Add(id);
                continue;
            }

            result.Add(fsm);
        }

        foreach (int id in deadIds)
        {
            trackedShieldFsms.Remove(id);
        }

        return result;
    }

    private static void PruneRotateOriginals()
    {
        if (rotateOriginals.Count == 0)
        {
            return;
        }

        HashSet<Rotate> liveRotates = [];
        foreach (PlayMakerFSM fsm in GetTrackedShields())
        {
            FsmState? follow = fsm.Fsm.GetState("Follow");
            if (follow?.Actions == null)
            {
                continue;
            }

            foreach (Rotate rotate in follow.Actions.OfType<Rotate>())
            {
                liveRotates.Add(rotate);
            }
        }

        foreach (PendingRotatePatch patch in pendingRotatePatches.Values)
        {
            foreach ((_, Rotate? rot) in patch.RemovedActions)
            {
                if (rot != null)
                {
                    liveRotates.Add(rot);
                }
            }
        }

        foreach (Rotate rotate in rotateOriginals.Keys.ToArray())
        {
            if (rotate == null)
            {
                continue;
            }

            if (!liveRotates.Contains(rotate))
            {
                rotateOriginals.Remove(rotate);
            }
        }
    }

    private static void TryRefreshFollowState(PlayMakerFSM fsm)
    {
        if (!string.Equals(fsm.ActiveStateName, "Follow", StringComparison.Ordinal))
        {
            return;
        }

        fsm.Fsm.SetState("Follow");
    }

    private static void ApplyRotationSpeed(Rotate rotate)
    {
        if (!rotateOriginals.TryGetValue(rotate, out (float x, float y, float z) orig))
        {
            orig = (GetFloat(rotate, "xAngle"), GetFloat(rotate, "yAngle"), GetFloat(rotate, "zAngle"));
            rotateOriginals[rotate] = orig;
        }

        SetFloat(rotate, "xAngle", orig.x * rotationSpeed);
        SetFloat(rotate, "yAngle", orig.y * rotationSpeed);
        SetFloat(rotate, "zAngle", orig.z * rotationSpeed);
    }

    private static void RestoreRotationSpeed(Rotate rotate)
    {
        if (!rotateOriginals.TryGetValue(rotate, out (float x, float y, float z) orig))
        {
            return;
        }

        SetFloat(rotate, "xAngle", orig.x);
        SetFloat(rotate, "yAngle", orig.y);
        SetFloat(rotate, "zAngle", orig.z);
    }

    private static float GetFloat(object target, string fieldName)
    {
        FieldInfo? fi = target.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (fi == null)
        {
            return 0f;
        }

        if (fi.GetValue(target) is FsmFloat fsmFloat)
        {
            return fsmFloat.Value;
        }

        if (fi.GetValue(target) is float f)
        {
            return f;
        }

        return 0f;
    }

    private static void SetFloat(object target, string fieldName, float value)
    {
        FieldInfo? fi = target.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (fi == null)
        {
            return;
        }

        if (fi.GetValue(target) is FsmFloat fsmFloat)
        {
            fsmFloat.Value = value;
        }
        else if (fi.FieldType == typeof(float))
        {
            fi.SetValue(target, value);
        }
    }

}
