using RestaurantApi.DTOs.Orders;
using RestaurantApi.Filters;
using RestaurantApi.Models;
using RestaurantApi.Services.Interfaces;

namespace RestaurantApi.Endpoints;

public static class OrderEndpoints
{
    public static RouteGroupBuilder MapOrderEndpoints( this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/orders").WithTags("Orders");
        // GET all orders
        group.MapGet("/", async(string? search, DateOnly? startDate, DateOnly? endDate, IOrderService service) =>
        {
            if (startDate is not null && endDate is not null && startDate > endDate)
            {
                return Results.BadRequest(new { Message = "startDate must be on or before endDate." });
            }

            var orders = await service.ListAsync(search, startDate, endDate);
            return Results.Ok(orders.Select(ToResponse));
           
        })
        .WithSummary("List all orders")
        .WithDescription("Retrieves a list of all available orders.");

        // GET order by ID
        group.MapGet("/{id:guid}", async(Guid id, IOrderService service) =>
        {
            var order = await service.GetByIdAsync(id);
            if(order is null)
            {
                return Results.NotFound(new { Message = $"Order with ID {id} not found." });
            }
            return Results.Ok(ToResponse(order));
        })
        .AddEndpointFilter<RejectEmptyIdFilter>()
        .WithSummary("Get order by ID")
        .WithDescription("Retrieves an order by its unique ID.");

        // POST create a new order
        group.MapPost("/", async(CreateOrderRequestDto dto, IOrderService service) =>
        {
            var order = await service.CreateAsync(dto);
            if (order is null)
            {
                return Results.BadRequest(new { Message = $"Menu with ID {dto.MenuId} not found." });
            }
            return Results.Created($"/orders/{order.Id}", ToResponse(order));
        })
        .WithSummary("Create a new order")
        .WithDescription("Creates a new order with the provided details.");

        // PATCH update an existing order
        group.MapPatch("/{id:guid}", async(Guid id, UpdateOrderRequestDto dto, IOrderService service) =>
        {
            var order = await service.UpdateAsync(id, dto);
            if (order is null)
            {
                return Results.NotFound(new { Message = $"Order with ID {id} not found." });
            }
            return Results.Ok(ToResponse(order));
        })
        .AddEndpointFilter<RejectEmptyIdFilter>()
        .WithSummary("Update an order")
        .WithDescription("Updates the details of an existing order by its ID.");

        // DELETE an order
        group.MapDelete("/{id:guid}", async(Guid id, IOrderService service) =>
        {
            var deleted = await service.DeleteAsync(id);

            return deleted 
                ? Results.NoContent()
                : Results.NotFound(new { Message = $"Order with ID {id} not found." });
        })
        .AddEndpointFilter<RejectEmptyIdFilter>()
        .WithSummary("Delete an order")
        .WithDescription("Deletes an existing order by its ID.");

        return group;
    }

    // Helper method to convert Order to OrderResponseDto
    private static OrderResponseDto ToResponse(Order order) => new(
        order.Id,
        order.MenuId,
        order.OrderName,
        order.Quantity,
        order.UnitPrice,
        order.TotalPrice,
        order.Status,
        order.Date);
}
