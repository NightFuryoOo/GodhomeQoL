using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private static readonly Type[] BossManipulateGlobalP5ModuleTypes = new[]
        {
            typeof(Modules.BossChallenge.GruzMotherHelper),
            typeof(Modules.BossChallenge.BroodingMawlekHelper),
            typeof(Modules.BossChallenge.HornetProtectorHelper),
            typeof(Modules.BossChallenge.HornetSentinelHelper),
            typeof(Modules.BossChallenge.MassiveMossChargerHelper),
            typeof(Modules.BossChallenge.CrystalGuardianHelper),
            typeof(Modules.BossChallenge.EnragedGuardianHelper),
            typeof(Modules.BossChallenge.MarmuHelper),
            typeof(Modules.BossChallenge.XeroHelper),
            typeof(Modules.BossChallenge.MarkothHelper),
            typeof(Modules.BossChallenge.GalienHelper),
            typeof(Modules.BossChallenge.GorbHelper),
            typeof(Modules.BossChallenge.ElderHuHelper),
            typeof(Modules.BossChallenge.NoEyesHelper),
            typeof(Modules.BossChallenge.DungDefenderHelper),
            typeof(Modules.BossChallenge.WhiteDefenderHelper),
            typeof(Modules.BossChallenge.HiveKnightHelper),
            typeof(Modules.BossChallenge.BrokenVesselHelper),
            typeof(Modules.BossChallenge.LostKinHelper),
            typeof(Modules.BossChallenge.WingedNoskHelper),
            typeof(Modules.BossChallenge.UumuuHelper),
            typeof(Modules.BossChallenge.TraitorLordHelper),
            typeof(Modules.BossChallenge.TroupeMasterGrimmHelper),
            typeof(Modules.BossChallenge.NightmareKingGrimmHelper),
            typeof(Modules.BossChallenge.PureVesselHelper),
            typeof(Modules.BossChallenge.AbsoluteRadianceHelper),
            typeof(Modules.BossChallenge.PaintmasterSheoHelper),
            typeof(Modules.BossChallenge.SoulWarriorHelper),
            typeof(Modules.BossChallenge.NailsageSlyHelper),
            typeof(Modules.BossChallenge.SoulMasterHelper),
            typeof(Modules.BossChallenge.SoulTyrantHelper),
            typeof(Modules.BossChallenge.WatcherKnightHelper),
            typeof(Modules.BossChallenge.OroMatoHelper),
            typeof(Modules.BossChallenge.GodTamerHelper),
            typeof(Modules.BossChallenge.OblobblesHelper),
            typeof(Modules.BossChallenge.FalseKnightHelper),
            typeof(Modules.BossChallenge.FailedChampionHelper),
            typeof(Modules.BossChallenge.SisterOfBattleHelper),
            typeof(Modules.BossChallenge.FlukemarmHelper),
            typeof(Modules.BossChallenge.VengeflyKing),
            typeof(Modules.CollectorPhases.CollectorPhases),
        };

        private void OnOverlayBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetOverlayVisible(false);
        }
    }
}
