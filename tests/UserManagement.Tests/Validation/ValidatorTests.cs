using FluentValidation.TestHelper;
using UserManagement.API.DTOs;
using UserManagement.API.Validators;

namespace UserManagement.Tests.Validation;

public class ValidatorTests
{
    private readonly CreateUserDtoValidation _createUserDtoValidation = new();
    private readonly UpdateUserDtoValidation _updateUserDtoValidation = new();

    [Fact]
    public void CreateUserDto_CorrectValidation()
    {
        var dto = new CreateUserDto(
            "Zoro",
            "zoro@gmail.com",
            DateTime.Now.AddYears(-30)
        );
        
        var result = _createUserDtoValidation.TestValidate(dto);
        
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("A very long name that exceeds the maximum length of fifty characters which is not valid")]
    public void CreateUserDto_InvalidName_Should_Have_Validation_Errors(string invalidName)
    {
        var dto = new CreateUserDto(
            invalidName,
            "email@gmail.com",
            DateTime.Now.AddYears(-30)
        );
        
        var result = _createUserDtoValidation.TestValidate(dto);
        
        result.ShouldHaveValidationErrorFor(x => x.Nome)
            .WithErrorMessage(invalidName == null || string.IsNullOrWhiteSpace(invalidName)
                ? "O nome é obrigatório."
                : "O nome deve conter no máximo 50 caracteres.");
        
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("email invalido sem arroba")]
    public void CreateUserDto_InvalidEmail_Should_Have_Validation_Errors(string invalidEmail)
    {
        var dto = new CreateUserDto(
            "Zoro",
            invalidEmail,
            DateTime.Now.AddYears(-30)
        );
        
        var result = _createUserDtoValidation.TestValidate(dto);
        
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage(invalidEmail == null || string.IsNullOrWhiteSpace(invalidEmail)
                ? "O email é obrigatório."
                : "O email deve ser um endereço de email válido.");
        
    }

    [Fact]
    public void CreateUserDto_InvalidEmail_Bigger_Than_254_Should_Have_Validation_Errors()
    {
        var dto = new CreateUserDto(
            "Zoro",
            new string('a', 255) + "@gmail.com",
            DateTime.Now.AddYears(-30)
        );
        
        var result = _createUserDtoValidation.TestValidate(dto);
        
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("O email deve conter no máximo 254 caracteres.");
        
    }

    [Fact]
    public void CreateUserDto_InvalidDate_Should_Have_Validation_Errors()
    {
        var dto = new CreateUserDto(
            "Zoro",
            "zoro@gmail.com",
            DateTime.Now.AddYears(30)
            );
        
        var result = _createUserDtoValidation.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.DataNascimento)
            .WithErrorMessage("A data de nascimento não pode ser no futuro.");
    }

    [Fact]
    public void UpdateUserDto_CorrectValidation()
    {
        var dto = new UpdateUserDto(
            "Zoro",
            "zoro@gmail.com",
            DateTime.Now.AddYears(-30)
        );
        
        var result = _updateUserDtoValidation.TestValidate(dto);
        
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        result.ShouldNotHaveAnyValidationErrors();
    }
}