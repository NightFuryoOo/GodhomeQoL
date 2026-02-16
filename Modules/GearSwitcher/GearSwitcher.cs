using Mono.Cecil.Cil;
using MonoMod.Cil;
using IL;
using InControl;
using GodhomeQoL.Modules.BossChallenge;
using ToggleableBindings;
using ToggleableBindings.VanillaBindings;

namespace GodhomeQoL.Modules.Tools;

public sealed class GearSwitcher : Module
{
    public override bool DefaultEnabled => false;
    public override bool Hidden => true;

    private const int VoidHeartCharmId = 36;
    private static readonly int[] PaleCourtCharmIds = { 41, 42, 43, 44 };

    private static bool pantheonActive;
    private static bool pantheonShellBound;
    private static bool pantheonShellSelectedFromUi;
    private static bool pantheonShellSelectionKnown;
    private static bool pendingApply;
    private static string pendingPresetName = string.Empty;
    private static bool pendingSpellsApply;
    private static bool pendingNailArtsApply;
    private static bool pendingAbilitiesApply;
    private static bool pendingDreamNailApply;
    private static bool pendingBindingsApply;
    private static bool pendingStatsApply;
    private static bool pendingCharmCostApply;
    private static bool pendingNailInputApply;
    private static bool pendingOvercharmedApply;
    private static GearPreset? pendingSpellsPreset;
    private static GearPreset? pendingNailArtsPreset;
    private static GearPreset? pendingAbilitiesPreset;
    private static GearPreset? pendingDreamNailPreset;
    private static GearPreset? pendingBindingsPreset;
    private static GearPreset? pendingStatsPreset;
    private static GearPreset? pendingCharmCostPreset;
    private static GearPreset? pendingNailInputPreset;
    private static GearPreset? pendingOvercharmedPreset;
    internal static bool IsApplyingPreset { get; private set; }
    private static bool pendingCoroutineRunning;
    private static readonly List<BindingSource> savedNailAttackBindings = new();
    private static bool hasSavedNailAttackBindings;
    private static int mainSoulGainOverride = 11;
    private static int reserveSoulGainOverride = 6;

        internal static GearSwitcherSettings Settings => GodhomeQoL.GlobalSettings.GearSwitcher ??= new GearSwitcherSettings();

        internal static bool IsGloballyEnabled
        {
            get => Settings.Enabled;
            set
            {
                Settings.Enabled = value;
                GodhomeQoL.SaveGlobalSettingsSafe();
            }
        }

    private protected override void Load()
    {
        On.HeroController.Start += OnHeroStart;
        On.HeroController.CharmUpdate += OnCharmUpdate;
        On.InputHandler.OnGUI += OnInputHandlerOnGUI;
        IL.HeroController.SoulGain += OnSoulGainIL;
        On.HealthManager.TakeDamage += OnEnemyDamaged;
        USceneManager.activeSceneChanged += OnSceneChanged;
        On.BossDoorChallengeUI.HideSequence += OnBossDoorHideSequence;
    }

    private protected override void Unload()
    {
        On.HeroController.Start -= OnHeroStart;
        On.HeroController.CharmUpdate -= OnCharmUpdate;
        On.InputHandler.OnGUI -= OnInputHandlerOnGUI;
        IL.HeroController.SoulGain -= OnSoulGainIL;
        On.HealthManager.TakeDamage -= OnEnemyDamaged;
        USceneManager.activeSceneChanged -= OnSceneChanged;
        On.BossDoorChallengeUI.HideSequence -= OnBossDoorHideSequence;
    }

    private static void OnHeroStart(On.HeroController.orig_Start orig, HeroController self)
    {
        orig(self);
        UpdatePantheonShellBindingState();
        const string startupPreset = "FullGear";
        CancelPendingPresetApply();
        if (TryGetPreset(startupPreset, out GearPreset fullGearPreset))
        {
            // Force a clean baseline on save load to avoid stale shell binding after abrupt exits.
            GodhomeQoL.GlobalSettings.GearSwitcher.LastPreset = startupPreset;
            GodhomeQoL.SaveGlobalSettingsSafe();
            QueueApplyPreset(startupPreset);
            ApplyNailInputImmediate(fullGearPreset);
        }
        else
        {
            // Fallback: keep previous behavior only if FullGear is missing unexpectedly.
            string lastPreset = GetLastPresetName();
            if (!string.IsNullOrEmpty(lastPreset))
            {
                lastPreset = NormalizeBuiltinPresetName(lastPreset);
                QueueApplyPreset(lastPreset);
                if (TryGetPreset(lastPreset, out GearPreset preset))
                {
                    ApplyNailInputImmediate(preset);
                }
            }
        }
        ScheduleOvercharmedReapply();
        ScheduleNailInputReapply();
        ScheduleShellBindingResync();
    }

    private static void OnCharmUpdate(On.HeroController.orig_CharmUpdate orig, HeroController self)
    {
        orig(self);
        ReapplyForcedOvercharmed();
    }

    private static void OnInputHandlerOnGUI(On.InputHandler.orig_OnGUI orig, InputHandler self)
    {
        orig(self);
        EnforceNaillessInput();
    }

