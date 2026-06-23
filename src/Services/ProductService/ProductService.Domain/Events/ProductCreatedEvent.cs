using MediatR;

namespace ProductService.Domain.Events;

public record ProductCreatedEvent(Guid ProductId, string Name, string Category, decimal Price) : INotification;