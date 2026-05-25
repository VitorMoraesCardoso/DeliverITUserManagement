using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.API.DTOs;
using UserManagement.Infra.Context;
using UserManagement.Tests.Setup;

namespace UserManagement.Tests.Endpoints;

public class CreateUserEndpointTests(UserManagementWebApplicationFactory factory)
    : IClassFixture<UserManagementWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateUser_Correctly()
    {
        var dto = new CreateUserDto(
            "Nami",
            "nami@gmail.com",
            DateTime.Now.AddYears(-20)
        );
        
        var response = await _client.PostAsJsonAsync("users", dto);
        
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var responseString = await response.Content.ReadFromJsonAsync<UserResponseDto>();
        
        Assert.NotNull(responseString);
        
        Assert.True(responseString.Id>0);
        Assert.Equal(responseString.Nome, dto.Nome);
        Assert.Equal(responseString.Email.ToLower(), dto.Email.ToLower());
        Assert.Equal(responseString.DataNascimento.Date, dto.DataNascimento.Date);

        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"users/{responseString.Id}",  response.Headers.Location.OriginalString);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userInDb =  await db.Users.FindAsync(responseString.Id);
        
        Assert.NotNull(userInDb);
        Assert.Equal(userInDb.Nome, dto.Nome);
    }

    [Fact]
    public async Task CreateUser_Duplicated_Email_Should_Return_Conflict()
    {
        var emailDuplicado = "email@gamil.com";
        
        var dto1 = new CreateUserDto("Luffy",  emailDuplicado,  DateTime.Now.AddYears(-20));
        var dto2 = new CreateUserDto("Zoro",  emailDuplicado,  DateTime.Now.AddYears(-20));
        
        await _client.PostAsJsonAsync("users", dto1);
        
        var responseDuplicated = await _client.PostAsJsonAsync("users", dto2);
        
        Assert.Equal(HttpStatusCode.Conflict, responseDuplicated.StatusCode);
    }

    [Fact]
    public async Task CreateUser_Invalid_Should_Return_BadRequest()
    {
        var invalidDto = new CreateUserDto("", "", DateTime.Now.AddYears(-20));
        
        var response = await _client.PostAsJsonAsync("users", invalidDto);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_InvalidDate_Should_Be_Caught_By_Validation_And_Return_BadRequest()
    {
        var invalidDto = new CreateUserDto("Luffy", "luffy@gmail.com", DateTime.Now.AddYears(+20));
        
        var response = await _client.PostAsJsonAsync("users", invalidDto);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}