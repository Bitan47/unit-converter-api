using System.ComponentModel.DataAnnotations;
using UnitConverter.Api.Validation;

namespace UnitConverter.Api.Models;

/// <summary>
/// Request payload for a unit conversion operation.
/// </summary>
public class ConversionRequest
{
    /// <summary>
    /// The conversion category (Length, Temperature, Weight).
    /// </summary>
    [Required]
    public ConversionCategory Category { get; set; }

    /// <summary>
    /// The unit of the input value (e.g., "Meter", "Celsius", "Kilogram").
    /// Must be a valid unit code within the selected category.
    /// </summary>
    [Required(ErrorMessage = "FromUnit is required.")]
    [MinLength(1, ErrorMessage = "FromUnit cannot be empty.")]
    public string FromUnit { get; set; } = string.Empty;

    /// <summary>
    /// The desired output unit (e.g., "Foot", "Fahrenheit", "Pound").
    /// Must be a valid unit code within the selected category.
    /// </summary>
    [Required(ErrorMessage = "ToUnit is required.")]
    [MinLength(1, ErrorMessage = "ToUnit cannot be empty.")]
    public string ToUnit { get; set; } = string.Empty;

    /// <summary>
    /// The numeric value to convert.
    /// Negative values are rejected for Length and Weight (physically meaningless),
    /// but allowed for Temperature (e.g., -40 Celsius is valid).
    /// </summary>
    [Required]
    [FiniteNumber]
    public double Value { get; set; }
}
