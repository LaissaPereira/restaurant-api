using System.ComponentModel.DataAnnotations;

namespace RestaurantApi.DTOs.Menus;

public record CreateMenuDTO(
    [property: Required] 
    [property: StringLength(100, MinimumLength = 2)]
    string Name,

    [property: Required] 
    [property: StringLength(500, MinimumLength = 5)]
    string Description,

    [property: Required] 
    [property: Range(0.01, double.MaxValue)]
    decimal Price
);

