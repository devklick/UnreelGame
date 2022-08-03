using Godot;
using System;

public class HUD : CanvasLayer
{
    [Signal]
    delegate void ToggleSpin();
    [Signal]
    delegate void EnableHoldSelection();
    [Signal]
    delegate void EnableNudgeSelection();

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

        // Could just do GetParent().GetChildren, but think
        // it's more definitive coming at it from the root?
        foreach (var child in GetTree().Root.GetNode("Main").GetChildren())
        {
            if (child is Reel reel)
            {
                Connect("EnableHoldSelection", reel, "_on_HUD_EnableHoldSelection");
                Connect("EnableNudgeSelection", reel, "_on_HUD_EnableNudgeSelection");
                Connect("ToggleSpin", reel, "_on_HUD_ToggleSpin");

                reel.Connect("ReelLandedOn", this, nameof(_on_Reel_LandedOn));
            }
        }


    }

    public void _on_StartStopButton_pressed()
    {
        EmitSignal(nameof(ToggleSpin));
        spinning = !spinning;
        spinButton.Text = spinning ? "Stop" : "Spin";
    }

    public void _on_HoldButton_pressed()
    {
        EmitSignal(nameof(EnableHoldSelection));
    }

    public void _on_NudgeButton_pressed()
    {
        EmitSignal(nameof(EnableNudgeSelection));
    }

    public void _on_Reel_LandedOn(BaseSector sector)
    {
        switch (sector)
        {
            case NudgeSector ns:
                nudgesAvailable += ns.Nudges;
                break;
            case HoldSector hs:
                holdsAvailable += hs.HoldFor;
                break;
        }

        holdButton.Text = $"Hold ({holdsAvailable})";
        nudgeButton.Text = $"Nudge ({nudgesAvailable})";

        holdButton.Disabled = holdsAvailable == 0;
        nudgeButton.Disabled = nudgesAvailable == 0;
    }
}
