namespace GodhomeQoL.Modules
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

        // Hidden controls menu visibility only.
        // AlwaysEnabled is for service modules that should stay loaded
        // and use their own internal feature toggles.
        public virtual bool AlwaysEnabled => false;

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
                .StripStart($"{nameof(GodhomeQoL)}.{nameof(Modules)}.")
                .StripEnd($".{Name}")
                ?? nameof(Misc);
            enabled = DefaultEnabled;
        }

        public bool Enabled
        {
            get => enabled || AlwaysEnabled;
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
                    try
                    {
                        Unload();
                        LogDebug($"Deactivated module {Name}");
                        Loaded = false;
                    }
                    catch (Exception e)
                    {
                        LogError($"Failed to deactivate module {Name} - {e}");
                    }
                }
            }
        }

        private protected virtual void Load() { }

        private protected virtual void Unload() { }
    }
}
