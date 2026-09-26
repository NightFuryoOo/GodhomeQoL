using System.IO;
using InControl;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class QuickMenu : Module
{
    private sealed partial class QuickMenuController : MonoBehaviour
    {
        private static GearSwitcherSettings GearSwitcherSettings =>
            GearSwitcher.Settings;

        private string[] GetGearSwitcherPresetOptions()
        {
            return GearSwitcher.GetPresetOrder().ToArray();
        }

        private static bool IsFullGearPreset(string presetName) =>
            string.Equals(presetName, "FullGear", StringComparison.OrdinalIgnoreCase);

        private string GetFullGearDisplayName()
        {
            string name = GearSwitcherSettings.FullGearDisplayName;
            return string.IsNullOrWhiteSpace(name) ? "FullGear" : name;
        }

        private void SetFullGearDisplayName(string name)
        {
            GearSwitcherSettings.FullGearDisplayName = name;
            GodhomeQoL.SaveGlobalSettingsSafe();
        }

        private bool IsGearSwitcherPresetNameTaken(string name, string? ignoreName = null)
        {
            foreach (string preset in GetGearSwitcherPresetOptions())
            {
                if (!string.IsNullOrEmpty(ignoreName)
                    && string.Equals(preset, ignoreName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (IsFullGearPreset(preset))
                {
                    continue;
                }

                if (string.Equals(preset, name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private string GetGearSwitcherPresetDisplayName(string presetName)
        {
            return IsFullGearPreset(presetName) ? GetFullGearDisplayName() : presetName;
        }

        private string GetSelectedPresetDisplayLabel(string presetName)
        {
            string displayName = GetGearSwitcherPresetDisplayName(presetName);
            if (IsBuiltinPresetName(presetName) && IsBuiltinPresetEdited(presetName))
            {
                return $"[Edited] {displayName}";
            }

            return displayName;
        }

        private void UpdateGearSwitcherSelectedPresetColor(string presetName)
        {
            if (gearSwitcherSelectedPresetValue == null)
            {
                return;
            }

            bool edited = IsBuiltinPresetEdited(presetName);
            gearSwitcherSelectedPresetValue.color = edited ? GearSwitcherPresetEditedColor : Color.white;
        }

        private void MarkGearSwitcherPresetEdited()
        {
            UpdateGearSwitcherSelectedPresetLabel(gearSwitcherSelectedPreset);
            UpdateGearSwitcherSelectedPresetColor(gearSwitcherSelectedPreset);
        }

        private void UpdateGearSwitcherSelectedPresetLabel(string presetName)
        {
            if (gearSwitcherSelectedPresetValue == null)
            {
                return;
            }

            gearSwitcherSelectedPresetValue.text = string.IsNullOrWhiteSpace(presetName)
                ? string.Empty
                : GetSelectedPresetDisplayLabel(presetName);
        }

        private bool IsBuiltinPresetEdited(string presetName)
        {
            if (!IsBuiltinPresetName(presetName))
            {
                return false;
            }

            if (GearSwitcherSettings.Presets == null
                || !GearSwitcherSettings.Presets.TryGetValue(presetName, out GearPreset current))
            {
                return false;
            }

            Dictionary<string, GearPreset> defaults = GearPresetDefaults.CreateDefaults();
            if (!defaults.TryGetValue(presetName, out GearPreset baseline))
            {
                return false;
            }

            return !ArePresetsEqual(current, baseline);
        }

        private static bool ArePresetsEqual(GearPreset current, GearPreset baseline)
        {
            if (current.MaxHealth != baseline.MaxHealth
                || current.SoulVessels != baseline.SoulVessels
                || current.NailDamage != baseline.NailDamage
                || current.CharmSlots != baseline.CharmSlots
                || current.MainSoulGain != baseline.MainSoulGain
                || current.ReserveSoulGain != baseline.ReserveSoulGain
                || !AreCharmCostsEqual(current, baseline)
                || current.CarefreeMelodyCost != baseline.CarefreeMelodyCost
                || current.GrimmchildCost != baseline.GrimmchildCost
                || current.VoidHeartCost != baseline.VoidHeartCost
                || current.KingsoulCost != baseline.KingsoulCost
                || current.UseVoidHeart != baseline.UseVoidHeart
                || current.UseGrimmchild != baseline.UseGrimmchild
                || current.DreamNailLevel != baseline.DreamNailLevel
                || current.Nailless != baseline.Nailless
                || current.Overcharmed != baseline.Overcharmed)
            {
                return false;
            }

            if (!EffectiveBoolsEqual(current.HasMoveAbilities, current.HasAllMoveAbilities,
                    baseline.HasMoveAbilities, baseline.HasAllMoveAbilities,
                    new[] { "AcidArmour", "Dash", "Walljump", "SuperDash", "ShadowDash", "DoubleJump" })
                || !DictionaryEquals(current.SpellsLevel, baseline.SpellsLevel)
                || !DictionaryEquals(current.HasNailArts, baseline.HasNailArts)
                || !EffectiveBoolsEqual(current.Bindings, current.HasAllBindings,
                    baseline.Bindings, baseline.HasAllBindings,
                    new[] { "CharmsBinding", "NailBinding", "ShellBinding", "SoulBinding" }))
            {
                return false;
            }

            return ListsEqual(current.EquippedCharms, baseline.EquippedCharms);
        }

        private static bool DictionaryEquals<TKey, TValue>(Dictionary<TKey, TValue>? current, Dictionary<TKey, TValue>? baseline)
            where TKey : notnull
        {
            if (ReferenceEquals(current, baseline))
            {
                return true;
            }

            if (current == null || baseline == null)
            {
                return false;
            }

            if (current.Count != baseline.Count)
            {
                return false;
            }

            foreach (KeyValuePair<TKey, TValue> pair in current)
            {
                if (!baseline.TryGetValue(pair.Key, out TValue? value)
                    || !EqualityComparer<TValue>.Default.Equals(pair.Value, value))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool EffectiveBoolsEqual(
            Dictionary<string, bool>? current,
            bool currentAll,
            Dictionary<string, bool>? baseline,
            bool baselineAll,
            string[] keys)
        {
            foreach (string key in keys)
            {
                bool currentValue = currentAll
                    || (current != null && current.TryGetValue(key, out bool currentValueResult) && currentValueResult);
                bool baselineValue = baselineAll
                    || (baseline != null && baseline.TryGetValue(key, out bool baselineValueResult) && baselineValueResult);
                if (currentValue != baselineValue)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ListsEqual(List<int>? current, List<int>? baseline)
        {
            if (current == null || current.Count == 0)
            {
                return baseline == null || baseline.Count == 0;
            }

            if (baseline == null || baseline.Count == 0)
            {
                return false;
            }

            if (current.Count != baseline.Count)
            {
                return false;
            }

            List<int> currentSorted = new(current);
            List<int> baselineSorted = new(baseline);
            currentSorted.Sort();
            baselineSorted.Sort();

            for (int i = 0; i < currentSorted.Count; i++)
            {
                if (currentSorted[i] != baselineSorted[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsBuiltinPresetName(string presetName)
        {
            foreach (string name in GearPresetDefaults.DefaultOrder())
            {
                if (string.Equals(presetName, name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private string NormalizeCustomPresetName(string currentName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return currentName;
            }

            if (string.Equals(value, currentName, StringComparison.OrdinalIgnoreCase))
            {
                return currentName;
            }

            if (IsBuiltinPresetName(value))
            {
                return currentName;
            }

            if (IsGearSwitcherPresetNameTaken(value, currentName))
            {
                return currentName;
            }

            return value;
        }
    }
}
