namespace GodhomeQoL.Modules.Tools;

public sealed partial class GearSwitcher : Module
{
    internal const string TemporaryPresetName = "GodhomeQoL Custom Pantheon Gear";

    private static readonly string[] SnapshotIntFields =
    {
        "maxHealth", "maxHealthBase", "health", "prevHealth", "MPReserveMax", "MPReserve", "charmSlots", "nailDamage",
        "fireballLevel", "quakeLevel", "screamLevel", "royalCharmState", "grimmChildLevel"
    };

    private static readonly string[] SnapshotBoolFields =
    {
        "hasAcidArmour", "hasDash", "canDash", "hasWalljump", "hasSuperDash", "canSuperDash", "hasShadowDash", "canShadowDash",
        "hasDoubleJump", "hasNailArt", "hasAllNailArts", "hasCyclone", "hasDashSlash", "hasUpwardSlash", "hasDreamNail",
        "hasDreamGate", "dreamNailUpgraded", "overcharmed", "brokenCharm_23", "brokenCharm_24", "brokenCharm_25",
        "fragileHealth_unbreakable", "fragileGreed_unbreakable", "fragileStrength_unbreakable"
    };

    private sealed class PlayerGearSnapshot
    {
        internal readonly Dictionary<string, int> Ints = new(StringComparer.Ordinal);
        internal readonly Dictionary<string, bool> Bools = new(StringComparer.Ordinal);
        internal List<int> EquippedCharms = new();
    }

    internal static GearPreset? EditOnlyPreset { get; set; }

    internal static bool PreserveHealthOnApply { get; set; }

    internal static bool HasPendingApply => pendingApply;

    internal static void SetChainHealth(int value)
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        int target = Math.Max(1, Math.Min(pd.CurrentMaxHealth, value));
        if (pd.health == target)
        {
            return;
        }

