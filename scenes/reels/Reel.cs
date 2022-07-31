using Godot;

public class Reel : Node2D
{
    private Spinner spinner;

    public override void _Ready()
    {
        spinner = GetNode<Spinner>("Spinner");
    }

    public void TrySpin() => spinner.TrySpin();

    public void StopSpinning() => spinner.StopSpinning();
}
