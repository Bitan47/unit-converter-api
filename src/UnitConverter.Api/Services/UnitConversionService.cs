using UnitConverter.Api.Exceptions;
using UnitConverter.Api.Models;
using UnitConverter.Api.Strategies;

namespace UnitConverter.Api.Services;

/// <summary>
/// Resolves the correct <see cref="IConversionStrategy"/> for a given
/// <see cref="ConversionCategory"/> and delegates the conversion to it.
///
/// All available strategies are injected via DI (registered as
/// IEnumerable&lt;IConversionStrategy&gt; in Program.cs) and indexed by
/// category at construction time. Adding support for a new category is
/// purely additive: implement <see cref="IConversionStrategy"/> and register
/// it in the DI container - this class and the controller require zero
/// changes.
/// </summary>
public sealed class UnitConversionService : IUnitConversionService
{
    private readonly IReadOnlyDictionary<ConversionCategory, IConversionStrategy> _strategies;

    public UnitConversionService(IEnumerable<IConversionStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Category);
    }

    public ConversionResponse Convert(ConversionRequest request)
    {
        if (!_strategies.TryGetValue(request.Category, out var strategy))
        {
            throw new UnsupportedCategoryException(request.Category.ToString());
        }

        var result = strategy.Convert(request.FromUnit, request.ToUnit, request.Value);

        return new ConversionResponse
        {
            Category = request.Category,
            FromUnit = request.FromUnit,
            ToUnit = request.ToUnit,
            InputValue = request.Value,
            ConvertedValue = result
        };
    }
}
