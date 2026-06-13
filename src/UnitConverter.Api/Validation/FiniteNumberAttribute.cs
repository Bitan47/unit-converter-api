using System.ComponentModel.DataAnnotations;

namespace UnitConverter.Api.Validation;

/// <summary>
/// Validates that a numeric value is a finite number (not NaN, not Infinity),
/// guarding against malformed JSON payloads such as "value": "NaN".
/// </summary>
public sealed class FiniteNumberAttribute : ValidationAttribute
{
    public FiniteNumberAttribute()
    {
        ErrorMessage = "Value must be a finite number.";
    }

    public override bool IsValid(object? value)
    {
        if (value is double d)
        {
            return !double.IsNaN(d) && !double.IsInfinity(d);
        }

        // Let [Required] handle null; anything else is not our concern here.
        return true;
    }
}
