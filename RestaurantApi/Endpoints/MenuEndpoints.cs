using RestaurantApi.DTOs.Menus;
using RestaurantApi.Models;
using RestaurantApi.Filters;
using RestaurantApi.Services.Interfaces;

namespace RestaurantApi.Endpoints;

public static class MenuEndpoints
{
    public static RouteGroupBuilder MapMenuEndpoints( this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/menus").WithTags("Menus");

        // GET all menus
        group.MapGet("/", async(IMenuService service) =>
        {
            var menus = await service.ListAsync();
            var response = menus.Select(ToResponseDto);
            return Results.Ok(response);
        }).WithSummary("List all menus")
        .WithDescription("Retrieves a list of all available menus.");


        // GET menu by ID
        group.MapGet("/{id:guid}", async(Guid id, IMenuService service) =>
        {
            var menu = await service.GetByIdAsync(id);
            if (menu is null)
            {
                return Results.NotFound();
            }
            var response = ToResponseDto(menu);
            return Results.Ok(response);
        })
        .AddEndpointFilter<RejectEmptyIdFilter>()
        .WithSummary("Get menu by ID")
        .WithDescription("Retrieves a menu by its unique ID.");

        // POST create a new menu
        group.MapPost("/", async(CreateMenuRequestDto dto, IMenuService service) =>
        {
            var menu = await service.CreateAsync(dto);
            var response = new MenuResponseDto(
                menu.Id,
                menu.Name,
                menu.Description,
                menu.Price,
                menu.CreatedAt,
                menu.UpdatedAt
            );
            return Results.Created($"/menus/{menu.Id}", response);
        })
        .WithSummary("Create a new menu")
        .WithDescription("Creates a new menu with the provided details.");
        
        // PATCH update an existing menu
        group.MapPatch("/{id:guid}", async(Guid id, UpdateMenuRequestDto dto, IMenuService service)=>
        {
            var menu = await service.UpdateAsync(id, dto);
            if (menu is null)
            {
                return Results.NotFound(new { Message = $"Menu with ID {id} not found." });
            }
            var response = ToResponseDto(menu);
            return Results.Ok(response);
        })
        .AddEndpointFilter<RejectEmptyIdFilter>()
        .WithSummary("Update a menu")
        .WithDescription("Updates the details of an existing menu by its ID.");

        // DELETE a menu
        group.MapDelete("/{id:guid}", async(Guid id, IMenuService service) =>
        {
            var deleted = await service.DeleteAsync(id);

            return deleted 
                ? Results.NoContent()
                : Results.NotFound(new { Message = $"Menu with ID {id} not found." });
        })
        .AddEndpointFilter<RejectEmptyIdFilter>()
        .WithSummary("Delete a menu")
        .WithDescription("Deletes an existing menu by its ID.");

        return group;
    }
        // Helper method to convert Menu to MenuResponseDto 
    private static MenuResponseDto ToResponseDto(Menu menu)
    {
        return new MenuResponseDto(
            menu.Id,
            menu.Name,
            menu.Description,
            menu.Price,
            menu.CreatedAt,
            menu.UpdatedAt
        );
        }
} 

