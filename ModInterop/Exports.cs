using MonoMod.ModInterop;

using GodhomeQoL.Modules.BossChallenge;
using GodhomeQoL.Modules.QoL;
using System;

namespace GodhomeQoL.ModInterop;

[ModExportName(nameof(GodhomeQoL))]
public static class Exports
{
    public static int SuppressModules(string suppressor, params string[] modules)
    {
        try { return ModuleManager.SuppressModules(suppressor, modules); } catch (Exception ex) { Logger.Log(ex.Message); return 0; }
    }

    public static void CancelSuppression(int handle)
    {
        try
        {
            ModuleManager.CancelSuppression(handle);
        }
        catch (Exception ex) { Logger.Log(ex.Message); }
    }

    //public static void AddFastDashPredicate(Func<Scene, Scene, bool> predicate) =>
    //	FastDash.predicates.Add(predicate);

    public static void AddInfiniteChallengeReturnScenePredicate(Func<GameManager.SceneLoadInfo, bool> predicate)
    {
        _ = AddInfiniteChallengeReturnScenePredicateHandle(predicate);
    }

    public static int AddInfiniteChallengeReturnScenePredicateHandle(Func<GameManager.SceneLoadInfo, bool> predicate)
    {
        try
        {
            return InfiniteChallenge.AddReturnScenePredicate(predicate);
        }
        catch (Exception ex)
        {
            Logger.Log(ex.Message);
            return 0;
        }
    }

    public static void RemoveInfiniteChallengeReturnScenePredicate(int handle)
    {
        try
        {
            InfiniteChallenge.RemoveReturnScenePredicate(handle);
        }
        catch (Exception ex) { Logger.Log(ex.Message); }
    }

    public static bool RemoveInfiniteChallengeReturnScenePredicateByDelegate(Func<GameManager.SceneLoadInfo, bool> predicate)
    {
        try
        {
            return InfiniteChallenge.RemoveReturnScenePredicate(predicate) > 0;
        }
        catch (Exception ex)
        {
            Logger.Log(ex.Message);
            return false;
        }
    }
}
