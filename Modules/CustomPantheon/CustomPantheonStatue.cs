using Vasi;

namespace GodhomeQoL.Modules.BossChallenge;

internal static class CustomPantheonStatue
{
    internal const string WorkshopScene = "GG_Workshop";
    internal const string ReturnGateName = "door_dreamReturnGG_GodhomeQoLKnight";
    private const string ReturnGatePrefix = "door_dreamReturnGG";
    private const string AskKey = "GODHOMEQOL_CUSTOM_PANTHEON_ASK";
    private const string AskSheet = "Prompts";
    private const string StatueRootName = "GG_Statue_Knight";
    private const string CageName = "Knight Statue Cage";
    private const string TopVersionName = "Knight_v03";
    private const string InteractName = "Interact";
    private const string ConversationFsmName = "Conversation Control";
    private const string JournalStateName = "Journal";
    private const string TalkFinishStateName = "Talk Finish";
    private const string AskStateName = "GodhomeQoL Ask";
    private const string YesStateName = "GodhomeQoL Yes";
    private const string NoStateName = "GodhomeQoL No";
    private const float StandOffsetX = 1.6f;
    private const float StandOffsetY = 0.75f;
    private const float GateOffsetY = 1.05f;
    private const float AskDelaySeconds = 0.35f;

    private static bool installed;
    private static int askToken;

    internal sealed class ChallengeAssets
    {
        internal AudioClip? Unsheath;
        internal AudioClip? Impact;
        internal GameObject? DreamEffect;
    }

    private static CustomPantheonChain? SelectedChain => GodhomeQoL.GlobalSettings.CustomPantheon?.SelectedChain;

    internal static void Install()
    {
        if (installed)
        {
            return;
        }

        installed = true;
        On.BossStatueCompletionStates.Start += OnCompletionStatesStart;
        On.PlayMakerFSM.Start += OnFsmStart;
        ModHooks.LanguageGetHook += OnLanguageGet;
        ReturnGateGuard.Install();
    }

    internal static void Uninstall()
    {
        if (!installed)
        {
            return;
        }

        installed = false;
        On.BossStatueCompletionStates.Start -= OnCompletionStatesStart;
        On.PlayMakerFSM.Start -= OnFsmStart;
        ModHooks.LanguageGetHook -= OnLanguageGet;
        ReturnGateGuard.Uninstall();
    }

    private static string OnLanguageGet(string key, string sheetTitle, string orig) =>
        string.Equals(key, AskKey, StringComparison.Ordinal) ? "CustomPantheon/Ask".Localize() : orig;

    private static void OnCompletionStatesStart(On.BossStatueCompletionStates.orig_Start orig, BossStatueCompletionStates self)
    {
        if (!IsKnightStatuePart(self) || GetEarnedTier(self) == BossStatueCompletionStates.Tiers.Tier3)
        {
            orig(self);
            return;
        }

        try
        {
            for (int i = 0; i < self.tierStates.Length; i++)
            {
                self.tierStates[i].SetActive(false);
            }

            self.tierStates[(int)BossStatueCompletionStates.Tiers.Tier3].SetActive(true);
        }
        catch (Exception ex)
        {
            LogError($"[CustomPantheon] Showing the Knight statue failed: {ex}");
            orig(self);
        }
    }

    private static bool IsKnightStatuePart(BossStatueCompletionStates states) =>
        states != null
        && states.tierStates != null
        && states.tierStates.Length > (int)BossStatueCompletionStates.Tiers.Tier3
        && string.Equals(states.gameObject.scene.name, WorkshopScene, StringComparison.Ordinal)
        && (string.Equals(states.gameObject.name, CageName, StringComparison.Ordinal) || HasAncestor(states.transform, StatueRootName));

