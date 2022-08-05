using Godot;

public class Reel : Area2D
{
    #region Exports
    #endregion

    #region Signals
    [Signal]
    public delegate void ReelLandedOn(BaseSector sector);
    public static string ReelLandedOnSignalName = nameof(ReelLandedOn);

    [Signal]
    public delegate void NudgeUsed();
    public static string NudgeUsedSignalName = nameof(NudgeUsed);
    [Signal]
    public delegate void HoldUsed();
    public static string HoldUsedSignalName = nameof(HoldUsed);
    #endregion

    #region Privates
    private Spinner spinner;
    private Pointer pointer;

    /// <summary>
    /// Whether or not the reel is currently in hold mode, 
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

        var main = GetTree().Root.GetNode<Main>("Main");
        main.Connect(Main.HoldsAvailableUpdatedSignalName, this, nameof(_on_Main_HoldsAvailableUpdated));
        main.Connect(Main.NudgesAvailableUpdatedSignalName, this, nameof(_on_Main_NudgesAvailableUpdated));
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
        if (!canBePressed) return;

        if (inputEvent is InputEventMouseButton mouseButton && mouseButton.Pressed && pointer.PointsTo is BaseSector baseSector)
        {
            if (holdSelectionEnabled) UseHold();
            else if (nudgeSelectionEnabled) UseNudge();
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

    public void _on_Main_NudgesAvailableUpdated(int value) => nudgesAvailable = value;
    public void _on_Main_HoldsAvailableUpdated(int value) => holdsAvailable = value;

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

        EmitSignal(nameof(ReelLandedOn), sector);
    }

    private void UseHold()
    {
        spinner.HoldForNSpins(1);
        EmitSignal(HoldUsedSignalName);
    }
    private void UseNudge()
    {
        spinner.Nudge();
        EmitSignal(NudgeUsedSignalName);
    }
    #endregion
}
