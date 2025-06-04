using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HabitFlow.Application.Features.Categories.Commands.CreateCategory.Dtos;
using HabitFlow.Application.Features.Categories.Commands.UpdateCategory.Dtos;
using HabitFlow.Application.Features.Users.Commands.LoginUser.Dtos;
using HabitFlow.Application.Features.Users.Commands.RegisterUser.Dtos;
using HabitFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace HabitFlow.IntegrationTests.Controllers;

public class CategoriesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private string _authToken;

    public CategoriesControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var registerDto = new RegisterUserDto
        {
            Name = "Test User",
            Email = "test@categories.com",
            Password = "Test@123"
        };

        await _client.PostAsJsonAsync("/auth/register", registerDto);

        var loginDto = new LoginUserDto
        {
            Email = "test@categories.com",
            Password = "Test@123"
        };

        var loginResponse = await _client.PostAsJsonAsync("/auth/login", loginDto);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoggedInUserDto>();

        _authToken = loginResult.Token;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
    }

    [Fact]
    public async Task CRUD_Flow_Should_Work_Correctly()
    {
        // Arrange
        await AuthenticateAsync();

        // Create
        var createDto = new CreateCategoryDto
        {
            Name = "Health",
            Description = "Health related habits",
            Color = "#4CAF50"
        };

        var createResponse = await _client.PostAsJsonAsync("/categories", createDto);
        createResponse.EnsureSuccessStatusCode();
        var createdCategory = await createResponse.Content.ReadFromJsonAsync<CreatedCategoryDto>();

        Assert.NotNull(createdCategory);
        Assert.Equal(createDto.Name, createdCategory.Name);
        Assert.Equal(createDto.Description, createdCategory.Description);
        Assert.Equal(createDto.Color, createdCategory.Color);

        // Get All
        var getAllResponse = await _client.GetAsync("/categories");
        getAllResponse.EnsureSuccessStatusCode();
        var categories = await getAllResponse.Content.ReadFromJsonAsync<CreatedCategoryDto[]>();

        Assert.NotNull(categories);
        Assert.Single(categories);
        Assert.Equal(createdCategory.Id, categories.First().Id);

        // Get By Id
        var getByIdResponse = await _client.GetAsync($"/categories/{createdCategory.Id}");
        getByIdResponse.EnsureSuccessStatusCode();
        var category = await getByIdResponse.Content.ReadFromJsonAsync<CreatedCategoryDto>();

        Assert.NotNull(category);
        Assert.Equal(createdCategory.Id, category.Id);

        // Update
        var updateDto = new UpdateCategoryDto
        {
            Name = "Health & Wellness",
            Description = "All about health and wellness",
            Color = "#2E7D32"
        };

        var updateResponse = await _client.PutAsJsonAsync($"/categories/{createdCategory.Id}", updateDto);
        updateResponse.EnsureSuccessStatusCode();
        var updatedCategory = await updateResponse.Content.ReadFromJsonAsync<CreatedCategoryDto>();

        Assert.NotNull(updatedCategory);
        Assert.Equal(updateDto.Name, updatedCategory.Name);
        Assert.Equal(updateDto.Description, updatedCategory.Description);
        Assert.Equal(updateDto.Color, updatedCategory.Color);

        // Delete
        var deleteResponse = await _client.DeleteAsync($"/categories/{createdCategory.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify deleted
        var getAfterDeleteResponse = await _client.GetAsync($"/categories/{createdCategory.Id}");
        Assert.Equal(HttpStatusCode.BadRequest, getAfterDeleteResponse.StatusCode);
    }
}
