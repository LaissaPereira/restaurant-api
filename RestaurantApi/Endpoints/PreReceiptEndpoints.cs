using RestaurantApi.DTOs.PreReceipts;
using RestaurantApi.Filters;
using RestaurantApi.Services.Interfaces;


namespace RestaurantApi.Endpoints;

public static class PreReceiptEndpoints
{
    public static RouteGroupBuilder MapPreReceiptEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/preReceipts").WithTags("PreReceipts");

        group.MapGet("/", async(IPreReceiptService service) =>
        {
            var preReceipts = await service.ListAsync();

            var response = preReceipts.Select(preReceipt => new PreReceiptResponseDTO(
                preReceipt.Id,
                preReceipt.OrderId,
                preReceipt.OrderName,
                preReceipt.Quantity,
                preReceipt.UnitPrice,
                preReceipt.TotalPrice,
                preReceipt.PreReceiptDate
            ));
            return Results.Ok(response);
        });

        group.MapGet("/{id:guid}", async(Guid id, IPreReceiptService service) =>
        {
            var preReceipt = await service.GetByIdAsync(id);
            if (preReceipt is null)
            {
                return Results.NotFound(new { Message = $"PreReceipt with ID {id} not found." });
            }
            var response = new PreReceiptResponseDTO(
                preReceipt.Id,
                preReceipt.OrderId,
                preReceipt.OrderName,
                preReceipt.Quantity,
                preReceipt.UnitPrice,
                preReceipt.TotalPrice,
                preReceipt.PreReceiptDate
            );
            return Results.Ok(response);
        })
        .AddEndpointFilter<RejectEmptyIdFilter>();

        group.MapPost("/", async(CreatePreReceiptDTO dto, IPreReceiptService service) =>
        {
            var preReceipt = await service.CreateAsync(dto.OrderId);
            if (preReceipt is null)
            {
                return Results.BadRequest(new { Message = $"Order with ID {dto.OrderId} not found." });
            }

            var response = new PreReceiptResponseDTO(
                preReceipt.Id,
                preReceipt.OrderId,
                preReceipt.OrderName,
                preReceipt.Quantity,
                preReceipt.UnitPrice,
                preReceipt.TotalPrice,
                preReceipt.PreReceiptDate
            );
            return Results.Created($"/preReceipts/{preReceipt.Id}", response);
        });

        group.MapDelete("/{id:guid}", async(Guid id, IPreReceiptService service) =>
        {
            var deleted = await service.DeleteAsync(id);

            return deleted
                ? Results.NoContent()
                : Results.NotFound(new { Message = $"PreReceipt with ID {id} not found." });
        })
        .AddEndpointFilter<RejectEmptyIdFilter>();

        return group;
    }
}
