using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private bool GetMenuAnimationMasterEnabled() => menuAnimMasterEnabled;

        internal void InvalidateQolSnapshotFromExternal()
        {
            qolMasterHasSnapshot = false;
        }

        internal void InvalidateMenuAnimationSnapshotFromExternal()
        {
            menuAnimMasterHasSnapshot = false;
        }

        internal void InvalidateBossAnimationSnapshotFromExternal()
        {
            bossAnimMasterHasSnapshot = false;
        }

        private void SetMenuAnimationMasterEnabled(bool value)
        {
            if (menuAnimMasterEnabled == value)
            {
                return;
            }

            menuAnimMasterEnabled = value;
            if (!value)
            {
                CaptureMenuAnimationSnapshot();
                SetMenuAnimationAll(false);
            }
            else
            {
                RestoreMenuAnimationSnapshot();
            }

            RefreshMenuAnimationUi();
            UpdateQuickMenuEntryStateColors();
            SaveMasterSettings();
        }

        private void CaptureMenuAnimationSnapshot()
        {
            menuAnimMasterHasSnapshot = true;
            menuAnimSavedDoorDefaultBegin = GetDoorDefaultBeginEnabled();
            menuAnimSavedFasterLoads = GetFasterLoadsEnabled();
            menuAnimSavedFastMenus = GetFastMenusEnabled();
            menuAnimSavedFastText = GetFastTextEnabled();
            menuAnimSavedAutoSkip = Modules.QoL.SkipCutscenes.AutoSkipCinematics;
            menuAnimSavedAllowSkipping = Modules.QoL.SkipCutscenes.AllowSkippingNonskippable;
            menuAnimSavedSkipWithoutPrompt = Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt;
        }

        private void RestoreMenuAnimationSnapshot()
        {
            if (!menuAnimMasterHasSnapshot)
            {
                return;
            }

            SetDoorDefaultBeginEnabled(menuAnimSavedDoorDefaultBegin);
            SetFasterLoadsEnabled(menuAnimSavedFasterLoads);
            SetFastMenusEnabled(menuAnimSavedFastMenus);
            SetFastTextEnabled(menuAnimSavedFastText);
            Modules.QoL.SkipCutscenes.AutoSkipCinematics = menuAnimSavedAutoSkip;
            Modules.QoL.SkipCutscenes.AllowSkippingNonskippable = menuAnimSavedAllowSkipping;
            Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt = menuAnimSavedSkipWithoutPrompt;
        }

        private void SetMenuAnimationAll(bool value)
        {
            SetDoorDefaultBeginEnabled(value);
            SetFasterLoadsEnabled(value);
            SetFastMenusEnabled(value);
            SetFastTextEnabled(value);
            Modules.QoL.SkipCutscenes.AutoSkipCinematics = value;
            Modules.QoL.SkipCutscenes.AllowSkippingNonskippable = value;
            Modules.QoL.SkipCutscenes.SkipCutscenesWithoutPrompt = value;
        }

        private bool GetQolMasterEnabled() => qolMasterEnabled;

        private void SetQolMasterEnabled(bool value)
        {
            if (qolMasterEnabled == value)
            {
                return;
            }

            qolMasterEnabled = value;
            if (!value)
            {
                CaptureQolSnapshot();
                SetQolAll(false);
            }
            else
            {
                RestoreQolSnapshot();
            }

            RefreshQolUi();
            UpdateQuickMenuEntryStateColors();
            SaveMasterSettings();
        }

        private void CaptureQolSnapshot()
        {
            qolMasterHasSnapshot = true;
            qolSavedFastDreamWarp = GetFastDreamWarpEnabled();
            qolSavedShortDeath = GetShortDeathAnimationEnabled();
            qolSavedUnlockAllModes = GetUnlockAllModesEnabled();
            qolSavedUnlockPantheons = GetUnlockPantheonsEnabled();
            qolSavedUnlockRadiance = GetUnlockRadianceEnabled();
            qolSavedUnlockRadiant = GetUnlockRadiantEnabled();
            qolSavedInvincibleIndicator = GetInvincibleIndicatorEnabled();
            qolSavedScreenShake = GetScreenShakeEnabled();
        }

        private void RestoreQolSnapshot()
        {
            if (!qolMasterHasSnapshot)
            {
                return;
            }

            SetFastDreamWarpEnabled(qolSavedFastDreamWarp);
            SetShortDeathAnimationEnabled(qolSavedShortDeath);
            SetUnlockAllModesEnabled(qolSavedUnlockAllModes);
            SetUnlockPantheonsEnabled(qolSavedUnlockPantheons);
            SetUnlockRadianceEnabled(qolSavedUnlockRadiance);
            SetUnlockRadiantEnabled(qolSavedUnlockRadiant);
            SetInvincibleIndicatorEnabled(qolSavedInvincibleIndicator);
            SetScreenShakeEnabled(qolSavedScreenShake);
        }

        private void SetQolAll(bool value)
        {
            SetFastDreamWarpEnabled(value);
            SetShortDeathAnimationEnabled(value);
            SetUnlockAllModesEnabled(value);
            SetUnlockPantheonsEnabled(value);
            SetUnlockRadianceEnabled(value);
            SetUnlockRadiantEnabled(value);
            SetInvincibleIndicatorEnabled(value);
            SetScreenShakeEnabled(value);
        }
    }
}
