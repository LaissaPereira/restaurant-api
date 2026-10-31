using RestaurantApi.Services.Interfaces;

namespace RestaurantApi.Endpoints;

public static class ReportEndpoints
{
    public static RouteGroupBuilder MapReportEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/reports").WithTags("Reports");

        // GET summary report
        group.MapGet("/summary", async (DateOnly startDate, DateOnly endDate, IReportService service) =>
        {
            if (startDate > endDate)
            {
                return Results.BadRequest(new { Message = "Start Date must be on or before the end date." });
            }

            return Results.Ok(await service.GetSummaryReportAsync(startDate, endDate));
        })
        .WithSummary("Get summary report")
        .WithDescription("Retrieves a summary report for the specified date range.");

        return group;
    }
}
