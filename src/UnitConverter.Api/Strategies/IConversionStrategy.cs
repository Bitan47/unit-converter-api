using UnitConverter.Api.Models;

namespace UnitConverter.Api.Strategies;

/// <summary>
/// Defines the contract for a category-specific conversion strategy
/// (e.g., Length, Temperature, Weight).
///
/// Implementations encapsulate everything needed to convert between any two
/// units within their category: unit recognition, validation, and the
/// actual numeric conversion. This is the seam that allows the system to
/// scale to hundreds of units/categories without touching controllers,
/// middleware, or other strategies - each implementation is self-contained
/// and independently testable, and could just as easily be backed by a
/// database-driven lookup (PostgreSQL, CosmosDB, etc.) in the future by
/// swapping the implementation behind this same interface.
/// </summary>
public interface IConversionStrategy
{
    /// <summary>
    /// The category this strategy handles.
    /// </summary>
    ConversionCategory Category { get; }

    /// <summary>
    /// Returns true if the supplied unit code is recognized by this strategy
    /// (case-insensitive).
    /// </summary>
    bool IsUnitSupported(string unit);

    /// <summary>
    /// Converts <paramref name="value"/> from <paramref name="fromUnit"/> to
    /// <paramref name="toUnit"/>.
    /// </summary>
    /// <exception cref="Exceptions.UnsupportedUnitException">
    /// Thrown when either unit is not recognized by this strategy.
    /// </exception>
    /// <exception cref="Exceptions.InvalidConversionValueException">
    /// Thrown when <paramref name="value"/> is outside the valid domain
    /// for the given unit (e.g., negative length, below absolute zero).
    /// </exception>
    double Convert(string fromUnit, string toUnit, double value);
}
