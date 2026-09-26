using Modding;
using Satchel;
using Satchel.Futils;
using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class VengeflyKing : Module
{
    internal static void ReapplyLiveSettings()
    {
        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyVengeflyHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        ApplySummonLimitSettingsIfPresent();
    }

    internal static void SetP5HpEnabled(bool value)
    {
        if (value)
        {
            if (!vengeflyKingP5Hp)
            {
                vengeflyKingLeftMaxHpBeforeP5 = ClampHp(vengeflyKingLeftMaxHp);
                vengeflyKingRightMaxHpBeforeP5 = ClampHp(vengeflyKingRightMaxHp);
                vengeflyKingSummonMaxHpBeforeP5 = ClampHp(vengeflyKingSummonMaxHp);
                vengeflyKingUseMaxHpBeforeP5 = vengeflyKingUseMaxHp;
                vengeflyKingUseCustomSummonLimitBeforeP5 = vengeflyKingUseCustomSummonLimit;
                vengeflyKingHasStoredStateBeforeP5 = true;
            }

            vengeflyKingP5Hp = true;
            vengeflyKingUseMaxHp = true;
            vengeflyKingUseCustomSummonLimit = false;
            vengeflyKingLeftMaxHp = P5LeftHp;
            vengeflyKingRightMaxHp = P5RightHp;
            vengeflyKingSummonMaxHp = P5SummonHp;
        }
        else
        {
            if (vengeflyKingP5Hp && vengeflyKingHasStoredStateBeforeP5)
            {
                vengeflyKingLeftMaxHp = ClampHp(vengeflyKingLeftMaxHpBeforeP5);
                vengeflyKingRightMaxHp = ClampHp(vengeflyKingRightMaxHpBeforeP5);
                vengeflyKingSummonMaxHp = ClampHp(vengeflyKingSummonMaxHpBeforeP5);
                vengeflyKingUseMaxHp = vengeflyKingUseMaxHpBeforeP5;
                vengeflyKingUseCustomSummonLimit = vengeflyKingUseCustomSummonLimitBeforeP5;
            }

            vengeflyKingP5Hp = false;
            vengeflyKingHasStoredStateBeforeP5 = false;
        }

        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!vengeflyKingP5Hp)
        {
            return;
        }

        if (!vengeflyKingHasStoredStateBeforeP5)
        {
            vengeflyKingLeftMaxHpBeforeP5 = ClampHp(vengeflyKingLeftMaxHp);
            vengeflyKingRightMaxHpBeforeP5 = ClampHp(vengeflyKingRightMaxHp);
            vengeflyKingSummonMaxHpBeforeP5 = ClampHp(vengeflyKingSummonMaxHp);
            vengeflyKingUseMaxHpBeforeP5 = vengeflyKingUseMaxHp;
            vengeflyKingUseCustomSummonLimitBeforeP5 = vengeflyKingUseCustomSummonLimit;
            vengeflyKingHasStoredStateBeforeP5 = true;
        }

        vengeflyKingUseMaxHp = true;
        vengeflyKingUseCustomSummonLimit = false;
        vengeflyKingLeftMaxHp = P5LeftHp;
        vengeflyKingRightMaxHp = P5RightHp;
        vengeflyKingSummonMaxHp = P5SummonHp;
    }
}
