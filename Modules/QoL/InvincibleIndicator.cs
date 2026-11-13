using GodhomeQoL.Modules.Tools;

namespace GodhomeQoL.Modules.QoL;

public sealed class InvincibleIndicator : Module
{
    [GlobalSetting]
    public static float InvincibleTextScale = 1f;

    public override bool DefaultEnabled => true;

    private const float InvincibleThresholdSeconds = 10f;
    private const string IndicatorText = "Invincible ON";
    private const int IndicatorSortOrder = HudLabel.BelowReplayLoggerSortOrder;
    private const int IndicatorFontSize = 34;

    private float invincibleTimer;
    private bool indicatorVisible;
    private InvincibleIndicatorDisplay? display;
    private static InvincibleIndicatorDisplay? previewDisplay;

    private protected override void Load()
    {
        display ??= previewDisplay ?? new InvincibleIndicatorDisplay(IndicatorSortOrder);
        previewDisplay = display;
        invincibleTimer = 0f;
        indicatorVisible = false;
        display.Hide();
        ModHooks.HeroUpdateHook += OnHeroUpdate;
    }

    private protected override void Unload()
    {
        ModHooks.HeroUpdateHook -= OnHeroUpdate;
        invincibleTimer = 0f;
        indicatorVisible = false;

        if (display != null)
        {
            display.Destroy();
            if (ReferenceEquals(previewDisplay, display))
            {
                previewDisplay = null;
            }

            display = null;
        }
    }

    private void OnHeroUpdate()
    {
        if (!ShouldCheckHero())
        {
            ResetIndicator();
            return;
        }

        if (IsHeroInvincible())
        {
            invincibleTimer += Time.unscaledDeltaTime;
            if (!indicatorVisible && invincibleTimer >= InvincibleThresholdSeconds)
            {
                display?.Show(IndicatorText);
                indicatorVisible = true;
            }

            return;
        }

        ResetIndicator();
    }

    private void ResetIndicator()
    {
        invincibleTimer = 0f;
        if (indicatorVisible)
        {
            indicatorVisible = false;
            display?.Hide();
        }
    }

    private static bool ShouldCheckHero()
    {
        GameManager? manager = GameManager.instance;
        if (manager == null || manager.gameState != GameState.PLAYING)
        {
            return false;
        }

        return HeroController.instance != null;
    }

    private static bool IsHeroInvincible()
    {
        if (InvincibilityClaims.HasActiveClaims)
        {
            return true;
        }

        HeroController? hero = HeroController.instance;
        if (hero != null)
        {
            try
            {
                if (hero.cState.invulnerable)
                {
                    return true;
                }
            }
            catch (Exception swallowed)
            {
                LogSuppressed(swallowed, "InvincibleIndicator.cs");
            }
        }

        if (!IsHeroBoxActive())
        {
            return true;
        }

        PlayerData pd = PlayerData.instance;
        if (pd != null && pd.isInvincible)
        {
            return true;
        }

        return PlayerDataR.isInvincible;
    }

    private static bool IsHeroBoxActive()
    {
        HeroController? hero = HeroController.instance;
        if (hero == null)
        {
            return true;
        }

        Transform heroBoxTransform = hero.transform.Find("HeroBox");
        if (heroBoxTransform == null)
        {
            return true;
        }

        return heroBoxTransform.gameObject.activeInHierarchy;
    }

    internal static void PreviewText()
    {
        previewDisplay ??= new InvincibleIndicatorDisplay(IndicatorSortOrder);
        previewDisplay.Preview(IndicatorText);
    }

    private sealed class InvincibleIndicatorDisplay
    {
        private readonly HudLabel label;

        public InvincibleIndicatorDisplay(int sortOrder)
        {
            label = new HudLabel("InvincibleIndicatorCanvas", Vector2.zero, new Vector2(256f, 910f), IndicatorFontSize, () => HudTextScales.InvincibleText, sortOrder, highlight: true);
        }

        public void Show(string message) => label.Show(message);

        public void Hide() => label.Hide();

        public void Preview(string message) => label.Preview(message);

        public void Destroy() => label.Destroy();
    }
}
