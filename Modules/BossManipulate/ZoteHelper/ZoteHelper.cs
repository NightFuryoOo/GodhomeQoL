using Modding;
using Satchel;
using Satchel.Futils;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class ZoteHelper : Module
{
    private const string ZoteScene = "GG_Grey_Prince_Zote";
    private const string PatchFlag = "GodhomeQoL_ZoteHelper_Patched";
    private const int DefaultSummonFlyingHp = 20;
    private const int DefaultSummonHoppingHp = 20;
    private const int DefaultSummonLimit = 3;
    private const int MinSummonHp = 0;
    private const int MaxSummonHp = 99;

    [LocalSetting]
    internal static int zoteBossHp = DefaultBossHp;

    [LocalSetting]
    [BoolOption]
    internal static bool zoteUseCustomBossHp = true;

    [LocalSetting]
    internal static bool zoteImmortal = false;

    [LocalSetting]
    internal static bool zoteSpawnFlying = true;

    [LocalSetting]
    internal static bool zoteSpawnHopping = true;

    [LocalSetting]
    internal static int zoteSummonFlyingHp = DefaultSummonFlyingHp;

    [LocalSetting]
    [BoolOption]
    internal static bool zoteUseCustomFlyingHp = true;

    [LocalSetting]
    internal static int zoteSummonHoppingHp = DefaultSummonHoppingHp;

    [LocalSetting]
    [BoolOption]
    internal static bool zoteUseCustomHoppingHp = true;

    [LocalSetting]
    internal static int zoteSummonLimit = DefaultSummonLimit;

    [LocalSetting]
    [BoolOption]
    internal static bool zoteUseCustomSummonLimit = false;

    [LocalSetting]
    [BoolOption]
    internal static bool zoteDoubleSummons = false;

    [LocalSetting]
    [BoolOption]
    internal static bool zoteAttackSummonsOnlyOnStart = false;

    [LocalSetting]
    internal static bool ZoteHoGOnly = true;

    private static readonly List<GameObject> activeZotelings = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override bool DefaultEnabled => false;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private enum ZotelingType
    {
        Hopping,
        Flying
    }

    private sealed class ZotelingTypeMarker : MonoBehaviour
    {
        public ZotelingType Type;
        public bool TypeSet;
        public bool HealthInitialized;
    }

    private sealed class ZotelingTracker : MonoBehaviour
    {
        private void OnDestroy()
        {
            UntrackZoteling(gameObject);
        }
    }

    private static bool IsPatched(PlayMakerFSM fsm)
    {
        FsmBool? flag = FindFsmBool(fsm, PatchFlag);
        return flag != null && flag.Value;
    }

    private static void MarkPatched(PlayMakerFSM fsm)
    {
        FsmBool? flag = FindFsmBool(fsm, PatchFlag);
        if (flag == null)
        {
            flag = new FsmBool(PatchFlag);
            fsm.FsmVariables.BoolVariables = fsm.FsmVariables.BoolVariables.Append(flag).ToArray();
        }

        flag.Value = true;
    }

    private static FsmBool? FindFsmBool(PlayMakerFSM fsm, string name)
    {
        foreach (FsmBool variable in fsm.FsmVariables.BoolVariables)
        {
            if (variable.Name == name)
            {
                return variable;
            }
        }

        return null;
    }

    private static int ClampSummonHp(int value)
    {
        if (value < MinSummonHp)
        {
            return MinSummonHp;
        }

        return value > MaxSummonHp ? MaxSummonHp : value;
    }

    private static FsmGameObject GetFsmGameObjectVariable(PlayMakerFSM fsm, string name)
    {
        foreach (FsmGameObject variable in fsm.FsmVariables.GameObjectVariables)
        {
            if (variable.Name == name)
            {
                return variable;
            }
        }

        FsmGameObject created = new(name);
        fsm.FsmVariables.GameObjectVariables = fsm.FsmVariables.GameObjectVariables.Append(created).ToArray();
        return created;
    }
}
