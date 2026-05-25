using System.Net.Http.Json;
using UserManagement.API.DTOs;
using UserManagement.Tests.Setup;

namespace UserManagement.Tests.Endpoints;

public class UpdateUserEndpointTests(UserManagementWebApplicationFactory factory)
    : IClassFixture<UserManagementWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    private async Task UpdateUser_Correctly()
    {
        // Arrange
        var createUserRequest = new CreateUserDto(
            "Luffy criado", "luffy@gmail.com", DateTime.Now.AddYears(-20));

        var createResponse = await _client.PostAsJsonAsync("/users", createUserRequest);
        createResponse.EnsureSuccessStatusCode();

        var createdUserResponse = await createResponse.Content.ReadFromJsonAsync<UserResponseDto>();
        
        Assert.NotNull(createdUserResponse);

        var updateUserRequest = new UpdateUserDto(
            "Luffy atualizado", 
            "Luffy@gmail.com",
            DateTime.Now.AddYears(-25));
        
        var updateResponse = await _client.PutAsJsonAsync($"/users/{createdUserResponse.Id}", updateUserRequest);
        
        updateResponse.EnsureSuccessStatusCode();
        
        var updatedUser = await updateResponse.Content.ReadFromJsonAsync<UserResponseDto>();
        
        Assert.NotNull(updatedUser);
        Assert.Equal(createdUserResponse.Id, updatedUser.Id);
        Assert.Equal(updateUserRequest.Nome, updatedUser.Nome);
        Assert.Equal(updateUserRequest.Email.ToLower(), updatedUser.Email.ToLower());
        Assert.Equal(updateUserRequest.DataNascimento.Date, updatedUser.DataNascimento.Date);
    }

    [Fact]
    private async Task UpdateUser_ShouldReturnNotFound_WhenUserIsNotUpdated()
    {
        var createUserRequest = new CreateUserDto(
            "Luffy criado", "luffyasd@gmail.com", DateTime.Now.AddYears(-20));

        var createResponse = await _client.PostAsJsonAsync("/users", createUserRequest);
        createResponse.EnsureSuccessStatusCode();

        var createdUserResponse = await createResponse.Content.ReadFromJsonAsync<UserResponseDto>();
        
        Assert.NotNull(createdUserResponse);

        var updateUserRequest = new UpdateUserDto(
            "Luffy atualizado", 
            "Luffy@gmail.com",
            DateTime.Now.AddYears(-25));
        
        var updateResponse = await _client.PutAsJsonAsync($"/users/999", updateUserRequest);
        
        Assert.Equal(System.Net.HttpStatusCode.NotFound, updateResponse.StatusCode);
    }
    
    [Fact]
    private async Task UpdateUser_DuplicateEmail_ShouldReturnConflict()
    {
        // Arrange
        var createUserRequest = new CreateUserDto(
            "Luffy criado 1", "luffyd@gmail.com", DateTime.Now.AddYears(-20));
        var createUserRequest2 = new CreateUserDto(
            "Luffy criado 2", "luffy2@gmail.com", DateTime.Now.AddYears(-20));

        var createResponse = await _client.PostAsJsonAsync("/users", createUserRequest);
        createResponse.EnsureSuccessStatusCode();
        var createResponse2 = await _client.PostAsJsonAsync("/users", createUserRequest2);
        createResponse.EnsureSuccessStatusCode();

        var createdUserResponse = await createResponse.Content.ReadFromJsonAsync<UserResponseDto>();
        
        Assert.NotNull(createdUserResponse);

        var updateUserRequest = new UpdateUserDto(
            "Luffy atualizado", 
            "Luffy2@gmail.com",
            DateTime.Now.AddYears(-25));
        
        var updateResponse = await _client.PutAsJsonAsync($"/users/{createdUserResponse.Id}", updateUserRequest);
        
        Assert.Equal(System.Net.HttpStatusCode.Conflict, updateResponse.StatusCode);
    }
    
    [Fact]
    private async Task UpdateUser_Invalid_SouldReturnBadRequest()
    {
        // Arrange
        var createUserRequest = new CreateUserDto(
            "Luffy sd", "luffygg@gmail.com", DateTime.Now.AddYears(-20));

        var createResponse = await _client.PostAsJsonAsync("/users", createUserRequest);
        createResponse.EnsureSuccessStatusCode();

        var createdUserResponse = await createResponse.Content.ReadFromJsonAsync<UserResponseDto>();
        
        Assert.NotNull(createdUserResponse);

        var updateUserRequest = new UpdateUserDto(
            "Luffy atualizado", 
            "Luffy@gmail.com",
            DateTime.Now.AddYears(25));
        
        var updateResponse = await _client.PutAsJsonAsync($"/users/{createdUserResponse.Id}", updateUserRequest);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }
}