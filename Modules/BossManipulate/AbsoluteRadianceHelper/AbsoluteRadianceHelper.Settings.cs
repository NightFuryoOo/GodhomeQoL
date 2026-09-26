using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class AbsoluteRadianceHelper : Module
{
    internal static void ReapplyLiveSettings()
    {
        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyAbsoluteRadianceHealthIfPresent();
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
            if (!absoluteRadianceP5Hp)
            {
                absoluteRadianceMaxHpBeforeP5 = ClampAbsoluteRadianceHp(absoluteRadianceMaxHp);
                absoluteRadianceUseMaxHpBeforeP5 = absoluteRadianceUseMaxHp;
                absoluteRadianceUseCustomPhaseBeforeP5 = absoluteRadianceUseCustomPhase;
                absoluteRadiancePhase2HpBeforeP5 = absoluteRadiancePhase2Hp;
                absoluteRadiancePhase3HpBeforeP5 = absoluteRadiancePhase3Hp;
                absoluteRadiancePhase4HpBeforeP5 = absoluteRadiancePhase4Hp;
                absoluteRadiancePhase5HpBeforeP5 = absoluteRadiancePhase5Hp;
                absoluteRadianceFinalPhaseHpBeforeP5 = absoluteRadianceFinalPhaseHp;
                absoluteRadianceHasStoredStateBeforeP5 = true;
            }

            absoluteRadianceP5Hp = true;
            absoluteRadianceUseMaxHp = true;
            absoluteRadianceUseCustomPhase = false;
            absoluteRadianceMaxHp = P5AbsoluteRadianceHp;
        }
        else
        {
            if (absoluteRadianceP5Hp && absoluteRadianceHasStoredStateBeforeP5)
            {
                absoluteRadianceMaxHp = ClampAbsoluteRadianceHp(absoluteRadianceMaxHpBeforeP5);
                absoluteRadianceUseMaxHp = absoluteRadianceUseMaxHpBeforeP5;
                absoluteRadianceUseCustomPhase = absoluteRadianceUseCustomPhaseBeforeP5;
                absoluteRadiancePhase2Hp = absoluteRadiancePhase2HpBeforeP5;
                absoluteRadiancePhase3Hp = absoluteRadiancePhase3HpBeforeP5;
                absoluteRadiancePhase4Hp = absoluteRadiancePhase4HpBeforeP5;
                absoluteRadiancePhase5Hp = absoluteRadiancePhase5HpBeforeP5;
                absoluteRadianceFinalPhaseHp = absoluteRadianceFinalPhaseHpBeforeP5;
            }

            absoluteRadianceP5Hp = false;
            absoluteRadianceHasStoredStateBeforeP5 = false;
        }

        NormalizeCustomPhaseThresholdState();
        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!absoluteRadianceP5Hp)
        {
            return;
        }

        if (!absoluteRadianceHasStoredStateBeforeP5)
        {
            absoluteRadianceMaxHpBeforeP5 = ClampAbsoluteRadianceHp(absoluteRadianceMaxHp);
            absoluteRadianceUseMaxHpBeforeP5 = absoluteRadianceUseMaxHp;
            absoluteRadianceUseCustomPhaseBeforeP5 = absoluteRadianceUseCustomPhase;
            absoluteRadiancePhase2HpBeforeP5 = absoluteRadiancePhase2Hp;
            absoluteRadiancePhase3HpBeforeP5 = absoluteRadiancePhase3Hp;
            absoluteRadiancePhase4HpBeforeP5 = absoluteRadiancePhase4Hp;
            absoluteRadiancePhase5HpBeforeP5 = absoluteRadiancePhase5Hp;
            absoluteRadianceFinalPhaseHpBeforeP5 = absoluteRadianceFinalPhaseHp;
            absoluteRadianceHasStoredStateBeforeP5 = true;
        }

        absoluteRadianceUseMaxHp = true;
        absoluteRadianceUseCustomPhase = false;
        absoluteRadianceMaxHp = P5AbsoluteRadianceHp;
        NormalizeCustomPhaseThresholdState();
    }

    private static void NormalizeCustomPhaseThresholdState()
    {
        int maxHp = ClampAbsoluteRadianceHp(absoluteRadianceMaxHp);
        absoluteRadiancePhase2Hp = ClampAbsoluteRadiancePhase2Hp(absoluteRadiancePhase2Hp, maxHp);
        absoluteRadiancePhase3Hp = ClampAbsoluteRadiancePhaseHpBelowPrevious(absoluteRadiancePhase3Hp, absoluteRadiancePhase2Hp);
        absoluteRadiancePhase4Hp = ClampAbsoluteRadiancePhaseHpBelowPrevious(absoluteRadiancePhase4Hp, absoluteRadiancePhase3Hp);
        absoluteRadiancePhase5Hp = ClampAbsoluteRadiancePhaseHpBelowPrevious(absoluteRadiancePhase5Hp, absoluteRadiancePhase4Hp);
        absoluteRadianceFinalPhaseHp = ClampAbsoluteRadianceFinalPhaseHp(absoluteRadianceFinalPhaseHp);
    }
}
