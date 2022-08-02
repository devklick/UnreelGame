using Godot;
using System;

/// <summary>
/// This sector represents logic that causes the spinner to be held for n number of spins.
/// </summary>
public class HoldSector : BaseLogicSector
{
    private int _holdFor;

    public static HoldSector Instance(PackedScene scene, Vector2[] points, int holdFor, Spinner spinner)
    {
        var sector = BaseSector.Instance<HoldSector>(scene, points, spinner);
        sector._holdFor = holdFor;
        return sector;
    }

    public override void ExecuteLogic()
    {
        base.Spinner.HoldForNSpins(_holdFor);
    }
}
