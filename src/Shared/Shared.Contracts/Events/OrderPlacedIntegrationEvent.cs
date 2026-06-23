namespace Shared.Contracts.Events;

public record OrderItemDto(Guid ProductId, int Quantity, decimal UnitPrice);

public record OrderPlacedIntegrationEvent(
    Guid OrderId,
    Guid UserId,
    List<OrderItemDto> Items,
    decimal TotalAmount,
    DateTime OccurredOn
);