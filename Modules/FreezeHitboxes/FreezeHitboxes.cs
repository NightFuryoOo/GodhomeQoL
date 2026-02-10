namespace GodhomeQoL.Modules.Tools;

public sealed class FreezeHitboxes : Module
{
    public override bool DefaultEnabled => false;
    public override bool Hidden => true;

    private static FreezeHitboxesSettings Settings => GodhomeQoL.GlobalSettings.FreezeHitboxes ??= new FreezeHitboxesSettings();

    private static bool freezeActive;
    private static float storedTimeScale = 1f;
    private static float storedGenericTimeScale = 1f;
    private static float lastNonZeroTimeScale = 1f;
    private static float lastNonZeroGenericTimeScale = 1f;
    private static float unfreezeCooldownUntil;
    private static bool pendingHitFreeze;
    private static int pendingHitHealth;
    private static int pendingHitFrame;
    private static bool pendingHitLethal;
    private static FreezeHitboxesViewer? hitboxViewer;

    internal static bool GetEnabled() => Settings.Enabled;

    internal static void SetEnabled(bool value)
    {
        if (Settings.Enabled == value)
        {
            if (!value)
            {
                EndFreeze();
            }
            return;
        }

        Settings.Enabled = value;
        GodhomeQoL.SaveGlobalSettingsSafe();

        if (!value)
        {
            EndFreeze();
        }
    }

    private protected override void Load()
    {
        On.GameManager.SetTimeScale_float += OnGameManagerSetTimeScale;
        ModHooks.AfterTakeDamageHook += OnAfterTakeDamage;
        ModHooks.HeroUpdateHook += OnHeroUpdate;
        On.InputHandler.OnGUI += OnInputHandlerOnGUI;
        On.HeroController.Die += OnHeroDie;
    }

    private protected override void Unload()
    {
        On.GameManager.SetTimeScale_float -= OnGameManagerSetTimeScale;
        ModHooks.AfterTakeDamageHook -= OnAfterTakeDamage;
        ModHooks.HeroUpdateHook -= OnHeroUpdate;
        On.InputHandler.OnGUI -= OnInputHandlerOnGUI;
        On.HeroController.Die -= OnHeroDie;
        EndFreeze();
    }

    internal static bool GetAnyHitsMode() => Settings.AnyHits;

    internal static void SetAnyHitsMode(bool value)
    {
        Settings.AnyHits = value;
        GodhomeQoL.SaveGlobalSettingsSafe();
    }

    internal static string GetUnfreezeKeybind() => Settings.UnfreezeKeybind ?? string.Empty;

    internal static void SetUnfreezeKeybind(string value)
    {
        Settings.UnfreezeKeybind = value;
        GodhomeQoL.SaveGlobalSettingsSafe();
    }

    internal static KeyCode GetUnfreezeKey()
    {
        string raw = Settings.UnfreezeKeybind ?? string.Empty;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return KeyCode.None;
        }

