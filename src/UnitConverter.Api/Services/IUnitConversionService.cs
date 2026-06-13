using UnitConverter.Api.Models;

namespace UnitConverter.Api.Services;

/// <summary>
/// Top-level service that dispatches conversion requests to the appropriate
/// <see cref="Strategies.IConversionStrategy"/> based on category.
/// </summary>
public interface IUnitConversionService
{
    /// <summary>
    /// Performs the conversion described by <paramref name="request"/>.
    /// </summary>
    /// <exception cref="Exceptions.UnsupportedCategoryException">
    /// Thrown when no strategy is registered for the requested category.
    /// </exception>
    ConversionResponse Convert(ConversionRequest request);
}
