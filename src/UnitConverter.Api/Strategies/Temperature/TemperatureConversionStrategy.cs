using UnitConverter.Api.Exceptions;
using UnitConverter.Api.Models;

namespace UnitConverter.Api.Strategies.Temperature;

/// <summary>
/// Handles conversions between temperature units.
///
/// Unlike Length/Weight, temperature scales are not simple multiplicative
/// factors of each other (they have different zero-points), so this strategy
/// implements <see cref="IConversionStrategy"/> directly rather than
/// extending <see cref="LinearFactorConversionStrategy"/>. Internally,
/// Kelvin is used as the common base: every supported unit defines a
/// ToKelvin/FromKelvin conversion pair.
///
/// To add a new temperature scale, add one entry to <see cref="Converters"/>.
/// </summary>
public sealed class TemperatureConversionStrategy : IConversionStrategy
{
    public ConversionCategory Category => ConversionCategory.Temperature;

    private const double AbsoluteZeroKelvin = 0.0;

    /// <summary>
    /// Per-unit conversion functions to/from Kelvin (the base unit).
    /// </summary>
    private static readonly IReadOnlyDictionary<string, (Func<double, double> ToKelvin, Func<double, double> FromKelvin)> Converters =
        new Dictionary<string, (Func<double, double> ToKelvin, Func<double, double> FromKelvin)>
        {
            ["celsius"] = (c => c + 273.15, k => k - 273.15),
            ["c"] = (c => c + 273.15, k => k - 273.15),
            ["fahrenheit"] = (f => (f - 32) * 5.0 / 9.0 + 273.15, k => (k - 273.15) * 9.0 / 5.0 + 32),
            ["f"] = (f => (f - 32) * 5.0 / 9.0 + 273.15, k => (k - 273.15) * 9.0 / 5.0 + 32),
            ["kelvin"] = (k => k, k => k),
            ["k"] = (k => k, k => k),
        };

    public bool IsUnitSupported(string unit) => Converters.ContainsKey(Normalize(unit));

    public double Convert(string fromUnit, string toUnit, double value)
    {
        var from = Normalize(fromUnit);
        var to = Normalize(toUnit);

        if (!Converters.TryGetValue(from, out var fromConverter))
        {
            throw new UnsupportedUnitException(fromUnit, Category.ToString());
        }

        if (!Converters.TryGetValue(to, out var toConverter))
        {
            throw new UnsupportedUnitException(toUnit, Category.ToString());
        }

        var kelvin = fromConverter.ToKelvin(value);

        if (kelvin < AbsoluteZeroKelvin)
        {
            throw new InvalidConversionValueException(
                $"Value {value} {fromUnit} is below absolute zero and is not a physically valid temperature.");
        }

        return toConverter.FromKelvin(kelvin);
    }

    private static string Normalize(string unit) => unit.Trim().ToLowerInvariant();
}
