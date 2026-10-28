using RestaurantApi.DTOs.Orders;
using RestaurantApi.Filters;
using RestaurantApi.Services.Interfaces;

namespace RestaurantApi.Endpoints;

public static class OrderEndpoints
{
    public static RouteGroupBuilder MapOrderEndpoints( this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/orders").WithTags("Orders");

        group.MapGet("/", async(string? search, IOrderService service) =>
        {
            var orders = await service.ListAsync(search);

            var response = orders.Select(order => new OrderResponseDTO(
                order.Id,
                order.MenuId,
                order.OrderName,
                order.Quantity,
                order.UnitPrice,
                order.OrderStatus,
                order.CreatedAt,
                order.UpdatedAt

            ));
            return Results.Ok(response);
        });

        group.MapGet("/{id:guid}", async(Guid id, IOrderService service) =>
        {
            var order = await service.GetByIdAsync(id);
            if(order is null)
            {
                return Results.NotFound(new { Message = $"Order with ID {id} not found." });
            }
            var response = new OrderResponseDTO(
                order.Id,
                order.MenuId,
                order.OrderName,
                order.Quantity,
                order.UnitPrice,
                order.OrderStatus,
                order.CreatedAt,
                order.UpdatedAt
            );
            return Results.Ok(response);
        })
        .AddEndpointFilter<RejectEmptyIdFilter>();

        group.MapPost("/", async(CreateOrderDTO dto, IOrderService service) =>
        {
            var order = await service.CreateAsync(dto.MenuId, dto.Quantity);
            if (order is null)
            {
                return Results.BadRequest(new { Message = $"Menu with ID {dto.MenuId} not found." });
            }

            var response = new OrderResponseDTO(
                order.Id,
                order.MenuId,
                order.OrderName,
                order.Quantity,
                order.UnitPrice,
                order.OrderStatus,
                order.CreatedAt,
                order.UpdatedAt
            );
            return Results.Created($"/orders/{order.Id}", response);
        });


        group.MapPatch("/{id:guid}", async(Guid id, UpdateOrderDTO dto, IOrderService service) =>
        {
            var order = await service.UpdateAsync(id, dto.MenuId, dto.Quantity);
            if (order is null)
            {
                return Results.NotFound(new { Message = $"Order with ID {id} not found." });
            }
            var response = new OrderResponseDTO(
                order.Id,
                order.MenuId,
                order.OrderName,
                order.Quantity,
                order.UnitPrice,
                order.OrderStatus,
                order.CreatedAt,
                order.UpdatedAt
            );
            return Results.Ok(response);
        })
        .AddEndpointFilter<RejectEmptyIdFilter>();

        group.MapPatch("/{id:guid}/status", async(Guid id, UpdateOrderStatusDTO dto, IOrderService service) =>
        {
            var order = await service.UpdateStatusAsync(id, dto.Status);
            if (order is null)
            {
                return Results.NotFound(new { Message = $"Order with ID {id} not found." });
            }
            var response = new OrderResponseDTO(
                order.Id,
                order.MenuId,
                order.OrderName,
                order.Quantity,
                order.UnitPrice,
                order.OrderStatus,
                order.CreatedAt,
                order.UpdatedAt
            );
            return Results.Ok(response);
        })
        .AddEndpointFilter<RejectEmptyIdFilter>();

        group.MapDelete("/{id:guid}", async(Guid id, IOrderService service) =>
        {
            var deleted = await service.DeleteAsync(id);

            return deleted 
                ? Results.NoContent()
                : Results.NotFound(new { Message = $"Order with ID {id} not found." });
        })
        .AddEndpointFilter<RejectEmptyIdFilter>();

        return group;
    }
}