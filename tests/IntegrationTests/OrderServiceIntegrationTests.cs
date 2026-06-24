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
using OrderService.Application.Commands.PlaceOrder;
using OrderService.Application.DTOs;
using OrderService.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace IntegrationTests;

public class OrderServiceIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("OrderDbTest")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private WebApplicationFactory<OrderProgram> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        _factory = new WebApplicationFactory<OrderProgram>().WithWebHostBuilder(builder =>
        {
            // روش صحیح و قطعی برای Aspire
            builder.UseSetting("ConnectionStrings:OrderDb", _dbContainer.GetConnectionString());

            builder.ConfigureServices(services =>
            {
                services.AddMassTransitTestHarness();
            });
        });

        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        _factory.Dispose();
        _client.Dispose();
    }

    [Fact]
    public async Task Post_PlaceOrder_ReturnsCreated_And_Get_ReturnsOrder()
    {
        var items = new List<OrderItemDto> { new(Guid.NewGuid(), 2, 50.0m) };
        var command = new PlaceOrderCommand(Guid.NewGuid(), items);

        var postResponse = await _client.PostAsJsonAsync("/api/orders", command);

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var responseNode = await postResponse.Content.ReadFromJsonAsync<JsonElement>();
        var orderId = responseNode.GetProperty("id").GetGuid();

        orderId.Should().NotBeEmpty();

        var getResponse = await _client.GetAsync($"/api/orders/{orderId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var order = await getResponse.Content.ReadFromJsonAsync<OrderDto>();
        order.Should().NotBeNull();
        order!.Status.Should().Be("Pending");
        order.TotalAmount.Should().Be(100.0m);
    }
}