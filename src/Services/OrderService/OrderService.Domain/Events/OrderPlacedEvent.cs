using MediatR;

namespace OrderService.Domain.Events;

public record OrderPlacedEvent(Guid OrderId, Guid UserId, decimal TotalAmount) : INotification;