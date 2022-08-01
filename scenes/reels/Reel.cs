using Godot;

public class Reel : Node2D
{
    private Spinner spinner;
    private Pointer pointer;

    public override void _Ready()
    {
        spinner = GetNode<Spinner>("Spinner");
        pointer = GetNode<Pointer>("Pointer");
    }

    public void TrySpin() => spinner.TrySpin();

    public void StopSpinning()
    {
        spinner.StopSpinning();
        var sector = pointer.PointsTo();
        GD.Print(sector?.GetType()?.FullName);
    }
}
