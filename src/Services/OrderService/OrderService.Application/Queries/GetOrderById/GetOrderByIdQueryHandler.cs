using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Repositories;

namespace OrderService.Application.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderRepository _repository;

    public GetOrderByIdQueryHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (order is null) return null;

        var items = order.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity, i.UnitPrice)).ToList();
        return new OrderDto(order.Id, order.UserId, order.Status.ToString(), order.TotalAmount, items);
    }
}