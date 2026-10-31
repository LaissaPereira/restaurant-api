using RestaurantApi.DTOs.Reports;

namespace RestaurantApi.Services.Interfaces;

public interface IReportService
{
    Task<SummaryReportResponseDto> GetSummaryReportAsync(DateOnly startDate, DateOnly endDate);
    
}
