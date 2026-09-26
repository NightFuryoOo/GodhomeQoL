#nullable enable
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ToggleableBindings.Exceptions;
using ToggleableBindings.Extensions;
using ToggleableBindings.HKQuickSettings;
using ToggleableBindings.Utility;
using ToggleableBindings.VanillaBindings;
using UnityEngine;
using Vasi;
using TB = ToggleableBindings.ToggleableBindings;

namespace ToggleableBindings
{
    public static class BindingManager
    {
        #region Exception Messages

        private const string ExcTypeIsBaseBinding = "The specified type must not be equal to 'typeof(" + nameof(Binding) + ")'.";
        private const string ExcTypeIsNotBinding = "The specified type does not inherit from '" + nameof(Binding) + "'.";
        private const string ExcNoBindingWithType = "No registered binding was of the specified type.";
        private const string ExcNoBindingWithID = "No registered binding had the specified ID.";
        private const string ExcBindingAlreadyRegistered = "A binding with the same type is already registered.";
        private const string ExcDeregisterVanillaBinding = "Cannot deregister a base game (vanilla) binding.";

        #endregion

        private static readonly object _lock = new();
        private static readonly Dictionary<Type, Binding> _registeredBindings = new();
        private static readonly Dictionary<string, Type> _bindingIDTypeMap = new();
        private static readonly Type _baseBindingType = typeof(Binding);

        [QuickSetting(true, nameof(RegisteredBindings))]
        private static List<Binding> _serializedBindings = new();

        public static event BindingEventHandler? BindingRegistered;
        public static event BindingEventHandler? BindingDeregistered;
        public static event BindingEventHandler? BindingApplied;
        public static event BindingEventHandler? BindingRestored;

        public static IReadOnlyDictionary<Type, Binding> RegisteredBindings { get; } = _registeredBindings;

        internal static void Initialize()
        {
            TB.MainMenuOrQuit += CleanUpForQuit;

            EnsureVanillaBindings();
        }

        internal static void Unload()
        {
            TB.MainMenuOrQuit -= CleanUpForQuit;

            RestoreAllBindings();
        }

        private static void EnsureVanillaBindings()
        {
            lock (_lock)
            {
                if (!IsBindingRegistered<NailBinding>())
                    RegisterBinding<NailBinding>();

                if (!IsBindingRegistered<ShellBinding>())
                    RegisterBinding<ShellBinding>();

                if (!IsBindingRegistered<CharmsBinding>())
                    RegisterBinding<CharmsBinding>();

                if (!IsBindingRegistered<SoulBinding>())
                    RegisterBinding<SoulBinding>();
            }
        }

        private static void CleanUpForQuit()
        {
            Action<int>? handler = null;
            TB.Instance.Settings.SaveSettingsSaved += handler = (saveSlot) =>
            {
                TB.Instance.Settings.SaveSettingsSaved -= handler;

                RestoreAllBindings();
                EnsureVanillaBindings();
            };
        }

        #region IsBindingRegistered

        public static bool IsBindingRegistered(Binding binding)
        {
            if (binding == null)
                throw new ArgumentNullException(nameof(binding));

            lock (_lock)
                return RegisteredBindings.TryGetValue(binding.GetType(), out var registered) && binding == registered;
        }

        public static bool IsBindingRegistered(Type bindingType)
        {
            TypeIsValidBinding(bindingType, nameof(bindingType)).ThrowIfUnsuccessful();

            lock (_lock)
                return RegisteredBindings.ContainsKey(bindingType);
        }

        public static bool IsBindingRegistered(string bindingID)
        {
            if (bindingID == null)
                throw new ArgumentNullException(nameof(bindingID));

            lock (_lock)
                return _bindingIDTypeMap.ContainsKey(bindingID);
        }

        public static bool IsBindingRegistered<T>() where T : Binding
        {
            TypeIsValidBinding<T>(nameof(T)).ThrowIfUnsuccessful();

            lock (_lock)
                return RegisteredBindings.ContainsKey(typeof(T));
        }

        #endregion

        #region RegisterBinding

        public static void RegisterBinding(Binding binding)
        {
            if (binding == null)
                throw new ArgumentNullException(nameof(binding));

            Type bindingType = binding.GetType();
            if (IsBindingRegistered(bindingType))
                throw new InvalidOperationException(ExcBindingAlreadyRegistered);

            AddBinding(binding);

            OnBindingRegistered(binding);
        }

        public static void RegisterBinding<T>() where T : Binding, new()
        {
            TypeIsValidBinding<T>(nameof(T)).ThrowIfUnsuccessful();

            RegisterBinding(new T());
        }

        private static void OnBindingRegistered(Binding binding)
        {
            TB.Instance.LogDebug($"Registered binding '{binding.ID}'.");
            BindingRegistered?.Invoke(binding);
        }

