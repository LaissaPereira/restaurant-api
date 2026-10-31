namespace RestaurantApi.DTOs.Menus;

public record MenuResponseDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);