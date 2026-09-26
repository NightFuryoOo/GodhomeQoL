using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void UpdateQolInteractivity()
        {
            SetContentInteractivity(qolContent, qolMasterEnabled, "QolEnableRow");
        }

        private void UpdateMenuAnimationInteractivity()
        {
            SetContentInteractivity(menuAnimationContent, menuAnimMasterEnabled, "MenuAnimEnableRow");
        }

        private void UpdateBossChallengeInteractivity()
        {
            SetContentInteractivity(bossChallengeContent, bossChallengeMasterEnabled, "BossChallengeEnableRow");
        }

        private void UpdateBossAnimationInteractivity()
        {
            SetContentInteractivity(bossAnimationContent, bossAnimMasterEnabled, "BossAnimEnableRow");
        }

        private void UpdateRandomPantheonsInteractivity()
        {
            SetContentInteractivity(randomPantheonsContent, randomPantheonsMasterEnabled, "RandomPantheonsToggleRow");
        }

        private void UpdateTrueBossRushInteractivity()
        {
            SetContentInteractivity(trueBossRushContent, trueBossRushMasterEnabled, "TrueBossRushToggleRow");
        }

        private void UpdateCheatsInteractivity()
        {
            SetContentInteractivity(cheatsContent, cheatsMasterEnabled, "CheatsEnableRow");
        }

        private void UpdateFastSuperDashInteractivity()
        {
            SetContentInteractivity(fastSuperDashContent, GetModuleEnabled(), "ModuleToggleRow");
        }

        private void UpdateCollectorInteractivity()
        {
            SetContentInteractivity(collectorContent, GetCollectorPhasesEnabled(), "CollectorModuleToggleRow");
        }

        private void UpdateFastReloadInteractivity()
        {
            SetContentInteractivity(fastReloadContent, GetFastReloadEnabled(), "FastReloadToggleRow");
        }

        private void UpdateDreamshieldInteractivity()
        {
            SetContentInteractivity(dreamshieldContent, GetDreamshieldEnabled(), "DreamshieldToggleRow");
        }

        private void UpdateShowHpOnDeathInteractivity()
        {
            SetContentInteractivity(showHpOnDeathContent, GetShowHpOnDeathEnabled(), "ShowHPOnDeathGlobalRow");
        }

        private void UpdateMaskDamageInteractivity()
        {
            SetContentInteractivity(maskDamageContent, GetMaskDamageEnabled(), "MaskDamageEnableRow");
        }

        private void UpdateFreezeHitboxesInteractivity()
        {
            SetContentInteractivity(freezeHitboxesContent, GetFreezeHitboxesEnabled(), "FreezeHitboxesEnableRow");
        }

        private void UpdateFpsBoostInteractivity()
        {
            SetContentInteractivity(fpsBoostContent, GetFpsBoostEnabled(), "FpsBoostEnableRow");
        }

        private void UpdateSpeedChangerInteractivity()
        {
            SetContentInteractivity(speedChangerContent, SpeedChanger.globalSwitch, "SpeedChangerGlobalRow");
        }

        private void UpdateTeleportKitInteractivity()
        {
            SetContentInteractivity(teleportKitContent, GetTeleportKitEnabled(), "TeleportKitToggleRow");
        }

        private void UpdateGearSwitcherInteractivity()
        {
            SetContentInteractivity(gearSwitcherContent, GetGearSwitcherEnabled(), "GearSwitcherEnableRow");
        }

        private void UpdateZoteHelperInteractivity()
        {
            SetContentInteractivity(zoteHelperContent, GetZoteHelperEnabled(), "ZoteHelperEnableRow");
            if (!GetZoteHelperEnabled())
            {
                return;
            }

            SetRowInteractivity(zoteHelperContent, "ZoteBossHpRow", Modules.BossChallenge.ZoteHelper.zoteUseCustomBossHp);
            SetRowInteractivity(zoteHelperContent, "ZoteFlyingHpRow", Modules.BossChallenge.ZoteHelper.zoteUseCustomFlyingHp);
            SetRowInteractivity(zoteHelperContent, "ZoteHoppingHpRow", Modules.BossChallenge.ZoteHelper.zoteUseCustomHoppingHp);
            SetRowInteractivity(zoteHelperContent, "ZoteSummonLimitRow", Modules.BossChallenge.ZoteHelper.zoteUseCustomSummonLimit);
        }

        private void UpdateGruzHelperInteractivity()
        {
            SetContentInteractivity(gruzHelperContent, GetGruzMotherHelperEnabled(), "GruzHelperEnableRow");
            if (!GetGruzMotherHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.GruzMotherHelper.gruzUseMaxHp;
            bool p5Hp = Modules.BossChallenge.GruzMotherHelper.gruzP5Hp;
            SetRowInteractivity(gruzHelperContent, "GruzUseMaxHpRow", !p5Hp);
            SetRowInteractivity(gruzHelperContent, "GruzMaxHpRow", useMaxHp && !p5Hp);
        }

        private void UpdateHornetHelperInteractivity()
        {
            SetContentInteractivity(hornetHelperContent, GetHornetProtectorHelperEnabled(), "HornetHelperEnableRow");
            if (!GetHornetProtectorHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.HornetProtectorHelper.hornetUseMaxHp;
            bool p5Hp = Modules.BossChallenge.HornetProtectorHelper.hornetP5Hp;
            SetRowInteractivity(hornetHelperContent, "HornetUseMaxHpRow", !p5Hp);
            SetRowInteractivity(hornetHelperContent, "HornetMaxHpRow", useMaxHp && !p5Hp);
        }

        private void UpdateMawlekHelperInteractivity()
        {
            SetContentInteractivity(mawlekHelperContent, GetBroodingMawlekHelperEnabled(), "MawlekHelperEnableRow");
            if (!GetBroodingMawlekHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.BroodingMawlekHelper.mawlekUseMaxHp;
            bool p5Hp = Modules.BossChallenge.BroodingMawlekHelper.mawlekP5Hp;
            SetRowInteractivity(mawlekHelperContent, "MawlekUseMaxHpRow", !p5Hp);
            SetRowInteractivity(mawlekHelperContent, "MawlekMaxHpRow", useMaxHp && !p5Hp);
        }

        private void UpdateMassiveMossHelperInteractivity()
        {
            SetContentInteractivity(massiveMossHelperContent, GetMassiveMossChargerHelperEnabled(), "MassiveMossHelperEnableRow");
            if (!GetMassiveMossChargerHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.MassiveMossChargerHelper.massiveMossUseMaxHp;
            bool p5Hp = Modules.BossChallenge.MassiveMossChargerHelper.massiveMossP5Hp;
            SetRowInteractivity(massiveMossHelperContent, "MassiveMossUseMaxHpRow", !p5Hp);
            SetRowInteractivity(massiveMossHelperContent, "MassiveMossMaxHpRow", useMaxHp && !p5Hp);
        }

        private void UpdateCrystalGuardianHelperInteractivity()
        {
            SetContentInteractivity(crystalGuardianHelperContent, GetCrystalGuardianHelperEnabled(), "CrystalGuardianHelperEnableRow");
            if (!GetCrystalGuardianHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianUseMaxHp;
            bool p5Hp = Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianP5Hp;
            SetRowInteractivity(crystalGuardianHelperContent, "CrystalGuardianUseMaxHpRow", !p5Hp);
            SetRowInteractivity(crystalGuardianHelperContent, "CrystalGuardianMaxHpRow", useMaxHp && !p5Hp);
        }

        private void UpdateEnragedGuardianHelperInteractivity()
        {
            SetContentInteractivity(enragedGuardianHelperContent, GetEnragedGuardianHelperEnabled(), "EnragedGuardianHelperEnableRow");
            if (!GetEnragedGuardianHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianUseMaxHp;
            bool p5Hp = Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianP5Hp;
            SetRowInteractivity(enragedGuardianHelperContent, "EnragedGuardianUseMaxHpRow", !p5Hp);
            SetRowInteractivity(enragedGuardianHelperContent, "EnragedGuardianMaxHpRow", useMaxHp && !p5Hp);
        }

        private void UpdateHornetSentinelHelperInteractivity()
        {
            SetContentInteractivity(hornetSentinelHelperContent, GetHornetSentinelHelperEnabled(), "HornetSentinelHelperEnableRow");
            if (!GetHornetSentinelHelperEnabled())
            {
                return;
            }

            bool useMaxHp = Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseMaxHp;
            bool p5Hp = Modules.BossChallenge.HornetSentinelHelper.hornetSentinelP5Hp;
            bool useCustomPhase = Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseCustomPhase;
            SetRowInteractivity(hornetSentinelHelperContent, "HornetSentinelUseMaxHpRow", !p5Hp);
            SetRowInteractivity(hornetSentinelHelperContent, "HornetSentinelMaxHpRow", useMaxHp && !p5Hp);
            SetRowInteractivity(hornetSentinelHelperContent, "HornetSentinelUseCustomPhaseRow", !p5Hp);
            SetRowInteractivity(hornetSentinelHelperContent, "HornetSentinelPhase2HpRow", useCustomPhase && !p5Hp);
        }

        private static void SetRowInteractivity(RectTransform? content, string rowName, bool enabled)
        {
            if (content == null)
            {
                return;
            }

            Transform? row = content.Find(rowName);
            if (row == null)
            {
                return;
            }

            CanvasGroup group = row.GetComponent<CanvasGroup>() ?? row.gameObject.AddComponent<CanvasGroup>();
            group.alpha = enabled ? 1f : DisabledContentAlpha;

            Selectable[] selectables = row.GetComponentsInChildren<Selectable>(true);
            foreach (Selectable selectable in selectables)
            {
                selectable.interactable = enabled;
            }
        }

        private static void SetContentInteractivity(RectTransform? content, bool enabled, string masterRowName)
        {
            if (content == null)
            {
                return;
            }

            float dimAlpha = enabled ? 1f : DisabledContentAlpha;
            int childCount = content.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = content.GetChild(i);
                CanvasGroup group = child.GetComponent<CanvasGroup>() ?? child.gameObject.AddComponent<CanvasGroup>();
                if (!enabled && string.Equals(child.name, masterRowName, StringComparison.Ordinal))
                {
                    group.alpha = 1f;
                }
                else
                {
                    group.alpha = dimAlpha;
                }
            }

            Selectable[] selectables = content.GetComponentsInChildren<Selectable>(true);
            foreach (Selectable selectable in selectables)
            {
                bool allow = enabled;
                if (!enabled)
                {
                    Transform? current = selectable.transform;
                    while (current != null && current != content)
                    {
                        if (string.Equals(current.name, masterRowName, StringComparison.Ordinal))
                        {
                            allow = true;
                            break;
                        }
                        current = current.parent;
                    }
                }

                selectable.interactable = allow;
            }
        }
    }
}
