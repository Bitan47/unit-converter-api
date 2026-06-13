using UnitConverter.Api.Exceptions;
using UnitConverter.Api.Models;

namespace UnitConverter.Api.Strategies.Weight;

/// <summary>
/// Handles conversions between weight/mass units.
/// Base unit: Kilogram.
/// </summary>
public sealed class WeightConversionStrategy : LinearFactorConversionStrategy
{
    public override ConversionCategory Category => ConversionCategory.Weight;

    private static readonly IReadOnlyDictionary<string, double> Factors = new Dictionary<string, double>
    {
        // unit code (lowercase) -> kilograms per unit
        ["kilogram"] = 1.0,
        ["kilograms"] = 1.0,
        ["kg"] = 1.0,
        ["gram"] = 0.001,
        ["grams"] = 0.001,
        ["g"] = 0.001,
        ["milligram"] = 0.000001,
        ["milligrams"] = 0.000001,
        ["mg"] = 0.000001,
        ["pound"] = 0.45359237,
        ["pounds"] = 0.45359237,
        ["lb"] = 0.45359237,
        ["lbs"] = 0.45359237,
        ["ounce"] = 0.028349523125,
        ["ounces"] = 0.028349523125,
        ["oz"] = 0.028349523125,
        ["tonne"] = 1000.0,
        ["tonnes"] = 1000.0,
        ["metricton"] = 1000.0,
        ["t"] = 1000.0,
    };

    protected override IReadOnlyDictionary<string, double> GetFactors() => Factors;

    protected override void ValidateValue(double value, string unit)
    {
        if (value < 0)
        {
            throw new InvalidConversionValueException(
                $"Weight value cannot be negative. Received: {value} {unit}.");
        }
    }
}
