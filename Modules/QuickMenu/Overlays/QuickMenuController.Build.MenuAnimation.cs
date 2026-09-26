using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildMenuAnimationOverlayUi()
        {
            menuAnimationRoot = CreateOverlayFrame("MenuAnimationOverlayCanvas", MenuAnimationCanvasSortOrder, "MenuAnimationPanel", MenuAnimationPanelHeight, "Categories/MenuAnimationSkipping".Localize(), 52, 260f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = MenuAnimationPanelHeight;
            RectTransform content = CreateOverlayContent(panel, MenuAnimationPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            menuAnimationContent = content;

            float rowY = GetRowStartY(panelHeight, MenuAnimationRowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "MenuAnimEnableRow",
                "Settings/MenuAnimation/Enable".Localize(),
                rowY,
                GetMenuAnimationMasterEnabled,
                SetMenuAnimationMasterEnabled,
                out menuAnimEnableValue,
                out menuAnimEnableIcon
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "MenuAnimDoorDefaultRow",
                "Modules/DoorDefaultBegin".Localize(),
                rowY,
                GetDoorDefaultBeginEnabled,
                SetDoorDefaultBeginEnabled,
                out menuAnimDoorDefaultValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "MenuAnimFasterLoadsRow",
                "Modules/FasterLoads".Localize(),
                rowY,
                GetFasterLoadsEnabled,
                SetFasterLoadsEnabled,
                out menuAnimFasterLoadsValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "MenuAnimFastMenusRow",
                "Modules/FastMenus".Localize(),
                rowY,
                GetFastMenusEnabled,
                SetFastMenusEnabled,
                out menuAnimFastMenusValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "MenuAnimFastTextRow",
                "Modules/FastText".Localize(),
                rowY,
                GetFastTextEnabled,
                SetFastTextEnabled,
                out menuAnimFastTextValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "MenuAnimAutoSkipRow",
                "Settings/AutoSkipCinematics".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.AutoSkipCinematics,
                value => Modules.QoL.SkipCutscenes.AutoSkipCinematics = value,
                out menuAnimAutoSkipValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "MenuAnimAllowSkippingRow",
                "Settings/AllowSkippingNonskippable".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.AllowSkippingNonskippable,
                value => Modules.QoL.SkipCutscenes.AllowSkippingNonskippable = value,
                out menuAnimAllowSkippingValue
            );

            lastY = rowY;
            rowY += RowSpacing;
            CreateToggleRow(
                content,
                "MenuAnimSkipWithoutPromptRow",
                "Settings/SkipCutscenesWithoutPrompt".Localize(),
                rowY,
                () => Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt,
                value => Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt = value,
                out menuAnimSkipWithoutPromptValue
            );

            lastY = rowY;
            rowY += RowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "MenuAnimationResetRow", "Settings/MenuAnimation/Reset".Localize(), resetY, OnMenuAnimationResetDefaultsClicked);
            CreateButtonRow(panel.transform, "MenuAnimationBackRow", "Back", backY, OnMenuAnimationBackClicked);
        }
    }
}
