using UnitConverter.Api.Exceptions;
using UnitConverter.Api.Strategies.Weight;
using Xunit;

namespace UnitConverter.Tests.Strategies;

public class WeightConversionStrategyTests
{
    private readonly WeightConversionStrategy _strategy = new();

    [Theory]
    [InlineData("kilogram", "pound", 1, 2.20462)]
    [InlineData("gram", "kilogram", 1000, 1)]
    [InlineData("pound", "ounce", 1, 16)]
    [InlineData("tonne", "kilogram", 1, 1000)]
    public void Convert_ReturnsExpectedResult_ForValidUnits(string from, string to, double input, double expected)
    {
        var result = _strategy.Convert(from, to, input);
        Assert.Equal(expected, result, precision: 3);
    }

    [Fact]
    public void Convert_NegativeValue_ThrowsInvalidConversionValueException()
    {
        Assert.Throws<InvalidConversionValueException>(() => _strategy.Convert("kg", "lb", -1));
    }

    [Fact]
    public void Convert_UnsupportedUnit_ThrowsUnsupportedUnitException()
    {
        Assert.Throws<UnsupportedUnitException>(() => _strategy.Convert("stone", "kg", 1));
    }

    [Fact]
    public void Convert_ZeroValue_IsValid()
    {
        var result = _strategy.Convert("kg", "lb", 0);
        Assert.Equal(0, result, precision: 6);
    }
}
