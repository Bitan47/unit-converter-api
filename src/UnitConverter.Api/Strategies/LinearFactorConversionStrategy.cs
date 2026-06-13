using UnitConverter.Api.Exceptions;
using UnitConverter.Api.Models;

namespace UnitConverter.Api.Strategies;

/// <summary>
/// Base class for conversion strategies whose units relate to a common base
/// unit via a simple multiplicative factor (e.g., Length, Weight/Mass).
///
/// Each unit is defined by a factor representing "how many base units equal
/// one of this unit". Conversion is then a two-step operation:
/// value_in_base = value * factor[from]
/// result = value_in_base / factor[to]
///
/// To add a new unit to a category, simply add one entry to the dictionary
/// returned by <see cref="GetFactors"/> - no other code changes required.
/// This same shape generalizes cleanly to hundreds of units, and the
/// dictionary could be replaced by a repository-backed lookup (e.g., from
/// PostgreSQL or CosmosDB) without changing the public contract.
/// </summary>
public abstract class LinearFactorConversionStrategy : IConversionStrategy
{
    public abstract ConversionCategory Category { get; }

    /// <summary>
    /// Maps unit codes (case-insensitive) to their factor relative to the
    /// category's base unit.
    /// </summary>
    protected abstract IReadOnlyDictionary<string, double> GetFactors();

    /// <summary>
    /// Optional hook for category-specific domain validation
    /// (e.g., rejecting negative lengths). Default is a no-op.
    /// </summary>
    protected virtual void ValidateValue(double value, string unit)
    {
    }

    public bool IsUnitSupported(string unit) =>
        GetFactors().ContainsKey(Normalize(unit));

    public double Convert(string fromUnit, string toUnit, double value)
    {
        var factors = GetFactors();
        var from = Normalize(fromUnit);
        var to = Normalize(toUnit);

        if (!factors.TryGetValue(from, out var fromFactor))
        {
            throw new UnsupportedUnitException(fromUnit, Category.ToString());
        }

        if (!factors.TryGetValue(to, out var toFactor))
        {
            throw new UnsupportedUnitException(toUnit, Category.ToString());
        }

        ValidateValue(value, fromUnit);

        var baseValue = value * fromFactor;
        return baseValue / toFactor;
    }

    private static string Normalize(string unit) => unit.Trim().ToLowerInvariant();
}
