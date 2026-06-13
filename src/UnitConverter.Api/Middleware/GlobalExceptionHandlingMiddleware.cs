using System.Net;
using System.Text.Json;
using UnitConverter.Api.Exceptions;
using UnitConverter.Api.Models;

namespace UnitConverter.Api.Middleware;

/// <summary>
/// Centralized exception handling middleware. Catches all unhandled
/// exceptions, logs them, and translates them into structured
/// <see cref="ErrorResponse"/> JSON payloads with appropriate HTTP status
/// codes - so individual controllers/strategies never need try/catch blocks
/// for cross-cutting error handling.
/// </summary>
public sealed class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        var (statusCode, errorResponse) = exception switch
        {
            UnsupportedCategoryException ex => (
                HttpStatusCode.BadRequest,
                new ErrorResponse { Code = ex.Code, Message = ex.Message, TraceId = traceId }),

            UnsupportedUnitException ex => (
                HttpStatusCode.BadRequest,
                new ErrorResponse { Code = ex.Code, Message = ex.Message, TraceId = traceId }),

            InvalidConversionValueException ex => (
                HttpStatusCode.BadRequest,
                new ErrorResponse { Code = ex.Code, Message = ex.Message, TraceId = traceId }),

            _ => (
                HttpStatusCode.InternalServerError,
                new ErrorResponse
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Message = "An unexpected error occurred while processing the request.",
                    TraceId = traceId
                })
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception processing request {TraceId}", traceId);
        }
        else
        {
            _logger.LogWarning("Handled domain exception ({Code}) for request {TraceId}: {Message}",
                errorResponse.Code, traceId, errorResponse.Message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, JsonOptions));
    }
}
