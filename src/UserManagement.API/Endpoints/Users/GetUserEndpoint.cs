using Microsoft.AspNetCore.Mvc;
using UserManagement.API.DTOs;
using UserManagement.Domain.Interfaces;

namespace UserManagement.API.Endpoints.Users;

public class GetUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{id:int}", HandleAsync)
            .WithName("GetUserById")
            .WithTags("Users")
            .Produces<UserResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        int id,
        [FromServices] IUserRepository userRepository,
        [FromServices] ILogger<GetUserEndpoint> logger)
    {
        var user = await userRepository.GetByIdAsync(id);

        if (user == null)
        {
            return Results.NotFound(new { Message = $"User with id: {id} was not found." });
        }
        
        var response = new UserResponseDto(
            user.Id,
            user.Nome,
            user.Email,
            user.DataNascimento,
            user.DataCriacao,
            user.DataEdicao);
        
        return Results.Ok(response);
    }
}