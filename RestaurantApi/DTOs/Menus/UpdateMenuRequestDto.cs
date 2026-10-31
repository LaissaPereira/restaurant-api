using System.ComponentModel.DataAnnotations;
namespace RestaurantApi.DTOs.Menus;

public record UpdateMenuRequestDto(
    [property: StringLength(100, MinimumLength = 2)]
    string? Name,

    [property: StringLength(500, MinimumLength = 5)]
    string? Description,

    [property: Range(typeof(decimal), "0.01", "1000000", ParseLimitsInInvariantCulture = true  )]
    decimal? Price
);
