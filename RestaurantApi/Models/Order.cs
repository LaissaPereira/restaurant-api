using RestaurantApi.Enums;

namespace RestaurantApi.Models;

public class Order
{
    public Guid Id { get; set; }
    public Guid MenuId { get; set; }
    public Menu? Menu { get; set; }
    public string? OrderName => Menu?.Name;
    public int Quantity { get; set; }
    public decimal UnitPrice => Menu?.Price ?? 0;
    public decimal TotalPrice => UnitPrice * Quantity;
    public OrderStatus OrderStatus { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public PreReceipt? PreReceipt { get; set; }
}
