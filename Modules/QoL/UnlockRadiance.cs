<<<<<<< HEAD
namespace GodhomeQoL.Modules.QoL;
=======
﻿namespace SafeGodseekerQoL.Modules.QoL;
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265

public sealed class UnlockRadiance : Module
{
    private const string SceneName = "Radiance Boss Scene";

    public override bool DefaultEnabled => true;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ReloadSave;

    private protected override void Load() =>
        On.HeroController.Start += SetRadianceUnlocked;

    private protected override void Unload() =>
        On.HeroController.Start -= SetRadianceUnlocked;

    private static void SetRadianceUnlocked(On.HeroController.orig_Start orig, HeroController self)
    {
        orig(self);

        if (PlayerDataR.bossRushMode && !PlayerDataR.unlockedBossScenes.Contains(SceneName))
        {
            PlayerDataR.unlockedBossScenes.Add(SceneName);
            Ref.SD.persistentBoolItems.Set("GG_Workshop", "Radiance Statue Cage", true);
            LogDebug("Radiance unlocked");
        }
    }
}
