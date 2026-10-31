using RestaurantApi.Models;

namespace RestaurantApi.DTOs.Reports;

public record SummaryReportResponseDto(
    DateOnly StartDate,
    DateOnly EndDate,
    decimal TotalRevenue,
    int TotalOrders,
    Dictionary<OrderStatus, int> OrderStatusSummary 
);