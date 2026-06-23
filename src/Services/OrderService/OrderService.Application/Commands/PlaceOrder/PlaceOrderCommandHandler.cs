using MassTransit;
using MediatR;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;
using Shared.Contracts.Events;

namespace OrderService.Application.Commands.PlaceOrder;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Guid>
{
    private readonly IOrderRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public PlaceOrderCommandHandler(IOrderRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var items = request.Items.Select(i => (i.ProductId, i.Quantity, i.UnitPrice)).ToList();
        var order = Order.Create(request.UserId, items);

        await _repository.AddAsync(order, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        var integrationEvent = new OrderPlacedIntegrationEvent(
            order.Id,
            order.UserId,
            request.Items.Select(i => new Shared.Contracts.Events.OrderItemDto(i.ProductId, i.Quantity, i.UnitPrice)).ToList(),
            order.TotalAmount,
            DateTime.UtcNow
        );

        await _publishEndpoint.Publish(integrationEvent, cancellationToken);

        return order.Id;
    }
}