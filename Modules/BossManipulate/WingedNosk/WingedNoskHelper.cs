using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class WingedNoskHelper : Module
{
    private const string WingedNoskScene = "GG_Nosk_Hornet";
    private const string WingedNoskName = "Hornet Nosk";
    private static readonly string[] WingedNoskSummonNameHints =
    {
        "Parasite Balloon Spawner",
        "Parasite Balloon",
        "Buzzer",
    };
    private const string WingedNoskHalfHpVariableName = "Half HP";
    private const string WingedNoskHpVariableName = "HP";
    private const string WingedNoskEnemyCountVariableName = "Enemy Count";
    private const int DefaultWingedNoskMaxHp = 1050;
    private const int P5WingedNoskHp = 750;
    private const int MinWingedNoskHp = 1;
    private const int MaxWingedNoskHp = 999999;
    [LocalSetting]
    [BoolOption]
    internal static bool wingedNoskUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool wingedNoskP5Hp = false;

    [LocalSetting]
    internal static int wingedNoskMaxHp = DefaultWingedNoskMaxHp;

    [LocalSetting]
    internal static int wingedNoskMaxHpBeforeP5 = DefaultWingedNoskMaxHp;

    [LocalSetting]
    internal static bool wingedNoskUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool wingedNoskHasStoredStateBeforeP5 = false;

    [LocalSetting]
    internal static bool wingedNoskUseCustomPhase = false;

    [LocalSetting]
    internal static int wingedNoskPhase2Hp = DefaultWingedNoskPhase2Hp;

    [LocalSetting]
    internal static bool wingedNoskUseCustomSummonHp = false;

    [LocalSetting]
    internal static int wingedNoskSummonHp = DefaultWingedNoskSummonHp;

    [LocalSetting]
    internal static bool wingedNoskUseCustomSummonLimit = false;

    [LocalSetting]
    internal static int wingedNoskSummonLimit = DefaultWingedNoskSummonLimit;

    [LocalSetting]
    internal static bool wingedNoskUseCustomPhaseBeforeP5 = false;

    [LocalSetting]
    internal static int wingedNoskPhase2HpBeforeP5 = DefaultWingedNoskPhase2Hp;

    [LocalSetting]
    internal static bool wingedNoskUseCustomSummonHpBeforeP5 = false;

    [LocalSetting]
    internal static int wingedNoskSummonHpBeforeP5 = DefaultWingedNoskSummonHp;

    [LocalSetting]
    internal static bool wingedNoskUseCustomSummonLimitBeforeP5 = false;

    [LocalSetting]
    internal static int wingedNoskSummonLimitBeforeP5 = DefaultWingedNoskSummonLimit;

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static readonly Dictionary<int, int> vanillaSummonHpByInstance = new();
    private static readonly Dictionary<int, int> vanillaSummonLimitByFsm = new();
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
            ApplyWingedNoskHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplyWingedNoskSummonHealthIfPresent();
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

        ApplyPhaseThresholdSettingsIfPresent();
    }

    internal static void SetP5HpEnabled(bool value)
    {
        if (value)
        {
            if (!wingedNoskP5Hp)
            {
                wingedNoskMaxHpBeforeP5 = ClampWingedNoskHp(wingedNoskMaxHp);
                wingedNoskUseMaxHpBeforeP5 = wingedNoskUseMaxHp;
                wingedNoskUseCustomPhaseBeforeP5 = wingedNoskUseCustomPhase;
                wingedNoskPhase2HpBeforeP5 = ClampWingedNoskPhase2Hp(wingedNoskPhase2Hp, ResolvePhase2MaxHp());
                wingedNoskUseCustomSummonHpBeforeP5 = wingedNoskUseCustomSummonHp;
                wingedNoskSummonHpBeforeP5 = ClampWingedNoskHp(wingedNoskSummonHp);
                wingedNoskUseCustomSummonLimitBeforeP5 = wingedNoskUseCustomSummonLimit;
                wingedNoskSummonLimitBeforeP5 = ClampWingedNoskSummonLimit(wingedNoskSummonLimit);
                wingedNoskHasStoredStateBeforeP5 = true;
            }

            wingedNoskP5Hp = true;
            wingedNoskUseMaxHp = true;
            wingedNoskUseCustomPhase = false;
            wingedNoskUseCustomSummonHp = false;
            wingedNoskUseCustomSummonLimit = false;
            wingedNoskMaxHp = P5WingedNoskHp;
        }
        else
        {
            if (wingedNoskP5Hp && wingedNoskHasStoredStateBeforeP5)
            {
                wingedNoskMaxHp = ClampWingedNoskHp(wingedNoskMaxHpBeforeP5);
                wingedNoskUseMaxHp = wingedNoskUseMaxHpBeforeP5;
                wingedNoskUseCustomPhase = wingedNoskUseCustomPhaseBeforeP5;
                wingedNoskPhase2Hp = ClampWingedNoskPhase2Hp(wingedNoskPhase2HpBeforeP5, ResolvePhase2MaxHp());
                wingedNoskUseCustomSummonHp = wingedNoskUseCustomSummonHpBeforeP5;
                wingedNoskSummonHp = ClampWingedNoskHp(wingedNoskSummonHpBeforeP5);
                wingedNoskUseCustomSummonLimit = wingedNoskUseCustomSummonLimitBeforeP5;
                wingedNoskSummonLimit = ClampWingedNoskSummonLimit(wingedNoskSummonLimitBeforeP5);
            }

            wingedNoskP5Hp = false;
            wingedNoskHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!wingedNoskP5Hp)
        {
            return;
        }

        if (!wingedNoskHasStoredStateBeforeP5)
        {
            wingedNoskMaxHpBeforeP5 = ClampWingedNoskHp(wingedNoskMaxHp);
            wingedNoskUseMaxHpBeforeP5 = wingedNoskUseMaxHp;
            wingedNoskUseCustomPhaseBeforeP5 = wingedNoskUseCustomPhase;
            wingedNoskPhase2HpBeforeP5 = ClampWingedNoskPhase2Hp(wingedNoskPhase2Hp, ResolvePhase2MaxHp());
            wingedNoskUseCustomSummonHpBeforeP5 = wingedNoskUseCustomSummonHp;
            wingedNoskSummonHpBeforeP5 = ClampWingedNoskHp(wingedNoskSummonHp);
            wingedNoskUseCustomSummonLimitBeforeP5 = wingedNoskUseCustomSummonLimit;
            wingedNoskSummonLimitBeforeP5 = ClampWingedNoskSummonLimit(wingedNoskSummonLimit);
            wingedNoskHasStoredStateBeforeP5 = true;
        }

        wingedNoskUseMaxHp = true;
        wingedNoskUseCustomPhase = false;
        wingedNoskUseCustomSummonHp = false;
        wingedNoskUseCustomSummonLimit = false;
        wingedNoskMaxHp = P5WingedNoskHp;
        NormalizePhaseThresholdState();
    }

    internal static void ApplyWingedNoskHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        if (!TryFindWingedNoskHealthManager(out HealthManager? hm))
        {
            return;
        }

        if (hm != null && hm.gameObject != null)
        {
            ApplyWingedNoskHealth(hm.gameObject, hm);
        }
    }
}
