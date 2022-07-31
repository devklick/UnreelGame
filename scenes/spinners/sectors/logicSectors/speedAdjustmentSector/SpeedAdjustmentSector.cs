using Godot;
using System;

public class SpeedAdjustmentSector : BaseLogicSector
{
    /// <summary>
    /// The amount, as a percentage, that the spinner should speed up or slow down by.
    /// A positive number will increase ths speed, and a negative will decrease it.
    /// </summary>
    [Export]
    private float speedVariancePercentage;

    protected override void ExecuteLogic()
    {
        var currentSpeed = base.Spinner.WaitBeforeMoving;
        var decimalAdjustment = speedVariancePercentage / 100;
        var newSpeed = currentSpeed + (currentSpeed * decimalAdjustment);
        base.Spinner.WaitBeforeMoving = (long)newSpeed;
        // TODO: Might need to use CallDeferred
    }
}
