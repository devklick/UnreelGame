using Godot;
using System;

public class ClickableArea2D : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    public void _on_ClickableArea2D_input_event(Viewport viewport, InputEvent inputEvent, Shape shape)
    {
        if (inputEvent is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            GD.Print("Clicked");
        }
    }
}
