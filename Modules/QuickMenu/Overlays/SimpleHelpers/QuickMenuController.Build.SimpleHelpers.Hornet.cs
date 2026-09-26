using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildHornetHelperOverlayUi()
        {
            hornetHelperRoot = CreateOverlayFrame("HornetHelperOverlayCanvas", HornetHelperCanvasSortOrder, "HornetHelperPanel", HornetHelperPanelHeight, "Modules/HornetProtectorHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = HornetHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, HornetHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            hornetHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "HornetHelperEnableRow",
                "Settings/HornetProtectorHelper/Enable".Localize(),
                rowY,
                GetHornetProtectorHelperEnabled,
                SetHornetProtectorHelperEnabled,
                out hornetHelperToggleValue,
                out hornetHelperToggleIcon
            );

            lastY = rowY;
            rowY += HornetHelperRowSpacing;
            CreateToggleRow(
                content,
                "HornetP5HpRow",
                "Settings/HornetProtectorHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.HornetProtectorHelper.hornetP5Hp,
                SetHornetP5HpEnabled,
                out hornetP5HpValue
            );

            lastY = rowY;
            rowY += HornetHelperRowSpacing;
            CreateToggleRow(
                content,
                "HornetUseMaxHpRow",
                "Settings/HornetProtectorHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.HornetProtectorHelper.hornetUseMaxHp,
                SetHornetUseMaxHpEnabled,
                out hornetUseMaxHpValue
            );

            lastY = rowY;
            rowY += HornetHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "HornetMaxHpRow",
                "Settings/HornetProtectorHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.HornetProtectorHelper.hornetMaxHp,
                SetHornetMaxHp,
                1,
                999999,
                10,
                out hornetMaxHpField
            );

            lastY = rowY;
            rowY += HornetHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "HornetHelperResetRow", "Settings/HornetProtectorHelper/Reset".Localize(), resetY, OnHornetHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "HornetHelperBackRow", "Back", backY, OnHornetHelperBackClicked);
        }
    }
}
