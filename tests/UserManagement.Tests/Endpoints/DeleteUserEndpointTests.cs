using System.Net;
using System.Net.Http.Json;
using UserManagement.API.DTOs;
using UserManagement.Tests.Setup;

namespace UserManagement.Tests.Endpoints;

public class DeleteUserEndpointTests(UserManagementWebApplicationFactory factory)
    : IClassFixture<UserManagementWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    private async Task DeleteUser_Correctly()
    {
        var dto = new CreateUserDto("Usopp",
            "usopp@gmail.com",
            DateTime.Now.AddYears(-20));
        
        var createResponse = await _client.PostAsJsonAsync("/users", dto);
        createResponse.EnsureSuccessStatusCode();
        
        var createdUserResponse = await createResponse.Content.ReadFromJsonAsync<UserResponseDto>();
        
        Assert.NotNull(createdUserResponse);
        
        var user = await _client.GetFromJsonAsync<UserResponseDto>($"/users/{createdUserResponse.Id}");
        Assert.NotNull(user);
        Assert.Equal(createdUserResponse.Id, user.Id);
        Assert.Equal(createdUserResponse.Nome, user.Nome);
        Assert.Equal(createdUserResponse.Email, user.Email);
        Assert.Equal(createdUserResponse.DataNascimento.Date, user.DataNascimento.Date);
        
        var deleteResponse = await _client.DeleteAsync($"/users/{createdUserResponse.Id}");
        deleteResponse.EnsureSuccessStatusCode();
        
        var getDeletedUserResponse = await _client.GetAsync($"/users/{createdUserResponse.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getDeletedUserResponse.StatusCode);
    }
    
    [Fact]
    private async Task DeleteInvalid_Should_Return_NotFound()
    {
        var response = await _client.DeleteAsync("/users/999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}