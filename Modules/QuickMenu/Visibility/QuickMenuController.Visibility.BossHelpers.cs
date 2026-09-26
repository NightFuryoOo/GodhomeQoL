using UnityEngine;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void SetCollectorVisible(bool value)
        {
            collectorVisible = value;
            ApplyPanelVisible(collectorRoot, value, RefreshCollectorPhasesUi);
        }

        private void SetZoteHelperVisible(bool value)
        {
            zoteHelperVisible = value;
            ApplyPanelVisible(zoteHelperRoot, value, RefreshZoteHelperUi);
        }

        private void SetGruzHelperVisible(bool value)
        {
            gruzHelperVisible = value;
            ApplyPanelVisible(gruzHelperRoot, value, RefreshGruzHelperUi);
        }

        private void SetHornetHelperVisible(bool value)
        {
            hornetHelperVisible = value;
            ApplyPanelVisible(hornetHelperRoot, value, RefreshHornetHelperUi);
        }

        private void SetMawlekHelperVisible(bool value)
        {
            mawlekHelperVisible = value;
            ApplyPanelVisible(mawlekHelperRoot, value, RefreshMawlekHelperUi);
        }

        private void SetMassiveMossHelperVisible(bool value)
        {
            massiveMossHelperVisible = value;
            ApplyPanelVisible(massiveMossHelperRoot, value, RefreshMassiveMossHelperUi);
        }

        private void SetCrystalGuardianHelperVisible(bool value)
        {
            crystalGuardianHelperVisible = value;
            ApplyPanelVisible(crystalGuardianHelperRoot, value, RefreshCrystalGuardianHelperUi);
        }

        private void SetEnragedGuardianHelperVisible(bool value)
        {
            enragedGuardianHelperVisible = value;
            ApplyPanelVisible(enragedGuardianHelperRoot, value, RefreshEnragedGuardianHelperUi);
        }

        private void SetHornetSentinelHelperVisible(bool value)
        {
            hornetSentinelHelperVisible = value;
            ApplyPanelVisible(hornetSentinelHelperRoot, value, RefreshHornetSentinelHelperUi);
        }

        private void SetBossManipulateVisible(bool value)
        {
            bossManipulateVisible = value;
            if (bossManipulateRoot != null)
            {
                bossManipulateRoot.SetActive(value);
            }

            if (value)
            {
                RefreshBossManipulateCardVisuals();
                RefreshBossManipulateGlobalUi();
                SetBossManipulateResetConfirmVisible(false);
            }
            else
            {
                SetBossManipulateResetConfirmVisible(false);
            }

            UpdateUiState();
        }
    }
}
