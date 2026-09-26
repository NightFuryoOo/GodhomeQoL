using UnityEngine;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private void OnFpsBoostBackClicked()
        {
            bool reopenQuick = returnToQuickOnClose;
            returnToQuickOnClose = false;

            if (reopenQuick)
            {
                SetQuickVisible(true);
            }

            SetFpsBoostVisible(false);
        }

        private void OnFpsBoostResetDefaultsClicked()
        {
            SetFpsBoostEnabled(false);
            ResetFpsBoostDefaults();
            RefreshFpsBoostUi();
            UpdateQuickMenuEntryStateColors();
        }
    }
}
