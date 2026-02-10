using SFCore.Utils;
using GodhomeQoL.Modules.Tools;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class AlwaysFurious : Module
{
    private int? savedHealth;
    private int? savedMaxHealth;
    private int? savedMaxHealthBase;
    private static bool charmUpdateInProgress;
    private static bool heroRefreshPending;
    private static bool hudRefreshPending;
    private static bool refreshCoroutineRunning;
    private static bool pendingApply;
    private static bool pendingRestore;
    private static bool healthActionCoroutineRunning;

    private sealed class FuryFsmSnapshot
    {
        public PlayMakerFSM? Fsm { get; init; }
        public bool HadEquipGlobalTransition { get; init; }
        public string? InitFinished { get; init; }
        public string? CheckHpCancel { get; init; }
        public string? ActivateHealedFull { get; init; }
        public string? StayFuriedHealedFull { get; init; }
        public string? RecheckFinished { get; init; }
        public bool Modified { get; set; }
    }

    private static readonly Dictionary<int, FuryFsmSnapshot> FurySnapshots = new();

    private protected override void Load()
    {
        On.PlayMakerFSM.OnEnable += OnFsmEnable;
        On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.OnEnter += OnPlayerDataBoolTestAction;
        On.HeroController.CharmUpdate += OnCharmUpdate;
        TryModifyExistingFsm();
        TryApplyForcedHealth();
        QuickMenu.RefreshQuickMenuEntryColors();
    }

    private protected override void Unload()
    {
        On.PlayMakerFSM.OnEnable -= OnFsmEnable;
        On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.OnEnter -= OnPlayerDataBoolTestAction;
        On.HeroController.CharmUpdate -= OnCharmUpdate;
        RestoreModifiedFsms();
        TryRestoreForcedHealth();
        QuickMenu.RefreshQuickMenuEntryColors();
    }

    private void OnPlayerDataBoolTestAction(
        On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.orig_OnEnter orig,
        HutongGames.PlayMaker.Actions.PlayerDataBoolTest self)
    {
        if (self.Fsm.Name == "Fury"
            && self.Fsm.GameObject.name == "Charm Effects"
            && self.State.Name == "Check HP")
        {
            if (!IsFuryEquipped())
            {
                orig(self);
                return;
            }

            FsmEvent? originalIsTrue = self.isTrue;
            try
            {
                self.isTrue = FsmEvent.GetFsmEvent("FURY");
                orig(self);
            }
            finally
            {
                self.isTrue = originalIsTrue;
            }

            return;
        }

        orig(self);
    }

    private void OnFsmEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (self.gameObject.name == "Charm Effects" && self.FsmName == "Fury")
        {
            TryModifyFuryFsm(self);
            TryApplyForcedHealth();
        }
    }

    private void OnCharmUpdate(On.HeroController.orig_CharmUpdate orig, HeroController self)
    {
        if (charmUpdateInProgress)
        {
            orig(self);
            return;
        }

        charmUpdateInProgress = true;
        try
        {
            orig(self);
        }
        finally
        {
            charmUpdateInProgress = false;
        }

        if (!Loaded)
        {
            return;
        }

        if (IsFuryEquipped())
        {
            TryModifyExistingFsm();
            TryApplyForcedHealth();
        }
        else
        {
            TryRestoreForcedHealth();
        }
    }

    private void TryModifyExistingFsm()
    {
        GameObject? charmEffects = GameObject.Find("Charm Effects");
        if (charmEffects == null)
        {
            return;
        }

        PlayMakerFSM? fsm = charmEffects.LocateMyFSM("Fury");
        if (fsm == null)
        {
            return;
        }

        TryModifyFuryFsm(fsm);
    }

    private static void TryModifyFuryFsm(PlayMakerFSM fsm)
    {
        FuryFsmSnapshot snapshot = GetOrCreateSnapshot(fsm);
        if (snapshot.Modified)
        {
            TryForceEnable(fsm);
            return;
        }

        if (!snapshot.HadEquipGlobalTransition)
        {
            fsm.AddFsmGlobalTransitions("CHARM EQUIP CHECK", "Check HP");
        }

        TrySetTransition(fsm, "Init", "FINISHED", "Check HP");
        TrySetTransition(fsm, "Check HP", "CANCEL", "Deactivate");
        TrySetTransition(fsm, "Activate", "HERO HEALED FULL", "Recheck");
        TrySetTransition(fsm, "Stay Furied", "HERO HEALED FULL", "Recheck");
        TrySetTransition(fsm, "Recheck", "FINISHED", "Stay Furied");
        snapshot.Modified = true;
        TryForceEnable(fsm);
    }

    private static FuryFsmSnapshot GetOrCreateSnapshot(PlayMakerFSM fsm)
    {
        int id = fsm.GetInstanceID();
        if (FurySnapshots.TryGetValue(id, out FuryFsmSnapshot? snapshot))
        {
            return snapshot;
        }

        snapshot = new FuryFsmSnapshot
        {
            Fsm = fsm,
            HadEquipGlobalTransition = HasGlobalTransition(fsm, "CHARM EQUIP CHECK"),
            InitFinished = GetTransitionTarget(fsm, "Init", "FINISHED"),
            CheckHpCancel = GetTransitionTarget(fsm, "Check HP", "CANCEL"),
            ActivateHealedFull = GetTransitionTarget(fsm, "Activate", "HERO HEALED FULL"),
            StayFuriedHealedFull = GetTransitionTarget(fsm, "Stay Furied", "HERO HEALED FULL"),
            RecheckFinished = GetTransitionTarget(fsm, "Recheck", "FINISHED"),
            Modified = false
        };

        FurySnapshots[id] = snapshot;
        return snapshot;
    }

    private static void RestoreModifiedFsms()
    {
        foreach ((int id, FuryFsmSnapshot snapshot) in FurySnapshots.ToArray())
        {
            PlayMakerFSM? fsm = snapshot.Fsm;
            if (fsm == null)
            {
                FurySnapshots.Remove(id);
                continue;
            }

            if (!snapshot.Modified)
            {
                continue;
            }

            if (!snapshot.HadEquipGlobalTransition)
            {
                TryRemoveGlobalTransition(fsm, "CHARM EQUIP CHECK");
            }

            TrySetTransition(fsm, "Init", "FINISHED", snapshot.InitFinished);
            TrySetTransition(fsm, "Check HP", "CANCEL", snapshot.CheckHpCancel);
            TrySetTransition(fsm, "Activate", "HERO HEALED FULL", snapshot.ActivateHealedFull);
            TrySetTransition(fsm, "Stay Furied", "HERO HEALED FULL", snapshot.StayFuriedHealedFull);
            TrySetTransition(fsm, "Recheck", "FINISHED", snapshot.RecheckFinished);
            TryForceRecheck(fsm);
            snapshot.Modified = false;
        }
    }

    private static bool HasGlobalTransition(PlayMakerFSM fsm, string eventName)
    {
        foreach (FsmTransition transition in fsm.Fsm.GlobalTransitions)
        {
            if (transition.EventName == eventName)
            {
                return true;
            }
        }

        return false;
    }

    private static string? GetTransitionTarget(PlayMakerFSM fsm, string stateName, string eventName)
    {
        FsmState? state = fsm.Fsm.GetState(stateName);
        if (state == null)
        {
            return null;
        }

        foreach (FsmTransition transition in state.Transitions)
        {
            if (transition.EventName == eventName)
            {
                return transition.ToState;
            }
        }

        return null;
    }

    private static void TrySetTransition(PlayMakerFSM fsm, string stateName, string eventName, string? targetState)
    {
        if (string.IsNullOrEmpty(targetState))
        {
            return;
        }

        try
        {
            fsm.ChangeFsmTransition(stateName, eventName, targetState);
        }
        catch
        {
            // ignore missing states or transitions
        }
    }

    private static void TryRemoveGlobalTransition(PlayMakerFSM fsm, string eventName)
    {
        try
        {
            fsm.RemoveFsmGlobalTransition(eventName);
        }
        catch
        {
            // ignore missing transition
        }
    }

    private static void TryForceRecheck(PlayMakerFSM fsm)
    {
        try
        {
            PlayerData? pd = PlayerData.instance;
            if (pd != null)
            {
                int totalHealth = pd.health + pd.healthBlue;
                if (totalHealth > 1 && fsm.Fsm.GetState("Deactivate") != null)
                {
                    fsm.Fsm.SetState("Deactivate");
                    return;
                }
            }

            if (fsm.Fsm.GetState("Check HP") != null)
            {
                fsm.Fsm.SetState("Check HP");
            }
        }
        catch
        {
            // ignore state issues
        }
    }

    private void TryApplyForcedHealth()
    {
        if (GearSwitcher.IsApplyingPreset)
        {
            RequestApplyWhenSafe();
            return;
        }

        if (savedHealth.HasValue || savedMaxHealth.HasValue || savedMaxHealthBase.HasValue)
        {
            return;
        }

        if (!IsFuryEquipped())
        {
            return;
        }

        if (!IsSafeToAdjustHealth())
        {
            RequestApplyWhenSafe();
            return;
        }

        if (Ref.HC == null)
        {
            return;
        }

        PlayerData? pd = PlayerData.instance;
        if (pd == null || pd.health <= 0)
        {
            return;
        }

        savedHealth = pd.health;
        savedMaxHealth = pd.maxHealth;
        savedMaxHealthBase = pd.maxHealthBase;
        SetMaxHealth(1, 1);
        SetNormalHealth(1);
    }

    private void TryRestoreForcedHealth()
    {
        if (GearSwitcher.IsApplyingPreset)
        {
            RequestRestoreWhenSafe();
            return;
        }

        if (!savedHealth.HasValue && !savedMaxHealth.HasValue && !savedMaxHealthBase.HasValue)
        {
            return;
        }

        if (!IsSafeToAdjustHealth())
        {
            RequestRestoreWhenSafe();
            return;
        }

        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            savedHealth = null;
            savedMaxHealth = null;
            savedMaxHealthBase = null;
            return;
        }

        int targetMaxBase = Math.Max(1, savedMaxHealthBase ?? pd.maxHealthBase);
        int targetMax = Math.Max(1, savedMaxHealth ?? pd.maxHealth);
        SetMaxHealth(targetMax, targetMaxBase);

        int targetHealth = savedHealth ?? pd.health;
        targetHealth = Math.Max(1, Math.Min(targetHealth, targetMax));

        savedHealth = null;
        savedMaxHealth = null;
        savedMaxHealthBase = null;
        SetNormalHealth(targetHealth);
    }

    private static void TryForceEnable(PlayMakerFSM fsm)
    {
        try
        {
            if (!IsFuryEquipped())
            {
                return;
            }

            if (!IsSafeToAdjustHealth())
            {
                return;
            }

            HeroController? hero = HeroController.instance;
            if (hero != null && !charmUpdateInProgress)
            {
                hero.CharmUpdate();
            }

            PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
            PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");

            if (fsm.Fsm.GetState("Check HP") != null)
            {
                fsm.SendEvent("CHARM EQUIP CHECK");
                return;
            }

            if (fsm.Fsm.GetState("Activate") != null)
            {
                fsm.Fsm.SetState("Activate");
            }
        }
        catch
        {
            // ignore state issues
        }
    }

    private static void SetNormalHealth(int value)
    {
        PlayerDataR.health = value;
        RequestHeroRefresh();
    }

    private static void SetMaxHealth(int maxHealth, int maxHealthBase)
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return;
        }

        pd.maxHealth = maxHealth;
        pd.maxHealthBase = maxHealthBase;
        RequestHeroRefresh();
        RequestHudRefresh();
    }

    private static void RequestHeroRefresh()
    {
        heroRefreshPending = true;
        EnsureRefreshCoroutine();
    }

    private static void RequestHudRefresh()
    {
        hudRefreshPending = true;
        EnsureRefreshCoroutine();
    }

    private static void EnsureRefreshCoroutine()
    {
        if (refreshCoroutineRunning)
        {
            return;
        }

        refreshCoroutineRunning = true;
        _ = GlobalCoroutineExecutor.Start(RefreshWhenSafe());
    }

    private static IEnumerator RefreshWhenSafe()
    {
        while (heroRefreshPending || hudRefreshPending)
        {
            if (IsSafeToAdjustHealth())
            {
                if (heroRefreshPending)
                {
                    heroRefreshPending = false;
                    TryRefreshHeroHealth();
                }

                if (hudRefreshPending)
                {
                    hudRefreshPending = false;
                    TryRefreshHudMasks();
                }
            }

            yield return null;
        }

        refreshCoroutineRunning = false;
    }

    private static void TryRefreshHudMasks()
    {
        try
        {
            if (!IsSafeToAdjustHealth())
            {
                return;
            }

            GameObject? hud = Ref.GC?.hudCanvas?.gameObject;
            if (hud == null)
            {
                return;
            }

            if (!hud.activeInHierarchy)
            {
                hud.SetActive(true);
                return;
            }

            hud.SetActive(false);
            hud.SetActive(true);
        }
        catch
        {
            // ignore HUD refresh issues
        }
    }

    private static void TryRefreshHeroHealth()
    {
        try
        {
            if (!IsSafeToAdjustHealth())
            {
                return;
            }

            Ref.HC?.proxyFSM?.SendEvent("HeroCtrl-HeroDamaged");
        }
        catch
        {
            // ignore update failures
        }
    }

    private static bool IsSafeToAdjustHealth()
    {
        if (GearSwitcher.IsApplyingPreset)
        {
            return false;
        }

        if (Ref.GM == null || Ref.GM.gameState != GameState.PLAYING)
        {
            return false;
        }

        HeroController? hero = Ref.HC;
        if (hero == null)
        {
            return false;
        }

        if (!hero.acceptingInput || hero.controlReqlinquished)
        {
            return false;
        }

        PlayerData? pd = PlayerData.instance;
        if (pd != null && pd.atBench)
        {
            return false;
        }

        return true;
    }

    private static void RequestApplyWhenSafe()
    {
        pendingApply = true;
        pendingRestore = false;
        EnsureHealthActionCoroutine();
    }

    private static void RequestRestoreWhenSafe()
    {
        pendingRestore = true;
        pendingApply = false;
        EnsureHealthActionCoroutine();
    }

    private static void EnsureHealthActionCoroutine()
    {
        if (healthActionCoroutineRunning)
        {
            return;
        }

        healthActionCoroutineRunning = true;
        _ = GlobalCoroutineExecutor.Start(ProcessPendingHealthActions());
    }

    private static IEnumerator ProcessPendingHealthActions()
    {
        while (pendingApply || pendingRestore)
        {
            if (GearSwitcher.IsApplyingPreset)
            {
                yield return null;
                continue;
            }

            if (IsSafeToAdjustHealth())
            {
                if (!ModuleManager.TryGetModule(typeof(AlwaysFurious), out Module? module))
                {
                    pendingApply = false;
                    pendingRestore = false;
                    break;
                }

                AlwaysFurious? alwaysFurious = module as AlwaysFurious;
                if (alwaysFurious == null)
                {
                    pendingApply = false;
                    pendingRestore = false;
                    break;
                }

                if (pendingRestore)
                {
                    pendingRestore = false;
                    alwaysFurious.TryRestoreForcedHealth();
                }
                else if (pendingApply)
                {
                    if (!alwaysFurious.Loaded || !IsFuryEquipped())
                    {
                        pendingApply = false;
                    }
                    else
                    {
                        pendingApply = false;
                        alwaysFurious.TryApplyForcedHealth();
                    }
                }
            }

            yield return null;
        }

        healthActionCoroutineRunning = false;
    }

    internal static void NotifyGearSwitcherApplied()
    {
        if (!ModuleManager.TryGetModule(typeof(AlwaysFurious), out Module? module))
        {
            return;
        }

        if (module is not AlwaysFurious alwaysFurious)
        {
            return;
        }

        if (!module.Enabled)
        {
            alwaysFurious.TryRestoreForcedHealth();
            return;
        }

        if (IsFuryEquipped())
        {
            alwaysFurious.ForceReapplyFromCurrent();
        }
        else
        {
            RequestRestoreWhenSafe();
        }
    }

    private void ForceReapplyFromCurrent()
    {
        savedHealth = null;
        savedMaxHealth = null;
        savedMaxHealthBase = null;

        if (!IsFuryEquipped())
        {
            return;
        }

        if (!IsSafeToAdjustHealth())
        {
            RequestApplyWhenSafe();
            return;
        }

        TryApplyForcedHealth();
    }

    private static bool IsFuryEquipped()
    {
        PlayerData? pd = PlayerData.instance;
        if (pd == null)
        {
            return false;
        }

        return CharmUtil.EquippedCharm(Charm.FuryOfTheFallen);
    }
}
