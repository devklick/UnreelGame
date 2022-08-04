using Godot;
using System;

/// <summary>
/// This sector represents logic that causes the spinner to be held for n number of spins.
/// </summary>
public class HoldSector : BaseLogicSector
{
    public int Holds { get; private set; }

    public static HoldSector Instance(PackedScene scene, Vector2[] points, int holds, Spinner spinner)
    {
        var sector = BaseSector.Instance<HoldSector>(scene, points, spinner);
        sector.Holds = holds;
        return sector;
    }

    public override void ExecuteLogic()
    {
        base.Spinner.HoldForNSpins(Holds);
    }
}
