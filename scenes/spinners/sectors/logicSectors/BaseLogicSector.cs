/// <summary>
/// A class to be implemented by any Sectors which are used to execute logic
/// </summary>
public abstract class BaseLogicSector : BaseSector
{
    public virtual bool IsOptional { get => true; }
    public abstract void ExecuteLogic();
}
