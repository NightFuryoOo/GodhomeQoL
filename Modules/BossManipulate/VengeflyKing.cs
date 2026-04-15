using Modding;
using Satchel;
using Satchel.Futils;
using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GodhomeQoL.Modules.BossChallenge;

public sealed class VengeflyKing : Module
{
    private const string VengeflyScene = "GG_Vengefly_V";
    private const string HoGWorkshopScene = "GG_Workshop";
    private const string LeftVengeflyName = "Giant Buzzer Col (1)";
    private const string RightVengeflyName = "Giant Buzzer Col";

    private const int DefaultLeftMaxHp = 750;
    private const int DefaultRightMaxHp = 430;
    private const int DefaultSummonMaxHp = 8;
    private const int DefaultSummonLimit = 4;
    private const int DefaultSummonAttackLimit = 15;
    private const int P5LeftHp = 450;
    private const int P5RightHp = 190;
    private const int P5SummonHp = 8;
    private const int MinHp = 1;
    private const int MaxHp = 999999;
    private const int MinSummonLimit = 0;
    private const int MaxSummonLimit = 999;
    private const int MinSummonAttackLimit = 0;
    private const int MaxSummonAttackLimit = 999;

    [LocalSetting]
    [BoolOption]
    internal static bool vengeflyKingUseMaxHp = false;

    [LocalSetting]
    [BoolOption]
    internal static bool vengeflyKingP5Hp = false;

    [LocalSetting]
    internal static int vengeflyKingLeftMaxHp = DefaultLeftMaxHp;

    [LocalSetting]
    internal static int vengeflyKingRightMaxHp = DefaultRightMaxHp;

    [LocalSetting]
    internal static int vengeflyKingSummonMaxHp = DefaultSummonMaxHp;

    [LocalSetting]
    internal static bool vengeflyKingUseCustomSummonLimit = false;

    [LocalSetting]
    internal static int vengeflyKingLeftSummonLimit = DefaultSummonLimit;

    [LocalSetting]
    internal static int vengeflyKingRightSummonLimit = DefaultSummonLimit;

    [LocalSetting]
    internal static int vengeflyKingLeftSummonAttackLimit = DefaultSummonAttackLimit;

    [LocalSetting]
    internal static int vengeflyKingRightSummonAttackLimit = DefaultSummonAttackLimit;

    [LocalSetting]
    internal static int vengeflyKingLeftMaxHpBeforeP5 = DefaultLeftMaxHp;

    [LocalSetting]
    internal static int vengeflyKingRightMaxHpBeforeP5 = DefaultRightMaxHp;

    [LocalSetting]
    internal static int vengeflyKingSummonMaxHpBeforeP5 = DefaultSummonMaxHp;

    [LocalSetting]
    internal static bool vengeflyKingUseMaxHpBeforeP5 = false;

    [LocalSetting]
    internal static bool vengeflyKingUseCustomSummonLimitBeforeP5 = false;

    [LocalSetting]
    internal static bool vengeflyKingHasStoredStateBeforeP5 = false;

    private enum VengeflySide
    {
        Unknown = 0,
        Left = 1,
        Right = 2,
        Summon = 3,
    }

    private static readonly Dictionary<int, int> vanillaHpByInstance = new();
    private static readonly Dictionary<int, int> vanillaSummonAttackLimitByFsm = new();
    private static bool moduleActive;
    private static bool hoGEntryAllowed;

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;

    private protected override void Load()
    {
        moduleActive = true;
        NormalizeP5State();
        vanillaHpByInstance.Clear();
        vanillaSummonAttackLimitByFsm.Clear();
        On.HealthManager.Awake += OnHealthManagerAwake;
        On.HealthManager.Start += OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable += OnPlayMakerFsmOnEnable;
        USceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook += OnBeforeSceneLoad;
    }

    private protected override void Unload()
    {
        RestoreVanillaHealthIfPresent();
        RestoreVanillaSummonLimitsIfPresent();
        moduleActive = false;
        On.HealthManager.Awake -= OnHealthManagerAwake;
        On.HealthManager.Start -= OnHealthManagerStart;
        On.PlayMakerFSM.OnEnable -= OnPlayMakerFsmOnEnable;
        USceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        ModHooks.BeforeSceneLoadHook -= OnBeforeSceneLoad;
        vanillaHpByInstance.Clear();
        vanillaSummonAttackLimitByFsm.Clear();
        hoGEntryAllowed = false;
    }

