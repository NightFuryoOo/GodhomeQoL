using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void BuildBossChallengeOverlayUi()
        {
            bossChallengeRoot = CreateOverlayFrame("BossChallengeOverlayCanvas", BossChallengeCanvasSortOrder, "BossChallengePanel", BossChallengePanelHeight, "Categories/BossChallenge".Localize(), 52, 420f, out GameObject panel, out RectTransform titleRect);

            float panelHeight = BossChallengePanelHeight;
            RectTransform content = CreateOverlayContent(panel, BossChallengePanelHeight, titleRect, out float resetY, out float backY, out float topOffset, out float viewHeight);
            bossChallengeContent = content;

            float rowY = GetRowStartY(panelHeight, BossChallengeRowStartY, topOffset);
            float lastY = rowY;
            CreateToggleRowWithIcon(
                content,
                "BossChallengeEnableRow",
                "Settings/BossChallenge/Enable".Localize(),
                rowY,
                GetBossChallengeMasterEnabled,
                SetBossChallengeMasterEnabled,
                out bossChallengeEnableValue,
                out bossChallengeEnableIcon
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossInfiniteChallengeRow",
                "Modules/InfiniteChallenge".Localize(),
                rowY,
                GetInfiniteChallengeEnabled,
                SetInfiniteChallengeEnabled,
                out bossInfiniteChallengeValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossRestartOnSuccessRow",
                "Settings/restartFightOnSuccess".Localize(),
                rowY,
                () => Modules.BossChallenge.InfiniteChallenge.restartFightOnSuccess,
                value => Modules.BossChallenge.InfiniteChallenge.restartFightOnSuccess = value,
                out bossRestartOnSuccessValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossRestartAndMusicRow",
                "Settings/restartFightAndMusic".Localize(),
                rowY,
                () => Modules.BossChallenge.InfiniteChallenge.restartFightAndMusic,
                value => Modules.BossChallenge.InfiniteChallenge.restartFightAndMusic = value,
                out bossRestartAndMusicValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateSelectRow(
                content,
                "QolCarefreeMelodyRow",
                "Modules/CarefreeMelodyReset".Localize(),
                rowY,
                GetCarefreeMelodyModeLabel,
                ToggleCarefreeMelodyMode,
                out qolCarefreeMelodyValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossForceArriveRow",
                "Modules/ForceArriveAnimation".Localize(),
                rowY,
                GetForceArriveAnimationEnabled,
                SetForceArriveAnimationEnabled,
                out bossForceArriveValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateCycleRow(
                content,
                "BossChallengeGpzRow",
                "Modules/ForceGreyPrinceEnterType".Localize(),
                rowY,
                BossChallengeGpzOptions,
                GetBossGpzIndex,
                ApplyBossGpzOption,
                out bossGpzValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossInfiniteGrimmRow",
                "Modules/InfiniteGrimmPufferfish".Localize(),
                rowY,
                GetInfiniteGrimmPufferfishEnabled,
                SetInfiniteGrimmPufferfishEnabled,
                out bossInfiniteGrimmValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossInfiniteRadianceRow",
                "Modules/InfiniteRadianceClimbing".Localize(),
                rowY,
                GetInfiniteRadianceClimbingEnabled,
                SetInfiniteRadianceClimbingEnabled,
                out bossInfiniteRadianceValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossSegmentedP5Row",
                "Modules/SegmentedP5".Localize(),
                rowY,
                GetSegmentedP5Enabled,
                SetSegmentedP5Enabled,
                out bossSegmentedP5Value
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossAddLifebloodRow",
                "Modules/AddLifeblood".Localize(),
                rowY,
                GetAddLifebloodEnabled,
                SetAddLifebloodEnabled,
                out bossAddLifebloodValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateAdjustInputRow(
                content,
                "BossLifebloodAmountRow",
                "Settings/lifebloodAmount".Localize(),
                rowY,
                () => Modules.BossChallenge.AddLifeblood.lifebloodAmount,
                value => Modules.BossChallenge.AddLifeblood.lifebloodAmount = Math.Max(0, Math.Min(99, value)),
                0,
                99,
                1,
                out bossLifebloodAmountField
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateToggleRow(
                content,
                "BossAddSoulRow",
                "Modules/AddSoul".Localize(),
                rowY,
                GetAddSoulEnabled,
                SetAddSoulEnabled,
                out bossAddSoulValue
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;
            CreateAdjustInputRow(
                content,
                "BossSoulAmountRow",
                "Settings/soulAmount".Localize(),
                rowY,
                () => Modules.BossChallenge.AddSoul.soulAmount,
                value => Modules.BossChallenge.AddSoul.soulAmount = Math.Max(0, Math.Min(999, value)),
                0,
                999,
                11,
                out bossSoulAmountField
            );

            lastY = rowY;
            rowY += BossChallengeRowSpacing;

            SetScrollContentHeight(content, viewHeight, lastY, RowHeight);
            CreateButtonRow(panel.transform, "BossChallengeResetRow", "BossChallenge/Reset".Localize(), resetY, OnBossChallengeResetDefaultsClicked);
            CreateButtonRow(panel.transform, "BossChallengeBackRow", "Back", backY, OnBossChallengeBackClicked);
        }
    }
}
