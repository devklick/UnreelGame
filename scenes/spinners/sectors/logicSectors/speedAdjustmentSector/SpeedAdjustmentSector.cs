using Godot;

public class SpeedAdjustmentSector : BaseLogicSector
{
    /// <summary>
    /// Speed adjustments are not optional - they will be executed 
    /// as soon as the reel stops spinning and will take effect on the next spin.
    /// </summary>
    public override bool IsOptional => false;

    /// <summary>
    /// The amount, as a percentage, that the spinner should speed up or slow down by.
    /// A positive number will increase ths speed, and a negative will decrease it.
    /// </summary>
    [Export]
    private float speedVariancePercentage;

    public override void ExecuteLogic()
    {
        var currentSpeed = base.Spinner.WaitBeforeMoving;
        var decimalAdjustment = speedVariancePercentage / 100;
        // Because the speed is actually the delay between sectors shifting rather than the actual speed of rotation, 
        // we dont want to add to the value, we want to subtract from it. 
        // That way, if we hit a speed increase, we reduce the delay between clicks, 
        // and if we hit a speed decrease, we increase the delay between clicks.
        var newSpeed = currentSpeed - (currentSpeed * decimalAdjustment);
        base.Spinner.WaitBeforeMoving = (long)newSpeed;
    }
}
