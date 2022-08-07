using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public enum GameState
{
    Initializing,
    WaitingForInput,
    Spinning,
    HoldSelection,
    ReelSelection,
    Won
}

public class Main : Node
{
    #region Signals
    /// <summary>
    /// Signal to broadcast the new total nudges available for the player to use.
    /// </summary>
    /// <param name="nudgesAvailable">The new value</param>
    [Signal]
    public delegate void NudgesAvailableUpdated(int nudgesAvailable);
    /// <summary>
    /// A strongly typed accessor containing the name of the `NudgesAvailableUpdated` delegate.
    /// </summary>
    /// <returns>The name of the delegate</returns>
    public static readonly string NudgesAvailableUpdatedSignalName = nameof(NudgesAvailableUpdated);

    /// <summary>
    /// Signal to broadcast the new total holds available for the player to use.
    /// </summary>
    /// <param name="nudgesAvailable">The new value</param>
    [Signal]
    public delegate void HoldsAvailableUpdated(int holdsAvailable);
    /// <summary>
    /// A strongly typed accessor containing the name of the `HoldsAvailableUpdated` delegate.
    /// </summary>
    /// <returns>The name of the delegate</returns>
    public static readonly string HoldsAvailableUpdatedSignalName = nameof(HoldsAvailableUpdated);

    /// <summary>
    /// Signal to broadcast any changes to the GameState.
    /// </summary>
    /// <param name="newGameState">The new GameState</param>
    [Signal]
    public delegate void GameStateUpdated(GameState newGameState);
    /// <summary>
    /// A strongly typed accessor containing the name of the `GameStateUpdated` delegate.
    /// </summary>
    /// <returns>The name of the delegate</returns>
    public static string GameStateUpdatedSignalName = nameof(GameStateUpdated);
    #endregion

    #region Privates
    /// <summary>
    /// The number of nudges that are currently available to the player.
    /// </summary>
    private int nudgesAvailable;
    /// <summary>
    /// The number of holds that are currently available to the player
    /// </summary>
    private int holdsAvailable;
    /// <summary>
    /// The total number of reels on the scene
    /// </summary>
    private int reelCount;
    /// <summary>
    /// The current value that each is currently sat on. This is updated when the reels stop, so if the reels are spinning
    /// it contains the value of the last stationary position of each reel. 
    /// 
    /// The key is the reel number, and the value is the value of the reel. 
    /// Note that if the reel was landed on a non-value reel (for example, a logic reel), 
    /// the value in the dictionary will be null
    /// </summary>
    /// <typeparam name="int">The reel number</typeparam>
    /// <typeparam name="int?">The value from the `ValueSector`, if the reel landed on a value</typeparam>
    private readonly Dictionary<int, int?> reelValues = new Dictionary<int, int?>();
    /// <summary>
    /// The current state of the game
    /// </summary>
    private GameState gameState = GameState.Initializing;
    #endregion

    #region Overrides
    /// <summary>
    /// Basic construction for the Main scene.
    /// </summary>
    public override void _Ready()
    {
        foreach (var child in GetChildren())
        {
            if (child is HUD hud)
            {
                SubscribeToHudEvents(hud);
            }
            if (child is Reel reel)
            {
                reelCount++;
                SubscribeToReelEvents(reel);
            }
        }

        UpdateGameState(GameState.WaitingForInput);
    }
    #endregion

    #region Event Handlers
    public void _on_HUD_SpinPressed()
    {
        holdsAvailable = 0;
        nudgesAvailable = 0;
    }

    /// <summary>
    /// Handles the `LandedOn` event fired in the `Reel` class.
    /// Takes care of capturing the reel value or executing the reel logic etc.
    /// </summary>
    /// <param name="reelNo">The number of the reel</param>
    /// <param name="sector">The sector the reel has landed on</param>
    public void _on_Reel_LandedOn(int reelNo, BaseSector sector)
    {
        // If it's a mandatory logic sector, we just want to execute the logic. 
        // Otherwise, we want to broadcast the sector the reel has landed on, sp 
        // other components can be notified of it
        if (sector is ValueSector valueSector) HandleLandedOnValueSector(reelNo, valueSector);
        else if (sector is BaseLogicSector logicSector && !logicSector.IsOptional) HandleLandedOnMandatoryLogicSector(logicSector);
        else if (sector is NudgeSector nudgeSector) HandleLandedOnNudgeSector(nudgeSector);
        else if (sector is HoldSector holdSector) HandleLandedOnHoldSector(holdSector);
    }

