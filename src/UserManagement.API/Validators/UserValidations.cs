using FluentValidation;
using UserManagement.API.DTOs;

namespace UserManagement.API.Validators;

public class CreateUserDtoValidation : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidation()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(50).WithMessage("O nome deve conter no máximo 50 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório.")
            .MaximumLength(254).WithMessage("O email deve conter no máximo 254 caracteres.")
            .EmailAddress().WithMessage("O email deve ser um endereço de email válido.");

        RuleFor(x => x.DataNascimento)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
            .LessThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("A data de nascimento não pode ser no futuro.");
    }
}

public class UpdateUserDtoValidation : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidation()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(50).WithMessage("O nome deve conter no máximo 50 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório.")
            .MaximumLength(254).WithMessage("O email deve conter no máximo 254 caracteres.")
            .EmailAddress().WithMessage("O email deve ser um endereço de email válido.");

        RuleFor(x => x.DataNascimento)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
            .LessThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("A data de nascimento não pode ser no futuro.");
    }
}

public static class UserValidationsExtensions
{
    public static IServiceCollection AddUserValidations(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateUserDto>, CreateUserDtoValidation>();
        services.AddScoped<IValidator<UpdateUserDto>, UpdateUserDtoValidation>();
        
        return services;
    }
}