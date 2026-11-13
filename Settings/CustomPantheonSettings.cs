namespace GodhomeQoL.Settings;

public sealed class CustomPantheonSettings
{
    public List<CustomPantheonChain> Chains = new();

    internal CustomPantheonChain? SelectedChain => Chains?.FirstOrDefault(chain => chain != null && chain.Selected);

    internal void Select(CustomPantheonChain? selected)
    {
        foreach (CustomPantheonChain chain in Chains ?? new List<CustomPantheonChain>())
        {
            if (chain != null)
            {
                chain.Selected = ReferenceEquals(chain, selected);
            }
        }
    }

    internal void Normalize()
    {
        Chains ??= new List<CustomPantheonChain>();
        Chains.RemoveAll(chain => chain == null);
        bool selectedFound = false;
        foreach (CustomPantheonChain chain in Chains)
        {
            chain.Normalize();
            if (chain.Selected)
            {
                chain.Selected = !selectedFound;
                selectedFound = true;
            }
        }
    }
}

public sealed class CustomPantheonChain
{
    public string Name = string.Empty;
    public bool Selected;
    public List<string> Scenes = new();
    public List<CustomPantheonFight> Fights = new();

    internal void Normalize()
    {
        Name ??= string.Empty;
        Fights ??= new List<CustomPantheonFight>();
        Fights.RemoveAll(fight => fight == null || string.IsNullOrEmpty(fight.Scene));
        foreach (CustomPantheonFight fight in Fights)
        {
            fight.Normalize();
        }

        if (Scenes != null && Scenes.Count > 0)
        {
            foreach (string scene in Scenes)
            {
                if (!string.IsNullOrEmpty(scene))
                {
                    Fights.Add(new CustomPantheonFight { Scene = scene });
                }
            }
        }

        Scenes = new List<string>();
    }
}

public sealed class CustomPantheonFight
{
    public string Scene = string.Empty;
    public string Caption = string.Empty;
    public GearPreset? Gear;
    public bool BossEnabled;
    public Dictionary<string, string>? BossSettings;
    public float GameSpeed = 1f;
    public int MaskDamage = 1;
    public bool RestoreHealth = true;
    public bool InfiniteHp;
    public bool ArrivalAnimation;
    public int? Soul;
    public int? Lifeblood;

    internal const float MaxGameSpeed = 10f;
    internal const float MinGameSpeed = 0.1f;

    internal static bool TryParseGameSpeed(string? text, out float value)
    {
        value = 1f;
        string normalized = (text ?? string.Empty).Trim().Replace(',', '.');
        if (!float.TryParse(normalized, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float parsed)
            || float.IsNaN(parsed)
            || float.IsInfinity(parsed))
        {
            return false;
        }

        value = Math.Max(MinGameSpeed, Math.Min(MaxGameSpeed, (float)Math.Round(parsed, 2)));
        return true;
    }
    internal const int MaxSoul = 198;
    internal const int MaxLifeblood = 99;

    internal bool HasBossSettings => BossEnabled && BossSettings != null && BossSettings.Count > 0;

    internal bool HasCustomSettings =>
        Gear != null
        || HasBossSettings
        || MaskDamage > 1
        || (GameSpeed > 0f && Math.Abs(GameSpeed - 1f) > 0.001f)
        || !RestoreHealth
        || InfiniteHp
        || ArrivalAnimation
        || Soul.HasValue
        || Lifeblood.HasValue;

    internal void Normalize()
    {
        Caption ??= string.Empty;
        if (GameSpeed > MaxGameSpeed)
        {
            GameSpeed = MaxGameSpeed;
        }

        if (Soul.HasValue)
        {
            Soul = Math.Max(0, Math.Min(MaxSoul, Soul.Value));
        }

        if (Lifeblood.HasValue)
        {
            Lifeblood = Math.Max(0, Math.Min(MaxLifeblood, Lifeblood.Value));
        }
    }
}
