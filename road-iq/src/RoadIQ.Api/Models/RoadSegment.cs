namespace RoadIQ.Api.Models;

public class RoadSegment
{
    public int Id { get; set; }

    public double LatStart { get; set; }
    public double LonStart { get; set; }

    public double LatEnd { get; set; }
    public double LonEnd { get; set; }

    public ICollection<RoadWearSnapshot> WearSnapshots { get; set; }
        = new List<RoadWearSnapshot>();
}