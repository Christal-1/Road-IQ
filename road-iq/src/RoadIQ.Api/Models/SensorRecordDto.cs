namespace RoadIQ.Api.Models;

public class SensorRecordDto
{
    public DateTime Timestamp { get; set; }

    public double Lat { get; set; }

    public double Lon { get; set; }

    public double SpeedKmh { get; set; }

    public double AccelZ { get; set; }
}