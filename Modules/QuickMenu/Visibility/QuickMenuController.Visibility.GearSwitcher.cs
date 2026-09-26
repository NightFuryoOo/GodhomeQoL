using UnityEngine;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private void SetGearSwitcherVisible(bool value)
        {
            gearSwitcherVisible = value;
            if (gearSwitcherRoot != null)
            {
                gearSwitcherRoot.SetActive(value);
            }

            if (value)
            {
                UpdateGearSwitcherHeaderLayout();
                RefreshGearSwitcherUi();
            }
            else
            {
                SetGearSwitcherResetConfirmVisible(false);
            }

            UpdateUiState();
        }

        private void SetGearSwitcherCharmCostVisible(bool value)
        {
            gearSwitcherCharmCostVisible = value;
            ApplyPanelVisible(gearSwitcherCharmCostRoot, value, RefreshGearSwitcherCharmCostUi);
        }

        private void SetGearSwitcherPresetVisible(bool value)
        {
            gearSwitcherPresetVisible = value;
            if (gearSwitcherPresetRoot != null)
            {
                gearSwitcherPresetRoot.SetActive(value);
            }

            if (value)
            {
                RefreshGearSwitcherPresetSelectUi();
            }
            else if (gearSwitcherPresetRenameField != null)
            {
                CommitGearSwitcherPresetRename(gearSwitcherPresetRenameField.text);
            }

            if (!value)
            {
                SetGearSwitcherPresetDeleteVisible(false);
            }

            UpdateUiState();
        }
    }
}
