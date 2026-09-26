using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildHornetSentinelHelperOverlayUi()
        {
            hornetSentinelHelperRoot = CreateOverlayFrame("HornetSentinelHelperOverlayCanvas", HornetSentinelHelperCanvasSortOrder, "HornetSentinelHelperPanel", HornetSentinelHelperPanelHeight, "Modules/HornetSentinelHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = HornetSentinelHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, HornetSentinelHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            hornetSentinelHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "HornetSentinelHelperEnableRow",
                "Settings/HornetSentinelHelper/Enable".Localize(),
                rowY,
                GetHornetSentinelHelperEnabled,
                SetHornetSentinelHelperEnabled,
                out hornetSentinelHelperToggleValue,
                out hornetSentinelHelperToggleIcon
            );

            lastY = rowY;
            rowY += HornetSentinelHelperRowSpacing;
            CreateToggleRow(
                content,
                "HornetSentinelP5HpRow",
                "Settings/HornetSentinelHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.HornetSentinelHelper.hornetSentinelP5Hp,
                SetHornetSentinelP5HpEnabled,
                out hornetSentinelP5HpValue
            );

            lastY = rowY;
            rowY += HornetSentinelHelperRowSpacing;
            CreateToggleRow(
                content,
                "HornetSentinelUseMaxHpRow",
                "Settings/HornetSentinelHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseMaxHp,
                SetHornetSentinelUseMaxHpEnabled,
                out hornetSentinelUseMaxHpValue
            );

            lastY = rowY;
            rowY += HornetSentinelHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "HornetSentinelMaxHpRow",
                "Settings/HornetSentinelHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.HornetSentinelHelper.hornetSentinelMaxHp,
                SetHornetSentinelMaxHp,
                1,
                999999,
                10,
                out hornetSentinelMaxHpField
            );

            lastY = rowY;
            rowY += HornetSentinelHelperRowSpacing;
            CreateToggleRow(
                content,
                "HornetSentinelUseCustomPhaseRow",
                "Settings/HornetSentinelHelper/UseCustomPhase".Localize(),
                rowY,
                () => Modules.BossChallenge.HornetSentinelHelper.hornetSentinelUseCustomPhase,
                SetHornetSentinelUseCustomPhaseEnabled,
                out hornetSentinelUseCustomPhaseValue
            );

            lastY = rowY;
            rowY += HornetSentinelHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "HornetSentinelPhase2HpRow",
                "Settings/HornetSentinelHelper/Phase2HP".Localize(),
                rowY,
                () => Modules.BossChallenge.HornetSentinelHelper.hornetSentinelPhase2Hp,
                SetHornetSentinelPhase2Hp,
                1,
                999999,
                10,
                out hornetSentinelPhase2HpField
            );

            lastY = rowY;
            rowY += HornetSentinelHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "HornetSentinelHelperResetRow", "Settings/HornetSentinelHelper/Reset".Localize(), resetY, OnHornetSentinelHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "HornetSentinelHelperBackRow", "Back", backY, OnHornetSentinelHelperBackClicked);
        }
    }
}
