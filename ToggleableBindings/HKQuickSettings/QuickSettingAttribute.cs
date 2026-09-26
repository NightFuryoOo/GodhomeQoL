#nullable enable

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ToggleableBindings.HKQuickSettings
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class QuickSettingAttribute : Attribute
    {
        public string? SettingName { get; }

        public bool IsPerSave { get; }

        public QuickSettingAttribute([CallerMemberName] string? settingName = null, bool isPerSave = false)
        {
            SettingName = settingName;
            IsPerSave = isPerSave;
        }

        public QuickSettingAttribute(bool isPerSave, [CallerMemberName] string? settingName = null) : this(settingName, isPerSave) { }
    }
}