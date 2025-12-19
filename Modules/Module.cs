<<<<<<< HEAD
namespace GodhomeQoL.Modules
=======
﻿namespace SafeGodseekerQoL.Modules
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265
{
    [MeansImplicitUse]
    [UsedImplicitly]
    public abstract class Module
    {
        public Type Type { get; private init; }

        public string Name { get; private init; }

        public string Category { get; private init; }

        public virtual bool DefaultEnabled => false;

        public virtual ToggleableLevel ToggleableLevel => ToggleableLevel.AnyTime;

        public virtual bool Hidden => false;

        public bool Loaded { get; private set; }

        internal Dictionary<int, string> suppressorMap = [];

        internal bool Suppressed => suppressorMap.Count > 0;

        private bool enabled;

        private bool active;


        internal Module()
        {
            Type = GetType();
            Name = Type.Name;
            Category = Type.FullName
<<<<<<< HEAD
                .StripStart($"{nameof(GodhomeQoL)}.{nameof(Modules)}.")
=======
                .StripStart($"{nameof(SafeGodseekerQoL)}.{nameof(Modules)}.")
>>>>>>> 4ce2448229730eb047aa9980d21cea2bcc48d265
                .StripEnd($".{Name}")
                ?? nameof(Misc);
            enabled = DefaultEnabled;
        }

        public bool Enabled
        {
            get => enabled || Hidden;
            internal set
            {
                enabled = Setting.Global.Modules[Name] = value;
                UpdateStatus();
            }
        }

        internal bool Active
        {
            get => active && !Suppressed;
            set
            {
                active = value;
                UpdateStatus();
            }
        }

        internal void UpdateStatus()
        {
            if (Active && Enabled)
            {
                if (!Loaded)
                {
                    try
                    {
                        Load();
                        LogDebug($"Activated module {Name}");
                        Loaded = true;
                    }
                    catch (Exception e)
                    {
                        LogError($"Failed to activate module {Name} - {e}");
                    }
                }
            }
            else
            {
                if (Loaded)
                {
                    Unload();
                    LogDebug($"Deactivated module {Name}");
                    Loaded = false;
                }
            }
        }

        private protected virtual void Load() { }

        private protected virtual void Unload() { }
    }
}
