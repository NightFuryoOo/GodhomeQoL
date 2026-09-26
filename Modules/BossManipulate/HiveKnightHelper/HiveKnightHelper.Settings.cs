using Modding;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class HiveKnightHelper : Module
{
    internal static void ReapplyLiveSettings()
    {
        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyHiveKnightHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        ApplyPhaseThresholdSettingsIfPresent();
    }

    internal static void SetP5HpEnabled(bool value)
    {
        if (value)
        {
            if (!hiveKnightP5Hp)
            {
                hiveKnightMaxHpBeforeP5 = ClampHiveKnightHp(hiveKnightMaxHp);
                hiveKnightUseMaxHpBeforeP5 = hiveKnightUseMaxHp;
                hiveKnightUseCustomPhaseBeforeP5 = hiveKnightUseCustomPhase;
                hiveKnightPhase2HpBeforeP5 = ClampHiveKnightPhase2Hp(hiveKnightPhase2Hp);
                hiveKnightPhase3HpBeforeP5 = ClampHiveKnightPhase3Hp(hiveKnightPhase3Hp, hiveKnightPhase2Hp);
                hiveKnightHasStoredStateBeforeP5 = true;
            }

            hiveKnightP5Hp = true;
            hiveKnightUseMaxHp = true;
            hiveKnightUseCustomPhase = false;
            hiveKnightMaxHp = P5HiveKnightHp;
        }
        else
        {
            if (hiveKnightP5Hp && hiveKnightHasStoredStateBeforeP5)
            {
                hiveKnightMaxHp = ClampHiveKnightHp(hiveKnightMaxHpBeforeP5);
                hiveKnightUseMaxHp = hiveKnightUseMaxHpBeforeP5;
                hiveKnightUseCustomPhase = hiveKnightUseCustomPhaseBeforeP5;
                hiveKnightPhase2Hp = ClampHiveKnightPhase2Hp(hiveKnightPhase2HpBeforeP5);
                hiveKnightPhase3Hp = ClampHiveKnightPhase3Hp(hiveKnightPhase3HpBeforeP5, hiveKnightPhase2Hp);
            }

            hiveKnightP5Hp = false;
            hiveKnightHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!hiveKnightP5Hp)
        {
            return;
        }

        if (!hiveKnightHasStoredStateBeforeP5)
        {
            hiveKnightMaxHpBeforeP5 = ClampHiveKnightHp(hiveKnightMaxHp);
            hiveKnightUseMaxHpBeforeP5 = hiveKnightUseMaxHp;
            hiveKnightUseCustomPhaseBeforeP5 = hiveKnightUseCustomPhase;
            hiveKnightPhase2HpBeforeP5 = ClampHiveKnightPhase2Hp(hiveKnightPhase2Hp);
            hiveKnightPhase3HpBeforeP5 = ClampHiveKnightPhase3Hp(hiveKnightPhase3Hp, hiveKnightPhase2Hp);
            hiveKnightHasStoredStateBeforeP5 = true;
        }

        hiveKnightUseMaxHp = true;
        hiveKnightUseCustomPhase = false;
        hiveKnightMaxHp = P5HiveKnightHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        hiveKnightPhase2Hp = ClampHiveKnightPhase2Hp(hiveKnightPhase2Hp);
        hiveKnightPhase3Hp = ClampHiveKnightPhase3Hp(hiveKnightPhase3Hp, hiveKnightPhase2Hp);
    }
}
