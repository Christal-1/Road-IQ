using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadIQ.Api.Models;

public class RoadDegradationProfile
{
    [Key]
    public int Id { get; set; }

    // Foreign key to the road segment
    [Required]
    public int RoadSegmentId { get; set; }

    [ForeignKey(nameof(RoadSegmentId))]
    public RoadSegment RoadSegment { get; set; } = null!;

    // Current state
    public double CurrentWearIndex { get; set; }

    // Trend (delta wear per day)
    public double WearTrendPerDay { get; set; }

    // Human-readable classification
    [MaxLength(50)]
    public string DegradationClass { get; set; } = "Unknown";

    // Simple forecast
    public double PredictedWear30Days { get; set; }

    // Transparency & trust
    public int SampleWindowDays { get; set; }

    public DateTime LastUpdatedAt { get; set; }
}