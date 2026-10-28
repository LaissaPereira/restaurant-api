namespace RestaurantApi.DTOs.PreReceipts;

public record PreReceiptResponseDTO(
    Guid Id,
    Guid OrderId,
    string? OrderName,
    int? Quantity,
    decimal? UnitPrice,
    decimal TotalPrice,
    DateTimeOffset ReceiptDate
);