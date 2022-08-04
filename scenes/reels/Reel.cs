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

    private int nudgesAvailable;
    private int holdsAvailable;
    private bool spinning;

    #endregion

    #region Override methods
    public override void _Ready()
    {
        spinner = GetNode<Spinner>("Spinner");
        pointer = GetNode<Pointer>("Pointer");

        AddChild(new CollisionShape2D
        {
            Shape = new CircleShape2D
            {
                Radius = spinner.Radius
            }
        });

        var hud = GetParent().GetNode<HUD>("HUD");
        hud.Connect(HUD.ToggleSpinSignalName, this, nameof(_on_HUD_ToggleSpin));
        hud.Connect(HUD.EnableHoldSelectionSignalName, this, nameof(_on_HUD_EnableHoldSelection));
        hud.Connect(HUD.EnableNudgeSelectionSignalName, this, nameof(_on_HUD_EnableNudgeSelection));
        Connect("ReelLandedOn", hud, "_on_Reel_LandedOn");
    }

    /// <summary>
    /// Handles the clicking of a reel when it's possible to do do. 
    /// For example, when the player has nudges or holds available.
    /// If the game is not in a state that it's possible to click the reel, 
    /// nothing will happen.
    /// /// </summary>
    /// <param name="viewport"></param>
    /// <param name="inputEvent"></param>
    /// <param name="shape"></param>
    public void _on_Reel_input_event(Viewport viewport, InputEvent inputEvent, Shape shape)
    {
        // Otherwise, if the user clicks the reel that's landed on a logic sector
        // that's currently enabled for selection, execute the sectors logic.
        if (inputEvent is InputEventMouseButton mouseButton && mouseButton.Pressed && pointer.PointsTo is BaseSector baseSector)
        {
            // If the user has no holds or nudges available, clicking the reel does nothing - return
            if (holdsAvailable == 0 && nudgesAvailable == 0) return;

            if (baseSector is ValueSector)
            {
                // TODO
            }
            else
            {
                if (holdsAvailable > 0 && baseSector is HoldSector holdSector)
                {
                    holdSector.ExecuteLogic();
                }
                else if (nudgesAvailable > 0 && baseSector is NudgeSector nudgeSector)
                {
                    nudgeSector.ExecuteLogic();
                }
            }
        }
    }
    #endregion


    #region Public methods
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

    /// <summary>
    /// Handles the ToggleSpin event triggered by the HUD
    /// </summary>
    public void _on_HUD_ToggleSpin()
    {
        if (!spinning)
        {
            spinner.TrySpin();
            holdsAvailable = 0;
            nudgesAvailable = 0;
        }
        else StopSpinning();

        spinning = !spinning;
    }
    #endregion

    #region Private methods
    private void StopSpinning()
    {
        spinner.StopSpinning();
        var sector = pointer.PointsTo;
        GD.Print(sector.GetType().FullName);

        // If it's a mandatory logic sector, we just want to execute the logic. 
        // Otherwise, we want to broadcast the sector the reel has landed on, sp 
        // other components can be notified of it
        if (sector is ValueSector valueSector)
        {
            // TODO: Broadcast updating the value - the Main needs to know about this
            // since the aim of the game is to end up on all reels with the same value.
        }
        else if (sector is BaseLogicSector logicSector && !logicSector.IsOptional) logicSector.ExecuteLogic();
        else if (sector is NudgeSector nudgeSector) nudgesAvailable += nudgeSector.Nudges;
        else if (sector is HoldSector holdSector) holdsAvailable += holdSector.Holds;

        EmitSignal(nameof(ReelLandedOn), sector);
    }
    #endregion
}
