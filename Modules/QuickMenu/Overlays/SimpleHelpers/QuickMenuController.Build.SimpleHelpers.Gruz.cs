using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildGruzHelperOverlayUi()
        {
            gruzHelperRoot = CreateOverlayFrame("GruzHelperOverlayCanvas", GruzHelperCanvasSortOrder, "GruzHelperPanel", GruzHelperPanelHeight, "Modules/GruzMotherHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = GruzHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, GruzHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            gruzHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "GruzHelperEnableRow",
                "Settings/GruzMotherHelper/Enable".Localize(),
                rowY,
                GetGruzMotherHelperEnabled,
                SetGruzMotherHelperEnabled,
                out gruzHelperToggleValue,
                out gruzHelperToggleIcon
            );

            lastY = rowY;
            rowY += GruzHelperRowSpacing;
            CreateToggleRow(
                content,
                "GruzP5HpRow",
                "Settings/GruzMotherHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.GruzMotherHelper.gruzP5Hp,
                SetGruzP5HpEnabled,
                out gruzP5HpValue
            );

            lastY = rowY;
            rowY += GruzHelperRowSpacing;
            CreateToggleRow(
                content,
                "GruzUseMaxHpRow",
                "Settings/GruzMotherHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.GruzMotherHelper.gruzUseMaxHp,
                SetGruzUseMaxHpEnabled,
                out gruzUseMaxHpValue
            );

            lastY = rowY;
            rowY += GruzHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "GruzMaxHpRow",
                "Settings/GruzMotherHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.GruzMotherHelper.gruzMaxHp,
                SetGruzMaxHp,
                1,
                999999,
                10,
                out gruzMaxHpField
            );

            lastY = rowY;
            rowY += GruzHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "GruzHelperResetRow", "Settings/GruzMotherHelper/Reset".Localize(), resetY, OnGruzHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "GruzHelperBackRow", "Back", backY, OnGruzHelperBackClicked);
        }
    }
}
