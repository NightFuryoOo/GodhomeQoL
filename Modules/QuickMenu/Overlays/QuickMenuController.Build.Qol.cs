using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildQolOverlayUi()
        {
            qolRoot = CreateOverlayFrame("QolOverlayCanvas", QolCanvasSortOrder, "QolPanel", PanelHeight, "Categories/QoL".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = PanelHeight;
            RectTransform content = CreateOverlayContent(panel, PanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            qolContent = content;
            qolScrollRect = content.GetComponentInParent<ScrollRect>();

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "QolEnableRow",
                "Settings/QoL/Enable".Localize(),
                rowY,
                GetQolMasterEnabled,
                SetQolMasterEnabled,
                out qolEnableValue,
                out qolEnableIcon
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateToggleRow(
                content,
                "QolFastDreamWarpRow",
                "Modules/FastDreamWarp".Localize(),
                rowY,
                GetFastDreamWarpEnabled,
                SetFastDreamWarpEnabled,
                out bossAnimFastDreamWarpValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateKeybindRow(
                content,
                "QolFastDreamWarpKeyRow",
                "Settings/FastDreamWarp/Hotkey".Localize(),
                rowY,
                GetFastDreamWarpBindingLabel,
                StartFastDreamWarpRebind,
                out fastDreamWarpKeyValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateToggleRow(
                content,
                "QolShortDeathRow",
                "Modules/ShortDeathAnimation".Localize(),
                rowY,
                GetShortDeathAnimationEnabled,
                SetShortDeathAnimationEnabled,
                out bossAnimShortDeathValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateToggleRow(
                content,
                "QolUnlockAllModesRow",
                "Modules/UnlockAllModes".Localize(),
                rowY,
                GetUnlockAllModesEnabled,
                SetUnlockAllModesEnabled,
                out qolUnlockAllModesValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateToggleRow(
                content,
                "QolUnlockPantheonsRow",
                "Modules/UnlockPantheons".Localize(),
                rowY,
                GetUnlockPantheonsEnabled,
                SetUnlockPantheonsEnabled,
                out qolUnlockPantheonsValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateToggleRow(
                content,
                "QolUnlockRadianceRow",
                "Modules/UnlockRadiance".Localize(),
                rowY,
                GetUnlockRadianceEnabled,
                SetUnlockRadianceEnabled,
                out qolUnlockRadianceValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateToggleRow(
                content,
                "QolUnlockRadiantRow",
                "Modules/UnlockRadiant".Localize(),
                rowY,
                GetUnlockRadiantEnabled,
                SetUnlockRadiantEnabled,
                out qolUnlockRadiantValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateToggleRow(
                content,
                "QolInvincibleIndicatorRow",
                "Modules/InvincibleIndicator".Localize(),
                rowY,
                GetInvincibleIndicatorEnabled,
                SetInvincibleIndicatorEnabled,
                out qolInvincibleIndicatorValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateToggleRow(
                content,
                "QolScreenShakeRow",
                "Disable ScreenShake",
                rowY,
                GetScreenShakeEnabled,
                SetScreenShakeEnabled,
                out qolScreenShakeValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;
            CreateKeybindRow(
                content,
                "QolNailDamageCheckKeyRow",
                "Settings/QoL/NailDamageCheck".Localize(),
                rowY,
                GetNailDamageCheckKeyLabel,
                StartNailDamageCheckRebind,
                out nailDamageCheckKeyValue
            );

            lastY = rowY;
            rowY += QolRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "QolResetRow", "Settings/QoL/Reset".Localize(), resetY, OnQolResetDefaultsClicked);
            CreateButtonRow(panel.transform, "QolBackRow", "Back", backY, OnQolBackClicked);
        }
    }
}
