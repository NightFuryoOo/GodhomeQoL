using Mono.Cecil.Cil;
using MonoMod.Cil;
using IL;
using InControl;
using GodhomeQoL.Modules.BossChallenge;
using GodhomeQoL.Modules.QoL;
using ToggleableBindings;
using ToggleableBindings.VanillaBindings;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class GearSwitcher : Module
{
    private const int DefaultMainSoulGain = 11;
    private const int DefaultReserveSoulGain = 6;

    internal static void ApplyStatsImmediate(GearPreset preset, StatsPart parts = StatsPart.All)
    {
        if (!IsGloballyEnabled)
        {
            return;
        }

        if (!IsSafeToApply())
        {
            pendingStatsPreset = preset;
            pendingStatsParts |= parts;
            pendingStatsApply = true;
            EnsurePendingCoroutine();
            return;
        }

        ApplyStatsParts(preset, parts);
    }

    private static void ApplyStatsParts(GearPreset preset, StatsPart parts)
    {
        if (parts == StatsPart.All)
        {
            ApplyStats(preset);
            return;
        }

        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        if (parts.HasFlag(StatsPart.MaxHealth))
        {
            ApplyMaxHealthOnly(pd, preset);
        }

        if (parts.HasFlag(StatsPart.SoulVessels))
        {
            ApplySoulVesselsOnly(pd, preset);
        }

        if (parts.HasFlag(StatsPart.CharmSlots))
        {
            ApplyCharmSlotsOnly(pd, preset);
        }

        if (parts.HasFlag(StatsPart.NailDamage))
        {
            ApplyNailDamageOnly(pd, preset);
        }
    }

    internal static void ApplySpellsImmediate(GearPreset preset)
    {
        if (!IsGloballyEnabled)
        {
            return;
        }

        if (!IsSafeToApply())
        {
            pendingSpellsPreset = preset;
            pendingSpellsApply = true;
            EnsurePendingCoroutine();
            return;
        }

        ApplySpells(preset);
    }

    internal static void ApplyNailArtsImmediate(GearPreset preset)
    {
        if (!IsGloballyEnabled)
        {
            return;
        }

        if (!IsSafeToApply())
        {
            pendingNailArtsPreset = preset;
            pendingNailArtsApply = true;
            EnsurePendingCoroutine();
            return;
        }

        ApplyNailArts(preset);
    }

    internal static void ApplyAbilitiesImmediate(GearPreset preset)
    {
        if (!IsGloballyEnabled)
        {
            return;
        }

        if (!IsSafeToApply())
        {
            pendingAbilitiesPreset = preset;
            pendingAbilitiesApply = true;
            EnsurePendingCoroutine();
            return;
        }

        ApplyAbilities(preset);
    }

    internal static void ApplyDreamNailImmediate(GearPreset preset)
    {
        if (!IsGloballyEnabled)
        {
            return;
        }

        if (!IsSafeToApply())
        {
            pendingDreamNailPreset = preset;
            pendingDreamNailApply = true;
            EnsurePendingCoroutine();
            return;
        }

        ApplyDreamNail(preset);
    }

    internal static void ApplyOvercharmedImmediate(GearPreset preset)
    {
        if (!IsGloballyEnabled)
        {
            return;
        }

        if (!IsSafeToApply())
        {
            pendingOvercharmedPreset = preset;
            pendingOvercharmedApply = true;
            EnsurePendingCoroutine();
            return;
        }

        ApplyOvercharmed(preset);
    }

    internal static void DisableAndRestoreState()
        => RestoreRuntimeState(disableGlobalToggle: true);

    internal static void PrepareForEnable()
    {
        coroutineGeneration++;
        pendingCoroutineRunning = false;
        ClearPendingApplies();
    }

    private static void RestoreRuntimeState(bool disableGlobalToggle)
    {
        coroutineGeneration++;
        pendingCoroutineRunning = false;
        ClearPendingApplies();

        if (disableGlobalToggle)
        {
            IsGloballyEnabled = false;
        }

        RestoreAllBindingsImmediate(force: true);
        RestoreNailInputState();
        RestoreOvercharmedToNatural();
        RestoreCharmsBindingExemptDefaults();
        managedCharmsApplied.Clear();
        pantheonActive = false;
        pantheonShellBound = false;
    }

    private static void RestoreOvercharmedToNatural()
    {
        try
        {
            PlayerData? pd = PlayerData.instance;
            if (pd == null)
            {
                return;
            }

            pd.overcharmed = IsNaturallyOvercharmed(pd);
            PlayMakerFSM.BroadcastEvent(CharmIndicatorCheckEvent);
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "GearSwitcher.Apply.cs");
        }
    }

    private static int GetMainSoulGain() =>
        TryGetActivePreset(out GearPreset preset) ? ClampSoulGain(preset.MainSoulGain) : DefaultMainSoulGain;

    private static int GetReserveSoulGain() =>
        TryGetActivePreset(out GearPreset preset) ? ClampSoulGain(preset.ReserveSoulGain) : DefaultReserveSoulGain;

    private static int ClampSoulGain(int value) => Math.Max(0, Math.Min(198, value));

    private static bool TryGetActivePreset(out GearPreset preset)
    {
        preset = null!;
        return IsGloballyEnabled
            && Settings.Presets != null
            && Settings.Presets.TryGetValue(NormalizeBuiltinPresetName(GetLastPresetName()), out preset!);
    }

    private static void ApplyStats(GearPreset preset)
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        int maxHealth = GetPresetMaxHealth(preset);
        int charmSlots = GetPresetCharmSlots(preset);
        int vessels = GetPresetSoulVessels(preset);
        int nailDamage = GetPresetNailDamage(preset);

        int previousHealth = pd.health;
        int previousMaxHealth = pd.maxHealth;
        int previousReserveMax = pd.MPReserveMax;

        pd.maxHealth = maxHealth;
        pd.maxHealthBase = maxHealth;
        pd.prevHealth = maxHealth;
        pd.MaxHealth();

        int reserve = vessels * 33;
        if (pd.MPReserveMax < reserve && HeroController.instance != null)
        {
            HeroController.instance.AddToMaxMPReserve(reserve - pd.MPReserveMax);
        }
        else
        {
            pd.MPReserveMax = reserve;
        }

        pd.charmSlots = charmSlots;

        pd.nailDamage = nailDamage;
        PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");

        if (!IsApplyingPreset)
        {
            ApplyOvercharmed(preset);
        }

        ReapplyNonSoulActiveBindings();

        if (pd.health != previousHealth || pd.maxHealth != previousMaxHealth)
        {
            ScheduleHealthHudRefresh();
        }

        if (pd.MPReserveMax != previousReserveMax)
        {
            TryRefreshVesselsHud();
        }

        if (!IsApplyingPreset)
        {
            ResyncBindingHudIndicators(includeSoul: true);
            ScheduleBindingHudResync();
        }
    }

    private static int GetPresetMaxHealth(GearPreset preset) =>
        AlwaysFurious.IsGearSwitcherHealthLockActive() ? 1 : Math.Max(1, Math.Min(9, preset.MaxHealth));

    private static int GetPresetCharmSlots(GearPreset preset) => Math.Max(3, Math.Min(999, preset.CharmSlots));

    private static int GetPresetSoulVessels(GearPreset preset) => Math.Max(0, Math.Min(3, preset.SoulVessels));

    private static int GetPresetNailDamage(GearPreset preset) =>
        Math.Max(1, Math.Min(99999, preset.NailDamage));

    private static void ApplyMaxHealthOnly(PlayerData pd, GearPreset preset)
    {
        int maxHealth = GetPresetMaxHealth(preset);
        int previousHealth = pd.health;
        int previousMaxHealth = pd.maxHealth;

        pd.maxHealth = maxHealth;
        pd.maxHealthBase = maxHealth;
        int cap = pd.CurrentMaxHealth;
        if (pd.health > cap)
        {
            pd.health = cap;
        }

        if (pd.health != previousHealth || pd.maxHealth != previousMaxHealth)
        {
            PlayMakerFSM.BroadcastEvent(CharmIndicatorCheckEvent);
            ScheduleHealthHudRefresh();
        }
    }

    private static void ApplySoulVesselsOnly(PlayerData pd, GearPreset preset)
    {
        int reserve = GetPresetSoulVessels(preset) * 33;
        int previousReserveMax = pd.MPReserveMax;
        if (previousReserveMax == reserve)
        {
            return;
        }

        if (previousReserveMax < reserve && HeroController.instance != null)
        {
            HeroController.instance.AddToMaxMPReserve(reserve - previousReserveMax);
        }
        else
        {
            pd.MPReserveMax = reserve;
        }

        if (pd.MPReserve > pd.MPReserveMax)
        {
            pd.MPReserve = pd.MPReserveMax;
        }

        RefreshChangedVessels(previousReserveMax, pd.MPReserveMax);
    }

    private static void ApplyCharmSlotsOnly(PlayerData pd, GearPreset preset)
    {
        int charmSlots = GetPresetCharmSlots(preset);
        if (pd.charmSlots == charmSlots)
        {
            return;
        }

        pd.charmSlots = charmSlots;
        ApplyOvercharmed(preset);
        PlayMakerFSM.BroadcastEvent(CharmIndicatorCheckEvent);
    }

    private static void ApplyNailDamageOnly(PlayerData pd, GearPreset preset)
    {
        int nailDamage = GetPresetNailDamage(preset);
        if (pd.nailDamage == nailDamage)
        {
            return;
        }

        pd.nailDamage = nailDamage;
        PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
    }

    private static void ApplyAbilities(GearPreset preset)
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        bool hasAcidArmour = GetPresetBool(preset.HasMoveAbilities, "AcidArmour");
        bool hasDash = GetPresetBool(preset.HasMoveAbilities, "Dash");
        bool hasWalljump = GetPresetBool(preset.HasMoveAbilities, "Walljump");
        bool hasSuperDash = GetPresetBool(preset.HasMoveAbilities, "SuperDash");
        bool hasShadowDash = GetPresetBool(preset.HasMoveAbilities, "ShadowDash");
        bool hasDoubleJump = GetPresetBool(preset.HasMoveAbilities, "DoubleJump");

        SetAbility(pd, "hasAcidArmour", hasAcidArmour);
        SetAbility(pd, "hasDash", hasDash);
        SetAbility(pd, "canDash", hasDash);
        SetAbility(pd, "hasWalljump", hasWalljump);
        SetAbility(pd, "hasSuperDash", hasSuperDash);
        SetAbility(pd, "canSuperDash", hasSuperDash);
        SetAbility(pd, "hasShadowDash", hasShadowDash);
        SetAbility(pd, "canShadowDash", hasShadowDash);
        SetAbility(pd, "hasDoubleJump", hasDoubleJump);
    }

    private static void ApplySpells(GearPreset preset)
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        SetPlayerDataInt(pd, "fireballLevel", GetPresetInt(preset.SpellsLevel, "fireballLevel"));
        SetPlayerDataInt(pd, "quakeLevel", GetPresetInt(preset.SpellsLevel, "quakeLevel"));
        SetPlayerDataInt(pd, "screamLevel", GetPresetInt(preset.SpellsLevel, "screamLevel"));
    }

    private static void ApplyNailArts(GearPreset preset)
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        bool hasCyclone = GetPresetBool(preset.HasNailArts, "hasCyclone");
        bool hasDashSlash = GetPresetBool(preset.HasNailArts, "hasDashSlash");
        bool hasUpwardSlash = GetPresetBool(preset.HasNailArts, "hasUpwardSlash");
        bool anyArts = hasCyclone || hasDashSlash || hasUpwardSlash;
        bool allArts = hasCyclone && hasDashSlash && hasUpwardSlash;

        SetAbility(pd, "hasNailArt", anyArts);
        SetAbility(pd, "hasAllNailArts", allArts);
        SetAbility(pd, "hasCyclone", hasCyclone);
        SetAbility(pd, "hasDashSlash", hasDashSlash);
        SetAbility(pd, "hasUpwardSlash", hasUpwardSlash);
    }

    private static void ApplyDreamNail(GearPreset preset)
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        int level = Math.Max(0, Math.Min(3, preset.DreamNailLevel));
        bool hasDreamNail = level >= 1;
        bool hasDreamGate = level >= 2;
        bool upgraded = level >= 3;

        SetAbility(pd, "hasDreamNail", hasDreamNail);
        SetAbility(pd, "hasDreamGate", hasDreamGate);
        SetAbility(pd, "dreamNailUpgraded", upgraded);
    }

    private static void ApplyOvercharmed(GearPreset preset)
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        bool natural = IsNaturallyOvercharmed(pd);
        pd.overcharmed = preset.Overcharmed || natural;
    }

    private static bool IsNaturallyOvercharmed(PlayerData pd)
    {
        int total = 0;
        foreach (int idCharm in GetEquippedCharms())
        {
            total += pd.GetInt($"charmCost_{idCharm}");
        }

        return total > pd.charmSlots;
    }

    private static void SetAbility(PlayerData pd, string fieldName, bool value)
    {
        try
        {
            ReflectionHelper.SetField(pd, fieldName, value);
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "GearSwitcher.Apply.cs/" + fieldName);
        }
    }

    private static void SetPlayerDataInt(PlayerData pd, string fieldName, int value)
    {
        try
        {
            ReflectionHelper.SetField(pd, fieldName, value);
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "GearSwitcher.Apply.cs");
        }
    }
}
