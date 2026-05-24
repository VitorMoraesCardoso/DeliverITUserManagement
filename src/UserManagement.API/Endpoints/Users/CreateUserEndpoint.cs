using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.DTOs;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagement.API.Endpoints.Users;

public class CreateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/users", HandleAsync)
            .WithName("CreateUser")
            .WithTags("Users")
            .Produces<UserResponseDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateUserDto dto,
        [FromServices] IUserRepository  userRepository,
        [FromServices] IValidator<CreateUserDto> validator,
        [FromServices] ILogger<CreateUserEndpoint> logger)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for CreateUserDto: {Errors}", validationResult.Errors);
            return Results.BadRequest(validationResult.Errors);
        }

        var emailAlreadyExists = await userRepository.EmailExistsAsync(dto.Email);
        
        if (emailAlreadyExists)
        {
            logger.LogWarning("Email already exists: {Email}", dto.Email);
            return Results.Conflict(new { Message = "Email already exists." });
        }

        try
        {
            var user = new User(dto.Nome, dto.Email, dto.DataNascimento);
            await userRepository.AddAsync(user);
            await userRepository.SaveChangesAsync();
            
            var response = new UserResponseDto(
                user.Id,
                user.Nome,
                user.Email,
                user.DataNascimento,
                user.DataCriacao,
                user.DataEdicao);

            return Results.Created($"/users/{user.Id}", response);
        }
        catch (DbUpdateException)
        {
            if (emailAlreadyExists)
            {
                logger.LogWarning("Email already exists (DB constraint violation): {Email}", dto.Email);
                return Results.Conflict(new { Message = "Email already exists." });
            }
            throw;
        }
        catch (ArgumentException e)
        {
            return Results.BadRequest(new { e.Message });
        }
    }
}