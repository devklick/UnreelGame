using Godot;
using System;

public enum Direction
{
    Clockwise,
    AntiClockwise
}

public class Spinner : Node2D
{
    #region Exports
    /// <summary>
    /// The radius of the spinner in pixels
    /// </summary>
    [Export]
    public int Radius;

    [Export]
    private Direction direction;

    /// <summary>
    /// The number of sectors that the spinner should consist of
    /// </summary>
    [Export]
    private int numberOfSectors;

    /// <summary>
    /// The base color of the spinner
    /// </summary>
    [Export]
    private Color color = Colors.AliceBlue;

    [Export]
    private float minWaitBeforeRotate;

    [Export]
    private float maxWaitBeforeMove;
    #endregion

    #region Publics
    public int SectorSize => 360 / numberOfSectors;

    public long WaitBeforeMoving;
    #endregion

    #region Privates
    /// <summary>
    /// The sectors that make up this spinner
    /// </summary>
    private SectorFactory _sectorFactory;
    private BaseSector[] _sectors;
    private int _heldForNSpins;
    private bool _held => _heldForNSpins > 0;
    private bool _spinning;
    private RandomNumberGenerator _random = new RandomNumberGenerator();
    private long lastMovedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    #endregion

    #region Overrides
    public override void _Ready()
    {
        _random.Randomize();
        _sectorFactory = GetNode<SectorFactory>("SectorFactory");
        _sectors = _sectorFactory.CreateSectors(numberOfSectors, SectorSize, Radius, this);

        WaitBeforeMoving = (long)_random.RandfRange(minWaitBeforeRotate * 1000, maxWaitBeforeMove * 1000);

        foreach (var sector in _sectors) AddChild(sector);
    }
    public override void _Process(float delta)
    {
        MaybeMove();
    }
    #endregion

    #region Public Methods
    public void Nudge()
    {
        Rotation += direction switch
        {
            Direction.Clockwise => SectorSize,
            Direction.AntiClockwise => -SectorSize,
            _ => throw new NotImplementedException($"Can't nudge spinner. Direction {direction} not currently supported.")
        };
    }

    public void TrySpin()
    {
        if (!_held) _spinning = true;
        else _heldForNSpins--;
    }

    public void StopSpinning()
    {
        _spinning = false;
    }

    public void HoldForNSpins(int numberOfSpins) => _heldForNSpins = numberOfSpins;
    #endregion

    #region Private Methods
    private void MaybeMove()
    {
        var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var nextMoveMin = lastMovedAt + WaitBeforeMoving;
        if (_spinning && nextMoveMin <= now)
        {
            Rotation += Mathf.Deg2Rad(SectorSize);
            lastMovedAt = now;
        }
    }
    #endregion
}
