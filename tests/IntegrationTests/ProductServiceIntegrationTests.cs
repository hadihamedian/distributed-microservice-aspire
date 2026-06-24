using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProductService.Application.Commands.CreateProduct;
using ProductService.Application.DTOs;
using ProductService.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace IntegrationTests;

public class ProductServiceIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("ProductDbTest")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private WebApplicationFactory<ProductProgram> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        _factory = new WebApplicationFactory<ProductProgram>().WithWebHostBuilder(builder =>
        {
            // روش صحیح و قطعی برای Aspire
            builder.UseSetting("ConnectionStrings:ProductDb", _dbContainer.GetConnectionString());

            builder.ConfigureServices(services =>
            {
                services.AddMassTransitTestHarness();
            });
        });

        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        _factory.Dispose();
        _client.Dispose();
    }

    [Fact]
    public async Task Post_CreateProduct_ReturnsCreated_And_Get_ReturnsProduct()
    {
        var command = new CreateProductCommand("Laptop", "Gaming", 1500, 10, "Electronics");
        var postResponse = await _client.PostAsJsonAsync("/api/products", command);

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var responseNode = await postResponse.Content.ReadFromJsonAsync<JsonElement>();
        var productId = responseNode.GetProperty("id").GetGuid();

        productId.Should().NotBeEmpty();

        var getResponse = await _client.GetAsync($"/api/products/{productId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var product = await getResponse.Content.ReadFromJsonAsync<ProductDto>();
        product.Should().NotBeNull();
        product!.Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task Get_GetAllProducts_ReturnsList()
    {
        var response = await _client.GetAsync("/api/products");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        products.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_ProductById_ReturnsNotFound_ForInvalidId()
    {
        var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}