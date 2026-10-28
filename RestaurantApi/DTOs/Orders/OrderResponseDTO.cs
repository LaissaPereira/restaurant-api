using RestaurantApi.Enums;
namespace RestaurantApi.DTOs.Orders;


public record OrderResponseDTO(
    Guid Id,
    Guid MenuId,
    string? OrderName,
    int Quantity,
    decimal UnitPrice,
    OrderStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);