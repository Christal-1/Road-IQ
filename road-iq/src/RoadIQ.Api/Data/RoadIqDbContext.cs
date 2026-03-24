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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RoadSegment>()
            .HasMany(r => r.WearSnapshots)
            .WithOne(w => w.RoadSegment)
            .HasForeignKey(w => w.RoadSegmentId);

        base.OnModelCreating(modelBuilder);
    }
}
