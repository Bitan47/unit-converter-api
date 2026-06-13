namespace UnitConverter.Api.Models;

/// <summary>
/// Represents a category of unit conversions (e.g., Length, Temperature, Weight).
/// Adding a new category here requires a corresponding IConversionStrategy implementation
/// registered in the strategy resolver - no changes to controllers or core dispatch logic.
/// </summary>
public enum ConversionCategory
{
    Length,
    Temperature,
    Weight
}
