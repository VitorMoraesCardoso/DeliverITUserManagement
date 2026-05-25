using Microsoft.AspNetCore.Mvc;
using UserManagement.API.DTOs;
using UserManagement.Domain.Interfaces;

namespace UserManagement.API.Endpoints.Users;

public class GetUserPagedEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users", HandleAsync)
            .WithName("GetUsersPaged")
            .WithTags("Users")
            .Produces<PagedResponseDto<UserResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }
    
    private static async Task<IResult> HandleAsync(
        [FromServices] IUserRepository userRepository,
        [FromServices] ILogger<GetUserPagedEndpoint> logger,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page <= 0 || pageSize <= 0)
        {
            page = 1;
            pageSize = 10;
        }
        
        var users = await userRepository.GetPagedAsync(page, pageSize);
        var totalCount = await userRepository.GetTotalCountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var usersDtos = users.Select(user => new UserResponseDto(
            user.Id,
            user.Nome,
            user.Email,
            user.DataNascimento,
            user.DataCriacao,
            user.DataEdicao)); 
        
        var response = new PagedResponseDto<UserResponseDto>(
            usersDtos,
            totalCount,
            page,
            pageSize,
            totalPages);
        
        return Results.Ok(response);
    }
}