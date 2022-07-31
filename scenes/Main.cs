using Godot;
using System.Collections.Generic;

public class Main : Node
{
    private Reel[] reels;
    private bool spinning;

    public override void _Ready()
    {
        reels = GetReelNodes();
    }
    public void _on_HUD_ToggleSpin()
    {
        spinning = !spinning;
        foreach (var spinner in reels)
        {
            if (spinning) spinner.TrySpin();
            else spinner.StopSpinning();
        }
    }

    private Reel[] GetReelNodes()
    {
        var reels = new List<Reel>();
        foreach (var child in GetChildren())
            if (child is Reel)
                reels.Add(child as Reel);

        return reels.ToArray();
    }
}
