namespace RoadIQ.Api.Models;

public class RoadDegradationForecastResponse
{
    public int RoadSegmentId { get; set; }

    public double CurrentWearIndex { get; set; }

    public double WearTrendPerDay { get; set; }

    public string DegradationClass { get; set; } = string.Empty;

    public double PredictedWear30Days { get; set; }

    public int SampleWindowDays { get; set; }

    public DateTime LastUpdatedAt { get; set; }
}