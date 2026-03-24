namespace RoadIQ.Api.Models;

public class RouteDto
{
    public string RouteName { get; set; } = string.Empty;

    public List<SensorRecordDto> SensorRecords { get; set; } = new();
}