using HutongGames.PlayMaker;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class NightmareKingGrimmHelper : Module
{
    internal static void ReapplyLiveSettings()
    {
        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyNightmareKingGrimmHealthIfPresent();
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
            if (!nightmareKingGrimmP5Hp)
            {
                nightmareKingGrimmMaxHpBeforeP5 = ClampNightmareKingGrimmHp(nightmareKingGrimmMaxHp);
                nightmareKingGrimmUseMaxHpBeforeP5 = nightmareKingGrimmUseMaxHp;
                nightmareKingGrimmUseCustomPhaseBeforeP5 = nightmareKingGrimmUseCustomPhase;
                nightmareKingGrimmRagePhase1HpBeforeP5 = ClampNightmareKingGrimmRagePhase1Hp(nightmareKingGrimmRagePhase1Hp);
                nightmareKingGrimmRagePhase2HpBeforeP5 = ClampNightmareKingGrimmRagePhase2Hp(nightmareKingGrimmRagePhase2Hp, nightmareKingGrimmRagePhase1Hp);
                nightmareKingGrimmRagePhase3HpBeforeP5 = ClampNightmareKingGrimmRagePhase3Hp(nightmareKingGrimmRagePhase3Hp, nightmareKingGrimmRagePhase2Hp);
                nightmareKingGrimmHasStoredStateBeforeP5 = true;
            }

            nightmareKingGrimmP5Hp = true;
            nightmareKingGrimmUseMaxHp = true;
            nightmareKingGrimmUseCustomPhase = false;
            nightmareKingGrimmMaxHp = P5NightmareKingGrimmHp;
        }
        else
        {
            if (nightmareKingGrimmP5Hp && nightmareKingGrimmHasStoredStateBeforeP5)
            {
                nightmareKingGrimmMaxHp = ClampNightmareKingGrimmHp(nightmareKingGrimmMaxHpBeforeP5);
                nightmareKingGrimmUseMaxHp = nightmareKingGrimmUseMaxHpBeforeP5;
                nightmareKingGrimmUseCustomPhase = nightmareKingGrimmUseCustomPhaseBeforeP5;
                nightmareKingGrimmRagePhase1Hp = ClampNightmareKingGrimmRagePhase1Hp(nightmareKingGrimmRagePhase1HpBeforeP5);
                nightmareKingGrimmRagePhase2Hp = ClampNightmareKingGrimmRagePhase2Hp(nightmareKingGrimmRagePhase2HpBeforeP5, nightmareKingGrimmRagePhase1Hp);
                nightmareKingGrimmRagePhase3Hp = ClampNightmareKingGrimmRagePhase3Hp(nightmareKingGrimmRagePhase3HpBeforeP5, nightmareKingGrimmRagePhase2Hp);
            }

            nightmareKingGrimmP5Hp = false;
            nightmareKingGrimmHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!nightmareKingGrimmP5Hp)
        {
            return;
        }

        if (!nightmareKingGrimmHasStoredStateBeforeP5)
        {
            nightmareKingGrimmMaxHpBeforeP5 = ClampNightmareKingGrimmHp(nightmareKingGrimmMaxHp);
            nightmareKingGrimmUseMaxHpBeforeP5 = nightmareKingGrimmUseMaxHp;
            nightmareKingGrimmUseCustomPhaseBeforeP5 = nightmareKingGrimmUseCustomPhase;
            nightmareKingGrimmRagePhase1HpBeforeP5 = ClampNightmareKingGrimmRagePhase1Hp(nightmareKingGrimmRagePhase1Hp);
            nightmareKingGrimmRagePhase2HpBeforeP5 = ClampNightmareKingGrimmRagePhase2Hp(nightmareKingGrimmRagePhase2Hp, nightmareKingGrimmRagePhase1Hp);
            nightmareKingGrimmRagePhase3HpBeforeP5 = ClampNightmareKingGrimmRagePhase3Hp(nightmareKingGrimmRagePhase3Hp, nightmareKingGrimmRagePhase2Hp);
            nightmareKingGrimmHasStoredStateBeforeP5 = true;
        }

        nightmareKingGrimmUseMaxHp = true;
        nightmareKingGrimmUseCustomPhase = false;
        nightmareKingGrimmMaxHp = P5NightmareKingGrimmHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        nightmareKingGrimmRagePhase1Hp = ClampNightmareKingGrimmRagePhase1Hp(nightmareKingGrimmRagePhase1Hp);
        nightmareKingGrimmRagePhase2Hp = ClampNightmareKingGrimmRagePhase2Hp(nightmareKingGrimmRagePhase2Hp, nightmareKingGrimmRagePhase1Hp);
        nightmareKingGrimmRagePhase3Hp = ClampNightmareKingGrimmRagePhase3Hp(nightmareKingGrimmRagePhase3Hp, nightmareKingGrimmRagePhase2Hp);
    }
}
