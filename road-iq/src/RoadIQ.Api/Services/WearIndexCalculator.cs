using RoadIQ.Api.Models;

namespace RoadIQ.Api.Services;

public class WearIndexCalculator
{
    public WearIndexResponse Calculate(List<SensorRecordDto> records)
    {
        if (records == null || records.Count == 0)
        {
            throw new ArgumentException("Sensor records cannot be empty.");
        }

        var avgAbsAccel = records.Average(r => Math.Abs(r.AccelZ));
        var accelVariance = CalculateVariance(records.Select(r => r.AccelZ));
        var shockCount = records.Count(r => Math.Abs(r.AccelZ) > 2.0);
        var speedKmh = records.Average(r => r.SpeedKmh);

        var roughnessScore = avgAbsAccel * 2;
        var shockPenalty = shockCount * 0.01;
        var speedFactor = speedKmh / 50;

        var rawScore = (roughnessScore + shockPenalty) * speedFactor;
        var wearIndex = Math.Round(Math.Min(rawScore, 10), 2);

        return new WearIndexResponse
        {
            WearIndex = wearIndex,
            AvgAbsAccel = Math.Round(avgAbsAccel, 4),
            AccelVariance = Math.Round(accelVariance, 4),
            ShockCount = shockCount,
            SpeedKmh = Math.Round(speedKmh, 1)
        };
    }

    private static double CalculateVariance(IEnumerable<double> values)
    {
        var list = values.ToList();
        var mean = list.Average();
        return list.Average(v => Math.Pow(v - mean, 2));
    }
}