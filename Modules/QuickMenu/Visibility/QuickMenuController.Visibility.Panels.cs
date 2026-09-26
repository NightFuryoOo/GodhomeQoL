using UnityEngine;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void SetFastReloadVisible(bool value)
        {
            fastReloadVisible = value;
            if (fastReloadRoot != null)
            {
                fastReloadRoot.SetActive(value);
            }

            if (value)
            {
                RefreshFastReloadUi();
            }
            else
            {
                CancelFastReloadRebind();
            }

            UpdateUiState();
        }

        private void SetDreamshieldVisible(bool value)
        {
            dreamshieldVisible = value;
            ApplyPanelVisible(dreamshieldRoot, value, RefreshDreamshieldUi);
        }

        private void SetShowHpOnDeathVisible(bool value)
        {
            showHpOnDeathVisible = value;
            if (showHpOnDeathRoot != null)
            {
                showHpOnDeathRoot.SetActive(value);
            }

            if (value)
            {
                RefreshShowHpOnDeathUi();
            }
            else
            {
                CancelShowHpOnDeathRebind();
            }

            UpdateUiState();
        }

        private void SetMaskDamageVisible(bool value)
        {
            maskDamageVisible = value;
            if (maskDamageRoot != null)
            {
                maskDamageRoot.SetActive(value);
            }

            if (value)
            {
                RefreshMaskDamageUi();
            }
            else
            {
                CancelMaskDamageRebind();
            }

            UpdateUiState();
        }

        private void SetFreezeHitboxesVisible(bool value)
        {
            freezeHitboxesVisible = value;
            if (freezeHitboxesRoot != null)
            {
                freezeHitboxesRoot.SetActive(value);
            }

            if (value)
            {
                RefreshFreezeHitboxesUi();
            }
            else
            {
                CancelFreezeHitboxesRebind();
            }

            UpdateUiState();
        }

        private void SetFpsBoostVisible(bool value)
        {
            fpsBoostVisible = value;
            if (fpsBoostRoot != null)
            {
                fpsBoostRoot.SetActive(value);
            }

            if (value)
            {
                RefreshFpsBoostUi();
            }

            UpdateUiState();
        }

        private void SetSpeedChangerVisible(bool value)
        {
            speedChangerVisible = value;
            if (speedChangerRoot != null)
            {
                speedChangerRoot.SetActive(value);
            }

            if (value)
            {
                RefreshSpeedChangerUi();
            }
            else
            {
                CancelSpeedChangerRebind();
            }

            UpdateUiState();
        }

        internal bool IsSpeedChangerVisible()
        {
            return speedChangerVisible;
        }

        internal void SetSpeedChangerVisibleFromExternal(bool value)
        {
            SetSpeedChangerVisible(value);
        }

        private void SetTeleportKitVisible(bool value)
        {
            teleportKitVisible = value;
            if (teleportKitRoot != null)
            {
                teleportKitRoot.SetActive(value);
            }

            if (value)
            {
                RefreshTeleportKitUi();
            }
            else
            {
                CancelTeleportKitRebind();
            }

            UpdateUiState();
        }

        private void SetBossChallengeVisible(bool value)
        {
            bossChallengeVisible = value;
            ApplyPanelVisible(bossChallengeRoot, value, RefreshBossChallengeUi);
        }

        private void SetRandomPantheonsVisible(bool value)
        {
            randomPantheonsVisible = value;
            ApplyPanelVisible(randomPantheonsRoot, value, RefreshRandomPantheonsUi);
        }

        private void SetTrueBossRushVisible(bool value)
        {
            trueBossRushVisible = value;
            ApplyPanelVisible(trueBossRushRoot, value, RefreshTrueBossRushUi);
        }

        private void SetCheatsVisible(bool value)
        {
            cheatsVisible = value;
            if (cheatsRoot != null)
            {
                cheatsRoot.SetActive(value);
            }

            if (value)
            {
                RefreshCheatsUi();
            }
            else
            {
                CancelCheatsKillAllRebind();
            }

            UpdateUiState();
        }

        private void SetAlwaysFuriousVisible(bool value)
        {
            alwaysFuriousVisible = value;
            ApplyPanelVisible(alwaysFuriousRoot, value, RefreshAlwaysFuriousUi);
        }

        private void SetQolVisible(bool value)
        {
            qolVisible = value;
            if (qolRoot != null)
            {
                qolRoot.SetActive(value);
            }

            if (value)
            {
                RefreshQolUi();
                ScrollToTop(qolScrollRect);
            }
            else
            {
                CancelNailDamageCheckRebind();
            }

            UpdateUiState();
        }

        private void SetMenuAnimationVisible(bool value)
        {
            menuAnimationVisible = value;
            ApplyPanelVisible(menuAnimationRoot, value, RefreshMenuAnimationUi);
        }

        private void SetBossAnimationVisible(bool value)
        {
            bossAnimationVisible = value;
            if (bossAnimationRoot != null)
            {
                bossAnimationRoot.SetActive(value);
            }

            if (value)
            {
                RefreshBossAnimationUi();
            }
            else
            {
                CancelFastDreamWarpRebind();
            }

            UpdateUiState();
        }
    }
}
