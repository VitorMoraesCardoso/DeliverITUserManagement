using System.Net;
using System.Net.Http.Json;
using UserManagement.API.DTOs;
using UserManagement.Tests.Setup;

namespace UserManagement.Tests.Endpoints;

public class GetUserEndPointTests(UserManagementWebApplicationFactory factory)
    : IClassFixture<UserManagementWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    private async Task GetUserById_Correctly()
    {
        var dto = new CreateUserDto("Franky",
            "franky@gmail.com",
            DateTime.Now.AddYears(-40));
        
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
    }
    
    [Fact]
    private async Task GetUserByIdInvalid_Should_Return_NotFound()
    {
        var response = await _client.GetAsync("/users/999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    private async Task GetUsersPaged_Correctly()
    {
        var dto = new CreateUserDto("Robin",
            "robin@gmail.com",
            DateTime.Now.AddYears(-40));
        
        var dto2 = new CreateUserDto("Brook",
            "brook@gmail.com",
            DateTime.Now.AddYears(-40));
        
        var createResponse = await _client.PostAsJsonAsync("/users", dto);
        createResponse.EnsureSuccessStatusCode();
        
        var createResponse2 = await _client.PostAsJsonAsync("/users", dto2);
        createResponse2.EnsureSuccessStatusCode();
        
        var createdUserResponse = await createResponse.Content.ReadFromJsonAsync<UserResponseDto>();
        var createdUserResponse2 = await createResponse2.Content.ReadFromJsonAsync<UserResponseDto>();
        
        Assert.NotNull(createdUserResponse);
        Assert.NotNull(createdUserResponse2);
        
        var responseUsersPaged = await _client.GetFromJsonAsync<PagedResponseDto<UserResponseDto>>("/users");
        Assert.NotNull(responseUsersPaged);
        Assert.Contains(createdUserResponse.Id, responseUsersPaged.Items.Select(u => u.Id));
        Assert.Contains(createdUserResponse.Nome, responseUsersPaged.Items.Select(u => u.Nome));
        Assert.Contains(createdUserResponse.Email.ToLower(), responseUsersPaged.Items.Select(u => u.Email.ToLower()));
        
        Assert.Contains(createdUserResponse2.Id, responseUsersPaged.Items.Select(u => u.Id));
        Assert.Contains(createdUserResponse2.Nome, responseUsersPaged.Items.Select(u => u.Nome));
        Assert.Contains(createdUserResponse2.Email.ToLower(), responseUsersPaged.Items.Select(u => u.Email.ToLower()));
        
    }
}