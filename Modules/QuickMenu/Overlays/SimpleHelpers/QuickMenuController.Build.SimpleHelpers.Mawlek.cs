using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildMawlekHelperOverlayUi()
        {
            mawlekHelperRoot = CreateOverlayFrame("MawlekHelperOverlayCanvas", MawlekHelperCanvasSortOrder, "MawlekHelperPanel", MawlekHelperPanelHeight, "Modules/BroodingMawlekHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = MawlekHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, MawlekHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            mawlekHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "MawlekHelperEnableRow",
                "Settings/BroodingMawlekHelper/Enable".Localize(),
                rowY,
                GetBroodingMawlekHelperEnabled,
                SetBroodingMawlekHelperEnabled,
                out mawlekHelperToggleValue,
                out mawlekHelperToggleIcon
            );

            lastY = rowY;
            rowY += MawlekHelperRowSpacing;
            CreateToggleRow(
                content,
                "MawlekP5HpRow",
                "Settings/BroodingMawlekHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.BroodingMawlekHelper.mawlekP5Hp,
                SetMawlekP5HpEnabled,
                out mawlekP5HpValue
            );

            lastY = rowY;
            rowY += MawlekHelperRowSpacing;
            CreateToggleRow(
                content,
                "MawlekUseMaxHpRow",
                "Settings/BroodingMawlekHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.BroodingMawlekHelper.mawlekUseMaxHp,
                SetMawlekUseMaxHpEnabled,
                out mawlekUseMaxHpValue
            );

            lastY = rowY;
            rowY += MawlekHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "MawlekMaxHpRow",
                "Settings/BroodingMawlekHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.BroodingMawlekHelper.mawlekMaxHp,
                SetMawlekMaxHp,
                1,
                999999,
                10,
                out mawlekMaxHpField
            );

            lastY = rowY;
            rowY += MawlekHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "MawlekHelperResetRow", "Settings/BroodingMawlekHelper/Reset".Localize(), resetY, OnMawlekHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "MawlekHelperBackRow", "Back", backY, OnMawlekHelperBackClicked);
        }
    }
}
