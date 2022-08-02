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

    public override void _Ready()
    {
        // Could just do GetParent().GetChildren, but think
        // it's more definitive coming at it from the root?
        foreach (var child in GetTree().Root.GetNode("Main").GetChildren())
        {
            if (child is Reel reel)
            {
                Connect("EnableHoldSelection", reel, "_on_HUD_EnableHoldSelection");
                Connect("EnableNudgeSelection", reel, "_on_HUD_EnableNudgeSelection");
                Connect("ToggleSpin", reel, "_on_HUD_ToggleSpin");
            }
        }
    }

    public void _on_StartStopButton_pressed()
    {
        EmitSignal(nameof(ToggleSpin));
    }

    public void _on_HoldButton_pressed()
    {
        EmitSignal(nameof(EnableHoldSelection));
    }

    public void _on_NudgeButton_pressed()
    {
        EmitSignal(nameof(EnableNudgeSelection));
    }
}
