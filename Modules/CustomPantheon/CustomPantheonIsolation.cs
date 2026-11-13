namespace GodhomeQoL.Modules.BossChallenge;

internal static class CustomPantheonIsolation
{
    private const string Suppressor = "Custom Pantheon";

    private static readonly HashSet<string> SuppressedCategories = new(StringComparer.Ordinal) { "BossChallenge", "CollectorPhases", "Cheats" };
    private static readonly HashSet<string> KeepLoaded = new(StringComparer.Ordinal) { "RandomPantheons", "TrueBossRush", "SegmentedP5" };

    private static readonly Dictionary<string, Type> HelperByScene = new(StringComparer.Ordinal)
    {
        ["GG_Gruz_Mother"] = typeof(GruzMotherP1Helper),
        ["GG_Gruz_Mother_V"] = typeof(GruzMotherHelper),
        ["GG_Vengefly"] = typeof(VengeflyKingP1Helper),
        ["GG_Vengefly_V"] = typeof(VengeflyKing),
        ["GG_Brooding_Mawlek"] = typeof(BroodingMawlekP1Helper),
        ["GG_Brooding_Mawlek_V"] = typeof(BroodingMawlekHelper),
        ["GG_False_Knight"] = typeof(FalseKnightHelper),
        ["GG_Failed_Champion"] = typeof(FailedChampionHelper),
        ["GG_Hornet_1"] = typeof(HornetProtectorHelper),
        ["GG_Hornet_2"] = typeof(HornetSentinelHelper),
        ["GG_Mega_Moss_Charger"] = typeof(MassiveMossChargerHelper),
        ["GG_Flukemarm"] = typeof(FlukemarmHelper),
        ["GG_Mantis_Lords"] = typeof(MantisLordHelper),
        ["GG_Mantis_Lords_V"] = typeof(SisterOfBattleHelper),
        ["GG_Oblobbles"] = typeof(OblobblesHelper),
        ["GG_Hive_Knight"] = typeof(HiveKnightHelper),
        ["GG_Broken_Vessel"] = typeof(BrokenVesselHelper),
        ["GG_Lost_Kin"] = typeof(LostKinHelper),
        ["GG_Nosk"] = typeof(NoskP2Helper),
        ["GG_Nosk_V"] = typeof(NoskHelper),
        ["GG_Nosk_Hornet"] = typeof(WingedNoskHelper),
        ["GG_Collector"] = typeof(CollectorPhases.CollectorPhases),
        ["GG_Collector_V"] = typeof(CollectorPhases.CollectorPhases),
        ["GG_God_Tamer"] = typeof(GodTamerHelper),
        ["GG_Crystal_Guardian"] = typeof(CrystalGuardianHelper),
        ["GG_Crystal_Guardian_2"] = typeof(EnragedGuardianHelper),
        ["GG_Uumuu"] = typeof(UumuuP3Helper),
        ["GG_Uumuu_V"] = typeof(UumuuHelper),
        ["GG_Traitor_Lord"] = typeof(TraitorLordHelper),
        ["GG_Grey_Prince_Zote"] = typeof(ZoteHelper),
        ["GG_Mage_Knight"] = typeof(SoulWarriorP1Helper),
        ["GG_Mage_Knight_V"] = typeof(SoulWarriorHelper),
        ["GG_Soul_Master"] = typeof(SoulMasterHelper),
        ["GG_Soul_Tyrant"] = typeof(SoulTyrantHelper),
        ["GG_Dung_Defender"] = typeof(DungDefenderHelper),
        ["GG_White_Defender"] = typeof(WhiteDefenderHelper),
        ["GG_Watcher_Knights"] = typeof(WatcherKnightHelper),
        ["GG_Ghost_No_Eyes"] = typeof(NoEyesP4Helper),
        ["GG_Ghost_No_Eyes_V"] = typeof(NoEyesHelper),
        ["GG_Ghost_Marmu"] = typeof(MarmuP2Helper),
        ["GG_Ghost_Marmu_V"] = typeof(MarmuHelper),
        ["GG_Ghost_Xero"] = typeof(XeroP2Helper),
        ["GG_Ghost_Xero_V"] = typeof(XeroHelper),
        ["GG_Ghost_Markoth"] = typeof(MarkothP4Helper),
        ["GG_Ghost_Markoth_V"] = typeof(MarkothHelper),
        ["GG_Ghost_Galien"] = typeof(GalienHelper),
        ["GG_Ghost_Gorb"] = typeof(GorbP1Helper),
        ["GG_Ghost_Gorb_V"] = typeof(GorbHelper),
        ["GG_Ghost_Hu"] = typeof(ElderHuHelper),
        ["GG_Nailmasters"] = typeof(OroMatoHelper),
        ["GG_Painter"] = typeof(PaintmasterSheoHelper),
        ["GG_Sly"] = typeof(NailsageSlyHelper),
        ["GG_Hollow_Knight"] = typeof(PureVesselHelper),
        ["GG_Grimm"] = typeof(TroupeMasterGrimmHelper),
        ["GG_Grimm_Nightmare"] = typeof(NightmareKingGrimmHelper),
        ["GG_Radiance"] = typeof(AbsoluteRadianceHelper)
    };

    private static readonly Dictionary<Type, (Dictionary<string, string> Values, bool Enabled)> originals = new();
    private static readonly Dictionary<Type, Dictionary<string, string>> defaults = new();
    private static int baseHandle;
    private static int helperHandle;
    private static string? allowedScene;

    internal static IReadOnlyDictionary<string, Type> HelpersByScene => HelperByScene;

