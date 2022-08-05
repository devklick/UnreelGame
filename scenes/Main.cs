using System.Collections.Generic;
using System.Linq;
using Godot;

public class Main : Node
{
    [Signal]
    public delegate void NudgesAvailableUpdated(int nudgesAvailable);
    public static string NudgesAvailableUpdatedSignalName = nameof(NudgesAvailableUpdated);
    [Signal]
    public delegate void HoldsAvailableUpdated(int holdsAvailable);
    public static string HoldsAvailableUpdatedSignalName = nameof(HoldsAvailableUpdated);

    private int nudgesAvailable;
    public int holdsAvailable { get; private set; }

    private readonly List<int> values = new List<int>();

    public override void _Ready()
    {
        foreach (var child in GetChildren())
        {
            if (child is Reel reel) SubscribeToReelEvents(reel);
        }
    }

    public void _on_Reel_LandedOn(BaseSector sector)
    {
        // If it's a mandatory logic sector, we just want to execute the logic. 
        // Otherwise, we want to broadcast the sector the reel has landed on, sp 
        // other components can be notified of it
        if (sector is ValueSector valueSector) HandleValueSector(valueSector);
        else if (sector is BaseLogicSector logicSector && !logicSector.IsOptional) HandleMandatoryLogicSector(logicSector);
        else if (sector is NudgeSector nudgeSector) HandleNudgeSector(nudgeSector);
        else if (sector is HoldSector holdSector) HandleHoldSector(holdSector);
    }

    public void _on_Reel_NudgeUsed() => UpdateNudgesAvailable(nudgesAvailable - 1);
    public void _on_Reel_HoldUsed() => UpdateHoldsAvailable(holdsAvailable - 1);

    private void HandleValueSector(ValueSector valueSector)
    {
        values.Add(valueSector.Value);
    }
    private void HandleMandatoryLogicSector(BaseLogicSector logicSector)
    {
        logicSector.ExecuteLogic();
    }

    private void HandleNudgeSector(NudgeSector nudgeSector) => UpdateNudgesAvailable(nudgesAvailable + nudgeSector.Nudges);
    private void HandleHoldSector(HoldSector holdSector) => UpdateHoldsAvailable(holdsAvailable + holdSector.Holds);

    private void UpdateNudgesAvailable(int newValue)
    {
        nudgesAvailable = newValue;
        EmitSignal(nameof(NudgesAvailableUpdated), nudgesAvailable);
    }
    private void UpdateHoldsAvailable(int newValue)
    {
        holdsAvailable = newValue;
        EmitSignal(nameof(HoldsAvailableUpdated), holdsAvailable);
    }

    private void SubscribeToReelEvents(Reel reel)
    {
        reel.Connect(Reel.ReelLandedOnSignalName, this, nameof(_on_Reel_LandedOn));
        reel.Connect(Reel.NudgeUsedSignalName, this, nameof(_on_Reel_NudgeUsed));
        reel.Connect(Reel.HoldUsedSignalName, this, nameof(_on_Reel_HoldUsed));
    }
}
