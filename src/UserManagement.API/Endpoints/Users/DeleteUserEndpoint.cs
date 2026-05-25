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
            logger.LogError("User with id ({id}) not found", id);
            return Results.NotFound(new { Message = "User not found with this id.", id });
        }
        
        await userRepository.DeleteAsync(user);
        logger.LogInformation("User with id ({userId}) successfully deleted", id);
        await userRepository.SaveChangesAsync();
        
        return Results.NoContent();
    }
}