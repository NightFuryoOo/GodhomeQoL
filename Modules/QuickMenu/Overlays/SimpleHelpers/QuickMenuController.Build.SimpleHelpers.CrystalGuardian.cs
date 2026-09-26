using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildCrystalGuardianHelperOverlayUi()
        {
            crystalGuardianHelperRoot = CreateOverlayFrame("CrystalGuardianHelperOverlayCanvas", CrystalGuardianHelperCanvasSortOrder, "CrystalGuardianHelperPanel", CrystalGuardianHelperPanelHeight, "Modules/CrystalGuardianHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = CrystalGuardianHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, CrystalGuardianHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            crystalGuardianHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "CrystalGuardianHelperEnableRow",
                "Settings/CrystalGuardianHelper/Enable".Localize(),
                rowY,
                GetCrystalGuardianHelperEnabled,
                SetCrystalGuardianHelperEnabled,
                out crystalGuardianHelperToggleValue,
                out crystalGuardianHelperToggleIcon
            );

            lastY = rowY;
            rowY += CrystalGuardianHelperRowSpacing;
            CreateToggleRow(
                content,
                "CrystalGuardianP5HpRow",
                "Settings/CrystalGuardianHelper/P5HP".Localize(),
                rowY,
                () => Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianP5Hp,
                SetCrystalGuardianP5HpEnabled,
                out crystalGuardianP5HpValue
            );

            lastY = rowY;
            rowY += CrystalGuardianHelperRowSpacing;
            CreateToggleRow(
                content,
                "CrystalGuardianUseMaxHpRow",
                "Settings/CrystalGuardianHelper/UseMaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianUseMaxHp,
                SetCrystalGuardianUseMaxHpEnabled,
                out crystalGuardianUseMaxHpValue
            );

            lastY = rowY;
            rowY += CrystalGuardianHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "CrystalGuardianMaxHpRow",
                "Settings/CrystalGuardianHelper/MaxHP".Localize(),
                rowY,
                () => Modules.BossChallenge.CrystalGuardianHelper.crystalGuardianMaxHp,
                SetCrystalGuardianMaxHp,
                1,
                999999,
                10,
                out crystalGuardianMaxHpField
            );

            lastY = rowY;
            rowY += CrystalGuardianHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "CrystalGuardianHelperResetRow", "Settings/CrystalGuardianHelper/Reset".Localize(), resetY, OnCrystalGuardianHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "CrystalGuardianHelperBackRow", "Back", backY, OnCrystalGuardianHelperBackClicked);
        }
    }
}
