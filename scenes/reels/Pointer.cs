using Godot;
using System;

public class Pointer : Node2D
{
    #region Exports
    [Export]
    private Color color;
    #endregion

    #region Publics
    public BaseSector PointsTo { get; private set; }
    #endregion

    private Vector2[] points;
    public override void _Ready()
    {
        points = new Vector2[3] { new Vector2(0, 20), new Vector2(10, 0), new Vector2(-10, 0) };

        AddChild(new CollisionPolygon2D
        {
            Polygon = points,
        });
    }

    // public override void _Process(float delta)
    // {
    //     this.Position = new Vector2(this.Position.x - 1, this.Position.y);
    //     base._Process(delta);
    // }

    public override void _Draw()
    {
        base.DrawColoredPolygon(points, color);
    }

    public void _on_Pointer_area_entered(Area2D area)
    {
        if (area is BaseSector sector)
        {
            PointsTo = sector;
        }
    }

    public void _on_HUD_ToggleSpin()
    {
        GD.Print("Pointer knows that the spinner stopped spinning");
    }
}
