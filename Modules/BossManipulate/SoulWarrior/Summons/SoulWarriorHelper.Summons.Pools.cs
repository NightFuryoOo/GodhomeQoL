using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class SoulWarriorHelper : Module
{
    private static void EnsureObjectPoolCapacity(GameObject? prefab, int desiredTotal)
    {
        if (prefab == null || !IsSoulWarriorSummonObject(prefab))
        {
            return;
        }

        int clampedDesired = ClampSoulWarriorSummonLimit(desiredTotal);
        if (clampedDesired <= 0)
        {
            return;
        }

        try
        {
            int pooled = ObjectPool.CountPooled(prefab);
            int spawned = ObjectPool.CountSpawned(prefab);
            int total = pooled + spawned;
            if (total >= clampedDesired)
            {
                return;
            }

            ObjectPool.CreatePool(prefab, clampedDesired);
            pooled = ObjectPool.CountPooled(prefab);
            spawned = ObjectPool.CountSpawned(prefab);
            total = pooled + spawned;

            int guard = 0;
            while (total < clampedDesired && guard < clampedDesired * 2)
            {
                GameObject spawnedObj = ObjectPool.Spawn(prefab);
                if (spawnedObj == null)
                {
                    break;
                }

                if (ShouldUseCustomSummonHp())
                {
                    HealthManager hm = spawnedObj.GetComponent<HealthManager>();
                    if (hm != null)
                    {
                        RememberVanillaSummonHp(hm);
                        ApplySoulWarriorSummonHealth(spawnedObj, hm);
                    }
                }

                ObjectPool.Recycle(spawnedObj);
                pooled = ObjectPool.CountPooled(prefab);
                spawned = ObjectPool.CountSpawned(prefab);
                total = pooled + spawned;
                guard++;
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "SoulWarriorHelper.Summons.Pools.cs");
        }
    }

    private static void RememberVanillaSummonPools(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (vanillaSummonPoolsByFsm.ContainsKey(fsmId))
        {
            return;
        }

        Dictionary<string, GameObject[]> pools = new(StringComparer.Ordinal);
        foreach ((string variableName, _, List<GameObject> entries) in FindSummonPoolVariables(fsm))
        {
            if (entries.Count == 0)
            {
                continue;
            }

            pools[variableName] = entries.Where(go => go != null).ToArray();
        }

        vanillaSummonPoolsByFsm[fsmId] = pools;
    }

    private static int ResolveVanillaSummonPoolSize(PlayMakerFSM fsm)
    {
        RememberVanillaSummonPools(fsm);
        int fsmId = fsm.GetInstanceID();
        if (!vanillaSummonPoolsByFsm.TryGetValue(fsmId, out Dictionary<string, GameObject[]>? pools) || pools == null || pools.Count == 0)
        {
            return 0;
        }

        int max = 0;
        foreach (GameObject[] entries in pools.Values)
        {
            if (entries != null && entries.Length > max)
            {
                max = entries.Length;
            }
        }

        return max;
    }

    private static void SetSummonPoolLimitOnFsm(PlayMakerFSM fsm, int limit)
    {
        int fsmId = fsm.GetInstanceID();
        RememberVanillaSummonPools(fsm);
        if (!vanillaSummonPoolsByFsm.TryGetValue(fsmId, out Dictionary<string, GameObject[]>? pools) || pools == null || pools.Count == 0)
        {
            if (ShouldUseCustomSummonLimit())
            {
                EnsureSceneSummonPoolSize(ClampSoulWarriorSummonLimit(limit));
            }

            return;
        }

        DestroyCreatedSummonClones(fsmId);
        List<GameObject> createdClones = [];
        int clampedLimit = ClampSoulWarriorSummonLimit(limit);

        foreach (KeyValuePair<string, GameObject[]> pair in pools)
        {
            FsmArray? poolVar = fsm.FsmVariables.FindFsmArray(pair.Key);
            if (poolVar == null)
            {
                continue;
            }

            List<GameObject> baseEntries = pair.Value.Where(go => go != null).ToList();
            if (baseEntries.Count == 0)
            {
                poolVar.Values = Array.Empty<object>();
                continue;
            }

            EnsureObjectPoolCapacity(baseEntries[0], clampedLimit);

            if (clampedLimit <= 0)
            {
                poolVar.Values = Array.Empty<object>();
                continue;
            }

            List<GameObject> targetEntries = [];
            int copyCount = Math.Min(clampedLimit, baseEntries.Count);
            for (int i = 0; i < copyCount; i++)
            {
                targetEntries.Add(baseEntries[i]);
            }

            int cloneIndex = 0;
            while (targetEntries.Count < clampedLimit)
            {
                GameObject source = baseEntries[cloneIndex % baseEntries.Count];
                GameObject? clone = CreateSummonPoolClone(source, targetEntries.Count + 1);
                cloneIndex++;
                if (clone == null)
                {
                    break;
                }

                createdClones.Add(clone);
                targetEntries.Add(clone);
            }

            poolVar.Values = targetEntries.Cast<object>().ToArray();
        }

        if (createdClones.Count > 0)
        {
            createdSummonClonesByFsm[fsmId] = createdClones;
        }

        List<FsmInt> summonCountVars = FindSummonCountVariables(fsm);
        if (summonCountVars.Count > 0)
        {
            int vanillaPoolSize = ResolveVanillaSummonPoolSize(fsm);
            foreach (FsmInt countVar in summonCountVars)
            {
                if (countVar != null && (countVar.Value <= 0 || countVar.Value == vanillaPoolSize))
                {
                    countVar.Value = clampedLimit;
                }
            }
        }

        if (ShouldUseCustomSummonLimit())
        {
            EnsureSceneSummonPoolSize(clampedLimit);
        }
    }

    private static void RestoreVanillaSummonPool(PlayMakerFSM fsm)
    {
        int fsmId = fsm.GetInstanceID();
        if (!vanillaSummonPoolsByFsm.TryGetValue(fsmId, out Dictionary<string, GameObject[]>? pools) || pools == null || pools.Count == 0)
        {
            return;
        }

        foreach (KeyValuePair<string, GameObject[]> pair in pools)
        {
            FsmArray? poolVar = fsm.FsmVariables.FindFsmArray(pair.Key);
            if (poolVar == null)
            {
                continue;
            }

            poolVar.Values = pair.Value.Cast<object>().ToArray();
        }

        DestroyCreatedSummonClones(fsmId);
    }

    private static GameObject? CreateSummonPoolClone(GameObject source, int index)
    {
        if (source == null)
        {
            return null;
        }

        GameObject clone = UObject.Instantiate(source);
        if (clone == null)
        {
            return null;
        }

        Transform sourceTransform = source.transform;
        Transform cloneTransform = clone.transform;
        if (sourceTransform != null && cloneTransform != null)
        {
            cloneTransform.SetParent(sourceTransform.parent, false);
            cloneTransform.localPosition = sourceTransform.localPosition;
            cloneTransform.localRotation = sourceTransform.localRotation;
            cloneTransform.localScale = sourceTransform.localScale;
        }

        clone.name = source.name;
        clone.SetActive(false);

        if (ShouldUseCustomSummonHp())
        {
            HealthManager hm = clone.GetComponent<HealthManager>();
            if (hm != null)
            {
                RememberVanillaSummonHp(hm);
                ApplySoulWarriorSummonHealth(clone, hm);
            }
        }

        return clone;
    }

    private static void EnsureSceneSummonPoolSize(int desiredCount)
    {
        int clampedDesired = ClampSoulWarriorSummonLimit(desiredCount);
        if (clampedDesired <= 0)
        {
            return;
        }

        List<GameObject> current = GetSoulWarriorSummonObjects(includeInactive: true);
        if (current.Count >= clampedDesired)
        {
            return;
        }

        List<GameObject> sources = current.Where(go => go != null).ToList();
        if (sources.Count == 0)
        {
            return;
        }

        int cloneIndex = 0;
        while (current.Count < clampedDesired)
        {
            GameObject source = sources[cloneIndex % sources.Count];
            GameObject? clone = CreateSummonPoolClone(source, current.Count + 1);
            cloneIndex++;
            if (clone == null)
            {
                break;
            }

            createdSceneSummonClones.Add(clone);
            current.Add(clone);
        }
    }

    private static List<GameObject> GetSoulWarriorSummonObjects(bool includeInactive)
    {
        Dictionary<int, GameObject> unique = new();
        IEnumerable<HealthManager> healthManagers = includeInactive
            ? Resources.FindObjectsOfTypeAll<HealthManager>()
            : UObject.FindObjectsOfType<HealthManager>();

        foreach (HealthManager hm in healthManagers)
        {
            if (hm == null || hm.gameObject == null || !IsSoulWarriorSummonObject(hm.gameObject))
            {
                continue;
            }

            if (includeInactive)
            {
                Scene scene = hm.gameObject.scene;
                if (!scene.IsValid())
                {
                    continue;
                }
            }

            int id = hm.gameObject.GetInstanceID();
            if (!unique.ContainsKey(id))
            {
                unique[id] = hm.gameObject;
            }
        }

        return unique.Values.ToList();
    }

    private static void DestroyCreatedSummonClones(int fsmId)
    {
        if (!createdSummonClonesByFsm.TryGetValue(fsmId, out List<GameObject>? clones) || clones == null)
        {
            return;
        }

        foreach (GameObject clone in clones)
        {
            if (clone != null)
            {
                UObject.Destroy(clone);
            }
        }

        createdSummonClonesByFsm.Remove(fsmId);
    }

    private static void DestroyAllCreatedSummonClones()
    {
        if (createdSummonClonesByFsm.Count == 0)
        {
            return;
        }

        foreach (List<GameObject> clones in createdSummonClonesByFsm.Values)
        {
            if (clones == null)
            {
                continue;
            }

            foreach (GameObject clone in clones)
            {
                if (clone != null)
                {
                    UObject.Destroy(clone);
                }
            }
        }

        createdSummonClonesByFsm.Clear();

        foreach (GameObject clone in createdSceneSummonClones)
        {
            if (clone != null)
            {
                UObject.Destroy(clone);
            }
        }

        createdSceneSummonClones.Clear();
    }
}
