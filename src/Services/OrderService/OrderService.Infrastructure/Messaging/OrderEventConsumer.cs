using MassTransit;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Persistence;
using Shared.Contracts.Events;

namespace OrderService.Infrastructure.Messaging;

// Anti-Corruption Layer: Consumes external event and stores a read model
public class OrderEventConsumer : IConsumer<ProductCreatedIntegrationEvent>
{
    private readonly OrderDbContext _context;

    public OrderEventConsumer(OrderDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<ProductCreatedIntegrationEvent> context)
    {
        var message = context.Message;
        
        var product = new ProductReadModel(message.ProductId, message.Name, message.Price);
        
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }
}