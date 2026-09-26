using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class PureVesselHelper : Module
{
    internal static void ReapplyLiveSettings()
    {
        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyPureVesselHealthIfPresent();
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
            if (!pureVesselP5Hp)
            {
                pureVesselMaxHpBeforeP5 = ClampPureVesselHp(pureVesselMaxHp);
                pureVesselUseMaxHpBeforeP5 = pureVesselUseMaxHp;
                pureVesselUseCustomPhaseBeforeP5 = pureVesselUseCustomPhase;
                pureVesselPhase2HpBeforeP5 = ClampPureVesselPhase2Hp(pureVesselPhase2Hp);
                pureVesselPhase3HpBeforeP5 = ClampPureVesselPhase3Hp(pureVesselPhase3Hp, pureVesselPhase2Hp);
                pureVesselHasStoredStateBeforeP5 = true;
            }

            pureVesselP5Hp = true;
            pureVesselUseMaxHp = true;
            pureVesselUseCustomPhase = false;
            pureVesselMaxHp = P5PureVesselHp;
        }
        else
        {
            if (pureVesselP5Hp && pureVesselHasStoredStateBeforeP5)
            {
                pureVesselMaxHp = ClampPureVesselHp(pureVesselMaxHpBeforeP5);
                pureVesselUseMaxHp = pureVesselUseMaxHpBeforeP5;
                pureVesselUseCustomPhase = pureVesselUseCustomPhaseBeforeP5;
                pureVesselPhase2Hp = ClampPureVesselPhase2Hp(pureVesselPhase2HpBeforeP5);
                pureVesselPhase3Hp = ClampPureVesselPhase3Hp(pureVesselPhase3HpBeforeP5, pureVesselPhase2Hp);
            }

            pureVesselP5Hp = false;
            pureVesselHasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!pureVesselP5Hp)
        {
            return;
        }

        if (!pureVesselHasStoredStateBeforeP5)
        {
            pureVesselMaxHpBeforeP5 = ClampPureVesselHp(pureVesselMaxHp);
            pureVesselUseMaxHpBeforeP5 = pureVesselUseMaxHp;
            pureVesselUseCustomPhaseBeforeP5 = pureVesselUseCustomPhase;
            pureVesselPhase2HpBeforeP5 = ClampPureVesselPhase2Hp(pureVesselPhase2Hp);
            pureVesselPhase3HpBeforeP5 = ClampPureVesselPhase3Hp(pureVesselPhase3Hp, pureVesselPhase2Hp);
            pureVesselHasStoredStateBeforeP5 = true;
        }

        pureVesselUseMaxHp = true;
        pureVesselUseCustomPhase = false;
        pureVesselMaxHp = P5PureVesselHp;
        NormalizePhaseThresholdState();
    }

    private static void NormalizePhaseThresholdState()
    {
        pureVesselPhase2Hp = ClampPureVesselPhase2Hp(pureVesselPhase2Hp);
        pureVesselPhase3Hp = ClampPureVesselPhase3Hp(pureVesselPhase3Hp, pureVesselPhase2Hp);
    }
}
