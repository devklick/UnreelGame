using System;
using Godot;

public class Reel : Area2D
{
    #region Exports
    [Export]
    private int reelNo;

    #endregion

    #region Signals
    [Signal]
    public delegate void LandedOn(int reelNo, BaseSector sector);
    public static string LandedOnSignalName = nameof(LandedOn);

    [Signal]
    public delegate void NudgeUsed(int reelNo, BaseSector newPointsTo);
    public static string NudgeUsedSignalName = nameof(NudgeUsed);
    [Signal]
    public delegate void HoldUsed(int reelNo);
    public static string HoldUsedSignalName = nameof(HoldUsed);

    [Signal]
    public delegate void ClickedMouseDown(int reelNo);
    public static string ClickedMouseDownSignalName = nameof(ClickedMouseDown);

    [Signal]
    public delegate void ClickedMouseUp(int reelNo);
    public static string ClickedMouseUpSignalName = nameof(ClickedMouseUp);
    #endregion

    #region Publics
    public int ReelNo => reelNo;
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
        SubscribeToHudEvents(hud);

        var main = GetTree().Root.GetNode<Main>("Main");
        SubscribeToMainEvents(main);

        spinner.SetParentReelNo(reelNo);
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

        if (inputEvent is InputEventMouseButton mouseButton && pointer.PointsTo is BaseSector baseSector)
        {
            if (mouseButton.Pressed)
            {
                if (holdSelectionEnabled) TryUseHold();
                else if (nudgeSelectionEnabled) TryUseNudge();
            }
            else
            {
                EmitSignal(ClickedMouseUpSignalName, reelNo);
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

        EmitSignal(nameof(LandedOn), reelNo, sector);
    }

    private void TryUseHold()
    {
        if (holdsAvailable <= 0) return;
        spinner.HoldForNSpins(1);
        EmitSignal(HoldUsedSignalName, reelNo);
        EmitSignal(ClickedMouseDownSignalName, reelNo);
    }
    private void TryUseNudge()
    {
        if (nudgesAvailable <= 0) return;
        spinner.Nudge();
        EmitSignal(NudgeUsedSignalName, reelNo, (BaseSector)pointer.PointsTo);
        EmitSignal(ClickedMouseDownSignalName, reelNo);
    }

    private void SubscribeToMainEvents(Main main)
    {
        main.Connect(Main.HoldsAvailableUpdatedSignalName, this, nameof(_on_Main_HoldsAvailableUpdated));
        main.Connect(Main.NudgesAvailableUpdatedSignalName, this, nameof(_on_Main_NudgesAvailableUpdated));
    }

    private void SubscribeToHudEvents(HUD hud)
    {
        hud.Connect(HUD.ToggleSpinSignalName, this, nameof(_on_HUD_ToggleSpin));
        hud.Connect(HUD.EnableHoldSelectionSignalName, this, nameof(_on_HUD_EnableHoldSelection));
        hud.Connect(HUD.EnableNudgeSelectionSignalName, this, nameof(_on_HUD_EnableNudgeSelection));
    }
    #endregion
}
