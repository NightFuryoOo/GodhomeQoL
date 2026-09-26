#nullable enable

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using ToggleableBindings.Utility;
using ToggleableBindings.VanillaBindings;
using UnityEngine;
using TB = ToggleableBindings.ToggleableBindings;

namespace ToggleableBindings
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Binding
    {
        internal static Sprite UnknownDefault => EmbeddedAssetLoader.UnknownBindingDefault;

        internal static Sprite UnknownSelected => EmbeddedAssetLoader.UnknownBindingSelected;

        private const string MustBeNearBench = "Must be near a bench to {0} this binding.";

        public event BindingEventHandler? Applied;

        public event BindingEventHandler? Restored;

        public string ID { get; }

        public string Name { get; }

        public bool IsApplied { get; private set; }

        public bool IsVanillaBinding { get; }

        public virtual Sprite? DefaultSprite { get; protected set; }

        public virtual Sprite? SelectedSprite { get; protected set; }

        [JsonProperty(Order = -5)]
        protected internal bool WasApplied { get; protected set; }

        public Binding(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Type bindingType = GetType();
            string asmName = bindingType.Assembly.GetName().Name;

            ID = $"{asmName}::{bindingType.Name}";
            Name = name;
            if (IsVanilla(bindingType))
                IsVanillaBinding = true;
        }

        public virtual ResultInfo<bool> CanBeApplied()
        {
            return true;
        }

        public virtual ResultInfo<bool> CanBeRestored()
        {
            return true;
        }

        public void Apply()
        {
            if (IsApplied)
                return;

            IsApplied = true;
            OnApplied();
            Applied?.Invoke(this);
        }

        public void Restore()
        {
            if (!IsApplied)
                return;

            IsApplied = false;
            OnRestored();
            Restored?.Invoke(this);
        }

        protected abstract void OnApplied();

        protected abstract void OnRestored();

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            WasApplied = IsApplied;
        }

        [OnSerialized]
        private void OnSerialized(StreamingContext context)
        {
            WasApplied = false;
        }

        private static bool IsVanilla(Type bindingType)
        {
            return bindingType.IsDefined(typeof(VanillaBindingAttribute), false);
        }
    }
}
