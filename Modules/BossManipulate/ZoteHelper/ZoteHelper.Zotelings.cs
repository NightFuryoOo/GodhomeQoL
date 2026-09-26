using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class ZoteHelper : Module
{
    private const string ZotelingPrefix = "Zoteling";

    internal static void ApplyZotelingHealthIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        if (!hoGEntryAllowed)
        {
            return;
        }

        PruneZotelings();
        foreach (GameObject zoteling in activeZotelings)
        {
            ApplyZotelingHealth(zoteling);
        }
    }

    private static void ApplyZotelingChanges(PlayMakerFSM fsm)
    {
        FsmState? choice = fsm.Fsm.GetState("Choice");
        if (choice != null)
        {
            choice.InsertCustomAction(() =>
            {
                if (!moduleActive)
                {
                    return;
                }

                if (!zoteSpawnFlying && !zoteSpawnHopping)
                {
                    UObject.Destroy(fsm.gameObject);
                    return;
                }

                if (zoteSpawnFlying ^ zoteSpawnHopping)
                {
                    ZotelingType forcedType = zoteSpawnFlying ? ZotelingType.Flying : ZotelingType.Hopping;
                    SetZotelingTypeMarker(fsm.gameObject, forcedType);
                    fsm.SendEvent(zoteSpawnFlying ? "BUZZER" : "HOPPER");
                    ApplyZotelingHealthOnce(fsm.gameObject);
                }
            }, 0);
        }

        FsmState? reset = fsm.Fsm.GetState("Reset");
        if (reset != null)
        {
            reset.AddCustomAction(() =>
            {
                if (!moduleActive)
                {
                    return;
                }

                UObject.Destroy(fsm.gameObject);
            });
        }

        AttachZotelingTypeHooks(fsm);
        if (TryGetZotelingTypeMarker(fsm.gameObject, out _))
        {
            ApplyZotelingHealthOnce(fsm.gameObject);
        }
    }

    private static bool IsZotelingObject(GameObject gameObject)
    {
        string name = gameObject.name;
        return name.StartsWith(ZotelingPrefix, StringComparison.Ordinal);
    }

    private static void ApplyZotelingHealth(GameObject zoteling, HealthManager? hm = null)
    {
        if (zoteling == null)
        {
            return;
        }

        if (IsBalloon(zoteling.name))
        {
            return;
        }

        hm ??= zoteling.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        int? desiredHp = GetCustomZotelingHp(zoteling);
        if (!desiredHp.HasValue)
        {
            return;
        }

        int targetHp = ClampSummonHp(desiredHp.Value);
        hm.hp = targetHp;
    }

    private static int? GetCustomZotelingHp(GameObject zoteling)
    {
        return GetZotelingType(zoteling) switch
        {
            ZotelingType.Flying when zoteUseCustomFlyingHp => zoteSummonFlyingHp,
            ZotelingType.Hopping when zoteUseCustomHoppingHp => zoteSummonHoppingHp,
            _ => null
        };
    }

    private static ZotelingType GetZotelingType(GameObject zoteling)
    {
        if (TryGetZotelingTypeMarker(zoteling, out ZotelingType markerType))
        {
            return markerType;
        }

        PlayMakerFSM? control = GetControlFsm(zoteling);
        if (control != null)
        {
            return GetZotelingType(control);
        }

        ZotelingType? byName = GetZotelingTypeByName(zoteling.name);
        return byName ?? ZotelingType.Hopping;
    }

    private static bool IsBalloon(string name)
    {
        return name.IndexOf("balloon", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static ZotelingType GetZotelingType(PlayMakerFSM fsm)
    {
        string? activeStateName = fsm.ActiveStateName;
        if (!string.IsNullOrEmpty(activeStateName))
        {
            if (activeStateName.IndexOf("buzzer", StringComparison.OrdinalIgnoreCase) >= 0
                || activeStateName.IndexOf("fly", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ZotelingType.Flying;
            }

            if (activeStateName.IndexOf("hopper", StringComparison.OrdinalIgnoreCase) >= 0
                || activeStateName.IndexOf("hop", StringComparison.OrdinalIgnoreCase) >= 0
                || activeStateName.IndexOf("jump", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ZotelingType.Hopping;
            }
        }

        ZotelingType? byName = GetZotelingTypeByName(fsm.gameObject.name);
        if (byName.HasValue)
        {
            return byName.Value;
        }

        return ZotelingType.Hopping;
    }

    private static ZotelingType? GetZotelingTypeByName(string name)
    {
        if (name.IndexOf("fly", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return ZotelingType.Flying;
        }

        if (name.IndexOf("hop", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return ZotelingType.Hopping;
        }

        return null;
    }

    private static PlayMakerFSM? GetControlFsm(GameObject zoteling)
    {
        PlayMakerFSM[] fsms = zoteling.GetComponents<PlayMakerFSM>();
        for (int i = 0; i < fsms.Length; i++)
        {
            PlayMakerFSM fsm = fsms[i];
            if (fsm.FsmName == "Control")
            {
                return fsm;
            }
        }

        return null;
    }

    private static void AttachZotelingTypeHooks(PlayMakerFSM fsm)
    {
        foreach (FsmState state in fsm.FsmStates)
        {
            string stateName = state.Name;
            if (stateName.IndexOf("buzzer", StringComparison.OrdinalIgnoreCase) >= 0
                || stateName.IndexOf("fly", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                state.InsertCustomAction(() =>
                {
                    if (!moduleActive)
                    {
                        return;
                    }

                    SetZotelingTypeMarker(fsm.gameObject, ZotelingType.Flying);
                    ApplyZotelingHealthOnce(fsm.gameObject);
                }, 0);
            }
            else if (stateName.IndexOf("hopper", StringComparison.OrdinalIgnoreCase) >= 0
                || stateName.IndexOf("hop", StringComparison.OrdinalIgnoreCase) >= 0
                || stateName.IndexOf("jump", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                state.InsertCustomAction(() =>
                {
                    if (!moduleActive)
                    {
                        return;
                    }

                    SetZotelingTypeMarker(fsm.gameObject, ZotelingType.Hopping);
                    ApplyZotelingHealthOnce(fsm.gameObject);
                }, 0);
            }
        }
    }

    private static void ApplyZotelingHealthOnce(GameObject zoteling)
    {
        if (zoteling == null)
        {
            return;
        }

        ZotelingTypeMarker marker = zoteling.GetComponent<ZotelingTypeMarker>() ?? zoteling.AddComponent<ZotelingTypeMarker>();
        if (marker.HealthInitialized)
        {
            return;
        }

        ApplyZotelingHealth(zoteling);
        marker.HealthInitialized = true;
    }

    private static void SetZotelingTypeMarker(GameObject zoteling, ZotelingType type)
    {
        if (zoteling == null)
        {
            return;
        }

        ZotelingTypeMarker marker = zoteling.GetComponent<ZotelingTypeMarker>() ?? zoteling.AddComponent<ZotelingTypeMarker>();
        marker.Type = type;
        marker.TypeSet = true;
    }

    private static bool TryGetZotelingTypeMarker(GameObject zoteling, out ZotelingType type)
    {
        ZotelingTypeMarker? marker = zoteling.GetComponent<ZotelingTypeMarker>();
        if (marker != null && marker.TypeSet)
        {
            type = marker.Type;
            return true;
        }

        type = default;
        return false;
    }

    private static void TrackZoteling(GameObject zoteling)
    {
        PruneZotelings();
        if (!activeZotelings.Contains(zoteling))
        {
            activeZotelings.Add(zoteling);
        }

        ZotelingTypeMarker? marker = zoteling.GetComponent<ZotelingTypeMarker>();
        if (marker != null)
        {
            marker.TypeSet = false;
            marker.HealthInitialized = false;
        }

        if (zoteling.GetComponent<ZotelingTracker>() == null)
        {
            zoteling.AddComponent<ZotelingTracker>();
        }
    }

    private static void UntrackZoteling(GameObject zoteling)
    {
        if (activeZotelings.Remove(zoteling))
        {
            return;
        }
    }

    private static void PruneZotelings()
    {
        for (int i = activeZotelings.Count - 1; i >= 0; i--)
        {
            if (activeZotelings[i] == null)
            {
                activeZotelings.RemoveAt(i);
            }
        }
    }
}
