using Godot;
using System;
using System.Linq;

enum SectorType
{
    Value,
    Nudge,
    Hold,
    IncreaseSpeed,
    DecreaseSpeed
}

public class SectorFactory : Node2D
{
    /// <summary>
    /// The scene that contains the individual sector
    /// </summary>
    [Export]
    private PackedScene valueSectorScene;
    [Export]
    private PackedScene nudgeSectorScene;
    [Export]
    private PackedScene holdSectorScene;
    [Export]
    private PackedScene increaseSpeedSectorScene;
    [Export]
    private PackedScene decreaseSpeedSectorScene;

    private readonly RandomNumberGenerator random = new RandomNumberGenerator();

    public override void _Ready()
    {
        random.Randomize();
    }

    public BaseSector[] CreateSectors(int numberOfSectors, int sectorSize, float radius, Spinner parentSpinner)
    {
        var sectors = new BaseSector[numberOfSectors];
        var maxSectorSize = 360 - sectorSize;

        for (int sectorNo = 0; sectorNo < numberOfSectors; sectorNo++)
        {
            var sectorStart = sectorSize * sectorNo;
            var sectorEnd = sectorStart + sectorSize;
            sectors[sectorNo] = CreateSector(radius, sectorStart, sectorEnd, parentSpinner);
        }

        return sectors;
    }

    private BaseSector CreateSector(float radius, float angleFrom, float angleTo, Spinner parentSpinner)
    {
        int nbPoints = 32;
        var pointsArc = new Vector2[nbPoints + 2];
        pointsArc[0] = Vector2.Zero;

        for (int i = 0; i <= nbPoints; ++i)
        {
            float anglePoint = Mathf.Deg2Rad(angleFrom + i * (angleTo - angleFrom) / nbPoints - 90);
            pointsArc[i + 1] = new Vector2(Mathf.Cos(anglePoint), Mathf.Sin(anglePoint)) * radius;
        }

        var sectorType = SectorTypeToCreate();
        return sectorType switch
        {
            SectorType.Value => CreateValueSector(pointsArc, parentSpinner),
            SectorType.Nudge => CreateNudgeSector(pointsArc, parentSpinner),
            SectorType.Hold => CreateHoldSector(pointsArc, parentSpinner),
            SectorType.IncreaseSpeed => CreateIncreaseSpeedSector(pointsArc, parentSpinner),
            SectorType.DecreaseSpeed => CreateDecreaseSpeedSector(pointsArc, parentSpinner),
            _ => throw new NotImplementedException($"No creation logic exists for sector type {sectorType}")
        };
    }

    private SectorType SectorTypeToCreate()
    {
        // TODO: Better distribution of types selected at "random"
        var chance = random.Randf();
        if (chance < 0.6) return SectorType.Value;
        if (chance < 0.7) return SectorType.Nudge;
        if (chance < 0.8) return SectorType.Hold;
        if (chance < 0.9) return SectorType.IncreaseSpeed;
        return SectorType.DecreaseSpeed;
    }

    private ValueSector CreateValueSector(Vector2[] points, Spinner parentSpinner)
    {
        var value = random.RandiRange(1, 6);
        return ValueSector.Instance(valueSectorScene, points, value, parentSpinner);
    }

    private NudgeSector CreateNudgeSector(Vector2[] points, Spinner parentSpinner)
    {
        var nudges = random.RandiRange(1, 3);
        return NudgeSector.Instance(nudgeSectorScene, points, nudges, parentSpinner);
    }

    private HoldSector CreateHoldSector(Vector2[] points, Spinner parentSpinner)
    {
        var holdFor = random.RandiRange(1, 3);
        return HoldSector.Instance(holdSectorScene, points, holdFor, parentSpinner);
    }

    private SpeedAdjustmentSector CreateIncreaseSpeedSector(Vector2[] points, Spinner parentSpinner)
    {
        return BaseSector.Instance<SpeedAdjustmentSector>(increaseSpeedSectorScene, points, parentSpinner);
    }

    private SpeedAdjustmentSector CreateDecreaseSpeedSector(Vector2[] points, Spinner parentSpinner)
    {
        return BaseSector.Instance<SpeedAdjustmentSector>(decreaseSpeedSectorScene, points, parentSpinner);
    }
}
