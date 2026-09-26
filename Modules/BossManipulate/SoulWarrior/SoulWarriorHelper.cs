using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class SoulWarriorHelper : Module
{
    private const string SoulWarriorScene = "GG_Mage_Knight_V";
    private const string SoulWarriorName = "Mage Knight";
    private static readonly string[] SoulWarriorSummonNameHints =
    {
        "Mage Balloon Spawner",
        "Mage Balloon",
        "Balloon Spawner",
        "Balloon",
    };
    private static readonly string[] SoulWarriorSummonStateHints =
    {
        "Count Summon",
        "Summon Choice",
        "Summon Start",
        "Idle or Summoned?",
    };
    private static readonly string[] SoulWarriorSummonCountVariableNames =
    {
        "Balls",
        "Balloons",
    };
    private const int DefaultSoulWarriorMaxHp = 1000;
    private const int P5SoulWarriorHp = 750;
    private const int MinSoulWarriorHp = 1;
    private const int MaxSoulWarriorHp = 999999;
    [LocalSetting]
    [BoolOption]
    internal static bool soulWarriorUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool soulWarriorP5Hp = false;

    [LocalSetting]
    internal static int soulWarriorMaxHp = DefaultSoulWarriorMaxHp;

    [LocalSetting]
    internal static int soulWarriorMaxHpBeforeP5 = DefaultSoulWarriorMaxHp;

    [LocalSetting]
    internal static bool soulWarriorUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool soulWarriorHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool soulWarriorUseCustomSummonHp = false;

    [LocalSetting]
    internal static int soulWarriorSummonHp = DefaultSoulWarriorSummonHp;

    [LocalSetting]
    internal static bool soulWarriorUseCustomSummonLimit = false;

    [LocalSetting]
    internal static int soulWarriorSummonLimit = DefaultSoulWarriorSummonLimit;

    [LocalSetting]
    internal static bool soulWarriorUseCustomSummonHpBeforeP5 = false;

    [LocalSetting]
    internal static int soulWarriorSummonHpBeforeP5 = DefaultSoulWarriorSummonHp;

    [LocalSetting]
    internal static bool soulWarriorUseCustomSummonLimitBeforeP5 = false;

    [LocalSetting]
    internal static int soulWarriorSummonLimitBeforeP5 = DefaultSoulWarriorSummonLimit;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static readonly Dictionary<int, int> vanillaSummonHpByInstance = new();
    private static readonly Dictionary<int, int> vanillaSummonLimitByFsm = new();
    private static readonly Dictionary<int, Dictionary<string, int>> vanillaSummonCountByFsm = new();
    private static readonly Dictionary<int, Dictionary<string, GameObject[]>> vanillaSummonPoolsByFsm = new();
    private static readonly Dictionary<int, List<GameObject>> createdSummonClonesByFsm = new();
    private static readonly List<GameObject> createdSceneSummonClones = [];
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    internal static void ReapplyLiveSettings()
    {
        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplySoulWarriorHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplySoulWarriorSummonHealthIfPresent();
        }
        else
        {
            RestoreVanillaSummonHealthIfPresent();
        }

        if (ShouldUseCustomSummonLimit())
        {
            ApplySummonLimitSettingsIfPresent();
        }
        else
        {
            RestoreVanillaSummonLimitsIfPresent();
        }
    }

    internal static void SetP5HpEnabled(bool value)
    {
        if (value)
        {
            if (!soulWarriorP5Hp)
            {
                soulWarriorMaxHpBeforeP5 = ClampSoulWarriorHp(soulWarriorMaxHp);
                soulWarriorUseMaxHpBeforeP5 = soulWarriorUseMaxHp;
                soulWarriorUseCustomSummonHpBeforeP5 = soulWarriorUseCustomSummonHp;
                soulWarriorSummonHpBeforeP5 = ClampSoulWarriorHp(soulWarriorSummonHp);
                soulWarriorUseCustomSummonLimitBeforeP5 = soulWarriorUseCustomSummonLimit;
                soulWarriorSummonLimitBeforeP5 = ClampSoulWarriorSummonLimit(soulWarriorSummonLimit);
                soulWarriorHasStoredStateBeforeP5 = true;
            }

            soulWarriorP5Hp = true;
            soulWarriorUseMaxHp = true;
            soulWarriorUseCustomSummonHp = false;
            soulWarriorUseCustomSummonLimit = false;
            soulWarriorMaxHp = P5SoulWarriorHp;
        }
        else
        {
            if (soulWarriorP5Hp && soulWarriorHasStoredStateBeforeP5)
            {
                soulWarriorMaxHp = ClampSoulWarriorHp(soulWarriorMaxHpBeforeP5);
                soulWarriorUseMaxHp = soulWarriorUseMaxHpBeforeP5;
                soulWarriorUseCustomSummonHp = soulWarriorUseCustomSummonHpBeforeP5;
                soulWarriorSummonHp = ClampSoulWarriorHp(soulWarriorSummonHpBeforeP5);
                soulWarriorUseCustomSummonLimit = soulWarriorUseCustomSummonLimitBeforeP5;
                soulWarriorSummonLimit = ClampSoulWarriorSummonLimit(soulWarriorSummonLimitBeforeP5);
            }

            soulWarriorP5Hp = false;
            soulWarriorHasStoredStateBeforeP5 = false;
        }

        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        soulWarriorSummonHp = ClampSoulWarriorHp(soulWarriorSummonHp);
        soulWarriorSummonLimit = ClampSoulWarriorSummonLimit(soulWarriorSummonLimit);
        if (!soulWarriorP5Hp)
        {
            return;
        }

        if (!soulWarriorHasStoredStateBeforeP5)
        {
            soulWarriorMaxHpBeforeP5 = ClampSoulWarriorHp(soulWarriorMaxHp);
            soulWarriorUseMaxHpBeforeP5 = soulWarriorUseMaxHp;
            soulWarriorUseCustomSummonHpBeforeP5 = soulWarriorUseCustomSummonHp;
            soulWarriorSummonHpBeforeP5 = ClampSoulWarriorHp(soulWarriorSummonHp);
            soulWarriorUseCustomSummonLimitBeforeP5 = soulWarriorUseCustomSummonLimit;
            soulWarriorSummonLimitBeforeP5 = ClampSoulWarriorSummonLimit(soulWarriorSummonLimit);
            soulWarriorHasStoredStateBeforeP5 = true;
        }

        soulWarriorUseMaxHp = true;
        soulWarriorUseCustomSummonHp = false;
        soulWarriorUseCustomSummonLimit = false;
        soulWarriorMaxHp = P5SoulWarriorHp;
    }

    internal static void ApplySoulWarriorHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindSoulWarriorHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplySoulWarriorHealth(hm.gameObject, hm);
        }
    }
    private static List<(string VariableName, FsmArray Array, List<GameObject> Entries)> FindSummonPoolVariables(PlayMakerFSM fsm)
    {
        List<(string VariableName, FsmArray Array, List<GameObject> Entries)> result = [];
        if (fsm?.FsmVariables?.ArrayVariables == null)
        {
            return result;
        }

        foreach (FsmArray array in fsm.FsmVariables.ArrayVariables)
        {
            if (array == null || array.Values == null || array.Values.Length == 0)
            {
                continue;
            }

            List<GameObject> entries = [];
            foreach (object value in array.Values)
            {
                if (value is not GameObject go || go == null || !IsSoulWarriorSummonObject(go))
                {
                    continue;
                }

                entries.Add(go);
            }

            if (entries.Count == 0)
            {
                continue;
            }

            result.Add((array.Name, array, entries));
        }

        return result;
    }
}
