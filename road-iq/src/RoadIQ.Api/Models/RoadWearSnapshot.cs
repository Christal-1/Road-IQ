namespace RoadIQ.Api.Models;

public class RoadWearSnapshot
{
    public int Id { get; set; }

    public int RoadSegmentId { get; set; }
    public RoadSegment RoadSegment { get; set; } = null!;

    public DateTime RecordedAt { get; set; }

    public double WearIndex { get; set; }

    public int SampleCount { get; set; }
}
