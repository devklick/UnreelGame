using Godot;
using System;

public class HUD : CanvasLayer
{
    [Signal]
    delegate void ToggleSpin();

    public override void _Ready()
    {
    }

    public void _on_StartStopButton_pressed()
    {
        EmitSignal(nameof(ToggleSpin));
    }
}
