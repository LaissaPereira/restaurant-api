using System.ComponentModel.DataAnnotations;
using RestaurantApi.Models;

namespace RestaurantApi.DTOs.Orders;

public record UpdateOrderRequestDto(
    Guid? MenuId,

    [property: Range(1, int.MaxValue)]
    int? Quantity,

    [property: EnumDataType(typeof(OrderStatus))]
    OrderStatus? Status


);