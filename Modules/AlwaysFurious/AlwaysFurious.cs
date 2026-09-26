using GodhomeQoL.Modules.Tools;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class AlwaysFurious : Module
{
    private int? savedHealth;
    private int? savedMaxHealth;
    private int? savedMaxHealthBase;
    private bool wasFuryEquippedOnLastCharmUpdate;
    private static bool charmUpdateInProgress;
    private static bool heroRefreshPending;
    private static bool hudRefreshPending;
    private static bool refreshCoroutineRunning;
    private static bool pendingApply;
    private static bool pendingRestore;
    private static bool healthActionCoroutineRunning;
    private static bool disablingFromMainMenuTransition;
    private const string MainMenuSceneName = "Menu_Title";

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
}
