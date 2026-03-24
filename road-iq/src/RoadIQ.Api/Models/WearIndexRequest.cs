namespace RoadIQ.Api.Models;

public class WearIndexRequest
{
    public List<SensorRecordDto> SensorRecords { get; set; } = new();
}
