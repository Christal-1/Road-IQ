namespace RoadIQ.Api.Models;

public class RouteComparisonRequest
{
    public RouteDto RouteA { get; set; } = new();

    public RouteDto RouteB { get; set; } = new();
}
