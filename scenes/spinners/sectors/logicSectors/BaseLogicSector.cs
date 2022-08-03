/// <summary>
/// A class to be implemented by any Sectors which are used to execute logic
/// </summary>
public abstract class BaseLogicSector : BaseSector
{
    /// <summary>
    /// Whether or not the logic is optional or should always be executed. 
    /// Optional logic requires user to trigger some input event (such as a click), 
    /// to trigger the logic. Non-optional logic will be executed as soon as the reel
    /// stops spinning and lands on a logic sector.
    /// </summary>
    /// <value></value>
    public virtual bool IsOptional { get => true; }

    /// <summary>
    /// Executes the logic assigned to this sector.
    /// </summary>
    public abstract void ExecuteLogic();
}
