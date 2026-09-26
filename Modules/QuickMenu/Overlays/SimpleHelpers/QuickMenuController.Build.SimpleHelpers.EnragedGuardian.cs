using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildEnragedGuardianHelperOverlayUi()
        {
            enragedGuardianHelperRoot = CreateOverlayFrame("EnragedGuardianHelperOverlayCanvas", EnragedGuardianHelperCanvasSortOrder, "EnragedGuardianHelperPanel", EnragedGuardianHelperPanelHeight, "Modules/EnragedGuardianHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = EnragedGuardianHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, EnragedGuardianHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            enragedGuardianHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "EnragedGuardianHelperEnableRow",
                "Settings/EnragedGuardianHelper/Enable".Localize(),
                rowY,
                GetEnragedGuardianHelperEnabled,
                SetEnragedGuardianHelperEnabled,
                out enragedGuardianHelperToggleValue,
                out enragedGuardianHelperToggleIcon
            );

            lastY = rowY;
            rowY += EnragedGuardianHelperRowSpacing;
            CreateToggleRow(
                content,
                "EnragedGuardianP5HpRow",
                "Settings/EnragedGuardianHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianP5Hp,
                SetEnragedGuardianP5HpEnabled,
                out enragedGuardianP5HpValue
            );

            lastY = rowY;
            rowY += EnragedGuardianHelperRowSpacing;
            CreateToggleRow(
                content,
                "EnragedGuardianUseMaxHpRow",
                "Settings/EnragedGuardianHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianUseMaxHp,
                SetEnragedGuardianUseMaxHpEnabled,
                out enragedGuardianUseMaxHpValue
            );

            lastY = rowY;
            rowY += EnragedGuardianHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "EnragedGuardianMaxHpRow",
                "Settings/EnragedGuardianHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.EnragedGuardianHelper.enragedGuardianMaxHp,
                SetEnragedGuardianMaxHp,
                1,
                999999,
                10,
                out enragedGuardianMaxHpField
            );

            lastY = rowY;
            rowY += EnragedGuardianHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "EnragedGuardianHelperResetRow", "Settings/EnragedGuardianHelper/Reset".Localize(), resetY, OnEnragedGuardianHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "EnragedGuardianHelperBackRow", "Back", backY, OnEnragedGuardianHelperBackClicked);
        }
    }
}
