using GodhomeQoL.Modules.BossChallenge;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private static readonly string[] GearSwitcherChainHiddenRows =
        {
            "GearSwitcherEnableRow",
            "GearSwitcherPresetRow",
            "GearSwitcherResetGearRow",
            "GearSwitcherResetRow"
        };

        private sealed class CustomPantheonBossEditState
        {
            internal CustomPantheonFight Fight = null!;
            internal int ChainIndex;
            internal int FightIndex;
            internal Type Helper = null!;
            internal Dictionary<string, string> OriginalValues = new();
            internal bool OriginalEnabled;
        }

        private GearPreset? gearSwitcherChainTarget;
        private string gearSwitcherChainTitle = string.Empty;
        private Action? gearSwitcherChainReturn;
        private CustomPantheonBossEditState? customPantheonBossEdit;

        private bool IsGearSwitcherChainMode => gearSwitcherChainTarget != null;

        private void OpenGearSwitcherChainEditor(GearPreset target, string title, Action onReturn)
        {
            gearSwitcherChainTarget = target;
            gearSwitcherChainTitle = title;
            gearSwitcherChainReturn = onReturn;
            GearSwitcher.EditOnlyPreset = target;
            ApplyGearSwitcherChainLayout(true);
            SetGearSwitcherVisible(true);
            ShowStatusMessage("CustomPantheon/GearEditHint".Localize());
        }

        private void ExitGearSwitcherChainMode(bool reopen)
        {
            if (!IsGearSwitcherChainMode)
            {
                return;
            }

            Action? back = gearSwitcherChainReturn;
            gearSwitcherChainTarget = null;
            gearSwitcherChainReturn = null;
            GearSwitcher.EditOnlyPreset = null;
            ApplyGearSwitcherChainLayout(false);
            GodhomeQoL.SaveGlobalSettingsSafe();
            if (gearSwitcherVisible)
            {
                RefreshGearSwitcherUi();
            }

            if (reopen)
            {
                back?.Invoke();
            }
        }

        private void ApplyGearSwitcherChainLayout(bool chain)
        {
            if (gearSwitcherRoot == null)
            {
                return;
            }

            foreach (string name in GearSwitcherChainHiddenRows)
            {
                Transform? row = FindChildRecursive(gearSwitcherRoot.transform, name);
                if (row != null)
                {
                    row.gameObject.SetActive(!chain);
                }
            }

            Text? title = gearSwitcherTitleRect != null ? gearSwitcherTitleRect.GetComponent<Text>() : null;
            if (title != null)
            {
                title.text = chain ? gearSwitcherChainTitle : "Modules/GearSwitcher".Localize();
            }

            if (!chain)
            {
                UpdateGearSwitcherResetGearVisibility();
            }
        }

        private static Transform? FindChildRecursive(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (string.Equals(child.name, name, StringComparison.Ordinal))
                {
                    return child;
                }

                Transform? found = FindChildRecursive(child, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private Action? GetBossManipulatePanelOpener(Type helper)
        {
            foreach (var card in GetBossManipulateCards().Concat(GetBossManipulateOtherRoomsCards()))
            {
                if (card.ModuleType == helper)
                {
                    return card.OnClick;
                }
            }

            return null;
        }

        private bool CanEditCustomPantheonBoss(CustomPantheonFight fight) =>
            CustomPantheonIsolation.GetHelperType(fight.Scene) is Type helper && GetBossManipulatePanelOpener(helper) != null;

        private void StartCustomPantheonBossEdit(int chainIndex, int fightIndex)
        {
            if (chainIndex < 0 || chainIndex >= CustomPantheonChains.Count)
            {
                return;
            }

            CustomPantheonChain chain = CustomPantheonChains[chainIndex];
            if (fightIndex < 0 || fightIndex >= chain.Fights.Count)
            {
                return;
            }

            if (CustomPantheonRunner.IsActive)
            {
                ShowStatusMessage("CustomPantheon/Busy".Localize());
                return;
            }

            CustomPantheonFight fight = chain.Fights[fightIndex];
            Type? helper = CustomPantheonIsolation.GetHelperType(fight.Scene);
            Action? open = helper != null ? GetBossManipulatePanelOpener(helper) : null;
            if (helper == null || open == null)
            {
                ShowStatusMessage("CustomPantheon/NoBossSettings".Localize());
                return;
            }

            CustomPantheonBossEditState state = new()
            {
                Fight = fight,
                ChainIndex = chainIndex,
                FightIndex = fightIndex,
                Helper = helper,
                OriginalValues = CustomPantheonIsolation.CaptureValues(helper),
                OriginalEnabled = CustomPantheonIsolation.IsHelperEnabled(helper)
            };

            if (fight.BossSettings != null)
            {
                CustomPantheonIsolation.ApplyValues(helper, CustomPantheonIsolation.WithDefaults(helper, fight.BossSettings));
                CustomPantheonIsolation.SetHelperEnabled(helper, fight.BossEnabled);
            }

            customPantheonBossEdit = state;
            customPantheonVisible = false;
            SetCustomPantheonConfirmVisible(false);
            if (customPantheonRoot != null)
            {
                customPantheonRoot.SetActive(false);
            }

            open();
            returnToBossManipulateOnClose = false;
            returnToBossManipulateOtherRoomsOnClose = false;
            returnToQuickOnClose = false;
            UpdateQuickMenuEntryStateColors();
            UpdateUiState();
            ShowStatusMessage("CustomPantheon/BossEditHint".Localize());
        }

        private void UpdateCustomPantheonBossEdit()
        {
            if (customPantheonBossEdit != null && !IsAnyUiVisible())
            {
                FinishCustomPantheonBossEdit(true);
            }
        }

        private void FinishCustomPantheonBossEdit(bool reopen)
        {
            CustomPantheonBossEditState? state = customPantheonBossEdit;
            if (state == null)
            {
                return;
            }

            customPantheonBossEdit = null;
            try
            {
                Dictionary<string, string> values = CustomPantheonIsolation.CaptureValues(state.Helper);
                bool enabled = CustomPantheonIsolation.IsHelperEnabled(state.Helper);
                state.Fight.BossEnabled = enabled;
                state.Fight.BossSettings = enabled ? values : null;
                CustomPantheonIsolation.ApplyValues(state.Helper, state.OriginalValues);
                CustomPantheonIsolation.SetHelperEnabled(state.Helper, state.OriginalEnabled);
                GodhomeQoL.SaveGlobalSettingsSafe();
                UpdateQuickMenuEntryStateColors();
            }
            catch (Exception ex)
            {
                LogError($"[CustomPantheon] Saving boss settings failed: {ex}");
            }

            if (reopen)
            {
                customPantheonEditing = state.ChainIndex;
                customPantheonFightEditing = state.FightIndex;
                customPantheonPicking = false;
                SetCustomPantheonVisible(true);
            }
        }

        private void LeaveCustomPantheonModes()
        {
            ExitGearSwitcherChainMode(false);
            FinishCustomPantheonBossEdit(false);
        }
    }
}
