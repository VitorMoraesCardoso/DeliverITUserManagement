using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.DTOs;
using UserManagement.Domain.Interfaces;

namespace UserManagement.API.Endpoints.Users;

public class UpdateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
            app.MapPut("/users/{id:int}", HandleAsync)
                .WithName("UpdateUser")
                .WithTags("Users")
                .Produces<UserResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound);
        }
    
        private static async Task<IResult> HandleAsync(
            int id,
            [FromBody] UpdateUserDto dto,
            [FromServices] IUserRepository userRepository,
            [FromServices] IValidator<UpdateUserDto> validator,
            [FromServices] ILogger<UpdateUserEndpoint> logger)
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                logger.LogWarning("Validation failed for EditUserDto: {Errors}", validationResult.Errors);
                return Results.BadRequest(validationResult.Errors);
            }
    
            var user = await userRepository.GetByIdAsync(id);
            
            if (user == null)
            {
                return Results.NotFound(new { Message = "User not found with this id.", id });
            }
    
            var emailAlreadyExists = await userRepository.EmailExistsAsync(dto.Email.ToLower(), excludeUserId: id);
            
            if (emailAlreadyExists)
            {
                logger.LogWarning("Email already exists: {Email}", dto.Email);
                return Results.Conflict(new
                {
                    Message = "E-mail already exists. Try another e-mail or create a new user", dto.Email
                });
            }
    
            user.Update(dto.Nome, dto.Email, dto.DataNascimento);
            
            try
            {
                await userRepository.SaveChangesAsync();

                var response = new UserResponseDto(
                    user.Id,
                    user.Nome,
                    user.Email,
                    user.DataNascimento,
                    user.DataCriacao,
                    user.DataEdicao);

                return Results.Ok(response);
            }
            catch (ArgumentException e)
            {
                logger.LogError(e, "Argument exception: {Message}", e.Message);
                return Results.BadRequest(new { e.Message });
            }
    }
}