    private static void OnSoulGainIL(ILContext il)
    {
        ILCursor cursor = new(il);
        if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(11)))
        {
            cursor.EmitDelegate<Func<int, int>>(_ => GetMainSoulGain());
        }

        cursor = new ILCursor(il);
        if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(6)))
        {
            cursor.EmitDelegate<Func<int, int>>(_ => GetReserveSoulGain());
        }
    }

    private static void OnSceneChanged(Scene from, Scene to)
    {
        UpdatePantheonShellBindingState();
    }

    private static IEnumerator OnBossDoorHideSequence(On.BossDoorChallengeUI.orig_HideSequence orig, BossDoorChallengeUI self, bool sendEvent)
    {
        IEnumerator origEnum = orig(self, sendEvent);
        while (origEnum.MoveNext())
        {
            yield return origEnum.Current;
        }

        TryRecordPantheonShellBindingFromUi(self);
        UpdatePantheonShellBindingState();
    }

    private static void UpdatePantheonShellBindingState()
    {
        bool bossRush = PlayerData.instance != null && PlayerData.instance.bossRushMode;
        bool shellBound = bossRush && GetPantheonShellBindingSelected();
        if (bossRush == pantheonActive && shellBound == pantheonShellBound)
        {
            return;
        }

        pantheonActive = bossRush;
        pantheonShellBound = shellBound;
        if (!bossRush)
        {
            pantheonShellSelectedFromUi = false;
            pantheonShellSelectionKnown = false;
        }
        ApplyPantheonShellBindingOverride();
    }

    private static void TryRecordPantheonShellBindingFromUi(BossDoorChallengeUI ui)
    {
        try
        {
            pantheonShellSelectedFromUi = ui != null && ui.boundHeartButton != null && ui.boundHeartButton.Selected;
            pantheonShellSelectionKnown = true;
        }
        catch
        {
            // ignore missing UI binding button
        }
    }

    private static bool GetPantheonShellBindingSelected()
    {
        if (pantheonShellSelectionKnown)
        {
            return pantheonShellSelectedFromUi;
        }

        // Fallback to the BossSequenceController backing fields (not the BoundShell property),
        // because our ShellBinding detour forces BoundShell=true when the mod binding is applied.
        try
        {
            Type type = typeof(BossSequenceController);
            foreach (string fieldName in new[] { "boundHeart", "boundShell" })
            {
                FieldInfo? field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (field != null && field.FieldType == typeof(bool))
                {
                    return (bool)(field.GetValue(null) ?? false);
                }
            }
        }
        catch
        {
            // ignore missing binding flags
        }

        return false;
    }

    private static void ApplyPantheonShellBindingOverride()
    {
        if (!TryGetPreset(GetLastPresetName(), out GearPreset preset))
        {
            return;
        }

        bool shouldEnableShell = GetPresetShellBindingState(preset);
        if (IsPantheonSequenceActive() && pantheonShellBound)
        {
            RestoreBindingIfActive<ShellBinding>();
            return;
        }

        if (shouldEnableShell)
        {
            BindingManager.ApplyBinding<ShellBinding>();
        }
        else
        {
            RestoreBindingIfActive<ShellBinding>();
        }
    }

    private static bool GetBossBindingFlag(string propertyName, params string[] fieldNames)
    {
        try
        {
            Type type = typeof(BossSequenceController);
            PropertyInfo? prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (prop != null && prop.PropertyType == typeof(bool))
            {
                return (bool)(prop.GetValue(null, null) ?? false);
            }

            foreach (string fieldName in fieldNames)
            {
                FieldInfo? field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (field != null && field.FieldType == typeof(bool))
                {
                    return (bool)(field.GetValue(null) ?? false);
                }
            }
        }
        catch
        {
            // ignore missing binding flags
        }

        return false;
    }

    private static void SetBossBindingFlag(bool value, string propertyName, params string[] fieldNames)
    {
        try
        {
            Type type = typeof(BossSequenceController);
            PropertyInfo? prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (prop != null && prop.PropertyType == typeof(bool) && prop.CanWrite)
            {
                prop.SetValue(null, value, null);
                return;
            }

            foreach (string fieldName in fieldNames)
            {
                FieldInfo? field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (field != null && field.FieldType == typeof(bool))
                {
                    field.SetValue(null, value);
                }
            }
        }
        catch
        {
            // ignore missing binding flags
        }
    }

    private static void InvokeBossSequenceRestoreBindings()
    {
        try
        {
            MethodInfo? method = typeof(BossSequenceController).GetMethod("RestoreBindings", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            method?.Invoke(null, null);
        }
        catch
        {
            // ignore restore errors
        }
    }

    private static void InvokeBossSequenceApplyBindings()
    {
        try
        {
            MethodInfo? method = typeof(BossSequenceController).GetMethod("ApplyBindings", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            method?.Invoke(null, null);
        }
        catch
        {
            // ignore apply errors
        }
    }

    internal static IReadOnlyList<string> GetPresetOrder()
    {
        EnsurePresetDefaults();

        List<string> defaults = GearPresetDefaults.DefaultOrder();
        List<string> order = Settings.PresetOrder ?? new List<string>(defaults);
        List<string> result = new();

        foreach (string name in defaults)
        {
            if (Settings.Presets.ContainsKey(name) && !result.Contains(name))
            {
                result.Add(name);
            }
        }

        foreach (string name in order)
        {
            if (Settings.Presets.ContainsKey(name) && !result.Contains(name))
            {
                result.Add(name);
            }
        }

        foreach (string name in Settings.Presets.Keys)
        {
            if (!result.Contains(name))
            {
                result.Add(name);
            }
        }

        Settings.PresetOrder = result;
        return result;
    }

    internal static bool TryGetPreset(string name, out GearPreset preset)
    {
        EnsurePresetDefaults();
        return Settings.Presets.TryGetValue(name, out preset!);
    }

    internal static void ResetDefaults()
    {
        Settings.Presets = GearPresetDefaults.CreateDefaults();
        Settings.PresetOrder = GearPresetDefaults.DefaultOrder();
        GodhomeQoL.GlobalSettings.GearSwitcher.LastPreset = "FullGear";
        GodhomeQoL.SaveGlobalSettingsSafe();
    }

    private static void EnsurePresetDefaults()
    {
        if (Settings.Presets == null || Settings.Presets.Count == 0)
        {
            Settings.Presets = GearPresetDefaults.CreateDefaults();
        }

        MigrateLegacyPresetName("O4", "04");
        MigrateLegacyPresetName("Ow", "0w");

        Dictionary<string, GearPreset> defaults = GearPresetDefaults.CreateDefaults();
        foreach (KeyValuePair<string, GearPreset> entry in defaults)
        {
            if (!Settings.Presets.ContainsKey(entry.Key))
            {
                Settings.Presets[entry.Key] = entry.Value;
            }
        }

        if (Settings.PresetOrder != null)
        {
            Settings.PresetOrder = Settings.PresetOrder.Where(Settings.Presets.ContainsKey).ToList();
        }
    }

    private static void MigrateLegacyPresetName(string legacyName, string newName)
    {
        if (Settings.Presets == null)
        {
            return;
        }

        string? legacyKey = Settings.Presets.Keys.FirstOrDefault(key => string.Equals(key, legacyName, StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(legacyKey))
        {
            return;
        }

        if (Settings.Presets.TryGetValue(newName, out _))
        {
            Settings.Presets.Remove(legacyKey);
        }
        else if (Settings.Presets.TryGetValue(legacyKey, out GearPreset preset))
        {
            Settings.Presets.Remove(legacyKey);
            preset.Name = newName;
            Settings.Presets[newName] = preset;
        }

        if (Settings.PresetOrder != null)
        {
            for (int i = 0; i < Settings.PresetOrder.Count; i++)
            {
                if (string.Equals(Settings.PresetOrder[i], legacyName, StringComparison.OrdinalIgnoreCase))
                {
                    Settings.PresetOrder[i] = newName;
                }
            }
        }

        if (string.Equals(GodhomeQoL.GlobalSettings.GearSwitcher.LastPreset, legacyName, StringComparison.OrdinalIgnoreCase))
        {
            GodhomeQoL.GlobalSettings.GearSwitcher.LastPreset = newName;
        }
    }

    internal static string CreateCustomPresetFromFullGear()
    {
        EnsurePresetDefaults();
        Dictionary<string, GearPreset> defaults = GearPresetDefaults.CreateDefaults();
        if (!defaults.TryGetValue("FullGear", out GearPreset preset))
        {
            return string.Empty;
        }

        string name = GetNextPresetName();
        preset.Name = name;
        Settings.Presets[name] = preset;
        Settings.PresetOrder ??= new List<string>(GearPresetDefaults.DefaultOrder());
        if (!Settings.PresetOrder.Contains(name))
        {
            Settings.PresetOrder.Add(name);
        }

        GodhomeQoL.SaveGlobalSettingsSafe();
        return name;
    }

    internal static bool RenameCustomPreset(string currentName, string newName)
    {
        if (string.IsNullOrWhiteSpace(currentName) || string.IsNullOrWhiteSpace(newName))
        {
            return false;
        }

        EnsurePresetDefaults();
        if (IsBuiltinPresetName(currentName))
        {
            return false;
        }

        if (!Settings.Presets.TryGetValue(currentName, out GearPreset preset))
        {
            return false;
        }

        if (IsBuiltinPresetName(newName))
        {
            return false;
        }

        foreach (string key in Settings.Presets.Keys)
        {
            if (string.Equals(key, newName, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(key, currentName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        if (string.Equals(currentName, newName, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        Settings.Presets.Remove(currentName);
        preset.Name = newName;
        Settings.Presets[newName] = preset;

        if (Settings.PresetOrder != null)
        {
            for (int i = 0; i < Settings.PresetOrder.Count; i++)
            {
                if (string.Equals(Settings.PresetOrder[i], currentName, StringComparison.OrdinalIgnoreCase))
                {
                    Settings.PresetOrder[i] = newName;
                }
            }
        }

        if (string.Equals(GodhomeQoL.GlobalSettings.GearSwitcher.LastPreset, currentName, StringComparison.OrdinalIgnoreCase))
        {
            GodhomeQoL.GlobalSettings.GearSwitcher.LastPreset = newName;
        }

        GodhomeQoL.SaveGlobalSettingsSafe();
        return true;
    }

    internal static bool DeleteCustomPreset(string presetName)
    {
        if (string.IsNullOrWhiteSpace(presetName))
        {
            return false;
        }

        EnsurePresetDefaults();
        if (IsBuiltinPresetName(presetName))
        {
            return false;
        }

        bool removed = Settings.Presets.Remove(presetName);
        if (!removed)
        {
            return false;
        }

        if (Settings.PresetOrder != null)
        {
            Settings.PresetOrder.RemoveAll(name => string.Equals(name, presetName, StringComparison.OrdinalIgnoreCase));
        }

        if (string.Equals(GodhomeQoL.GlobalSettings.GearSwitcher.LastPreset, presetName, StringComparison.OrdinalIgnoreCase))
        {
            GodhomeQoL.GlobalSettings.GearSwitcher.LastPreset = "FullGear";
        }

        GodhomeQoL.SaveGlobalSettingsSafe();
        return true;
    }

    private static string GetNextPresetName()
    {
        const string prefix = "Preset ";
        HashSet<string> existing = new(Settings.Presets.Keys, StringComparer.OrdinalIgnoreCase);
        int index = 1;
        while (existing.Contains($"{prefix}{index}"))
        {
            index++;
        }

        return $"{prefix}{index}";
    }

    internal static void ApplyPreset(string presetName, bool allowQueue)
    {
        if (!IsGloballyEnabled)
        {
            return;
        }

        presetName = NormalizeBuiltinPresetName(presetName);
        UpdatePantheonShellBindingState();
        if (allowQueue)
        {
            CancelPendingPresetApply();
        }
        if (!TryGetPreset(presetName, out GearPreset preset))
        {
            return;
        }

        ApplySoulGainImmediate(preset);

        if (IsBuiltinPresetName(presetName))
        {
            ResetBuiltinPreset(presetName);
            if (!TryGetPreset(presetName, out preset))
            {
                return;
            }
        }

        if (IsAllBindingsPreset(presetName))
        {
            EnsureAllBindingsPresetValues(preset);
        }

        if (!HasAnyBindingEnabled(preset))
        {
            RestoreAllBindingsImmediate();
        }

        GodhomeQoL.GlobalSettings.GearSwitcher.LastPreset = presetName;
        GodhomeQoL.SaveGlobalSettingsSafe();

        if (!IsSafeToApply())
        {
            if (allowQueue)
            {
                QueueApplyPreset(presetName);
            }

            return;
        }

        IsApplyingPreset = true;
        try
        {
            ApplyStats(preset);
            ApplyAbilities(preset);
            ApplySpells(preset);
            ApplyNailArts(preset);
            ApplyDreamNail(preset);
            ApplyBindings(preset);
            ApplyNailInput(preset);
            ApplyCharmCosts(preset);
        }
        finally
        {
            IsApplyingPreset = false;
            AlwaysFurious.NotifyGearSwitcherApplied();
            ScheduleShellBindingResync();
        }
    }

    private static bool IsAllBindingsPreset(string presetName)
    {
        return string.Equals(presetName, "AB", StringComparison.OrdinalIgnoreCase)
            || string.Equals(presetName, "04", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsBuiltinPresetName(string presetName)
    {
        foreach (string name in GearPresetDefaults.DefaultOrder())
        {
            if (string.Equals(presetName, name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static string NormalizeBuiltinPresetName(string presetName)
    {
        if (string.Equals(presetName, "O4", StringComparison.OrdinalIgnoreCase)
            || string.Equals(presetName, "04", StringComparison.OrdinalIgnoreCase))
        {
            return "04";
        }

        if (string.Equals(presetName, "Ow", StringComparison.OrdinalIgnoreCase)
            || string.Equals(presetName, "0w", StringComparison.OrdinalIgnoreCase))
        {
            return "0w";
        }

        return presetName;
    }

    private static void ResetBuiltinPreset(string presetName)
    {
        Dictionary<string, GearPreset> defaults = GearPresetDefaults.CreateDefaults();
        if (!defaults.TryGetValue(presetName, out GearPreset preset))
        {
            return;
        }

        Settings.Presets[presetName] = preset;
        GodhomeQoL.SaveGlobalSettingsSafe();
    }

    private static bool HasAnyBindingEnabled(GearPreset preset)
    {
        if (preset.HasAllBindings)
        {
            return true;
        }

        return GetPresetBool(preset.Bindings, "CharmsBinding")
            || GetPresetBool(preset.Bindings, "NailBinding")
            || GetPresetBool(preset.Bindings, "ShellBinding")
            || GetPresetBool(preset.Bindings, "SoulBinding");
    }

    private static void EnsureAllBindingsPresetValues(GearPreset preset)
    {
        preset.HasAllBindings = true;
        preset.Bindings ??= new Dictionary<string, bool>();
        preset.Bindings["CharmsBinding"] = true;
        preset.Bindings["NailBinding"] = true;
        preset.Bindings["ShellBinding"] = true;
        preset.Bindings["SoulBinding"] = true;
        GodhomeQoL.SaveGlobalSettingsSafe();
    }

    internal static void ApplyStatsImmediate(GearPreset preset)
    {
        if (!IsGloballyEnabled)
        {
            return;
        }

        if (!IsSafeToApply())
        {
            pendingStatsPreset = preset;
            pendingStatsApply = true;
            EnsurePendingCoroutine();
            return;
        }

        ApplyStats(preset);
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

    internal static void ApplyBindingsImmediate(GearPreset preset)
    {
        if (!IsGloballyEnabled)
        {
            return;
        }

        if (!IsSafeToApply())
        {
            pendingBindingsPreset = preset;
            pendingBindingsApply = true;
            EnsurePendingCoroutine();
            return;
        }

        ApplyBindings(preset);
    }

    internal static void ApplyNailInputImmediate(GearPreset preset)
    {
        if (!IsGloballyEnabled)
        {
            return;
        }

        if (!IsSafeToApply())
        {
            pendingNailInputPreset = preset;
            pendingNailInputApply = true;
            EnsurePendingCoroutine();
            return;
        }

        ApplyNailInput(preset);
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

    internal static void RestoreAllBindingsImmediate()
    {
        if (!IsGloballyEnabled)
        {
            return;
        }

        try
        {
            BindingManager.RestoreBinding<NailBinding>();
            // Shell binding can remain stale after abrupt exits (ALT+F4 on Godseeker),
            // so use the hard restore path that also clears BossSequence flags.
            ForceRestoreShellBindingHard();
            BindingManager.RestoreBinding<CharmsBinding>();
            BindingManager.RestoreBinding<SoulBinding>();
            TryRefreshNailDamage();
        }
        catch
        {
            // ignore binding restore errors
        }
    }

    internal static void ApplyCharmCostsImmediate(GearPreset preset)
    {
        if (!IsGloballyEnabled)
        {
            return;
        }

        if (!IsSafeToApply())
        {
            pendingCharmCostPreset = preset;
            pendingCharmCostApply = true;
            EnsurePendingCoroutine();
            return;
        }

        ApplyCharmCosts(preset);
    }

    private static void CancelPendingPresetApply()
    {
        pendingApply = false;
        pendingPresetName = string.Empty;
    }
    private static void QueueApplyPreset(string presetName)
    {
        pendingPresetName = presetName;
        pendingApply = true;
        EnsurePendingCoroutine();
    }

    internal static void ClearPendingApplies()
    {
        pendingApply = false;
        pendingPresetName = string.Empty;
        pendingSpellsApply = false;
        pendingNailArtsApply = false;
        pendingAbilitiesApply = false;
        pendingDreamNailApply = false;
        pendingBindingsApply = false;
        pendingStatsApply = false;
        pendingCharmCostApply = false;
        pendingNailInputApply = false;
        pendingOvercharmedApply = false;
        pendingSpellsPreset = null;
        pendingNailArtsPreset = null;
        pendingAbilitiesPreset = null;
        pendingDreamNailPreset = null;
        pendingBindingsPreset = null;
        pendingStatsPreset = null;
        pendingCharmCostPreset = null;
        pendingNailInputPreset = null;
        pendingOvercharmedPreset = null;
    }

    private static void EnsurePendingCoroutine()
    {
        if (pendingCoroutineRunning)
        {
            return;
        }

        pendingCoroutineRunning = true;
        _ = GlobalCoroutineExecutor.Start(ApplyWhenSafe());
    }

    private static void ScheduleOvercharmedReapply()
    {
        _ = GlobalCoroutineExecutor.Start(DelayedOvercharmedReapply());
    }

    private static void ScheduleNailInputReapply()
    {
        _ = GlobalCoroutineExecutor.Start(DelayedNailInputReapply());
    }

    private static void ReapplyForcedOvercharmed()
    {
        if (!TryGetPreset(GetLastPresetName(), out GearPreset preset))
        {
            return;
        }

        if (!preset.Overcharmed)
        {
            return;
        }

        ApplyOvercharmedImmediate(preset);
    }

    private static IEnumerator DelayedOvercharmedReapply()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return null;
        }

        if (!TryGetPreset(GetLastPresetName(), out GearPreset preset))
        {
            yield break;
        }

        ApplyOvercharmedImmediate(preset);
    }

    private static IEnumerator DelayedNailInputReapply()
    {
        for (int i = 0; i < 30; i++)
        {
            yield return null;
        }

        EnsureNailInputState();

        for (int i = 0; i < 30; i++)
        {
            yield return null;
        }

        EnsureNailInputState();
    }


    private static void ScheduleShellBindingResync()
    {
        _ = GlobalCoroutineExecutor.Start(DelayedShellBindingResync());
    }

    private static IEnumerator DelayedShellBindingResync()
    {
        // Re-sync several times because serialized bindings can be re-applied shortly after load.
        for (int i = 0; i < 2; i++)
        {
            yield return null;
        }

        SyncShellBindingWithLastPreset();

        for (int i = 0; i < 20; i++)
        {
            yield return null;
        }

        SyncShellBindingWithLastPreset();

        for (int i = 0; i < 40; i++)
        {
            yield return null;
        }

        SyncShellBindingWithLastPreset();
    }

    private static void SyncShellBindingWithLastPreset()
    {
        string lastPreset = NormalizeBuiltinPresetName(GetLastPresetName());
        if (string.IsNullOrEmpty(lastPreset))
        {
            ForceRestoreShellBinding();
            return;
        }

        if (!TryGetPreset(lastPreset, out GearPreset preset))
        {
            return;
        }

        bool shouldEnableShell = GetPresetShellBindingState(preset);
        if (IsPantheonSequenceActive() && pantheonShellBound)
        {
            shouldEnableShell = false;
        }

        if (shouldEnableShell)
        {
            SetBinding<ShellBinding>(true);
            return;
        }

        ForceRestoreShellBinding();
    }

    private static void ForceRestoreShellBinding()
    {
        ForceRestoreShellBindingHard();
    }

    private static IEnumerator EnsureShellBindingRestored()
    {
        for (int attempt = 0; attempt < 3; attempt++)
        {
            yield return null;

            if (!IsShellBindingApplied())
            {
                yield break;
            }

            SetBinding<ShellBinding>(false);
        }
    }

    private static bool IsShellBindingApplied()
    {
        try
        {
            if (BindingManager.TryGetBinding<ShellBinding>(out ShellBinding? binding) && binding != null)
            {
                return binding.IsApplied;
            }
        }
        catch
        {
            // ignore binding read errors
        }

        return false;
    }

    private static void EnsureNailInputState()
    {
        if (!IsGloballyEnabled)
        {
            return;
        }

        string lastPreset = NormalizeBuiltinPresetName(GetLastPresetName());
        if (string.IsNullOrEmpty(lastPreset))
        {
            return;
        }

        if (!TryGetPreset(lastPreset, out GearPreset preset))
        {
            return;
        }

        ApplyNailInputImmediate(preset);
    }

    private static IEnumerator ApplyWhenSafe()
    {
        while (pendingApply || pendingSpellsApply || pendingNailArtsApply || pendingAbilitiesApply || pendingDreamNailApply || pendingBindingsApply || pendingStatsApply || pendingCharmCostApply || pendingNailInputApply || pendingOvercharmedApply)
        {
            if (!IsGloballyEnabled)
            {
                ClearPendingApplies();
                break;
            }

            if (IsSafeToApply())
            {
                if (pendingApply)
                {
                    pendingApply = false;
                    ApplyPreset(pendingPresetName, false);
                }

                if (pendingStatsApply)
                {
                    pendingStatsApply = false;
                    if (pendingStatsPreset != null)
                    {
                        ApplyStats(pendingStatsPreset);
                    }
                }

                if (pendingSpellsApply)
                {
                    pendingSpellsApply = false;
                    if (pendingSpellsPreset != null)
                    {
                        ApplySpells(pendingSpellsPreset);
                    }
                }

                if (pendingNailArtsApply)
                {
                    pendingNailArtsApply = false;
                    if (pendingNailArtsPreset != null)
                    {
                        ApplyNailArts(pendingNailArtsPreset);
                    }
                }

                if (pendingAbilitiesApply)
                {
                    pendingAbilitiesApply = false;
                    if (pendingAbilitiesPreset != null)
                    {
                        ApplyAbilities(pendingAbilitiesPreset);
                    }
                }

                if (pendingDreamNailApply)
                {
                    pendingDreamNailApply = false;
                    if (pendingDreamNailPreset != null)
                    {
                        ApplyDreamNail(pendingDreamNailPreset);
                    }
                }

                if (pendingBindingsApply)
                {
                    pendingBindingsApply = false;
                    if (pendingBindingsPreset != null)
                    {
                        ApplyBindings(pendingBindingsPreset);
                    }
                }

                if (pendingNailInputApply)
                {
                    pendingNailInputApply = false;
                    if (pendingNailInputPreset != null)
                    {
                        ApplyNailInput(pendingNailInputPreset);
                    }
                }

                if (pendingOvercharmedApply)
                {
                    pendingOvercharmedApply = false;
                    if (pendingOvercharmedPreset != null)
                    {
                        ApplyOvercharmed(pendingOvercharmedPreset);
                    }
                }

                if (pendingCharmCostApply)
                {
                    pendingCharmCostApply = false;
                    if (pendingCharmCostPreset != null)
                    {
                        ApplyCharmCosts(pendingCharmCostPreset);
                    }
                }
            }

            yield return null;
        }

        pendingCoroutineRunning = false;
    }

    private static string GetLastPresetName() =>
        GodhomeQoL.GlobalSettings.GearSwitcher.LastPreset ?? string.Empty;

    internal static void ApplySoulGainImmediate(GearPreset preset)
    {
        if (!IsGloballyEnabled)
        {
            return;
        }

        mainSoulGainOverride = ClampSoulGain(preset.MainSoulGain);
        reserveSoulGainOverride = ClampSoulGain(preset.ReserveSoulGain);
    }

    private static int GetMainSoulGain() => mainSoulGainOverride;

    private static int GetReserveSoulGain() => reserveSoulGainOverride;

    private static int ClampSoulGain(int value) => Math.Max(0, Math.Min(198, value));

    private static void SaveCurrentCharmsToLastPreset()
    {
        string lastPreset = GetLastPresetName();
        if (string.IsNullOrEmpty(lastPreset))
        {
            return;
        }

        if (!TryGetPreset(lastPreset, out GearPreset preset))
        {
            return;
        }

        preset.EquippedCharms = GetEquippedCharms();
    }

    private static void ApplyStats(GearPreset preset)
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        int maxHealth = Math.Max(1, Math.Min(9, preset.MaxHealth));
        int charmSlots = Math.Max(3, Math.Min(999, preset.CharmSlots));
        int vessels = Math.Max(0, Math.Min(3, preset.SoulVessels));
        int rawNailDamage = Math.Max(-99999, Math.Min(99999, preset.NailDamage));
        int nailDamage = rawNailDamage < 1 ? 1 : rawNailDamage;

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

        ApplyOvercharmed(preset);
        ReapplyActiveBindings();
        TryRefreshHud();
    }

    private static void OnEnemyDamaged(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
    {
        if (!IsGloballyEnabled)
        {
            orig(self, hitInstance);
            return;
        }

        string lastPreset = NormalizeBuiltinPresetName(GetLastPresetName());
        if (!TryGetPreset(lastPreset, out GearPreset preset))
        {
            orig(self, hitInstance);
            return;
        }

        int rawNailDamage = Math.Max(-99999, Math.Min(99999, preset.NailDamage));
        if (rawNailDamage >= 0 || !IsNailAttack(hitInstance))
        {
            orig(self, hitInstance);
            return;
        }

        int healAmount = Math.Abs(rawNailDamage);
        if (healAmount <= 0)
        {
            orig(self, hitInstance);
            return;
        }

        hitInstance.DamageDealt = 1;
        self.hp += healAmount + 1;
        orig(self, hitInstance);
    }

    private static bool IsNailAttack(HitInstance hitInstance) =>
        hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam;

    private static void ReapplyActiveBindings()
    {
        ReapplyBindingIfActive<NailBinding>();
        ReapplyBindingIfActive<ShellBinding>();
        ReapplyBindingIfActive<CharmsBinding>();
        ReapplyBindingIfActive<SoulBinding>();
    }

    private static void ReapplyBindingIfActive<T>() where T : Binding
    {
        try
        {
            if (BindingManager.TryGetBinding<T>(out T? binding) && binding != null && binding.IsApplied)
            {
                BindingManager.RestoreBinding<T>();
                BindingManager.ApplyBinding<T>();
            }
        }
        catch
        {
            // ignore binding refresh errors
        }
    }

    private static void RestoreBindingIfActive<T>() where T : Binding
    {
        try
        {
            if (BindingManager.TryGetBinding<T>(out T? binding) && binding != null && binding.IsApplied)
            {
                BindingManager.RestoreBinding<T>();
            }
        }
        catch
        {
            // ignore binding restore errors
        }
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
        SetAbility(pd, "canAcidArmour", hasAcidArmour);
        SetAbility(pd, "hasDash", hasDash);
        SetAbility(pd, "canDash", hasDash);
        SetAbility(pd, "hasWalljump", hasWalljump);
        SetAbility(pd, "canWalljump", hasWalljump);
        SetAbility(pd, "hasSuperDash", hasSuperDash);
        SetAbility(pd, "canSuperDash", hasSuperDash);
        SetAbility(pd, "hasShadowDash", hasShadowDash);
        SetAbility(pd, "canShadowDash", hasShadowDash);
        SetAbility(pd, "hasDoubleJump", hasDoubleJump);
        SetAbility(pd, "canDoubleJump", hasDoubleJump);
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

    private static void ApplyBindings(GearPreset preset)
    {
        try
        {
            bool applyCharms = preset.HasAllBindings || GetPresetBool(preset.Bindings, "CharmsBinding");
            bool applyNail = preset.HasAllBindings || GetPresetBool(preset.Bindings, "NailBinding");
            bool applyShell = GetPresetShellBindingState(preset);
            bool applySoul = preset.HasAllBindings || GetPresetBool(preset.Bindings, "SoulBinding");

            if (IsPantheonSequenceActive() && pantheonShellBound)
            {
                applyShell = false;
            }

            if (applyCharms)
            {
                SetCharmsBindingExemptCharms();
            }

            SetBinding<CharmsBinding>(applyCharms);
            SetBinding<NailBinding>(applyNail);
            SetShellBinding(applyShell);
            SetBinding<SoulBinding>(applySoul);

            if (applyCharms)
            {
                RemoveCharmsExceptVoidHeart();
            }

            TryRefreshNailDamage();
        }
        catch
        {
            // ignore binding errors
        }
    }

    private static void SetCharmsBindingExemptCharms()
    {
        try
        {
            ToggleableBindings.VanillaBindings.CharmsBinding.ExemptCharms = new[] { VoidHeartCharmId };
        }
        catch
        {
            // ignore if binding isn't available
        }
    }

    private static void RemoveCharmsExceptVoidHeart()
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        RemoveCharms(pd);
        PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
        PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
        PlayMakerFSM.BroadcastEvent("UPDATE BLUE HEALTH");
    }

    private static void ApplyNailInput(GearPreset preset)
    {
        PlayerAction? action = GetNailAttackAction();
        if (action == null)
        {
            pendingNailInputPreset = preset;
            pendingNailInputApply = true;
            EnsurePendingCoroutine();
            return;
        }

        if (preset.Nailless)
        {
            DisableNailAttack(action, preserveStored: false);
        }
        else
        {
            RestoreNailAttack(action);
        }
    }

    private static PlayerAction? GetNailAttackAction()
    {
        GameManager? manager = GameManager.instance;
        if (manager == null || manager.inputHandler == null)
        {
            return null;
        }

        return manager.inputHandler.ActionButtonToPlayerAction(HeroActionButton.ATTACK);
    }

    private static void CacheNailAttackBindings(PlayerAction action)
    {
        if (action.Bindings.Count == 0)
        {
            return;
        }

        savedNailAttackBindings.Clear();
        foreach (BindingSource binding in action.Bindings)
        {
            savedNailAttackBindings.Add(binding);
        }

        hasSavedNailAttackBindings = true;
        SaveNailAttackBindings(action);
    }

    private static void SaveNailAttackBindings(PlayerAction action)
    {
        InputHandler.KeyOrMouseBinding keyBinding = KeybindUtil.GetKeyOrMouseBinding(action);
        InputControlType controllerBinding = KeybindUtil.GetControllerButtonBinding(action);

        string keyName = string.Empty;
        string mouseName = string.Empty;

        if (TryGetBindingKey(keyBinding, out Key key) && !EqualityComparer<Key>.Default.Equals(key, default))
        {
            keyName = key.ToString();
        }
        else if (TryGetBindingMouse(keyBinding, out Mouse mouse) && !EqualityComparer<Mouse>.Default.Equals(mouse, default))
        {
            mouseName = mouse.ToString();
        }

        Settings.NailAttackKeyBinding = keyName;
        Settings.NailAttackMouseBinding = mouseName;
        Settings.NailAttackControllerBinding = controllerBinding;
        Settings.NailAttackBindingsStored = !string.IsNullOrEmpty(keyName)
            || !string.IsNullOrEmpty(mouseName)
            || !EqualityComparer<InputControlType>.Default.Equals(controllerBinding, default);

        GodhomeQoL.SaveGlobalSettingsSafe();
    }

    private static void RestoreNailAttackFromSettings(PlayerAction action)
    {
        action.ClearBindings();

        InputControlType controllerBinding = Settings.NailAttackControllerBinding;
        if (!EqualityComparer<InputControlType>.Default.Equals(controllerBinding, default))
        {
            KeybindUtil.AddInputControlType(action, controllerBinding);
        }

        if (!string.IsNullOrEmpty(Settings.NailAttackKeyBinding)
            && Enum.TryParse(Settings.NailAttackKeyBinding, out Key key))
        {
            KeybindUtil.AddKeyOrMouseBinding(action, new InputHandler.KeyOrMouseBinding(key));
            return;
        }

        if (!string.IsNullOrEmpty(Settings.NailAttackMouseBinding)
            && Enum.TryParse(Settings.NailAttackMouseBinding, out Mouse mouse))
        {
            KeybindUtil.AddKeyOrMouseBinding(action, new InputHandler.KeyOrMouseBinding(mouse));
        }
    }

    private static bool TryGetBindingKey(InputHandler.KeyOrMouseBinding binding, out Key key)
    {
        object boxed = binding;
        Type type = boxed.GetType();

        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (property.PropertyType == typeof(Key))
            {
                key = (Key)property.GetValue(boxed);
                return true;
            }
        }

        foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (field.FieldType == typeof(Key))
            {
                key = (Key)field.GetValue(boxed);
                return true;
            }
        }

        key = default;
        return false;
    }

    private static bool TryGetBindingMouse(InputHandler.KeyOrMouseBinding binding, out Mouse mouse)
    {
        object boxed = binding;
        Type type = boxed.GetType();

        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (property.PropertyType == typeof(Mouse))
            {
                mouse = (Mouse)property.GetValue(boxed);
                return true;
            }
        }

        foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (field.FieldType == typeof(Mouse))
            {
                mouse = (Mouse)field.GetValue(boxed);
                return true;
            }
        }

        mouse = default;
        return false;
    }

    private static void DisableNailAttack(PlayerAction action, bool preserveStored)
    {
        if (!preserveStored || (!hasSavedNailAttackBindings && !Settings.NailAttackBindingsStored))
        {
            CacheNailAttackBindings(action);
        }

        action.ClearBindings();
    }

    private static void RestoreNailAttack(PlayerAction action)
    {
        if (action.Bindings.Count > 0)
        {
            CacheNailAttackBindings(action);
            return;
        }

        if (hasSavedNailAttackBindings)
        {
            action.ClearBindings();
            foreach (BindingSource binding in savedNailAttackBindings)
            {
                action.AddBinding(binding);
            }

            return;
        }

        if (Settings.NailAttackBindingsStored)
        {
            RestoreNailAttackFromSettings(action);
            CacheNailAttackBindings(action);
        }
        else
        {
            action.ClearBindings();
        }
    }

    private static void EnforceNaillessInput()
    {
        if (!IsGloballyEnabled)
        {
            return;
        }

        string lastPreset = NormalizeBuiltinPresetName(GetLastPresetName());
        if (string.IsNullOrEmpty(lastPreset))
        {
            return;
        }

        if (!TryGetPreset(lastPreset, out GearPreset preset) || !preset.Nailless)
        {
            return;
        }

        PlayerAction? action = GetNailAttackAction();
        if (action == null || action.Bindings.Count == 0)
        {
            return;
        }

        DisableNailAttack(action, preserveStored: true);
    }

    private static void ApplyCharmCosts(GearPreset preset)
    {
        PlayerData? data = PlayerData.instance;
        if (data == null)
        {
            return;
        }

        int gatheringCost = Math.Max(0, Math.Min(99, preset.GatheringSwarmCost));
        int compassCost = Math.Max(0, Math.Min(99, preset.WaywardCompassCost));
        int stalwartCost = Math.Max(0, Math.Min(99, preset.StalwartShellCost));
        int soulCatcherCost = Math.Max(0, Math.Min(99, preset.SoulCatcherCost));
        int shamanStoneCost = Math.Max(0, Math.Min(99, preset.ShamanStoneCost));
        int soulEaterCost = Math.Max(0, Math.Min(99, preset.SoulEaterCost));
        int dashmasterCost = Math.Max(0, Math.Min(99, preset.DashmasterCost));
        int sprintmasterCost = Math.Max(0, Math.Min(99, preset.SprintmasterCost));
        int grubsongCost = Math.Max(0, Math.Min(99, preset.GrubsongCost));
        int grubberflysElegyCost = Math.Max(0, Math.Min(99, preset.GrubberflysElegyCost));
        int unbreakableHeartCost = Math.Max(0, Math.Min(99, preset.UnbreakableHeartCost));
        int unbreakableGreedCost = Math.Max(0, Math.Min(99, preset.UnbreakableGreedCost));
        int unbreakableStrengthCost = Math.Max(0, Math.Min(99, preset.UnbreakableStrengthCost));
        int spellTwisterCost = Math.Max(0, Math.Min(99, preset.SpellTwisterCost));
        int steadyBodyCost = Math.Max(0, Math.Min(99, preset.SteadyBodyCost));
        int heavyBlowCost = Math.Max(0, Math.Min(99, preset.HeavyBlowCost));
        int quickSlashCost = Math.Max(0, Math.Min(99, preset.QuickSlashCost));
        int longnailCost = Math.Max(0, Math.Min(99, preset.LongnailCost));
        int markOfPrideCost = Math.Max(0, Math.Min(99, preset.MarkOfPrideCost));
        int furyOfTheFallenCost = Math.Max(0, Math.Min(99, preset.FuryOfTheFallenCost));
        int thornsOfAgonyCost = Math.Max(0, Math.Min(99, preset.ThornsOfAgonyCost));
        int baldurShellCost = Math.Max(0, Math.Min(99, preset.BaldurShellCost));
        int flukenestCost = Math.Max(0, Math.Min(99, preset.FlukenestCost));
        int defendersCrestCost = Math.Max(0, Math.Min(99, preset.DefendersCrestCost));
        int glowingWombCost = Math.Max(0, Math.Min(99, preset.GlowingWombCost));
        int quickFocusCost = Math.Max(0, Math.Min(99, preset.QuickFocusCost));
        int deepFocusCost = Math.Max(0, Math.Min(99, preset.DeepFocusCost));
        int lifebloodHeartCost = Math.Max(0, Math.Min(99, preset.LifebloodHeartCost));
        int lifebloodCoreCost = Math.Max(0, Math.Min(99, preset.LifebloodCoreCost));
        int jonisBlessingCost = Math.Max(0, Math.Min(99, preset.JonisBlessingCost));
        int hivebloodCost = Math.Max(0, Math.Min(99, preset.HivebloodCost));
        int sporeShroomCost = Math.Max(0, Math.Min(99, preset.SporeShroomCost));
        int sharpShadowCost = Math.Max(0, Math.Min(99, preset.SharpShadowCost));
        int shapeOfUnnCost = Math.Max(0, Math.Min(99, preset.ShapeOfUnnCost));
        int nailmastersGloryCost = Math.Max(0, Math.Min(99, preset.NailmastersGloryCost));
        int weaversongCost = Math.Max(0, Math.Min(99, preset.WeaversongCost));
        int dreamWielderCost = Math.Max(0, Math.Min(99, preset.DreamWielderCost));
        int dreamshieldCost = Math.Max(0, Math.Min(99, preset.DreamshieldCost));
        bool useGrimmchild = preset.UseGrimmchild;
        int grimmchildCost = Math.Max(0, Math.Min(99, preset.GrimmchildCost));
        int carefreeMelodyCost = Math.Max(0, Math.Min(99, preset.CarefreeMelodyCost));
        bool useVoidHeart = preset.UseVoidHeart;
        int voidHeartCost = Math.Max(0, Math.Min(99, useVoidHeart ? preset.VoidHeartCost : preset.KingsoulCost));

        int previousGrimmchildLevel = data.grimmChildLevel;

        data.charmCost_1 = gatheringCost;
        data.charmCost_2 = compassCost;
        data.charmCost_4 = stalwartCost;
        data.charmCost_20 = soulCatcherCost;
        data.charmCost_19 = shamanStoneCost;
        data.charmCost_21 = soulEaterCost;
        data.charmCost_31 = dashmasterCost;
        data.charmCost_37 = sprintmasterCost;
        data.charmCost_3 = grubsongCost;
        data.charmCost_35 = grubberflysElegyCost;
        data.charmCost_23 = unbreakableHeartCost;
        data.charmCost_24 = unbreakableGreedCost;
        data.charmCost_25 = unbreakableStrengthCost;
        data.charmCost_33 = spellTwisterCost;
        data.charmCost_14 = steadyBodyCost;
        data.charmCost_15 = heavyBlowCost;
        data.charmCost_32 = quickSlashCost;
        data.charmCost_18 = longnailCost;
        data.charmCost_13 = markOfPrideCost;
        data.charmCost_6 = furyOfTheFallenCost;
        data.charmCost_12 = thornsOfAgonyCost;
        data.charmCost_5 = baldurShellCost;
        data.charmCost_11 = flukenestCost;
        data.charmCost_10 = defendersCrestCost;
        data.charmCost_22 = glowingWombCost;
        data.charmCost_7 = quickFocusCost;
        data.charmCost_34 = deepFocusCost;
        data.charmCost_8 = lifebloodHeartCost;
        data.charmCost_9 = lifebloodCoreCost;
        data.charmCost_27 = jonisBlessingCost;
        data.charmCost_29 = hivebloodCost;
        data.charmCost_17 = sporeShroomCost;
        data.charmCost_16 = sharpShadowCost;
        data.charmCost_28 = shapeOfUnnCost;
        data.charmCost_26 = nailmastersGloryCost;
        data.charmCost_39 = weaversongCost;
        data.charmCost_30 = dreamWielderCost;
        data.charmCost_38 = dreamshieldCost;
        data.charmCost_40 = useGrimmchild ? grimmchildCost : carefreeMelodyCost;
        data.grimmChildLevel = useGrimmchild ? 4 : 5;
        data.SetBoolInternal("destroyedNightmareLantern", !useGrimmchild);
        data.charmCost_36 = voidHeartCost;
        data.royalCharmState = useVoidHeart ? 4 : 3;

        HeroController? hero = HeroController.instance;
        if (hero != null)
        {
            if (useGrimmchild)
            {
                EnsureGrimmchildUnlocked(data);
                RefreshGrimmchildCharm(data);
            }
            else
            {
                RemoveGrimmchildCompanion();
                if (previousGrimmchildLevel != data.grimmChildLevel)
                {
                    RefreshGrimmchildCharm(data);
                }
            }

            hero.CharmUpdate();
            PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
            PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
        }

        ApplyOvercharmed(preset);
    }

    private static void RefreshGrimmchildCharm(PlayerData data)
    {
        if (!data.GetBoolInternal("equippedCharm_40"))
        {
            return;
        }

        try
        {
            data.SetBoolInternal("equippedCharm_40", false);
            data.UnequipCharm(40);
            data.SetBoolInternal("equippedCharm_40", true);
            data.EquipCharm(40);
            data.CalculateNotchesUsed();
            HeroController.instance?.CharmUpdate();
            PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
            PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");

            if (data.grimmChildLevel < 5)
            {
                GameObject? existing = GameObject.FindWithTag("Grimmchild");
                if (existing != null)
                {
                    UnityEngine.Object.Destroy(existing);
                }

                if (HeroController.instance != null)
                {
                    GameManager.instance?.StartCoroutine(SpawnGrimmChildCoroutine());
                }
            }
        }
        catch
        {
            // ignore charm refresh errors
        }
    }

    private static IEnumerator SpawnGrimmChildCoroutine()
    {
        for (int i = 0; i < 2; i++)
        {
            yield return null;
        }

        try
        {
            Transform? effects = HeroController.instance?.transform.Find("Charm Effects");
            if (effects == null)
            {
                yield break;
            }

            PlayMakerFSM? fsm = effects.gameObject.LocateMyFSM("Spawn Grimmchild");
            fsm?.SendEvent("CHARM EQUIP CHECK");
        }
        catch
        {
            // ignore spawn errors
        }
    }

    private static void RemoveGrimmchildCompanion()
    {
        try
        {
            GameObject? existing = GameObject.FindWithTag("Grimmchild");
            if (existing != null)
            {
                UnityEngine.Object.Destroy(existing);
            }
        }
        catch
        {
            // ignore removal errors
        }
    }

    private static void EnsureGrimmchildUnlocked(PlayerData data)
    {
        try
        {
            data.SetBoolInternal("gotCharm_40", true);
            data.SetBoolInternal("newCharm_40", false);
        }
        catch
        {
            // ignore missing fields
        }
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

    private static void ApplyCharms(List<int>? charmIds)
    {
        if (charmIds == null)
        {
            return;
        }

        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        RemoveCharms(pd);
        EquipCharms(pd, charmIds);

        PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
        PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
        PlayMakerFSM.BroadcastEvent("UPDATE BLUE HEALTH");
    }

    private static void SetShellBinding(bool value)
    {
        if (value)
        {
            SetBinding<ShellBinding>(true);
            return;
        }

        ForceRestoreShellBindingHard();
    }

    private static void ForceRestoreShellBindingHard()
    {
        SetBinding<ShellBinding>(false);

        bool preservePantheonSelection = IsPantheonSequenceActive() && pantheonShellBound;
        if (!preservePantheonSelection)
        {
            // Clear potential stale Godhome shell flags that can survive abrupt exits.
            SetBossBindingFlag(false, "BoundShell", "boundHeart", "boundShell");
            InvokeBossSequenceRestoreBindings();
        }

        TryRefreshHud();
        _ = GlobalCoroutineExecutor.Start(EnsureShellBindingFullyRestored());
    }

    private static IEnumerator EnsureShellBindingFullyRestored()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return null;

            bool preservePantheonSelection = IsPantheonSequenceActive() && pantheonShellBound;
            if (!preservePantheonSelection)
            {
                SetBossBindingFlag(false, "BoundShell", "boundHeart", "boundShell");
                InvokeBossSequenceRestoreBindings();
            }

            if (!IsShellBindingApplied())
            {
                TryRefreshHud();
                yield break;
            }

            SetBinding<ShellBinding>(false);
        }

        TryRefreshHud();
    }

    private static bool IsGodhomeHubScene()
    {
        Scene active = USceneManager.GetActiveScene();
        string name = active.name ?? string.Empty;
        return string.Equals(name, "GG_Workshop", StringComparison.Ordinal)
            || string.Equals(name, "GG_Atrium", StringComparison.Ordinal)
            || string.Equals(name, "GG_Atrium_Roof", StringComparison.Ordinal);
    }

    private static bool IsPantheonSequenceActive()
    {
        try
        {
            return BossSequenceController.IsInSequence;
        }
        catch
        {
            return false;
        }
    }    private static void SetBinding<T>(bool value) where T : Binding, new()
    {
        try
        {
            if (value)
            {
                BindingManager.ApplyBinding<T>();
            }
            else
            {
                BindingManager.RestoreBinding<T>();
            }
        }
        catch
        {
            // ignore binding errors
        }
    }

    private static bool SetEquippedCharms(PlayerData pd, List<int> charmIds)
    {
        try
        {
            object? value = ReflectionHelper.GetField<PlayerData, object>(pd, "equippedCharms");
            if (value is List<int> list)
            {
                list.Clear();
                list.AddRange(charmIds);
                return true;
            }

            if (value is int[] array)
            {
                Array.Clear(array, 0, array.Length);
                for (int i = 0; i < array.Length && i < charmIds.Count; i++)
                {
                    array[i] = charmIds[i];
                }
                return true;
            }

            if (value is IList listObj)
            {
                listObj.Clear();
                foreach (int charm in charmIds)
                {
                    listObj.Add(charm);
                }
                return true;
            }

            if (value is bool[] bools)
            {
                Array.Clear(bools, 0, bools.Length);
                foreach (int charm in charmIds)
                {
                    int index = charm - 1;
                    if (index >= 0 && index < bools.Length)
                    {
                        bools[index] = true;
                    }
                }
                return true;
            }
        }
        catch
        {
            // ignore charm errors
        }

        return false;
    }

    private static void RemoveCharms(PlayerData pd)
    {
        List<int> equippedCharms = GetEquippedCharms();
        foreach (int idCharm in equippedCharms)
        {
            if (pd.royalCharmState == 4 && idCharm == 36)
            {
                continue;
            }

            pd.SetBoolInternal($"equippedCharm_{idCharm}", false);
            pd.UnequipCharm(idCharm);
        }

        RemovePaleCourtCharms(pd);
        pd.overcharmed = false;
        pd.CalculateNotchesUsed();
        HeroController.instance?.CharmUpdate();
    }

    private static void RemovePaleCourtCharms(PlayerData pd)
    {
        foreach (int charmId in PaleCourtCharmIds)
        {
            string charmKey = $"equippedCharm_{charmId}";
            if (!pd.GetBool(charmKey))
            {
                continue;
            }

            pd.SetBool(charmKey, false);
            pd.UnequipCharm(charmId);
        }
    }

    private static void EquipCharms(PlayerData pd, List<int> charms)
    {
        int usedCharmSlots = 0;
        for (int i = 0; i < charms.Count; i++)
        {
            int idCharm = charms[i];
            if (!pd.GetBoolInternal($"equippedCharm_{idCharm}"))
            {
                pd.SetBoolInternal($"equippedCharm_{idCharm}", true);
                pd.EquipCharm(idCharm);
                usedCharmSlots += pd.GetInt($"charmCost_{idCharm}");
            }

            if (pd.charmSlots <= usedCharmSlots)
            {
                if (i < charms.Count - 1)
                {
                    RemoveCharms(pd);
                }

                break;
            }
        }

        if (pd.charmSlots < usedCharmSlots)
        {
            pd.charmSlotsFilled = usedCharmSlots;
            pd.overcharmed = true;
        }

        pd.CalculateNotchesUsed();
        HeroController.instance?.CharmUpdate();
    }

    internal static void ApplyFreeCharms(bool isFree)
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        int multiplier = isFree ? 0 : 1;
        pd.SetInt("charmCost_1", 1 * multiplier);
        pd.SetInt("charmCost_2", 1 * multiplier);
        pd.SetInt("charmCost_3", 1 * multiplier);
        pd.SetInt("charmCost_4", 2 * multiplier);
        pd.SetInt("charmCost_5", 2 * multiplier);
        pd.SetInt("charmCost_6", 2 * multiplier);
        pd.SetInt("charmCost_7", 3 * multiplier);
        pd.SetInt("charmCost_8", 2 * multiplier);
        pd.SetInt("charmCost_9", 3 * multiplier);
        pd.SetInt("charmCost_10", 1 * multiplier);
        pd.SetInt("charmCost_11", 3 * multiplier);
        pd.SetInt("charmCost_12", 1 * multiplier);
        pd.SetInt("charmCost_13", 3 * multiplier);
        pd.SetInt("charmCost_14", 1 * multiplier);
        pd.SetInt("charmCost_15", 2 * multiplier);
        pd.SetInt("charmCost_16", 2 * multiplier);
        pd.SetInt("charmCost_17", 1 * multiplier);
        pd.SetInt("charmCost_18", 2 * multiplier);
        pd.SetInt("charmCost_19", 3 * multiplier);
        pd.SetInt("charmCost_20", 2 * multiplier);
        pd.SetInt("charmCost_21", 4 * multiplier);
        pd.SetInt("charmCost_22", 2 * multiplier);
        pd.SetInt("charmCost_23", 2 * multiplier);
        pd.SetInt("charmCost_24", 2 * multiplier);
        pd.SetInt("charmCost_25", 3 * multiplier);
        pd.SetInt("charmCost_26", 1 * multiplier);
        pd.SetInt("charmCost_27", 4 * multiplier);
        pd.SetInt("charmCost_28", 2 * multiplier);
        pd.SetInt("charmCost_29", 4 * multiplier);
        pd.SetInt("charmCost_30", 1 * multiplier);
        pd.SetInt("charmCost_31", 2 * multiplier);
        pd.SetInt("charmCost_32", 3 * multiplier);
        pd.SetInt("charmCost_33", 2 * multiplier);
        pd.SetInt("charmCost_34", 4 * multiplier);
        pd.SetInt("charmCost_35", 3 * multiplier);

        if (pd.royalCharmState == 4)
        {
            pd.SetInt("charmCost_36", 0);
        }
        else
        {
            pd.SetInt("charmCost_36", 5 * multiplier);
        }

        pd.SetInt("charmCost_37", 1 * multiplier);
        pd.SetInt("charmCost_38", 3 * multiplier);
        pd.SetInt("charmCost_39", 2 * multiplier);
        pd.SetInt("charmCost_40", 2 * multiplier);
    }

    private static List<int> GetEquippedCharms()
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return new List<int>();
        }

        try
        {
            object? value = ReflectionHelper.GetField<PlayerData, object>(pd, "equippedCharms");
            if (value is List<int> list)
            {
                return new List<int>(list);
            }

            if (value is int[] array)
            {
                return array.Where(id => id > 0).ToList();
            }

            if (value is bool[] bools)
            {
                List<int> charms = new();
                for (int i = 0; i < bools.Length; i++)
                {
                    if (bools[i])
                    {
                        charms.Add(i + 1);
                    }
                }

                return charms;
            }

            if (value is IList listObj)
            {
                List<int> charms = new();
                foreach (object obj in listObj)
                {
                    if (obj is int charm)
                    {
                        charms.Add(charm);
                    }
                }

                return charms;
            }
        }
        catch
        {
            // ignore charm errors
        }

        return new List<int>();
    }

    private static void TryRefreshHud()
    {
        try
        {
            GameObject? hud = Ref.GC?.hudCanvas?.gameObject;
            if (hud == null)
            {
                return;
            }

            if (!hud.activeInHierarchy)
            {
                hud.SetActive(true);
            }
            else
            {
                hud.SetActive(false);
                hud.SetActive(true);
            }

            Ref.GC?.soulOrbFSM?.SendEvent("MP GAIN SPA");
            Ref.GC?.soulVesselFSM?.SendEvent("MP RESERVE UP");
        }
        catch
        {
            // ignore HUD issues
        }
    }

    private static void TryRefreshNailDamage()
    {
        try
        {
            PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");
        }
        catch
        {
            // ignore nail refresh issues
        }
    }

    private static bool IsSafeToApply()
    {
        if (Ref.GM == null || Ref.GM.gameState != GameState.PLAYING)
        {
            return false;
        }

        if (Ref.GM.IsInSceneTransition)
        {
            return false;
        }

        HeroController? hero = Ref.HC;
        if (hero == null)
        {
            return false;
        }

        if (!hero.acceptingInput || hero.controlReqlinquished)
        {
            return false;
        }

        PlayerData? pd = PlayerData.instance;
        if (pd != null && pd.atBench)
        {
            return hero.acceptingInput && !hero.controlReqlinquished;
        }

        return true;
    }

    private static bool GetPresetBool(Dictionary<string, bool> values, string key)
    {
        if (values != null && values.TryGetValue(key, out bool value))
        {
            return value;
        }

        return false;
    }

    private static bool GetPresetShellBindingState(GearPreset preset)
    {
        if (preset.HasAllBindings)
        {
            return true;
        }

        return GetPresetBool(preset.Bindings, "ShellBinding");
    }

    private static int GetPresetInt(Dictionary<string, int> values, string key)
    {
        if (values != null && values.TryGetValue(key, out int value))
        {
            return value;
        }

        return 0;
    }

    private static void SetAbility(PlayerData pd, string fieldName, bool value)
    {
        try
        {
            ReflectionHelper.SetField(pd, fieldName, value);
        }
        catch
        {
            // ignore missing fields
        }
    }

    private static void SetPlayerDataInt(PlayerData pd, string fieldName, int value)
    {
        try
        {
            ReflectionHelper.SetField(pd, fieldName, value);
        }
        catch
        {
            // ignore missing fields
        }
    }
}