    /// <summary>
    /// Handles the `NudgesUsed` event fired in the `Reel` class.
    /// Takes care of updating the reel value and the new number of nudges available.
    /// </summary>
    /// <param name="reelNo">The number of the reel</param>
    /// <param name="sector">The sector that the reel has landed on</param>
    public void _on_Reel_NudgeUsed(int reelNo, BaseSector sector)
    {
        UpdateNudgesAvailable(nudgesAvailable - 1);
        var newValue = (sector as ValueSector)?.Value;
        reelValues[reelNo] = newValue;
    }
    /// <summary>
    /// Handles the `HoldsUsed` event fired in the `Reel` class.
    /// Takes care of updating the new number of holds available.
    /// </summary>
    /// <param name="reelNo"></param>
    public void _on_Reel_HoldUsed(int reelNo) => UpdateHoldsAvailable(holdsAvailable - 1);
    #endregion

    #region  Private methods

    /// <summary>
    /// Captures the value from the `valueSector`
    /// </summary>
    /// <param name="reelNo">The reel number used to uniquely identify the reel</param>
    /// <param name="valueSector">The `ValueSector` that the reel landed on</param>
    private void HandleLandedOnValueSector(int reelNo, ValueSector valueSector)
    {
        reelValues[reelNo] = valueSector.Value;
    }

    /// <summary>
    /// Executes the sectors logic
    /// </summary>
    /// <param name="logicSector">The sector containing the logic to be executed</param>
    private void HandleLandedOnMandatoryLogicSector(BaseLogicSector logicSector)
    {
        logicSector.ExecuteLogic();
    }

    /// <summary>
    /// Updates the new number of nudges available
    /// </summary>
    /// <param name="nudgeSector">The sector that the reel landed on</param>
    private void HandleLandedOnNudgeSector(NudgeSector nudgeSector) => UpdateNudgesAvailable(nudgesAvailable + nudgeSector.Nudges);

    /// <summary>
    /// Updates the new number of holds available
    /// </summary>
    /// <param name="holdSector">The sector that the reel landed on</param>
    private void HandleLandedOnHoldSector(HoldSector holdSector) => UpdateHoldsAvailable(holdsAvailable + holdSector.Holds);

    /// <summary>
    /// Updates the number of nudges available and broadcasts it.
    /// </summary>
    /// <param name="newValue">The new number of nudges available</param>
    private void UpdateNudgesAvailable(int newValue)
    {
        nudgesAvailable = newValue;
        EmitSignal(nameof(NudgesAvailableUpdated), nudgesAvailable);
    }

    /// <summary>
    /// Updates the number of holds available and broadcasts it.
    /// </summary>
    /// <param name="newValue">The new number of holds available</param>
    private void UpdateHoldsAvailable(int newValue)
    {
        holdsAvailable = newValue;
        EmitSignal(nameof(HoldsAvailableUpdated), holdsAvailable);
    }

    /// <summary>
    /// Subscribes to all the `Reel` event that the `Main` needs to know about
    /// </summary>
    /// <param name="reel"></param>
    private void SubscribeToReelEvents(Reel reel)
    {
        reel.Connect(Reel.LandedOnSignalName, this, nameof(_on_Reel_LandedOn));
        reel.Connect(Reel.NudgeUsedSignalName, this, nameof(_on_Reel_NudgeUsed));
        reel.Connect(Reel.HoldUsedSignalName, this, nameof(_on_Reel_HoldUsed));
    }

    /// <summary>
    /// Checks if all the reels have landed on a value, and if so, all values are the same.
    /// If this is the case, the user has won, so the game state is updated to reflect this.
    /// </summary>
    private void CheckForWin()
    {
        if (reelValues.Count == reelCount)
        {
            if (reelValues.Values.Distinct().Count() == 1)
            {
                GD.Print($"Winner with 4 of a kind - {reelValues.First().Value * reelCount} points");
                UpdateGameState(GameState.Won);
            }
            else if (IsSequential(reelValues.Values.OrderBy(v => v).ToArray()))
            {
                GD.Print($"Winner with a straight - {reelValues.Values.Sum()} points");
                UpdateGameState(GameState.Won);
            }

            // TODO: Proper win alert
        }
    }
    private bool IsSequential(int?[] array)
    {
        return array.Zip(array.Skip(1), (a, b) => (a + 1) == b).All(x => x);
    }

    /// <summary>
    /// Updates the game state and broadcasts it.
    /// </summary>
    /// <param name="newGameState">The new game state</param>
    private void UpdateGameState(GameState newGameState)
    {
        gameState = newGameState;
        EmitSignal(GameStateUpdatedSignalName, gameState);
    }

    private void SubscribeToHudEvents(HUD hud)
    {
        hud.Connect(HUD.SpinPressedSignalName, this, nameof(_on_HUD_SpinPressed));
    }
    #endregion
}
