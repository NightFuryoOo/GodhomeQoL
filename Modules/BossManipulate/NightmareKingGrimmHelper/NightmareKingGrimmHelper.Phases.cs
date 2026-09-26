using HutongGames.PlayMaker;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class NightmareKingGrimmHelper : Module
{
    private const string RagePhase1VariableName = "Rage HP 1";
    private const string RagePhase2VariableName = "Rage HP 2";
    private const string RagePhase3VariableName = "Rage HP 3";
    private const int DefaultNightmareKingGrimmRagePhase1Hp = 1238;
    private const int DefaultNightmareKingGrimmRagePhase2Hp = 826;
    private const int DefaultNightmareKingGrimmRagePhase3Hp = 414;
    private const int MinNightmareKingGrimmRagePhase1Hp = 3;
    private const int MinNightmareKingGrimmRagePhase2Hp = 2;
    private const int MinNightmareKingGrimmRagePhase3Hp = 1;

    internal static void ApplyPhaseThresholdSettingsIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsNightmareKingGrimmPhaseControlFsm(fsm))
            {
                continue;
            }

            ApplyPhaseThresholdSettings(fsm);
        }
    }

    internal static void RestoreVanillaPhaseThresholdsIfPresent()
    {
        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsNightmareKingGrimmPhaseControlFsm(fsm))
            {
                continue;
            }

            if (!ShouldApplySettings(fsm.gameObject))
            {
                continue;
            }

            (int ragePhase1Hp, int ragePhase2Hp, int ragePhase3Hp) = GetVanillaRageThresholds(fsm.gameObject);
            SetRageThresholdsOnFsm(fsm, ragePhase1Hp, ragePhase2Hp, ragePhase3Hp);
        }
    }

    private static void ApplyPhaseThresholdSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsNightmareKingGrimmPhaseControlFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        (int ragePhase1Hp, int ragePhase2Hp, int ragePhase3Hp) thresholds = ShouldUseCustomPhaseThresholds()
            ? (
                ClampNightmareKingGrimmRagePhase1Hp(nightmareKingGrimmRagePhase1Hp),
                ClampNightmareKingGrimmRagePhase2Hp(nightmareKingGrimmRagePhase2Hp, nightmareKingGrimmRagePhase1Hp),
                ClampNightmareKingGrimmRagePhase3Hp(nightmareKingGrimmRagePhase3Hp, nightmareKingGrimmRagePhase2Hp)
            )
            : GetVanillaRageThresholds(fsm.gameObject);

        SetRageThresholdsOnFsm(fsm, thresholds.ragePhase1Hp, thresholds.ragePhase2Hp, thresholds.ragePhase3Hp);
    }

    private static void SetRageThresholdsOnFsm(PlayMakerFSM fsm, int ragePhase1Hp, int ragePhase2Hp, int ragePhase3Hp)
    {
        int targetRagePhase1Hp = ClampNightmareKingGrimmRagePhase1Hp(ragePhase1Hp);
        int targetRagePhase2Hp = ClampNightmareKingGrimmRagePhase2Hp(ragePhase2Hp, targetRagePhase1Hp);
        int targetRagePhase3Hp = ClampNightmareKingGrimmRagePhase3Hp(ragePhase3Hp, targetRagePhase2Hp);

        FsmInt? ragePhase1Variable = fsm.FsmVariables.GetFsmInt(RagePhase1VariableName);
        if (ragePhase1Variable != null)
        {
            ragePhase1Variable.Value = targetRagePhase1Hp;
        }

        FsmInt? ragePhase2Variable = fsm.FsmVariables.GetFsmInt(RagePhase2VariableName);
        if (ragePhase2Variable != null)
        {
            ragePhase2Variable.Value = targetRagePhase2Hp;
        }

        FsmInt? ragePhase3Variable = fsm.FsmVariables.GetFsmInt(RagePhase3VariableName);
        if (ragePhase3Variable != null)
        {
            ragePhase3Variable.Value = targetRagePhase3Hp;
        }
    }

    private static (int ragePhase1Hp, int ragePhase2Hp, int ragePhase3Hp) GetVanillaRageThresholds(GameObject bossObject)
    {
        int referenceHp = GetPhaseReferenceHp(bossObject);
        int quarterHp = referenceHp / 4;

        int ragePhase1Hp = referenceHp - quarterHp;
        int ragePhase2Hp = ragePhase1Hp - quarterHp;
        int ragePhase3Hp = ragePhase2Hp - quarterHp;

        int clampedRagePhase1Hp = ClampNightmareKingGrimmRagePhase1Hp(ragePhase1Hp);
        int clampedRagePhase2Hp = ClampNightmareKingGrimmRagePhase2Hp(ragePhase2Hp, clampedRagePhase1Hp);
        int clampedRagePhase3Hp = ClampNightmareKingGrimmRagePhase3Hp(ragePhase3Hp, clampedRagePhase2Hp);
        return (clampedRagePhase1Hp, clampedRagePhase2Hp, clampedRagePhase3Hp);
    }

    private static int GetPhaseReferenceHp(GameObject bossObject)
    {
        if (bossObject == null)
        {
            return DefaultNightmareKingGrimmVanillaHp;
        }

        HealthManager? hm = bossObject.GetComponent<HealthManager>();
        if (hm != null)
        {
            int maxHp = hm.hp;
            if (maxHp > 0)
            {
                return ClampNightmareKingGrimmHp(maxHp);
            }

            if (hm.hp > 0)
            {
                return ClampNightmareKingGrimmHp(hm.hp);
            }
        }

        int configuredHp = ClampNightmareKingGrimmHp(nightmareKingGrimmMaxHp);
        return configuredHp > 0 ? configuredHp : DefaultNightmareKingGrimmVanillaHp;
    }

    private static int ClampNightmareKingGrimmRagePhase1Hp(int value)
    {
        int maxRagePhase1Hp = ResolveRagePhase1MaxHp();

        if (value < MinNightmareKingGrimmRagePhase1Hp)
        {
            return MinNightmareKingGrimmRagePhase1Hp;
        }

        return value > maxRagePhase1Hp ? maxRagePhase1Hp : value;
    }

    private static int ResolveRagePhase1MaxHp()
    {
        return ShouldUseCustomHp()
            ? ClampNightmareKingGrimmHp(nightmareKingGrimmMaxHp)
            : DefaultNightmareKingGrimmVanillaHp;
    }

    internal static int GetRagePhase1MaxHpForUi()
    {
        return ResolveRagePhase1MaxHp();
    }

    internal static int GetRagePhase2MaxHpForUi()
    {
        int ragePhase1Hp = ClampNightmareKingGrimmRagePhase1Hp(nightmareKingGrimmRagePhase1Hp);
        return Math.Max(MinNightmareKingGrimmRagePhase2Hp, ragePhase1Hp - 1);
    }

    internal static int GetRagePhase3MaxHpForUi()
    {
        int ragePhase2Hp = ClampNightmareKingGrimmRagePhase2Hp(nightmareKingGrimmRagePhase2Hp, nightmareKingGrimmRagePhase1Hp);
        return Math.Max(MinNightmareKingGrimmRagePhase3Hp, ragePhase2Hp - 1);
    }

    private static int ClampNightmareKingGrimmRagePhase2Hp(int value, int ragePhase1Hp)
    {
        int clampedRagePhase1Hp = ClampNightmareKingGrimmRagePhase1Hp(ragePhase1Hp);
        int maxRagePhase2Hp = Math.Max(MinNightmareKingGrimmRagePhase2Hp, clampedRagePhase1Hp - 1);

        if (value < MinNightmareKingGrimmRagePhase2Hp)
        {
            return MinNightmareKingGrimmRagePhase2Hp;
        }

        return value > maxRagePhase2Hp ? maxRagePhase2Hp : value;
    }

    private static int ClampNightmareKingGrimmRagePhase3Hp(int value, int ragePhase2Hp)
    {
        int clampedRagePhase2Hp = ragePhase2Hp < MinNightmareKingGrimmRagePhase2Hp
            ? MinNightmareKingGrimmRagePhase2Hp
            : ragePhase2Hp;
        int maxRagePhase3Hp = Math.Max(MinNightmareKingGrimmRagePhase3Hp, clampedRagePhase2Hp - 1);

        if (value < MinNightmareKingGrimmRagePhase3Hp)
        {
            return MinNightmareKingGrimmRagePhase3Hp;
        }

        return value > maxRagePhase3Hp ? maxRagePhase3Hp : value;
    }
}
