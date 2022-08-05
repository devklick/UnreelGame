using Godot;

/// <summary>
/// A ValueSector is a node which is a sector within a spinner which represents only a value.
/// It is used for matching against other ValueSectors.
/// </summary>
public class ValueSector : BaseSector
{
    public int Value { get; private set; }
    public static ValueSector Instance(PackedScene scene, Vector2[] points, int value, Spinner spinner, Label label)
    {
        var sector = BaseSector.Instance<ValueSector>(scene, points, spinner, label);
        sector.Value = value;
        return sector;
    }
}
