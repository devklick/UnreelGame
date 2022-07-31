using Godot;

/// <summary>
/// A ValueSector is a node which is a sector within a spinner which represents only a value.
/// It is used for matching against other ValueSectors.
/// </summary>
public class ValueSector : BaseSector
{
    private int value;
    public static ValueSector Instance(PackedScene scene, Vector2[] points, int value, Spinner spinner)
    {
        var sector = BaseSector.Instance<ValueSector>(scene, points, spinner);
        sector.value = value;
        return sector;
    }
}
