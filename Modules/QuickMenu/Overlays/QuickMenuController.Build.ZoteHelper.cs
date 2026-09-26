using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildZoteHelperOverlayUi()
        {
            zoteHelperRoot = CreateOverlayFrame("ZoteHelperOverlayCanvas", ZoteHelperCanvasSortOrder, "ZoteHelperPanel", ZoteHelperPanelHeight, "Modules/ZoteHelper".Localize(), 52, 210f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = ZoteHelperPanelHeight;
            RectTransform content = CreateOverlayContent(panel, ZoteHelperPanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            zoteHelperContent = content;

            float rowY = GetRowStartY(panelHeight, RowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "ZoteHelperEnableRow",
                "Settings/ZoteHelper/Enable".Localize(),
                rowY,
                GetZoteHelperEnabled,
                SetZoteHelperEnabled,
                out zoteHelperToggleValue,
                out zoteHelperToggleIcon
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateToggleRow(
                content,
                "ZoteUseCustomBossHpRow",
                "Settings/ZoteHelper/UseCustomBossHP".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteUseCustomBossHp,
                SetZoteUseCustomBossHpEnabled,
                out zoteUseCustomBossHpValue
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "ZoteBossHpRow",
                "Settings/ZoteHelper/ZoteBossHP".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteBossHp,
                SetZoteBossHp,
                100,
                999999,
                10,
                out zoteBossHpField
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateToggleRow(
                content,
                "ZoteImmortalRow",
                "Settings/ZoteHelper/ZoteImmortal".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteImmortal,
                SetZoteImmortalEnabled,
                out zoteImmortalValue
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateToggleRow(
                content,
                "ZoteSpawnFlyingRow",
                "Settings/ZoteHelper/ZoteSpawnFlying".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteSpawnFlying,
                SetZoteSpawnFlyingEnabled,
                out zoteSpawnFlyingValue
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateToggleRow(
                content,
                "ZoteSpawnHoppingRow",
                "Settings/ZoteHelper/ZoteSpawnHopping".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteSpawnHopping,
                SetZoteSpawnHoppingEnabled,
                out zoteSpawnHoppingValue
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateToggleRow(
                content,
                "ZoteUseCustomFlyingHpRow",
                "Settings/ZoteHelper/UseCustomFlyingHP".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteUseCustomFlyingHp,
                SetZoteUseCustomFlyingHpEnabled,
                out zoteUseCustomFlyingHpValue
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "ZoteFlyingHpRow",
                "Settings/ZoteHelper/ZoteFlyingHP".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteSummonFlyingHp,
                SetZoteFlyingHp,
                0,
                99,
                1,
                out zoteSummonFlyingHpField
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateToggleRow(
                content,
                "ZoteUseCustomHoppingHpRow",
                "Settings/ZoteHelper/UseCustomHoppingHP".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteUseCustomHoppingHp,
                SetZoteUseCustomHoppingHpEnabled,
                out zoteUseCustomHoppingHpValue
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "ZoteHoppingHpRow",
                "Settings/ZoteHelper/ZoteHoppingHP".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteSummonHoppingHp,
                SetZoteHoppingHp,
                0,
                99,
                1,
                out zoteSummonHoppingHpField
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateToggleRow(
                content,
                "ZoteUseCustomSummonLimitRow",
                "Settings/ZoteHelper/UseCustomSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteUseCustomSummonLimit,
                SetZoteUseCustomSummonLimitEnabled,
                out zoteUseCustomSummonLimitValue
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateAdjustInputRow(
                content,
                "ZoteSummonLimitRow",
                "Settings/ZoteHelper/ZoteSummonLimit".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteSummonLimit,
                SetZoteSummonLimit,
                0,
                99,
                1,
                out zoteSummonLimitField
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateToggleRow(
                content,
                "ZoteDoubleSummonsRow",
                "Settings/ZoteHelper/DoubleSummons".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteDoubleSummons,
                SetZoteDoubleSummonsEnabled,
                out zoteDoubleSummonsValue
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;
            CreateToggleRow(
                content,
                "ZoteAttackSummonsOnlyOnStartRow",
                "Settings/ZoteHelper/AttackSummonsOnlyOnStart".Localize(),
                rowY,
                () => Modules.BossChallenge.ZoteHelper.zoteAttackSummonsOnlyOnStart,
                SetZoteAttackSummonsOnlyOnStartEnabled,
                out zoteAttackSummonsOnlyOnStartValue
            );

            lastY = rowY;
            rowY += ZoteHelperRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "ZoteHelperResetRow", "Settings/ZoteHelper/Reset".Localize(), resetY, OnZoteHelperResetDefaultsClicked);
            CreateButtonRow(panel.transform, "ZoteHelperBackRow", "Back", backY, OnZoteHelperBackClicked);
        }
    }
}
