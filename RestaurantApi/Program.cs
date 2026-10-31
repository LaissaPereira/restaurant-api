using System.Text.Json.Serialization;
using RestaurantApi.Infrastructure;
using RestaurantApi.Services.Interfaces;
using RestaurantApi.Services;
using RestaurantApi.Endpoints;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// DOC's AND VALIDATION
builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.AddProblemDetails();

// STRING <=> ENUM CONVERTER (e.g., OrderStatus)
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
});


// DATABASE CONTEXT
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// APP SERVICES
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReportService, ReportService>();


// CREATE THE APP
var app = builder.Build();

// MIGRATE DATABASE ON STARTUP
// CREATE A SCOPE TO APPLY MIGRATIONS
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// MAP ENDPOINTS - Routes for the API
app.MapOpenApi();
app.MapScalarApiReference();
// MAP MENU ENDPOINTS
app.MapMenuEndpoints();
// MAP ORDER ENDPOINTS
app.MapOrderEndpoints();
// MAP REPORT ENDPOINTS
app.MapReportEndpoints();



// DOC's
app.Logger.LogInformation("OpenAPI JSON: http://localhost:5262/openapi/v1.json");
app.Logger.LogInformation("Scalar UI: http://localhost:5262/scalar/v1");


app.Run();
