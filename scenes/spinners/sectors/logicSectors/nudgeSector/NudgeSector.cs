using Godot;
using System;

public class NudgeSector : BaseLogicSector
{
    private int _nudges;

    public static NudgeSector Instance(PackedScene scene, Vector2[] points, int nudges, Spinner spinner)
    {
        var sector = BaseSector.Instance<NudgeSector>(scene, points, spinner);
        sector._nudges = nudges;
        return sector;
    }

    /// <summary>
    /// Nudges the spinner by one sector in the direction the spinner was last spinning in.
    /// </summary>
    protected override void ExecuteLogic()
    {
        Spinner.Nudge();
    }
}
