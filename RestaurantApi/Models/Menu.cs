namespace RestaurantApi.Models;

public class Menu
{
    public Guid Id { get; set;}
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}