using Godot;
using System;

public class Pointer : Node2D
{
    [Export]
    private Color color;
    private Vector2[] points;
    public override void _Ready()
    {
        points = new Vector2[3] { new Vector2(0, 20), new Vector2(10, 0), new Vector2(-10, 0) };
    }

    public override void _Draw()
    {
        base.DrawColoredPolygon(points, color);
    }
}
