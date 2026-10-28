using System.ComponentModel.DataAnnotations;

namespace RestaurantApi.DTOs.PreReceipts;

public record CreatePreReceiptDTO(
    [property: Required]
    Guid OrderId
);