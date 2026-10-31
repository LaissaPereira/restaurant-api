using RestaurantApi.Models;
using RestaurantApi.DTOs.Orders;
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

    public async Task<IReadOnlyList<Order>> ListAsync(string? search = null, DateOnly? startDate = null, DateOnly? endDate = null)
    {
        var query = _dbContext.Orders.Include(o => o.Menu).AsQueryable();

        if (startDate is not null)
        {
            query = query.Where(o => o.Date >= startDate.Value);
        }

        if (endDate is not null)
        {
            query = query.Where(o => o.Date <= endDate.Value);
        }

        // Most recent orders first
        var orders = await query.OrderByDescending(o => o.Date).ToListAsync();

        // Status is stored as an integer, so the text search runs in memory 
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            orders = orders
                .Where(o => o.Status.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                    || (o.OrderName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();

            //Example: /orders?search=pizza&startDate=2026-10-28&endDate=2026-10-30
        }

        return orders;
    }

     public async Task<Order?> GetByIdAsync(Guid id)
    {
        //Operation to get an order by its ID
        return await _dbContext.Orders
            .Include(o => o.Menu)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> CreateAsync( CreateOrderRequestDto createOrderRequest)
    {
        //Check if the menu exists for the given menu ID
       var menu = await _dbContext.Menus.FindAsync(createOrderRequest.MenuId);
       if (menu is null)
       {
           throw new BadHttpRequestException(
                $"Menu with ID '{createOrderRequest.MenuId}' was not found",
                StatusCodes.Status400BadRequest);
       }

        //Create a new order instance with the provided details and default status as Pending
       var order = new Order
       {
           Id = Guid.NewGuid(),
           MenuId = createOrderRequest.MenuId,
           Menu = menu,
           Quantity = createOrderRequest.Quantity,
           Status = OrderStatus.Pending,
           Date = createOrderRequest.Date!.Value,
        
           CreatedAt = DateTimeOffset.UtcNow,
           UpdatedAt = DateTimeOffset.UtcNow
       };

       _dbContext.Orders.Add(order);
       await _dbContext.SaveChangesAsync();
       return order;

    }

    public async Task<Order?> UpdateAsync(Guid id, UpdateOrderRequestDto updateOrderRequest)
    {
        //Check if the order exists for the given ID and include the associated menu
        var order = await _dbContext.Orders
            .Include(o => o.Menu)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            return null;
        }

        //Operation to update the menu if a new menu ID is provided and it is different from the current one
        if (updateOrderRequest.MenuId is not null && updateOrderRequest.MenuId.Value != order.MenuId)
        {
            var menu = await _dbContext.Menus.FindAsync(updateOrderRequest.MenuId.Value);
            if (menu is null)
            {
                throw new BadHttpRequestException(
                    $"Menu with ID '{updateOrderRequest.MenuId}' was not found",
                    StatusCodes.Status400BadRequest);
            }
            order.MenuId = updateOrderRequest.MenuId.Value;
            order.Menu = menu;
        }

        //Operation to update the quantity if provided and it is different from the current one
        if (updateOrderRequest.Quantity is not null)
        {
            order.Quantity = updateOrderRequest.Quantity.Value;
        }
        //Operation to update the status if provided and it is different from the current one
        if (updateOrderRequest.Status is not null)
        {
            order.Status = updateOrderRequest.Status.Value;
        }


        //Update the timestamp to reflect the modification
        order.UpdatedAt = DateTimeOffset.UtcNow;
        await _dbContext.SaveChangesAsync();
        return order;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        //Check if the order exists for the given ID before attempting deletion
        var order = await _dbContext.Orders.FindAsync(id);
        if (order is null)
        {
            return false;
        }
        //Operation to remove the order from the database
        _dbContext.Orders.Remove(order);
        await _dbContext.SaveChangesAsync();
        return true;
    }

}