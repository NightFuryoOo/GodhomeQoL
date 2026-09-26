using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

using TB = ToggleableBindings.ToggleableBindings;

namespace ToggleableBindings.Extensions
{
    public static class ComponentExtensions
    {
        private const BindingFlags AllInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        internal static T CopyFrom<T>(this T component, T source, bool replaceSelfGORefs = true, BindingFlags reflFlags = AllInstance) where T : Component
        {
            if (!component)
                throw new ArgumentNullException(nameof(component));

            if (!source)
                throw new ArgumentNullException(nameof(source));

            Type compType = typeof(T);
            GameObject sourceGO = source.gameObject;
            GameObject selfGO = component.gameObject;

            var componentMembers = compType.GetMembers(reflFlags).Where(member => member is FieldInfo or PropertyInfo);
            foreach (var member in componentMembers)
            {
                bool canReplaceGO = replaceSelfGORefs && member.GetUnderlyingType() == typeof(GameObject);

                if (member is PropertyInfo property && property.CanWrite)
                {
                    try
                    {
                        var sourceValue = property.GetValue(source, null);
                        var finalValue = (canReplaceGO && ReferenceEquals(sourceValue, sourceGO)) ? selfGO : sourceValue;

                        property.SetValue(component, finalValue, null);
                    }
                    catch 
                    {
                        TB.Instance.LogWarn($"Couldn't copy component value for property '{property.Name}'.");
                    }
                }
                else if (member is FieldInfo field)
                {
                    var sourceValue = field.GetValue(source);
                    var finalValue = (canReplaceGO && ReferenceEquals(sourceValue, sourceGO)) ? selfGO : sourceValue;

                    field.SetValue(component, finalValue);
                }
            }

            return component;
        }
    }
}
