using System.ComponentModel.DataAnnotations;


namespace RestaurantApi.DTOs.Orders;

public record CreateOrderDTO(
    [property: Required] 
    Guid MenuId,

    [property: Required] 
    [property: Range(1, int.MaxValue)]
    int Quantity

);