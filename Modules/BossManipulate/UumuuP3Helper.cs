using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class UumuuP3Helper : UumuuHelperBase
{
    private const string UumuuScene = "GG_Uumuu";
    private const string UumuuName = "Mega Jellyfish GG";
    private const string UumuuSummonNameHint = "Jellyfish";
    private const string UumuuSpawnerNameHint = "Spawner";
    private const string UumuuMegaNameHint = "Mega Jellyfish";
    private const int DefaultUumuuMaxHp = 350;
    private const int DefaultUumuuSummonHp = 1;
    private const int DefaultUumuuVanillaHp = 350;
    private const int MinUumuuHp = 1;
    private const int MaxUumuuHp = 999999;

    [LocalSetting]
    [BoolOption]
    internal static bool uumuuP3UseMaxHp = false;

    [LocalSetting]
    internal static int uumuuP3MaxHp = DefaultUumuuMaxHp;

    [LocalSetting]
    [BoolOption]
    internal static bool uumuuP3UseCustomSummonHp = false;

    [LocalSetting]
    internal static int uumuuP3SummonHp = DefaultUumuuSummonHp;

    private static UumuuP3Helper? instance;

    public UumuuP3Helper() => instance = this;

    private protected override string SceneName => UumuuScene;
    private protected override string BossObjectName => UumuuName;
    private protected override int DefaultVanillaHp => DefaultUumuuVanillaHp;
    private protected override int MinHp => MinUumuuHp;
    private protected override int MaxHpLimit => MaxUumuuHp;
    private protected override bool UseMaxHp { get => uumuuP3UseMaxHp; set => uumuuP3UseMaxHp = value; }
    private protected override int MaxHp { get => uumuuP3MaxHp; set => uumuuP3MaxHp = value; }
    private protected override int SummonHp { get => uumuuP3SummonHp; set => uumuuP3SummonHp = value; }

    private protected override void Load()
    {
        moduleActive = true;
        BossManipulateEntryGuard.EnsureHooks();
        vanillaHpByInstance.Clear();
        vanillaSummonHpByInstance.Clear();
        On.SetHP.OnEnter += OnSetHpEnter;
        On.HealthManager.OnEnable += OnHealthManagerOnEnable;
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override bool IsBossSummonObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        string name = gameObject.name;
        if (name.StartsWith(UumuuName, StringComparison.Ordinal))
        {
            return false;
        }

        if (name.IndexOf(UumuuMegaNameHint, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return false;
        }

        if (name.IndexOf(UumuuSpawnerNameHint, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return false;
        }

        if (name.IndexOf(UumuuSummonNameHint, StringComparison.OrdinalIgnoreCase) < 0)
        {
            return false;
        }

        if (string.Equals(gameObject.scene.name, UumuuScene, StringComparison.Ordinal))
        {
            return true;
        }

        return string.Equals(USceneManager.GetActiveScene().name, UumuuScene, StringComparison.Ordinal);
    }

    private protected override bool ShouldUseCustomSummonHp() => uumuuP3UseCustomSummonHp;

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void ApplyUumuuHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyUumuuSummonHealthIfPresent() => instance?.ApplySummonHealthIfPresentCore();

    internal static void RestoreVanillaSummonHealthIfPresent() => instance?.RestoreVanillaSummonHealthIfPresentCore();
}
