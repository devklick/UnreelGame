using Godot;

public class BaseSector : Area2D
{
    [Export]
    protected Color Color;

    protected Vector2[] Points;
    protected Spinner Spinner;
    protected CollisionPolygon2D CollisionPolygon2D;

    public override void _Ready()
    {
        CollisionPolygon2D = new CollisionPolygon2D();
        CollisionPolygon2D.Polygon = Points;
        AddChild(CollisionPolygon2D);
    }

    public static TSector Instance<TSector>(PackedScene scene, Vector2[] points, Spinner spinner)
        where TSector : BaseSector
    {
        var sector = scene.Instance<TSector>();
        sector.Points = points;
        sector.Spinner = spinner;

        return sector;
    }

    public override void _Draw()
    {
        DrawColoredPolygon(Points, Color);
        DrawPolyline(Points, Colors.DimGray, 3f);
    }
}
