using Godot;
using System;
using System.Collections.Generic;
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
        if (numberOfSectors < 10)
            throw new NotSupportedException("A spinner must consist of at least 10 sectors");

        var sectors = new BaseSector[numberOfSectors];
        var maxSectorSize = 360 - sectorSize;

        var sectorsToCreate = GetSectorsToCreate(numberOfSectors, 6);

        for (int sectorNo = 0; sectorNo < numberOfSectors; sectorNo++)
        {
            var sectorStart = sectorSize * sectorNo;
            var sectorEnd = sectorStart + sectorSize;
            var sectorToCreate = sectorsToCreate[sectorNo];
            sectors[sectorNo] = CreateSector(reelNo, sectorNo, sectorToCreate.Key, sectorToCreate.Value, radius, sectorSize, sectorStart, sectorEnd, parentSpinner);
        }

        return sectors;
    }

    private BaseSector CreateSector(int reelNo, int sectorNo, SectorType sectorType, int? sectorValue, float radius, float sectorSize, float angleFrom, float angleTo, Spinner parentSpinner)
    {
        int nbPoints = 32;
        var pointsArc = new Vector2[nbPoints + 2];

        pointsArc[0] = Vector2.Zero;
        for (int i = 0; i <= nbPoints; ++i)
        {
            float anglePoint = Mathf.Deg2Rad(angleFrom + i * (angleTo - angleFrom) / nbPoints - 90);
            pointsArc[i + 1] = new Vector2(Mathf.Cos(anglePoint), Mathf.Sin(anglePoint)) * radius;
        }
        var labelX = pointsArc.Average(p => p.x) / 2;
        var labelY = pointsArc.Average(p => p.y) / 2;
        var label = new Label { RectPosition = new Vector2(labelX, labelY), Modulate = Colors.Black, RectRotation = -90 + (sectorSize / 2) + (sectorSize * sectorNo) };

        return CreateSectorType(sectorType, sectorValue, pointsArc, parentSpinner, label);
    }

    private List<KeyValuePair<SectorType, int?>> GetSectorsToCreate(int numberOfSectors, int maxValueSector)
    {
        var sectorsToCreate = new List<KeyValuePair<SectorType, int?>>();

        // Each reel must have exactly one of each possible value sector
        for (int s = 1; s <= maxValueSector; s++)
            sectorsToCreate.Add(new KeyValuePair<SectorType, int?>(SectorType.Value, s));

        // The remaining sectors should be randomly chosen to be one of the logic sectors.
        // Multiple of the same logic sector are allowed.
        for (int l = 0; l < numberOfSectors - maxValueSector; l++)
            sectorsToCreate.Add(new KeyValuePair<SectorType, int?>(LogicSectorTypeToCreate(), null));

        return sectorsToCreate.OrderBy(s => random.Randf()).ToList();
    }

    private SectorType LogicSectorTypeToCreate()
    {
        var chance = random.Randf();
        if (chance < 0.25) return SectorType.Nudge;
        if (chance < 0.50) return SectorType.Hold;
        if (chance < 0.75) return SectorType.IncreaseSpeed;
        return SectorType.DecreaseSpeed;
    }

    private BaseSector CreateSectorType(SectorType sectorType, int? value, Vector2[] points, Spinner parentSpinner, Label label) => sectorType switch
    {
        SectorType.Value => CreateValueSector(points, parentSpinner, label, value ?? throw new InvalidOperationException("A non-null value must be specified when creating a ValueSector")),
        SectorType.Nudge => CreateNudgeSector(points, parentSpinner, label),
        SectorType.Hold => CreateHoldSector(points, parentSpinner, label),
        SectorType.IncreaseSpeed => CreateIncreaseSpeedSector(points, parentSpinner, label),
        SectorType.DecreaseSpeed => CreateDecreaseSpeedSector(points, parentSpinner, label),
        _ => throw new NotImplementedException($"No creation logic exists for sector type {sectorType}")
    };

    private ValueSector CreateValueSector(Vector2[] points, Spinner parentSpinner, Label label, int value)
    {
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
