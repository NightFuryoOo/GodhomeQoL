using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController
    {
        private bool GetRandomPantheonsEnabled()
        {
            return GetRandomPantheonsModule()?.Enabled ?? false;
        }

        private void SetRandomPantheonsEnabled(bool value)
        {
            if (!value)
            {
                Modules.BossChallenge.RandomPantheons.ForceRestoreNow();
            }

            Module? module = GetRandomPantheonsModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            if (!value)
            {
                Modules.BossChallenge.RandomPantheons.ForceRestoreNow();
            }

            UpdateQuickMenuEntryStateColors();
        }

        private bool GetRandomPantheonsMasterEnabled() => randomPantheonsMasterEnabled;

        private void SetRandomPantheonsMasterEnabled(bool value)
        {
            if (randomPantheonsMasterEnabled == value)
            {
                return;
            }

            if (value)
            {
                bool hadTrueBossRush =
                    GetTrueBossRushMasterEnabled()
                    || Modules.BossChallenge.TrueBossRush.AnyPantheonEnabled;
                bool hadSegmentedP5 = GetSegmentedP5Enabled();

                if (hadTrueBossRush)
                {
                    SetTrueBossRushMasterEnabled(false);
                    _ = Modules.BossChallenge.PantheonSequenceCompatibility.DisableTrueBossRush();
                }

                if (hadSegmentedP5)
                {
                    SetSegmentedP5Enabled(false);
                }

                if (hadTrueBossRush || hadSegmentedP5)
                {
                    ShowStatusMessage("Random Pantheons disabled True Boss Rush / Segmented P5.");
                }
            }

            randomPantheonsMasterEnabled = value;
            if (!value)
            {
                CaptureRandomPantheonsSnapshot();
                SetRandomPantheonsAll(false);
                SetRandomPantheonsEnabled(false);
            }
            else
            {
                if (!GetRandomPantheonsEnabled())
                {
                    SetRandomPantheonsEnabled(true);
                }
                RestoreRandomPantheonsSnapshot();
            }

            RefreshRandomPantheonsUi();
            UpdateQuickMenuEntryStateColors();
            SaveMasterSettings();
        }

        private void CaptureRandomPantheonsSnapshot()
        {
            randomPantheonsMasterHasSnapshot = true;
            randomPantheonsSavedP1 = GetRandomPantheonsP1Enabled();
            randomPantheonsSavedP2 = GetRandomPantheonsP2Enabled();
            randomPantheonsSavedP3 = GetRandomPantheonsP3Enabled();
            randomPantheonsSavedP4 = GetRandomPantheonsP4Enabled();
            randomPantheonsSavedP5 = GetRandomPantheonsP5Enabled();
        }

        private void RestoreRandomPantheonsSnapshot()
        {
            if (!randomPantheonsMasterHasSnapshot)
            {
                return;
            }

            SetRandomPantheonsP1Enabled(randomPantheonsSavedP1);
            SetRandomPantheonsP2Enabled(randomPantheonsSavedP2);
            SetRandomPantheonsP3Enabled(randomPantheonsSavedP3);
            SetRandomPantheonsP4Enabled(randomPantheonsSavedP4);
            SetRandomPantheonsP5Enabled(randomPantheonsSavedP5);
        }

        private void SetRandomPantheonsAll(bool value)
        {
            SetRandomPantheonsP1Enabled(value);
            SetRandomPantheonsP2Enabled(value);
            SetRandomPantheonsP3Enabled(value);
            SetRandomPantheonsP4Enabled(value);
            SetRandomPantheonsP5Enabled(value);
        }

        private bool GetRandomPantheonsP1Enabled() => Modules.BossChallenge.RandomPantheons.Pantheon1Enabled;

        private void SetRandomPantheonsP1Enabled(bool value)
        {
            Modules.BossChallenge.RandomPantheons.Pantheon1Enabled = value;
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(1);
            SaveMasterSettings();
        }

        private bool GetRandomPantheonsP2Enabled() => Modules.BossChallenge.RandomPantheons.Pantheon2Enabled;

        private void SetRandomPantheonsP2Enabled(bool value)
        {
            Modules.BossChallenge.RandomPantheons.Pantheon2Enabled = value;
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(2);
            SaveMasterSettings();
        }

        private bool GetRandomPantheonsP3Enabled() => Modules.BossChallenge.RandomPantheons.Pantheon3Enabled;

        private void SetRandomPantheonsP3Enabled(bool value)
        {
            Modules.BossChallenge.RandomPantheons.Pantheon3Enabled = value;
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(3);
            SaveMasterSettings();
        }

        private bool GetRandomPantheonsP4Enabled() => Modules.BossChallenge.RandomPantheons.Pantheon4Enabled;

        private void SetRandomPantheonsP4Enabled(bool value)
        {
            Modules.BossChallenge.RandomPantheons.Pantheon4Enabled = value;
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(4);
            SaveMasterSettings();
        }

        private bool GetRandomPantheonsP5Enabled() => Modules.BossChallenge.RandomPantheons.Pantheon5Enabled;

        private void SetRandomPantheonsP5Enabled(bool value)
        {
            Modules.BossChallenge.RandomPantheons.Pantheon5Enabled = value;
            Modules.BossChallenge.RandomPantheons.RefreshPantheon(5);
            SaveMasterSettings();
        }

        private bool GetTrueBossRushEnabled()
        {
            return GetTrueBossRushModule()?.Enabled ?? false;
        }

        private void SetTrueBossRushEnabled(bool value)
        {
            if (!value)
            {
                Modules.BossChallenge.TrueBossRush.ForceRestoreNow();
                Modules.BossChallenge.RandomPantheons.ForceRestoreNow();
            }

            Module? module = GetTrueBossRushModule();
            if (module != null)
            {
                module.Enabled = value;
            }

            if (!value)
            {
                Modules.BossChallenge.TrueBossRush.ForceRestoreNow();
                Modules.BossChallenge.RandomPantheons.ForceRestoreNow();
            }

            UpdateQuickMenuEntryStateColors();
        }

        private bool GetTrueBossRushMasterEnabled() => trueBossRushMasterEnabled;

        private void SetTrueBossRushMasterEnabled(bool value)
        {
            if (trueBossRushMasterEnabled == value)
            {
                return;
            }

            if (value)
            {
                bool hadRandomPantheons =
                    GetRandomPantheonsMasterEnabled()
                    || GetRandomPantheonsEnabled()
                    || Modules.BossChallenge.RandomPantheons.AnyPantheonEnabled;
                bool hadSegmentedP5 = GetSegmentedP5Enabled();

                if (hadRandomPantheons)
                {
                    SetRandomPantheonsMasterEnabled(false);
                    SetRandomPantheonsEnabled(false);
                    _ = Modules.BossChallenge.PantheonSequenceCompatibility.DisableRandomPantheons();
                }

                if (hadSegmentedP5)
                {
                    SetSegmentedP5Enabled(false);
                }

                if (hadRandomPantheons || hadSegmentedP5)
                {
                    ShowStatusMessage("True Boss Rush disabled Random Pantheons / Segmented P5.");
                }
            }

            trueBossRushMasterEnabled = value;
            if (!value)
            {
                CaptureTrueBossRushSnapshot();
                SetTrueBossRushAll(false);
                SetTrueBossRushEnabled(false);
            }
            else
            {
                if (!GetTrueBossRushEnabled())
                {
                    SetTrueBossRushEnabled(true);
                }

                RestoreTrueBossRushSnapshot();
            }

            RefreshTrueBossRushUi();
            UpdateQuickMenuEntryStateColors();
            SaveMasterSettings();
        }

        private void CaptureTrueBossRushSnapshot()
        {
            trueBossRushMasterHasSnapshot = true;
            trueBossRushSavedP1 = GetTrueBossRushP1Enabled();
            trueBossRushSavedP2 = GetTrueBossRushP2Enabled();
            trueBossRushSavedP3 = GetTrueBossRushP3Enabled();
            trueBossRushSavedP4 = GetTrueBossRushP4Enabled();
            trueBossRushSavedP5 = GetTrueBossRushP5Enabled();
        }

        private void RestoreTrueBossRushSnapshot()
        {
            if (!trueBossRushMasterHasSnapshot)
            {
                return;
            }

            SetTrueBossRushP1Enabled(trueBossRushSavedP1);
            SetTrueBossRushP2Enabled(trueBossRushSavedP2);
            SetTrueBossRushP3Enabled(trueBossRushSavedP3);
            SetTrueBossRushP4Enabled(trueBossRushSavedP4);
            SetTrueBossRushP5Enabled(trueBossRushSavedP5);
        }

        private void SetTrueBossRushAll(bool value)
        {
            SetTrueBossRushP1Enabled(value);
            SetTrueBossRushP2Enabled(value);
            SetTrueBossRushP3Enabled(value);
            SetTrueBossRushP4Enabled(value);
            SetTrueBossRushP5Enabled(value);
        }

        private bool GetTrueBossRushP1Enabled() => Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon1Enabled;

        private void SetTrueBossRushP1Enabled(bool value)
        {
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon1Enabled = value;
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(1);
            SaveMasterSettings();
        }

        private bool GetTrueBossRushP2Enabled() => Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon2Enabled;

        private void SetTrueBossRushP2Enabled(bool value)
        {
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon2Enabled = value;
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(2);
            SaveMasterSettings();
        }

        private bool GetTrueBossRushP3Enabled() => Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon3Enabled;

        private void SetTrueBossRushP3Enabled(bool value)
        {
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon3Enabled = value;
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(3);
            SaveMasterSettings();
        }

        private bool GetTrueBossRushP4Enabled() => Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon4Enabled;

        private void SetTrueBossRushP4Enabled(bool value)
        {
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon4Enabled = value;
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(4);
            SaveMasterSettings();
        }

        private bool GetTrueBossRushP5Enabled() => Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon5Enabled;

        private void SetTrueBossRushP5Enabled(bool value)
        {
            Modules.BossChallenge.TrueBossRush.TrueBossRushPantheon5Enabled = value;
            Modules.BossChallenge.TrueBossRush.RefreshPantheon(5);
            SaveMasterSettings();
        }
    }
}