        #endregion

        #region DeregisterBinding

        public static void DeregisterBinding(Binding binding)
        {
            if (binding == null)
                throw new ArgumentNullException(nameof(binding));

            Type bindingType = binding.GetType();
            if (!IsBindingRegistered(bindingType))
                throw new InvalidOperationException(ExcNoBindingWithType);

            if (binding.IsApplied)
                binding.Restore();

            RemoveBinding(binding);

            OnBindingDeregistered(binding);
        }

        public static void DeregisterBinding(Type bindingType)
        {
            TypeIsValidBinding(bindingType, nameof(bindingType)).ThrowIfUnsuccessful();
            TypeIsRegisteredBinding(bindingType).ThrowIfUnsuccessful();

            Binding binding;
            lock (_lock)
                binding = RegisteredBindings[bindingType];

            DeregisterBinding(binding);
        }

        public static void DeregisterBinding(string bindingID)
        {
            if (bindingID == null)
                throw new ArgumentNullException(nameof(bindingID));

            if (!IsBindingRegistered(bindingID))
                throw new InvalidOperationException(ExcNoBindingWithID);

            Type bindingType;
            lock (_lock)
                bindingType = _bindingIDTypeMap[bindingID];

            DeregisterBinding(bindingType);
        }

        public static void DeregisterBinding<T>() where T : Binding
        {
            TypeIsValidBinding<T>(nameof(T)).ThrowIfUnsuccessful();

            DeregisterBinding(typeof(T));
        }

        private static void OnBindingDeregistered(Binding binding)
        {
            TB.Instance.LogDebug($"Deregistered binding '{binding.ID}'.");
            BindingDeregistered?.Invoke(binding);
        }

        #endregion

        #region GetBinding

        public static Binding GetBinding(Type bindingType)
        {
            TypeIsValidBinding(bindingType, nameof(bindingType)).ThrowIfUnsuccessful();

            return TypeIsRegisteredBinding(bindingType).GetValueOrThrow();
        }

        public static Binding GetBinding(string bindingID)
        {
            if (bindingID == null)
                throw new ArgumentNullException(nameof(bindingID));

            return IDIsRegisteredBinding(bindingID).GetValueOrThrow();
        }

        public static T GetBinding<T>() where T : Binding
        {
            TypeIsValidBinding<T>(nameof(T)).ThrowIfUnsuccessful();

            return TypeIsRegisteredBinding<T>().GetValueOrThrow();
        }

        public static bool TryGetBinding([NotNullWhen(true)] Type? bindingType, [NotNullWhen(true)] out Binding? value)
        {
            value = null;

            bool validBinding = TypeIsValidBinding(bindingType, nameof(bindingType));
            if (!validBinding)
                return false;

            TryResult<Binding> registeredBinding = TypeIsRegisteredBinding(bindingType);
            if (!registeredBinding)
                return false;

            value = registeredBinding.Value;
            return true;
        }

        public static bool TryGetBinding([NotNullWhen(true)] string? bindingID, [NotNullWhen(true)] out Binding? value)
        {
            value = null;

            if (bindingID == null)
                return false;

            TryResult<Binding> registeredBinding = IDIsRegisteredBinding(bindingID);
            if (!registeredBinding)
                return false;

            value = registeredBinding.Value;
            return true;
        }

        public static bool TryGetBinding<T>([NotNullWhen(true)] out T? value) where T : Binding
        {
            value = default;

            bool validBinding = TypeIsValidBinding<T>(nameof(T));
            if (!validBinding)
                return false;

            TryResult<T> registeredBinding = TypeIsRegisteredBinding<T>();
            if (!registeredBinding)
                return false;

            value = registeredBinding.Value;
            return true;
        }

        #endregion

        #region ApplyBinding

        public static void ApplyBinding(Type bindingType)
        {
            Binding binding = GetBinding(bindingType);
            binding.Apply();
        }

        public static void ApplyBinding(string bindingID)
        {
            Binding binding = GetBinding(bindingID);
            binding.Apply();
        }

        public static void ApplyBinding<T>() where T : Binding
        {
            Binding binding = GetBinding<T>();
            binding.Apply();
        }

        private static void OnBindingApplied(Binding binding)
        {
            TB.Instance.LogDebug($"Applied binding '{binding.ID}'.");
            BindingApplied?.Invoke(binding);
        }

        #endregion

        #region RestoreBinding

        public static void RestoreBinding(Type bindingType)
        {
            Binding binding = GetBinding(bindingType);
            binding.Restore();
        }

        public static void RestoreBinding(string bindingID)
        {
            Binding binding = GetBinding(bindingID);
            binding.Restore();
        }

