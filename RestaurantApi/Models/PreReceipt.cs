
namespace RestaurantApi.Models;

public class PreReceipt
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }
    public string? OrderName => Order?.Menu?.Name;
    public int? Quantity => Order?.Quantity;
    public decimal? UnitPrice => Order?.UnitPrice;

    public decimal TotalPrice => Order?.TotalPrice ?? 0;
    public DateTimeOffset PreReceiptDate { get; set; }
}