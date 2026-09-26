using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using Modding;
using Satchel;
using Satchel.Futils;
using UnityEngine;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class ArmoredBossHelper : Module
{
    private protected override void Load()
    {
        moduleActive = true;
        BossManipulateEntryGuard.EnsureHooks();
        NormalizeP5State();
        NormalizeConfiguredHpState();
        vanillaArmorHpByInstance.Clear();
        vanillaRecoverHpByFsm.Clear();
        trackedPhaseByArmorInstance.Clear();
        appliedHeadKillsByArmorInstance.Clear();
        lastHeadHpByInstance.Clear();
        confirmedHeadKillCount = 0;
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
        moduleActive = false;
        On.HealthManager.Awake -= OnHealthManagerAwake;
        On.HealthManager.Start -= OnHealthManagerStart;
        On.HealthManager.Update -= OnHealthManagerUpdate;
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable_Boss;
        On.PlayMakerFSM.Start -= OnPlayMakerFsmStart_Boss;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaArmorHpByInstance.Clear();
        vanillaRecoverHpByFsm.Clear();
        trackedPhaseByArmorInstance.Clear();
        appliedHeadKillsByArmorInstance.Clear();
        lastHeadHpByInstance.Clear();
        confirmedHeadKillCount = 0;
        hoGEntryAllowed = false;
    }

    private void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsArmor(self))
        {
            return;
        }

        RememberVanillaArmorHp(self);
        if (ShouldApplySettings(self.gameObject))
        {
            ApplyPhaseSettings(self, forceReapply: true);
        }
    }

    private void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsArmor(self))
        {
            return;
        }

        RememberVanillaArmorHp(self);
        if (ShouldApplySettings(self.gameObject))
        {
            ApplyPhaseSettings(self, forceReapply: true);
            _ = self.StartCoroutine(DeferredApply(self));
        }
    }

    private void OnHealthManagerUpdate(On.HealthManager.orig_Update orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !ShouldApplySettings(self.gameObject))
        {
            return;
        }

        if (IsHead(self))
        {
            TrackHeadHealth(self);
            return;
        }

        if (!IsArmor(self))
        {
            return;
        }

        ApplyPhaseSettings(self, forceReapply: false);
    }

    private void OnPlayMakerFsmOnEnable_Boss(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsBossScene(self.gameObject.scene.name))
        {
            return;
        }

        if (!hoGEntryAllowed)
        {
            return;
        }

        if (IsRecoverFsm(self) || IsMainFsm(self))
        {
            ApplySettingsIfPresentCore(forceReapply: true);
        }
    }

    private void OnPlayMakerFsmStart_Boss(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null || !IsBossScene(self.gameObject.scene.name))
        {
            return;
        }

        if (!hoGEntryAllowed)
        {
            return;
        }

        if (IsRecoverFsm(self) || IsMainFsm(self))
        {
            ApplySettingsIfPresentCore(forceReapply: true);
        }
    }

    private IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsArmor(hm) || !ShouldApplySettings(hm.gameObject))
        {
            yield break;
        }

        ApplyPhaseSettings(hm, forceReapply: true);
        yield return new WaitForSeconds(0.01f);
        if (moduleActive && hm != null && hm.gameObject != null && IsArmor(hm) && ShouldApplySettings(hm.gameObject))
        {
            ApplyPhaseSettings(hm, forceReapply: true);
        }
    }

    private void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!IsBossScene(to.name))
        {
            vanillaArmorHpByInstance.Clear();
            vanillaRecoverHpByFsm.Clear();
            trackedPhaseByArmorInstance.Clear();
            appliedHeadKillsByArmorInstance.Clear();
            lastHeadHpByInstance.Clear();
            confirmedHeadKillCount = 0;
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        ApplySettingsIfPresentCore(forceReapply: true);
    }

    private string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);
        return newSceneName;
    }

    private void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (IsBossScene(nextScene))
        {
            if (BossManipulateEntryGuard.IsAllowedBossEntry(currentScene, nextScene))
            {
                hoGEntryAllowed = true;
            }
            else if (IsBossScene(currentScene) && hoGEntryAllowed)
            {
                hoGEntryAllowed = true;
            }
            else
            {
                hoGEntryAllowed = false;
            }

            return;
        }

        hoGEntryAllowed = false;
    }
}
