namespace GodhomeQoL.Modules.Performance;

public sealed partial class FpsBoost : Module
{
    private sealed class ParticleOriginal
    {
        public int maxParticles;
        public bool emissionEnabled;
        public float rateOverTimeMultiplier;
        public float rateOverDistanceMultiplier;
        public ParticleSystem.Burst[] bursts = Array.Empty<ParticleSystem.Burst>();
        public float appliedFactor = 1f;
    }

    private static readonly Dictionary<ParticleSystem, ParticleOriginal> particleStates = new();
    private static readonly HashSet<ParticleSystem> ineligibleParticles = new();
    private static readonly List<ParticleSystem> spawnScratch = new();

    private static bool IsParticleEligible(ParticleSystem ps)
    {
        ParticleSystem.CollisionModule collision = ps.collision;
        if (collision.enabled && collision.sendCollisionMessages)
        {
            return false;
        }

        return !ps.trigger.enabled;
    }

    private static void ApplyParticleScale(ParticleSystem ps, float factor)
    {
        if (ps == null)
        {
            return;
        }

        if (!particleStates.TryGetValue(ps, out ParticleOriginal? original))
        {
            if (factor >= 0.999f || ineligibleParticles.Contains(ps))
            {
                return;
            }

            if (!IsParticleEligible(ps))
            {
                ineligibleParticles.Add(ps);
                return;
            }

            original = CaptureParticle(ps);
            particleStates[ps] = original;
        }

        if (factor >= 0.999f)
        {
            RestoreParticle(ps, original);
            particleStates.Remove(ps);
            return;
        }

        if (Mathf.Approximately(original.appliedFactor, factor))
        {
            return;
        }

        WriteParticle(ps, original, factor);
        original.appliedFactor = factor;
    }

    private static ParticleOriginal CaptureParticle(ParticleSystem ps)
    {
        ParticleSystem.MainModule main = ps.main;
        ParticleSystem.EmissionModule emission = ps.emission;

        ParticleOriginal original = new()
        {
            maxParticles = main.maxParticles,
            emissionEnabled = emission.enabled,
            rateOverTimeMultiplier = emission.rateOverTimeMultiplier,
            rateOverDistanceMultiplier = emission.rateOverDistanceMultiplier,
            bursts = new ParticleSystem.Burst[emission.burstCount]
        };

        emission.GetBursts(original.bursts);
        return original;
    }

    private static void WriteParticle(ParticleSystem ps, ParticleOriginal original, float factor)
    {
        ParticleSystem.MainModule main = ps.main;
        ParticleSystem.EmissionModule emission = ps.emission;

        if (factor <= 0.001f)
        {
            emission.enabled = false;
            return;
        }

        emission.enabled = original.emissionEnabled;
        emission.rateOverTimeMultiplier = original.rateOverTimeMultiplier * factor;
        emission.rateOverDistanceMultiplier = original.rateOverDistanceMultiplier * factor;
        main.maxParticles = Mathf.Max(1, Mathf.RoundToInt(original.maxParticles * factor));

        if (original.bursts.Length > 0)
        {
            ParticleSystem.Burst[] scaled = new ParticleSystem.Burst[original.bursts.Length];
            for (int i = 0; i < scaled.Length; i++)
            {
                scaled[i] = ScaleBurst(original.bursts[i], factor);
            }

            emission.SetBursts(scaled);
        }
    }

    private static ParticleSystem.Burst ScaleBurst(ParticleSystem.Burst burst, float factor)
    {
        ParticleSystem.MinMaxCurve count = burst.count;
        switch (count.mode)
        {
            case ParticleSystemCurveMode.Constant:
                float scaled = count.constant * factor;
                if (scaled >= 1f || count.constant < 1f)
                {
                    count.constant = scaled;
                }
                else
                {
                    count.constant = 1f;
                    burst.probability *= scaled;
                }

                break;
            case ParticleSystemCurveMode.TwoConstants:
                count.constantMin *= factor;
                count.constantMax *= factor;
                break;
            default:
                count.curveMultiplier *= factor;
                break;
        }

        burst.count = count;
        return burst;
    }

    private static void RestoreParticle(ParticleSystem ps, ParticleOriginal original)
    {
        if (ps == null)
        {
            return;
        }

        ParticleSystem.MainModule main = ps.main;
        ParticleSystem.EmissionModule emission = ps.emission;
        main.maxParticles = original.maxParticles;
        emission.enabled = original.emissionEnabled;
        emission.rateOverTimeMultiplier = original.rateOverTimeMultiplier;
        emission.rateOverDistanceMultiplier = original.rateOverDistanceMultiplier;
        if (original.bursts.Length > 0)
        {
            emission.SetBursts(original.bursts);
        }
    }

