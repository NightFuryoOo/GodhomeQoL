using Modding;
using Satchel;
using Satchel.Futils;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class FlukemarmHelper : Module
{
    internal static void ReapplyLiveSettings()
    {
        if (!moduleActive)
        {
            return;
        }

        ApplyFlukemarmHealthIfPresent();
        RestoreVanillaHealthIfPresent();
        ApplySummonLimitSettingsIfPresent();
    }

    internal static void SetP5HpEnabled(bool value)
    {
        if (value)
        {
            if (!flukemarmP5Hp)
            {
                flukemarmMaxHpBeforeP5 = ClampFlukemarmHp(flukemarmMaxHp);
                flukemarmFlyHpBeforeP5 = ClampFlukemarmHp(flukemarmFlyHp);
                flukemarmUseMaxHpBeforeP5 = flukemarmUseMaxHp;
                flukemarmUseCustomSummonLimitBeforeP5 = flukemarmUseCustomSummonLimit;
                flukemarmSummonLimitBeforeP5 = ClampSummonLimit(flukemarmSummonLimit);
                flukemarmHasStoredStateBeforeP5 = true;
            }

            flukemarmP5Hp = true;
            flukemarmUseMaxHp = true;
            flukemarmUseCustomSummonLimit = false;
            flukemarmMaxHp = P5FlukemarmHp;
            flukemarmFlyHp = P5FlukeFlyHp;
        }
        else
        {
            if (flukemarmP5Hp && flukemarmHasStoredStateBeforeP5)
            {
                flukemarmMaxHp = ClampFlukemarmHp(flukemarmMaxHpBeforeP5);
                flukemarmFlyHp = ClampFlukemarmHp(flukemarmFlyHpBeforeP5);
                flukemarmUseMaxHp = flukemarmUseMaxHpBeforeP5;
                flukemarmUseCustomSummonLimit = flukemarmUseCustomSummonLimitBeforeP5;
                flukemarmSummonLimit = ClampSummonLimit(flukemarmSummonLimitBeforeP5);
            }

            flukemarmP5Hp = false;
            flukemarmHasStoredStateBeforeP5 = false;
        }

        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        flukemarmSummonLimit = ClampSummonLimit(flukemarmSummonLimit);
        if (!flukemarmP5Hp)
        {
            return;
        }

        if (!flukemarmHasStoredStateBeforeP5)
        {
            flukemarmMaxHpBeforeP5 = ClampFlukemarmHp(flukemarmMaxHp);
            flukemarmFlyHpBeforeP5 = ClampFlukemarmHp(flukemarmFlyHp);
            flukemarmUseMaxHpBeforeP5 = flukemarmUseMaxHp;
            flukemarmUseCustomSummonLimitBeforeP5 = flukemarmUseCustomSummonLimit;
            flukemarmSummonLimitBeforeP5 = ClampSummonLimit(flukemarmSummonLimit);
            flukemarmHasStoredStateBeforeP5 = true;
        }

        flukemarmUseMaxHp = true;
        flukemarmUseCustomSummonLimit = false;
        flukemarmMaxHp = P5FlukemarmHp;
        flukemarmFlyHp = P5FlukeFlyHp;
    }
}
