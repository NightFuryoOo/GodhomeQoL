using BloomOptimized = UnityStandardAssets.ImageEffects.BloomOptimized;
using ColorCorrectionCurves = UnityStandardAssets.ImageEffects.ColorCorrectionCurves;
using FastNoise = UnityStandardAssets.ImageEffects.FastNoise;

namespace GodhomeQoL.Modules.Performance;

public sealed partial class FpsBoost : Module
{
    private static bool bloomTouched;
    private static bool bloomIntended;
    private static bool colorTouched;
    private static bool colorIntended;
    private static bool grainTouched;
    private static bool grainIntended;
    private static bool brightnessTouched;
    private static bool brightnessIntended;

    private static void OnApplyEffectConfiguration(On.CameraController.orig_ApplyEffectConfiguration orig, CameraController self, bool fastNoise, bool bloom)
    {
        orig(self, fastNoise, bloom);

        if (!moduleActive)
        {
            return;
        }

        try
        {
            EnforcePostEffects(self, true);
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "FpsBoost.PostEffects.cs");
        }
    }

    private static void ApplyPostEffectsNow()
    {
        CameraController? controller = UObject.FindObjectOfType<CameraController>();
        if (controller != null)
        {
            EnforcePostEffects(controller, false);
        }
    }

    private static void EnforcePostEffects(CameraController controller, bool fromGameConfiguration)
    {
        BloomOptimized? bloom = controller.GetComponent<BloomOptimized>();
        if (bloom != null)
        {
            EnforceEffect(bloom, fpsNoBloom, fromGameConfiguration, ref bloomTouched, ref bloomIntended);
        }

        ColorCorrectionCurves? color = controller.GetComponent<ColorCorrectionCurves>();
        if (color != null)
        {
            EnforceEffect(color, fpsNoColorCorrection, fromGameConfiguration, ref colorTouched, ref colorIntended);
        }

        FastNoise? grain = controller.GetComponent<FastNoise>();
        if (grain != null)
        {
            EnforceEffect(grain, fpsNoFilmGrain, fromGameConfiguration, ref grainTouched, ref grainIntended);
        }

        BrightnessEffect? brightness = controller.GetComponent<BrightnessEffect>();
        if (brightness != null)
        {
            EnforceBrightness(brightness, fromGameConfiguration);
        }
    }

    private static void EnforceEffect(Behaviour effect, bool disable, bool fromGameConfiguration, ref bool touched, ref bool intended)
    {
        if (disable)
        {
            if (fromGameConfiguration || !touched)
            {
                intended = effect.enabled;
            }

            effect.enabled = false;
            touched = true;
        }
        else if (touched)
        {
            if (!fromGameConfiguration)
            {
                effect.enabled = intended;
            }

            touched = false;
        }
    }

    private static void RestoreEffect(Behaviour? effect, ref bool touched, bool intended)
    {
        if (effect != null && touched)
        {
            effect.enabled = intended;
        }

        touched = false;
    }

    private static bool IsBrightnessNeutral(BrightnessEffect effect)
    {
        return Mathf.Approximately(effect._Brightness, 1f) && Mathf.Approximately(effect._Contrast, 1f);
    }

    private static bool IsCameraTextureDisplayInUse()
    {
        GameCameraTextureDisplay? display = GameCameraTextureDisplay.Instance;
        return display != null && display.gameObject.activeInHierarchy;
    }

    private static void EnforceBrightness(BrightnessEffect effect, bool fromGameConfiguration)
    {
        bool skip = fpsSkipNeutralBrightness && IsBrightnessNeutral(effect) && !IsCameraTextureDisplayInUse();
        EnforceEffect(effect, skip, fromGameConfiguration, ref brightnessTouched, ref brightnessIntended);
    }

    private static void OnBrightnessChanged(On.BrightnessEffect.orig_SetBrightness orig, BrightnessEffect self, float value)
    {
        orig(self, value);
        ReevaluateBrightness(self);
    }

    private static void OnContrastChanged(On.BrightnessEffect.orig_SetContrast orig, BrightnessEffect self, float value)
    {
        orig(self, value);
        ReevaluateBrightness(self);
    }

    private static void ReevaluateBrightness(BrightnessEffect effect)
    {
        if (!moduleActive || effect == null || (!fpsSkipNeutralBrightness && !brightnessTouched))
        {
            return;
        }

        try
        {
            EnforceBrightness(effect, false);
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "FpsBoost.PostEffects.cs");
        }
    }

    private static void RestorePostEffects()
    {
        if (!bloomTouched && !colorTouched && !grainTouched && !brightnessTouched)
        {
            return;
        }

        CameraController? controller = UObject.FindObjectOfType<CameraController>();
        RestoreEffect(controller != null ? controller.GetComponent<BloomOptimized>() : null, ref bloomTouched, bloomIntended);
        RestoreEffect(controller != null ? controller.GetComponent<ColorCorrectionCurves>() : null, ref colorTouched, colorIntended);
        RestoreEffect(controller != null ? controller.GetComponent<FastNoise>() : null, ref grainTouched, grainIntended);
        RestoreEffect(controller != null ? controller.GetComponent<BrightnessEffect>() : null, ref brightnessTouched, brightnessIntended);
    }
}
