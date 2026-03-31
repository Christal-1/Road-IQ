using Microsoft.EntityFrameworkCore;
using RoadIQ.Api.Models;

namespace RoadIQ.Api.Data;

public class RoadIqDbContext : DbContext
{
    public RoadIqDbContext(DbContextOptions<RoadIqDbContext> options)
        : base(options)
    {
    }

    public DbSet<RoadSegment> RoadSegments => Set<RoadSegment>();
    public DbSet<RoadWearSnapshot> RoadWearSnapshots => Set<RoadWearSnapshot>();
    public DbSet<RoadDegradationProfile> RoadDegradationProfiles => Set<RoadDegradationProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RoadSegment>()
        .HasOne<RoadDegradationProfile>()
        .WithOne(p => p.RoadSegment)
        .HasForeignKey<RoadDegradationProfile>(p => p.RoadSegmentId);

        base.OnModelCreating(modelBuilder);
    }

}
