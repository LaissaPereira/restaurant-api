using RestaurantApi.Models;
using RestaurantApi.DTOs.Reports;
using RestaurantApi.Services.Interfaces;
using RestaurantApi.Infrastructure;
using Microsoft.EntityFrameworkCore;


namespace RestaurantApi.Services;

public sealed class ReportService : IReportService
{
    private readonly ApplicationDbContext _dbContext;

    public ReportService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async  Task<SummaryReportResponseDto> GetSummaryReportAsync(DateOnly startDate, DateOnly endDate)
    {
        // Menu is needed because TotalPrice is computed from Menu.Price.
        var orders = await _dbContext.Orders
            .Include(order => order.Menu)
            .Where(order => order.Date >= startDate && order.Date <= endDate)
            .ToListAsync();

        var counts = orders
            .GroupBy(order => order.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        var summary = Enum.GetValues<OrderStatus>()
            .ToDictionary(status => status, status => counts.GetValueOrDefault(status));

        return new SummaryReportResponseDto(
            startDate,
            endDate,
            orders.Where(order => order.Status == OrderStatus.Completed).Sum(order => order.TotalPrice),
            orders.Count,
            summary);
    }
}