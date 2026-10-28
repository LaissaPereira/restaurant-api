using RestaurantApi.Models;
using RestaurantApi.Enums;
namespace RestaurantApi.Services.Interfaces;

public interface IOrderService
{
    
    Task<Order?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<Order>> ListAsync(string? search = null);

    Task<Order?> CreateAsync(Guid menuId, int quantity, OrderStatus orderStatus = OrderStatus.Pending);

    Task<Order?> UpdateAsync(Guid id, Guid? menuId, int? quantity);

    Task<Order?> UpdateStatusAsync(Guid id, OrderStatus status);

    Task<bool> DeleteAsync(Guid id);
}