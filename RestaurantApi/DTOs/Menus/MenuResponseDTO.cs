namespace RestaurantApi.DTOs.Menus;

public record MenuResponseDTO(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);