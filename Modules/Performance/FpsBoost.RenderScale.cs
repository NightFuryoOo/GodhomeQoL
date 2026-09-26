namespace GodhomeQoL.Modules.Performance;

public sealed partial class FpsBoost : Module
{
    internal const int MinRenderScale = 25;

    private static int renderScalePercent = 100;
    private static bool renderScalePending;
    private static float renderScaleApplyAt;
    private static bool renderScaleApplied;
    private static int renderScaleBaseWidth;
    private static int renderScaleBaseHeight;
    private static int renderScaleAppliedWidth;
    private static int renderScaleAppliedHeight;
    private static float renderScaleAppliedAt;

    internal static int GetRenderScale() => renderScalePercent;

    internal static void SetRenderScale(int value)
    {
        renderScalePercent = Mathf.Clamp(value, MinRenderScale, 100);
        if (moduleActive)
        {
            renderScalePending = true;
            renderScaleApplyAt = Time.unscaledTime + 0.6f;
        }
    }

    private static int MakeEven(int value) => Mathf.Max(2, (value + 1) & ~1);

    private static void ApplyRenderScale()
    {
        renderScalePending = false;

        try
        {
            if (renderScalePercent >= 100)
            {
                RestoreRenderScale();
                return;
            }

            if (Screen.fullScreenMode != FullScreenMode.FullScreenWindow)
            {
                return;
            }

            if (!renderScaleApplied)
            {
                renderScaleBaseWidth = Screen.width;
                renderScaleBaseHeight = Screen.height;
            }

            if (renderScaleBaseWidth <= 0 || renderScaleBaseHeight <= 0)
            {
                return;
            }

            int width = MakeEven(Mathf.RoundToInt(renderScaleBaseWidth * renderScalePercent / 100f));
            int height = MakeEven(Mathf.RoundToInt(renderScaleBaseHeight * renderScalePercent / 100f));

            Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow, Screen.currentResolution.refreshRate);
            renderScaleAppliedWidth = width;
            renderScaleAppliedHeight = height;
            renderScaleApplied = true;
            renderScaleAppliedAt = Time.unscaledTime;
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "FpsBoost.RenderScale.cs");
        }
    }

    private static void RestoreRenderScale()
    {
        renderScalePending = false;
        if (!renderScaleApplied)
        {
            return;
        }

        renderScaleApplied = false;
        try
        {
            if (renderScaleBaseWidth > 0 && renderScaleBaseHeight > 0 && Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
            {
                Screen.SetResolution(renderScaleBaseWidth, renderScaleBaseHeight, FullScreenMode.FullScreenWindow, Screen.currentResolution.refreshRate);
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "FpsBoost.RenderScale.cs");
        }
    }

    private static void UpdateRenderScale(float now)
    {
        if (renderScalePending && now >= renderScaleApplyAt)
        {
            ApplyRenderScale();
        }

        if (renderScaleApplied
            && now >= renderScaleAppliedAt + 3f
            && (Screen.width != renderScaleAppliedWidth
                || Screen.height != renderScaleAppliedHeight
                || Screen.fullScreenMode != FullScreenMode.FullScreenWindow))
        {
            renderScaleApplied = false;
            renderScalePercent = 100;
        }
    }
}
