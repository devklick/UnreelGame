using Godot;
using System;

public class HUD : CanvasLayer
{
    [Signal]
    delegate void ToggleSpin();
    public static string ToggleSpinSignalName = nameof(ToggleSpin);

    [Signal]
    delegate void EnableHoldSelection();
    public static string EnableHoldSelectionSignalName = nameof(EnableHoldSelection);

    [Signal]
    delegate void EnableNudgeSelection();
    public static string EnableNudgeSelectionSignalName = nameof(EnableNudgeSelection);

    private Button holdButton;
    private Button nudgeButton;
    private Button spinButton;

    private int nudgesAvailable;
    private int holdsAvailable;
    private bool spinning;

    public override void _Ready()
    {
        holdButton = GetNode<Button>("HoldButton");
        nudgeButton = GetNode<Button>("NudgeButton");
        spinButton = GetNode<Button>("StartStopButton");

        holdButton.Text = $"Hold ({holdsAvailable})";
        nudgeButton.Text = $"Hold ({nudgesAvailable})";
        spinButton.Text = "Spin";

        holdButton.Disabled = true;
        nudgeButton.Disabled = true;

        var main = GetTree().Root.GetNode<Main>("Main");
        main.Connect(Main.HoldsAvailableUpdatedSignalName, this, nameof(_on_Main_HoldsAvailableUpdated));
        main.Connect(Main.NudgesAvailableUpdatedSignalName, this, nameof(_on_Main_NudgesAvailableUpdated));
    }

    public void _on_StartStopButton_pressed()
    {
        EmitSignal(nameof(ToggleSpin));
        spinning = !spinning;
        spinButton.Text = spinning ? "Stop" : "Spin";

        if (spinning) ResetButton();

        UpdateButtonLabels();
    }

    public void _on_HoldButton_pressed()
    {
        EmitSignal(nameof(EnableHoldSelection));
    }

    public void _on_NudgeButton_pressed()
    {
        EmitSignal(nameof(EnableNudgeSelection));
    }

    private void ResetButton()
    {
        nudgesAvailable = holdsAvailable = 0;
        holdButton.Disabled = nudgeButton.Disabled = true;
    }

    private void UpdateButtonLabels()
    {
        holdButton.Text = $"Hold ({holdsAvailable})";
        nudgeButton.Text = $"Nudge ({nudgesAvailable})";
    }

    public void _on_Main_NudgesAvailableUpdated(int value)
    {
        nudgesAvailable = value;
        nudgeButton.Disabled = nudgesAvailable == 0;
        UpdateButtonLabels();
    }
    public void _on_Main_HoldsAvailableUpdated(int value)
    {
        holdsAvailable = value;
        holdButton.Disabled = holdsAvailable == 0;
        UpdateButtonLabels();
    }
}
