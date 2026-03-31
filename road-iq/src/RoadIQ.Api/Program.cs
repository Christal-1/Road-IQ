using Microsoft.EntityFrameworkCore;
using RoadIQ.Api.Data;
using RoadIQ.Api.Models;
using RoadIQ.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Swagger
// --------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------
//  CORS CONFIGURATION (REQUIRED FOR FRONTEND)
// --------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(
                "http://127.0.0.1:5500",
                "http://localhost:5500"
            );
    });
});

// --------------------
// Database (SQLite)
// --------------------
builder.Services.AddDbContext<RoadIqDbContext>(options =>
    options.UseSqlite("Data Source=road-iq.db"));

// --------------------
// Domain services
// --------------------
builder.Services.AddScoped<WearIndexCalculator>();
builder.Services.AddScoped<WearIndexPersistenceService>();
builder.Services.AddScoped<RouteComparisonService>();
builder.Services.AddScoped<RoadDegradationService>();
builder.Services.AddScoped<RoadWearCostService>();

var app = builder.Build();

// --------------------
//  ENABLE CORS (MUST BE BEFORE MAP* CALLS)
// --------------------
app.UseCors("FrontendPolicy");

// --------------------
// Swagger UI
// --------------------
app.UseSwagger();
app.UseSwaggerUI();

// --------------------
// Health check
// --------------------
app.MapGet("/", () => "RoadIQ API is running ✅");

// --------------------
// Wear Index (CALCULATE + PERSIST + DEGRADATION UPDATE)
// --------------------
app.MapPost("/wear-index/calculate", (
    WearIndexRequest request,
    WearIndexPersistenceService persistenceService) =>
{
    var result = persistenceService.CalculateAndPersist(
        request.SensorRecords);

    return Results.Ok(result);
});

// --------------------
// Route comparison
// --------------------
app.MapPost("/routes/compare", (
    RouteComparisonRequest request,
    RouteComparisonService comparisonService) =>
{
    var result = comparisonService.CompareRoutes(
        request.RouteA,
        request.RouteB);

    return Results.Ok(result);
});

// --------------------
// Road degradation + cost forecast
// --------------------
app.MapGet("/roads/{roadSegmentId}/degradation-forecast", async (
    int roadSegmentId,
    RoadIqDbContext db,
    RoadWearCostService costService) =>
{
    var profile = await db.RoadDegradationProfiles
        .FirstOrDefaultAsync(p => p.RoadSegmentId == roadSegmentId);

    if (profile == null)
        return Results.NotFound(
            $"No degradation data found for road segment {roadSegmentId}");

    var (cost30, cost90, risk) =
        costService.CalculateCostImpact(
            profile.CurrentWearIndex,
            profile.PredictedWear30Days);

    var response = new RoadSegmentForecastWithCostDto
    {
        RoadSegmentId = profile.RoadSegmentId,
        CurrentWearIndex = profile.CurrentWearIndex,
        WearTrendPerDay = profile.WearTrendPerDay,
        PredictedWear30Days = profile.PredictedWear30Days,
        DegradationClass = profile.DegradationClass,
        ProjectedCost30Days = cost30,
        ProjectedCost90Days = cost90,
        CostRiskLevel = risk,
        SampleWindowDays = profile.SampleWindowDays,
        LastUpdatedAt = profile.LastUpdatedAt
    };

    return Results.Ok(response);
});

app.Run();