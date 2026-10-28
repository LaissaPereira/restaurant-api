using System.ComponentModel.DataAnnotations;
using RestaurantApi.Enums;

namespace RestaurantApi.DTOs.Orders;

public record UpdateOrderStatusDTO(
    [property: Required]
    [property: EnumDataType(typeof(OrderStatus))]
    OrderStatus Status
);