        public static void RestoreBinding<T>() where T : Binding
        {
            Binding binding = GetBinding<T>();
            binding.Restore();
        }

        public static void RestoreAllBindings()
        {
            lock (_lock)
            {
                foreach (var binding in RegisteredBindings.Values)
                    binding.Restore();
            }
        }

        private static void OnBindingRestored(Binding binding)
        {
            TB.Instance.LogDebug($"Restored binding '{binding.ID}'.");
            BindingRestored?.Invoke(binding);
        }

        #endregion

        #region SetActiveBindings

        public static void SetActiveBindings(IEnumerable<Binding> bindings)
        {
            SetActiveBindings(bindings.Select(binding => binding.GetType()));
        }

        public static void SetActiveBindings(IEnumerable<Type> bindingTypes)
        {
            lock (_lock)
            {
                HashSet<Type> shouldBeActiveSet = new(bindingTypes);

                foreach (var item in RegisteredBindings)
                {
                    Type type = item.Key;
                    Binding binding = item.Value;

                    bool shouldToggle = shouldBeActiveSet.Contains(type) != binding.IsApplied;
                    if (shouldToggle)
                    {
                        if (!binding.IsApplied)
                            ApplyBinding(type);
                        else
                            RestoreBinding(type);
                    }
                }
            }
        }

        #endregion

        #region Validation/Utility Methods

        private static void AddBinding(Binding binding)
        {
            binding.Applied += OnBindingApplied;
            binding.Restored += OnBindingRestored;

            Type bindingType = binding.GetType();
            lock (_lock)
            {
                _registeredBindings.Add(bindingType, binding);
                _bindingIDTypeMap.Add(binding.ID, bindingType);
            }
        }

        private static void RemoveBinding(Binding binding)
        {
            binding.Applied -= OnBindingApplied;
            binding.Restored -= OnBindingRestored;

            Type bindingType = binding.GetType();
            lock (_lock)
            {
                _registeredBindings.Remove(bindingType);
                _bindingIDTypeMap.Remove(binding.ID);
            }
        }

        private static TryResult TypeIsValidBinding([System.Diagnostics.CodeAnalysis.NotNull] Type? type, string paramName)
        {
            if (type == null)
#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.
                return new ArgumentNullException(paramName);
#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.

            if (type == _baseBindingType)
                return new ArgumentException(ExcTypeIsBaseBinding, paramName);

            if (!type.IsAssignableTo(_baseBindingType))
                return new ArgumentException(ExcTypeIsNotBinding);

            return TryResult.Success;
        }

        private static TryResult TypeIsValidBinding<T>(string typeParamName)
        {
            if (typeof(T) == _baseBindingType)
                return new TypeArgumentException(ExcTypeIsBaseBinding, typeParamName);

            return TryResult.Success;
        }

        private static TryResult<Binding> TypeIsRegisteredBinding(Type type)
        {
            lock (_lock)
            {
                if (!RegisteredBindings.TryGetValue(type, out Binding? value))
                    return new InvalidOperationException(ExcNoBindingWithType);

                return value;
            }
        }

        private static TryResult<Binding> IDIsRegisteredBinding(string id)
        {
            lock (_lock)
            {
                if (!_bindingIDTypeMap.TryGetValue(id, out Type? value))
                    return new InvalidOperationException(ExcNoBindingWithID);

                return TypeIsRegisteredBinding(value);
            }
        }

        private static TryResult<T> TypeIsRegisteredBinding<T>() where T : Binding
        {
            lock (_lock)
            {
                if (!RegisteredBindings.TryGetValue(typeof(T), out Binding? value))
                    return new InvalidOperationException(ExcNoBindingWithType);

                return (T)value;
            }
        }

        #endregion

        #region Serialization

        private static void OnSerializing()
        {
            TB.Instance.LogDebug(nameof(BindingManager) + ": OnSerializing");
            _serializedBindings = _registeredBindings.Values.ToList();
        }

        private static void OnDeserialized()
        {
            TB.Instance.LogDebug(nameof(BindingManager) + ": OnDeserialized");
            CoroutineController.Start(RegisterDeserializedBindings());
        }

        private static IEnumerator RegisterDeserializedBindings()
        {
            yield return null;

            lock (_lock)
            {
                foreach (var binding in _serializedBindings)
                {
                    Type bindingType = binding.GetType();

                    if (RegisteredBindings.TryGetValue(bindingType, out Binding? existingBinding))
                    {
                        if (existingBinding.IsApplied)
                            existingBinding.Restore();

                        RemoveBinding(existingBinding);
                    }

                    AddBinding(binding);

                    if (binding.WasApplied)
                        ApplyBinding(bindingType);
                }
            }
        }

        #endregion
    }
}
