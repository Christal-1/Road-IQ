using RoadIQ.Api.Models;

namespace RoadIQ.Api.Services;

public class RoadWearCostService
{
    // v1: average passenger vehicle
    private const decimal CostFactorPerWearPointPerMonth = 250m;

    public (decimal cost30Days, decimal cost90Days, string riskLevel)
        CalculateCostImpact(double currentWear, double predictedWear30Days)
    {
        var incrementalWear = Math.Max(predictedWear30Days - currentWear, 0);

        var cost30Days =
            (decimal)incrementalWear * CostFactorPerWearPointPerMonth;

        var cost90Days = cost30Days * 3;

        var riskLevel = ClassifyCostRisk(cost90Days);

        return (
            Math.Round(cost30Days, 2),
            Math.Round(cost90Days, 2),
            riskLevel
        );
    }

    private static string ClassifyCostRisk(decimal cost90Days)
    {
        if (cost90Days < 300)
            return "Low";

        if (cost90Days < 1000)
            return "Moderate";

        return "High";
    }
}