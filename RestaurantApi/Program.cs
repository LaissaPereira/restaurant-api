using System.Text.Json.Serialization;
using RestaurantApi.Infrastructure;
using RestaurantApi.Services.Interfaces;
using RestaurantApi.Services;
using RestaurantApi.Endpoints;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;




var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.AddProblemDetails();

// Serialize enums (e.g. OrderStatus) as their string name (e.g. "Pending") instead of the
// underlying int, both for responses and for incoming request bodies.
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPreReceiptService, PreReceiptService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.MapOpenApi();
app.MapScalarApiReference();


app.MapMenuEndpoints();
app.MapOrderEndpoints();
app.MapPreReceiptEndpoints();

app.Run();
