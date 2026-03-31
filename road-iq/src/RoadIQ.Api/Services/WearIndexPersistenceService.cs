using RoadIQ.Api.Data;
using RoadIQ.Api.Models;

namespace RoadIQ.Api.Services;

public class WearIndexPersistenceService
{
    private readonly RoadIqDbContext _db;
    private readonly WearIndexCalculator _calculator;
    private readonly RoadDegradationService _degradationService;

    public WearIndexPersistenceService(
        RoadIqDbContext db,
        WearIndexCalculator calculator,
        RoadDegradationService degradationService)
    {
        _db = db;
        _calculator = calculator;
        _degradationService = degradationService;
    }

    public WearIndexResponse CalculateAndPersist(
        List<SensorRecordDto> records)
    {
        if (records == null || records.Count == 0)
            throw new ArgumentException("Sensor records cannot be empty.");

        // 1️⃣ Calculate Wear Index
        var result = _calculator.Calculate(records);

        // 2️⃣ Determine segment identity (rounded coordinates)
        var startLat = RoundCoord(records.First().Lat);
        var startLon = RoundCoord(records.First().Lon);
        var endLat = RoundCoord(records.Last().Lat);
        var endLon = RoundCoord(records.Last().Lon);

        // 3️⃣ Try to find an existing segment
        var segment = _db.RoadSegments.FirstOrDefault(s =>
            Math.Round(s.LatStart, 4) == startLat &&
            Math.Round(s.LonStart, 4) == startLon &&
            Math.Round(s.LatEnd, 4) == endLat &&
            Math.Round(s.LonEnd, 4) == endLon
        );

        // 4️⃣ Create segment if it does not exist
        if (segment == null)
        {
            segment = new RoadSegment
            {
                LatStart = startLat,
                LonStart = startLon,
                LatEnd = endLat,
                LonEnd = endLon
            };

            _db.RoadSegments.Add(segment);
            _db.SaveChanges();
        }

        // 5️⃣ Persist wear snapshot (time-series data)
        var snapshot = new RoadWearSnapshot
        {
            RoadSegmentId = segment.Id,
            RecordedAt = DateTime.UtcNow,
            WearIndex = result.WearIndex,
            SampleCount = records.Count
        };

        _db.RoadWearSnapshots.Add(snapshot);
        _db.SaveChanges();

        // ✅ 6️⃣ Automatically update degradation profile
        _degradationService.UpdateDegradationProfileAsync(segment.Id).Wait();

        return result;
    }

    /// <summary>
    /// Rounds coordinates to ~11m precision.
    /// This ensures repeated drives on the same road
    /// map to the same RoadSegment.
    /// </summary>
    private static double RoundCoord(double value)
    {
        return Math.Round(value, 4);
    }
}