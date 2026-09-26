using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class UumuuHelperBase : Module
{
    private protected void ReapplyLiveSettingsCore()
    {
        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyHealthIfPresentCore();
        }
        else
        {
            RestoreVanillaHealthIfPresentCore();
        }

        if (ShouldUseCustomSummonHp())
        {
            ApplySummonHealthIfPresentCore();
        }
        else
        {
            RestoreVanillaSummonHealthIfPresentCore();
        }
    }
}
