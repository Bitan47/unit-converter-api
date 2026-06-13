namespace UnitConverter.Api.Models;

/// <summary>
/// Structured response returned for a successful conversion.
/// </summary>
public class ConversionResponse
{
    public ConversionCategory Category { get; set; }
    public string FromUnit { get; set; } = string.Empty;
    public string ToUnit { get; set; } = string.Empty;
    public double InputValue { get; set; }
    public double ConvertedValue { get; set; }
}

/// <summary>
/// Standard structured error response shape used across the API,
/// returned both by validation failures and the global exception handler.
/// </summary>
public class ErrorResponse
{
    /// <summary>Machine-readable error code (e.g., "VALIDATION_ERROR", "UNSUPPORTED_UNIT").</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Human-readable summary of the error.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Optional field-level validation details.</summary>
    public IDictionary<string, string[]>? Details { get; set; }

    /// <summary>Correlation/trace id to help support debug a specific request.</summary>
    public string? TraceId { get; set; }
}
