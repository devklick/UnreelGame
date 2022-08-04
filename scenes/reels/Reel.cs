using Godot;

public class Reel : Area2D
{
    #region Exports
    #endregion

    #region Signals
    [Signal]
    delegate void ReelLandedOn(BaseSector sector);
    #endregion

    #region Privates
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
    #endregion

    #region Override methods
    public override void _Ready()
    {
        spinner = GetNode<Spinner>("Spinner");
        pointer = GetNode<Pointer>("Pointer");


        var hud = GetParent().GetNode<HUD>("HUD");
        hud.Connect(HUD.ToggleSpinSignalName, this, nameof(_on_HUD_ToggleSpin));
        hud.Connect(HUD.EnableHoldSelectionSignalName, this, nameof(_on_HUD_EnableHoldSelection));
        hud.Connect(HUD.EnableNudgeSelectionSignalName, this, nameof(_on_HUD_EnableNudgeSelection));
        Connect("ReelLandedOn", hud, "_on_Reel_LandedOn");

        AddChild(new CollisionShape2D
        {
            Shape = new CircleShape2D
            {
                Radius = spinner.Radius
            }
        });
    }

    public void _on_Reel_input_event(Viewport viewport, InputEvent inputEvent, Shape shape)
    {
        if (inputEvent is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            GD.Print("Reel clicked");
        }
    }
    #endregion


    #region Public methods
    public void TrySpin() => spinner.TrySpin();

    public void StopSpinning()
    {
        spinner.StopSpinning();
        var sector = pointer.PointsTo;
        GD.Print(sector.GetType().FullName);

        // If it's a mandatory logic sector, we just want to execute the logic. 
        // Otherwise, we want to broadcast the sector the reel has landed on, sp 
        // other components can be notified of it
        if (sector is BaseLogicSector logicSector && !logicSector.IsOptional)
        {
            logicSector.ExecuteLogic();
        }
        else
        {
            EmitSignal(nameof(ReelLandedOn), sector);
        }
    }
    #endregion

    #region EventHandler methods
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
    #endregion
}
