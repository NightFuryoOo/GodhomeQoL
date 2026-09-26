using HutongGames.PlayMaker.Actions;
using Modding;
using Satchel.Futils;
using Satchel;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class UumuuHelper : UumuuHelperBase
{
    private const string UumuuScene = "GG_Uumuu_V";
    private const string UumuuName = "Mega Jellyfish GG";
    private const string UumuuSummonNameHint = "Jellyfish";
    private const string UumuuSpawnerNameHint = "Spawner";
    private const string UumuuMegaNameHint = "Mega Jellyfish";
    private const int DefaultUumuuMaxHp = 700;
    private const int DefaultUumuuSummonHp = 1;
    private const int DefaultUumuuVanillaHp = 700;
    private const int P5UumuuHp = 350;
    private const int MinUumuuHp = 1;
    private const int MaxUumuuHp = 999999;

    [LocalSetting]
    [BoolOption]
    internal static bool uumuuUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool uumuuP5Hp = false;

    [LocalSetting]
    internal static int uumuuMaxHp = DefaultUumuuMaxHp;

    [LocalSetting]
    internal static bool uumuuUseCustomSummonHp = false;

    [LocalSetting]
    internal static int uumuuSummonHp = DefaultUumuuSummonHp;

    [LocalSetting]
    internal static int uumuuMaxHpBeforeP5 = DefaultUumuuMaxHp;

    [LocalSetting]
    internal static bool uumuuUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool uumuuUseCustomSummonHpBeforeP5 = false;

    [LocalSetting]
    internal static int uumuuSummonHpBeforeP5 = DefaultUumuuSummonHp;

    [LocalSetting]
    internal static bool uumuuHasStoredStateBeforeP5 = false;

    private static UumuuHelper? instance;

    public UumuuHelper() => instance = this;

    private protected override string SceneName => UumuuScene;
    private protected override string BossObjectName => UumuuName;
    private protected override int DefaultVanillaHp => DefaultUumuuVanillaHp;
    private protected override int MinHp => MinUumuuHp;
    private protected override int MaxHpLimit => MaxUumuuHp;
    private protected override bool UseMaxHp { get => uumuuUseMaxHp; set => uumuuUseMaxHp = value; }
    private protected override int MaxHp { get => uumuuMaxHp; set => uumuuMaxHp = value; }
    private protected override int SummonHp { get => uumuuSummonHp; set => uumuuSummonHp = value; }

    private protected override void Load()
    {
        moduleActive = true;
        BossManipulateEntryGuard.EnsureHooks();
        NormalizeP5State();
        vanillaHpByInstance.Clear();
        vanillaSummonHpByInstance.Clear();
        On.SetHP.OnEnter += OnSetHpEnter;
        On.HealthManager.OnEnable += OnHealthManagerOnEnable;
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private void NormalizeP5State()
    {
        if (!uumuuP5Hp)
        {
            return;
        }

        if (!uumuuHasStoredStateBeforeP5)
        {
            uumuuMaxHpBeforeP5 = ClampBossHp(uumuuMaxHp);
            uumuuUseMaxHpBeforeP5 = uumuuUseMaxHp;
            uumuuUseCustomSummonHpBeforeP5 = uumuuUseCustomSummonHp;
            uumuuSummonHpBeforeP5 = ClampBossHp(uumuuSummonHp);
            uumuuHasStoredStateBeforeP5 = true;
        }

        uumuuUseMaxHp = true;
        uumuuUseCustomSummonHp = false;
        uumuuMaxHp = P5UumuuHp;
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

        return hoGEntryAllowed && string.Equals(USceneManager.GetActiveScene().name, UumuuScene, StringComparison.Ordinal);
    }

    private protected override bool ShouldUseCustomSummonHp() => uumuuUseCustomSummonHp && !uumuuP5Hp;

    private void SetP5HpEnabledCore(bool value)
    {
        if (value)
        {
            if (!uumuuP5Hp)
            {
                uumuuMaxHpBeforeP5 = ClampBossHp(uumuuMaxHp);
                uumuuUseMaxHpBeforeP5 = uumuuUseMaxHp;
                uumuuUseCustomSummonHpBeforeP5 = uumuuUseCustomSummonHp;
                uumuuSummonHpBeforeP5 = ClampBossHp(uumuuSummonHp);
                uumuuHasStoredStateBeforeP5 = true;
            }

            uumuuP5Hp = true;
            uumuuUseMaxHp = true;
            uumuuUseCustomSummonHp = false;
            uumuuMaxHp = P5UumuuHp;
        }
        else
        {
            if (uumuuP5Hp && uumuuHasStoredStateBeforeP5)
            {
                uumuuMaxHp = ClampBossHp(uumuuMaxHpBeforeP5);
                uumuuUseMaxHp = uumuuUseMaxHpBeforeP5;
                uumuuUseCustomSummonHp = uumuuUseCustomSummonHpBeforeP5;
                uumuuSummonHp = ClampBossHp(uumuuSummonHpBeforeP5);
            }

            uumuuP5Hp = false;
            uumuuHasStoredStateBeforeP5 = false;
        }

        ReapplyLiveSettingsCore();
    }

    internal static void ReapplyLiveSettings() => instance?.ReapplyLiveSettingsCore();

    internal static void SetP5HpEnabled(bool value) => instance?.SetP5HpEnabledCore(value);

    internal static void ApplyUumuuHealthIfPresent() => instance?.ApplyHealthIfPresentCore();

    internal static void RestoreVanillaHealthIfPresent() => instance?.RestoreVanillaHealthIfPresentCore();

    internal static void ApplyUumuuSummonHealthIfPresent() => instance?.ApplySummonHealthIfPresentCore();

    internal static void RestoreVanillaSummonHealthIfPresent() => instance?.RestoreVanillaSummonHealthIfPresentCore();
}
