using RestaurantApi.Models;
using RestaurantApi.Enums;
using RestaurantApi.Services.Interfaces;
using RestaurantApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace RestaurantApi.Services;

public sealed class PreReceiptService : IPreReceiptService
{
    private readonly ApplicationDbContext _dbContext;

    public PreReceiptService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PreReceipt?> GetByIdAsync(Guid id)
    {
        return await _dbContext.PreReceipts
            .Include(pr => pr.Order)
            .ThenInclude(o => o!.Menu)
            .FirstOrDefaultAsync(pr => pr.Id == id);
    }

    public async Task<IReadOnlyList<PreReceipt>> ListAsync()
    {
        return await _dbContext.PreReceipts
            .Include(pr => pr.Order)
            .ThenInclude(o => o!.Menu)
            .ToListAsync();
    }

    public async Task<PreReceipt?> CreateAsync(Guid orderId)
    {
        var order = await _dbContext.Orders
            .Include(o => o.Menu)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        if (order is null)
        {
            throw new BadHttpRequestException(
                $"Order with ID '{orderId}' was not found.",
                StatusCodes.Status404NotFound);
        }
        if (order.OrderStatus != OrderStatus.Completed)
        {
            throw new BadHttpRequestException(
                $"Order with ID '{orderId}' must be Completed before a pre-receipt can be created. Current status: '{order.OrderStatus}'.",
                StatusCodes.Status400BadRequest);
        }

        var hasExistingPreReceipt = await _dbContext.PreReceipts.AnyAsync(pr => pr.OrderId == orderId);
        if (hasExistingPreReceipt)
        {
            throw new BadHttpRequestException(
                $"A pre-receipt already exists for order '{orderId}'.",
                StatusCodes.Status409Conflict);
        }

        var preReceipt = new PreReceipt
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Order = order,
            PreReceiptDate = DateTimeOffset.UtcNow
        };
        _dbContext.PreReceipts.Add(preReceipt);
        await _dbContext.SaveChangesAsync();
        return preReceipt;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var preReceipt = await _dbContext.PreReceipts.FindAsync(id);
        if (preReceipt is null)
        {
            return false;
        }
        _dbContext.PreReceipts.Remove(preReceipt);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
