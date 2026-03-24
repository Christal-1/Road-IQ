using RoadIQ.Api.Models;

namespace RoadIQ.Api.Services;

public class RouteComparisonService
{
    private readonly WearIndexCalculator _wearIndexCalculator;

    public RouteComparisonService(WearIndexCalculator wearIndexCalculator)
    {
        _wearIndexCalculator = wearIndexCalculator;
    }

    public RouteComparisonResponse CompareRoutes(
        RouteDto routeA,
        RouteDto routeB)
    {
        if (routeA.SensorRecords.Count == 0 || routeB.SensorRecords.Count == 0)
        {
            throw new ArgumentException("Both routes must contain sensor data.");
        }

        var resultA = _wearIndexCalculator.Calculate(routeA.SensorRecords);
        var resultB = _wearIndexCalculator.Calculate(routeB.SensorRecords);

        var recommendedRoute =
            resultA.WearIndex <= resultB.WearIndex
                ? routeA.RouteName
                : routeB.RouteName;

        var reason =
            $"Route '{recommendedRoute}' has lower mechanical wear " +
            $"({Math.Min(resultA.WearIndex, resultB.WearIndex)})";

        return new RouteComparisonResponse
        {
            RecommendedRoute = recommendedRoute,
            WearIndexRouteA = resultA.WearIndex,
            WearIndexRouteB = resultB.WearIndex,
            Reason = reason
        };
    }
}