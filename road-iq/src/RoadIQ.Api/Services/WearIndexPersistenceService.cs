using RoadIQ.Api.Data;
using RoadIQ.Api.Models;

namespace RoadIQ.Api.Services;

public class WearIndexPersistenceService
{
    private readonly RoadIqDbContext _db;
    private readonly WearIndexCalculator _calculator;

    public WearIndexPersistenceService(
        RoadIqDbContext db,
        WearIndexCalculator calculator)
    {
        _db = db;
        _calculator = calculator;
    }

    public WearIndexResponse CalculateAndPersist(
        List<SensorRecordDto> records)
    {
        var result = _calculator.Calculate(records);

        // Very simple segment logic for v1:
        // One segment per calculation (can be improved later)
        var segment = new RoadSegment
        {
            LatStart = records.First().Lat,
            LonStart = records.First().Lon,
            LatEnd = records.Last().Lat,
            LonEnd = records.Last().Lon
        };

        _db.RoadSegments.Add(segment);
        _db.SaveChanges();

        var snapshot = new RoadWearSnapshot
        {
            RoadSegmentId = segment.Id,
            RecordedAt = DateTime.UtcNow,
            WearIndex = result.WearIndex,
            SampleCount = records.Count
        };

        _db.RoadWearSnapshots.Add(snapshot);
        _db.SaveChanges();

        return result;
    }
}
