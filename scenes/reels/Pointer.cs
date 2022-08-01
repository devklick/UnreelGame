using Godot;
using System;

public class Pointer : Node2D
{
    [Export]
    private Color color;
    private Vector2[] points;
    private RayCast2D rayCast2D;
    public override void _Ready()
    {
        rayCast2D = GetNode<RayCast2D>("RayCast2D");
        points = new Vector2[3] { new Vector2(0, 20), new Vector2(10, 0), new Vector2(-10, 0) };
    }

    public override void _Draw()
    {
        base.DrawColoredPolygon(points, color);
    }

    public BaseSector PointsTo()
    {
        var collider = rayCast2D.GetCollider();

        if (collider is BaseSector)
        {
            return collider as BaseSector;
        }

        return null;
    }
}
