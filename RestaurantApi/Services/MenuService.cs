using RestaurantApi.Models;
using RestaurantApi.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using RestaurantApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;


namespace RestaurantApi.Services;


public sealed class MenuService : IMenuService
{
    private readonly ApplicationDbContext _dbContext;

    public MenuService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Menu?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Menus.FindAsync(id);
    }

    public async Task<IReadOnlyList<Menu>> ListAsync()
    {
        return await _dbContext.Menus.ToListAsync();
    }

    public async Task<Menu> CreateAsync(string name, string description, decimal price)
    {
        if(await _dbContext.Menus.AnyAsync(m => m.Name == name))
        {
            throw new InvalidOperationException($"A menu with the name '{name}' already exists.");
        }

        var menu = new Menu
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.Menus.Add(menu);
        await _dbContext.SaveChangesAsync();
        return menu;
    }

    public async Task<Menu?> UpdateAsync(Guid id, string? name, string? description, decimal? price)
    {
        var menu = await _dbContext.Menus.FindAsync(id);

        if (menu is null)
        {
            return null;
        }

        if (name is not null)
        {
            menu.Name = name;
        }
        if (description is not null)
        {
            menu.Description = description;
        }
        if (price is not null)
        {
            menu.Price = price.Value;
        }
        menu.UpdatedAt = DateTimeOffset.UtcNow;
        await _dbContext.SaveChangesAsync();
        return menu;
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var menu = await _dbContext.Menus.FindAsync(id);
        if (menu is null)
        {
            return false;
        }

        var hasOrders = await _dbContext.Orders.AnyAsync(o => o.MenuId == id);
        if (hasOrders)
        {
            throw new BadHttpRequestException(
                $"Menu with ID '{id}' cannot be deleted because it has existing orders.",
                StatusCodes.Status409Conflict);
        }

        _dbContext.Menus.Remove(menu);
        await _dbContext.SaveChangesAsync();
        return true;
    }

}
    