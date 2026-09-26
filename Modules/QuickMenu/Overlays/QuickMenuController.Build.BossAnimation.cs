using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildBossAnimationOverlayUi()
        {
            bossAnimationRoot = CreateOverlayFrame("BossAnimationOverlayCanvas", BossAnimationCanvasSortOrder, "BossAnimationPanel", BossAnimationPanelHeight, "Categories/BossAnimationSkipping".Localize(), 52, 340f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = BossAnimationPanelHeight;
            RectTransform content = CreateOverlayContent(panel, BossAnimationPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            bossAnimationContent = content;

            float rowY = GetRowStartY(panelHeight, BossAnimationRowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "BossAnimEnableRow",
                "Settings/BossAnimation/Enable".Localize(),
                rowY,
                GetBossAnimationMasterEnabled,
                SetBossAnimationMasterEnabled,
                out bossAnimEnableValue,
                out bossAnimEnableIcon
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "BossAnimHallOfGodsRow",
                "Settings/HallOfGodsStatues".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.HallOfGodsStatues,
                value => Modules.QoL.SkipCutscenes.HallOfGodsStatues = value,
                out bossAnimHallOfGodsValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "BossAnimAbsoluteRadianceRow",
                "Settings/AbsoluteRadiance".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.AbsoluteRadiance,
                value => Modules.QoL.SkipCutscenes.AbsoluteRadiance = value,
                out bossAnimAbsoluteRadianceValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "BossAnimPantheonVEndingRow",
                "Settings/PantheonVEnding".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.PantheonVEnding,
                value => Modules.QoL.SkipCutscenes.PantheonVEnding = value,
                out bossAnimPantheonVEndingValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "BossAnimPureVesselRow",
                "Settings/PureVesselRoar".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.PureVesselRoar,
                value => Modules.QoL.SkipCutscenes.PureVesselRoar = value,
                out bossAnimPureVesselValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "BossAnimGrimmNightmareRow",
                "Settings/GrimmNightmare".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.GrimmNightmare,
                value => Modules.QoL.SkipCutscenes.GrimmNightmare = value,
                out bossAnimGrimmNightmareValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "BossAnimGreyPrinceRow",
                "Settings/GreyPrinceZote".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.GreyPrinceZote,
                value => Modules.QoL.SkipCutscenes.GreyPrinceZote = value,
                out bossAnimGreyPrinceValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "BossAnimCollectorRow",
                "Settings/Collector".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.Collector,
                value => Modules.QoL.SkipCutscenes.Collector = value,
                out bossAnimCollectorValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "BossAnimSoulMasterRow",
                "Settings/SoulMasterPhaseTransitionSkip".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip,
                value => Modules.QoL.SkipCutscenes.SoulMasterPhaseTransitionSkip = value,
                out bossAnimSoulMasterValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "BossAnimCollectorRoarRow",
                "Modules/CollectorRoarMute".Localize(),
                rowY,
                GetCollectorRoarEnabled,
                SetCollectorRoarEnabled,
                out bossAnimCollectorRoarValue
            );

            lastY = rowY;
            rowY += RowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "BossAnimationResetRow", "Settings/BossAnimation/Reset".Localize(), resetY, OnBossAnimationResetDefaultsClicked);
            CreateButtonRow(panel.transform, "BossAnimationBackRow", "Back", backY, OnBossAnimationBackClicked);
        }
    }
}
