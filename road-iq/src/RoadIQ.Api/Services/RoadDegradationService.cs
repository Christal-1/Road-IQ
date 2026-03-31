using Microsoft.EntityFrameworkCore;
using RoadIQ.Api.Data;
using RoadIQ.Api.Models;

namespace RoadIQ.Api.Services;

public class RoadDegradationService
{
    private readonly RoadIqDbContext _db;

    // Number of days to look back for trend calculation
    private const int SampleWindowDays = 14;

    public RoadDegradationService(RoadIqDbContext db)
    {
        _db = db;
    }

    public async Task UpdateDegradationProfileAsync(int roadSegmentId)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-SampleWindowDays);

        var snapshots = await _db.RoadWearSnapshots
            .Where(s => s.RoadSegmentId == roadSegmentId && s.RecordedAt >= cutoffDate)
            .OrderBy(s => s.RecordedAt)
            .ToListAsync();

        // Need at least two data points
        if (snapshots.Count < 2)
            return;

        var oldest = snapshots.First();
        var newest = snapshots.Last();

        var daysBetween =
            (newest.RecordedAt - oldest.RecordedAt).TotalDays;

        if (daysBetween <= 0)
            return;

        var wearTrendPerDay =
            (newest.WearIndex - oldest.WearIndex) / daysBetween;

        var profile = await _db.RoadDegradationProfiles
            .FirstOrDefaultAsync(p => p.RoadSegmentId == roadSegmentId);

        if (profile == null)
        {
            profile = new RoadDegradationProfile
            {
                RoadSegmentId = roadSegmentId
            };

            _db.RoadDegradationProfiles.Add(profile);
        }

        profile.CurrentWearIndex = newest.WearIndex;
        profile.WearTrendPerDay = Math.Round(wearTrendPerDay, 4);
        profile.DegradationClass = ClassifyDegradation(profile.WearTrendPerDay);

        // Simple 30-day wear forecast (v1)
        profile.PredictedWear30Days =
            Math.Round(
                profile.CurrentWearIndex + (profile.WearTrendPerDay * 30),
                2
            );

        profile.SampleWindowDays = SampleWindowDays;
        profile.LastUpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    private static string ClassifyDegradation(double wearTrendPerDay)
    {
        if (wearTrendPerDay <= 0.01)
            return "Stable";

        if (wearTrendPerDay <= 0.05)
            return "Normal";

        if (wearTrendPerDay <= 0.10)
            return "Accelerating";

        return "Rapid Degradation";
    }

    public async Task<RoadDegradationProfile?> GetDegradationProfileAsync(int roadSegmentId)
    {
        return await _db.RoadDegradationProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.RoadSegmentId == roadSegmentId);
    }

}