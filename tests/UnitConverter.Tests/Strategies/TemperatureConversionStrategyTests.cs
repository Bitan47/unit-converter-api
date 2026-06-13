using UnitConverter.Api.Exceptions;
using UnitConverter.Api.Strategies.Temperature;
using Xunit;

namespace UnitConverter.Tests.Strategies;

public class TemperatureConversionStrategyTests
{
    private readonly TemperatureConversionStrategy _strategy = new();

    [Theory]
    [InlineData("celsius", "fahrenheit", 0, 32)]
    [InlineData("celsius", "fahrenheit", 100, 212)]
    [InlineData("fahrenheit", "celsius", 32, 0)]
    [InlineData("celsius", "kelvin", 0, 273.15)]
    [InlineData("kelvin", "celsius", 0, -273.15)]
    [InlineData("fahrenheit", "celsius", -40, -40)]
    public void Convert_ReturnsExpectedResult_ForValidUnits(string from, string to, double input, double expected)
    {
        var result = _strategy.Convert(from, to, input);
        Assert.Equal(expected, result, precision: 2);
    }

    [Fact]
    public void Convert_NegativeCelsius_IsValid()
    {
        // Negative values are valid for temperature, unlike Length/Weight.
        var result = _strategy.Convert("celsius", "fahrenheit", -10);
        Assert.Equal(14, result, precision: 2);
    }

    [Fact]
    public void Convert_BelowAbsoluteZero_ThrowsInvalidConversionValueException()
    {
        Assert.Throws<InvalidConversionValueException>(() => _strategy.Convert("kelvin", "celsius", -1));
    }

    [Fact]
    public void Convert_UnsupportedUnit_ThrowsUnsupportedUnitException()
    {
        Assert.Throws<UnsupportedUnitException>(() => _strategy.Convert("rankine", "celsius", 1));
    }

    [Fact]
    public void Convert_SameUnit_ReturnsSameValue()
    {
        var result = _strategy.Convert("celsius", "celsius", 25);
        Assert.Equal(25, result, precision: 6);
    }
}
