using System;
using UnityEngine;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private static void ResetFpsBoostDefaults()
        {
            Modules.Performance.FpsBoost.ResetSettings();
        }

        private bool GetFpsBoostEnabled()
        {
            return GetFpsBoostModule()?.Enabled ?? false;
        }

        private void SetFpsBoostEnabled(bool value)
        {
            Module? module = GetFpsBoostModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            UpdateFpsBoostInteractivity();
            UpdateQuickMenuEntryStateColors();
        }

        private void SetFpsBoostOption(Modules.Performance.FpsBoost.Option option, bool value)
        {
            Modules.Performance.FpsBoost.SetOption(option, value);
            RefreshFpsBoostUi();
        }

        private void SetFpsBoostParticleAmount(int value)
        {
            Modules.Performance.FpsBoost.SetParticleAmount(value);
        }

        private void SetFpsBoostRenderScale(int value)
        {
            Modules.Performance.FpsBoost.SetRenderScale(value);
        }

        private void RefreshFpsBoostUi()
        {
            bool enabled = GetFpsBoostEnabled();
            UpdateToggleValue(fpsBoostToggleValue, enabled);
            UpdateToggleIcon(fpsBoostToggleIcon, enabled);
            UpdateToggleValue(fpsBoostBlurValue, Modules.Performance.FpsBoost.GetOption(Modules.Performance.FpsBoost.Option.Blur));
            UpdateToggleValue(fpsBoostFogValue, Modules.Performance.FpsBoost.GetOption(Modules.Performance.FpsBoost.Option.Fog));
            UpdateToggleValue(fpsBoostHazeValue, Modules.Performance.FpsBoost.GetOption(Modules.Performance.FpsBoost.Option.Haze));
            UpdateToggleValue(fpsBoostVignetteValue, Modules.Performance.FpsBoost.GetOption(Modules.Performance.FpsBoost.Option.Vignette));
            UpdateToggleValue(fpsBoostBloomValue, Modules.Performance.FpsBoost.GetOption(Modules.Performance.FpsBoost.Option.Bloom));
            UpdateToggleValue(fpsBoostColorCorrectionValue, Modules.Performance.FpsBoost.GetOption(Modules.Performance.FpsBoost.Option.ColorCorrection));
            UpdateToggleValue(fpsBoostBrightnessValue, Modules.Performance.FpsBoost.GetOption(Modules.Performance.FpsBoost.Option.Brightness));
            UpdateToggleValue(fpsBoostFilmGrainValue, Modules.Performance.FpsBoost.GetOption(Modules.Performance.FpsBoost.Option.FilmGrain));
            UpdateToggleValue(fpsBoostMenuCacheValue, Modules.Performance.FpsBoost.GetOption(Modules.Performance.FpsBoost.Option.MenuHotkeyCache));
            UpdateIntInputValue(fpsBoostParticleField, Modules.Performance.FpsBoost.GetParticleAmount());
            UpdateIntInputValue(fpsBoostRenderScaleField, Modules.Performance.FpsBoost.GetRenderScale());

            UpdateFpsBoostInteractivity();
        }
    }
}
