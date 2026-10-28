using RestaurantApi.DTOs.Menus;
using RestaurantApi.Filters;
using RestaurantApi.Services.Interfaces;

namespace RestaurantApi.Endpoints;

public static class MenuEndpoints
{
    public static RouteGroupBuilder MapMenuEndpoints( this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/menus").WithTags("Menus");

        group.MapGet("/", async(IMenuService service) =>
        {
            var menus = await service.ListAsync();
            var response = menus.Select(menu => new MenuResponseDTO(
                menu.Id,
                menu.Name,
                menu.Description,
                menu.Price,
                menu.CreatedAt,
                menu.UpdatedAt
            ));
            return Results.Ok(response);
        });


        group.MapGet("/{id:guid}", async(Guid id, IMenuService service) =>
        {
            var menu = await service.GetByIdAsync(id);
            if (menu is null)
            {
                return Results.NotFound();
            }
            var response = new MenuResponseDTO(
                menu.Id,
                menu.Name,
                menu.Description,
                menu.Price,
                menu.CreatedAt,
                menu.UpdatedAt
            );
            return Results.Ok(response);
        })
        .AddEndpointFilter<RejectEmptyIdFilter>();

        group.MapPost("/", async(CreateMenuDTO dto, IMenuService service) =>
        {
            var menu = await service.CreateAsync(dto.Name, dto.Description, dto.Price);
            var response = new MenuResponseDTO(
                menu.Id,
                menu.Name,
                menu.Description,
                menu.Price,
                menu.CreatedAt,
                menu.UpdatedAt
            );
            return Results.Created($"/menus/{menu.Id}", response);
        });
        

        group.MapPatch("/{id:guid}", async(Guid id, UpdateMenuDTO dto, IMenuService service)=>
        {
            var menu = await service.UpdateAsync(id, dto.Name, dto.Description, dto.Price);
            if (menu is null)
            {
                return Results.NotFound(new { Message = $"Menu with ID {id} not found." });
            }
            var response = new MenuResponseDTO(
                menu.Id,
                menu.Name,
                menu.Description,
                menu.Price,
                menu.CreatedAt,
                menu.UpdatedAt
            );
            return Results.Ok(response);
        })
        .AddEndpointFilter<RejectEmptyIdFilter>();

        group.MapDelete("/{id:guid}", async(Guid id, IMenuService service) =>
        {
            var deleted = await service.DeleteAsync(id);

            return deleted 
                ? Results.NoContent()
                : Results.NotFound(new { Message = $"Menu with ID {id} not found." });
        })
        .AddEndpointFilter<RejectEmptyIdFilter>();

        return group;
    }
} 