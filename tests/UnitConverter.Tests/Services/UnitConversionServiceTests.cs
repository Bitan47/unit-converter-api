using UnitConverter.Api.Exceptions;
using UnitConverter.Api.Models;
using UnitConverter.Api.Services;
using UnitConverter.Api.Strategies;
using UnitConverter.Api.Strategies.Length;
using UnitConverter.Api.Strategies.Temperature;
using UnitConverter.Api.Strategies.Weight;
using Xunit;

namespace UnitConverter.Tests.Services;

public class UnitConversionServiceTests
{
    private static UnitConversionService CreateService()
    {
        var strategies = new IConversionStrategy[]
        {
            new LengthConversionStrategy(),
            new TemperatureConversionStrategy(),
            new WeightConversionStrategy()
        };

        return new UnitConversionService(strategies);
    }

    [Fact]
    public void Convert_DispatchesToCorrectStrategy_Length()
    {
        var service = CreateService();
        var request = new ConversionRequest
        {
            Category = ConversionCategory.Length,
            FromUnit = "meter",
            ToUnit = "feet",
            Value = 1
        };

        var result = service.Convert(request);

        Assert.Equal(ConversionCategory.Length, result.Category);
        Assert.Equal(1, result.InputValue);
        Assert.Equal(3.28084, result.ConvertedValue, precision: 4);
    }

    [Fact]
    public void Convert_DispatchesToCorrectStrategy_Temperature()
    {
        var service = CreateService();
        var request = new ConversionRequest
        {
            Category = ConversionCategory.Temperature,
            FromUnit = "celsius",
            ToUnit = "fahrenheit",
            Value = 100
        };

        var result = service.Convert(request);

        Assert.Equal(212, result.ConvertedValue, precision: 2);
    }

    [Fact]
    public void Convert_DispatchesToCorrectStrategy_Weight()
    {
        var service = CreateService();
        var request = new ConversionRequest
        {
            Category = ConversionCategory.Weight,
            FromUnit = "kilogram",
            ToUnit = "pound",
            Value = 1
        };

        var result = service.Convert(request);

        Assert.Equal(2.20462, result.ConvertedValue, precision: 4);
    }

    [Fact]
    public void Convert_UnregisteredCategory_ThrowsUnsupportedCategoryException()
    {
        // Service constructed without a strategy for Weight.
        var service = new UnitConversionService(new IConversionStrategy[]
        {
            new LengthConversionStrategy(),
            new TemperatureConversionStrategy()
        });

        var request = new ConversionRequest
        {
            Category = ConversionCategory.Weight,
            FromUnit = "kg",
            ToUnit = "lb",
            Value = 1
        };

        Assert.Throws<UnsupportedCategoryException>(() => service.Convert(request));
    }

    [Fact]
    public void Convert_PropagatesUnsupportedUnitException_FromStrategy()
    {
        var service = CreateService();
        var request = new ConversionRequest
        {
            Category = ConversionCategory.Length,
            FromUnit = "lightyear",
            ToUnit = "meter",
            Value = 1
        };

        Assert.Throws<UnsupportedUnitException>(() => service.Convert(request));
    }
}
