using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Interfaces;

namespace UserManagement.API.Endpoints.Users;

public class DeleteUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/users/{id:int}", HandleAsync)
            .WithName("DeleteUser")
            .WithTags("Users")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        int id,
        [FromServices] IUserRepository userRepository,
        [FromServices] ILogger<DeleteUserEndpoint> logger)
    {
        var user  = await userRepository.GetByIdAsync(id);
        if (user == null)
        {
            logger.LogError($"User with id {id} not found");
            return Results.NotFound(new { Message = $"User with id: {id} was not found." });
        }
        
        await userRepository.DeleteAsync(user);
        logger.LogInformation($"User with id {id} successfully deleted");
        await userRepository.SaveChangesAsync();
        
        return Results.NoContent();
    }
}