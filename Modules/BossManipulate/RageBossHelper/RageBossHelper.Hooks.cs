using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class RageBossHelper : Module
{
    private protected override void Load()
    {
        moduleActive = true;
        BossManipulateEntryGuard.EnsureHooks();
        NormalizeP5State();
        NormalizePhaseThresholdState();
        vanillaHpByInstance.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.HealthManager.Update += OnHealthManagerUpdate;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable_Boss;
        On.PlayMakerFSM.Start += OnPlayMakerFsmStart_Boss;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresentCore();
        RestoreVanillaPhaseThresholdsIfPresentCore();
        moduleActive = false;
        On.HealthManager.Awake -= OnHealthManagerAwake;
        On.HealthManager.Start -= OnHealthManagerStart;
        On.HealthManager.Update -= OnHealthManagerUpdate;
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_Boss;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_Boss;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
        hoGEntryAllowed = false;
    }

    private protected void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsBoss(self))
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyBossHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private protected void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsBoss(self))
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyBossHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private protected virtual void OnHealthManagerUpdate(On.HealthManager.orig_Update orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsBoss(self))
        {
            return;
        }

        if (!ShouldApplySettings(self.gameObject) || !ShouldUseCustomPhaseThreshold())
        {
            return;
        }

        int threshold = ClampBossPhase2Hp(Phase2Hp, ResolvePhase2MaxHp());
        if (self.hp > threshold)
        {
            return;
        }

        TryForceBossRage(self.gameObject);
    }

    private protected void OnPlayMakerFsmOnEnable_Boss(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsBossPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private protected virtual void OnPlayMakerFsmStart_Boss(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsBossPhaseControlFsm(self))
        {
            return;
        }

        ApplyPhaseThresholdSettings(self);
    }

    private protected IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsBoss(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyBossHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsBoss(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyBossHealth(hm.gameObject, hm);
            }
        }

        ApplyPhaseThresholdSettingsIfPresentCore();
    }

    private protected virtual void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, SceneName, StringComparison.Ordinal))
        {
            vanillaHpByInstance.Clear();
            return;
        }

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

    private protected string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);
        return newSceneName;
    }

    private protected virtual void TryForceBossRage(GameObject bossObject)
    {
        if (bossObject == null || !ShouldApplyPhaseSettings(bossObject))
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsBossObject(fsm.gameObject))
            {
                continue;
            }

            if (!string.Equals(fsm.FsmName, AttackingFsmName, StringComparison.Ordinal))
            {
                continue;
            }

            FsmBool? rageFlag = fsm.FsmVariables.GetFsmBool(RageVariableName);
            if (rageFlag == null || rageFlag.Value)
            {
                continue;
            }

            rageFlag.Value = true;
            fsm.SendEvent(SummonEventName);
        }
    }
}
