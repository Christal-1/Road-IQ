namespace RoadIQ.Api.Models;

public class WearIndexResponse
{
    public double WearIndex { get; set; }

    public double AvgAbsAccel { get; set; }

    public double AccelVariance { get; set; }

    public int ShockCount { get; set; }

    public double SpeedKmh { get; set; }
}