    internal static void ReapplyLiveSettings()
    {
        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyVengeflyHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        ApplySummonLimitSettingsIfPresent();
    }

    internal static void SetP5HpEnabled(bool value)
    {
        if (value)
        {
            if (!vengeflyKingP5Hp)
            {
                vengeflyKingLeftMaxHpBeforeP5 = ClampHp(vengeflyKingLeftMaxHp);
                vengeflyKingRightMaxHpBeforeP5 = ClampHp(vengeflyKingRightMaxHp);
                vengeflyKingSummonMaxHpBeforeP5 = ClampHp(vengeflyKingSummonMaxHp);
                vengeflyKingUseMaxHpBeforeP5 = vengeflyKingUseMaxHp;
                vengeflyKingUseCustomSummonLimitBeforeP5 = vengeflyKingUseCustomSummonLimit;
                vengeflyKingHasStoredStateBeforeP5 = true;
            }

            vengeflyKingP5Hp = true;
            vengeflyKingUseMaxHp = true;
            vengeflyKingUseCustomSummonLimit = false;
            vengeflyKingLeftMaxHp = P5LeftHp;
            vengeflyKingRightMaxHp = P5RightHp;
            vengeflyKingSummonMaxHp = P5SummonHp;
        }
        else
        {
            if (vengeflyKingP5Hp && vengeflyKingHasStoredStateBeforeP5)
            {
                vengeflyKingLeftMaxHp = ClampHp(vengeflyKingLeftMaxHpBeforeP5);
                vengeflyKingRightMaxHp = ClampHp(vengeflyKingRightMaxHpBeforeP5);
                vengeflyKingSummonMaxHp = ClampHp(vengeflyKingSummonMaxHpBeforeP5);
                vengeflyKingUseMaxHp = vengeflyKingUseMaxHpBeforeP5;
                vengeflyKingUseCustomSummonLimit = vengeflyKingUseCustomSummonLimitBeforeP5;
            }

            vengeflyKingP5Hp = false;
            vengeflyKingHasStoredStateBeforeP5 = false;
        }

        ReapplyLiveSettings();
    }

    private static void NormalizeP5State()
    {
        if (!vengeflyKingP5Hp)
        {
            return;
        }

        if (!vengeflyKingHasStoredStateBeforeP5)
        {
            vengeflyKingLeftMaxHpBeforeP5 = ClampHp(vengeflyKingLeftMaxHp);
            vengeflyKingRightMaxHpBeforeP5 = ClampHp(vengeflyKingRightMaxHp);
            vengeflyKingSummonMaxHpBeforeP5 = ClampHp(vengeflyKingSummonMaxHp);
            vengeflyKingUseMaxHpBeforeP5 = vengeflyKingUseMaxHp;
            vengeflyKingUseCustomSummonLimitBeforeP5 = vengeflyKingUseCustomSummonLimit;
            vengeflyKingHasStoredStateBeforeP5 = true;
        }

        vengeflyKingUseMaxHp = true;
        vengeflyKingUseCustomSummonLimit = false;
        vengeflyKingLeftMaxHp = P5LeftHp;
        vengeflyKingRightMaxHp = P5RightHp;
        vengeflyKingSummonMaxHp = P5SummonHp;
    }

    internal static void ApplyVengeflyHealthIfPresent()
    {
        if (!moduleActive || !ShouldUseCustomHp())
        {
            return;
        }

        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsVengefly(hm))
            {
                continue;
            }