    internal static bool IsActive => baseHandle != 0 || helperHandle != 0;

    internal static Type? GetHelperType(string? scene) =>
        HelperByScene.TryGetValue(scene ?? string.Empty, out Type? type) ? type : null;

    internal static bool IsAllowedEntry(string? nextScene) =>
        allowedScene != null && string.Equals(allowedScene, nextScene, StringComparison.Ordinal);

    internal static IEnumerable<FieldInfo> GetSettingFields(Type helper)
    {
        for (Type? type = helper; type != null && type != typeof(Module); type = type.BaseType)
        {
            foreach (FieldInfo field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
            {
                if (field.IsDefined(typeof(LocalSettingAttribute), false) || field.IsDefined(typeof(GlobalSettingAttribute), false))
                {
                    yield return field;
                }
            }
        }
    }

    internal static Dictionary<string, string> CaptureValues(Type helper)
    {
        Dictionary<string, string> values = new(StringComparer.Ordinal);
        foreach (FieldInfo field in GetSettingFields(helper))
        {
            try
            {
                values[field.Name] = JsonConvert.SerializeObject(field.GetValue(null));
            }
            catch (Exception ex)
            {
                LogSuppressed(ex, "CustomPantheonIsolation.cs/Capture");
            }
        }

        return values;
    }

    internal static void CaptureDefaults()
    {
        foreach (Type helper in HelperByScene.Values.Distinct())
        {
            if (!defaults.ContainsKey(helper))
            {
                defaults[helper] = CaptureValues(helper);
            }
        }
    }

    internal static Dictionary<string, string>? WithDefaults(Type helper, Dictionary<string, string>? values)
    {
        if (values == null || !defaults.TryGetValue(helper, out Dictionary<string, string>? fallback))
        {
            return values;
        }

        Dictionary<string, string> merged = new(fallback, StringComparer.Ordinal);
        foreach (KeyValuePair<string, string> pair in values)
        {
            merged[pair.Key] = pair.Value;
        }

        return merged;
    }

    internal static void ApplyValues(Type helper, Dictionary<string, string>? values)
    {
        if (values == null)
        {
            return;
        }

        foreach (FieldInfo field in GetSettingFields(helper))
        {
            if (!values.TryGetValue(field.Name, out string? json) || json == null)
            {
                continue;
            }

            try
            {
                field.SetValue(null, JsonConvert.DeserializeObject(json, field.FieldType));
            }
            catch (Exception ex)
            {
                LogSuppressed(ex, "CustomPantheonIsolation.cs/Apply");
            }
        }
    }

    internal static bool IsHelperEnabled(Type helper) =>
        ModuleManager.TryGetModule(helper, out Module? module) && module.Enabled;

    internal static void SetHelperEnabled(Type helper, bool value)
    {
        if (ModuleManager.TryGetModule(helper, out Module? module) && module.Enabled != value)
        {
            module.Enabled = value;
        }
    }

    private static Module[] GetHelperModules(Type? except) =>
        HelperByScene.Values
            .Distinct()
            .Where(type => type != except)
            .Select(type => ModuleManager.TryGetModule(type, out Module? module) ? module : null)
            .Where(module => module != null)
            .Cast<Module>()
            .ToArray();

    private static Module[] GetBaseModules()
    {
        HashSet<Type> helpers = new(HelperByScene.Values);
        return ModuleManager.Modules.Values
            .Where(module => SuppressedCategories.Contains(module.Category)
                && !KeepLoaded.Contains(module.Name)
                && !helpers.Contains(module.Type))
            .ToArray();
    }

    private static int Suppress(Module[] modules) =>
        modules.Length == 0 ? 0 : ModuleManager.SuppressModules(Suppressor, modules);

    private static void Cancel(ref int handle)
    {
        if (handle != 0)
        {
            ModuleManager.CancelSuppression(handle);
            handle = 0;
        }
    }

    internal static void Begin()
    {
        if (IsActive)
        {
            return;
        }

        originals.Clear();
        allowedScene = null;
        baseHandle = Suppress(GetBaseModules());
        helperHandle = Suppress(GetHelperModules(null));
    }

    internal static void PrepareFight(CustomPantheonFight? fight)
    {
        if (!IsActive)
        {
            return;
        }

        Type? helper = fight != null && fight.HasBossSettings ? GetHelperType(fight.Scene) : null;
        allowedScene = helper != null ? fight!.Scene : null;
        int next = Suppress(GetHelperModules(helper));
        if (helper != null && ModuleManager.TryGetModule(helper, out Module? module))
        {
            int reload = Suppress(new[] { module });
            Cancel(ref helperHandle);
            if (!originals.ContainsKey(helper))
            {
                originals[helper] = (CaptureValues(helper), module.Enabled);
            }

            ApplyValues(helper, WithDefaults(helper, fight!.BossSettings));
            SetHelperEnabled(helper, true);
            Cancel(ref reload);
        }
        else
        {
            Cancel(ref helperHandle);
        }

        helperHandle = next;
    }

    internal static void End()
    {
        allowedScene = null;
        if (!IsActive && originals.Count == 0)
        {
            return;
        }

        int hold = Suppress(GetHelperModules(null));
        foreach ((Type helper, (Dictionary<string, string> values, bool enabled)) in originals.ToArray())
        {
            ApplyValues(helper, values);
            SetHelperEnabled(helper, enabled);
        }

        originals.Clear();
        Cancel(ref helperHandle);
        Cancel(ref hold);
        Cancel(ref baseHandle);
    }
}
