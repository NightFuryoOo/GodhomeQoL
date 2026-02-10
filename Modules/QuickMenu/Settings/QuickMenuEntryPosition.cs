namespace GodhomeQoL.Settings;

public sealed class QuickMenuEntryPosition
{
    public QuickMenuEntryPosition()
    {
    }

    public QuickMenuEntryPosition(float x, float y)
    {
        X = x;
        Y = y;
    }

    public float X { get; set; }
    public float Y { get; set; }
}