    private static float CurrentParticleFactor() => GetParticleAmount() / 100f;

    private static void ScanParticlesActive()
    {
        float factor = CurrentParticleFactor();
        ParticleSystem[] all = UObject.FindObjectsOfType<ParticleSystem>();
        for (int i = 0; i < all.Length; i++)
        {
            ApplyParticleScale(all[i], factor);
        }
    }

    private static void ApplyParticlesEverywhere()
    {
        float factor = CurrentParticleFactor();

        List<ParticleSystem> tracked = new(particleStates.Keys);
        for (int i = 0; i < tracked.Count; i++)
        {
            ParticleSystem ps = tracked[i];
            if (ps == null)
            {
                particleStates.Remove(ps!);
                continue;
            }

            ApplyParticleScale(ps, factor);
        }

        if (factor < 0.999f)
        {
            ScanParticlesActive();
        }
    }

    private static void RestoreParticles()
    {
        List<KeyValuePair<ParticleSystem, ParticleOriginal>> tracked = new(particleStates);
        for (int i = 0; i < tracked.Count; i++)
        {
            RestoreParticle(tracked[i].Key, tracked[i].Value);
        }

        particleStates.Clear();
        ineligibleParticles.Clear();
    }

    private static void PruneParticles()
    {
        List<ParticleSystem> dead = new();
        foreach (ParticleSystem ps in particleStates.Keys)
        {
            if (ps == null)
            {
                dead.Add(ps!);
            }
        }

        for (int i = 0; i < dead.Count; i++)
        {
            particleStates.Remove(dead[i]);
        }

        ineligibleParticles.RemoveWhere(ps => ps == null);
    }

    private static GameObject? AfterSpawn(GameObject? spawned)
    {
        if (spawned == null || !moduleActive || (fpsParticleAmount >= 100 && particleStates.Count == 0))
        {
            return spawned;
        }

        try
        {
            float factor = CurrentParticleFactor();
            spawned.GetComponentsInChildren(true, spawnScratch);
            for (int i = 0; i < spawnScratch.Count; i++)
            {
                ApplyParticleScale(spawnScratch[i], factor);
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "FpsBoost.Particles.cs");
        }
        finally
        {
            spawnScratch.Clear();
        }

        return spawned;
    }

    private static GameObject OnSpawn(On.ObjectPool.orig_Spawn_GameObject orig, GameObject prefab)
    {
        return AfterSpawn(orig(prefab))!;
    }

    private static GameObject OnSpawnPosition(On.ObjectPool.orig_Spawn_GameObject_Vector3 orig, GameObject prefab, Vector3 position)
    {
        return AfterSpawn(orig(prefab, position))!;
    }

    private static GameObject OnSpawnParent(On.ObjectPool.orig_Spawn_GameObject_Transform orig, GameObject prefab, Transform parent)
    {
        return AfterSpawn(orig(prefab, parent))!;
    }

    private static GameObject OnSpawnPositionRotation(On.ObjectPool.orig_Spawn_GameObject_Vector3_Quaternion orig, GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return AfterSpawn(orig(prefab, position, rotation))!;
    }

    private static GameObject OnSpawnParentPosition(On.ObjectPool.orig_Spawn_GameObject_Transform_Vector3 orig, GameObject prefab, Transform parent, Vector3 position)
    {
        return AfterSpawn(orig(prefab, parent, position))!;
    }

    private static GameObject OnSpawnParentPositionRotation(On.ObjectPool.orig_Spawn_GameObject_Transform_Vector3_Quaternion orig, GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
    {
        return AfterSpawn(orig(prefab, parent, position, rotation))!;
    }

    private static void OnReduceParticleEffectsSetEmission(On.ReduceParticleEffects.orig_SetEmission orig, ReduceParticleEffects self)
    {
        orig(self);

        if (!moduleActive || self == null || (fpsParticleAmount >= 100 && particleStates.Count == 0))
        {
            return;
        }

        try
        {
            ParticleSystem? emitter = ReflectionHelper.GetField<ReduceParticleEffects, ParticleSystem>(self, "emitter");
            if (emitter != null)
            {
                particleStates.Remove(emitter);
                ApplyParticleScale(emitter, CurrentParticleFactor());
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "FpsBoost.Particles.cs");
        }
    }
}
