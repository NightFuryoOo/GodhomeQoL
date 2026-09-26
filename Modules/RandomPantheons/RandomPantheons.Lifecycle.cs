using System.Collections.Generic;
using System.Linq;
using Satchel.Futils;
using Vasi;
using Random = System.Random;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed partial class RandomPantheons : Module
{
    private protected override void Load()
    {
        ClearSequenceCaches();
        if (AnyPantheonEnabled)
        {
            _ = PantheonSequenceCompatibility.DisableTrueBossRush();
            _ = PantheonSequenceCompatibility.DisableSegmentedP5();
        }

        Instance = this;
        On.BossSequenceDoor.Start += CacheSequenceDoor;
        On.BossSequenceController.SetupNewSequence += ApplySequenceToStart;
        On.PlayMakerFSM.Start += ModifyRadiance;
        USceneManager.activeSceneChanged += OnSceneChange;
        CacheLiveSequenceDoors();
        if (AnyPantheonEnabled)
        {
            RefreshAllPantheons();
        }
    }

    private protected override void Unload()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        On.BossSequenceDoor.Start -= CacheSequenceDoor;
        On.BossSequenceController.SetupNewSequence -= ApplySequenceToStart;
        On.PlayMakerFSM.Start -= ModifyRadiance;
        USceneManager.activeSceneChanged -= OnSceneChange;
        RestoreAllKnownSequencesOnDisable();
        ClearSequenceCaches();
    }

    private void OnSceneChange(Scene prev, Scene next)
    {
        if (Ref.GM == null || !VanishedHud.Contains(prev.name))
        {
            return;
        }

        Ref.GM.StartCoroutine(EnsureHudVisible());
    }

    private static IEnumerator EnsureHudVisible()
    {
        yield return null;

        if (GameCameras.instance != null)
        {
            GameCameras.instance.hudCanvas.LocateMyFSM("Slide Out").SendEvent("IN");
        }
    }

    private static void ModifyRadiance(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
    {
        orig(self);

        if (self is { name: "Absolute Radiance", FsmName: "Control" }
            && PantheonSequenceCompatibility.ShouldApplyRandomPantheonsRadianceEndingSceneOverride())
        {
            self.GetAction<SetStaticVariable>("Ending Scene", 1).setValue.boolValue = false;
        }
    }
}
