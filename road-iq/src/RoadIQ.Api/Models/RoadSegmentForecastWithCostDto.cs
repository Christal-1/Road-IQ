namespace RoadIQ.Api.Models;

public class RoadSegmentForecastWithCostDto
{
    public int RoadSegmentId { get; set; }

    public double CurrentWearIndex { get; set; }

    public double WearTrendPerDay { get; set; }

    public double PredictedWear30Days { get; set; }

    public string DegradationClass { get; set; } = string.Empty;

    //  Cost impact
    public decimal ProjectedCost30Days { get; set; }

    public decimal ProjectedCost90Days { get; set; }

    public string CostRiskLevel { get; set; } = string.Empty;

    public int SampleWindowDays { get; set; }

    public DateTime LastUpdatedAt { get; set; }
}