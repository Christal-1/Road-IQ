using Microsoft.EntityFrameworkCore;
using RoadIQ.Api.Data;
using RoadIQ.Api.Models;
using RoadIQ.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database (SQLite)
builder.Services.AddDbContext<RoadIqDbContext>(options =>
    options.UseSqlite("Data Source=road-iq.db"));

// Domain services
builder.Services.AddScoped<WearIndexCalculator>();
builder.Services.AddScoped<WearIndexPersistenceService>();
builder.Services.AddScoped<RouteComparisonService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// --------------------
// Health check
// --------------------
app.MapGet("/", () => "RoadIQ API is running ✅");

// --------------------
// Wear Index (CALCULATE + PERSIST)
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

app.Run();