using RestaurantApi.Models;
using RestaurantApi.DTOs.Orders;


namespace RestaurantApi.Services.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<Order>> ListAsync(string? search = null, DateOnly? startDate = null, DateOnly? endDate = null);
    
    Task<Order?> GetByIdAsync(Guid id);

    Task<Order?> CreateAsync(CreateOrderRequestDto createOrderRequest);

    Task<Order?> UpdateAsync(Guid id, UpdateOrderRequestDto updateOrderRequest);

    Task<bool> DeleteAsync(Guid id);
}