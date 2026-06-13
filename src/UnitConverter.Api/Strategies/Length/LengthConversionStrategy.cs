using UnitConverter.Api.Exceptions;
using UnitConverter.Api.Models;

namespace UnitConverter.Api.Strategies.Length;

/// <summary>
/// Handles conversions between length/distance units.
/// Base unit: Meter.
/// </summary>
public sealed class LengthConversionStrategy : LinearFactorConversionStrategy
{
    public override ConversionCategory Category => ConversionCategory.Length;

    private static readonly IReadOnlyDictionary<string, double> Factors = new Dictionary<string, double>
    {
        // unit code (lowercase) -> meters per unit
        ["meter"] = 1.0,
        ["meters"] = 1.0,
        ["m"] = 1.0,
        ["kilometer"] = 1000.0,
        ["kilometers"] = 1000.0,
        ["km"] = 1000.0,
        ["centimeter"] = 0.01,
        ["centimeters"] = 0.01,
        ["cm"] = 0.01,
        ["millimeter"] = 0.001,
        ["millimeters"] = 0.001,
        ["mm"] = 0.001,
        ["foot"] = 0.3048,
        ["feet"] = 0.3048,
        ["ft"] = 0.3048,
        ["inch"] = 0.0254,
        ["inches"] = 0.0254,
        ["in"] = 0.0254,
        ["yard"] = 0.9144,
        ["yards"] = 0.9144,
        ["yd"] = 0.9144,
        ["mile"] = 1609.344,
        ["miles"] = 1609.344,
        ["mi"] = 1609.344,
    };

    protected override IReadOnlyDictionary<string, double> GetFactors() => Factors;

    protected override void ValidateValue(double value, string unit)
    {
        if (value < 0)
        {
            throw new InvalidConversionValueException(
                $"Length value cannot be negative. Received: {value} {unit}.");
        }
    }
}
