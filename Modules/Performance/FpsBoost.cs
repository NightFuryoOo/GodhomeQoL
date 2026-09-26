namespace GodhomeQoL.Modules.Performance;

public sealed partial class FpsBoost : Module
{
    internal enum Option
    {
        Blur,
        Fog,
        Haze,
        Vignette,
        Bloom,
        ColorCorrection,
        Brightness,
        FilmGrain,
        MenuHotkeyCache
    }

    internal const int DefaultParticleAmount = 100;

    [GlobalSetting]
    internal static bool fpsNoBlur = false;

    [GlobalSetting]
    internal static bool fpsNoFog = false;

    [GlobalSetting]
    internal static bool fpsNoHaze = false;

    [GlobalSetting]
    internal static bool fpsNoVignette = false;

    [GlobalSetting]
    internal static bool fpsNoBloom = false;

    [GlobalSetting]
    internal static bool fpsNoColorCorrection = false;

    [GlobalSetting]
    internal static bool fpsSkipNeutralBrightness = false;

    [GlobalSetting]
    internal static bool fpsNoFilmGrain = false;

    [GlobalSetting]
    internal static bool fpsCacheMenuHotkeys = false;

    [GlobalSetting]
    internal static int fpsParticleAmount = DefaultParticleAmount;

    private static bool moduleActive;
    private static int rescanStage;
    private static float rescanAtTime;
    private static float nextParticleScanTime;

    public override bool DefaultEnabled => false;

    private protected override void Load()
    {
        moduleActive = true;
        USceneManager.activeSceneChanged += OnActiveSceneChanged;
        ModHooks.HeroUpdateHook += OnHeroUpdate;
        On.CameraController.ApplyEffectConfiguration += OnApplyEffectConfiguration;
        On.BrightnessEffect.SetBrightness += OnBrightnessChanged;
        On.BrightnessEffect.SetContrast += OnContrastChanged;
        On.BlurPlane.OnEnable += OnBlurPlaneEnable;
        On.BlurManager.Update += OnBlurManagerUpdate;
        On.ReduceParticleEffects.SetEmission += OnReduceParticleEffectsSetEmission;
        On.ObjectPool.Spawn_GameObject += OnSpawn;
        On.ObjectPool.Spawn_GameObject_Vector3 += OnSpawnPosition;
        On.ObjectPool.Spawn_GameObject_Transform += OnSpawnParent;
        On.ObjectPool.Spawn_GameObject_Vector3_Quaternion += OnSpawnPositionRotation;
        On.ObjectPool.Spawn_GameObject_Transform_Vector3 += OnSpawnParentPosition;
        On.ObjectPool.Spawn_GameObject_Transform_Vector3_Quaternion += OnSpawnParentPositionRotation;
        ApplyAll();
    }

    private protected override void Unload()
    {
        moduleActive = false;
        USceneManager.activeSceneChanged -= OnActiveSceneChanged;
        ModHooks.HeroUpdateHook -= OnHeroUpdate;
        On.CameraController.ApplyEffectConfiguration -= OnApplyEffectConfiguration;
        On.BrightnessEffect.SetBrightness -= OnBrightnessChanged;
        On.BrightnessEffect.SetContrast -= OnContrastChanged;
        On.BlurPlane.OnEnable -= OnBlurPlaneEnable;
        On.BlurManager.Update -= OnBlurManagerUpdate;
        On.ReduceParticleEffects.SetEmission -= OnReduceParticleEffectsSetEmission;
        On.ObjectPool.Spawn_GameObject -= OnSpawn;
        On.ObjectPool.Spawn_GameObject_Vector3 -= OnSpawnPosition;
        On.ObjectPool.Spawn_GameObject_Transform -= OnSpawnParent;
        On.ObjectPool.Spawn_GameObject_Vector3_Quaternion -= OnSpawnPositionRotation;
        On.ObjectPool.Spawn_GameObject_Transform_Vector3 -= OnSpawnParentPosition;
        On.ObjectPool.Spawn_GameObject_Transform_Vector3_Quaternion -= OnSpawnParentPositionRotation;
        RestoreAll();
    }

    internal static bool GetOption(Option option)
    {
        return option switch
        {
            Option.Blur => fpsNoBlur,
            Option.Fog => fpsNoFog,
            Option.Haze => fpsNoHaze,
            Option.Vignette => fpsNoVignette,
            Option.Bloom => fpsNoBloom,
            Option.ColorCorrection => fpsNoColorCorrection,
            Option.Brightness => fpsSkipNeutralBrightness,
            Option.FilmGrain => fpsNoFilmGrain,
            Option.MenuHotkeyCache => fpsCacheMenuHotkeys,
            _ => false
        };
    }

