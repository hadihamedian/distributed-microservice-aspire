namespace Shared.Contracts.Events;

public record ProductCreatedIntegrationEvent(
    Guid ProductId,
    string Name,
    string Category,
    decimal Price,
    DateTime OccurredOn
);