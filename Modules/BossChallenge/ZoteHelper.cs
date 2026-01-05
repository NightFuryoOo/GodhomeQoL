using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class ZoteHelper : Module
{
    private const string ZoteScene = "GG_Grey_Prince_Zote";
    private const string GreyPrinceName = "Grey Prince";
    private const string ZotelingPrefix = "Zoteling";
    private const string PatchFlag = "GodhomeQoL_ZoteHelper_Patched";
    private const int MinBossHp = 100;
    private const int MaxBossHp = 999999;
    private const int MinSummonHp = 0;
    private const int MaxSummonHp = 99;

    [LocalSetting]
    internal static int zoteBossHp = 1400;

    [LocalSetting]
    internal static bool zoteImmortal = false;

    [LocalSetting]
    internal static bool zoteSpawnFlying = true;

    [LocalSetting]
    internal static bool zoteSpawnHopping = true;

    [LocalSetting]
    internal static int zoteSummonFlyingHp = 20;

    [LocalSetting]
    internal static int zoteSummonHoppingHp = 20;

    [LocalSetting]
    internal static int zoteSummonLimit = 3;

    private static readonly List<GameObject> activeZotelings = new();
    private static bool moduleActive;

    public override bool DefaultEnabled => true;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        activeZotelings.Clear();
        On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
        On.HealthManager.Update += OnHealthManagerUpdate;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private protected override void Unload()
    {
        moduleActive = false;
        On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
        On.HealthManager.Update -= OnHealthManagerUpdate;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        activeZotelings.Clear();
    }

    internal static void ApplyBossHealthIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        GameObject? greyPrince = GameObject.Find(GreyPrinceName);
        if (greyPrince == null || greyPrince.scene.name != ZoteScene)
        {
            return;
        }

        ApplyBossHealth(greyPrince);
    }

    internal static void ApplyZotelingHealthIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        PruneZotelings();
        foreach (GameObject zoteling in activeZotelings)
        {
            ApplyZotelingHealth(zoteling);
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        if (to.name != ZoteScene)
        {
            activeZotelings.Clear();
            return;
        }

        activeZotelings.Clear();
        ApplyBossHealthIfPresent();
    }

    private static void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (self.gameObject.scene.name == ZoteScene
            && self.gameObject.name == GreyPrinceName
            && self.FsmName == "Control")
        {
            ApplyGreyPrinceChanges(self);
        }
        else if (self.gameObject.scene.name == ZoteScene
            && IsZotelingObject(self.gameObject)
            && self.FsmName == "Control")
        {
            ApplyZotelingChanges(self);
        }
    }

    private static void ApplyGreyPrinceChanges(PlayMakerFSM fsm)
    {
        if (IsPatched(fsm))
        {
            ApplyBossHealthIfPresent();
            return;
        }

        MarkPatched(fsm);

        FsmState? spitAntic = fsm.Fsm.GetState("Spit Antic");
        if (spitAntic != null)
        {
            spitAntic.RemoveAction(3);
            spitAntic.InsertCustomAction(() =>
            {
                if (!moduleActive)
                {
                    return;
                }

                PruneZotelings();
                if (activeZotelings.Count >= Math.Max(0, zoteSummonLimit))
                {
                    fsm.SendEvent("CANCEL");
                }
            }, 0);
        }

        void SummonZoteling()
        {
            if (!moduleActive)
            {
                return;
            }

            FsmGameObject zotelingVar = GetFsmGameObjectVariable(fsm, "Zoteling");
            GameObject? zotelingPrefab = zotelingVar.Value;
            if (zotelingPrefab == null)
            {
                return;
            }

            GameObject newZoteling = UObject.Instantiate(zotelingPrefab);
            zotelingVar.Value = newZoteling;
            TrackZoteling(newZoteling);
        }

        fsm.InsertCustomAction("Spit L", SummonZoteling, 1);
        fsm.InsertCustomAction("Spit R", SummonZoteling, 1);

        ApplyBossHealthAfterInit(fsm);
        ApplyBossHealth(fsm.gameObject);
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

                if (zoteSpawnFlying && zoteSpawnHopping)
                {
                    return;
                }

                if (!zoteSpawnFlying && !zoteSpawnHopping)
                {
                    UObject.Destroy(fsm.gameObject);
                    return;
                }

                fsm.SendEvent(zoteSpawnFlying ? "BUZZER" : "HOPPER");
            }, 0);

            choice.InsertCustomAction(() =>
            {
                if (!moduleActive)
                {
                    return;
                }

                HealthManager? hm = fsm.gameObject.GetComponent<HealthManager>();
                if (hm != null)
                {
                    ApplyZotelingHealth(fsm.gameObject, hm);
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

        ApplyZotelingHealth(fsm.gameObject);
    }

    private enum ZotelingType
    {
        Hopping,
        Flying
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

        int targetHp = ClampSummonHp(GetZotelingHp(zoteling));
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static int GetZotelingHp(GameObject zoteling)
    {
        return GetZotelingType(zoteling) switch
        {
            ZotelingType.Flying => zoteSummonFlyingHp,
            _ => zoteSummonHoppingHp
        };
    }

    private static ZotelingType GetZotelingType(GameObject zoteling)
    {
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
        ZotelingType? byName = GetZotelingTypeByName(fsm.gameObject.name);
        if (byName.HasValue)
        {
            return byName.Value;
        }

        foreach (FsmState state in fsm.FsmStates)
        {
            string stateName = state.Name;
            if (stateName.IndexOf("fly", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ZotelingType.Flying;
            }

            if (stateName.IndexOf("hop", StringComparison.OrdinalIgnoreCase) >= 0
                || stateName.IndexOf("jump", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ZotelingType.Hopping;
            }
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

    private static void OnHealthManagerUpdate(On.HealthManager.orig_Update orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive)
        {
            return;
        }

        if (!zoteImmortal || !IsGreyPrince(self))
        {
            return;
        }

        int targetHp = ClampBossHp(zoteBossHp);
        if (self.hp != targetHp)
        {
            ApplyBossHealth(self.gameObject, self, targetHp);
        }
    }

    private static void TrackZoteling(GameObject zoteling)
    {
        PruneZotelings();
        if (!activeZotelings.Contains(zoteling))
        {
            activeZotelings.Add(zoteling);
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

    private sealed class ZotelingTracker : MonoBehaviour
    {
        private void OnDestroy()
        {
            UntrackZoteling(gameObject);
        }
    }

    private static bool IsPatched(PlayMakerFSM fsm)
    {
        FsmBool? flag = FindFsmBool(fsm, PatchFlag);
        return flag != null && flag.Value;
    }

    private static void MarkPatched(PlayMakerFSM fsm)
    {
        FsmBool? flag = FindFsmBool(fsm, PatchFlag);
        if (flag == null)
        {
            flag = new FsmBool(PatchFlag);
            fsm.FsmVariables.BoolVariables = fsm.FsmVariables.BoolVariables.Append(flag).ToArray();
        }

        flag.Value = true;
    }

    private static FsmBool? FindFsmBool(PlayMakerFSM fsm, string name)
    {
        foreach (FsmBool variable in fsm.FsmVariables.BoolVariables)
        {
            if (variable.Name == name)
            {
                return variable;
            }
        }

        return null;
    }

    private static int ClampBossHp(int value)
    {
        if (value < MinBossHp)
        {
            return MinBossHp;
        }

        return value > MaxBossHp ? MaxBossHp : value;
    }

    private static int ClampSummonHp(int value)
    {
        if (value < MinSummonHp)
        {
            return MinSummonHp;
        }

        return value > MaxSummonHp ? MaxSummonHp : value;
    }

    private static void ApplyBossHealth(GameObject greyPrince, HealthManager? hm = null, int? overrideHp = null)
    {
        int targetHp = ClampBossHp(overrideHp ?? zoteBossHp);
        greyPrince.manageHealth(targetHp);

        hm ??= greyPrince.GetComponent<HealthManager>();
        if (hm != null)
        {
            hm.hp = targetHp;
            TrySetMaxHp(hm, targetHp);
        }
    }

    private static void ApplyBossHealthAfterInit(PlayMakerFSM fsm)
    {
        FsmState? roarEnd = fsm.Fsm.GetState("Roar End");
        if (roarEnd != null)
        {
            roarEnd.AddCustomAction(ApplyBossHealthIfPresent);
        }

        FsmState? init = fsm.Fsm.GetState("Init");
        if (init != null)
        {
            init.AddCustomAction(ApplyBossHealthIfPresent);
        }
    }

    private static void TrySetMaxHp(HealthManager hm, int value)
    {
        try
        {
            ReflectionHelper.SetField(hm, "maxHP", value);
        }
        catch
        {
            // Ignore if field not present in this version.
        }
    }

    private static bool IsGreyPrince(HealthManager hm)
    {
        if (hm == null)
        {
            return false;
        }

        return hm.gameObject.scene.name == ZoteScene
            && hm.gameObject.name == GreyPrinceName;
    }

    private static FsmGameObject GetFsmGameObjectVariable(PlayMakerFSM fsm, string name)
    {
        foreach (FsmGameObject variable in fsm.FsmVariables.GameObjectVariables)
        {
            if (variable.Name == name)
            {
                return variable;
            }
        }

        FsmGameObject created = new(name);
        fsm.FsmVariables.GameObjectVariables = fsm.FsmVariables.GameObjectVariables.Append(created).ToArray();
        return created;
    }
}
