using RestaurantApi.Models;
using RestaurantApi.DTOs.Menus;
using RestaurantApi.Services.Interfaces;
using RestaurantApi.Infrastructure;
using Microsoft.EntityFrameworkCore;


namespace RestaurantApi.Services;


public sealed class MenuService : IMenuService
{
    private readonly ApplicationDbContext _dbContext;

    public MenuService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Menu>> ListAsync()
    {
        return await _dbContext.Menus.ToListAsync();
    }

    public async Task<Menu?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Menus.FindAsync(id);
    }

    public async Task<Menu> CreateAsync(CreateMenuRequestDto createMenuRequest)
    {
        // Validate: Check if a menu with the same name already exists in the database
        if(await _dbContext.Menus.AnyAsync(m => m.Name == createMenuRequest.Name))
        {
            throw new InvalidOperationException($"A menu with the name '{createMenuRequest.Name}' already exists.");
        }

        // Operation: Create a new menu instance with the provided details
        var menu = new Menu
        {
            Id = Guid.NewGuid(),
            Name = createMenuRequest.Name,
            Description = createMenuRequest.Description,
            Price = createMenuRequest.Price,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Menus.Add(menu);
        await _dbContext.SaveChangesAsync();
        return menu;
    }

    public async Task<Menu?> UpdateAsync(Guid id, UpdateMenuRequestDto updateMenuRequest)
    {
        var menu = await _dbContext.Menus.FindAsync(id);

        if (menu is null)
        {
            return null;
        }
        // Operation: Update the menu instance with the provided details
        if (updateMenuRequest.Name is not null)
        {
            menu.Name = updateMenuRequest.Name.Trim();
        }
        if (updateMenuRequest.Description is not null)
        {
            menu.Description = updateMenuRequest.Description.Trim();
        }
        if (updateMenuRequest.Price is not null)
        {
            menu.Price = updateMenuRequest.Price.Value;
        }
        menu.UpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();
        return menu;
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var menu = await _dbContext.Menus.FindAsync(id);
        // Validate: Check if the menu exists before attempting deletion
        if (menu is null)
        {
            return false;
        }

        // Validate: Check if the menu has existing orders before attempting deletion
        var hasOrders = await _dbContext.Orders.AnyAsync(o => o.MenuId == id);
        if (hasOrders)
        {
            throw new BadHttpRequestException(
                $"Menu with ID '{id}' cannot be deleted because it has existing orders.",
                StatusCodes.Status409Conflict);
        }

        // Operation: Remove the menu from the database
        _dbContext.Menus.Remove(menu);
        await _dbContext.SaveChangesAsync();
        return true;
    }

}
    