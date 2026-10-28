using RestaurantApi.Models;

namespace RestaurantApi.Services.Interfaces;

public interface IPreReceiptService
{
    
    Task<PreReceipt?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<PreReceipt>> ListAsync();

    Task<PreReceipt?> CreateAsync(Guid orderId);

    Task<bool> DeleteAsync(Guid id);

}