using System.ComponentModel.DataAnnotations;
namespace RestaurantApi.DTOs.Menus;

public record UpdateMenuDTO(
    [property: StringLength(100, MinimumLength = 2)]
    string? Name,

    [property: StringLength(500, MinimumLength = 5)]
    string? Description,

    [property: Range(0.01, double.MaxValue)]
    decimal? Price
);
