using RestaurantApi.Models;

namespace RestaurantApi.DTOs.Orders;

public record OrderResponseDto(
    Guid Id,
    Guid MenuId,
    string? OrderName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    OrderStatus Status,
    DateOnly Date
);