        pd.health = target;
        pd.prevHealth = target;
        PlayMakerFSM.BroadcastEvent(CharmIndicatorCheckEvent);
        ScheduleHealthHudRefresh();
    }

    internal static void FillChainHealth()
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null || pd.health >= pd.CurrentMaxHealth)
        {
            return;
        }

        pd.MaxHealth();
        pd.prevHealth = pd.health;
        ScheduleHealthHudRefresh();
    }

    internal static void SetChainLifeblood(int value)
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        int target = Math.Max(0, value);
        if (pd.healthBlue == target)
        {
            return;
        }

        PlayMakerFSM? blue = Ref.GC?.hudCanvas != null ? Ref.GC.hudCanvas.Child("Health")?.LocateMyFSM("Blue Health Control") : null;
        if (blue != null && blue.ActiveStateName == "Wait")
        {
            blue.SendEvent("LAST HP ADDED");
        }

        pd.healthBlue = target;
        EventRegister.SendEvent(UpdateBlueHealthEvent);
        PlayMakerFSM.BroadcastEvent(UpdateBlueHealthEvent);
        ScheduleHealthHudRefresh();
    }

    internal static void SetChainSoul(int value)
    {
        PlayerData? pd = PlayerData.instance;
        HeroController? hero = HeroController.instance;
        if (pd == null || hero == null)
        {
            return;
        }

        int target = Math.Max(0, value);
        if (pd.MPCharge + pd.MPReserve == target)
        {
            return;
        }

        int token = ++chainSoulToken;
        _ = GlobalCoroutineExecutor.Start(SetChainSoulRoutine(target, token));
    }

    private static int chainSoulToken;

    private static readonly HashSet<string> BoundVesselStates = new(StringComparer.Ordinal) { "Bound", "Obtained", "Not obtained", "Not Obtained", "Setup" };

    internal static string DescribeSoul()
    {
        PlayerData? pd = PlayerData.instance;
        return pd == null ? "no PlayerData" : $"charge {pd.MPCharge}/{pd.maxMP}, reserve {pd.MPReserve}/{pd.MPReserveMax}";
    }

    internal static void RefreshSoulVesselsHud()
    {
        try
        {
            PlayerData? pd = PlayerData.instance;
            bool bound = global::ToggleableBindings.VanillaBindings.SoulBinding.IsSoulBound();
            System.Text.StringBuilder states = new();
            for (int vessel = 1; vessel <= 4; vessel++)
            {
                Transform? child = Ref.GC?.hudCanvas?.transform.Find($"Soul Orb/Vessels/Vessel {vessel}");
                PlayMakerFSM? orb = child != null ? child.gameObject.LocateMyFSM("vessel_orb") : null;
                if (child == null || orb == null)
                {
                    continue;
                }

                states.Append($" {vessel}:{orb.ActiveStateName}");
                if (bound || pd == null || !child.gameObject.activeInHierarchy)
                {
                    continue;
                }

                if (BoundVesselStates.Contains(orb.ActiveStateName) || pd.MPReserveMax < 33 * vessel)
                {
                    orb.SetState("Init");
                }
                else
                {
                    orb.SetState("Up Check");
                }
            }

            LogDebug($"[CustomPantheon] Soul HUD refresh: {DescribeSoul()}, soul bound {bound}, vessels{states}");
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "GearSwitcher.CustomPantheon.cs/Vessels");
        }
    }

    private static IEnumerator SetChainSoulRoutine(int target, int token)
    {
        HeroController? hero = HeroController.instance;
        if (hero == null)
        {
            yield break;
        }

        hero.ClearMPSendEvents();
        yield return new WaitForSecondsRealtime(0.35f);
        hero = HeroController.instance;
        if (token != chainSoulToken || hero == null || target <= 0)
        {
            yield break;
        }

        hero.AddMPCharge(target);
        LogDebug($"[CustomPantheon] Soul set to {target}: {DescribeSoul()}");
        yield return new WaitForSecondsRealtime(0.35f);
        PlayerData? pd = PlayerData.instance;
        GameManager? manager = GameManager.instance;
        if (token == chainSoulToken && pd != null && pd.MPReserve > 0 && manager != null && manager.soulVessel_fsm != null)
        {
            manager.soulVessel_fsm.SendEvent("MP RESERVE UP");
        }

        float deadline = Time.realtimeSinceStartup + SoulOrbRedrawTimeoutSeconds;
        while (token == chainSoulToken && Time.realtimeSinceStartup < deadline && !IsSoulOrbIdle())
        {
            yield return null;
        }

        if (token == chainSoulToken)
        {
            RedrawSoulOrb();
        }
    }

    private const float SoulOrbRedrawTimeoutSeconds = 2f;
    private const string SoulOrbIdleState = "Idle";

    private static bool IsSoulOrbIdle()
    {
        PlayMakerFSM? orb = GameManager.instance != null ? GameManager.instance.soulOrb_fsm : null;
        return orb != null && string.Equals(orb.ActiveStateName, SoulOrbIdleState, StringComparison.Ordinal);
    }

    private static void RedrawSoulOrb()
    {
        try
        {
            PlayMakerFSM? orb = GameManager.instance != null ? GameManager.instance.soulOrb_fsm : null;
            string state = orb != null ? orb.ActiveStateName : "none";
            if (orb != null && string.Equals(state, SoulOrbIdleState, StringComparison.Ordinal))
            {
                orb.SendEvent(MPSetEvent);
            }

            LogDebug($"[CustomPantheon] Soul orb redraw from '{state}': {DescribeSoul()}");
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "GearSwitcher.CustomPantheon.cs/Orb");
        }
    }

    private static bool temporaryGearActive;
    private static bool temporaryGearEnabledBefore;
    private static string temporaryGearPreviousPreset = string.Empty;
    private static List<int>? temporaryGearPreviousCharms;
    private static List<int>? temporaryGearPreviousLocalCharms;
    private static PlayerGearSnapshot? temporaryGearSnapshot;
    private static GearPreset? temporaryGearApplied;

    internal static bool IsTemporaryGearActive => temporaryGearActive;

    private static bool IsEditOnly(GearPreset? preset) => preset != null && ReferenceEquals(preset, EditOnlyPreset);

    internal static GearPreset ClonePreset(GearPreset source)
    {
        GearPreset? copy = JsonConvert.DeserializeObject<GearPreset>(JsonConvert.SerializeObject(source));
        return copy ?? new GearPreset();
    }

    internal static void CleanupTemporaryPreset()
    {
        GearSwitcherSettings settings = Settings;
        settings.Presets?.Remove(TemporaryPresetName);
        settings.PresetOrder?.RemoveAll(name => string.Equals(name, TemporaryPresetName, StringComparison.Ordinal));
        if (string.Equals(settings.LastPreset, TemporaryPresetName, StringComparison.Ordinal))
        {
            settings.LastPreset = "FullGear";
        }
    }

    internal static void ApplyTemporaryGear(GearPreset gear)
    {
        if (gear == null || ReferenceEquals(gear, temporaryGearApplied))
        {
            return;
        }

        try
        {
            EnsurePresetDefaults();
            if (!temporaryGearActive)
            {
                temporaryGearActive = true;
                temporaryGearEnabledBefore = IsGloballyEnabled;
                temporaryGearPreviousPreset = NormalizeBuiltinPresetName(GetLastPresetName());
                temporaryGearPreviousCharms = TryGetPreset(temporaryGearPreviousPreset, out GearPreset previous) && previous?.EquippedCharms != null
                    ? previous.EquippedCharms.ToList()
                    : null;
                temporaryGearPreviousLocalCharms = GodhomeQoL.LocalSettings.GearSwitcherTemporaryCharms?.ToList();
                temporaryGearSnapshot = temporaryGearEnabledBefore ? null : CaptureSnapshot();
                if (!IsGloballyEnabled)
                {
                    PrepareForEnable();
                    IsGloballyEnabled = true;
                }
            }

            GearPreset copy = ClonePreset(gear);
            copy.Name = TemporaryPresetName;
            Settings.Presets[TemporaryPresetName] = copy;
            temporaryGearApplied = gear;
            ApplyPreset(TemporaryPresetName, allowQueue: true);
        }
        catch (Exception ex)
        {
            LogError($"[GearSwitcher] Custom Pantheon gear failed: {ex}");
        }
    }

    internal static void EndTemporaryGear()
    {
        if (!temporaryGearActive)
        {
            return;
        }

        temporaryGearActive = false;
        temporaryGearApplied = null;
        try
        {
            string previous = string.IsNullOrEmpty(temporaryGearPreviousPreset) ? "FullGear" : temporaryGearPreviousPreset;
            if (IsBuiltinPresetName(previous) && TryGetPreset(previous, out GearPreset preset) && preset != null)
            {
                preset.EquippedCharms = temporaryGearPreviousCharms?.ToList();
            }

            GodhomeQoL.LocalSettings.GearSwitcherTemporaryCharms = temporaryGearPreviousLocalCharms?.ToList();
            if (temporaryGearEnabledBefore)
            {
                ApplyPreset(previous, allowQueue: true);
            }
            else
            {
                Settings.LastPreset = previous;
                DisableAndRestoreState();
                PlayerGearSnapshot? snapshot = temporaryGearSnapshot;
                if (snapshot != null)
                {
                    _ = GlobalCoroutineExecutor.Start(RestoreSnapshotWhenSafe(snapshot));
                }
            }

            CleanupTemporaryPreset();
            GodhomeQoL.SaveGlobalSettingsSafe();
        }
        catch (Exception ex)
        {
            LogError($"[GearSwitcher] Restoring gear after Custom Pantheon failed: {ex}");
        }
        finally
        {
            temporaryGearSnapshot = null;
            temporaryGearPreviousCharms = null;
            temporaryGearPreviousLocalCharms = null;
            temporaryGearPreviousPreset = string.Empty;
        }
    }

    private static PlayerGearSnapshot? CaptureSnapshot()
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return null;
        }

        PlayerGearSnapshot snapshot = new();
        foreach (string name in SnapshotIntFields)
        {
            snapshot.Ints[name] = pd.GetIntInternal(name);
        }

        for (int id = FirstCharmId; id <= LastCharmId; id++)
        {
            snapshot.Ints[$"charmCost_{id}"] = pd.GetIntInternal($"charmCost_{id}");
            snapshot.Bools[$"gotCharm_{id}"] = pd.GetBoolInternal($"gotCharm_{id}");
            snapshot.Bools[$"equippedCharm_{id}"] = pd.GetBoolInternal($"equippedCharm_{id}");
        }

        foreach (string name in SnapshotBoolFields)
        {
            snapshot.Bools[name] = pd.GetBoolInternal(name);
        }

        snapshot.EquippedCharms = GetEquippedCharms().ToList();
        return snapshot;
    }

    private static IEnumerator RestoreSnapshotWhenSafe(PlayerGearSnapshot snapshot)
    {
        float deadline = Time.realtimeSinceStartup + 15f;
        while (Time.realtimeSinceStartup < deadline
            && (GameManager.instance == null || GameManager.instance.IsInSceneTransition || HeroController.instance == null))
        {
            yield return null;
        }

        yield return null;
        RestoreSnapshot(snapshot);
    }

    private static void RestoreSnapshot(PlayerGearSnapshot snapshot)
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        try
        {
            foreach (KeyValuePair<string, int> pair in snapshot.Ints)
            {
                pd.SetIntInternal(pair.Key, pair.Value);
            }

            foreach (KeyValuePair<string, bool> pair in snapshot.Bools)
            {
                pd.SetBoolInternal(pair.Key, pair.Value);
            }

            pd.equippedCharms = snapshot.EquippedCharms.ToList();
            pd.CalculateNotchesUsed();
            HeroController.instance?.CharmUpdate();
            if (pd.GetBoolInternal($"equippedCharm_{GrimmchildCharmId}"))
            {
                RefreshGrimmchildCharm(pd);
            }
            else
            {
                RemoveGrimmchildCompanion();
            }

            PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
            PlayMakerFSM.BroadcastEvent(CharmIndicatorCheckEvent);
            PlayMakerFSM.BroadcastEvent(UpdateBlueHealthEvent);
            TryRefreshNailDamage();
            ScheduleHealthHudRefresh();
            TryRefreshVesselsHud();
        }
        catch (Exception ex)
        {
            LogError($"[GearSwitcher] Gear snapshot restore failed: {ex}");
        }
    }
}
