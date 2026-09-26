using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class TroupeMasterGrimmHelper : Module
{
    internal static void ReapplyLiveSettings()
    {
        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyTroupeMasterGrimmHealthIfPresent();
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
            if (!troupeMasterGrimmP5Hp)
            {
                int referenceHp = GetPhaseReferenceHp(null);
                troupeMasterGrimmMaxHpBeforeP5 = ClampTroupeMasterGrimmHp(troupeMasterGrimmMaxHp);
                troupeMasterGrimmUseMaxHpBeforeP5 = troupeMasterGrimmUseMaxHp;
                troupeMasterGrimmUseCustomPhaseBeforeP5 = troupeMasterGrimmUseCustomPhase;
                troupeMasterGrimmPhase2HpBeforeP5 = ClampTroupeMasterGrimmPhase2Hp(troupeMasterGrimmPhase2Hp, referenceHp);
                troupeMasterGrimmPhase3HpBeforeP5 = ClampTroupeMasterGrimmPhase3Hp(troupeMasterGrimmPhase3Hp, troupeMasterGrimmPhase2HpBeforeP5);
                troupeMasterGrimmPhase4HpBeforeP5 = ClampTroupeMasterGrimmPhase4Hp(troupeMasterGrimmPhase4Hp, troupeMasterGrimmPhase3HpBeforeP5);
                troupeMasterGrimmHasStoredStateBeforeP5 = true;
            }

            troupeMasterGrimmP5Hp = true;
            troupeMasterGrimmUseMaxHp = true;
            troupeMasterGrimmUseCustomPhase = false;
            troupeMasterGrimmMaxHp = P5TroupeMasterGrimmHp;
        }
        else
        {
            if (troupeMasterGrimmP5Hp && troupeMasterGrimmHasStoredStateBeforeP5)
            {
                troupeMasterGrimmMaxHp = ClampTroupeMasterGrimmHp(troupeMasterGrimmMaxHpBeforeP5);
                troupeMasterGrimmUseMaxHp = troupeMasterGrimmUseMaxHpBeforeP5;
                troupeMasterGrimmUseCustomPhase = troupeMasterGrimmUseCustomPhaseBeforeP5;
                troupeMasterGrimmPhase2Hp = troupeMasterGrimmPhase2HpBeforeP5;
                troupeMasterGrimmPhase3Hp = troupeMasterGrimmPhase3HpBeforeP5;
                troupeMasterGrimmPhase4Hp = troupeMasterGrimmPhase4HpBeforeP5;
            }

            troupeMasterGrimmP5Hp = false;
            troupeMasterGrimmHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!troupeMasterGrimmP5Hp)
        {
            return;
        }

        if (!troupeMasterGrimmHasStoredStateBeforeP5)
        {
            int referenceHp = GetPhaseReferenceHp(null);
            troupeMasterGrimmMaxHpBeforeP5 = ClampTroupeMasterGrimmHp(troupeMasterGrimmMaxHp);
            troupeMasterGrimmUseMaxHpBeforeP5 = troupeMasterGrimmUseMaxHp;
            troupeMasterGrimmUseCustomPhaseBeforeP5 = troupeMasterGrimmUseCustomPhase;
            troupeMasterGrimmPhase2HpBeforeP5 = ClampTroupeMasterGrimmPhase2Hp(troupeMasterGrimmPhase2Hp, referenceHp);
            troupeMasterGrimmPhase3HpBeforeP5 = ClampTroupeMasterGrimmPhase3Hp(troupeMasterGrimmPhase3Hp, troupeMasterGrimmPhase2HpBeforeP5);
            troupeMasterGrimmPhase4HpBeforeP5 = ClampTroupeMasterGrimmPhase4Hp(troupeMasterGrimmPhase4Hp, troupeMasterGrimmPhase3HpBeforeP5);
            troupeMasterGrimmHasStoredStateBeforeP5 = true;
        }

        troupeMasterGrimmUseMaxHp = true;
        troupeMasterGrimmUseCustomPhase = false;
        troupeMasterGrimmMaxHp = P5TroupeMasterGrimmHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        int referenceHp = GetPhaseReferenceHp(null);
        troupeMasterGrimmPhase2Hp = ClampTroupeMasterGrimmPhase2Hp(troupeMasterGrimmPhase2Hp, referenceHp);
        troupeMasterGrimmPhase3Hp = ClampTroupeMasterGrimmPhase3Hp(troupeMasterGrimmPhase3Hp, troupeMasterGrimmPhase2Hp);
        troupeMasterGrimmPhase4Hp = ClampTroupeMasterGrimmPhase4Hp(troupeMasterGrimmPhase4Hp, troupeMasterGrimmPhase3Hp);
    }
}
