# Unit Conversion API

This is an ASP.NET Core Web API designed to handle numerical conversions between different units of measurement. Out of the box, it supports Length, Temperature, and Weight/Mass. 

I designed the architecture with scalability in mind. The goal was to ensure that as the application grows to support hundreds of new units or conversion categories, the core logic doesn't turn into a massive, unmaintainable `switch` statement.

---

## 1. Project Structure

The codebase is split into the main API application and a dedicated unit testing project.

```
UnitConverter.sln
├── src/
│   └── UnitConverter.Api/
│       ├── Controllers/
│       │   └── ConversionsController.cs
│       ├── Middleware/
│       │   └── GlobalExceptionHandlingMiddleware.cs
│       ├── Models/
│       │   ├── ConversionCategory.cs
│       │   ├── ConversionRequest.cs
│       │   └── ConversionResponse.cs
│       ├── Exceptions/
│       │   └── ConversionExceptions.cs
│       ├── Validation/
│       │   └── FiniteNumberAttribute.cs
│       ├── Strategies/
│       │   ├── IConversionStrategy.cs
│       │   ├── LinearFactorConversionStrategy.cs
│       │   ├── Length/LengthConversionStrategy.cs
│       │   ├── Weight/WeightConversionStrategy.cs
│       │   └── Temperature/TemperatureConversionStrategy.cs
│       ├── Services/
│       │   ├── IUnitConversionService.cs
│       │   └── UnitConversionService.cs
│       └── Program.cs
└── tests/
    └── UnitConverter.Tests/
        ├── Strategies/
        │   ├── LengthConversionStrategyTests.cs
        │   ├── WeightConversionStrategyTests.cs
        │   └── TemperatureConversionStrategyTests.cs
        └── Services/
            └── UnitConversionServiceTests.cs
```

---

## 2. How to Run It Locally

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed on your machine.

### Getting Started

1. **Clone or download the repository** and open a terminal in the root folder (where the `UnitConverter.sln` file lives).
2. **Build the project** to restore dependencies:
   `dotnet build`
3. **Run the API:**
   `dotnet run --project src/UnitConverter.Api`
   *(Note: If you run into local environment issues, you can also run it self-contained using `dotnet run --project src/UnitConverter.Api --self-contained true -r win-x64`)*

4. **Test the API via Swagger UI:**
   Once the console says the app is listening, open your web browser and navigate to the local port provided (typically `http://localhost:5080/swagger`). This will open an interactive interface where you can quickly test conversions.

### Running the Unit Tests
To verify the math and validation logic, run the test suite from the root directory:
`dotnet test`

---

## 3. How to Use the API

The application exposes a single endpoint that handles all categories.

**`POST /api/v1/conversions`**

### Example Request
{
  "category": "Temperature",
  "fromUnit": "Celsius",
  "toUnit": "Fahrenheit",
  "value": 100
}

### Example Success Response (200 OK)
{
  "category": "Temperature",
  "fromUnit": "Celsius",
  "toUnit": "Fahrenheit",
  "inputValue": 100,
  "convertedValue": 212
}

---

## 4. Architecture & Design Decisions

Because the requirements mentioned future-proofing the application for hundreds of units, I made a few specific architectural choices to keep the codebase clean and extensible.

### The Strategy Pattern
Instead of writing a massive nested `if/else` block inside the controller to figure out how to convert meters to feet, I implemented the Strategy Pattern. 

Every category (Length, Weight, Temperature) has its own dedicated class that implements `IConversionStrategy`. When a request comes in, the `UnitConversionService` automatically routes it to the correct strategy. 
* **Why this helps:** If we need to add a new category later (like Volume or Speed), we just create a new strategy class. We don't have to touch or modify the controller or the service dispatcher at all. 

### Linear vs. Non-Linear Conversions
I noticed that Length and Weight share the exact same mathematical behavior: everything can be calculated by multiplying against a base unit (like meters or kilograms). To prevent code duplication, I created an abstract `LinearFactorConversionStrategy`. To add a new length or weight unit, a developer just needs to add one single line to a dictionary with its base multiplier. 

Temperature, however, doesn't work that way (because of different zero-points like absolute zero). So, it bypasses the linear base class and implements the math directly, keeping the abstractions honest.

### Preparing for a Database
Currently, the unit factors are hardcoded in-memory as permitted by the requirements. However, because everything is decoupled behind the `IConversionStrategy` interface, moving to a database (like PostgreSQL or CosmosDB) in the future will be seamless. We would just inject a new database-backed strategy without needing to rewrite any of the API endpoints.

### Global Error Handling & Validation
To keep the controllers clean, I implemented a custom `GlobalExceptionHandlingMiddleware`. Whether a user submits a negative weight, a unit that doesn't exist, or a completely invalid JSON payload, the middleware catches it and formats it into a standardized, readable JSON error response (usually a `400 Bad Request`). This ensures internal server stack traces never leak to the client.