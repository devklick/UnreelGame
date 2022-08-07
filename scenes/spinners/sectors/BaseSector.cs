using System;
using Godot;

public class BaseSector : Area2D
{
    [Export]
    protected Color Color;

    protected Vector2[] Points;
    protected Spinner Spinner;
    protected CollisionPolygon2D CollisionPolygon2D;

    private float outlineWeight = 1f;

    private int parentReelNo;

    public override void _Ready()
    {
        CollisionPolygon2D = new CollisionPolygon2D();
        CollisionPolygon2D.Polygon = Points;
        AddChild(CollisionPolygon2D);

        foreach (var node in GetTree().GetNodesInGroup("Reel"))
        {
            if (node is Reel reel)
            {
                SubscribeToReelEvents(reel);
            }
        }
    }

    private void SubscribeToReelEvents(Reel reel)
    {
        reel.Connect(Reel.ClickedMouseDownSignalName, this, nameof(_on_Reel_ClickedMouseDown));
        reel.Connect(Reel.ClickedMouseUpSignalName, this, nameof(_on_Reel_ClickedMouseUp));
    }

    public static TSector Instance<TSector>(PackedScene scene, Vector2[] points, Spinner spinner, Label label)
        where TSector : BaseSector
    {
        var sector = scene.Instance<TSector>();
        sector.Points = points;
        sector.Spinner = spinner;
        sector.AddChild(label);
        return sector;
    }

    public void _on_Reel_ClickedMouseDown(int reelNo)
    {
        if (reelNo == parentReelNo)
        {
            outlineWeight = 3f;
            Update();
        }
    }

    public void _on_Reel_ClickedMouseUp(int reelNo)
    {
        if (reelNo == parentReelNo)
        {
            outlineWeight = 1f;
            Update();
        }
    }

    public override void _Draw()
    {
        DrawColoredPolygon(Points, Color, null, null, null, true);
        DrawPolyline(Points, Colors.DimGray, outlineWeight, true);
    }

    public void SetParentReelNo(int reelNo) => parentReelNo = reelNo;
}