        return Enum.TryParse(raw, true, out KeyCode key) ? key : KeyCode.None;
    }

    private static void OnGameManagerSetTimeScale(On.GameManager.orig_SetTimeScale_float orig, GameManager self, float newTimeScale)
    {
        if (!Settings.Enabled)
        {
            orig(self, newTimeScale);
            return;
        }

        if (freezeActive)
        {
            Time.timeScale = 0f;
            TimeController.GenericTimeScale = 0f;
            return;
        }

        if (newTimeScale > 0f)
        {
            lastNonZeroTimeScale = newTimeScale;
        }

        if (TimeController.GenericTimeScale > 0f)
        {
            lastNonZeroGenericTimeScale = TimeController.GenericTimeScale;
        }

        orig(self, newTimeScale);
    }

    private static int OnAfterTakeDamage(int hazardType, int damageAmount)
    {
        if (!Settings.Enabled)
        {
            return damageAmount;
        }

        if (damageAmount <= 0 || freezeActive)
        {
            return damageAmount;
        }

        if (Settings.AnyHits && IsDeathOrRespawnState(ignoreControlReqlinquished: true))
        {
            return damageAmount;
        }

        if (Settings.AnyHits && Time.unscaledTime < unfreezeCooldownUntil)
        {
            return damageAmount;
        }

        if (Settings.AnyHits)
        {
            pendingHitFreeze = true;
            PlayerData? pd = PlayerData.instance;
            if (pd != null)
            {
                pendingHitHealth = pd.health + Math.Max(0, damageAmount);
            }
            else
            {
                pendingHitHealth = damageAmount;
            }
            pendingHitLethal = damageAmount > 0 && PlayerData.instance != null
                && PlayerData.instance.health - damageAmount <= 0;
            pendingHitFrame = Time.frameCount;
            return damageAmount;
        }

        if (IsPlayerDead())
        {
            StartFreeze();
        }

        return damageAmount;
    }

    private static void OnInputHandlerOnGUI(On.InputHandler.orig_OnGUI orig, InputHandler self)
    {
        orig(self);
        if (!Settings.Enabled)
        {
            return;
        }
        if (!freezeActive)
        {
            return;
        }

        KeyCode key = GetUnfreezeKey();
        if (key != KeyCode.None && Input.GetKeyDown(key))
        {
            EndFreeze();
        }
    }

    private static void OnHeroUpdate()
    {
        if (!Settings.Enabled)
        {
            return;
        }

        if (!pendingHitFreeze || freezeActive || !Settings.AnyHits)
        {
            return;
        }

        if (Time.frameCount == pendingHitFrame)
        {
            return;
        }

        pendingHitFreeze = false;
        if (Time.unscaledTime < unfreezeCooldownUntil)
        {
            pendingHitLethal = false;
            return;
        }

        if (!pendingHitLethal && IsDeathOrRespawnState(ignoreControlReqlinquished: true))
        {
            pendingHitLethal = false;
            return;
        }

        PlayerData? pd = PlayerData.instance;
        if (pd != null && (pendingHitLethal || pd.health < pendingHitHealth))
        {
            StartFreeze();
        }

        pendingHitLethal = false;
    }

    private static bool IsPlayerDead()
    {
        if (HeroController.instance != null && HeroController.instance.cState.dead)
        {
            return true;
        }

        return PlayerData.instance != null && PlayerData.instance.health <= 0;
    }

    private static bool IsHeroInvulnerable()
    {
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
            catch
            {
                // ignore missing field
            }
        }

        PlayerData pd = PlayerData.instance;
        if (pd != null && pd.isInvincible)
        {
            return true;
        }

        return PlayerDataR.isInvincible;
    }


    private static bool IsDeathOrRespawnState(bool ignoreControlReqlinquished = false)
    {
        GameManager? gm = GameManager.instance;
        if (gm == null || gm.gameState != GameState.PLAYING || gm.IsInSceneTransition)
        {
            return true;
        }

        HeroController? hero = HeroController.instance;
        if (hero == null || hero.cState.dead || (!ignoreControlReqlinquished && hero.controlReqlinquished))
        {
            return true;
        }

        return PlayerData.instance != null && PlayerData.instance.health <= 0;
    }

    private static IEnumerator OnHeroDie(On.HeroController.orig_Die orig, HeroController self)
    {
        IEnumerator origEnum = orig(self);
        if (Settings.Enabled && !freezeActive && !Settings.AnyHits)
        {
            _ = self.StartCoroutine(FreezeNextFrame());
        }

        while (origEnum.MoveNext())
        {
            yield return origEnum.Current;
        }
    }

    private static IEnumerator FreezeNextFrame()
    {
        yield return null;
        if (!freezeActive && !Settings.AnyHits)
        {
            StartFreeze();
        }
    }

    private static void StartFreeze()
    {
        if (freezeActive)
        {
            return;
        }

        freezeActive = true;
        storedTimeScale = Time.timeScale > 0f ? Time.timeScale : lastNonZeroTimeScale;
        storedGenericTimeScale = TimeController.GenericTimeScale > 0f
            ? TimeController.GenericTimeScale
            : lastNonZeroGenericTimeScale;

        Time.timeScale = 0f;
        TimeController.GenericTimeScale = 0f;

        EnsureViewer();
        hitboxViewer?.Load();
    }

    private static void EndFreeze()
    {
        if (!freezeActive)
        {
            return;
        }

        freezeActive = false;
        if (storedTimeScale <= 0f)
        {
            storedTimeScale = lastNonZeroTimeScale > 0f ? lastNonZeroTimeScale : 1f;
        }

        if (storedGenericTimeScale <= 0f)
        {
            storedGenericTimeScale = lastNonZeroGenericTimeScale > 0f ? lastNonZeroGenericTimeScale : 1f;
        }

        TimeController.GenericTimeScale = storedGenericTimeScale;
        Time.timeScale = storedTimeScale;
        unfreezeCooldownUntil = Time.unscaledTime + 1f;

        hitboxViewer?.Unload();
    }

    private static void EnsureViewer()
    {
        hitboxViewer ??= new FreezeHitboxesViewer();
    }
}
