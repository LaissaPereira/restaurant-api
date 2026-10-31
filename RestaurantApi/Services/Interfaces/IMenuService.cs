using RestaurantApi.Models;
using RestaurantApi.DTOs.Menus;

namespace RestaurantApi.Services.Interfaces;

public interface IMenuService
{
    Task<IReadOnlyList<Menu>> ListAsync();
    Task<Menu?> GetByIdAsync(Guid id);
    Task<Menu> CreateAsync(CreateMenuRequestDto createMenuRequest);
    Task<Menu?> UpdateAsync(Guid id, UpdateMenuRequestDto updateMenuRequest);
    Task<bool> DeleteAsync(Guid id);
}