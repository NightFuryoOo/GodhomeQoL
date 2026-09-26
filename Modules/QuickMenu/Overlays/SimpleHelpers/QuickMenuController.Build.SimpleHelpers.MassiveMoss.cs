using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildMassiveMossHelperOverlayUi()
        {
            massiveMossHelperRoot = CreateOverlayFrame("MassiveMossHelperOverlayCanvas", MassiveMossHelperCanvasSortOrder, "MassiveMossHelperPanel", MassiveMossHelperPanelHeight, "Modules/MassiveMossChargerHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = MassiveMossHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, MassiveMossHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            massiveMossHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "MassiveMossHelperEnableRow",
                "Settings/MassiveMossChargerHelper/Enable".Localize(),
                rowY,
                GetMassiveMossChargerHelperEnabled,
                SetMassiveMossChargerHelperEnabled,
                out massiveMossHelperToggleValue,
                out massiveMossHelperToggleIcon
            );

            lastY = rowY;
            rowY += MassiveMossHelperRowSpacing;
            CreateToggleRow(
                content,
                "MassiveMossP5HpRow",
                "Settings/MassiveMossChargerHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.MassiveMossChargerHelper.massiveMossP5Hp,
                SetMassiveMossP5HpEnabled,
                out massiveMossP5HpValue
            );

            lastY = rowY;
            rowY += MassiveMossHelperRowSpacing;
            CreateToggleRow(
                content,
                "MassiveMossUseMaxHpRow",
                "Settings/MassiveMossChargerHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.MassiveMossChargerHelper.massiveMossUseMaxHp,
                SetMassiveMossUseMaxHpEnabled,
                out massiveMossUseMaxHpValue
            );

            lastY = rowY;
            rowY += MassiveMossHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MassiveMossMaxHpRow",
                "Settings/MassiveMossChargerHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.MassiveMossChargerHelper.massiveMossMaxHp,
                SetMassiveMossMaxHp,
                1,
                999999,
                10,
                out massiveMossMaxHpField
            );

            lastY = rowY;
            rowY += MassiveMossHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "MassiveMossHelperResetRow", "Settings/MassiveMossChargerHelper/Reset".Localize(), resetY, OnMassiveMossHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "MassiveMossHelperBackRow", "Back", backY, OnMassiveMossHelperBackClicked);
        }
    }
}