            ApplyVengeflyHealth(hm.gameObject, hm);
        }
    }

    internal static void RestoreVanillaHealthIfPresent()
    {
        foreach (HealthManager hm in UObject.FindObjectsOfType<HealthManager>())
        {
            if (hm == null || hm.gameObject == null || !IsVengefly(hm))
            {
                continue;
            }

            RestoreVanillaHealth(hm.gameObject, hm);
        }
    }

    internal static void ApplySummonLimitSettingsIfPresent()
    {
        if (!moduleActive)
        {
            return;
        }

        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null)
            {
                continue;
            }

            ApplySummonLimitSettings(fsm);
        }
    }

    internal static void RestoreVanillaSummonLimitsIfPresent()
    {
        foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
        {
            if (fsm == null || fsm.gameObject == null || !IsBigBuzzerFsm(fsm))
            {
                continue;
            }

            if (!IsVengeflyObject(fsm.gameObject))
            {
                continue;
            }

            RememberVanillaSummonAttackLimit(fsm);
            SetSummonLimitOnBigBuzzerFsm(fsm, DefaultSummonLimit);
            SetSummonAttackLimitOnBigBuzzerFsm(fsm, GetVanillaSummonAttackLimit(fsm));
        }
    }

    private static void OnPlayMakerFsmOnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);

        if (!moduleActive || self == null || self.gameObject == null)
        {
            return;
        }

        ApplySummonLimitSettings(self);
    }

    private static void OnHealthManagerAwake(On.HealthManager.orig_Awake orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsVengefly(self))
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyVengeflyHealth(self.gameObject, self);
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static void OnHealthManagerStart(On.HealthManager.orig_Start orig, HealthManager self)
    {
        orig(self);

        if (!moduleActive || !IsVengefly(self))
        {
            return;
        }

        RememberVanillaHp(self);
        if (!ShouldApplySettings(self.gameObject))
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyVengeflyHealth(self.gameObject, self);
            _ = self.StartCoroutine(DeferredApply(self));
        }
        else
        {
            RestoreVanillaHealth(self.gameObject, self);
        }
    }

    private static IEnumerator DeferredApply(HealthManager hm)
    {
        yield return null;

        if (!moduleActive || hm == null || hm.gameObject == null || !IsVengefly(hm))
        {
            yield break;
        }

        if (ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
        {
            ApplyVengeflyHealth(hm.gameObject, hm);
            yield return new WaitForSeconds(0.01f);
            if (moduleActive && hm != null && hm.gameObject != null && IsVengefly(hm) && ShouldUseCustomHp() && ShouldApplySettings(hm.gameObject))
            {
                ApplyVengeflyHealth(hm.gameObject, hm);
            }
        }
    }

    private static void SceneManager_activeSceneChanged(Scene from, Scene to)
    {
        UpdateHoGEntryAllowed(from.name, to.name);
        if (!string.Equals(to.name, VengeflyScene, StringComparison.Ordinal))
        {
            vanillaHpByInstance.Clear();
            vanillaSummonAttackLimitByFsm.Clear();
            return;
        }

        if (!moduleActive)
        {
            return;
        }

        if (ShouldUseCustomHp())
        {
            ApplyVengeflyHealthIfPresent();
        }
        else
        {
            RestoreVanillaHealthIfPresent();
        }

        ApplySummonLimitSettingsIfPresent();
    }

    private static string OnBeforeSceneLoad(string newSceneName)
    {
        UpdateHoGEntryAllowed(USceneManager.GetActiveScene().name, newSceneName);
        return newSceneName;
    }

    private static bool IsVengefly(HealthManager hm)
    {
        if (hm == null || hm.gameObject == null)
        {
            return false;
        }

        return IsVengeflyObject(hm.gameObject);
    }

    private static bool IsVengeflyObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        if (!string.Equals(gameObject.scene.name, VengeflyScene, StringComparison.Ordinal))
        {
            return false;
        }

        return GetVengeflySide(gameObject) != VengeflySide.Unknown;
    }

    private static bool ShouldApplySettings(GameObject? gameObject)
    {
        if (gameObject == null || !IsVengeflyObject(gameObject))
        {
            return false;
        }

        return hoGEntryAllowed;
    }

    private static bool ShouldUseCustomHp() => vengeflyKingUseMaxHp;

    private static bool ShouldUseCustomSummonLimit() => vengeflyKingUseCustomSummonLimit && !vengeflyKingP5Hp;

    private static void UpdateHoGEntryAllowed(string currentScene, string nextScene)
    {
        if (string.Equals(nextScene, VengeflyScene, StringComparison.Ordinal))
        {
            if (string.Equals(currentScene, HoGWorkshopScene, StringComparison.Ordinal))
            {
                hoGEntryAllowed = true;
            }
            else if (string.Equals(currentScene, VengeflyScene, StringComparison.Ordinal) && hoGEntryAllowed)
            {
                hoGEntryAllowed = true;
            }
            else
            {
                hoGEntryAllowed = false;
            }

            return;
        }

        hoGEntryAllowed = false;
    }

    private static void ApplyVengeflyHealth(GameObject boss, HealthManager? hm = null)
    {
        if (!ShouldApplySettings(boss) || !ShouldUseCustomHp())
        {
            return;
        }

        VengeflySide side = GetVengeflySide(boss);
        if (side == VengeflySide.Unknown)
        {
            return;
        }

        hm ??= boss.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        RememberVanillaHp(hm);
        int configuredHp = side switch
        {
            VengeflySide.Left => vengeflyKingLeftMaxHp,
            VengeflySide.Right => vengeflyKingRightMaxHp,
            VengeflySide.Summon => vengeflyKingSummonMaxHp,
            _ => 0,
        };
        if (configuredHp <= 0)
        {
            return;
        }

        int targetHp = ClampHp(configuredHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static void ApplySummonLimitSettings(PlayMakerFSM fsm)
    {
        if (fsm == null || fsm.gameObject == null || !IsBigBuzzerFsm(fsm))
        {
            return;
        }

        if (!ShouldApplySettings(fsm.gameObject))
        {
            return;
        }

        VengeflySide side = GetVengeflySide(fsm.gameObject);
        if (side != VengeflySide.Left && side != VengeflySide.Right)
        {
            return;
        }

        RememberVanillaSummonAttackLimit(fsm);

        int targetLimit = DefaultSummonLimit;
        int targetSummonAttackLimit = GetVanillaSummonAttackLimit(fsm);
        if (ShouldUseCustomSummonLimit())
        {
            targetLimit = GetConfiguredSummonLimit(side);
            targetSummonAttackLimit = GetConfiguredSummonAttackLimit(side);
        }

        SetSummonLimitOnBigBuzzerFsm(fsm, targetLimit);
        SetSummonAttackLimitOnBigBuzzerFsm(fsm, targetSummonAttackLimit);
    }

    private static bool IsBigBuzzerFsm(PlayMakerFSM fsm) =>
        fsm != null && string.Equals(fsm.FsmName, "Big Buzzer", StringComparison.Ordinal);

    private static void SetSummonLimitOnBigBuzzerFsm(PlayMakerFSM fsm, int value)
    {
        int clampedLimit = ClampSummonLimit(value);
        int patched = SetStateIntCompareThreshold(fsm, "Check Summon GG", clampedLimit);

        // Fallback for variants where the cap compare still lives in Wait Frame.
        if (patched == 0)
        {
            _ = SetStateIntCompareThreshold(fsm, "Wait Frame", clampedLimit);
        }
    }

    private static int SetStateIntCompareThreshold(PlayMakerFSM fsm, string stateName, int threshold)
    {
        FsmState? state = fsm.Fsm?.GetState(stateName);
        if (state?.Actions == null)
        {
            return 0;
        }

        int patched = 0;
        foreach (IntCompare compare in state.Actions.OfType<IntCompare>())
        {
            if (compare.integer2 == null)
            {
                continue;
            }

            compare.integer2.Value = threshold;
            patched++;
        }

        return patched;
    }

    private static int GetConfiguredSummonLimit(VengeflySide side)
    {
        return side switch
        {
            VengeflySide.Left => ClampSummonLimit(vengeflyKingLeftSummonLimit),
            VengeflySide.Right => ClampSummonLimit(vengeflyKingRightSummonLimit),
            _ => DefaultSummonLimit,
        };
    }

    private static int GetConfiguredSummonAttackLimit(VengeflySide side)
    {
        return side switch
        {
            VengeflySide.Left => ClampSummonAttackLimit(vengeflyKingLeftSummonAttackLimit),
            VengeflySide.Right => ClampSummonAttackLimit(vengeflyKingRightSummonAttackLimit),
            _ => DefaultSummonAttackLimit,
        };
    }

    private static void SetSummonAttackLimitOnBigBuzzerFsm(PlayMakerFSM fsm, int value)
    {
        FsmInt? summons = fsm.FsmVariables?.FindFsmInt("Summons");
        if (summons == null)
        {
            return;
        }

        summons.Value = ClampSummonAttackLimit(value);
    }

    private static void RememberVanillaSummonAttackLimit(PlayMakerFSM fsm)
    {
        int id = fsm.GetInstanceID();
        if (vanillaSummonAttackLimitByFsm.ContainsKey(id))
        {
            return;
        }

        FsmInt? summons = fsm.FsmVariables?.FindFsmInt("Summons");
        int value = summons?.Value ?? DefaultSummonAttackLimit;
        vanillaSummonAttackLimitByFsm[id] = ClampSummonAttackLimit(value);
    }

    private static int GetVanillaSummonAttackLimit(PlayMakerFSM fsm)
    {
        int id = fsm.GetInstanceID();
        if (vanillaSummonAttackLimitByFsm.TryGetValue(id, out int stored))
        {
            return ClampSummonAttackLimit(stored);
        }

        FsmInt? summons = fsm.FsmVariables?.FindFsmInt("Summons");
        int value = ClampSummonAttackLimit(summons?.Value ?? DefaultSummonAttackLimit);
        vanillaSummonAttackLimitByFsm[id] = value;
        return value;
    }

    private static void RestoreVanillaHealth(GameObject boss, HealthManager? hm = null)
    {
        if (boss == null || !IsVengeflyObject(boss))
        {
            return;
        }

        VengeflySide side = GetVengeflySide(boss);
        if (side == VengeflySide.Unknown)
        {
            return;
        }

        hm ??= boss.GetComponent<HealthManager>();
        if (hm == null)
        {
            return;
        }

        if (!TryGetVanillaHp(hm, side, out int vanillaHp))
        {
            return;
        }

        int targetHp = ClampHp(vanillaHp);
        boss.manageHealth(targetHp);
        hm.hp = targetHp;
        TrySetMaxHp(hm, targetHp);
    }

    private static VengeflySide GetVengeflySide(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return VengeflySide.Unknown;
        }

        string name = gameObject.name;
        if (name.StartsWith(LeftVengeflyName, StringComparison.Ordinal))
        {
            return VengeflySide.Left;
        }

        if (name.StartsWith(RightVengeflyName, StringComparison.Ordinal))
        {
            return VengeflySide.Right;
        }

        if (name.IndexOf("Buzzer", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return VengeflySide.Summon;
        }

        return VengeflySide.Unknown;
    }

    private static void RememberVanillaHp(HealthManager hm)
    {
        int instanceId = hm.GetInstanceID();
        if (vanillaHpByInstance.ContainsKey(instanceId))
        {
            return;
        }

        VengeflySide side = GetVengeflySide(hm.gameObject);
        if (side == VengeflySide.Unknown)
        {
            return;
        }

        int hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, GetDefaultVanillaHp(side));
        if (hp > 0)
        {
            vanillaHpByInstance[instanceId] = hp;
        }
    }

    private static bool TryGetVanillaHp(HealthManager hm, VengeflySide side, out int hp)
    {
        if (vanillaHpByInstance.TryGetValue(hm.GetInstanceID(), out hp) && hp > 0)
        {
            hp = Math.Max(hp, GetDefaultVanillaHp(side));
            return true;
        }

        hp = ReadMaxHp(hm);
        if (hp <= 0)
        {
            hp = hm.hp;
        }

        hp = Math.Max(hp, GetDefaultVanillaHp(side));
        return hp > 0;
    }

    private static int GetDefaultVanillaHp(VengeflySide side)
    {
        return side switch
        {
            VengeflySide.Right => DefaultRightMaxHp,
            VengeflySide.Summon => DefaultSummonMaxHp,
            _ => DefaultLeftMaxHp,
        };
    }

    private static int ReadMaxHp(HealthManager hm)
    {
        try
        {
            int maxHp = ReflectionHelper.GetField<HealthManager, int>(hm, "maxHP");
            if (maxHp > 0)
            {
                return maxHp;
            }
        }
        catch
        {
            // Ignore if field is unavailable.
        }

        return hm.hp;
    }

    private static void TrySetMaxHp(HealthManager hm, int value)
    {
        try
        {
            ReflectionHelper.SetField(hm, "maxHP", value);
        }
        catch
        {
            // Ignore if field is unavailable.
        }
    }

    private static int ClampHp(int value)
    {
        if (value < MinHp)
        {
            return MinHp;
        }

        return value > MaxHp ? MaxHp : value;
    }

    private static int ClampSummonLimit(int value)
    {
        if (value < MinSummonLimit)
        {
            return MinSummonLimit;
        }

        return value > MaxSummonLimit ? MaxSummonLimit : value;
    }

    private static int ClampSummonAttackLimit(int value)
    {
        if (value < MinSummonAttackLimit)
        {
            return MinSummonAttackLimit;
        }

        return value > MaxSummonAttackLimit ? MaxSummonAttackLimit : value;
    }
}
