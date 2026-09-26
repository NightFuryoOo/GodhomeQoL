using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class PhaseThresholdBossHelper : Module
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

        ApplyPhaseThresholdSettingsIfPresentCore();
    }

    private protected virtual void SetP5HpEnabledCore(bool value)
    {
        if (value)
        {
            if (!P5HpEnabled)
            {
                MaxHpBeforeP5 = ClampBossHp(MaxHp);
                UseMaxHpBeforeP5 = UseMaxHp;
                UseCustomPhaseBeforeP5 = UseCustomPhase;
                Phase2HpBeforeP5 = ClampBossPhase2Hp(Phase2Hp, ResolvePhase2MaxHp());
                Phase3HpBeforeP5 = ClampBossPhase3Hp(Phase3Hp, Phase2Hp);
                HasStoredStateBeforeP5 = true;
            }

            P5HpEnabled = true;
            UseMaxHp = true;
            UseCustomPhase = false;
            MaxHp = P5Hp;
        }
        else
        {
            if (P5HpEnabled && HasStoredStateBeforeP5)
            {
                MaxHp = ClampBossHp(MaxHpBeforeP5);
                UseMaxHp = UseMaxHpBeforeP5;
                UseCustomPhase = UseCustomPhaseBeforeP5;
                Phase2Hp = ClampBossPhase2Hp(Phase2HpBeforeP5, ResolvePhase2MaxHp());
                Phase3Hp = ClampBossPhase3Hp(Phase3HpBeforeP5, Phase2Hp);
            }

            P5HpEnabled = false;
            HasStoredStateBeforeP5 = false;
        }

        NormalizePhaseThresholdState();
        ReapplyLiveSettingsCore();
    }

    private protected virtual void NormalizeP5State()
    {
        if (!P5HpEnabled)
        {
            return;
        }

        if (!HasStoredStateBeforeP5)
        {
            MaxHpBeforeP5 = ClampBossHp(MaxHp);
            UseMaxHpBeforeP5 = UseMaxHp;
            UseCustomPhaseBeforeP5 = UseCustomPhase;
            Phase2HpBeforeP5 = ClampBossPhase2Hp(Phase2Hp, ResolvePhase2MaxHp());
            Phase3HpBeforeP5 = ClampBossPhase3Hp(Phase3Hp, Phase2Hp);
            HasStoredStateBeforeP5 = true;
        }

        UseMaxHp = true;
        UseCustomPhase = false;
        MaxHp = P5Hp;
        NormalizePhaseThresholdState();
    }

    private protected void NormalizePhaseThresholdState()
    {
        Phase2Hp = ClampBossPhase2Hp(Phase2Hp, ResolvePhase2MaxHp());
        Phase3Hp = ClampBossPhase3Hp(Phase3Hp, Phase2Hp);
    }
}
