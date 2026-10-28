using RestaurantApi.Models;


namespace RestaurantApi.Services.Interfaces;

public interface IMenuService
{
    Task<Menu?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Menu>> ListAsync();
    Task<Menu> CreateAsync(string name, string description, decimal price);
    Task<Menu?> UpdateAsync(Guid id, string? name, string? description, decimal? price);
    Task<bool> DeleteAsync(Guid id);
}