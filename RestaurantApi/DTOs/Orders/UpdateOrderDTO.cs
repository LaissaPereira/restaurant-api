using System.ComponentModel.DataAnnotations;

namespace RestaurantApi.DTOs.Orders;

public record UpdateOrderDTO(
    Guid? MenuId,

    [property: Range(1, int.MaxValue)]
    int? Quantity
);