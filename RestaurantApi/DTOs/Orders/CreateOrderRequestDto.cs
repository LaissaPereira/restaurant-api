using System.ComponentModel.DataAnnotations;
using RestaurantApi.Models;

namespace RestaurantApi.DTOs.Orders;

public record CreateOrderRequestDto(
    [property: Required] 
    Guid MenuId,

    [property: Required, EnumDataType(typeof(OrderStatus))] 
    OrderStatus? Status,

    [property: Required] 
    [property: Range(1, int.MaxValue)]
    int Quantity,

    [property: Required]
    DateOnly? Date

);