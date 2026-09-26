using Mono.Cecil.Cil;
using MonoMod.Cil;
using IL;
using InControl;
using GodhomeQoL.Modules.BossChallenge;
using GodhomeQoL.Modules.QoL;
using ToggleableBindings;
using ToggleableBindings.VanillaBindings;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class GearSwitcher : Module
{
    private const int VoidHeartCharmId = 36;
    private const int GrimmchildCharmId = 40;
    private const string ShowBoundCharmsEvent = "SHOW BOUND CHARMS";
    private const string HideBoundCharmsEvent = "HIDE BOUND CHARMS";
    private const string CharmIndicatorCheckEvent = "CHARM INDICATOR CHECK";

    private static void RestoreCharmsBindingExemptDefaults()
    {
        try
        {
            CharmsBinding.ExemptCharms = new[] { VoidHeartCharmId, GrimmchildCharmId };
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "GearSwitcher.Charms.cs");
        }
    }

    internal static void ApplyCharmCostsImmediate(GearPreset preset)
    {
        if (!IsGloballyEnabled)
        {
            return;
        }

        if (!IsSafeToApply())
        {
            pendingCharmCostPreset = preset;
            pendingCharmCostApply = true;
            EnsurePendingCoroutine();
            return;
        }

        ApplyCharmCosts(preset);
    }

    private static void SetCharmsBindingExemptCharms()
    {
        try
        {
            ToggleableBindings.VanillaBindings.CharmsBinding.ExemptCharms = new[] { VoidHeartCharmId };
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "GearSwitcher.Charms.cs");
        }
    }

    private static void RemoveCharmsExceptVoidHeart()
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        RemoveCharms(pd);
        PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
        PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
        PlayMakerFSM.BroadcastEvent("UPDATE BLUE HEALTH");
    }

    private static void ApplyCharmCosts(GearPreset preset)
    {
        PlayerData? data = PlayerData.instance;
        if (data == null)
        {
            return;
        }

        int gatheringCost = Math.Max(0, Math.Min(99, preset.GatheringSwarmCost));
        int compassCost = Math.Max(0, Math.Min(99, preset.WaywardCompassCost));
        int stalwartCost = Math.Max(0, Math.Min(99, preset.StalwartShellCost));
        int soulCatcherCost = Math.Max(0, Math.Min(99, preset.SoulCatcherCost));
        int shamanStoneCost = Math.Max(0, Math.Min(99, preset.ShamanStoneCost));
        int soulEaterCost = Math.Max(0, Math.Min(99, preset.SoulEaterCost));
        int dashmasterCost = Math.Max(0, Math.Min(99, preset.DashmasterCost));
        int sprintmasterCost = Math.Max(0, Math.Min(99, preset.SprintmasterCost));
        int grubsongCost = Math.Max(0, Math.Min(99, preset.GrubsongCost));
        int grubberflysElegyCost = Math.Max(0, Math.Min(99, preset.GrubberflysElegyCost));
        int unbreakableHeartCost = Math.Max(0, Math.Min(99, preset.UnbreakableHeartCost));
        int unbreakableGreedCost = Math.Max(0, Math.Min(99, preset.UnbreakableGreedCost));
        int unbreakableStrengthCost = Math.Max(0, Math.Min(99, preset.UnbreakableStrengthCost));
        int spellTwisterCost = Math.Max(0, Math.Min(99, preset.SpellTwisterCost));
        int steadyBodyCost = Math.Max(0, Math.Min(99, preset.SteadyBodyCost));
        int heavyBlowCost = Math.Max(0, Math.Min(99, preset.HeavyBlowCost));
        int quickSlashCost = Math.Max(0, Math.Min(99, preset.QuickSlashCost));
        int longnailCost = Math.Max(0, Math.Min(99, preset.LongnailCost));
        int markOfPrideCost = Math.Max(0, Math.Min(99, preset.MarkOfPrideCost));
        int furyOfTheFallenCost = Math.Max(0, Math.Min(99, preset.FuryOfTheFallenCost));
        int thornsOfAgonyCost = Math.Max(0, Math.Min(99, preset.ThornsOfAgonyCost));
        int baldurShellCost = Math.Max(0, Math.Min(99, preset.BaldurShellCost));
        int flukenestCost = Math.Max(0, Math.Min(99, preset.FlukenestCost));
        int defendersCrestCost = Math.Max(0, Math.Min(99, preset.DefendersCrestCost));
        int glowingWombCost = Math.Max(0, Math.Min(99, preset.GlowingWombCost));
        int quickFocusCost = Math.Max(0, Math.Min(99, preset.QuickFocusCost));
        int deepFocusCost = Math.Max(0, Math.Min(99, preset.DeepFocusCost));
        int lifebloodHeartCost = Math.Max(0, Math.Min(99, preset.LifebloodHeartCost));
        int lifebloodCoreCost = Math.Max(0, Math.Min(99, preset.LifebloodCoreCost));
        int jonisBlessingCost = Math.Max(0, Math.Min(99, preset.JonisBlessingCost));
        int hivebloodCost = Math.Max(0, Math.Min(99, preset.HivebloodCost));
        int sporeShroomCost = Math.Max(0, Math.Min(99, preset.SporeShroomCost));
        int sharpShadowCost = Math.Max(0, Math.Min(99, preset.SharpShadowCost));
        int shapeOfUnnCost = Math.Max(0, Math.Min(99, preset.ShapeOfUnnCost));
        int nailmastersGloryCost = Math.Max(0, Math.Min(99, preset.NailmastersGloryCost));
        int weaversongCost = Math.Max(0, Math.Min(99, preset.WeaversongCost));
        int dreamWielderCost = Math.Max(0, Math.Min(99, preset.DreamWielderCost));
        int dreamshieldCost = Math.Max(0, Math.Min(99, preset.DreamshieldCost));
        bool useGrimmchild = preset.UseGrimmchild;
        int grimmchildCost = Math.Max(0, Math.Min(99, preset.GrimmchildCost));
        int carefreeMelodyCost = Math.Max(0, Math.Min(99, preset.CarefreeMelodyCost));
        bool useVoidHeart = preset.UseVoidHeart;
        int voidHeartCost = Math.Max(0, Math.Min(99, useVoidHeart ? preset.VoidHeartCost : preset.KingsoulCost));

        int previousGrimmchildLevel = data.grimmChildLevel;

        data.charmCost_1 = gatheringCost;
        data.charmCost_2 = compassCost;
        data.charmCost_4 = stalwartCost;
        data.charmCost_20 = soulCatcherCost;
        data.charmCost_19 = shamanStoneCost;
        data.charmCost_21 = soulEaterCost;
        data.charmCost_31 = dashmasterCost;
        data.charmCost_37 = sprintmasterCost;
        data.charmCost_3 = grubsongCost;
        data.charmCost_35 = grubberflysElegyCost;
        data.charmCost_23 = unbreakableHeartCost;
        data.charmCost_24 = unbreakableGreedCost;
        data.charmCost_25 = unbreakableStrengthCost;
        data.charmCost_33 = spellTwisterCost;
        data.charmCost_14 = steadyBodyCost;
        data.charmCost_15 = heavyBlowCost;
        data.charmCost_32 = quickSlashCost;
        data.charmCost_18 = longnailCost;
        data.charmCost_13 = markOfPrideCost;
        data.charmCost_6 = furyOfTheFallenCost;
        data.charmCost_12 = thornsOfAgonyCost;
        data.charmCost_5 = baldurShellCost;
        data.charmCost_11 = flukenestCost;
        data.charmCost_10 = defendersCrestCost;
        data.charmCost_22 = glowingWombCost;
        data.charmCost_7 = quickFocusCost;
        data.charmCost_34 = deepFocusCost;
        data.charmCost_8 = lifebloodHeartCost;
        data.charmCost_9 = lifebloodCoreCost;
        data.charmCost_27 = jonisBlessingCost;
        data.charmCost_29 = hivebloodCost;
        data.charmCost_17 = sporeShroomCost;
        data.charmCost_16 = sharpShadowCost;
        data.charmCost_28 = shapeOfUnnCost;
        data.charmCost_26 = nailmastersGloryCost;
        data.charmCost_39 = weaversongCost;
        data.charmCost_30 = dreamWielderCost;
        data.charmCost_38 = dreamshieldCost;
        data.charmCost_40 = useGrimmchild ? grimmchildCost : carefreeMelodyCost;
        data.grimmChildLevel = useGrimmchild ? 4 : 5;
        data.SetBoolInternal("destroyedNightmareLantern", !useGrimmchild);
        data.charmCost_36 = voidHeartCost;
        data.royalCharmState = useVoidHeart ? 4 : 3;

        HeroController? hero = HeroController.instance;
        if (hero != null)
        {
            if (useGrimmchild)
            {
                EnsureGrimmchildUnlocked(data);
                RefreshGrimmchildCharm(data);
            }
            else
            {
                RemoveGrimmchildCompanion();
                if (previousGrimmchildLevel != data.grimmChildLevel)
                {
                    RefreshGrimmchildCharm(data);
                }
            }

            hero.CharmUpdate();
            PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
            PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
        }

        data.CalculateNotchesUsed();
        ApplyCharmLoadout(preset);

        if (!IsApplyingPreset)
        {
            ApplyOvercharmed(preset);
        }
    }

    private static void RefreshGrimmchildCharm(PlayerData data)
    {
        if (!data.GetBoolInternal("equippedCharm_40"))
        {
            return;
        }

        try
        {
            data.SetBoolInternal("equippedCharm_40", false);
            data.UnequipCharm(40);
            data.SetBoolInternal("equippedCharm_40", true);
            data.EquipCharm(40);
            data.CalculateNotchesUsed();
            HeroController.instance?.CharmUpdate();
            PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
            PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");

            if (data.grimmChildLevel < 5)
            {
                GameObject? existing = GameObject.FindWithTag("Grimmchild");
                if (existing != null)
                {
                    UnityEngine.Object.Destroy(existing);
                }

                if (HeroController.instance != null)
                {
                    GameManager.instance?.StartCoroutine(SpawnGrimmChildCoroutine());
                }
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "GearSwitcher.Charms.cs");
        }
    }

    private static IEnumerator SpawnGrimmChildCoroutine()
    {
        for (int i = 0; i < 2; i++)
        {
            yield return null;
        }

        Transform? effects = null;
        try
        {
            effects = HeroController.instance?.transform.Find("Charm Effects");
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "GearSwitcher.Charms.cs");
        }

        if (effects == null)
        {
            yield break;
        }

        try
        {
            PlayMakerFSM? fsm = effects.gameObject.LocateMyFSM("Spawn Grimmchild");
            fsm?.SendEvent("CHARM EQUIP CHECK");
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "GearSwitcher.Charms.cs");
        }
    }

    private static void RemoveGrimmchildCompanion()
    {
        try
        {
            GameObject? existing = GameObject.FindWithTag("Grimmchild");
            if (existing != null)
            {
                UnityEngine.Object.Destroy(existing);
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "GearSwitcher.Charms.cs");
        }
    }

    private static void EnsureGrimmchildUnlocked(PlayerData data)
    {
        try
        {
            data.SetBoolInternal("gotCharm_40", true);
            data.SetBoolInternal("newCharm_40", false);
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "GearSwitcher.Charms.cs");
        }
    }

    private static void RemoveCharms(PlayerData pd)
    {
        List<int> equippedCharms = GetEquippedCharms();
        foreach (int idCharm in equippedCharms)
        {
            if (pd.royalCharmState == 4 && idCharm == 36)
            {
                continue;
            }

            pd.SetBoolInternal($"equippedCharm_{idCharm}", false);
            pd.UnequipCharm(idCharm);
        }

        RemovePaleCourtCharms(pd);
        pd.overcharmed = false;
        pd.CalculateNotchesUsed();
        HeroController.instance?.CharmUpdate();
    }

    private static void RemovePaleCourtCharms(PlayerData pd)
    {
        foreach (int charmId in PaleCourtCharmIds)
        {
            string charmKey = $"equippedCharm_{charmId}";
            if (!pd.GetBool(charmKey))
            {
                continue;
            }

            pd.SetBool(charmKey, false);
            pd.UnequipCharm(charmId);
        }
    }

    internal static void ApplyFreeCharms(bool isFree)
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        int multiplier = isFree ? 0 : 1;
        pd.SetInt("charmCost_1", 1 * multiplier);
        pd.SetInt("charmCost_2", 1 * multiplier);
        pd.SetInt("charmCost_3", 1 * multiplier);
        pd.SetInt("charmCost_4", 2 * multiplier);
        pd.SetInt("charmCost_5", 2 * multiplier);
        pd.SetInt("charmCost_6", 2 * multiplier);
        pd.SetInt("charmCost_7", 3 * multiplier);
        pd.SetInt("charmCost_8", 2 * multiplier);
        pd.SetInt("charmCost_9", 3 * multiplier);
        pd.SetInt("charmCost_10", 1 * multiplier);
        pd.SetInt("charmCost_11", 3 * multiplier);
        pd.SetInt("charmCost_12", 1 * multiplier);
        pd.SetInt("charmCost_13", 3 * multiplier);
        pd.SetInt("charmCost_14", 1 * multiplier);
        pd.SetInt("charmCost_15", 2 * multiplier);
        pd.SetInt("charmCost_16", 2 * multiplier);
        pd.SetInt("charmCost_17", 1 * multiplier);
        pd.SetInt("charmCost_18", 2 * multiplier);
        pd.SetInt("charmCost_19", 3 * multiplier);
        pd.SetInt("charmCost_20", 2 * multiplier);
        pd.SetInt("charmCost_21", 4 * multiplier);
        pd.SetInt("charmCost_22", 2 * multiplier);
        pd.SetInt("charmCost_23", 2 * multiplier);
        pd.SetInt("charmCost_24", 2 * multiplier);
        pd.SetInt("charmCost_25", 3 * multiplier);
        pd.SetInt("charmCost_26", 1 * multiplier);
        pd.SetInt("charmCost_27", 4 * multiplier);
        pd.SetInt("charmCost_28", 2 * multiplier);
        pd.SetInt("charmCost_29", 4 * multiplier);
        pd.SetInt("charmCost_30", 1 * multiplier);
        pd.SetInt("charmCost_31", 2 * multiplier);
        pd.SetInt("charmCost_32", 3 * multiplier);
        pd.SetInt("charmCost_33", 2 * multiplier);
        pd.SetInt("charmCost_34", 4 * multiplier);
        pd.SetInt("charmCost_35", 3 * multiplier);

        if (pd.royalCharmState == 4)
        {
            pd.SetInt("charmCost_36", 0);
        }
        else
        {
            pd.SetInt("charmCost_36", 5 * multiplier);
        }

        pd.SetInt("charmCost_37", 1 * multiplier);
        pd.SetInt("charmCost_38", 3 * multiplier);
        pd.SetInt("charmCost_39", 2 * multiplier);
        pd.SetInt("charmCost_40", 2 * multiplier);
    }

    private static List<int> GetEquippedCharms()
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return new List<int>();
        }

        try
        {
            object? value = ReflectionHelper.GetField<PlayerData, object>(pd, "equippedCharms");
            if (value is List<int> list)
            {
                return new List<int>(list);
            }

            if (value is int[] array)
            {
                return array.Where(id => id > 0).ToList();
            }

            if (value is bool[] bools)
            {
                List<int> charms = new();
                for (int i = 0; i < bools.Length; i++)
                {
                    if (bools[i])
                    {
                        charms.Add(i + 1);
                    }
                }

                return charms;
            }

            if (value is IList listObj)
            {
                List<int> charms = new();
                foreach (object obj in listObj)
                {
                    if (obj is int charm)
                    {
                        charms.Add(charm);
                    }
                }

                return charms;
            }
        }
        catch (Exception swallowed)
        {
            LogSuppressed(swallowed, "GearSwitcher.Charms.cs");
        }

        return new List<int>();
    }
}
