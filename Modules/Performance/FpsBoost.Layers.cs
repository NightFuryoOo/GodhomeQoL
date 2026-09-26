namespace GodhomeQoL.Modules.Performance;

public sealed partial class FpsBoost : Module
{
    private static readonly List<GameObject> hiddenBlur = new();
    private static readonly List<GameObject> hiddenFog = new();
    private static readonly List<GameObject> hiddenHaze = new();
    private static readonly List<GameObject> hiddenVignette = new();

    private static bool heroVignetteHidden;
    private static bool blurCameraSuppressed;
    private static BlurManager? suppressedBlurManager;

    private static List<GameObject> GetHiddenList(Option option)
    {
        return option switch
        {
            Option.Blur => hiddenBlur,
            Option.Fog => hiddenFog,
            Option.Haze => hiddenHaze,
            _ => hiddenVignette
        };
    }

    private static bool IsBlurName(string lower)
    {
        return lower.Contains("blurplane") && lower != "no blur";
    }

    private static bool IsFogName(string lower)
    {
        if (lower == "no fog")
        {
            return false;
        }

        return (lower.Contains("fog") && !lower.Contains("fog_canyon")) || lower.Contains("fog_canyon_fog");
    }

    private static readonly HashSet<string> HazeExcludedScenes = new(StringComparer.Ordinal)
    {
        "GG_Radiance"
    };

    private static bool IsHazeName(string lower)
    {
        return lower.Contains("haze") && lower != "no haze";
    }

    private static bool IsSceneVignetteName(string lower)
    {
        return lower.Contains("vignette")
            && !lower.Contains("low health")
            && !lower.Contains("death")
            && !lower.Contains("shade hit")
            && !lower.Contains("dialogue");
    }

    private static void HideObject(GameObject go, List<GameObject> hidden)
    {
        if (go != null && go.activeSelf)
        {
            go.SetActive(false);
            hidden.Add(go);
        }
    }

    private static void ScanScene()
    {
        if (!fpsNoBlur && !fpsNoFog && !fpsNoHaze && !fpsNoVignette)
        {
            return;
        }

        HeroController? hero = HeroController.SilentInstance;
        GameObject? heroVignette = hero != null && hero.vignette != null ? hero.vignette.gameObject : null;
        bool normalDarkness = IsNormalDarkness();

        GameObject[] all = UObject.FindObjectsOfType<GameObject>();
        for (int i = 0; i < all.Length; i++)
        {
            GameObject go = all[i];
            if (go == null)
            {
                continue;
            }

            string lower = go.name.ToLowerInvariant();
            List<GameObject>? target = null;
            if (fpsNoBlur && IsBlurName(lower))
            {
                target = hiddenBlur;
            }
            else if (fpsNoFog && IsFogName(lower))
            {
                target = hiddenFog;
            }
            else if (fpsNoHaze && IsHazeName(lower))
            {
                target = hiddenHaze;
            }
            else if (fpsNoVignette && normalDarkness && IsSceneVignetteName(lower))
            {
                target = hiddenVignette;
            }

            if (target == null || IsUiObject(go))
            {
                continue;
            }

            if (target == hiddenVignette && (go == heroVignette || (hero != null && go.transform.IsChildOf(hero.transform))))
            {
                continue;
            }

            if (target == hiddenHaze && HazeExcludedScenes.Contains(go.scene.name))
            {
                continue;
            }

            HideObject(go, target);
        }
    }

    private static bool IsUiObject(GameObject go)
    {
        return go.GetComponent<RectTransform>() != null || go.GetComponentInParent<Canvas>() != null;
    }

    private static void RestoreLayer(Option option)
    {
        List<GameObject> hidden = GetHiddenList(option);
        for (int i = 0; i < hidden.Count; i++)
        {
            GameObject go = hidden[i];
            if (go != null && !go.activeSelf)
            {
                go.SetActive(true);
            }
        }

        hidden.Clear();

        if (option == Option.Vignette)
        {
            RestoreHeroVignette();
        }
        else if (option == Option.Blur)
        {
            RestoreBlurCamera();
        }
    }

    private static void RestoreLayers()
    {
        RestoreLayer(Option.Blur);
        RestoreLayer(Option.Fog);
        RestoreLayer(Option.Haze);
        RestoreLayer(Option.Vignette);
    }

    private static void PruneLayers()
    {
        hiddenBlur.RemoveAll(go => go == null);
        hiddenFog.RemoveAll(go => go == null);
        hiddenHaze.RemoveAll(go => go == null);
        hiddenVignette.RemoveAll(go => go == null);
    }

    private static void OnBlurPlaneEnable(On.BlurPlane.orig_OnEnable orig, BlurPlane self)
    {
        orig(self);

        if (!moduleActive || !fpsNoBlur || self == null)
        {
            return;
        }

        try
        {
            HideObject(self.gameObject, hiddenBlur);
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "FpsBoost.Layers.cs");
        }
    }

    private static void OnBlurManagerUpdate(On.BlurManager.orig_Update orig, BlurManager self)
    {
        orig(self);

        if (!moduleActive || !fpsNoBlur || self == null)
        {
            return;
        }

        try
        {
            LightBlurredBackground? background = self.GetComponent<LightBlurredBackground>();
            if (background != null && background.enabled)
            {
                background.enabled = false;
                blurCameraSuppressed = true;
                suppressedBlurManager = self;
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "FpsBoost.Layers.cs");
        }
    }

    private static void RestoreBlurCamera()
    {
        if (!blurCameraSuppressed)
        {
            return;
        }

        blurCameraSuppressed = false;
        BlurManager? manager = suppressedBlurManager != null ? suppressedBlurManager : UObject.FindObjectOfType<BlurManager>();
        suppressedBlurManager = null;
        if (manager == null)
        {
            return;
        }

        try
        {
            ReflectionHelper.SetField(manager, "appliedShaderQuality", (ShaderQualities)(-1));
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "FpsBoost.Layers.cs");
        }
    }

    private static bool IsNormalDarkness()
    {
        GameManager? manager = GameManager.instance;
        return manager == null || manager.sm == null || manager.sm.darknessLevel == 0;
    }

    private static void UpdateHeroVignette()
    {
        if (!fpsNoVignette && !heroVignetteHidden)
        {
            return;
        }

        HeroController? hero = HeroController.SilentInstance;
        SpriteRenderer? vignette = hero != null ? hero.vignette : null;
        if (vignette == null)
        {
            return;
        }

        if (fpsNoVignette && IsNormalDarkness())
        {
            if (vignette.enabled)
            {
                vignette.enabled = false;
            }

            heroVignetteHidden = true;
        }
        else if (heroVignetteHidden)
        {
            vignette.enabled = true;
            heroVignetteHidden = false;
        }
    }

    private static void RestoreHeroVignette()
    {
        if (!heroVignetteHidden)
        {
            return;
        }

        HeroController? hero = HeroController.SilentInstance;
        SpriteRenderer? vignette = hero != null ? hero.vignette : null;
        if (vignette != null)
        {
            vignette.enabled = true;
        }

        heroVignetteHidden = false;
    }
}
