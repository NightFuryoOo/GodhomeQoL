using Modding;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class FlukemarmHelper : Module
{
    private const string FlukeFlyName = "Fluke Fly";
    private const int DefaultFlukeFlyHp = 35;
    private const int DefaultFlukeFlyVanillaHp = 35;
    private const int DefaultFlukeFlyPoolSize = 6;
    private const int DefaultFlukemarmSummonLimit = 6;
    private const int DefaultFlukemarmVanillaSummonLimit = 8;
    private const int P5FlukeFlyHp = 35;
    private const int MinSummonLimit = DefaultFlukemarmSummonLimit;
    private const int MaxSummonLimit = 999;

    internal static void ApplySummonLimitSettingsIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null)
            {
                continue;
            }

            ApplySummonLimitSettings(fsm);
        }
    }

    internal static void RestoreVanillaSummonLimitsIfPresent()
    {
        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsFlukemarmSummonControlFsm(fsm))
            {
                continue;
            }

            if (!IsFlukemarmObject(fsm.gameObject))
            {
                continue;
            }

            RememberVanillaSummonLimit(fsm);
            SetSummonLimitOnFlukemarmFsm(fsm, GetVanillaSummonLimit(fsm));
        }

        DestroyCustomFlukeFlyPoolEntries();
    }

    private static void ApplySummonLimitSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsFlukemarmSummonControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        RememberVanillaSummonLimit(fsm);
        int targetLimit = ShouldUseCustomSummonLimit()
            ? ClampSummonLimit(flukemarmSummonLimit)
            : GetVanillaSummonLimit(fsm);
        SyncFlukeFlyPoolSize(fsm, targetLimit);
        SetSummonLimitOnFlukemarmFsm(fsm, targetLimit);
    }

    private static void SyncFlukeFlyPoolSize(PlayMakerFSM fsm, int targetLimit)
    {
        RemoveDestroyedPoolEntries();

        if (!ShouldUseCustomSummonLimit())
        {
            DestroyCustomFlukeFlyPoolEntries();
            return;
        }

        GameObject? cage = ResolveCageObject(fsm);
        if (cage == null)
        {
            return;
        }

        int currentPoolSize = CountFlukeFlyPoolEntries(cage);
        if (vanillaFlukeFlyPoolSize < 0)
        {
            vanillaFlukeFlyPoolSize = Math.Max(DefaultFlukeFlyPoolSize, currentPoolSize);
        }

        int requiredPoolSize = Math.Max(
            Math.Max(DefaultFlukeFlyPoolSize, vanillaFlukeFlyPoolSize),
            ClampSummonLimit(targetLimit)
        );

        int toCreate = requiredPoolSize - currentPoolSize;
        if (toCreate <= 0)
        {
            return;
        }

        GameObject? template = FindFlukeFlyPoolTemplate(cage);
        if (template == null)
        {
            return;
        }

        for (int i = 0; i < toCreate; i++)
        {
            GameObject clone = UObject.Instantiate(template, cage.transform);
            clone.name = $"{template.name} (Custom Pool)";
            customFlukeFlyPoolEntries.Add(clone);

            HealthManager hm = clone.GetComponent<HealthManager>();
            if (hm != null)
            {
                RememberVanillaHp(hm);
                if (ShouldUseCustomHp(FlukemarmTarget.Fly) && ShouldApplySettings(clone))
                {
                    ApplyFlukemarmHealth(clone, hm);
                }
            }
        }
    }

    private static GameObject? ResolveCageObject(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null)
        {
            return null;
        }

        FsmGameObject? cageVar = fsm.FsmVariables?.FindFsmGameObject("Cage");
        if (cageVar?.Value != null)
        {
            return cageVar.Value;
        }

        Transform bossRoot = fsm.gameObject.transform;
        Transform? directCage = bossRoot.Find("Cage");
        if (directCage != null)
        {
            return directCage.gameObject;
        }

        foreach (Transform child in bossRoot.GetComponentsInChildren<Transform>(true))
        {
            if (child != null && string.Equals(child.name, "Cage", StringComparison.Ordinal))
            {
                return child.gameObject;
            }
        }

        return null;
    }

    private static GameObject? FindFlukeFlyPoolTemplate(GameObject cage)
    {
        if (cage == null)
        {
            return null;
        }

        foreach (Transform child in cage.transform)
        {
            if (child == null || child.gameObject == null)
            {
                continue;
            }

            if (GetFlukemarmTarget(child.gameObject) == FlukemarmTarget.Fly)
            {
                return child.gameObject;
            }
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsFlukemarmObject(fsm.gameObject))
            {
                continue;
            }

            if (!string.Equals(fsm.FsmName, "Fluke Fly", StringComparison.Ordinal))
            {
                continue;
            }

            return fsm.gameObject;
        }

        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsFlukemarmObject(hm.gameObject))
            {
                continue;
            }

            if (GetFlukemarmTarget(hm.gameObject) == FlukemarmTarget.Fly)
            {
                return hm.gameObject;
            }
        }

        return null;
    }

    private static int CountFlukeFlyPoolEntries(GameObject cage)
    {
        if (cage == null)
        {
            return 0;
        }

        int count = 0;
        foreach (Transform child in cage.transform)
        {
            if (child == null || child.gameObject == null)
            {
                continue;
            }

            if (GetFlukemarmTarget(child.gameObject) == FlukemarmTarget.Fly)
            {
                count++;
            }
        }

        return count;
    }

    private static void RemoveDestroyedPoolEntries()
    {
        for (int i = customFlukeFlyPoolEntries.Count - 1; i >= 0; i--)
        {
            if (customFlukeFlyPoolEntries[i] == null)
            {
                customFlukeFlyPoolEntries.RemoveAt(i);
            }
        }
    }

    private static void DestroyCustomFlukeFlyPoolEntries()
    {
        if (customFlukeFlyPoolEntries.Count == 0)
        {
            return;
        }

        for (int i = customFlukeFlyPoolEntries.Count - 1; i >= 0; i--)
        {
            GameObject entry = customFlukeFlyPoolEntries[i];
            if (entry != null)
            {
                UObject.Destroy(entry);
            }
        }

        customFlukeFlyPoolEntries.Clear();
    }

    private static void SetSummonLimitOnFlukemarmFsm(PlayMakerFSM fsm, int value)
    {
        int clampedLimit = ClampSummonLimit(value);

        FsmInt? spawnedMax = fsm.FsmVariables?.FindFsmInt("Spawned Max");
        if (spawnedMax != null)
        {
            spawnedMax.Value = clampedLimit;
        }

        FsmState? checkSpawn = fsm.Fsm?.GetState("Check Spawn");
        if (checkSpawn?.Actions == null)
        {
            return;
        }

        foreach (FsmStateAction action in checkSpawn.Actions)
        {
            if (action is IntCompare compare && compare.integer2 != null)
            {
                compare.integer2.Value = clampedLimit;
            }
        }
    }

    private static void RememberVanillaSummonLimit(PlayMakerFSM fsm)
    {
        int id = fsm.GetInstanceID();
        if (vanillaSummonLimitByFsm.ContainsKey(id))
        {
            return;
        }

        FsmInt? spawnedMax = fsm.FsmVariables?.FindFsmInt("Spawned Max");
        int value = spawnedMax?.Value ?? DefaultFlukemarmVanillaSummonLimit;
        vanillaSummonLimitByFsm[id] = ClampSummonLimit(value);
    }

    private static int GetVanillaSummonLimit(PlayMakerFSM fsm)
    {
        int id = fsm.GetInstanceID();
        if (vanillaSummonLimitByFsm.TryGetValue(id, out int stored))
        {
            return ClampSummonLimit(stored);
        }

        FsmInt? spawnedMax = fsm.FsmVariables?.FindFsmInt("Spawned Max");
        int value = ClampSummonLimit(spawnedMax?.Value ?? DefaultFlukemarmVanillaSummonLimit);
        vanillaSummonLimitByFsm[id] = value;
        return value;
    }

    private static int ClampSummonLimit(int value)
    {
        if (value < MinSummonLimit)
        {
            return MinSummonLimit;
        }

        return value > MaxSummonLimit ? MaxSummonLimit : value;
    }
}
