using Godot;
using System;

public class NudgeSector : BaseLogicSector
{
    public int Nudges { get; private set; }

    public static NudgeSector Instance(PackedScene scene, Vector2[] points, int nudges, Spinner spinner, Label label)
    {
        var sector = BaseSector.Instance<NudgeSector>(scene, points, spinner, label);
        sector.Nudges = nudges;
        return sector;
    }

    /// <summary>
    /// Nudges the spinner by one sector in the direction the spinner was last spinning in.
    /// </summary>
    public override void ExecuteLogic()
    {
        Spinner.Nudge();
    }
}
