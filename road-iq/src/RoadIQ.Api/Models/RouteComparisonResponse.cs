namespace RoadIQ.Api.Models;

public class RouteComparisonResponse
{
    public string RecommendedRoute { get; set; } = string.Empty;

    public double WearIndexRouteA { get; set; }

    public double WearIndexRouteB { get; set; }

    public string Reason { get; set; } = string.Empty;
}