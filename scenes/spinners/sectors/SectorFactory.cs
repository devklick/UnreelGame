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

    public BaseSector[] CreateSectors(int reelNo, int numberOfSectors, int sectorSize, float radius, Spinner parentSpinner)
    {
        var sectors = new BaseSector[numberOfSectors];
        var maxSectorSize = 360 - sectorSize;

        for (int sectorNo = 0; sectorNo < numberOfSectors; sectorNo++)
        {
            var sectorStart = sectorSize * sectorNo;
            var sectorEnd = sectorStart + sectorSize;
            sectors[sectorNo] = CreateSector(reelNo, sectorNo, radius, sectorSize, sectorStart, sectorEnd, parentSpinner);
        }

        return sectors;
    }

    private BaseSector CreateSector(int reelNo, int sectorNo, float radius, float sectorSize, float angleFrom, float angleTo, Spinner parentSpinner)
    {
        int nbPoints = 32;
        var pointsArc = new Vector2[nbPoints + 2];

        pointsArc[0] = Vector2.Zero;
        for (int i = 0; i <= nbPoints; ++i)
        {
            float anglePoint = Mathf.Deg2Rad(angleFrom + i * (angleTo - angleFrom) / nbPoints - 90);
            pointsArc[i + 1] = new Vector2(Mathf.Cos(anglePoint), Mathf.Sin(anglePoint)) * radius;
        }
        var avgX = pointsArc.Sum(p => p.x) / pointsArc.Length / 2;
        var avgY = pointsArc.Sum(p => p.y) / pointsArc.Length / 2;
        var label = new Label { RectPosition = new Vector2(avgX, avgY), Modulate = Colors.Black, RectRotation = -90 + (sectorSize / 2) + (sectorSize * sectorNo) };
        var sectorType = SectorTypeToCreate();

        return CreateSectorType(sectorType, pointsArc, parentSpinner, label);
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

    private BaseSector CreateSectorType(SectorType sectorType, Vector2[] points, Spinner parentSpinner, Label label) => sectorType switch
    {
        SectorType.Value => CreateValueSector(points, parentSpinner, label),
        SectorType.Nudge => CreateNudgeSector(points, parentSpinner, label),
        SectorType.Hold => CreateHoldSector(points, parentSpinner, label),
        SectorType.IncreaseSpeed => CreateIncreaseSpeedSector(points, parentSpinner, label),
        SectorType.DecreaseSpeed => CreateDecreaseSpeedSector(points, parentSpinner, label),
        _ => throw new NotImplementedException($"No creation logic exists for sector type {sectorType}")
    };

    private ValueSector CreateValueSector(Vector2[] points, Spinner parentSpinner, Label label)
    {
        var value = random.RandiRange(1, 6);
        label.Text = value.ToString();
        return ValueSector.Instance(valueSectorScene, points, value, parentSpinner, label);
    }

    private NudgeSector CreateNudgeSector(Vector2[] points, Spinner parentSpinner, Label label)
    {
        var nudges = random.RandiRange(1, 2);
        label.Text = $"Nudge ({nudges})";
        return NudgeSector.Instance(nudgeSectorScene, points, nudges, parentSpinner, label);
    }

    private HoldSector CreateHoldSector(Vector2[] points, Spinner parentSpinner, Label label)
    {
        var holds = random.RandiRange(1, 2);
        label.Text = $"Hold ({holds})";
        return HoldSector.Instance(holdSectorScene, points, holds, parentSpinner, label);
    }

    private SpeedAdjustmentSector CreateIncreaseSpeedSector(Vector2[] points, Spinner parentSpinner, Label label)
    {
        label.Text = "Faster";
        return BaseSector.Instance<SpeedAdjustmentSector>(increaseSpeedSectorScene, points, parentSpinner, label);
    }

    private SpeedAdjustmentSector CreateDecreaseSpeedSector(Vector2[] points, Spinner parentSpinner, Label label)
    {
        label.Text = "Slower";
        return BaseSector.Instance<SpeedAdjustmentSector>(decreaseSpeedSectorScene, points, parentSpinner, label);
    }
}
