using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using Modding;
using Satchel;
using Satchel.Futils;
using UnityEngine;

namespace GodhomeQoL.Modules.BossChallenge;

public abstract partial class ArmoredBossHelper : Module
{
    private protected const string MainOpened2StateName = "Opened 2";
    private protected const string MainHit2StateName = "Hit 2";
    private protected const string RageCountVariableName = "Rages";
    private protected const string StunnedAmountVariableName = "Stunned Amount";
    private readonly Dictionary<int, int> vanillaArmorHpByInstance = new();
    private readonly Dictionary<int, int> vanillaRecoverHpByFsm = new();
    private readonly Dictionary<int, int> trackedPhaseByArmorInstance = new();
    private readonly Dictionary<int, int> appliedHeadKillsByArmorInstance = new();
    private readonly Dictionary<int, int> lastHeadHpByInstance = new();
    private int confirmedHeadKillCount;
    private bool moduleActive;
    private bool hoGEntryAllowed;

    private protected abstract string SceneName { get; }
    private protected abstract int DefaultArmorPhaseHp { get; }
    private protected abstract int P5ArmorPhaseHp { get; }
    private protected abstract bool P5HpEnabled { get; set; }
    private protected abstract int ArmorPhase1Hp { get; set; }
    private protected abstract int ArmorPhase2Hp { get; set; }
    private protected abstract int ArmorPhase3Hp { get; set; }
    private protected abstract int ArmorPhase1HpBeforeP5 { get; set; }
    private protected abstract int ArmorPhase2HpBeforeP5 { get; set; }
    private protected abstract int ArmorPhase3HpBeforeP5 { get; set; }
    private protected abstract bool HasStoredStateBeforeP5 { get; set; }

    public override ToggleableLevel ToggleableLevel => ToggleableLevel.ChangeScene;
}
