#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ToggleableBindings.HKQuickSettings
{
    internal sealed class QuickSettingInfo
    {
        private readonly string? _name;

        public MemberInfo MemberInfo { get; init; }

        [AllowNull]
        public string Name
        {
            get => _name ?? MemberInfo.Name;
            init => _name = value;
        }

        public string Key => $"{MemberInfo.DeclaringType.Name}.{Name}";

        public bool IsPerSave { get; init; }

        public QuickSettingInfo(MemberInfo memberInfo, string? settingName, bool isPerSave = false)
        {
            MemberInfo = memberInfo;
            Name = settingName;
            IsPerSave = isPerSave;
        }
    }
}