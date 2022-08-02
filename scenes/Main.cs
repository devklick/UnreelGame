using Godot;
using System.Collections.Generic;
using System.Linq;

enum GameState
{
    WaitingForInput,
    Spinning
}

public class Main : Node
{
    private Reel[] reels;
    private GameState gameState;
    private bool Spinning => gameState == GameState.Spinning;
    private int holdsAvailable;
    private int nudgesAvailable;

    public override void _Ready()
    {
        // var hud = GetNode<HUD>("HUD");
        reels = GetReelNodes();
        // reels.ToList().ForEach(reel =>
        // {
        //     Connect("EnableHoldSelection", reel, nameof(reel._on_HUD_EnableHoldSelection));
        //     Connect("EnableNudgeSelection", reel, nameof(reel._on_HUD_EnableNudgeSelection));
        // });
    }

    public void _on_HUD_ToggleSpin()
    {
        if (gameState == GameState.Spinning) gameState = GameState.WaitingForInput;
        else if (gameState == GameState.WaitingForInput) gameState = GameState.Spinning;

        foreach (var spinner in reels)
        {
            if (Spinning) spinner.TrySpin();
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
