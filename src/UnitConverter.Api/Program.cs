using Microsoft.AspNetCore.Mvc;
using UnitConverter.Api.Middleware;
using UnitConverter.Api.Models;
using UnitConverter.Api.Services;
using UnitConverter.Api.Strategies;
using UnitConverter.Api.Strategies.Length;
using UnitConverter.Api.Strategies.Temperature;
using UnitConverter.Api.Strategies.Weight;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------
// Services
// ---------------------------------------------------------------------

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Unit Converter API",
        Version = "v1",
        Description = "A scalable unit conversion API supporting Length, Temperature, and Weight conversions."
    });
});

// Register each conversion strategy. Adding a new category (e.g., Volume,
// Speed, Pressure) is as simple as creating a new IConversionStrategy
// implementation and adding one line here - no other code changes needed.
builder.Services.AddSingleton<IConversionStrategy, LengthConversionStrategy>();
builder.Services.AddSingleton<IConversionStrategy, TemperatureConversionStrategy>();
builder.Services.AddSingleton<IConversionStrategy, WeightConversionStrategy>();

builder.Services.AddSingleton<IUnitConversionService, UnitConversionService>();

// Ensure built-in [ApiController] validation errors use the same
// structured ErrorResponse shape as the global exception middleware.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(kvp => kvp.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        var response = new ErrorResponse
        {
            Code = "VALIDATION_ERROR",
            Message = "One or more fields are invalid.",
            Details = errors,
            TraceId = context.HttpContext.TraceIdentifier
        };

        return new BadRequestObjectResult(response);
    };
});

var app = builder.Build();

// ---------------------------------------------------------------------
// Middleware pipeline
// ---------------------------------------------------------------------

// Global exception handling must be first so it can catch errors from
// every subsequent middleware/controller.
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Unit Converter API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Exposed for WebApplicationFactory-based integration tests.
public partial class Program { }
