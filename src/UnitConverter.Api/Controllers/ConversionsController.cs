using Microsoft.AspNetCore.Mvc;
using UnitConverter.Api.Models;
using UnitConverter.Api.Services;

namespace UnitConverter.Api.Controllers;

/// <summary>
/// Exposes endpoints for converting numeric values between units across
/// supported categories (Length, Temperature, Weight, ...).
/// </summary>
[ApiController]
[Route("api/v1/conversions")]
[Produces("application/json")]
public sealed class ConversionsController : ControllerBase
{
    private readonly IUnitConversionService _conversionService;
    private readonly ILogger<ConversionsController> _logger;

    public ConversionsController(IUnitConversionService conversionService, ILogger<ConversionsController> logger)
    {
        _conversionService = conversionService;
        _logger = logger;
    }

    /// <summary>
    /// Converts a numeric value from one unit to another within a given category.
    /// </summary>
    /// <param name="request">The conversion request, including category, source/target units and value.</param>
    /// <response code="200">The conversion was performed successfully.</response>
    /// <response code="400">The request was invalid (e.g., unsupported unit, negative length, validation failure).</response>
    [HttpPost]
    [ProducesResponseType(typeof(ConversionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public ActionResult<ConversionResponse> Convert([FromBody] ConversionRequest request)
    {
        // Model validation (e.g., [Required], [FiniteNumber]) is handled
        // automatically via [ApiController] -> returns 400 with
        // ValidationProblemDetails before this method is even invoked.

        _logger.LogInformation(
            "Conversion requested: {Category} {Value} {FromUnit} -> {ToUnit}",
            request.Category, request.Value, request.FromUnit, request.ToUnit);

        var result = _conversionService.Convert(request);

        return Ok(result);
    }
}