    internal static bool IsMenuHotkeyCacheActive() => moduleActive && fpsCacheMenuHotkeys;

    internal static void SetOption(Option option, bool value)
    {
        switch (option)
        {
            case Option.Blur:
                fpsNoBlur = value;
                break;
            case Option.Fog:
                fpsNoFog = value;
                break;
            case Option.Haze:
                fpsNoHaze = value;
                break;
            case Option.Vignette:
                fpsNoVignette = value;
                break;
            case Option.Bloom:
                fpsNoBloom = value;
                break;
            case Option.ColorCorrection:
                fpsNoColorCorrection = value;
                break;
            case Option.Brightness:
                fpsSkipNeutralBrightness = value;
                break;
            case Option.FilmGrain:
                fpsNoFilmGrain = value;
                break;
            case Option.MenuHotkeyCache:
                fpsCacheMenuHotkeys = value;
                break;
        }

        GodhomeQoL.SaveGlobalSettingsSafe();
        if (moduleActive)
        {
            ApplyOption(option);
        }
    }

    internal static int GetParticleAmount() => Mathf.Clamp(fpsParticleAmount, 0, 100);

    internal static void SetParticleAmount(int value)
    {
        fpsParticleAmount = Mathf.Clamp(value, 0, 100);
        GodhomeQoL.SaveGlobalSettingsSafe();
        if (moduleActive)
        {
            ApplyParticlesEverywhere();
        }
    }

    internal static void ResetSettings()
    {
        fpsNoBlur = false;
        fpsNoFog = false;
        fpsNoHaze = false;
        fpsNoVignette = false;
        fpsNoBloom = false;
        fpsNoColorCorrection = false;
        fpsSkipNeutralBrightness = false;
        fpsNoFilmGrain = false;
        fpsCacheMenuHotkeys = false;
        fpsParticleAmount = DefaultParticleAmount;
        renderScalePercent = 100;
        renderScalePending = false;
        GodhomeQoL.SaveGlobalSettingsSafe();
        if (moduleActive)
        {
            ApplyAll();
        }
    }

    private static void ApplyAll()
    {
        try
        {
            foreach (Option option in Enum.GetValues(typeof(Option)))
            {
                ApplyOption(option, false);
            }

            ScanScene();
            ApplyParticlesEverywhere();
            ApplyRenderScale();
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "FpsBoost.cs");
        }
    }

    private static void RestoreAll()
    {
        try
        {
            RestoreLayers();
            RestorePostEffects();
            RestoreHeroVignette();
            RestoreParticles();
            RestoreRenderScale();
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "FpsBoost.cs");
        }
    }

    private static void ApplyOption(Option option, bool scan = true)
    {
        try
        {
            switch (option)
            {
                case Option.Blur:
                case Option.Fog:
                case Option.Haze:
                case Option.Vignette:
                    if (GetOption(option))
                    {
                        if (scan)
                        {
                            ScanScene();
                        }
                    }
                    else
                    {
                        RestoreLayer(option);
                    }

                    break;
                case Option.Bloom:
                case Option.ColorCorrection:
                case Option.Brightness:
                case Option.FilmGrain:
                    ApplyPostEffectsNow();
                    break;
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "FpsBoost.cs");
        }
    }

    private static void OnActiveSceneChanged(Scene from, Scene to)
    {
        try
        {
            PruneLayers();
            PruneParticles();
            ScanScene();
            ApplyPostEffectsNow();
            if (fpsParticleAmount < 100)
            {
                ScanParticlesActive();
            }

            rescanStage = 2;
            rescanAtTime = Time.unscaledTime + 1f;
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "FpsBoost.cs");
        }
    }

    private static void OnHeroUpdate()
    {
        try
        {
            float now = Time.unscaledTime;
            if (rescanStage > 0 && now >= rescanAtTime)
            {
                rescanStage--;
                rescanAtTime = now + 2f;
                ScanScene();
            }

            UpdateHeroVignette();
            UpdateRenderScale(now);

            if (fpsParticleAmount < 100 && now >= nextParticleScanTime)
            {
                nextParticleScanTime = now + 1.5f;
                ScanParticlesActive();
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "FpsBoost.cs");
        }
    }
}
