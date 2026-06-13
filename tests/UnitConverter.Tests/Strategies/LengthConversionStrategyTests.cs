using UnitConverter.Api.Exceptions;
using UnitConverter.Api.Strategies.Length;
using Xunit;

namespace UnitConverter.Tests.Strategies;

public class LengthConversionStrategyTests
{
    private readonly LengthConversionStrategy _strategy = new();

    [Theory]
    [InlineData("meter", "feet", 1, 3.28084)]
    [InlineData("kilometer", "meter", 1, 1000)]
    [InlineData("inch", "centimeter", 1, 2.54)]
    [InlineData("mile", "kilometer", 1, 1.609344)]
    public void Convert_ReturnsExpectedResult_ForValidUnits(string from, string to, double input, double expected)
    {
        var result = _strategy.Convert(from, to, input);
        Assert.Equal(expected, result, precision: 4);
    }

    [Fact]
    public void Convert_SameUnit_ReturnsSameValue()
    {
        var result = _strategy.Convert("meter", "meter", 42);
        Assert.Equal(42, result, precision: 6);
    }

    [Fact]
    public void Convert_IsCaseInsensitive()
    {
        var result = _strategy.Convert("METER", "Feet", 1);
        Assert.Equal(3.28084, result, precision: 4);
    }

    [Fact]
    public void Convert_UnsupportedFromUnit_ThrowsUnsupportedUnitException()
    {
        Assert.Throws<UnsupportedUnitException>(() => _strategy.Convert("lightyear", "meter", 1));
    }

    [Fact]
    public void Convert_UnsupportedToUnit_ThrowsUnsupportedUnitException()
    {
        Assert.Throws<UnsupportedUnitException>(() => _strategy.Convert("meter", "lightyear", 1));
    }

    [Fact]
    public void Convert_NegativeValue_ThrowsInvalidConversionValueException()
    {
        Assert.Throws<InvalidConversionValueException>(() => _strategy.Convert("meter", "feet", -5));
    }

    [Fact]
    public void Convert_ZeroValue_IsValid()
    {
        var result = _strategy.Convert("meter", "feet", 0);
        Assert.Equal(0, result, precision: 6);
    }

    [Fact]
    public void IsUnitSupported_ReturnsTrueForKnownUnit_FalseForUnknown()
    {
        Assert.True(_strategy.IsUnitSupported("km"));
        Assert.True(_strategy.IsUnitSupported("Meters"));
        Assert.False(_strategy.IsUnitSupported("parsec"));
    }
}
