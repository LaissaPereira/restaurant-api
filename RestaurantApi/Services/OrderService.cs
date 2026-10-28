using RestaurantApi.Models;
using RestaurantApi.Enums;
using RestaurantApi.Services.Interfaces;
using RestaurantApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace RestaurantApi.Services;

public sealed class OrderService : IOrderService
{
    private readonly ApplicationDbContext _dbContext;

    public OrderService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Orders
            .Include(o => o.Menu)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IReadOnlyList<Order>> ListAsync(string? search = null)
    {
        var query = _dbContext.Orders.Include(o => o.Menu).AsQueryable();

        if(!string.IsNullOrEmpty(search))
        {
            var trimmedSearch = search.Trim();
            query = query.Where(o => o.Menu != null && o.Menu.Name.Contains(trimmedSearch));
        }

        var orders = await query.ToListAsync();
        return orders;
    }

    public async Task<Order?> CreateAsync(Guid menuId, int quantity, OrderStatus orderStatus = OrderStatus.Pending)
    {
       var menu = await _dbContext.Menus.FindAsync(menuId);
       if (menu is null)
       {
           throw new BadHttpRequestException(
                $"Menu with ID '{menuId}' was not found",
                StatusCodes.Status400BadRequest);
       }

       var order = new Order
       {
           Id = Guid.NewGuid(),
           MenuId = menuId,
           Menu = menu,
           Quantity = quantity,
           OrderStatus = orderStatus,
           CreatedAt = DateTimeOffset.UtcNow,
           UpdatedAt = DateTimeOffset.UtcNow
       };

       _dbContext.Orders.Add(order);
       await _dbContext.SaveChangesAsync();
       return order;

    }

    public async Task<Order?> UpdateAsync(Guid id, Guid? menuId, int? quantity)
    {
        var order = await _dbContext.Orders
            .Include(o => o.Menu)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            return null;
        }

        if (menuId is not null && menuId.Value != order.MenuId)
        {
            var menu = await _dbContext.Menus.FindAsync(menuId.Value);
            if (menu is null)
            {
                throw new BadHttpRequestException(
                    $"Menu with ID '{menuId}' was not found",
                    StatusCodes.Status400BadRequest);
            }
            order.MenuId = menuId.Value;
            order.Menu = menu;
        }
        if (quantity is not null)
        {
            order.Quantity = quantity.Value;
        }
        order.UpdatedAt = DateTimeOffset.UtcNow;
        await _dbContext.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> UpdateStatusAsync(Guid id, OrderStatus status)
    {
        var order = await _dbContext.Orders
            .Include(o => o.Menu)
            .FirstOrDefaultAsync(o => o.Id == id);
        if (order is null)
        {
            return null;
        }
        order.OrderStatus = status;
        order.UpdatedAt = DateTimeOffset.UtcNow;
        await _dbContext.SaveChangesAsync();
        return order;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var order = await _dbContext.Orders.FindAsync(id);
        if (order is null)
        {
            return false;
        }
        _dbContext.Orders.Remove(order);
        await _dbContext.SaveChangesAsync();
        return true;
    }

}