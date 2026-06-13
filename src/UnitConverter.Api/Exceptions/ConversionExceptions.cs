namespace UnitConverter.Api.Exceptions;

/// <summary>
/// Base type for all conversion-domain exceptions. Caught explicitly by the
/// global exception handling middleware and mapped to 4xx responses.
/// </summary>
public abstract class ConversionException : Exception
{
    public string Code { get; }

    protected ConversionException(string code, string message) : base(message)
    {
        Code = code;
    }
}

/// <summary>
/// Thrown when the requested conversion category has no registered strategy.
/// </summary>
public sealed class UnsupportedCategoryException : ConversionException
{
    public UnsupportedCategoryException(string category)
        : base("UNSUPPORTED_CATEGORY", $"No conversion strategy is registered for category '{category}'.")
    {
    }
}

/// <summary>
/// Thrown when a unit code is not recognized within its category.
/// </summary>
public sealed class UnsupportedUnitException : ConversionException
{
    public UnsupportedUnitException(string unit, string category)
        : base("UNSUPPORTED_UNIT", $"Unit '{unit}' is not supported for category '{category}'.")
    {
    }
}

/// <summary>
/// Thrown when an input value is outside the physically valid domain for a unit
/// (e.g., a negative length, or a temperature below absolute zero).
/// </summary>
public sealed class InvalidConversionValueException : ConversionException
{
    public InvalidConversionValueException(string message)
        : base("INVALID_VALUE", message)
    {
    }
}