    private static bool HasAncestor(Transform? transform, string name)
    {
        for (Transform? current = transform; current != null; current = current.parent)
        {
            if (string.Equals(current.name, name, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static BossStatueCompletionStates.Tiers? GetEarnedTier(BossStatueCompletionStates? states)
    {
        if (states == null)
        {
            return null;
        }

        try
        {
            return states.GetHighestCompletedTier();
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "CustomPantheonStatue.cs/Tier");
            return null;
        }
    }

    private static void OnFsmStart(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);
        if (!string.Equals(self.FsmName, ConversationFsmName, StringComparison.Ordinal) || !IsTopInteract(self.gameObject))
        {
            return;
        }

        try
        {
            PatchConversation(self);
        }
        catch (Exception ex)
        {
            LogError($"[CustomPantheon] Patching the Knight statue failed: {ex}");
        }
    }

    private static bool IsTopInteract(GameObject go) =>
        string.Equals(go.name, InteractName, StringComparison.Ordinal)
        && go.transform.parent != null
        && string.Equals(go.transform.parent.name, TopVersionName, StringComparison.Ordinal)
        && string.Equals(go.scene.name, WorkshopScene, StringComparison.Ordinal);

    private static void PatchConversation(PlayMakerFSM fsm)
    {
        if (FindState(fsm, AskStateName) != null)
        {
            return;
        }

        FsmState? journal = FindState(fsm, JournalStateName);
        FsmState? talkFinish = FindState(fsm, TalkFinishStateName);
        if (journal == null || talkFinish == null)
        {
            return;
        }

        GameObject? journalMessage = journal.Actions.OfType<CreateObject>().FirstOrDefault()?.gameObject?.Value;
        journal.InsertAction(0, new JournalGuard(journalMessage));

        FsmState ask = fsm.CreateState(AskStateName);
        FsmState yes = fsm.CreateState(YesStateName);
        FsmState no = fsm.CreateState(NoStateName);
        ask.InsertAction(0, new AskAction());
        yes.InsertMethod(0, () => OnYes(fsm));
        no.InsertMethod(0, CloseYesNo);

        AddTransition(ask, "FINISHED", talkFinish);
        AddTransition(ask, "YES", yes);
        AddTransition(ask, "NO", no);
        AddTransition(no, "FINISHED", talkFinish);
        Redirect(journal, "FINISHED", ask);
    }

    private static FsmState? FindState(PlayMakerFSM fsm, string name) =>
        fsm.FsmStates.FirstOrDefault(state => string.Equals(state.Name, name, StringComparison.Ordinal));

    private static void AddTransition(FsmState state, string eventName, FsmState target)
    {
        FsmEvent fsmEvent = FsmEvent.GetFsmEvent(eventName) ?? new FsmEvent(eventName);
        FsmTransition[] current = state.Transitions ?? Array.Empty<FsmTransition>();
        FsmTransition[] updated = new FsmTransition[current.Length + 1];
        Array.Copy(current, updated, current.Length);
        updated[current.Length] = new FsmTransition { FsmEvent = fsmEvent, ToState = target.Name, ToFsmState = target };
        state.Transitions = updated;
    }

    private static void Redirect(FsmState state, string eventName, FsmState target)
    {
        foreach (FsmTransition transition in state.Transitions)
        {
            if (string.Equals(transition.EventName, eventName, StringComparison.Ordinal))
            {
                transition.ToState = target.Name;
                transition.ToFsmState = target;
            }
        }
    }

    private sealed class JournalGuard : FsmStateAction
    {
        private readonly GameObject? message;

        internal JournalGuard(GameObject? message)
        {
            this.message = message;
        }

        public override void OnEnter()
        {
            BossStatueCompletionStates.Tiers? earned = GetEarnedTier(Owner != null ? Owner.GetComponentInParent<BossStatueCompletionStates>() : null);
            if (earned == BossStatueCompletionStates.Tiers.Tier3)
            {
                Finish();
                return;
            }

            try
            {
                if (earned.HasValue)
                {
                    GrantJournal((int)earned.Value + 1, message);
                }
            }
            catch (Exception ex)
            {
                LogSuppressed(ex, "CustomPantheonStatue.cs/Journal");
            }

            Finish();
            Fsm.Event(FsmEvent.Finished);
        }
    }

    private static void GrantJournal(int tier, GameObject? message)
    {
        PlayerData? data = PlayerData.instance;
        if (data == null || data.GetBool("bossRushMode") || data.GetBool($"killedVoidIdol_{tier}"))
        {
            return;
        }

        if (message != null)
        {
            UObject.Instantiate(message, Vector3.zero, Quaternion.identity);
        }

        if (tier == 2)
        {
            data.SetBool("killedVoidIdol_1", false);
        }

        data.SetBool($"killedVoidIdol_{tier}", true);
        data.SetInt($"killsVoidIdol_{tier}", 0);
        data.SetBool($"newDataVoidIdol_{tier}", true);
    }

    private sealed class AskAction : FsmStateAction
    {
        public override void OnEnter()
        {
            if (!CustomPantheonRunner.CanOfferStart(SelectedChain))
            {
                Finish();
                return;
            }

            int token = ++askToken;
            SendDialogueEvent("BOX DOWN");
            _ = GlobalCoroutineExecutor.Start(OpenAskLater(Fsm.FsmComponent, Owner, token));
        }
    }

    private static IEnumerator OpenAskLater(PlayMakerFSM fsm, GameObject requester, int token)
    {
        yield return new WaitForSecondsRealtime(AskDelaySeconds);
        if (token != askToken || fsm == null || !string.Equals(fsm.ActiveStateName, AskStateName, StringComparison.Ordinal))
        {
            yield break;
        }

        if (!OpenYesNo(requester))
        {
            fsm.SendEvent("NO");
        }
    }

    private static void OnYes(PlayMakerFSM fsm)
    {
        CloseYesNo();
        if (CustomPantheonRunner.TryStartAtStatue(SelectedChain, fsm, out string messageKey))
        {
            return;
        }

        Tools.QuickMenu.ShowStatusMessageFromExternal(messageKey.Localize());
        fsm.SendEvent("TALK FINISH");
    }

    private static GameObject? GlobalObject(string name)
    {
        try
        {
            return FsmVariables.GlobalVariables.FindFsmGameObject(name)?.Value;
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "CustomPantheonStatue.cs/Global");
            return null;
        }
    }

    private static GameObject? DialogueManager() => GlobalObject("DialogueManager") ?? GameObject.Find("DialogueManager");

    private static void SendDialogueEvent(string eventName)
    {
        GameObject? manager = DialogueManager();
        if (manager != null)
        {
            FSMUtility.SendEventToGameObject(manager, eventName, false);
        }
    }

    private static bool OpenYesNo(GameObject requester)
    {
        GameObject? manager = DialogueManager();
        GameObject? textYesNo = GlobalObject("DialogueTextYN") ?? GameObject.Find("Text YN");
        if (manager == null || textYesNo == null)
        {
            return false;
        }

        PlayMakerFSM? page = textYesNo.LocateMyFSM("Dialogue Page Control");
        PlayMakerFSM? opener = manager.LocateMyFSM("Box Open YN");
        DialogueBox? box = textYesNo.GetComponent<DialogueBox>();
        FsmGameObject? requesterVariable = page != null ? page.FsmVariables.FindFsmGameObject("Requester") : null;
        if (page == null || opener == null || box == null || requesterVariable == null)
        {
            return false;
        }

        FsmInt? cost = page.FsmVariables.FindFsmInt("Toll Cost");
        if (cost != null)
        {
            cost.Value = 0;
        }

        requesterVariable.Value = requester;
        opener.SendEvent("BOX UP YN");
        box.StartConversation(AskKey, AskSheet);
        return true;
    }

    private static void CloseYesNo()
    {
        GameObject? manager = DialogueManager();
        PlayMakerFSM? opener = manager != null ? manager.LocateMyFSM("Box Open YN") : null;
        if (opener != null)
        {
            opener.SendEvent("BOX DOWN YN");
        }
    }

    internal static Transform? FindTopInteract()
    {
        GameObject? root = GameObject.Find(StatueRootName);
        if (root == null)
        {
            return null;
        }

        Transform? version = root.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(child => string.Equals(child.name, TopVersionName, StringComparison.Ordinal));
        return version != null ? version.Find(InteractName) : null;
    }

    internal static bool TryGetStandPoint(out Vector3 position, out float statueX)
    {
        Transform? interact = FindTopInteract();
        if (interact == null)
        {
            position = Vector3.zero;
            statueX = 0f;
            return false;
        }

        Vector3 anchor = interact.position;
        float z = HeroController.instance != null ? HeroController.instance.transform.position.z : 0f;
        position = new Vector3(anchor.x - StandOffsetX, anchor.y - StandOffsetY, z);
        statueX = anchor.x;
        return true;
    }

    internal static void DisableStatuePrompt()
    {
        Transform? interact = FindTopInteract();
        if (interact != null)
        {
            FSMUtility.SendEventToGameObject(interact.gameObject, "NPC CONTROL OFF", false);
        }
    }

    internal static string? PrepareReturnGate(bool victory)
    {
        Transform? interact = FindTopInteract();
        if (interact == null)
        {
            return null;
        }

        Scene scene = interact.gameObject.scene;
        TransitionPoint? existing = TransitionPoint.TransitionPoints.FirstOrDefault(point =>
            point != null && point.gameObject.scene == scene && string.Equals(point.name, ReturnGateName, StringComparison.Ordinal));
        if (existing != null)
        {
            PlayMakerFSM? existingFsm = existing.gameObject.LocateMyFSM("Return from boss");
            if (existingFsm != null)
            {
                existingFsm.enabled = !victory;
            }

            return ReturnGateName;
        }

        Vector3 anchor = interact.position;
        TransitionPoint? source = TransitionPoint.TransitionPoints
            .Where(point => point != null
                && point.gameObject.activeInHierarchy
                && point.gameObject.scene == scene
                && point.name.StartsWith(ReturnGatePrefix, StringComparison.Ordinal))
            .OrderBy(point => Mathf.Abs(point.transform.position.y - anchor.y))
            .ThenBy(point => Mathf.Abs(point.transform.position.x - anchor.x))
            .FirstOrDefault();
        if (source == null)
        {
            return null;
        }

        GameObject clone = UObject.Instantiate(source.gameObject, source.transform.parent);
        clone.name = ReturnGateName;
        clone.transform.position = new Vector3(anchor.x - StandOffsetX, anchor.y - GateOffsetY, source.transform.position.z);
        PlayMakerFSM? returnFsm = clone.LocateMyFSM("Return from boss");
        FsmGameObject? inspect = returnFsm != null ? returnFsm.FsmVariables.FindFsmGameObject("Inspect") : null;
        if (inspect != null)
        {
            inspect.Value = interact.gameObject;
        }

        if (returnFsm != null && victory)
        {
            returnFsm.enabled = false;
        }

        return ReturnGateName;
    }

    internal static string? FindNearestReturnGate()
    {
        Transform? interact = FindTopInteract();
        Vector3 anchor = interact != null ? interact.position : Vector3.zero;
        TransitionPoint? nearest = TransitionPoint.TransitionPoints
            .Where(point => point != null
                && point.gameObject.activeInHierarchy
                && string.Equals(point.gameObject.scene.name, WorkshopScene, StringComparison.Ordinal)
                && point.name.StartsWith(ReturnGatePrefix, StringComparison.Ordinal))
            .OrderBy(point => (point.transform.position - anchor).sqrMagnitude)
            .FirstOrDefault();
        return nearest != null ? nearest.name : null;
    }

    internal static ChallengeAssets FindChallengeAssets()
    {
        ChallengeAssets assets = new();
        try
        {
            PlayMakerFSM? ui = UObject.FindObjectsOfType<PlayMakerFSM>()
                .FirstOrDefault(fsm => fsm != null && string.Equals(fsm.FsmName, "GG Boss UI", StringComparison.Ordinal));
            if (ui == null)
            {
                return assets;
            }

            assets.Unsheath = FindState(ui, "Challenge Audio")?.Actions.OfType<AudioPlay>().FirstOrDefault()?.oneShotClip?.Value as AudioClip;
            assets.DreamEffect = FindState(ui, "Impact")?.Actions.OfType<CreateObject>().FirstOrDefault()?.gameObject?.Value;
            AudioSource? source = ui.GetComponent<AudioSource>();
            assets.Impact = source != null ? source.clip : null;
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "CustomPantheonStatue.cs/Assets");
        }

        return assets;
    }

    internal static void FadeBlanker(bool white, bool show, float seconds)
    {
        GameObject? blanker = GlobalObject(white ? "HUD Blanker White" : "HUD Blanker");
        PlayMakerFSM? control = blanker != null ? blanker.LocateMyFSM("Blanker Control") : null;
        if (control == null)
        {
            return;
        }

        FsmFloat? fadeTime = control.FsmVariables.FindFsmFloat("Fade Time");
        if (fadeTime != null)
        {
            fadeTime.Value = seconds;
        }

        control.SendEvent(show ? "FADE IN" : "FADE OUT");
    }

    internal static bool IsBlankerVisible(bool white)
    {
        GameObject? blanker = GlobalObject(white ? "HUD Blanker White" : "HUD Blanker");
        if (blanker == null || !blanker.activeInHierarchy)
        {
            return false;
        }

        SpriteRenderer? sprite = blanker.GetComponent<SpriteRenderer>();
        return sprite == null || (sprite.enabled && sprite.color.a > 0.05f);
    }
}
