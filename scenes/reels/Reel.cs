using Godot;

public class Reel : Node2D
{
    private Spinner spinner;
    private Pointer pointer;

    /// <summary>
    /// Whether or not the reel is currently in selection mode, 
    /// where the user can click it so that it wont be rotated on the next spin.
    /// </summary>
    private bool holdSelectionEnabled;

    /// <summary>
    /// Whether or not the reel is currently in nudge mode, 
    /// where the user can click it to move nudge it one click.
    /// </summary>
    private bool nudgeSelectionEnabled;

    /// <summary>
    /// Whether or not the reel can be pressed. 
    /// This is typically allowed when the reel is either in "hold selection" (where the user can select which reels to hold)
    /// or in "nudge selection" mode, where the user can select which reels to nudge.
    /// </summary>
    private bool canBePressed => holdSelectionEnabled || nudgeSelectionEnabled;

    public override void _Input(InputEvent inputEvent)
    {
        if (!canBePressed) return;

        if (inputEvent is InputEventMouseButton)
        {
            // TODO: The number of spins comes from the sector that the spinner landed on. 
            // Need to figure out how to get that here
            if (holdSelectionEnabled) spinner.HoldForNSpins(1);
            else if (nudgeSelectionEnabled) spinner.Nudge();
        }
    }

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
        GD.Print(sector.GetType().FullName);

        if (sector is BaseLogicSector logicSector && !logicSector.IsOptional)
        {
            logicSector.ExecuteLogic();
        }
    }

    public void _on_HUD_EnableHoldSelection()
    {
        nudgeSelectionEnabled = false;
        holdSelectionEnabled = true;
    }

    public void _on_HUD_EnableNudgeSelection()
    {
        holdSelectionEnabled = false;
        nudgeSelectionEnabled = true;
    }

    public void _on_HUD_ToggleSpin()
    {

    }
}
