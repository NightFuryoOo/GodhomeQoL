using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {        private void OnQuickFastSuperDashClicked()
        {
            returnToQuickOnClose = true;
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(false);
            SetOverlayVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickCollectorPhasesClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(false);
            SetCollectorVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickFastReloadClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(false);
            SetFastReloadVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickDreamshieldClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(false);
            SetDreamshieldVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickShowHpOnDeathClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(false);
            SetShowHpOnDeathVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickMaskDamageClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(false);
            SetMaskDamageVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickFreezeHitboxesClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(false);
            SetFreezeHitboxesVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickSpeedChangerClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(false);
            SetSpeedChangerVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickTeleportKitClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(false);
            SetTeleportKitVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickBossChallengeClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(false);
            SetRandomPantheonsVisible(false);
            SetBossChallengeVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickRandomPantheonsClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(false);
            SetRandomPantheonsVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickTrueBossRushClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(false);
            SetRandomPantheonsVisible(false);
            SetTrueBossRushVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickCheatsClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetRandomPantheonsVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickAlwaysFuriousClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetRandomPantheonsVisible(false);
            SetGearSwitcherVisible(false);
            SetAlwaysFuriousVisible(true);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(false);
            SetQuickVisible(false);
        }

        private void OnQuickGearSwitcherClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherCharmCostVisible(false);
            SetGearSwitcherPresetVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(false);
            SetGearSwitcherVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickZoteHelperClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetCheatsVisible(false);
            SetZoteHelperVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickBossAnimationClicked()
        {
            returnToQuickOnClose = true;
            returnToQolOnClose = false;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetQolVisible(false);
            SetMenuAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(false);
            SetBossAnimationVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickMenuAnimationClicked()
        {
            returnToQuickOnClose = true;
            returnToQolOnClose = false;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetQolVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(false);
            SetMenuAnimationVisible(true);
            SetQuickVisible(false);
        }

        private void OnQuickQolClicked()
        {
            returnToQuickOnClose = true;
            SetOverlayVisible(false);
            SetCollectorVisible(false);
            SetFastReloadVisible(false);
            SetDreamshieldVisible(false);
            SetShowHpOnDeathVisible(false);
            SetMaskDamageVisible(false);
            SetFreezeHitboxesVisible(false);
            SetSpeedChangerVisible(false);
            SetTeleportKitVisible(false);
            SetBossChallengeVisible(false);
            SetTrueBossRushVisible(false);
            SetAlwaysFuriousVisible(false);
            SetGearSwitcherVisible(false);
            SetMenuAnimationVisible(false);
            SetBossAnimationVisible(false);
            SetZoteHelperVisible(false);
            SetCheatsVisible(false);
            returnToQolOnClose = false;
            SetQolVisible(true);
            SetQuickVisible(false);
        }
    }
}


